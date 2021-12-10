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
            this.NetCDF = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.FoodWebButton = new System.Windows.Forms.Button();
            this.ReminButton = new System.Windows.Forms.Button();
            this.SiteButton = new System.Windows.Forms.Button();
            this.ChemButton = new System.Windows.Forms.Button();
            this.AnimButton = new System.Windows.Forms.Button();
            this.PlantsButton = new System.Windows.Forms.Button();
            this.Diagenesis = new System.Windows.Forms.Button();
            this.ParamsButton = new System.Windows.Forms.Button();
            this.RunStatusLabel = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.MultiSegButton = new System.Windows.Forms.Button();
            this.HelpButton = new System.Windows.Forms.Button();
            this.modelRunningLabel = new System.Windows.Forms.Label();
            this.browserButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
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
            this.integrate.Location = new System.Drawing.Point(26, 88);
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
            this.progressBar1.Size = new System.Drawing.Size(659, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Value = 1;
            this.progressBar1.Visible = false;
            // 
            // outputbutton
            // 
            this.outputbutton.Location = new System.Drawing.Point(26, 271);
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
            this.AnimalDB.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.ReminDB.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.ChemDB.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.SiteDB.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.PlantsDB.Cursor = System.Windows.Forms.Cursors.Default;
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
            this.SVListBox.Location = new System.Drawing.Point(459, 111);
            this.SVListBox.Name = "SVListBox";
            this.SVListBox.Size = new System.Drawing.Size(346, 259);
            this.SVListBox.TabIndex = 9;
            this.SVListBox.Visible = false;
            this.SVListBox.SelectedIndexChanged += new System.EventHandler(this.SVListBox_SelectedIndexChanged);
            this.SVListBox.DoubleClick += new System.EventHandler(this.SVListBox_DoubleClick);
            // 
            // StudyNameBox
            // 
            this.StudyNameBox.Location = new System.Drawing.Point(146, 30);
            this.StudyNameBox.Name = "StudyNameBox";
            this.StudyNameBox.Size = new System.Drawing.Size(287, 23);
            this.StudyNameBox.TabIndex = 10;
            this.StudyNameBox.TextChanged += new System.EventHandler(this.StudyNameBox_TextChanged);
            // 
            // NetCDF
            // 
            this.NetCDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NetCDF.Location = new System.Drawing.Point(357, 381);
            this.NetCDF.Name = "NetCDF";
            this.NetCDF.Size = new System.Drawing.Size(56, 25);
            this.NetCDF.TabIndex = 11;
            this.NetCDF.Text = "NetCDF";
            this.NetCDF.UseVisualStyleBackColor = true;
            this.NetCDF.Visible = false;
            this.NetCDF.Click += new System.EventHandler(this.NetCDF_Click);
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
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.PlantsDB);
            this.panel1.Controls.Add(this.SiteDB);
            this.panel1.Controls.Add(this.ChemDB);
            this.panel1.Controls.Add(this.ReminDB);
            this.panel1.Controls.Add(this.AnimalDB);
            this.panel1.Location = new System.Drawing.Point(463, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 55);
            this.panel1.TabIndex = 16;
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
            this.ButtonPanel.Controls.Add(this.ParamsButton);
            this.ButtonPanel.Location = new System.Drawing.Point(191, 110);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(244, 259);
            this.ButtonPanel.TabIndex = 17;
            this.ButtonPanel.Visible = false;
            // 
            // FoodWebButton
            // 
            this.FoodWebButton.Location = new System.Drawing.Point(22, 203);
            this.FoodWebButton.Name = "FoodWebButton";
            this.FoodWebButton.Size = new System.Drawing.Size(92, 25);
            this.FoodWebButton.TabIndex = 8;
            this.FoodWebButton.Text = "Food Web";
            this.FoodWebButton.UseVisualStyleBackColor = true;
            this.FoodWebButton.Click += new System.EventHandler(this.FoodWebButton_Click);
            // 
            // ReminButton
            // 
            this.ReminButton.Location = new System.Drawing.Point(130, 70);
            this.ReminButton.Name = "ReminButton";
            this.ReminButton.Size = new System.Drawing.Size(92, 25);
            this.ReminButton.TabIndex = 2;
            this.ReminButton.Text = "Org. Matter";
            this.ReminButton.UseVisualStyleBackColor = true;
            this.ReminButton.Click += new System.EventHandler(this.Remin);
            // 
            // SiteButton
            // 
            this.SiteButton.Location = new System.Drawing.Point(22, 70);
            this.SiteButton.Name = "SiteButton";
            this.SiteButton.Size = new System.Drawing.Size(92, 25);
            this.SiteButton.TabIndex = 3;
            this.SiteButton.Text = "Site";
            this.SiteButton.UseVisualStyleBackColor = true;
            this.SiteButton.Click += new System.EventHandler(this.Sites);
            // 
            // ChemButton
            // 
            this.ChemButton.Location = new System.Drawing.Point(129, 106);
            this.ChemButton.Name = "ChemButton";
            this.ChemButton.Size = new System.Drawing.Size(92, 25);
            this.ChemButton.TabIndex = 4;
            this.ChemButton.Text = "Chemicals";
            this.ChemButton.UseVisualStyleBackColor = true;
            this.ChemButton.Click += new System.EventHandler(this.Chems);
            // 
            // AnimButton
            // 
            this.AnimButton.Location = new System.Drawing.Point(22, 142);
            this.AnimButton.Name = "AnimButton";
            this.AnimButton.Size = new System.Drawing.Size(92, 25);
            this.AnimButton.TabIndex = 5;
            this.AnimButton.Text = "Animals";
            this.AnimButton.UseVisualStyleBackColor = true;
            this.AnimButton.Click += new System.EventHandler(this.AnimButton_Click);
            // 
            // PlantsButton
            // 
            this.PlantsButton.Location = new System.Drawing.Point(22, 106);
            this.PlantsButton.Name = "PlantsButton";
            this.PlantsButton.Size = new System.Drawing.Size(92, 25);
            this.PlantsButton.TabIndex = 6;
            this.PlantsButton.Text = "Plants";
            this.PlantsButton.UseVisualStyleBackColor = true;
            this.PlantsButton.Click += new System.EventHandler(this.Plants);
            // 
            // Diagenesis
            // 
            this.Diagenesis.Location = new System.Drawing.Point(130, 142);
            this.Diagenesis.Name = "Diagenesis";
            this.Diagenesis.Size = new System.Drawing.Size(91, 25);
            this.Diagenesis.TabIndex = 7;
            this.Diagenesis.Text = "Diagenesis";
            this.Diagenesis.UseVisualStyleBackColor = true;
            this.Diagenesis.Click += new System.EventHandler(this.Diagensis);
            // 
            // ParamsButton
            // 
            this.ParamsButton.Location = new System.Drawing.Point(22, 20);
            this.ParamsButton.Name = "ParamsButton";
            this.ParamsButton.Size = new System.Drawing.Size(91, 25);
            this.ParamsButton.TabIndex = 1;
            this.ParamsButton.Text = "Edit Setup";
            this.ParamsButton.UseVisualStyleBackColor = true;
            this.ParamsButton.Click += new System.EventHandler(this.Setup_Click);
            // 
            // RunStatusLabel
            // 
            this.RunStatusLabel.AutoSize = true;
            this.RunStatusLabel.Location = new System.Drawing.Point(28, 122);
            this.RunStatusLabel.Name = "RunStatusLabel";
            this.RunStatusLabel.Size = new System.Drawing.Size(63, 15);
            this.RunStatusLabel.TabIndex = 18;
            this.RunStatusLabel.Text = "Run Status";
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.AddButton.Location = new System.Drawing.Point(462, 382);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(60, 25);
            this.AddButton.TabIndex = 21;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Visible = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.EditButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.EditButton.Location = new System.Drawing.Point(602, 382);
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
            this.DeleteButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DeleteButton.Location = new System.Drawing.Point(532, 382);
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
            this.MultiSegButton.Location = new System.Drawing.Point(26, 381);
            this.MultiSegButton.Name = "MultiSegButton";
            this.MultiSegButton.Size = new System.Drawing.Size(138, 24);
            this.MultiSegButton.TabIndex = 22;
            this.MultiSegButton.Text = "Multi-Segment Runs";
            this.MultiSegButton.UseVisualStyleBackColor = true;
            this.MultiSegButton.Click += new System.EventHandler(this.MultiSegButton_Click);
            // 
            // HelpButton
            // 
            this.HelpButton.Image = global::GUI.AQUATOX.Properties.Resources.help_icon;
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(26, 190);
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
            this.browserButton.Location = new System.Drawing.Point(26, 222);
            this.browserButton.Margin = new System.Windows.Forms.Padding(0);
            this.browserButton.Name = "browserButton";
            this.browserButton.Size = new System.Drawing.Size(87, 23);
            this.browserButton.TabIndex = 25;
            this.browserButton.Text = "Select Browser";
            this.browserButton.UseVisualStyleBackColor = true;
            this.browserButton.Click += new System.EventHandler(this.browserButton_Click);
            // 
            // AQTTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 428);
            this.Controls.Add(this.browserButton);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.MultiSegButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.RunStatusLabel);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NetCDF);
            this.Controls.Add(this.StudyNameBox);
            this.Controls.Add(this.SVListBox);
            this.Controls.Add(this.outputbutton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.integrate);
            this.Controls.Add(this.loadJSON);
            this.Controls.Add(this.saveJSON);
            this.Controls.Add(this.modelRunningLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(828, 465);
            this.Name = "AQTTestForm";
            this.Text = "AQUATOX.NET 1.0.0 ";
            this.Load += new System.EventHandler(this.AQTTestForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
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
        private Button NetCDF;
        private Label label1;
        private Label label3;
        private Panel panel1;
        private Panel ButtonPanel;
        private Button ReminButton;
        private Button SiteButton;
        private Button ChemButton;
        private Button AnimButton;
        private Button PlantsButton;
        private Button Diagenesis;
        private Button ParamsButton;
        private Label RunStatusLabel;
        private Button AddButton;
        private Button EditButton;
        private Button DeleteButton;
        private Button FoodWebButton;
        private Button MultiSegButton;
        private Button HelpButton;
        private Label modelRunningLabel;
        private Button browserButton;
    }


}

