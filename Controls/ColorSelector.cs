using C64.Dialogs;
using C64.Graphics;
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
  
    public partial class ColorSelector : UserControl
    {
        private byte _c64ColorIndex;

        [Category ("C64 Properties"), Description("The index of the color slot (0-7)")]
        public byte SlotIndex {  get; set; }

        ColorSelectorEventArgs colorSelectorEventArgs;
        public ColorSelector()
        {
            InitializeComponent();
            Cursor = Cursors.Hand;
            C64ColorIndex = 0; // ברירת מחדל לצבע שחור
        }

        [Category("C64 Events"), Description("Fires when the color is changed")]
        public event EventHandler ColorChanged;

        [Category ("C64 Properties"), Description("The index of the color in the C64 palette (0-15)")]
        public byte C64ColorIndex
        {
            get => _c64ColorIndex;

            set 
            {
                if (_c64ColorIndex != value && value >= 0 && value < 16)
                {
                    _c64ColorIndex = value;

                    BackColor = C64Palette.GetColor(_c64ColorIndex);
                    Invalidate();
                }
            }

        }

        [Category ("C64 Events"), Description("Fires when the color selector is clicked")]
        public event EventHandler<ColorSelectorEventArgs> ColorSelectorClicked;

        protected override void OnPaint(PaintEventArgs e)
        {
            Pen outline = new Pen(Color.Black, 2);

            if (Focused)
            {
                outline.Color = Color.Red;
            } else 
            {
                outline.Color = Color.Black;
            }

            e.Graphics.DrawRectangle(outline, 0, 0, Width - 1, Height - 1);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Right)
            {
              PaletteDialog paletteDialog = new PaletteDialog();
              if (paletteDialog.ShowDialog() == DialogResult.OK)
              {
                   C64ColorIndex = paletteDialog.GetSelectedColorIndex(); 
                   ColorChanged?.Invoke(this, EventArgs.Empty);
              }
            }
            else
            {
                colorSelectorEventArgs = new ColorSelectorEventArgs(_c64ColorIndex, SlotIndex);
                ColorSelectorClicked?.Invoke(this, colorSelectorEventArgs);
            }
        }

    }

   

    public class ColorSelectorEventArgs : EventArgs
    {
        public byte C64ColorIndex { get; }
        public byte SlotIndex { get; }
        public ColorSelectorEventArgs(byte c64ColorIndex, byte slotIndex)
        {
            C64ColorIndex = c64ColorIndex;
            SlotIndex = slotIndex;
        }
    }
}
