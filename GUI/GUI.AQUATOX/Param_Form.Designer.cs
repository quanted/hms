namespace GUI.AQUATOX
{
    partial class Param_Form
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
            this.CancelButt = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.DB_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.Location = new System.Drawing.Point(878, 10);
            this.CancelButt.Name = "CancelButt";
            this.CancelButt.Size = new System.Drawing.Size(61, 23);
            this.CancelButt.TabIndex = 0;
            this.CancelButt.Text = "Cancel";
            this.CancelButt.UseVisualStyleBackColor = true;
            this.CancelButt.Click += new System.EventHandler(this.cancel_click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(811, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // DB_Button
            // 
            this.DB_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DB_Button.Location = new System.Drawing.Point(602, 10);
            this.DB_Button.Name = "DB_Button";
            this.DB_Button.Size = new System.Drawing.Size(105, 23);
            this.DB_Button.TabIndex = 1;
            this.DB_Button.Text = "Read from DB";
            this.DB_Button.UseVisualStyleBackColor = true;
            this.DB_Button.Click += new System.EventHandler(this.DB_Button_Click);
            // 
            // Param_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 498);
            this.Controls.Add(this.DB_Button);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CancelButt);
            this.Name = "Param_Form";
            this.Text = "Param_Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Param_Form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Param_Form_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button DB_Button;
    }
}