namespace C64.Controls
{
    partial class SpritePalette
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
            spriteColorColorSelector = new ColorSelector();
            multiColor1ColorSelector = new ColorSelector();
            multiColor2colorSelector = new ColorSelector();
            backgroundColorSelector = new ColorSelector();
            SuspendLayout();
            // 
            // spriteColorColorSelector
            // 
            spriteColorColorSelector.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            spriteColorColorSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            spriteColorColorSelector.C64ColorIndex = 1;
            spriteColorColorSelector.Location = new System.Drawing.Point(122, 3);
            spriteColorColorSelector.Name = "spriteColorColorSelector";
            spriteColorColorSelector.Size = new System.Drawing.Size(113, 53);
            spriteColorColorSelector.SlotIndex = 1;
            spriteColorColorSelector.TabIndex = 0;
            spriteColorColorSelector.ColorChanged += OnColorChanged;
            spriteColorColorSelector.ColorSelectorClicked += ColorSelectorClicked;
            // 
            // multiColor1ColorSelector
            // 
            multiColor1ColorSelector.BackColor = System.Drawing.Color.FromArgb(136, 0, 0);
            multiColor1ColorSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            multiColor1ColorSelector.C64ColorIndex = 2;
            multiColor1ColorSelector.Location = new System.Drawing.Point(241, 3);
            multiColor1ColorSelector.Name = "multiColor1ColorSelector";
            multiColor1ColorSelector.Size = new System.Drawing.Size(113, 53);
            multiColor1ColorSelector.SlotIndex = 2;
            multiColor1ColorSelector.TabIndex = 1;
            multiColor1ColorSelector.ColorChanged += OnColorChanged;
            multiColor1ColorSelector.ColorSelectorClicked += ColorSelectorClicked;
            // 
            // multiColor2colorSelector
            // 
            multiColor2colorSelector.BackColor = System.Drawing.Color.FromArgb(170, 255, 238);
            multiColor2colorSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            multiColor2colorSelector.C64ColorIndex = 3;
            multiColor2colorSelector.Location = new System.Drawing.Point(360, 3);
            multiColor2colorSelector.Name = "multiColor2colorSelector";
            multiColor2colorSelector.Size = new System.Drawing.Size(113, 53);
            multiColor2colorSelector.SlotIndex = 3;
            multiColor2colorSelector.TabIndex = 2;
            multiColor2colorSelector.ColorChanged += OnColorChanged;
            multiColor2colorSelector.ColorSelectorClicked += ColorSelectorClicked;
            // 
            // backgroundColorSelector
            // 
            backgroundColorSelector.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            backgroundColorSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            backgroundColorSelector.C64ColorIndex = 0;
            backgroundColorSelector.Location = new System.Drawing.Point(3, 3);
            backgroundColorSelector.Name = "backgroundColorSelector";
            backgroundColorSelector.Size = new System.Drawing.Size(113, 53);
            backgroundColorSelector.SlotIndex = 0;
            backgroundColorSelector.TabIndex = 3;
            backgroundColorSelector.ColorChanged += OnColorChanged;
            backgroundColorSelector.ColorSelectorClicked += ColorSelectorClicked;
            // 
            // SpritePalette
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(backgroundColorSelector);
            Controls.Add(multiColor2colorSelector);
            Controls.Add(multiColor1ColorSelector);
            Controls.Add(spriteColorColorSelector);
            Name = "SpritePalette";
            Size = new System.Drawing.Size(476, 59);
            ResumeLayout(false);
        }

        #endregion

        private ColorSelector spriteColorColorSelector;
        private ColorSelector multiColor1ColorSelector;
        private ColorSelector multiColor2colorSelector;
        private ColorSelector backgroundColorSelector;
    }
}
