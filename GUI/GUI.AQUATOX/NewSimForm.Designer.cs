namespace GUI.AQUATOX
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
            this.label4 = new System.Windows.Forms.Label();
            this.EndCOMIDBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comidBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ChooseTemplateButton = new System.Windows.Forms.Button();
            this.HelpButton2 = new System.Windows.Forms.Button();
            this.infolabel1 = new System.Windows.Forms.Label();
            this.infolabel2 = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button3 = new System.Windows.Forms.Button();
            this.SimNameEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.WBCLabel = new System.Windows.Forms.Label();
            this.SimJSONLabel = new System.Windows.Forms.Label();
            this.ReadNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(973, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OK_click);
            // 
            // SimBaseLabel
            // 
            this.SimBaseLabel.AutoSize = true;
            this.SimBaseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SimBaseLabel.Location = new System.Drawing.Point(17, 98);
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
            this.ReadNetworkPanel.Controls.Add(this.label4);
            this.ReadNetworkPanel.Controls.Add(this.EndCOMIDBox);
            this.ReadNetworkPanel.Controls.Add(this.label1);
            this.ReadNetworkPanel.Controls.Add(this.comidBox);
            this.ReadNetworkPanel.Location = new System.Drawing.Point(354, 594);
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
            this.label6.Location = new System.Drawing.Point(204, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 15);
            this.label6.TabIndex = 28;
            this.label6.Text = "km";
            // 
            // spanLabel
            // 
            this.spanLabel.AutoSize = true;
            this.spanLabel.Location = new System.Drawing.Point(41, 76);
            this.spanLabel.Name = "spanLabel";
            this.spanLabel.Size = new System.Drawing.Size(79, 15);
            this.spanLabel.TabIndex = 27;
            this.spanLabel.Text = "Up-river Span";
            // 
            // spanBox
            // 
            this.spanBox.Location = new System.Drawing.Point(127, 73);
            this.spanBox.Name = "spanBox";
            this.spanBox.Size = new System.Drawing.Size(71, 23);
            this.spanBox.TabIndex = 26;
            this.spanBox.Text = "5";
            this.spanBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 15);
            this.label4.TabIndex = 25;
            this.label4.Text = "(optional) endComID";
            // 
            // EndCOMIDBox
            // 
            this.EndCOMIDBox.Location = new System.Drawing.Point(127, 47);
            this.EndCOMIDBox.Name = "EndCOMIDBox";
            this.EndCOMIDBox.Size = new System.Drawing.Size(71, 23);
            this.EndCOMIDBox.TabIndex = 24;
            this.EndCOMIDBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 23;
            this.label1.Text = "ComID";
            // 
            // comidBox
            // 
            this.comidBox.Location = new System.Drawing.Point(127, 19);
            this.comidBox.Name = "comidBox";
            this.comidBox.Size = new System.Drawing.Size(71, 23);
            this.comidBox.TabIndex = 22;
            this.comidBox.Text = "23398915";
            this.comidBox.Leave += new System.EventHandler(this.comidBox_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(356, 576);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "Read Stream Network";
            this.label7.Visible = false;
            // 
            // ChooseTemplateButton
            // 
            this.ChooseTemplateButton.Location = new System.Drawing.Point(124, 138);
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
            this.HelpButton2.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton2.Image")));
            this.HelpButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton2.Location = new System.Drawing.Point(705, 9);
            this.HelpButton2.Name = "HelpButton2";
            this.HelpButton2.Size = new System.Drawing.Size(78, 28);
            this.HelpButton2.TabIndex = 30;
            this.HelpButton2.Text = "   Help";
            this.HelpButton2.UseVisualStyleBackColor = true;
            this.HelpButton2.Visible = false;
            this.HelpButton2.Click += new System.EventHandler(this.HelpButton2_Click);
            // 
            // infolabel1
            // 
            this.infolabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel1.AutoSize = true;
            this.infolabel1.Location = new System.Drawing.Point(619, 575);
            this.infolabel1.Name = "infolabel1";
            this.infolabel1.Size = new System.Drawing.Size(187, 15);
            this.infolabel1.TabIndex = 39;
            this.infolabel1.Text = "Click on a Lake/Reservoir to Select";
            // 
            // infolabel2
            // 
            this.infolabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infolabel2.AutoSize = true;
            this.infolabel2.Location = new System.Drawing.Point(619, 594);
            this.infolabel2.Name = "infolabel2";
            this.infolabel2.Size = new System.Drawing.Size(244, 15);
            this.infolabel2.TabIndex = 40;
            this.infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom.";
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(297, 55);
            this.webView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(830, 517);
            this.webView.TabIndex = 43;
            this.webView.ZoomFactor = 1D;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(1066, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(61, 23);
            this.button3.TabIndex = 46;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // SimNameEdit
            // 
            this.SimNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SimNameEdit.Location = new System.Drawing.Point(122, 14);
            this.SimNameEdit.Name = "SimNameEdit";
            this.SimNameEdit.Size = new System.Drawing.Size(260, 23);
            this.SimNameEdit.TabIndex = 47;
            this.SimNameEdit.Tag = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(17, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 48;
            this.label2.Text = "Simulation Name";
            // 
            // WBCLabel
            // 
            this.WBCLabel.AutoSize = true;
            this.WBCLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.WBCLabel.Location = new System.Drawing.Point(17, 57);
            this.WBCLabel.Name = "WBCLabel";
            this.WBCLabel.Size = new System.Drawing.Size(148, 15);
            this.WBCLabel.TabIndex = 49;
            this.WBCLabel.Text = "WB COMID:  (unselected)";
            // 
            // SimJSONLabel
            // 
            this.SimJSONLabel.AutoSize = true;
            this.SimJSONLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.SimJSONLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SimJSONLabel.Location = new System.Drawing.Point(17, 118);
            this.SimJSONLabel.Name = "SimJSONLabel";
            this.SimJSONLabel.Size = new System.Drawing.Size(113, 15);
            this.SimJSONLabel.TabIndex = 50;
            this.SimJSONLabel.Text = "\"Default Lake.JSON\"";
            // 
            // NewSimForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 620);
            this.Controls.Add(this.SimJSONLabel);
            this.Controls.Add(this.WBCLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SimNameEdit);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.infolabel2);
            this.Controls.Add(this.infolabel1);
            this.Controls.Add(this.HelpButton2);
            this.Controls.Add(this.ChooseTemplateButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ReadNetworkPanel);
            this.Controls.Add(this.SimBaseLabel);
            this.Controls.Add(this.button1);
            this.MinimumSize = new System.Drawing.Size(1003, 553);
            this.Name = "NewSimForm";
            this.Text = "NewSimForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewSimForm_FormClosing);
            this.Shown += new System.EventHandler(this.NewSimForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.NewSimForm_ResizeEnd);
            this.Resize += new System.EventHandler(this.NewSimForm_Resize);
            this.ReadNetworkPanel.ResumeLayout(false);
            this.ReadNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox EndCOMIDBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox comidBox;
        private System.Windows.Forms.Label label7;
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
        private System.Windows.Forms.Label WBCLabel;
        private System.Windows.Forms.Label SimJSONLabel;
    }
}