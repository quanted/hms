
namespace GUI.AQUATOX
{
    partial class Output
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.DelRunButton = new System.Windows.Forms.Button();
            this.EditGraphButton = new System.Windows.Forms.Button();
            this.DeleteGraphButton = new System.Windows.Forms.Button();
            this.NewGraphButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(339, 33);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(236, 23);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.Visible = false;
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
            // 
            // EditGraphButton
            // 
            this.EditGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.EditGraphButton.Location = new System.Drawing.Point(581, 31);
            this.EditGraphButton.Name = "EditGraphButton";
            this.EditGraphButton.Size = new System.Drawing.Size(56, 24);
            this.EditGraphButton.TabIndex = 9;
            this.EditGraphButton.Text = "Edit";
            this.EditGraphButton.UseVisualStyleBackColor = true;
            // 
            // DeleteGraphButton
            // 
            this.DeleteGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.DeleteGraphButton.Location = new System.Drawing.Point(643, 31);
            this.DeleteGraphButton.Name = "DeleteGraphButton";
            this.DeleteGraphButton.Size = new System.Drawing.Size(56, 24);
            this.DeleteGraphButton.TabIndex = 10;
            this.DeleteGraphButton.Text = "Delete";
            this.DeleteGraphButton.UseVisualStyleBackColor = true;
            // 
            // NewGraphButton
            // 
            this.NewGraphButton.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.NewGraphButton.Location = new System.Drawing.Point(498, 6);
            this.NewGraphButton.Name = "NewGraphButton";
            this.NewGraphButton.Size = new System.Drawing.Size(77, 23);
            this.NewGraphButton.TabIndex = 11;
            this.NewGraphButton.Text = "New Graph";
            this.NewGraphButton.UseVisualStyleBackColor = true;
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
            this.label2.Location = new System.Drawing.Point(342, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "Saved Graphs";
            // 
            // Output
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 512);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewGraphButton);
            this.Controls.Add(this.DeleteGraphButton);
            this.Controls.Add(this.EditGraphButton);
            this.Controls.Add(this.DelRunButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.OutputBox);
            this.Name = "Output";
            this.Text = "Output";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox OutputBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button DelRunButton;
        private System.Windows.Forms.Button EditGraphButton;
        private System.Windows.Forms.Button DeleteGraphButton;
        private System.Windows.Forms.Button NewGraphButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}