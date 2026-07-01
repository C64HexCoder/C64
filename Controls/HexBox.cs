using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace C64.Controls
{
    public partial class HexBox : UserControl
    {
        public int HexByte = 0;

        [Category("HexBox Behavior")]
        [Description("Accure when A Hex number was changed")]
        public event EventHandler HexBoxChanged;
        public HexBox()
        {
            InitializeComponent();
        }


        [Description("Enables/Disables Input in Hex TextBox")]
        [Category("TextBox Behavior")]
   
        public int Value
        {
            get
            {
                return Convert.ToInt32(hexTextBox.Text);
            }

            set
            {
                hexTextBox.Text = value.ToString("X4");
                Invalidate();

            }
        }

        private void hexTextBox_TextChanged(object sender, EventArgs e)
        {
            Label txt = (Label)sender;
            if (txt.Text.Length == 4)
            {
                HexByte = int.Parse(txt.Text, System.Globalization.NumberStyles.HexNumber);
                HexBoxChanged?.Invoke(this, new EventArgs());
            }
        }

        private void hexTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string allowdChars = "0123456789abcdefABCDEF";

            if (allowdChars.Contains(e.KeyChar))
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

    }
}
