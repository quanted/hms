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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.CancelButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.FlowsButton = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.CreateButton = new System.Windows.Forms.Button();
            this.proglabel = new System.Windows.Forms.Label();
            this.ReadNetworkPanel = new System.Windows.Forms.Panel();
            this.SILabel3 = new System.Windows.Forms.Label();
            this.SILabel2 = new System.Windows.Forms.Label();
            this.SILabel1 = new System.Windows.Forms.Label();
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
            this.NRCheckBox = new System.Windows.Forms.CheckBox();
            this.LabelCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowBoundBox = new System.Windows.Forms.CheckBox();
            this.outputjump = new System.Windows.Forms.CheckBox();
            this.PlotButton = new System.Windows.Forms.Button();
            this.infolabel1 = new System.Windows.Forms.Label();
            this.infolabel2 = new System.Windows.Forms.Label();
            this.TestOrderButton = new System.Windows.Forms.Button();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.RecentLabel = new System.Windows.Forms.Label();
            this.RecentFilesBox = new System.Windows.Forms.ComboBox();
            this.NewProject = new System.Windows.Forms.Button();
            this.Separator = new System.Windows.Forms.Label();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.BrowseJSON = new System.Windows.Forms.Button();
            this.toggleLog = new System.Windows.Forms.Button();
            this.GraphLabel = new System.Windows.Forms.Label();
            this.resetZoom = new System.Windows.Forms.Button();
            this.LogPanel = new System.Windows.Forms.Panel();
            this.LogChange = new System.Windows.Forms.Button();
            this.WarningsBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.InfoBox = new System.Windows.Forms.CheckBox();
            this.InputsBox = new System.Windows.Forms.CheckBox();
            this.ErrorsBox = new System.Windows.Forms.CheckBox();
            this.logfilen = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.ReadNetworkPanel.SuspendLayout();
            this.OutputPanel.SuspendLayout();
            this.TogglePanel.SuspendLayout();
            this.PlotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.LogPanel.SuspendLayout();
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
            this.ProcessLog.Location = new System.Drawing.Point(306, 90);
            this.ProcessLog.Multiline = true;
            this.ProcessLog.Name = "ProcessLog";
            this.ProcessLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProcessLog.Size = new System.Drawing.Size(817, 456);
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
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.StatusLabel);
            this.panel1.Controls.Add(this.FlowsButton);
            this.panel1.Controls.Add(this.SetupButton);
            this.panel1.Controls.Add(this.executeButton);
            this.panel1.Controls.Add(this.CreateButton);
            this.panel1.Controls.Add(this.proglabel);
            this.panel1.Location = new System.Drawing.Point(24, 291);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 187);
            this.panel1.TabIndex = 23;
            // 
            // progressBar1
            // 
            this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            this.progressBar1.Location = new System.Drawing.Point(2, 135);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(252, 21);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 27;
            this.progressBar1.Value = 1;
            this.progressBar1.Visible = false;
            // 
            // CancelButton
            // 
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CancelButton.ForeColor = System.Drawing.Color.Black;
            this.CancelButton.Location = new System.Drawing.Point(87, 160);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(68, 23);
            this.CancelButton.TabIndex = 50;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Visible = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(11, 135);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(235, 15);
            this.StatusLabel.TabIndex = 26;
            this.StatusLabel.Text = "Model Status Label";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FlowsButton
            // 
            this.FlowsButton.Location = new System.Drawing.Point(58, 65);
            this.FlowsButton.Name = "FlowsButton";
            this.FlowsButton.Size = new System.Drawing.Size(127, 23);
            this.FlowsButton.TabIndex = 23;
            this.FlowsButton.Text = "Overland Flows";
            this.FlowsButton.UseVisualStyleBackColor = true;
            this.FlowsButton.Click += new System.EventHandler(this.OverlandFlow_Click);
            // 
            // SetupButton
            // 
            this.SetupButton.Location = new System.Drawing.Point(58, 11);
            this.SetupButton.Name = "SetupButton";
            this.SetupButton.Size = new System.Drawing.Size(127, 23);
            this.SetupButton.TabIndex = 21;
            this.SetupButton.Text = "Master Setup";
            this.SetupButton.UseVisualStyleBackColor = true;
            this.SetupButton.Click += new System.EventHandler(this.SetupButton_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(58, 92);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(127, 23);
            this.executeButton.TabIndex = 9;
            this.executeButton.Text = "Execute Network";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // CreateButton
            // 
            this.CreateButton.Location = new System.Drawing.Point(58, 38);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(127, 23);
            this.CreateButton.TabIndex = 8;
            this.CreateButton.Text = "Create Linked Inputs";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // proglabel
            // 
            this.proglabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.proglabel.Location = new System.Drawing.Point(10, 118);
            this.proglabel.Name = "proglabel";
            this.proglabel.Size = new System.Drawing.Size(235, 15);
            this.proglabel.TabIndex = 51;
            this.proglabel.Text = "Model Progress Label";
            this.proglabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.proglabel.Visible = false;
            // 
            // ReadNetworkPanel
            // 
            this.ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReadNetworkPanel.Controls.Add(this.SILabel3);
            this.ReadNetworkPanel.Controls.Add(this.SILabel2);
            this.ReadNetworkPanel.Controls.Add(this.SILabel1);
            this.ReadNetworkPanel.Location = new System.Drawing.Point(24, 133);
            this.ReadNetworkPanel.Name = "ReadNetworkPanel";
            this.ReadNetworkPanel.Size = new System.Drawing.Size(259, 143);
            this.ReadNetworkPanel.TabIndex = 24;
            // 
            // SILabel3
            // 
            this.SILabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SILabel3.Location = new System.Drawing.Point(10, 81);
            this.SILabel3.Name = "SILabel3";
            this.SILabel3.Size = new System.Drawing.Size(235, 48);
            this.SILabel3.TabIndex = 56;
            this.SILabel3.Text = "XX segments including XX lake/reservoir segments";
            // 
            // SILabel2
            // 
            this.SILabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SILabel2.Location = new System.Drawing.Point(11, 41);
            this.SILabel2.Name = "SILabel2";
            this.SILabel2.Size = new System.Drawing.Size(235, 40);
            this.SILabel2.TabIndex = 55;
            this.SILabel2.Text = "Pour-Point COMID: 123457890 with a XX km up-network reach";
            // 
            // SILabel1
            // 
            this.SILabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SILabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SILabel1.Location = new System.Drawing.Point(10, 16);
            this.SILabel1.Name = "SILabel1";
            this.SILabel1.Size = new System.Drawing.Size(235, 19);
            this.SILabel1.TabIndex = 54;
            this.SILabel1.Text = "Stream Network Simulation";
            // 
            // OutputPanel
            // 
            this.OutputPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OutputPanel.Controls.Add(this.chartButton);
            this.OutputPanel.Controls.Add(this.SVBox);
            this.OutputPanel.Controls.Add(this.label3);
            this.OutputPanel.Controls.Add(this.CSVButton);
            this.OutputPanel.Enabled = false;
            this.OutputPanel.Location = new System.Drawing.Point(24, 490);
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
            this.label7.Size = new System.Drawing.Size(108, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "Spatial Information";
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
            this.ChooseTemplateButton.Location = new System.Drawing.Point(79, 92);
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
            this.TogglePanel.Location = new System.Drawing.Point(305, 551);
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
            this.PlotPanel.Controls.Add(this.NRCheckBox);
            this.PlotPanel.Controls.Add(this.LabelCheckBox);
            this.PlotPanel.Controls.Add(this.ShowBoundBox);
            this.PlotPanel.Controls.Add(this.outputjump);
            this.PlotPanel.Controls.Add(this.PlotButton);
            this.PlotPanel.Location = new System.Drawing.Point(305, 52);
            this.PlotPanel.Name = "PlotPanel";
            this.PlotPanel.Size = new System.Drawing.Size(814, 23);
            this.PlotPanel.TabIndex = 38;
            // 
            // NRCheckBox
            // 
            this.NRCheckBox.AutoSize = true;
            this.NRCheckBox.Location = new System.Drawing.Point(623, 2);
            this.NRCheckBox.Name = "NRCheckBox";
            this.NRCheckBox.Size = new System.Drawing.Size(162, 19);
            this.NRCheckBox.TabIndex = 43;
            this.NRCheckBox.Text = "Show Non-Run Segments";
            this.NRCheckBox.UseVisualStyleBackColor = true;
            this.NRCheckBox.CheckedChanged += new System.EventHandler(this.NRCheckBox_CheckedChanged);
            // 
            // LabelCheckBox
            // 
            this.LabelCheckBox.AutoSize = true;
            this.LabelCheckBox.Location = new System.Drawing.Point(543, 3);
            this.LabelCheckBox.Name = "LabelCheckBox";
            this.LabelCheckBox.Size = new System.Drawing.Size(59, 19);
            this.LabelCheckBox.TabIndex = 42;
            this.LabelCheckBox.Text = "Labels";
            this.LabelCheckBox.UseVisualStyleBackColor = true;
            this.LabelCheckBox.CheckedChanged += new System.EventHandler(this.LabelCheckBox_CheckedChanged);
            // 
            // ShowBoundBox
            // 
            this.ShowBoundBox.AutoSize = true;
            this.ShowBoundBox.Checked = true;
            this.ShowBoundBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowBoundBox.Location = new System.Drawing.Point(359, 3);
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
            this.outputjump.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.outputjump.Location = new System.Drawing.Point(120, 3);
            this.outputjump.Name = "outputjump";
            this.outputjump.Size = new System.Drawing.Size(115, 19);
            this.outputjump.TabIndex = 40;
            this.outputjump.Text = "Click to Outputs";
            this.outputjump.UseVisualStyleBackColor = true;
            this.outputjump.CheckedChanged += new System.EventHandler(this.showCOMIDsBox_CheckedChanged);
            // 
            // PlotButton
            // 
            this.PlotButton.Location = new System.Drawing.Point(3, 0);
            this.PlotButton.Name = "PlotButton";
            this.PlotButton.Size = new System.Drawing.Size(99, 23);
            this.PlotButton.TabIndex = 36;
            this.PlotButton.Text = "Reset Map";
            this.PlotButton.UseVisualStyleBackColor = true;
            this.PlotButton.Click += new System.EventHandler(this.PlotButton_Click);
            // 
            // infolabel1
            // 
            this.infolabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel1.AutoSize = true;
            this.infolabel1.Location = new System.Drawing.Point(619, 553);
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
            this.infolabel2.Location = new System.Drawing.Point(619, 572);
            this.infolabel2.Name = "infolabel2";
            this.infolabel2.Size = new System.Drawing.Size(244, 15);
            this.infolabel2.TabIndex = 40;
            this.infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom.";
            this.infolabel2.Visible = false;
            // 
            // TestOrderButton
            // 
            this.TestOrderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TestOrderButton.Location = new System.Drawing.Point(1091, 51);
            this.TestOrderButton.Name = "TestOrderButton";
            this.TestOrderButton.Size = new System.Drawing.Size(28, 23);
            this.TestOrderButton.TabIndex = 42;
            this.TestOrderButton.Text = "Step 0";
            this.TestOrderButton.UseVisualStyleBackColor = true;
            this.TestOrderButton.Visible = false;
            this.TestOrderButton.Click += new System.EventHandler(this.TestOrderButtonClick);
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(306, 81);
            this.webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(817, 466);
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
            this.Separator.Size = new System.Drawing.Size(1135, 2);
            this.Separator.TabIndex = 47;
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Location = new System.Drawing.Point(54, 481);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(126, 15);
            this.OutputLabel.TabIndex = 48;
            this.OutputLabel.Text = "View Network Outputs";
            // 
            // BrowseJSON
            // 
            this.BrowseJSON.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BrowseJSON.ForeColor = System.Drawing.Color.Black;
            this.BrowseJSON.Location = new System.Drawing.Point(227, 90);
            this.BrowseJSON.Margin = new System.Windows.Forms.Padding(0);
            this.BrowseJSON.Name = "BrowseJSON";
            this.BrowseJSON.Size = new System.Drawing.Size(56, 25);
            this.BrowseJSON.TabIndex = 50;
            this.BrowseJSON.Text = "Browse";
            this.BrowseJSON.UseVisualStyleBackColor = true;
            this.BrowseJSON.Click += new System.EventHandler(this.BrowseJSON_Click);
            // 
            // toggleLog
            // 
            this.toggleLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleLog.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.toggleLog.Location = new System.Drawing.Point(1008, 554);
            this.toggleLog.Name = "toggleLog";
            this.toggleLog.Size = new System.Drawing.Size(114, 24);
            this.toggleLog.TabIndex = 51;
            this.toggleLog.Text = "Toggle Log Scale";
            this.toggleLog.UseVisualStyleBackColor = true;
            this.toggleLog.Click += new System.EventHandler(this.toggleLog_Click);
            // 
            // GraphLabel
            // 
            this.GraphLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GraphLabel.Location = new System.Drawing.Point(619, 555);
            this.GraphLabel.Name = "GraphLabel";
            this.GraphLabel.Size = new System.Drawing.Size(262, 32);
            this.GraphLabel.TabIndex = 53;
            this.GraphLabel.Text = "Draw a box to zoom or \"right click\" on a point to get its name and value.";
            // 
            // resetZoom
            // 
            this.resetZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetZoom.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.resetZoom.Location = new System.Drawing.Point(907, 554);
            this.resetZoom.Name = "resetZoom";
            this.resetZoom.Size = new System.Drawing.Size(95, 24);
            this.resetZoom.TabIndex = 52;
            this.resetZoom.Text = "Reset Zoom";
            this.resetZoom.UseVisualStyleBackColor = true;
            this.resetZoom.Click += new System.EventHandler(this.resetZoom_Click);
            // 
            // LogPanel
            // 
            this.LogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LogPanel.Controls.Add(this.LogChange);
            this.LogPanel.Controls.Add(this.WarningsBox);
            this.LogPanel.Controls.Add(this.label1);
            this.LogPanel.Controls.Add(this.InfoBox);
            this.LogPanel.Controls.Add(this.InputsBox);
            this.LogPanel.Controls.Add(this.ErrorsBox);
            this.LogPanel.Location = new System.Drawing.Point(602, 551);
            this.LogPanel.Name = "LogPanel";
            this.LogPanel.Size = new System.Drawing.Size(521, 40);
            this.LogPanel.TabIndex = 54;
            this.LogPanel.Visible = false;
            // 
            // LogChange
            // 
            this.LogChange.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LogChange.ForeColor = System.Drawing.Color.Black;
            this.LogChange.Location = new System.Drawing.Point(381, 5);
            this.LogChange.Margin = new System.Windows.Forms.Padding(0);
            this.LogChange.Name = "LogChange";
            this.LogChange.Size = new System.Drawing.Size(95, 25);
            this.LogChange.TabIndex = 57;
            this.LogChange.Text = "Change Log";
            this.LogChange.UseVisualStyleBackColor = true;
            this.LogChange.Click += new System.EventHandler(this.LogChange_Click);
            // 
            // WarningsBox
            // 
            this.WarningsBox.AutoSize = true;
            this.WarningsBox.Checked = true;
            this.WarningsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WarningsBox.Location = new System.Drawing.Point(169, 11);
            this.WarningsBox.Name = "WarningsBox";
            this.WarningsBox.Size = new System.Drawing.Size(76, 19);
            this.WarningsBox.TabIndex = 56;
            this.WarningsBox.Text = "Warnings";
            this.WarningsBox.UseVisualStyleBackColor = true;
            this.WarningsBox.CheckedChanged += new System.EventHandler(this.LogOptions_CheckChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(60, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 55;
            this.label1.Text = "Show:";
            // 
            // InfoBox
            // 
            this.InfoBox.AutoSize = true;
            this.InfoBox.Checked = true;
            this.InfoBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.InfoBox.Location = new System.Drawing.Point(309, 11);
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.Size = new System.Drawing.Size(47, 19);
            this.InfoBox.TabIndex = 44;
            this.InfoBox.Text = "Info";
            this.InfoBox.UseVisualStyleBackColor = true;
            this.InfoBox.CheckedChanged += new System.EventHandler(this.LogOptions_CheckChanged);
            // 
            // InputsBox
            // 
            this.InputsBox.AutoSize = true;
            this.InputsBox.Checked = true;
            this.InputsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.InputsBox.Location = new System.Drawing.Point(247, 11);
            this.InputsBox.Name = "InputsBox";
            this.InputsBox.Size = new System.Drawing.Size(59, 19);
            this.InputsBox.TabIndex = 43;
            this.InputsBox.Text = "Inputs";
            this.InputsBox.UseVisualStyleBackColor = true;
            this.InputsBox.CheckedChanged += new System.EventHandler(this.LogOptions_CheckChanged);
            // 
            // ErrorsBox
            // 
            this.ErrorsBox.AutoSize = true;
            this.ErrorsBox.Checked = true;
            this.ErrorsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ErrorsBox.Location = new System.Drawing.Point(111, 11);
            this.ErrorsBox.Name = "ErrorsBox";
            this.ErrorsBox.Size = new System.Drawing.Size(56, 19);
            this.ErrorsBox.TabIndex = 41;
            this.ErrorsBox.Text = "Errors";
            this.ErrorsBox.UseVisualStyleBackColor = true;
            this.ErrorsBox.CheckedChanged += new System.EventHandler(this.LogOptions_CheckChanged);
            // 
            // logfilen
            // 
            this.logfilen.AutoSize = true;
            this.logfilen.Location = new System.Drawing.Point(313, 78);
            this.logfilen.Name = "logfilen";
            this.logfilen.Size = new System.Drawing.Size(83, 15);
            this.logfilen.TabIndex = 55;
            this.logfilen.Text = "Log File Name";
            this.logfilen.Visible = false;
            // 
            // MultiSegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 598);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.logfilen);
            this.Controls.Add(this.LogPanel);
            this.Controls.Add(this.GraphLabel);
            this.Controls.Add(this.resetZoom);
            this.Controls.Add(this.toggleLog);
            this.Controls.Add(this.BrowseJSON);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.Separator);
            this.Controls.Add(this.NewProject);
            this.Controls.Add(this.RecentFilesBox);
            this.Controls.Add(this.RecentLabel);
            this.Controls.Add(this.TestOrderButton);
            this.Controls.Add(this.infolabel2);
            this.Controls.Add(this.infolabel1);
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
            this.Text = "Multiple Segment Runs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiSegForm_FormClosing);
            this.Shown += new System.EventHandler(this.MultiSegForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.MultiSegForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.MultiSegForm_Resize);
            this.panel1.ResumeLayout(false);
            this.ReadNetworkPanel.ResumeLayout(false);
            this.OutputPanel.ResumeLayout(false);
            this.OutputPanel.PerformLayout();
            this.TogglePanel.ResumeLayout(false);
            this.TogglePanel.PerformLayout();
            this.PlotPanel.ResumeLayout(false);
            this.PlotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.LogPanel.ResumeLayout(false);
            this.LogPanel.PerformLayout();
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
        private System.Windows.Forms.Button TestOrderButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Label RecentLabel;
        private System.Windows.Forms.ComboBox RecentFilesBox;
        private System.Windows.Forms.CheckBox ShowBoundBox;
        private System.Windows.Forms.Button NewProject;
        private System.Windows.Forms.Label Separator;
        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.Button viewOutputButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button BrowseJSON;
        private System.Windows.Forms.Button toggleLog;
        private System.Windows.Forms.Label GraphLabel;
        private System.Windows.Forms.Button resetZoom;
        private System.Windows.Forms.CheckBox LabelCheckBox;
        private System.Windows.Forms.CheckBox NRCheckBox;
        private System.Windows.Forms.Label SILabel3;
        private System.Windows.Forms.Label SILabel2;
        private System.Windows.Forms.Label SILabel1;
        private System.Windows.Forms.Panel LogPanel;
        private System.Windows.Forms.CheckBox InfoBox;
        private System.Windows.Forms.CheckBox InputsBox;
        private System.Windows.Forms.CheckBox ErrorsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox WarningsBox;
        private System.Windows.Forms.Button LogChange;
        private System.Windows.Forms.Label logfilen;
        private System.Windows.Forms.Label proglabel;
    }
}