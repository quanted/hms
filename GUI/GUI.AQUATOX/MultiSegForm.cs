﻿using AQUATOX.AQSim_2D;
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
using System.Drawing.Drawing2D;
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
        private bool DrawMap = true;

        static float xmax = -1000000;
        static float ymax = -1000000;
        static float xmin = 1000000;
        static float ymin = 1000000;
        Graphics MPG;
        int xBuffer;
        int yBuffer;


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

            this.MapPanel.MouseWheel += MapPanel_MouseWheel;


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
            chart1.Location = new System.Drawing.Point(ScaleX(305), ScaleY(66));
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(ScaleX(MapPanel.Width), ScaleY(MapPanel.Height));
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

        private void MapPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!MapPanel.Visible) return;
            if (!VerifyStreamNetwork()) return;

            double xchange = (xmax - xmin) * 0.001 * e.Delta;
            double ychange = (ymax - ymin) * 0.001 * e.Delta;

            xmin = (float) (xmin + xchange);
            ymin = (float) (ymin + ychange);

            xmax = (float)(xmax - xchange);
            ymax = (float)(ymax - ychange);

            DrawMapPanel();
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
            ConsoleButton.Checked = true;

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
                    if (!ValidFilen(BaseDir + "StreamNetwork.JSON", true))
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
            // ConsoleButton.Checked = true;
            chart1.Visible = false;
            string BaseDir = basedirBox.Text;

            if (!VerifyStreamNetwork()) return;

            AddToProcessLog("Please wait, creating individual AQUATOX JSONS for each segment and reading flow data." + Environment.NewLine);

            string BaseFileN = BaseJSONBox.Text;
            if (!ValidFilen(BaseJSONBox.Text, true)) return;
            AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
            string msj = MasterSetupJson();

            //          Parallel.For(1, AQT2D.nSegs+1 , iSeg =>  

            UseWaitCursor = true;
            for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
            {
                string comid = AQT2D.SN.network[iSeg][0];
                string filen = BaseDir + "AQT_2D_" + comid + ".JSON";

                string errmessage = AQT2D.PopulateStreamNetwork(iSeg, msj, out string jsondata);

                if (errmessage == "")
                {
                    File.WriteAllText(filen, jsondata);

                    foreach (IShape iS in Drawing)
                    {
                        if (iS.ID == comid)
                        {
                            iS.LineColor = Color.Blue;
                            iS.Rescale(xBuffer, xBuffer + xScale, yBuffer, yBuffer + yScale, xmin, xmax, ymin, ymax);
                            iS.Draw(MPG, showCOMIDsBox.Checked);

                        }
                    }

                    TSafeAddToProcessLog("Read Flows and Saved JSON for " + comid);
                }
                else
                {
                    TSafeAddToProcessLog(errmessage);
                    UseWaitCursor = false;
                    foreach (IShape iS in Drawing)
                    {
                        if (iS.ID == comid)
                        {
                            iS.LineColor = Color.Red;
                            iS.Rescale(xBuffer, xBuffer + xScale, yBuffer, yBuffer + yScale, xmin, xmax, ymin, ymax);
                            iS.Draw(MPG, showCOMIDsBox.Checked);

                        }
                    }
                    return;
                }
            }

            AddToProcessLog("");
            if (AQT2D.SN.boundary.TryGetValue("out-of-network", out int[] boundaries))
            {
                string bnote = "Note: Boundary Condition Flows and State Variable upriver inputs should be added to COMIDs: ";
                foreach (long bid in boundaries) bnote = bnote + bid.ToString() + ", ";
                AddToProcessLog(bnote);
            }

            UseWaitCursor = false;
            AddToProcessLog("Finished creating linked inputs" + Environment.NewLine);

        }


        private void executeButton_Click(object sender, EventArgs e)  //execute the full model run given initialized AQT2D
        {
            chart1.Visible = false;

            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.SN == null)
            {
                string BaseDir = basedirBox.Text;
                if (!ValidFilen(BaseDir + "StreamNetwork.JSON", true)) return;
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
                    if (!ValidFilen(FileN, false)) { TSafeAddToProcessLog("Error File Missing " + FileN); return; }
                    string json = File.ReadAllText(BaseDir + "AQT_2D_" + runID.ToString() + ".JSON");  //read one segment of 2D model
                    if (AQT2D.executeModel(runID, MasterSetupJson(), ref strout, ref json))   //run one segment of 2D model
                        File.WriteAllText(BaseDir + "AQT_Run_" + runID.ToString() + ".JSON", json);

                    foreach (IShape iS in Drawing)
                    {
                        if (iS.ID == runID.ToString())
                        {
                            iS.LineColor = Color.Green;
                            iS.Rescale(xBuffer, xBuffer + xScale, yBuffer, yBuffer + yScale, xmin, xmax, ymin, ymax);
                            iS.Draw(MPG, showCOMIDsBox.Checked);
                            
                        }
                    }

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

        private bool ValidFilen(string filen, bool Showmessage)
        {
            if (!File.Exists(filen))
            {
                if (Showmessage) MessageBox.Show("Cannot find file " + Path.GetFullPath(filen));
                return false;
            }
            return true;
        }

        private AQTSim InstantiateBaseJSON()
        {
            if (!ValidFilen(BaseJSONBox.Text, true)) { MessageBox.Show("Specify Valid File in Base JSON"); return null; }
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
                    if (!ValidFilen(FileN, false))
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
            if (!gf.ShowGrid(OverlandTable, true, false, "")) return;

            for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
            {
                int ColNum = 0;
                DataRow row = OverlandTable.Rows[iSeg - 1];
                string comid = AQT2D.SN.network[iSeg][0];
                string BaseDir = basedirBox.Text;
                string FileN = BaseDir + "AQT_2D_" + comid + ".JSON";
                if (!ValidFilen(FileN, false)) { AddToProcessLog("Error File Missing " + FileN); return; }

                string JSON = System.IO.File.ReadAllText(FileN);
                if (HasN)
                {
                    JSON = Sim.InsertLoadings(JSON, "TNH4Obj", 2, (double)row[ColNum + 1], 1);
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

            comidBox.Text = Properties.Settings.Default.COMID;
            EndCOMIDBox.Text = Properties.Settings.Default.EndCOMID;
            spanBox.Text = Properties.Settings.Default.UpSpan;

        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string target = "_Toc77252249";
            AQTTestForm.OpenUrl(target);
        }

        int xScale;
        int yScale;

        List<IShape> Drawing = new List<IShape>();

        public interface IShape
        {
            GraphicsPath GetPath();
            public Color LineColor { get; set; }
            bool HitTest(Point p);
            void Draw(Graphics g, bool showLabel);
            void Move(Point d);
            void Rescale(int bxmin, int bxmax, int bymin, int bymax, float xMn, float xMx, float yMn, float yMx);  //scale into the border bxmin bxmax based on shape xy and max/min shape xy
            public string ID { get; set; }
        }
        public class Circle : IShape
        {
            public string ID { get; set; }
            public Color LineColor { get; set; }
            public Circle() { FillColor = Color.Red; }
            public Circle(int x1, int y1, string inID) {
                FillColor = Color.Red;
                Center = new Point(x1, y1);
                Radius = 4;
                ID = inID;
            }

            public Color FillColor { get; set; }
            public Point Center { get; set; }
            public int Radius { get; set; }
            public GraphicsPath GetPath()
            {
                var path = new GraphicsPath();
                var p = Center;
                p.Offset(-Radius, -Radius);
                path.AddEllipse(p.X, p.Y, 2 * Radius, 2 * Radius);
                return path;
            }

            public bool HitTest(Point p)
            {
                var result = false;
                using (var path = GetPath())
                    result = path.IsVisible(p);
                return result;
            }
            public void Draw(Graphics g, bool showLabel)
            {
                using (var path = GetPath())
                {
                    using (var brush = new SolidBrush(FillColor))
                        g.FillPath(brush, path);
                    using (var pen = new Pen(Color.Black))
                        g.DrawPath(pen, path);
                }

            }
            public void Move(Point d)
            {
                Center = new Point(Center.X + d.X, Center.Y + d.Y);
            }

            public void Rescale(int bxmin, int bxmax, int bymin, int bymax, float xMn, float xMx, float yMn, float yMx)  //scale into the border xmin xmax based on shape xy
            {
                Center = new Point((int)Math.Round(bxmin + (bxmax - bxmin) * (Center.X - xMn) / (xMx - xMn)), (int)Math.Round(bymax - (bymax - bymin) * (Center.Y - yMn) / (yMx - yMn)));
            }

        }

        public class PolyLine : IShape
        {
            public string ID { get; set; }
            public Color LineColor { get; set; }
            public List<PointF> StartPoints { get; set; }
            public List<PointF> EndPoints { get; set; }
            private List<Point> PlotStartPoints { get; set; }
            private List<Point> PlotEndPoints { get; set; }
            public int LineWidth { get; set; }


            public PolyLine() { LineWidth = 2; LineColor = Color.Black; }

            private void CheckMinMax(PointF pt)
            {
                if (pt.X > xmax) xmax = pt.X;
                if (pt.Y > ymax) ymax = pt.Y;
                if (pt.X < xmin) xmin = pt.X;
                if (pt.Y < ymin) ymin = pt.Y;
            }

            public PolyLine(List<PointF> sPts, List<PointF> ePts, string inID)
            {
                LineWidth = 2;
                LineColor = Color.Black;
                StartPoints = sPts;
                EndPoints = ePts;
                ID = inID;

                foreach (PointF pt in StartPoints) CheckMinMax(pt);
                foreach (PointF pt in EndPoints) CheckMinMax(pt);
            }

            public GraphicsPath GetPath()
            {
                var path = new GraphicsPath();
                for (int i = 0; i < PlotStartPoints.Count; i++)
                    path.AddLine(PlotStartPoints[i], PlotEndPoints[i]);

                return path;
            }

            public bool HitTest(Point p)
            {
                var result = false;
                using (var path = GetPath())
                using (var pen = new Pen(LineColor, LineWidth + 2))
                    result = path.IsOutlineVisible(p, pen);
                return result;
            }

            public void Draw(Graphics g, bool ShowCOMIDs)
            {
                Pen pen = new Pen(LineColor, LineWidth);  // older draw line, arrowheads too small
                                                          // pen.StartCap = LineCap.ArrowAnchor;
                var gpath = GetPath();
                using (pen)
                    g.DrawPath(pen, gpath);

                if (ShowCOMIDs)
                {
                    RectangleF rectf = gpath.GetBounds();
                    Point Location = new Point((int)Math.Round(rectf.X + rectf.Width / 2), (int)Math.Round(rectf.Y + rectf.Height / 2)+6);
                    Size sizeOfText = TextRenderer.MeasureText(ID, new Font("Arial", 8));
                    Rectangle rect = new Rectangle(Location, sizeOfText);
                    g.FillRectangle(Brushes.White, rect);
                    g.DrawString(ID, new Font("Arial", 8), new SolidBrush(Color.Navy), Location);
                }

            }

            public void Move(Point d)
            {
            }


            public void Rescale(int bxmin, int bxmax, int bymin, int bymax, float xMn, float xMx, float yMn, float yMx)  //scale into the border xmin xmax based on shape xy
            {
                Point rsc(PointF pt)
                {
                    return new Point((int)Math.Round(bxmin + (bxmax - bxmin) * (pt.X - xMn) / (xMx - xMn)), (int)Math.Round(bymax - (bymax - bymin) * (pt.Y - yMn) / (yMx - yMn)));
                }

                PlotStartPoints = new List<Point>();
                PlotEndPoints = new List<Point>();
                for (int i = 0; i < StartPoints.Count; i++)
                {
                    PlotStartPoints.Add(rsc(StartPoints[i]));
                    PlotEndPoints.Add(rsc(EndPoints[i]));
                }
            }
        }


        public class Arrow : IShape
        {
            public string ID { get; set; }
            public Arrow() { LineWidth = 3; LineColor = Color.Black; }
            public Arrow(int x1, int y1, int x2, int y2, string inID)
            {
                LineWidth = 3;
                LineColor = Color.Black;
                Point1 = new Point(x1, y1);
                Point2 = new Point(x2, y2);
                ID = inID;

                if (x1 > xmax) xmax = x1;
                if (y1 > ymax) ymax = y1;
                if (x1 < xmin) xmin = x1;
                if (y1 < ymin) ymin = y1;

                if (x2 > xmax) xmax = x2;
                if (y2 > ymax) ymax = y2;
                if (x2 < xmin) xmin = x2;
                if (y2 < ymin) ymin = y2;

            }
            public int LineWidth { get; set; }
            public Color LineColor { get; set; }
            public Point Point1 { get; set; }
            public Point Point2 { get; set; }
            public Point PlotPoint1 { get; set; }
            public Point PlotPoint2 { get; set; }

            public GraphicsPath GetPath()
            {
                var path = new GraphicsPath();
                path.AddLine(PlotPoint1, PlotPoint2);
                return path;
            }
            public bool HitTest(Point p)
            {
                var result = false;
                using (var path = GetPath())
                using (var pen = new Pen(LineColor, LineWidth + 2))
                    result = path.IsOutlineVisible(p, pen);
                return result;
            }

            public void Draw(Graphics g, bool ShowCOMIDs)
            {
                DrawArrow(g, PlotPoint2, PlotPoint1, LineColor, 2, 1);

                //                    Pen pen = new Pen(LineColor, LineWidth);  // older draw line, arrowheads too small
                // pen.StartCap = LineCap.ArrowAnchor;
                //                    using (var path = GetPath())
                //                    using (pen)
                //                    g.DrawPath(pen, path);

                if (ShowCOMIDs)
                {
                    Point Location = new Point(5 + (PlotPoint1.X + PlotPoint2.X) / 2, -5 + (PlotPoint2.Y + PlotPoint2.Y) / 2);
                    Size sizeOfText = TextRenderer.MeasureText(ID, new Font("Arial", 8));
                    Rectangle rect = new Rectangle(Location, sizeOfText);
                    g.FillRectangle(Brushes.White, rect);
                    g.DrawString(ID, new Font("Arial", 8), new SolidBrush(Color.Navy), Location);

                }

            }

            public void Move(Point d)
            {
                Point1 = new Point(Point1.X + d.X, Point1.Y + d.Y);
                Point2 = new Point(Point2.X + d.X, Point2.Y + d.Y);
            }


            public void Rescale(int bxmin, int bxmax, int bymin, int bymax, float xMn, float xMx, float yMn, float yMx)  //scale into the border xmin xmax based on shape xy
            {
                PlotPoint1 = new Point((int)Math.Round(bxmin + (bxmax - bxmin) * (Point1.X - xMn) / (xMx - xMn)), (int)Math.Round(bymax - (bymax - bymin) * (Point1.Y - yMn) / (yMx - yMn)));
                PlotPoint2 = new Point((int)Math.Round(bxmin + (bxmax - bxmin) * (Point2.X - xMn) / (xMx - xMn)), (int)Math.Round(bymax - (bymax - bymin) * (Point2.Y - yMn) / (yMx - yMn)));
            }

            private void DrawArrow(Graphics g, PointF ArrowStart, PointF ArrowEnd, Color ArrowColor, int LineWidth, int ArrowMultiplier)
            {
                //create the pen
                Pen p = new Pen(ArrowColor, LineWidth);

                //draw the line
                g.DrawLine(p, ArrowStart, ArrowEnd);

                //determine the coords for the arrow point

                //tip of the arrow
                PointF arrowPoint = ArrowEnd;

                //determine line length
                double arrowLength = Math.Sqrt(Math.Pow(Math.Abs(ArrowStart.X - ArrowEnd.X), 2) +
                                               Math.Pow(Math.Abs(ArrowStart.Y - ArrowEnd.Y), 2));

                //determine line angle
                double arrowAngle = Math.Atan2(Math.Abs(ArrowStart.Y - ArrowEnd.Y), Math.Abs(ArrowStart.X - ArrowEnd.X));

                //get the x,y of the back of the point

                //to change from an arrow to a diamond, change the 3
                //in the next if/else blocks to 6

                double pointX, pointY;
                if (ArrowStart.X > ArrowEnd.X)
                {
                    pointX = ArrowStart.X - (Math.Cos(arrowAngle) * (arrowLength - (3 * ArrowMultiplier)));
                }
                else
                {
                    pointX = Math.Cos(arrowAngle) * (arrowLength - (3 * ArrowMultiplier)) + ArrowStart.X;
                }

                if (ArrowStart.Y > ArrowEnd.Y)
                {
                    pointY = ArrowStart.Y - (Math.Sin(arrowAngle) * (arrowLength - (3 * ArrowMultiplier)));
                }
                else
                {
                    pointY = (Math.Sin(arrowAngle) * (arrowLength - (3 * ArrowMultiplier))) + ArrowStart.Y;
                }

                PointF arrowPointBack = new PointF((float)pointX, (float)pointY);

                //get the secondary angle of the left tip
                double angleB = Math.Atan2((3 * ArrowMultiplier), (arrowLength - (3 * ArrowMultiplier)));

                double angleC = Math.PI * (90 - (arrowAngle * (180 / Math.PI)) - (angleB * (180 / Math.PI))) / 180;

                //get the secondary length
                double secondaryLength = (3 * ArrowMultiplier) / Math.Sin(angleB);

                if (ArrowStart.X > ArrowEnd.X)
                {
                    pointX = ArrowStart.X - (Math.Sin(angleC) * secondaryLength);
                }
                else
                {
                    pointX = (Math.Sin(angleC) * secondaryLength) + ArrowStart.X;
                }

                if (ArrowStart.Y > ArrowEnd.Y)
                {
                    pointY = ArrowStart.Y - (Math.Cos(angleC) * secondaryLength);
                }
                else
                {
                    pointY = (Math.Cos(angleC) * secondaryLength) + ArrowStart.Y;
                }

                //get the left point
                PointF arrowPointLeft = new PointF((float)pointX, (float)pointY);

                //move to the right point
                angleC = arrowAngle - angleB;

                if (ArrowStart.X > ArrowEnd.X)
                {
                    pointX = ArrowStart.X - (Math.Cos(angleC) * secondaryLength);
                }
                else
                {
                    pointX = (Math.Cos(angleC) * secondaryLength) + ArrowStart.X;
                }

                if (ArrowStart.Y > ArrowEnd.Y)
                {
                    pointY = ArrowStart.Y - (Math.Sin(angleC) * secondaryLength);
                }
                else
                {
                    pointY = (Math.Sin(angleC) * secondaryLength) + ArrowStart.Y;
                }

                PointF arrowPointRight = new PointF((float)pointX, (float)pointY);

                //create the point list
                PointF[] arrowPoints = new PointF[4];
                arrowPoints[0] = arrowPoint;
                arrowPoints[1] = arrowPointLeft;
                arrowPoints[2] = arrowPointBack;
                arrowPoints[3] = arrowPointRight;

                //draw the outline
                g.DrawPolygon(p, arrowPoints);

                //fill the polygon
                // g.FillPolygon(new SolidBrush(ArrowColor), arrowPoints);
            }

        }

        bool PlotCOMIDArrow(int COMID, int x1, int y1, int x2, int y2)
        {
            string CString = COMID.ToString();

            Drawing.Add(new Arrow(x1, y1, x2, y2, COMID.ToString()));
            int[] boundaries;
            if (AQT2D.SN.boundary.TryGetValue("out-of-network", out boundaries))
                if (boundaries.Contains(COMID)) Drawing.Add(new Circle(x2, y2, COMID.ToString()));

            int iSeg = -1;
            for (int i = 0; i < AQT2D.SN.network.GetLength(0); i++)
                if (AQT2D.SN.network[i][0] == CString)
                {
                    iSeg = i;
                    break;
                }

            if (iSeg <= 0) return false;

            double lenkm = double.Parse(AQT2D.SN.network[iSeg][4]);
            // AddToProcessLog(COMID + " " + x1.ToString() + "," + y1.ToString() + "|" + x2.ToString() + "," + y2.ToString() + " length " + lenkm.ToString());  debugging info

            int nSources = 0;
            if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                foreach (int SrcID in Sources)
                {
                    if ((SrcID != COMID) && !(boundaries.Contains(SrcID)))  // set to itself in boundaries 
                    {
                        if (PlotCOMIDArrow(SrcID, x2, y2, x2 + nSources, y2 + 1)) nSources++;
                    }
                };

            return true;
        }

        public class GeoJSonType
        {
            public StreamGeometry stream_geometry { get; set; }
        }
        public class StreamGeometry
        {
            public Feature[] features { get; set; }
        }
        public class Feature
        {
            public Geometry geometry { get; set; }
        }
        public class Geometry
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }


        bool PlotCOMIDMap()
        {
            string BaseDir = basedirBox.Text;
            string GeoJSON = "";
            double[][] polyline;

            for (int i = 0; i < AQT2D.SN.order.Length; i++)
                for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                {
                    string CString = AQT2D.SN.order[i][j].ToString();
                    if (File.Exists(BaseDir + CString + ".GeoJSON"))
                    { GeoJSON = System.IO.File.ReadAllText(BaseDir + CString + ".GeoJSON"); }
                    else
                    {
                        GeoJSON = AQT2D.ReadGeoJSON(CString);  // read from web service
                        if (GeoJSON.IndexOf("ERROR") >= 0)
                        {
                            AddToProcessLog("Error reading GeoJSON: web service returned: " + GeoJSON);
                            // show process log  fixme
                            return false;
                        }
                        // fixme check on AQT_2D_23399987.JSON
                        System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", GeoJSON);  //  write to disk
                    }

                    GeoJSonType coords = JsonConvert.DeserializeObject<GeoJSonType>(GeoJSON);
                    try 
                    {
                        polyline = coords.stream_geometry.features[0].geometry.coordinates;
                    }
                    catch
                    {
                        AddToProcessLog("Error deserializing GeoJSON  " + BaseDir + CString + ".GeoJSON"); return false;
                    }


                    List<PointF> startpoints = new List<PointF>();
                    List<PointF> endpoints = new List<PointF>();

                    for (int k = 0; k < polyline.Length - 1; k++)
                    {
                        startpoints.Add(new PointF((float)polyline[k][0], (float)polyline[k][1]));
                        endpoints.Add(new PointF((float)polyline[k + 1][0], (float)polyline[k + 1][1]));
                    }
                    Drawing.Add(new PolyLine(startpoints, endpoints, CString));
                    }

            return true;
        }

        private void PlotButton_Click(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;
            RedrawShapes();
        }

        private void RedrawShapes()
        {
            Drawing.Clear();
            xmax = -1000000;
            ymax = -1000000;
            xmin = 1000000;
            ymin = 1000000;

            if (DrawMap)
            { if (PlotCOMIDMap()) DrawMapPanel(); }
            else
            {
                int EndID = AQT2D.SN.order[AQT2D.SN.order.Length - 1][0];
                if (PlotCOMIDArrow(EndID, 0, 0, 0, 1)) DrawMapPanel();
            }

        }

        private void RescaleMap()
        {
            MPG = MapPanel.CreateGraphics();
            xBuffer = 80;
            yBuffer = 10;
            if (DrawMap) xBuffer = 10;

            yScale = MapPanel.Size.Height - 2 * yBuffer;
            xScale = MapPanel.Size.Width - 2 * xBuffer;

            if (DrawMap)  // resize and buffer to preserve map's aspect ratio
            {
                double XoverY = ((double)(xmax - xmin) / (double)(ymax - ymin));
                double aspectratio = ((double)xScale / (double)yScale) / XoverY;
                if (aspectratio > 1)
                {
                    xScale = (int)Math.Round(xScale / aspectratio);
                    xBuffer = (MapPanel.Size.Width - xScale) / 2;
                }
                else
                {
                    yScale = (int)Math.Round(yScale * aspectratio);
                    yBuffer = (MapPanel.Size.Height - yScale) / 2;
                }
            }
        }

        private void DrawMapPanel()
        {
            RescaleMap();
            MapButton2.Checked = true;
            MapPanel.Visible = true;
            MPG.Clear(Color.White);

            foreach (IShape iS in Drawing)
            {
                iS.Rescale(xBuffer, xBuffer + xScale, yBuffer, yBuffer + yScale, xmin, xmax, ymin, ymax);
                iS.Draw(MPG, showCOMIDsBox.Checked);
            }
        }

        private void MultiSegForm_ResizeEnd(object sender, EventArgs e)
        {
            if (!MapPanel.Visible) return;
            if (!VerifyStreamNetwork()) return;
            // RedrawShapes();  // optimize
            DrawMapPanel();
        }

        private Point clickPosition;

        private void MapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            clickPosition.X = e.X;
            clickPosition.Y = e.Y;

            for (var i = Drawing.Count - 1; i >= 0; i--)
                if (Drawing[i].HitTest(e.Location)) { EditCOMID(Drawing[i].ID); }; 
                        // MessageBox.Show("COMID: " + Drawing[i].ID); }; 

            base.OnMouseDown(e);
        }

        private void mapButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;
            DrawMap = mapButton.Checked;
            RedrawShapes();
        }

        private void ConsoleButton_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Visible = false;
            if (ConsoleButton.Checked) MapPanel.Visible = false;
            else
            {
                MapPanel.Visible = true;
                if (!VerifyStreamNetwork()) return;
                RedrawShapes();             }
        }

        private void EditCOMID(string CString)
        {
            string BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_2D_" + CString + ".JSON";
            if (ValidFilen(filen, false))
            {
                string json = File.ReadAllText(filen);  //read one segment of multi-seg model
                AQTTestForm AQForm = new AQTTestForm();
                if (AQForm.EditLinkedInput(ref json)) File.WriteAllText(filen,json); 
            }
                else { MessageBox.Show("COMID: " + CString+ ".  Linked input for this COMID not yet generated."); };
        }

        private void MapPanel_MouseHover(object sender, EventArgs e)
        {
            MapPanel.Focus();
        }


        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
           if (e.Button == MouseButtons.Left)
           {
                if ((Math.Abs(e.X - clickPosition.X)<3)&&(Math.Abs(e.Y - clickPosition.Y) < 3)) return;
                
                float DeltaX = ((float) (clickPosition.X - e.X) / (float) MapPanel.Width) * (xmax - xmin);
                float DeltaY = ((float) (e.Y - clickPosition.Y) / (float) MapPanel.Height) * (ymax - ymin);

                if ((DeltaX == 0)&&(DeltaY == 0)) return;

                xmin = xmin + DeltaX; 
                xmax = xmax + DeltaX;
                ymin = ymin + DeltaY;
                ymax = ymax + DeltaY;

                if (!VerifyStreamNetwork()) return;
                DrawMapPanel();

                clickPosition.X = e.X;
                clickPosition.Y = e.Y;


            }
        }
    }
}
