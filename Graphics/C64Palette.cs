using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace C64.Graphics
{
    internal class C64Palette
    {
        public enum C64PaletteType
        {
            PeptoVICE,      // הסטנדרט המדויק והרשמי של VICE
            CommunityVivid, // צבעים עזים ורווים
            CCS64           // הגוונים החמים של האמולטור הישן
        }

        static Color[] _activePalette;
        static C64Palette()
        {
            // כבר הגדרנו את הפלטה הפעילה כברירת מחדל_v
            _activePalette = _vividPalette;
        }


        public static readonly Color[] _vividPalette = new Color[16]
        {
            Color.FromArgb(0, 0, 0), // Black
            Color.FromArgb(255, 255, 255), // White
            Color.FromArgb(136, 0, 0), // Red
            Color.FromArgb(170, 255, 238), // Cyan
            Color.FromArgb(204, 68, 204), // Purple
            Color.FromArgb(0, 204, 85), // Green
            Color.FromArgb(0, 0, 170), // Blue
            Color.FromArgb(238, 238, 119), // Yellow
            Color.FromArgb(221, 136, 85), // Orange
            Color.FromArgb(102, 68, 0), // Brown
            Color.FromArgb(255, 119, 119), // Light Red
            Color.FromArgb(51, 51, 51), // Dark Grey
            Color.FromArgb(119, 119, 119), // Grey
            Color.FromArgb(170, 255, 102), // Light Green
            Color.FromArgb(0, 136, 255), // Light Blue
            Color.FromArgb(187, 187, 187) // Light Grey{ }
        };

        // פאלטת Pepto (הרשמית והמדויקת ביותר)
        private static readonly Color[] _peptoPalette = new Color[16]
        {
            Color.FromArgb(0, 0, 0),       // 0: Black
            Color.FromArgb(255, 255, 255), // 1: White
            Color.FromArgb(136, 57, 50),   // 2: Red
            Color.FromArgb(103, 182, 189), // 3: Cyan
            Color.FromArgb(139, 63, 150),  // 4: Purple
            Color.FromArgb(85, 160, 73),   // 5: Green
            Color.FromArgb(64, 49, 141),   // 6: Blue
            Color.FromArgb(191, 206, 114), // 7: Yellow
            Color.FromArgb(139, 84, 31),   // 8: Orange
            Color.FromArgb(87, 66, 0),     // 9: Brown
            Color.FromArgb(184, 105, 98),  // 10: Light Red
            Color.FromArgb(80, 80, 80),    // 11: Dark Grey
            Color.FromArgb(120, 120, 120), // 12: Grey
            Color.FromArgb(148, 224, 137), // 13: Light Green
            Color.FromArgb(120, 105, 196), // 14: Light Blue
            Color.FromArgb(159, 159, 159)  // 15: Light Grey
        };

        private static readonly Color[] _ccs64Palette = new Color[16]
        {
            Color.FromArgb(0, 0, 0),       // 0: Black
            Color.FromArgb(255, 255, 255), // 1: White
            Color.FromArgb(112, 48, 32),   // 2: Red
            Color.FromArgb(144, 208, 224), // 3: Cyan
            Color.FromArgb(144, 64, 160),  // 4: Purple
            Color.FromArgb(64, 144, 64),   // 5: Green
            Color.FromArgb(48, 32, 144),   // 6: Blue
            Color.FromArgb(224, 224, 128), // 7: Yellow
            Color.FromArgb(144, 96, 32),   // 8: Orange
            Color.FromArgb(80, 64, 0),     // 9: Brown
            Color.FromArgb(176, 112, 112), // 10: Light Red
            Color.FromArgb(64, 64, 64),    // 11: Dark Grey
            Color.FromArgb(112, 112, 112), // 12: Grey
            Color.FromArgb(144, 224, 144), // 13: Light Green
            Color.FromArgb(112, 144, 224), // 14: Light Blue
            Color.FromArgb(160, 160, 160)  // 15: Light Grey
        };
        public static Color[] ActivePalette => _activePalette;

        enum C64Colors
        {
            Black = 0,
            White = 1,
            Red = 2,
            Cyan = 3,
            Purple = 4,
            Green = 5,
            Blue = 6,
            Yellow = 7,
            Orange = 8,
            Brown = 9,
            LightRed = 10,
            DarkGray = 11,
            MediumGray = 12,
            LightGreen = 13,
            LightBlue = 14,
            LightGray = 15
        }
        C64Colors c64Colors;

        static string[] ColorNames = {
            "Black","White","Red","Cyan","Purple","Green","Blue","Yellow","Orange","Brown","Light Red","Dark Gray","Medium Gray","Light Green","light Blue","Light Gray" };

         

        public static Color GetColor(int index)
        {
            return _activePalette[index];
        }

        public static string GetColorName (int index)
        {
            return ColorNames[index];
        }

        public static void SelectPalette(C64PaletteType type)
        {
            switch (type)
            {
                case C64PaletteType.PeptoVICE:
                    _activePalette = _peptoPalette;
                    break;
                case C64PaletteType.CommunityVivid:
                    _activePalette = _vividPalette;
                    break;
                default:
                    _activePalette = _peptoPalette;
                    break;
            }
        }

        public static byte MapRGBToC64Index(Color pcColor)
        {
            Vector3 targetVector = new Vector3(pcColor.R, pcColor.G, pcColor.B);
            float minDistance = float.MaxValue;
            byte bestColorIndex = 0;

            for (byte i = 0; i < 16; i++)
            {
                Vector3 currentPaletteVector = new Vector3(_activePalette[i].R, _activePalette[i].G, _activePalette[i].B);
                float distance = Vector3.DistanceSquared(targetVector, currentPaletteVector);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestColorIndex = i;
                }
            }

            return bestColorIndex; // מחזיר מספר בין 0 ל-15 (אינדקס החומרה של הקומודור)
        }

        public static Color MapRGBToC64Color(Color pcColor)
        {
            Vector3 targetVector = new Vector3(pcColor.R, pcColor.G, pcColor.B);
            float minDistance = float.MaxValue;
            Color bestColor = Color.Black;

            for (byte i = 0; i < 16; i++)
            {
                Vector3 currentPaletteVector = new Vector3(_activePalette[i].R, _activePalette[i].G, _activePalette[i].B);
                float distance = Vector3.DistanceSquared(targetVector, currentPaletteVector);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestColor = _activePalette[i];
                }
            }
            return bestColor;
        }
    }
}
