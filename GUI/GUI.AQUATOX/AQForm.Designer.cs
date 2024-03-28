using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;


namespace GUI.AQUATOX
{
    partial class AQTMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AQTMainForm));
            loadJSON = new Button();
            saveJSON = new Button();
            integrate = new Button();
            progressBar1 = new ProgressBar();
            outputbutton = new Button();
            AnimalDB = new Button();
            ReminDB = new Button();
            ChemDB = new Button();
            SiteDB = new Button();
            PlantsDB = new Button();
            SVListBox = new ListBox();
            StudyNameBox = new TextBox();
            label1 = new Label();
            label3 = new Label();
            DBPanel = new Panel();
            ButtonPanel = new Panel();
            FoodWebButton = new Button();
            ReminButton = new Button();
            SiteButton = new Button();
            ChemButton = new Button();
            AnimButton = new Button();
            PlantsButton = new Button();
            Diagenesis = new Button();
            RunStatusLabel = new Label();
            AddButton = new Button();
            EditButton = new Button();
            DeleteButton = new Button();
            Help_Button = new Button();
            modelRunningLabel = new Label();
            browserButton = new Button();
            ParametersLabel = new Label();
            StateVarLabel = new Label();
            Cancel_Button = new Button();
            SetupButton = new Button();
            pictureBox1 = new PictureBox();
            OKButton = new Button();
            CancelButt = new Button();
            DBPanel.SuspendLayout();
            ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // loadJSON
            // 
            loadJSON.Location = new System.Drawing.Point(26, 11);
            loadJSON.Name = "loadJSON";
            loadJSON.Size = new System.Drawing.Size(87, 24);
            loadJSON.TabIndex = 0;
            loadJSON.Text = "Load ";
            loadJSON.UseVisualStyleBackColor = true;
            loadJSON.Click += loadJSON_Click;
            // 
            // saveJSON
            // 
            saveJSON.Enabled = false;
            saveJSON.Location = new System.Drawing.Point(26, 43);
            saveJSON.Name = "saveJSON";
            saveJSON.Size = new System.Drawing.Size(87, 24);
            saveJSON.TabIndex = 0;
            saveJSON.Text = "Save";
            saveJSON.UseVisualStyleBackColor = true;
            saveJSON.Click += saveJSON_Click;
            // 
            // integrate
            // 
            integrate.Location = new System.Drawing.Point(26, 152);
            integrate.Name = "integrate";
            integrate.Size = new System.Drawing.Size(87, 25);
            integrate.TabIndex = 1;
            integrate.Text = "Run";
            integrate.UseVisualStyleBackColor = true;
            integrate.Visible = false;
            integrate.Click += integrate_Click;
            // 
            // progressBar1
            // 
            progressBar1.AccessibleRole = AccessibleRole.Dial;
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(146, 76);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(579, 23);
            progressBar1.Step = 1;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 3;
            progressBar1.Value = 1;
            progressBar1.Visible = false;
            // 
            // outputbutton
            // 
            outputbutton.Location = new System.Drawing.Point(26, 321);
            outputbutton.Name = "outputbutton";
            outputbutton.Size = new System.Drawing.Size(87, 25);
            outputbutton.TabIndex = 1;
            outputbutton.Text = "Output";
            outputbutton.UseVisualStyleBackColor = true;
            outputbutton.Visible = false;
            outputbutton.Click += graph_Click;
            // 
            // AnimalDB
            // 
            AnimalDB.Location = new System.Drawing.Point(76, 21);
            AnimalDB.Name = "AnimalDB";
            AnimalDB.Size = new System.Drawing.Size(60, 25);
            AnimalDB.TabIndex = 0;
            AnimalDB.Text = "Animals";
            AnimalDB.UseVisualStyleBackColor = true;
            AnimalDB.Click += AnimDB_Click;
            // 
            // ReminDB
            // 
            ReminDB.Location = new System.Drawing.Point(142, 21);
            ReminDB.Name = "ReminDB";
            ReminDB.Size = new System.Drawing.Size(60, 25);
            ReminDB.TabIndex = 5;
            ReminDB.Text = "Remin. ";
            ReminDB.UseVisualStyleBackColor = true;
            ReminDB.Click += ReminDB_Click;
            // 
            // ChemDB
            // 
            ChemDB.Location = new System.Drawing.Point(208, 20);
            ChemDB.Name = "ChemDB";
            ChemDB.Size = new System.Drawing.Size(60, 25);
            ChemDB.TabIndex = 6;
            ChemDB.Text = "Chems";
            ChemDB.UseVisualStyleBackColor = true;
            ChemDB.Click += ChemDB_Click;
            // 
            // SiteDB
            // 
            SiteDB.Location = new System.Drawing.Point(10, 21);
            SiteDB.Name = "SiteDB";
            SiteDB.Size = new System.Drawing.Size(60, 25);
            SiteDB.TabIndex = 7;
            SiteDB.Text = "Sites";
            SiteDB.UseVisualStyleBackColor = true;
            SiteDB.Click += SiteDB_Click;
            // 
            // PlantsDB
            // 
            PlantsDB.Location = new System.Drawing.Point(274, 20);
            PlantsDB.Name = "PlantsDB";
            PlantsDB.Size = new System.Drawing.Size(60, 25);
            PlantsDB.TabIndex = 8;
            PlantsDB.Text = "Plants";
            PlantsDB.UseVisualStyleBackColor = true;
            PlantsDB.Click += PlantsDB_Click;
            // 
            // SVListBox
            // 
            SVListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SVListBox.FormattingEnabled = true;
            SVListBox.ItemHeight = 15;
            SVListBox.Location = new System.Drawing.Point(440, 126);
            SVListBox.Name = "SVListBox";
            SVListBox.Size = new System.Drawing.Size(356, 319);
            SVListBox.TabIndex = 9;
            SVListBox.Visible = false;
            SVListBox.SelectedIndexChanged += SVListBox_SelectedIndexChanged;
            SVListBox.DoubleClick += SVListBox_DoubleClick;
            // 
            // StudyNameBox
            // 
            StudyNameBox.Location = new System.Drawing.Point(146, 27);
            StudyNameBox.Name = "StudyNameBox";
            StudyNameBox.Size = new System.Drawing.Size(287, 23);
            StudyNameBox.TabIndex = 10;
            StudyNameBox.TextChanged += StudyNameBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(142, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(113, 15);
            label1.TabIndex = 13;
            label1.Text = "Name of Simulation";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 2);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(167, 15);
            label3.TabIndex = 15;
            label3.Text = "Databases of Parameter Values";
            // 
            // DBPanel
            // 
            DBPanel.BorderStyle = BorderStyle.FixedSingle;
            DBPanel.Controls.Add(label3);
            DBPanel.Controls.Add(PlantsDB);
            DBPanel.Controls.Add(SiteDB);
            DBPanel.Controls.Add(ChemDB);
            DBPanel.Controls.Add(ReminDB);
            DBPanel.Controls.Add(AnimalDB);
            DBPanel.Location = new System.Drawing.Point(453, 9);
            DBPanel.Name = "DBPanel";
            DBPanel.Size = new System.Drawing.Size(343, 55);
            DBPanel.TabIndex = 16;
            // 
            // ButtonPanel
            // 
            ButtonPanel.BorderStyle = BorderStyle.FixedSingle;
            ButtonPanel.Controls.Add(FoodWebButton);
            ButtonPanel.Controls.Add(ReminButton);
            ButtonPanel.Controls.Add(SiteButton);
            ButtonPanel.Controls.Add(ChemButton);
            ButtonPanel.Controls.Add(AnimButton);
            ButtonPanel.Controls.Add(PlantsButton);
            ButtonPanel.Controls.Add(Diagenesis);
            ButtonPanel.Location = new System.Drawing.Point(163, 127);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new System.Drawing.Size(244, 182);
            ButtonPanel.TabIndex = 17;
            ButtonPanel.Visible = false;
            // 
            // FoodWebButton
            // 
            FoodWebButton.Location = new System.Drawing.Point(66, 130);
            FoodWebButton.Name = "FoodWebButton";
            FoodWebButton.Size = new System.Drawing.Size(92, 25);
            FoodWebButton.TabIndex = 8;
            FoodWebButton.Text = "Food Web";
            FoodWebButton.UseVisualStyleBackColor = true;
            FoodWebButton.Click += FoodWebButton_Click;
            // 
            // ReminButton
            // 
            ReminButton.Location = new System.Drawing.Point(127, 14);
            ReminButton.Name = "ReminButton";
            ReminButton.Size = new System.Drawing.Size(92, 25);
            ReminButton.TabIndex = 2;
            ReminButton.Text = "Org. Matter";
            ReminButton.UseVisualStyleBackColor = true;
            ReminButton.Click += Remin;
            // 
            // SiteButton
            // 
            SiteButton.Location = new System.Drawing.Point(19, 14);
            SiteButton.Name = "SiteButton";
            SiteButton.Size = new System.Drawing.Size(92, 25);
            SiteButton.TabIndex = 3;
            SiteButton.Text = "Site";
            SiteButton.UseVisualStyleBackColor = true;
            SiteButton.Click += Sites;
            // 
            // ChemButton
            // 
            ChemButton.Location = new System.Drawing.Point(126, 50);
            ChemButton.Name = "ChemButton";
            ChemButton.Size = new System.Drawing.Size(92, 25);
            ChemButton.TabIndex = 4;
            ChemButton.Text = "Chemicals";
            ChemButton.UseVisualStyleBackColor = true;
            ChemButton.Click += Chems;
            // 
            // AnimButton
            // 
            AnimButton.Location = new System.Drawing.Point(19, 86);
            AnimButton.Name = "AnimButton";
            AnimButton.Size = new System.Drawing.Size(92, 25);
            AnimButton.TabIndex = 5;
            AnimButton.Text = "Animals";
            AnimButton.UseVisualStyleBackColor = true;
            AnimButton.Click += AnimButton_Click;
            // 
            // PlantsButton
            // 
            PlantsButton.Location = new System.Drawing.Point(19, 50);
            PlantsButton.Name = "PlantsButton";
            PlantsButton.Size = new System.Drawing.Size(92, 25);
            PlantsButton.TabIndex = 6;
            PlantsButton.Text = "Plants";
            PlantsButton.UseVisualStyleBackColor = true;
            PlantsButton.Click += Plants;
            // 
            // Diagenesis
            // 
            Diagenesis.Location = new System.Drawing.Point(127, 86);
            Diagenesis.Name = "Diagenesis";
            Diagenesis.Size = new System.Drawing.Size(91, 25);
            Diagenesis.TabIndex = 7;
            Diagenesis.Text = "Diagenesis";
            Diagenesis.UseVisualStyleBackColor = true;
            Diagenesis.Click += Diagensis;
            // 
            // RunStatusLabel
            // 
            RunStatusLabel.AutoSize = true;
            RunStatusLabel.Location = new System.Drawing.Point(28, 187);
            RunStatusLabel.Name = "RunStatusLabel";
            RunStatusLabel.Size = new System.Drawing.Size(63, 15);
            RunStatusLabel.TabIndex = 18;
            RunStatusLabel.Text = "Run Status";
            // 
            // AddButton
            // 
            AddButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            AddButton.Location = new System.Drawing.Point(444, 450);
            AddButton.Name = "AddButton";
            AddButton.Size = new System.Drawing.Size(60, 25);
            AddButton.TabIndex = 21;
            AddButton.Text = "Add";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Visible = false;
            AddButton.Click += AddButton_Click;
            // 
            // EditButton
            // 
            EditButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            EditButton.Location = new System.Drawing.Point(584, 450);
            EditButton.Name = "EditButton";
            EditButton.Size = new System.Drawing.Size(60, 25);
            EditButton.TabIndex = 20;
            EditButton.Text = "Edit";
            EditButton.UseVisualStyleBackColor = true;
            EditButton.Visible = false;
            EditButton.Click += EditButton_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            DeleteButton.Location = new System.Drawing.Point(514, 450);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new System.Drawing.Size(60, 25);
            DeleteButton.TabIndex = 19;
            DeleteButton.Text = "Delete";
            DeleteButton.UseVisualStyleBackColor = true;
            DeleteButton.Visible = false;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // HelpButton
            // 
            Help_Button.Image = Properties.Resources.help_icon;
            Help_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Help_Button.Location = new System.Drawing.Point(26, 240);
            Help_Button.Name = "HelpButton";
            Help_Button.Size = new System.Drawing.Size(87, 28);
            Help_Button.TabIndex = 23;
            Help_Button.Text = "  Help";
            Help_Button.UseVisualStyleBackColor = true;
            Help_Button.Click += HelpButton_Click;
            // 
            // modelRunningLabel
            // 
            modelRunningLabel.AutoSize = true;
            modelRunningLabel.Location = new System.Drawing.Point(145, 60);
            modelRunningLabel.Name = "modelRunningLabel";
            modelRunningLabel.Size = new System.Drawing.Size(100, 15);
            modelRunningLabel.TabIndex = 24;
            modelRunningLabel.Text = "Model is Running";
            modelRunningLabel.Visible = false;
            // 
            // browserButton
            // 
            browserButton.Font = new System.Drawing.Font("Arial Narrow", 8.25F);
            browserButton.ForeColor = System.Drawing.Color.Black;
            browserButton.Location = new System.Drawing.Point(26, 272);
            browserButton.Margin = new Padding(0);
            browserButton.Name = "browserButton";
            browserButton.Size = new System.Drawing.Size(87, 23);
            browserButton.TabIndex = 25;
            browserButton.Text = "Select Browser";
            browserButton.UseVisualStyleBackColor = true;
            browserButton.Click += browserButton_Click;
            // 
            // ParametersLabel
            // 
            ParametersLabel.AutoSize = true;
            ParametersLabel.Location = new System.Drawing.Point(163, 110);
            ParametersLabel.Name = "ParametersLabel";
            ParametersLabel.Size = new System.Drawing.Size(103, 15);
            ParametersLabel.TabIndex = 26;
            ParametersLabel.Text = "Model Parameters";
            ParametersLabel.Visible = false;
            // 
            // StateVarLabel
            // 
            StateVarLabel.AutoSize = true;
            StateVarLabel.Location = new System.Drawing.Point(441, 108);
            StateVarLabel.Name = "StateVarLabel";
            StateVarLabel.Size = new System.Drawing.Size(193, 15);
            StateVarLabel.TabIndex = 27;
            StateVarLabel.Text = "Model State Variables and Loadings";
            StateVarLabel.Visible = false;
            // 
            // CancelButton
            // 
            Cancel_Button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Cancel_Button.Font = new System.Drawing.Font("Arial Narrow", 9.75F);
            Cancel_Button.ForeColor = System.Drawing.Color.Black;
            Cancel_Button.Location = new System.Drawing.Point(727, 75);
            Cancel_Button.Margin = new Padding(0);
            Cancel_Button.Name = "CancelButton";
            Cancel_Button.Size = new System.Drawing.Size(68, 25);
            Cancel_Button.TabIndex = 28;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.UseVisualStyleBackColor = true;
            Cancel_Button.Visible = false;
            Cancel_Button.Click += CancelButton_Click;
            // 
            // SetupButton
            // 
            SetupButton.Location = new System.Drawing.Point(22, 98);
            SetupButton.Name = "SetupButton";
            SetupButton.Size = new System.Drawing.Size(91, 25);
            SetupButton.TabIndex = 29;
            SetupButton.Text = "Edit Setup";
            SetupButton.UseVisualStyleBackColor = true;
            SetupButton.Visible = false;
            SetupButton.Click += Setup_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(220, 116);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(468, 288);
            pictureBox1.TabIndex = 30;
            pictureBox1.TabStop = false;
            // 
            // OKButton
            // 
            OKButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            OKButton.DialogResult = DialogResult.OK;
            OKButton.Location = new System.Drawing.Point(659, 11);
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size(61, 23);
            OKButton.TabIndex = 35;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Visible = false;
            // 
            // CancelButt
            // 
            CancelButt.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CancelButt.Location = new System.Drawing.Point(735, 11);
            CancelButt.Name = "CancelButt";
            CancelButt.Size = new System.Drawing.Size(61, 23);
            CancelButt.TabIndex = 36;
            CancelButt.Text = "Cancel";
            CancelButt.UseVisualStyleBackColor = true;
            CancelButt.Visible = false;
            CancelButt.Click += CancelButt_Click;
            // 
            // AQTMainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(813, 490);
            Controls.Add(OKButton);
            Controls.Add(CancelButt);
            Controls.Add(SetupButton);
            Controls.Add(Cancel_Button);
            Controls.Add(StateVarLabel);
            Controls.Add(ParametersLabel);
            Controls.Add(browserButton);
            Controls.Add(Help_Button);
            Controls.Add(AddButton);
            Controls.Add(EditButton);
            Controls.Add(DeleteButton);
            Controls.Add(RunStatusLabel);
            Controls.Add(ButtonPanel);
            Controls.Add(DBPanel);
            Controls.Add(label1);
            Controls.Add(StudyNameBox);
            Controls.Add(SVListBox);
            Controls.Add(outputbutton);
            Controls.Add(progressBar1);
            Controls.Add(integrate);
            Controls.Add(loadJSON);
            Controls.Add(saveJSON);
            Controls.Add(modelRunningLabel);
            Controls.Add(pictureBox1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MinimumSize = new System.Drawing.Size(828, 465);
            Name = "AQTMainForm";
            Text = "AQUATOX.NET 1.0.001 ";
            FormClosing += AQTTestForm_FormClosing;
            Load += AQTTestForm_Load;
            DBPanel.ResumeLayout(false);
            DBPanel.PerformLayout();
            ButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button loadJSON;
        private Button saveJSON;
        private Button integrate;
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
        private Button AddButton;
        private Button EditButton;
        private Button DeleteButton;
        private Button FoodWebButton;
        private Button Help_Button;
        private Label modelRunningLabel;
        private Button browserButton;
        private Label ParametersLabel;
        private Label StateVarLabel;
        private Button Cancel_Button;
        private Button SetupButton;
        private PictureBox pictureBox1;
        private Button OKButton;
        private Button CancelButt;
    }


}

