using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace C64.Controls
{
    public partial class ByteDisplay : UserControl
    {
        public byte byteValue = 0;

        public ByteDisplay()
        {
            InitializeComponent();
            //byteTextBox.Text = data.ToString("X2");
        }

        [Category("Behavior"),Description ("The value displayed insdie the control")]
        public byte ByteValue
        {
            get { return byteValue; }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException("value must be between 0 and 255");
                }

                byteValue = value;
           
                byteTextBox.Text = byteValue.ToString ("X2");
            }
        }
     
    }
}
