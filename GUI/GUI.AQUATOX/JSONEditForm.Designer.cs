namespace GUI.AQUATOX
{
    partial class JSONEditForm
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
            cancelButton = new System.Windows.Forms.Button();
            okButton = new System.Windows.Forms.Button();
            textBox = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            API_key_textbox = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Location = new System.Drawing.Point(639, 23);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(68, 26);
            cancelButton.TabIndex = 48;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Location = new System.Drawing.Point(514, 23);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(101, 26);
            okButton.TabIndex = 47;
            okButton.Text = "Run HAWQS";
            okButton.UseVisualStyleBackColor = true;
            // 
            // textBox
            // 
            textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBox.Location = new System.Drawing.Point(28, 71);
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBox.Size = new System.Drawing.Size(678, 406);
            textBox.TabIndex = 49;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(30, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(92, 15);
            label1.TabIndex = 51;
            label1.Text = "HAWQS API Key";
            // 
            // API_key_textbox
            // 
            API_key_textbox.Location = new System.Drawing.Point(126, 24);
            API_key_textbox.Name = "API_key_textbox";
            API_key_textbox.Size = new System.Drawing.Size(342, 23);
            API_key_textbox.TabIndex = 50;
            // 
            // JSONEditForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(729, 506);
            Controls.Add(label1);
            Controls.Add(API_key_textbox);
            Controls.Add(textBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            MinimumSize = new System.Drawing.Size(745, 545);
            Name = "JSONEditForm";
            Text = "HAWQS Input JSON:";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox API_key_textbox;
    }
}