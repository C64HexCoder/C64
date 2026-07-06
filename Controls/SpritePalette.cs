using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C64.Graphics;

namespace C64.Controls
{
    public partial class SpritePalette : UserControl, ISpriteColorProvider
    {

        public byte SelectedSlotIndex { get; set; }

        public Color GetColorForSlot(int slotIndex)
        {
            switch (slotIndex)
            {
                case 0:
                    return C64Palette.ActivePalette[backgroundColorSelector.C64ColorIndex];
                    break;
                case 1:
                    return C64Palette.ActivePalette[spriteColorColorSelector.C64ColorIndex];
                    break;
                case 2:
                    return C64Palette.ActivePalette[multiColor1ColorSelector.C64ColorIndex];
                    break;
                default:
                    return C64Palette.ActivePalette[multiColor2colorSelector.C64ColorIndex];
                    break;
            }
        }

        public Color GetSelectedColor()
        {
            return C64Palette.ActivePalette[SelectedSlotIndex];
        }




        public SpritePalette()
        {
            InitializeComponent();
         
        }
        public event EventHandler ColorChanged;
        private void OnColorChanged()
        {
            ColorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ColorSelectorClicked(object sender, ColorSelectorEventArgs e)
        {
            ColorSelector cs = (ColorSelector)sender;
            SelectedSlotIndex = e.SlotIndex;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (!DesignMode && Parent != null)
            {
                foreach (var control in Parent.Controls)
                {
                    if (control is SpriteCtrl spr)
                    {
                        spr.colorProvider = this;
                    }
                }
            }
        }

        private void OnColorChanged(object sender, EventArgs e)
        {
            ColorChanged?.Invoke(this, EventArgs.Empty);
        }

       
    }
}
