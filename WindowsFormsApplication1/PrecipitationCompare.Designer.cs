namespace WindowsFormsApplication1
{
    partial class PrecipitationCompare
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
            this.lblPrecipCompare = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.cmbState = new System.Windows.Forms.ComboBox();
            this.lblStation = new System.Windows.Forms.Label();
            this.cmbStation = new System.Windows.Forms.ComboBox();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblLat = new System.Windows.Forms.Label();
            this.lblLatValue = new System.Windows.Forms.Label();
            this.lblLong = new System.Windows.Forms.Label();
            this.lblLongValue = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.lblStartDateValue = new System.Windows.Forms.Label();
            this.lblEndDateValue = new System.Windows.Forms.Label();
            this.lblLocationID = new System.Windows.Forms.Label();
            this.lblLocationIDValue = new System.Windows.Forms.Label();
            this.lblNLDAS = new System.Windows.Forms.Label();
            this.lblNLDASendDateValue = new System.Windows.Forms.Label();
            this.lblNLDASstartDateValue = new System.Windows.Forms.Label();
            this.lblNLDASendDate = new System.Windows.Forms.Label();
            this.lblNLDASstartDate = new System.Windows.Forms.Label();
            this.lblGLDASendDateValue = new System.Windows.Forms.Label();
            this.lblGLDASstartDateValue = new System.Windows.Forms.Label();
            this.lblGLDASendDate = new System.Windows.Forms.Label();
            this.lblGLDASstartDate = new System.Windows.Forms.Label();
            this.lblGLDAS = new System.Windows.Forms.Label();
            this.lblDaymetEndDateValue = new System.Windows.Forms.Label();
            this.lblDaymetStartDateValue = new System.Windows.Forms.Label();
            this.lblDaymetEndDate = new System.Windows.Forms.Label();
            this.lblDaymetStartDate = new System.Windows.Forms.Label();
            this.lblDaymet = new System.Windows.Forms.Label();
            this.lblStartDateFinal = new System.Windows.Forms.Label();
            this.txtBxStartDate = new System.Windows.Forms.TextBox();
            this.txtBxEndDate = new System.Windows.Forms.TextBox();
            this.lblEndDateFinal = new System.Windows.Forms.Label();
            this.lblDateRange = new System.Windows.Forms.Label();
            this.bttnCompareData = new System.Windows.Forms.Button();
            this.dataGVCompare = new System.Windows.Forms.DataGridView();
            this.lblerrorMsg = new System.Windows.Forms.Label();
            this.lblStationDetails = new System.Windows.Forms.Label();
            this.bttnSave = new System.Windows.Forms.Button();
            this.lblStationID = new System.Windows.Forms.Label();
            this.txtBxStationID = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGVCompare)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPrecipCompare
            // 
            this.lblPrecipCompare.AutoSize = true;
            this.lblPrecipCompare.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecipCompare.Location = new System.Drawing.Point(32, 31);
            this.lblPrecipCompare.Name = "lblPrecipCompare";
            this.lblPrecipCompare.Size = new System.Drawing.Size(225, 20);
            this.lblPrecipCompare.TabIndex = 0;
            this.lblPrecipCompare.Text = "Precipication Comparison";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(76, 93);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(45, 17);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "State:";
            // 
            // cmbState
            // 
            this.cmbState.FormattingEnabled = true;
            this.cmbState.Location = new System.Drawing.Point(136, 89);
            this.cmbState.Name = "cmbState";
            this.cmbState.Size = new System.Drawing.Size(54, 24);
            this.cmbState.TabIndex = 2;
            this.cmbState.SelectedValueChanged += new System.EventHandler(this.cmbState_SelectedValueChanged);
            this.cmbState.Click += new System.EventHandler(this.cmbState_SelectedValueChanged);
            // 
            // lblStation
            // 
            this.lblStation.AutoSize = true;
            this.lblStation.Location = new System.Drawing.Point(76, 119);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(56, 17);
            this.lblStation.TabIndex = 3;
            this.lblStation.Text = "Station:";
            this.lblStation.Visible = false;
            // 
            // cmbStation
            // 
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(136, 115);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(271, 24);
            this.cmbStation.TabIndex = 4;
            this.cmbStation.Visible = false;
            this.cmbStation.SelectedIndexChanged += new System.EventHandler(this.cmbStation_SelectedIndexChanged);
            // 
            // lblDetails
            // 
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(76, 186);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(103, 17);
            this.lblDetails.TabIndex = 5;
            this.lblDetails.Text = "Station Details:";
            this.lblDetails.Visible = false;
            // 
            // lblLat
            // 
            this.lblLat.AutoSize = true;
            this.lblLat.Location = new System.Drawing.Point(97, 212);
            this.lblLat.Name = "lblLat";
            this.lblLat.Size = new System.Drawing.Size(63, 17);
            this.lblLat.TabIndex = 6;
            this.lblLat.Text = "Latitude:";
            this.lblLat.Visible = false;
            // 
            // lblLatValue
            // 
            this.lblLatValue.AutoSize = true;
            this.lblLatValue.Location = new System.Drawing.Point(175, 212);
            this.lblLatValue.Name = "lblLatValue";
            this.lblLatValue.Size = new System.Drawing.Size(42, 17);
            this.lblLatValue.TabIndex = 7;
            this.lblLatValue.Text = "lblLat";
            this.lblLatValue.Visible = false;
            // 
            // lblLong
            // 
            this.lblLong.AutoSize = true;
            this.lblLong.Location = new System.Drawing.Point(97, 229);
            this.lblLong.Name = "lblLong";
            this.lblLong.Size = new System.Drawing.Size(75, 17);
            this.lblLong.TabIndex = 8;
            this.lblLong.Text = "Longitude:";
            this.lblLong.Visible = false;
            // 
            // lblLongValue
            // 
            this.lblLongValue.AutoSize = true;
            this.lblLongValue.Location = new System.Drawing.Point(175, 229);
            this.lblLongValue.Name = "lblLongValue";
            this.lblLongValue.Size = new System.Drawing.Size(54, 17);
            this.lblLongValue.TabIndex = 9;
            this.lblLongValue.Text = "lblLong";
            this.lblLongValue.Visible = false;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(97, 246);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(72, 17);
            this.lblStartDate.TabIndex = 10;
            this.lblStartDate.Text = "StartDate:";
            this.lblStartDate.Visible = false;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(97, 263);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(67, 17);
            this.lblEndDate.TabIndex = 11;
            this.lblEndDate.Text = "EndDate:";
            this.lblEndDate.Visible = false;
            // 
            // lblStartDateValue
            // 
            this.lblStartDateValue.AutoSize = true;
            this.lblStartDateValue.Location = new System.Drawing.Point(175, 246);
            this.lblStartDateValue.Name = "lblStartDateValue";
            this.lblStartDateValue.Size = new System.Drawing.Size(82, 17);
            this.lblStartDateValue.TabIndex = 12;
            this.lblStartDateValue.Text = "lblStartDate";
            this.lblStartDateValue.Visible = false;
            // 
            // lblEndDateValue
            // 
            this.lblEndDateValue.AutoSize = true;
            this.lblEndDateValue.Location = new System.Drawing.Point(175, 263);
            this.lblEndDateValue.Name = "lblEndDateValue";
            this.lblEndDateValue.Size = new System.Drawing.Size(77, 17);
            this.lblEndDateValue.TabIndex = 13;
            this.lblEndDateValue.Text = "lblEndDate";
            this.lblEndDateValue.Visible = false;
            // 
            // lblLocationID
            // 
            this.lblLocationID.AutoSize = true;
            this.lblLocationID.Location = new System.Drawing.Point(97, 280);
            this.lblLocationID.Name = "lblLocationID";
            this.lblLocationID.Size = new System.Drawing.Size(79, 17);
            this.lblLocationID.TabIndex = 14;
            this.lblLocationID.Text = "LocationID:";
            this.lblLocationID.Visible = false;
            // 
            // lblLocationIDValue
            // 
            this.lblLocationIDValue.AutoSize = true;
            this.lblLocationIDValue.Location = new System.Drawing.Point(175, 280);
            this.lblLocationIDValue.Name = "lblLocationIDValue";
            this.lblLocationIDValue.Size = new System.Drawing.Size(89, 17);
            this.lblLocationIDValue.TabIndex = 15;
            this.lblLocationIDValue.Text = "lblLocationID";
            this.lblLocationIDValue.Visible = false;
            // 
            // lblNLDAS
            // 
            this.lblNLDAS.AutoSize = true;
            this.lblNLDAS.Location = new System.Drawing.Point(291, 186);
            this.lblNLDAS.Name = "lblNLDAS";
            this.lblNLDAS.Size = new System.Drawing.Size(105, 17);
            this.lblNLDAS.TabIndex = 16;
            this.lblNLDAS.Text = "NLDAS Details:";
            this.lblNLDAS.Visible = false;
            // 
            // lblNLDASendDateValue
            // 
            this.lblNLDASendDateValue.AutoSize = true;
            this.lblNLDASendDateValue.Location = new System.Drawing.Point(381, 263);
            this.lblNLDASendDateValue.Name = "lblNLDASendDateValue";
            this.lblNLDASendDateValue.Size = new System.Drawing.Size(77, 17);
            this.lblNLDASendDateValue.TabIndex = 20;
            this.lblNLDASendDateValue.Text = "lblEndDate";
            this.lblNLDASendDateValue.Visible = false;
            // 
            // lblNLDASstartDateValue
            // 
            this.lblNLDASstartDateValue.AutoSize = true;
            this.lblNLDASstartDateValue.Location = new System.Drawing.Point(381, 246);
            this.lblNLDASstartDateValue.Name = "lblNLDASstartDateValue";
            this.lblNLDASstartDateValue.Size = new System.Drawing.Size(82, 17);
            this.lblNLDASstartDateValue.TabIndex = 19;
            this.lblNLDASstartDateValue.Text = "lblStartDate";
            this.lblNLDASstartDateValue.Visible = false;
            // 
            // lblNLDASendDate
            // 
            this.lblNLDASendDate.AutoSize = true;
            this.lblNLDASendDate.Location = new System.Drawing.Point(308, 263);
            this.lblNLDASendDate.Name = "lblNLDASendDate";
            this.lblNLDASendDate.Size = new System.Drawing.Size(67, 17);
            this.lblNLDASendDate.TabIndex = 18;
            this.lblNLDASendDate.Text = "EndDate:";
            this.lblNLDASendDate.Visible = false;
            // 
            // lblNLDASstartDate
            // 
            this.lblNLDASstartDate.AutoSize = true;
            this.lblNLDASstartDate.Location = new System.Drawing.Point(308, 246);
            this.lblNLDASstartDate.Name = "lblNLDASstartDate";
            this.lblNLDASstartDate.Size = new System.Drawing.Size(72, 17);
            this.lblNLDASstartDate.TabIndex = 17;
            this.lblNLDASstartDate.Text = "StartDate:";
            this.lblNLDASstartDate.Visible = false;
            // 
            // lblGLDASendDateValue
            // 
            this.lblGLDASendDateValue.AutoSize = true;
            this.lblGLDASendDateValue.Location = new System.Drawing.Point(565, 263);
            this.lblGLDASendDateValue.Name = "lblGLDASendDateValue";
            this.lblGLDASendDateValue.Size = new System.Drawing.Size(77, 17);
            this.lblGLDASendDateValue.TabIndex = 25;
            this.lblGLDASendDateValue.Text = "lblEndDate";
            this.lblGLDASendDateValue.Visible = false;
            // 
            // lblGLDASstartDateValue
            // 
            this.lblGLDASstartDateValue.AutoSize = true;
            this.lblGLDASstartDateValue.Location = new System.Drawing.Point(565, 246);
            this.lblGLDASstartDateValue.Name = "lblGLDASstartDateValue";
            this.lblGLDASstartDateValue.Size = new System.Drawing.Size(82, 17);
            this.lblGLDASstartDateValue.TabIndex = 24;
            this.lblGLDASstartDateValue.Text = "lblStartDate";
            this.lblGLDASstartDateValue.Visible = false;
            // 
            // lblGLDASendDate
            // 
            this.lblGLDASendDate.AutoSize = true;
            this.lblGLDASendDate.Location = new System.Drawing.Point(492, 263);
            this.lblGLDASendDate.Name = "lblGLDASendDate";
            this.lblGLDASendDate.Size = new System.Drawing.Size(67, 17);
            this.lblGLDASendDate.TabIndex = 23;
            this.lblGLDASendDate.Text = "EndDate:";
            this.lblGLDASendDate.Visible = false;
            // 
            // lblGLDASstartDate
            // 
            this.lblGLDASstartDate.AutoSize = true;
            this.lblGLDASstartDate.Location = new System.Drawing.Point(492, 246);
            this.lblGLDASstartDate.Name = "lblGLDASstartDate";
            this.lblGLDASstartDate.Size = new System.Drawing.Size(72, 17);
            this.lblGLDASstartDate.TabIndex = 22;
            this.lblGLDASstartDate.Text = "StartDate:";
            this.lblGLDASstartDate.Visible = false;
            // 
            // lblGLDAS
            // 
            this.lblGLDAS.AutoSize = true;
            this.lblGLDAS.Location = new System.Drawing.Point(475, 186);
            this.lblGLDAS.Name = "lblGLDAS";
            this.lblGLDAS.Size = new System.Drawing.Size(106, 17);
            this.lblGLDAS.TabIndex = 21;
            this.lblGLDAS.Text = "GLDAS Details:";
            this.lblGLDAS.Visible = false;
            // 
            // lblDaymetEndDateValue
            // 
            this.lblDaymetEndDateValue.AutoSize = true;
            this.lblDaymetEndDateValue.Location = new System.Drawing.Point(753, 263);
            this.lblDaymetEndDateValue.Name = "lblDaymetEndDateValue";
            this.lblDaymetEndDateValue.Size = new System.Drawing.Size(77, 17);
            this.lblDaymetEndDateValue.TabIndex = 30;
            this.lblDaymetEndDateValue.Text = "lblEndDate";
            this.lblDaymetEndDateValue.Visible = false;
            // 
            // lblDaymetStartDateValue
            // 
            this.lblDaymetStartDateValue.AutoSize = true;
            this.lblDaymetStartDateValue.Location = new System.Drawing.Point(753, 246);
            this.lblDaymetStartDateValue.Name = "lblDaymetStartDateValue";
            this.lblDaymetStartDateValue.Size = new System.Drawing.Size(82, 17);
            this.lblDaymetStartDateValue.TabIndex = 29;
            this.lblDaymetStartDateValue.Text = "lblStartDate";
            this.lblDaymetStartDateValue.Visible = false;
            // 
            // lblDaymetEndDate
            // 
            this.lblDaymetEndDate.AutoSize = true;
            this.lblDaymetEndDate.Location = new System.Drawing.Point(680, 263);
            this.lblDaymetEndDate.Name = "lblDaymetEndDate";
            this.lblDaymetEndDate.Size = new System.Drawing.Size(67, 17);
            this.lblDaymetEndDate.TabIndex = 28;
            this.lblDaymetEndDate.Text = "EndDate:";
            this.lblDaymetEndDate.Visible = false;
            // 
            // lblDaymetStartDate
            // 
            this.lblDaymetStartDate.AutoSize = true;
            this.lblDaymetStartDate.Location = new System.Drawing.Point(680, 246);
            this.lblDaymetStartDate.Name = "lblDaymetStartDate";
            this.lblDaymetStartDate.Size = new System.Drawing.Size(72, 17);
            this.lblDaymetStartDate.TabIndex = 27;
            this.lblDaymetStartDate.Text = "StartDate:";
            this.lblDaymetStartDate.Visible = false;
            // 
            // lblDaymet
            // 
            this.lblDaymet.AutoSize = true;
            this.lblDaymet.Location = new System.Drawing.Point(663, 186);
            this.lblDaymet.Name = "lblDaymet";
            this.lblDaymet.Size = new System.Drawing.Size(107, 17);
            this.lblDaymet.TabIndex = 26;
            this.lblDaymet.Text = "Daymet Details:";
            this.lblDaymet.Visible = false;
            // 
            // lblStartDateFinal
            // 
            this.lblStartDateFinal.AutoSize = true;
            this.lblStartDateFinal.Location = new System.Drawing.Point(443, 92);
            this.lblStartDateFinal.Name = "lblStartDateFinal";
            this.lblStartDateFinal.Size = new System.Drawing.Size(76, 17);
            this.lblStartDateFinal.TabIndex = 31;
            this.lblStartDateFinal.Text = "Start Date:";
            this.lblStartDateFinal.Visible = false;
            // 
            // txtBxStartDate
            // 
            this.txtBxStartDate.Location = new System.Drawing.Point(525, 88);
            this.txtBxStartDate.Name = "txtBxStartDate";
            this.txtBxStartDate.Size = new System.Drawing.Size(100, 22);
            this.txtBxStartDate.TabIndex = 32;
            this.txtBxStartDate.Visible = false;
            // 
            // txtBxEndDate
            // 
            this.txtBxEndDate.Location = new System.Drawing.Point(525, 116);
            this.txtBxEndDate.Name = "txtBxEndDate";
            this.txtBxEndDate.Size = new System.Drawing.Size(100, 22);
            this.txtBxEndDate.TabIndex = 34;
            this.txtBxEndDate.Visible = false;
            // 
            // lblEndDateFinal
            // 
            this.lblEndDateFinal.AutoSize = true;
            this.lblEndDateFinal.Location = new System.Drawing.Point(443, 119);
            this.lblEndDateFinal.Name = "lblEndDateFinal";
            this.lblEndDateFinal.Size = new System.Drawing.Size(71, 17);
            this.lblEndDateFinal.TabIndex = 33;
            this.lblEndDateFinal.Text = "End Date:";
            this.lblEndDateFinal.Visible = false;
            // 
            // lblDateRange
            // 
            this.lblDateRange.AutoSize = true;
            this.lblDateRange.Location = new System.Drawing.Point(443, 68);
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Size = new System.Drawing.Size(84, 17);
            this.lblDateRange.TabIndex = 35;
            this.lblDateRange.Text = "Date Range";
            this.lblDateRange.Visible = false;
            // 
            // bttnCompareData
            // 
            this.bttnCompareData.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.bttnCompareData.Location = new System.Drawing.Point(666, 92);
            this.bttnCompareData.Name = "bttnCompareData";
            this.bttnCompareData.Size = new System.Drawing.Size(177, 40);
            this.bttnCompareData.TabIndex = 36;
            this.bttnCompareData.Text = "Compare Data";
            this.bttnCompareData.UseVisualStyleBackColor = false;
            this.bttnCompareData.Visible = false;
            this.bttnCompareData.Click += new System.EventHandler(this.bttnCompareData_Click);
            // 
            // dataGVCompare
            // 
            this.dataGVCompare.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGVCompare.Location = new System.Drawing.Point(79, 328);
            this.dataGVCompare.Name = "dataGVCompare";
            this.dataGVCompare.RowTemplate.Height = 24;
            this.dataGVCompare.Size = new System.Drawing.Size(764, 419);
            this.dataGVCompare.TabIndex = 37;
            this.dataGVCompare.Visible = false;
            // 
            // lblerrorMsg
            // 
            this.lblerrorMsg.AutoSize = true;
            this.lblerrorMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblerrorMsg.Location = new System.Drawing.Point(86, 308);
            this.lblerrorMsg.Name = "lblerrorMsg";
            this.lblerrorMsg.Size = new System.Drawing.Size(162, 17);
            this.lblerrorMsg.TabIndex = 38;
            this.lblerrorMsg.Text = "ERROR/status message";
            this.lblerrorMsg.Visible = false;
            // 
            // lblStationDetails
            // 
            this.lblStationDetails.AutoSize = true;
            this.lblStationDetails.Location = new System.Drawing.Point(76, 68);
            this.lblStationDetails.Name = "lblStationDetails";
            this.lblStationDetails.Size = new System.Drawing.Size(114, 17);
            this.lblStationDetails.TabIndex = 39;
            this.lblStationDetails.Text = "Station Selection";
            // 
            // bttnSave
            // 
            this.bttnSave.Location = new System.Drawing.Point(756, 295);
            this.bttnSave.Name = "bttnSave";
            this.bttnSave.Size = new System.Drawing.Size(87, 27);
            this.bttnSave.TabIndex = 40;
            this.bttnSave.Text = "Save";
            this.bttnSave.UseVisualStyleBackColor = true;
            this.bttnSave.Visible = false;
            this.bttnSave.Click += new System.EventHandler(this.bttnSave_Click);
            // 
            // lblStationID
            // 
            this.lblStationID.Location = new System.Drawing.Point(235, 90);
            this.lblStationID.Name = "lblStationID";
            this.lblStationID.Size = new System.Drawing.Size(100, 23);
            this.lblStationID.TabIndex = 41;
            this.lblStationID.Text = "StationID:";
            // 
            // txtBxStationID
            // 
            this.txtBxStationID.Location = new System.Drawing.Point(307, 90);
            this.txtBxStationID.Name = "txtBxStationID";
            this.txtBxStationID.Size = new System.Drawing.Size(100, 22);
            this.txtBxStationID.TabIndex = 42;
            this.txtBxStationID.Leave += new System.EventHandler(this.txtBxStationID_TextChanged);
            // 
            // PrecipitationCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 775);
            this.Controls.Add(this.txtBxStationID);
            this.Controls.Add(this.lblStationID);
            this.Controls.Add(this.bttnSave);
            this.Controls.Add(this.lblStationDetails);
            this.Controls.Add(this.lblerrorMsg);
            this.Controls.Add(this.dataGVCompare);
            this.Controls.Add(this.bttnCompareData);
            this.Controls.Add(this.lblDateRange);
            this.Controls.Add(this.txtBxEndDate);
            this.Controls.Add(this.lblEndDateFinal);
            this.Controls.Add(this.txtBxStartDate);
            this.Controls.Add(this.lblStartDateFinal);
            this.Controls.Add(this.lblDaymetEndDateValue);
            this.Controls.Add(this.lblDaymetStartDateValue);
            this.Controls.Add(this.lblDaymetEndDate);
            this.Controls.Add(this.lblDaymetStartDate);
            this.Controls.Add(this.lblDaymet);
            this.Controls.Add(this.lblGLDASendDateValue);
            this.Controls.Add(this.lblGLDASstartDateValue);
            this.Controls.Add(this.lblGLDASendDate);
            this.Controls.Add(this.lblGLDASstartDate);
            this.Controls.Add(this.lblGLDAS);
            this.Controls.Add(this.lblNLDASendDateValue);
            this.Controls.Add(this.lblNLDASstartDateValue);
            this.Controls.Add(this.lblNLDASendDate);
            this.Controls.Add(this.lblNLDASstartDate);
            this.Controls.Add(this.lblNLDAS);
            this.Controls.Add(this.lblLocationIDValue);
            this.Controls.Add(this.lblLocationID);
            this.Controls.Add(this.lblEndDateValue);
            this.Controls.Add(this.lblStartDateValue);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.lblLongValue);
            this.Controls.Add(this.lblLong);
            this.Controls.Add(this.lblLatValue);
            this.Controls.Add(this.lblLat);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.cmbStation);
            this.Controls.Add(this.lblStation);
            this.Controls.Add(this.cmbState);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.lblPrecipCompare);
            this.Name = "PrecipitationCompare";
            this.Text = "PrecipitationCompare";
            ((System.ComponentModel.ISupportInitialize)(this.dataGVCompare)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPrecipCompare;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.ComboBox cmbState;
        private System.Windows.Forms.Label lblStation;
        private System.Windows.Forms.ComboBox cmbStation;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblLat;
        private System.Windows.Forms.Label lblLatValue;
        private System.Windows.Forms.Label lblLong;
        private System.Windows.Forms.Label lblLongValue;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Label lblStartDateValue;
        private System.Windows.Forms.Label lblEndDateValue;
        private System.Windows.Forms.Label lblLocationID;
        private System.Windows.Forms.Label lblLocationIDValue;
        private System.Windows.Forms.Label lblNLDAS;
        private System.Windows.Forms.Label lblNLDASendDateValue;
        private System.Windows.Forms.Label lblNLDASstartDateValue;
        private System.Windows.Forms.Label lblNLDASendDate;
        private System.Windows.Forms.Label lblNLDASstartDate;
        private System.Windows.Forms.Label lblGLDASendDateValue;
        private System.Windows.Forms.Label lblGLDASstartDateValue;
        private System.Windows.Forms.Label lblGLDASendDate;
        private System.Windows.Forms.Label lblGLDASstartDate;
        private System.Windows.Forms.Label lblGLDAS;
        private System.Windows.Forms.Label lblDaymetEndDateValue;
        private System.Windows.Forms.Label lblDaymetStartDateValue;
        private System.Windows.Forms.Label lblDaymetEndDate;
        private System.Windows.Forms.Label lblDaymetStartDate;
        private System.Windows.Forms.Label lblDaymet;
        private System.Windows.Forms.Label lblStartDateFinal;
        private System.Windows.Forms.TextBox txtBxStartDate;
        private System.Windows.Forms.TextBox txtBxEndDate;
        private System.Windows.Forms.Label lblEndDateFinal;
        private System.Windows.Forms.Label lblDateRange;
        private System.Windows.Forms.DataGridView dataGVCompare;
        private System.Windows.Forms.Label lblerrorMsg;
        private System.Windows.Forms.Label lblStationDetails;
        private System.Windows.Forms.Button bttnSave;
        private System.Windows.Forms.Button bttnCompareData;
        private System.Windows.Forms.Label lblStationID;
        private System.Windows.Forms.TextBox txtBxStationID;
    }
}