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
            this.loadJSON = new System.Windows.Forms.Button();
            this.saveJSON = new System.Windows.Forms.Button();
            this.integrate = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.outputbutton = new System.Windows.Forms.Button();
            this.ParamsButton = new System.Windows.Forms.Button();
            this.Diagenesis = new System.Windows.Forms.Button();
            this.PlantsButton = new System.Windows.Forms.Button();
            this.AnimButton = new System.Windows.Forms.Button();
            this.ChemButton = new System.Windows.Forms.Button();
            this.SiteButton = new System.Windows.Forms.Button();
            this.ReminButton = new System.Windows.Forms.Button();
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
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadJSON
            // 
            this.loadJSON.Location = new System.Drawing.Point(26, 13);
            this.loadJSON.Name = "loadJSON";
            this.loadJSON.Size = new System.Drawing.Size(91, 24);
            this.loadJSON.TabIndex = 0;
            this.loadJSON.Text = "Load ";
            this.loadJSON.UseVisualStyleBackColor = true;
            this.loadJSON.Click += new System.EventHandler(this.loadJSON_Click);
            // 
            // saveJSON
            // 
            this.saveJSON.Location = new System.Drawing.Point(26, 51);
            this.saveJSON.Name = "saveJSON";
            this.saveJSON.Size = new System.Drawing.Size(87, 24);
            this.saveJSON.TabIndex = 0;
            this.saveJSON.Text = "Save";
            this.saveJSON.UseVisualStyleBackColor = true;
            this.saveJSON.Click += new System.EventHandler(this.saveJSON_Click);
            // 
            // integrate
            // 
            this.integrate.Location = new System.Drawing.Point(25, 181);
            this.integrate.Name = "integrate";
            this.integrate.Size = new System.Drawing.Size(91, 25);
            this.integrate.TabIndex = 1;
            this.integrate.Text = "Integrate";
            this.integrate.UseVisualStyleBackColor = true;
            this.integrate.Visible = false;
            this.integrate.Click += new System.EventHandler(this.integrate_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(25, 295);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(395, 156);
            this.textBox1.TabIndex = 2;
            this.textBox1.Visible = false;
            this.textBox1.WordWrap = false;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(142, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(655, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // outputbutton
            // 
            this.outputbutton.Location = new System.Drawing.Point(26, 229);
            this.outputbutton.Name = "outputbutton";
            this.outputbutton.Size = new System.Drawing.Size(91, 25);
            this.outputbutton.TabIndex = 1;
            this.outputbutton.Text = "Output";
            this.outputbutton.UseVisualStyleBackColor = true;
            this.outputbutton.Visible = false;
            this.outputbutton.Click += new System.EventHandler(this.graph_Click);
            // 
            // ParamsButton
            // 
            this.ParamsButton.Location = new System.Drawing.Point(190, 121);
            this.ParamsButton.Name = "ParamsButton";
            this.ParamsButton.Size = new System.Drawing.Size(91, 25);
            this.ParamsButton.TabIndex = 0;
            this.ParamsButton.Text = "Edit Setup";
            this.ParamsButton.UseVisualStyleBackColor = true;
            this.ParamsButton.Visible = false;
            this.ParamsButton.Click += new System.EventHandler(this.SaveParams);
            // 
            // Diagenesis
            // 
            this.Diagenesis.Location = new System.Drawing.Point(297, 229);
            this.Diagenesis.Name = "Diagenesis";
            this.Diagenesis.Size = new System.Drawing.Size(91, 25);
            this.Diagenesis.TabIndex = 0;
            this.Diagenesis.Text = "Diagenesis";
            this.Diagenesis.UseVisualStyleBackColor = true;
            this.Diagenesis.Visible = false;
            this.Diagenesis.Click += new System.EventHandler(this.Diagensis);
            // 
            // PlantsButton
            // 
            this.PlantsButton.Location = new System.Drawing.Point(189, 193);
            this.PlantsButton.Name = "PlantsButton";
            this.PlantsButton.Size = new System.Drawing.Size(92, 25);
            this.PlantsButton.TabIndex = 0;
            this.PlantsButton.Text = "Plants";
            this.PlantsButton.UseVisualStyleBackColor = true;
            this.PlantsButton.Visible = false;
            this.PlantsButton.Click += new System.EventHandler(this.Plants);
            // 
            // AnimButton
            // 
            this.AnimButton.Location = new System.Drawing.Point(189, 229);
            this.AnimButton.Name = "AnimButton";
            this.AnimButton.Size = new System.Drawing.Size(92, 25);
            this.AnimButton.TabIndex = 0;
            this.AnimButton.Text = "Animals";
            this.AnimButton.UseVisualStyleBackColor = true;
            this.AnimButton.Visible = false;
            this.AnimButton.Click += new System.EventHandler(this.AnimButton_Click);
            // 
            // ChemButton
            // 
            this.ChemButton.Location = new System.Drawing.Point(296, 193);
            this.ChemButton.Name = "ChemButton";
            this.ChemButton.Size = new System.Drawing.Size(92, 25);
            this.ChemButton.TabIndex = 0;
            this.ChemButton.Text = "Chemicals";
            this.ChemButton.UseVisualStyleBackColor = true;
            this.ChemButton.Visible = false;
            this.ChemButton.Click += new System.EventHandler(this.Chems);
            // 
            // SiteButton
            // 
            this.SiteButton.Location = new System.Drawing.Point(189, 157);
            this.SiteButton.Name = "SiteButton";
            this.SiteButton.Size = new System.Drawing.Size(92, 25);
            this.SiteButton.TabIndex = 0;
            this.SiteButton.Text = "Site";
            this.SiteButton.UseVisualStyleBackColor = true;
            this.SiteButton.Visible = false;
            this.SiteButton.Click += new System.EventHandler(this.Sites);
            // 
            // ReminButton
            // 
            this.ReminButton.Location = new System.Drawing.Point(297, 157);
            this.ReminButton.Name = "ReminButton";
            this.ReminButton.Size = new System.Drawing.Size(92, 25);
            this.ReminButton.TabIndex = 0;
            this.ReminButton.Text = "Org. Matter";
            this.ReminButton.UseVisualStyleBackColor = true;
            this.ReminButton.Visible = false;
            this.ReminButton.Click += new System.EventHandler(this.Remin);
            // 
            // AnimalDB
            // 
            this.AnimalDB.Cursor = System.Windows.Forms.Cursors.SizeAll;
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
            this.ReminDB.Cursor = System.Windows.Forms.Cursors.SizeAll;
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
            this.ChemDB.Cursor = System.Windows.Forms.Cursors.SizeAll;
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
            this.SiteDB.Cursor = System.Windows.Forms.Cursors.SizeAll;
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
            this.PlantsDB.Cursor = System.Windows.Forms.Cursors.SizeAll;
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
            this.SVListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SVListBox.FormattingEnabled = true;
            this.SVListBox.ItemHeight = 15;
            this.SVListBox.Location = new System.Drawing.Point(451, 103);
            this.SVListBox.Name = "SVListBox";
            this.SVListBox.Size = new System.Drawing.Size(346, 349);
            this.SVListBox.TabIndex = 9;
            this.SVListBox.Visible = false;
            this.SVListBox.SelectedIndexChanged += new System.EventHandler(this.SVListBox_SelectedIndexChanged);
            this.SVListBox.DoubleClick += new System.EventHandler(this.SVListBox_DoubleClick);
            // 
            // StudyNameBox
            // 
            this.StudyNameBox.Location = new System.Drawing.Point(142, 30);
            this.StudyNameBox.Name = "StudyNameBox";
            this.StudyNameBox.Size = new System.Drawing.Size(278, 23);
            this.StudyNameBox.TabIndex = 10;
            this.StudyNameBox.TextChanged += new System.EventHandler(this.StudyNameBox_TextChanged);
            // 
            // NetCDF
            // 
            this.NetCDF.Location = new System.Drawing.Point(463, 427);
            this.NetCDF.Name = "NetCDF";
            this.NetCDF.Size = new System.Drawing.Size(58, 25);
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
            this.label1.Click += new System.EventHandler(this.label1_Click);
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
            // AQTTestForm
            // 
            this.ClientSize = new System.Drawing.Size(812, 479);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NetCDF);
            this.Controls.Add(this.StudyNameBox);
            this.Controls.Add(this.SVListBox);
            this.Controls.Add(this.ReminButton);
            this.Controls.Add(this.SiteButton);
            this.Controls.Add(this.ChemButton);
            this.Controls.Add(this.AnimButton);
            this.Controls.Add(this.PlantsButton);
            this.Controls.Add(this.Diagenesis);
            this.Controls.Add(this.ParamsButton);
            this.Controls.Add(this.outputbutton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.integrate);
            this.Controls.Add(this.loadJSON);
            this.Controls.Add(this.saveJSON);
            this.MinimumSize = new System.Drawing.Size(828, 298);
            this.Name = "AQTTestForm";
            this.Load += new System.EventHandler(this.AQTTestForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadJSON;
        private System.Windows.Forms.Button saveJSON;
        private System.Windows.Forms.Button integrate;
        private System.Windows.Forms.TextBox textBox1;
        private ProgressBar progressBar1;
        private Button outputbutton;
        private Button ParamsButton;
        private Button Diagenesis;
        private Button PlantsButton;
        private Button AnimButton;
        private Button ChemButton;
        private Button SiteButton;
        private Button ReminButton;
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
    }


}

