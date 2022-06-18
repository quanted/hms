using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using AQUATOX.Loadings;
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
using Microsoft.Web.WebView2.Core;
//using Web.Services.Controllers;

namespace GUI.AQUATOX

{
    public partial class MultiSegForm : Form
    {
        // private BackgroundWorker Worker = new BackgroundWorker();  potentially to be added, to report progress
        private AQSim_2D AQT2D = null;

        private FormWindowState LastWindowState = FormWindowState.Minimized;
        private Chart chart1 = new Chart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        DataTable OverlandTable = null;
        private System.Drawing.Graphics graphics;
        private ScreenSettings ScrSettings = new();
        private List<int> executed = new List<int>(); // list of comids that have been asked to execute

        public class ScreenSettings
        {
            public string BaseJSONstr = "";
            public string COMIDstr = "";
            public string EndCOMIDstr = "";
            public string UpSpanStr = "";
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

        public MultiSegForm()
        {
            AutoScroll = true;
            InitializeComponent();

            this.Resize += new System.EventHandler(this.FormResize);
            
            InitializeAsync();

            webView.Source = new Uri(Path.Combine(Environment.CurrentDirectory, @"html\leafletMap.html"));

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
            chart1.Location = new System.Drawing.Point(ScaleX(webView.Left), ScaleY(webView.Top));
            chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(ScaleX(webView.Width), ScaleY(webView.Height));
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

            chart1.Location = new System.Drawing.Point(webView.Left, webView.Top);
            chart1.Size = new System.Drawing.Size(webView.Width, webView.Height);

        }

        private void FormResize(object sender, EventArgs e)
        {
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.WebMessageReceived += MessageReceived;
        }

        private void SendMessage(object sender, EventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.PostWebMessageAsString("Message from Dotnet buttton");
            }
        }

        void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String content = args.TryGetWebMessageAsString();
            webView_MouseDown(content);
            // MessageBox.Show(content); debug
        }

        protected override void WndProc(ref Message m)  
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128) return;
            base.WndProc(ref m);
            // https://stackoverflow.com/questions/8848203/alt-key-causes-form-to-redraw
        }

        private void SaveScreenSettings()
        {
            try
            {
                if (!VerifyBaseDir()) return;
                string BaseDir = basedirBox.Text;
                string ScrString = JsonConvert.SerializeObject(ScrSettings); 
                File.WriteAllText(BaseDir + "ScreenSettings.JSON", ScrString);
            }
            catch (Exception ex)
            {
                ProcessLog.Text = "Error saving Screen Settings: " + ex.Message;
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void LoadScreenSettings()
        {
            ScrSettings.BaseJSONstr = "";
            ScrSettings.COMIDstr = "";
            ScrSettings.EndCOMIDstr = "";
            ScrSettings.UpSpanStr = "";

            try
            {
                if (!VerifyBaseDir()) return;
                string BaseDir = basedirBox.Text;
                if (!File.Exists((BaseDir + "ScreenSettings.JSON"))) return;
                string ScrString = File.ReadAllText(BaseDir + "ScreenSettings.JSON");
                ScrSettings = JsonConvert.DeserializeObject<ScreenSettings>(ScrString);

            }
            catch (Exception ex)
            {
                ProcessLog.Text = "Error loading Screen Settings: " + ex.Message;
                MessageBox.Show(ex.Message);
                return;
            }

        }


        private void UpdateScreen()
        {
            bool validDirectory = VerifyBaseDir();
            bool streamnetwork = false;
            if (validDirectory) streamnetwork = VerifyStreamNetwork();
            bool inputsegs = false;
            DateTime modelrun = DateTime.MinValue;

            if (streamnetwork)
            {
                inputsegs = SegmentsCreated();
                if (inputsegs)
                {
                    modelrun = ModelRunDate();
                }
            }
            else
            {
                ConsoleButton.Checked = true;
                webView.Visible = false;
                chart1.Visible = false;
            }

            comidBox.Text = ScrSettings.COMIDstr;
            EndCOMIDBox.Text = ScrSettings.EndCOMIDstr;
            spanBox.Text = ScrSettings.UpSpanStr;
            BaseJSONBox.Text = ScrSettings.BaseJSONstr;

            if (modelrun != DateTime.MinValue) StatusLabel.Text = "Run on " + modelrun.ToLocalTime();
            else if (inputsegs) StatusLabel.Text = "Linked Input Segments Created";
            else if (streamnetwork) StatusLabel.Text = "Stream Network Created";
            else if (validDirectory) StatusLabel.Text = "Model Not Initiated";
            else StatusLabel.Text = "Invalid Directory Specified";

            BaseJSONBox.Enabled = validDirectory;
            ChooseTemplateButton.Enabled = validDirectory;
            ReadNetworkPanel.Enabled = validDirectory;
            PlotPanel.Enabled = streamnetwork; 
            TogglePanel.Enabled = streamnetwork;
            SetupButton.Enabled = validDirectory;
            createButton.Enabled = streamnetwork;
            FlowsButton.Enabled = inputsegs;
            executeButton.Enabled = inputsegs;
            OutputPanel.Enabled = ArchivedOutput();
            outputjump.Enabled = (modelrun != DateTime.MinValue);
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
            if (VerifyStreamNetwork())
                if (MessageBox.Show("Overwrite the existing stream network and any inputs and outputs?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            string BaseDir = basedirBox.Text;

            string[] directoryFiles = System.IO.Directory.GetFiles(BaseDir, "*.JSON");
            foreach (string directoryFile in directoryFiles)
            {
                System.IO.File.Delete(directoryFile);
            }

            ConsoleButton.Checked = true;

            if (AQT2D == null) AQT2D = new AQSim_2D();

            AddToProcessLog("Please wait, reading stream network from web service");
            string SNJSON = AQT2D.ReadStreamNetwork(ScrSettings.COMIDstr,  ScrSettings.EndCOMIDstr, ScrSettings.UpSpanStr);
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

            //string BaseFileN = BaseJSONBox.Text;
            //AddToProcessLog(" Basefile = " + BaseFileN);

            AddToProcessLog(" BaseDir = " + BaseDir);

            File.WriteAllText(BaseDir + "StreamNetwork.JSON", SNJSON);
            AddToProcessLog("Finished reading stream network" + Environment.NewLine);

            UpdateScreen();

        }

        private bool SegmentsCreated()
        {
            string BaseDir = basedirBox.Text;
            string comid = AQT2D.SN.network[AQT2D.SN.network.Length-1][0];  //check last segment
            string FileN = BaseDir + "AQT_2D_" + comid.ToString() + ".JSON";
            return (ValidFilen(FileN, false));
        }

        private DateTime ModelRunDate()
        {
            string BaseDir = basedirBox.Text;
            string comid = AQT2D.SN.network[AQT2D.SN.network.Length - 1][0];
            string FileN = BaseDir + "AQT_Run_" + comid.ToString() + ".JSON";
            if (!ValidFilen(FileN, false)) return DateTime.MinValue;
            return File.GetLastWriteTime(FileN);
        }

        private bool ArchivedOutput()
        {

            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.archive == null) return false;
            if (AQT2D.archive.Count == 0)
            {
                string archfilen = basedirBox.Text + "Output_Summary.json";
                string svlistfilen = basedirBox.Text + "SVList.json";
                if (!File.Exists(archfilen)) return false;
                try
                {
                    string arch = File.ReadAllText(archfilen);
                    AQT2D.archive = JsonConvert.DeserializeObject<Dictionary<int, AQSim_2D.archived_results>>(arch);
                    string svlist = File.ReadAllText(svlistfilen);
                    AQT2D.SVList = JsonConvert.DeserializeObject<List<string>>(svlist);
                    bindgraphlist();
                }
                catch
                { 
                    AQT2D.archive = null;  
                    return false; 
                }
            }
            return true;
        }

        private bool VerifyBaseDir()
        {
            string BaseDir = basedirBox.Text;
            return Directory.Exists(BaseDir);
        }

        private bool VerifyStreamNetwork()
        {
            string BaseDir = basedirBox.Text;
            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.SN == null)
            {
                try
                {
                    if (!ValidFilen(BaseDir + "StreamNetwork.JSON", false))
                    {
                        // AddToProcessLog("Cannot find stream network file " + BaseDir + "StreamNetwork.JSON");
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
            try
            {
                ConsoleButton.Checked = true;
                chart1.Visible = false;
                string BaseDir = basedirBox.Text;

                if (!VerifyStreamNetwork()) return;

                if (SegmentsCreated())
                    if (MessageBox.Show("Overwrite the existing set of segments and any edits made to the inputs?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

                AddToProcessLog("Please wait, creating individual AQUATOX JSONS for each segment and reading flow data." + Environment.NewLine);

                string BaseFileN = BaseJSONBox.Text;
                if (!ValidFilen(BaseJSONBox.Text, true)) return;
                AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
                string msj = MasterSetupJson();

                //   Parallel.For(1, AQT2D.nSegs+1 , iSeg =>  

                UseWaitCursor = true;
                progressBar1.Visible = true;
                Application.DoEvents();

                for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
                {
                    string comid = AQT2D.SN.network[iSeg][0];
                    string filen = BaseDir + "AQT_2D_" + comid + ".JSON";

                    bool in_waterbody = false;
                    if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.SN.waterbodies.comid_wb.ContainsKey(int.Parse(comid));
                    if (in_waterbody)
                    {
                        TSafeAddToProcessLog(comid + " is not modeled as a stream segment as it is part of a lake/reservoir.");
                        continue;
                    }


                    string errmessage = AQT2D.PopulateStreamNetwork(iSeg, msj, out string jsondata);

                    if (errmessage == "")
                    {
                        File.WriteAllText(filen, jsondata);

                        TSafeAddToProcessLog("Read Flows and Saved JSON for " + comid);
                        int Prog = (int)((float)iSeg / (float)AQT2D.nSegs * 100.0);
                        if (Prog < 100) progressBar1.Value = (Prog + 1);  // workaround of animation bug
                        progressBar1.Value = Math.Max(Prog, 1);

                        Application.DoEvents();
                    }
                    else
                    {
                        TSafeAddToProcessLog(errmessage);
                        UseWaitCursor = false;
                        // Fixme draw COMID shape in red
                        progressBar1.Visible = false;  // fixme not thread safe
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

                progressBar1.Visible = false;
                UseWaitCursor = false;
                AddToProcessLog("Finished creating linked inputs" + Environment.NewLine);
                UpdateScreen();
            }

            catch (Exception ex)
            {
                ProcessLog.Text = "Error creating segments: " + ex.Message;
                MessageBox.Show(ex.Message);
                return;
            }



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

            if (ModelRunDate() != DateTime.MinValue)
                if (MessageBox.Show("Overwrite all existing model-run results in this directory?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            AddToProcessLog("Starting model execution...");
            if (AQT2D.archive == null) AQT2D.archive = new Dictionary<int, AQSim_2D.archived_results>();
            AQT2D.archive.Clear();

            UseWaitCursor = true;
            progressBar1.Visible = true;

            int[] outofnetwork = new int[0];
            if (AQT2D.SN.boundary != null)
                AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

            try
            {

                for (int ordr = 0; ordr < AQT2D.SN.order.Length; ordr++)
                {
                    Parallel.ForEach(AQT2D.SN.order[ordr], runID =>
                    //                foreach (int runID in AQT2D.SN.order[ordr])
                    {
                        string strout = "";
                        string BaseDir = basedirBox.Text;
                        string FileN = BaseDir + "AQT_2D_" + runID.ToString() + ".JSON";
                        if (!ValidFilen(FileN, false))
                        {
                            TSafeAddToProcessLog("Error File Missing " + FileN); UseWaitCursor = false;
                            progressBar1.Visible = false; return;  // FIXME, progressbar visible is not thread safe
                        }
                        string json = File.ReadAllText(BaseDir + "AQT_2D_" + runID.ToString() + ".JSON");  //read one segment of 2D model

                        List<ITimeSeriesOutput<List<double>>> divergence_flows = null;

                        if (AQT2D.SN.divergentpaths != null)
                         if (AQT2D.SN.divergentpaths.TryGetValue(runID.ToString(), out int[] Divg))
                            foreach (int ID in Divg)
                            {
                                TimeSeriesOutput<List<double>> ITSO = null;
                                string DivSeg = File.ReadAllText(BaseDir + "AQT_2D_" + ID.ToString() + ".JSON");  //read the divergent segment of 2D model 
                                AQTSim DivSim = new AQTSim();
                                string outstr = DivSim.Instantiate(DivSeg);
                                if (outstr == "")
                                {
                                    if (divergence_flows == null) divergence_flows = new List<ITimeSeriesOutput<List<double>>>();
                                    DivSim.AQTSeg.SetMemLocRec();
                                    TVolume tvol = DivSim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                                    TLoadings InflowLoad = tvol.LoadsRec.Alt_Loadings[0];
                                    ITSO = InflowLoad.TimeSeriesAsTSOutput("Divergence Flows", "COMID " + ID.ToString(), 1.0 / 86400.0);  // output flows as m2/s
                                }
                                divergence_flows.Add(ITSO);
                            }

                        if (AQT2D.executeModel(runID, MasterSetupJson(), ref strout, ref json, divergence_flows, outofnetwork))   //run one segment of 2D model
                        File.WriteAllText(BaseDir + "AQT_Run_" + runID.ToString() + ".JSON", json);

                        // fixme draw COMID shape in green

                        TSafeAddToProcessLog(strout);  //write update to status log

                    // }  // non-parallel foreach format for debugging
                });  // parallel foreach format

                    int Prog = (int)((float)ordr / (float)AQT2D.SN.order.Length * 100.0);
                    if (Prog < 100) progressBar1.Value = (Prog + 1);  // workaround of animation bug
                    progressBar1.Value = Math.Max(Prog, 1);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                ProcessLog.Text = "Error running linked segments: " + ex.Message;
                MessageBox.Show(ex.Message);
                return;
            }


            bindgraphlist();
            OutputPanel.Enabled = true;

            UseWaitCursor = false;
            progressBar1.Visible = false    ;

            string archfilen = basedirBox.Text + "Output_Summary.json";
            string svlistfilen = basedirBox.Text + "SVList.json";
            string arch = JsonConvert.SerializeObject(AQT2D.archive);
            File.WriteAllText(archfilen, arch);
            string svlist = JsonConvert.SerializeObject(AQT2D.SVList);
            File.WriteAllText(svlistfilen, svlist);

            AddToProcessLog("Model execution complete");
            UpdateScreen();

        }


        private void bindgraphlist()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = null;
            bs.DataSource = AQT2D.SVList;
            SVBox.DataSource = bs;   //update state variable list for graphing, CSV
        }

        private void CSVButton_Click(object sender, EventArgs e)  //write CSV output from archived results given selected state variable in dropdown box
        {
            
            chart1.Hide();
            if (!ConsoleButton.Checked) ConsoleButton.Checked = true;

            if (AQT2D == null) return;
            if (AQT2D.archive == null) return;

            int SVIndex = SVBox.SelectedIndex;
            StringBuilder csv = new StringBuilder(SVBox.Text + Environment.NewLine + "Date,");

            int NDates = 0;
            double[][] outputs = new double[AQT2D.archive.Count][];
            int i = 0;

            DateTime[] dateList = null;

            foreach (KeyValuePair<int, AQSim_2D.archived_results> entry in AQT2D.archive)
            {
                csv.Append((entry.Key) + ",");
                if (i == 0)
                {
                    NDates = entry.Value.dates.Count();
                    dateList = entry.Value.dates;
                }

                outputs[i] = entry.Value.concs[SVIndex];

                i++;
            }
            csv.Append(Environment.NewLine);


            for (int j = 0; j < NDates; j++)
            {
                csv.Append(dateList[j].ToShortDateString()+" "+ dateList[j].ToShortTimeString()+",");
                for (i = 0; i < outputs.GetLength(0); i++)
                {
                    csv.Append((outputs[i][j]) + ",");
                }
                csv.Append(Environment.NewLine);
            }

            ProcessLog.Text = csv.ToString();

            if (MessageBox.Show("Save CSV to text?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV File|*.CSV";
            saveFileDialog1.Title = "Save to Comma-Separated Text";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, csv.ToString());
            }


        }

        private void ChartButtonClick(object sender, EventArgs e)   //produce graphics from archived results given selected state variable in dropdown box
        {
            if (AQT2D == null) return;

            try
            {
                int SVIndex = SVBox.SelectedIndex;

                GraphButton.Checked = true;
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.Titles.Add(SVBox.Text);

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
                    if (sercnt > 20) ser.Enabled = false;

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
            if (filen == "") return false;
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
                DateTime firstday = SR.FirstDay.Val;
                DateTime lastday = SR.LastDay.Val;

                TParameter[] SS = new TParameter[] {new TSubheading ("Timestep Settings",""), SR.FirstDay,SR.LastDay,SR.RelativeError,SR.UseFixStepSize,SR.FixStepSize,
                SR.ModelTSDays, new TSubheading("Output Storage Options",""), SR.StepSizeInDays, SR.StoreStepSize, SR.AverageOutput, SR.SaveBRates,
                new TSubheading("Biota Modeling Options",""),SR.NFix_UseRatio,SR.NtoPRatio,
                new TSubheading("Chemical Options",""),SR.ChemsDrivingVars,SR.TSedDetrIsDriving,SR.UseExternalConcs,SR.T1IsAggregate };
                Param_Form SetupForm = new Param_Form();
                SetupForm.SuppressComment = true;
                SetupForm.SuppressSymbol = true;
                SetupForm.EditParams(ref SS, "Simulation Setup", true, "", "SetupWindow");

                if ((firstday != SR.FirstDay.Val) || (lastday != SR.LastDay.Val))
                    if (SegmentsCreated()) MessageBox.Show("The dates of the simulation have been changed.  Note that water flows from the National Water Model will not be updated with the new date range and applied to the linked system until 'Create Linked Inputs' is selected, overwriting the existing linked system.");  

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
            if (GraphButton.Checked) ChartButtonClick(sender, e);
            if (ConsoleButton.Checked) CSVButton_Click(sender, e);
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
            if (!gf.ShowGrid(OverlandTable, true, false, "Overland_Flows")) return;

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

            // string warning = "";
            // if (SegmentsCreated()) warning = ".  Note: This template will not be applied to the linked system until 'Create Linked Inputs' is selected, overwriting the existing linked system.";
            // MessageBox.Show("Selected Template File: " + filen+ warning);
            BaseJSONBox_Leave(sender, e);
        }

        private void basedirBox_Leave(object sender, EventArgs e)
        {
            if (!basedirBox.Text.EndsWith("\\")) basedirBox.Text = basedirBox.Text + "\\";
            AQT2D = null;
            LoadScreenSettings();
            UpdateScreen();
            if (PlotPanel.Enabled)
            {
                MapButton2.Checked = true;                
            }
            SaveBaseDir();
        }

        private void SaveBaseDir()
        {
            if (Properties.Settings.Default.MS_Directory == basedirBox.Text) return;
            Properties.Settings.Default.MS_Directory = basedirBox.Text;
            Properties.Settings.Default.Save();
        }

        private void MultiSegForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveScreenSettings();
            SaveBaseDir();

        }

        private void MultiSegForm_Shown(object sender, EventArgs e)
        {
            basedirBox.Text = Properties.Settings.Default.MS_Directory;
            LoadScreenSettings();

            UpdateScreen();
            if (PlotPanel.Enabled) MapButton2.Checked = true;

        }

        private void HelpButton2_Click(object sender, EventArgs e)
        {
            string target = "Multi_Segment_Runs";
            AQTTestForm.OpenUrl(target);
        }



        bool PlotCOMIDMap()
        {
            string BaseDir = basedirBox.Text;
            string GeoJSON;
            webView.CoreWebView2.PostWebMessageAsString("ERASE");

            int[] boundaries = new int[0];
            if (AQT2D.SN.boundary != null) AQT2D.SN.boundary.TryGetValue("out-of-network", out boundaries);

            if (AQT2D.SN.waterbodies != null)
              for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)
              {
                string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                int WBID = int.Parse(WBString);
                    if (WBID != -9998)
                    {
                        if (File.Exists(BaseDir + WBString + ".GeoJSON"))
                        { GeoJSON = System.IO.File.ReadAllText(BaseDir + WBString + ".GeoJSON"); }
                        else
                        {
                            webView.Visible = false;
                            AddToProcessLog("Reading GEOJSON (map data) from webservice for WB_COMID " + WBString);
                            GeoJSON = AQT2D.ReadWBGeoJSON(WBString);  // read from web service

                            if (GeoJSON.IndexOf("ERROR") >= 0)
                            {
                                AddToProcessLog("Error reading GeoJSON: web service returned: " + GeoJSON);
                                // show process log 
                                if (GeoJSON.IndexOf("Unable to find catchment in database") >= 0) System.IO.File.WriteAllText(BaseDir + WBString + ".GeoJSON", "{}");  //  write to disk
                                continue;
                                // return false;
                            }
                            System.IO.File.WriteAllText(BaseDir + WBString + ".GeoJSON", GeoJSON);  //  write to disk
                        }

                        if ((GeoJSON != "{}") && (webView != null && webView.CoreWebView2 != null))
                        {
                            webView.CoreWebView2.PostWebMessageAsString("ADDWB|" + GeoJSON);
                        }
                    } 
              }


            for (int i = 0; i < AQT2D.SN.order.Length; i++)
                for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                {
                    int COMID = AQT2D.SN.order[i][j];
                    string CString = COMID.ToString();
                    if (File.Exists(BaseDir + CString + ".GeoJSON"))
                    { GeoJSON = System.IO.File.ReadAllText(BaseDir + CString + ".GeoJSON"); }
                    else
                    {
                        webView.Visible = false;
                        AddToProcessLog("Reading GEOJSON (map data) from webservice for COMID " + CString);
                        GeoJSON = AQT2D.ReadGeoJSON(CString);  // read from web service
                        if (GeoJSON.IndexOf("ERROR") >= 0)
                        {
                            AddToProcessLog("Error reading GeoJSON: web service returned: " + GeoJSON);
                            // show process log 
                            if (GeoJSON.IndexOf("Unable to find catchment in database") >= 0) System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", "{}");  //  write to disk 
                            if (GeoJSON.IndexOf("unknown error") >= 0) System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", "{}");  //  write to disk to avoid re-query
                            continue;
                        }
                        System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", GeoJSON);  //  write to disk
                    }

                    if ((GeoJSON != "{}") && (webView != null && webView.CoreWebView2 != null))
                    {
                        webView.CoreWebView2.PostWebMessageAsString("ADD|"+GeoJSON);
                    }


                    //if (GeoJSON == "{}") polyline = null;
                    //else
                    //{
                    //    GeoJSonType coords = JsonConvert.DeserializeObject<GeoJSonType>(GeoJSON);
                    //    try
                    //    {
                    //        polyline = coords.stream_geometry.features[0].geometry.coordinates;
                    //    }
                    //    catch
                    //    {
                    //        AddToProcessLog("Error deserializing GeoJSON  " + BaseDir + CString + ".GeoJSON"); return false;
                    //    }
                    //}

                    List<PointF> startpoints = new List<PointF>();
                    List<PointF> endpoints = new List<PointF>();

                    bool in_waterbody = false;
                    if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.SN.waterbodies.comid_wb.ContainsKey(COMID);



                    //if (polyline != null)
                    //{
                    //    for (int k = 0; k < polyline.Length - 1; k++)
                    //    {
                    //        startpoints.Add(new PointF((float)polyline[k][0], (float)polyline[k][1]));
                    //        endpoints.Add(new PointF((float)polyline[k + 1][0], (float)polyline[k + 1][1]));
                    //    }
                    //    Drawing.Add(new PolyLine(startpoints, endpoints, CString,lcolor,lwidth));

                    //    if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                    //        foreach (int SrcID in Sources)
                    //            if (boundaries.Contains(SrcID))  //ID inflow points with green circles
                    //            {
                    //                Drawing.Add(new Circle((float) polyline[0][0], (float) polyline[0][1], COMID.ToString(), Color.Chartreuse));
                    //            }

                    //    if (i == AQT2D.SN.order.Length-1) //ID pour point with red circle
                    //    {
                    //        Drawing.Add(new Circle((float)polyline[polyline.Length-1][0], (float)polyline[polyline.Length - 1][1], COMID.ToString(), Color.Red));
                    //    }

                    //}
                }

            webView.CoreWebView2.PostWebMessageAsString("RENDER");

            // webView.CoreWebView2.PostWebMessageAsString("COLOR|" + "2648392" + "|red");  this was a test

            webView.Visible = true;
            chart1.Visible = false;

            return true;
        }

        private void PlotButton_Click(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;
            RedrawShapes();
        }

        private void RedrawShapes()
        {
            if (AQT2D == null) return; 

            PlotCOMIDMap(); 

        }


        private void MultiSegForm_ResizeEnd(object sender, EventArgs e)
        {
        }


        private void webView_MouseDown(string COMIDstr)
        {
            {
                if ((outputjump.Enabled) && (outputjump.Checked))
                     ViewOutput(COMIDstr);
                     else EditCOMID(COMIDstr);
            }
            
        }

        private void ConsoleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as RadioButton).Checked) return;

            if (GraphButton.Checked)
            {

                chart1.Visible = true;
                chart1.BringToFront();
                infolabel1.Visible = false;
                infolabel2.Visible = false;
            }
            else if (ConsoleButton.Checked)
            {
                chart1.Visible = false;
                webView.Visible = false;
                infolabel1.Visible = false;
                infolabel2.Visible = false;
            }
            else
            {
                infolabel1.Visible = true;
                infolabel2.Visible = true;
                chart1.Visible = false;
                webView.Visible = true;
                if (!VerifyStreamNetwork()) return;
                //RedrawShapes();
            }
        }

        private void EditCOMID(string CString)
        {
            int COMID = Int32.Parse(CString);
            string BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_2D_" + CString + ".JSON";
            if (ValidFilen(filen, false))
            {
                string json = File.ReadAllText(filen);  //read one segment of multi-seg model
                AQTTestForm AQForm = new AQTTestForm();

                bool isBoundarySeg = false;
                AQT2D.SN.boundary.TryGetValue("out-of-network", out int[] boundaries);
                if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                    foreach (int SrcID in Sources)
                        if (boundaries.Contains(SrcID)) isBoundarySeg = true;

                if (AQForm.EditLinkedInput(ref json, isBoundarySeg)) File.WriteAllText(filen, json);
            }
            else { MessageBox.Show("COMID: " + CString + ".  Linked input for this COMID not yet generated."); };
        }

        private void ViewOutput(string CString)
        {
            string graphjson = "";
            string BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_Run_" + CString + ".JSON";
            string graphfilen = BaseDir + "OutputGraphs"+ ".JSON";

            if (ValidFilen(filen, false))
            {
                string json = File.ReadAllText(filen);  //read one segment of executed multi-seg model
                AQTSim Sim = new AQTSim();
                string err = Sim.Instantiate(json);
                if (err != "") { MessageBox.Show(err); return; }
                Sim.AQTSeg.SetMemLocRec();
                Sim.ArchiveSimulation();
                OutputForm OutForm = new OutputForm();
                OutForm.Text = "Multi-Segment output from " + "AQT_Run_" + CString + ".JSON";

                if (ValidFilen(graphfilen, false))
                {
                    graphjson = File.ReadAllText(graphfilen);  //read user generated graphs
                    Sim.SavedRuns.Values.Last().Graphs = JsonConvert.DeserializeObject<TGraphs> (graphjson);  
                }

                OutForm.ShowOutput(Sim);

                graphjson = JsonConvert.SerializeObject(Sim.SavedRuns.Values.Last().Graphs);
                File.WriteAllText(graphfilen, graphjson);

            }
            else { MessageBox.Show("COMID: " + CString+ ".  Linked output for this COMID not available."); };
        }

        private void webView_MouseHover(object sender, EventArgs e)
        {
            webView.Focus();
        }


        private void browserButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (VerifyBaseDir()) fbd.SelectedPath = Path.GetFullPath(basedirBox.Text);
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    basedirBox.Text = fbd.SelectedPath;
                    basedirBox_Leave(sender, e);
                }    
            }
        }

        private void MultiSegForm_Resize(object sender, EventArgs e)
        {
            // When window state changes
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                MultiSegForm_ResizeEnd(sender, e);
            }

        }

        private void showCOMIDsBox_CheckedChanged(object sender, EventArgs e)
        {
            MultiSegForm_ResizeEnd(sender, e);
        }


        string templatestring = "";
        private void BaseJSONBox_Leave(object sender, EventArgs e)
        {
            if (BaseJSONBox.Text == templatestring) return;

            ScrSettings.BaseJSONstr = BaseJSONBox.Text;
            if (!ValidFilen(BaseJSONBox.Text, true)) return;

            if (VerifyStreamNetwork())
            {
                if (SegmentsCreated()) MessageBox.Show("Selected new Base JSON to use as basis for linked-segment system.  Note that this template will not be applied to the model until 'Create Linked Inputs' is selected, overwriting the existing linked system.");
            }
            SaveScreenSettings();
        }

        private void BaseJSONBox_Enter(object sender, EventArgs e)
        {
            templatestring = BaseJSONBox.Text;
        }

        private void comidBox_Leave(object sender, EventArgs e)
        {
            ScrSettings.COMIDstr = comidBox.Text;
            ScrSettings.EndCOMIDstr = EndCOMIDBox.Text;
            ScrSettings.UpSpanStr = spanBox.Text;

            SaveScreenSettings();
        }

                
        private int ExecuteComidWithinLake(int runID)
        {
            executed.Add(runID);  // add comid to list that is ready to run

            int WBcomid;
            if (!AQT2D.SN.waterbodies.comid_wb.TryGetValue(runID, out WBcomid)) return -9999;
            for (int i = 0; i < AQT2D.SN.waterbodies.comid_wb.Count; i++)
            {
                if (AQT2D.SN.waterbodies.comid_wb.Values.ElementAt(i) == WBcomid)
                    if (!executed.Contains(AQT2D.SN.waterbodies.comid_wb.Keys.ElementAt(i)))
                        return -9999;
            }

            return WBcomid;  
        }

        private void basedirBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                basedirBox_Leave(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        int Step = 0;
        private void button2_Click(object sender, EventArgs e)
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

            int[] outofnetwork;
            AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

            Step++;
            if (Step > AQT2D.SN.order.Length)
            {
                Step = 0;
                // Fixme draw all shapes in gray
                return;
            }


                    try
            {
                    foreach (int runID in AQT2D.SN.order[Step-1])  // step through each COMID in this "order" 
                    {
                    bool in_waterbody = false;
                    if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.SN.waterbodies.comid_wb.ContainsKey(runID);  // is this listed as a lake/res

                    int IDtoRun = runID;
                    if (in_waterbody) IDtoRun = ExecuteComidWithinLake(runID);  // return water body IDtoRun or -9999 if the lake is not ready
                    

                    Color lineColor = Color.Red;
                    if (IDtoRun == -9999) { IDtoRun = runID; lineColor = Color.White; }
                    // fixme draw selected ID in linecolor


                     }  // non-parallel foreach format for debugging

            }
            catch (Exception ex)
            {
                ProcessLog.Text = "Error Updating Drawing: " + ex.Message;
                MessageBox.Show(ex.Message);
                return;
            }


            button2.Text = "Step " + Step.ToString();

            UpdateScreen();

        }

    }
}

