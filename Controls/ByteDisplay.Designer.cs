namespace C64.Controls
{
    partial class ByteDisplay
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
            byteTextBox = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // byteTextBox
            // 
            byteTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            byteTextBox.Location = new System.Drawing.Point(3, 3);
            byteTextBox.Name = "byteTextBox";
            byteTextBox.Size = new System.Drawing.Size(88, 31);
            byteTextBox.TabIndex = 0;
            byteTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ByteDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(byteTextBox);
            Name = "ByteDisplay";
            Size = new System.Drawing.Size(94, 34);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label byteTextBox;
    }
}
