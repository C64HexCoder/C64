using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Numerics;

namespace C64.Graphics
{
    public class C64ConversionResult
    {
        public byte[] ScreenMap { get; set; }
        public byte[] ColorRam { get; set; }
        public byte[] GraphicData { get; set; } // יכול להיות Char Data או Bitmap Data
    }
    internal abstract class C64GraphicsBase
    {
        C64Palette C64Colors = new C64Palette();
        protected byte MapRGBToC64Color(Color pcColor)
        {
           return C64Palette.MapRGBToC64Index(pcColor);      
        }

        // 2. החוזה (החלק המופשט) - כל בן חייב לממש אותו בדרך שלו
        public abstract C64ConversionResult Convert(Bitmap sourceImage);
    }
}
