
namespace GUI.AQUATOX
{
    partial class LoadingsForm
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
            this.ICLabel = new System.Windows.Forms.Label();
            this.ICEdit = new System.Windows.Forms.TextBox();
            this.SVNameLabel = new System.Windows.Forms.Label();
            this.ICUnit = new System.Windows.Forms.Label();
            this.IgnoreLoadingsBox = new System.Windows.Forms.CheckBox();
            this.LoadingsPanel = new System.Windows.Forms.Panel();
            this.RBPanel = new System.Windows.Forms.Panel();
            this.RB3 = new System.Windows.Forms.RadioButton();
            this.RB2 = new System.Windows.Forms.RadioButton();
            this.RB1 = new System.Windows.Forms.RadioButton();
            this.RB0 = new System.Windows.Forms.RadioButton();
            this.RBLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ToxICUnitLabel = new System.Windows.Forms.Label();
            this.ToxICLabel = new System.Windows.Forms.Label();
            this.ToxIC = new System.Windows.Forms.TextBox();
            this.TSUnit = new System.Windows.Forms.Label();
            this.LTLabel = new System.Windows.Forms.Label();
            this.LTBox = new System.Windows.Forms.ComboBox();
            this.UseTimeSeriesRadio = new System.Windows.Forms.RadioButton();
            this.UseConstRadio = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.MultLoadBox = new System.Windows.Forms.TextBox();
            this.CLUnit = new System.Windows.Forms.Label();
            this.ConstLoadBox = new System.Windows.Forms.TextBox();
            this.NotesEdit2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NotesEdit = new System.Windows.Forms.TextBox();
            this.HMS_Button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.File_Import = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ParameterButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButt = new System.Windows.Forms.Button();
            this.AmmoniaDriveLabel = new System.Windows.Forms.Label();
            this.LoadingsPanel.SuspendLayout();
            this.RBPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ICLabel
            // 
            this.ICLabel.AutoSize = true;
            this.ICLabel.Location = new System.Drawing.Point(13, 41);
            this.ICLabel.Name = "ICLabel";
            this.ICLabel.Size = new System.Drawing.Size(92, 15);
            this.ICLabel.TabIndex = 15;
            this.ICLabel.Text = "Initial Condition";
            // 
            // ICEdit
            // 
            this.ICEdit.Location = new System.Drawing.Point(111, 38);
            this.ICEdit.Name = "ICEdit";
            this.ICEdit.Size = new System.Drawing.Size(95, 23);
            this.ICEdit.TabIndex = 14;
            this.ICEdit.TextChanged += new System.EventHandler(this.ICEdit_TextChanged);
            // 
            // SVNameLabel
            // 
            this.SVNameLabel.AutoSize = true;
            this.SVNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SVNameLabel.Location = new System.Drawing.Point(12, 9);
            this.SVNameLabel.Name = "SVNameLabel";
            this.SVNameLabel.Size = new System.Drawing.Size(120, 15);
            this.SVNameLabel.TabIndex = 16;
            this.SVNameLabel.Text = "State Variable Name";
            // 
            // ICUnit
            // 
            this.ICUnit.AutoSize = true;
            this.ICUnit.Location = new System.Drawing.Point(212, 41);
            this.ICUnit.Name = "ICUnit";
            this.ICUnit.Size = new System.Drawing.Size(33, 15);
            this.ICUnit.TabIndex = 17;
            this.ICUnit.Text = "units";
            // 
            // IgnoreLoadingsBox
            // 
            this.IgnoreLoadingsBox.AutoSize = true;
            this.IgnoreLoadingsBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.IgnoreLoadingsBox.Location = new System.Drawing.Point(13, 67);
            this.IgnoreLoadingsBox.Name = "IgnoreLoadingsBox";
            this.IgnoreLoadingsBox.Size = new System.Drawing.Size(131, 19);
            this.IgnoreLoadingsBox.TabIndex = 18;
            this.IgnoreLoadingsBox.Text = "Ignore All Loadings";
            this.IgnoreLoadingsBox.UseVisualStyleBackColor = true;
            this.IgnoreLoadingsBox.CheckedChanged += new System.EventHandler(this.IgnoreLoadingsBox_CheckedChanged);
            // 
            // LoadingsPanel
            // 
            this.LoadingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LoadingsPanel.Controls.Add(this.RBPanel);
            this.LoadingsPanel.Controls.Add(this.panel1);
            this.LoadingsPanel.Controls.Add(this.HMS_Button);
            this.LoadingsPanel.Controls.Add(this.label2);
            this.LoadingsPanel.Controls.Add(this.File_Import);
            this.LoadingsPanel.Controls.Add(this.dataGridView1);
            this.LoadingsPanel.Location = new System.Drawing.Point(12, 101);
            this.LoadingsPanel.Name = "LoadingsPanel";
            this.LoadingsPanel.Size = new System.Drawing.Size(677, 425);
            this.LoadingsPanel.TabIndex = 21;
            this.LoadingsPanel.Visible = false;
            this.LoadingsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ButtonPanel_Paint);
            // 
            // RBPanel
            // 
            this.RBPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RBPanel.Controls.Add(this.RB3);
            this.RBPanel.Controls.Add(this.RB2);
            this.RBPanel.Controls.Add(this.RB1);
            this.RBPanel.Controls.Add(this.RB0);
            this.RBPanel.Controls.Add(this.RBLabel);
            this.RBPanel.Location = new System.Drawing.Point(13, 13);
            this.RBPanel.Name = "RBPanel";
            this.RBPanel.Size = new System.Drawing.Size(345, 130);
            this.RBPanel.TabIndex = 33;
            // 
            // RB3
            // 
            this.RB3.AutoSize = true;
            this.RB3.Location = new System.Drawing.Point(38, 97);
            this.RB3.Name = "RB3";
            this.RB3.Size = new System.Drawing.Size(236, 19);
            this.RB3.TabIndex = 21;
            this.RB3.TabStop = true;
            this.RB3.Text = "Use Known Vals. (discharge is fn. inflow)";
            this.RB3.UseVisualStyleBackColor = true;
            this.RB3.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RB2
            // 
            this.RB2.AutoSize = true;
            this.RB2.Location = new System.Drawing.Point(38, 75);
            this.RB2.Name = "RB2";
            this.RB2.Size = new System.Drawing.Size(259, 19);
            this.RB2.TabIndex = 20;
            this.RB2.TabStop = true;
            this.RB2.Text = "Calculate (vol. is fn. inflow, discharge, evap.)";
            this.RB2.UseVisualStyleBackColor = true;
            this.RB2.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RB1
            // 
            this.RB1.AutoSize = true;
            this.RB1.Location = new System.Drawing.Point(38, 53);
            this.RB1.Name = "RB1";
            this.RB1.Size = new System.Drawing.Size(228, 19);
            this.RB1.TabIndex = 19;
            this.RB1.TabStop = true;
            this.RB1.Text = "Keep Constant (discharge is fn. inflow)";
            this.RB1.UseVisualStyleBackColor = true;
            this.RB1.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RB0
            // 
            this.RB0.AutoSize = true;
            this.RB0.Location = new System.Drawing.Point(38, 32);
            this.RB0.Name = "RB0";
            this.RB0.Size = new System.Drawing.Size(219, 19);
            this.RB0.TabIndex = 18;
            this.RB0.TabStop = true;
            this.RB0.Text = "Manning\'s Eqn. (vol. is fn. discharge)";
            this.RB0.UseVisualStyleBackColor = true;
            this.RB0.CheckedChanged += new System.EventHandler(this.RB_Changed);
            // 
            // RBLabel
            // 
            this.RBLabel.AutoSize = true;
            this.RBLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RBLabel.Location = new System.Drawing.Point(8, 8);
            this.RBLabel.Name = "RBLabel";
            this.RBLabel.Size = new System.Drawing.Size(95, 15);
            this.RBLabel.TabIndex = 17;
            this.RBLabel.Text = "Volume Options";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ToxICUnitLabel);
            this.panel1.Controls.Add(this.ToxICLabel);
            this.panel1.Controls.Add(this.ToxIC);
            this.panel1.Controls.Add(this.TSUnit);
            this.panel1.Controls.Add(this.LTLabel);
            this.panel1.Controls.Add(this.LTBox);
            this.panel1.Controls.Add(this.UseTimeSeriesRadio);
            this.panel1.Controls.Add(this.UseConstRadio);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.MultLoadBox);
            this.panel1.Controls.Add(this.CLUnit);
            this.panel1.Controls.Add(this.ConstLoadBox);
            this.panel1.Controls.Add(this.NotesEdit2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.NotesEdit);
            this.panel1.Location = new System.Drawing.Point(13, 158);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 247);
            this.panel1.TabIndex = 32;
            // 
            // ToxICUnitLabel
            // 
            this.ToxICUnitLabel.AutoSize = true;
            this.ToxICUnitLabel.Location = new System.Drawing.Point(269, 44);
            this.ToxICUnitLabel.Name = "ToxICUnitLabel";
            this.ToxICUnitLabel.Size = new System.Drawing.Size(33, 15);
            this.ToxICUnitLabel.TabIndex = 46;
            this.ToxICUnitLabel.Text = "units";
            this.ToxICUnitLabel.Visible = false;
            // 
            // ToxICLabel
            // 
            this.ToxICLabel.AutoSize = true;
            this.ToxICLabel.Location = new System.Drawing.Point(70, 44);
            this.ToxICLabel.Name = "ToxICLabel";
            this.ToxICLabel.Size = new System.Drawing.Size(92, 15);
            this.ToxICLabel.TabIndex = 45;
            this.ToxICLabel.Text = "Initial Condition";
            this.ToxICLabel.Visible = false;
            // 
            // ToxIC
            // 
            this.ToxIC.Location = new System.Drawing.Point(185, 41);
            this.ToxIC.Name = "ToxIC";
            this.ToxIC.Size = new System.Drawing.Size(78, 23);
            this.ToxIC.TabIndex = 44;
            this.ToxIC.Visible = false;
            this.ToxIC.TextChanged += new System.EventHandler(this.ToxIC_TextChanged);
            // 
            // TSUnit
            // 
            this.TSUnit.AutoSize = true;
            this.TSUnit.Location = new System.Drawing.Point(270, 109);
            this.TSUnit.Name = "TSUnit";
            this.TSUnit.Size = new System.Drawing.Size(33, 15);
            this.TSUnit.TabIndex = 43;
            this.TSUnit.Text = "units";
            // 
            // LTLabel
            // 
            this.LTLabel.AutoSize = true;
            this.LTLabel.Location = new System.Drawing.Point(9, 15);
            this.LTLabel.Name = "LTLabel";
            this.LTLabel.Size = new System.Drawing.Size(77, 15);
            this.LTLabel.TabIndex = 42;
            this.LTLabel.Text = "Loading Type";
            // 
            // LTBox
            // 
            this.LTBox.FormattingEnabled = true;
            this.LTBox.Location = new System.Drawing.Point(107, 12);
            this.LTBox.Name = "LTBox";
            this.LTBox.Size = new System.Drawing.Size(157, 23);
            this.LTBox.TabIndex = 41;
            this.LTBox.SelectedIndexChanged += new System.EventHandler(this.LTBox_SelectedIndexChanged);
            // 
            // UseTimeSeriesRadio
            // 
            this.UseTimeSeriesRadio.AutoSize = true;
            this.UseTimeSeriesRadio.Location = new System.Drawing.Point(24, 105);
            this.UseTimeSeriesRadio.Name = "UseTimeSeriesRadio";
            this.UseTimeSeriesRadio.Size = new System.Drawing.Size(153, 19);
            this.UseTimeSeriesRadio.TabIndex = 40;
            this.UseTimeSeriesRadio.TabStop = true;
            this.UseTimeSeriesRadio.Text = "Use Time-series Loading";
            this.UseTimeSeriesRadio.UseVisualStyleBackColor = true;
            this.UseTimeSeriesRadio.CheckedChanged += new System.EventHandler(this.UseConstRadio_CheckedChanged);
            // 
            // UseConstRadio
            // 
            this.UseConstRadio.AutoSize = true;
            this.UseConstRadio.Location = new System.Drawing.Point(24, 76);
            this.UseConstRadio.Name = "UseConstRadio";
            this.UseConstRadio.Size = new System.Drawing.Size(138, 19);
            this.UseConstRadio.TabIndex = 39;
            this.UseConstRadio.TabStop = true;
            this.UseConstRadio.Text = "Use Constant Load of";
            this.UseConstRadio.UseVisualStyleBackColor = true;
            this.UseConstRadio.CheckedChanged += new System.EventHandler(this.UseConstRadio_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 15);
            this.label5.TabIndex = 38;
            this.label5.Text = "Multiply Loading by:";
            // 
            // MultLoadBox
            // 
            this.MultLoadBox.Location = new System.Drawing.Point(168, 134);
            this.MultLoadBox.Name = "MultLoadBox";
            this.MultLoadBox.Size = new System.Drawing.Size(62, 23);
            this.MultLoadBox.TabIndex = 37;
            this.MultLoadBox.TextChanged += new System.EventHandler(this.MultLoadBox_TextChanged);
            // 
            // CLUnit
            // 
            this.CLUnit.AutoSize = true;
            this.CLUnit.Location = new System.Drawing.Point(270, 78);
            this.CLUnit.Name = "CLUnit";
            this.CLUnit.Size = new System.Drawing.Size(33, 15);
            this.CLUnit.TabIndex = 36;
            this.CLUnit.Text = "units";
            // 
            // ConstLoadBox
            // 
            this.ConstLoadBox.Location = new System.Drawing.Point(184, 72);
            this.ConstLoadBox.Name = "ConstLoadBox";
            this.ConstLoadBox.Size = new System.Drawing.Size(79, 23);
            this.ConstLoadBox.TabIndex = 35;
            this.ConstLoadBox.TextChanged += new System.EventHandler(this.ConstLoadBox_TextChanged);
            // 
            // NotesEdit2
            // 
            this.NotesEdit2.Location = new System.Drawing.Point(52, 210);
            this.NotesEdit2.Name = "NotesEdit2";
            this.NotesEdit2.Size = new System.Drawing.Size(277, 23);
            this.NotesEdit2.TabIndex = 34;
            this.NotesEdit2.TextChanged += new System.EventHandler(this.NotesEdit2_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 33;
            this.label1.Text = "Notes:";
            // 
            // NotesEdit
            // 
            this.NotesEdit.Location = new System.Drawing.Point(52, 184);
            this.NotesEdit.Name = "NotesEdit";
            this.NotesEdit.Size = new System.Drawing.Size(277, 23);
            this.NotesEdit.TabIndex = 32;
            this.NotesEdit.TextChanged += new System.EventHandler(this.NotesEdit_TextChanged);
            // 
            // HMS_Button
            // 
            this.HMS_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HMS_Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HMS_Button.Location = new System.Drawing.Point(536, 392);
            this.HMS_Button.Name = "HMS_Button";
            this.HMS_Button.Size = new System.Drawing.Size(57, 23);
            this.HMS_Button.TabIndex = 30;
            this.HMS_Button.Text = "HMS";
            this.HMS_Button.UseVisualStyleBackColor = true;
            this.HMS_Button.Click += new System.EventHandler(this.HMS_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(373, 396);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 15);
            this.label2.TabIndex = 29;
            this.label2.Text = "Import from";
            // 
            // File_Import
            // 
            this.File_Import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.File_Import.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.File_Import.Location = new System.Drawing.Point(462, 392);
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
            this.dataGridView1.Location = new System.Drawing.Point(371, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(285, 371);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // ParameterButton
            // 
            this.ParameterButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ParameterButton.Location = new System.Drawing.Point(376, 12);
            this.ParameterButton.Name = "ParameterButton";
            this.ParameterButton.Size = new System.Drawing.Size(90, 23);
            this.ParameterButton.TabIndex = 22;
            this.ParameterButton.Text = "Parameters";
            this.ParameterButton.UseVisualStyleBackColor = true;
            this.ParameterButton.Click += new System.EventHandler(this.ParameterButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(563, 12);
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
            this.CancelButt.Location = new System.Drawing.Point(630, 12);
            this.CancelButt.Name = "CancelButt";
            this.CancelButt.Size = new System.Drawing.Size(61, 23);
            this.CancelButt.TabIndex = 24;
            this.CancelButt.Text = "Cancel";
            this.CancelButt.UseVisualStyleBackColor = true;
            this.CancelButt.Click += new System.EventHandler(this.CancelButt_Click);
            // 
            // AmmoniaDriveLabel
            // 
            this.AmmoniaDriveLabel.AutoSize = true;
            this.AmmoniaDriveLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AmmoniaDriveLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.AmmoniaDriveLabel.Location = new System.Drawing.Point(284, 71);
            this.AmmoniaDriveLabel.Name = "AmmoniaDriveLabel";
            this.AmmoniaDriveLabel.Size = new System.Drawing.Size(346, 15);
            this.AmmoniaDriveLabel.TabIndex = 25;
            this.AmmoniaDriveLabel.Text = "Ammonia Selected as a Driving Variable in the Setup Window";
            // 
            // LoadingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 538);
            this.Controls.Add(this.AmmoniaDriveLabel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButt);
            this.Controls.Add(this.ParameterButton);
            this.Controls.Add(this.LoadingsPanel);
            this.Controls.Add(this.IgnoreLoadingsBox);
            this.Controls.Add(this.ICUnit);
            this.Controls.Add(this.SVNameLabel);
            this.Controls.Add(this.ICLabel);
            this.Controls.Add(this.ICEdit);
            this.MinimumSize = new System.Drawing.Size(692, 410);
            this.Name = "LoadingsForm";
            this.Text = "Loadings Form";
            this.Load += new System.EventHandler(this.GridForm_Load);
            this.LoadingsPanel.ResumeLayout(false);
            this.LoadingsPanel.PerformLayout();
            this.RBPanel.ResumeLayout(false);
            this.RBPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ICLabel;
        private System.Windows.Forms.TextBox ICEdit;
        private System.Windows.Forms.Label SVNameLabel;
        private System.Windows.Forms.Label ICUnit;
        private System.Windows.Forms.CheckBox IgnoreLoadingsBox;
        private System.Windows.Forms.Panel LoadingsPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button ParameterButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Label AmmoniaDriveLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button File_Import;
        private System.Windows.Forms.Button HMS_Button;
        private System.Windows.Forms.Panel RBPanel;
        private System.Windows.Forms.RadioButton RB2;
        private System.Windows.Forms.RadioButton RB1;
        private System.Windows.Forms.RadioButton RB0;
        private System.Windows.Forms.Label RBLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label TSUnit;
        private System.Windows.Forms.Label LTLabel;
        private System.Windows.Forms.ComboBox LTBox;
        private System.Windows.Forms.RadioButton UseTimeSeriesRadio;
        private System.Windows.Forms.RadioButton UseConstRadio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox MultLoadBox;
        private System.Windows.Forms.Label CLUnit;
        private System.Windows.Forms.TextBox ConstLoadBox;
        private System.Windows.Forms.TextBox NotesEdit2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NotesEdit;
        private System.Windows.Forms.RadioButton RB3;
        private System.Windows.Forms.Label ToxICUnitLabel;
        private System.Windows.Forms.Label ToxICLabel;
        private System.Windows.Forms.TextBox ToxIC;
    }
}