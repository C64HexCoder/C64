using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public abstract class BaseSprite
    {
        // המערך הכללי שמשמש את רוב הספרייטים ה"רגילים"
        protected int[,] pixels;
        protected byte[] RawData; // מערך של 64 בתים שמייצג את הספרייט בפורמט המקורי של ה-C64

        public abstract int Width { get; }
        public abstract int Height { get; }

        protected BaseSprite(int width, int height)
        {
            pixels = new int[width, height];
        }
        protected BaseSprite() { } // מאפשר לירושה להגדיר את המערך בעצמה

        // 🌟 וירטואלי: ברירת המחדל עובדת על מערך 2D פשוט
        public virtual int this[int x, int y]
        {
            get => pixels[x, y];
            set => pixels[x, y] = value;
        }

      //  protected abstract byte[] GetRawData();
    }
}