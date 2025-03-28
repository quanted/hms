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
            CancelButt = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            DB_Button = new System.Windows.Forms.Button();
            Help_Button = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // CancelButt
            // 
            CancelButt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CancelButt.Font = new System.Drawing.Font("Segoe UI", 9F);
            CancelButt.Location = new System.Drawing.Point(883, 10);
            CancelButt.Name = "CancelButt";
            CancelButt.Size = new System.Drawing.Size(61, 23);
            CancelButt.TabIndex = 0;
            CancelButt.Text = "Cancel";
            CancelButt.UseVisualStyleBackColor = true;
            CancelButt.Click += cancel_click;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            button1.Location = new System.Drawing.Point(816, 10);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(61, 23);
            button1.TabIndex = 0;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OK_click;
            // 
            // DB_Button
            // 
            DB_Button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            DB_Button.Font = new System.Drawing.Font("Segoe UI", 9F);
            DB_Button.Location = new System.Drawing.Point(574, 10);
            DB_Button.Name = "DB_Button";
            DB_Button.Size = new System.Drawing.Size(105, 23);
            DB_Button.TabIndex = 1;
            DB_Button.Text = "Read from DB";
            DB_Button.UseVisualStyleBackColor = true;
            DB_Button.Click += DB_Button_Click;
            // 
            // Help_Button
            // 
            Help_Button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            Help_Button.Font = new System.Drawing.Font("Segoe UI", 9F);
            Help_Button.Image = Properties.Resources.help_icon;
            Help_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Help_Button.Location = new System.Drawing.Point(703, 8);
            Help_Button.Name = "Help_Button";
            Help_Button.Size = new System.Drawing.Size(87, 27);
            Help_Button.TabIndex = 24;
            Help_Button.Text = "  Help";
            Help_Button.UseVisualStyleBackColor = true;
            Help_Button.Click += HelpButton_Click;
            // 
            // Param_Form
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            ClientSize = new System.Drawing.Size(962, 497);
            Controls.Add(Help_Button);
            Controls.Add(DB_Button);
            Controls.Add(button1);
            Controls.Add(CancelButt);
            MinimumSize = new System.Drawing.Size(978, 385);
            Name = "Param_Form";
            Text = "Edit Simulation Parameters";
            FormClosing += Param_Form_FormClosing;
            FormClosed += Param_Form_FormClosed;
            Load += Param_Form_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button DB_Button;
        private System.Windows.Forms.Button Help_Button;
    }
}