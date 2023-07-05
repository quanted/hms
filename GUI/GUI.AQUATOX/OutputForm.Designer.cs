
namespace GUI.AQUATOX
{
    partial class OutputForm
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
            this.OutputBox = new System.Windows.Forms.ComboBox();
            this.graphBox = new System.Windows.Forms.ComboBox();
            this.DelRunButton = new System.Windows.Forms.Button();
            this.EditGraphButton = new System.Windows.Forms.Button();
            this.DeleteGraphButton = new System.Windows.Forms.Button();
            this.NewGraphButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ExportButton = new System.Windows.Forms.Button();
            this.toggleLog = new System.Windows.Forms.Button();
            this.resetZoom = new System.Windows.Forms.Button();
            this.HelpButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Graph_to_CSV = new System.Windows.Forms.Button();
            this.graphOption = new System.Windows.Forms.RadioButton();
            this.zoomOption = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // OutputBox
            // 
            this.OutputBox.FormattingEnabled = true;
            this.OutputBox.Location = new System.Drawing.Point(12, 33);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Size = new System.Drawing.Size(233, 23);
            this.OutputBox.TabIndex = 5;
            this.OutputBox.Visible = false;
            this.OutputBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // graphBox
            // 
            this.graphBox.FormattingEnabled = true;
            this.graphBox.Location = new System.Drawing.Point(419, 35);
            this.graphBox.Name = "graphBox";
            this.graphBox.Size = new System.Drawing.Size(236, 23);
            this.graphBox.TabIndex = 6;
            this.graphBox.SelectedIndexChanged += new System.EventHandler(this.graphBox_selectedIndexChange);
            // 
            // DelRunButton
            // 
            this.DelRunButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DelRunButton.Location = new System.Drawing.Point(251, 33);
            this.DelRunButton.Name = "DelRunButton";
            this.DelRunButton.Size = new System.Drawing.Size(64, 24);
            this.DelRunButton.TabIndex = 8;
            this.DelRunButton.Text = "Del. Run";
            this.DelRunButton.UseVisualStyleBackColor = true;
            this.DelRunButton.Click += new System.EventHandler(this.DelRunButton_Click);
            // 
            // EditGraphButton
            // 
            this.EditGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.EditGraphButton.Location = new System.Drawing.Point(661, 33);
            this.EditGraphButton.Name = "EditGraphButton";
            this.EditGraphButton.Size = new System.Drawing.Size(56, 24);
            this.EditGraphButton.TabIndex = 9;
            this.EditGraphButton.Text = "Edit";
            this.EditGraphButton.UseVisualStyleBackColor = true;
            this.EditGraphButton.Click += new System.EventHandler(this.EditGraphButton_Click);
            // 
            // DeleteGraphButton
            // 
            this.DeleteGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DeleteGraphButton.Location = new System.Drawing.Point(723, 33);
            this.DeleteGraphButton.Name = "DeleteGraphButton";
            this.DeleteGraphButton.Size = new System.Drawing.Size(56, 24);
            this.DeleteGraphButton.TabIndex = 10;
            this.DeleteGraphButton.Text = "Delete";
            this.DeleteGraphButton.UseVisualStyleBackColor = true;
            this.DeleteGraphButton.Click += new System.EventHandler(this.DeleteGraphButton_Click);
            // 
            // NewGraphButton
            // 
            this.NewGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.NewGraphButton.Location = new System.Drawing.Point(578, 8);
            this.NewGraphButton.Name = "NewGraphButton";
            this.NewGraphButton.Size = new System.Drawing.Size(77, 23);
            this.NewGraphButton.TabIndex = 11;
            this.NewGraphButton.Text = "New Graph";
            this.NewGraphButton.UseVisualStyleBackColor = true;
            this.NewGraphButton.Click += new System.EventHandler(this.NewGraphButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "Simulation Results";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(421, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Graphs";
            // 
            // ExportButton
            // 
            this.ExportButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.ExportButton.Location = new System.Drawing.Point(321, 33);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(69, 24);
            this.ExportButton.TabIndex = 14;
            this.ExportButton.Text = "All to CSV";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // toggleLog
            // 
            this.toggleLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleLog.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.toggleLog.Location = new System.Drawing.Point(689, 432);
            this.toggleLog.Name = "toggleLog";
            this.toggleLog.Size = new System.Drawing.Size(114, 24);
            this.toggleLog.TabIndex = 15;
            this.toggleLog.Text = "Toggle Log Scale";
            this.toggleLog.UseVisualStyleBackColor = true;
            this.toggleLog.Click += new System.EventHandler(this.toggleLog_Click);
            // 
            // resetZoom
            // 
            this.resetZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetZoom.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.resetZoom.Location = new System.Drawing.Point(578, 432);
            this.resetZoom.Name = "resetZoom";
            this.resetZoom.Size = new System.Drawing.Size(95, 24);
            this.resetZoom.TabIndex = 16;
            this.resetZoom.Text = "Reset Zoom";
            this.resetZoom.UseVisualStyleBackColor = true;
            this.resetZoom.Click += new System.EventHandler(this.resetZoom_Click);
            // 
            // HelpButton
            // 
            this.HelpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HelpButton.Image = global::GUI.AQUATOX.Properties.Resources.help_icon;
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(8, 431);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(87, 24);
            this.HelpButton.TabIndex = 29;
            this.HelpButton.Text = "  Help";
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(19, 70);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(807, 356);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Graph_to_CSV
            // 
            this.Graph_to_CSV.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Graph_to_CSV.Location = new System.Drawing.Point(785, 33);
            this.Graph_to_CSV.Name = "Graph_to_CSV";
            this.Graph_to_CSV.Size = new System.Drawing.Size(41, 24);
            this.Graph_to_CSV.TabIndex = 31;
            this.Graph_to_CSV.Text = "CSV";
            this.Graph_to_CSV.UseVisualStyleBackColor = true;
            this.Graph_to_CSV.Click += new System.EventHandler(this.graph_to_CSV_Click);
            // 
            // graphOption
            // 
            this.graphOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.graphOption.AutoSize = true;
            this.graphOption.Checked = true;
            this.graphOption.Location = new System.Drawing.Point(307, 432);
            this.graphOption.Name = "graphOption";
            this.graphOption.Size = new System.Drawing.Size(247, 19);
            this.graphOption.TabIndex = 33;
            this.graphOption.TabStop = true;
            this.graphOption.Text = "Click on the graph to show date and value";
            this.graphOption.UseVisualStyleBackColor = true;
            // 
            // zoomOption
            // 
            this.zoomOption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.zoomOption.AutoSize = true;
            this.zoomOption.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.zoomOption.Location = new System.Drawing.Point(157, 431);
            this.zoomOption.Name = "zoomOption";
            this.zoomOption.Size = new System.Drawing.Size(131, 19);
            this.zoomOption.TabIndex = 32;
            this.zoomOption.Text = "Draw a box to zoom";
            this.zoomOption.UseVisualStyleBackColor = true;
            this.zoomOption.CheckedChanged += new System.EventHandler(this.zoomOption_CheckedChanged);
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 459);
            this.Controls.Add(this.graphOption);
            this.Controls.Add(this.zoomOption);
            this.Controls.Add(this.Graph_to_CSV);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.resetZoom);
            this.Controls.Add(this.toggleLog);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewGraphButton);
            this.Controls.Add(this.DeleteGraphButton);
            this.Controls.Add(this.EditGraphButton);
            this.Controls.Add(this.DelRunButton);
            this.Controls.Add(this.graphBox);
            this.Controls.Add(this.OutputBox);
            this.MinimumSize = new System.Drawing.Size(826, 463);
            this.Name = "OutputForm";
            this.Text = "Output Window";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox OutputBox;
        private System.Windows.Forms.ComboBox graphBox;
        private System.Windows.Forms.Button DelRunButton;
        private System.Windows.Forms.Button EditGraphButton;
        private System.Windows.Forms.Button DeleteGraphButton;
        private System.Windows.Forms.Button NewGraphButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.Button toggleLog;
        private System.Windows.Forms.Button resetZoom;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button Graph_to_CSV;
        private System.Windows.Forms.RadioButton graphOption;
        private System.Windows.Forms.RadioButton zoomOption;
    }
}
