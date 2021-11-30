using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using AQUATOX.OrgMatter;
using AQUATOX.Volume;
using Data;
using Globals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
//using Web.Services.Controllers;

namespace GUI.AQUATOX
{
    public partial class MultiSegForm : Form
    {
        // private BackgroundWorker Worker = new BackgroundWorker();  potentially to be added, to report progress
        private AQSim_2D AQT2D = null;

        private Chart chart1 = new Chart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        DataTable OverlandTable = null;
        private System.Drawing.Graphics graphics;

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

        public MultiSegForm()
        {
            AutoScroll = true;
            InitializeComponent();
            //Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            //Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            //Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunCompleted);
            //Worker.WorkerReportsProgress = true;

            // 
            // chart1
            // 
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            SuspendLayout();
            graphics = this.CreateGraphics();

            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea1);

            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            chart1.Location = new System.Drawing.Point(ScaleX(306), ScaleY(59));
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(ScaleX(520), ScaleY(405));
            chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            chart1.Series.Clear();
            chart1.Visible = false;
            this.chart1.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart1_CustomizeLegend_1);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);

            Controls.Add(chart1);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            // 
            // end chart1
            // 

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void chart1_MouseDown(object sender, MouseEventArgs e)  // display value from chart
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

        private void chart1_CustomizeLegend_1(object sender, CustomizeLegendEventArgs e) // legend info for chart
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


        private void cancel_click(object sender, EventArgs e)
        {
            this.Close();

            //if (MessageBox.Show("Cancel any changes to inputs?", "Confirm",
            //     MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            //     MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            //{
            //    // UserCanceled = true;
            //    this.Close();
            //}
        }

        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddToProcessLog(string msg)
        {
            ProcessLog.AppendText(msg + Environment.NewLine);
            Application.DoEvents();
        }

        private void TSafeAddToProcessLog(string msg)  //thread safe addition to progress log
        {

            if (InvokeRequired)
                ProcessLog.BeginInvoke((MethodInvoker)delegate ()
                {
                    ProcessLog.AppendText(msg + Environment.NewLine);
                });
            else AddToProcessLog(msg);
        }

        private void ReadNetwork_Click(object sender, EventArgs e) // initializes the AQT2D object, reads the stream network from web services, saves the stream network object
        {
            if (AQT2D == null) AQT2D = new AQSim_2D();

            AddToProcessLog("Please wait, reading stream network from web service");
            string SNJSON = AQT2D.ReadStreamNetwork(comidBox.Text, EndCOMIDBox.Text, spanBox.Text);
            if (SNJSON == "")
            {
                AddToProcessLog("Error: web service returned empty JSON."); return;
            }
            if (SNJSON.IndexOf("ERROR") >= 0)
            {
                AddToProcessLog("Error: web service returned: " + SNJSON); return;
            }
            try
            { AQT2D.CreateStreamNetwork(SNJSON); }
            catch
            { AddToProcessLog("Error converting JSON:" + SNJSON); return; }

            AddToProcessLog("System has " + AQT2D.nSegs.ToString() + " segments");
            string BaseFileN = BaseJSONBox.Text;
            AddToProcessLog(" Basefile = " + BaseFileN);

            string BaseDir = basedirBox.Text;
            AddToProcessLog(" BaseDir = " + BaseDir);

            File.WriteAllText(BaseDir + "StreamNetwork.JSON", SNJSON);
            AddToProcessLog("Finished reading stream network" + Environment.NewLine);

        }

        private bool VerifyStreamNetwork()
        {
            string BaseDir = basedirBox.Text;
            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.SN == null)
            {
                try
                {
                    if (!ValidFilen(BaseDir + "StreamNetwork.JSON"))
                    {
                        AddToProcessLog("Cannot find stream network file " + BaseDir + "StreamNetwork.JSON");
                        return false;
                    }
                    string SNJSON = System.IO.File.ReadAllText(BaseDir + "StreamNetwork.JSON", Encoding.Default);
                    AQT2D.CreateStreamNetwork(SNJSON);
                }
                catch
                {
                    AddToProcessLog("Cannot process stream network file " + BaseDir + "StreamNetwork.JSON");
                    return false;
                }
            }
            return true;
        }


        private void createButton_Click(object sender, EventArgs e)  //create a set of 0D jsons for stream network given AQT2D "SN" object
        {
            chart1.Visible = false;
            string BaseDir = basedirBox.Text;

            if (!VerifyStreamNetwork()) return;

            AddToProcessLog("Please wait, creating individual AQUATOX JSONS for each segment and reading flow data." + Environment.NewLine);

            string BaseFileN = BaseJSONBox.Text;
            if (!ValidFilen(BaseJSONBox.Text)) return;
            AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
            string msj = MasterSetupJson();

//          Parallel.For(1, AQT2D.nSegs+1 , iSeg =>  
            for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
               {
                  string comid = AQT2D.SN.network[iSeg][0];
                  string filen = BaseDir + "AQT_2D_" + comid + ".JSON";
                 
                  string errmessage = AQT2D.PopulateStreamNetwork(iSeg, msj, out string jsondata);

                  if (errmessage == "")
                  {
                      File.WriteAllText(filen, jsondata);
                      TSafeAddToProcessLog("Read Flows and Saved JSON for " + comid);
                  }
                  else
                  {
                      TSafeAddToProcessLog(errmessage);
                      return;
                  }
               }

            AddToProcessLog("");
            if (AQT2D.SN.sources.TryGetValue("boundaries", out int[] boundaries))
            {
                string bnote = "Note: Boundary Condition Flows and State Variable upriver inputs should be added to COMIDs: ";
                foreach (long bid in boundaries) bnote = bnote + bid.ToString() + ", ";
                AddToProcessLog(bnote);
            }

            AddToProcessLog("Finished creating linked inputs" + Environment.NewLine);

        }


        private void executeButton_Click(object sender, EventArgs e)  //execute the full model run given initialized AQT2D
        {
            chart1.Visible = false;

            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.SN == null)
            {
                string BaseDir = basedirBox.Text;
                if (!ValidFilen(BaseDir + "StreamNetwork.JSON")) return;
                string SNJSON = System.IO.File.ReadAllText(BaseDir + "StreamNetwork.JSON", Encoding.Default);  //read stored streamnetwork (SN object) if necessary
                AQT2D.SN = JsonConvert.DeserializeObject<AQSim_2D.streamNetwork>(SNJSON);
                AddToProcessLog("Read stream network from " + BaseDir + "StreamNetwork.JSON");
            }

            AddToProcessLog("Starting model execution...");
            AQT2D.archive.Clear();
            for (int ordr = 0; ordr < AQT2D.SN.order.Length; ordr++)
            {
                Parallel.ForEach(AQT2D.SN.order[ordr], runID =>
                {
                    string strout = "";
                    string BaseDir = basedirBox.Text;
                    string FileN = BaseDir + "AQT_2D_" + runID.ToString() + ".JSON";
                    if (!ValidFilen(FileN)) { AddToProcessLog("Error File Missing " + FileN); return; }
                    string json = File.ReadAllText(BaseDir + "AQT_2D_" + runID.ToString() + ".JSON");  //read one segment of 2D model
                    if (AQT2D.executeModel(runID, MasterSetupJson(), ref strout, ref json))   //run one segment of 2D model
                         File.WriteAllText(BaseDir + "AQT_Run_" + runID.ToString() + ".JSON", json);
                    TSafeAddToProcessLog(strout);  //write update to status log
                 });
                Application.DoEvents();
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = null;
            bs.DataSource = AQT2D.SVList;
            SVBox.DataSource = bs;   //update state variable list for graphing, CSV

            OutputPanel.Enabled = true;

            AddToProcessLog("Model execution complete");
        }

        private void CSVButton_Click(object sender, EventArgs e)  //write CSV output from archived results given selected state variable in dropdown box
        {
            chart1.Hide();

            if (AQT2D == null) return;
            if (AQT2D.archive == null) return;

            int SVIndex = SVBox.SelectedIndex;
            string csv = "";

            int NDates = 0;
            int[] comids = new int[AQT2D.archive.Count];
            int i = 0;

            foreach (KeyValuePair<int, AQSim_2D.archived_results> entry in AQT2D.archive)
            {
                csv += (entry.Key) + ",";
                if (i == 0) NDates = entry.Value.dates.Count();
                comids[i] = entry.Key;
                i++;
            }
            csv += Environment.NewLine;

            for (i = 0; i < NDates; i++)
            {
                for (int j = 0; j < AQT2D.archive.Count; j++)
                {
                    AQT2D.archive.TryGetValue(comids[j], out AQSim_2D.archived_results val);
                    csv += (val.concs[SVIndex][i]) + ",";
                }
                csv += Environment.NewLine;
            }

            ProcessLog.Text = csv;
        }

        private void ChartButtonClick(object sender, EventArgs e)   //produce graphics from archived results given selected state variable in dropdown box
        {
            if (AQT2D == null) return;

            try
            {
                int SVIndex = SVBox.SelectedIndex;

                chart1.Series.Clear();
                chart1.BringToFront();
                int sercnt = 0;

                foreach (KeyValuePair<int, AQSim_2D.archived_results> entry in AQT2D.archive)
                {
                    Series ser = chart1.Series.Add(entry.Key.ToString());
                    ser.ChartType = SeriesChartType.Line;
                    ser.BorderWidth = 2;
                    ser.MarkerStyle = MarkerStyle.Diamond;
                    ser.Enabled = true;
                    sercnt++;

                    for (int i = 0; i < entry.Value.dates.Count(); i++)
                    {
                        ser.Points.AddXY(entry.Value.dates[i], entry.Value.concs[SVIndex][i]);
                    }
                }

                chart1.Visible = true;
            }
            catch (Exception ex)
            {
                ProcessLog.Text = "Error rendering chart: " + ex.Message;
                chart1.Visible = false;
            }
        }

        private bool ValidFilen(string filen)
        {
            if (!File.Exists(filen)) { MessageBox.Show("Cannot find file "+Path.GetFullPath(filen)); return false; }
            return true;
        }

        private AQTSim InstantiateBaseJSON()
        {
            if (!ValidFilen(BaseJSONBox.Text)) { MessageBox.Show("Specify Valid File in Base JSON"); return null; }
            string json = File.ReadAllText(BaseJSONBox.Text);

            AQTSim Sim = new AQTSim();
            string err = Sim.Instantiate(json);
            if (err != "") { MessageBox.Show(err); return null; }
            return Sim;
        }

        private string MasterSetupJson()
        {
            string msfilen = basedirBox.Text + "MasterSetup.json";
            if (File.Exists(msfilen))
            {
                return File.ReadAllText(msfilen);
            }
            else  // create the master record from the base file
            {
                AQTSim Sim = InstantiateBaseJSON();
                if (Sim == null) return "";

                Setup_Record SR = Sim.AQTSeg.PSetup;
                string msr = JsonConvert.SerializeObject(SR);
                File.WriteAllText(msfilen, msr);
                AddToProcessLog("Wrote master setup record.  Copied from base JSON to " + msfilen);
                return msr;
            }

        }

        private void SetupButton_Click(object sender, EventArgs e)  //Open the master setup record for editing, save to "Mastersetup" json
        {
            try
            {
                chart1.Visible = false;
                string msj = MasterSetupJson();
                if (msj == "") return;
                Setup_Record SR = JsonConvert.DeserializeObject<Setup_Record>(msj);

                SR.Setup(false);
                TParameter[] SS = new TParameter[] {new TSubheading ("Timestep Settings",""), SR.FirstDay,SR.LastDay,SR.RelativeError,SR.UseFixStepSize,SR.FixStepSize,
                SR.ModelTSDays, new TSubheading("Output Storage Options",""), SR.StepSizeInDays, SR.StoreStepSize, SR.AverageOutput, SR.SaveBRates,
                new TSubheading("Biota Modeling Options",""),SR.NFix_UseRatio,SR.NtoPRatio,
                new TSubheading("Chemical Options",""),SR.ChemsDrivingVars,SR.TSedDetrIsDriving,SR.UseExternalConcs,SR.T1IsAggregate };
                Param_Form SetupForm = new Param_Form();
                SetupForm.SuppressComment = true;
                SetupForm.SuppressSymbol = true;
                SetupForm.EditParams(ref SS, "Simulation Setup", true, "SiteLib.JSON", "_Toc77252216");

                string msfilen = basedirBox.Text + "MasterSetup.json";
                string msr = JsonConvert.SerializeObject(SR);
                File.WriteAllText(msfilen, msr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SVBox_SelectedIndexChanged(object sender, EventArgs e)  //update chart when dropdown changes
        {
            ChartButtonClick(sender, e);
        }

        private void OverlandFlow_Click(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;

            AQTSim Sim = InstantiateBaseJSON();
            if (Sim == null) return;

            TStateVariable TNH4 = Sim.AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            TStateVariable TNO3 = Sim.AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            TStateVariable TTSP = Sim.AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            TStateVariable TOM = Sim.AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);

            bool HasN = (TNH4 != null);
            bool HasP = (TTSP != null);
            bool HasOM = (TOM != null);

            if (OverlandTable == null)
            {   // set up input table based on base JSON specification 

                OverlandTable = new DataTable();

                OverlandTable.Columns.Add(new DataColumn("COMID"));

                if (HasN)
                {
                    OverlandTable.Columns.Add(new DataColumn("Ammonia (g/d)", System.Type.GetType("System.Double")));
                    OverlandTable.Columns.Add(new DataColumn("Nitrate (g/d)", System.Type.GetType("System.Double")));
                }

                if (HasP)
                    OverlandTable.Columns.Add(new DataColumn("Phosphate (g/d)", System.Type.GetType("System.Double")));

                if (HasOM)
                    OverlandTable.Columns.Add(new DataColumn("Organic Matter (g/d)", System.Type.GetType("System.Double")));

                string BaseDir = basedirBox.Text;
                for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
                {
                    int ColNum = 0;
                    DataRow row = OverlandTable.NewRow();
                    string comid = AQT2D.SN.network[iSeg][0];
                    row[0] = comid;

                    string FileN = BaseDir + "AQT_2D_" + comid.ToString() + ".JSON";
                    if (!ValidFilen(FileN))
                    {
                        AddToProcessLog("Cannot Find File " + FileN + ".  Create Linked Inputs before specifying overland flows");
                        return;
                    }
                    string json = File.ReadAllText(FileN);

                    if (HasN)
                    {
                        row[ColNum + 1] = TNH4.LoadsRec.Alt_Loadings[2].ConstLoad;
                        ColNum++;
                        row[ColNum + 1] = TNO3.LoadsRec.Alt_Loadings[2].ConstLoad;
                        ColNum++;
                    }

                    if (HasP)
                    {
                        row[ColNum + 1] = TTSP.LoadsRec.Alt_Loadings[2].ConstLoad;
                        ColNum++;
                    }

                    if (HasOM)
                    {
                        row[ColNum + 1] = ((TDissRefrDetr)TOM).InputRecord.Load.Alt_Loadings[2].ConstLoad;
                        ColNum++;
                    }

                    OverlandTable.Rows.Add(row);
                }
            }

            GridForm gf = new GridForm();
            gf.Text = "Add Non-Point Source (Overland) Inputs";
            if (!gf.ShowGrid(OverlandTable,true,false, "")) return;

            for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
            {
                int ColNum = 0;
                DataRow row = OverlandTable.Rows[iSeg-1];
                string comid = AQT2D.SN.network[iSeg][0];
                string BaseDir = basedirBox.Text;
                string FileN = BaseDir + "AQT_2D_" + comid + ".JSON";
                if (!ValidFilen(FileN)) { AddToProcessLog("Error File Missing " + FileN); return; }

                string JSON = System.IO.File.ReadAllText(FileN);
                if (HasN)
                {
                    JSON = Sim.InsertLoadings(JSON, "TNH4Obj", 2, (double)row[ColNum+1], 1);
                    ColNum++;
                    JSON = Sim.InsertLoadings(JSON, "TNO3Obj", 2, (double)row[ColNum + 1], 1);
                    ColNum++;
                }

                if (HasP)
                {
                    JSON = Sim.InsertLoadings(JSON, "TPO4Obj", 2, (double)row[ColNum + 1], 1);
                    ColNum++;
                }

                if (HasOM)
                {
                    JSON = Sim.InsertLoadings(JSON, "TDissRefrDetr", 2, (double)row[ColNum + 1], 1);
                    ColNum++;
                }

                System.IO.File.WriteAllText(FileN, JSON);
                AddToProcessLog("Overwrote non point-source loadings with selected inputs for " + comid); 
            }
        }

        private void Choose_from_Template_Click(object sender, EventArgs e)
        {
            CheckboxForm CBF = new CheckboxForm();
            CBF.Text = "Select elements to include in template";
            List<bool> BoolList = CBF.SelectFromBoxes(AQSim_2D.MultiSegSimFlags());
            if (BoolList == null) return;

            string filen = AQSim_2D.MultiSegSimName(BoolList);

            BaseJSONBox.Text = "..\\..\\..\\2D_Inputs\\BaseJSON\\" + filen;
            MessageBox.Show("Selected Template File: " + filen);
        }

        private void basedirBox_Leave(object sender, EventArgs e)
        {
            if (!basedirBox.Text.EndsWith("\\")) basedirBox.Text = basedirBox.Text + "\\";
        }

        private void MultiSegForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.BaseJSON = BaseJSONBox.Text;
            Properties.Settings.Default.MS_Directory = basedirBox.Text;
            Properties.Settings.Default.COMID = comidBox.Text;
            Properties.Settings.Default.EndCOMID = EndCOMIDBox.Text;
            Properties.Settings.Default.UpSpan = spanBox.Text;

            Properties.Settings.Default.Save();
        }

        private void MultiSegForm_Shown(object sender, EventArgs e)
        {
            BaseJSONBox.Text = Properties.Settings.Default.BaseJSON;  // default is "..\..\..\2D_Inputs\LBR Glenwood 4.JSON"
            basedirBox.Text = Properties.Settings.Default.MS_Directory; // default is "..\..\..\2D_Inputs\TestDir1\"

            comidBox.Text = Properties.Settings.Default.COMID ;
            EndCOMIDBox.Text = Properties.Settings.Default.EndCOMID;
            spanBox.Text = Properties.Settings.Default.UpSpan;

        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string target = "_Toc77252249";
            AQTTestForm.OpenUrl(target);
        }


    }
}
