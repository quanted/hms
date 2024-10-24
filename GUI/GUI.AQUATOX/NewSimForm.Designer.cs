﻿namespace GUI.AQUATOX
{
    partial class NewSimForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewSimForm));
            button1 = new System.Windows.Forms.Button();
            SimBaseLabel = new System.Windows.Forms.Label();
            ReadNetworkPanel = new System.Windows.Forms.Panel();
            ReadSNButton = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            spanLabel = new System.Windows.Forms.Label();
            spanBox = new System.Windows.Forms.TextBox();
            endCOMIDLabel = new System.Windows.Forms.Label();
            EndCOMIDBox = new System.Windows.Forms.TextBox();
            comidLabel = new System.Windows.Forms.Label();
            comidBox = new System.Windows.Forms.TextBox();
            ChooseTemplateButton = new System.Windows.Forms.Button();
            HelpButton2 = new System.Windows.Forms.Button();
            infolabel1 = new System.Windows.Forms.Label();
            infolabel2 = new System.Windows.Forms.Label();
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            button3 = new System.Windows.Forms.Button();
            SimNameEdit = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SimJSONLabel = new System.Windows.Forms.Label();
            StartDate = new System.Windows.Forms.DateTimePicker();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            EndDate = new System.Windows.Forms.DateTimePicker();
            LS_Button = new System.Windows.Forms.Button();
            TogglePanel = new System.Windows.Forms.Panel();
            StreamButton = new System.Windows.Forms.RadioButton();
            LakeButton = new System.Windows.Forms.RadioButton();
            panel2 = new System.Windows.Forms.Panel();
            BrowseJSONButton = new System.Windows.Forms.Button();
            NetworkLabel = new System.Windows.Forms.Label();
            SegLoadLabel = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            Summary2Label = new System.Windows.Forms.Label();
            Summary1Label = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            ReadNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView).BeginInit();
            TogglePanel.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            button1.Location = new System.Drawing.Point(888, 11);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(61, 26);
            button1.TabIndex = 0;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OK_click;
            // 
            // SimBaseLabel
            // 
            SimBaseLabel.AutoSize = true;
            SimBaseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            SimBaseLabel.Location = new System.Drawing.Point(18, 28);
            SimBaseLabel.Name = "SimBaseLabel";
            SimBaseLabel.Size = new System.Drawing.Size(175, 15);
            SimBaseLabel.TabIndex = 17;
            SimBaseLabel.Text = "Simulation Base:  Default Lake";
            // 
            // ReadNetworkPanel
            // 
            ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ReadNetworkPanel.Controls.Add(ReadSNButton);
            ReadNetworkPanel.Controls.Add(label6);
            ReadNetworkPanel.Controls.Add(spanLabel);
            ReadNetworkPanel.Controls.Add(spanBox);
            ReadNetworkPanel.Controls.Add(endCOMIDLabel);
            ReadNetworkPanel.Controls.Add(EndCOMIDBox);
            ReadNetworkPanel.Controls.Add(comidLabel);
            ReadNetworkPanel.Controls.Add(comidBox);
            ReadNetworkPanel.Location = new System.Drawing.Point(17, 155);
            ReadNetworkPanel.Name = "ReadNetworkPanel";
            ReadNetworkPanel.Size = new System.Drawing.Size(259, 143);
            ReadNetworkPanel.TabIndex = 24;
            ReadNetworkPanel.Visible = false;
            // 
            // ReadSNButton
            // 
            ReadSNButton.Location = new System.Drawing.Point(126, 107);
            ReadSNButton.Name = "ReadSNButton";
            ReadSNButton.Size = new System.Drawing.Size(99, 23);
            ReadSNButton.TabIndex = 29;
            ReadSNButton.Text = "Read Network";
            ReadSNButton.UseVisualStyleBackColor = true;
            ReadSNButton.Click += ReadNetwork_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(203, 78);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(24, 15);
            label6.TabIndex = 28;
            label6.Text = "km";
            // 
            // spanLabel
            // 
            spanLabel.AutoSize = true;
            spanLabel.Location = new System.Drawing.Point(41, 78);
            spanLabel.Name = "spanLabel";
            spanLabel.Size = new System.Drawing.Size(79, 15);
            spanLabel.TabIndex = 27;
            spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            spanBox.Location = new System.Drawing.Point(127, 75);
            spanBox.Name = "spanBox";
            spanBox.Size = new System.Drawing.Size(71, 23);
            spanBox.TabIndex = 26;
            spanBox.Text = "5";
            spanBox.Leave += comidBox_Leave;
            // 
            // endCOMIDLabel
            // 
            endCOMIDLabel.AutoSize = true;
            endCOMIDLabel.Location = new System.Drawing.Point(1, 50);
            endCOMIDLabel.Name = "endCOMIDLabel";
            endCOMIDLabel.Size = new System.Drawing.Size(119, 15);
            endCOMIDLabel.TabIndex = 25;
            endCOMIDLabel.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            EndCOMIDBox.Location = new System.Drawing.Point(127, 47);
            EndCOMIDBox.Name = "EndCOMIDBox";
            EndCOMIDBox.Size = new System.Drawing.Size(71, 23);
            EndCOMIDBox.TabIndex = 24;
            EndCOMIDBox.Leave += comidBox_Leave;
            // 
            // comidLabel
            // 
            comidLabel.AutoSize = true;
            comidLabel.Location = new System.Drawing.Point(76, 22);
            comidLabel.Name = "comidLabel";
            comidLabel.Size = new System.Drawing.Size(44, 15);
            comidLabel.TabIndex = 23;
            comidLabel.Text = "ComID";
            // 
            // comidBox
            // 
            comidBox.BackColor = System.Drawing.SystemColors.Window;
            comidBox.Location = new System.Drawing.Point(127, 19);
            comidBox.Name = "comidBox";
            comidBox.Size = new System.Drawing.Size(71, 23);
            comidBox.TabIndex = 22;
            comidBox.Text = "23398915";
            comidBox.Leave += comidBox_Leave;
            // 
            // ChooseTemplateButton
            // 
            ChooseTemplateButton.Location = new System.Drawing.Point(18, 66);
            ChooseTemplateButton.Name = "ChooseTemplateButton";
            ChooseTemplateButton.Size = new System.Drawing.Size(117, 23);
            ChooseTemplateButton.TabIndex = 29;
            ChooseTemplateButton.Text = "Choose Template";
            ChooseTemplateButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            ChooseTemplateButton.UseVisualStyleBackColor = true;
            ChooseTemplateButton.Click += Choose_from_Template_Click;
            // 
            // HelpButton2
            // 
            HelpButton2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            HelpButton2.Image = (System.Drawing.Image)resources.GetObject("HelpButton2.Image");
            HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            HelpButton2.Location = new System.Drawing.Point(780, 9);
            HelpButton2.Name = "HelpButton2";
            HelpButton2.Size = new System.Drawing.Size(78, 28);
            HelpButton2.TabIndex = 30;
            HelpButton2.Text = "   Help";
            HelpButton2.UseVisualStyleBackColor = true;
            HelpButton2.Click += HelpButton2_Click;
            // 
            // infolabel1
            // 
            infolabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel1.AutoSize = true;
            infolabel1.Location = new System.Drawing.Point(398, 531);
            infolabel1.Name = "infolabel1";
            infolabel1.Size = new System.Drawing.Size(378, 15);
            infolabel1.TabIndex = 39;
            infolabel1.Text = "Click on a pour-point stream segment then right-click on an upstream";
            // 
            // infolabel2
            // 
            infolabel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            infolabel2.AutoSize = true;
            infolabel2.Location = new System.Drawing.Point(398, 550);
            infolabel2.Name = "infolabel2";
            infolabel2.Size = new System.Drawing.Size(357, 15);
            infolabel2.TabIndex = 40;
            infolabel2.Text = "segment or input an up-river span in km and click \"Read Network\"";
            // 
            // webView
            // 
            webView.AllowExternalDrop = true;
            webView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            webView.CreationProperties = null;
            webView.DefaultBackgroundColor = System.Drawing.Color.White;
            webView.Location = new System.Drawing.Point(297, 59);
            webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            webView.Name = "webView";
            webView.Size = new System.Drawing.Size(743, 462);
            webView.TabIndex = 43;
            webView.ZoomFactor = 1D;
            // 
            // button3
            // 
            button3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button3.Location = new System.Drawing.Point(979, 11);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(61, 26);
            button3.TabIndex = 46;
            button3.Text = "Cancel";
            button3.UseVisualStyleBackColor = true;
            // 
            // SimNameEdit
            // 
            SimNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            SimNameEdit.Location = new System.Drawing.Point(121, 13);
            SimNameEdit.Name = "SimNameEdit";
            SimNameEdit.Size = new System.Drawing.Size(260, 23);
            SimNameEdit.TabIndex = 47;
            SimNameEdit.Tag = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(16, 16);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(102, 15);
            label2.TabIndex = 48;
            label2.Text = "Simulation Name";
            // 
            // SimJSONLabel
            // 
            SimJSONLabel.AutoSize = true;
            SimJSONLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            SimJSONLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            SimJSONLabel.Location = new System.Drawing.Point(18, 48);
            SimJSONLabel.Name = "SimJSONLabel";
            SimJSONLabel.Size = new System.Drawing.Size(113, 15);
            SimJSONLabel.TabIndex = 50;
            SimJSONLabel.Text = "\"Default Lake.JSON\"";
            // 
            // StartDate
            // 
            StartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            StartDate.Location = new System.Drawing.Point(109, 127);
            StartDate.Name = "StartDate";
            StartDate.Size = new System.Drawing.Size(103, 23);
            StartDate.TabIndex = 51;
            StartDate.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(37, 132);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 15);
            label3.TabIndex = 52;
            label3.Text = "Start Date";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(44, 158);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(57, 15);
            label5.TabIndex = 54;
            label5.Text = "End Date";
            // 
            // EndDate
            // 
            EndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            EndDate.Location = new System.Drawing.Point(109, 154);
            EndDate.Name = "EndDate";
            EndDate.Size = new System.Drawing.Size(103, 23);
            EndDate.TabIndex = 53;
            EndDate.Value = new System.DateTime(2011, 1, 1, 0, 0, 0, 0);
            // 
            // LS_Button
            // 
            LS_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            LS_Button.Location = new System.Drawing.Point(888, 540);
            LS_Button.Name = "LS_Button";
            LS_Button.Size = new System.Drawing.Size(153, 25);
            LS_Button.TabIndex = 55;
            LS_Button.Text = "Create Template JSON";
            LS_Button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            LS_Button.UseVisualStyleBackColor = true;
            LS_Button.Visible = false;
            LS_Button.Click += MS_Surrogate_Button_Click;
            // 
            // TogglePanel
            // 
            TogglePanel.Controls.Add(StreamButton);
            TogglePanel.Controls.Add(LakeButton);
            TogglePanel.Location = new System.Drawing.Point(388, 8);
            TogglePanel.Name = "TogglePanel";
            TogglePanel.Size = new System.Drawing.Size(316, 26);
            TogglePanel.TabIndex = 58;
            // 
            // StreamButton
            // 
            StreamButton.AutoSize = true;
            StreamButton.Checked = true;
            StreamButton.Location = new System.Drawing.Point(146, 4);
            StreamButton.Name = "StreamButton";
            StreamButton.Size = new System.Drawing.Size(162, 19);
            StreamButton.TabIndex = 2;
            StreamButton.TabStop = true;
            StreamButton.Text = "Stream Network (w. lakes)";
            StreamButton.UseVisualStyleBackColor = true;
            StreamButton.CheckedChanged += MapType_CheckChanged;
            // 
            // LakeButton
            // 
            LakeButton.AutoSize = true;
            LakeButton.Location = new System.Drawing.Point(12, 4);
            LakeButton.Name = "LakeButton";
            LakeButton.Size = new System.Drawing.Size(124, 19);
            LakeButton.TabIndex = 0;
            LakeButton.Text = "0-D Lake/Reservoir";
            LakeButton.UseVisualStyleBackColor = true;
            LakeButton.CheckedChanged += MapType_CheckChanged;
            // 
            // panel2
            // 
            panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel2.Controls.Add(BrowseJSONButton);
            panel2.Controls.Add(ChooseTemplateButton);
            panel2.Controls.Add(SimBaseLabel);
            panel2.Controls.Add(SimJSONLabel);
            panel2.Controls.Add(StartDate);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(EndDate);
            panel2.Controls.Add(label5);
            panel2.Location = new System.Drawing.Point(17, 319);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(259, 202);
            panel2.TabIndex = 59;
            // 
            // BrowseJSONButton
            // 
            BrowseJSONButton.Location = new System.Drawing.Point(145, 66);
            BrowseJSONButton.Name = "BrowseJSONButton";
            BrowseJSONButton.Size = new System.Drawing.Size(67, 23);
            BrowseJSONButton.TabIndex = 55;
            BrowseJSONButton.Text = "Browse";
            BrowseJSONButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            BrowseJSONButton.UseVisualStyleBackColor = true;
            BrowseJSONButton.Click += BrowseJSONButton_Click;
            // 
            // NetworkLabel
            // 
            NetworkLabel.AutoSize = true;
            NetworkLabel.Location = new System.Drawing.Point(26, 146);
            NetworkLabel.Name = "NetworkLabel";
            NetworkLabel.Size = new System.Drawing.Size(157, 15);
            NetworkLabel.TabIndex = 26;
            NetworkLabel.Text = "Parameters to Read Network";
            NetworkLabel.Visible = false;
            // 
            // SegLoadLabel
            // 
            SegLoadLabel.AutoSize = true;
            SegLoadLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            SegLoadLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            SegLoadLabel.Location = new System.Drawing.Point(438, 39);
            SegLoadLabel.Name = "SegLoadLabel";
            SegLoadLabel.Size = new System.Drawing.Size(342, 15);
            SegLoadLabel.TabIndex = 60;
            SegLoadLabel.Text = "Zoom in to see stream segments and Lake/Reservoir shapes.";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(26, 311);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(146, 15);
            label7.TabIndex = 61;
            label7.Text = "Base Simulation and Dates";
            // 
            // panel1
            // 
            panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel1.Controls.Add(Summary2Label);
            panel1.Controls.Add(Summary1Label);
            panel1.Location = new System.Drawing.Point(17, 59);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(259, 79);
            panel1.TabIndex = 62;
            // 
            // Summary2Label
            // 
            Summary2Label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Summary2Label.Location = new System.Drawing.Point(15, 33);
            Summary2Label.Name = "Summary2Label";
            Summary2Label.Size = new System.Drawing.Size(224, 34);
            Summary2Label.TabIndex = 58;
            Summary2Label.Text = "Surface Area (sq. km):";
            Summary2Label.Visible = false;
            // 
            // Summary1Label
            // 
            Summary1Label.AutoSize = true;
            Summary1Label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            Summary1Label.Location = new System.Drawing.Point(15, 11);
            Summary1Label.Name = "Summary1Label";
            Summary1Label.Size = new System.Drawing.Size(148, 15);
            Summary1Label.TabIndex = 57;
            Summary1Label.Text = "WB COMID:  (unselected)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(26, 50);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(133, 15);
            label8.TabIndex = 63;
            label8.Text = "Selected Model Domain";
            // 
            // NewSimForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1053, 576);
            Controls.Add(label8);
            Controls.Add(panel1);
            Controls.Add(label7);
            Controls.Add(NetworkLabel);
            Controls.Add(SegLoadLabel);
            Controls.Add(panel2);
            Controls.Add(TogglePanel);
            Controls.Add(LS_Button);
            Controls.Add(label2);
            Controls.Add(SimNameEdit);
            Controls.Add(button3);
            Controls.Add(webView);
            Controls.Add(infolabel2);
            Controls.Add(infolabel1);
            Controls.Add(HelpButton2);
            Controls.Add(ReadNetworkPanel);
            Controls.Add(button1);
            MinimumSize = new System.Drawing.Size(1003, 553);
            Name = "NewSimForm";
            Text = "New Simulation Window";
            FormClosing += NewSimForm_FormClosing;
            Shown += NewSimForm_Shown;
            ReadNetworkPanel.ResumeLayout(false);
            ReadNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView).EndInit();
            TogglePanel.ResumeLayout(false);
            TogglePanel.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label SimBaseLabel;
        private System.Windows.Forms.Panel ReadNetworkPanel;
        private System.Windows.Forms.Button ReadSNButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label spanLabel;
        private System.Windows.Forms.TextBox spanBox;
        private System.Windows.Forms.Label endCOMIDLabel;
        private System.Windows.Forms.TextBox EndCOMIDBox;
        private System.Windows.Forms.Label comidLabel;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.Button ChooseTemplateButton;
        private System.Windows.Forms.Button HelpButton2;
        private System.Windows.Forms.CheckBox showCOMIDsBox;
        private System.Windows.Forms.RadioButton flowchartButton;
        private System.Windows.Forms.Label infolabel1;
        private System.Windows.Forms.Label infolabel2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox SimNameEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label SimJSONLabel;
        private System.Windows.Forms.DateTimePicker StartDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker EndDate;
        private System.Windows.Forms.Button LS_Button;
        private System.Windows.Forms.Panel TogglePanel;
        private System.Windows.Forms.RadioButton StreamButton;
        private System.Windows.Forms.RadioButton LakeButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label NetworkLabel;
        private System.Windows.Forms.Label SegLoadLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label Summary2Label;
        private System.Windows.Forms.Label Summary1Label;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button BrowseJSONButton;
    }
}