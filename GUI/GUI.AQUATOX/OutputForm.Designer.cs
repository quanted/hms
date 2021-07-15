
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
            this.graphBox.Location = new System.Drawing.Point(418, 35);
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
            this.DelRunButton.Size = new System.Drawing.Size(56, 24);
            this.DelRunButton.TabIndex = 8;
            this.DelRunButton.Text = "Delete";
            this.DelRunButton.UseVisualStyleBackColor = true;
            this.DelRunButton.Click += new System.EventHandler(this.DelRunButton_Click);
            // 
            // EditGraphButton
            // 
            this.EditGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.EditGraphButton.Location = new System.Drawing.Point(660, 33);
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
            this.DeleteGraphButton.Location = new System.Drawing.Point(722, 33);
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
            this.NewGraphButton.Location = new System.Drawing.Point(577, 8);
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
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 15);
            this.label1.TabIndex = 12;
            this.label1.Text = "Saved Simulation Results";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(421, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Saved Graphs";
            // 
            // ExportButton
            // 
            this.ExportButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.ExportButton.Location = new System.Drawing.Point(318, 33);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(75, 24);
            this.ExportButton.TabIndex = 14;
            this.ExportButton.Text = "Save CSV";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 512);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewGraphButton);
            this.Controls.Add(this.DeleteGraphButton);
            this.Controls.Add(this.EditGraphButton);
            this.Controls.Add(this.DelRunButton);
            this.Controls.Add(this.graphBox);
            this.Controls.Add(this.OutputBox);
            this.Name = "OutputForm";
            this.Text = "Output";
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
    }
}