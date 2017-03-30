namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.txtLatitude = new System.Windows.Forms.TextBox();
            this.txtLongitude = new System.Windows.Forms.TextBox();
            this.cmbDataSet = new System.Windows.Forms.ComboBox();
            this.lblDataset = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.txtStartDate = new System.Windows.Forms.TextBox();
            this.txtEndDate = new System.Windows.Forms.TextBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.cmbSource = new System.Windows.Forms.ComboBox();
            this.lblERROR = new System.Windows.Forms.Label();
            this.grpbTime = new System.Windows.Forms.GroupBox();
            this.rdbLocal = new System.Windows.Forms.RadioButton();
            this.rdbGMT = new System.Windows.Forms.RadioButton();
            this.bttnData = new System.Windows.Forms.Button();
            this.lblElevation = new System.Windows.Forms.Label();
            this.lblElevationResult = new System.Windows.Forms.Label();
            this.lblLatitude2Result = new System.Windows.Forms.Label();
            this.lblLatitude2 = new System.Windows.Forms.Label();
            this.lblLongitude2Result = new System.Windows.Forms.Label();
            this.lblLongitude2 = new System.Windows.Forms.Label();
            this.lblCellSize = new System.Windows.Forms.Label();
            this.lblCellSizeResult = new System.Windows.Forms.Label();
            this.lblShapefile = new System.Windows.Forms.Label();
            this.chkbShapefile = new System.Windows.Forms.CheckBox();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.chklstbxDatasetOptions = new System.Windows.Forms.CheckedListBox();
            this.lblSoilMoisture = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.lblTimerResult = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dGVData = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btnPrecipCompare = new System.Windows.Forms.Button();
            this.grpbTime.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGVData)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(38, 29);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(85, 18);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "HMS Data";
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(57, 110);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(63, 17);
            this.lblLatitude.TabIndex = 1;
            this.lblLatitude.Text = "Latitude:";
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(57, 145);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(75, 17);
            this.lblLongitude.TabIndex = 2;
            this.lblLongitude.Text = "Longitude:";
            // 
            // txtLatitude
            // 
            this.txtLatitude.Location = new System.Drawing.Point(143, 105);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.Size = new System.Drawing.Size(155, 22);
            this.txtLatitude.TabIndex = 2;
            // 
            // txtLongitude
            // 
            this.txtLongitude.Location = new System.Drawing.Point(143, 140);
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.Size = new System.Drawing.Size(155, 22);
            this.txtLongitude.TabIndex = 3;
            // 
            // cmbDataSet
            // 
            this.cmbDataSet.FormattingEnabled = true;
            this.cmbDataSet.Items.AddRange(new object[] {
            "Precipitation",
            "LandSurfaceFlow",
            "BaseFlow",
            "Evapotranspiration",
            "SoilMoisture",
            "Temperature",
            "TotalFlow"});
            this.cmbDataSet.Location = new System.Drawing.Point(143, 70);
            this.cmbDataSet.Name = "cmbDataSet";
            this.cmbDataSet.Size = new System.Drawing.Size(155, 24);
            this.cmbDataSet.TabIndex = 1;
            this.cmbDataSet.SelectedValueChanged += new System.EventHandler(this.cmbDataSet_SelectedValueChanged);
            // 
            // lblDataset
            // 
            this.lblDataset.AutoSize = true;
            this.lblDataset.Location = new System.Drawing.Point(57, 77);
            this.lblDataset.Name = "lblDataset";
            this.lblDataset.Size = new System.Drawing.Size(61, 17);
            this.lblDataset.TabIndex = 6;
            this.lblDataset.Text = "Dataset:";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(491, 110);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(76, 17);
            this.lblStartDate.TabIndex = 7;
            this.lblStartDate.Text = "Start Date:";
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(491, 144);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(71, 17);
            this.lblEndDate.TabIndex = 8;
            this.lblEndDate.Text = "End Date:";
            // 
            // txtStartDate
            // 
            this.txtStartDate.Location = new System.Drawing.Point(582, 105);
            this.txtStartDate.Name = "txtStartDate";
            this.txtStartDate.Size = new System.Drawing.Size(113, 22);
            this.txtStartDate.TabIndex = 5;
            // 
            // txtEndDate
            // 
            this.txtEndDate.Location = new System.Drawing.Point(582, 139);
            this.txtEndDate.Name = "txtEndDate";
            this.txtEndDate.Size = new System.Drawing.Size(113, 22);
            this.txtEndDate.TabIndex = 6;
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(491, 77);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(53, 17);
            this.lblSource.TabIndex = 11;
            this.lblSource.Text = "Source";
            // 
            // cmbSource
            // 
            this.cmbSource.FormattingEnabled = true;
            this.cmbSource.Items.AddRange(new object[] {
            "NLDAS",
            "GLDAS"});
            this.cmbSource.Location = new System.Drawing.Point(582, 68);
            this.cmbSource.Name = "cmbSource";
            this.cmbSource.Size = new System.Drawing.Size(113, 24);
            this.cmbSource.TabIndex = 4;
            this.cmbSource.SelectedValueChanged += new System.EventHandler(this.cmbSource_SelectedValueChanged);
            // 
            // lblERROR
            // 
            this.lblERROR.AutoSize = true;
            this.lblERROR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblERROR.Location = new System.Drawing.Point(57, 178);
            this.lblERROR.Name = "lblERROR";
            this.lblERROR.Size = new System.Drawing.Size(132, 17);
            this.lblERROR.TabIndex = 14;
            this.lblERROR.Text = "ERROR_status_text";
            this.lblERROR.Visible = false;
            this.lblERROR.TextChanged += new System.EventHandler(this.lblERROR_TextChanged);
            // 
            // grpbTime
            // 
            this.grpbTime.Controls.Add(this.rdbLocal);
            this.grpbTime.Controls.Add(this.rdbGMT);
            this.grpbTime.Location = new System.Drawing.Point(725, 68);
            this.grpbTime.Name = "grpbTime";
            this.grpbTime.Size = new System.Drawing.Size(200, 50);
            this.grpbTime.TabIndex = 7;
            this.grpbTime.TabStop = false;
            this.grpbTime.Text = "Time Options";
            // 
            // rdbLocal
            // 
            this.rdbLocal.AutoSize = true;
            this.rdbLocal.Location = new System.Drawing.Point(109, 21);
            this.rdbLocal.Name = "rdbLocal";
            this.rdbLocal.Size = new System.Drawing.Size(63, 21);
            this.rdbLocal.TabIndex = 1;
            this.rdbLocal.Text = "Local";
            this.rdbLocal.UseVisualStyleBackColor = true;
            // 
            // rdbGMT
            // 
            this.rdbGMT.AutoSize = true;
            this.rdbGMT.Checked = true;
            this.rdbGMT.Location = new System.Drawing.Point(30, 21);
            this.rdbGMT.Name = "rdbGMT";
            this.rdbGMT.Size = new System.Drawing.Size(60, 21);
            this.rdbGMT.TabIndex = 0;
            this.rdbGMT.TabStop = true;
            this.rdbGMT.Text = "GMT";
            this.rdbGMT.UseVisualStyleBackColor = true;
            // 
            // bttnData
            // 
            this.bttnData.Location = new System.Drawing.Point(734, 224);
            this.bttnData.Name = "bttnData";
            this.bttnData.Size = new System.Drawing.Size(200, 35);
            this.bttnData.TabIndex = 8;
            this.bttnData.Text = "Get Data";
            this.bttnData.UseVisualStyleBackColor = true;
            this.bttnData.Click += new System.EventHandler(this.bttnData_Click);
            // 
            // lblElevation
            // 
            this.lblElevation.AutoSize = true;
            this.lblElevation.Location = new System.Drawing.Point(722, 328);
            this.lblElevation.Name = "lblElevation";
            this.lblElevation.Size = new System.Drawing.Size(95, 17);
            this.lblElevation.TabIndex = 15;
            this.lblElevation.Text = "Elevation (m):";
            // 
            // lblElevationResult
            // 
            this.lblElevationResult.AutoSize = true;
            this.lblElevationResult.Location = new System.Drawing.Point(831, 328);
            this.lblElevationResult.Name = "lblElevationResult";
            this.lblElevationResult.Size = new System.Drawing.Size(101, 17);
            this.lblElevationResult.TabIndex = 16;
            this.lblElevationResult.Text = "elevationValue";
            this.lblElevationResult.Visible = false;
            // 
            // lblLatitude2Result
            // 
            this.lblLatitude2Result.AutoSize = true;
            this.lblLatitude2Result.Location = new System.Drawing.Point(831, 362);
            this.lblLatitude2Result.Name = "lblLatitude2Result";
            this.lblLatitude2Result.Size = new System.Drawing.Size(90, 17);
            this.lblLatitude2Result.TabIndex = 18;
            this.lblLatitude2Result.Text = "latitudeValue";
            this.lblLatitude2Result.Visible = false;
            // 
            // lblLatitude2
            // 
            this.lblLatitude2.AutoSize = true;
            this.lblLatitude2.Location = new System.Drawing.Point(722, 362);
            this.lblLatitude2.Name = "lblLatitude2";
            this.lblLatitude2.Size = new System.Drawing.Size(63, 17);
            this.lblLatitude2.TabIndex = 17;
            this.lblLatitude2.Text = "Latitude:";
            // 
            // lblLongitude2Result
            // 
            this.lblLongitude2Result.AutoSize = true;
            this.lblLongitude2Result.Location = new System.Drawing.Point(831, 396);
            this.lblLongitude2Result.Name = "lblLongitude2Result";
            this.lblLongitude2Result.Size = new System.Drawing.Size(102, 17);
            this.lblLongitude2Result.TabIndex = 20;
            this.lblLongitude2Result.Text = "longitudeValue";
            this.lblLongitude2Result.Visible = false;
            // 
            // lblLongitude2
            // 
            this.lblLongitude2.AutoSize = true;
            this.lblLongitude2.Location = new System.Drawing.Point(722, 396);
            this.lblLongitude2.Name = "lblLongitude2";
            this.lblLongitude2.Size = new System.Drawing.Size(75, 17);
            this.lblLongitude2.TabIndex = 19;
            this.lblLongitude2.Text = "Longitude:";
            // 
            // lblCellSize
            // 
            this.lblCellSize.AutoSize = true;
            this.lblCellSize.Location = new System.Drawing.Point(722, 429);
            this.lblCellSize.Name = "lblCellSize";
            this.lblCellSize.Size = new System.Drawing.Size(104, 17);
            this.lblCellSize.TabIndex = 21;
            this.lblCellSize.Text = "Cell Size (deg):";
            // 
            // lblCellSizeResult
            // 
            this.lblCellSizeResult.AutoSize = true;
            this.lblCellSizeResult.Location = new System.Drawing.Point(832, 429);
            this.lblCellSizeResult.Name = "lblCellSizeResult";
            this.lblCellSizeResult.Size = new System.Drawing.Size(90, 17);
            this.lblCellSizeResult.TabIndex = 22;
            this.lblCellSizeResult.Text = "cellsizeValue";
            this.lblCellSizeResult.Visible = false;
            // 
            // lblShapefile
            // 
            this.lblShapefile.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShapefile.Location = new System.Drawing.Point(722, 149);
            this.lblShapefile.Name = "lblShapefile";
            this.lblShapefile.Size = new System.Drawing.Size(203, 23);
            this.lblShapefile.TabIndex = 23;
            this.lblShapefile.Text = "shapefile path";
            // 
            // chkbShapefile
            // 
            this.chkbShapefile.AutoSize = true;
            this.chkbShapefile.Location = new System.Drawing.Point(725, 125);
            this.chkbShapefile.Name = "chkbShapefile";
            this.chkbShapefile.Size = new System.Drawing.Size(89, 21);
            this.chkbShapefile.TabIndex = 24;
            this.chkbShapefile.Text = "Shapefile";
            this.chkbShapefile.UseVisualStyleBackColor = true;
            this.chkbShapefile.CheckedChanged += new System.EventHandler(this.chkbShapefile_CheckedChanged);
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(786, 277);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(96, 27);
            this.btnSaveData.TabIndex = 25;
            this.btnSaveData.Text = "Save Data";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // chklstbxDatasetOptions
            // 
            this.chklstbxDatasetOptions.FormattingEnabled = true;
            this.chklstbxDatasetOptions.Location = new System.Drawing.Point(328, 90);
            this.chklstbxDatasetOptions.Name = "chklstbxDatasetOptions";
            this.chklstbxDatasetOptions.Size = new System.Drawing.Size(142, 72);
            this.chklstbxDatasetOptions.TabIndex = 27;
            this.chklstbxDatasetOptions.Visible = false;
            // 
            // lblSoilMoisture
            // 
            this.lblSoilMoisture.AutoSize = true;
            this.lblSoilMoisture.Location = new System.Drawing.Point(328, 70);
            this.lblSoilMoisture.Name = "lblSoilMoisture";
            this.lblSoilMoisture.Size = new System.Drawing.Size(146, 17);
            this.lblSoilMoisture.TabIndex = 28;
            this.lblSoilMoisture.Text = "Soil Moisture Options:";
            this.lblSoilMoisture.Visible = false;
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(722, 458);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(48, 17);
            this.lblTimer.TabIndex = 30;
            this.lblTimer.Text = "Timer:";
            // 
            // lblTimerResult
            // 
            this.lblTimerResult.AutoSize = true;
            this.lblTimerResult.Location = new System.Drawing.Point(777, 458);
            this.lblTimerResult.Name = "lblTimerResult";
            this.lblTimerResult.Size = new System.Drawing.Size(98, 17);
            this.lblTimerResult.TabIndex = 31;
            this.lblTimerResult.Text = "lblTimerResult";
            this.lblTimerResult.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(667, 512);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dGVData);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(667, 512);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Timeseries";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dGVData
            // 
            this.dGVData.AllowUserToAddRows = false;
            this.dGVData.AllowUserToDeleteRows = false;
            this.dGVData.AllowUserToResizeColumns = false;
            this.dGVData.AllowUserToResizeRows = false;
            this.dGVData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVData.Location = new System.Drawing.Point(15, 19);
            this.dGVData.Name = "dGVData";
            this.dGVData.RowTemplate.Height = 24;
            this.dGVData.Size = new System.Drawing.Size(635, 470);
            this.dGVData.TabIndex = 14;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(41, 207);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(675, 541);
            this.tabControl1.TabIndex = 32;
            // 
            // btnPrecipCompare
            // 
            this.btnPrecipCompare.Location = new System.Drawing.Point(734, 717);
            this.btnPrecipCompare.Name = "btnPrecipCompare";
            this.btnPrecipCompare.Size = new System.Drawing.Size(191, 31);
            this.btnPrecipCompare.TabIndex = 33;
            this.btnPrecipCompare.Text = "Precipitaiton Compare";
            this.btnPrecipCompare.UseVisualStyleBackColor = true;
            this.btnPrecipCompare.Click += new System.EventHandler(this.btnPrecipCompare_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 779);
            this.Controls.Add(this.btnPrecipCompare);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblTimerResult);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblSoilMoisture);
            this.Controls.Add(this.chklstbxDatasetOptions);
            this.Controls.Add(this.btnSaveData);
            this.Controls.Add(this.chkbShapefile);
            this.Controls.Add(this.lblShapefile);
            this.Controls.Add(this.lblCellSizeResult);
            this.Controls.Add(this.lblCellSize);
            this.Controls.Add(this.lblLongitude2Result);
            this.Controls.Add(this.lblLongitude2);
            this.Controls.Add(this.lblLatitude2Result);
            this.Controls.Add(this.lblLatitude2);
            this.Controls.Add(this.lblElevationResult);
            this.Controls.Add(this.lblElevation);
            this.Controls.Add(this.bttnData);
            this.Controls.Add(this.grpbTime);
            this.Controls.Add(this.lblERROR);
            this.Controls.Add(this.cmbSource);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.txtEndDate);
            this.Controls.Add(this.txtStartDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.lblDataset);
            this.Controls.Add(this.cmbDataSet);
            this.Controls.Add(this.txtLongitude);
            this.Controls.Add(this.txtLatitude);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.lblLatitude);
            this.Controls.Add(this.lblTitle);
            this.Name = "Form1";
            this.Text = "Form1";
            this.grpbTime.ResumeLayout(false);
            this.grpbTime.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGVData)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.TextBox txtLatitude;
        private System.Windows.Forms.TextBox txtLongitude;
        private System.Windows.Forms.ComboBox cmbDataSet;
        private System.Windows.Forms.Label lblDataset;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.TextBox txtStartDate;
        private System.Windows.Forms.TextBox txtEndDate;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.ComboBox cmbSource;
        private System.Windows.Forms.Label lblERROR;
        private System.Windows.Forms.GroupBox grpbTime;
        private System.Windows.Forms.RadioButton rdbLocal;
        private System.Windows.Forms.RadioButton rdbGMT;
        private System.Windows.Forms.Button bttnData;
        private System.Windows.Forms.Label lblElevation;
        private System.Windows.Forms.Label lblElevationResult;
        private System.Windows.Forms.Label lblLatitude2Result;
        private System.Windows.Forms.Label lblLatitude2;
        private System.Windows.Forms.Label lblLongitude2Result;
        private System.Windows.Forms.Label lblLongitude2;
        private System.Windows.Forms.Label lblCellSize;
        private System.Windows.Forms.Label lblCellSizeResult;
        private System.Windows.Forms.Label lblShapefile;
        private System.Windows.Forms.CheckBox chkbShapefile;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.CheckedListBox chklstbxDatasetOptions;
        private System.Windows.Forms.Label lblSoilMoisture;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label lblTimerResult;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dGVData;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button btnPrecipCompare;
    }
}

