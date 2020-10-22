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
            this.graphbutton = new System.Windows.Forms.Button();
            this.ParamsButton = new System.Windows.Forms.Button();
            this.Diagenesis = new System.Windows.Forms.Button();
            this.PlantsButton = new System.Windows.Forms.Button();
            this.AnimButton = new System.Windows.Forms.Button();
            this.ChemButton = new System.Windows.Forms.Button();
            this.SiteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadJSON
            // 
            this.loadJSON.Location = new System.Drawing.Point(26, 21);
            this.loadJSON.Name = "loadJSON";
            this.loadJSON.Size = new System.Drawing.Size(91, 24);
            this.loadJSON.TabIndex = 0;
            this.loadJSON.Text = "Load JSON";
            this.loadJSON.UseVisualStyleBackColor = true;
            this.loadJSON.Click += new System.EventHandler(this.loadJSON_Click);
            // 
            // saveJSON
            // 
            this.saveJSON.Location = new System.Drawing.Point(130, 21);
            this.saveJSON.Name = "saveJSON";
            this.saveJSON.Size = new System.Drawing.Size(87, 24);
            this.saveJSON.TabIndex = 0;
            this.saveJSON.Text = "Save JSON";
            this.saveJSON.UseVisualStyleBackColor = true;
            this.saveJSON.Click += new System.EventHandler(this.saveJSON_Click);
            // 
            // integrate
            // 
            this.integrate.Location = new System.Drawing.Point(26, 57);
            this.integrate.Name = "integrate";
            this.integrate.Size = new System.Drawing.Size(85, 25);
            this.integrate.TabIndex = 1;
            this.integrate.Text = "Integrate";
            this.integrate.UseVisualStyleBackColor = true;
            this.integrate.Click += new System.EventHandler(this.integrate_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(20, 96);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(831, 141);
            this.textBox1.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Dial;
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(568, 57);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(284, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // graphbutton
            // 
            this.graphbutton.Location = new System.Drawing.Point(133, 57);
            this.graphbutton.Name = "graphbutton";
            this.graphbutton.Size = new System.Drawing.Size(85, 25);
            this.graphbutton.TabIndex = 1;
            this.graphbutton.Text = "Graph";
            this.graphbutton.UseVisualStyleBackColor = true;
            this.graphbutton.Click += new System.EventHandler(this.graph_Click);
            // 
            // ParamsButton
            // 
            this.ParamsButton.Location = new System.Drawing.Point(237, 20);
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
            this.Diagenesis.Location = new System.Drawing.Point(451, 21);
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
            this.PlantsButton.Location = new System.Drawing.Point(344, 21);
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
            this.AnimButton.Location = new System.Drawing.Point(344, 57);
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
            this.ChemButton.Location = new System.Drawing.Point(451, 57);
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
            this.SiteButton.Location = new System.Drawing.Point(236, 57);
            this.SiteButton.Name = "SiteButton";
            this.SiteButton.Size = new System.Drawing.Size(92, 25);
            this.SiteButton.TabIndex = 0;
            this.SiteButton.Text = "Site";
            this.SiteButton.UseVisualStyleBackColor = true;
            this.SiteButton.Visible = false;
            this.SiteButton.Click += new System.EventHandler(this.Sites);
            // 
            // AQTTestForm
            // 
            this.ClientSize = new System.Drawing.Size(871, 577);
            this.Controls.Add(this.SiteButton);
            this.Controls.Add(this.ChemButton);
            this.Controls.Add(this.AnimButton);
            this.Controls.Add(this.PlantsButton);
            this.Controls.Add(this.Diagenesis);
            this.Controls.Add(this.ParamsButton);
            this.Controls.Add(this.graphbutton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.integrate);
            this.Controls.Add(this.loadJSON);
            this.Controls.Add(this.saveJSON);
            this.Name = "AQTTestForm";
            this.Load += new System.EventHandler(this.AQTTestForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadJSON;
        private System.Windows.Forms.Button saveJSON;
        private System.Windows.Forms.Button integrate;
        private System.Windows.Forms.TextBox textBox1;
        private ProgressBar progressBar1;
        private Button graphbutton;
        private Button ParamsButton;
        private Button Diagenesis;
        private Button PlantsButton;
        private Button AnimButton;
        private Button ChemButton;
        private Button SiteButton;
    }


}

