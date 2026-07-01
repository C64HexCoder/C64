using C64.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C64.IO
{
    public static class PRG
    {

        public static void LoadPRGFile(string filePath)
        {
            byte[] prgData = File.ReadAllBytes(filePath);
            if (prgData.Length < 2)
            {
                Console.WriteLine("Invalid PRG file.");
                return;
            }

            // The first two bytes of a PRG file indicate the load address
            ushort loadAddress = (ushort)(prgData[0] | (prgData[1] << 8));
            byte[] programData = prgData.Skip(2).ToArray();

            // Here you would typically load the program data into the C64's memory
            // at the specified load address. This is just a placeholder for demonstration.
            Console.WriteLine($"Loaded PRG file with load address: {loadAddress:X4} and data length: {programData.Length}");
        }

        // גודל של ספרייט בודד של קומודור 64 בבתים (63 בתים של דאטה + 1 בית זבל/הרחבה)
        private const int C64_SPRITE_SIZE_BYTES = 64;

        public static List<Sprite> LoadFromPRGFile(string filePath)
        {
            List<Sprite> spritesList = new List<Sprite>();

            // קריאת כל הבתים של קובץ ה-PRG
            byte[] fileBytes = File.ReadAllBytes(filePath);

            if (fileBytes.Length < 2)
                throw new Exception("קובץ ה-PRG קצר מדי או פגום.");

            // 💡 בקומודור 64 (Little Endian), שני הבתים הראשונים הם כתובת הטעינה בזיכרון (Load Address)
            ushort loadAddress = (ushort)(fileBytes[0] | (fileBytes[1] << 8));

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

    public static bool SavePRGFile(string filePath, ushort loadAddress, byte[] programData)

        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    // Write the load address as the first two bytes
                    fs.WriteByte((byte)(loadAddress & 0xFF)); // Low byte
                    fs.WriteByte((byte)((loadAddress >> 8) & 0xFF)); // High byte
                    // Write the program data
                    fs.Write(programData, 0, programData.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving PRG file: {ex.Message}");
                return false;
            }
        }
    }
}
