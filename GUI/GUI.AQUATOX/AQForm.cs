using AQUATOX.Animals;
using AQUATOX.AQSite;
using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using AQUATOX.Diagenesis;
using AQUATOX.Plants;
using Globals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Linq.Expressions;

//TODO Fix issue of multiple url_info.txt on build overwriting each other.  Currently ignored in csproj using <ErrorOnDuplicatePublishOutputFiles>false</ErrorOnDuplicatePublishOutputFiles> 

namespace GUI.AQUATOX
{


    public partial class AQTTestForm : Form
    {
        private BackgroundWorker Worker = new BackgroundWorker();
        private BackgroundWorker ParAnnealWorker = new BackgroundWorker();
        private string errmessage;
        static private string defaultBrowser;

        private bool MultiSegmentInput = false;
        private bool isBoundarySegment = true;
        public AQTSim aQTS = null;
        private List<string> SVList = null;
        private List<TStateVariable> TSVList = null;
        private System.Drawing.Graphics graphics;


        public AQTTestForm()
        {
            InitializeComponent();

            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunCompleted);
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;

            ParAnnealWorker.DoWork += new DoWorkEventHandler(ParAnnealWorker_DoWork);
            ParAnnealWorker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            ParAnnealWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AnnealWorker_RunCompleted);
            ParAnnealWorker.WorkerReportsProgress = true;
            ParAnnealWorker.WorkerSupportsCancellation = true;

            graphics = this.CreateGraphics();
            defaultBrowser = Properties.Settings.Default.BrowserExe;
        }

        private void AQTTestForm_Load(object sender, EventArgs e)
        {

        }


        //private string TEST_INSERT_LOAD(string json)
        //{
        //    AQTSim AQS = new AQTSim();

        //    json= AQS.InsertLoadings(json, "TN", 2, 56.56, 1.56);  //2 = non-point source loading

        //    SortedList<DateTime, double> TSL = new SortedList<DateTime, double>() { { new DateTime(2004, 1, 1), 2004.11 }, { new DateTime(2004, 5, 5), 2004.55 } };
        //    json = AQS.InsertLoadings(json, "TP", -1, TSL,1.2004);  //-1 = inflow loading

        //    json = AQS.InsertLoadings(json, "TP", 0, 123456,1.23456);   //0 = point source loading

        //    json = AQS.InsertLoadings(json, "TDissRefrDetr", 0, 3.1415926,1.314); //0 = point source loading; TDissRefrDetr=Organic Matter

        //    TSL = new SortedList<DateTime, double>() { { new DateTime(2009, 2, 2), 2009.22 }, { new DateTime(2012, 3, 3), 2012.33 } };
        //    json = AQS.InsertLoadings(json, "TDissRefrDetr", 2, TSL,1.2009);  //2 = non-point source loading; TDissRefrDetr=Organic Matter 

        //    return json;
        //}

        private bool LoadJSON(string json)
        {
            AQTSim Sim = new AQTSim();
            string err = Sim.Instantiate(json);
            if (err != "") { MessageBox.Show(err); return false; }

            aQTS = Sim;
            aQTS.AQTSeg.SetMemLocRec();
            return true;
        }

        private void loadJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                string json = File.ReadAllText(openFileDialog1.FileName);

                // json = TEST_INSERT_LOAD(json);  // temporary used to test insert load code

                if (!LoadJSON(json)) return;
                aQTS.AQTSeg.FileName = openFileDialog1.FileName;

                ButtonPanel.Visible = true;
                AnnealPanel.Visible = true;
                AddButton.Visible = true;
                DeleteButton.Visible = true;
                EditButton.Visible = true;
                integrate.Visible = true;

                aQTS.ArchiveSimulation();

                ShowStudyInfo();


            }
        }

        private void saveJSON_Click(object sender, EventArgs e)
        {
            if (aQTS == null) return;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JSON File|*.JSON";
            saveFileDialog1.Title = "Save to JSON File";
            saveFileDialog1.FileName = aQTS.AQTSeg.FileName;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                string jsondata = "";
                string errmessage = aQTS.SaveJSON(ref jsondata);
                if (errmessage == "")
                {
                    aQTS.AQTSeg.FileName = saveFileDialog1.FileName;
                    File.WriteAllText(saveFileDialog1.FileName, jsondata);
                    ShowStudyInfo();
                }
                else MessageBox.Show(errmessage);
            }
        }

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


        private DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(ScaleX(400), ScaleY(70));

            Form inputBox = new Form();
            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Enter an ID for this simulation (optional)";

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - ScaleX(40), ScaleY(23));
            textBox.Location = new System.Drawing.Point(ScaleX(25), ScaleY(5));
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(ScaleX(75), ScaleY(23));
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - ScaleX(160), ScaleY(39));
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(ScaleX(75), ScaleY(23));
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - ScaleX(80), ScaleY(39));
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private void Set_Buttons(bool Running)
        {
            modelRunningLabel.Visible = Running;

            foreach (Control c in Controls)  // disable all buttons
            {
                if (c != AnnealPanel)
                  c.Enabled = !Running;
            }

            modelRunningLabel.Enabled = Running;
            CancelButton.Enabled = Running;
            CancelButton.Visible = Running;
            progressBar1.Enabled = Running;
            progressBar1.Visible = Running;
        }


        private void integrate_Click(object sender, EventArgs e)
        {
            string SimName = "";


            if (aQTS == null) { MessageBox.Show("No Simulation Loaded"); return; }
            else
            {
                //if (aQTS.Has_Chemicals())
                //{
                //    AQTCM = new(AQTChemicalModel(aQTS));
                //    string errmsg = AQTCM CheckDataRequirements();
                //    if (errmsg != "") textBox1.Text = errmsg;
                //}

                modelRunningLabel.Text = "Model is Running";
                if (ShowInputDialog(ref SimName) == DialogResult.Cancel) return;
                if (SimName != "") SimName = SimName + ": ";
                aQTS.AQTSeg.RunID = SimName + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                Set_Buttons(true);

                Worker.RunWorkerAsync();
            }
        }



        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            aQTS.AQTSeg.ProgWorker = worker;
            errmessage = aQTS.Integrate();
        }

        private void Worker_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Update();
            if (e.Error != null) MessageBox.Show("Error Raised: " + e.Error.Message);
            if ((!e.Cancelled) && (errmessage == ""))
            {
                Application.DoEvents();
                graph_Click(null, null);
                ShowStudyInfo();
            };

            if ((errmessage != "") && (errmessage != "User Canceled")) MessageBox.Show(errmessage);

            Set_Buttons(false);

            ShowStudyInfo();

        }


        // This event handler updates the progress bar
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100) progressBar1.Value = (e.ProgressPercentage + 1);  // workaround of animation bug
            progressBar1.Value = Math.Max(e.ProgressPercentage, 1);    //always show a little bit of green in progress bar

            if (e.UserState != null) modelRunningLabel.Text = e.UserState.ToString();
        }


        private void graph_Click(object sender, EventArgs e)
        {
            if (aQTS == null) { MessageBox.Show("No Simulation Loaded"); return; }
            if (aQTS.AQTSeg.SimulationDate > DateTime.MinValue)
                aQTS.ArchiveSimulation();
            else { if (!aQTS.HasResults()) { MessageBox.Show("No Results Available"); return; } }

            //textBox1.Text = "Please wait one moment -- writing and plotting results";
            Application.DoEvents();

            OutputForm OutForm = new OutputForm();
            OutForm.ShowOutput(aQTS);
            ShowStudyInfo();
        }



        private void Setup_Click(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            Setup_Record SR = aQTS.AQTSeg.PSetup;
            SR.Setup(false);
            TParameter[] SS = new TParameter[] {new TSubheading ("Timestep Settings",""), SR.FirstDay,SR.LastDay,SR.RelativeError,SR.UseFixStepSize,SR.FixStepSize,
                SR.ModelTSDays, new TSubheading("Output Storage Options",""), SR.StepSizeInDays, SR.StoreStepSize, SR.AverageOutput, SR.SaveBRates,
                new TSubheading("Biota Modeling Options",""),SR.Internal_Nutrients,SR.NFix_UseRatio,SR.NtoPRatio,
                new TSubheading("Chemical Options",""),SR.ChemsDrivingVars,SR.TSedDetrIsDriving,SR.UseExternalConcs,SR.T1IsAggregate };

            Param_Form SetupForm = new Param_Form();
            SetupForm.SuppressComment = true;
            SetupForm.SuppressSymbol = true;
            SetupForm.EditParams(ref SS, "Simulation Setup", true, "", "SetupWindow");
            ShowStudyInfo();
        }

        private void Diagensis(object sender, EventArgs e)
        {
            if (aQTS == null) return;
            Diagenesis_Rec DR = aQTS.AQTSeg.Diagenesis_Params;
            DR.Setup(false);

            // Diagenesis Parameters setup window
            TParameter[] PA3 = new TParameter[] { new TSubheading("Diagenesis Parameters",""), DR.m1, DR.m2,DR.H1,DR.Dd,DR.w2,DR.H2,DR.KappaNH3f,DR.KappaNH3s,DR.KappaNO3_1f,DR.KappaNO3_1s,DR.KappaNO3_2,
                DR.KappaCH4,DR.KM_NH3,DR.KM_O2_NH3,DR.KdNH3,DR.KdPO42,DR.dKDPO41f,DR.dKDPO41s,DR.O2critPO4,
                DR.ThtaDd,DR.ThtaNH3,DR.ThtaNO3,DR.ThtaCH4,DR.SALTSW,DR.SALTND,DR.KappaH2Sd1,DR.KappaH2Sp1,DR.ThtaH2S,DR.KMHSO2,DR.KdH2S1,
                DR.KdH2S2,new TSubheading("Mineralization","Decay rates for organic matter"),DR.kpon1,DR.kpon2, DR.kpon3,DR.kpoc1,DR.kpoc2,DR.kpoc3,DR.kpop1,DR.kpop2,DR.kpop3,DR.ThtaPON1,DR.ThtaPON2,
                DR.ThtaPON3,DR.ThtaPOC1,DR.ThtaPOC2,DR.ThtaPOC3, DR.ThtaPOP1,DR.ThtaPOP2,DR.ThtaPOP3,DR.kBEN_STR,new TSubheading("Silica Parameters",""),DR.ksi,DR.ThtaSi,DR.KMPSi,
                DR.SiSat,DR.KDSi2,DR.DKDSi1,DR.O2critSi,DR.LigninDetr, DR.Si_Diatom   };

            Param_Form PF3 = new Param_Form();
            PF3.EditParams(ref PA3, "Diagenesis Parameters", false, "", "Remineralization");
        }

        private void Plants(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            List<PlantRecord> PlantDB = new List<PlantRecord>();

            int nplt = 0;
            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                if (TSV.IsPlant())
                {
                    TPlant TP = TSV as TPlant;
                    PlantDB.Add(TP.PAlgalRec);
                    nplt++;
                }

            if (nplt == 0)
            {
                MessageBox.Show("There are no plants in the current simulation");
                return;
            }

            PlantRecord PR = PlantDB[0]; PR.Setup();
            TParameter[] PPS = PR.InputArray();

            DataTable table = ParmArray_to_Table("PlantGrid", PPS);
            for (int r = 0; r < PlantDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, PlantDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, false, "PlantData"))
            {
                if (gf.gridChange)
                {
                    List<PlantRecord> PlantDB2 = Table_to_PlantDB(table);
                    nplt = 0;
                    foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                        if (TSV.IsPlant())
                        {
                            TPlant TP = TSV as TPlant;
                            TP.PAlgalRec = PlantDB2[nplt];
                            TP.ChangeData();
                            TP.UpdateName();
                            nplt++;
                        }
                    ShowStudyInfo();
                }
            }
        }

        private void ShowStudyInfo()
        {
            if (aQTS == null) return;

            saveJSON.Enabled = true;
            pictureBox1.Visible = false;
            Refresh();
            Application.DoEvents();

            this.Text = aQTS.AQTSeg.FileName;
            StudyNameBox.Text = aQTS.AQTSeg.StudyName;
            outputbutton.Visible = aQTS.HasResults();
            Diagenesis.Enabled = aQTS.AQTSeg.Diagenesis_Included();
            ChemButton.Enabled = aQTS.AQTSeg.Has_Chemicals();

            DeleteResultsButton.Visible = false;
            if (MultiSegmentInput) RunStatusLabel.Visible = false;
            else
            {
                if (!aQTS.HasResults()) RunStatusLabel.Text = "No Saved Runs";
                else
                {
                    RunStatusLabel.Text = aQTS.SavedRuns.Count + " Archived Results";
                    DeleteResultsButton.Visible = true;
                }

            }

            aQTS.AQTSeg.DisplayNames(ref SVList, ref TSVList);

            ParametersLabel.Visible = true;
            StateVarLabel.Visible = true;
            SVListBox.Visible = true;
            SetupButton.Visible = (!MultiSegmentInput);
            SVListBox.DataSource = null;
            SVListBox.DataSource = SVList;

            Application.DoEvents();
        }

        private void Chems(object sender, EventArgs e)
        {
            void EditChemical(int chemindex)
            {
                Param_Form Chemform = new Param_Form();
                TStateVariable TSV = aQTS.AQTSeg.SV[chemindex];
                TToxics TC = TSV as TToxics;
                Chemform = new Param_Form();
                ChemicalRecord CR = TC.ChemRec; CR.Setup();
                TParameter[] PPS = CR.InputArray();
                Chemform.EditParams(ref PPS, "Chem Parameters", false, "ChemLib.JSON", "ChemData");
                TC.UpdateName();
            }
            // ------------------------------------------------------------------

            List<string> ChemNames = new List<string>();
            int[] cindices = new int[20];

            if (aQTS == null) return;

            int nchm = 0;
            for (int j = 0; j < aQTS.AQTSeg.SV.Count; j++)
            {
                TStateVariable TSV = aQTS.AQTSeg.SV[j];
                if (TSV.NState == AllVariables.H2OTox)
                {
                    ChemNames.Add(TSV.PName);
                    cindices[nchm] = j;
                    nchm++;
                }
            }

            if (nchm == 0)
            {
                MessageBox.Show("There are no chemicals in the current simulation");
                return;
            }

            if (nchm == 1) EditChemical(cindices[0]);
            else
            {
                ListForm LF = new ListForm();
                int index = LF.SelectFromList(ChemNames);
                if (index >= 0) EditChemical(index);
            }

            ShowStudyInfo();
        }

        private void Sites(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            SiteForm SF = new SiteForm();
            SF.EditSiteInfo(aQTS);
            ShowStudyInfo();
        }

        private void Remin(object sender, EventArgs e)
        {
            Param_Form Reminform = new Param_Form();

            if (aQTS == null) return;

            ReminRecord RR = aQTS.AQTSeg.Location.Remin;
            RR.Setup();
            TParameter[] PPS = RR.InputArray();

            Reminform.EditParams(ref PPS, "Remineralization Parameters", false, "ReminLib.JSON", "Remineralization");
            ShowStudyInfo();
        }


        private void AddColumn(ref TParameter Param, DataTable table)
        {
            DataColumn column;

            if (Param is TSubheading) return;

            column = new DataColumn();
            column.ColumnName = Param.Symbol;
            // if (table.Columns.Count == 0) column.Unique = true;
            if (Param.Symbol == "") column.ColumnName = Param.Name;

            if (Param.GetType() == typeof(TParameter))
            {
                column.DataType = System.Type.GetType("System.Double"); table.Columns.Add(column);
                column = new DataColumn();
                column.ColumnName = Param.Symbol + " Reference";
                column.DataType = System.Type.GetType("System.String");
            }
            else if ((Param is TStringParam) || (Param is TDropDownParam))
                column.DataType = System.Type.GetType("System.String");
            else if (Param is TBoolParam)
                column.DataType = System.Type.GetType("System.Boolean");
            else if (Param is TDateParam)
                column.DataType = System.Type.GetType("System.DateTime");

            table.Columns.Add(column);

        }


        private void AddCell(ref TParameter Param, DataRow row, ref int ColNum)
        {
            if (Param is TSubheading) return;

            if (Param.GetType() == typeof(TParameter))
            {
                row[ColNum] = Param.Val;
                row[ColNum + 1] = Param.Comment;
                ColNum++;
            }
            else if (Param is TStringParam)
                row[ColNum] = (Param as TStringParam).Val;
            else if (Param is TDropDownParam)
                row[ColNum] = (Param as TDropDownParam).Val;
            else if (Param is TBoolParam)
                row[ColNum] = (Param as TBoolParam).Val;
            else if (Param is TDateParam)
                row[ColNum] = (Param as TDateParam).Val;

            ColNum++;
        }

        private void ReadCell(ref TParameter Param, DataRow row, ref int ColNum)
        {
            if (Param is TSubheading) return;

            if (Param.GetType() == typeof(TParameter))
            {
                Param.Val = (double)row[ColNum];
                Param.Comment = (string)row[ColNum + 1];
                ColNum++;
            }
            else if (Param is TStringParam)
                (Param as TStringParam).Val = (string)row[ColNum];
            else if (Param is TDropDownParam)
                (Param as TDropDownParam).Val = (string)row[ColNum];
            else if (Param is TBoolParam)
                (Param as TBoolParam).Val = (bool)row[ColNum];
            else if (Param is TDateParam)
                (Param as TDateParam).Val = (DateTime)row[ColNum];

            ColNum++;
        }

        private DataTable ParmArray_to_Table(string TableName, TParameter[] arr)
        {
            DataTable table = new DataTable(TableName);
            for (int i = 0; i < arr.Length; i++)
            {
                AddColumn(ref arr[i], table);
            }

            return table;
        }

        private bool ParmArray_to_Table_Row(ref DataTable table, TParameter[] arr)
        {
            int ColNum = 0;
            DataRow row = table.NewRow();
            for (int i = 0; i < arr.Length; i++)
                AddCell(ref arr[i], row, ref ColNum);
            table.Rows.Add(row);
            return true;
        }

        private List<ReminRecord> Table_to_ReminDB(DataTable table)
        {
            List<ReminRecord> SRL = new List<ReminRecord>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int ColNum = 0;
                ReminRecord SR = new ReminRecord();
                SR.Setup();
                DataRow row = table.Rows[i];
                TParameter[] TPS = SR.InputArray();
                for (int j = 0; j < TPS.Length; j++)
                {
                    ReadCell(ref TPS[j], row, ref ColNum);
                }
                SRL.Add(SR);
            }
            return SRL;
        }

        private List<SiteRecord> Table_to_SiteDB(DataTable table)
        {
            List<SiteRecord> SRL = new List<SiteRecord>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int ColNum = 0;
                SiteRecord SR = new SiteRecord();
                SR.Setup();
                DataRow row = table.Rows[i];
                TParameter[] TPS = SR.InputArray();
                for (int j = 0; j < TPS.Length; j++)
                {
                    ReadCell(ref TPS[j], row, ref ColNum);
                }
                SRL.Add(SR);
            }
            return SRL;
        }

        private List<ChemicalRecord> Table_to_ChemDB(DataTable table)
        {
            List<ChemicalRecord> SRL = new List<ChemicalRecord>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int ColNum = 0;
                ChemicalRecord SR = new ChemicalRecord();
                SR.Setup();
                DataRow row = table.Rows[i];
                TParameter[] TPS = SR.InputArray();
                for (int j = 0; j < TPS.Length; j++)
                {
                    ReadCell(ref TPS[j], row, ref ColNum);
                }
                SRL.Add(SR);
            }
            return SRL;
        }

        private List<PlantRecord> Table_to_PlantDB(DataTable table)
        {
            List<PlantRecord> SRL = new List<PlantRecord>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int ColNum = 0;
                PlantRecord SR = new PlantRecord();
                SR.Setup();
                DataRow row = table.Rows[i];
                TParameter[] TPS = SR.InputArray();
                for (int j = 0; j < TPS.Length; j++)
                {
                    ReadCell(ref TPS[j], row, ref ColNum);
                }
                SRL.Add(SR);
            }
            return SRL;
        }

        private List<AnimalRecord> Table_to_AnimDB(DataTable table)
        {
            List<AnimalRecord> ARL = new List<AnimalRecord>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int ColNum = 0;
                AnimalRecord AR = new AnimalRecord();
                AR.Setup();
                DataRow row = table.Rows[i];
                TParameter[] TPS = AR.InputArray();
                for (int j = 0; j < TPS.Length; j++)
                {
                    ReadCell(ref TPS[j], row, ref ColNum);
                }
                ARL.Add(AR);
            }
            return ARL;
        }



        private void AnimDB_Click(object sender, EventArgs e)
        {
            string fileN = ReadDBPath("AnimalLib.JSON");
            if (fileN == "") return;

            string json = File.ReadAllText(fileN);
            List<AnimalRecord> AnimDB = JsonConvert.DeserializeObject<List<AnimalRecord>>(json);

            AnimalRecord SR = AnimDB[0]; SR.Setup();
            TParameter[] PPS = SR.InputArray();

            DataTable table = ParmArray_to_Table("AnimGrid", PPS);
            for (int r = 0; r < AnimDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, AnimDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, true, "AnimalDataScreen"))
            {
                if (gf.gridChange)
                {
                    fileN = ReadSaveName();
                    if (fileN == "") return;

                    List<AnimalRecord> AnimDB2 = Table_to_AnimDB(table);
                    json = JsonConvert.SerializeObject(AnimDB2);
                    File.WriteAllText(fileN, json);
                }
            }
        }

        private void AnimButton_Click(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            List<AnimalRecord> AnimDB = new List<AnimalRecord>();

            int nanm = 0;
            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                if (TSV.IsAnimal())
                {
                    TAnimal TA = TSV as TAnimal;
                    AnimDB.Add(TA.PAnimalData);
                    nanm++;
                }

            if (nanm == 0)
            {
                MessageBox.Show("There are no animals in the current simulation");
                return;
            }

            AnimalRecord AIR = AnimDB[0]; AIR.Setup();
            TParameter[] PPS = AIR.InputArray();

            DataTable table = ParmArray_to_Table("AnimalGrid", PPS);
            for (int r = 0; r < AnimDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, AnimDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, false, "AnimalDataScreen"))
            {
                if (gf.gridChange)
                {
                    List<AnimalRecord> AnimalDB2 = Table_to_AnimDB(table);
                    nanm = 0;
                    foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                        if (TSV.IsAnimal())
                        {
                            TAnimal TA = TSV as TAnimal;
                            TA.PAnimalData = AnimalDB2[nanm];
                            TA.ChangeData();
                            TA.UpdateName();
                            nanm++;
                        }
                    ShowStudyInfo();
                }
            }
        }

        private void ReminDB_Click(object sender, EventArgs e)
        {
            string fileN = ReadDBPath("ReminLib.JSON");
            if (fileN == "") return;

            string json = File.ReadAllText(fileN);
            List<ReminRecord> ReminDB = JsonConvert.DeserializeObject<List<ReminRecord>>(json);

            ReminRecord SR = ReminDB[0]; SR.Setup();
            TParameter[] PPS = SR.InputArray();

            DataTable table = ParmArray_to_Table("ReminGrid", PPS);
            for (int r = 0; r < ReminDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, ReminDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, true, "Remineralization"))
            {
                if (gf.gridChange)
                {
                    fileN = ReadSaveName();
                    if (fileN == "") return;

                    List<ReminRecord> ReminDB2 = Table_to_ReminDB(table);
                    json = JsonConvert.SerializeObject(ReminDB2);
                    File.WriteAllText(fileN, json);
                }
            }
        }

        private void ChemDB_Click(object sender, EventArgs e)
        {
            string fileN = ReadDBPath("ChemLib.JSON");
            if (fileN == "") return;

            string json = File.ReadAllText(fileN);
            List<ChemicalRecord> ChemDB = JsonConvert.DeserializeObject<List<ChemicalRecord>>(json);

            ChemicalRecord SR = ChemDB[0]; SR.Setup();
            TParameter[] PPS = SR.InputArray();

            DataTable table = ParmArray_to_Table("ChemGrid", PPS);
            for (int r = 0; r < ChemDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, ChemDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, true, "ChemData"))
            {
                if (gf.gridChange)
                {
                    fileN = ReadSaveName();
                    if (fileN == "") return;

                    List<ChemicalRecord> ChemDB2 = Table_to_ChemDB(table);
                    json = JsonConvert.SerializeObject(ChemDB2);
                    File.WriteAllText(fileN, json);
                }
            }
        }

        private string ReadDBPath(string deflt)
        {
            string fileN = Path.GetFullPath("..\\..\\..\\DB\\" + deflt);
            if (MessageBox.Show("Open the default database: '" + fileN + "'?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Text File|*.txt;*.json";
                openFileDialog1.Title = "Open a JSON File";
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return "";
                fileN = openFileDialog1.FileName;
            }

            return fileN;
        }

        private string ReadSaveName()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JSON files (*.JSON)|*.JSON|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.OverwritePrompt = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK) return saveFileDialog1.FileName;
            else return "";

        }

        private void SiteDB_Click(object sender, EventArgs e)
        {
            string fileN = ReadDBPath("SiteLib.JSON");
            if (fileN == "") return;

            string json = File.ReadAllText(fileN);
            List<SiteRecord> SiteDB = JsonConvert.DeserializeObject<List<SiteRecord>>(json);

            SiteRecord SR = SiteDB[0]; SR.Setup();
            TParameter[] PPS = SR.InputArray();

            DataTable table = ParmArray_to_Table("SiteGrid", PPS);
            for (int r = 0; r < SiteDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, SiteDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, true, "Site"))
            {
                if (gf.gridChange)
                {
                    fileN = ReadSaveName();
                    if (fileN == "") return;

                    List<SiteRecord> SiteDB2 = Table_to_SiteDB(table);
                    json = JsonConvert.SerializeObject(SiteDB2);
                    File.WriteAllText(fileN, json);
                }
            }
        }

        private void PlantsDB_Click(object sender, EventArgs e)
        {
            string fileN = ReadDBPath("PlantLib.JSON");
            if (fileN == "") return;

            string json = File.ReadAllText(fileN);
            List<PlantRecord> PlantDB = JsonConvert.DeserializeObject<List<PlantRecord>>(json);

            PlantRecord PR = PlantDB[0]; PR.Setup();
            TParameter[] PPS = PR.InputArray();

            DataTable table = ParmArray_to_Table("PlantGrid", PPS);
            for (int r = 0; r < PlantDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, PlantDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            if (gf.ShowGrid(table, false, true, "PlantData"))
            {
                if (gf.gridChange)
                {
                    fileN = ReadSaveName();
                    if (fileN == "") return;

                    List<PlantRecord> PlantDB2 = Table_to_PlantDB(table);
                    json = JsonConvert.SerializeObject(PlantDB2);
                    File.WriteAllText(fileN, json);
                }
            }
        }


        private void StudyNameBox_TextChanged(object sender, EventArgs e)
        {
            if (aQTS == null) {; return; }
            aQTS.AQTSeg.StudyName = StudyNameBox.Text;
        }

        private void SVListBox_DoubleClick(object sender, EventArgs e)
        {
            if (SVListBox.SelectedIndex == -1) { MessageBox.Show("No State Variable is Selected."); return; }
            TStateVariable TSV = TSVList[SVListBox.SelectedIndex];

            LoadingsForm LF = new LoadingsForm();
            LF.EditSV(ref TSV, aQTS, isBoundarySegment);

            // AQTStudy.Adjust_Internal_Nutrients;  // Future code to enable -- in case plant types have changed

            ShowStudyInfo();

        }



        private void SVListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NetCDF_Click(object sender, EventArgs e)
        {

            // Gets the path to the NetCDF file to be used as a data source.
            //var dataset = sds.DataSet.Open("N:\\AQUATOX\\CSRA\\outputrch.nc?openMode=readOnly");
            // var dataset = sds.DataSet.Open("N:\\AQUATOX\\CSRA\\outputhru.nc?openMode=readOnly");

            //  sds.MetadataDictionary dt = dataset.Metadata;

            string fieldname = "NO3GWkg_ha";

            double[,,] dataValues = null; //  dataset.GetData<double[,,]>(fieldname);
            var result = string.Join(",", dataValues);

            StreamWriter file = new StreamWriter("N:\\AQUATOX\\CSRA\\" + fieldname + ".csv");
            int topi = dataValues.GetLength(0);
            int topj = dataValues.GetLength(1);
            int topk = dataValues.GetLength(2);

            topi = 365; //output first year for debugging 

            for (int i = 0; i < topi; i++)
            {
                for (int j = 0; j < topj; j++)
                    for (int k = 0; k < topk; k++)
                    {
                        file.Write(dataValues[i, j, k]);
                        file.Write(",");
                    }
                //go to next line
                file.Write("\n");
            }

            file.Close();

            MessageBox.Show("Exported N:\\AQUATOX\\CSRA\\" + fieldname + ".csv");

        }



        private void EditButton_Click(object sender, EventArgs e)
        {
            SVListBox_DoubleClick(sender, e);
        }

        private void FoodWebButton_Click(object sender, EventArgs e)
        {
            DataTable[] tTables = new DataTable[3];

            tTables = aQTS.AQTSeg.TrophInt_to_Table();

            TrophMatrix tm = new TrophMatrix();
            if (tm.ShowGrid(tTables, aQTS.AQTSeg))
                aQTS.AQTSeg.Tables_to_Trophint(tTables);
        }

        private void MultiSegButton_Click(object sender, EventArgs e)
        {
            MultiSegForm MSForm = new MultiSegForm();
            MSForm.ShowDialog();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            ListForm LF = new ListForm();
            List<AllVariables> SVs = null;

            int index = LF.SelectFromList(aQTS.AQTSeg.GetInsertableStates(ref SVs));
            if (index < 0) return;

            AllVariables ns = SVs[index];
            Param_Form PF = new Param_Form();

            TStateVariable NewSV = null;

            if ((ns >= Consts.FirstAnimal) && (ns <= Consts.LastAnimal))
            {
                AnimalRecord AR = PF.ReturnRecordFromDB("AnimalLib.JSON") as AnimalRecord;
                if (AR == null) return;
                NewSV = aQTS.AQTSeg.AddStateVariable(ns, T_SVLayer.WaterCol);
                if (NewSV != null)
                {
                    ((TAnimal)NewSV).PAnimalData = AR;
                    ((TAnimal)NewSV).ChangeData();
                }
            }
            else if ((ns >= Consts.FirstPlant) && (ns <= Consts.LastPlant))
            {
                PlantRecord PR = PF.ReturnRecordFromDB("PlantLib.JSON") as PlantRecord;
                if (PR == null) return;
                NewSV = aQTS.AQTSeg.AddStateVariable(ns, T_SVLayer.WaterCol);
                if (NewSV != null)
                {
                    ((TPlant)NewSV).PAlgalRec = PR;
                    ((TPlant)NewSV).ChangeData();
                }
            }
            else if (ns == AllVariables.H2OTox)
            {
                ChemicalRecord CR = PF.ReturnRecordFromDB("ChemLib.JSON") as ChemicalRecord;
                if (CR == null) return;
                NewSV = aQTS.AQTSeg.Add_OrgTox_SVs(CR);
            }
            else NewSV = aQTS.AQTSeg.AddStateVariable(ns, T_SVLayer.WaterCol);

            if (NewSV == null) MessageBox.Show("Error adding State Variable");

            NewSV.UpdateName();
            ShowStudyInfo();
        }

        public void DeleteStateVariable(int index) // Deletes the variable identified by DeleteVar and all associated toxicants
        {

            TStateVariable T = aQTS.AQTSeg.SV[index];
            aQTS.AQTSeg.DeleteVar(index);

            T_SVType ToxLoop;  // Delete associated toxicant records if they exist,  Also Internal Nutrient records

            for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= T_SVType.PIntrnl; ToxLoop++)
            {
                TStateVariable TSV = aQTS.AQTSeg.GetStatePointer(T.NState, ToxLoop, T_SVLayer.WaterCol);
                if (TSV != null) aQTS.AQTSeg.DeleteVar(TSV);
            }

            // in the future zero out reciprocal same species record
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (SVListBox.SelectedIndex == -1) { MessageBox.Show("No State Variable is Selected."); return; }
            AllVariables ns = TSVList[SVListBox.SelectedIndex].NState;

            int index = -1;
            for (int i = 0; i < aQTS.AQTSeg.SV.Count; i++)
            {
                if ((ns == aQTS.AQTSeg.SV[i].NState) && ((aQTS.AQTSeg.SV[i].SVType == T_SVType.StV) || (ns == AllVariables.H2OTox))) { index = i; break; }
            }

            if (MessageBox.Show("Delete the state variable: '" + aQTS.AQTSeg.SV[index].PName + "'?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            if (ns == AllVariables.H2OTox)
                aQTS.AQTSeg.RemoveOrgToxStateVariable(SVListBox.SelectedIndex);
            else DeleteStateVariable(index);

            ShowStudyInfo();

        }

        private static List<string> BrowserNames = new List<string> { "Chrome", "Firefox", "Internet Explorer", "Microsoft Edge", "Safari", "Other" };
        private static string[] BrowserExe = { "chrome.exe", "firefox.exe", "iexplore.exe", "msedge.exe", "safari.exe", "" };

        static public string SelectDefaultBrowser()
        {
            ListForm LF = new ListForm();
            LF.Text = "Select Default Browser";
            int index = LF.SelectFromList(BrowserNames);
            if (index < 0) return "";
            string bExe = BrowserExe[index];
            if (bExe == "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Browser EXE|*.exe";
                openFileDialog1.Title = "Select Default Browser EXE";
                openFileDialog1.ShowDialog();
                bExe = Path.GetFileName(openFileDialog1.FileName);
            }
            return bExe;
        }

        static public void OpenUrl(string bookmark)
        {
            string url = "file:" + Path.GetFullPath("../../../Docs/AQUATOX.NET_1.0_UMAN.htm");
            url = Uri.UnescapeDataString(url + "#" + bookmark);
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = false });  //true opens default browser, but not at context-sensitive bookmark
            }
            catch
            {
                // workaround because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (defaultBrowser == "")
                    {
                        defaultBrowser = SelectDefaultBrowser();
                        Properties.Settings.Default.BrowserExe = defaultBrowser;
                    }

                    url = url.Replace("&", "^&");
                    // string args = $"/c start {url}";
                    string args = $"/c start {defaultBrowser} {url}";

                    Process.Start(new ProcessStartInfo("cmd", args) { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }


        private void HelpButton_Click(object sender, EventArgs e)
        {
            string target = "MainScreen";
            OpenUrl(target);
        }

        private void browserButton_Click(object sender, EventArgs e)
        {
            defaultBrowser = SelectDefaultBrowser();
            Properties.Settings.Default.BrowserExe = defaultBrowser;
        }


        public bool EditLinkedInput(ref string json, bool isBoundary)
        {
            if (!LoadJSON(json)) return false;

            SVListBox.Top = ScaleY(78);
            StateVarLabel.Top = ScaleY(60);
            SVListBox.Height = ScaleY(390);

            HelpButton.Top = ScaleY(78);
            browserButton.Top = ScaleY(110);

            ButtonPanel.Top = ScaleY(78);
            ParametersLabel.Top = ScaleY(60);

            isBoundarySegment = isBoundary;
            MultiSegmentInput = true;
            ButtonPanel.Visible = true;
            AnnealPanel.Visible = false;
            SetupButton.Visible = false;
            MultiSegButton.Visible = false;
            DBPanel.Visible = false;
            loadJSON.Visible = false;
            saveJSON.Visible = false;

            OKButton.Visible = true;
            CancelButt.Visible = true;

            ShowStudyInfo();

            if (ShowDialog() == DialogResult.Cancel) return false;

            string errmessage = aQTS.SaveJSON(ref json);
            if (errmessage != "") MessageBox.Show("Error creating json string: " + errmessage);
            return (errmessage == "");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Worker.CancelAsync();
            ParAnnealWorker.CancelAsync();
        }

        private void CancelButt_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Discard all edits made to this linked segment?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes) this.DialogResult = DialogResult.Cancel;

        }

        // Random rnd = new Random();
        private double RandUniform(double DMin, double DMax)
        {
            return (DMax - DMin) * ThreadLocalRandom.Instance.NextDouble() + DMin;
        }

        private void AnnealWorker_RunCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Update();

            Set_Buttons(false);
            ExportBest.Enabled = false;

            ShowStudyInfo();
        }


        private double SimError(AQTSim Sim)
        {
            bool WINTERSTART = (DateTime.Compare(Sim.AQTSeg.PSetup.FirstDay.Val, new DateTime(1999, 1, 1, 0, 0, 0)) == 0);  // winterstart triggered based on 1/1/1999 start date

            int[] Indices = new int[6] { 3, 22, 42, 63, 83, 103 };    
            double[,] ObsPctPhyto = new double[4, 6] {{ 11.382, 29.0909, 0, 0, 0, 0 }, //Diatoms
                                                     { 21.14,9.7,17.65,7.27,4.31,31.29 }, //Greens
                                                     { 36.59,25.45,61.18,70.0,12.35,9.4 }, //cyanobacteria
                                                     { 30.89,35.76,21.18,22.73,83.33,59.31 }}; //chrysophytes


            double[,] ObsBioEst = new double[7, 4] {{0.00129,0.00239,0.00413,0.00349},
                                                    {0.00808,0.00269,0.00707,0.00994},
                                                    {0.01559,0.00387,0.00616,0.01426},
                                                    {0.00000,0.00320,0.01108,0.00384},
                                                    {0.00000,0.00061,0.00585,0.00190},
                                                    {0.00000,0.00444,0.01273,0.08586},
                                                    {0.00000,0.02282,0.00686,0.04326}};

            int[] BioIndices = new int[7] { 3, 22, 29, 42, 63, 83, 103 };    // annual 

            int[] ChlaInd = new int[15] { 8, 14, 22, 28, 36, 43, 51, 58, 64, 71, 78, 87, 104, 110, 121 };
            double[] ObsChla = new double[15] { 0.43, 0.66, 0.779, 1.393, 0.512, 0.799, 0.328, 0.35, 0.471, 0.615, 1.885, 4.467, 2.602, 2.48, 2.623 };

            if (WINTERSTART)
            {
                for (int i = 0; i < Indices.Length; i++)  // start simulation on 1/1/1999 rather than 6/1/1999  
                {
                    Indices[i] += 151;
                }

                for (int i = 0; i < ChlaInd.Length; i++)
                {
                    ChlaInd[i] += 151;
                }

                for (int i = 0; i < BioIndices.Length; i++)
                {
                    BioIndices[i] += 151;
                }
            }


            //        int DiatomPos = 12;
            //        int GreensPos = 13;
            //        int CyanPos = 14;
            //        int ChrysoPos = 15;

            double[] ModelSumPhyto = new double[6];
            double[] ModelChla = new double[15];

            double[,] PctPhyto = new double[4, 6];
            double[] phyto = new double[4];

            for (int i = 0; i < 6; i++)
            {
                ModelSumPhyto[i] = 0;
                for (int p = 12; p < 16; p++)
                {
                    phyto[p - 12] = Convert.ToDouble(Sim.AQTSeg.SV[p].SVoutput.Data.Values.ElementAt(Indices[i])[0]);
                    ModelSumPhyto[i] += phyto[p - 12];
                }

                for (int p = 12; p < 16; p++)
                    PctPhyto[p - 12, i] = 100 * (phyto[p - 12] / ModelSumPhyto[i]);
            }

            //double Bioerror = 0;
            //for (int i = 0; i < 7; i++)
            //{
            //   for (int p = 12; p < 16; p++)
            //    {
            //        double ModelBio = Convert.ToDouble(Sim.AQTSeg.SV[p].SVoutput.Data.Values.ElementAt(BioIndices[i])[0]);
            //        double ObsBio = ObsBioEst[i, p-12];
            //        Bioerror += Math.Abs(ModelBio - ObsBio);
            //    }
            //}
            //return Bioerror*100;

            // double chlaMAE25 = 0;
            double chlaFE3 = 0;
            for (int i = 0; i < 15; i++)
            {
                ModelChla[i] = 0;
                for (int p = 12; p < 16; p++)
                {
                    TPlant TP = Sim.AQTSeg.SV[p] as TPlant;
                    double Plant_to_ChlA = TP.PAlgalRec.Plant_to_Chla.Val;

                    ModelChla[i] +=
                        Convert.ToDouble(Sim.AQTSeg.SV[p].SVoutput.Data.Values.ElementAt(ChlaInd[i])[0]) * (0.526 / Plant_to_ChlA) * 1000.0;
                }
             // chlaMAE25 += Math.Abs(ModelChla[i] - ObsChla[i]);
                chlaFE3 += Math.Max(ModelChla[i], ObsChla[i]) / Math.Min(ModelChla[i], ObsChla[i]);
            }

            if (WINTERSTART)
            {
                double[] WinterChla = new double[4] { 1.0, 1.0, 1.0, 1.0 };
                int[] WChlaInd = new int[4] { 15, 40, 70, 90 }; //WINTER chl-a indices

                double ModelWChla; //WINTER
                for (int i = 0; i < 4; i++)
                {
                    ModelWChla = 0;
                    for (int p = 12; p < 16; p++)
                    {
                        TPlant TP = Sim.AQTSeg.SV[p] as TPlant;
                        double Plant_to_ChlA = TP.PAlgalRec.Plant_to_Chla.Val;

                        ModelWChla +=
                            Convert.ToDouble(Sim.AQTSeg.SV[p].SVoutput.Data.Values.ElementAt(WChlaInd[i])[0]) * (0.526 / Plant_to_ChlA);
                    }
                    chlaFE3 += Math.Max(ModelWChla, WinterChla[i]) / Math.Min(ModelWChla, WinterChla[i]);
                    //chlaMAE25 += Math.Abs(ModelWChla - WinterChla[i]);
                }
                // chlaMAE25 = (chlaMAE25 / 19) * 25;  //WINTER 19 observations
                chlaFE3 = (chlaFE3 / 19.0) * 3.0;  //WINTER 19 observations
            }
            else
            {
                //chlaMAE25 = (chlaMAE25 / 15) * 25;  //summer 15 observations
                chlaFE3 = (chlaFE3 / 15.0) * 3.0;  //WINTER 19 observations
            }

            double phytoMAE = 0;
            for (int i = 0; i < 6; i++)
                for (int p = 0; p < 4; p++)
                    phytoMAE += Math.Abs(PctPhyto[p, i] - ObsPctPhyto[p, i]);
            phytoMAE = phytoMAE / 24.0;

            //return chlaMAE25 + phytoMAE;
            return chlaFE3 + phytoMAE;
        }

        public static class ThreadLocalRandom
        {
            private static readonly Random globalRandom = new Random();
            private static readonly object globalLock = new object();

            private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(NewRandom);

            public static Random NewRandom()
            {
                lock (globalLock)
                {
                    return new Random(globalRandom.Next());
                }
            }

            public static Random Instance { get { return threadRandom.Value; } }

            public static int Next()
            {
                return Instance.Next();
            }
        }



        private void ParallelAnnealClick(object sender, EventArgs e)
        {
            if (aQTS == null) { MessageBox.Show("No Simulation Loaded"); return; }
            else
            {
                Set_Buttons(true);
                ExportBest.Enabled = true;


                ParAnnealWorker.RunWorkerAsync();
            }
        }

        private void ParAnnealWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            aQTS.AQTSeg.ProgWorker = worker;
            ParallelAnneal();
        }


        const int NDTypes = 10;   // CHANGEDIST
        const int NDists = NDTypes * 4;
        double[] BestParms = new double[NDists];
        int[] DTypes = new int[NDists];
        int[] SVIndices = new int[NDists];


        private void ParallelAnneal()
        {

            string logfile;
            

            double Old_Error = 100000;
            double Min_Error = 100000;
            double New_Error = 100000;
            double Min_Error_Logged = 100000;
            double T = double.Parse(TStartBox.Text);  // start at temperature of 50
            int NUMDRAWS = int.Parse(IterationBox.Text); // Number of draws to run at each temperature;  fixme 
            const double T_min = 0.0001;
            const double Alpha = 0.9;
            TPlant TP;

            //DType 0 = TOpt
            //DType 1 = PMax
            //DType 2 = RespRate
            //DType 3 = LightSat     NOT Currently MinSat 6/10/2023  // CHANGEDIST
            //DType 4 = NHalfSat
            //DType 5 = PHalfSat
            //DType 6 = EMort
            //DType 7 = Mort Coeff
            //DType 8 = Min Temp
            //DType 9 = Max Temp
            //DType 7 = MaxSat 6/10/2023 // CHANGEDIST

            double[,] DMins = new double[NDTypes, 4];
            double[,] DMaxs = new double[NDTypes, 4];

            DMins[0, 0] = 15.0; DMaxs[0, 0] = 30.0; //DType 0 = TOpt  DIATOMS
            DMins[0, 1] = 10.0; DMaxs[0, 1] = 35.0; //DType 0 = TOpt  Chlorella
            DMins[0, 2] = 12.0; DMaxs[0, 2] = 35.0; //DType 0 = TOpt  Cyanobacteria
            DMins[0, 3] = 10.0; DMaxs[0, 3] = 23.0; //DType 0 = TOpt  Chrysophyte

            DMins[1, 0] = 0.4; DMaxs[1, 0] = 3.5; //DType 1 = PMax  DIATOMS
            DMins[1, 1] = 0.1; DMaxs[1, 1] = 3.5; //DType 1 = PMax  Chlorella
            DMins[1, 2] = 0.2; DMaxs[1, 2] = 3.5; //DType 1 = PMax  Cyanobacteria
            DMins[1, 3] = 0.2; DMaxs[1, 3] = 1.0; //DType 1 = PMax  Chrysophyte

            DMins[8, 0] = 0.0; DMaxs[8, 0] = 15; //DType 8 = TMin  DIATOMS
            DMins[8, 1] = 0.0; DMaxs[8, 1] = 15; //DType 8 = TMin  Chlorella
            DMins[8, 2] = 1.0; DMaxs[8, 2] = 20; //DType 8 = TMin  Cyanobacteria
            DMins[8, 3] = -2.0; DMaxs[8, 3] = 12; //DType 8 = TMin  Chrysophyte

            DMins[9, 0] = 25.0; DMaxs[9, 0] = 35.0; //DType 9 = TMax  DIATOMS
            DMins[9, 1] = 30.0; DMaxs[9, 1] = 48.0; //DType 9 = TMax  Chlorella
            DMins[9, 2] = 20.0; DMaxs[9, 2] = 42.0; //DType 9 = TMax  Cyanobacteria
            DMins[9, 3] = 25.0; DMaxs[9, 3] = 37.0; //DType 9 = TMax  Chrysophyte


            for (int i = 0; i < 4; i++)
            {
                DMins[2, i] = 0.001; DMaxs[2, i] = 0.2;   //DType 2 = RespRate20
                DMins[3, i] = 18; DMaxs[3, i] = 1000;  //DType 3 = EnteredLightSat  // CHANGEDIST
                DMins[4, i] = 0.0005; DMaxs[4, i] = 0.53;  //DType 4 = NHalfSat
                DMins[5, i] = 0.0001; DMaxs[5, i] = 0.1;   //DType 5 = PHalfSat
                DMins[6, i] = 0.005; DMaxs[6, i] = 0.05;  //DType 6 = EMort
                DMins[7, i] = 0.0001; DMaxs[7, i] = 0.03;  //DType 7 = Mortality Coeff
            }

            double[] OldParms = new double[NDists];

            int dtype = 0;
            int org = 0;
            int svindex = 12; //12-15 are diatoms through chrysophytes
            
            for (int d = 0; d < NDists; d++)  // set up distributions
            {
                DTypes[d] = dtype;
                SVIndices[d] = svindex;
                if (randomBox.Checked) OldParms[d] = RandUniform(DMins[DTypes[d], org], DMaxs[DTypes[d], org]);  // first random value

                dtype++;
                if (dtype > NDTypes - 1)
                {
                    dtype = 0;
                    svindex++;
                    org++;
                }
            };

            if (!randomBox.Checked)
            {
                CopySimToParm(aQTS.AQTSeg, OldParms);
            }

            aQTS.SavedRuns.Clear();  //segment will be copied, so simplify
            aQTS.AQTSeg.ClearResults();

            logfile = @"N:\Temporary\" + Path.GetFileName(aQTS.AQTSeg.FileName) + "PSALog.txt";

            string AQTSJSON = Newtonsoft.Json.JsonConvert.SerializeObject(aQTS, AQTSim.AQTJSONSettings());

            bool UserCancel = false;
            int totruns = 0;
            int uselessruns = 0;
            int Truns = 0;

            while (T > T_min)
            {
                if (UserCancel) return;

                int LastAccept = -1;
                int lastprog = 0;
                int runs = 0;

                Parallel.For(0, NUMDRAWS, (dr, state) =>
                {
                double[] NewParms = new double[NDists];

                OldParms.CopyTo(NewParms, 0);
                int OldParmIndex = LastAccept;  // interation basis for this test

                int index = ThreadLocalRandom.Instance.Next(0, NDists - 1);

                NewParms[index] = RandUniform(DMins[DTypes[index], SVIndices[index] - 12], DMaxs[DTypes[index], SVIndices[index] - 12]);

                AQTSim LocalAQTS = Newtonsoft.Json.JsonConvert.DeserializeObject<AQTSim>(AQTSJSON, AQTSim.AQTJSONSettings());  //likely costly in terms of time
                LocalAQTS.AQTSeg.SetupLinks();
                CopyParmToSim(LocalAQTS.AQTSeg,NewParms);

                LocalAQTS.Integrate();

                New_Error = SimError(LocalAQTS);

                double AP = (Old_Error - New_Error) / T;
                if (AP > 100) AP = 3E+43;
                else AP = Math.Exp(AP);

                bool AcceptThis = ((New_Error < Min_Error) || (AP > ThreadLocalRandom.Instance.NextDouble()));

                if ((OldParmIndex != LastAccept) && (AcceptThis))
                        uselessruns++;  // acceptable run, but last run was accepted so throw it out 

                totruns++; //total runs
                runs++; //runs in this loop
                Truns++; //runs at this temperature this loop

                int progint = Math.Max((int)(((double)runs / (double)NUMDRAWS) * 100.0), 1);
                if (progint > lastprog)
                    {
                        ParAnnealWorker.ReportProgress(progint, "Temperature: " + T.ToString("0.####") + "; Err: " + Min_Error.ToString(("00.###")) +
                            "; Used: " + ((((double)Truns - (double)uselessruns) / (double)Truns) * 100.0).ToString("#0.0") +
                            "% ( " + (Truns - uselessruns).ToString() + " of " + Truns.ToString() + ").  TotRuns = " + totruns.ToString());
                        lastprog = progint;
                    }

                if (ParAnnealWorker.CancellationPending)
                    {
                        UserCancel = true;
                        state.Break();
                    }

                if ((OldParmIndex == LastAccept) && (AcceptThis)) // accept 
                    {
                        LastAccept = dr; //this is the latest accepted parameter set now
                        Old_Error = New_Error;
                        lock (OldParms) { NewParms.CopyTo(OldParms, 0); }

                        if (New_Error < Min_Error)
                        {
                            Min_Error = New_Error;
                            lock (BestParms) { NewParms.CopyTo(BestParms, 0); }
                        }
                    }
                }); //parallel.For

                if (Min_Error < Min_Error_Logged)
                    using (StreamWriter sw = File.AppendText(logfile))
                    {
                        sw.Write("Error " + Min_Error.ToString() + "; Temperature " + T.ToString() + "; ");

                        foreach (double item in BestParms) sw.Write(item.ToString() + "; ");
                        sw.WriteLine();

                        Min_Error_Logged = Min_Error;
                    }

                if ((Truns - uselessruns) > (NUMDRAWS / 2))
                {
                    T = Alpha * T;
                    Truns = 0;
                    uselessruns = 0;
                }
            }

            CopyParmToSim(aQTS.AQTSeg, BestParms);
        }


        private void CopyParmToSim(AQUATOXSegment ASeg, double[] Parms)
        {
            TPlant TP;

            for (int d = 0; d < NDists; d++) // copy back results to simulation
            {
                TP = ASeg.SV[SVIndices[d]] as TPlant;

                switch (DTypes[d])
                {
                    case 0: TP.PAlgalRec.TOpt.Val = Parms[d]; break;
                    case 1: TP.PAlgalRec.PMax.Val = Parms[d]; break;
                    case 2: TP.PAlgalRec.Resp20.Val = Parms[d]; break;
                    case 3: TP.PAlgalRec.EnteredLightSat.Val = Parms[d]; break;  
                    case 4: TP.PAlgalRec.KN.Val = Parms[d]; break;
                    case 5: TP.PAlgalRec.KPO4.Val = Parms[d]; break;
                    case 6: TP.PAlgalRec.EMort.Val = Parms[d]; break;
                    case 7: TP.PAlgalRec.KMort.Val = Parms[d]; break;
                    case 8: TP.PAlgalRec.TRef.Val = Parms[d]; break;
                    case 9: TP.PAlgalRec.TMax.Val = Parms[d]; break;

                }
            }
        }

        private void CopySimToParm(AQUATOXSegment ASeg, double[] Parms)
        {
            TPlant TP;

            for (int d = 0; d < NDists; d++) // copy back results to simulation
            {
                TP = ASeg.SV[SVIndices[d]] as TPlant;

                switch (DTypes[d])
                {
                    case 0: Parms[d] = TP.PAlgalRec.TOpt.Val; break;
                    case 1: Parms[d] = TP.PAlgalRec.PMax.Val ; break;
                    case 2: Parms[d] = TP.PAlgalRec.Resp20.Val ; break;
                    case 3: Parms[d] = TP.PAlgalRec.EnteredLightSat.Val ; break;
                    case 4: Parms[d] = TP.PAlgalRec.KN.Val ; break;
                    case 5: Parms[d] = TP.PAlgalRec.KPO4.Val ; break;
                    case 6: Parms[d] = TP.PAlgalRec.EMort.Val ; break;
                    case 7: Parms[d] = TP.PAlgalRec.KMort.Val ; break;
                    case 8: Parms[d] = TP.PAlgalRec.TRef.Val ; break;
                    case 9: Parms[d] = TP.PAlgalRec.TMax.Val ; break;
                }
            }
        }

        private void ExportBestClick(object sender, EventArgs e)
        {
            CopyParmToSim(aQTS.AQTSeg,BestParms);
            saveJSON_Click(sender, e);
        }

        private void DeleteResultsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all archived results?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                aQTS.SavedRuns.Clear();
                ShowStudyInfo();
            }
        }


        // -------------------------------------------------------------------------------------------------------------------------------------

        string CsvFilePath = @"N:\AQUATOX\CSRA\GIS\estuaries\ACEBI2_IW10_Simplified.csv";
        string DBFilePath = @"N:\AQUATOX\CSRA\GIS\estuaries\Intersecting_Estuary_to_COMID_DB_051223_B.csv";
        string OutDBFilePath = @"N:\AQUATOX\CSRA\GIS\estuaries\Intersecting_Estuary_to_COMID_DB_051523.csv";
        char csvDelimiter = ',';

        public static void DataTableToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        public DataTable CSVtoDataTable(string strFilePath, char csvDelimiter)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(csvDelimiter);
                foreach (string header in headers)
                {
                    try
                    {
                        dt.Columns.Add(header);
                    }
                    catch { }
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(csvDelimiter);
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
            return dt;
        }

        DataTable dt;
        DataTable DBt;


        private string[] downCOMIDs(string COMIDstr)
        {
            string searchstr = "COMID='" + COMIDstr + "'";
            DataRow[] rows = dt.Select(searchstr);
            if (rows == null) return null;
            if (rows.Count() == 0) return null;

            searchstr = "FROMNODE='" + rows[0]["TONODE"].ToString() + "'";
            rows = dt.Select(searchstr);
            if (rows == null) return null;
            if (rows.Count() == 0) return null;

            string[] res = new string[rows.Count()];
            for (int r = 0; r < rows.Count(); r++)
                res[r] = rows[r]["COMID"].ToString();
            return res;
        }


        private List<string> traverseDown5(string COMIDstr)
        {

            List<string> res;

            void downCOMIDs5(string COMIDstr, int depth)
            {
                if (depth > 5) return;
                string[] DCIs = downCOMIDs(COMIDstr);
                if (DCIs != null)
                {
                    if (DCIs.Length > 1)  // choose dominant pathway in a divergence
                    {
                        foreach (string DCI in DCIs)
                        {
                            string searchstr = "COMID='" + DCI + "'";
                            DataRow[] divrows = dt.Select(searchstr);
                            if (divrows == null) return;
                            string DivString = divrows[0]["DIVERGENCE"].ToString();
                            int DivInt = (int)float.Parse(DivString);
                            if (DivInt < 2)
                            {
                                res.Add(DCI);
                                downCOMIDs5(DCI, depth + 1);
                            }
                        }
                    }

                    else // just one downstream traversal
                    {
                        res.Add(DCIs[0]);
                        downCOMIDs5(DCIs[0], depth + 1);
                    };
                }
            };

            res = new List<string>();
            downCOMIDs5(COMIDstr, 1);  //recursively traverse down river 5 levels if possible
            return res;
        }

        private void estdbbutton_Click(object sender, EventArgs e)
        {
            dt = CSVtoDataTable(CsvFilePath, csvDelimiter);
            //          string[] dcs = downCOMIDs("20149957");

            DBt = CSVtoDataTable(DBFilePath, csvDelimiter);

            DataTable NewDBt = new DataTable();
            NewDBt.Columns.Add("Estuary ID");
            NewDBt.Columns.Add("Intersecting COMID");

            int r = 0;
            string CurrentEstID = "";
            while (r < DBt.Rows.Count)
            {
                string EstID = DBt.Rows[r]["Estuary ID"].ToString();
                if (CurrentEstID != EstID)
                {
                    CurrentEstID = EstID;
                    string searchstr = "[Estuary ID]='" + EstID + "'";
                    DataRow[] intersections = DBt.Select(searchstr);
                    string[] intCOMIDs = new string[intersections.Length];
                    for (int i = 0; i < intersections.Count(); i++) intCOMIDs[i] = intersections[i]["Intersecting COMID"].ToString();

                    for (int i = 0; i < intersections.Count(); i++)
                    {
                        List<string> dcid5 = traverseDown5(intersections[i]["Intersecting COMID"].ToString());
                        if (intCOMIDs[i] == "6721589")
                        {
                            intCOMIDs[i] = intCOMIDs[i] + ""; // Trap cases for QA/QC
                        }
                        bool DoubleCount = false;
                        foreach (string c in dcid5) if (intCOMIDs.Contains(c)) DoubleCount = true;
                        if (!DoubleCount) NewDBt.Rows.Add(intersections[i].ItemArray);  // no double counting so keep in DB
                    }

                }
                r++;
            }

            //DataTableToCSV(NewDBt, OutDBFilePath);

        }

        private void calcerror_Click(object sender, EventArgs e)
        {
            if (aQTS == null) { MessageBox.Show("No Simulation Loaded"); return; }

            try
            {
                MessageBox.Show(SimError(aQTS).ToString());
            }
            catch
            {
                MessageBox.Show("exception raised while calculating error");
            }
        }
    }
    
}
