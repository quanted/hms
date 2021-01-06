
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
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.LTLabel = new System.Windows.Forms.Label();
            this.LTBox = new System.Windows.Forms.ComboBox();
            this.UseTimeSeriesRadio = new System.Windows.Forms.RadioButton();
            this.UseConstRadio = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.CLUnit = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.NotesEdit2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NotesEdit = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ParameterButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CancelButt = new System.Windows.Forms.Button();
            this.AmmoniaDriveLabel = new System.Windows.Forms.Label();
            this.ButtonPanel.SuspendLayout();
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
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ButtonPanel.Controls.Add(this.LTLabel);
            this.ButtonPanel.Controls.Add(this.LTBox);
            this.ButtonPanel.Controls.Add(this.UseTimeSeriesRadio);
            this.ButtonPanel.Controls.Add(this.UseConstRadio);
            this.ButtonPanel.Controls.Add(this.label5);
            this.ButtonPanel.Controls.Add(this.textBox4);
            this.ButtonPanel.Controls.Add(this.CLUnit);
            this.ButtonPanel.Controls.Add(this.textBox3);
            this.ButtonPanel.Controls.Add(this.NotesEdit2);
            this.ButtonPanel.Controls.Add(this.label1);
            this.ButtonPanel.Controls.Add(this.NotesEdit);
            this.ButtonPanel.Controls.Add(this.dataGridView1);
            this.ButtonPanel.Location = new System.Drawing.Point(12, 101);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(652, 264);
            this.ButtonPanel.TabIndex = 21;
            this.ButtonPanel.Visible = false;
            // 
            // LTLabel
            // 
            this.LTLabel.AutoSize = true;
            this.LTLabel.Location = new System.Drawing.Point(10, 13);
            this.LTLabel.Name = "LTLabel";
            this.LTLabel.Size = new System.Drawing.Size(77, 15);
            this.LTLabel.TabIndex = 27;
            this.LTLabel.Text = "Loading Type";
            // 
            // LTBox
            // 
            this.LTBox.FormattingEnabled = true;
            this.LTBox.Location = new System.Drawing.Point(108, 10);
            this.LTBox.Name = "LTBox";
            this.LTBox.Size = new System.Drawing.Size(124, 23);
            this.LTBox.TabIndex = 26;
            this.LTBox.Visible = false;
            // 
            // UseTimeSeriesRadio
            // 
            this.UseTimeSeriesRadio.AutoSize = true;
            this.UseTimeSeriesRadio.Location = new System.Drawing.Point(25, 98);
            this.UseTimeSeriesRadio.Name = "UseTimeSeriesRadio";
            this.UseTimeSeriesRadio.Size = new System.Drawing.Size(153, 19);
            this.UseTimeSeriesRadio.TabIndex = 25;
            this.UseTimeSeriesRadio.TabStop = true;
            this.UseTimeSeriesRadio.Text = "Use Time-series Loading";
            this.UseTimeSeriesRadio.UseVisualStyleBackColor = true;
            // 
            // UseConstRadio
            // 
            this.UseConstRadio.AutoSize = true;
            this.UseConstRadio.Location = new System.Drawing.Point(25, 63);
            this.UseConstRadio.Name = "UseConstRadio";
            this.UseConstRadio.Size = new System.Drawing.Size(138, 19);
            this.UseConstRadio.TabIndex = 24;
            this.UseConstRadio.TabStop = true;
            this.UseConstRadio.Text = "Use Constant Load of";
            this.UseConstRadio.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 15);
            this.label5.TabIndex = 23;
            this.label5.Text = "Multiply Loading by:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(169, 132);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(62, 23);
            this.textBox4.TabIndex = 22;
            // 
            // CLUnit
            // 
            this.CLUnit.AutoSize = true;
            this.CLUnit.Location = new System.Drawing.Point(271, 65);
            this.CLUnit.Name = "CLUnit";
            this.CLUnit.Size = new System.Drawing.Size(33, 15);
            this.CLUnit.TabIndex = 21;
            this.CLUnit.Text = "units";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(169, 59);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(96, 23);
            this.textBox3.TabIndex = 19;
            // 
            // NotesEdit2
            // 
            this.NotesEdit2.Location = new System.Drawing.Point(57, 208);
            this.NotesEdit2.Name = "NotesEdit2";
            this.NotesEdit2.Size = new System.Drawing.Size(286, 23);
            this.NotesEdit2.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Notes:";
            // 
            // NotesEdit
            // 
            this.NotesEdit.Location = new System.Drawing.Point(57, 182);
            this.NotesEdit.Name = "NotesEdit";
            this.NotesEdit.Size = new System.Drawing.Size(286, 23);
            this.NotesEdit.TabIndex = 16;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(363, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(263, 228);
            this.dataGridView1.TabIndex = 1;
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
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(538, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 23;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CancelButt
            // 
            this.CancelButt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButt.Location = new System.Drawing.Point(605, 12);
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
            this.ClientSize = new System.Drawing.Size(676, 377);
            this.Controls.Add(this.AmmoniaDriveLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CancelButt);
            this.Controls.Add(this.ParameterButton);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.IgnoreLoadingsBox);
            this.Controls.Add(this.ICUnit);
            this.Controls.Add(this.SVNameLabel);
            this.Controls.Add(this.ICLabel);
            this.Controls.Add(this.ICEdit);
            this.MinimumSize = new System.Drawing.Size(692, 410);
            this.Name = "LoadingsForm";
            this.Text = "Loadings Form";
            this.Load += new System.EventHandler(this.GridForm_Load);
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
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
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.RadioButton UseTimeSeriesRadio;
        private System.Windows.Forms.RadioButton UseConstRadio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label CLUnit;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox NotesEdit2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox NotesEdit;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button ParameterButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Label LTLabel;
        private System.Windows.Forms.ComboBox LTBox;
        private System.Windows.Forms.Label AmmoniaDriveLabel;
    }
}