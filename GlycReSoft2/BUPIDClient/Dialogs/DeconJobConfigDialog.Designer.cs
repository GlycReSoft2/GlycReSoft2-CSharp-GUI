namespace BUPIDClient.Dialogs
{
    partial class DeconJobConfigDialog
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
            this.DeconBatchRadioButton = new System.Windows.Forms.RadioButton();
            this.DeconGroupRadioButton = new System.Windows.Forms.RadioButton();
            this.RunJobButton = new System.Windows.Forms.Button();
            this.OptionsPanel = new System.Windows.Forms.Panel();
            this.JobDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.OptionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DeconBatchRadioButton
            // 
            this.DeconBatchRadioButton.AutoSize = true;
            this.DeconBatchRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeconBatchRadioButton.Location = new System.Drawing.Point(12, 12);
            this.DeconBatchRadioButton.Name = "DeconBatchRadioButton";
            this.DeconBatchRadioButton.Size = new System.Drawing.Size(52, 17);
            this.DeconBatchRadioButton.TabIndex = 0;
            this.DeconBatchRadioButton.TabStop = true;
            this.DeconBatchRadioButton.Text = "Batch";
            this.DeconBatchRadioButton.UseVisualStyleBackColor = true;
            // 
            // DeconGroupRadioButton
            // 
            this.DeconGroupRadioButton.AutoSize = true;
            this.DeconGroupRadioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeconGroupRadioButton.Location = new System.Drawing.Point(12, 35);
            this.DeconGroupRadioButton.Name = "DeconGroupRadioButton";
            this.DeconGroupRadioButton.Size = new System.Drawing.Size(106, 17);
            this.DeconGroupRadioButton.TabIndex = 2;
            this.DeconGroupRadioButton.TabStop = true;
            this.DeconGroupRadioButton.Text = "Group Precursors";
            this.DeconGroupRadioButton.UseVisualStyleBackColor = true;
            // 
            // RunJobButton
            // 
            this.RunJobButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.RunJobButton.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.RunJobButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RunJobButton.Location = new System.Drawing.Point(11, 77);
            this.RunJobButton.Name = "RunJobButton";
            this.RunJobButton.Size = new System.Drawing.Size(75, 23);
            this.RunJobButton.TabIndex = 3;
            this.RunJobButton.Text = "Run";
            this.RunJobButton.UseVisualStyleBackColor = false;
            this.RunJobButton.Click += new System.EventHandler(this.RunJobButton_Click);
            // 
            // OptionsPanel
            // 
            this.OptionsPanel.Controls.Add(this.JobDescriptionTextBox);
            this.OptionsPanel.Controls.Add(this.RunJobButton);
            this.OptionsPanel.Location = new System.Drawing.Point(1, 4);
            this.OptionsPanel.Name = "OptionsPanel";
            this.OptionsPanel.Size = new System.Drawing.Size(328, 110);
            this.OptionsPanel.TabIndex = 4;
            // 
            // JobDescriptionTextBox
            // 
            this.JobDescriptionTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.JobDescriptionTextBox.Location = new System.Drawing.Point(143, 8);
            this.JobDescriptionTextBox.Multiline = true;
            this.JobDescriptionTextBox.Name = "JobDescriptionTextBox";
            this.JobDescriptionTextBox.Size = new System.Drawing.Size(175, 92);
            this.JobDescriptionTextBox.TabIndex = 4;
            // 
            // DeconJobConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 116);
            this.Controls.Add(this.DeconGroupRadioButton);
            this.Controls.Add(this.DeconBatchRadioButton);
            this.Controls.Add(this.OptionsPanel);
            this.Name = "DeconJobConfigDialog";
            this.Text = "Deconvolution Job Options";
            this.OptionsPanel.ResumeLayout(false);
            this.OptionsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton DeconBatchRadioButton;
        private System.Windows.Forms.RadioButton DeconGroupRadioButton;
        private System.Windows.Forms.Button RunJobButton;
        private System.Windows.Forms.Panel OptionsPanel;
        private System.Windows.Forms.TextBox JobDescriptionTextBox;
    }
}