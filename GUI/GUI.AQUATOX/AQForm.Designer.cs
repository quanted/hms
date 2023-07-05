using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;


namespace GUI.AQUATOX
{
    partial class AQTTestForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AQTTestForm));
            this.loadJSON = new System.Windows.Forms.Button();
            this.saveJSON = new System.Windows.Forms.Button();
            this.integrate = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.outputbutton = new System.Windows.Forms.Button();
            this.AnimalDB = new System.Windows.Forms.Button();
            this.ReminDB = new System.Windows.Forms.Button();
            this.ChemDB = new System.Windows.Forms.Button();
            this.SiteDB = new System.Windows.Forms.Button();
            this.PlantsDB = new System.Windows.Forms.Button();
            this.SVListBox = new System.Windows.Forms.ListBox();
            this.StudyNameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DBPanel = new System.Windows.Forms.Panel();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.FoodWebButton = new System.Windows.Forms.Button();
            this.ReminButton = new System.Windows.Forms.Button();
            this.SiteButton = new System.Windows.Forms.Button();
            this.ChemButton = new System.Windows.Forms.Button();
            this.AnimButton = new System.Windows.Forms.Button();
            this.PlantsButton = new System.Windows.Forms.Button();
            this.Diagenesis = new System.Windows.Forms.Button();
            this.RunStatusLabel = new System.Windows.Forms.Label();
            this.EditButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.MultiSegButton = new System.Windows.Forms.Button();
            this.HelpButton = new System.Windows.Forms.Button();
            this.modelRunningLabel = new System.Windows.Forms.Label();
            this.browserButton = new System.Windows.Forms.Button();
            this.ParametersLabel = new System.Windows.Forms.Label();
            this.StateVarLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SetupButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButt = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.DeleteResultsButton = new System.Windows.Forms.Button();
            this.estdbbutton = new System.Windows.Forms.Button();
            this.AnnealPanel = new System.Windows.Forms.Panel();
            this.randomBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.IterationBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TStartBox = new System.Windows.Forms.TextBox();
            this.calcerror = new System.Windows.Forms.Button();
            this.ExportBest = new System.Windows.Forms.Button();
            this.parallelanneal = new System.Windows.Forms.Button();
            this.DBPanel.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.AnnealPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadJSON
            // 
            this.loadJSON.Location = new System.Drawing.Point(26, 11);
            this.loadJSON.Name = "loadJSON";
            this.loadJSON.Size = new System.Drawing.Size(87, 24);
            this.loadJSON.TabIndex = 0;
            this.loadJSON.Text = "Load ";
            this.loadJSON.UseVisualStyleBackColor = true;
            this.loadJSON.Click += new System.EventHandler(this.loadJSON_Click);
            // 
            // saveJSON
            // 
            this.saveJSON.Enabled = false;
            this.saveJSON.Location = new System.Drawing.Point(26, 43);
            this.saveJSON.Name = "saveJSON";
            this.saveJSON.Size = new System.Drawing.Size(87, 24);
            this.saveJSON.TabIndex = 0;
            this.saveJSON.Text = "Save";
            this.saveJSON.UseVisualStyleBackColor = true;
            this.saveJSON.Click += new System.EventHandler(this.saveJSON_Click);
            // 
            // integrate
            // 
            this.integrate.Location = new System.Drawing.Point(26, 152);
            this.integrate.Name = "integrate";
            this.integrate.Size = new System.Drawing.Size(87, 25);
            this.integrate.TabIndex = 1;
            this.integrate.Text = "Run";
            this.integrate.UseVisualStyleBackColor = true;
            this.integrate.Visible = false;
            this.integrate.Click += new System.EventHandler(this.integrate_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(146, 76);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(579, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Value = 1;
            this.progressBar1.Visible = false;
            // 
            // outputbutton
            // 
            this.outputbutton.Location = new System.Drawing.Point(26, 345);
            this.outputbutton.Name = "outputbutton";
            this.outputbutton.Size = new System.Drawing.Size(87, 25);
            this.outputbutton.TabIndex = 1;
            this.outputbutton.Text = "Output";
            this.outputbutton.UseVisualStyleBackColor = true;
            this.outputbutton.Visible = false;
            this.outputbutton.Click += new System.EventHandler(this.graph_Click);
            // 
            // AnimalDB
            // 
            this.AnimalDB.Location = new System.Drawing.Point(76, 21);
            this.AnimalDB.Name = "AnimalDB";
            this.AnimalDB.Size = new System.Drawing.Size(60, 25);
            this.AnimalDB.TabIndex = 0;
            this.AnimalDB.Text = "Animals";
            this.AnimalDB.UseVisualStyleBackColor = true;
            this.AnimalDB.Click += new System.EventHandler(this.AnimDB_Click);
            // 
            // ReminDB
            // 
            this.ReminDB.Location = new System.Drawing.Point(142, 21);
            this.ReminDB.Name = "ReminDB";
            this.ReminDB.Size = new System.Drawing.Size(60, 25);
            this.ReminDB.TabIndex = 5;
            this.ReminDB.Text = "Remin. ";
            this.ReminDB.UseVisualStyleBackColor = true;
            this.ReminDB.Click += new System.EventHandler(this.ReminDB_Click);
            // 
            // ChemDB
            // 
            this.ChemDB.Location = new System.Drawing.Point(208, 20);
            this.ChemDB.Name = "ChemDB";
            this.ChemDB.Size = new System.Drawing.Size(60, 25);
            this.ChemDB.TabIndex = 6;
            this.ChemDB.Text = "Chems";
            this.ChemDB.UseVisualStyleBackColor = true;
            this.ChemDB.Click += new System.EventHandler(this.ChemDB_Click);
            // 
            // SiteDB
            // 
            this.SiteDB.Location = new System.Drawing.Point(10, 21);
            this.SiteDB.Name = "SiteDB";
            this.SiteDB.Size = new System.Drawing.Size(60, 25);
            this.SiteDB.TabIndex = 7;
            this.SiteDB.Text = "Sites";
            this.SiteDB.UseVisualStyleBackColor = true;
            this.SiteDB.Click += new System.EventHandler(this.SiteDB_Click);
            // 
            // PlantsDB
            // 
            this.PlantsDB.Location = new System.Drawing.Point(274, 20);
            this.PlantsDB.Name = "PlantsDB";
            this.PlantsDB.Size = new System.Drawing.Size(60, 25);
            this.PlantsDB.TabIndex = 8;
            this.PlantsDB.Text = "Plants";
            this.PlantsDB.UseVisualStyleBackColor = true;
            this.PlantsDB.Click += new System.EventHandler(this.PlantsDB_Click);
            // 
            // SVListBox
            // 
            this.SVListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SVListBox.FormattingEnabled = true;
            this.SVListBox.ItemHeight = 15;
            this.SVListBox.Location = new System.Drawing.Point(440, 126);
            this.SVListBox.Name = "SVListBox";
            this.SVListBox.Size = new System.Drawing.Size(356, 319);
            this.SVListBox.TabIndex = 9;
            this.SVListBox.Visible = false;
            this.SVListBox.SelectedIndexChanged += new System.EventHandler(this.SVListBox_SelectedIndexChanged);
            this.SVListBox.DoubleClick += new System.EventHandler(this.SVListBox_DoubleClick);
            // 
            // StudyNameBox
            // 
            this.StudyNameBox.Location = new System.Drawing.Point(146, 27);
            this.StudyNameBox.Name = "StudyNameBox";
            this.StudyNameBox.Size = new System.Drawing.Size(287, 23);
            this.StudyNameBox.TabIndex = 10;
            this.StudyNameBox.TextChanged += new System.EventHandler(this.StudyNameBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name of Simulation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "Databases of Parameter Values";
            // 
            // DBPanel
            // 
            this.DBPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DBPanel.Controls.Add(this.label3);
            this.DBPanel.Controls.Add(this.PlantsDB);
            this.DBPanel.Controls.Add(this.SiteDB);
            this.DBPanel.Controls.Add(this.ChemDB);
            this.DBPanel.Controls.Add(this.ReminDB);
            this.DBPanel.Controls.Add(this.AnimalDB);
            this.DBPanel.Location = new System.Drawing.Point(454, 6);
            this.DBPanel.Name = "DBPanel";
            this.DBPanel.Size = new System.Drawing.Size(343, 55);
            this.DBPanel.TabIndex = 16;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ButtonPanel.Controls.Add(this.FoodWebButton);
            this.ButtonPanel.Controls.Add(this.ReminButton);
            this.ButtonPanel.Controls.Add(this.SiteButton);
            this.ButtonPanel.Controls.Add(this.ChemButton);
            this.ButtonPanel.Controls.Add(this.AnimButton);
            this.ButtonPanel.Controls.Add(this.PlantsButton);
            this.ButtonPanel.Controls.Add(this.Diagenesis);
            this.ButtonPanel.Location = new System.Drawing.Point(163, 127);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(244, 182);
            this.ButtonPanel.TabIndex = 17;
            this.ButtonPanel.Visible = false;
            // 
            // FoodWebButton
            // 
            this.FoodWebButton.Location = new System.Drawing.Point(66, 130);
            this.FoodWebButton.Name = "FoodWebButton";
            this.FoodWebButton.Size = new System.Drawing.Size(92, 25);
            this.FoodWebButton.TabIndex = 8;
            this.FoodWebButton.Text = "Food Web";
            this.FoodWebButton.UseVisualStyleBackColor = true;
            this.FoodWebButton.Click += new System.EventHandler(this.FoodWebButton_Click);
            // 
            // ReminButton
            // 
            this.ReminButton.Location = new System.Drawing.Point(127, 14);
            this.ReminButton.Name = "ReminButton";
            this.ReminButton.Size = new System.Drawing.Size(92, 25);
            this.ReminButton.TabIndex = 2;
            this.ReminButton.Text = "Org. Matter";
            this.ReminButton.UseVisualStyleBackColor = true;
            this.ReminButton.Click += new System.EventHandler(this.Remin);
            // 
            // SiteButton
            // 
            this.SiteButton.Location = new System.Drawing.Point(19, 14);
            this.SiteButton.Name = "SiteButton";
            this.SiteButton.Size = new System.Drawing.Size(92, 25);
            this.SiteButton.TabIndex = 3;
            this.SiteButton.Text = "Site";
            this.SiteButton.UseVisualStyleBackColor = true;
            this.SiteButton.Click += new System.EventHandler(this.Sites);
            // 
            // ChemButton
            // 
            this.ChemButton.Location = new System.Drawing.Point(126, 50);
            this.ChemButton.Name = "ChemButton";
            this.ChemButton.Size = new System.Drawing.Size(92, 25);
            this.ChemButton.TabIndex = 4;
            this.ChemButton.Text = "Chemicals";
            this.ChemButton.UseVisualStyleBackColor = true;
            this.ChemButton.Click += new System.EventHandler(this.Chems);
            // 
            // AnimButton
            // 
            this.AnimButton.Location = new System.Drawing.Point(19, 86);
            this.AnimButton.Name = "AnimButton";
            this.AnimButton.Size = new System.Drawing.Size(92, 25);
            this.AnimButton.TabIndex = 5;
            this.AnimButton.Text = "Animals";
            this.AnimButton.UseVisualStyleBackColor = true;
            this.AnimButton.Click += new System.EventHandler(this.AnimButton_Click);
            // 
            // PlantsButton
            // 
            this.PlantsButton.Location = new System.Drawing.Point(19, 50);
            this.PlantsButton.Name = "PlantsButton";
            this.PlantsButton.Size = new System.Drawing.Size(92, 25);
            this.PlantsButton.TabIndex = 6;
            this.PlantsButton.Text = "Plants";
            this.PlantsButton.UseVisualStyleBackColor = true;
            this.PlantsButton.Click += new System.EventHandler(this.Plants);
            // 
            // Diagenesis
            // 
            this.Diagenesis.Location = new System.Drawing.Point(127, 86);
            this.Diagenesis.Name = "Diagenesis";
            this.Diagenesis.Size = new System.Drawing.Size(91, 25);
            this.Diagenesis.TabIndex = 7;
            this.Diagenesis.Text = "Diagenesis";
            this.Diagenesis.UseVisualStyleBackColor = true;
            this.Diagenesis.Click += new System.EventHandler(this.Diagensis);
            // 
            // RunStatusLabel
            // 
            this.RunStatusLabel.AutoSize = true;
            this.RunStatusLabel.Location = new System.Drawing.Point(28, 187);
            this.RunStatusLabel.Name = "RunStatusLabel";
            this.RunStatusLabel.Size = new System.Drawing.Size(63, 15);
            this.RunStatusLabel.TabIndex = 18;
            this.RunStatusLabel.Text = "Run Status";
            // 
            // EditButton
            // 
            this.EditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditButton.Location = new System.Drawing.Point(584, 450);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(60, 25);
            this.EditButton.TabIndex = 20;
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Visible = false;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeleteButton.Location = new System.Drawing.Point(514, 450);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(60, 25);
            this.DeleteButton.TabIndex = 19;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Visible = false;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // MultiSegButton
            // 
            this.MultiSegButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MultiSegButton.Location = new System.Drawing.Point(26, 445);
            this.MultiSegButton.Name = "MultiSegButton";
            this.MultiSegButton.Size = new System.Drawing.Size(138, 24);
            this.MultiSegButton.TabIndex = 22;
            this.MultiSegButton.Text = "NWM-Linked Runs";
            this.MultiSegButton.UseVisualStyleBackColor = true;
            this.MultiSegButton.Click += new System.EventHandler(this.MultiSegButton_Click);
            // 
            // HelpButton
            // 
            this.HelpButton.Image = global::GUI.AQUATOX.Properties.Resources.help_icon;
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(26, 264);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(87, 28);
            this.HelpButton.TabIndex = 23;
            this.HelpButton.Text = "  Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // modelRunningLabel
            // 
            this.modelRunningLabel.AutoSize = true;
            this.modelRunningLabel.Location = new System.Drawing.Point(145, 60);
            this.modelRunningLabel.Name = "modelRunningLabel";
            this.modelRunningLabel.Size = new System.Drawing.Size(100, 15);
            this.modelRunningLabel.TabIndex = 24;
            this.modelRunningLabel.Text = "Model is Running";
            this.modelRunningLabel.Visible = false;
            // 
            // browserButton
            // 
            this.browserButton.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.browserButton.ForeColor = System.Drawing.Color.Black;
            this.browserButton.Location = new System.Drawing.Point(26, 296);
            this.browserButton.Margin = new System.Windows.Forms.Padding(0);
            this.browserButton.Name = "browserButton";
            this.browserButton.Size = new System.Drawing.Size(87, 23);
            this.browserButton.TabIndex = 25;
            this.browserButton.Text = "Select Browser";
            this.browserButton.UseVisualStyleBackColor = true;
            this.browserButton.Click += new System.EventHandler(this.browserButton_Click);
            // 
            // ParametersLabel
            // 
            this.ParametersLabel.AutoSize = true;
            this.ParametersLabel.Location = new System.Drawing.Point(163, 110);
            this.ParametersLabel.Name = "ParametersLabel";
            this.ParametersLabel.Size = new System.Drawing.Size(103, 15);
            this.ParametersLabel.TabIndex = 26;
            this.ParametersLabel.Text = "Model Parameters";
            this.ParametersLabel.Visible = false;
            // 
            // StateVarLabel
            // 
            this.StateVarLabel.AutoSize = true;
            this.StateVarLabel.Location = new System.Drawing.Point(441, 108);
            this.StateVarLabel.Name = "StateVarLabel";
            this.StateVarLabel.Size = new System.Drawing.Size(193, 15);
            this.StateVarLabel.TabIndex = 27;
            this.StateVarLabel.Text = "Model State Variables and Loadings";
            this.StateVarLabel.Visible = false;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CancelButton.ForeColor = System.Drawing.Color.Black;
            this.CancelButton.Location = new System.Drawing.Point(727, 75);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(68, 25);
            this.CancelButton.TabIndex = 28;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Visible = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SetupButton
            // 
            this.SetupButton.Location = new System.Drawing.Point(22, 98);
            this.SetupButton.Name = "SetupButton";
            this.SetupButton.Size = new System.Drawing.Size(91, 25);
            this.SetupButton.TabIndex = 29;
            this.SetupButton.Text = "Edit Setup";
            this.SetupButton.UseVisualStyleBackColor = true;
            this.SetupButton.Visible = false;
            this.SetupButton.Click += new System.EventHandler(this.Setup_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(220, 116);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(468, 288);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(659, 11);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(61, 23);
            this.OKButton.TabIndex = 35;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Visible = false;
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.Location = new System.Drawing.Point(735, 11);
            this.CancelButt.Name = "CancelButt";
            this.CancelButt.Size = new System.Drawing.Size(61, 23);
            this.CancelButt.TabIndex = 36;
            this.CancelButt.Text = "Cancel";
            this.CancelButt.UseVisualStyleBackColor = true;
            this.CancelButt.Visible = false;
            this.CancelButt.Click += new System.EventHandler(this.CancelButt_Click);
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddButton.Location = new System.Drawing.Point(444, 450);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(60, 25);
            this.AddButton.TabIndex = 21;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Visible = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DeleteResultsButton
            // 
            this.DeleteResultsButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DeleteResultsButton.ForeColor = System.Drawing.Color.Black;
            this.DeleteResultsButton.Location = new System.Drawing.Point(47, 205);
            this.DeleteResultsButton.Margin = new System.Windows.Forms.Padding(0);
            this.DeleteResultsButton.Name = "DeleteResultsButton";
            this.DeleteResultsButton.Size = new System.Drawing.Size(64, 22);
            this.DeleteResultsButton.TabIndex = 39;
            this.DeleteResultsButton.Text = "Delete all";
            this.DeleteResultsButton.UseVisualStyleBackColor = true;
            this.DeleteResultsButton.Visible = false;
            this.DeleteResultsButton.Click += new System.EventHandler(this.DeleteResultsButton_Click);
            // 
            // estdbbutton
            // 
            this.estdbbutton.Location = new System.Drawing.Point(175, 19);
            this.estdbbutton.Name = "estdbbutton";
            this.estdbbutton.Size = new System.Drawing.Size(59, 20);
            this.estdbbutton.TabIndex = 40;
            this.estdbbutton.Text = "est db";
            this.estdbbutton.UseVisualStyleBackColor = true;
            this.estdbbutton.Click += new System.EventHandler(this.estdbbutton_Click);
            // 
            // AnnealPanel
            // 
            this.AnnealPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AnnealPanel.Controls.Add(this.randomBox);
            this.AnnealPanel.Controls.Add(this.label4);
            this.AnnealPanel.Controls.Add(this.IterationBox);
            this.AnnealPanel.Controls.Add(this.label2);
            this.AnnealPanel.Controls.Add(this.TStartBox);
            this.AnnealPanel.Controls.Add(this.calcerror);
            this.AnnealPanel.Controls.Add(this.ExportBest);
            this.AnnealPanel.Controls.Add(this.parallelanneal);
            this.AnnealPanel.Controls.Add(this.estdbbutton);
            this.AnnealPanel.Location = new System.Drawing.Point(163, 325);
            this.AnnealPanel.Name = "AnnealPanel";
            this.AnnealPanel.Size = new System.Drawing.Size(252, 97);
            this.AnnealPanel.TabIndex = 48;
            this.AnnealPanel.Visible = false;
            // 
            // randomBox
            // 
            this.randomBox.AutoSize = true;
            this.randomBox.Checked = true;
            this.randomBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.randomBox.Location = new System.Drawing.Point(165, 57);
            this.randomBox.Name = "randomBox";
            this.randomBox.Size = new System.Drawing.Size(70, 19);
            this.randomBox.TabIndex = 55;
            this.randomBox.Text = "rnd start";
            this.randomBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 15);
            this.label4.TabIndex = 54;
            this.label4.Text = "n / T";
            // 
            // IterationBox
            // 
            this.IterationBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IterationBox.Location = new System.Drawing.Point(118, 55);
            this.IterationBox.Name = "IterationBox";
            this.IterationBox.Size = new System.Drawing.Size(28, 22);
            this.IterationBox.TabIndex = 53;
            this.IterationBox.Text = "200";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 52;
            this.label2.Text = "T start";
            // 
            // TStartBox
            // 
            this.TStartBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TStartBox.Location = new System.Drawing.Point(46, 54);
            this.TStartBox.Name = "TStartBox";
            this.TStartBox.Size = new System.Drawing.Size(28, 22);
            this.TStartBox.TabIndex = 51;
            this.TStartBox.Text = "100";
            // 
            // calcerror
            // 
            this.calcerror.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.calcerror.ForeColor = System.Drawing.Color.Black;
            this.calcerror.Location = new System.Drawing.Point(166, 17);
            this.calcerror.Margin = new System.Windows.Forms.Padding(0);
            this.calcerror.Name = "calcerror";
            this.calcerror.Size = new System.Drawing.Size(71, 22);
            this.calcerror.TabIndex = 50;
            this.calcerror.Text = "Calc Error";
            this.calcerror.UseVisualStyleBackColor = true;
            this.calcerror.Click += new System.EventHandler(this.calcerror_Click);
            // 
            // ExportBest
            // 
            this.ExportBest.Enabled = false;
            this.ExportBest.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ExportBest.ForeColor = System.Drawing.Color.Black;
            this.ExportBest.Location = new System.Drawing.Point(84, 17);
            this.ExportBest.Margin = new System.Windows.Forms.Padding(0);
            this.ExportBest.Name = "ExportBest";
            this.ExportBest.Size = new System.Drawing.Size(71, 22);
            this.ExportBest.TabIndex = 49;
            this.ExportBest.Text = "ExportBest";
            this.ExportBest.UseVisualStyleBackColor = true;
            this.ExportBest.Click += new System.EventHandler(this.ExportBestClick);
            // 
            // parallelanneal
            // 
            this.parallelanneal.Location = new System.Drawing.Point(13, 17);
            this.parallelanneal.Name = "parallelanneal";
            this.parallelanneal.Size = new System.Drawing.Size(59, 23);
            this.parallelanneal.TabIndex = 48;
            this.parallelanneal.Text = "|| anneal";
            this.parallelanneal.UseVisualStyleBackColor = true;
            this.parallelanneal.Click += new System.EventHandler(this.ParallelAnnealClick);
            // 
            // AQTTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 490);
            this.Controls.Add(this.AnnealPanel);
            this.Controls.Add(this.DeleteResultsButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButt);
            this.Controls.Add(this.SetupButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.StateVarLabel);
            this.Controls.Add(this.ParametersLabel);
            this.Controls.Add(this.browserButton);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.MultiSegButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.RunStatusLabel);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.DBPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StudyNameBox);
            this.Controls.Add(this.SVListBox);
            this.Controls.Add(this.outputbutton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.integrate);
            this.Controls.Add(this.loadJSON);
            this.Controls.Add(this.saveJSON);
            this.Controls.Add(this.modelRunningLabel);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(828, 465);
            this.Name = "AQTTestForm";
            this.Text = "AQUATOX.NET 1.0.0 ";
            this.Load += new System.EventHandler(this.AQTTestForm_Load);
            this.DBPanel.ResumeLayout(false);
            this.DBPanel.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.AnnealPanel.ResumeLayout(false);
            this.AnnealPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadJSON;
        private System.Windows.Forms.Button saveJSON;
        private System.Windows.Forms.Button integrate;
        private ProgressBar progressBar1;
        private Button outputbutton;
        private Button AnimalDB;
        private Button ReminDB;
        private Button ChemDB;
        private Button SiteDB;
        private Button PlantsDB;
        private ListBox SVListBox;
        private TextBox StudyNameBox;
        private Label label1;
        private Label label3;
        private Panel DBPanel;
        private Panel ButtonPanel;
        private Button ReminButton;
        private Button SiteButton;
        private Button ChemButton;
        private Button AnimButton;
        private Button PlantsButton;
        private Button Diagenesis;
        private Label RunStatusLabel;
        private Button EditButton;
        private Button DeleteButton;
        private Button FoodWebButton;
        private Button MultiSegButton;
        private Button HelpButton;
        private Label modelRunningLabel;
        private Button browserButton;
        private Label ParametersLabel;
        private Label StateVarLabel;
        private Button CancelButton;
        private Button SetupButton;
        private PictureBox pictureBox1;
        private Button OKButton;
        private Button CancelButt;
        private Button AddButton;
        private Button button3;
        private Button DeleteResultsButton;
        private Button estdbbutton;
        private Panel AnnealPanel;
        private CheckBox randomBox;
        private Label label4;
        private TextBox IterationBox;
        private Label label2;
        private TextBox TStartBox;
        private Button calcerror;
        private Button ExportBest;
        private Button parallelanneal;
    }


}

