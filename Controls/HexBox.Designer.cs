using System.Drawing;
using System.Windows.Forms;

namespace C64.Controls
{
    partial class HexBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            hexTextBox = new Label();
            SuspendLayout();
            // 
            // hexTextBox
            // 
            hexTextBox.BorderStyle = BorderStyle.None;
            hexTextBox.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            hexTextBox.Location = new Point(3, 3);
            hexTextBox.Name = "hexTextBox";
            hexTextBox.Size = new Size(144, 54);
            hexTextBox.TabIndex = 0;
            hexTextBox.Text = "0000";
            hexTextBox.TextChanged += hexTextBox_TextChanged;
            hexTextBox.KeyPress += hexTextBox_KeyPress;
            hexTextBox.AutoSize = true;
            // 
            // HexBox
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(hexTextBox);
            Name = "HexBox";
            Size = new Size(150, 60);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label hexTextBox;
    }
}
