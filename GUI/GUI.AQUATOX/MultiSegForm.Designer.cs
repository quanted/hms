﻿namespace GUI.AQUATOX
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
            basedirBox = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            ProcessLog = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            BaseJSONBox = new System.Windows.Forms.TextBox();
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            button3 = new System.Windows.Forms.Button();
            HAWQS_button = new System.Windows.Forms.Button();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            CancelButton = new System.Windows.Forms.Button();
            StatusLabel = new System.Windows.Forms.Label();
            FlowsButton = new System.Windows.Forms.Button();
            SetupButton = new System.Windows.Forms.Button();
            executeButton = new System.Windows.Forms.Button();
            CreateButton = new System.Windows.Forms.Button();
            proglabel = new System.Windows.Forms.Label();
            ReadNetworkPanel = new System.Windows.Forms.Panel();
            SILabel3 = new System.Windows.Forms.Label();
            SILabel2 = new System.Windows.Forms.Label();
            SILabel1 = new System.Windows.Forms.Label();
            OutputPanel = new System.Windows.Forms.Panel();
            chartButton = new System.Windows.Forms.Button();
            SVBox = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            CSVButton = new System.Windows.Forms.Button();
            viewOutputButton = new System.Windows.Forms.Button();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            ChooseTemplateButton = new System.Windows.Forms.Button();
            HelpButton2 = new System.Windows.Forms.Button();
            TogglePanel = new System.Windows.Forms.Panel();
            GraphButton = new System.Windows.Forms.RadioButton();
            MapButton2 = new System.Windows.Forms.RadioButton();
            ConsoleButton = new System.Windows.Forms.RadioButton();
            browseButton = new System.Windows.Forms.Button();
            PlotPanel = new System.Windows.Forms.Panel();
            ShowH14Box = new System.Windows.Forms.CheckBox();
            NRCheckBox = new System.Windows.Forms.CheckBox();
            LabelCheckBox = new System.Windows.Forms.CheckBox();
            ShowBoundBox = new System.Windows.Forms.CheckBox();
            outputjump = new System.Windows.Forms.CheckBox();
            PlotButton = new System.Windows.Forms.Button();
            infolabel1 = new System.Windows.Forms.Label();
            infolabel2 = new System.Windows.Forms.Label();
            TestOrderButton = new System.Windows.Forms.Button();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            RecentLabel = new System.Windows.Forms.Label();
            RecentFilesBox = new System.Windows.Forms.ComboBox();
            NewProject = new System.Windows.Forms.Button();
            Separator = new System.Windows.Forms.Label();
            OutputLabel = new System.Windows.Forms.Label();
            BrowseJSON = new System.Windows.Forms.Button();
            toggleLog = new System.Windows.Forms.Button();
            GraphLabel = new System.Windows.Forms.Label();
            resetZoom = new System.Windows.Forms.Button();
            LogPanel = new System.Windows.Forms.Panel();
            LogChange = new System.Windows.Forms.Button();
            WarningsBox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            InfoBox = new System.Windows.Forms.CheckBox();
            InputsBox = new System.Windows.Forms.CheckBox();
            ErrorsBox = new System.Windows.Forms.CheckBox();
            logfilen = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ReadNetworkPanel.SuspendLayout();
            OutputPanel.SuspendLayout();
            TogglePanel.SuspendLayout();
            PlotPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            LogPanel.SuspendLayout();
            SuspendLayout();
            // 
            // basedirBox
            // 
            basedirBox.Location = new System.Drawing.Point(79, 9);
            basedirBox.Name = "basedirBox";
            basedirBox.Size = new System.Drawing.Size(427, 23);
            basedirBox.TabIndex = 2;
            basedirBox.Tag = "";
            basedirBox.Text = "..\\..\\..\\2D_Inputs\\TestDir1\\";
            basedirBox.KeyDown += basedirBox_KeyDown;
            basedirBox.Leave += basedirBox_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label2.Location = new System.Drawing.Point(10, 12);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(67, 15);
            label2.TabIndex = 4;
            label2.Text = "Project Dir";
            // 
            // ProcessLog
            // 
            ProcessLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ProcessLog.Location = new System.Drawing.Point(306, 92);
            ProcessLog.Multiline = true;
            ProcessLog.Name = "ProcessLog";
            ProcessLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            ProcessLog.Size = new System.Drawing.Size(822, 454);
            ProcessLog.TabIndex = 6;
            ProcessLog.WordWrap = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(14, 72);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(62, 15);
            label5.TabIndex = 17;
            label5.Text = "Base JSON";
            // 
            // BaseJSONBox
            // 
            BaseJSONBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            BaseJSONBox.Location = new System.Drawing.Point(83, 69);
            BaseJSONBox.Name = "BaseJSONBox";
            BaseJSONBox.Size = new System.Drawing.Size(200, 23);
            BaseJSONBox.TabIndex = 16;
            BaseJSONBox.Tag = "";
            BaseJSONBox.Text = "..\\..\\..\\2D_Inputs\\LBR Glenwood 4.JSON";
            BaseJSONBox.Enter += BaseJSONBox_Enter;
            BaseJSONBox.Leave += BaseJSONBox_Leave;
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(CancelButton);
            panel1.Controls.Add(StatusLabel);
            panel1.Controls.Add(FlowsButton);
            panel1.Controls.Add(SetupButton);
            panel1.Controls.Add(executeButton);
            panel1.Controls.Add(CreateButton);
            panel1.Controls.Add(proglabel);
            panel1.Location = new System.Drawing.Point(24, 291);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(259, 187);
            panel1.TabIndex = 23;
            // 
            // panel2
            // 
            panel2.Controls.Add(button3);
            panel2.Controls.Add(HAWQS_button);
            panel2.Location = new System.Drawing.Point(29, 38);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(187, 77);
            panel2.TabIndex = 52;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(29, 42);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(127, 23);
            button3.TabIndex = 13;
            button3.Text = "Read HAWQS data";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // HAWQS_button
            // 
            HAWQS_button.Location = new System.Drawing.Point(29, 13);
            HAWQS_button.Name = "HAWQS_button";
            HAWQS_button.Size = new System.Drawing.Size(127, 23);
            HAWQS_button.TabIndex = 12;
            HAWQS_button.Text = "Run HAWQS";
            HAWQS_button.UseVisualStyleBackColor = true;
            HAWQS_button.Click += HAWQS_Click;
            // 
            // progressBar1
            // 
            progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            progressBar1.Location = new System.Drawing.Point(2, 135);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(252, 21);
            progressBar1.Step = 1;
            progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 27;
            progressBar1.Value = 1;
            progressBar1.Visible = false;
            // 
            // CancelButton
            // 
            CancelButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            CancelButton.ForeColor = System.Drawing.Color.Black;
            CancelButton.Location = new System.Drawing.Point(87, 160);
            CancelButton.Margin = new System.Windows.Forms.Padding(0);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new System.Drawing.Size(68, 23);
            CancelButton.TabIndex = 50;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Visible = false;
            CancelButton.Click += CancelButton_Click;
            // 
            // StatusLabel
            // 
            StatusLabel.Location = new System.Drawing.Point(11, 135);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new System.Drawing.Size(235, 15);
            StatusLabel.TabIndex = 26;
            StatusLabel.Text = "Model Status Label";
            StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FlowsButton
            // 
            FlowsButton.Location = new System.Drawing.Point(58, 65);
            FlowsButton.Name = "FlowsButton";
            FlowsButton.Size = new System.Drawing.Size(127, 23);
            FlowsButton.TabIndex = 23;
            FlowsButton.Text = "Overland Flows";
            FlowsButton.UseVisualStyleBackColor = true;
            FlowsButton.Click += OverlandFlow_Click;
            // 
            // SetupButton
            // 
            SetupButton.Location = new System.Drawing.Point(58, 11);
            SetupButton.Name = "SetupButton";
            SetupButton.Size = new System.Drawing.Size(127, 23);
            SetupButton.TabIndex = 21;
            SetupButton.Text = "Master Setup";
            SetupButton.UseVisualStyleBackColor = true;
            SetupButton.Click += SetupButton_Click;
            // 
            // executeButton
            // 
            executeButton.Location = new System.Drawing.Point(58, 92);
            executeButton.Name = "executeButton";
            executeButton.Size = new System.Drawing.Size(127, 23);
            executeButton.TabIndex = 9;
            executeButton.Text = "Execute Network";
            executeButton.UseVisualStyleBackColor = true;
            executeButton.Click += executeButton_Click;
            // 
            // CreateButton
            // 
            CreateButton.Location = new System.Drawing.Point(57, 38);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new System.Drawing.Size(127, 23);
            CreateButton.TabIndex = 8;
            CreateButton.Text = "Create Linked Inputs";
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += createButton_Click;
            // 
            // proglabel
            // 
            proglabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            proglabel.Location = new System.Drawing.Point(10, 118);
            proglabel.Name = "proglabel";
            proglabel.Size = new System.Drawing.Size(235, 15);
            proglabel.TabIndex = 51;
            proglabel.Text = "Model Progress Label";
            proglabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            proglabel.Visible = false;
            // 
            // ReadNetworkPanel
            // 
            ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ReadNetworkPanel.Controls.Add(SILabel3);
            ReadNetworkPanel.Controls.Add(SILabel2);
            ReadNetworkPanel.Controls.Add(SILabel1);
            ReadNetworkPanel.Location = new System.Drawing.Point(24, 133);
            ReadNetworkPanel.Name = "ReadNetworkPanel";
            ReadNetworkPanel.Size = new System.Drawing.Size(259, 143);
            ReadNetworkPanel.TabIndex = 24;
            // 
            // SILabel3
            // 
            SILabel3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            SILabel3.Location = new System.Drawing.Point(10, 81);
            SILabel3.Name = "SILabel3";
            SILabel3.Size = new System.Drawing.Size(235, 48);
            SILabel3.TabIndex = 56;
            SILabel3.Text = "XX segments including XX lake/reservoir segments";
            // 
            // SILabel2
            // 
            SILabel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            SILabel2.Location = new System.Drawing.Point(11, 41);
            SILabel2.Name = "SILabel2";
            SILabel2.Size = new System.Drawing.Size(235, 40);
            SILabel2.TabIndex = 55;
            SILabel2.Text = "Pour-Point COMID: 123457890 with a XX km up-network reach";
            // 
            // SILabel1
            // 
            SILabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            SILabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            SILabel1.Location = new System.Drawing.Point(10, 16);
            SILabel1.Name = "SILabel1";
            SILabel1.Size = new System.Drawing.Size(235, 19);
            SILabel1.TabIndex = 54;
            SILabel1.Text = "Stream Network Simulation";
            // 
            // OutputPanel
            // 
            OutputPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            OutputPanel.Controls.Add(chartButton);
            OutputPanel.Controls.Add(SVBox);
            OutputPanel.Controls.Add(label3);
            OutputPanel.Controls.Add(CSVButton);
            OutputPanel.Enabled = false;
            OutputPanel.Location = new System.Drawing.Point(24, 490);
            OutputPanel.Name = "OutputPanel";
            OutputPanel.Size = new System.Drawing.Size(259, 81);
            OutputPanel.TabIndex = 25;
            // 
            // chartButton
            // 
            chartButton.Location = new System.Drawing.Point(165, 46);
            chartButton.Name = "chartButton";
            chartButton.Size = new System.Drawing.Size(75, 23);
            chartButton.TabIndex = 23;
            chartButton.Text = "Graph";
            chartButton.UseVisualStyleBackColor = true;
            chartButton.Click += ChartButtonClick;
            // 
            // SVBox
            // 
            SVBox.FormattingEnabled = true;
            SVBox.Location = new System.Drawing.Point(63, 17);
            SVBox.Name = "SVBox";
            SVBox.Size = new System.Drawing.Size(178, 23);
            SVBox.TabIndex = 22;
            SVBox.SelectionChangeCommitted += SVBox_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 20);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(52, 15);
            label3.TabIndex = 21;
            label3.Text = "SV Index";
            // 
            // CSVButton
            // 
            CSVButton.Location = new System.Drawing.Point(81, 46);
            CSVButton.Name = "CSVButton";
            CSVButton.Size = new System.Drawing.Size(75, 23);
            CSVButton.TabIndex = 20;
            CSVButton.Text = "CSV";
            CSVButton.UseVisualStyleBackColor = true;
            CSVButton.Click += CSVButton_Click;
            // 
            // viewOutputButton
            // 
            viewOutputButton.Location = new System.Drawing.Point(83, 489);
            viewOutputButton.Name = "viewOutputButton";
            viewOutputButton.Size = new System.Drawing.Size(127, 23);
            viewOutputButton.TabIndex = 49;
            viewOutputButton.Text = "View Outputs";
            viewOutputButton.UseVisualStyleBackColor = true;
            viewOutputButton.Click += viewOutputButton_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(53, 125);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(108, 15);
            label7.TabIndex = 26;
            label7.Text = "Spatial Information";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(53, 282);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(152, 15);
            label8.TabIndex = 27;
            label8.Text = "Model Setup and Execution";
            // 
            // ChooseTemplateButton
            // 
            ChooseTemplateButton.Location = new System.Drawing.Point(79, 92);
            ChooseTemplateButton.Name = "ChooseTemplateButton";
            ChooseTemplateButton.Size = new System.Drawing.Size(117, 22);
            ChooseTemplateButton.TabIndex = 29;
            ChooseTemplateButton.Text = "Choose Template";
            ChooseTemplateButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            ChooseTemplateButton.UseVisualStyleBackColor = true;
            ChooseTemplateButton.Click += Choose_from_Template_Click;
            // 
            // HelpButton2
            // 
            HelpButton2.Image = (System.Drawing.Image)resources.GetObject("HelpButton2.Image");
            HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton2.Location = new System.Drawing.Point(1045, 8);
            HelpButton2.Name = "HelpButton2";
            HelpButton2.Size = new System.Drawing.Size(78, 28);
            HelpButton2.TabIndex = 30;
            HelpButton2.Text = "   Help";
            HelpButton2.UseVisualStyleBackColor = true;
            HelpButton2.Click += HelpButton2_Click;
            // 
            // TogglePanel
            // 
            TogglePanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            TogglePanel.Controls.Add(GraphButton);
            TogglePanel.Controls.Add(MapButton2);
            TogglePanel.Controls.Add(ConsoleButton);
            TogglePanel.Location = new System.Drawing.Point(305, 551);
            TogglePanel.Name = "TogglePanel";
            TogglePanel.Size = new System.Drawing.Size(291, 23);
            TogglePanel.TabIndex = 36;
            // 
            // GraphButton
            // 
            GraphButton.AutoSize = true;
            GraphButton.Location = new System.Drawing.Point(186, 2);
            GraphButton.Name = "GraphButton";
            GraphButton.Size = new System.Drawing.Size(89, 19);
            GraphButton.TabIndex = 2;
            GraphButton.Text = "Show Graph";
            GraphButton.UseVisualStyleBackColor = true;
            GraphButton.CheckedChanged += ConsoleButton_CheckedChanged;
            // 
            // MapButton2
            // 
            MapButton2.AutoSize = true;
            MapButton2.Location = new System.Drawing.Point(95, 3);
            MapButton2.Name = "MapButton2";
            MapButton2.Size = new System.Drawing.Size(81, 19);
            MapButton2.TabIndex = 1;
            MapButton2.Text = "Show Map";
            MapButton2.UseVisualStyleBackColor = true;
            MapButton2.CheckedChanged += ConsoleButton_CheckedChanged;
            // 
            // ConsoleButton
            // 
            ConsoleButton.AutoSize = true;
            ConsoleButton.Checked = true;
            ConsoleButton.Location = new System.Drawing.Point(3, 3);
            ConsoleButton.Name = "ConsoleButton";
            ConsoleButton.Size = new System.Drawing.Size(77, 19);
            ConsoleButton.TabIndex = 0;
            ConsoleButton.TabStop = true;
            ConsoleButton.Text = "Show Log";
            ConsoleButton.UseVisualStyleBackColor = true;
            ConsoleButton.CheckedChanged += ConsoleButton_CheckedChanged;
            // 
            // browseButton
            // 
            browseButton.Font = new System.Drawing.Font("Arial Narrow", 9F);
            browseButton.ForeColor = System.Drawing.Color.Black;
            browseButton.Location = new System.Drawing.Point(506, 8);
            browseButton.Margin = new System.Windows.Forms.Padding(0);
            browseButton.Name = "browseButton";
            browseButton.Size = new System.Drawing.Size(56, 25);
            browseButton.TabIndex = 37;
            browseButton.Text = "Browse";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += browserButton_Click;
            // 
            // PlotPanel
            // 
            PlotPanel.Controls.Add(ShowH14Box);
            PlotPanel.Controls.Add(NRCheckBox);
            PlotPanel.Controls.Add(LabelCheckBox);
            PlotPanel.Controls.Add(ShowBoundBox);
            PlotPanel.Controls.Add(outputjump);
            PlotPanel.Controls.Add(PlotButton);
            PlotPanel.Location = new System.Drawing.Point(305, 49);
            PlotPanel.Name = "PlotPanel";
            PlotPanel.Size = new System.Drawing.Size(814, 23);
            PlotPanel.TabIndex = 38;
            // 
            // ShowH14Box
            // 
            ShowH14Box.AutoSize = true;
            ShowH14Box.Location = new System.Drawing.Point(252, 3);
            ShowH14Box.Name = "ShowH14Box";
            ShowH14Box.Size = new System.Drawing.Size(100, 19);
            ShowH14Box.TabIndex = 66;
            ShowH14Box.Text = "Show HUC14s";
            ShowH14Box.UseVisualStyleBackColor = true;
            ShowH14Box.CheckedChanged += ShowH14Box_CheckedChanged;
            // 
            // NRCheckBox
            // 
            NRCheckBox.AutoSize = true;
            NRCheckBox.Location = new System.Drawing.Point(623, 2);
            NRCheckBox.Name = "NRCheckBox";
            NRCheckBox.Size = new System.Drawing.Size(162, 19);
            NRCheckBox.TabIndex = 43;
            NRCheckBox.Text = "Show Non-Run Segments";
            NRCheckBox.UseVisualStyleBackColor = true;
            NRCheckBox.CheckedChanged += NRCheckBox_CheckedChanged;
            // 
            // LabelCheckBox
            // 
            LabelCheckBox.AutoSize = true;
            LabelCheckBox.Location = new System.Drawing.Point(556, 3);
            LabelCheckBox.Name = "LabelCheckBox";
            LabelCheckBox.Size = new System.Drawing.Size(59, 19);
            LabelCheckBox.TabIndex = 42;
            LabelCheckBox.Text = "Labels";
            LabelCheckBox.UseVisualStyleBackColor = true;
            LabelCheckBox.CheckedChanged += LabelCheckBox_CheckedChanged;
            // 
            // ShowBoundBox
            // 
            ShowBoundBox.AutoSize = true;
            ShowBoundBox.Checked = true;
            ShowBoundBox.CheckState = System.Windows.Forms.CheckState.Checked;
            ShowBoundBox.Location = new System.Drawing.Point(379, 3);
            ShowBoundBox.Name = "ShowBoundBox";
            ShowBoundBox.Size = new System.Drawing.Size(170, 19);
            ShowBoundBox.TabIndex = 41;
            ShowBoundBox.Text = "Show Boundary Conditions";
            ShowBoundBox.UseVisualStyleBackColor = true;
            ShowBoundBox.CheckedChanged += ShowBoundBox_CheckedChanged;
            // 
            // outputjump
            // 
            outputjump.AutoSize = true;
            outputjump.Checked = true;
            outputjump.CheckState = System.Windows.Forms.CheckState.Checked;
            outputjump.Enabled = false;
            outputjump.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            outputjump.Location = new System.Drawing.Point(120, 3);
            outputjump.Name = "outputjump";
            outputjump.Size = new System.Drawing.Size(115, 19);
            outputjump.TabIndex = 40;
            outputjump.Text = "Click to Outputs";
            outputjump.UseVisualStyleBackColor = true;
            outputjump.CheckedChanged += showCOMIDsBox_CheckedChanged;
            // 
            // PlotButton
            // 
            PlotButton.Location = new System.Drawing.Point(3, 0);
            PlotButton.Name = "PlotButton";
            PlotButton.Size = new System.Drawing.Size(99, 23);
            PlotButton.TabIndex = 36;
            PlotButton.Text = "Reset Map";
            PlotButton.UseVisualStyleBackColor = true;
            PlotButton.Click += PlotButton_Click;
            // 
            // infolabel1
            // 
            infolabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel1.AutoSize = true;
            infolabel1.Location = new System.Drawing.Point(619, 553);
            infolabel1.Name = "infolabel1";
            infolabel1.Size = new System.Drawing.Size(361, 15);
            infolabel1.TabIndex = 39;
            infolabel1.Text = "Click on a COMID to view/edit model inputs (or outputs if relevant)";
            infolabel1.Visible = false;
            // 
            // infolabel2
            // 
            infolabel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel2.AutoSize = true;
            infolabel2.Location = new System.Drawing.Point(619, 572);
            infolabel2.Name = "infolabel2";
            infolabel2.Size = new System.Drawing.Size(244, 15);
            infolabel2.TabIndex = 40;
            infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom.";
            infolabel2.Visible = false;
            // 
            // TestOrderButton
            // 
            TestOrderButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            TestOrderButton.Location = new System.Drawing.Point(1096, 51);
            TestOrderButton.Name = "TestOrderButton";
            TestOrderButton.Size = new System.Drawing.Size(28, 23);
            TestOrderButton.TabIndex = 42;
            TestOrderButton.Text = "Step 0";
            TestOrderButton.UseVisualStyleBackColor = true;
            TestOrderButton.Visible = false;
            TestOrderButton.Click += TestOrderButtonClick;
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Location = new System.Drawing.Point(306, 93);
            webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(822, 453);
            webView.TabIndex = 43;
            webView.ZoomFactor = 1D;
            // 
            // RecentLabel
            // 
            RecentLabel.AutoSize = true;
            RecentLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            RecentLabel.Location = new System.Drawing.Point(699, 14);
            RecentLabel.Name = "RecentLabel";
            RecentLabel.Size = new System.Drawing.Size(95, 15);
            RecentLabel.TabIndex = 44;
            RecentLabel.Text = "Recent Projects";
            // 
            // RecentFilesBox
            // 
            RecentFilesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            RecentFilesBox.FormattingEnabled = true;
            RecentFilesBox.Location = new System.Drawing.Point(794, 9);
            RecentFilesBox.Name = "RecentFilesBox";
            RecentFilesBox.Size = new System.Drawing.Size(201, 23);
            RecentFilesBox.TabIndex = 45;
            RecentFilesBox.SelectionChangeCommitted += RecentFilesBox_SelectionChangeCommitted;
            // 
            // NewProject
            // 
            NewProject.Font = new System.Drawing.Font("Segoe UI", 9F);
            NewProject.ForeColor = System.Drawing.Color.Black;
            NewProject.Location = new System.Drawing.Point(582, 7);
            NewProject.Margin = new System.Windows.Forms.Padding(0);
            NewProject.Name = "NewProject";
            NewProject.Size = new System.Drawing.Size(86, 25);
            NewProject.TabIndex = 46;
            NewProject.Text = "New Project";
            NewProject.UseVisualStyleBackColor = true;
            NewProject.Click += NewProject_Click;
            // 
            // Separator
            // 
            Separator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Separator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            Separator.Enabled = false;
            Separator.Location = new System.Drawing.Point(1, 41);
            Separator.Name = "Separator";
            Separator.Size = new System.Drawing.Size(1140, 2);
            Separator.TabIndex = 47;
            // 
            // OutputLabel
            // 
            OutputLabel.AutoSize = true;
            OutputLabel.Location = new System.Drawing.Point(54, 481);
            OutputLabel.Name = "OutputLabel";
            OutputLabel.Size = new System.Drawing.Size(126, 15);
            OutputLabel.TabIndex = 48;
            OutputLabel.Text = "View Network Outputs";
            // 
            // BrowseJSON
            // 
            BrowseJSON.Font = new System.Drawing.Font("Arial Narrow", 9F);
            BrowseJSON.ForeColor = System.Drawing.Color.Black;
            BrowseJSON.Location = new System.Drawing.Point(227, 90);
            BrowseJSON.Margin = new System.Windows.Forms.Padding(0);
            BrowseJSON.Name = "BrowseJSON";
            BrowseJSON.Size = new System.Drawing.Size(56, 25);
            BrowseJSON.TabIndex = 50;
            BrowseJSON.Text = "Browse";
            BrowseJSON.UseVisualStyleBackColor = true;
            BrowseJSON.Click += BrowseJSON_Click;
            // 
            // toggleLog
            // 
            toggleLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            toggleLog.Cursor = System.Windows.Forms.Cursors.SizeAll;
            toggleLog.Location = new System.Drawing.Point(1013, 554);
            toggleLog.Name = "toggleLog";
            toggleLog.Size = new System.Drawing.Size(114, 24);
            toggleLog.TabIndex = 51;
            toggleLog.Text = "Toggle Log Scale";
            toggleLog.UseVisualStyleBackColor = true;
            toggleLog.Click += toggleLog_Click;
            // 
            // GraphLabel
            // 
            GraphLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            GraphLabel.Location = new System.Drawing.Point(619, 555);
            GraphLabel.Name = "GraphLabel";
            GraphLabel.Size = new System.Drawing.Size(262, 32);
            GraphLabel.TabIndex = 53;
            GraphLabel.Text = "Draw a box to zoom or \"right click\" on a point to get its name and value.";
            // 
            // resetZoom
            // 
            resetZoom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            resetZoom.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resetZoom.Location = new System.Drawing.Point(912, 554);
            resetZoom.Name = "resetZoom";
            resetZoom.Size = new System.Drawing.Size(95, 24);
            resetZoom.TabIndex = 52;
            resetZoom.Text = "Reset Zoom";
            resetZoom.UseVisualStyleBackColor = true;
            resetZoom.Click += resetZoom_Click;
            // 
            // LogPanel
            // 
            LogPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            LogPanel.Controls.Add(LogChange);
            LogPanel.Controls.Add(WarningsBox);
            LogPanel.Controls.Add(label1);
            LogPanel.Controls.Add(InfoBox);
            LogPanel.Controls.Add(InputsBox);
            LogPanel.Controls.Add(ErrorsBox);
            LogPanel.Location = new System.Drawing.Point(607, 551);
            LogPanel.Name = "LogPanel";
            LogPanel.Size = new System.Drawing.Size(521, 40);
            LogPanel.TabIndex = 54;
            LogPanel.Visible = false;
            // 
            // LogChange
            // 
            LogChange.Font = new System.Drawing.Font("Arial", 9F);
            LogChange.ForeColor = System.Drawing.Color.Black;
            LogChange.Location = new System.Drawing.Point(381, 5);
            LogChange.Margin = new System.Windows.Forms.Padding(0);
            LogChange.Name = "LogChange";
            LogChange.Size = new System.Drawing.Size(95, 25);
            LogChange.TabIndex = 57;
            LogChange.Text = "Change Log";
            LogChange.UseVisualStyleBackColor = true;
            LogChange.Click += LogChange_Click;
            // 
            // WarningsBox
            // 
            WarningsBox.AutoSize = true;
            WarningsBox.Checked = true;
            WarningsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            WarningsBox.Location = new System.Drawing.Point(169, 11);
            WarningsBox.Name = "WarningsBox";
            WarningsBox.Size = new System.Drawing.Size(76, 19);
            WarningsBox.TabIndex = 56;
            WarningsBox.Text = "Warnings";
            WarningsBox.UseVisualStyleBackColor = true;
            WarningsBox.CheckedChanged += LogOptions_CheckChanged;
            // 
            // label1
            // 
            label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(60, 12);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 15);
            label1.TabIndex = 55;
            label1.Text = "Show:";
            // 
            // InfoBox
            // 
            InfoBox.AutoSize = true;
            InfoBox.Checked = true;
            InfoBox.CheckState = System.Windows.Forms.CheckState.Checked;
            InfoBox.Location = new System.Drawing.Point(309, 11);
            InfoBox.Name = "InfoBox";
            InfoBox.Size = new System.Drawing.Size(47, 19);
            InfoBox.TabIndex = 44;
            InfoBox.Text = "Info";
            InfoBox.UseVisualStyleBackColor = true;
            InfoBox.CheckedChanged += LogOptions_CheckChanged;
            // 
            // InputsBox
            // 
            InputsBox.AutoSize = true;
            InputsBox.Checked = true;
            InputsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            InputsBox.Location = new System.Drawing.Point(247, 11);
            InputsBox.Name = "InputsBox";
            InputsBox.Size = new System.Drawing.Size(59, 19);
            InputsBox.TabIndex = 43;
            InputsBox.Text = "Inputs";
            InputsBox.UseVisualStyleBackColor = true;
            InputsBox.CheckedChanged += LogOptions_CheckChanged;
            // 
            // ErrorsBox
            // 
            ErrorsBox.AutoSize = true;
            ErrorsBox.Checked = true;
            ErrorsBox.CheckState = System.Windows.Forms.CheckState.Checked;
            ErrorsBox.Location = new System.Drawing.Point(111, 11);
            ErrorsBox.Name = "ErrorsBox";
            ErrorsBox.Size = new System.Drawing.Size(56, 19);
            ErrorsBox.TabIndex = 41;
            ErrorsBox.Text = "Errors";
            ErrorsBox.UseVisualStyleBackColor = true;
            ErrorsBox.CheckedChanged += LogOptions_CheckChanged;
            // 
            // logfilen
            // 
            logfilen.AutoSize = true;
            logfilen.Location = new System.Drawing.Point(313, 77);
            logfilen.Name = "logfilen";
            logfilen.Size = new System.Drawing.Size(83, 15);
            logfilen.TabIndex = 55;
            logfilen.Text = "Log File Name";
            logfilen.Visible = false;
            // 
            // MultiSegForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1140, 598);
            Controls.Add(webView);
            Controls.Add(logfilen);
            Controls.Add(LogPanel);
            Controls.Add(GraphLabel);
            Controls.Add(resetZoom);
            Controls.Add(toggleLog);
            Controls.Add(BrowseJSON);
            Controls.Add(OutputLabel);
            Controls.Add(Separator);
            Controls.Add(NewProject);
            Controls.Add(RecentFilesBox);
            Controls.Add(RecentLabel);
            Controls.Add(TestOrderButton);
            Controls.Add(infolabel2);
            Controls.Add(infolabel1);
            Controls.Add(PlotPanel);
            Controls.Add(browseButton);
            Controls.Add(TogglePanel);
            Controls.Add(HelpButton2);
            Controls.Add(ChooseTemplateButton);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(OutputPanel);
            Controls.Add(ReadNetworkPanel);
            Controls.Add(panel1);
            Controls.Add(label5);
            Controls.Add(BaseJSONBox);
            Controls.Add(ProcessLog);
            Controls.Add(label2);
            Controls.Add(basedirBox);
            Controls.Add(viewOutputButton);
            MinimumSize = new System.Drawing.Size(1077, 553);
            Name = "MultiSegForm";
            Text = "Multiple Segment Runs";
            FormClosing += MultiSegForm_FormClosing;
            Shown += MultiSegForm_Shown;
            ResizeEnd += MultiSegForm_ResizeEnd;
            Resize += MultiSegForm_Resize;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ReadNetworkPanel.ResumeLayout(false);
            OutputPanel.ResumeLayout(false);
            OutputPanel.PerformLayout();
            TogglePanel.ResumeLayout(false);
            TogglePanel.PerformLayout();
            PlotPanel.ResumeLayout(false);
            PlotPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            LogPanel.ResumeLayout(false);
            LogPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button HAWQS_button;
        private System.Windows.Forms.CheckBox ShowH14Box;
    }
}