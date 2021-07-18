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
            this.button1 = new System.Windows.Forms.Button();
            this.basedirBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ProcessLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BaseJSONBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ReadSNButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.spanLabel = new System.Windows.Forms.Label();
            this.spanBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pourIDBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comidBox = new System.Windows.Forms.TextBox();
            this.OutputPanel = new System.Windows.Forms.Panel();
            this.chartButton = new System.Windows.Forms.Button();
            this.SVBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CSVButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.HelpButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.OutputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(760, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // basedirBox
            // 
            this.basedirBox.Location = new System.Drawing.Point(79, 48);
            this.basedirBox.Name = "basedirBox";
            this.basedirBox.Size = new System.Drawing.Size(183, 23);
            this.basedirBox.TabIndex = 2;
            this.basedirBox.Tag = "";
            this.basedirBox.Text = "..\\..\\..\\2D_Inputs\\TestDir1\\";
            this.basedirBox.Leave += new System.EventHandler(this.basedirBox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output Dir";
            // 
            // ProcessLog
            // 
            this.ProcessLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessLog.Location = new System.Drawing.Point(306, 59);
            this.ProcessLog.Multiline = true;
            this.ProcessLog.Name = "ProcessLog";
            this.ProcessLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProcessLog.Size = new System.Drawing.Size(515, 413);
            this.ProcessLog.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "Base JSON";
            // 
            // BaseJSONBox
            // 
            this.BaseJSONBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BaseJSONBox.Location = new System.Drawing.Point(80, 10);
            this.BaseJSONBox.Name = "BaseJSONBox";
            this.BaseJSONBox.Size = new System.Drawing.Size(327, 23);
            this.BaseJSONBox.TabIndex = 16;
            this.BaseJSONBox.Tag = "";
            this.BaseJSONBox.Text = "..\\..\\..\\2D_Inputs\\LBR Glenwood 4.JSON";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.SetupButton);
            this.panel1.Controls.Add(this.executeButton);
            this.panel1.Controls.Add(this.createButton);
            this.panel1.Location = new System.Drawing.Point(25, 253);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 129);
            this.panel1.TabIndex = 23;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(58, 67);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 23);
            this.button2.TabIndex = 23;
            this.button2.Text = "Overland Flows";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OverlandFlow_Click);
            // 
            // SetupButton
            // 
            this.SetupButton.Location = new System.Drawing.Point(58, 13);
            this.SetupButton.Name = "SetupButton";
            this.SetupButton.Size = new System.Drawing.Size(127, 23);
            this.SetupButton.TabIndex = 21;
            this.SetupButton.Text = "Master Setup";
            this.SetupButton.UseVisualStyleBackColor = true;
            this.SetupButton.Click += new System.EventHandler(this.SetupButton_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(58, 94);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(127, 23);
            this.executeButton.TabIndex = 9;
            this.executeButton.Text = "Execute Network";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(58, 40);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(127, 23);
            this.createButton.TabIndex = 8;
            this.createButton.Text = "Create Linked Inputs";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ReadSNButton);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.spanLabel);
            this.panel2.Controls.Add(this.spanBox);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.pourIDBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.comidBox);
            this.panel2.Location = new System.Drawing.Point(25, 93);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(259, 145);
            this.panel2.TabIndex = 24;
            // 
            // ReadSNButton
            // 
            this.ReadSNButton.Location = new System.Drawing.Point(126, 107);
            this.ReadSNButton.Name = "ReadSNButton";
            this.ReadSNButton.Size = new System.Drawing.Size(99, 23);
            this.ReadSNButton.TabIndex = 29;
            this.ReadSNButton.Text = "Read Network";
            this.ReadSNButton.UseVisualStyleBackColor = true;
            this.ReadSNButton.Click += new System.EventHandler(this.ReadNetwork_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(204, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 15);
            this.label6.TabIndex = 28;
            this.label6.Text = "km";
            // 
            // spanLabel
            // 
            this.spanLabel.AutoSize = true;
            this.spanLabel.Location = new System.Drawing.Point(41, 76);
            this.spanLabel.Name = "spanLabel";
            this.spanLabel.Size = new System.Drawing.Size(79, 15);
            this.spanLabel.TabIndex = 27;
            this.spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            this.spanBox.Location = new System.Drawing.Point(127, 73);
            this.spanBox.Name = "spanBox";
            this.spanBox.Size = new System.Drawing.Size(71, 23);
            this.spanBox.TabIndex = 26;
            this.spanBox.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 25;
            this.label4.Text = "(optional) endComID";
            // 
            // pourIDBox
            // 
            this.pourIDBox.Location = new System.Drawing.Point(127, 47);
            this.pourIDBox.Name = "pourIDBox";
            this.pourIDBox.Size = new System.Drawing.Size(71, 23);
            this.pourIDBox.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 23;
            this.label1.Text = "ComID";
            // 
            // comidBox
            // 
            this.comidBox.Location = new System.Drawing.Point(127, 19);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(71, 23);
            this.comidBox.TabIndex = 22;
            this.comidBox.Text = "23398915";
            // 
            // OutputPanel
            // 
            this.OutputPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OutputPanel.Controls.Add(this.chartButton);
            this.OutputPanel.Controls.Add(this.SVBox);
            this.OutputPanel.Controls.Add(this.label3);
            this.OutputPanel.Controls.Add(this.CSVButton);
            this.OutputPanel.Enabled = false;
            this.OutputPanel.Location = new System.Drawing.Point(25, 392);
            this.OutputPanel.Name = "OutputPanel";
            this.OutputPanel.Size = new System.Drawing.Size(259, 81);
            this.OutputPanel.TabIndex = 25;
            // 
            // chartButton
            // 
            this.chartButton.Location = new System.Drawing.Point(165, 46);
            this.chartButton.Name = "chartButton";
            this.chartButton.Size = new System.Drawing.Size(75, 23);
            this.chartButton.TabIndex = 23;
            this.chartButton.Text = "Graph";
            this.chartButton.UseVisualStyleBackColor = true;
            this.chartButton.Click += new System.EventHandler(this.ChartButtonClick);
            // 
            // SVBox
            // 
            this.SVBox.FormattingEnabled = true;
            this.SVBox.Location = new System.Drawing.Point(63, 17);
            this.SVBox.Name = "SVBox";
            this.SVBox.Size = new System.Drawing.Size(178, 23);
            this.SVBox.TabIndex = 22;
            this.SVBox.SelectedIndexChanged += new System.EventHandler(this.SVBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 21;
            this.label3.Text = "SV Index";
            // 
            // CSVButton
            // 
            this.CSVButton.Location = new System.Drawing.Point(81, 46);
            this.CSVButton.Name = "CSVButton";
            this.CSVButton.Size = new System.Drawing.Size(75, 23);
            this.CSVButton.TabIndex = 20;
            this.CSVButton.Text = "CSV";
            this.CSVButton.UseVisualStyleBackColor = true;
            this.CSVButton.Click += new System.EventHandler(this.CSVButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "Read Stream Network";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(54, 243);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(152, 15);
            this.label8.TabIndex = 27;
            this.label8.Text = "Model Setup and Execution";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(54, 385);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 15);
            this.label9.TabIndex = 28;
            this.label9.Text = "View Outputs";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(419, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(117, 23);
            this.button3.TabIndex = 29;
            this.button3.Text = "Choose Template";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Choose_from_Template_Click);
            // 
            // HelpButton
            // 
            this.HelpButton.Image = global::GUI.AQUATOX.Properties.Resources.help_icon;
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(601, 12);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(87, 24);
            this.HelpButton.TabIndex = 30;
            this.HelpButton.Text = "Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 511);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.OutputPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BaseJSONBox);
            this.Controls.Add(this.ProcessLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.basedirBox);
            this.Controls.Add(this.button1);
            this.MinimumSize = new System.Drawing.Size(861, 524);
            this.Name = "MultiSegForm";
            this.Text = "MultiSegForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiSegForm_FormClosing);
            this.Shown += new System.EventHandler(this.MultiSegForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.OutputPanel.ResumeLayout(false);
            this.OutputPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox basedirBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ProcessLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox BaseJSONBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button SetupButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ReadSNButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label spanLabel;
        private System.Windows.Forms.TextBox spanBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pourIDBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.Button chartButton;
        private System.Windows.Forms.ComboBox SVBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CSVButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel OutputPanel;
        private System.Windows.Forms.Button HelpButton;
    }
}