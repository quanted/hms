namespace GUI.AQUATOX
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            panel1 = new System.Windows.Forms.Panel();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            panel2 = new System.Windows.Forms.Panel();
            label5 = new System.Windows.Forms.Label();
            SingleSegmentLabel = new System.Windows.Forms.Label();
            Help_Button = new System.Windows.Forms.Button();
            MultiSeg = new System.Windows.Forms.Button();
            SingleSeg = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.Black;
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(panel2);
            panel1.Location = new System.Drawing.Point(3, 4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(468, 308);
            panel1.TabIndex = 0;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.x1;
            pictureBox2.Location = new System.Drawing.Point(449, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(18, 19);
            pictureBox2.TabIndex = 8;
            pictureBox2.TabStop = false;
            pictureBox2.Click += Close_Click;
            // 
            // panel2
            // 
            panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            panel2.Controls.Add(label5);
            panel2.Controls.Add(SingleSegmentLabel);
            panel2.Controls.Add(Help_Button);
            panel2.Controls.Add(MultiSeg);
            panel2.Controls.Add(SingleSeg);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(pictureBox1);
            panel2.Location = new System.Drawing.Point(18, 17);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(431, 270);
            panel2.TabIndex = 1;
            panel2.MouseDown += panel1_MouseDown;
            panel2.MouseMove += panel1_MouseMove;
            // 
            // label5
            // 
            label5.BackColor = System.Drawing.Color.WhiteSmoke;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(180, 215);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(165, 46);
            label5.TabIndex = 26;
            label5.Text = "Create linked segments and/or link to data from NHDPlus, NWM, and HAWQS";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SingleSegmentLabel
            // 
            SingleSegmentLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            SingleSegmentLabel.Location = new System.Drawing.Point(23, 216);
            SingleSegmentLabel.Name = "SingleSegmentLabel";
            SingleSegmentLabel.Size = new System.Drawing.Size(146, 46);
            SingleSegmentLabel.TabIndex = 25;
            SingleSegmentLabel.Text = "Run one 0-D model segment with nutrients, chemicals, and a food web";
            SingleSegmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Help_Button
            // 
            Help_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Help_Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            Help_Button.Location = new System.Drawing.Point(349, 190);
            Help_Button.Name = "Help_Button";
            Help_Button.Size = new System.Drawing.Size(53, 23);
            Help_Button.TabIndex = 7;
            Help_Button.Text = "&Help";
            Help_Button.UseVisualStyleBackColor = true;
            Help_Button.Click += Help_Button_Click;
            // 
            // MultiSeg
            // 
            MultiSeg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            MultiSeg.Location = new System.Drawing.Point(186, 190);
            MultiSeg.Name = "MultiSeg";
            MultiSeg.Size = new System.Drawing.Size(144, 23);
            MultiSeg.TabIndex = 6;
            MultiSeg.Text = "&Multi-Segment Mode";
            MultiSeg.UseVisualStyleBackColor = true;
            MultiSeg.Click += MultiSeg_Click;
            // 
            // SingleSeg
            // 
            SingleSeg.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            SingleSeg.Location = new System.Drawing.Point(22, 190);
            SingleSeg.Name = "SingleSeg";
            SingleSeg.Size = new System.Drawing.Size(144, 23);
            SingleSeg.TabIndex = 5;
            SingleSeg.Text = "&Single-Segment Mode";
            SingleSeg.UseVisualStyleBackColor = true;
            SingleSeg.Click += SingleSeg_Click;
            // 
            // label4
            // 
            label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label4.ForeColor = System.Drawing.Color.Maroon;
            label4.Location = new System.Drawing.Point(61, 103);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(317, 75);
            label4.TabIndex = 4;
            label4.Text = "This software and associated files are distributed \"as is\" without any warranties of performance or fitness for any particular purpose.  No warranties are expressed or implied.";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(0, 75);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(430, 18);
            label3.TabIndex = 3;
            label3.Text = "Build 1.0.001";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(0, 56);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(425, 19);
            label2.TabIndex = 2;
            label2.Text = "Release 1.0 Beta";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(81, 22);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(279, 32);
            label1.TabIndex = 1;
            label1.Text = "AQUATOX DOT NET";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new System.Drawing.Point(27, 20);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Padding = new System.Windows.Forms.Padding(1);
            pictureBox1.Size = new System.Drawing.Size(38, 38);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // Splash
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(474, 316);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Splash";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Splash";
            Shown += Splash_Shown;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SingleSeg;
        private System.Windows.Forms.Button MultiSeg;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button Help_Button;
        private System.Windows.Forms.Label SingleSegmentLabel;
        private System.Windows.Forms.Label label5;
    }
}