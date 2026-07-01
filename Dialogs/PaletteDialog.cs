using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace C64.Dialogs
{
    public partial class PaletteDialog : Form
    {
        public PaletteDialog()
        {
            InitializeComponent();
        }

        private void paletteGrid_ColorSelected(object sender, C64.Controls.PaletteControl.ColorSelectedEventArgs e)
        {
            selectedColorLable.BackColor = e.SelectedColor;
            selectedColorLable.Text = $"{e.ColorIndex}, RGB({e.SelectedColor.R}, {e.SelectedColor.G}, {e.SelectedColor.B})";
            selectedColorLable.TextAlign = ContentAlignment.MiddleCenter;
            selectedColorLable.ForeColor = e.SelectedColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
            selectedColorLable.Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold);
          
        }

        public byte GetSelectedColorIndex()
        {
            return paletteGrid.SelectedColorIndex;
        }
    }


}
