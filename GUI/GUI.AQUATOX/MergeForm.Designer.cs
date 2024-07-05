
namespace GUI.AQUATOX
{
    partial class MergeForm
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
            OKBtn = new System.Windows.Forms.Button();
            CancelBtn = new System.Windows.Forms.Button();
            SILabel2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            Travel_Time_Edit = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // OKBtn
            // 
            OKBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            OKBtn.Location = new System.Drawing.Point(350, 12);
            OKBtn.Name = "OKBtn";
            OKBtn.Size = new System.Drawing.Size(61, 23);
            OKBtn.TabIndex = 1;
            OKBtn.Text = "OK";
            OKBtn.UseVisualStyleBackColor = true;
            OKBtn.Click += OKBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            CancelBtn.Location = new System.Drawing.Point(422, 12);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new System.Drawing.Size(61, 23);
            CancelBtn.TabIndex = 2;
            CancelBtn.Text = "Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SILabel2
            // 
            SILabel2.Location = new System.Drawing.Point(12, 105);
            SILabel2.Name = "SILabel2";
            SILabel2.Size = new System.Drawing.Size(477, 40);
            SILabel2.TabIndex = 56;
            SILabel2.Text = "The current network has ___ segments. /n The average travel time is  XX minutes.  The 5th percentile retention time is XX minutes.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(11, 62);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(274, 15);
            label1.TabIndex = 58;
            label1.Text = "Merge Segments with a travel time of less than ";
            // 
            // Travel_Time_Edit
            // 
            Travel_Time_Edit.Location = new System.Drawing.Point(288, 60);
            Travel_Time_Edit.Name = "Travel_Time_Edit";
            Travel_Time_Edit.Size = new System.Drawing.Size(34, 23);
            Travel_Time_Edit.TabIndex = 57;
            Travel_Time_Edit.Tag = "";
            Travel_Time_Edit.Text = "0.02";
            Travel_Time_Edit.TextChanged += Travel_Time_Edit_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            label2.Location = new System.Drawing.Point(328, 62);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(31, 15);
            label2.TabIndex = 59;
            label2.Text = "days";
            // 
            // label3
            // 
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(12, 9);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(300, 40);
            label3.TabIndex = 60;
            label3.Text = "Merging segments can prevent tiny segments from causing the model to run slowly.";
            // 
            // MergeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(501, 162);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Travel_Time_Edit);
            Controls.Add(SILabel2);
            Controls.Add(OKBtn);
            Controls.Add(CancelBtn);
            Name = "MergeForm";
            Text = "Merge Segments";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label SILabel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Travel_Time_Edit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}