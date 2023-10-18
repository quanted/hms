
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
            dataGridView1 = new System.Windows.Forms.DataGridView();
            OKButton = new System.Windows.Forms.Button();
            CancelButt = new System.Windows.Forms.Button();
            changedLabel = new System.Windows.Forms.Label();
            HelpButton = new System.Windows.Forms.Button();
            copybutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(23, 46);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new System.Drawing.Size(745, 472);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.DataError += dataGridView1_DataError;
            // 
            // OKButton
            // 
            OKButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            OKButton.Location = new System.Drawing.Point(643, 12);
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
            CancelButt.Location = new System.Drawing.Point(710, 12);
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
            changedLabel.Location = new System.Drawing.Point(573, 16);
            changedLabel.Name = "changedLabel";
            changedLabel.Size = new System.Drawing.Size(53, 15);
            changedLabel.TabIndex = 27;
            changedLabel.Text = "changed";
            changedLabel.Visible = false;
            // 
            // HelpButton
            // 
            HelpButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            HelpButton.Image = Properties.Resources.help_icon;
            HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton.Location = new System.Drawing.Point(455, 10);
            HelpButton.Name = "HelpButton";
            HelpButton.Size = new System.Drawing.Size(87, 27);
            HelpButton.TabIndex = 28;
            HelpButton.Text = "  Help";
            HelpButton.UseVisualStyleBackColor = true;
            HelpButton.Click += HelpButton_Click;
            // 
            // copybutton
            // 
            copybutton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            copybutton.Location = new System.Drawing.Point(23, 14);
            copybutton.Name = "copybutton";
            copybutton.Size = new System.Drawing.Size(61, 23);
            copybutton.TabIndex = 29;
            copybutton.Text = "Copy All";
            copybutton.UseVisualStyleBackColor = true;
            copybutton.Click += copybutton_Click;
            // 
            // GridForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(793, 541);
            Controls.Add(copybutton);
            Controls.Add(HelpButton);
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
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Button copybutton;
    }
}