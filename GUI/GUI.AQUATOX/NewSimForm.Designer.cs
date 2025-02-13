namespace GUI.AQUATOX
{
    partial class NewSimForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewSimForm));
            button1 = new System.Windows.Forms.Button();
            SimBaseLabel = new System.Windows.Forms.Label();
            ReadNetworkPanel = new System.Windows.Forms.Panel();
            ReadSNButton = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            spanLabel = new System.Windows.Forms.Label();
            spanBox = new System.Windows.Forms.TextBox();
            endCOMIDLabel = new System.Windows.Forms.Label();
            EndCOMIDBox = new System.Windows.Forms.TextBox();
            comidLabel = new System.Windows.Forms.Label();
            comidBox = new System.Windows.Forms.TextBox();
            ChooseTemplateButton = new System.Windows.Forms.Button();
            HelpButton2 = new System.Windows.Forms.Button();
            infolabel1 = new System.Windows.Forms.Label();
            infolabel2 = new System.Windows.Forms.Label();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            button3 = new System.Windows.Forms.Button();
            SimNameEdit = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SimJSONLabel = new System.Windows.Forms.Label();
            StartDate = new System.Windows.Forms.DateTimePicker();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            EndDate = new System.Windows.Forms.DateTimePicker();
            TogglePanel = new System.Windows.Forms.Panel();
            HUCNetworkButton = new System.Windows.Forms.RadioButton();
            HUCButton = new System.Windows.Forms.RadioButton();
            StreamButton = new System.Windows.Forms.RadioButton();
            LakeButton = new System.Windows.Forms.RadioButton();
            panel2 = new System.Windows.Forms.Panel();
            BrowseJSONButton = new System.Windows.Forms.Button();
            NetworkLabel = new System.Windows.Forms.Label();
            SegLoadLabel = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            SummaryPanel = new System.Windows.Forms.Panel();
            Summary2Label = new System.Windows.Forms.Label();
            Summary1Label = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            ShowH14Box = new System.Windows.Forms.CheckBox();
            BHUC12 = new System.Windows.Forms.RadioButton();
            BHUC14 = new System.Windows.Forms.RadioButton();
            BHUC10 = new System.Windows.Forms.RadioButton();
            BHUC8 = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            HUCSelectionPanel = new System.Windows.Forms.Panel();
            label4 = new System.Windows.Forms.Label();
            readHUCNetworkPanel = new System.Windows.Forms.Panel();
            clear_network = new System.Windows.Forms.Button();
            upperHUCBox = new System.Windows.Forms.TextBox();
            ModelHUC8checkBox = new System.Windows.Forms.CheckBox();
            ReadHUCNetworkButton = new System.Windows.Forms.Button();
            HUCLabel3 = new System.Windows.Forms.Label();
            HUCLabel2 = new System.Windows.Forms.Label();
            traverseHUCBox = new System.Windows.Forms.TextBox();
            HUCLabel1 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            HUCBox = new System.Windows.Forms.TextBox();
            HAWQSHUCLabel = new System.Windows.Forms.Label();
            HAWQSHUCHelp = new System.Windows.Forms.PictureBox();
            ReadNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            TogglePanel.SuspendLayout();
            panel2.SuspendLayout();
            SummaryPanel.SuspendLayout();
            HUCSelectionPanel.SuspendLayout();
            readHUCNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)HAWQSHUCHelp).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            button1.Location = new System.Drawing.Point(1970, 19);
            button1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(113, 55);
            button1.TabIndex = 0;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OK_click;
            // 
            // SimBaseLabel
            // 
            SimBaseLabel.AutoSize = true;
            SimBaseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            SimBaseLabel.Location = new System.Drawing.Point(33, 60);
            SimBaseLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            SimBaseLabel.Name = "SimBaseLabel";
            SimBaseLabel.Size = new System.Drawing.Size(259, 20);
            SimBaseLabel.TabIndex = 17;
            SimBaseLabel.Text = "Simulation Base:  Nutrients and OM";
            // 
            // ReadNetworkPanel
            // 
            ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ReadNetworkPanel.Controls.Add(ReadSNButton);
            ReadNetworkPanel.Controls.Add(label6);
            ReadNetworkPanel.Controls.Add(spanLabel);
            ReadNetworkPanel.Controls.Add(spanBox);
            ReadNetworkPanel.Controls.Add(endCOMIDLabel);
            ReadNetworkPanel.Controls.Add(EndCOMIDBox);
            ReadNetworkPanel.Controls.Add(comidLabel);
            ReadNetworkPanel.Controls.Add(comidBox);
            ReadNetworkPanel.Location = new System.Drawing.Point(32, 326);
            ReadNetworkPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ReadNetworkPanel.Name = "ReadNetworkPanel";
            ReadNetworkPanel.Size = new System.Drawing.Size(479, 303);
            ReadNetworkPanel.TabIndex = 24;
            ReadNetworkPanel.Visible = false;
            // 
            // ReadSNButton
            // 
            ReadSNButton.Location = new System.Drawing.Point(234, 228);
            ReadSNButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ReadSNButton.Name = "ReadSNButton";
            ReadSNButton.Size = new System.Drawing.Size(184, 49);
            ReadSNButton.TabIndex = 29;
            ReadSNButton.Text = "Read Network";
            ReadSNButton.UseVisualStyleBackColor = true;
            ReadSNButton.Click += ReadNetwork_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(377, 166);
            label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(49, 35);
            label6.TabIndex = 28;
            label6.Text = "km";
            // 
            // spanLabel
            // 
            spanLabel.AutoSize = true;
            spanLabel.Location = new System.Drawing.Point(76, 166);
            spanLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            spanLabel.Name = "spanLabel";
            spanLabel.Size = new System.Drawing.Size(168, 35);
            spanLabel.TabIndex = 27;
            spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            spanBox.Location = new System.Drawing.Point(236, 160);
            spanBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            spanBox.Name = "spanBox";
            spanBox.Size = new System.Drawing.Size(128, 39);
            spanBox.TabIndex = 26;
            spanBox.Leave += comidBox_Leave;
            // 
            // endCOMIDLabel
            // 
            endCOMIDLabel.AutoSize = true;
            endCOMIDLabel.Location = new System.Drawing.Point(2, 107);
            endCOMIDLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            endCOMIDLabel.Name = "endCOMIDLabel";
            endCOMIDLabel.Size = new System.Drawing.Size(249, 35);
            endCOMIDLabel.TabIndex = 25;
            endCOMIDLabel.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            EndCOMIDBox.Location = new System.Drawing.Point(236, 100);
            EndCOMIDBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            EndCOMIDBox.Name = "EndCOMIDBox";
            EndCOMIDBox.Size = new System.Drawing.Size(128, 39);
            EndCOMIDBox.TabIndex = 24;
            EndCOMIDBox.Leave += comidBox_Leave;
            // 
            // comidLabel
            // 
            comidLabel.AutoSize = true;
            comidLabel.Location = new System.Drawing.Point(141, 47);
            comidLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            comidLabel.Name = "comidLabel";
            comidLabel.Size = new System.Drawing.Size(92, 35);
            comidLabel.TabIndex = 23;
            comidLabel.Text = "ComID";
            // 
            // comidBox
            // 
            comidBox.BackColor = System.Drawing.SystemColors.Window;
            comidBox.Location = new System.Drawing.Point(236, 41);
            comidBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            comidBox.Name = "comidBox";
            comidBox.Size = new System.Drawing.Size(128, 39);
            comidBox.TabIndex = 22;
            comidBox.Text = "23398915";
            comidBox.Leave += comidBox_Leave;
            // 
            // ChooseTemplateButton
            // 
            ChooseTemplateButton.Location = new System.Drawing.Point(33, 141);
            ChooseTemplateButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ChooseTemplateButton.Name = "ChooseTemplateButton";
            ChooseTemplateButton.Size = new System.Drawing.Size(217, 49);
            ChooseTemplateButton.TabIndex = 29;
            ChooseTemplateButton.Text = "Choose Template";
            ChooseTemplateButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            ChooseTemplateButton.UseVisualStyleBackColor = true;
            ChooseTemplateButton.Click += Choose_from_Template_Click;
            // 
            // HelpButton2
            // 
            HelpButton2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            HelpButton2.Image = (System.Drawing.Image)resources.GetObject("HelpButton2.Image");
            HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton2.Location = new System.Drawing.Point(1785, 15);
            HelpButton2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HelpButton2.Name = "HelpButton2";
            HelpButton2.Size = new System.Drawing.Size(145, 60);
            HelpButton2.TabIndex = 30;
            HelpButton2.Text = "   Help";
            HelpButton2.UseVisualStyleBackColor = true;
            HelpButton2.Click += HelpButton2_Click;
            // 
            // infolabel1
            // 
            infolabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel1.AutoSize = true;
            infolabel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            infolabel1.ForeColor = System.Drawing.Color.Maroon;
            infolabel1.Location = new System.Drawing.Point(739, 1119);
            infolabel1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            infolabel1.Name = "infolabel1";
            infolabel1.Size = new System.Drawing.Size(495, 20);
            infolabel1.TabIndex = 39;
            infolabel1.Text = "Click on a pour-point stream segment then right-click on an upstream";
            // 
            // infolabel2
            // 
            infolabel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel2.AutoSize = true;
            infolabel2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            infolabel2.ForeColor = System.Drawing.Color.Maroon;
            infolabel2.Location = new System.Drawing.Point(739, 1159);
            infolabel2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            infolabel2.Name = "infolabel2";
            infolabel2.Size = new System.Drawing.Size(473, 20);
            infolabel2.TabIndex = 40;
            infolabel2.Text = "segment or input an up-river span in km and click \"Read Network\"";
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Location = new System.Drawing.Point(552, 151);
            webView.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(1697, 950);
            webView.TabIndex = 43;
            webView.ZoomFactor = 1D;
            // 
            // button3
            // 
            button3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button3.Location = new System.Drawing.Point(2136, 19);
            button3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(113, 55);
            button3.TabIndex = 46;
            button3.Text = "Cancel";
            button3.UseVisualStyleBackColor = true;
            // 
            // SimNameEdit
            // 
            SimNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SimNameEdit.Location = new System.Drawing.Point(217, 21);
            SimNameEdit.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            SimNameEdit.Name = "SimNameEdit";
            SimNameEdit.Size = new System.Drawing.Size(481, 39);
            SimNameEdit.TabIndex = 47;
            SimNameEdit.Tag = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label2.Location = new System.Drawing.Point(22, 28);
            label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(130, 20);
            label2.TabIndex = 48;
            label2.Text = "Simulation Name";
            // 
            // SimJSONLabel
            // 
            SimJSONLabel.AutoSize = true;
            SimJSONLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            SimJSONLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            SimJSONLabel.Location = new System.Drawing.Point(33, 102);
            SimJSONLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            SimJSONLabel.Name = "SimJSONLabel";
            SimJSONLabel.Size = new System.Drawing.Size(102, 20);
            SimJSONLabel.TabIndex = 50;
            SimJSONLabel.Text = "\"MS_OM.json\"";
            // 
            // StartDate
            // 
            StartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            StartDate.Location = new System.Drawing.Point(202, 271);
            StartDate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            StartDate.Name = "StartDate";
            StartDate.Size = new System.Drawing.Size(188, 39);
            StartDate.TabIndex = 51;
            StartDate.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label3.Location = new System.Drawing.Point(69, 282);
            label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(80, 20);
            label3.TabIndex = 52;
            label3.Text = "Start Date";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label5.Location = new System.Drawing.Point(82, 337);
            label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(72, 20);
            label5.TabIndex = 54;
            label5.Text = "End Date";
            // 
            // EndDate
            // 
            EndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            EndDate.Location = new System.Drawing.Point(202, 329);
            EndDate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            EndDate.Name = "EndDate";
            EndDate.Size = new System.Drawing.Size(188, 39);
            EndDate.TabIndex = 53;
            EndDate.Value = new System.DateTime(2011, 1, 1, 0, 0, 0, 0);
            // 
            // TogglePanel
            // 
            TogglePanel.Controls.Add(HUCNetworkButton);
            TogglePanel.Controls.Add(HUCButton);
            TogglePanel.Controls.Add(StreamButton);
            TogglePanel.Controls.Add(LakeButton);
            TogglePanel.Location = new System.Drawing.Point(795, 15);
            TogglePanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            TogglePanel.Name = "TogglePanel";
            TogglePanel.Size = new System.Drawing.Size(969, 60);
            TogglePanel.TabIndex = 58;
            // 
            // HUCNetworkButton
            // 
            HUCNetworkButton.AutoSize = true;
            HUCNetworkButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            HUCNetworkButton.Location = new System.Drawing.Point(773, 6);
            HUCNetworkButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HUCNetworkButton.Name = "HUCNetworkButton";
            HUCNetworkButton.Size = new System.Drawing.Size(126, 24);
            HUCNetworkButton.TabIndex = 5;
            HUCNetworkButton.Text = "HUC Network";
            HUCNetworkButton.UseVisualStyleBackColor = true;
            // 
            // HUCButton
            // 
            HUCButton.AutoSize = true;
            HUCButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            HUCButton.Location = new System.Drawing.Point(589, 6);
            HUCButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HUCButton.Name = "HUCButton";
            HUCButton.Size = new System.Drawing.Size(121, 24);
            HUCButton.TabIndex = 4;
            HUCButton.Text = "HAWQS HUC";
            HUCButton.UseVisualStyleBackColor = true;
            HUCButton.CheckedChanged += MapType_CheckChanged;
            // 
            // StreamButton
            // 
            StreamButton.AutoSize = true;
            StreamButton.Checked = true;
            StreamButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            StreamButton.Location = new System.Drawing.Point(13, 6);
            StreamButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            StreamButton.Name = "StreamButton";
            StreamButton.Size = new System.Drawing.Size(215, 24);
            StreamButton.TabIndex = 2;
            StreamButton.TabStop = true;
            StreamButton.Text = "Stream Network (w. lakes)";
            StreamButton.UseVisualStyleBackColor = true;
            StreamButton.CheckedChanged += MapType_CheckChanged;
            // 
            // LakeButton
            // 
            LakeButton.AutoSize = true;
            LakeButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            LakeButton.Location = new System.Drawing.Point(338, 6);
            LakeButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            LakeButton.Name = "LakeButton";
            LakeButton.Size = new System.Drawing.Size(166, 24);
            LakeButton.TabIndex = 0;
            LakeButton.Text = "0-D Lake/Reservoir";
            LakeButton.UseVisualStyleBackColor = true;
            LakeButton.CheckedChanged += MapType_CheckChanged;
            // 
            // panel2
            // 
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel2.Controls.Add(BrowseJSONButton);
            panel2.Controls.Add(ChooseTemplateButton);
            panel2.Controls.Add(SimBaseLabel);
            panel2.Controls.Add(SimJSONLabel);
            panel2.Controls.Add(StartDate);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(EndDate);
            panel2.Controls.Add(label5);
            panel2.Location = new System.Drawing.Point(32, 681);
            panel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(479, 429);
            panel2.TabIndex = 59;
            // 
            // BrowseJSONButton
            // 
            BrowseJSONButton.Location = new System.Drawing.Point(269, 141);
            BrowseJSONButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            BrowseJSONButton.Name = "BrowseJSONButton";
            BrowseJSONButton.Size = new System.Drawing.Size(124, 49);
            BrowseJSONButton.TabIndex = 55;
            BrowseJSONButton.Text = "Browse";
            BrowseJSONButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            BrowseJSONButton.UseVisualStyleBackColor = true;
            BrowseJSONButton.Click += BrowseJSONButton_Click;
            // 
            // NetworkLabel
            // 
            NetworkLabel.AutoSize = true;
            NetworkLabel.Location = new System.Drawing.Point(48, 307);
            NetworkLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            NetworkLabel.Name = "NetworkLabel";
            NetworkLabel.Size = new System.Drawing.Size(332, 35);
            NetworkLabel.TabIndex = 26;
            NetworkLabel.Text = "Parameters to Read Network";
            NetworkLabel.Visible = false;
            // 
            // SegLoadLabel
            // 
            SegLoadLabel.AutoSize = true;
            SegLoadLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 1, true);
            SegLoadLabel.ForeColor = System.Drawing.Color.Maroon;
            SegLoadLabel.Location = new System.Drawing.Point(1319, 98);
            SegLoadLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            SegLoadLabel.Name = "SegLoadLabel";
            SegLoadLabel.Size = new System.Drawing.Size(242, 20);
            SegLoadLabel.TabIndex = 60;
            SegLoadLabel.Text = "Zoom in to see stream segments.";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label7.Location = new System.Drawing.Point(48, 663);
            label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(271, 20);
            label7.TabIndex = 61;
            label7.Text = "Base AQUATOX Simulation and Dates";
            // 
            // SummaryPanel
            // 
            SummaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SummaryPanel.Controls.Add(Summary2Label);
            SummaryPanel.Controls.Add(Summary1Label);
            SummaryPanel.Location = new System.Drawing.Point(32, 126);
            SummaryPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            SummaryPanel.Name = "SummaryPanel";
            SummaryPanel.Size = new System.Drawing.Size(479, 166);
            SummaryPanel.TabIndex = 62;
            // 
            // Summary2Label
            // 
            Summary2Label.Font = new System.Drawing.Font("Segoe UI", 9F);
            Summary2Label.Location = new System.Drawing.Point(28, 70);
            Summary2Label.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            Summary2Label.Name = "Summary2Label";
            Summary2Label.Size = new System.Drawing.Size(416, 73);
            Summary2Label.TabIndex = 58;
            Summary2Label.Text = "Surface Area (sq. km):";
            Summary2Label.Visible = false;
            // 
            // Summary1Label
            // 
            Summary1Label.AutoSize = true;
            Summary1Label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            Summary1Label.Location = new System.Drawing.Point(28, 23);
            Summary1Label.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            Summary1Label.Name = "Summary1Label";
            Summary1Label.Size = new System.Drawing.Size(187, 20);
            Summary1Label.TabIndex = 57;
            Summary1Label.Text = "WB COMID:  (unselected)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(48, 107);
            label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(281, 35);
            label8.TabIndex = 63;
            label8.Text = "Selected Model Domain";
            // 
            // ShowH14Box
            // 
            ShowH14Box.AutoSize = true;
            ShowH14Box.Location = new System.Drawing.Point(765, 94);
            ShowH14Box.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ShowH14Box.Name = "ShowH14Box";
            ShowH14Box.Size = new System.Drawing.Size(311, 39);
            ShowH14Box.TabIndex = 65;
            ShowH14Box.Text = "Show HUC14 Boundaries";
            ShowH14Box.UseVisualStyleBackColor = true;
            ShowH14Box.CheckedChanged += ShowH14Box_CheckedChanged;
            // 
            // BHUC12
            // 
            BHUC12.AutoSize = true;
            BHUC12.Location = new System.Drawing.Point(141, 6);
            BHUC12.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            BHUC12.Name = "BHUC12";
            BHUC12.Size = new System.Drawing.Size(112, 39);
            BHUC12.TabIndex = 0;
            BHUC12.Text = "HUC12";
            BHUC12.UseVisualStyleBackColor = true;
            BHUC12.CheckedChanged += BHUC14_CheckedChanged;
            // 
            // BHUC14
            // 
            BHUC14.AutoSize = true;
            BHUC14.Checked = true;
            BHUC14.Location = new System.Drawing.Point(15, 6);
            BHUC14.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            BHUC14.Name = "BHUC14";
            BHUC14.Size = new System.Drawing.Size(112, 39);
            BHUC14.TabIndex = 2;
            BHUC14.TabStop = true;
            BHUC14.Text = "HUC14";
            BHUC14.UseVisualStyleBackColor = true;
            BHUC14.CheckedChanged += BHUC14_CheckedChanged;
            // 
            // BHUC10
            // 
            BHUC10.AutoSize = true;
            BHUC10.Location = new System.Drawing.Point(267, 6);
            BHUC10.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            BHUC10.Name = "BHUC10";
            BHUC10.Size = new System.Drawing.Size(112, 39);
            BHUC10.TabIndex = 5;
            BHUC10.Text = "HUC10";
            BHUC10.UseVisualStyleBackColor = true;
            BHUC10.CheckedChanged += BHUC14_CheckedChanged;
            // 
            // BHUC8
            // 
            BHUC8.AutoSize = true;
            BHUC8.Location = new System.Drawing.Point(394, 6);
            BHUC8.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            BHUC8.Name = "BHUC8";
            BHUC8.Size = new System.Drawing.Size(99, 39);
            BHUC8.TabIndex = 6;
            BHUC8.Text = "HUC8";
            BHUC8.UseVisualStyleBackColor = true;
            BHUC8.CheckedChanged += BHUC14_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(507, 13);
            label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(81, 35);
            label1.TabIndex = 7;
            label1.Text = "label1";
            // 
            // HUCSelectionPanel
            // 
            HUCSelectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            HUCSelectionPanel.Controls.Add(label1);
            HUCSelectionPanel.Controls.Add(BHUC8);
            HUCSelectionPanel.Controls.Add(BHUC10);
            HUCSelectionPanel.Controls.Add(BHUC14);
            HUCSelectionPanel.Controls.Add(BHUC12);
            HUCSelectionPanel.Location = new System.Drawing.Point(754, 85);
            HUCSelectionPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HUCSelectionPanel.Name = "HUCSelectionPanel";
            HUCSelectionPanel.Size = new System.Drawing.Size(515, 53);
            HUCSelectionPanel.TabIndex = 64;
            HUCSelectionPanel.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label4.Location = new System.Drawing.Point(728, 28);
            label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(42, 20);
            label4.TabIndex = 66;
            label4.Text = "Type";
            // 
            // readHUCNetworkPanel
            // 
            readHUCNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            readHUCNetworkPanel.Controls.Add(clear_network);
            readHUCNetworkPanel.Controls.Add(upperHUCBox);
            readHUCNetworkPanel.Controls.Add(ModelHUC8checkBox);
            readHUCNetworkPanel.Controls.Add(ReadHUCNetworkButton);
            readHUCNetworkPanel.Controls.Add(HUCLabel3);
            readHUCNetworkPanel.Controls.Add(HUCLabel2);
            readHUCNetworkPanel.Controls.Add(traverseHUCBox);
            readHUCNetworkPanel.Controls.Add(HUCLabel1);
            readHUCNetworkPanel.Controls.Add(label12);
            readHUCNetworkPanel.Controls.Add(HUCBox);
            readHUCNetworkPanel.Location = new System.Drawing.Point(32, 326);
            readHUCNetworkPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            readHUCNetworkPanel.Name = "readHUCNetworkPanel";
            readHUCNetworkPanel.Size = new System.Drawing.Size(479, 303);
            readHUCNetworkPanel.TabIndex = 30;
            readHUCNetworkPanel.Visible = false;
            // 
            // clear_network
            // 
            clear_network.Enabled = false;
            clear_network.Location = new System.Drawing.Point(28, 241);
            clear_network.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            clear_network.Name = "clear_network";
            clear_network.Size = new System.Drawing.Size(184, 49);
            clear_network.TabIndex = 67;
            clear_network.Text = "Clear Network";
            clear_network.UseVisualStyleBackColor = true;
            clear_network.Click += clear_network_Click;
            // 
            // upperHUCBox
            // 
            upperHUCBox.Enabled = false;
            upperHUCBox.Location = new System.Drawing.Point(247, 122);
            upperHUCBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            upperHUCBox.Name = "upperHUCBox";
            upperHUCBox.Size = new System.Drawing.Size(205, 39);
            upperHUCBox.TabIndex = 24;
            upperHUCBox.Leave += HUCBox_Leave;
            // 
            // ModelHUC8checkBox
            // 
            ModelHUC8checkBox.AutoSize = true;
            ModelHUC8checkBox.Checked = true;
            ModelHUC8checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            ModelHUC8checkBox.Location = new System.Drawing.Point(28, 81);
            ModelHUC8checkBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ModelHUC8checkBox.Name = "ModelHUC8checkBox";
            ModelHUC8checkBox.Size = new System.Drawing.Size(402, 39);
            ModelHUC8checkBox.TabIndex = 66;
            ModelHUC8checkBox.Text = "Model all up-river HUCS in HUC8";
            ModelHUC8checkBox.UseVisualStyleBackColor = true;
            ModelHUC8checkBox.CheckedChanged += ModelHUC8checkBox_CheckedChanged;
            // 
            // ReadHUCNetworkButton
            // 
            ReadHUCNetworkButton.Location = new System.Drawing.Point(234, 241);
            ReadHUCNetworkButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            ReadHUCNetworkButton.Name = "ReadHUCNetworkButton";
            ReadHUCNetworkButton.Size = new System.Drawing.Size(184, 49);
            ReadHUCNetworkButton.TabIndex = 29;
            ReadHUCNetworkButton.Text = "Read Network";
            ReadHUCNetworkButton.UseVisualStyleBackColor = true;
            ReadHUCNetworkButton.Click += ReadHUCNetworkButton_Click;
            // 
            // HUCLabel3
            // 
            HUCLabel3.AutoSize = true;
            HUCLabel3.Enabled = false;
            HUCLabel3.Location = new System.Drawing.Point(321, 188);
            HUCLabel3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            HUCLabel3.Name = "HUCLabel3";
            HUCLabel3.Size = new System.Drawing.Size(76, 35);
            HUCLabel3.TabIndex = 28;
            HUCLabel3.Text = "HUCs";
            // 
            // HUCLabel2
            // 
            HUCLabel2.AutoSize = true;
            HUCLabel2.Enabled = false;
            HUCLabel2.Location = new System.Drawing.Point(69, 186);
            HUCLabel2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            HUCLabel2.Name = "HUCLabel2";
            HUCLabel2.Size = new System.Drawing.Size(203, 35);
            HUCLabel2.TabIndex = 27;
            HUCLabel2.Text = "(or)  Traverse up ";
            // 
            // traverseHUCBox
            // 
            traverseHUCBox.Enabled = false;
            traverseHUCBox.Location = new System.Drawing.Point(247, 181);
            traverseHUCBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            traverseHUCBox.Name = "traverseHUCBox";
            traverseHUCBox.Size = new System.Drawing.Size(62, 39);
            traverseHUCBox.TabIndex = 26;
            traverseHUCBox.Leave += HUCBox_Leave;
            // 
            // HUCLabel1
            // 
            HUCLabel1.AutoSize = true;
            HUCLabel1.Enabled = false;
            HUCLabel1.Location = new System.Drawing.Point(7, 134);
            HUCLabel1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            HUCLabel1.Name = "HUCLabel1";
            HUCLabel1.Size = new System.Drawing.Size(274, 35);
            HUCLabel1.TabIndex = 25;
            HUCLabel1.Text = "(or specify) upper HUC:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label12.Location = new System.Drawing.Point(50, 23);
            label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(119, 20);
            label12.TabIndex = 23;
            label12.Text = "Pour-Point HUC";
            // 
            // HUCBox
            // 
            HUCBox.BackColor = System.Drawing.SystemColors.Window;
            HUCBox.Location = new System.Drawing.Point(247, 17);
            HUCBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HUCBox.Name = "HUCBox";
            HUCBox.Size = new System.Drawing.Size(205, 39);
            HUCBox.TabIndex = 22;
            HUCBox.Leave += HUCBox_Leave;
            // 
            // HAWQSHUCLabel
            // 
            HAWQSHUCLabel.AutoSize = true;
            HAWQSHUCLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 1, true);
            HAWQSHUCLabel.ForeColor = System.Drawing.Color.Black;
            HAWQSHUCLabel.Location = new System.Drawing.Point(585, 98);
            HAWQSHUCLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            HAWQSHUCLabel.Name = "HAWQSHUCLabel";
            HAWQSHUCLabel.Size = new System.Drawing.Size(107, 20);
            HAWQSHUCLabel.TabIndex = 67;
            HAWQSHUCLabel.Text = "HAWQS HUCs";
            HAWQSHUCLabel.Visible = false;
            // 
            // HAWQSHUCHelp
            // 
            HAWQSHUCHelp.Image = Properties.Resources.help_icon;
            HAWQSHUCHelp.Location = new System.Drawing.Point(542, 90);
            HAWQSHUCHelp.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            HAWQSHUCHelp.Name = "HAWQSHUCHelp";
            HAWQSHUCHelp.Size = new System.Drawing.Size(37, 43);
            HAWQSHUCHelp.TabIndex = 68;
            HAWQSHUCHelp.TabStop = false;
            HAWQSHUCHelp.Visible = false;
            HAWQSHUCHelp.Click += pictureBox1_Click;
            HAWQSHUCHelp.MouseHover += pictureBox1_MouseHover;
            // 
            // NewSimForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(2273, 1215);
            Controls.Add(HAWQSHUCHelp);
            Controls.Add(HAWQSHUCLabel);
            Controls.Add(NetworkLabel);
            Controls.Add(readHUCNetworkPanel);
            Controls.Add(label4);
            Controls.Add(ShowH14Box);
            Controls.Add(HUCSelectionPanel);
            Controls.Add(label8);
            Controls.Add(SummaryPanel);
            Controls.Add(SegLoadLabel);
            Controls.Add(label7);
            Controls.Add(panel2);
            Controls.Add(TogglePanel);
            Controls.Add(label2);
            Controls.Add(SimNameEdit);
            Controls.Add(button3);
            Controls.Add(webView);
            Controls.Add(infolabel2);
            Controls.Add(infolabel1);
            Controls.Add(HelpButton2);
            Controls.Add(ReadNetworkPanel);
            Controls.Add(button1);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            MinimumSize = new System.Drawing.Size(1918, 1028);
            Name = "NewSimForm";
            Text = "New Simulation Window";
            FormClosing += NewSimForm_FormClosing;
            Shown += NewSimForm_Shown;
            ReadNetworkPanel.ResumeLayout(false);
            ReadNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            TogglePanel.ResumeLayout(false);
            TogglePanel.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            SummaryPanel.ResumeLayout(false);
            SummaryPanel.PerformLayout();
            HUCSelectionPanel.ResumeLayout(false);
            HUCSelectionPanel.PerformLayout();
            readHUCNetworkPanel.ResumeLayout(false);
            readHUCNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)HAWQSHUCHelp).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label SimBaseLabel;
        private System.Windows.Forms.Panel ReadNetworkPanel;
        private System.Windows.Forms.Button ReadSNButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label spanLabel;
        private System.Windows.Forms.TextBox spanBox;
        private System.Windows.Forms.Label endCOMIDLabel;
        private System.Windows.Forms.TextBox EndCOMIDBox;
        private System.Windows.Forms.Label comidLabel;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.Button ChooseTemplateButton;
        private System.Windows.Forms.Button HelpButton2;
        private System.Windows.Forms.Label infolabel1;
        private System.Windows.Forms.Label infolabel2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox SimNameEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label SimJSONLabel;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.Panel TogglePanel;
        private System.Windows.Forms.RadioButton StreamButton;
        private System.Windows.Forms.RadioButton LakeButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label NetworkLabel;
        private System.Windows.Forms.Label SegLoadLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel SummaryPanel;
        private System.Windows.Forms.Label Summary2Label;
        private System.Windows.Forms.Label Summary1Label;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button BrowseJSONButton;
        private System.Windows.Forms.RadioButton HUCButton;
        private System.Windows.Forms.Panel HUCSelectionPanel;
        private System.Windows.Forms.RadioButton BHUC8;
        private System.Windows.Forms.RadioButton BHUC10;
        private System.Windows.Forms.RadioButton BHUC14;
        private System.Windows.Forms.RadioButton BHUC12;
        private System.Windows.Forms.CheckBox ShowH14Box;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton HUCNetworkButton;
        private System.Windows.Forms.Panel readHUCNetworkPanel;
        private System.Windows.Forms.Button ReadHUCNetworkButton;
        private System.Windows.Forms.Label HUCLabel3;
        private System.Windows.Forms.Label HUCLabel2;
        private System.Windows.Forms.TextBox traverseHUCBox;
        private System.Windows.Forms.Label HUCLabel1;
        private System.Windows.Forms.TextBox upperHUCBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox HUCBox;
        private System.Windows.Forms.CheckBox ModelHUC8checkBox;
        private System.Windows.Forms.Button clear_network;
        private System.Windows.Forms.Label HAWQSHUCLabel;
        private System.Windows.Forms.PictureBox HAWQSHUCHelp;
    }
}