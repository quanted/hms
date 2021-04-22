namespace GUI.AQUATOX
{
    partial class MultiSegForm
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
            this.comidBox = new System.Windows.Forms.TextBox();
            this.basedirBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.executeButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SVBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.Location = new System.Drawing.Point(728, 10);
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
            this.button1.Location = new System.Drawing.Point(661, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // comidBox
            // 
            this.comidBox.Location = new System.Drawing.Point(115, 42);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(127, 23);
            this.comidBox.TabIndex = 1;
            this.comidBox.Text = "23398915";
            // 
            // basedirBox
            // 
            this.basedirBox.Location = new System.Drawing.Point(115, 84);
            this.basedirBox.Name = "basedirBox";
            this.basedirBox.Size = new System.Drawing.Size(146, 23);
            this.basedirBox.TabIndex = 2;
            this.basedirBox.Tag = "";
            this.basedirBox.Text = "C:\\newtemp\\TestDir1\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "ComID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output Dir";
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(167, 129);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(75, 23);
            this.createButton.TabIndex = 5;
            this.createButton.Text = "Create";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(306, 59);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(483, 417);
            this.textBox3.TabIndex = 6;
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(167, 173);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 7;
            this.executeButton.Text = "Execute";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(167, 223);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "CSV";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // SVBox
            // 
            this.SVBox.Location = new System.Drawing.Point(108, 223);
            this.SVBox.Name = "SVBox";
            this.SVBox.Size = new System.Drawing.Size(29, 23);
            this.SVBox.TabIndex = 9;
            this.SVBox.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "SV Index";
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 515);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SVBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.basedirBox);
            this.Controls.Add(this.comidBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CancelButt);
            this.Name = "MultiSegForm";
            this.Text = "MultiSegForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.TextBox basedirBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox SVBox;
        private System.Windows.Forms.Label label3;
    }
}