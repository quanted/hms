using AQUATOX.Animals;
using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using AQUATOX.Loadings;
using AQUATOX.Plants;
using AQUATOX.OrgMatter;
using AQUATOX.Volume;
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
        private bool isInternalSeg = false;
        public int RBReturn = -1;
        private bool GridChanged = false;
        private bool DetritusScreen = false; //special case where the four suspended and dissolved detritus inputs are governed by one input
        private string Displayname;
        readonly private System.Drawing.Graphics graphics;

        TimeSeriesOutput ATSO;
        // string taskID = null;

        private TimeSeriesInput TSI = new TimeSeriesInput()
        {            Source = "nwis",
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

        private int ScaleX(int x)
        {
            double ScaleX = graphics.DpiX / 96;
            return Convert.ToInt32(x * ScaleX);
        }

        private int ScaleY(int y)
        {
            double ScaleY = graphics.DpiY / 96;
            return Convert.ToInt32(y * ScaleY);
        }


        public LoadingsForm()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {

        }

        public bool EditSV(ref TStateVariable IncomingS, AQTSim AQS, bool isBoundary)
        {

            string backup = Newtonsoft.Json.JsonConvert.SerializeObject(IncomingS, AQTSim.AQTJSONSettings());

            SV = IncomingS;
            isInternalSeg = !isBoundary;
            UpdateScreen();

            if (ShowDialog() == DialogResult.Cancel)
            {
                Newtonsoft.Json.JsonConvert.PopulateObject(backup, IncomingS, AQTSim.AQTJSONSettings());
                return false;
            }
            else return true;

        }

        public void UpdateUnits()
        {
            SV.UpdateUnits();
            ICUnit.Text = SV.StateUnit;

            string loadunit = SV.LoadUnit(LTBox.SelectedIndex);
            ToxICUnitLabel.Text = loadunit;
            CLUnit.Text = loadunit;
            TSUnit.Text = loadunit;
        }


        public void UpdateScreen()
        {
            // Future, add button for frac avail, trophic matrix, biotransformation
            // Future add photoperiod edit for light inputs
            // Future manage Detritus Inputs
            // Future CO2 Equilibrium button
            // Future Toxicant Exposure Inputs

            int sel_load = 0;

            IgnoreLoadingsBox.Text = SV.IgnoreLabel();  // add unique labels for state variables in which the ignore flag has different meanings (e.g. light, pH)

            List<string> RBList = SV.GUIRadioButtons();

            Displayname = SV.PName;
            DetritusScreen = (SV.NState == AllVariables.DissRefrDetr);
            if (DetritusScreen) Displayname = "Suspended and Dissolved Detritus";

            RBPanel.Visible = false;
            if (RBList != null)  // handle dynamic radio button portion of screen depending on list sent from state variable
            {
                RBPanel.Visible = true;
                RBLabel.Text = Displayname + " Options";

                int RBChecked = SV.RadioButtonState();

                RB0.Text = RBList[0];
                if (RBChecked == 0) RB0.Checked = true;

                RB1.Text = RBList[1];
                if (RBChecked == 1) RB1.Checked = true;

                RB2.Visible = (RBList.Count > 2);
                if (RBList.Count > 2)  RB2.Text = RBList[2];
                if (RBChecked == 2) RB2.Checked = true;


                RB3.Visible = (RBList.Count > 3);
                if (RBList.Count > 3) RB3.Text = RBList[3];
                if (RBChecked == 3) RB3.Checked = true;
            }  //end radio button code
            else LTPanel.Top = ScaleX(13);  // move up LoadingType Panel to fill blank space

            ParameterButton.Visible = ((SV.IsPlant()) || (SV.IsAnimal()) || (SV.NState == AllVariables.H2OTox));
            ToxicityButton.Visible = (SV.NState == AllVariables.H2OTox);

            WarningLabel.Visible = false;
            if ((SV.NState == AllVariables.Ammonia) && (SV.AQTSeg.PSetup.AmmoniaIsDriving.Val))
            {
                WarningLabel.Visible = true;
                WarningLabel.Text = "Ammonia Selected as a Driving Variable in the Setup Window";
            }
            if (isInternalSeg)
            {
                WarningLabel.Visible = true;
                WarningLabel.Text = "Inflow Loadings not relevant.";
            }


            NotesEdit.Text = SV.LoadNotes1;
            NotesEdit2.Text = SV.LoadNotes2;

            SVNameLabel.Text = Displayname;
            
            if (DetritusScreen) ICEdit.Text = ((TDissRefrDetr)SV).InputRecord.InitCond.ToString("G9");  //special case diss&susp detr
              else ICEdit.Text = SV.InitialCond.ToString("G9");

            IgnoreLoadingsBox.Checked = SV.LoadsRec.Loadings.NoUserLoad;
            LoadingsPanel.Visible = !SV.LoadsRec.Loadings.NoUserLoad;

            if (!SV.LoadsRec.Loadings.NoUserLoad)
            {
                LTBox.DataSource = null;
                LTBox.Items.Clear();
                LTBox.DataSource = SV.LoadList();
                LTBox.SelectedIndex = sel_load;
                ShowGrid();
            }

            UpdateUnits();
        }

        private TLoadings LoadShown;
        private TToxics TT = null;

        public void ShowGrid()
        {
            GridChanged = false;

            if (LTBox.SelectedIndex < 1) LoadShown = SV.LoadsRec.Loadings;   // Set LoadShown
            else if (LTBox.SelectedIndex < 4) LoadShown = SV.LoadsRec.Alt_Loadings[LTBox.SelectedIndex - 1];

            ToxIC.Visible = false;
            ToxICLabel.Visible = false;
            ToxICUnitLabel.Visible = false;

            if (DetritusScreen) //special case suspended & dissolved detr -- quick implementation, move this logic to .NET later
            {   // pull-down list is "In Inflow Water", "Point Source", "Non-Point Source", "Dissolved/Particulate", "Labile/Refractory"  special case for detritus
                DetritalInputRecordType DIR = ((TDissRefrDetr)SV).InputRecord;
                switch (LTBox.SelectedIndex)
                {
                    case 0: LoadShown = DIR.Load.Loadings; break;
                    case 1: LoadShown = DIR.Load.Alt_Loadings[0]; break;
                    case 2: LoadShown = DIR.Load.Alt_Loadings[2]; break;
                    case 3: LoadShown = DIR.Percent_Part.Loadings; break;
                    case 4: LoadShown = DIR.Percent_Refr.Loadings; break;
                }

                if (LTBox.SelectedIndex > 2)
                {
                    ToxIC.Visible = true;
                    ToxICLabel.Visible = true;
                    ToxICUnitLabel.Visible = true;
                    if (LTBox.SelectedIndex ==3) ToxIC.Text = DIR.Percent_PartIC.ToString("G9");
                    else ToxIC.Text = DIR.Percent_RefrIC.ToString("G9");
                }
            }

            if (LTBox.SelectedIndex > SV.nontoxloadings-1)  //then toxicant selected
            {
                int chemint = 1 + LTBox.SelectedIndex - SV.nontoxloadings;
                T_SVType chemtype = T_SVType.OrgTox1;  chemtype--;  //set to enumerated variable before
                int currentchem = 0;
                TT = null;
                while (chemint > currentchem)
                {
                    chemtype++;
                    TT = SV.AQTSeg.GetStatePointer(SV.NState, chemtype, T_SVLayer.WaterCol) as TToxics;
                    if (TT != null) { currentchem++; }
                }
                LoadShown = TT.LoadsRec.Loadings;

                ToxIC.Visible = true;
                ToxICLabel.Visible = true;
                ToxICUnitLabel.Visible = true;
                ToxIC.Text = TT.InitialCond.ToString("G9");

            }

            UseConstRadio.Checked = LoadShown.UseConstant;  // Update interface based on "LoadShown"
            UseTimeSeriesRadio.Checked = !LoadShown.UseConstant;
            dataGridView1.Enabled = !LoadShown.UseConstant;
            ConstLoadBox.Enabled = LoadShown.UseConstant;
            CLUnit.Enabled = LoadShown.UseConstant;
            timeSeriesLabel.Enabled = !LoadShown.UseConstant;
            timeSeriesLabel.Text = "Time Series \"" + LTBox.Text + "\" " + CLUnit.Text;

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

            dataGridView1.DataSource = null;
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
                plantform.EditParams(ref PPS, "Plant Parameters", false, "PlantLib.JSON","PlantData");
                TP.ChangeData();
            }

            if (SV.IsAnimal())
            {
                TAnimal TA = SV as TAnimal;
                Param_Form animform = new Param_Form();
                AnimalRecord AIR = TA.PAnimalData;
                AIR.Setup();
                TParameter[] PPS = AIR.InputArray();
                animform.EditParams(ref PPS, "Animal Parameters", false, "AnimalLib.JSON", "AnimalDataScreen");
                TA.ChangeData();
            }

            if (SV.NState == AllVariables.H2OTox)
            {
                TToxics TC = SV as TToxics;
                Param_Form chemform = new Param_Form();
                ChemicalRecord CR = TC.ChemRec; CR.Setup();
                TParameter[] PPS = CR.InputArray();
                chemform.EditParams(ref PPS, "Chem Parameters", false, "ChemLib.JSON","ChemData");
            }

            SV.UpdateName();
            UpdateScreen();
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
            SV.LoadsRec.Loadings.Translate_ITimeSeriesInput(0,0.0);
            ShowGrid();
        }


        /// <summary>
        /// Submit POST request to HMS web API
        /// </summary>
        public void submitRequest()
        {
            string requestURL = "https://qedcloud.net/hms/rest/api/";
            //string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            // string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "hydrology";
            string dataset = "streamflow";

            var request = (HttpWebRequest) WebRequest.Create(requestURL + "" + component + "/" + dataset + "/");  //fixme deprecated
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

                    LoadShown.list.Clear();

                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    string[] csvRows;
                    try
                    {
                        csvRows = System.IO.File.ReadAllLines(filePath, Encoding.Default);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading CSV: " + ex.Message);
                        return;
                    }

                    int errcount = 0;

                    foreach (var row in csvRows)
                    {
                        var columns = row.Split(',');

                        string field1 = columns[0];
                        string field2 = columns[1];

                        try
                        {
                            LoadShown.list.Add(DateTime.Parse(field1), double.Parse(field2));
                        }
                        catch
                        {
                            errcount++;
                            if (errcount > 1)  // header line error is OK
                            {
                                MessageBox.Show("Unexpected format.  A two column input file with [DATETIME, LOADING] expected.", "Error",
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
                try 
                {
                    LoadShown.list.Add(row.Field<DateTime>(0), row.Field<double>(1));
                }
                catch
                { }

            }
            
        }

        private void ICEdit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (DetritusScreen) ((TDissRefrDetr)SV).InputRecord.InitCond = double.Parse(ICEdit.Text);  //special case diss&susp detr
                else  SV.InitialCond = double.Parse(ICEdit.Text);
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

        private void RB_Changed(object sender, EventArgs e)
        {
            if (RB0.Checked) RBReturn = 0;
            else if (RB1.Checked) RBReturn = 1;
            else if (RB2.Checked) RBReturn = 2;
            else RBReturn = 3;

            SV.SetVarFromRadioButton(RBReturn);
        }

        private void ToxIC_TextChanged(object sender, EventArgs e)
        {
            try   
            {
                if (TT == null)
                {
                    DetritalInputRecordType DIR = ((TDissRefrDetr)SV).InputRecord;
                    if (LTBox.SelectedIndex == 3) DIR.Percent_PartIC = double.Parse(ToxIC.Text);
                    else DIR.Percent_RefrIC = double.Parse(ToxIC.Text);
                }
                else
                {
                    TT.InitialCond = double.Parse(ToxIC.Text);
                }
                ToxIC.BackColor = Color.White;

            }
            catch
            {
                ToxIC.BackColor = Color.Yellow;
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            switch (SV.NState)
            {
                case AllVariables.WindLoading: AQTTestForm.OpenUrl("Wind"); break;
                case AllVariables.Light: AQTTestForm.OpenUrl("Light"); break;
                case AllVariables.pH: AQTTestForm.OpenUrl("pHScreen"); break;
                case AllVariables.Volume: AQTTestForm.OpenUrl("Volume"); break;
                case AllVariables.TSS: AQTTestForm.OpenUrl("TSS"); break;
                case AllVariables.Temperature: AQTTestForm.OpenUrl("Temperature"); break;
                case AllVariables.DissRefrDetr: AQTTestForm.OpenUrl("SuspDiss"); break;
                case AllVariables.SedmLabDetr:
                case AllVariables.SedmRefrDetr: AQTTestForm.OpenUrl("Default_Sediment"); break;
                // case AllVariables.H2OTox: AQTTestForm.OpenUrl("DissOrgToxLoadings"); break;
                case AllVariables.Nitrate:
                case AllVariables.Ammonia:
                case AllVariables.Phosphate: AQTTestForm.OpenUrl("NutrientLoadings"); break;
                default : AQTTestForm.OpenUrl("ICandLoadings"); break;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TToxics TC = SV as TToxics;
            DataTable AT = TC.ATR_Table();
            DataTable PT = TC.PTR_Table();

            ChemToxForm ctf = new ChemToxForm();
            if (ctf.ShowGrids(AT,PT, ref TC.Anim_Method, ref TC.Plant_Method))
                if (ctf.gridChange)
                {
                    TC.SetPTR(PT);
                    TC.SetATR(AT);
                }
        }
    }
}
