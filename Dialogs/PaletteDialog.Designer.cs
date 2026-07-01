namespace C64.Dialogs
{
    partial class PaletteDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            paletteGrid = new C64.Controls.PaletteControl();
            selectedColorLable = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // paletteGrid
            // 
            paletteGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            paletteGrid.Location = new System.Drawing.Point(0, 0);
            paletteGrid.Name = "paletteGrid";
            paletteGrid.Size = new System.Drawing.Size(457, 457);
            paletteGrid.TabIndex = 0;
            paletteGrid.ColorSelected += paletteGrid_ColorSelected;
            // 
            // selectedColorLable
            // 
            selectedColorLable.Anchor = System.Windows.Forms.AnchorStyles.Left;
            selectedColorLable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            selectedColorLable.Location = new System.Drawing.Point(0, 488);
            selectedColorLable.Name = "selectedColorLable";
            selectedColorLable.Size = new System.Drawing.Size(457, 57);
            selectedColorLable.TabIndex = 1;
            // 
            // okButton
            // 
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(333, 580);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(112, 34);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            // 
            // PaletteDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(457, 626);
            Controls.Add(okButton);
            Controls.Add(selectedColorLable);
            Controls.Add(paletteGrid);
            Name = "PaletteDialog";
            Text = "Palette";
            ResumeLayout(false);
        }

        #endregion

        private Controls.PaletteControl paletteGrid;
        private System.Windows.Forms.Label selectedColorLable;
        private System.Windows.Forms.Button okButton;
    }
}