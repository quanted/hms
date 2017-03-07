namespace WindowsFormsApplication1
{
    partial class DataFrequency
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
            this.lblDataFrequencyTitle = new System.Windows.Forms.Label();
            this.cmbBxFrequency = new System.Windows.Forms.ComboBox();
            this.bttnFreqSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDataFrequencyTitle
            // 
            this.lblDataFrequencyTitle.AutoSize = true;
            this.lblDataFrequencyTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataFrequencyTitle.Location = new System.Drawing.Point(12, 19);
            this.lblDataFrequencyTitle.Name = "lblDataFrequencyTitle";
            this.lblDataFrequencyTitle.Size = new System.Drawing.Size(381, 17);
            this.lblDataFrequencyTitle.TabIndex = 0;
            this.lblDataFrequencyTitle.Text = "Please select temporal freqency of timeseries data:";
            // 
            // cmbBxFrequency
            // 
            this.cmbBxFrequency.FormattingEnabled = true;
            this.cmbBxFrequency.Items.AddRange(new object[] {
            "daily",
            "weekly",
            "monthly",
            "yearly"});
            this.cmbBxFrequency.Location = new System.Drawing.Point(26, 51);
            this.cmbBxFrequency.Name = "cmbBxFrequency";
            this.cmbBxFrequency.Size = new System.Drawing.Size(139, 24);
            this.cmbBxFrequency.TabIndex = 1;
            // 
            // bttnFreqSelect
            // 
            this.bttnFreqSelect.Location = new System.Drawing.Point(219, 51);
            this.bttnFreqSelect.Name = "bttnFreqSelect";
            this.bttnFreqSelect.Size = new System.Drawing.Size(93, 24);
            this.bttnFreqSelect.TabIndex = 2;
            this.bttnFreqSelect.Text = "Select";
            this.bttnFreqSelect.UseVisualStyleBackColor = true;
            this.bttnFreqSelect.Click += new System.EventHandler(this.bttnFreqSelect_Click);
            // 
            // DataFrequency
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 107);
            this.Controls.Add(this.bttnFreqSelect);
            this.Controls.Add(this.cmbBxFrequency);
            this.Controls.Add(this.lblDataFrequencyTitle);
            this.Name = "DataFrequency";
            this.Text = "Data Frequency";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDataFrequencyTitle;
        private System.Windows.Forms.ComboBox cmbBxFrequency;
        private System.Windows.Forms.Button bttnFreqSelect;
    }
}