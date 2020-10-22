using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using AQUATOX.AQTSegment;
using Globals;
using Data;
using System.ComponentModel;
using System.Collections.Generic;
using AQUATOX.Diagenesis;
using AQUATOX.Plants;
using AQUATOX.Animals;
using AQUATOX.Chemicals;
using AQUATOX.AQSite;

namespace GUI.AQUATOX
{
    public partial class AQTTestForm : Form
    {
        private Chart chart1 = new Chart();
        private BackgroundWorker Worker = new BackgroundWorker();
        private string errmessage;

        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        Series series1 = new Series();

        public AQTSim aQTS = null;

        public AQTTestForm()
        {
            InitializeComponent();
            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunCompleted);
            Worker.WorkerReportsProgress = true;

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            SuspendLayout();

            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);

            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(31, 250);
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(809, 233);
            chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            chart1.Series.Clear();
            this.chart1.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart1_CustomizeLegend_1);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);

            Controls.Add(chart1);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void AQTTestForm_Load(object sender, EventArgs e)
        {

        }

        public void DisplaySVs()

        {
            TStateVariable TSV1 = null;
            bool SuppressText = true;

            //outtxt = outtxt + "Times: ";
            //foreach (DateTime Times in aQTS.AQTSeg.SV.restimes)
            //    outtxt = outtxt + Times + ", ";
            //outtxt = outtxt + Environment.NewLine;

            chart1.Series.Clear();
            int sercnt = 0;
            string outtxt = "";

            foreach (TStateVariable TSV in aQTS.AQTSeg.SV) if (TSV.SVoutput != null)
                {
                    TSV1 = TSV; // identify TSV1 with an output that is not null
                    int cnt = 0;
                    if (sercnt == 0) outtxt = "Date, ";

                    List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                    for (int col = 1; col <= vallist.Count(); col++)
                    {
                        string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
                             TSV.SVoutput.Metadata["Name_" + col.ToString()] +
                             " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";
                        Series ser = chart1.Series.Add(sertxt);

                        if (col == 1) outtxt = outtxt + sertxt.Replace(",", "") + ", ";  // suppress commas in name for CSV output

                        ser.ChartType = SeriesChartType.Line;
                        ser.BorderWidth = 2;
                        ser.MarkerStyle = MarkerStyle.Diamond;
                        ser.Enabled = false;
                        sercnt++;

                        SuppressText = (TSV.SVoutput.Data.Keys.Count > 5000);
                        for (int i = 0; i < TSV.SVoutput.Data.Keys.Count; i++)
                        {
                            ITimeSeriesOutput ito = TSV.SVoutput;
                            string datestr = ito.Data.Keys.ElementAt(i).ToString();
                            Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[col - 1]);
                            ser.Points.AddXY(Convert.ToDateTime(datestr), Val);
                            cnt++;
                        }
                    }
                }

            outtxt = outtxt + Environment.NewLine;

            if (!SuppressText)
            {
                for (int i = 0; i < TSV1.SVoutput.Data.Keys.Count; i++)
                {
                    bool writedate = true;
                    foreach (TStateVariable TSV in aQTS.AQTSeg.SV) if (TSV.SVoutput != null)
                        {
                            ITimeSeriesOutput ito = TSV.SVoutput;
                            if (writedate)
                            {
                                string datestr = ito.Data.Keys.ElementAt(i).ToString();
                                outtxt = outtxt + datestr + ", ";
                                writedate = false;
                            }
                            Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                            outtxt = outtxt + Val.ToString() + ", ";
                        }
                    outtxt = outtxt + Environment.NewLine;
                }
            }

            textBox1.Text = outtxt;
        }


        private void loadJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Test File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                // FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                string json = File.ReadAllText(openFileDialog1.FileName);
                AQTSim Sim = new AQTSim();
                string err = Sim.Instantiate(json);
                aQTS = Sim;
                aQTS.AQTSeg.SetMemLocRec();
                ParamsButton.Visible = true;
                Diagenesis.Visible = aQTS.AQTSeg.Diagenesis_Included();
                PlantsButton.Visible = true;
                AnimButton.Visible = true;  
                ChemButton.Visible = true;
                SiteButton.Visible = true;

                if (err == "") textBox1.Text = "Read File " + openFileDialog1.FileName;
                else textBox1.Text = err;
            }
        }

        private void saveJSON_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Test File|*.txt";
            saveFileDialog1.Title = "Save to JSON File";
            saveFileDialog1.ShowDialog();


            if (saveFileDialog1.FileName != "")
            {
                string jsondata = "";
                string errmessage = aQTS.SaveJSON(ref jsondata);
                if (errmessage == "") File.WriteAllText(saveFileDialog1.FileName, jsondata);
                else textBox1.Text = errmessage;
            }
        }

        private void integrate_Click(object sender, EventArgs e)
        {

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
            if (e.Error != null) textBox1.Text = "Error Raised: " + e.Error.Message;
            else if (errmessage == "")
            {
                textBox1.Text = "Run Completed.  Please wait one moment -- writing and plotting results";
                Application.DoEvents();
                DisplaySVs();
                progressBar1.Visible = false;
            }
            else textBox1.Text = errmessage;
        }


        // This event handler updates the progress bar
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 100) progressBar1.Value = (e.ProgressPercentage + 1);  // workaround of animation bug
            progressBar1.Value = (e.ProgressPercentage);
        }


        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            HitTestResult resultExplode = chart1.HitTest(e.X, e.Y);
            if (resultExplode.Series != null)

            {
                if (resultExplode.Object is LegendItem)
                {
                    LegendItem legendItem = (LegendItem)resultExplode.Object;
                    Series selectedSeries = (Series)legendItem.Tag;
                    if (selectedSeries != null)
                        selectedSeries.Enabled = !selectedSeries.Enabled;
                    chart1.ChartAreas[0].RecalculateAxesScale();
                }

                string msgstr = resultExplode.Series.Name;
                if (resultExplode.PointIndex > 0)
                {
                    msgstr = msgstr + ": " +
                    resultExplode.Series.Points[resultExplode.PointIndex].YValues[0] + " \n " +
                    System.DateTime.FromOADate(resultExplode.Series.Points[resultExplode.PointIndex].XValue);
                    System.Windows.Forms.MessageBox.Show(msgstr);
                }

            }

        }

        private void chart1_CustomizeLegend_1(object sender, CustomizeLegendEventArgs e)
        {

            e.LegendItems.Clear();
            foreach (var series in this.chart1.Series)
            {
                var legendItem = new LegendItem();
                legendItem.SeriesName = series.Name;
                legendItem.ImageStyle = LegendImageStyle.Rectangle;
                legendItem.BorderColor = Color.Transparent;
                legendItem.Name = series.Name + "_legend_item";

                int i = legendItem.Cells.Add(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleCenter);
                legendItem.Cells.Add(LegendCellType.Text, series.Name, ContentAlignment.MiddleCenter);

                if (series.Enabled)
                { legendItem.Color = series.Color; legendItem.BorderColor = Color.Black; }
                else
                { legendItem.Color = Color.FromArgb(100, series.Color); legendItem.BorderColor = Color.White; }

                legendItem.Tag = series;
                e.LegendItems.Add(legendItem);
            }

        }

        private void graph_Click(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            textBox1.Text = "Run Completed.  Please wait one moment -- writing and plotting results";
            Application.DoEvents();
            DisplaySVs();
        }



        private void SaveParams(object sender, EventArgs e)
        {
            if (aQTS == null) return;

            Setup_Record SR = aQTS.AQTSeg.PSetup;
            SR.Setup(false);
            TParameter[] SS = new TParameter[] {new TSubheading ("Timestep Settings"), SR.FirstDay,SR.LastDay,SR.RelativeError,SR.UseFixStepSize,SR.FixStepSize,
                SR.ModelTSDays, new TSubheading("Output Storage Options"), SR.StepSizeInDays, SR.StoreStepSize, SR.AverageOutput, SR.SaveBRates,
                new TSubheading("Biota Modeling Options"),SR.Internal_Nutrients,SR.NFix_UseRatio,SR.NtoPRatio,
                new TSubheading("Chemical Options"),SR.ChemsDrivingVars,SR.TSedDetrIsDriving,SR.UseExternalConcs,SR.T1IsAggregate };

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
            TParameter[] PA3 = new TParameter[] { new TSubheading("Diagenesis Parameters"), DR.m1, DR.m2,DR.H1,DR.Dd,DR.w2,DR.H2,DR.KappaNH3f,DR.KappaNH3s,DR.KappaNO3_1f,DR.KappaNO3_1s,DR.KappaNO3_2,
                DR.KappaCH4,DR.KM_NH3,DR.KM_O2_NH3,DR.KdNH3,DR.KdPO42,DR.dKDPO41f,DR.dKDPO41s,DR.O2critPO4,
                DR.ThtaDd,DR.ThtaNH3,DR.ThtaNO3,DR.ThtaCH4,DR.SALTSW,DR.SALTND,DR.KappaH2Sd1,DR.KappaH2Sp1,DR.ThtaH2S,DR.KMHSO2,DR.KdH2S1,
                DR.KdH2S2,DR.kpon1,DR.kpon2, DR.kpon3,DR.kpoc1,DR.kpoc2,DR.kpoc3,DR.kpop1,DR.kpop2,DR.kpop3,DR.ThtaPON1,DR.ThtaPON2,
                DR.ThtaPON3,DR.ThtaPOC1,DR.ThtaPOC2,DR.ThtaPOC3, DR.ThtaPOP1,DR.ThtaPOP2,DR.ThtaPOP3,DR.kBEN_STR,DR.ksi,DR.ThtaSi,DR.KMPSi,
                DR.SiSat,DR.KDSi2,DR.DKDSi1,DR.O2critSi,DR.LigninDetr, DR.Si_Diatom   };

            Param_Form PF3 = new Param_Form();
            PF3.EditParams(ref PA3, "Diagenesis Parameters", false);

        }

        private void Plants(object sender, EventArgs e)
        {
            string[] plantnames = new string[20];
            int[] pindices = new int[20];
            Param_Form[] plantforms = new Param_Form[20];

            if (aQTS == null) return;

            int nplt = 0;
            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                if (TSV.IsPlant())
                {
                    TPlant TP = TSV as TPlant;
                    plantforms[nplt] = new Param_Form();

                    PlantRecord PIR = TP.PAlgalRec;
                    PIR.Setup();

                    TParameter[] PPS = new TParameter[] {new TSubheading("Plant Parameters for "+PIR.PlantName.Val), PIR.PlantName,
                        PIR.ScientificName, PIR.PlantType,PIR.SurfaceFloating,
                        PIR.Taxonomic_Type, PIR.ToxicityRecord, PIR.EnteredLightSat,
                        PIR.UseAdaptiveLight, PIR.MaxLightSat, PIR.MinLightSat, PIR.KPO4, PIR.KN,
                        PIR.KCarbon, PIR.Q10, PIR.TOpt, PIR.TMax, PIR.TRef, PIR.PMax, PIR.KResp, PIR.Resp20, PIR.KMort, PIR.EMort,
                        PIR.P2OrgInit, PIR.N2OrgInit, PIR.ECoeffPhyto, PIR.Wet2Dry, PIR.PlantFracLipid,
                        new TSubheading("Internal Nutrients Parameters:"), PIR.NHalfSatInternal, PIR.PHalfSatInternal, PIR.MaxNUptake,
                        PIR.MaxPUptake,  PIR.Min_N_Ratio, PIR.Min_P_Ratio,
                        new TSubheading("Phytoplankton Only:"), PIR.Plant_to_Chla, PIR.KSed1, PIR.KSedTemp, PIR.KSedSalinity, PIR.ESed,
                        new TSubheading("Periphyton and Macrophytes Only:"),  PIR.Macrophyte_Type, PIR.CarryCapac, PIR.Macro_VelMax, PIR.Red_Still_Water, PIR.FCrit,
                        new TSubheading("If in Stream:"),   PIR.PctSloughed, PIR.PrefRiffle, PIR.PrefPool,
                        new TSubheading("Salinity Effects:"),  PIR.SalMin_Phot, PIR.SalMax_Phot, PIR.SalCoeff1_Phot, PIR.SalCoeff2_Phot, PIR.SalMin_Mort, PIR.SalMax_Mort, PIR.SalCoeff1_Mort, PIR.SalCoeff2_Mort };

                    plantforms[nplt].EditParams(ref PPS, "Plant Parameters", false);
                    nplt++;
                }
        }

            private void AnimButton_Click(object sender, EventArgs e)
        {
            string[] animnames = new string[20];
            int[] aindices = new int[20];
            Param_Form[] animforms = new Param_Form[20];

            if (aQTS == null) return;

            int nanm = 0;
            foreach (TStateVariable TSV in aQTS.AQTSeg.SV)
                if (TSV.IsAnimal())
                {
                    TAnimal TA = TSV as TAnimal;
                    animforms[nanm] = new Param_Form();

                    AnimalRecord AIR = TA.PAnimalData;
                    AIR.Setup();

                    TParameter[] PPS = new TParameter[] {new TSubheading("Animal Parameters for "+AIR.AnimalName.Val),
                                AIR.AnimalName, AIR.ScientificName, AIR.Animal_Type,AIR.Guild_Taxa,AIR.ToxicityRecord,AIR.BenthicDesignation,
                                new TSubheading("Feeding"),AIR.FHalfSat,AIR.CMax,AIR.BMin,AIR.Sorting,AIR.Burrow_Index,AIR.CanSeekRefuge,AIR.Visual_Feeder,AIR.SuspSedFeeding,AIR.SlopeSSFeed,AIR.InterceptSSFeed,
                                new TSubheading("Temperature, Simple Bioenergetics, Stoichiometry "),AIR.Q10,AIR.TOpt,AIR.TMax,AIR.TRef,AIR.MeanWeight,AIR.EndogResp,AIR.KResp,AIR.KExcr,AIR.N2Org,
                                AIR.P2Org,AIR.Wet2Dry,AIR.PctGamete,AIR.GMort,AIR.KMort,
                                new TSubheading("Life History Parameters"),AIR.SensToSediment,AIR.SenstoPctEmbed,AIR.PctEmbedThreshold,AIR.KCap,AIR.AveDrift,AIR.Trigger,AIR.FracInWaterCol,AIR.VelMax,
                                AIR.Fishing_Frac,AIR.LifeSpan,AIR.FishFracLipid,
                                new TSubheading("Ammonia/Low-Oxygen Effects"),AIR.O2_LethalConc,AIR.O2_LethalPct,AIR.O2_EC50growth,AIR.O2_EC50repro,AIR.Ammonia_LC50,
                                new TSubheading("Salinity Effects Effects"),AIR.SalMin_Ing,AIR.SalMax_Ing ,AIR.SalCoeff1_Ing,AIR.SalCoeff2_Ing,AIR.SalMin_Gam,AIR.SalMax_Gam ,AIR.SalCoeff1_Gam,AIR.SalCoeff2_Gam,
                                AIR.SalMin_Rsp,AIR.SalMax_Rsp ,AIR.SalCoeff1_Rsp,AIR.SalCoeff2_Rsp,AIR.SalMin_Mort,AIR.SalMax_Mort ,AIR.SalCoeff1_Mort,AIR.SalCoeff2_Mort,
                                new TSubheading("Stream Habitat Parameters"),AIR.PrefRiffle,AIR.PrefPool,
                                new TSubheading("Spawning"),AIR.AutoSpawn,AIR.SpawnDate1,AIR.SpawnDate2,AIR.SpawnDate3,AIR.UnlimitedSpawning,AIR.SpawnLimit,
                                new TSubheading("Allometric Consumption and Respiration"),AIR.UseAllom_C,AIR.CA,AIR.CB,AIR.UseAllom_R,AIR.RA,AIR.RB,AIR.UseSet1,AIR.RQ,AIR.RTL,AIR.ACT,AIR.RTO,AIR.RK1,AIR.BACT,
                                AIR.RTM,AIR.RK4,AIR.ACT,
                    };

                    animforms[nanm].EditParams(ref PPS, "Animal Parameters", false);
                    nanm++;
                }
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

                    ChemicalRecord CR = TC.ChemRec;
                    CR.Setup();

                    TParameter[] PPS = new TParameter[] {new TSubheading("Chemical Parameters for "+CR.ChemName.Val),
                        CR.ChemName, CR.CASRegNo, CR.BCFUptake, CR.MolWt, CR.pka, CR.Henry, CR.LogKow, CR.ChemIsBase,
                        new TSubheading("Partitioning to Sediment/Organic Matter"),
                        CR.CalcKPSed,CR.KPSed, CR.CalcKOMRefrDOM, CR.KOMRefrDOM, CR.K1Detritus,
                        new TSubheading("Chemical Fate Parameters"),CR.ActEn, CR.KMDegrAnaerobic, CR.KMDegrdn, CR.KUnCat,
                        CR.KAcid, CR.KBase, CR.PhotolysisRate, new TSubheading("Chemical Toxicity Parameters"), CR.Weibull_Shape, CR.WeibullSlopeFactor};

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

            TParameter[] PPS = new TParameter[] {new TSubheading("Site Parameters"),
                            SR.SiteName,
                            SR.SiteLength,
                            SR.Volume,
                            SR.Min_Vol_Frac,
                            new TSubheading("Shape, Temperature, Light, Reaeration"),
                            SR.UseBathymetry,
                            SR.ICZMean,
                            SR.ZMax,
                            SR.TempMean,
                            SR.TempRange,
                            SR.Latitude,
                            SR.Altitude,
                            SR.LightMean,
                            SR.LightRange,
                            SR.EnclWallArea,
                            SR.MeanEvap,
                            SR.UseCovar,
                            SR.EnteredKReaer,
                            new TSubheading("Water Clarify Parameters"),
                            SR.ECoeffWater,
                            SR.ECoeffSed,
                            SR.ECoeffDOM,
                            SR.ECoeffPOM,
                            SR.BasePercentEmbed,
                            new TSubheading("Phytoplankton/Zooplankton Retention"),
                            SR.UsePhytoRetention,
                            SR.EnterTotalLength,
                            SR.TotalLength,
                            SR.WaterShedArea,
                            new TSubheading("Refuge"),
                            SR.FractalD,
                            SR.FD_Refuge_Coeff,
                            SR.HalfSatOysterRefuge,
                            new TSubheading("Stream Parameters"),
                            SR.Channel_Slope,
                            SR.UseEnteredManning,
                            SR.EnteredManning,
                            SR.StreamType,
                            SR.PctRiffle,
                            SR.PctPool
                };

            Siteform.EditParams(ref PPS, "Site Parameters", false);
        }
    }
}
