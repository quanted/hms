
namespace GUI.AQUATOX
{
    partial class SiteForm
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
            this.LoadingsPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.File_Import = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.RBPanel = new System.Windows.Forms.Panel();
            this.RBMarine = new System.Windows.Forms.RadioButton();
            this.RBEncl = new System.Windows.Forms.RadioButton();
            this.RBRes = new System.Windows.Forms.RadioButton();
            this.RBStream = new System.Windows.Forms.RadioButton();
            this.RBLake = new System.Windows.Forms.RadioButton();
            this.RBPond = new System.Windows.Forms.RadioButton();
            this.RBLabel = new System.Windows.Forms.Label();
            this.ParameterButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButt = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ConstLoadBox = new System.Windows.Forms.TextBox();
            this.LTLabel = new System.Windows.Forms.Label();
            this.LTBox = new System.Windows.Forms.ComboBox();
            this.UseTimeSeriesRadio = new System.Windows.Forms.RadioButton();
            this.UseConstRadio = new System.Windows.Forms.RadioButton();
            this.LoadingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.RBPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadingsPanel
            // 
            this.LoadingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LoadingsPanel.Controls.Add(this.label2);
            this.LoadingsPanel.Controls.Add(this.File_Import);
            this.LoadingsPanel.Controls.Add(this.dataGridView1);
            this.LoadingsPanel.Location = new System.Drawing.Point(178, 54);
            this.LoadingsPanel.Name = "LoadingsPanel";
            this.LoadingsPanel.Size = new System.Drawing.Size(324, 446);
            this.LoadingsPanel.TabIndex = 21;
            this.LoadingsPanel.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 418);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 29;
            this.label2.Text = "Import from";
            // 
            // File_Import
            // 
            this.File_Import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.File_Import.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.File_Import.Location = new System.Drawing.Point(109, 414);
            this.File_Import.Name = "File_Import";
            this.File_Import.Size = new System.Drawing.Size(57, 23);
            this.File_Import.TabIndex = 28;
            this.File_Import.Text = "File";
            this.File_Import.UseVisualStyleBackColor = true;
            this.File_Import.Click += new System.EventHandler(this.File_Import_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(31, 79);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(266, 327);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // RBPanel
            // 
            this.RBPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RBPanel.Controls.Add(this.RBMarine);
            this.RBPanel.Controls.Add(this.RBEncl);
            this.RBPanel.Controls.Add(this.RBRes);
            this.RBPanel.Controls.Add(this.RBStream);
            this.RBPanel.Controls.Add(this.RBLake);
            this.RBPanel.Controls.Add(this.RBPond);
            this.RBPanel.Controls.Add(this.RBLabel);
            this.RBPanel.Location = new System.Drawing.Point(12, 54);
            this.RBPanel.Name = "RBPanel";
            this.RBPanel.Size = new System.Drawing.Size(152, 197);
            this.RBPanel.TabIndex = 33;
            // 
            // RBMarine
            // 
            this.RBMarine.AutoSize = true;
            this.RBMarine.Location = new System.Drawing.Point(38, 143);
            this.RBMarine.Name = "RBMarine";
            this.RBMarine.Size = new System.Drawing.Size(62, 19);
            this.RBMarine.TabIndex = 23;
            this.RBMarine.TabStop = true;
            this.RBMarine.Text = "Marine";
            this.RBMarine.UseVisualStyleBackColor = true;
            this.RBMarine.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBEncl
            // 
            this.RBEncl.AutoSize = true;
            this.RBEncl.Location = new System.Drawing.Point(38, 121);
            this.RBEncl.Name = "RBEncl";
            this.RBEncl.Size = new System.Drawing.Size(76, 19);
            this.RBEncl.TabIndex = 22;
            this.RBEncl.TabStop = true;
            this.RBEncl.Text = "Enclosure";
            this.RBEncl.UseVisualStyleBackColor = true;
            this.RBEncl.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBRes
            // 
            this.RBRes.AutoSize = true;
            this.RBRes.Location = new System.Drawing.Point(38, 97);
            this.RBRes.Name = "RBRes";
            this.RBRes.Size = new System.Drawing.Size(73, 19);
            this.RBRes.TabIndex = 21;
            this.RBRes.TabStop = true;
            this.RBRes.Text = "Reservoir";
            this.RBRes.UseVisualStyleBackColor = true;
            this.RBRes.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBStream
            // 
            this.RBStream.AutoSize = true;
            this.RBStream.Location = new System.Drawing.Point(38, 75);
            this.RBStream.Name = "RBStream";
            this.RBStream.Size = new System.Drawing.Size(62, 19);
            this.RBStream.TabIndex = 20;
            this.RBStream.TabStop = true;
            this.RBStream.Text = "Stream";
            this.RBStream.UseVisualStyleBackColor = true;
            this.RBStream.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBLake
            // 
            this.RBLake.AutoSize = true;
            this.RBLake.Location = new System.Drawing.Point(38, 53);
            this.RBLake.Name = "RBLake";
            this.RBLake.Size = new System.Drawing.Size(49, 19);
            this.RBLake.TabIndex = 19;
            this.RBLake.TabStop = true;
            this.RBLake.Text = "Lake";
            this.RBLake.UseVisualStyleBackColor = true;
            this.RBLake.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBPond
            // 
            this.RBPond.AutoSize = true;
            this.RBPond.Location = new System.Drawing.Point(38, 32);
            this.RBPond.Name = "RBPond";
            this.RBPond.Size = new System.Drawing.Size(53, 19);
            this.RBPond.TabIndex = 18;
            this.RBPond.TabStop = true;
            this.RBPond.Text = "Pond";
            this.RBPond.UseVisualStyleBackColor = true;
            this.RBPond.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBLabel
            // 
            this.RBLabel.AutoSize = true;
            this.RBLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RBLabel.Location = new System.Drawing.Point(8, 8);
            this.RBLabel.Name = "RBLabel";
            this.RBLabel.Size = new System.Drawing.Size(58, 15);
            this.RBLabel.TabIndex = 17;
            this.RBLabel.Text = "Site Type";
            // 
            // ParameterButton
            // 
            this.ParameterButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ParameterButton.Location = new System.Drawing.Point(14, 12);
            this.ParameterButton.Name = "ParameterButton";
            this.ParameterButton.Size = new System.Drawing.Size(130, 23);
            this.ParameterButton.TabIndex = 22;
            this.ParameterButton.Text = "Site Parameters";
            this.ParameterButton.UseVisualStyleBackColor = true;
            this.ParameterButton.Click += new System.EventHandler(this.ParameterButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(526, 12);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(61, 23);
            this.OKButton.TabIndex = 23;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButt.Location = new System.Drawing.Point(593, 12);
            this.CancelButt.Name = "CancelButt";
            this.CancelButt.Size = new System.Drawing.Size(61, 23);
            this.CancelButt.TabIndex = 24;
            this.CancelButt.Text = "Cancel";
            this.CancelButt.UseVisualStyleBackColor = true;
            this.CancelButt.Click += new System.EventHandler(this.CancelButt_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ConstLoadBox);
            this.panel1.Controls.Add(this.LTLabel);
            this.panel1.Controls.Add(this.LTBox);
            this.panel1.Controls.Add(this.UseTimeSeriesRadio);
            this.panel1.Controls.Add(this.UseConstRadio);
            this.panel1.Location = new System.Drawing.Point(178, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(324, 103);
            this.panel1.TabIndex = 34;
            // 
            // ConstLoadBox
            // 
            this.ConstLoadBox.Location = new System.Drawing.Point(192, 45);
            this.ConstLoadBox.Name = "ConstLoadBox";
            this.ConstLoadBox.Size = new System.Drawing.Size(79, 23);
            this.ConstLoadBox.TabIndex = 43;
            this.ConstLoadBox.Visible = false;
            this.ConstLoadBox.TextChanged += new System.EventHandler(this.ConstLoadBox_TextChanged);
            // 
            // LTLabel
            // 
            this.LTLabel.AutoSize = true;
            this.LTLabel.Location = new System.Drawing.Point(11, 15);
            this.LTLabel.Name = "LTLabel";
            this.LTLabel.Size = new System.Drawing.Size(88, 15);
            this.LTLabel.TabIndex = 42;
            this.LTLabel.Text = "Site Time Series";
            // 
            // LTBox
            // 
            this.LTBox.FormattingEnabled = true;
            this.LTBox.Location = new System.Drawing.Point(107, 12);
            this.LTBox.Name = "LTBox";
            this.LTBox.Size = new System.Drawing.Size(176, 23);
            this.LTBox.TabIndex = 41;
            this.LTBox.SelectedIndexChanged += new System.EventHandler(this.LTBox_SelectedIndexChanged);
            // 
            // UseTimeSeriesRadio
            // 
            this.UseTimeSeriesRadio.AutoSize = true;
            this.UseTimeSeriesRadio.Location = new System.Drawing.Point(45, 74);
            this.UseTimeSeriesRadio.Name = "UseTimeSeriesRadio";
            this.UseTimeSeriesRadio.Size = new System.Drawing.Size(203, 19);
            this.UseTimeSeriesRadio.TabIndex = 40;
            this.UseTimeSeriesRadio.TabStop = true;
            this.UseTimeSeriesRadio.Text = "Use Time-series of Velocity (cm/s)";
            this.UseTimeSeriesRadio.UseVisualStyleBackColor = true;
            this.UseTimeSeriesRadio.CheckedChanged += new System.EventHandler(this.UseConstRadio_CheckedChanged);
            // 
            // UseConstRadio
            // 
            this.UseConstRadio.AutoSize = true;
            this.UseConstRadio.Location = new System.Drawing.Point(45, 46);
            this.UseConstRadio.Name = "UseConstRadio";
            this.UseConstRadio.Size = new System.Drawing.Size(118, 19);
            this.UseConstRadio.TabIndex = 39;
            this.UseConstRadio.TabStop = true;
            this.UseConstRadio.Text = "Calculate Velocity";
            this.UseConstRadio.UseVisualStyleBackColor = true;
            this.UseConstRadio.CheckedChanged += new System.EventHandler(this.UseConstRadio_CheckedChanged);
            // 
            // SiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 512);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RBPanel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButt);
            this.Controls.Add(this.ParameterButton);
            this.Controls.Add(this.LoadingsPanel);
            this.MinimumSize = new System.Drawing.Size(692, 410);
            this.Name = "SiteForm";
            this.Text = "Site Information";
            this.LoadingsPanel.ResumeLayout(false);
            this.LoadingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.RBPanel.ResumeLayout(false);
            this.RBPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel LoadingsPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button ParameterButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button File_Import;
        private System.Windows.Forms.Panel RBPanel;
        private System.Windows.Forms.RadioButton RBStream;
        private System.Windows.Forms.RadioButton RBLake;
        private System.Windows.Forms.RadioButton RBPond;
        private System.Windows.Forms.Label RBLabel;
        private System.Windows.Forms.RadioButton RBRes;
        private System.Windows.Forms.RadioButton RBMarine;
        private System.Windows.Forms.RadioButton RBEncl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LTLabel;
        private System.Windows.Forms.ComboBox LTBox;
        private System.Windows.Forms.RadioButton UseTimeSeriesRadio;
        private System.Windows.Forms.RadioButton UseConstRadio;
        private System.Windows.Forms.TextBox ConstLoadBox;
    }
}