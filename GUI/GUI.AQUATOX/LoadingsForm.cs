using AQUATOX.Animals;
using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using AQUATOX.Loadings;
using AQUATOX.Plants;
using Globals;
using System;
using Data;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GUI.AQUATOX
{

    public partial class LoadingsForm : Form
    {
        public TStateVariable SV;
        private bool GridChanged = false;

        TimeSeriesOutput ATSO;
        string taskID = null;

        private TimeSeriesInput TSI = new TimeSeriesInput()
        {
            Source = "nwis",
            DateTimeSpan = new DateTimeSpan()
            {
                StartDate = new DateTime(2015, 01, 01),
                EndDate = new DateTime(2015, 12, 31),
                DateTimeFormat = "yyyy-MM-dd HH"
            },
            Geometry = new TimeSeriesGeometry()
            {
                GeometryMetadata = new Dictionary<string, string>()
                {
                }
            },
            DataValueFormat = "E3",
            TemporalResolution = "daily",
            Units = "metric",
            OutputFormat = "json"
        };

        public LoadingsForm()
        {
            InitializeComponent();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {

        }

        public bool EditSV(ref TStateVariable IncomingS, AQTSim AQS)
        {

            string backup = Newtonsoft.Json.JsonConvert.SerializeObject(IncomingS, AQS.AQTJSONSettings());

            SV = IncomingS;
            UpdateScreen();

            if (ShowDialog() == DialogResult.Cancel)
            {
                Newtonsoft.Json.JsonConvert.PopulateObject(backup, IncomingS, AQS.AQTJSONSettings());
                return false;
            }
            else return true;

        }

        public void UpdateUnits()
        {
            SV.UpdateUnits();
            ICUnit.Text = SV.StateUnit;

            string loadunit = SV.LoadUnit(LTBox.SelectedIndex);
            CLUnit.Text = loadunit;
            TSUnit.Text = loadunit;
        }


        public void UpdateScreen()
        {
            // Future, add button for frac avail, trophic matrix, biotransformation
            // Future add photoperiod edit for light inputs
            // Future add TSS TSSolids vs TSSediment
            // Future add LInk Inflow/outflow warnings?
            // Future manage Detritus Inputs
            // Future CO2 Equilibrium button
            // Future Exposure Inputs

            ParameterButton.Visible = ((SV.IsPlant()) || (SV.IsAnimal()) || (SV.NState == AllVariables.H2OTox));

            AmmoniaDriveLabel.Visible = (SV.NState == AllVariables.Ammonia) && (SV.AQTSeg.PSetup.AmmoniaIsDriving.Val);

            NotesEdit.Text = SV.LoadNotes1;
            NotesEdit2.Text = SV.LoadNotes2;

            SVNameLabel.Text = SV.PName;
            ICEdit.Text = SV.InitialCond.ToString("G9");

            IgnoreLoadingsBox.Checked = SV.LoadsRec.Loadings.NoUserLoad;
            LoadingsPanel.Visible = !SV.LoadsRec.Loadings.NoUserLoad;

            if (!SV.LoadsRec.Loadings.NoUserLoad)
            {
                LTBox.DataSource = null;
                LTBox.Items.Clear();
                LTBox.DataSource = SV.LoadList();
                LTBox.SelectedIndex = 0;
                ShowGrid();
            }

            UpdateUnits();

        }

        TLoadings LoadShown;

        public void ShowGrid()
        {

            GridChanged = false;
            if (LTBox.SelectedIndex < 1) LoadShown = SV.LoadsRec.Loadings;
            else LoadShown = SV.LoadsRec.Alt_Loadings[LTBox.SelectedIndex - 1];

            UseConstRadio.Checked = LoadShown.UseConstant;
            UseTimeSeriesRadio.Checked = !LoadShown.UseConstant;
            dataGridView1.Enabled = !LoadShown.UseConstant;
            ConstLoadBox.Enabled = LoadShown.UseConstant;
            CLUnit.Enabled = LoadShown.UseConstant;

            ConstLoadBox.Text = LoadShown.ConstLoad.ToString("G9");
            MultLoadBox.Text = LoadShown.MultLdg.ToString("G9");

            DataTable LoadTable = new DataTable("Loadings");

            DataColumn datecolumn = new DataColumn();
            datecolumn.Unique = true;
            datecolumn.ColumnName = "Date";
            datecolumn.DataType = System.Type.GetType("System.DateTime");
            LoadTable.Columns.Add(datecolumn);

            DataColumn loadcolumn = new DataColumn();
            loadcolumn.Unique = false;
            loadcolumn.ColumnName = "Loading";
            loadcolumn.DataType = System.Type.GetType("System.Double");
            LoadTable.Columns.Add(loadcolumn);

            for (int i = 0; i < LoadShown.list.Count; i++)
            {
                DataRow row = LoadTable.NewRow();
                row[0] = (LoadShown.list.Keys[i]);
                row[1] = (LoadShown.list.Values[i]);
                LoadTable.Rows.Add(row);
            }

            dataGridView1.DataSource = LoadTable;
            if (LoadShown.UseConstant) dataGridView1.ForeColor = Color.Gray;
            else dataGridView1.ForeColor = Color.Black;
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            this.Close();

        }


        private void ParameterButton_Click(object sender, EventArgs e)
        {
            if (SV.IsPlant())
            {
                TPlant TP = SV as TPlant;
                Param_Form plantform = new Param_Form();
                PlantRecord PIR = TP.PAlgalRec;
                PIR.Setup();
                TParameter[] PPS = PIR.InputArray();
                plantform.EditParams(ref PPS, "Plant Parameters", false);
            }

            if (SV.IsAnimal())
            {
                TAnimal TA = SV as TAnimal;
                Param_Form animform = new Param_Form();
                AnimalRecord AIR = TA.PAnimalData;
                AIR.Setup();
                TParameter[] PPS = AIR.InputArray();
                animform.EditParams(ref PPS, "Animal Parameters", false);
            }

            if (SV.NState == AllVariables.H2OTox)
            {
                TToxics TC = SV as TToxics;
                Param_Form chemform = new Param_Form();
                ChemicalRecord CR = TC.ChemRec; CR.Setup();
                TParameter[] PPS = CR.InputArray();
                chemform.EditParams(ref PPS, "Chem Parameters", false);
            }

        }

        private void ButtonPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void IgnoreLoadingsBox_CheckedChanged(object sender, EventArgs e)
        {
            SV.LoadsRec.Loadings.NoUserLoad = IgnoreLoadingsBox.Checked;
            LoadingsPanel.Visible = !SV.LoadsRec.Loadings.NoUserLoad;
            UpdateScreen();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();
            this.Close();
        }

        private void LTBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();
            UpdateUnits();
            ShowGrid();
        }


        private void HMS_Click(object sender, EventArgs e)
        {
            // WSStreamflowController sfc = new WSStreamflowController(null);  // unnecessary 


            // check to see if HMS loadings may be available for this AQUATOX time-series

            if (!(SV.NState == AllVariables.Volume))
            {

                MessageBox.Show("HMS Linkages are not available for this time series", "Information",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);
                return;
            }


            // query HMS for loadings options   -- build service in HMS, data type is fixed, what sources of nutrient loadings
            //    -- build POST request for each source, user supplied parameters?

            // web services  -- streamflow for catchment, COMIDs, 
            // gauges, endpoint for that NWES
            // metadata time-steps missing

            // outputs from other models  (HAWQS)
            // Implement and end point that provides potential API endpoints along with optional sources / parameters

            // GUI allowing user to select which HMS loadings to import, time-period, and whether to import the data or link to file/URL
            // option to link to alternative COMID or time-period



            // pull actual time-series into model or link to data

            // Make a request body
            //     Get sample code for posting request body
            //     Deron linked to HMS open documentation
            //     https://qed.epa.gov/hms/api_doc/#/WSStreamflow/post_api_hydrology_streamflow


            // string crl = "curl -X POST \"https://qed.epa.gov/hms/rest/api/hydrology/streamflow\" -H \"accept: */*\" -H \"Content-Type: application/json-patch+json\" -d \"{\\\"source\\\":\\\"nwis\\\",\\\"dateTimeSpan\\\":{\\\"startDate\\\":\\\"2015-01-01T00:00:00\\\",\\\"endDate\\\":\\\"2015-12-31T00:00:00\\\",\\\"dateTimeFormat\\\":\\\"yyyy-MM-dd HH\\\"},\\\"geometry\\\":{\\\"description\\\":null,\\\"comID\\\":0,\\\"hucID\\\":null,\\\"stationID\\\":null,\\\"point\\\":null,\\\"geometryMetadata\\\":{\\\"gaugestation\\\":\\\"02191300\\\"},\\\"timezone\\\":null},\\\"dataValueFormat\\\":\\\"E3\\\",\\\"temporalResolution\\\":\\\"hourly\\\",\\\"timeLocalized\\\":false,\\\"units\\\":\\\"metric\\\",\\\"outputFormat\\\":\\\"json\\\",\\\"baseURL\\\":null,\\\"inputTimeSeries\\\":null}\"";


            //    https://github.com/quanted/hms_api_samples/blob/master/csharp-sample/csharp-sample/HMSSample.cs
            //            line 150, 144
            //            var request = (HttpWebRequest)WebRequest.Create(this.dataURL + this.taskID);
            //            var response = (HttpWebResponse)request.GetResponse();
            //            Task<IActionResult> res;

            submitRequest();


            if (SV.LoadsRec.Loadings.ITSI == null)
            {
                TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                SV.LoadsRec.Loadings.ITSI = input;

            }

            SV.LoadsRec.Loadings.ITSI.InputTimeSeries.Add("input", ATSO);
            SV.LoadsRec.Loadings.Translate_ITimeSeriesInput();
            ShowGrid();
        }


        /// <summary>
        /// Submit POST request to HMS web API
        /// </summary>
        public void submitRequest()
        {
            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            // string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "hydrology";
            string dataset = "streamflow";

            var request = (HttpWebRequest)WebRequest.Create(requestURL + "" + component + "/" + dataset + "/");
            var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(TSI));  //StreamFlowInput previously initialized
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            string rstring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            ATSO = JsonConvert.DeserializeObject<TimeSeriesOutput>(rstring);

        }

        ///// <summary>
        ///// Start polling HMS data api, with a 5 second delay.
        ///// </summary>
        //private void getData()
        //{
        //    string dataURL = "https://qed.epacdx.net/hms/rest/api/v2/hms/data?job_id=";

        //    bool completed = false;
        //    completed = false;
        //    string result = null;

        //    while (!completed)
        //    {
        //        int delay = 5000;
        //        Thread.Sleep(delay);
        //        var request = (HttpWebRequest)WebRequest.Create(dataURL + taskID);
        //        var response = (HttpWebResponse)request.GetResponse();
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
        //        if (data["status"].Equals("SUCCESS"))
        //        {
        //            completed = true;
        //            result = data["data"] + Environment.NewLine;
        //            Console.WriteLine("Data successfully downloaded");
        //        }
        //        else if (data["status"].Equals("FAILURE"))
        //        {
        //            completed = true;
        //            Console.WriteLine("Error: Failed to complete task.");
        //        }
        //    }
        //}




        private void File_Import_Click(object sender, EventArgs e)
        {

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv files(*.csv)|*.csv|tab delimited txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    SV.LoadsRec.Loadings.list.Clear();

                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    string[] csvRows = System.IO.File.ReadAllLines(filePath, Encoding.Default);
                    int errcount = 0;

                    foreach (var row in csvRows)
                    {
                        var columns = row.Split(',');

                        string field1 = columns[0];
                        string field2 = columns[1];

                        try
                        {
                            SV.LoadsRec.Loadings.list.Add(DateTime.Parse(field1), double.Parse(field2));
                        }
                        catch
                        {
                            errcount++;
                            if (errcount > 1)  // header line error is OK
                            {
                                MessageBox.Show("Unexpected format.  A two column input file with [DATE, LOADING] expected.", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return;
                            }
                        }
                    }

                    ShowGrid();

                }

            }
        }

        private void UseConstRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (GridChanged) SaveGrid();
            LoadShown.UseConstant = UseConstRadio.Checked;
            ShowGrid();
        }

        private void SaveGrid()
        {
            DataTable LoadTable = dataGridView1.DataSource as DataTable;
            LoadShown.list.Clear();
            LoadShown.list.Capacity = LoadTable.Rows.Count;
            for (int i = 0; i < LoadTable.Rows.Count; i++)
            {
                DataRow row = LoadTable.Rows[i];
                LoadShown.list.Add(row.Field<DateTime>(0), row.Field<double>(1));
            }
            
        }

        private void ICEdit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SV.InitialCond = double.Parse(ICEdit.Text);
                ICEdit.BackColor = Color.White;
            }
            catch
            {
                ICEdit.BackColor = Color.Yellow;
            }
        }

            private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            {
                if ((e.Context & DataGridViewDataErrorContexts.Parsing) == DataGridViewDataErrorContexts.Parsing)
                {
                    MessageBox.Show("Wrong data type entered.");
                }

                if ((e.Exception) is ConstraintException)
                {
                    MessageBox.Show(e.Exception.Message);
                }

                e.ThrowException = false;

            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridChanged = true;
        }

        private void NotesEdit_TextChanged(object sender, EventArgs e)
        {
            SV.LoadNotes1 = NotesEdit.Text;
        }

        private void NotesEdit2_TextChanged(object sender, EventArgs e)
        {
            SV.LoadNotes2 = NotesEdit2.Text;
        }

        private void ConstLoadBox_TextChanged(object sender, EventArgs e)
        {

            {
                try
                {
                    LoadShown.ConstLoad = double.Parse(ConstLoadBox.Text);
                    ConstLoadBox.BackColor = Color.White;
                }
                catch
                {
                    ConstLoadBox.BackColor = Color.Yellow;
                }
            }
        }

        private void MultLoadBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                LoadShown.MultLdg = double.Parse(MultLoadBox.Text);
                MultLoadBox.BackColor = Color.White;
            }
            catch
            {
                MultLoadBox.BackColor = Color.Yellow;
            }
        }

    }
}
