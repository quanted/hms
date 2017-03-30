using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class PrecipitationCompare : Form
    {
        public PrecipitationCompare()
        {
            InitializeComponent();
            DefineDatasetDetails();
            string[] states = ConfigurationManager.AppSettings["states"].ToString().Split(',');
            cmbState.Items.AddRange(states);
            InitializeMetStations();
            dataGVCompare.Visible = false;
        }

        private List<HMSTimeSeries.HMSTimeSeries> ts;
        private List<HMSJSON.HMSJSON.HMSData> jsonDataList;
        private List<Station> stations { get; set; }
        private List<Station> stationsInState { get; set; }
        public string dataFrequency { get; set; }

        private void InitializeMetStations()
        {
            this.stations = new List<Station>();
            string metPath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"bin\stations.json");
            StreamReader reader = new StreamReader(metPath);
            string json = reader.ReadToEnd();
            this.stations = JsonConvert.DeserializeObject<List<Station>>(json);
        }

        private void UpdateStations()
        {
            DateTime endDate = Convert.ToDateTime(lblDaymetEndDateValue.Text);           //Earliest end date value
            DateTime startDate = Convert.ToDateTime(lblGLDASstartDateValue.Text);        //Latest end date value
            string state = cmbState.SelectedItem.ToString();

            List<Station> stationsInState = stations.FindAll(x => x.name.Contains(" " + state + " ") &&
                                   DateTime.Compare(Convert.ToDateTime(x.maxdate), startDate) > 0 &&
                                   x.latitude > 24.0 && x.latitude < 52.0 &&
                                   x.longitude < -63.0 && x.longitude > -125.0);
            this.stationsInState = stationsInState.OrderBy(o => o.name).ToList();

            foreach (Station station in this.stationsInState)
            {
                cmbStation.Items.Add(station.name);
            }

        }

        private void MakeDetailsVisible(bool value)
        {
            //Station details
            lblDetails.Visible = value;
            //lblStation.Visible = value;
            lblLat.Visible = value;
            lblLatValue.Visible = value;
            lblLong.Visible = value;
            lblLongValue.Visible = value;
            lblStartDate.Visible = value;
            lblStartDateValue.Visible = value;
            lblEndDate.Visible = value;
            lblEndDateValue.Visible = value;
            lblLocationID.Visible = value;
            lblLocationIDValue.Visible = value;

            //NLDAS details
            lblNLDAS.Visible = value;
            lblNLDASstartDate.Visible = value;
            lblNLDASstartDateValue.Visible = value;
            lblNLDASendDate.Visible = value;
            lblNLDASendDateValue.Visible = value;

            //GLDAS details
            lblGLDAS.Visible = value;
            lblGLDASstartDate.Visible = value;
            lblGLDASstartDateValue.Visible = value;
            lblGLDASendDate.Visible = value;
            lblGLDASendDateValue.Visible = value;

            //Daymet details
            lblDaymet.Visible = value;
            lblDaymetStartDate.Visible = value;
            lblDaymetStartDateValue.Visible = value;
            lblDaymetEndDate.Visible = value;
            lblDaymetEndDateValue.Visible = value;

            //Dates
            lblStartDateFinal.Visible = value;
            lblEndDateFinal.Visible = value;
            txtBxEndDate.Visible = value;
            txtBxStartDate.Visible = value;
            lblDateRange.Visible = value;

            bttnCompareData.Visible = value;

        }

        private void DefineDatasetDetails()
        {
            lblNLDASstartDateValue.Text = "01/02/1979";
            lblNLDASendDateValue.Text = DateTime.Now.AddDays(-7.0).ToString("MM/dd/yyyy");
            lblGLDASstartDateValue.Text = "02/24/2000";
            lblGLDASendDateValue.Text = DateTime.Now.AddDays(-25.0).ToString("MM/dd/yyyy");
            lblDaymetStartDateValue.Text = "01/01/1980";
            lblDaymetEndDateValue.Text = "12/31/2015"; //DateTime.Now.AddYears(-1).ToString("MM/dd/yyyy");  //Daymet only currently goes till 2015
        }

        private void SetDates()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DateTime.TryParse(lblStartDateValue.Text, out startDate);
            DateTime.TryParse(lblEndDateValue.Text, out endDate);
            startDate = CompareDates(startDate, lblNLDASstartDateValue.Text, true);
            startDate = CompareDates(startDate, lblGLDASstartDateValue.Text, true);
            startDate = CompareDates(startDate, lblDaymetStartDateValue.Text, true);
            endDate = CompareDates(endDate, lblNLDASendDateValue.Text, false);
            endDate = CompareDates(endDate, lblGLDASendDateValue.Text, false);
            endDate = CompareDates(endDate, lblDaymetEndDateValue.Text, false);
            txtBxStartDate.Text = startDate.ToString("MM/dd/yyyy");
            txtBxEndDate.Text = endDate.ToString("MM/dd/yyyy");

        }

        private DateTime CompareDates(DateTime firstDate, string secondDate, bool start)
        {
            DateTime tempDate = new DateTime();
            DateTime.TryParse(secondDate, out tempDate);
            if (start == true)
            {
                if (DateTime.Compare(firstDate, tempDate) < 0)
                {
                    return tempDate;
                }
            }
            else
            {
                if (DateTime.Compare(firstDate, tempDate) > 0)
                {
                    return tempDate;
                }
            }
            return firstDate;
        }

        private void bttnCompareData_Click(object sender, EventArgs e)
        {
            lblerrorMsg.Text = "Retrieving data...";
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            dataGVCompare.DataSource = null;
            lblerrorMsg.Visible = true;
            bttnSave.Visible = false;
            string errorMsg = "";
            string latitude = lblLatValue.Text;
            string longitude = lblLongValue.Text;
            string startDate = txtBxStartDate.Text;
            string endDate = txtBxEndDate.Text;
            bool local = false;
            List<HMSTimeSeries.HMSTimeSeries> temp = new List<HMSTimeSeries.HMSTimeSeries>();
            this.ts = new List<HMSTimeSeries.HMSTimeSeries>();
            this.jsonDataList = new List<HMSJSON.HMSJSON.HMSData>();
            DataTable dt = new DataTable();

            dataGVCompare.Visible = true;

            HMSPrecipitation.Precipitation nldas = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "NLDAS", true, "");
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            temp = nldas.GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            jsonDataList.Add(nldas.jsonData);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation gldas = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "GLDAS", true, "", ts[0].gmtOffset.ToString(), ts[0].tzName);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            temp = gldas.GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            jsonDataList.Add(gldas.jsonData);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation daymet = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "Daymet", local, "");
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            temp = daymet.GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            jsonDataList.Add(daymet.jsonData);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation ncdc = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, "NCDC", lblLocationIDValue.Text);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            temp = ncdc.GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            jsonDataList.Add(ncdc.jsonData);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }

            PopulateTable(out errorMsg, dt);
            if (errorMsg.Contains("ERROR")) { lblerrorMsg.Text = errorMsg; return; }

            dataGVCompare.DataSource = dt.DefaultView;
            lblerrorMsg.Text = "Data successfully retrieved.";
            bttnSave.Visible = true;
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void PopulateTable(out string errorMsg, DataTable dt)
        {
            errorMsg = "";
            int index = jsonDataList.Count;

            DataColumn date = dt.Columns.Add("Date", typeof(String));
            string date0 = jsonDataList[0].data.Keys.ElementAt(0).ToString().Substring(0,10);
            HMSJSON.HMSJSON json = new HMSJSON.HMSJSON();

            for (int i = 0; i < jsonDataList.Count; i++)
            {
                HMSJSON.HMSJSON.HMSData altered = new HMSJSON.HMSJSON.HMSData();
                altered = json.CollectDataTotals(out errorMsg, this.jsonDataList[i], "daily");
                string locTitle = altered.source;
                DataColumn dc = dt.Columns.Add(locTitle, typeof(String));
                int iMax = 0;
                if (i == 0) { iMax = altered.data.Count; }
                else { iMax = dt.Rows.Count; }
                for (int j = 0; j < iMax; j++)
                {                    
                    if (i == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        dt.Rows[j][0] = altered.data.Keys.ElementAt(j);
                        string dateString = altered.data.Keys.ElementAt(j).ToString();
                        dt.Rows[j][1] = altered.data[dateString][0];
                    }
                    else if (j < altered.data.Keys.Count)
                    {
                        DataRow dr = dt.Rows[i + 1];
                        string dateString = altered.data.Keys.ElementAt(j).ToString();
                        dt.Rows[j][i+1] = altered.data[dateString][0];
                    }
                }
            }
        }

        private void cmbState_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbState.SelectedItem != null)
            {
                lblStation.Visible = true;
                cmbStation.SelectedIndex = -1;
                cmbStation.Items.Clear();
                UpdateStations();
                cmbStation.Visible = true;
                bttnSave.Visible = false;
                MakeDetailsVisible(false);
            }
        }

        private void cmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Station selectedStation = stationsInState.Find(x => x.name.Equals(cmbStation.SelectedItem.ToString()));
            lblLatValue.Text = selectedStation.latitude.ToString();
            lblLongValue.Text = selectedStation.longitude.ToString();
            lblStartDateValue.Text = selectedStation.mindate.ToString();
            lblEndDateValue.Text = selectedStation.maxdate.ToString();
            lblLocationIDValue.Text = selectedStation.id.ToString();
            txtBxStationID.Text = selectedStation.id.ToString();
            MakeDetailsVisible(true);
            bttnSave.Visible = false;
            SetDates();
        }

        private void bttnSave_Click(object sender, EventArgs e)
        {
            string errorMsg = "";
            DataFrequency df = new DataFrequency();
            df.ShowDialog();
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            this.dataFrequency = df.dataFrequency;
            HMSJSON.HMSJSON json = new HMSJSON.HMSJSON();
            lblerrorMsg.Text = "Saving " + jsonDataList[0].dataset + " data.";         
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); ;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Save " + jsonDataList[0].dataset + " Comparison Data";
                saveFileDialog.DefaultExt = "json";
                saveFileDialog.FileName = jsonDataList[0].dataset;
                saveFileDialog.Filter = "All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 2;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < jsonDataList.Count; i++)
                    {
                        HMSJSON.HMSJSON.HMSData altered = new HMSJSON.HMSJSON.HMSData();
                        altered = json.CollectDataTotals(out errorMsg, this.jsonDataList[i], this.dataFrequency);

                        string fileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName) + "_" + jsonDataList[i].source + "_" + this.dataFrequency + Path.GetExtension(saveFileDialog.FileName);
                        string filePath = Path.GetFullPath(saveFileDialog.FileName).Substring(0, Path.GetFullPath(saveFileDialog.FileName).LastIndexOf(Path.GetFileNameWithoutExtension(saveFileDialog.FileName))) + fileName;
                        string data = JsonConvert.SerializeObject(altered);
                        FileStream fs = File.Create(filePath);
                        fs.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));
                        fs.Close();
                    }
                    lblerrorMsg.Text = "Successfully saved " + jsonDataList[0].dataset + " data.";
                }
            }
            catch
            {
                lblerrorMsg.Text = "ERROR: " + jsonDataList[0].dataset + " data failed to save.";
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void txtBxStationID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lblerrorMsg.Visible = false;
                DateTime endDate = Convert.ToDateTime(lblDaymetEndDateValue.Text);           //Earliest end date value
                DateTime startDate = Convert.ToDateTime(lblGLDASstartDateValue.Text);        //Latest end date value
                List<Station> stationsInState = stations.FindAll(x => x.id.Contains(txtBxStationID.Text.Trim()));
                Station selectedStation = stationsInState.Find(x => x.id.Equals(txtBxStationID.Text.ToString()));
                lblLatValue.Text = selectedStation.latitude.ToString();
                lblLongValue.Text = selectedStation.longitude.ToString();
                lblStartDateValue.Text = selectedStation.mindate.ToString();
                lblEndDateValue.Text = selectedStation.maxdate.ToString();
                lblLocationIDValue.Text = selectedStation.id.ToString();
                MakeDetailsVisible(true);
                cmbStation.Visible = true;
                cmbStation.SelectedText = selectedStation.name.ToString();
                bttnSave.Visible = false;
                SetDates();
            }
            catch { lblerrorMsg.Text = "Station not found."; lblerrorMsg.Visible = true; }
        }
    }


    public class Station
    {
        public string name;
        public double latitude;
        public double longitude;
        public double datacoverage;
        public string maxdate;
        public string mindate;
        public string id;
        public double elevation;
        public string elevationUnit;
    }   
}
