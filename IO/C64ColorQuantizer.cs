using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using C64.Graphics;

namespace C64.IO
{
    public static class C64ColorQuantizer
    {
        // פלטת ה-Pepto המקובלת ל-C64
     /*   private static readonly Color[] C64Palette = new Color[]
        {
            Color.FromArgb(0, 0, 0),       // 0: Black
            Color.FromArgb(255, 255, 255), // 1: White
            Color.FromArgb(136, 0, 0),     // 2: Red
            Color.FromArgb(170, 255, 238), // 3: Cyan
            Color.FromArgb(204, 68, 204),  // 4: Purple
            Color.FromArgb(0, 204, 85),    // 5: Green
            Color.FromArgb(0, 0, 170),     // 6: Blue
            Color.FromArgb(238, 238, 119), // 7: Yellow
            Color.FromArgb(221, 136, 85),  // 8: Orange
            Color.FromArgb(102, 68, 0),    // 9: Brown
            Color.FromArgb(255, 119, 119), // 10: Light Red
            Color.FromArgb(51, 51, 51),    // 11: Dark Grey
            Color.FromArgb(119, 119, 119), // 12: Grey
            Color.FromArgb(170, 255, 102), // 13: Light Green
            Color.FromArgb(0, 136, 255),   // 14: Light Blue
            Color.FromArgb(187, 187, 187)  // 15: Light Grey
        };*/

        /// <summary>
        /// מקבלת תמונה ומחזירה עותק חדש שלה מומרת לפלטת C64
        /// </summary>
        public static Bitmap Quantize(Bitmap original)
        {
            // המרה לפורמט אחיד כדי למנוע בעיות של פיקסלים בגדלים שונים
            Bitmap quantizedBitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(quantizedBitmap))
            {
                g.DrawImage(original, 0, 0, original.Width, original.Height);
            }

            // נעילת הזיכרון לגישה מהירה
            BitmapData bmpData = quantizedBitmap.LockBits(
                new Rectangle(0, 0, quantizedBitmap.Width, quantizedBitmap.Height),
                ImageLockMode.ReadWrite,
                quantizedBitmap.PixelFormat);

            int bytes = Math.Abs(bmpData.Stride) * quantizedBitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // העתקת הנתונים למערך
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            // מעבר על כל פיקסל (4 בייטים לכל פיקסל: BGRA)
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                byte b = rgbValues[i];
                byte g = rgbValues[i + 1];
                byte r = rgbValues[i + 2];
                // byte a = rgbValues[i + 3]; // ניתן להשתמש באלפא אם רוצים להתעלם מפיקסלים שקופים

                Color closestColor = GetNearestC64Color(r, g, b);

                // עדכון המערך לצבע החדש
                rgbValues[i] = closestColor.B;
                rgbValues[i + 1] = closestColor.G;
                rgbValues[i + 2] = closestColor.R;
                rgbValues[i + 3] = 255; // הסרת שקיפות, ל-C64 אין Alpha Channel
            }

            // החזרת הנתונים לביטמאפ ושחרור הנעילה
            Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            quantizedBitmap.UnlockBits(bmpData);

            return quantizedBitmap;
        }

        /// <summary>
        /// מוצאת את הצבע הקרוב ביותר מתוך פלטת הקומודור
        /// </summary>
        private static Color GetNearestC64Color(byte r, byte g, byte b)
        {
            Color nearestColor = C64Palette.ActivePalette[0];

            int minDistance = int.MaxValue;

            foreach (Color c64Color in C64Palette.ActivePalette)
            {
                // חישוב המרחק האוקלידי ללא הוצאת שורש (לצורך ביצועים)
                int deltaR = r - c64Color.R;
                int deltaG = g - c64Color.G;
                int deltaB = b - c64Color.B;

                int distance = (deltaR * deltaR) + (deltaG * deltaG) + (deltaB * deltaB);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestColor = c64Color;
                }
            }

            return nearestColor;
        }
    }
}
