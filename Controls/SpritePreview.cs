using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C64.Controls
{
    public partial class SpritePreview : UserControl
    {
        const int SpriteWidth = 24;
        const int SpriteHeight = 21;
        const int MultiColorSpriteWidth = 12;

        private Bitmap SpritePreviewCanvas = new (SpriteWidth, SpriteHeight);
        public SpritePreview()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public ISpritePreview SpritePrevInterface { get; private set; }

        override protected void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
                return;

            // Custom painting code here
            int SprWidth = SpritePrevInterface?.IsMultiColor == true ? MultiColorSpriteWidth : SpriteWidth;
            for (int y = 0; y < SpriteHeight; y++)
            {
                for (int x = 0; x < SpriteWidth; x++)
                {
                   
        
                    if (SpritePrevInterface.IsMultiColor)
                    {
                        Color pixelColor = SpritePrevInterface?.GetPreviewPixel(x/2, y) ?? Color.Transparent;
                        // For multi-color sprites, we need to double the width of each pixel
                        SpritePreviewCanvas.SetPixel(x, y, pixelColor);
                        SpritePreviewCanvas.SetPixel(x + 1, y, pixelColor);
                        x++; // Skip the next pixel since it's already set
                    }
                    else
                    {
                        Color pixelColor = SpritePrevInterface?.GetPreviewPixel(x, y) ?? Color.Transparent;
                        // For multi-color sprites, we need to double the width of each pixel
                        SpritePreviewCanvas.SetPixel(x, y, pixelColor);
                    }

                }
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.DrawImage(SpritePreviewCanvas, 0, 0, this.Width, this.Height);

        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            // אם ה-ColorProvider עדיין לא קושר ידנית ב-Properties, והתוכנה רצה כרגע (לא ב-Designer)
            if (SpritePrevInterface == null && !DesignMode && FindForm() != null)
            {
                // סורקים את כל הפקדים בטופס ומחפשים מישהו שמממש את ה-Interface שלנו
                foreach (Control ctrl in FindForm().Controls)
                {
                    if (ctrl is ISpritePreview spritePreview)
                    {
                        // מצאנו! משדכים אוטומטית
                        SpritePrevInterface = spritePreview;
                        SpritePrevInterface.SpriteChanged += (s, e) => Invalidate(); // רישום לאירוע שינוי הספראייט
                        break; // מצאנו אחד, מספיק טוב
                    }
                }
            }
        }
    }
    
}
