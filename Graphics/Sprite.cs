using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace C64.Graphics
{
    public class Sprite : Common.BaseSprite
    {
        public const int TotalBytes = 64;
        private const int C64_SPRITE_SIZE_BYTES = 64;

        public override int Width => IsMulticolor ? 12 : 24;
        public override int Height => 21;

        // מערך חד-ממדי קשיח של 64 בתים - בדיוק כמו בחומרה של ה-C64
        public byte[] RawData { get; set; } = new byte[TotalBytes];

        // אינדקסר חכם שמפרק את הביטים של ה-64 בתים ל-X ו-Y פיקסלים עבור הפקד!
        private bool _isMulticolor = false;
        public bool IsMulticolor
        {
            get => _isMulticolor;
            set => _isMulticolor = value;
        }

        // האינדקסר משתנה מ-bool ל-int כדי להחזיר ערך צבע (0-3)
        public override int this[int x, int y]
        {
            get
            {
                if (y < 0 || y >= Height) return 0;
                int maxWidth = _isMulticolor ? 12 : Width; // 12 פיקסלים שמנים במולטיקולור
                if (x < 0 || x >= maxWidth) return 0;

                if (_isMulticolor)
                {
                    // --- חישוב עבור מצב MultiColor (זוג ביטים לפיקסל) ---
                    int byteIndex = (y * 3) + (x / 4); // כל בית מכיל 4 פיקסלים (8 ביטים / 2)
                    int pixelOffset = 3 - (x % 4);     // המיקום בתוך הבית (0, 1, 2, 3 משמאל לימין)
                    int bitShift = pixelOffset * 2;    // כמה ביטים להזיז (6, 4, 2, או 0)

                    // שליפת זוג הביטים באמצעות מסיכה בינארית (Mask) של 3 (שהיא %00000011)
                    return (RawData[byteIndex] >> bitShift) & 0x03;
                }
                else
                {
                    // --- חישוב עבור מצב Hi-Res סטנדרטי (ביט בודד לפיקסל) ---
                    int byteIndex = (y * 3) + (x / 8);
                    int bitIndex = 7 - (x % 8);
                    bool isBitOn = (RawData[byteIndex] & (1 << bitIndex)) != 0;
                    return isBitOn ? 1 : 0; // מחזיר 1 (צבע ספרייט) או 0 (שקוף)
                }
            }
            set
            {
                if (y < 0 || y >= Height) return;
                int maxWidth = _isMulticolor ? 12 : Width;
                if (x < 0 || x >= maxWidth) return;

                // וידוא שהערך הנכנס חוקי (בין 0 ל-3)
                int colorIndex = value & 0x03;

                if (_isMulticolor)
                {
                    // --- כתיבה עבור מצב MultiColor ---
                    int byteIndex = (y * 3) + (x / 4);
                    int pixelOffset = 3 - (x % 4);
                    int bitShift = pixelOffset * 2;

                    // 1. איפוס זוג הביטים הספציפי בתוך הבית מבלי לפגוע בשאר הפיקסלים
                    RawData[byteIndex] &= (byte)~(0x03 << bitShift);
                    // 2. הזרקת זוג הביטים החדש של הצבע
                    RawData[byteIndex] |= (byte)(colorIndex << bitShift);
                }
                else
                {
                    // --- כתיבה עבור מצב Hi-Res ---
                    int byteIndex = (y * 3) + (x / 8);
                    int bitIndex = 7 - (x % 8);

                    if (value > 0)
                    {
                        RawData[byteIndex] |= (byte)(1 << bitIndex);  // הדלקת ביט בודד
                    }
                    else
                    {
                        RawData[byteIndex] &= (byte)~(1 << bitIndex); // כיבוי ביט בודד
                    }
                }
            }
        }

        // פונקציות שמירה וטעינה מהירות בליין אחד בלי שום המרות
        public void SaveAsBinary(string filePath)
        {
            File.WriteAllBytes(filePath, RawData);
        }

        public void LoadFromBinary(string filePath)
        {
            if (!File.Exists(filePath)) return;

            byte[] loadedData = File.ReadAllBytes(filePath);

            if (loadedData.Length >= TotalBytes)
            {
                Array.Copy(loadedData, RawData, TotalBytes);
            }
        }

        public void SaveAsPRG(string filePath, ushort address)
        {
            Stream stream = new FileStream(filePath, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(address);

            for (int i = 0; i < TotalBytes; i++)
            {
                binaryWriter.Write(RawData[i]);
            }

            binaryWriter.Flush();
            binaryWriter.Close();
        }

        public void LoadFromPRG(string filePath)
        {
            FileStream fstream = new FileStream(filePath, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fstream);

            ushort address = binaryReader.ReadUInt16();
            RawData = binaryReader.ReadBytes(C64_SPRITE_SIZE_BYTES);
            binaryReader.Close();
            fstream.Close();
        }
        /// <summary>
        /// מייבאת גרפיקה מתוך אובייקט Bitmap של Windows וממירה אותה לאינדקסי צבע (0-3) בספרייט
        /// </summary>
        public void ImportFromBitmap(Bitmap sourceBmp, bool isMulticolorMode)
        {
            // 1. מנקים את מערך הברזל של ה-64 בתים לחלוטין
            Array.Clear(RawData, 0, TotalBytes);

            // 2. קביעת הגבול הלוגי של הציור
            int logicalWidth = isMulticolorMode ? 12 : Width; // 12 פיקסלים שמנים או 24 רגילים
            int scanHeight = Math.Min(sourceBmp.Height, Height);

            // 3. סריקה של התמונה פיקסל אחר פיקסל
            for (int y = 0; y < scanHeight; y++)
            {
                for (int x = 0; x < logicalWidth; x++)
                {
                    // מציאת המיקום האמיתי של הפיקסל בתמונת המקור (אם זה מולטיקולור, נדגום בקפיצות של 2)
                    int sourceX = isMulticolorMode ? (x * 2) : x;

                    if (sourceX >= sourceBmp.Width) continue;

                    Color pixelColor = sourceBmp.GetPixel(sourceX, y);

                    // בדיקה ראשונה: אם הפיקסל שקוף לחלוטין, הוא תמיד יקבל צבע 0 (שקוף/רקע)
                    if (pixelColor.A < 128)
                    {
                        this[x, y] = 0;
                        continue;
                    }

                    // חישוב בהירות (Brightness) של הפיקסל המודרני (ערך בין 0 ל-255)
                    double brightness = (0.299 * pixelColor.R) + (0.587 * pixelColor.G) + (0.114 * pixelColor.B);

                    if (isMulticolorMode)
                    {
                        // --- לוגיקת המרה עבור מצב MultiColor (מצפה לערכים 0, 1, 2, 3) ---
                        // פה נעשה מיפוי פשוט לפי בהירות (סוג של Quantization מהיר ל-4 רמות עומק):
                        if (brightness < 64) this[x, y] = 0; // כהה מאוד -> צבע רקע
                        else if (brightness < 128) this[x, y] = 1; // כהה בינוני -> מולטיקולור 1
                        else if (brightness < 192) this[x, y] = 2; // בהיר בינוני -> צבע ספרייט ייחודי
                        else this[x, y] = 3; // בהיר מאוד/לבן -> מולטיקולור 2
                    }
                    else
                    {
                        // --- לוגיקת המרה עבור מצב Hi-Res סטנדרטי (מצפה ל-0 או 1) ---
                        if (brightness > 128)
                        {
                            this[x, y] = 1; // בהיר -> צבע ספרייט
                        }
                        else
                        {
                            this[x, y] = 0; // כהה -> שקוף/רקע
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            Array.Clear(RawData, 0, TotalBytes);
        }
    }
}

