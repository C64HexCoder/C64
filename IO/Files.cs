using C64.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace C64.IO
{
    public static class Files
    {
        // ==========================================
        // 1. פורמט גולמי / בינארי (Raw Binary)
        // ==========================================
        public static void SaveSpriteAsBinary(string filePath, byte[] spriteData)
        {
            File.WriteAllBytes(filePath, spriteData);
        }

        public static byte[] LoadSpriteFromBinary(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        // ==========================================
        // 2. פורמט קומודור סטנדרטי (PRG)
        // כולל 2 בתים ראשונים של כתובת טעינה בזיכרון
        // ==========================================
        public static void SaveSpriteAsPRG(string filePath, byte[] spriteData, ushort loadAddress)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // כתיבת כתובת הטעינה (למשל $0c00) - קודם הבית הנמוך ואז הגבוה (Little Endian)
                writer.Write((byte)(loadAddress & 0xFF));
                writer.Write((byte)((loadAddress >> 8) & 0xFF));

                // כתיבת נתוני הספרייט
                writer.Write(spriteData);
            }
        }

        public static byte[] LoadSpriteFromPRG(string filePath, out ushort loadAddress)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                // קריאת 2 הבתים הראשונים שמייצגים את הכתובת
                byte low = reader.ReadByte();
                byte high = reader.ReadByte();
                loadAddress = (ushort)(low | (high << 8));

                // קריאת שאר הנתונים הבינאריים של הספרייט
                int remainingLength = (int)(reader.BaseStream.Length - 2);
                return reader.ReadBytes(remainingLength);
            }
        }

        // גודל של ספרייט בודד של קומודור 64 בבתים (63 בתים של דאטה + 1 בית זבל/הרחבה)
        private const int C64_SPRITE_SIZE_BYTES = 64;

        public static List<Sprite> LoadSpritesFromPRG(string filePath,out ushort loadAddress)
        {
            List<Sprite> spritesList = new List<Sprite>();

            // קריאת כל הבתים של קובץ ה-PRG
            byte[] fileBytes = File.ReadAllBytes(filePath);

            if (fileBytes.Length < 2)
                throw new Exception("קובץ ה-PRG קצר מדי או פגום.");

            // 💡 בקומודור 64 (Little Endian), שני הבתים הראשונים הם כתובת הטעינה בזיכרון (Load Address)
            loadAddress = (ushort)(fileBytes[0] | (fileBytes[1] << 8));

            // הנתונים הגולמיים של הגרפיקה מתחילים מהבית השלישי (אינדקס 2)
            int dataLength = fileBytes.Length - 2;

            // חישוב כמה ספרייטים מלאים קיימים בקובץ הזה
            int numberOfSprites = dataLength / C64_SPRITE_SIZE_BYTES;

            if (numberOfSprites == 0)
                throw new Exception("הקובץ אינו מכיל נתוני ספרייט מלאים.");

            // לולאה שרצה ומפרקת את המערך הגדול לספרייטים בודדים
            for (int i = 0; i < numberOfSprites; i++)
            {
                // יצירת מופע חדש ונקי של ספרייט בודד
                Sprite singleSprite = new Sprite();

                // חיתוך ה-64 בתים הרלוונטיים עבור הספרייט הנוכחי מהמערך הגדול
                byte[] spriteData = new byte[C64_SPRITE_SIZE_BYTES];
                int sourceOffset = 2 + (i * C64_SPRITE_SIZE_BYTES);
                Array.Copy(fileBytes, sourceOffset, spriteData, 0, C64_SPRITE_SIZE_BYTES);

                // הזרקת הבתים לתוך אובייקט הספרייט (נניח דרך פונקציית עזר או מתודה פנימית שיצרת)
                singleSprite.RawData = spriteData;
                

                // הוספה לרשימה המשותפת
                spritesList.Add(singleSprite);
            }

            return spritesList;
        }

        
        public static void SaveSpritesAsPRG (string filePath,List<Sprite> spritesList,UInt16 address = 0x3000)
        {
            FileStream fileStream = new FileStream(filePath,FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fileStream);

            writer.Write(address);

            foreach (Sprite sprite in spritesList)
            {
                writer.Write (sprite.RawData);             
            }
        }


        // ==========================================
        // 3. פורמט קואלה (Koala Painter Format)
        // פורמט מורכב יותר שכולל לרוב Bitmap, Screen RAM, Color RAM ורקע
        // ==========================================
        public static void SaveSpriteAsKoala(string filePath, byte[] spriteData, byte backgroundColor)
        {
            // כאן תכניס את לוגיקת האריזה של קואלה שכבר כתבת
            // (למשל הוספת הכתובת $6000 בראש, נתוני מסך, צבעים וכו')
        }

        public static byte[] LoadSpriteFromKoala(string filePath, out byte backgroundColor)
        {
            backgroundColor = 0;
            // כאן תכניס את לוגיקת הפירוק של קובץ קואלה שכבר כתבת
            return null;
        }

    }
}
