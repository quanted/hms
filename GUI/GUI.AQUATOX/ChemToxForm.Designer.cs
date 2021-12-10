
namespace GUI.AQUATOX
{
    partial class ChemToxForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButt = new System.Windows.Forms.Button();
            this.changedLabel = new System.Windows.Forms.Label();
            this.HelpButton = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.AnimBioPanel = new System.Windows.Forms.Panel();
            this.ACalcK1 = new System.Windows.Forms.RadioButton();
            this.ACalcK2 = new System.Windows.Forms.RadioButton();
            this.ACalcBCF = new System.Windows.Forms.RadioButton();
            this.AK2Only = new System.Windows.Forms.RadioButton();
            this.RBLabel = new System.Windows.Forms.Label();
            this.PlantBioPanel = new System.Windows.Forms.Panel();
            this.PCalcK2 = new System.Windows.Forms.RadioButton();
            this.PCalcBCF = new System.Windows.Forms.RadioButton();
            this.PK2Only = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.PCalcK1 = new System.Windows.Forms.RadioButton();
            this.WetvsDrylabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.AnimBioPanel.SuspendLayout();
            this.PlantBioPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(26, 50);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(754, 240);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(652, 10);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(61, 23);
            this.OKButton.TabIndex = 25;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.Location = new System.Drawing.Point(719, 10);
            this.CancelButt.Name = "CancelButt";
            this.CancelButt.Size = new System.Drawing.Size(61, 23);
            this.CancelButt.TabIndex = 26;
            this.CancelButt.Text = "Cancel";
            this.CancelButt.UseVisualStyleBackColor = true;
            this.CancelButt.Click += new System.EventHandler(this.CancelButt_Click);
            // 
            // changedLabel
            // 
            this.changedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.changedLabel.AutoSize = true;
            this.changedLabel.Location = new System.Drawing.Point(582, 14);
            this.changedLabel.Name = "changedLabel";
            this.changedLabel.Size = new System.Drawing.Size(53, 15);
            this.changedLabel.TabIndex = 27;
            this.changedLabel.Text = "changed";
            this.changedLabel.Visible = false;
            // 
            // HelpButton
            // 
            this.HelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpButton.Image = global::GUI.AQUATOX.Properties.Resources.help_icon;
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(464, 8);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(87, 27);
            this.HelpButton.TabIndex = 28;
            this.HelpButton.Text = "  Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(26, 322);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 51;
            this.dataGridView2.RowTemplate.Height = 25;
            this.dataGridView2.Size = new System.Drawing.Size(754, 196);
            this.dataGridView2.TabIndex = 29;
            this.dataGridView2.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // AnimBioPanel
            // 
            this.AnimBioPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AnimBioPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AnimBioPanel.Controls.Add(this.ACalcK1);
            this.AnimBioPanel.Controls.Add(this.ACalcK2);
            this.AnimBioPanel.Controls.Add(this.ACalcBCF);
            this.AnimBioPanel.Controls.Add(this.AK2Only);
            this.AnimBioPanel.Controls.Add(this.RBLabel);
            this.AnimBioPanel.Location = new System.Drawing.Point(26, 289);
            this.AnimBioPanel.Name = "AnimBioPanel";
            this.AnimBioPanel.Size = new System.Drawing.Size(754, 21);
            this.AnimBioPanel.TabIndex = 34;
            // 
            // ACalcK1
            // 
            this.ACalcK1.AutoSize = true;
            this.ACalcK1.Location = new System.Drawing.Point(579, -1);
            this.ACalcK1.Name = "ACalcK1";
            this.ACalcK1.Size = new System.Drawing.Size(164, 19);
            this.ACalcK1.TabIndex = 21;
            this.ACalcK1.TabStop = true;
            this.ACalcK1.Text = "Calc K1 (from K2 and BCF)";
            this.ACalcK1.UseVisualStyleBackColor = true;
            this.ACalcK1.Click += new System.EventHandler(this.RB_Click);
            // 
            // ACalcK2
            // 
            this.ACalcK2.AutoSize = true;
            this.ACalcK2.Location = new System.Drawing.Point(409, -1);
            this.ACalcK2.Name = "ACalcK2";
            this.ACalcK2.Size = new System.Drawing.Size(164, 19);
            this.ACalcK2.TabIndex = 20;
            this.ACalcK2.TabStop = true;
            this.ACalcK2.Text = "Calc K2 (from K1 and BCF)";
            this.ACalcK2.UseVisualStyleBackColor = true;
            this.ACalcK2.Click += new System.EventHandler(this.RB_Click);
            // 
            // ACalcBCF
            // 
            this.ACalcBCF.AutoSize = true;
            this.ACalcBCF.Location = new System.Drawing.Point(236, -1);
            this.ACalcBCF.Name = "ACalcBCF";
            this.ACalcBCF.Size = new System.Drawing.Size(167, 19);
            this.ACalcBCF.TabIndex = 19;
            this.ACalcBCF.TabStop = true;
            this.ACalcBCF.Text = "Calc. BCF (from K1 and K2)";
            this.ACalcBCF.UseVisualStyleBackColor = true;
            this.ACalcBCF.Click += new System.EventHandler(this.RB_Click);
            // 
            // AK2Only
            // 
            this.AK2Only.AutoSize = true;
            this.AK2Only.Location = new System.Drawing.Point(67, -1);
            this.AK2Only.Name = "AK2Only";
            this.AK2Only.Size = new System.Drawing.Size(163, 19);
            this.AK2Only.TabIndex = 18;
            this.AK2Only.TabStop = true;
            this.AK2Only.Text = "Enter K2, Calc. K1 and BCF";
            this.AK2Only.UseVisualStyleBackColor = true;
            this.AK2Only.Click += new System.EventHandler(this.RB_Click);
            // 
            // RBLabel
            // 
            this.RBLabel.AutoSize = true;
            this.RBLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RBLabel.Location = new System.Drawing.Point(8, 1);
            this.RBLabel.Name = "RBLabel";
            this.RBLabel.Size = new System.Drawing.Size(53, 15);
            this.RBLabel.TabIndex = 17;
            this.RBLabel.Text = "Animals:";
            // 
            // PlantBioPanel
            // 
            this.PlantBioPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlantBioPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PlantBioPanel.Controls.Add(this.PCalcK1);
            this.PlantBioPanel.Controls.Add(this.PCalcK2);
            this.PlantBioPanel.Controls.Add(this.PCalcBCF);
            this.PlantBioPanel.Controls.Add(this.PK2Only);
            this.PlantBioPanel.Controls.Add(this.label1);
            this.PlantBioPanel.Location = new System.Drawing.Point(24, 517);
            this.PlantBioPanel.Name = "PlantBioPanel";
            this.PlantBioPanel.Size = new System.Drawing.Size(756, 21);
            this.PlantBioPanel.TabIndex = 35;
            // 
            // PCalcK2
            // 
            this.PCalcK2.AutoSize = true;
            this.PCalcK2.Location = new System.Drawing.Point(409, -1);
            this.PCalcK2.Name = "PCalcK2";
            this.PCalcK2.Size = new System.Drawing.Size(164, 19);
            this.PCalcK2.TabIndex = 20;
            this.PCalcK2.TabStop = true;
            this.PCalcK2.Text = "Calc K2 (from K1 and BCF)";
            this.PCalcK2.UseVisualStyleBackColor = true;
            this.PCalcK2.Click += new System.EventHandler(this.RB_Click);
            // 
            // PCalcBCF
            // 
            this.PCalcBCF.AutoSize = true;
            this.PCalcBCF.Location = new System.Drawing.Point(236, -1);
            this.PCalcBCF.Name = "PCalcBCF";
            this.PCalcBCF.Size = new System.Drawing.Size(167, 19);
            this.PCalcBCF.TabIndex = 19;
            this.PCalcBCF.TabStop = true;
            this.PCalcBCF.Text = "Calc. BCF (from K1 and K2)";
            this.PCalcBCF.UseVisualStyleBackColor = true;
            this.PCalcBCF.Click += new System.EventHandler(this.RB_Click);
            // 
            // PK2Only
            // 
            this.PK2Only.AutoSize = true;
            this.PK2Only.Location = new System.Drawing.Point(67, -1);
            this.PK2Only.Name = "PK2Only";
            this.PK2Only.Size = new System.Drawing.Size(163, 19);
            this.PK2Only.TabIndex = 18;
            this.PK2Only.TabStop = true;
            this.PK2Only.Text = "Enter K2, Calc. K1 and BCF";
            this.PK2Only.UseVisualStyleBackColor = true;
            this.PK2Only.Click += new System.EventHandler(this.RB_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(8, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Animals:";
            // 
            // PCalcK1
            // 
            this.PCalcK1.AutoSize = true;
            this.PCalcK1.Location = new System.Drawing.Point(581, -1);
            this.PCalcK1.Name = "PCalcK1";
            this.PCalcK1.Size = new System.Drawing.Size(164, 19);
            this.PCalcK1.TabIndex = 22;
            this.PCalcK1.TabStop = true;
            this.PCalcK1.Text = "Calc K1 (from K2 and BCF)";
            this.PCalcK1.UseVisualStyleBackColor = true;
            this.PCalcK1.Click += new System.EventHandler(this.RB_Click);
            // 
            // WetvsDrylabel
            // 
            this.WetvsDrylabel.AutoSize = true;
            this.WetvsDrylabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WetvsDrylabel.ForeColor = System.Drawing.Color.Navy;
            this.WetvsDrylabel.Location = new System.Drawing.Point(29, 29);
            this.WetvsDrylabel.Name = "WetvsDrylabel";
            this.WetvsDrylabel.Size = new System.Drawing.Size(399, 13);
            this.WetvsDrylabel.TabIndex = 36;
            this.WetvsDrylabel.Text = "K1 and BCF are entered on a dry weight basis; lipid fraction is in wet weight.";
            // 
            // ChemToxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 553);
            this.Controls.Add(this.WetvsDrylabel);
            this.Controls.Add(this.PlantBioPanel);
            this.Controls.Add(this.AnimBioPanel);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.changedLabel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButt);
            this.Controls.Add(this.dataGridView1);
            this.MinimumSize = new System.Drawing.Size(810, 580);
            this.Name = "ChemToxForm";
            this.Text = "Chemical Bioaccumulation and Toxicity";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.AnimBioPanel.ResumeLayout(false);
            this.AnimBioPanel.PerformLayout();
            this.PlantBioPanel.ResumeLayout(false);
            this.PlantBioPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Label changedLabel;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Panel AnimBioPanel;
        private System.Windows.Forms.RadioButton ACalcK1;
        private System.Windows.Forms.RadioButton ACalcK2;
        private System.Windows.Forms.RadioButton ACalcBCF;
        private System.Windows.Forms.RadioButton AK2Only;
        private System.Windows.Forms.Label RBLabel;
        private System.Windows.Forms.Panel PlantBioPanel;
        private System.Windows.Forms.RadioButton PCalcK2;
        private System.Windows.Forms.RadioButton PCalcBCF;
        private System.Windows.Forms.RadioButton PK2Only;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton PCalcK1;
        private System.Windows.Forms.Label WetvsDrylabel;
    }
}