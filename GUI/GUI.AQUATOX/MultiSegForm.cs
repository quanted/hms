using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using AQUATOX.Loadings;
using AQUATOX.OrgMatter;
using AQUATOX.Volume;
using Data;
using Globals;
using Hawqs;
using Microsoft.Web.WebView2.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Utilities;

namespace GUI.AQUATOX

{
    public partial class MultiSegForm : Form
    {
        private CancellationTokenSource _cts;
        private AQSim_2D AQT2D = null;
        private int Lake0D = 0;
        private string HUC0D = "";

        private FormWindowState LastWindowState = FormWindowState.Minimized;
        private OChart chart1 = new OChart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        DataTable OverlandTable = null;
        private System.Drawing.Graphics graphics;
        private ScreenSettings ScrSettings = new();
        private StringCollection ShortDirNames = new();
        private List<int> executed = new List<int>(); // list of comids that have been asked to execute
        private string BaseDir;
        private string[] BoundStr;
        private string showinglog = "";

        public class OChart : Chart
        {
            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            {
                try
                {
                    base.OnPaint(e);
                }
                catch
                {
                    ChartAreas[0].AxisY.IsLogarithmic = false;
                    System.Windows.Forms.MessageBox.Show("Zero or negative values cannot be displayed on a logarithmic scale");
                    base.OnPaint(e);
                }
            }
        }

        public string HUCStr(string HUC)
        {
            return HUC.Length.ToString();
        }
        public class ScreenSettings
        {
            public string BaseJSONstr = "";
            public string COMIDstr = "";
            public string WBCOMIDstr = "";
            public string HUCChosen = "";
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

        Task webviewready;
        TaskCompletionSource tcs = new TaskCompletionSource();
        Task mapReadyForRender; // JScript signals when map is ready to render

        public MultiSegForm()
        {
            AutoScroll = true;
            InitializeComponent();

            mapReadyForRender = tcs.Task;
            webviewready = InitializeAsync();

            webView.Source = new Uri(Path.Combine(Environment.CurrentDirectory, @"html\leafletMap.html"));

            // 
            // chart1  -----------------------------------------------
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
            ChartVisible(false);
            this.chart1.CustomizeLegend += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.CustomizeLegendEventArgs>(this.chart1_CustomizeLegend_1);

            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);

            Controls.Add(chart1);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{0:#,##0.###}";

            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
            // 
            // end chart1 -----------------------------------------------
            // 

            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            chart1.Location = new System.Drawing.Point(webView.Left, webView.Top);
            chart1.Size = new System.Drawing.Size(webView.Width, webView.Height);

            DataSourceBox.SelectedIndex = 0;
        }


        private Task InitializeAsync()
        {
            return InitializeAsync(webView);
        }

        async Task InitializeAsync(Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.WebMessageReceived += MessageReceived;
            webView.CoreWebView2.ProcessFailed += WebView_ProcessFailed;
        }

        void WebView_ProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            MessageBox.Show("WebView Process Failed");
        }


        void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String content = args.TryGetWebMessageAsString();

            if (content == "DOMContentLoaded")
            {
                tcs.SetResult();  //set mapreadyforrender as complete
            }
            else if (content.StartsWith("H14"))
            {
                string filestr = @"N:\AQUATOX\CSRA\GIS\HUC_14\GeoJSON_split\H14_" + content.Substring(3, 2) + ".geojson";  // fixme deployment
                string HUC14 = File.ReadAllText(filestr);
                webView.CoreWebView2.PostWebMessageAsString("ADDH14|" + HUC14); // display layer
            }
            else if (content.StartsWith("DoneDraw"))
            {
                Application.DoEvents();
                this.Cursor = Cursors.Default; // Reset cursor to default
            }
            else if (content.StartsWith("Bounds"))
            {
                BoundStr = content.Substring(7).Split(',');
            }
            else System.Threading.SynchronizationContext.Current.Post((_) =>
            {
                webView_MouseDown(content);
            }, null);
        }

        protected override void WndProc(ref Message m)
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128) return;
            base.WndProc(ref m);
            // https://stackoverflow.com/questions/8848203/alt-key-causes-form-to-redraw
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        static string PathShortener(string path, int length)
        {
            StringBuilder sb = new StringBuilder();
            PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }

        private void UpdateShortDirNames()
        {
            ShortDirNames.Clear();
            foreach (string str in Properties.Settings.Default.MS_Recent)
                ShortDirNames.Add(PathShortener(str, 28));
        }

        private void UpdateRecentFiles(string BDir)
        {
            if (Properties.Settings.Default == null) return;
            if (Properties.Settings.Default.MS_Recent == null) Properties.Settings.Default.MS_Recent = new();
            int indx = Properties.Settings.Default.MS_Recent.IndexOf(BDir);
            if (indx == -1)
            {
                Properties.Settings.Default.MS_Recent.Insert(0, BDir);
                if (Properties.Settings.Default.MS_Recent.Count > 10)
                    Properties.Settings.Default.MS_Recent.RemoveAt(10);
            }
            else
            {
                Properties.Settings.Default.MS_Recent.RemoveAt(indx);
                Properties.Settings.Default.MS_Recent.Insert(0, BDir);  //move to top
            }

            UpdateShortDirNames();
            Properties.Settings.Default.Save();

            RecentFilesBox.DataSource = null;
            RecentFilesBox.DataSource = ShortDirNames;
        }

        private void SaveScreenSettings()
        {
            try
            {
                if (!VerifyBaseDir()) return;
                BaseDir = basedirBox.Text;
                string ScrString = JsonConvert.SerializeObject(ScrSettings);
                File.WriteAllText(BaseDir + "ScreenSettings.JSON", ScrString);
                UpdateRecentFiles(BaseDir);
            }
            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when saving Screen Settings: " + ex.Message);
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
                BaseDir = basedirBox.Text;
                if (!File.Exists((BaseDir + "ScreenSettings.JSON"))) return;
                string ScrString = File.ReadAllText(BaseDir + "ScreenSettings.JSON");
                ScrSettings = JsonConvert.DeserializeObject<ScreenSettings>(ScrString);

            }
            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when loading Screen Settings: " + ex.Message);
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private void setinfolabels(string s1, string s2, string s3)
        {
            SILabel1.Text = s1;
            SILabel2.Text = s2;
            SILabel3.Text = s3;
        }


        private void UpdateScreen()
        {

            bool validDirectory = VerifyBaseDir();
            bool validJSON = false;
            if (validDirectory) validJSON = VerifyStreamNetwork();
            bool isLake0D = (Lake0D > 0);
            bool isHUC0D = (HUC0D != "");
            bool inputsegs = false;
            DateTime modelrun = DateTime.MinValue;

            if (!validDirectory) setinfolabels("Please Specify a Valid Directory", "", "");

            if (isLake0D || isHUC0D)
            {
                CreateButton.Text = "Read NWM";
                FlowsButton.Text = "Model Params.";
                executeButton.Text = "Run Model";
                OutputLabel.Visible = false;
                OutputPanel.Visible = false;
                GraphButton.Visible = false;
                ShowBoundBox.Visible = false;
                LabelCheckBox.Visible = false;
                NRCheckBox.Visible = false;
                viewOutputButton.Visible = true;
                DataSourceBox.Items.Clear();
                if (isLake0D) DataSourceBox.Items.Add("NWM (flows only)");
                else DataSourceBox.Items.Add("HAWQS Simulation");
                DataSourceBox.SelectedIndex = 0;
            }
            else
            {
                CreateButton.Text = "Create Linked Inputs";
                FlowsButton.Text = "Overland Flows";
                executeButton.Text = "Execute Network";
                OutputLabel.Visible = true;
                OutputPanel.Visible = true;
                GraphButton.Visible = true;
                ShowBoundBox.Visible = true;
                LabelCheckBox.Visible = true;
                NRCheckBox.Visible = true;
                viewOutputButton.Visible = false;
                if (DataSourceBox.Items.Count < 2)
                {
                    DataSourceBox.Items.Clear();
                    DataSourceBox.Items.Add("HAWQS Simulation");
                    DataSourceBox.Items.Add("NWM (flows only)");
                    DataSourceBox.SelectedIndex = 0;  //fixme this could differ if model setup has been completed
                }
            };

            if (validJSON)
            {
                if (isLake0D) setinfolabels("0-D Lake/Reservoir Simulation", "WBCOMID " + Lake0D, "");
                else if (isHUC0D) setinfolabels("HUC" + HUCStr(HUC0D) + " Simulation", "HUC ID " + HUC0D, "");
                else
                {
                    string str3;
                    if (ScrSettings.EndCOMIDstr == "") str3 = "Upstream Span of " + ScrSettings.UpSpanStr + "km";
                    else str3 = "Upstream COMID " + ScrSettings.EndCOMIDstr;
                    setinfolabels("Stream Network Information:", "Pour Point COMID " + ScrSettings.COMIDstr + "; " + str3, AQT2D.SNStats());
                }

                UpdateRecentFiles(basedirBox.Text);
                inputsegs = SegmentsCreated();
                if (inputsegs)
                {
                    modelrun = ModelRunDate();
                }
            }
            else
            {
                setinfolabels("Empty Directory", "No AQUATOX NWM Model in this directory", "");
                ConsoleButton.Checked = true;
                webView.Visible = false;
                ChartVisible(false);

            }

            BaseJSONBox.Text = ScrSettings.BaseJSONstr;

            if (modelrun != DateTime.MinValue) StatusLabel.Text = "Run on " + modelrun.ToLocalTime();
            else if (inputsegs) StatusLabel.Text = "Linked Input Segments Created";
            else if (validJSON)
            {
                if (isLake0D) StatusLabel.Text = "Site Selected";
                else StatusLabel.Text = "Stream Network Created";
            }
            else if (validDirectory) StatusLabel.Text = "Model Not Initiated";
            else StatusLabel.Text = "Invalid Directory Specified";

            BaseJSONBox.Enabled = validDirectory;
            ChooseTemplateButton.Enabled = validDirectory;
            SystemInfoPanel.Enabled = validDirectory;
            PlotPanel.Enabled = validJSON;
            TogglePanel.Enabled = validJSON;
            SetupButton.Enabled = validDirectory;
            CreateButton.Enabled = validJSON;
            FlowsButton.Enabled = inputsegs;
            executeButton.Enabled = inputsegs;
            OutputPanel.Enabled = ArchivedOutput();
            outputjump.Enabled = (modelrun != DateTime.MinValue);
            viewOutputButton.Enabled = (modelrun != DateTime.MinValue);
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

                if (e.Button == MouseButtons.Right)
                {
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


        private string TodaysLogName()
        {
            return BaseDir + "Log_" + DateTime.Now.ToString("MM-dd-yyyy") + ".log";
        }

        private string FormatMsg(string msg)
        {
            return DateTime.Now.ToString("HH:mm:ss") + ": " + msg + Environment.NewLine; ;
        }

        private bool ShowMsg(string msg)
        {
            if (msg.Contains("ERROR")) return (ErrorsBox.Checked);
            if (msg.Contains("WARNING")) return (WarningsBox.Checked);
            if (msg.Contains("INFO")) return (InfoBox.Checked);
            if (msg.Contains("INPUT")) return (InputsBox.Checked);
            return true;
        }


        private void AddToProcessLog(string msg)
        {
            msg = FormatMsg(msg);
            if (ShowMsg(msg)) ProcessLog.AppendText(msg);
            File.AppendAllText(TodaysLogName(), msg);
            Application.DoEvents();
            if (msg.Contains("ERROR")) MessageBox.Show(msg);
        }

        private void TSafeAddToProcessLog(string msg)  //thread safe addition to progress log
        {

            ProcessLog.BeginInvoke((MethodInvoker)delegate ()
            {
                msg = FormatMsg(msg);
                if (ShowMsg(msg)) ProcessLog.AppendText(msg);
                File.AppendAllText(TodaysLogName(), msg);
                //  if (msg.Contains("ERROR")) MessageBox.Show(msg);
            });
        }

        private void TSafeHideProgBar()  //thread safe hide progress bar
        {
            progressBar1.BeginInvoke((MethodInvoker)delegate ()
                {
                    progressBar1.Visible = false;
                    proglabel.Visible = false;
                    CancelButton.Visible = false;
                });
        }

        private bool SegmentsCreated()
        {
            BaseDir = basedirBox.Text;
            string FileN = "";
            if (Lake0D > 0) FileN = BaseDir + "AQT_Input_" + Lake0D.ToString() + ".JSON";
            else if (HUC0D != "") FileN = BaseDir + "AQT_Input_" + HUC0D + ".JSON";
            else
            {
                string comid = AQT2D.SN.network[AQT2D.SN.network.Length - 1][0];  //check last segment
                FileN = BaseDir + "AQT_Input_" + comid.ToString() + ".JSON";
            }
            return (ValidFilen(FileN, false));
        }

        private DateTime ModelRunDate()
        {
            BaseDir = basedirBox.Text;
            string comid;
            if (Lake0D > 0) comid = Lake0D.ToString();
            if (HUC0D != "") comid = HUC0D;
            else comid = AQT2D.SN.network[AQT2D.SN.network.Length - 1][0];
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
            BaseDir = basedirBox.Text;
            return Directory.Exists(BaseDir);
        }

        private bool VerifyStreamNetwork()
        {
            BaseDir = basedirBox.Text;
            if (AQT2D == null) AQT2D = new AQSim_2D();
            if (AQT2D.SN == null)
            {
                Lake0D = 0;
                HUC0D = "";
                try
                {
                    if (!ValidFilen(BaseDir + "StreamNetwork.JSON", false))
                    {
                        return false;
                    }
                    string SNJSON = System.IO.File.ReadAllText(BaseDir + "StreamNetwork.JSON", Encoding.Default);
                    if (SNJSON.Substring(0, 12) == "{\"WBComid\": ")
                    {
                        string intstr = SNJSON.Substring(12, SNJSON.Length - 13);
                        Int32.TryParse(intstr, out Lake0D);
                    }
                    else if (SNJSON.Substring(0, 8) == "{\"HUC\": ")
                    {
                        HUC0D = SNJSON.Substring(8, SNJSON.Length - 9);
                    }
                    else AQT2D.CreateStreamNetwork(SNJSON);
                }
                catch
                {
                    AddToProcessLog("ERROR: Cannot process stream network file " + BaseDir + "StreamNetwork.JSON");
                    return false;
                }
            }
            return true;
        }


        private void createButton_Click(object sender, EventArgs e)  //create a set of 0D jsons for stream network given AQT2D "SN" object
        {
            Read_Water_Flows();
        }

        void Read_WB_Water_Flows(string WBComidStr, string msj)
        {
            int WBComid;
            if (!Int32.TryParse(WBComidStr, out WBComid)) return;

            string filen = BaseDir + "AQT_Input_" + WBComidStr + ".JSON";

            string errmessage = AQT2D.PopulateLakeRes(WBComid, msj, out string jsondata);

            if (errmessage == "")
            {
                File.WriteAllText(filen, jsondata);

                TSafeAddToProcessLog("INPUT: Read Volumes and Flows and Saved JSON for WaterBody: " + WBComidStr);

                Application.DoEvents();
            }
            else
            {
                TSafeAddToProcessLog("ERROR: "+errmessage);
                UseWaitCursor = false;
                PostWebviewMessage("COLOR|" + WBComid + "|red");
                TSafeHideProgBar();
                ConsoleButton.Checked = true;
            }


        }

        async void Read_Water_Flows()  //create a set of 0D jsons for stream network given AQT2D "SN" object
        {

            void ResetInterface()
            {
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    SetInterfaceBusy(false);
                    progressBar1.Visible = false;
                    proglabel.Visible = false;
                    CancelButton.Visible = false;
                    UseWaitCursor = false;
                    UpdateScreen();
                }));
            }

            try
            {
                ConsoleButton.Checked = true;
                ChartVisible(false);
                BaseDir = basedirBox.Text;

                if (!VerifyStreamNetwork()) return;

                if (SegmentsCreated())
                    if (MessageBox.Show("Overwrite the existing set of segments and any edits made to the inputs?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

                AddToProcessLog("INPUTS: Please wait, creating individual AQUATOX JSONS for each segment and reading flow data.");

                UseWaitCursor = true;
                progressBar1.Visible = true;
                proglabel.Text = "Reading NWM data";
                proglabel.Visible = true;
                Application.DoEvents();

                SetInterfaceBusy(true);
                await Task.Run(() =>
                {
                    string BaseFileN = FullBaseJSONName();
                    if (BaseFileN == "") return;

                    AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
                    string msj = MasterSetupJson();

                    for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
                    {
                        string comid = AQT2D.SN.network[iSeg][0];
                        string filen = BaseDir + "AQT_Input_" + comid + ".JSON";

                        bool in_waterbody = false;
                        if (AQT2D.SN.waterbodies != null)
                            if (AQT2D.SN.waterbodies.comid_wb != null) in_waterbody = AQT2D.NWM_Waterbody(int.Parse(comid));
                        if (in_waterbody)
                        {
                            TSafeAddToProcessLog("INPUT: " + comid + " is not modeled as a stream segment as it is part of a lake/reservoir.");
                            continue;
                        }

                        string errmessage = AQT2D.PopulateStreamNetwork(iSeg, msj, out string jsondata);

                        if (errmessage == "")
                        {
                            File.WriteAllText(filen, jsondata);
                            TSafeAddToProcessLog("INPUT: Read Volumes and Flows and Saved JSON for " + comid);
                            TSafeUpdateProgress((int)((float)iSeg / (float)AQT2D.nSegs * 100.0));
                        }
                        else
                        {
                            TSafeAddToProcessLog(errmessage);
                            PostWebviewMessage("COLOR|" + comid + "|red");
                            ResetInterface();
                            return;
                        }
                    }


                    if (Lake0D > 0) Read_WB_Water_Flows(Lake0D.ToString(), msj);
                    else
                    {
                        if (AQT2D.SN.waterbodies != null)
                            for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)
                            {
                                string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                                Read_WB_Water_Flows(WBString, msj);
                            }
                    }

                    if (Lake0D == 0)
                    {
                        if (AQT2D.SN.boundary.TryGetValue("out-of-network", out int[] outofnetwork))
                            if (outofnetwork.Length > 0)
                            {
                                string bnote = "Note: Boundary Condition Flows and State Variable upriver inputs should be added to COMIDs: ";
                                foreach (long bid in outofnetwork) bnote = bnote + bid.ToString() + ", ";
                                TSafeAddToProcessLog("INPUT: " + bnote);
                            }
                    }

                    ResetInterface();

                });

                return;
            }

            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when creating segments: " + ex.Message);
                MessageBox.Show(ex.Message);
                ResetInterface();
                return;
            }
        }

        private void TSafeUpdateProgress(int Prog)
        {
            progressBar1.BeginInvoke((MethodInvoker)delegate ()
            {
                if (Prog < 100) Prog++;  // workaround of animation bug
                Prog = Math.Max(Prog, 1);
                if ((Prog > progressBar1.Value) || (Prog == 1) || (progressBar1.Value == 100)) progressBar1.Value = Math.Max(Prog, 1); // avoid jumping back and forth  
            });
        }

        private void SetInterfaceBusy(bool busy)
        {
            NewProject.Enabled = !busy;
            CreateButton.Enabled = !busy;
            FlowsButton.Enabled = !busy;
            executeButton.Enabled = !busy;
            RecentFilesBox.Enabled = !busy;
            basedirBox.Enabled = !busy;
            browseButton.Enabled = !busy;
        }

        private void reset_interface_after_run()
        {
            UseWaitCursor = false;
            progressBar1.Visible = false;
            proglabel.Visible = false;
            CancelButton.Visible = false;
            SetInterfaceBusy(false);
            UpdateScreen();
        }


        private bool SetupForRun()
        {
            AddToProcessLog("INFO: Starting model execution...");
            if (AQT2D.archive == null) AQT2D.archive = new Dictionary<int, AQSim_2D.archived_results>();
            AQT2D.archive.Clear();

            UseWaitCursor = true;
            progressBar1.Visible = true;
            proglabel.Text = "Model Run Progress";
            proglabel.Visible = true;

            SetInterfaceBusy(true);

            CancelButton.Visible = true;
            var progressHandler = new Progress<int>(value =>
            {
                TSafeUpdateProgress(value);
            });
            var progress = progressHandler as IProgress<int>;
            AQT2D.ProgHandle = progress;

            _cts = new CancellationTokenSource();
            AQT2D.CancelToken = _cts.Token;

            executed.Clear();

            return true;
        }

        private void PostWebviewMessage(string str)
        {
            webView.BeginInvoke((MethodInvoker)(() =>
            {
                webView.CoreWebView2.PostWebMessageAsString(str);
            }));
        }

        async private void executeButton_Click(object sender, EventArgs e)
        {
            ChartVisible(false);
            if (!VerifyStreamNetwork()) return;

            if (IsModelRunOverwriteConfirmed() == false) return;

            if (!SetupForRun()) return;

            AddToProcessLog("INPUTS: Model Run Initiated");
            try
            {
                if (Lake0D != 0)
                {
                    await Run0DLakeOrReservoir();
                }
                else
                {
                    await RunStreamNetworkModel();
                }
            }
            catch (Exception ex)
            {
                HandleModelRunException(ex);
            }
        }

        private bool IsModelRunOverwriteConfirmed()
        {
            return ModelRunDate() != DateTime.MinValue &&
                   MessageBox.Show("Overwrite all existing model-run results in this directory?", "Confirm",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        private async Task Run0DLakeOrReservoir()
        {
            List<string> strout = new();
            bool success = true;
            string json = File.ReadAllText(BaseDir + "AQT_Input_" + Lake0D.ToString());

            await Task.Run(() =>
            {
                success = AQT2D.executeModel(Lake0D, MasterSetupJson(), ref strout, ref json, null, null);
                Handle0DLakeOrReservoirRun(success, strout, json);
            });
        }

        private void Handle0DLakeOrReservoirRun(bool success, List<string> strout, string json)
        {
            if (!success)
            {
                strout.ForEach(TSafeAddToProcessLog);
            }
            else
            {
                File.WriteAllText(BaseDir + "AQT_Run_" + Lake0D.ToString() + ".JSON", json);
                strout.ForEach(TSafeAddToProcessLog);
                TSafeAddToProcessLog("INPUT: Run saved as " + "AQT_Run_" + Lake0D.ToString() + ".JSON");
            }

            reset_interface_after_run();
        }

        private async Task RunStreamNetworkModel()
        {
            int[] outofnetwork = new int[0];
            if (AQT2D.SN.boundary != null)
                AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

            for (int ordr = 0; ordr < AQT2D.SN.order.Length; ordr++)
            {
                proglabel.Text = $"Step {ordr + 1} of {AQT2D.SN.order.Length}";
                await ExecuteModelForEachSegment(ordr, outofnetwork);
                UpdateProgress(ordr);

                if (ordr == AQT2D.SN.order.Length - 1)
                    FinalizeModelRun();
            }
        }

        private async Task ExecuteModelForEachSegment(int order, int[] outofnetwork)
        {
            await Task.Run(() => Parallel.ForEach(AQT2D.SN.order[order], runID =>
            {
                ExecuteSingleSNSegment(runID, outofnetwork);
            }));
        }

        private void ExecuteSingleSNSegment(int runID, int[] outofnetwork)
        // execute a single segment that is part of a stream network
        {
            List<string> strout = new();
            BaseDir = basedirBox.Text;

            bool in_waterbody = false;
            if (AQT2D.SN.waterbodies != null)
                in_waterbody = AQT2D.NWM_Waterbody(runID);

            int IDtoRun = runID;
            if (in_waterbody)
                IDtoRun = ExecuteComidWithinLake(runID);  // return water body IDtoRun or -9999 if the lake is not ready
            if (IDtoRun == -9999)
                return;

            string runIDstr = IDtoRun.ToString();
            string FileN = BaseDir + "AQT_Input_" + runIDstr + ".JSON";
            if (!ValidFilen(FileN, false))
            {
                TSafeAddToProcessLog("ERROR: File Missing " + FileN);
                UseWaitCursor = false;
                TSafeHideProgBar();
                return;
            }
            string json = File.ReadAllText(FileN);  //read one segment of 2D model

            List<ITimeSeriesOutput<List<double>>> divergence_flows = null;  //code block handles divergences
            if (AQT2D.SN.divergentpaths != null)
            {
                if (AQT2D.SN.divergentpaths.TryGetValue(runIDstr, out int[] Divg))
                {
                    foreach (int ID in Divg)
                    {
                        TimeSeriesOutput<List<double>> ITSO = null;
                        string DivSeg = File.ReadAllText(BaseDir + "AQT_Input_" + ID.ToString());  //read the divergent segment of 2D model 
                        AQTSim DivSim = new AQTSim();
                        string outstr = DivSim.Instantiate(DivSeg);
                        if (outstr == "")
                        {
                            if (divergence_flows == null)
                                divergence_flows = new List<ITimeSeriesOutput<List<double>>>();
                            DivSim.AQTSeg.SetMemLocRec();
                            TVolume tvol = DivSim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                            TLoadings InflowLoad = tvol.LoadsRec.Alt_Loadings[0];
                            ITSO = InflowLoad.TimeSeriesAsTSOutput("Divergence Flows", "COMID " + ID.ToString(), 1.0 / 86400.0);  // output flows as m2/s
                        }
                        divergence_flows.Add(ITSO);
                    }
                }
            }

            if (AQT2D.executeModel(IDtoRun, MasterSetupJson(), ref strout, ref json, divergence_flows, outofnetwork))   //run one segment of 2D model
                File.WriteAllText(BaseDir + "AQT_Run_" + runIDstr + ".JSON", json);

            PostWebviewMessage("COLOR|" + runIDstr + "|green");  // draw COMID shape in green after execute

            foreach (string msg in strout)
                TSafeAddToProcessLog(msg); //write update to status log
        }

        private void UpdateProgress(int currentStep)
        {
            TSafeUpdateProgress((int)(((float)currentStep / (float)AQT2D.SN.order.Length) * 100.0));
        }

        private void FinalizeModelRun()
        {
            bindgraphlist();
            OutputPanel.Enabled = true;
            SaveModelRunResults();
            reset_interface_after_run();
            AddToProcessLog("INPUTS: Model execution complete");
        }

        private void SaveModelRunResults()
        {
            string archfilen = basedirBox.Text + "Output_Summary.json";
            string svlistfilen = basedirBox.Text + "SVList.json";
            string arch = JsonConvert.SerializeObject(AQT2D.archive);
            File.WriteAllText(archfilen, arch);
            string svlist = JsonConvert.SerializeObject(AQT2D.SVList);
            File.WriteAllText(svlistfilen, svlist);
        }

        private void HandleModelRunException(Exception ex)
        {
            AddToProcessLog($"ERROR: running linked segments: {ex.Message}");
            MessageBox.Show(ex.Message);
            reset_interface_after_run();
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
                csv.Append(dateList[j].ToShortDateString() + " " + dateList[j].ToShortTimeString() + ",");
                for (i = 0; i < outputs.GetLength(0); i++)
                {
                    csv.Append((outputs[i][j]) + ",");
                }
                csv.Append(Environment.NewLine);
            }

            //ProcessLog.Text = csv.ToString();

            //if (MessageBox.Show("Save CSV to text?", "Confirm",
            //MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

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
            if (SVBox.Items.Count == 0) return;
            unZoom();

            try
            {
                int SVIndex = SVBox.SelectedIndex;

                GraphButton.Checked = true;
                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.Titles.Add(SVBox.Items[SVIndex].ToString());
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

                ChartVisible(true);
            }
            catch (Exception ex)
            {
                AddToProcessLog("ERROR: while rendering chart: " + ex.Message);
                ChartVisible(false);
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
            string BaseJSONFilen = FullBaseJSONName();
            if (BaseJSONFilen == "") return null;

            string json = File.ReadAllText(BaseJSONFilen);

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
                TSafeAddToProcessLog("INPUT: Updated master setup record.  Copied from base JSON to " + msfilen);
                return msr;
            }

        }

        private void SetupButton_Click(object sender, EventArgs e)  //Open the master setup record for editing, save to "Mastersetup" json
        {
            try
            {
                ChartVisible(false);
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
                    if (SegmentsCreated()) MessageBox.Show("The dates of the simulation have been changed.  Note that water flows from the National Water Model will not be updated with the new date range and applied to the linked system until '" +
                        CreateButton.Text + "' is selected, overwriting the existing linked system.");

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
            // button is "model parameters" in 0D mode
            if (Lake0D > 0)
            {
                EditCOMID(Lake0D.ToString());
                return;
            }

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

            BaseDir = basedirBox.Text;

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

                for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
                {
                    string comid = AQT2D.SN.network[iSeg][0];
                    string FileN = BaseDir + "AQT_Input_" + comid + ".JSON";
                    if (!ValidFilen(FileN, false)) continue;  // stream segment not modeled, must be a lake/res segment
                    Add_Row(comid);
                }

                if (AQT2D.SN.waterbodies != null)
                    for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)
                    {
                        string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                        string FileN = BaseDir + "AQT_Input_" + WBString + ".JSON";
                        if (!ValidFilen(FileN, false)) continue;  // stream segment not modeled, must be a lake/res segment
                        Add_Row("WB_" + WBString);
                    }


                //--------------------------------- local procedure Add_Row()
                void Add_Row(string comid)
                {
                    int ColNum = 0;
                    DataRow row = OverlandTable.NewRow();

                    row[0] = comid;

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
                };
                //--------------------------------- local procedure Add_Row()
            }

            GridForm gf = new GridForm();
            gf.Text = "Add Non-Point Source (Overland) Inputs";
            if (!gf.ShowGrid(OverlandTable, true, false, "Overland_Flows")) return;

            for (int iSeg = 0; iSeg < OverlandTable.Rows.Count; iSeg++)
            {
                int ColNum = 0;
                DataRow row = OverlandTable.Rows[iSeg];
                string comid = row[0].ToString();
                comid = comid.Replace("WB_", "");

                string FileN = BaseDir + "AQT_Input_" + comid + ".JSON";

                if (!ValidFilen(FileN, false)) { AddToProcessLog("ERROR: File Missing " + FileN); continue; }

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
                AddToProcessLog("INPUTS: Overwrote non point-source loadings with user input overland-flow inputs for " + comid);
            }
        }

        private void Choose_from_Template_Click(object sender, EventArgs e)

        {
            string lakename;
            AQTSim BSim;

            string filen = NewSimForm.ChooseJSONTemplate(out lakename, out BSim);

            if (filen == "") return;
            BaseJSONBox.Text = filen;
            string BFJSON = JsonConvert.SerializeObject(BSim, AQTSim.AQTJSONSettings());
            File.WriteAllText(basedirBox.Text + filen, BFJSON);    // save back as JSON in project directory

            MessageBox.Show("Selected Template File: " + filen);
            AddToProcessLog("INPUTS: Selected Template File: " + filen);
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
                if (!MapButton2.Checked) MapButton2.Checked = true;
                else RedrawShapes();
            }
            SaveBaseDir();

            showinglog = "";
            logfilen.Visible = false;
            LogOptions_CheckChanged(sender, e);
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
            public LineStringGeometry geometry { get; set; }
        }
        public class LineStringGeometry
        {
            public string type { get; set; }
            public double[][] coordinates { get; set; }
        }


        //string ReadGeoJSON(string comid)
        //{
        //GIS.Operations.Catchment catchment = new GIS.Operations.Catchment(comid,true);
        //object SG = catchment.GetStreamGeometryV2(comid);
        //if (SG == null) return "null";
        //return "not null";

        //return;
        //WSCatchment WSC = new();
        //Task<Dictionary<string, object>> rslt;
        //await Task.Factory.StartNew<>(WSC.Get(WBcomid, streamGeometry: true));
        //   }

        async void PlotGeoJSON(string identifier, string type, string name)
        {
            string GeoJSON;
            BaseDir = basedirBox.Text;
            if (string.IsNullOrEmpty(identifier)) return;

            string fileName = BaseDir + identifier + ".GeoJSON";
            if (File.Exists(fileName))
            {
                GeoJSON = File.ReadAllText(fileName);
            }
            else
            {
                webView.Visible = false;
                AddToProcessLog($"INFO: Reading GEOJSON (map data) from webservice for {type} " + identifier);

                if (type == "HUC")
                {
                    GeoJSON = await AQT2D.ReadHUCGeoJSON("HUC" + HUCStr(identifier), identifier);
                }
                else // WB_COMID
                {
                    GeoJSON = await AQT2D.ReadWBGeoJSON(identifier);
                }

                if (GeoJSON.IndexOf("ERROR") >= 0)  // Fixme, check for valid geojson
                {
                    AddToProcessLog("ERROR: while reading GeoJSON, web service returned: " + GeoJSON);
                    if (type != "HUC" && GeoJSON.IndexOf("Unable to find catchment in database") >= 0)
                        File.WriteAllText(fileName, "{}"); // write empty geojson to disk for WB_COMID
                    return;
                }
                File.WriteAllText(fileName, GeoJSON);
                AddToProcessLog($"INFO: Completed reading GEOJSON (map data) from webservice for {type} " + identifier);
            }

            if (GeoJSON != "{}" && webView != null && webView.CoreWebView2 != null)
            {
                if (type == "WB_COMID")
                {
                    int IDIndex = GeoJSON.IndexOf("\"COMID\":");
                    if (IDIndex == -1)
                    {
                        int IDLoc = GeoJSON.IndexOf("\"GNIS_NAME\"");
                        if (IDLoc > -1) GeoJSON = GeoJSON.Insert(IDLoc, "\"COMID\":" + identifier + ",");
                    }
                }
                GeoJSON = InsertPropertiesInGeoJSON(GeoJSON, identifier, name);
                PostWebviewMessage("ADDWB|" + GeoJSON);
            }
        }

        private bool IsValidGeoJSON(string jsonString)
        {
            try
            {
                var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonString);  // Check for basic GeoJSON structure
                return jsonObject["type"] != null && jsonObject["features"] != null;
            }
            catch
            {
                return false;  // If parsing failed, jsonString is not valid JSON
            }
        }


        public string InsertPropertiesInGeoJSON(string geoJsonString, string id, string name)
        {
            // Parse the GeoJSON string into a JObject
            var geoJson = JObject.Parse(geoJsonString);

            // Check if the root object contains a "features" array
            if (geoJson["features"] is JArray features)
            {
                foreach (var feature in features)
                {
                    // Ensure each feature has a "properties" object
                    var properties = feature["properties"] as JObject ?? new JObject();

                    // Add or update the ID and Name properties
                    properties["ID"] = id;
                    properties["Name"] = name;

                    // Assign the updated properties back to the feature
                    feature["properties"] = properties;
                }
            }

            // Return the modified GeoJSON as a string
            return geoJson.ToString();
        }


        async void PlotCOMIDMap()
        {
            BaseDir = basedirBox.Text;
            string GeoJSON;
            double[][] polyline;

            await mapReadyForRender; // segments can't render until page is loaded

            logfilen.Visible = false;
            webView.Visible = true;
            PostWebviewMessage("ERASE");

            int[] outofnetwork = new int[0];

            if (Lake0D > 0) PlotGeoJSON(Lake0D.ToString(), "WB_COMID", "");  // plot stand alone 0-D lake    fixme, name
            else if (HUC0D != "") PlotGeoJSON(HUC0D, "HUC", "");  // plot stand alone HUC  fixme, name
            else  // loop through lake/res waterbodies if they exist
            {
                if (AQT2D.SN.boundary != null) AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

                if (AQT2D.SN.waterbodies != null)
                    for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)
                    {
                        string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                        PlotGeoJSON(WBString, "WB_COMID", ""); // fixme, name
                    }

                for (int i = 0; i < AQT2D.SN.order.Length; i++)
                    for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                    {
                        int COMID = AQT2D.SN.order[i][j];
                        string CString = COMID.ToString();

                        if (!NRCheckBox.Checked)
                        {   // suppress show un-run COMIDs (those contained in waterbodies)
                            bool in_waterbody = false;
                            if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.NWM_Waterbody(COMID);
                            if (in_waterbody) continue;  // don't plot segments that are superceded by their lake/reservoir waterbody.
                        }

                        if (File.Exists(BaseDir + CString + ".GeoJSON"))
                        { GeoJSON = System.IO.File.ReadAllText(BaseDir + CString + ".GeoJSON"); }
                        else
                        {

                            webView.Visible = false;
                            AddToProcessLog("INFO: Reading GEOJSON (map data) from webservice for COMID " + CString);
                            // GeoJSON = "{}";
                            GeoJSON = AQT2D.ReadGeoJSON(CString);  // read from web service    
                            if (!IsValidGeoJSON(GeoJSON))
                            {
                                AddToProcessLog("ERROR: while reading GeoJSON, web service returned: " + GeoJSON);
                                if (GeoJSON.IndexOf("Unable to find catchment in database") >= 0) System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", "{}");  //  write to disk 
                                if (GeoJSON.IndexOf("unknown error") >= 0) System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", "{}");  //  write to disk to avoid re-query
                                continue;
                            }
                            System.IO.File.WriteAllText(BaseDir + CString + ".GeoJSON", GeoJSON);  //  write to disk
                        }

                        if ((GeoJSON != "{}") && (webView != null && webView.CoreWebView2 != null))
                        {
                            PostWebviewMessage("ADD|" + GeoJSON);
                        }

                        if (GeoJSON == "{}") polyline = null;
                        else
                        {
                            try
                            {
                                GeoJSonType coords = JsonConvert.DeserializeObject<GeoJSonType>(GeoJSON);
                                polyline = coords.stream_geometry.features[0].geometry.coordinates;
                            }
                            catch
                            {
                                AddToProcessLog("ERROR: while deserializing GeoJSON  " + BaseDir + CString + ".GeoJSON"); return;
                            }
                        }

                        if (polyline != null)
                        {
                            if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                                foreach (int SrcID in Sources)
                                    if (outofnetwork.Contains(SrcID))  //ID inflow points with green markers
                                    {
                                        PostWebviewMessage("MARKER|green|" + polyline[0][0] + "|" + polyline[0][1] + "|boundry condition inflow from " + SrcID);
                                    }

                            if (i == AQT2D.SN.order.Length - 1) //ID pour point with red marker
                            {
                                PostWebviewMessage("MARKER|red|" + polyline[polyline.Length - 1][0] + "|" + polyline[polyline.Length - 1][1] + "|pour point");
                            }

                        }
                    }
            }

            PostWebviewMessage("RENDER");

            LabelCheckBox_CheckedChanged(null, null);

            if (!ShowBoundBox.Checked) ShowBoundBox.Checked = true;

            webView.Visible = true;
            ChartVisible(false);
        }

        private void PlotButton_Click(object sender, EventArgs e)
        {
            MapButton2.Checked = true;
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
                ChartVisible(true);
                chart1.BringToFront();
                if (chart1.Series.Count == 0) SVBox_SelectedIndexChanged(sender, e);
                infolabel1.Visible = false;
                infolabel2.Visible = false;
                LogPanel.Visible = false;
                logfilen.Visible = false;
            }
            else if (ConsoleButton.Checked)
            {
                ChartVisible(false);
                webView.Visible = false;
                infolabel1.Visible = false;
                infolabel2.Visible = false;
                LogPanel.Visible = true;
                logfilen.Visible = true;
            }
            else
            {
                infolabel1.Visible = true;
                infolabel2.Visible = true;
                LogPanel.Visible = false;
                logfilen.Visible = false;
                ChartVisible(false);
                webView.Visible = true;
                if (!VerifyStreamNetwork()) return;
                RedrawShapes();
            }
        }

        private void EditCOMID(string CString)
        {

            int COMID = Int32.Parse(CString);
            BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_Input_" + CString + ".JSON";
            if (ValidFilen(filen, false))
            {
                string json = File.ReadAllText(filen);  //read one segment 
                AQTTestForm AQForm = new AQTTestForm();

                bool isBoundarySeg = true;
                if (Lake0D == 0)
                {
                    isBoundarySeg = false;
                    int[] outofnetwork = new int[0];
                    if (AQT2D.SN.boundary != null)
                        AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

                    if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                        foreach (int SrcID in Sources)
                            if (outofnetwork.Contains(SrcID)) isBoundarySeg = true;
                }

                if (AQForm.EditLinkedInput(ref json, isBoundarySeg)) File.WriteAllText(filen, json);
                AddToProcessLog("INPUTS:  Possible user edits made to parameters in segment " + filen);
            }
            else { MessageBox.Show("COMID: " + CString + ".  Linked input for this COMID not yet generated."); };
        }

        private void ViewOutput(string CString)
        {
            string graphjson = "";
            BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_Run_" + CString + ".JSON";
            string graphfilen = BaseDir + "OutputGraphs" + ".JSON";

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
                    Sim.SavedRuns.Values.Last().Graphs = JsonConvert.DeserializeObject<TGraphs>(graphjson);
                }

                OutForm.ShowOutput(Sim);

                graphjson = JsonConvert.SerializeObject(Sim.SavedRuns.Values.Last().Graphs);
                File.WriteAllText(graphfilen, graphjson);

            }
            else { MessageBox.Show("COMID: " + CString + ".  Linked output for this COMID not available."); };
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

        string FullBaseJSONName()
        {
            if (ValidFilen(BaseJSONBox.Text, false)) return BaseJSONBox.Text;
            if (ValidFilen(basedirBox.Text + BaseJSONBox.Text, true)) return basedirBox.Text + BaseJSONBox.Text;
            return "";
        }

        string templatestring = "";  //used to test if BaseJSONBox has changed
        private void BaseJSONBox_Leave(object sender, EventArgs e)
        {
            if (BaseJSONBox.Text == templatestring) return;

            ScrSettings.BaseJSONstr = BaseJSONBox.Text;

            if (FullBaseJSONName() == "") return;

            if (VerifyStreamNetwork())
            {
                string msg = "Selected new Base JSON to use as basis for linked-segment system.  Note that this template will not be applied to the model until '" +
                        CreateButton.Text + "' is selected, overwriting the existing linked system.";

                if (SegmentsCreated()) MessageBox.Show(msg);
                AddToProcessLog("INPUTS: " + msg);
            }
            SaveScreenSettings();
        }

        private void BaseJSONBox_Enter(object sender, EventArgs e)
        {
            templatestring = BaseJSONBox.Text;
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

        int ShowStep = 0;


        private void RecentFilesBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            basedirBox.Text = Properties.Settings.Default.MS_Recent[RecentFilesBox.SelectedIndex];
            basedirBox_Leave(sender, e);
        }

        private void ShowBoundBox_CheckedChanged(object sender, EventArgs e)
        {
            if (webView.CoreWebView2 == null) return;

            PostWebviewMessage("BOUNDS|" + ShowBoundBox.Checked.ToString());
        }

        private void NewProject_Click(object sender, EventArgs e)
        {
            NewSimForm NSForm = new NewSimForm();
            // NSForm.SimType = typeIndex;
            if (NSForm.ShowDialog() == DialogResult.OK)
            {
                string oldbasedir = basedirBox.Text;
                using (var fbd = new FolderBrowserDialog())
                {
                    bool fbd_canceled = false;
                    bool fbd_done = false;

                    while (!fbd_done)
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            basedirBox.Text = fbd.SelectedPath + "\\";
                            BaseDir = basedirBox.Text;
                            string[] directoryFiles = System.IO.Directory.GetFiles(BaseDir, "*.JSON");
                            if (directoryFiles.Length > 0)
                            {
                                if (MessageBox.Show("Directory is not empty, overwrite existing JSON files?", "Confirm",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                                {
                                    foreach (string directoryFile in directoryFiles)
                                    {
                                        System.IO.File.Delete(directoryFile);
                                    }
                                    fbd_done = true;
                                }
                            }
                            else fbd_done = true; // no JSON files in directory 
                        }
                        else
                        {
                            fbd_done = true;
                            fbd_canceled = true;
                            basedirBox.Text = oldbasedir;
                        }
                    }

                    if (!fbd_canceled)
                    {
                        ScrSettings.UpSpanStr = "";
                        ScrSettings.EndCOMIDstr = "";

                        string BaseJSONFileN = NSForm.BaseJSON_FileN;

                        BaseJSONBox.Text = BaseJSONFileN;
                        ScrSettings.COMIDstr = NSForm.COMID;
                        ScrSettings.WBCOMIDstr = NSForm.WBCOMID;
                        ScrSettings.HUCChosen = NSForm.HUCChosen;
                        ScrSettings.BaseJSONstr = BaseJSONFileN;

                        basedirBox.Text = fbd.SelectedPath + "\\";
                        BaseDir = basedirBox.Text;

                        if ((NSForm.WBCOMID != "") || (NSForm.HUCChosen != ""))
                        {
                            bool IsHUC = (NSForm.HUCChosen != "");
                            string SNJSON;
                            if (IsHUC) SNJSON = "{\"HUC\": " + NSForm.HUCChosen + "}";
                            else SNJSON = "{\"WBComid\": " + NSForm.WBCOMID + "}";
                            File.WriteAllText(BaseDir + "StreamNetwork.JSON", SNJSON);

                            string IDstr;
                            if (IsHUC) IDstr = NSForm.HUCChosen; else IDstr = NSForm.WBCOMID;

                            string Geostr;
                            if (NSForm.GeoJSON.TryGetValue(IDstr, out Geostr)) File.WriteAllText(BaseDir + IDstr + ".GeoJSON", Geostr);

                            if (File.Exists(BaseDir + "MasterSetup.json")) System.IO.File.Delete(BaseDir + "MasterSetup.json");

                            AQTSim BSim = NSForm.BSim;
                            if (BSim == null)
                            {
                                string StudyStr;
                                if (IsHUC) StudyStr = "..\\..\\..\\2D_Inputs\\BaseJSON\\" + "MS_OM.json";  // fixme deployment?
                                else StudyStr = "..\\..\\..\\Studies\\Default Lake.JSON";  // fixme deployment?
                                BSim = new AQTSim();
                                if (File.Exists(StudyStr))
                                    BSim.Instantiate(File.ReadAllText(StudyStr));
                                else
                                {
                                    ShowMsg("Error, cannot find file " + StudyStr);
                                    return;
                                }
                            }

                            BSim.AQTSeg.PSetup.FirstDay.Val = NSForm.StartDT;    //update start and end date from input on screen
                            BSim.AQTSeg.PSetup.LastDay.Val = NSForm.EndDT;

                            if ((!IsHUC) && (NSForm.SArea > 0))
                            {
                                BSim.AQTSeg.Location.Locale.SurfArea.Val = NSForm.SArea;  //AQUATOX units are m2
                                BSim.AQTSeg.Location.Locale.SurfArea.Comment = "Estimate from NWM_Lakes_and_Reservoirs web service";
                            }

                            string BFJSON = JsonConvert.SerializeObject(BSim, AQTSim.AQTJSONSettings());
                            if (NSForm.fromtemplate) File.WriteAllText(BaseDir + BaseJSONFileN, BFJSON);    // save template study back as JSON in project directory

                            if (IsHUC) AddToProcessLog("INPUTS: New HUC simulation setup.  HUC: " + NSForm.HUCChosen);
                            else AddToProcessLog("INPUTS: New 0-D lake/reservoir simulation setup.  COMID: " + NSForm.WBCOMID);
                            AddToProcessLog("INPUTS: Base simulation = " + BaseJSONFileN);
                            AddToProcessLog("INPUTS: Start date and end date set from inputs on screen.");
                        }
                        else //NSForm.SNPopulated must be true
                        {
                            string SNJSON = NSForm.ExportSNJSON;
                            File.WriteAllText(BaseDir + "StreamNetwork.JSON", SNJSON);

                            if (File.Exists(BaseDir + "MasterSetup.json")) System.IO.File.Delete(BaseDir + "MasterSetup.json");
                            AQTSim BSim = NSForm.BSim;
                            if (BSim == null)
                            {
                                BSim = new AQTSim();
                                BSim.Instantiate(File.ReadAllText("..\\..\\..\\2D_Inputs\\BaseJSON\\" + "MS_OM.json"));  // fixme deployment?
                            }

                            AQT2D = null;
                            if (VerifyStreamNetwork())
                                for (int i = 0; i < AQT2D.SN.order.Length; i++)
                                    for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                                    {
                                        int COMID = AQT2D.SN.order[i][j];
                                        string CString = COMID.ToString();
                                        string Geostr;
                                        if (NSForm.GeoJSON.TryGetValue(CString, out Geostr))
                                        {
                                            Geostr = Geostr.Replace(@"\", "");
                                            Geostr = "{\"stream_geometry\":{\"type\":\"FeatureCollection\",\"features\":[" + Geostr + "]}}";
                                            File.WriteAllText(BaseDir + CString + ".GeoJSON", Geostr);
                                        }
                                    }

                            BSim.AQTSeg.PSetup.FirstDay.Val = NSForm.StartDT;    //update start and end date from input on screen
                            BSim.AQTSeg.PSetup.LastDay.Val = NSForm.EndDT;

                            string BFJSON = JsonConvert.SerializeObject(BSim, AQTSim.AQTJSONSettings());
                            if (NSForm.fromtemplate) File.WriteAllText(BaseDir + BaseJSONFileN, BFJSON);    // save template study back as JSON in project directory

                            ScrSettings.COMIDstr = NSForm.NScrSettings.COMIDstr;
                            ScrSettings.UpSpanStr = NSForm.NScrSettings.UpSpanStr;
                            ScrSettings.EndCOMIDstr = NSForm.NScrSettings.EndCOMIDstr;

                            AddToProcessLog("INPUTS: New stream network simulation setup.  Pour Point COMID " + ScrSettings.COMIDstr);
                            AddToProcessLog("INPUTS: " + AQT2D.SNStats());
                            AddToProcessLog("INPUTS: Base simulation for reaches = " + BaseJSONFileN);
                            AddToProcessLog("INPUTS: Start date and end date set from inputs on screen. ");
                        }

                        SaveScreenSettings();
                        basedirBox_Leave(sender, e);

                    }
                }
            }
        }

        private void viewOutputButton_Click(object sender, EventArgs e)
        {
            if (Lake0D > 0)
                ViewOutput(Lake0D.ToString());
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (_cts != null)
                _cts.Cancel();

            reset_interface_after_run();
        }

        private void BrowseJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;

            if (openFileDialog1.FileName != "")
            {
                BaseJSONBox.Text = openFileDialog1.FileName;
                BaseJSONBox_Leave(sender, e);
            }
        }

        private void toggleLog_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.IsLogarithmic = !(chart1.ChartAreas[0].AxisY.IsLogarithmic);
        }

        private void ChartVisible(bool vis)
        {
            resetZoom.Visible = vis;
            chart1.Visible = vis;
            GraphLabel.Visible = vis;
            toggleLog.Visible = vis;
        }

        private void unZoom()
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
        }

        private void resetZoom_Click(object sender, EventArgs e)
        {
            unZoom();
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
        }

        private void NRCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PlotCOMIDMap();
        }

        private void LabelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string boolstr = "False";
            if (LabelCheckBox.Checked) boolstr = "True";
            PostWebviewMessage("LABELS|" + boolstr);
        }

        private void TestOrderButtonClick(object sender, EventArgs e)
        {
            {
                ChartVisible(false);

                if (AQT2D == null) AQT2D = new AQSim_2D();
                if (AQT2D.SN == null)
                {
                    BaseDir = basedirBox.Text;
                    if (!ValidFilen(BaseDir + "StreamNetwork.JSON", true)) return;
                    string SNJSON = System.IO.File.ReadAllText(BaseDir + "StreamNetwork.JSON", Encoding.Default);  //read stored streamnetwork (SN object) if necessary
                    AQT2D.SN = JsonConvert.DeserializeObject<AQSim_2D.streamNetwork>(SNJSON);
                    AddToProcessLog("INFO: Read stream network from " + BaseDir + "StreamNetwork.JSON");
                }


                ShowStep++;
                if (ShowStep > AQT2D.SN.order.Length)
                {
                    ShowStep = 0;

                    for (int s2 = 0; s2 < AQT2D.SN.order.Length; s2++)
                        foreach (int runID in AQT2D.SN.order[s2])
                            PostWebviewMessage("COLOR|" + runID.ToString() + "|grey");
                    if (AQT2D.SN.waterbodies != null)
                        foreach (string[] WBID in AQT2D.SN.waterbodies.wb_table)
                            PostWebviewMessage("COLOR|" + WBID[0] + "|grey");
                    executed.Clear();

                    // draw all shapes in gray
                    return;
                }

                try
                {
                    foreach (int runID in AQT2D.SN.order[ShowStep - 1])  // step through each COMID in this "order" 
                    {
                        bool in_waterbody = false;
                        if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.NWM_Waterbody(runID);  // is this identified as a lake/res in streamnetwork

                        int IDtoRun = runID;
                        if (in_waterbody) IDtoRun = ExecuteComidWithinLake(runID);  // return water body IDtoRun or -9999 if the lake is not ready

                        string lineColor = "red";
                        if (IDtoRun == -9999) { IDtoRun = runID; lineColor = "white"; }
                        PostWebviewMessage("COLOR|" + IDtoRun.ToString() + "|" + lineColor);
                    }

                }
                catch (Exception ex)
                {
                    AddToProcessLog("ERROR: while updating drawing: " + ex.Message);
                    MessageBox.Show(ex.Message);
                    return;
                }

                TestOrderButton.Text = "Step " + ShowStep.ToString();

                UpdateScreen();
            }
        }

        private void LogOptions_CheckChanged(object sender, EventArgs e)
        {
            if (showinglog == "") showinglog = TodaysLogName();
            ShowLog();
        }


        private void ShowLog()
        {
            ProcessLog.Clear();

            logfilen.Visible = true;
            logfilen.Text = showinglog;
            if (!File.Exists(showinglog)) return;

            string ScrString = File.ReadAllText(showinglog);
            ProcessLog.Visible = false;
            using (var reader = new StringReader(ScrString))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (ShowMsg(line)) ProcessLog.AppendText(line + Environment.NewLine);
                }
            }
            ProcessLog.Visible = true;
        }

        private void LogChange_Click(object sender, EventArgs e)
        {
            if (LogChange.Text == "Today's Log")
            {
                LogChange.Text = "Change Log";
                showinglog = TodaysLogName();
                ShowLog();
                return;
            }

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = BaseDir;
            openFileDialog1.Filter = "Log File|*.log";
            openFileDialog1.Title = "Open a Log File";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;

            LogChange.Text = "Today's Log";  // allow user to toggle back to today's log
            if (openFileDialog1.FileName != "")
            {
                showinglog = openFileDialog1.FileName;
                ShowLog();
            }

        }


        string apikey = "mL+mO3xTBi1boKpdYHTdBTictavSe4opAvDAXzIAXwA=";  // fixme deployment
        public class runstatus
        {
            public string id { get; set; }
            public string url { get; set; }
        }

        public string dbpath = @"..\..\..\2D_Inputs\COMID_HUC14_Unique.sqlite";  // fixme deployment

        public List<string> ID_Relevant_HUC14s()
        {
            List<string> H14s = new List<string>();
            // Identify relevant HUC14s
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbpath))
            {
                // Use the connection for database operations
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT HUC14 FROM COMID_to_HUC14 WHERE COMID = @Index", connection))
                {
                    for (int i = 0; i < AQT2D.SN.order.Length; i++)
                        foreach (int COMID in AQT2D.SN.order[i])
                        {
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Index", COMID);

                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string HUC14 = reader["HUC14"].ToString();
                                    if (!H14s.Contains(HUC14))
                                        H14s.Add(HUC14);
                                }
                            }
                        }
                }
            }
            return H14s;
        }

        private string SubsetGeoJSON(string GeoJSON_in, List<string> HUC14s)
        {
            var geoJson = JObject.Parse(GeoJSON_in);
            var features = geoJson["features"] as JArray;

            if (features == null)
            {
                return GeoJSON_in; // Return original if no features found
            }

            JArray filteredFeatures = new JArray();
            foreach (var feature in features)
            {
                var properties = feature["properties"];
                if (properties != null && properties["HUC14"] != null)
                {
                    string huc14Value = properties["HUC14"].ToString();
                    if (HUC14s.Contains(huc14Value))
                    {
                        filteredFeatures.Add(feature);
                    }
                }
            }

            geoJson["features"] = filteredFeatures;
            return geoJson.ToString();
        }

        private string LonCode(double lon)
        {
            if (lon > -83.48) return "1";
            else if (lon > -91.72) return "2";
            else if (lon > -99.96) return "3";
            else if (lon > -108.20) return "4";
            else if (lon > -116.44) return "5";
            else return "6";
        }

        private string LatCode(double lat)
        {
            if (lat < 36.0) return "0";
            else if (lat < 41.5) return "1";
            else return "2";
        }


        private void Render_RelevantH14s()
        {
            this.Cursor = Cursors.WaitCursor; // Set cursor to wait cursor

            List<string> H14s = ID_Relevant_HUC14s();
            foreach (string H14 in H14s)
                TSafeAddToProcessLog(H14);

            double[] latlons = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (double.TryParse(BoundStr[i], out double db))
                    latlons[i] = db;
                else return;
            }

            string S_Code = LatCode(latlons[0]);
            string W_Code = LonCode(latlons[1]);
            string N_Code = LatCode(latlons[2]);
            string E_Code = LonCode(latlons[3]);

            HashSet<string> Codes = new HashSet<string>();  //HashSet ensures uniqueness
            Codes.Add(E_Code + N_Code);
            Codes.Add(E_Code + S_Code);
            Codes.Add(W_Code + N_Code);
            Codes.Add(W_Code + S_Code);

            foreach (string code in Codes)
            {
                string filestr = @"N:\AQUATOX\CSRA\GIS\HUC_14\GeoJSON_split\H14_" + code + ".geojson";  // fixme deployment
                string HUC14layer = File.ReadAllText(filestr);
                string RelevantH14s = SubsetGeoJSON(HUC14layer, H14s);
                PostWebviewMessage("RH14s|" + RelevantH14s);
            }

            this.Cursor = Cursors.Default; // Set cursor to wait cursor

        }


        async private void HAWQS_Click(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;
            if (Lake0D > 0) ShowMsg("Lake0D setup TBD");
            bool isHUC0D = (HUC0D != "");

            if (!isHUC0D)
            {
                Render_RelevantH14s();
                ShowMsg("Relevant HUC14s are displayed.  Further model setup for stream network TBD");
                return;
            }

            //HUC0D HAWQS run

            Hawqs.Hawqs hw = new();
            runstatus RS;

            AQSim_2D.HAWQSInput HAWQSInp = new();
            AQSim_2D.HAWQSInfo HAWQSInf = new();

            try
            {
                string hucstr = HUCStr(HUC0D);  // "8" to "14"
                HAWQSInp.dataset = "HUC" + hucstr;
                HAWQSInp.downstreamSubbasin = HUC0D;

                HAWQSInf.sourceHUCs = new();
                HAWQSInf.LoadFromtoData(HUC0D); //load relevant fromto data to dictionary
                HAWQSInf.AddSourceHUCs(HUC0D);
                HAWQSInp.upstreamSubbasins = HAWQSInf.sourceHUCs.ToArray();

                string msj = MasterSetupJson();
                if (msj == "")
                {
                    AddToProcessLog("ERROR: MasterSetup.json missing and could not be created.");
                    return;
                }
                Setup_Record SR = JsonConvert.DeserializeObject<Setup_Record>(msj);

                DateTime firstHAWQSday = SR.FirstDay.Val.AddYears(-2);  //default is a two-year spin up
                HAWQSInp.startingSimulationDate = firstHAWQSday.ToString("yyyy-MM-dd");
                HAWQSInp.endingSimulationDate = SR.LastDay.Val.ToString("yyyy-MM-dd");

                HAWQSInp.setHrus.method = "area";
                HAWQSInp.setHrus.units = "km2";

                HAWQSInp.setHrus.target = 1.0;
                if (hucstr == "12") HAWQSInp.setHrus.target = 0.5;
                if (hucstr == "14") HAWQSInp.setHrus.target = 0.1; //TAMU specs

                ConsoleButton.Checked = true;
                ChartVisible(false);
                BaseDir = basedirBox.Text;


                //if (!VerifyStreamNetwork()) return;

                //if (SegmentsCreated())
                //if (MessageBox.Show("Overwrite the existing set of segments and any edits made to the inputs?", "Confirm",
                //MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

                AddToProcessLog("INPUTS: Initializing HAWQS.");

                if (File.Exists(BaseDir + "output_rch_daily.csv"))
                    if (MessageBox.Show("Overwrite the existing set of HAWQS linkage data?  (" + BaseDir + "output_rch_daily.csv" + ")", "Confirm",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

                UseWaitCursor = true;
                progressBar1.Visible = true;
                proglabel.Text = "HAWQS RUN";
                proglabel.Visible = true;
                Application.DoEvents();
                SetInterfaceBusy(true);
                
                string hawqsinput = JsonConvert.SerializeObject(HAWQSInp);

                AddToProcessLog("INPUTS: HAWQS Input-- "+hawqsinput);

                Task<string> tsk = hw.SubmitProject(apikey, hawqsinput);

                await tsk;

                // AddToProcessLog(tsk.Result + Environment.NewLine);
                RS = JsonConvert.DeserializeObject<runstatus>(tsk.Result);

                if (RS == null) throw new ArgumentException("HAWQS did not return any results");

                AddToProcessLog("INFO: HAWQS Run ID= " + RS.id + Environment.NewLine);

                int progress = 0;
                while (progress < 100)
                {
                    Thread.Sleep(1000);  // check every one second
                    Task<HawqsStatus> ths = hw.GetProjectStatus(apikey, RS.id);
                    await (ths);
                    progress = ths.Result.progress;
                    TSafeUpdateProgress(progress);
                }

                Task<List<HawqsOutput>> GPDT = hw.GetProjectData(apikey, RS.id, false);
                await (GPDT);
                List<HawqsOutput> LHO = GPDT.Result;

                string HO = JsonConvert.SerializeObject(LHO);
                AddToProcessLog((HO) + Environment.NewLine);

                bool foundrch = false;
                int indx = 0;
                string url = "";
                while ((!foundrch) && (indx < LHO.Count))
                {
                    url = LHO[indx].url;
                    foundrch = url.Contains("output_rch_daily.csv");
                    indx++;
                }

                if (!foundrch) AddToProcessLog("ERROR: daily reach output CSV not found in HAWQS data URLs.");
                else
                {
                    AddToProcessLog("INFO: Downloading daily reach data from " + url);

                    HttpClient hc = new HttpClient();

                    try
                    {
                        using (var fs = await hc.GetStreamAsync(url))
                        using (var fileStream = new FileStream(BaseDir + "output_rch_daily.csv", FileMode.Create))
                            await fs.CopyToAsync(fileStream);
                        AddToProcessLog("INFO: Saved daily reach data to " + BaseDir + "output_rch_daily.csv");
                        hc.Dispose();
                    }
                    catch (Exception ex)
                    {
                        AddToProcessLog("ERROR: when downloading file: " + ex.Message);
                        hc.Dispose();
                    }
                }

                SetInterfaceBusy(false);
                progressBar1.Visible = false;
                proglabel.Visible = false;
                CancelButton.Visible = false;
                UseWaitCursor = false;
                UpdateScreen();

                return;
            }

            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when running HAWQS: " + ex.Message);
                SetInterfaceBusy(false);
                progressBar1.Visible = false;
                proglabel.Visible = false;
                CancelButton.Visible = false;
                UseWaitCursor = false;
                UpdateScreen();

                return;
            }

        }

        private void ReadHAWQS_Click(object sender, EventArgs e)
        {
            ConsoleButton.Checked = true;
            ChartVisible(false);
            BaseDir = basedirBox.Text;

            if (!VerifyStreamNetwork()) return;

            BaseDir = basedirBox.Text;
            string filen = BaseDir + "output_rch_daily.csv";
            if (!File.Exists(filen))
            {
                AddToProcessLog("ERROR: can't find reach output file: " + filen);
                return;
            }

            if (Lake0D > 0)
            {
                AddToProcessLog("ERROR: HAWQS linkages are not currently enabled for 0-D lake simulations due to horizontal mismatch.");
                return;
            }

            if (SegmentsCreated())
                if (MessageBox.Show("Overwrite the existing set of segments and any edits made to the inputs?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            string[] colnames;
            
            Dictionary<long, Dictionary<DateTime, HAWQSRCHRow>> HAWQSRchData = new();  //nested dictionary to allow multi-key lookup
            void AddHAWQSRchData(long longKey, DateTime dateTimeKey, HAWQSRCHRow data)
            {
                // Check if the outer dictionary already has the long key
                if (!HAWQSRchData.ContainsKey(longKey))
                {
                    // If not, create a new inner dictionary
                    HAWQSRchData[longKey] = new Dictionary<DateTime, HAWQSRCHRow>();
                }

                // Now, add or update the data in the inner dictionary
                HAWQSRchData[longKey][dateTimeKey] = data;
            }

            try
            {
                string[] csvlines = File.ReadAllLines(filen);
                colnames = csvlines[0].Split(',');  //read header line, note columns 0-3 have "date" through "sub-basin"
                colnames = colnames.Skip(4).ToArray();  // colnames now will correspond with row.vals
                for (int i = 1; i < csvlines.Length; i++)
                {
                    string[] rowdata = csvlines[i].Split(',');  //split 
                    DateTime date = DateTime.ParseExact(rowdata[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    HAWQSRCHRow row = new();
                    row.lat = Convert.ToDouble(rowdata[1]);
                    row.lon = Convert.ToDouble(rowdata[2]);
                    long SubBasin = Convert.ToInt64(rowdata[3]);
                    row.vals = new double[rowdata.Length - 4];
                    for (int j = 4; j < rowdata.Length; j++) row.vals[j - 4] = Convert.ToDouble(rowdata[j]);
                    AddHAWQSRchData(SubBasin, date,  row);
                }

                AddToProcessLog("INPUTS: read HAWQS file " + filen);
            }

            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when reading CSV file: " + filen + "; " + ex.Message);
                MessageBox.Show(ex.Message);
                return;
            }

            string BaseFileN = FullBaseJSONName();
            if (BaseFileN == "") return;
            AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
            string msj = MasterSetupJson();

            if (HUC0D != "")
            {
                string errmessage = AQT2D.HAWQSRead(HAWQSRchData, HUC0D, msj, true, true, out string AQSimJSON);

                if (errmessage == "")
                {
                    string OutFilen = BaseDir + "AQT_Input_" + HUC0D + ".JSON";
                    File.WriteAllText(OutFilen, AQSimJSON);
                    TSafeAddToProcessLog("INPUT: Read HAWQS Data for HUC: " + HUC0D);
                    Application.DoEvents();
                }
                else
                {
                    TSafeAddToProcessLog("ERROR: " + errmessage);
                    ConsoleButton.Checked = true;
                }
            }
        }

        private void ShowH14Box_CheckedChanged(object sender, EventArgs e)
        {
            string showstr = "false";
            if (ShowH14Box.Checked)
            {
                showstr = "true";
                this.Cursor = Cursors.WaitCursor; // Set cursor to wait cursor
            }
            webView.CoreWebView2.PostWebMessageAsString("SHOWH14|" + showstr);
        }

        private void DataSourceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            HAWQSButtonPanel.Visible = DataSourceBox.Text.Contains("HAWQS");
        }
    }
}

