using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;
using Newtonsoft.Json;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private string shapefilePath;
        private List<HMSTimeSeries.HMSTimeSeries> ts;
        private List<HMSJSON.HMSJSON> jsonDataOLD;
        private bool localtime;
        private double gmtOffset;
        private HMSJSON.HMSJSON.HMSData jsonData;

        public Form1()
        {
            InitializeComponent();
        }

        private void bttnData_Click(object sender, EventArgs e)
        {
            string errorMsg = "";

            lblERROR.Visible = false;
            lblTimerResult.Visible = false;
            lblElevationResult.Visible = false;
            lblLatitude2Result.Visible = false;
            lblLongitude2Result.Visible = false;
            lblCellSizeResult.Visible = false;
            DateTime start = DateTime.Now;
            lblERROR.Text = "";
            dGVData.DataSource = null;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            string latitude = txtLatitude.Text;
            string longitude = txtLongitude.Text;
            string startDate = txtStartDate.Text;
            string endDate = txtEndDate.Text;
            string dataset = "";
            string source = "";

            if (cmbDataSet.SelectedItem != null)
            {
                dataset = cmbDataSet.SelectedItem.ToString();
            }
            else { errorMsg = "ERROR: Dataset required."; }
            if (cmbSource.SelectedItem != null)
            {
                source = cmbSource.SelectedItem.ToString();
            }
            else
            {
                if (errorMsg.Equals(""))
                {
                    errorMsg = "ERROR: Source required.";
                }
                else { errorMsg += " Source required."; }
            }
            if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }

            bool local = false;
            if (rdbLocal.Checked == true) { local = true; }

            double offset = 0.0;
            DataTable dt = new DataTable();
            ts = new List<HMSTimeSeries.HMSTimeSeries>();
            if (String.IsNullOrWhiteSpace(shapefilePath) && (txtLatitude.Text == "" || txtLongitude.Text == "")) { lblERROR.Text = "Shapefile or coordinates must be provided"; return; }

            lblERROR.Text = "Retrieving data.";
            if (dataset == "Precipitation")
            {
                HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = precip.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = precip.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = precip.jsonData;
            }
            else if (dataset == "LandSurfaceFlow")
            {
                HMSLandSurfaceFlow.LandSurfaceFlow landFlow = new HMSLandSurfaceFlow.LandSurfaceFlow();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    landFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    landFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = landFlow.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = landFlow.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = landFlow.jsonData;
            }
            else if ( dataset == "BaseFlow")
            {
                HMSBaseFlow.BaseFlow baseFlow = new HMSBaseFlow.BaseFlow();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    baseFlow = new HMSBaseFlow.BaseFlow(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    baseFlow = new HMSBaseFlow.BaseFlow(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = baseFlow.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = baseFlow.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = baseFlow.jsonData;
            }
            else if (dataset == "TotalFlow")
            {
                HMSTotalFlow.TotalFlow totalFlow = new HMSTotalFlow.TotalFlow();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    totalFlow = new HMSTotalFlow.TotalFlow(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    totalFlow = new HMSTotalFlow.TotalFlow(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = totalFlow.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = totalFlow.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = totalFlow.jsonData;
            }
            else if (dataset == "Evapotranspiration")
            {
                HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = evapo.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = evapo.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = evapo.jsonData;
            }
            else if (dataset == "Temperature")
            {
                HMSTemperature.Temperature temp = new HMSTemperature.Temperature();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    temp = new HMSTemperature.Temperature(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    temp = new HMSTemperature.Temperature(out errorMsg, startDate, endDate, source, local, shapefilePath);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = temp.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = temp.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = temp.jsonData;
            }
            else if (dataset == "Complete")     //Currently removed from list
            {
                HMSCompleteDataSet.CompleteDataSet complete = new HMSCompleteDataSet.CompleteDataSet();
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    complete = new HMSCompleteDataSet.CompleteDataSet(out errorMsg, latitude, longitude, startDate, endDate, source, local, "");   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    lblERROR.Text = "ERROR: Complete dataset only accepts latitude/longitude coordinates for input.";             //Returns data based on the centroid of the shapefile provided
                    return;
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = complete.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = complete.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                //this.jsonData = complete.jsonData;
            }
            else if (dataset == "SoilMoisture")
            {
                if (chklstbxDatasetOptions.CheckedIndices.Count == 0) { lblERROR.Text = "At least one soil moisture depth must be selected."; return; }
                HMSSoilMoisture.SoilMoisture soilM = new HMSSoilMoisture.SoilMoisture();
                int[] layers = new int[chklstbxDatasetOptions.CheckedIndices.Count];
                chklstbxDatasetOptions.CheckedIndices.CopyTo(layers, 0);
                if (String.IsNullOrWhiteSpace(shapefilePath))
                {
                    soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, latitude, longitude, startDate, endDate, source, local, "", layers);   //Returns data based on the latitude/longitude values provided
                }
                else
                {
                    soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, startDate, endDate, source, local, shapefilePath, layers);             //Returns data based on the centroid of the shapefile provided
                }
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                ts = soilM.GetDataSets(out errorMsg);
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                offset = soilM.gmtOffset;
                if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                this.jsonData = soilM.jsonData;
            }
            else
            {
                lblERROR.Text = "ERROR: Unknown dataset selected.";
                return;
            }
            if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
            this.gmtOffset = offset;
            this.localtime = local;

            PopulateTableV2(out errorMsg, dt, this.jsonData, local, offset);

            lblERROR.Text = "Data successfully retrieved.";
            
            dGVData.DataSource = dt.DefaultView;
            lblElevationResult.Text = Convert.ToString(ts[0].metaElev);
            lblElevationResult.Visible = true;
            lblLatitude2Result.Text = Convert.ToString(ts[0].metaLat);
            lblLatitude2Result.Visible = true;
            lblLongitude2Result.Text = Convert.ToString(ts[0].metaLon);
            lblLongitude2Result.Visible = true;
            lblCellSizeResult.Text = Convert.ToString(ts[0].cellSize / 2) + " x " + Convert.ToString(ts[0].cellSize / 2);
            lblCellSizeResult.Visible = true;

            DateTime end = DateTime.Now;
            lblTimerResult.Text = Convert.ToString(end - start);
            lblTimerResult.Visible = true;

            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void PopulateTable(out string errorMsg, DataTable dt, bool local, double offset)
        {
            errorMsg = "";
            string[] dataArray = ts[0].timeSeries.Split('\n');
            string units = @" (kg/m^2)";
            if (cmbSource.SelectedItem.ToString().Contains("Daymet")) { units = "(mm/day)"; }

            List<string[]> data = new List<string[]>();

            DataColumn dc1 = dt.Columns.Add("Date/Hour", typeof(String));
            for (int i = 0; i < ts.Count; i++)
            {
                string title = ts[i].metaLat + ", " + ts[i].metaLon;
                DataColumn dc = dt.Columns.Add(title + units, typeof(String));
                data.Add(ts[i].timeSeries.Split('\n'));
            }

            HMSGDAL.HMSGDAL gdal = new HMSGDAL.HMSGDAL();
            for (int i = 0; i < data[0].Length - 1; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                string[] line1 = data[0][i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (local == true)
                {
                    dr[0] = gdal.SetDateToLocal(out errorMsg, line1[0].Trim() + " " + line1[1].Trim(), offset);
                    if (errorMsg.Contains("ERROR")) { lblERROR.Text = errorMsg; return; }
                }
                else
                {
                    dr[0] = line1[0].Trim() + " " + line1[1].Trim();
                }
                dr[1] = line1[2].Trim();
                for (int j = 1; j < ts.Count; j++)
                {
                    string[] line2 = data[j][i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    dr[j+1] = line2[2].Trim();
                }
            }
        }

        private void PopulateTableV2(out string errorMsg, DataTable dt, HMSJSON.HMSJSON.HMSData jsonData, bool local, double offset)
        {
            errorMsg = "";
            string units = jsonData.metadata["unit"];
            string[] locTitle;
            if (jsonData.source.Contains("LDAS")) { locTitle = new string[2] { "lat", "lon" }; }
            else { locTitle = new string[2] { "Latitude", "Longitude" }; }
            DataColumn dc1 = dt.Columns.Add("Date/Hour", typeof(String));
            string date0 = jsonData.data.Keys.ElementAt(0).ToString();
            if (jsonData.data[date0].Count == 1)
            {
                DataColumn dc = dt.Columns.Add(jsonData.metadata[locTitle[0]] + ", " + jsonData.metadata[locTitle[1]] + units, typeof(String));
            }
            else
            {
                for (int i = 0; i < jsonData.data[date0].Count; i++)
                {
                    string title = jsonData.metadata["timeseries_" + (i + 1)];
                    DataColumn dc = dt.Columns.Add(title + units, typeof(String));
                }
            }
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dr[0] = jsonData.data.Keys.ElementAt(i);
                string date = jsonData.data.Keys.ElementAt(i).ToString();
                for (int j = 0; j < jsonData.data[date].Count; j++)
                {
                    dr[j + 1] = jsonData.data[date][j];

                }
            }
        }

        private void chkbShapefile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbShapefile.Checked.Equals(true))
            {
                Stream myStream = null;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.Filter = "All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.InitialDirectory = System.Environment.GetEnvironmentVariable("HOMEPATH");

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            lblShapefile.Text = shapefilePath = Path.GetFullPath(openFileDialog1.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblERROR.Text = "ERROR: " + ex.Message;
                    }
                }
            }
            else
            {
                lblShapefile.Text = "shapefile path";
                shapefilePath = "";
            }
            if (chkbShapefile.Checked.Equals(true) && shapefilePath == "") { chkbShapefile.Checked = false; lblShapefile.Text = "shapefile path"; }
        }

        private void SaveData(out string errorMsg)
        {
            errorMsg = "";
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = @"C:\Users\dsmith\Documents\HMS Data\";
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.Title = "Save HMS Data";
                saveFileDialog1.DefaultExt = "json";
                saveFileDialog1.FileName = cmbDataSet.SelectedItem.ToString() + "_data_" + cmbSource.SelectedItem.ToString();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.jsonDataOLD = new List<HMSJSON.HMSJSON>();
                    string dataset = cmbDataSet.SelectedItem.ToString();
                    string source = cmbSource.SelectedItem.ToString();

                    for (int i = 0; i < ts.Count; i++)
                    {
                        this.jsonDataOLD.Add(new HMSJSON.HMSJSON());
                        string data = jsonDataOLD[i].GetJSONString(out errorMsg, this.ts[i].timeSeries, this.ts[i].newMetaData, ts[i].metaData, dataset, source, this.localtime, this.gmtOffset, this.ts[i].cellCoverage );

                        string fileName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_" + (i + 1) + Path.GetExtension(saveFileDialog1.FileName);
                        string filePath = Path.GetDirectoryName(saveFileDialog1.FileName) + @"\" + fileName;
                        FileStream fs = File.Create(filePath);
                        fs.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetByteCount(data));
                        fs.Close();
                    }
                    if (errorMsg.Contains("ERROR")) { return; }
                    lblERROR.Text = "Data successfully saved to: " + saveFileDialog1.FileName;
                }
            }
            catch
            {
                lblERROR.Text = "ERROR: Failed to saved data to file.";
                return;
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void SaveDataV2(out string errorMsg)
        {
            errorMsg = "";
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = @"C:\Users\dsmith\Documents\HMS Data\";
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.Title = "Save HMS Data";
                saveFileDialog1.DefaultExt = "json";
                saveFileDialog1.FileName = cmbDataSet.SelectedItem.ToString() + "_data_" + cmbSource.SelectedItem.ToString();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string dataset = cmbDataSet.SelectedItem.ToString();
                    string source = cmbSource.SelectedItem.ToString();
                    string fileName = Path.GetFullPath(saveFileDialog1.FileName);
                    string data = JsonConvert.SerializeObject(this.jsonData);
                    FileStream fs = File.Create(fileName);
                    fs.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetByteCount(data));
                    fs.Close();
                    if (errorMsg.Contains("ERROR")) { return; }
                    lblERROR.Text = "Data successfully saved to: " + saveFileDialog1.FileName;
                }
            }
            catch
            {
                lblERROR.Text = "ERROR: Failed to saved data to file.";
                return;
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            string errorMsg = "";
            SaveDataV2(out errorMsg);
        }

        private string ConvertToCSV(out string errorMsg, string data)
        {
            errorMsg = "";
            string convertedData = "";
            try
            {
                string[] lines = data.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    convertedData += line[0] + " " + line[1] + ", " + line[2] + ", " + line[3] + ", " + line[4] + ", " + line[5] + ", " + line[6] + ", " + line[7];
                }
            }
            catch
            {
                errorMsg = "ERROR: Failed to convert data to csv format.";
                return null;
            }

            return convertedData;
        }

        private void cmbDataSet_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbDataSet.SelectedItem.ToString().Contains("SoilMoisture")) { lblSoilMoisture.Visible = true; chklstbxDatasetOptions.Visible = true; }
            else { lblSoilMoisture.Visible = false; chklstbxDatasetOptions.Visible = false; }

            if (cmbDataSet.SelectedItem.ToString().Contains("Precipitation"))
            {
                cmbSource.Items.Add("Daymet");
            }
            else
            {
                if(cmbSource.Items.Contains("Daymet")) { cmbSource.Items.Remove("Daymet"); }
            }

        }

        private void cmbSource_SelectedValueChanged(object sender, EventArgs e)
        {
            
            chklstbxDatasetOptions.SelectedIndex = -1;
            if (cmbSource.SelectedItem.ToString().Contains("NLDAS"))
            {
                chklstbxDatasetOptions.Items.Clear();
                chklstbxDatasetOptions.Items.Add("0-10cm");
                chklstbxDatasetOptions.Items.Add("10-40cm");
                chklstbxDatasetOptions.Items.Add("40-100cm");
                chklstbxDatasetOptions.Items.Add("100-200cm");
                chklstbxDatasetOptions.Items.Add("0-100cm");
                chklstbxDatasetOptions.Items.Add("0-200cm");
            }
            else
            {
                chklstbxDatasetOptions.Items.Clear();
                chklstbxDatasetOptions.Items.Add("0-10cm");
                chklstbxDatasetOptions.Items.Add("10-40cm");
                chklstbxDatasetOptions.Items.Add("40-100cm");
                chklstbxDatasetOptions.Items.Add("0-100cm");
            }
            chklstbxDatasetOptions.SelectionMode = SelectionMode.MultiSimple;
        }

        private void lblERROR_TextChanged(object sender, EventArgs e)
        {
            lblERROR.Visible = true;
        }

        private void btnPrecipCompare_Click(object sender, EventArgs e)
        {
            PrecipitationCompare compare = new PrecipitationCompare();
            compare.Show();
        }
    }
}
