using AQUATOX.Animals;
using AQUATOX.AQSite;
using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using AQUATOX.Diagenesis;
using AQUATOX.Plants;
using Globals;
//using Microsoft.Research.Science.Data.Imperative;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
// using sds = Microsoft.Research.Science.Data;

namespace GUI.AQUATOX
{

    public partial class AQTTestForm : Form
    {
        private BackgroundWorker Worker = new BackgroundWorker();
        private string errmessage;

        public AQTSim aQTS = null;
        private List<string> SVList = null;
        private List<TStateVariable> TSVList = null;

        public AQTTestForm()
        {
            InitializeComponent();
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunCompleted);
            Worker.WorkerReportsProgress = true;
        }

        private void AQTTestForm_Load(object sender, EventArgs e)
        {

        }

        //public void DisplaySVs()

        //{
        //    TStateVariable TSV1 = null;
        //    bool SuppressText = true;

        //    int sercnt = 0;
        //    string outtxt = "";

        //    foreach (TStateVariable TSV in outSeg.SV) if (TSV.SVoutput != null)
        //        {
        //            TSV1 = TSV; // identify TSV1 with an output that is not null
        //            int cnt = 0;
        //            if (sercnt == 0) outtxt = "Date, ";

        //            List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
        //            for (int col = 1; col <= vallist.Count(); col++)
        //            {
        //                string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
        //                     TSV.SVoutput.Metadata["Name_" + col.ToString()] +
        //                     " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";

        //                if (col == 1) outtxt = outtxt + sertxt.Replace(",", "") + ", ";  // suppress commas in name for CSV output

        //                sercnt++;

        //                SuppressText = (TSV.SVoutput.Data.Keys.Count > 5000);
        //                for (int i = 0; i < TSV.SVoutput.Data.Keys.Count; i++)
        //                {
        //                    ITimeSeriesOutput ito = TSV.SVoutput;
        //                    string datestr = ito.Data.Keys.ElementAt(i).ToString();
        //                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[col - 1]);
        //                    cnt++;
        //                }
        //            }
        //        }

        //    outtxt = outtxt + Environment.NewLine;

        //    if (!SuppressText)
        //    {
        //        for (int i = 0; i < TSV1.SVoutput.Data.Keys.Count; i++)
        //        {
        //            bool writedate = true;
        //            foreach (TStateVariable TSV in outSeg.SV) if (TSV.SVoutput != null)
        //                {
        //                    ITimeSeriesOutput ito = TSV.SVoutput;
        //                    if (writedate)
        //                    {
        //                        string datestr = ito.Data.Keys.ElementAt(i).ToString();
        //                        outtxt = outtxt + datestr + ", ";
        //                        writedate = false;
        //                    }
        //                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
        //                    outtxt = outtxt + Val.ToString() + ", ";
        //                }
        //            outtxt = outtxt + Environment.NewLine;
        //        }
        //    }

        //    textBox1.Text = outtxt;
        //}


        private void loadJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                string json = File.ReadAllText(openFileDialog1.FileName);
                AQTSim Sim = new AQTSim();
                string err = Sim.Instantiate(json);
                if (err != "") {MessageBox.Show(err); return; }

                aQTS = Sim;
                aQTS.AQTSeg.SetMemLocRec();
                aQTS.AQTSeg.FileName = openFileDialog1.FileName;
                ButtonPanel.Visible = true;
                integrate.Visible = true;
                aQTS.ArchiveSimulation();
                ShowStudyInfo();

            }
        }

        private void saveJSON_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Test File|*.JSON";
            saveFileDialog1.Title = "Save to JSON File";
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

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(400, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Enter an ID for this simulation (optional)";

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 40, 23);
            textBox.Location = new System.Drawing.Point(25, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private void integrate_Click(object sender, EventArgs e)
        {
            string SimName = "";

            if (aQTS == null) { MessageBox.Show("No Simulation Loaded");  return; }
                else
                {
                //if (aQTS.Has_Chemicals())
                //{
                //    AQTCM = new(AQTChemicalModel(aQTS));
                //    string errmsg = AQTCM CheckDataRequirements();
                //    if (errmsg != "") textBox1.Text = errmsg;
                //}

                if (ShowInputDialog(ref SimName) == DialogResult.Cancel) return;
                if (SimName != "") SimName = SimName + ": ";
                aQTS.AQTSeg.RunID = SimName + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                progressBar1.Visible = true;
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
            // progressBar1.Visible = false;

            progressBar1.Update();
            if (e.Error != null) MessageBox.Show("Error Raised: " + e.Error.Message);
            else if (errmessage == "")
            {
                // textBox1.Text = "Run Completed.";
                Application.DoEvents();
                graph_Click(null, null);
                ShowStudyInfo();
                progressBar1.Visible = false;
            }
            else MessageBox.Show(errmessage);
        }


        // This event handler updates the progress bar
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100) progressBar1.Value = (e.ProgressPercentage + 1);  // workaround of animation bug
            progressBar1.Value = (e.ProgressPercentage);
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
            SetupForm.EditParams(ref SS, "Simulation Setup", true);

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
            PF3.EditParams(ref PA3, "Diagenesis Parameters", false);
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

            if (nplt == 0) return;

            PlantRecord PR = PlantDB[0]; PR.Setup();
            TParameter[] PPS = PR.InputArray();

            DataTable table = ParmArray_to_Table("PlantGrid", PPS);
            for (int r = 0; r < PlantDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, PlantDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            gf.ShowGrid(table);
        }

        private void ShowStudyInfo()
        {
            if (aQTS == null) return;

            this.Text = aQTS.AQTSeg.FileName;
            StudyNameBox.Text = aQTS.AQTSeg.StudyName;
            outputbutton.Visible = aQTS.HasResults();
            Diagenesis.Enabled = aQTS.AQTSeg.Diagenesis_Included();
            ChemButton.Enabled = aQTS.AQTSeg.Has_Chemicals();

            if (!aQTS.HasResults()) RunStatusLabel.Text = "No Saved Runs";
                else RunStatusLabel.Text = aQTS.SavedRuns.Count + " Archived Results";

            aQTS.AQTSeg.DisplayNames(ref SVList, ref TSVList);
            SVListBox.Visible = true;
            SVListBox.DataSource = null;
            SVListBox.DataSource = SVList;

            Application.DoEvents();
        }

        private void Chems(object sender, EventArgs e)
        {
            string[] Chemnames = new string[20];
            int[] cindices = new int[20];
            Param_Form[] Chemforms = new Param_Form[20];

            if (aQTS == null) return;

            int nchm = 0;
            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                if (TSV.NState == AllVariables.H2OTox)
                {
                    TToxics TC = TSV as TToxics;
                    Chemforms[nchm] = new Param_Form();

                    ChemicalRecord CR = TC.ChemRec;  CR.Setup();
                    TParameter[] PPS = CR.InputArray();

                    Chemforms[nchm].EditParams(ref PPS, "Chem Parameters", false);
                    nchm++;
                }
        }

        private void Sites(object sender, EventArgs e)
        {
            Param_Form Siteform = new Param_Form();

            if (aQTS == null) return;

            SiteRecord SR = aQTS.AQTSeg.Location.Locale;
            SR.Setup();
            TParameter[] PPS = SR.InputArray(); 

            Siteform.EditParams(ref PPS, "Site Parameters", false);
        }

        private void Remin(object sender, EventArgs e)
        {
            Param_Form Reminform = new Param_Form();

            if (aQTS == null) return;

            ReminRecord RR = aQTS.AQTSeg.Location.Remin;
            RR.Setup(); 
            TParameter[] PPS = RR.InputArray();

            Reminform.EditParams(ref PPS, "Remineralization Parameters", false);
        }


        private void AddColumn(ref TParameter Param, DataTable table)
        {
            DataColumn column;

            if (Param is TSubheading) return;

            column = new DataColumn();
            column.ColumnName = Param.Symbol;
            if (table.Columns.Count==0) column.Unique = true;
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

        int ColNum;

        private void AddCell(ref TParameter Param, DataRow row)
        {
            if (Param is TSubheading) return;

            if (Param.GetType() == typeof(TParameter))
            {   row[ColNum] = Param.Val;
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
            ColNum = 0;
            DataRow row = table.NewRow();
            for (int i = 0; i < arr.Length; i++)
                  AddCell(ref arr[i], row);
            table.Rows.Add(row);
            return true;
        }

        private TParameter[] Table_to_ParmArray(DataTable table)
        {
            TParameter[] arr = new TParameter[table.Rows.Count];
            ColNum = 0;
            DataRow row = table.NewRow();
            for (int i = 0; i < arr.Length; i++)
                AddCell(ref arr[i], row);  //workhere
            table.Rows.Add(row);
            return arr;
        }


        private void AnimDB_Click(object sender, EventArgs e)
        {
            string json = File.ReadAllText("..\\..\\..\\DB\\AnimalLib.JSON");
            List<AnimalRecord> AnimDB = JsonConvert.DeserializeObject<List<AnimalRecord>>(json);

            AnimalRecord AIR = AnimDB[0];  AIR.Setup();
            TParameter[] PPS = AIR.InputArray();
            
            DataTable table = ParmArray_to_Table("AnimalGrid", PPS);

            for (int r = 0; r < AnimDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, AnimDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            gf.ShowGrid(table); 

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

            if (nanm == 0) return;

            AnimalRecord AIR = AnimDB[0]; AIR.Setup();
            TParameter[] PPS = AIR.InputArray();

            DataTable table = ParmArray_to_Table("AnimalGrid", PPS);
                        for (int r = 0; r < AnimDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, AnimDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            gf.ShowGrid(table);
        }

        private void ReminDB_Click(object sender, EventArgs e)
        {
                string json = File.ReadAllText("..\\..\\..\\DB\\ReminLib.JSON");
                List<ReminRecord> ReminDB = JsonConvert.DeserializeObject<List<ReminRecord>>(json);

                ReminRecord RR = ReminDB[0]; RR.Setup();
                TParameter[] PPS = RR.InputArray();

                DataTable table = ParmArray_to_Table("ReminGrid", PPS);

                for (int r = 0; r < ReminDB.Count; r++)
                {
                    ParmArray_to_Table_Row(ref table, ReminDB[r].InputArray());
                }

                GridForm gf = new GridForm();
                gf.ShowGrid(table);

        }

        private void ChemDB_Click(object sender, EventArgs e)
        {
            string json = File.ReadAllText("..\\..\\..\\DB\\ChemLib.JSON");
            List<ChemicalRecord> ChemDB = JsonConvert.DeserializeObject<List<ChemicalRecord>>(json);

            ChemicalRecord CR = ChemDB[0]; CR.Setup();
            TParameter[] PPS = CR.InputArray();

            DataTable table = ParmArray_to_Table("ChemGrid", PPS);

            for (int r = 0; r < ChemDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, ChemDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            gf.ShowGrid(table);

        }

        private void SiteDB_Click(object sender, EventArgs e)
        {



            string fileN = Path.GetFullPath("..\\..\\..\\DB\\SiteLib.JSON");
            if (MessageBox.Show("Open the default database: '" + fileN + "'?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Text File|*.txt;*.json";
                openFileDialog1.Title = "Open a JSON File";
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
                fileN = openFileDialog1.FileName;
            }


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
            if (gf.ShowGrid(table))
            {
                if (gf.gridChange)
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "JSON files (*.JSON)|*.JSON|All files (*.*)|*.*";
                    saveFileDialog1.FilterIndex = 1;
                    saveFileDialog1.RestoreDirectory = true;
                    saveFileDialog1.OverwritePrompt = true;

                    // SiteDB = Table_to_ParmArray(table); workhere

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        json = JsonConvert.SerializeObject(SiteDB);
                        File.WriteAllText(saveFileDialog1.FileName, json);
                    }
                }
            }
        }

        private void PlantsDB_Click(object sender, EventArgs e)
        {
            string json = File.ReadAllText("..\\..\\..\\DB\\PlantLib.JSON");
            List<PlantRecord> PlantDB = JsonConvert.DeserializeObject<List<PlantRecord>>(json);

            PlantRecord PR = PlantDB[0]; PR.Setup();
            TParameter[] PPS = PR.InputArray();

            DataTable table = ParmArray_to_Table("PlantGrid", PPS);
            for (int r = 0; r < PlantDB.Count; r++)
            {
                ParmArray_to_Table_Row(ref table, PlantDB[r].InputArray());
            }

            GridForm gf = new GridForm();
            gf.ShowGrid(table);

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
            LF.EditSV(TSV, aQTS);

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

            StreamWriter file = new StreamWriter("N:\\AQUATOX\\CSRA\\"+ fieldname+".csv");
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

            MessageBox.Show("Exported N:\\AQUATOX\\CSRA\\"+ fieldname+".csv");

        }



        private void EditButton_Click(object sender, EventArgs e)
        {
            SVListBox_DoubleClick(sender, e);
        }

        private void FoodWebButton_Click(object sender, EventArgs e)
        {
            DataTable table = aQTS.AQTSeg.TrophInt_to_Table();
            TrophMatrix tm = new TrophMatrix();
            tm.ShowGrid(table);

        }

        private void MultiSegButton_Click(object sender, EventArgs e)
        {
            MultiSegForm MSForm = new MultiSegForm();
            MSForm.ShowDialog();

        }
    }
}
