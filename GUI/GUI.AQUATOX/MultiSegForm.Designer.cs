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
            this.basedirBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ProcessLog = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BaseJSONBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.FlowsButton = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.CreateButton = new System.Windows.Forms.Button();
            this.ReadNetworkPanel = new System.Windows.Forms.Panel();
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
            this.viewOutputButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ChooseTemplateButton = new System.Windows.Forms.Button();
            this.HelpButton2 = new System.Windows.Forms.Button();
            this.TogglePanel = new System.Windows.Forms.Panel();
            this.GraphButton = new System.Windows.Forms.RadioButton();
            this.MapButton2 = new System.Windows.Forms.RadioButton();
            this.ConsoleButton = new System.Windows.Forms.RadioButton();
            this.browseButton = new System.Windows.Forms.Button();
            this.PlotPanel = new System.Windows.Forms.Panel();
            this.ShowBoundBox = new System.Windows.Forms.CheckBox();
            this.outputjump = new System.Windows.Forms.CheckBox();
            this.PlotButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.infolabel1 = new System.Windows.Forms.Label();
            this.infolabel2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.RecentLabel = new System.Windows.Forms.Label();
            this.RecentFilesBox = new System.Windows.Forms.ComboBox();
            this.NewProject = new System.Windows.Forms.Button();
            this.Separator = new System.Windows.Forms.Label();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.ReadNetworkPanel.SuspendLayout();
            this.OutputPanel.SuspendLayout();
            this.TogglePanel.SuspendLayout();
            this.PlotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // basedirBox
            // 
            this.basedirBox.Location = new System.Drawing.Point(79, 9);
            this.basedirBox.Name = "basedirBox";
            this.basedirBox.Size = new System.Drawing.Size(427, 23);
            this.basedirBox.TabIndex = 2;
            this.basedirBox.Tag = "";
            this.basedirBox.Text = "..\\..\\..\\2D_Inputs\\TestDir1\\";
            this.basedirBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.basedirBox_KeyDown);
            this.basedirBox.Leave += new System.EventHandler(this.basedirBox_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(10, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Project Dir";
            // 
            // ProcessLog
            // 
            this.ProcessLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessLog.Location = new System.Drawing.Point(306, 80);
            this.ProcessLog.Multiline = true;
            this.ProcessLog.Name = "ProcessLog";
            this.ProcessLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProcessLog.Size = new System.Drawing.Size(818, 454);
            this.ProcessLog.TabIndex = 6;
            this.ProcessLog.WordWrap = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "Base JSON";
            // 
            // BaseJSONBox
            // 
            this.BaseJSONBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BaseJSONBox.Location = new System.Drawing.Point(83, 69);
            this.BaseJSONBox.Name = "BaseJSONBox";
            this.BaseJSONBox.Size = new System.Drawing.Size(200, 23);
            this.BaseJSONBox.TabIndex = 16;
            this.BaseJSONBox.Tag = "";
            this.BaseJSONBox.Text = "..\\..\\..\\2D_Inputs\\LBR Glenwood 4.JSON";
            this.BaseJSONBox.Enter += new System.EventHandler(this.BaseJSONBox_Enter);
            this.BaseJSONBox.Leave += new System.EventHandler(this.BaseJSONBox_Leave);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.StatusLabel);
            this.panel1.Controls.Add(this.FlowsButton);
            this.panel1.Controls.Add(this.SetupButton);
            this.panel1.Controls.Add(this.executeButton);
            this.panel1.Controls.Add(this.CreateButton);
            this.panel1.Location = new System.Drawing.Point(24, 291);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 178);
            this.panel1.TabIndex = 23;
            // 
            // CancelButton
            // 
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CancelButton.ForeColor = System.Drawing.Color.Black;
            this.CancelButton.Location = new System.Drawing.Point(87, 151);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(68, 24);
            this.CancelButton.TabIndex = 50;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Visible = false;
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
            // CreateButton
            // 
            this.CreateButton.Location = new System.Drawing.Point(58, 40);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(127, 23);
            this.CreateButton.TabIndex = 8;
            this.CreateButton.Text = "Create Linked Inputs";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // ReadNetworkPanel
            // 
            this.ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReadNetworkPanel.Controls.Add(this.ReadSNButton);
            this.ReadNetworkPanel.Controls.Add(this.label6);
            this.ReadNetworkPanel.Controls.Add(this.spanLabel);
            this.ReadNetworkPanel.Controls.Add(this.spanBox);
            this.ReadNetworkPanel.Controls.Add(this.label4);
            this.ReadNetworkPanel.Controls.Add(this.EndCOMIDBox);
            this.ReadNetworkPanel.Controls.Add(this.label1);
            this.ReadNetworkPanel.Controls.Add(this.comidBox);
            this.ReadNetworkPanel.Location = new System.Drawing.Point(24, 133);
            this.ReadNetworkPanel.Name = "ReadNetworkPanel";
            this.ReadNetworkPanel.Size = new System.Drawing.Size(259, 143);
            this.ReadNetworkPanel.TabIndex = 24;
            // 
            // ReadSNButton
            // 
            this.ReadSNButton.Enabled = false;
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
            this.spanLabel.Enabled = false;
            this.spanLabel.Location = new System.Drawing.Point(41, 76);
            this.spanLabel.Name = "spanLabel";
            this.spanLabel.Size = new System.Drawing.Size(79, 15);
            this.spanLabel.TabIndex = 27;
            this.spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            this.spanBox.Enabled = false;
            this.spanBox.Location = new System.Drawing.Point(127, 73);
            this.spanBox.Name = "spanBox";
            this.spanBox.Size = new System.Drawing.Size(71, 23);
            this.spanBox.TabIndex = 26;
            this.spanBox.Text = "5";
            this.spanBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(1, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 25;
            this.label4.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            this.EndCOMIDBox.Enabled = false;
            this.EndCOMIDBox.Location = new System.Drawing.Point(127, 47);
            this.EndCOMIDBox.Name = "EndCOMIDBox";
            this.EndCOMIDBox.Size = new System.Drawing.Size(71, 23);
            this.EndCOMIDBox.TabIndex = 24;
            this.EndCOMIDBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(76, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 23;
            this.label1.Text = "ComID";
            // 
            // comidBox
            // 
            this.comidBox.Enabled = false;
            this.comidBox.Location = new System.Drawing.Point(127, 19);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(71, 23);
            this.comidBox.TabIndex = 22;
            this.comidBox.Text = "23398915";
            this.comidBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // OutputPanel
            // 
            this.OutputPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OutputPanel.Controls.Add(this.chartButton);
            this.OutputPanel.Controls.Add(this.SVBox);
            this.OutputPanel.Controls.Add(this.label3);
            this.OutputPanel.Controls.Add(this.CSVButton);
            this.OutputPanel.Enabled = false;
            this.OutputPanel.Location = new System.Drawing.Point(24, 481);
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
            // viewOutputButton
            // 
            this.viewOutputButton.Location = new System.Drawing.Point(83, 485);
            this.viewOutputButton.Name = "viewOutputButton";
            this.viewOutputButton.Size = new System.Drawing.Size(127, 23);
            this.viewOutputButton.TabIndex = 49;
            this.viewOutputButton.Text = "View Outputs";
            this.viewOutputButton.UseVisualStyleBackColor = true;
            this.viewOutputButton.Click += new System.EventHandler(this.viewOutputButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(53, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "Read Stream Network";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(53, 282);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(152, 15);
            this.label8.TabIndex = 27;
            this.label8.Text = "Model Setup and Execution";
            // 
            // ChooseTemplateButton
            // 
            this.ChooseTemplateButton.Location = new System.Drawing.Point(167, 92);
            this.ChooseTemplateButton.Name = "ChooseTemplateButton";
            this.ChooseTemplateButton.Size = new System.Drawing.Size(117, 22);
            this.ChooseTemplateButton.TabIndex = 29;
            this.ChooseTemplateButton.Text = "Choose Template";
            this.ChooseTemplateButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ChooseTemplateButton.UseVisualStyleBackColor = true;
            this.ChooseTemplateButton.Click += new System.EventHandler(this.Choose_from_Template_Click);
            // 
            // HelpButton2
            // 
            this.HelpButton2.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton2.Image")));
            this.HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton2.Location = new System.Drawing.Point(1045, 8);
            this.HelpButton2.Name = "HelpButton2";
            this.HelpButton2.Size = new System.Drawing.Size(78, 28);
            this.HelpButton2.TabIndex = 30;
            this.HelpButton2.Text = "   Help";
            this.HelpButton2.UseVisualStyleBackColor = true;
            this.HelpButton2.Click += new System.EventHandler(this.HelpButton2_Click);
            // 
            // TogglePanel
            // 
            this.TogglePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TogglePanel.Controls.Add(this.GraphButton);
            this.TogglePanel.Controls.Add(this.MapButton2);
            this.TogglePanel.Controls.Add(this.ConsoleButton);
            this.TogglePanel.Location = new System.Drawing.Point(305, 539);
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
            // browseButton
            // 
            this.browseButton.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.browseButton.ForeColor = System.Drawing.Color.Black;
            this.browseButton.Location = new System.Drawing.Point(506, 8);
            this.browseButton.Margin = new System.Windows.Forms.Padding(0);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(56, 25);
            this.browseButton.TabIndex = 37;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browserButton_Click);
            // 
            // PlotPanel
            // 
            this.PlotPanel.Controls.Add(this.ShowBoundBox);
            this.PlotPanel.Controls.Add(this.outputjump);
            this.PlotPanel.Controls.Add(this.PlotButton);
            this.PlotPanel.Location = new System.Drawing.Point(305, 52);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Size = new System.Drawing.Size(497, 23);
            this.PlotPanel.TabIndex = 38;
            // 
            // ShowBoundBox
            // 
            this.ShowBoundBox.AutoSize = true;
            this.ShowBoundBox.Checked = true;
            this.ShowBoundBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowBoundBox.Location = new System.Drawing.Point(252, 3);
            this.ShowBoundBox.Name = "ShowBoundBox";
            this.ShowBoundBox.Size = new System.Drawing.Size(170, 19);
            this.ShowBoundBox.TabIndex = 41;
            this.ShowBoundBox.Text = "Show Boundary Conditions";
            this.ShowBoundBox.UseVisualStyleBackColor = true;
            this.ShowBoundBox.CheckedChanged += new System.EventHandler(this.ShowBoundBox_CheckedChanged);
            // 
            // outputjump
            // 
            this.outputjump.AutoSize = true;
            this.outputjump.Checked = true;
            this.outputjump.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputjump.Enabled = false;
            this.outputjump.Location = new System.Drawing.Point(114, 3);
            this.outputjump.Name = "outputjump";
            this.outputjump.Size = new System.Drawing.Size(112, 19);
            this.outputjump.TabIndex = 40;
            this.outputjump.Text = "Click to Outputs";
            this.outputjump.UseVisualStyleBackColor = true;
            this.outputjump.CheckedChanged += new System.EventHandler(this.showCOMIDsBox_CheckedChanged);
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
            this.progressBar1.Location = new System.Drawing.Point(27, 419);
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
            this.infolabel1.Location = new System.Drawing.Point(619, 541);
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
            this.infolabel2.Location = new System.Drawing.Point(619, 560);
            this.infolabel2.Name = "infolabel2";
            this.infolabel2.Size = new System.Drawing.Size(244, 15);
            this.infolabel2.TabIndex = 40;
            this.infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom.";
            this.infolabel2.Visible = false;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(1045, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 42;
            this.button2.Text = "Step 0";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(306, 80);
            this.webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(818, 454);
            this.webView.TabIndex = 43;
            this.webView.ZoomFactor = 1D;
            // 
            // RecentLabel
            // 
            this.RecentLabel.AutoSize = true;
            this.RecentLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RecentLabel.Location = new System.Drawing.Point(699, 14);
            this.RecentLabel.Name = "RecentLabel";
            this.RecentLabel.Size = new System.Drawing.Size(95, 15);
            this.RecentLabel.TabIndex = 44;
            this.RecentLabel.Text = "Recent Projects";
            // 
            // RecentFilesBox
            // 
            this.RecentFilesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RecentFilesBox.FormattingEnabled = true;
            this.RecentFilesBox.Location = new System.Drawing.Point(794, 9);
            this.RecentFilesBox.Name = "RecentFilesBox";
            this.RecentFilesBox.Size = new System.Drawing.Size(201, 23);
            this.RecentFilesBox.TabIndex = 45;
            this.RecentFilesBox.SelectionChangeCommitted += new System.EventHandler(this.RecentFilesBox_SelectionChangeCommitted);
            // 
            // NewProject
            // 
            this.NewProject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NewProject.ForeColor = System.Drawing.Color.Black;
            this.NewProject.Location = new System.Drawing.Point(582, 7);
            this.NewProject.Margin = new System.Windows.Forms.Padding(0);
            this.NewProject.Name = "NewProject";
            this.NewProject.Size = new System.Drawing.Size(86, 25);
            this.NewProject.TabIndex = 46;
            this.NewProject.Text = "New Project";
            this.NewProject.UseVisualStyleBackColor = true;
            this.NewProject.Click += new System.EventHandler(this.NewProject_Click);
            // 
            // Separator
            // 
            this.Separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Separator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Separator.Enabled = false;
            this.Separator.Location = new System.Drawing.Point(1, 41);
            this.Separator.Name = "Separator";
            this.Separator.Size = new System.Drawing.Size(1136, 2);
            this.Separator.TabIndex = 47;
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Location = new System.Drawing.Point(54, 472);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(126, 15);
            this.OutputLabel.TabIndex = 48;
            this.OutputLabel.Text = "View Network Outputs";
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 586);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.Separator);
            this.Controls.Add(this.NewProject);
            this.Controls.Add(this.RecentFilesBox);
            this.Controls.Add(this.RecentLabel);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.infolabel2);
            this.Controls.Add(this.infolabel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.PlotPanel);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.TogglePanel);
            this.Controls.Add(this.HelpButton2);
            this.Controls.Add(this.ChooseTemplateButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.OutputPanel);
            this.Controls.Add(this.ReadNetworkPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BaseJSONBox);
            this.Controls.Add(this.ProcessLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.basedirBox);
            this.Controls.Add(this.viewOutputButton);
            this.MinimumSize = new System.Drawing.Size(1077, 553);
            this.Name = "MultiSegForm";
            this.Text = "MultiSegForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiSegForm_FormClosing);
            this.Shown += new System.EventHandler(this.MultiSegForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.MultiSegForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MultiSegForm_Resize);
            this.panel1.ResumeLayout(false);
            this.ReadNetworkPanel.ResumeLayout(false);
            this.ReadNetworkPanel.PerformLayout();
            this.OutputPanel.ResumeLayout(false);
            this.OutputPanel.PerformLayout();
            this.TogglePanel.ResumeLayout(false);
            this.TogglePanel.PerformLayout();
            this.PlotPanel.ResumeLayout(false);
            this.PlotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox basedirBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ProcessLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox BaseJSONBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button FlowsButton;
        private System.Windows.Forms.Button SetupButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Panel ReadNetworkPanel;
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
        private System.Windows.Forms.Button ChooseTemplateButton;
        private System.Windows.Forms.Panel OutputPanel;
        private System.Windows.Forms.Button HelpButton2;
        private System.Windows.Forms.Panel TogglePanel;
        private System.Windows.Forms.RadioButton MapButton2;
        private System.Windows.Forms.RadioButton ConsoleButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Panel PlotPanel;
        private System.Windows.Forms.CheckBox showCOMIDsBox;
        private System.Windows.Forms.RadioButton flowchartButton;
        private System.Windows.Forms.Button PlotButton;
        private System.Windows.Forms.CheckBox outputjump;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton GraphButton;
        private System.Windows.Forms.Label infolabel1;
        private System.Windows.Forms.Label infolabel2;
        private System.Windows.Forms.Button button2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Label RecentLabel;
        private System.Windows.Forms.ComboBox RecentFilesBox;
        private System.Windows.Forms.CheckBox ShowBoundBox;
        private System.Windows.Forms.Button NewProject;
        private System.Windows.Forms.Label Separator;
        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.Button viewOutputButton;
        private System.Windows.Forms.Button CancelButton;
    }
}