using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace C64.Graphics
{
    public static class C64SpriteColorQuantizer
    {
        /// <summary>
        /// פונקציה חכמה: מוצאת אוטומטית את 4 הצבעים השכיחים ביותר בתמונה (מתוך C64Palette.ActivePalette) 
        /// וממירה את התמונה לשימוש ב-4 הצבעים הללו בלבד.
        /// </summary>
        /// <param name="source">תמונת המקור</param>
        /// <param name="colorSlots">מערך בגודל 4 המכיל את אינדקסי הצבעים שנבחרו עבור 4 הסלוטים (00, 01, 10, 11)</param>
        /// <returns>Bitmap מומר מופחת צבעים</returns>
        public static Bitmap AutoQuantizeTo4Colors(Bitmap source, out byte[] colorSlots)
        {
            // 1. חילוץ 4 הצבעים (האינדקסים) השכיחים ביותר מתוך התמונה
            colorSlots = ExtractTop4C64Colors(source);

            // 2. שליפת ערכי ה-Color הממשיים מתוך C64Palette.ActivePalette
            Color[] target4Colors = new Color[4]
            {
                C64Palette.ActivePalette[colorSlots[0]], // Slot 00 (לרוב רקע)
                C64Palette.ActivePalette[colorSlots[1]], // Slot 01 (MC1)
                C64Palette.ActivePalette[colorSlots[2]], // Slot 10 (Individual)
                C64Palette.ActivePalette[colorSlots[3]]  // Slot 11 (MC2)
            };

            // 3. המרת התמונה ל-4 הצבעים שנבחרו
            return QuantizeToColorPalette(source, target4Colors);
        }

        /// <summary>
        /// המרת תמונה ל-Bitmap המוגבל ל-4 אינדקסי צבע מוגדרים מראש.
        /// </summary>
        public static Bitmap QuantizeTo4Colors(
            Bitmap source,
            byte bgIndex,
            byte mc1Index,
            byte spriteColorIndex,
            byte mc2Index)
        {
            Color[] target4Colors = new Color[4]
            {
                C64Palette.ActivePalette[bgIndex],
                C64Palette.ActivePalette[mc1Index],
                C64Palette.ActivePalette[spriteColorIndex],
                C64Palette.ActivePalette[mc2Index]
            };

            return QuantizeToColorPalette(source, target4Colors);
        }

        /// <summary>
        /// פונקציית הליבה: ממירה תמונה לביטמאפ המכיל רק צבעים מתוך מערך 4 הצבעים הנתון.
        /// </summary>
        public static Bitmap QuantizeToColorPalette(Bitmap source, Color[] target4Colors)
        {
            if (target4Colors == null || target4Colors.Length != 4)
            {
                throw new ArgumentException("הפונקציה דורשת מערך של בדיוק 4 צבעים.", nameof(target4Colors));
            }

            Bitmap outputBmp = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(outputBmp))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            BitmapData bmpData = outputBmp.LockBits(
                new Rectangle(0, 0, outputBmp.Width, outputBmp.Height),
                ImageLockMode.ReadWrite,
                outputBmp.PixelFormat);

            int byteCount = Math.Abs(bmpData.Stride) * outputBmp.Height;
            byte[] pixelBuffer = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, pixelBuffer, 0, byteCount);

            for (int i = 0; i < pixelBuffer.Length; i += 4)
            {
                byte b = pixelBuffer[i];
                byte g = pixelBuffer[i + 1];
                byte r = pixelBuffer[i + 2];
                byte a = pixelBuffer[i + 3];

                Color chosenColor;
                if (a < 128)
                {
                    chosenColor = target4Colors[0]; // שקוף -> ממופה לסלוט הרקע 00
                }
                else
                {
                    chosenColor = GetClosestColor(r, g, b, target4Colors);
                }

                pixelBuffer[i] = chosenColor.B;
                pixelBuffer[i + 1] = chosenColor.G;
                pixelBuffer[i + 2] = chosenColor.R;
                pixelBuffer[i + 3] = 255;
            }

            Marshal.Copy(pixelBuffer, 0, bmpData.Scan0, byteCount);
            outputBmp.UnlockBits(bmpData);

            return outputBmp;
        }

        /// <summary>
        /// סורקת את התמונה, ממפה לפי C64Palette.ActivePalette, ומוצאת את 4 האינדקסים הנפוצים ביותר.
        /// </summary>
        private static byte[] ExtractTop4C64Colors(Bitmap source)
        {
            int paletteSize = C64Palette.ActivePalette.Length;
            int[] colorCounts = new int[paletteSize];

            BitmapData bmpData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int byteCount = Math.Abs(bmpData.Stride) * source.Height;
            byte[] pixelBuffer = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, pixelBuffer, 0, byteCount);
            source.UnlockBits(bmpData);

            for (int i = 0; i < pixelBuffer.Length; i += 4)
            {
                byte b = pixelBuffer[i];
                byte g = pixelBuffer[i + 1];
                byte r = pixelBuffer[i + 2];
                byte a = pixelBuffer[i + 3];

                if (a < 128) continue; // התעלמות מפיקסלים שקופים

                byte closestIndex = GetClosestC64PaletteIndex(r, g, b);
                colorCounts[closestIndex]++;
            }

            var top4 = colorCounts
                .Select((count, index) => new { Index = (byte)index, Count = count })
                .OrderByDescending(x => x.Count)
                .Take(4)
                .Select(x => x.Index)
                .ToList();

            // השלמה ל-4 צבעים במקרה שיש בתמונה פחות מ-4 צבעים
            byte fallbackIndex = 0;
            while (top4.Count < 4 && fallbackIndex < paletteSize)
            {
                if (!top4.Contains(fallbackIndex))
                {
                    top4.Add(fallbackIndex);
                }
                fallbackIndex++;
            }

            return top4.ToArray();
        }

        private static byte GetClosestC64PaletteIndex(byte r, byte g, byte b)
        {
            byte bestMatchIndex = 0;
            int minDistance = int.MaxValue;

            for (byte i = 0; i < C64Palette.ActivePalette.Length; i++)
            {
                Color candidate = C64Palette.ActivePalette[i];
                int dR = r - candidate.R;
                int dG = g - candidate.G;
                int dB = b - candidate.B;

                int distanceSquare = (dR * dR) + (dG * dG) + (dB * dB);

                if (distanceSquare < minDistance)
                {
                    minDistance = distanceSquare;
                    bestMatchIndex = i;
                }
            }

            return bestMatchIndex;
        }

        private static Color GetClosestColor(byte r, byte g, byte b, Color[] allowedColors)
        {
            Color bestMatch = allowedColors[0];
            int minDistance = int.MaxValue;

            for (int i = 0; i < allowedColors.Length; i++)
            {
                Color candidate = allowedColors[i];
                int dR = r - candidate.R;
                int dG = g - candidate.G;
                int dB = b - candidate.B;

                int distanceSquare = (dR * dR) + (dG * dG) + (dB * dB);

                if (distanceSquare < minDistance)
                {
                    minDistance = distanceSquare;
                    bestMatch = candidate;
                }
            }
            return bestMatch;
        }
    }
}
