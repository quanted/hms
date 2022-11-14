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
            this.button1 = new System.Windows.Forms.Button();
            this.SimBaseLabel = new System.Windows.Forms.Label();
            this.ReadNetworkPanel = new System.Windows.Forms.Panel();
            this.ReadSNButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.spanLabel = new System.Windows.Forms.Label();
            this.spanBox = new System.Windows.Forms.TextBox();
            this.endCOMIDLabel = new System.Windows.Forms.Label();
            this.EndCOMIDBox = new System.Windows.Forms.TextBox();
            this.comidLabel = new System.Windows.Forms.Label();
            this.comidBox = new System.Windows.Forms.TextBox();
            this.ChooseTemplateButton = new System.Windows.Forms.Button();
            this.HelpButton2 = new System.Windows.Forms.Button();
            this.infolabel1 = new System.Windows.Forms.Label();
            this.infolabel2 = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button3 = new System.Windows.Forms.Button();
            this.SimNameEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SimJSONLabel = new System.Windows.Forms.Label();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.LS_Button = new System.Windows.Forms.Button();
            this.TogglePanel = new System.Windows.Forms.Panel();
            this.StreamButton = new System.Windows.Forms.RadioButton();
            this.LakeButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.NetworkLabel = new System.Windows.Forms.Label();
            this.SegLoadLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Summary2Label = new System.Windows.Forms.Label();
            this.Summary1Label = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ReadNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.TogglePanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(888, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // SimBaseLabel
            // 
            this.SimBaseLabel.AutoSize = true;
            this.SimBaseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SimBaseLabel.Location = new System.Drawing.Point(18, 28);
            this.SimBaseLabel.Name = "SimBaseLabel";
            this.SimBaseLabel.Size = new System.Drawing.Size(175, 15);
            this.SimBaseLabel.TabIndex = 17;
            this.SimBaseLabel.Text = "Simulation Base:  Default Lake";
            // 
            // ReadNetworkPanel
            // 
            this.ReadNetworkPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReadNetworkPanel.Controls.Add(this.ReadSNButton);
            this.ReadNetworkPanel.Controls.Add(this.label6);
            this.ReadNetworkPanel.Controls.Add(this.spanLabel);
            this.ReadNetworkPanel.Controls.Add(this.spanBox);
            this.ReadNetworkPanel.Controls.Add(this.endCOMIDLabel);
            this.ReadNetworkPanel.Controls.Add(this.EndCOMIDBox);
            this.ReadNetworkPanel.Controls.Add(this.comidLabel);
            this.ReadNetworkPanel.Controls.Add(this.comidBox);
            this.ReadNetworkPanel.Location = new System.Drawing.Point(17, 155);
            this.ReadNetworkPanel.Name = "ReadNetworkPanel";
            this.ReadNetworkPanel.Size = new System.Drawing.Size(259, 143);
            this.ReadNetworkPanel.TabIndex = 24;
            this.ReadNetworkPanel.Visible = false;
            // 
            // ReadSNButton
            // 
            this.ReadSNButton.Location = new System.Drawing.Point(126, 107);
            this.ReadSNButton.Name = "ReadSNButton";
            this.ReadSNButton.Size = new System.Drawing.Size(99, 23);
            this.ReadSNButton.TabIndex = 29;
            this.ReadSNButton.Text = "Read Network";
            this.ReadSNButton.UseVisualStyleBackColor = true;
            this.ReadSNButton.Click += new System.EventHandler(this.ReadNetwork_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(203, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 15);
            this.label6.TabIndex = 28;
            this.label6.Text = "km";
            // 
            // spanLabel
            // 
            this.spanLabel.AutoSize = true;
            this.spanLabel.Location = new System.Drawing.Point(41, 78);
            this.spanLabel.Name = "spanLabel";
            this.spanLabel.Size = new System.Drawing.Size(79, 15);
            this.spanLabel.TabIndex = 27;
            this.spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            this.spanBox.Location = new System.Drawing.Point(127, 75);
            this.spanBox.Name = "spanBox";
            this.spanBox.Size = new System.Drawing.Size(71, 23);
            this.spanBox.TabIndex = 26;
            this.spanBox.Text = "5";
            this.spanBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // endCOMIDLabel
            // 
            this.endCOMIDLabel.AutoSize = true;
            this.endCOMIDLabel.Location = new System.Drawing.Point(1, 50);
            this.endCOMIDLabel.Name = "endCOMIDLabel";
            this.endCOMIDLabel.Size = new System.Drawing.Size(119, 15);
            this.endCOMIDLabel.TabIndex = 25;
            this.endCOMIDLabel.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            this.EndCOMIDBox.Location = new System.Drawing.Point(127, 47);
            this.EndCOMIDBox.Name = "EndCOMIDBox";
            this.EndCOMIDBox.Size = new System.Drawing.Size(71, 23);
            this.EndCOMIDBox.TabIndex = 24;
            this.EndCOMIDBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // comidLabel
            // 
            this.comidLabel.AutoSize = true;
            this.comidLabel.Location = new System.Drawing.Point(76, 22);
            this.comidLabel.Name = "comidLabel";
            this.comidLabel.Size = new System.Drawing.Size(44, 15);
            this.comidLabel.TabIndex = 23;
            this.comidLabel.Text = "ComID";
            // 
            // comidBox
            // 
            this.comidBox.BackColor = System.Drawing.SystemColors.Window;
            this.comidBox.Location = new System.Drawing.Point(127, 19);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(71, 23);
            this.comidBox.TabIndex = 22;
            this.comidBox.Text = "23398915";
            this.comidBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // ChooseTemplateButton
            // 
            this.ChooseTemplateButton.Location = new System.Drawing.Point(120, 68);
            this.ChooseTemplateButton.Name = "ChooseTemplateButton";
            this.ChooseTemplateButton.Size = new System.Drawing.Size(117, 23);
            this.ChooseTemplateButton.TabIndex = 29;
            this.ChooseTemplateButton.Text = "Choose Template";
            this.ChooseTemplateButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ChooseTemplateButton.UseVisualStyleBackColor = true;
            this.ChooseTemplateButton.Click += new System.EventHandler(this.Choose_from_Template_Click);
            // 
            // HelpButton2
            // 
            this.HelpButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HelpButton2.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton2.Image")));
            this.HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton2.Location = new System.Drawing.Point(780, 9);
            this.HelpButton2.Name = "HelpButton2";
            this.HelpButton2.Size = new System.Drawing.Size(78, 28);
            this.HelpButton2.TabIndex = 30;
            this.HelpButton2.Text = "   Help";
            this.HelpButton2.UseVisualStyleBackColor = true;
            this.HelpButton2.Click += new System.EventHandler(this.HelpButton2_Click);
            // 
            // infolabel1
            // 
            this.infolabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel1.AutoSize = true;
            this.infolabel1.Location = new System.Drawing.Point(398, 531);
            this.infolabel1.Name = "infolabel1";
            this.infolabel1.Size = new System.Drawing.Size(378, 15);
            this.infolabel1.TabIndex = 39;
            this.infolabel1.Text = "Click on a pour-point stream segment then right-click on an upstream";
            // 
            // infolabel2
            // 
            this.infolabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel2.AutoSize = true;
            this.infolabel2.Location = new System.Drawing.Point(398, 550);
            this.infolabel2.Name = "infolabel2";
            this.infolabel2.Size = new System.Drawing.Size(357, 15);
            this.infolabel2.TabIndex = 40;
            this.infolabel2.Text = "segment or input an up-river span in km and click \"Read Network\"";
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(297, 59);
            this.webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(743, 462);
            this.webView.TabIndex = 43;
            this.webView.ZoomFactor = 1D;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(979, 11);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(61, 26);
            this.button3.TabIndex = 46;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // SimNameEdit
            // 
            this.SimNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SimNameEdit.Location = new System.Drawing.Point(121, 13);
            this.SimNameEdit.Name = "SimNameEdit";
            this.SimNameEdit.Size = new System.Drawing.Size(260, 23);
            this.SimNameEdit.TabIndex = 47;
            this.SimNameEdit.Tag = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 48;
            this.label2.Text = "Simulation Name";
            // 
            // SimJSONLabel
            // 
            this.SimJSONLabel.AutoSize = true;
            this.SimJSONLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.SimJSONLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SimJSONLabel.Location = new System.Drawing.Point(18, 48);
            this.SimJSONLabel.Name = "SimJSONLabel";
            this.SimJSONLabel.Size = new System.Drawing.Size(113, 15);
            this.SimJSONLabel.TabIndex = 50;
            this.SimJSONLabel.Text = "\"Default Lake.JSON\"";
            // 
            // StartDate
            // 
            this.StartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.StartDate.Location = new System.Drawing.Point(109, 117);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(103, 23);
            this.StartDate.TabIndex = 51;
            this.StartDate.Value = new System.DateTime(2010, 1, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(37, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 52;
            this.label3.Text = "Start Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(44, 148);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 15);
            this.label5.TabIndex = 54;
            this.label5.Text = "End Date";
            // 
            // EndDate
            // 
            this.EndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.EndDate.Location = new System.Drawing.Point(109, 144);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(103, 23);
            this.EndDate.TabIndex = 53;
            this.EndDate.Value = new System.DateTime(2011, 1, 1, 0, 0, 0, 0);
            // 
            // LS_Button
            // 
            this.LS_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LS_Button.Location = new System.Drawing.Point(924, 540);
            this.LS_Button.Name = "LS_Button";
            this.LS_Button.Size = new System.Drawing.Size(117, 25);
            this.LS_Button.TabIndex = 55;
            this.LS_Button.Text = "Create LS JSON";
            this.LS_Button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.LS_Button.UseVisualStyleBackColor = true;
            this.LS_Button.Visible = false;
            this.LS_Button.Click += new System.EventHandler(this.LSButton_Click);
            // 
            // TogglePanel
            // 
            this.TogglePanel.Controls.Add(this.StreamButton);
            this.TogglePanel.Controls.Add(this.LakeButton);
            this.TogglePanel.Location = new System.Drawing.Point(388, 8);
            this.TogglePanel.Name = "TogglePanel";
            this.TogglePanel.Size = new System.Drawing.Size(316, 26);
            this.TogglePanel.TabIndex = 58;
            // 
            // StreamButton
            // 
            this.StreamButton.AutoSize = true;
            this.StreamButton.Checked = true;
            this.StreamButton.Location = new System.Drawing.Point(146, 4);
            this.StreamButton.Name = "StreamButton";
            this.StreamButton.Size = new System.Drawing.Size(162, 19);
            this.StreamButton.TabIndex = 2;
            this.StreamButton.TabStop = true;
            this.StreamButton.Text = "Stream Network (w. lakes)";
            this.StreamButton.UseVisualStyleBackColor = true;
            this.StreamButton.CheckedChanged += new System.EventHandler(this.MapType_CheckChanged);
            // 
            // LakeButton
            // 
            this.LakeButton.AutoSize = true;
            this.LakeButton.Location = new System.Drawing.Point(12, 4);
            this.LakeButton.Name = "LakeButton";
            this.LakeButton.Size = new System.Drawing.Size(124, 19);
            this.LakeButton.TabIndex = 0;
            this.LakeButton.Text = "0-D Lake/Reservoir";
            this.LakeButton.UseVisualStyleBackColor = true;
            this.LakeButton.CheckedChanged += new System.EventHandler(this.MapType_CheckChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ChooseTemplateButton);
            this.panel2.Controls.Add(this.SimBaseLabel);
            this.panel2.Controls.Add(this.SimJSONLabel);
            this.panel2.Controls.Add(this.StartDate);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.EndDate);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(17, 319);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(259, 202);
            this.panel2.TabIndex = 59;
            // 
            // NetworkLabel
            // 
            this.NetworkLabel.AutoSize = true;
            this.NetworkLabel.Location = new System.Drawing.Point(26, 146);
            this.NetworkLabel.Name = "NetworkLabel";
            this.NetworkLabel.Size = new System.Drawing.Size(157, 15);
            this.NetworkLabel.TabIndex = 26;
            this.NetworkLabel.Text = "Parameters to Read Network";
            this.NetworkLabel.Visible = false;
            // 
            // SegLoadLabel
            // 
            this.SegLoadLabel.AutoSize = true;
            this.SegLoadLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SegLoadLabel.ForeColor = System.Drawing.SystemColors.InfoText;
            this.SegLoadLabel.Location = new System.Drawing.Point(438, 39);
            this.SegLoadLabel.Name = "SegLoadLabel";
            this.SegLoadLabel.Size = new System.Drawing.Size(191, 15);
            this.SegLoadLabel.TabIndex = 60;
            this.SegLoadLabel.Text = "Zoom in to see stream segments.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(26, 311);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 15);
            this.label7.TabIndex = 61;
            this.label7.Text = "Base Simulation and Dates";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Summary2Label);
            this.panel1.Controls.Add(this.Summary1Label);
            this.panel1.Location = new System.Drawing.Point(17, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 79);
            this.panel1.TabIndex = 62;
            // 
            // Summary2Label
            // 
            this.Summary2Label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Summary2Label.Location = new System.Drawing.Point(15, 33);
            this.Summary2Label.Name = "Summary2Label";
            this.Summary2Label.Size = new System.Drawing.Size(224, 34);
            this.Summary2Label.TabIndex = 58;
            this.Summary2Label.Text = "Surface Area (sq. km):";
            this.Summary2Label.Visible = false;
            // 
            // Summary1Label
            // 
            this.Summary1Label.AutoSize = true;
            this.Summary1Label.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Summary1Label.Location = new System.Drawing.Point(15, 11);
            this.Summary1Label.Name = "Summary1Label";
            this.Summary1Label.Size = new System.Drawing.Size(148, 15);
            this.Summary1Label.TabIndex = 57;
            this.Summary1Label.Text = "WB COMID:  (unselected)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(133, 15);
            this.label8.TabIndex = 63;
            this.label8.Text = "Selected Model Domain";
            // 
            // NewSimForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 576);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.NetworkLabel);
            this.Controls.Add(this.SegLoadLabel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.TogglePanel);
            this.Controls.Add(this.LS_Button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SimNameEdit);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.infolabel2);
            this.Controls.Add(this.infolabel1);
            this.Controls.Add(this.HelpButton2);
            this.Controls.Add(this.ReadNetworkPanel);
            this.Controls.Add(this.button1);
            this.MinimumSize = new System.Drawing.Size(1003, 553);
            this.Name = "NewSimForm";
            this.Text = "NewSimForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewSimForm_FormClosing);
            this.Shown += new System.EventHandler(this.NewSimForm_Shown);
            this.ReadNetworkPanel.ResumeLayout(false);
            this.ReadNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.TogglePanel.ResumeLayout(false);
            this.TogglePanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}