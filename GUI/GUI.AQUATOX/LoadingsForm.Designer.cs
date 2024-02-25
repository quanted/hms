
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
            ICLabel = new System.Windows.Forms.Label();
            ICEdit = new System.Windows.Forms.TextBox();
            SVNameLabel = new System.Windows.Forms.Label();
            ICUnit = new System.Windows.Forms.Label();
            IgnoreLoadingsBox = new System.Windows.Forms.CheckBox();
            LoadingsPanel = new System.Windows.Forms.Panel();
            timeSeriesLabel = new System.Windows.Forms.Label();
            RBPanel = new System.Windows.Forms.Panel();
            RB3 = new System.Windows.Forms.RadioButton();
            RB2 = new System.Windows.Forms.RadioButton();
            RB1 = new System.Windows.Forms.RadioButton();
            RB0 = new System.Windows.Forms.RadioButton();
            RBLabel = new System.Windows.Forms.Label();
            LTPanel = new System.Windows.Forms.Panel();
            ToxICUnitLabel = new System.Windows.Forms.Label();
            ToxICLabel = new System.Windows.Forms.Label();
            ToxIC = new System.Windows.Forms.TextBox();
            TSUnit = new System.Windows.Forms.Label();
            LTLabel = new System.Windows.Forms.Label();
            LTBox = new System.Windows.Forms.ComboBox();
            UseTimeSeriesRadio = new System.Windows.Forms.RadioButton();
            UseConstRadio = new System.Windows.Forms.RadioButton();
            label5 = new System.Windows.Forms.Label();
            MultLoadBox = new System.Windows.Forms.TextBox();
            CLUnit = new System.Windows.Forms.Label();
            ConstLoadBox = new System.Windows.Forms.TextBox();
            NotesEdit2 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            NotesEdit = new System.Windows.Forms.TextBox();
            HMS_Button = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            File_Import = new System.Windows.Forms.Button();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            ParameterButton = new System.Windows.Forms.Button();
            OKButton = new System.Windows.Forms.Button();
            CancelButt = new System.Windows.Forms.Button();
            WarningLabel = new System.Windows.Forms.Label();
            HelpButton = new System.Windows.Forms.Button();
            ToxicityButton = new System.Windows.Forms.Button();
            LoadingsPanel.SuspendLayout();
            RBPanel.SuspendLayout();
            LTPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // ICLabel
            // 
            ICLabel.AutoSize = true;
            ICLabel.Location = new System.Drawing.Point(13, 41);
            ICLabel.Name = "ICLabel";
            ICLabel.Size = new System.Drawing.Size(92, 15);
            ICLabel.TabIndex = 15;
            ICLabel.Text = "Initial Condition";
            // 
            // ICEdit
            // 
            ICEdit.Location = new System.Drawing.Point(111, 38);
            ICEdit.Name = "ICEdit";
            ICEdit.Size = new System.Drawing.Size(95, 23);
            ICEdit.TabIndex = 14;
            ICEdit.TextChanged += ICEdit_TextChanged;
            // 
            // SVNameLabel
            // 
            SVNameLabel.AutoSize = true;
            SVNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            SVNameLabel.Location = new System.Drawing.Point(12, 9);
            SVNameLabel.Name = "SVNameLabel";
            SVNameLabel.Size = new System.Drawing.Size(120, 15);
            SVNameLabel.TabIndex = 16;
            SVNameLabel.Text = "State Variable Name";
            // 
            // ICUnit
            // 
            ICUnit.AutoSize = true;
            ICUnit.Location = new System.Drawing.Point(212, 41);
            ICUnit.Name = "ICUnit";
            ICUnit.Size = new System.Drawing.Size(33, 15);
            ICUnit.TabIndex = 17;
            ICUnit.Text = "units";
            // 
            // IgnoreLoadingsBox
            // 
            IgnoreLoadingsBox.AutoSize = true;
            IgnoreLoadingsBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            IgnoreLoadingsBox.Location = new System.Drawing.Point(13, 67);
            IgnoreLoadingsBox.Name = "IgnoreLoadingsBox";
            IgnoreLoadingsBox.Size = new System.Drawing.Size(131, 19);
            IgnoreLoadingsBox.TabIndex = 18;
            IgnoreLoadingsBox.Text = "Ignore All Loadings";
            IgnoreLoadingsBox.UseVisualStyleBackColor = true;
            IgnoreLoadingsBox.CheckedChanged += IgnoreLoadingsBox_CheckedChanged;
            // 
            // LoadingsPanel
            // 
            LoadingsPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            LoadingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LoadingsPanel.Controls.Add(timeSeriesLabel);
            LoadingsPanel.Controls.Add(RBPanel);
            LoadingsPanel.Controls.Add(LTPanel);
            LoadingsPanel.Controls.Add(HMS_Button);
            LoadingsPanel.Controls.Add(label2);
            LoadingsPanel.Controls.Add(File_Import);
            LoadingsPanel.Controls.Add(dataGridView1);
            LoadingsPanel.Location = new System.Drawing.Point(12, 101);
            LoadingsPanel.Name = "LoadingsPanel";
            LoadingsPanel.Size = new System.Drawing.Size(652, 334);
            LoadingsPanel.TabIndex = 21;
            LoadingsPanel.Visible = false;
            // 
            // timeSeriesLabel
            // 
            timeSeriesLabel.AutoSize = true;
            timeSeriesLabel.Location = new System.Drawing.Point(373, 4);
            timeSeriesLabel.Name = "timeSeriesLabel";
            timeSeriesLabel.Size = new System.Drawing.Size(97, 15);
            timeSeriesLabel.TabIndex = 34;
            timeSeriesLabel.Text = "Time Series Input";
            // 
            // RBPanel
            // 
            RBPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            RBPanel.Controls.Add(RB3);
            RBPanel.Controls.Add(RB2);
            RBPanel.Controls.Add(RB1);
            RBPanel.Controls.Add(RB0);
            RBPanel.Controls.Add(RBLabel);
            RBPanel.Location = new System.Drawing.Point(13, 13);
            RBPanel.Name = "RBPanel";
            RBPanel.Size = new System.Drawing.Size(345, 130);
            RBPanel.TabIndex = 33;
            // 
            // RB3
            // 
            RB3.AutoSize = true;
            RB3.Location = new System.Drawing.Point(38, 97);
            RB3.Name = "RB3";
            RB3.Size = new System.Drawing.Size(236, 19);
            RB3.TabIndex = 21;
            RB3.TabStop = true;
            RB3.Text = "Use Known Vals. (discharge is fn. inflow)";
            RB3.UseVisualStyleBackColor = true;
            RB3.CheckedChanged += RB_Changed;
            // 
            // RB2
            // 
            RB2.AutoSize = true;
            RB2.Location = new System.Drawing.Point(38, 75);
            RB2.Name = "RB2";
            RB2.Size = new System.Drawing.Size(259, 19);
            RB2.TabIndex = 20;
            RB2.TabStop = true;
            RB2.Text = "Calculate (vol. is fn. inflow, discharge, evap.)";
            RB2.UseVisualStyleBackColor = true;
            RB2.CheckedChanged += RB_Changed;
            // 
            // RB1
            // 
            RB1.AutoSize = true;
            RB1.Location = new System.Drawing.Point(38, 53);
            RB1.Name = "RB1";
            RB1.Size = new System.Drawing.Size(228, 19);
            RB1.TabIndex = 19;
            RB1.TabStop = true;
            RB1.Text = "Keep Constant (discharge is fn. inflow)";
            RB1.UseVisualStyleBackColor = true;
            RB1.CheckedChanged += RB_Changed;
            // 
            // RB0
            // 
            RB0.AutoSize = true;
            RB0.Location = new System.Drawing.Point(38, 32);
            RB0.Name = "RB0";
            RB0.Size = new System.Drawing.Size(219, 19);
            RB0.TabIndex = 18;
            RB0.TabStop = true;
            RB0.Text = "Manning's Eqn. (vol. is fn. discharge)";
            RB0.UseVisualStyleBackColor = true;
            RB0.CheckedChanged += RB_Changed;
            // 
            // RBLabel
            // 
            RBLabel.AutoSize = true;
            RBLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            RBLabel.Location = new System.Drawing.Point(8, 8);
            RBLabel.Name = "RBLabel";
            RBLabel.Size = new System.Drawing.Size(95, 15);
            RBLabel.TabIndex = 17;
            RBLabel.Text = "Volume Options";
            // 
            // LTPanel
            // 
            LTPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            LTPanel.Controls.Add(ToxICUnitLabel);
            LTPanel.Controls.Add(ToxICLabel);
            LTPanel.Controls.Add(ToxIC);
            LTPanel.Controls.Add(TSUnit);
            LTPanel.Controls.Add(LTLabel);
            LTPanel.Controls.Add(LTBox);
            LTPanel.Controls.Add(UseTimeSeriesRadio);
            LTPanel.Controls.Add(UseConstRadio);
            LTPanel.Controls.Add(label5);
            LTPanel.Controls.Add(MultLoadBox);
            LTPanel.Controls.Add(CLUnit);
            LTPanel.Controls.Add(ConstLoadBox);
            LTPanel.Controls.Add(NotesEdit2);
            LTPanel.Controls.Add(label1);
            LTPanel.Controls.Add(NotesEdit);
            LTPanel.Location = new System.Drawing.Point(13, 158);
            LTPanel.Name = "LTPanel";
            LTPanel.Size = new System.Drawing.Size(345, 247);
            LTPanel.TabIndex = 32;
            // 
            // ToxICUnitLabel
            // 
            ToxICUnitLabel.AutoSize = true;
            ToxICUnitLabel.Location = new System.Drawing.Point(269, 44);
            ToxICUnitLabel.Name = "ToxICUnitLabel";
            ToxICUnitLabel.Size = new System.Drawing.Size(33, 15);
            ToxICUnitLabel.TabIndex = 46;
            ToxICUnitLabel.Text = "units";
            ToxICUnitLabel.Visible = false;
            // 
            // ToxICLabel
            // 
            ToxICLabel.AutoSize = true;
            ToxICLabel.Location = new System.Drawing.Point(70, 44);
            ToxICLabel.Name = "ToxICLabel";
            ToxICLabel.Size = new System.Drawing.Size(92, 15);
            ToxICLabel.TabIndex = 45;
            ToxICLabel.Text = "Initial Condition";
            ToxICLabel.Visible = false;
            // 
            // ToxIC
            // 
            ToxIC.Location = new System.Drawing.Point(185, 41);
            ToxIC.Name = "ToxIC";
            ToxIC.Size = new System.Drawing.Size(78, 23);
            ToxIC.TabIndex = 44;
            ToxIC.Visible = false;
            ToxIC.TextChanged += ToxIC_TextChanged;
            // 
            // TSUnit
            // 
            TSUnit.AutoSize = true;
            TSUnit.Location = new System.Drawing.Point(270, 109);
            TSUnit.Name = "TSUnit";
            TSUnit.Size = new System.Drawing.Size(33, 15);
            TSUnit.TabIndex = 43;
            TSUnit.Text = "units";
            // 
            // LTLabel
            // 
            LTLabel.AutoSize = true;
            LTLabel.Location = new System.Drawing.Point(9, 15);
            LTLabel.Name = "LTLabel";
            LTLabel.Size = new System.Drawing.Size(77, 15);
            LTLabel.TabIndex = 42;
            LTLabel.Text = "Loading Type";
            // 
            // LTBox
            // 
            LTBox.FormattingEnabled = true;
            LTBox.Location = new System.Drawing.Point(107, 12);
            LTBox.Name = "LTBox";
            LTBox.Size = new System.Drawing.Size(157, 23);
            LTBox.TabIndex = 41;
            LTBox.SelectedIndexChanged += LTBox_SelectedIndexChanged;
            // 
            // UseTimeSeriesRadio
            // 
            UseTimeSeriesRadio.AutoSize = true;
            UseTimeSeriesRadio.Location = new System.Drawing.Point(24, 105);
            UseTimeSeriesRadio.Name = "UseTimeSeriesRadio";
            UseTimeSeriesRadio.Size = new System.Drawing.Size(153, 19);
            UseTimeSeriesRadio.TabIndex = 40;
            UseTimeSeriesRadio.TabStop = true;
            UseTimeSeriesRadio.Text = "Use Time-series Loading";
            UseTimeSeriesRadio.UseVisualStyleBackColor = true;
            UseTimeSeriesRadio.CheckedChanged += UseConstRadio_CheckedChanged;
            // 
            // UseConstRadio
            // 
            UseConstRadio.AutoSize = true;
            UseConstRadio.Location = new System.Drawing.Point(24, 76);
            UseConstRadio.Name = "UseConstRadio";
            UseConstRadio.Size = new System.Drawing.Size(138, 19);
            UseConstRadio.TabIndex = 39;
            UseConstRadio.TabStop = true;
            UseConstRadio.Text = "Use Constant Load of";
            UseConstRadio.UseVisualStyleBackColor = true;
            UseConstRadio.CheckedChanged += UseConstRadio_CheckedChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(38, 137);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(116, 15);
            label5.TabIndex = 38;
            label5.Text = "Multiply Loading by:";
            // 
            // MultLoadBox
            // 
            MultLoadBox.Location = new System.Drawing.Point(168, 134);
            MultLoadBox.Name = "MultLoadBox";
            MultLoadBox.Size = new System.Drawing.Size(62, 23);
            MultLoadBox.TabIndex = 37;
            MultLoadBox.TextChanged += MultLoadBox_TextChanged;
            // 
            // CLUnit
            // 
            CLUnit.AutoSize = true;
            CLUnit.Location = new System.Drawing.Point(270, 78);
            CLUnit.Name = "CLUnit";
            CLUnit.Size = new System.Drawing.Size(33, 15);
            CLUnit.TabIndex = 36;
            CLUnit.Text = "units";
            // 
            // ConstLoadBox
            // 
            ConstLoadBox.Location = new System.Drawing.Point(184, 72);
            ConstLoadBox.Name = "ConstLoadBox";
            ConstLoadBox.Size = new System.Drawing.Size(79, 23);
            ConstLoadBox.TabIndex = 35;
            ConstLoadBox.TextChanged += ConstLoadBox_TextChanged;
            // 
            // NotesEdit2
            // 
            NotesEdit2.Location = new System.Drawing.Point(52, 210);
            NotesEdit2.Name = "NotesEdit2";
            NotesEdit2.Size = new System.Drawing.Size(277, 23);
            NotesEdit2.TabIndex = 34;
            NotesEdit2.TextChanged += NotesEdit2_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 184);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 15);
            label1.TabIndex = 33;
            label1.Text = "Notes:";
            // 
            // NotesEdit
            // 
            NotesEdit.Location = new System.Drawing.Point(52, 184);
            NotesEdit.Name = "NotesEdit";
            NotesEdit.Size = new System.Drawing.Size(277, 23);
            NotesEdit.TabIndex = 32;
            NotesEdit.TextChanged += NotesEdit_TextChanged;
            // 
            // HMS_Button
            // 
            HMS_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            HMS_Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            HMS_Button.Location = new System.Drawing.Point(536, 301);
            HMS_Button.Name = "HMS_Button";
            HMS_Button.Size = new System.Drawing.Size(57, 23);
            HMS_Button.TabIndex = 30;
            HMS_Button.Text = "HMS";
            HMS_Button.UseVisualStyleBackColor = true;
            HMS_Button.Click += HMS_Click;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(373, 305);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(72, 15);
            label2.TabIndex = 29;
            label2.Text = "Import from";
            // 
            // File_Import
            // 
            File_Import.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            File_Import.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            File_Import.Location = new System.Drawing.Point(462, 301);
            File_Import.Name = "File_Import";
            File_Import.Size = new System.Drawing.Size(57, 23);
            File_Import.TabIndex = 28;
            File_Import.Text = "File";
            File_Import.UseVisualStyleBackColor = true;
            File_Import.Click += File_Import_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(371, 22);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new System.Drawing.Size(260, 271);
            dataGridView1.TabIndex = 1;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.DataError += dataGridView1_DataError;
            // 
            // ParameterButton
            // 
            ParameterButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            ParameterButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            ParameterButton.Location = new System.Drawing.Point(414, 12);
            ParameterButton.Name = "ParameterButton";
            ParameterButton.Size = new System.Drawing.Size(90, 23);
            ParameterButton.TabIndex = 22;
            ParameterButton.Text = "Parameters";
            ParameterButton.UseVisualStyleBackColor = true;
            ParameterButton.Click += ParameterButton_Click;
            // 
            // OKButton
            // 
            OKButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            OKButton.Location = new System.Drawing.Point(538, 12);
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size(61, 23);
            OKButton.TabIndex = 23;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // CancelButt
            // 
            CancelButt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CancelButt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelButt.Location = new System.Drawing.Point(605, 12);
            CancelButt.Name = "CancelButt";
            CancelButt.Size = new System.Drawing.Size(61, 23);
            CancelButt.TabIndex = 24;
            CancelButt.Text = "Cancel";
            CancelButt.UseVisualStyleBackColor = true;
            CancelButt.Click += CancelButt_Click;
            // 
            // WarningLabel
            // 
            WarningLabel.AutoSize = true;
            WarningLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            WarningLabel.ForeColor = System.Drawing.Color.DarkRed;
            WarningLabel.Location = new System.Drawing.Point(284, 75);
            WarningLabel.Name = "WarningLabel";
            WarningLabel.Size = new System.Drawing.Size(346, 15);
            WarningLabel.TabIndex = 25;
            WarningLabel.Text = "Ammonia Selected as a Driving Variable in the Setup Window";
            // 
            // HelpButton
            // 
            HelpButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            HelpButton.Image = Properties.Resources.help_icon;
            HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton.Location = new System.Drawing.Point(291, 10);
            HelpButton.Name = "HelpButton";
            HelpButton.Size = new System.Drawing.Size(87, 27);
            HelpButton.TabIndex = 26;
            HelpButton.Text = "  Help";
            HelpButton.UseVisualStyleBackColor = true;
            HelpButton.Click += HelpButton_Click;
            // 
            // ToxicityButton
            // 
            ToxicityButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            ToxicityButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            ToxicityButton.Location = new System.Drawing.Point(414, 41);
            ToxicityButton.Name = "ToxicityButton";
            ToxicityButton.Size = new System.Drawing.Size(184, 23);
            ToxicityButton.TabIndex = 27;
            ToxicityButton.Text = "Toxicity and Bioaccumulation";
            ToxicityButton.UseVisualStyleBackColor = true;
            ToxicityButton.Visible = false;
            ToxicityButton.Click += button1_Click;
            // 
            // LoadingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(676, 447);
            Controls.Add(ToxicityButton);
            Controls.Add(HelpButton);
            Controls.Add(WarningLabel);
            Controls.Add(OKButton);
            Controls.Add(CancelButt);
            Controls.Add(ParameterButton);
            Controls.Add(LoadingsPanel);
            Controls.Add(IgnoreLoadingsBox);
            Controls.Add(ICUnit);
            Controls.Add(SVNameLabel);
            Controls.Add(ICLabel);
            Controls.Add(ICEdit);
            MinimumSize = new System.Drawing.Size(692, 410);
            Name = "LoadingsForm";
            Text = "State Variable Initial Condition and Loadings";
            Load += GridForm_Load;
            LoadingsPanel.ResumeLayout(false);
            LoadingsPanel.PerformLayout();
            RBPanel.ResumeLayout(false);
            RBPanel.PerformLayout();
            LTPanel.ResumeLayout(false);
            LTPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label WarningLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button File_Import;
        private System.Windows.Forms.Button HMS_Button;
        private System.Windows.Forms.Panel RBPanel;
        private System.Windows.Forms.RadioButton RB2;
        private System.Windows.Forms.RadioButton RB1;
        private System.Windows.Forms.RadioButton RB0;
        private System.Windows.Forms.Label RBLabel;
        private System.Windows.Forms.Panel LTPanel;
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
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Label timeSeriesLabel;
        private System.Windows.Forms.Button ToxicityButton;
    }
}