
namespace GUI.AQUATOX
{
    partial class GridForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            OKButton = new System.Windows.Forms.Button();
            CancelButt = new System.Windows.Forms.Button();
            changedLabel = new System.Windows.Forms.Label();
            Help_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.Location = new System.Drawing.Point(23, 46);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new System.Drawing.Size(828, 489);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.DataError += dataGridView1_DataError;
            // 
            // OKButton
            // 
            OKButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            OKButton.Location = new System.Drawing.Point(726, 12);
            OKButton.Name = "OKButton";
            OKButton.Size = new System.Drawing.Size(61, 23);
            OKButton.TabIndex = 25;
            OKButton.Text = "OK";
            OKButton.UseVisualStyleBackColor = true;
            OKButton.Click += OKButton_Click;
            // 
            // CancelButt
            // 
            CancelButt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CancelButt.Location = new System.Drawing.Point(793, 12);
            CancelButt.Name = "CancelButt";
            CancelButt.Size = new System.Drawing.Size(61, 23);
            CancelButt.TabIndex = 26;
            CancelButt.Text = "Cancel";
            CancelButt.UseVisualStyleBackColor = true;
            CancelButt.Click += CancelButt_Click;
            // 
            // changedLabel
            // 
            changedLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            changedLabel.AutoSize = true;
            changedLabel.Location = new System.Drawing.Point(656, 16);
            changedLabel.Name = "changedLabel";
            changedLabel.Size = new System.Drawing.Size(53, 15);
            changedLabel.TabIndex = 27;
            changedLabel.Text = "changed";
            changedLabel.Visible = false;
            // 
            // Help_Button
            // 
            Help_Button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            Help_Button.Image = Properties.Resources.help_icon;
            Help_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            Help_Button.Location = new System.Drawing.Point(538, 10);
            Help_Button.Name = "Help_Button";
            Help_Button.Size = new System.Drawing.Size(87, 27);
            Help_Button.TabIndex = 28;
            Help_Button.Text = "  Help";
            Help_Button.UseVisualStyleBackColor = true;
            Help_Button.Click += HelpButton_Click;
            // 
            // GridForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(876, 558);
            Controls.Add(Help_Button);
            Controls.Add(changedLabel);
            Controls.Add(OKButton);
            Controls.Add(CancelButt);
            Controls.Add(dataGridView1);
            Name = "GridForm";
            Text = "Matrix Data Entry";
            Load += GridForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButt;
        private System.Windows.Forms.Label changedLabel;
        private System.Windows.Forms.Button Help_Button;
    }
}