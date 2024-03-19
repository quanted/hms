
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
            OutputBox = new System.Windows.Forms.ComboBox();
            graphBox = new System.Windows.Forms.ComboBox();
            DelRunButton = new System.Windows.Forms.Button();
            EditGraphButton = new System.Windows.Forms.Button();
            DeleteGraphButton = new System.Windows.Forms.Button();
            NewGraphButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            ExportButton = new System.Windows.Forms.Button();
            toggleLog = new System.Windows.Forms.Button();
            resetZoom = new System.Windows.Forms.Button();
            HelpButton = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            Graph_to_CSV = new System.Windows.Forms.Button();
            graphOption = new System.Windows.Forms.RadioButton();
            zoomOption = new System.Windows.Forms.RadioButton();
            SaveParamsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // OutputBox
            // 
            OutputBox.FormattingEnabled = true;
            OutputBox.Location = new System.Drawing.Point(12, 33);
            OutputBox.Name = "OutputBox";
            OutputBox.Size = new System.Drawing.Size(233, 23);
            OutputBox.TabIndex = 5;
            OutputBox.Visible = false;
            OutputBox.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // graphBox
            // 
            graphBox.FormattingEnabled = true;
            graphBox.Location = new System.Drawing.Point(419, 35);
            graphBox.Name = "graphBox";
            graphBox.Size = new System.Drawing.Size(236, 23);
            graphBox.TabIndex = 6;
            graphBox.SelectedIndexChanged += graphBox_selectedIndexChange;
            // 
            // DelRunButton
            // 
            DelRunButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            DelRunButton.Location = new System.Drawing.Point(251, 33);
            DelRunButton.Name = "DelRunButton";
            DelRunButton.Size = new System.Drawing.Size(56, 24);
            DelRunButton.TabIndex = 8;
            DelRunButton.Text = "Delete";
            DelRunButton.UseVisualStyleBackColor = true;
            DelRunButton.Click += DelRunButton_Click;
            // 
            // EditGraphButton
            // 
            EditGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            EditGraphButton.Location = new System.Drawing.Point(661, 33);
            EditGraphButton.Name = "EditGraphButton";
            EditGraphButton.Size = new System.Drawing.Size(56, 24);
            EditGraphButton.TabIndex = 9;
            EditGraphButton.Text = "Edit";
            EditGraphButton.UseVisualStyleBackColor = true;
            EditGraphButton.Click += EditGraphButton_Click;
            // 
            // DeleteGraphButton
            // 
            DeleteGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            DeleteGraphButton.Location = new System.Drawing.Point(723, 33);
            DeleteGraphButton.Name = "DeleteGraphButton";
            DeleteGraphButton.Size = new System.Drawing.Size(56, 24);
            DeleteGraphButton.TabIndex = 10;
            DeleteGraphButton.Text = "Delete";
            DeleteGraphButton.UseVisualStyleBackColor = true;
            DeleteGraphButton.Click += DeleteGraphButton_Click;
            // 
            // NewGraphButton
            // 
            NewGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            NewGraphButton.Location = new System.Drawing.Point(578, 6);
            NewGraphButton.Name = "NewGraphButton";
            NewGraphButton.Size = new System.Drawing.Size(77, 23);
            NewGraphButton.TabIndex = 11;
            NewGraphButton.Text = "New Graph";
            NewGraphButton.UseVisualStyleBackColor = true;
            NewGraphButton.Click += NewGraphButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(13, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(109, 15);
            label1.TabIndex = 12;
            label1.Text = "Simulation Results";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label2.Location = new System.Drawing.Point(421, 15);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(46, 15);
            label2.TabIndex = 13;
            label2.Text = "Graphs";
            // 
            // ExportButton
            // 
            ExportButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            ExportButton.Location = new System.Drawing.Point(314, 33);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new System.Drawing.Size(75, 24);
            ExportButton.TabIndex = 14;
            ExportButton.Text = "All to CSV";
            ExportButton.UseVisualStyleBackColor = true;
            ExportButton.Click += ExportButton_Click;
            // 
            // toggleLog
            // 
            toggleLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            toggleLog.Cursor = System.Windows.Forms.Cursors.SizeAll;
            toggleLog.Location = new System.Drawing.Point(689, 432);
            toggleLog.Name = "toggleLog";
            toggleLog.Size = new System.Drawing.Size(114, 24);
            toggleLog.TabIndex = 15;
            toggleLog.Text = "Toggle Log Scale";
            toggleLog.UseVisualStyleBackColor = true;
            toggleLog.Click += toggleLog_Click;
            // 
            // resetZoom
            // 
            resetZoom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            resetZoom.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resetZoom.Location = new System.Drawing.Point(578, 432);
            resetZoom.Name = "resetZoom";
            resetZoom.Size = new System.Drawing.Size(95, 24);
            resetZoom.TabIndex = 16;
            resetZoom.Text = "Reset Zoom";
            resetZoom.UseVisualStyleBackColor = true;
            resetZoom.Click += resetZoom_Click;
            // 
            // HelpButton
            // 
            HelpButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            HelpButton.Image = Properties.Resources.help_icon;
            HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton.Location = new System.Drawing.Point(8, 431);
            HelpButton.Name = "HelpButton";
            HelpButton.Size = new System.Drawing.Size(87, 24);
            HelpButton.TabIndex = 29;
            HelpButton.Text = "  Help";
            HelpButton.UseVisualStyleBackColor = true;
            HelpButton.Click += HelpButton_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.Location = new System.Drawing.Point(19, 70);
            pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(807, 356);
            pictureBox1.TabIndex = 30;
            pictureBox1.TabStop = false;
            pictureBox1.Visible = false;
            // 
            // Graph_to_CSV
            // 
            Graph_to_CSV.Cursor = System.Windows.Forms.Cursors.SizeAll;
            Graph_to_CSV.Location = new System.Drawing.Point(785, 33);
            Graph_to_CSV.Name = "Graph_to_CSV";
            Graph_to_CSV.Size = new System.Drawing.Size(41, 24);
            Graph_to_CSV.TabIndex = 31;
            Graph_to_CSV.Text = "CSV";
            Graph_to_CSV.UseVisualStyleBackColor = true;
            Graph_to_CSV.Click += graph_to_CSV_Click;
            // 
            // graphOption
            // 
            graphOption.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            graphOption.AutoSize = true;
            graphOption.Checked = true;
            graphOption.Location = new System.Drawing.Point(307, 432);
            graphOption.Name = "graphOption";
            graphOption.Size = new System.Drawing.Size(247, 19);
            graphOption.TabIndex = 33;
            graphOption.TabStop = true;
            graphOption.Text = "Click on the graph to show date and value";
            graphOption.UseVisualStyleBackColor = true;
            // 
            // zoomOption
            // 
            zoomOption.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            zoomOption.AutoSize = true;
            zoomOption.Cursor = System.Windows.Forms.Cursors.SizeNS;
            zoomOption.Location = new System.Drawing.Point(157, 431);
            zoomOption.Name = "zoomOption";
            zoomOption.Size = new System.Drawing.Size(131, 19);
            zoomOption.TabIndex = 32;
            zoomOption.Text = "Draw a box to zoom";
            zoomOption.UseVisualStyleBackColor = true;
            zoomOption.CheckedChanged += zoomOption_CheckedChanged;
            // 
            // SaveParamsButton
            // 
            SaveParamsButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            SaveParamsButton.Location = new System.Drawing.Point(251, 6);
            SaveParamsButton.Name = "SaveParamsButton";
            SaveParamsButton.Size = new System.Drawing.Size(107, 23);
            SaveParamsButton.TabIndex = 34;
            SaveParamsButton.Text = "Save Parameters";
            SaveParamsButton.UseVisualStyleBackColor = true;
            SaveParamsButton.Click += SaveParametersClick;
            // 
            // OutputForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(847, 459);
            Controls.Add(SaveParamsButton);
            Controls.Add(graphOption);
            Controls.Add(zoomOption);
            Controls.Add(Graph_to_CSV);
            Controls.Add(pictureBox1);
            Controls.Add(HelpButton);
            Controls.Add(resetZoom);
            Controls.Add(toggleLog);
            Controls.Add(ExportButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(NewGraphButton);
            Controls.Add(DeleteGraphButton);
            Controls.Add(EditGraphButton);
            Controls.Add(DelRunButton);
            Controls.Add(graphBox);
            Controls.Add(OutputBox);
            MinimumSize = new System.Drawing.Size(826, 463);
            Name = "OutputForm";
            Text = "Output Window";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Button SaveParamsButton;
    }
}