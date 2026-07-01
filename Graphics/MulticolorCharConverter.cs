using System;
using System.Collections.Generic;
using System.Text;

namespace C64.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Numerics;

    namespace C64.Graphics
    {
        internal class MulticolorCharConverter : C64GraphicsBase
        {
            public override C64ConversionResult Convert(Bitmap sourceImage)
            {
                // נניח רוחב מסך סטנדרטי של 160 פיקסלים מולטיקולור (שזה 20 תווים לרוחב)
                // וגובה לפי התמונה שלך (למשל 56 פיקסלים שזה 7 תווים לגובה)
                int charsX = sourceImage.Width / 8;
                int charsY = sourceImage.Height / 8;

                List<byte> screenMap = new List<byte>();
                List<byte> colorRam = new List<byte>();
                List<byte> charData = new List<byte>(); // פה יישמרו ה-8 בתים של כל תו

                // לולאה שסורקת את התמונה ברמת ה"תווים" (המשבצות)
                for (int cy = 0; cy < charsY; cy++)
                {
                    for (int cx = 0; cx < charsX; cx++)
                    {
                        // מכינים מערך של 8 בתים שייצג את התו הנוכחי בחומרה של הקומודור
                        byte[] singleCharBytes = new byte[8];

                        // לולאה פנימית שסורקת את ה-8 שורות של הבלוק הנוכחי
                        for (int y = 0; y < 8; y++)
                        {
                            int pixelY = (cy * 8) + y;
                            byte rowByte = 0;

                            // רצים בזוגות פיקסלים (קפיצות של 2) כדי לכפות חומרת מולטיקולור!
                            for (int x = 0; x < 8; x += 2)
                            {
                                int pixelX = (cx * 8) + x;

                                Color leftPixel = sourceImage.GetPixel(pixelX, pixelY);
                                Color rightPixel = sourceImage.GetPixel(pixelX + 1, pixelY);

                                Color finalColor;

                                // אם תוכנת ה-PC לא תיאמה את הצמד, נפעיל את הכרעת השכנים
                                if (leftPixel != rightPixel)
                                {
                                    finalColor = ResolvePixelConflict(sourceImage, pixelX, pixelY, leftPixel, rightPixel);
                                }
                                else
                                {
                                    finalColor = leftPixel;
                                }

                                // מומר לאינדקס צבע רשמי של הקומודור (0-15) מתוך ה-Class הסטטי שלך
                                byte c64ColorIndex = C64Palette.MapRGBToC64Index(finalColor);

                                // משיגים את שני הביטים (00, 01, 10, 11) שמתאימים לצבע הזה בבלוק הנוכחי
                                byte bitPair = GetBitPairForColor(c64ColorIndex, cx, cy);

                                // דוחפים את שני הביטים למקום הנכון בתוך הבייט של השורה
                                // זוג 1 (פיקסל 0,1) נכנס לביטים 6,7
                                // זוג 2 (פיקסל 2,3) נכנס לביטים 4,5 ... וכן הלאה
                                int shift = 6 - x;
                                rowByte |= (byte)(bitPair << shift);
                            }

                            singleCharBytes[y] = rowByte;
                        }

                        // שומרים את הנתונים הגולמיים של התו
                        charData.AddRange(singleCharBytes);

                        // פה בהמשך נכניס את מפת המסך (ScreenMap) וזיכרון הצבעים (ColorRam)
                    }
                }

                // מחזירים את התוצאה למחלקה הראשית
                C64ConversionResult result = new C64ConversionResult();
                result.GraphicData = charData.ToArray();
                return result;
            }

            // אלגוריתם "בדיקת השכנים" שמונע את הארטיפקטים וחיתוך האותיות
            private Color ResolvePixelConflict(Bitmap img, int x, int y, Color colorLeft, Color colorRight)
            {
                int scoreLeft = 0;
                int scoreRight = 0;

                // סורקים חלון קטן מסביב לפיקסל השבור כדי לבדוק רצף של קווים (אנכיים או אופקיים)
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -2; dx <= 3; dx++)
                    {
                        if (dy == 0 && (dx == 0 || dx == 1)) continue; // מדלגים על הזוג הבעייתי עצמו

                        int checkX = x + dx;
                        int checkY = y + dy;

                        if (checkX >= 0 && checkX < img.Width && checkY >= 0 && checkY < img.Height)
                        {
                            Color neighbor = img.GetPixel(checkX, checkY);
                            if (neighbor == colorLeft) scoreLeft++;
                            if (neighbor == colorRight) scoreRight++;
                        }
                    }
                }

                // הצבע שיש לו יותר "שכנים" תומכים בסביבה מנצח, וכך הקו לא נקטע בריבוע שחור
                return (scoreLeft >= scoreRight) ? colorLeft : colorRight;
            }

            private byte GetBitPairForColor(byte c64ColorIndex, int charX, int charY)
            {
                // פונקציית עזר שתחזיר את הביטים (00-11) בהתאם לצבעי המולטיקולור שנבחרו
                // 00 = רקע כללי
                // 01 = מולטיקולור 1
                // 10 = מולטיקולור 2
                // 11 = צבע התו הייחודי

                if (c64ColorIndex == 0) return 0; // נניח שחור הוא תמיד הרקע (00)
                if (c64ColorIndex == 4) return 1; // נניח סגול הוא מולטי 1 (01)
                if (c64ColorIndex == 6) return 2; // נניח כחול הוא מולטי 2 (10)
                return 3;                         // כל צבע אחר (כמו לבן לגוף האות) יהיה צבע התו (11)
            }
        }
    }

}
