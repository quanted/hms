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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiSegForm));
            this.button1 = new System.Windows.Forms.Button();
            this.basedirBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ProcessLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BaseJSONBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.FlowsButton = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ReadSNButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.spanLabel = new System.Windows.Forms.Label();
            this.spanBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.EndCOMIDBox = new System.Windows.Forms.TextBox();
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
            this.MapPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TogglePanel = new System.Windows.Forms.Panel();
            this.GraphButton = new System.Windows.Forms.RadioButton();
            this.MapButton2 = new System.Windows.Forms.RadioButton();
            this.ConsoleButton = new System.Windows.Forms.RadioButton();
            this.browserButton = new System.Windows.Forms.Button();
            this.PlotPanel = new System.Windows.Forms.Panel();
            this.outputjump = new System.Windows.Forms.CheckBox();
            this.showCOMIDsBox = new System.Windows.Forms.CheckBox();
            this.flowchartButton = new System.Windows.Forms.RadioButton();
            this.mapButton = new System.Windows.Forms.RadioButton();
            this.PlotButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.infolabel1 = new System.Windows.Forms.Label();
            this.infolabel2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.OutputPanel.SuspendLayout();
            this.MapPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.TogglePanel.SuspendLayout();
            this.PlotPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(894, 12);
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
            this.basedirBox.Size = new System.Drawing.Size(172, 23);
            this.basedirBox.TabIndex = 2;
            this.basedirBox.Tag = "";
            this.basedirBox.Text = "..\\..\\..\\2D_Inputs\\TestDir1\\";
            this.basedirBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.basedirBox.Leave += new System.EventHandler(this.basedirBox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 51);
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
            this.ProcessLog.Location = new System.Drawing.Point(306, 66);
            this.ProcessLog.Multiline = true;
            this.ProcessLog.Name = "ProcessLog";
            this.ProcessLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProcessLog.Size = new System.Drawing.Size(649, 396);
            this.ProcessLog.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 13);
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
            this.BaseJSONBox.Enter += new System.EventHandler(this.BaseJSONBox_Enter);
            this.BaseJSONBox.Leave += new System.EventHandler(this.BaseJSONBox_Leave);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.StatusLabel);
            this.panel1.Controls.Add(this.FlowsButton);
            this.panel1.Controls.Add(this.SetupButton);
            this.panel1.Controls.Add(this.executeButton);
            this.panel1.Controls.Add(this.createButton);
            this.panel1.Location = new System.Drawing.Point(25, 253);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 157);
            this.panel1.TabIndex = 23;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(11, 127);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(235, 15);
            this.StatusLabel.TabIndex = 26;
            this.StatusLabel.Text = "Model Status Label";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FlowsButton
            // 
            this.FlowsButton.Location = new System.Drawing.Point(58, 67);
            this.FlowsButton.Name = "FlowsButton";
            this.FlowsButton.Size = new System.Drawing.Size(127, 23);
            this.FlowsButton.TabIndex = 23;
            this.FlowsButton.Text = "Overland Flows";
            this.FlowsButton.UseVisualStyleBackColor = true;
            this.FlowsButton.Click += new System.EventHandler(this.OverlandFlow_Click);
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
            this.panel2.Controls.Add(this.EndCOMIDBox);
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
            this.label4.Location = new System.Drawing.Point(1, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 25;
            this.label4.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            this.EndCOMIDBox.Location = new System.Drawing.Point(127, 47);
            this.EndCOMIDBox.Name = "EndCOMIDBox";
            this.EndCOMIDBox.Size = new System.Drawing.Size(71, 23);
            this.EndCOMIDBox.TabIndex = 24;
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
            this.OutputPanel.Location = new System.Drawing.Point(25, 426);
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
            this.SVBox.SelectionChangeCommitted += new System.EventHandler(this.SVBox_SelectedIndexChanged);
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
            this.label9.Location = new System.Drawing.Point(54, 419);
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
            this.HelpButton.Location = new System.Drawing.Point(662, 9);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(78, 28);
            this.HelpButton.TabIndex = 30;
            this.HelpButton.Text = "   Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // MapPanel
            // 
            this.MapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MapPanel.BackColor = System.Drawing.Color.White;
            this.MapPanel.Controls.Add(this.pictureBox1);
            this.MapPanel.Location = new System.Drawing.Point(305, 66);
            this.MapPanel.Name = "MapPanel";
            this.MapPanel.Size = new System.Drawing.Size(650, 396);
            this.MapPanel.TabIndex = 31;
            this.MapPanel.Visible = false;
            this.MapPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapPanel_MouseDown);
            this.MapPanel.MouseHover += new System.EventHandler(this.MapPanel_MouseHover);
            this.MapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapPanel_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(299, 329);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(346, 63);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // TogglePanel
            // 
            this.TogglePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TogglePanel.Controls.Add(this.GraphButton);
            this.TogglePanel.Controls.Add(this.MapButton2);
            this.TogglePanel.Controls.Add(this.ConsoleButton);
            this.TogglePanel.Location = new System.Drawing.Point(305, 467);
            this.TogglePanel.Name = "TogglePanel";
            this.TogglePanel.Size = new System.Drawing.Size(291, 23);
            this.TogglePanel.TabIndex = 36;
            // 
            // GraphButton
            // 
            this.GraphButton.AutoSize = true;
            this.GraphButton.Location = new System.Drawing.Point(186, 2);
            this.GraphButton.Name = "GraphButton";
            this.GraphButton.Size = new System.Drawing.Size(89, 19);
            this.GraphButton.TabIndex = 2;
            this.GraphButton.Text = "Show Graph";
            this.GraphButton.UseVisualStyleBackColor = true;
            this.GraphButton.CheckedChanged += new System.EventHandler(this.ConsoleButton_CheckedChanged);
            // 
            // MapButton2
            // 
            this.MapButton2.AutoSize = true;
            this.MapButton2.Location = new System.Drawing.Point(95, 3);
            this.MapButton2.Name = "MapButton2";
            this.MapButton2.Size = new System.Drawing.Size(81, 19);
            this.MapButton2.TabIndex = 1;
            this.MapButton2.Text = "Show Map";
            this.MapButton2.UseVisualStyleBackColor = true;
            this.MapButton2.CheckedChanged += new System.EventHandler(this.ConsoleButton_CheckedChanged);
            // 
            // ConsoleButton
            // 
            this.ConsoleButton.AutoSize = true;
            this.ConsoleButton.Checked = true;
            this.ConsoleButton.Location = new System.Drawing.Point(3, 3);
            this.ConsoleButton.Name = "ConsoleButton";
            this.ConsoleButton.Size = new System.Drawing.Size(77, 19);
            this.ConsoleButton.TabIndex = 0;
            this.ConsoleButton.TabStop = true;
            this.ConsoleButton.Text = "Show Log";
            this.ConsoleButton.UseVisualStyleBackColor = true;
            this.ConsoleButton.CheckedChanged += new System.EventHandler(this.ConsoleButton_CheckedChanged);
            // 
            // browserButton
            // 
            this.browserButton.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.browserButton.ForeColor = System.Drawing.Color.Black;
            this.browserButton.Location = new System.Drawing.Point(251, 48);
            this.browserButton.Margin = new System.Windows.Forms.Padding(0);
            this.browserButton.Name = "browserButton";
            this.browserButton.Size = new System.Drawing.Size(47, 23);
            this.browserButton.TabIndex = 37;
            this.browserButton.Text = "Browse";
            this.browserButton.UseVisualStyleBackColor = true;
            this.browserButton.Click += new System.EventHandler(this.browserButton_Click);
            // 
            // PlotPanel
            // 
            this.PlotPanel.Controls.Add(this.outputjump);
            this.PlotPanel.Controls.Add(this.showCOMIDsBox);
            this.PlotPanel.Controls.Add(this.flowchartButton);
            this.PlotPanel.Controls.Add(this.mapButton);
            this.PlotPanel.Controls.Add(this.PlotButton);
            this.PlotPanel.Location = new System.Drawing.Point(305, 42);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Size = new System.Drawing.Size(497, 23);
            this.PlotPanel.TabIndex = 38;
            // 
            // outputjump
            // 
            this.outputjump.AutoSize = true;
            this.outputjump.Checked = true;
            this.outputjump.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputjump.Enabled = false;
            this.outputjump.Location = new System.Drawing.Point(357, 3);
            this.outputjump.Name = "outputjump";
            this.outputjump.Size = new System.Drawing.Size(112, 19);
            this.outputjump.TabIndex = 40;
            this.outputjump.Text = "Click to Outputs";
            this.outputjump.UseVisualStyleBackColor = true;
            this.outputjump.CheckedChanged += new System.EventHandler(this.showCOMIDsBox_CheckedChanged);
            // 
            // showCOMIDsBox
            // 
            this.showCOMIDsBox.AutoSize = true;
            this.showCOMIDsBox.Checked = true;
            this.showCOMIDsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCOMIDsBox.Location = new System.Drawing.Point(243, 3);
            this.showCOMIDsBox.Name = "showCOMIDsBox";
            this.showCOMIDsBox.Size = new System.Drawing.Size(101, 19);
            this.showCOMIDsBox.TabIndex = 39;
            this.showCOMIDsBox.Text = "Label COMIDs";
            this.showCOMIDsBox.UseVisualStyleBackColor = true;
            this.showCOMIDsBox.CheckedChanged += new System.EventHandler(this.showCOMIDsBox_CheckedChanged);
            // 
            // flowchartButton
            // 
            this.flowchartButton.AutoSize = true;
            this.flowchartButton.Location = new System.Drawing.Point(162, 2);
            this.flowchartButton.Name = "flowchartButton";
            this.flowchartButton.Size = new System.Drawing.Size(75, 19);
            this.flowchartButton.TabIndex = 38;
            this.flowchartButton.Text = "flowchart";
            this.flowchartButton.UseVisualStyleBackColor = true;
            this.flowchartButton.CheckedChanged += new System.EventHandler(this.mapButton_CheckedChanged);
            // 
            // mapButton
            // 
            this.mapButton.AutoSize = true;
            this.mapButton.Checked = true;
            this.mapButton.Location = new System.Drawing.Point(110, 2);
            this.mapButton.Name = "mapButton";
            this.mapButton.Size = new System.Drawing.Size(49, 19);
            this.mapButton.TabIndex = 37;
            this.mapButton.TabStop = true;
            this.mapButton.Text = "map";
            this.mapButton.UseVisualStyleBackColor = true;
            this.mapButton.CheckedChanged += new System.EventHandler(this.mapButton_CheckedChanged);
            // 
            // PlotButton
            // 
            this.PlotButton.Location = new System.Drawing.Point(5, 1);
            this.PlotButton.Name = "PlotButton";
            this.PlotButton.Size = new System.Drawing.Size(99, 21);
            this.PlotButton.TabIndex = 36;
            this.PlotButton.Text = "Plot Network";
            this.PlotButton.UseVisualStyleBackColor = true;
            this.PlotButton.Click += new System.EventHandler(this.PlotButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            this.progressBar1.Location = new System.Drawing.Point(28, 381);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(252, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 27;
            this.progressBar1.Value = 1;
            this.progressBar1.Visible = false;
            // 
            // infolabel1
            // 
            this.infolabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel1.AutoSize = true;
            this.infolabel1.Location = new System.Drawing.Point(619, 469);
            this.infolabel1.Name = "infolabel1";
            this.infolabel1.Size = new System.Drawing.Size(361, 15);
            this.infolabel1.TabIndex = 39;
            this.infolabel1.Text = "Click on a COMID to view/edit model inputs (or outputs if relevant)";
            this.infolabel1.Visible = false;
            // 
            // infolabel2
            // 
            this.infolabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel2.AutoSize = true;
            this.infolabel2.Location = new System.Drawing.Point(619, 488);
            this.infolabel2.Name = "infolabel2";
            this.infolabel2.Size = new System.Drawing.Size(244, 15);
            this.infolabel2.TabIndex = 40;
            this.infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom.";
            this.infolabel2.Visible = false;
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 514);
            this.Controls.Add(this.infolabel2);
            this.Controls.Add(this.infolabel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.PlotPanel);
            this.Controls.Add(this.browserButton);
            this.Controls.Add(this.TogglePanel);
            this.Controls.Add(this.MapPanel);
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
            this.MinimumSize = new System.Drawing.Size(1003, 553);
            this.Name = "MultiSegForm";
            this.Text = "MultiSegForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiSegForm_FormClosing);
            this.Shown += new System.EventHandler(this.MultiSegForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.MultiSegForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MultiSegForm_Resize);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.OutputPanel.ResumeLayout(false);
            this.OutputPanel.PerformLayout();
            this.MapPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.TogglePanel.ResumeLayout(false);
            this.TogglePanel.PerformLayout();
            this.PlotPanel.ResumeLayout(false);
            this.PlotPanel.PerformLayout();
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
        private System.Windows.Forms.Button FlowsButton;
        private System.Windows.Forms.Button SetupButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ReadSNButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label spanLabel;
        private System.Windows.Forms.TextBox spanBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox EndCOMIDBox;
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
        private System.Windows.Forms.Panel MapPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel TogglePanel;
        private System.Windows.Forms.RadioButton MapButton2;
        private System.Windows.Forms.RadioButton ConsoleButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button browserButton;
        private System.Windows.Forms.Panel PlotPanel;
        private System.Windows.Forms.CheckBox showCOMIDsBox;
        private System.Windows.Forms.RadioButton flowchartButton;
        private System.Windows.Forms.RadioButton mapButton;
        private System.Windows.Forms.Button PlotButton;
        private System.Windows.Forms.CheckBox outputjump;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton GraphButton;
        private System.Windows.Forms.Label infolabel1;
        private System.Windows.Forms.Label infolabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}