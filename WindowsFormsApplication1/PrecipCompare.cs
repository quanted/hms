using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
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

    public partial class PrecipCompare : Form
    {

        private DataTable dt;
        private Dictionary<string, int> stationNames;
        private List<HMSTimeSeries.HMSTimeSeries> ts;
        private List<HMSJSON.HMSJSON> jsonData;

        private List<Station> stations { get; set; }
        private List<Station> stationsInState { get; set; }

        public PrecipCompare()
        {
            InitializeComponent();
            DefineDatasetDetails();
            string[] states = ConfigurationManager.AppSettings["states"].ToString().Split(',');
            cmbState.Items.AddRange(states);
            InitializeMetStations();
            dataGVCompare.Visible = false;
        }

        private void InitializeMetStations()
        {
            //dt = new DataTable();
            //string metPath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"bin\MetStations.csv");
            //StreamReader reader = new StreamReader(File.OpenRead(metPath));
            //string[] headers = reader.ReadLine().Split(',');
            //foreach (string header in headers)
            //{
            //    dt.Columns.Add(header);
            //}
            //while (!reader.EndOfStream)
            //{
            //    string[] rows = Regex.Split(reader.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            //    DataRow dr = dt.NewRow();
            //    for (int i = 0; i < headers.Length; i++)
            //    {
            //        dr[i] = rows[i];
            //    }
            //    dt.Rows.Add(dr);
            //}

            this.stations = new List<Station>();
            string metPath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"bin\stations.json");
            StreamReader reader = new StreamReader(metPath);
            string json = reader.ReadToEnd();
            this.stations = JsonConvert.DeserializeObject<List<Station>>(json);

        }

        //private void UpdateStations()
        //{
        //    DateTime endDate = Convert.ToDateTime(lblDaymetEndDateValue.Text);           //Earliest end date value
        //    DateTime startDate = Convert.ToDateTime(lblGLDASstartDateValue.Text);        //Latest end date value
        //    string state = cmbState.SelectedItem.ToString();
        //    stationNames = new Dictionary<string, int>();
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        if (dt.Rows[i].Field<string>("CONSTITUEN") == "PREC" &&
        //            dt.Rows[i].Field<string>("LOCATION").Contains(state) &&
        //            !stationNames.ContainsKey(dt.Rows[i].Field<string>("STANAM")) &&
        //            DateTime.Compare(Convert.ToDateTime(dt.Rows[i].Field<string>("ENDDATE")), startDate) > 0 &&
        //            Convert.ToDouble(dt.Rows[i].Field<string>("LATITUDE")) > 24.0 &&
        //            Convert.ToDouble(dt.Rows[i].Field<string>("LATITUDE")) < 52.0 &&
        //            Convert.ToDouble(dt.Rows[i].Field<string>("LONGITUDE")) < -63.0 &&
        //            Convert.ToDouble(dt.Rows[i].Field<string>("LONGITUDE")) > -125.0)
        //        {
        //            stationNames.Add(dt.Rows[i].Field<string>("STANAM"), i);
        //        }
        //    }

        //    cmbStation.Items.AddRange(stationNames.Keys.ToArray<string>());
        //}

        private void UpdateStations()
        {
            DateTime endDate = Convert.ToDateTime(lblDaymetEndDateValue.Text);           //Earliest end date value
            DateTime startDate = Convert.ToDateTime(lblGLDASstartDateValue.Text);        //Latest end date value
            string state = cmbState.SelectedItem.ToString();
            
            List <Station> stationsInState = stations.FindAll(x => x.name.Contains(" " + state + " ") &&
                                    DateTime.Compare(Convert.ToDateTime(x.maxdate), startDate) > 0 &&
                                    x.latitude > 24.0 && x.latitude < 52.0 &&
                                    x.longitude < -63.0 && x.longitude > -125.0);
            this.stationsInState = stationsInState.OrderBy(o => o.name).ToList();
            
            foreach(Station station in this.stationsInState)
            {
                cmbStation.Items.Add(station.name);
            }
        }

        private void cmbStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int index = stationNames[cmbStation.SelectedItem.ToString()];
            //lblLatValue.Text = dt.Rows[index].Field<string>("LATITUDE");
            //lblLongValue.Text = dt.Rows[index].Field<string>("LONGITUDE");
            //lblStartDateValue.Text = dt.Rows[index].Field<string>("STARTDATE");
            //lblEndDateValue.Text = dt.Rows[index].Field<string>("ENDDATE");
            //lblLocationIDValue.Text = dt.Rows[index].Field<string>("LOCATION");

            Station selectedStation = stationsInState.Find(x => x.name.Equals(cmbStation.SelectedItem.ToString()));
            lblLatValue.Text = selectedStation.latitude.ToString();
            lblLongValue.Text = selectedStation.longitude.ToString();
            lblStartDateValue.Text = selectedStation.mindate.ToString();
            lblEndDateValue.Text = selectedStation.maxdate.ToString();
            lblLocationIDValue.Text = selectedStation.id.ToString();
            MakeDetailsVisible(true);
            SetDates();
        }

        private void MakeDetailsVisible(bool value)
        {

            //Station details
            lblDetails.Visible = value;
            lblStation.Visible = value;
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

        private void cmbState_SelectionChangeCommitted(object sender, EventArgs e)
        {
            lblStation.Visible = true;
            cmbStation.SelectedIndex = -1;
            cmbStation.Items.Clear();
            UpdateStations();
            cmbStation.Visible = true;
            MakeDetailsVisible(false);
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
            lblErrorMsg.Text = "Retrieving data...";
            bttnSave.Visible = false;
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            dataGVCompare.DataSource = null;
            lblErrorMsg.Visible = true;
            string errorMsg = "";
            string latitude = lblLatValue.Text;
            string longitude = lblLongValue.Text;
            string startDate = txtBxStartDate.Text;
            string endDate = txtBxEndDate.Text;
            bool local = false;
            List<HMSTimeSeries.HMSTimeSeries> temp = new List<HMSTimeSeries.HMSTimeSeries>();
            ts = new List<HMSTimeSeries.HMSTimeSeries>();
            DataTable dt = new DataTable();

            dataGVCompare.Visible = true;

            HMSPrecipitation.Precipitation nldas = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "NLDAS", true, "");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            temp = nldas.GetDataSets(out errorMsg);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            PopulateTable(out errorMsg, dt, "NLDAS");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation gldas = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "GLDAS", true, "", ts[0].gmtOffset.ToString(), ts[0].tzName);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            temp = gldas.GetDataSets(out errorMsg);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            PopulateTable(out errorMsg, dt, "GLDAS");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation daymet = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, "Daymet", local, "");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            temp = daymet.GetDataSets(out errorMsg);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            PopulateTable(out errorMsg, dt, "Daymet");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }

            HMSPrecipitation.Precipitation ncdc = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, "NCDC", lblLocationIDValue.Text);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            temp = ncdc.GetDataSets(out errorMsg);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            ts.Add(temp[0]);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            PopulateTableWithNCDCData(out errorMsg, dt, ncdc.ts[0].timeSeriesDict);
            //PopulateTable(out errorMsg, dt, "NCDC");
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }

            dataGVCompare.DataSource = dt.DefaultView;
            lblErrorMsg.Text = "Data successfully retrieved.";
            System.Windows.Forms.Cursor.Current = Cursors.Default;
            bttnSave.Visible = true;
        }

        private void PopulateTable(out string errorMsg, DataTable dt, string source)
        {
            errorMsg = "";
            int index = ts.Count - 1;
            string[] dataArray = ts[index].timeSeries.Split('\n');
            string units = "(mm/day)";

            if (source.Contains("NLDAS"))
            {
                DataColumn date = dt.Columns.Add("Date", typeof(String));
                DataColumn dc = dt.Columns.Add(source + " " + units, typeof(String));
                double sum = 0.0;
                int step = 24;
                for (int i = 0; i < dataArray.Length - 1; i++)
                {
                    string[] line = dataArray[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    sum += Convert.ToDouble(line[2].Trim());
                    if (i % step == 0)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        dr[0] = line[0].Trim();
                        dr[1] = sum.ToString("E4");
                        sum = 0.0;
                    }
                }
            }
            else if (source.Contains("GLDAS"))
            {
                DataColumn dc = dt.Columns.Add(source + " " + units, typeof(String));
                double sum = 0.0;
                int step = 8;
                int counter = 0;
                for (int i = 0; i < dataArray.Length - 1; i++)
                {
                    string[] line = dataArray[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    sum += 3 * 3600 * Convert.ToDouble(line[2].Trim()); // Convert mm/sec to mm/hour and multiple for the 3 hour time interval
                    if (i % step == 0 && counter < dt.Rows.Count)
                    {
                        dt.Rows[counter][2] = sum.ToString("E4");
                        sum = 0.0;
                        counter += 1;
                    }
                }
            }
            else if (source.Contains("Daymet"))
            {
                DataColumn dc = dt.Columns.Add(source + " " + units, typeof(String));
                for (int i = 0; i < dataArray.Length - 1; i++)
                {
                    if (i < dt.Rows.Count)
                    {
                        string[] line = dataArray[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        double value = Convert.ToDouble(line[2].Trim());
                        dt.Rows[i][3] = value.ToString("E4");
                    }
                }
            }

        }

        private void PopulateTableWithNCDCData(out string errorMsg, DataTable dt, Dictionary<DateTime, double> data)
        {
            errorMsg = "";

            DataColumn dc = dt.Columns.Add("NCDC (mm)", typeof(String));
            for (int i = 0; i < data.Count - 1; i++)
            {
                if (i < dt.Rows.Count)
                {
                    dt.Rows[i][4] = data[Convert.ToDateTime(dt.Rows[i][0].ToString())].ToString("E4");
                }
            }
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
                saveFileDialog1.FileName = "precip_data";
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.jsonData = new List<HMSJSON.HMSJSON>();
                    string source = "NLDAS";

                    for (int i = 0; i < ts.Count; i++)
                    {
                        if (i == 1) { source = "GLDAS"; }
                        else if(i == 2) { source = "Daymet"; }
                        this.jsonData.Add(new HMSJSON.HMSJSON());
                        string data = jsonData[i].GetJSONString(out errorMsg, this.ts[i].timeSeries, this.ts[i].newMetaData, ts[i].metaData, "Precipitation", source, false, 0.0);

                        string fileName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + "_" + source + Path.GetExtension(saveFileDialog1.FileName);
                        string filePath = Path.GetDirectoryName(saveFileDialog1.FileName) + @"\" + fileName;
                        FileStream fs = File.Create(filePath);
                        fs.Write(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetByteCount(data));
                        fs.Close();
                    }
                    if (errorMsg.Contains("Error")) { return; }
                    lblErrorMsg.Text = "Data successfully saved to: " + saveFileDialog1.FileName;
                }
            }
            catch
            {
                lblErrorMsg.Text = "Error: Failed to saved data to file.";
                return;
            }
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void bttnSave_Click(object sender, EventArgs e)
        {
            string errorMsg = "";
            SaveData(out errorMsg);
            if (errorMsg.Contains("Error")) { lblErrorMsg.Text = errorMsg; return; }
            lblErrorMsg.Text = "Data successfully saved.";
        }
    }
}
