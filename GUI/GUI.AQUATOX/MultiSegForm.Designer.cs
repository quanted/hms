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
            this.comidBox = new System.Windows.Forms.TextBox();
            this.basedirBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.createButton = new System.Windows.Forms.Button();
            this.ProcessLog = new System.Windows.Forms.TextBox();
            this.executeButton = new System.Windows.Forms.Button();
            this.CSVButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pourIDBox = new System.Windows.Forms.TextBox();
            this.spanLabel = new System.Windows.Forms.Label();
            this.spanBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BaseJSONBox = new System.Windows.Forms.TextBox();
            this.SVBox = new System.Windows.Forms.ComboBox();
            this.chartButton = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.ReadSNButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(752, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // comidBox
            // 
            this.comidBox.Location = new System.Drawing.Point(141, 107);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(71, 23);
            this.comidBox.TabIndex = 1;
            this.comidBox.Text = "23398915";
            // 
            // basedirBox
            // 
            this.basedirBox.Location = new System.Drawing.Point(79, 48);
            this.basedirBox.Name = "basedirBox";
            this.basedirBox.Size = new System.Drawing.Size(183, 23);
            this.basedirBox.TabIndex = 2;
            this.basedirBox.Tag = "";
            this.basedirBox.Text = "..\\..\\..\\2D_Inputs\\TestDir1\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "ComID";
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
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(115, 252);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(127, 23);
            this.createButton.TabIndex = 5;
            this.createButton.Text = "Create Linked Inputs";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
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
            this.ProcessLog.Size = new System.Drawing.Size(507, 387);
            this.ProcessLog.TabIndex = 6;
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(115, 289);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(127, 23);
            this.executeButton.TabIndex = 7;
            this.executeButton.Text = "Execute Network";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // CSVButton
            // 
            this.CSVButton.Location = new System.Drawing.Point(167, 421);
            this.CSVButton.Name = "CSVButton";
            this.CSVButton.Size = new System.Drawing.Size(75, 23);
            this.CSVButton.TabIndex = 8;
            this.CSVButton.Text = "CSV";
            this.CSVButton.UseVisualStyleBackColor = true;
            this.CSVButton.Click += new System.EventHandler(this.CSVButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 384);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "SV Index";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "Pour Point ComID";
            // 
            // pourIDBox
            // 
            this.pourIDBox.Location = new System.Drawing.Point(141, 136);
            this.pourIDBox.Name = "pourIDBox";
            this.pourIDBox.Size = new System.Drawing.Size(71, 23);
            this.pourIDBox.TabIndex = 11;
            // 
            // spanLabel
            // 
            this.spanLabel.AutoSize = true;
            this.spanLabel.Location = new System.Drawing.Point(49, 168);
            this.spanLabel.Name = "spanLabel";
            this.spanLabel.Size = new System.Drawing.Size(79, 15);
            this.spanLabel.TabIndex = 14;
            this.spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            this.spanBox.Location = new System.Drawing.Point(141, 165);
            this.spanBox.Name = "spanBox";
            this.spanBox.Size = new System.Drawing.Size(71, 23);
            this.spanBox.TabIndex = 13;
            this.spanBox.Text = "5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(218, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "km";
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
            // SVBox
            // 
            this.SVBox.FormattingEnabled = true;
            this.SVBox.Location = new System.Drawing.Point(84, 381);
            this.SVBox.Name = "SVBox";
            this.SVBox.Size = new System.Drawing.Size(178, 23);
            this.SVBox.TabIndex = 18;
            this.SVBox.SelectedIndexChanged += new System.EventHandler(this.SVBox_SelectedIndexChanged);
            // 
            // chartButton
            // 
            this.chartButton.Location = new System.Drawing.Point(167, 450);
            this.chartButton.Name = "chartButton";
            this.chartButton.Size = new System.Drawing.Size(75, 23);
            this.chartButton.TabIndex = 19;
            this.chartButton.Text = "Graph";
            this.chartButton.UseVisualStyleBackColor = true;
            this.chartButton.Click += new System.EventHandler(this.ChartButtonClick);
            // 
            // SetupButton
            // 
            this.SetupButton.Location = new System.Drawing.Point(424, 10);
            this.SetupButton.Name = "SetupButton";
            this.SetupButton.Size = new System.Drawing.Size(123, 23);
            this.SetupButton.TabIndex = 20;
            this.SetupButton.Text = "Edit Master Setup";
            this.SetupButton.UseVisualStyleBackColor = true;
            this.SetupButton.Click += new System.EventHandler(this.SetupButton_Click);
            // 
            // ReadSNButton
            // 
            this.ReadSNButton.Location = new System.Drawing.Point(115, 218);
            this.ReadSNButton.Name = "ReadSNButton";
            this.ReadSNButton.Size = new System.Drawing.Size(127, 23);
            this.ReadSNButton.TabIndex = 21;
            this.ReadSNButton.Text = "Read Network";
            this.ReadSNButton.UseVisualStyleBackColor = true;
            this.ReadSNButton.Click += new System.EventHandler(this.ReadNetwork_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(567, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
            this.button2.TabIndex = 22;
            this.button2.Text = "Overland Flows";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OverlandFlow_Click);
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 485);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ReadSNButton);
            this.Controls.Add(this.SetupButton);
            this.Controls.Add(this.chartButton);
            this.Controls.Add(this.SVBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BaseJSONBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.spanLabel);
            this.Controls.Add(this.spanBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pourIDBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.CSVButton);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.ProcessLog);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.basedirBox);
            this.Controls.Add(this.comidBox);
            this.Controls.Add(this.button1);
            this.MinimumSize = new System.Drawing.Size(861, 524);
            this.Name = "MultiSegForm";
            this.Text = "MultiSegForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.TextBox basedirBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.TextBox ProcessLog;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button CSVButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pourIDBox;
        private System.Windows.Forms.Label spanLabel;
        private System.Windows.Forms.TextBox spanBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox BaseJSONBox;
        private System.Windows.Forms.ComboBox SVBox;
        private System.Windows.Forms.Button chartButton;
        private System.Windows.Forms.Button SetupButton;
        private System.Windows.Forms.Button ReadSNButton;
        private System.Windows.Forms.Button button2;
    }
}