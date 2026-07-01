using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using C64.Graphics;

namespace C64.Controls
{
    public partial class PaletteControl : UserControl
    {
        [Category("C64 Events"), Description("Fires when a color is selected from the palette")]
        public event EventHandler<ColorSelectedEventArgs> ColorSelected;
        public class ColorSelectedEventArgs : EventArgs
        {
            public int ColorIndex { get; }
            public Color SelectedColor { get; }
            public ColorSelectedEventArgs(int colorIndex, Color selectedColor)
            {
                ColorIndex = colorIndex;
                SelectedColor = selectedColor;
            }
        }

        [Category("C64 Properties"), Description("The index of the currently selected color in the palette")]
        public byte SelectedColorIndex { get; private set; } = 1;

        [Category ("C64 Properties"), Description("The color used for the border around the selected cell")]
        public Color SelectedCellBorderColor { get; private set; } = Color.Red;
        [Category("C64 Properties"), Description("The thickness of the border around the selected cell")]
        public int SelectedCellBorderThickness { get; private set; } = 3;

        public PaletteControl()
        {
            InitializeComponent();
        }

   

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int cellSize = Math.Min(Width, Height) / 4;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    int colorIndex = (y * 4) + x;
                    Color c64Color = C64Palette.GetColor(colorIndex);
                    using (SolidBrush brush = new SolidBrush(c64Color))
                    {
                        e.Graphics.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
                    }
                }
            }
            // Draw Thick Border Around The selected color
            using (Pen pen = new Pen(SelectedCellBorderColor, SelectedCellBorderThickness))
            {
                int selectedX = (SelectedColorIndex % 4) * cellSize;
                int selectedY = (SelectedColorIndex / 4) * cellSize;
                e.Graphics.DrawRectangle(pen, selectedX, selectedY, cellSize, cellSize);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Height = Width; // שמירה על יחס 1:1 כדי שהריבועים יהיו ריבועים אמיתיים
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int cellSize = Math.Min(Width, Height) / 4;
            int x = e.X / cellSize;
            int y = e.Y / cellSize;
            if (x >= 0 && x < 4 && y >= 0 && y < 4)
            {
                SelectedColorIndex = (byte)((y * 4) + x);
                Color selectedColor = C64Palette.GetColor(SelectedColorIndex);

                ColorSelectedEventArgs args = new ColorSelectedEventArgs(SelectedColorIndex, selectedColor);
                ColorSelected?.Invoke(this, args);
                Invalidate(); // Redraw the control to show the new selection

                //MessageBox.Show($"Selected C64 Color Index: {SelectedColorIndex}\nRGB: {selectedColor}");
            }
        }
    }
}
