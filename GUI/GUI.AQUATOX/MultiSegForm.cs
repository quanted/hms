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
using SevenZip;
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
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static AQUATOX.AQSim_2D.AQSim_2D;
using System.Globalization;
using System.Collections.Concurrent;

namespace GUI.AQUATOX

{
    public partial class MultiSegForm : Form
    {
        private CancellationTokenSource _cts;
        private AQSim_2D AQT2D = null;
        private int Lake0D = 0;
        private string HUC0D = "";
        public string HAWQS_apikey = "";

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
                catch (Exception ex)
                {
                    if (ex.Message.Contains("logarithmic"))
                    {
                        ChartAreas[0].AxisY.IsLogarithmic = false;
                        MessageBox.Show("Zero or negative values cannot be displayed on a logarithmic scale");
                        base.OnPaint(e);
                        // Code to execute if the exception message includes "logarithmic"
                    }
                    else MessageBox.Show("Error plotting graph: " + ex.Message);
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

            chart1.ChartAreas[0].CursorX.IsUserEnabled = false;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            chart1.ChartAreas[0].CursorY.IsUserEnabled = false;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
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
            ProcessLog.BackColor = SystemColors.Window; // Set the background color to the default window color despite being readonly true
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
                string filestr = @"..\2D_Inputs\HAWQS_data\HUC14\H14_" + content.Substring(3, 2) + ".geojson";
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

            if (BDir != "")
            {
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
            }

            UpdateShortDirNames();
            Properties.Settings.Default.Save();

            RecentFilesBox.DataSource = null;
            RecentFilesBox.DataSource = ShortDirNames;

            RecentFilesBox.SelectionChangeCommitted -= RecentFilesBox_SelectionChangeCommitted;
            RecentFilesBox.SelectedIndex = 0;
            RecentFilesBox.SelectionChangeCommitted += RecentFilesBox_SelectionChangeCommitted;
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

            bool validDirectory = VerifyBaseDir();   // base directory valid?
            bool validJSON = false;
            if (validDirectory) validJSON = VerifyStreamNetwork();  //stream network valid
            bool isLake0D = (Lake0D > 0);
            bool isHUC0D = (HUC0D != "");
            bool inputsegs = false;
            bool HAWQSrun = false;
            DateTime modelrun = DateTime.MinValue;

            if (!validDirectory) setinfolabels("Please Specify a Valid Directory", "", "");

            if (isLake0D || isHUC0D)
            {
                CreateButton.Text = "Read NWM";
                FlowsButton.Text = "Model Params.";
                executeButton.Text = "Run Model";
                OutputLabel.Visible = false;
                mergebutton.Visible = false;
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
                mergebutton.Visible = validJSON;
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
                    DataSourceBox.SelectedIndex = 0;
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
                if (isHUC0D) HAWQSrun = File.Exists(BaseDir + "output_rch_daily.csv");
                else HAWQSrun = File.Exists(BaseDir + "output_rch_disaggregated.csv");
                ReadHAWQSButton.Enabled = HAWQSrun;

                if (inputsegs)
                {   
                    modelrun = ModelRunDate();
                }
            }
            else
            {
                setinfolabels("Empty Directory", "No AQUATOX NWM Model in this directory", "");
                ConsoleButton.Checked = true;
                UpdateRecentFiles("");
                webView.Visible = false;
                ReadHAWQSButton.Enabled = false;
                ChartVisible(false);
            }

            BaseJSONBox.Text = ScrSettings.BaseJSONstr;

            if (modelrun != DateTime.MinValue) StatusLabel.Text = "Run on " + modelrun.ToLocalTime();
            else if (inputsegs) StatusLabel.Text = "Model Ready to Run: Input Segment(s) Created";
            else if (validJSON)
            {
                if (isLake0D) StatusLabel.Text = "Site Selected";
                else if (isHUC0D)
                {
                    if (File.Exists(BaseDir + "output_rch_daily.csv")) StatusLabel.Text = "HAWQS Run Completed";
                    else StatusLabel.Text = "HUC Selected";
                }
                else
                {
                    if (File.Exists(BaseDir + "output_rch_disaggregated.csv")) StatusLabel.Text = "HAWQS Run Completed";
                    StatusLabel.Text = "Stream Network Created";
                }
            }
            else if (validDirectory) StatusLabel.Text = "Model Not Initiated";
            else StatusLabel.Text = "Invalid Directory Specified";

            BaseJSONBox.Enabled = validDirectory;
            ChooseTemplateButton.Enabled = validDirectory;
            SystemInfoPanel.Enabled = validDirectory;
            PlotPanel.Enabled = validJSON;
            TogglePanel.Enabled = validJSON;
            SetupButton.Enabled = validJSON;
            CreateButton.Enabled = validJSON;
            FlowsButton.Enabled = inputsegs;
            HAWQS_button.Enabled = validJSON;
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
                    try
                    {
                        chart1.ChartAreas[0].RecalculateAxesScale();
                    }
                    catch (Exception)
                    {
                        // MessageBox.Show("Axis Resize Error: "+ex.Message);
                    }
                }

                if (!zoomOption.Checked)
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
            if (msg.IndexOf("WARNING", StringComparison.OrdinalIgnoreCase) >= 0) return (WarningsBox.Checked);
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
                    StatusLabel.Visible = true;
                    proglabel.Visible = false;
                    Cancel_Button.Visible = false;
                });
        }

        private bool SegmentsCreated()
        {
            BaseDir = basedirBox.Text;
            string FileN;
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
            else if (HUC0D != "") comid = HUC0D;
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

        string Read_WB_Water_Flows(string WBComidStr, bool daily, string msj)
        {
            int WBComid;
            if (!Int32.TryParse(WBComidStr, out WBComid)) return "";

            string filen = BaseDir + "AQT_Input_" + WBComidStr + ".JSON";

            string errmessage = AQT2D.PopulateLakeRes(WBComid, msj, daily, out string jsondata);

            if (errmessage == "")
            {
                File.WriteAllText(filen, jsondata);
                TSafeAddToProcessLog("INPUT: Read NWM Volumes and Flows and Saved JSON for WaterBody: " + WBComidStr);
                Application.DoEvents();
                return jsondata;
            }
            else
            {
                TSafeAddToProcessLog("ERROR linking NWM Lake Data: " + errmessage);
                UseWaitCursor = false;
                PostWebviewMessage("COLOR|" + WBComid + "|red");
                TSafeHideProgBar();
                ConsoleButton.Checked = true;
                return "";
            }


        }

        async void Read_Water_Flows()  //create a set of 0D jsons for stream network given AQT2D "SN" object
        {

            void ResetInterface()
            {
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    SetInterfaceBusy(false);
                    TSafeHideProgBar();
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
                StatusLabel.Visible = false;
                proglabel.Text = "Reading NWM data";
                proglabel.Visible = true;
                Application.DoEvents();

                SetInterfaceBusy(true);
                await Task.Run(() =>
                {
                    string BaseFileN = FullBaseJSONName();
                    if (BaseFileN == "") return;

                    AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
                    AQT2D.avgRetention = new Dictionary<int, double>();
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

                        List<string> merged = new List<string>();
                        if (AQT2D.SN.merged != null)
                            foreach (var merge in AQT2D.SN.merged)
                                if (merge[1] == comid) merged.Add(merge[0]);  // which other comids have been merged into this one

                        string errmessage = AQT2D.PopulateStreamNetwork(iSeg, msj, merged, out string jsondata);

                        if (errmessage == "")
                        {
                            File.WriteAllText(filen, jsondata);
                            TSafeAddToProcessLog("INPUT: Read Volumes and Flows and Saved JSON for " + comid);
                            if (merged.Count > 0)
                            {
                                TSafeAddToProcessLog("INPUT: Added to " + comid + " merged segments " + string.Join(", ", merged));
                            }
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


                    if (Lake0D > 0) Read_WB_Water_Flows(Lake0D.ToString(), false, msj);
                    else
                    {
                        if (AQT2D.SN.waterbodies != null)
                            for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)
                            {
                                string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                                Read_WB_Water_Flows(WBString, false, msj);
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
            HAWQS_button.Enabled = !busy;
            ReadHAWQSButton.Enabled = !busy;
            executeButton.Enabled = !busy;
            RecentFilesBox.Enabled = !busy;
            basedirBox.Leave -= basedirBox_Leave;
            basedirBox.Enabled = !busy;
            basedirBox.Leave += basedirBox_Leave;
            browseButton.Enabled = !busy;
        }

        private void reset_interface_after_run(bool success)
        {
            this.BeginInvoke((MethodInvoker)(() =>
            {
                UseWaitCursor = false;
                TSafeHideProgBar();
                SetInterfaceBusy(false);
                outputjump.Checked = success;
                UpdateScreen();
            }));
        }


        private bool SetupForRun()
        {
            AddToProcessLog("INFO: Starting model execution...");
            if (AQT2D.archive == null) AQT2D.archive = new Dictionary<int, AQSim_2D.archived_results>();
            AQT2D.archive.Clear();
            AQT2D.SVList = null;

            UseWaitCursor = true;
            progressBar1.Visible = true;
            StatusLabel.Visible = false;
            proglabel.Text = "Model Run Progress";
            proglabel.Visible = true;

            SetInterfaceBusy(true);

            Cancel_Button.Visible = true;
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

            if (!ConfirmOverwrite()) return;

            if (!SetupForRun()) return;

            AddToProcessLog("INPUTS: Model Run Initiated");
            //            try
            {
                if ((Lake0D != 0) || (HUC0D != ""))
                {
                    await Run0DSimulation();
                }
                else
                {
                    await RunStreamNetworkModel();
                }
            }
            //catch (Exception ex)
            //{
            //    HandleModelRunException(ex);
            //}
        }

        private bool ConfirmOverwrite()
        {
            return (ModelRunDate() == DateTime.MinValue) ||
                   MessageBox.Show("Overwrite all existing model-run results in this directory?", "Confirm",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes;
        }

        private async Task Run0DSimulation()
        {
            string segID;
            if (Lake0D != 0) segID = Lake0D.ToString();
            else segID = HUC0D;

            List<string> strout = new();
            bool success = true;
            string json = File.ReadAllText(BaseDir + "AQT_Input_" + segID + ".JSON");

            await Task.Run(() =>
            {
                success = AQT2D.executeModel(long.Parse(segID), MasterSetupJson(), ref strout, ref json, null, null);
                Process_0D_Run(success, strout, json, segID);
            });
        }

        private void Process_0D_Run(bool success, List<string> strout, string json, string segID)
        {
            if (!success)
            {
                strout.ForEach(TSafeAddToProcessLog);
            }
            else
            {
                File.WriteAllText(BaseDir + "AQT_Run_" + segID + ".JSON", json);
                strout.ForEach(TSafeAddToProcessLog);
                TSafeAddToProcessLog("INPUT: Run saved as " + "AQT_Run_" + segID + ".JSON");
            }

            reset_interface_after_run(success);
        }

        private async Task RunStreamNetworkModel()
        {
            int[] outofnetwork = new int[0];
            if (AQT2D.SN.boundary != null)
                AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);

            for (int ordr = 0; ordr < AQT2D.SN.order.Length; ordr++)
            {
                proglabel.Text = $"Step {ordr + 1} of {AQT2D.SN.order.Length}";
                bool success = await ExecuteModelForEachSegment(ordr, outofnetwork);

                UpdateProgress(ordr);

                if (ordr == AQT2D.SN.order.Length - 1)
                    FinalizeModelRun(success);

                if (!success)
                {
                    FinalizeModelRun(false);
                    return;
                }
            }
        }

        private async Task<bool> ExecuteModelForEachSegment(int order, int[] outofnetwork)
        {
            var results = new ConcurrentBag<bool>();  //thread safe storage of model results

            await Task.Run(() => Parallel.ForEach(AQT2D.SN.order[order], runID =>
            {
                bool result = ExecuteSingleSNSegment(runID, outofnetwork);
                results.Add(result);
            }));

            return results.All(r => r);  //returns true if all simulations were successful, otherwise false
        }

        private bool ExecuteSingleSNSegment(int runID, int[] outofnetwork)
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
                return true;

            string runIDstr = IDtoRun.ToString();
            string FileN = BaseDir + "AQT_Input_" + runIDstr + ".JSON";
            if (!ValidFilen(FileN, false))
            {
                TSafeAddToProcessLog("ERROR: File Missing " + FileN);
                UseWaitCursor = false;
                TSafeHideProgBar();
                return false;
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

            bool success = true;
            if (AQT2D.executeModel(IDtoRun, MasterSetupJson(), ref strout, ref json, divergence_flows, outofnetwork))   //run one segment of 2D model
            {
                File.WriteAllText(BaseDir + "AQT_Run_" + runIDstr + ".JSON", json);
                PostWebviewMessage("COLOR|" + runIDstr + "|green");  // draw COMID shape in green after execute
            }
            else
            {
                PostWebviewMessage("COLOR|" + runIDstr + "|red");  // draw COMID shape in red after error
                success = false;
            }

            foreach (string msg in strout)
                TSafeAddToProcessLog(msg); //write update to status log
            return success;
        }

        private void UpdateProgress(int currentStep)
        {
            TSafeUpdateProgress((int)(((float)currentStep / (float)AQT2D.SN.order.Length) * 100.0));
        }

        private void FinalizeModelRun(bool success)
        {
            bindgraphlist();
            OutputPanel.Enabled = true;
            SaveModelRunResults();
            reset_interface_after_run(success);
            if (success) TSafeAddToProcessLog("INPUTS: Model execution complete");
            else TSafeAddToProcessLog("ERROR:  Network Run terminated due to user cancelation or execution error.");
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
            reset_interface_after_run(false);
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
                    if (SegmentsCreated()) MessageBox.Show("The dates of the simulation have been changed.  Note that data from NWM/HAWQS will not be updated with the new date range and applied to the linked system until new NWM data are linked or HAWQS is re-run " +
                        ", overwriting the existing linked system.");

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
            AQTMainForm.OpenUrl(target);
        }


        public class GeoJsonType
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
            public object coordinates { get; set; }

            [JsonIgnore]
            public double[][] LineStringCoordinates
            {
                get => type == "LineString" ? (double[][])coordinates : null;
                set
                {
                    type = "LineString";
                    coordinates = value;
                }
            }

            [JsonIgnore]
            public double[][][] MultiLineStringCoordinates
            {
                get => type == "MultiLineString" ? (double[][][])coordinates : null;
                set
                {
                    type = "MultiLineString";
                    coordinates = value;
                }
            }
        }

        public class GeometryConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Geometry);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jObject = JObject.Load(reader);
                var geometry = new Geometry
                {
                    type = jObject["type"].Value<string>()
                };

                if (geometry.type == "LineString")
                {
                    geometry.coordinates = jObject["coordinates"].ToObject<double[][]>();
                }
                else if (geometry.type == "MultiLineString")
                {
                    geometry.coordinates = jObject["coordinates"].ToObject<double[][][]>();
                }

                return geometry;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var geometry = (Geometry)value;
                var jObject = new JObject
        {
            { "type", geometry.type },
            { "coordinates", JToken.FromObject(geometry.coordinates) }
        };

                jObject.WriteTo(writer);
            }
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

                if (GeoJSON.IndexOf("ERROR") >= 0)  // Fixme, thorough check for valid geojson?
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

                // First, access the "stream_geometry" object
                var streamGeometry = jsonObject["stream_geometry"] as Newtonsoft.Json.Linq.JObject;

                // Then check if this object contains both "type" and "features"
                if (streamGeometry != null)
                {
                    return streamGeometry["type"] != null && streamGeometry["features"] != null;
                }
                else return false;
                // return jsonObject["type"] != null && jsonObject["features"] != null;
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

            if (Lake0D > 0) PlotGeoJSON(Lake0D.ToString(), "WB_COMID", "");  // plot stand alone 0-D lake    fixme, could add name
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
                            GeoJSON = await AQT2D.ReadGeoJSON(CString);
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
                                var settings = new JsonSerializerSettings();
                                settings.Converters.Add(new GeometryConverter());
                                GeoJsonType coords = JsonConvert.DeserializeObject<GeoJsonType>(GeoJSON, settings);

                                // Access the coordinates based on the geometry type
                                var geometry = coords.stream_geometry.features[0].geometry;
                                polyline = (geometry.type == "MultiLineString") ? geometry.MultiLineStringCoordinates[0] : geometry.LineStringCoordinates;
                                if (i == AQT2D.SN.order.Length - 1) polyline = (geometry.type == "MultiLineString") ? geometry.MultiLineStringCoordinates[geometry.MultiLineStringCoordinates.Length - 1] : geometry.LineStringCoordinates;  //place red marker on correct merged segment
                            }
                            catch
                            {
                                AddToProcessLog("ERROR: while deserializing GeoJSON  " + BaseDir + CString + ".GeoJSON"); return;
                            }
                        }

                        if (polyline != null)
                        {
                            string SrcIDList = "";
                            if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                            {
                                foreach (int SrcID in Sources)
                                    if (outofnetwork.Contains(SrcID))  //ID inflow points with green markers
                                    {
                                        if (SrcIDList == "") SrcIDList = SrcID.ToString();
                                        else SrcIDList += ", " + SrcID;
                                    }
                                if (SrcIDList != "") PostWebviewMessage("MARKER|green|" + polyline[0][0] + "|" + polyline[0][1] + "|boundry condition inflow from " + SrcIDList);
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


        private async void webView_MouseDown(string COMIDstr)
        {
            {
                if ((outputjump.Enabled) && (outputjump.Checked))
                    await ViewOutput(COMIDstr);
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

            BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_Input_" + CString + ".JSON";
            if (ValidFilen(filen, false))
            {
                string json = File.ReadAllText(filen);  //read one segment 
                AQTMainForm AQForm = new AQTMainForm();

                bool isBoundarySeg = true;
                if ((Lake0D != 0) && (HUC0D != ""))
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
            else
            {
                if (Lake0D != 0) MessageBox.Show("WBCOMID: " + CString + ".  Model input for this waterbody not yet generated.");
                else if (HUC0D != "") MessageBox.Show("HUC" + HUCStr(CString) + ": " + CString + ".  Model input for this HUC not yet generated.");
                else MessageBox.Show("COMID: " + CString + ".  Linked input for this COMID not yet generated.");
            };
        }

        bool ViewOutputClicked = false;
        async Task ViewOutput(string CString)
        {
            if (ViewOutputClicked) return;
            ViewOutputClicked = true;

            string graphjson = "";
            BaseDir = basedirBox.Text;
            string filen = BaseDir + "AQT_Run_" + CString + ".JSON";
            string graphfilen = BaseDir + "OutputGraphs" + ".JSON";

            if (ValidFilen(filen, false))
            {
                try
                {
                    UseWaitCursor = true;

                    string json = File.ReadAllText(filen);  //read one segment of executed multi-seg model
                    AQTSim Sim = new AQTSim();
                    string err = Sim.Instantiate(json);
                    if (err != "") { MessageBox.Show(err); return; }
                    Sim.AQTSeg.SetMemLocRec();
                    Sim.ArchiveSimulation();
                    OutputForm OutForm = new OutputForm();
                    OutForm.Text = "Output from multi-segment run from " + "AQT_Run_" + CString + ".JSON";

                    if (ValidFilen(graphfilen, false))
                    {
                        graphjson = File.ReadAllText(graphfilen);  //read user generated graphs
                        Sim.SavedRuns.Values.Last().Graphs = JsonConvert.DeserializeObject<TGraphs>(graphjson);
                    }

                    await Task.Run(() => this.Invoke(new MethodInvoker(delegate
                    { OutForm.ShowOutput(Sim, this); })));
                    ViewOutputClicked = false;

                    graphjson = JsonConvert.SerializeObject(Sim.SavedRuns.Values.Last().Graphs);
                    File.WriteAllText(graphfilen, graphjson);
                }
                finally
                {
                    UseWaitCursor = false;
                }
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
                                if (IsHUC) StudyStr = "..\\2D_Inputs\\BaseJSON\\" + "MS_OM.json";
                                else StudyStr = "..\\Studies\\Default Lake.JSON";
                                BSim = new AQTSim();
                                if (File.Exists(StudyStr))
                                    BSim.Instantiate(File.ReadAllText(StudyStr));
                                else
                                {
                                    AddToProcessLog("ERROR: cannot find file " + StudyStr);
                                    return;
                                }
                            }

                            BSim.AQTSeg.PSetup.FirstDay.Val = NSForm.StartDT;    //update start and end date from input on screen
                            BSim.AQTSeg.PSetup.LastDay.Val = NSForm.EndDT;
                            BSim.AQTSeg.PSetup.StepSizeInDays.Val = true;

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
                                BSim.Instantiate(File.ReadAllText("..\\2D_Inputs\\BaseJSON\\" + "MS_OM.json"));
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
                            BSim.AQTSeg.PSetup.StepSizeInDays.Val = true;

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

        private async void viewOutputButton_Click(object sender, EventArgs e)
        {
            if (Lake0D > 0) await ViewOutput(Lake0D.ToString());

            if (HUC0D != "") await ViewOutput(HUC0D);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if ((HAWQS_Sim != null) && (HAWQS_RunStatus != null))
            {
                _ = HAWQS_Sim.CancelProjectExecution(HAWQS_apikey, HAWQS_RunStatus.id);
                TSafeAddToProcessLog("INFO: User Cancelation Requested ");
            }

            if (_cts != null)
            {
                _cts.Cancel();
                reset_interface_after_run(false);
            }
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
            GraphOptPanel.Visible = vis;
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
            if (zoomOption.Checked)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
                chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            }
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


        public class runstatus
        {
            public string id { get; set; }
            public string url { get; set; }
        }

        public string dbpath = @"..\2D_Inputs\HAWQS_data\COMID_HUC14_Unique.sqlite";

        public List<string> ID_Relevant_HUC14s(out string pourpoint)
        {
            try
            {
                List<string> H14s = new List<string>();
                // Identify relevant HUC14s
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbpath))
                {
                    // Use the connection for database operations
                    pourpoint = "";
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
                                        if (i == AQT2D.SN.order.Length - 1) pourpoint = HUC14;  // last to run in the order is the pourpoint
                                    }
                                }
                            }
                    }
                }
                return H14s;
            }
            catch (Exception ex)
            {
                MessageBox.Show("HUC14 Database Error: " + ex.Message);
                pourpoint = "";
                return null;
            }
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


        private List<string> Render_RelevantH14s(out string pourpoint)
        {
            this.Cursor = Cursors.WaitCursor; // Set cursor to wait cursor

            List<string> H14s = ID_Relevant_HUC14s(out pourpoint);
            if (H14s == null) return null;

            TSafeAddToProcessLog("INFO: Relevant HUC14s are " + string.Join(", ", H14s));

            double[] latlons = new double[4];
            for (int i = 0; i < 4; i++)
            {
                if (double.TryParse(BoundStr[i], out double db))
                    latlons[i] = db;
                else
                {
                    if (H14s == null) TSafeAddToProcessLog("WARNING: Could Not Render relevant HUC14s.  GIS error determining site boundaries.");
                    return H14s;
                }
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
                string filestr = @"..\2D_Inputs\HAWQS_data\HUC14\H14_" + code + ".geojson";
                string HUC14layer = File.ReadAllText(filestr);
                string RelevantH14s = SubsetGeoJSON(HUC14layer, H14s);
                PostWebviewMessage("RH14s|" + RelevantH14s);
            }

            this.Cursor = Cursors.Default; // Set cursor back from wait cursor
            return H14s;

        }

        static List<string> MergeLists(List<string> list1, List<string> list2)
        {
            HashSet<string> set1 = new HashSet<string>(list1);
            HashSet<string> set2 = new HashSet<string>(list2);
            set1.UnionWith(set2); // Merge sets and eliminate duplicates
            return set1.ToList();
        }

        Task<string> HAWQStsk;
        Hawqs.Hawqs HAWQS_Sim = null;
        runstatus HAWQS_RunStatus = null;

        public class CSVAggregator
        {
            public static async Task<(string csvContent, string metadata)> DownloadAndAggregateCSV(string fileUrl)
            {
                var tempFilePath = Path.GetTempFileName();
                var outputStringBuilder = new StringBuilder();
                var metadataStringBuilder = new StringBuilder();

                // Download file
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(tempFilePath, FileMode.Create))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        await stream.CopyToAsync(fs);
                    }
                }

                string architecture = Environment.Is64BitProcess ? "x64" : "x86";
                string libraryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), architecture, "7z.dll");
                SevenZipCompressor.SetLibraryPath(libraryPath);

                using (var extractor = new SevenZipExtractor(tempFilePath))  // Extract and aggregate CSVs
                {
                    foreach (var entry in extractor.ArchiveFileData.Where(file => file.FileName.EndsWith(".csv") && file.Size > 170))  // 170 byte files simply contain an error message
                    {
                        using (var entryStream = new MemoryStream())
                        {
                            extractor.ExtractFile(entry.FileName, entryStream);
                            entryStream.Position = 0;

                            using (var reader = new StreamReader(entryStream))
                            {
                                bool isFirstFile = outputStringBuilder.Length == 0;
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    if (isFirstFile || !reader.EndOfStream)
                                    {
                                        outputStringBuilder.AppendLine(line);
                                    }
                                    isFirstFile = false;
                                }
                            }
                        }
                    }

                    foreach (var entry in extractor.ArchiveFileData.Where(file => file.FileName.EndsWith("metadata.txt")))
                    {
                        using (var entryStream = new MemoryStream())
                        {
                            extractor.ExtractFile(entry.FileName, entryStream);
                            entryStream.Position = 0;

                            using (var reader = new StreamReader(entryStream))
                            {
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    if (!reader.EndOfStream)
                                        metadataStringBuilder.AppendLine(line);
                                }
                            }
                        }
                    }
                }

                File.Delete(tempFilePath);

                return (outputStringBuilder.ToString(), metadataStringBuilder.ToString());
            }
        }


        async private void HAWQS_Click(object sender, EventArgs e)
        {
            if (!VerifyStreamNetwork()) return;
            if (Lake0D > 0)
            {
                TSafeAddToProcessLog("ERROR: Lake0D setup for HAWQS not implemented");
                return;
            }

            bool isHUC0D = (HUC0D != "");

            string msj = MasterSetupJson();
            if (msj == "")
            {
                AddToProcessLog("ERROR: MasterSetup.json missing and could not be created.");
                return;
            }
            Setup_Record SR = JsonConvert.DeserializeObject<Setup_Record>(msj);
            if ((SR.FirstDay.Val < new DateTime(2002, 1, 1)) || (SR.LastDay.Val > new DateTime(2020, 12, 31)))
            {
                AddToProcessLog("ERROR: HAWQS Linkage is valid for simulations from 2002 to 2020 at this time.  Choose another date range in Master Setup.");
                return;
            };

            HAWQS_Sim = new();
            AQSim_2D.HAWQSInput HAWQSInp = new();
            AQT2D.HAWQSInf = new();
            AQT2D.HAWQSInf.upriverHUCs = new();
            AQT2D.HAWQSInf.modelDomain = new();
            AQT2D.HAWQSInf.HAWQSboundaryHUCs = new();
            string[] outputHUCs;

            string hucstr;
            if (!isHUC0D)  //streamnetwork code first
            {
                List<string> H14s = Render_RelevantH14s(out string pourpoint);  //identify HUC14s with modeled stream segments within them
                if (H14s == null) return;

                HAWQSInp.downstreamSubbasin = pourpoint;

                HAWQSInp.dataset = "HUC14";
                HAWQSInp.disaggregateComids = true;
                hucstr = "14";
                List<string> outhucs = new();
                AQT2D.HAWQSInf.modelDomain = H14s;

                foreach (string HUC14 in H14s)
                {
                    try { AQT2D.HAWQSInf.LoadFromtoData(HUC14); } //load relevant fromto data to dictionary
                    catch (Exception ex)
                    {
                        AddToProcessLog("ERROR: " + ex.Message); //file system error
                        return;
                    }
                    outhucs = MergeLists(outhucs, AQT2D.HAWQSInf.boundaryHUCs(HUC14, true));  //look up-river one segment and add to HAWQS model output for disaggregation, also add current H14 segment
                }
                outputHUCs = outhucs.ToArray();

                foreach (string HUC14 in outputHUCs)
                {
                    try { AQT2D.HAWQSInf.LoadFromtoData(HUC14); } //load relevant fromto data to dictionary
                    catch (Exception ex)
                    {
                        AddToProcessLog("ERROR: " + ex.Message); //file system error
                        return;
                    }
                    AQT2D.HAWQSInf.AddSourceHUCs(HUC14);   //traverse up-river until a HUC8 boundary is encountered, set that as the HAWQS upstream segment
                }
            }
            else  //HUC0D code
            {
                hucstr = HUCStr(HUC0D);  // string holding "8" to "14"
                HAWQSInp.dataset = "HUC" + hucstr;
                HAWQSInp.downstreamSubbasin = HUC0D;

                AQT2D.HAWQSInf.LoadFromtoData(HUC0D); //load relevant fromto data to dictionary
                AQT2D.HAWQSInf.AddSourceHUCs(HUC0D);
                outputHUCs = AQT2D.HAWQSInf.boundaryHUCs(HUC0D, true).ToArray();
            }

            HAWQSInp.upstreamSubbasins = AQT2D.HAWQSInf.HAWQSboundaryHUCs.ToArray();

            DateTime firstHAWQSday = SR.FirstDay.Val.AddYears(-2);  //default is a two-year spin up
            HAWQSInp.startingSimulationDate = firstHAWQSday.ToString("yyyy-MM-dd");
            HAWQSInp.endingSimulationDate = SR.LastDay.Val.ToString("yyyy-MM-dd");

            HAWQSInp.setHrus.method = "area";
            HAWQSInp.setHrus.units = "km2";

            HAWQSInp.setHrus.target = 1.0;
            if (hucstr == "12") HAWQSInp.setHrus.target = 0.5;
            if (hucstr == "14") HAWQSInp.setHrus.target = 0.1; //TAMU specs


            HAWQSInp.reportData.outputs.rch.subbasins = outputHUCs;

            // ConsoleButton.Checked = true;
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

            string hawqsinput;
            hawqsinput = JsonConvert.SerializeObject(HAWQSInp, Formatting.Indented);
            using (var form = new JSONEditForm(hawqsinput, HAWQS_apikey))
            {
                form.jsonString = hawqsinput;
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                    try
                    {
                        // Try to deserialize to the expected object type to validate JSON
                        (string APIEdit, hawqsinput) = form.GetEditedJson();
                        if (APIEdit == "")
                        {
                            MessageBox.Show("ERROR: A HAWQS API String is required", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (APIEdit != HAWQS_apikey)
                        {
                            Properties.Settings.Default.HAWQS_apikey = APIEdit;
                            Properties.Settings.Default.Save();
                            HAWQS_apikey = APIEdit;
                        }


                        AQSim_2D.HAWQSInput HI = JsonConvert.DeserializeObject<AQSim_2D.HAWQSInput>(hawqsinput);
                        hawqsinput = JsonConvert.SerializeObject(HI, Formatting.None);
                    }
                    catch (JsonReaderException ex)
                    {
                        MessageBox.Show("ERROR: The edited JSON is not valid: " + ex.Message, "Invalid JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                else return;  //cancel press
            }

            UseWaitCursor = true;
            progressBar1.Visible = true;
            StatusLabel.Visible = false;
            proglabel.Text = "HAWQS RUN";
            proglabel.Visible = true;
            Application.DoEvents();
            Cancel_Button.Visible = true;
            SetInterfaceBusy(true);

            AddToProcessLog("INPUTS: HAWQS Input-- " + hawqsinput);
            AddToProcessLog("INFO: Up-river Segments in HAWQS Domain -- " + string.Join(", ", AQT2D.HAWQSInf.upriverHUCs));

            try
            {

                HAWQStsk = HAWQS_Sim.SubmitProject(HAWQS_apikey, hawqsinput);

                await HAWQStsk;

                // AddToProcessLog(tsk.Result + Environment.NewLine);
                HAWQS_RunStatus = JsonConvert.DeserializeObject<runstatus>(HAWQStsk.Result);

                if (HAWQS_RunStatus == null) throw new ArgumentException("HAWQS did not return any results");

                AddToProcessLog("INFO: HAWQS Run ID= " + HAWQS_RunStatus.id);

                int progress = 0;
                while (progress < 100)
                {
                    Thread.Sleep(1000);  // check every one second
                    Task<HawqsStatus> ths = HAWQS_Sim.GetProjectStatus(HAWQS_apikey, HAWQS_RunStatus.id);
                    await (ths);
                    progress = ths.Result.progress;
                    TSafeUpdateProgress(progress);
                }

                Task<List<HawqsOutput>> GPDT = HAWQS_Sim.GetProjectData(HAWQS_apikey, HAWQS_RunStatus.id, false);
                await (GPDT);
                List<HawqsOutput> LHO = GPDT.Result;

                string urlName = "output_rch_daily.csv";
                if (!isHUC0D) urlName = "output_rch_daily_comids.7z";

                if (LHO.Count == 0) AddToProcessLog("WARNING: No HAWQS Results Returned.  For more information visit https://dev-api.hawqs.tamu.edu/#/docs/projects/" + HAWQS_RunStatus.id);
                else
                {
                    string HO = JsonConvert.SerializeObject(LHO);
                    AddToProcessLog("INFO: HAWQS URLs " + (HO));
                    bool foundrch = false;
                    int indx = 0;
                    string url = "";
                    while ((!foundrch) && (indx < LHO.Count))
                    {
                        url = LHO[indx].url;
                        foundrch = url.Contains(urlName);
                        indx++;
                    }

                    if (!foundrch) AddToProcessLog("ERROR: daily reach output " + urlName + " not found in HAWQS data URLs.");
                    else
                    {
                        AddToProcessLog("INFO: Downloading daily reach data from " + url);

                        HttpClient hc = new HttpClient();

                        try
                        {
                            if (!isHUC0D)  // aggregate all the CSVs in the 7z file
                            {
                                (string csvstring, string metadata) = await CSVAggregator.DownloadAndAggregateCSV(url);
                                File.WriteAllText(BaseDir + "output_rch_disaggregated.csv", csvstring);
                                AddToProcessLog("INFO: Saved daily reach data to " + BaseDir + "output_rch_disaggregated.csv");
                                File.WriteAllText(BaseDir + "disaggregation_metadata.csv", metadata);
                            }
                            else //download data for HUC0D
                            {
                                using (var fs = await hc.GetStreamAsync(url))
                                using (var fileStream = new FileStream(BaseDir + "output_rch_daily.csv", FileMode.Create))
                                    await fs.CopyToAsync(fileStream);
                                AddToProcessLog("INFO: Saved daily reach data to " + BaseDir + "output_rch_daily.csv");
                                hc.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            AddToProcessLog("ERROR: when downloading file: " + ex.Message);
                            hc.Dispose();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when running HAWQS: " + ex.Message);
            }

            finally
            {
                HAWQS_Sim = null;
                HAWQS_RunStatus = null;
                SetInterfaceBusy(false);
                TSafeHideProgBar();
                Cancel_Button.Visible = false;
                UseWaitCursor = false;
                UpdateScreen();
            }
        }

        async private void Link_HAWQS_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BaseJSONBox.Text))
            {
                AddToProcessLog("ERROR: Base JSON must be set prior to creating linked segments (upper-left part of Multi-Seg window).");
                return;
            }

            ConsoleButton.Checked = true;
            ChartVisible(false);
            BaseDir = basedirBox.Text;

            if (!VerifyStreamNetwork()) return;

            BaseDir = basedirBox.Text;

            string RCHfileN = BaseDir + "output_rch_daily.csv";
            if (HUC0D == "") RCHfileN = BaseDir + "output_rch_disaggregated.csv";

            if (!File.Exists(RCHfileN))
            {
                AddToProcessLog("ERROR: can't find reach output file: " + RCHfileN);
                return;
            }

            if (Lake0D > 0)
            {
                AddToProcessLog("ERROR: HAWQS linkages are not currently enabled for 0-D lake simulations due to spatial-data mismatch.");
                return;
            }

            if (SegmentsCreated())
                if (MessageBox.Show("Overwrite the existing set of segments and any edits made to the inputs?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            string metadata = "";
            string MetafileN = BaseDir + "disaggregation_metadata.csv";
            if (File.Exists(MetafileN)) metadata = File.ReadAllText(MetafileN);
            string[] metadataLines = metadata.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            AQT2D.avgRetention = new Dictionary<int, double>();
            Dictionary<long, Dictionary<DateTime, HAWQSRCHRow>> HAWQSRchData = new();  //nested dictionary to allow multi-key lookup by ID then date

            void AddHAWQSRchData(long longKey, DateTime dateTimeKey, HAWQSRCHRow data)  //add a reach and date to the nested dictionary
            {
                if (!HAWQSRchData.ContainsKey(longKey))  // Check if the outer dictionary already has the long key
                {
                    HAWQSRchData[longKey] = new Dictionary<DateTime, HAWQSRCHRow>();  // If not, create a new inner dictionary
                }

                HAWQSRchData[longKey][dateTimeKey] = data;  // Now, add or update the data in the inner dictionary
            }

            try  //add data from CSV file to the nested dictionary
            {
                string[] csvlines = File.ReadAllLines(RCHfileN);
                AQT2D.HAWQSInf = new();
                AQT2D.HAWQSInf.colnames = csvlines[0].Split(',');  //read header line, note columns 0-3 have "date" through "sub-basin"
                AQT2D.HAWQSInf.colnames = AQT2D.HAWQSInf.colnames.Skip(4).ToArray();  // colnames now will correspond with row.vals
                for (int i = 1; i < csvlines.Length; i++)
                {
                    string[] rowdata = csvlines[i].Split(',');  //split 
                    if (DateTime.TryParseExact(rowdata[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        HAWQSRCHRow row = new();
                        row.lat = Convert.ToDouble(rowdata[1]);
                        row.lon = Convert.ToDouble(rowdata[2]);
                        long SubBasin = Convert.ToInt64(rowdata[3]);
                        row.vals = new double[rowdata.Length - 4];
                        for (int j = 4; j < rowdata.Length; j++) row.vals[j - 4] = Convert.ToDouble(rowdata[j]);
                        AddHAWQSRchData(SubBasin, date, row);
                    }
                }

                AddToProcessLog("INPUTS: read HAWQS file " + RCHfileN + "   Please wait, linking segments.");
            }

            catch (Exception ex)
            {
                AddToProcessLog("ERROR: when reading CSV file: " + RCHfileN + "; " + ex.Message);
                MessageBox.Show(ex.Message);
                return;
            }


            string BaseFileN = FullBaseJSONName();
            if (BaseFileN == "") return;
            AQT2D.baseSimJSON = File.ReadAllText(BaseFileN);
            string msj = MasterSetupJson();

            UseWaitCursor = true;
            SetInterfaceBusy(true);


            if (HUC0D != "")
            {
                HAWQSInfo HInfo = new();
                HInfo.LoadFromtoData(HUC0D);
                List<string> Boundaries = HInfo.boundaryHUCs(HUC0D, false);  //rch data rows relevant to boundary conditions for this HUC14
                (string AQSimJSON, string errmessage) = await AQT2D.HAWQSRead(HAWQSRchData, Boundaries, HUC0D, msj, true, true, false);

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
            else  //multi-segment HAWQS Read
            {
                Dictionary<string, string> WB_JSONs = new Dictionary<string, string>();
                if ((AQT2D.SN.waterbodies != null) &&  //if there is a NWM lake or reservoir, read volumes and flows from NWM, overland and inflow nutrients will come from HAWQS
                    (AQT2D.SN.waterbodies.wb_table.Length > 1))
                {
                    TSafeAddToProcessLog("INPUT: Reading Lake-Reservoir Volumes and flows from NWM");

                    await Task.Run(() =>
                    {
                        for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)  //index 0 is the header
                        {
                            string WBString = AQT2D.SN.waterbodies.wb_table[i][0];
                            string WBJSON = Read_WB_Water_Flows(WBString, true, msj);  //daily volumes and flows from NWM
                            if (WBJSON != "")
                            {
                                TSafeAddToProcessLog("INPUT: " + WBString + " Lake/Reservoir data for volume and flow read from NWM.  Overland and inflow nutrients will come from HAWQS.");
                                WB_JSONs.Add(WBString, WBJSON);
                            }
                        }
                    });
                }

                int[] boundaries = { };
                int[] outofnetwork = new int[0];
                if (AQT2D.SN.boundary != null) AQT2D.SN.boundary.TryGetValue("out-of-network", out outofnetwork);
                Dictionary<int, int> wbCountTracker = new Dictionary<int, int>();  //tracks number of COMIDS identified to each waterbody so that weighted averaging can be completed

                for (int iSeg = 1; iSeg <= AQT2D.nSegs; iSeg++)
                {
                    string comid = AQT2D.SN.network[iSeg][0];
                    string inpfilen = BaseDir + "AQT_Input_" + comid + ".JSON";
                    int WBCOMID = -1;
                    int wbcount = 0;
                    string WBJSON = "";

                    foreach (string line in metadataLines)
                    {
                        if (line.Contains(comid)) TSafeAddToProcessLog(line); // pass relevant warnings about the disaggregation process for the COMID to the user
                    }

                    bool in_waterbody = false;
                    if ((AQT2D.SN.waterbodies != null) && (AQT2D.SN.waterbodies.comid_wb != null))
                    {
                        WBCOMID = AQT2D.NWM_WaterbodyID(int.Parse(comid));
                        in_waterbody = (WBCOMID != -1);
                        if (in_waterbody)
                        {
                            TSafeAddToProcessLog("INPUT: " + comid + " is not modeled as a stream segment as it is part of a lake/reservoir.  HAWQS inputs added to that waterbody.");
                            WB_JSONs.TryGetValue(WBCOMID.ToString(), out WBJSON);
                            inpfilen = BaseDir + "AQT_Input_" + WBCOMID + ".JSON";  //write data to lake/res input not COMID

                            if (wbCountTracker.ContainsKey(WBCOMID)) wbCountTracker[WBCOMID]++;
                            else wbCountTracker[WBCOMID] = 1;  //this WBCOMID hasn't had any COMIDs assigned to it yet
                            wbcount = wbCountTracker[WBCOMID];
                        }
                    }

                    bool boundaryseg = false;
                    int[] Sources = new int[0];
                    if (AQT2D.SN.sources.TryGetValue(comid, out Sources))
                        foreach (int SrcID in Sources)
                            if (outofnetwork.Contains(SrcID)) boundaryseg = true;   //ID inflow points to get inflow data

                    bool abort_task = false;

                    string errmessage = "";
                    string AQSimJSON = "";
                    if (in_waterbody) (AQSimJSON, errmessage) = await AQT2D.HAWQS_add_COMID_to_WB(HAWQSRchData, Sources.Select(x => x.ToString()).ToList(), comid, wbcount, WBJSON, boundaryseg, true);
                    else (AQSimJSON, errmessage) = await AQT2D.HAWQSRead(HAWQSRchData, Sources.Select(x => x.ToString()).ToList(), comid, msj, boundaryseg, true, true);

                    if (errmessage == "")
                    {
                        File.WriteAllText(inpfilen, AQSimJSON);
                        if (in_waterbody)
                        {
                            TSafeAddToProcessLog("INPUT: Added to Lake/Res " + WBCOMID + " HAWQS overland flows and any boundary condition inputs from COMID " + comid);
                            WB_JSONs[WBCOMID.ToString()] = AQSimJSON;
                        }
                        else TSafeAddToProcessLog("INPUT: Read NHD+ Geometries; Read HAWQS Reach data for Nutrients, OM, and Flows and saved JSON for " + comid);
                        // TSafeUpdateProgress((int)(iSeg / AQT2D.nSegs * 100.0));
                    }
                    else
                    {
                        TSafeAddToProcessLog(errmessage);
                        abort_task = true;
                    }

                    if (abort_task)
                    {
                        UseWaitCursor = false;
                        SetInterfaceBusy(false);
                        return;
                    }
                }
            }

            UseWaitCursor = false;
            SetInterfaceBusy(false);

            TSafeAddToProcessLog("INFO: Model Linkage Complete");
            UpdateScreen();
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

        private void zoomOption_CheckedChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].CursorX.IsUserEnabled = zoomOption.Checked;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = zoomOption.Checked;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = zoomOption.Checked;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = zoomOption.Checked;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = zoomOption.Checked;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = zoomOption.Checked;
        }

        public string SearchDialog(string st)
        {
            Form findDialog = new Form();
            findDialog.Width = 500;
            findDialog.Height = 142;
            findDialog.Text = "Find Text in Log File";
            Label textLabel = new Label() { Left = 10, Top = 20, Text = "Search string:", Width = 100 };
            TextBox inputBox = new TextBox() { Left = 150, Top = 20, Width = 300, Text = st };
            Button search = new Button() { Text = "Find", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            search.Click += (object sender, EventArgs e) => { findDialog.Close(); };
            findDialog.Controls.Add(search);
            findDialog.Controls.Add(textLabel);
            findDialog.Controls.Add(inputBox);
            findDialog.AcceptButton = search;
            findDialog.Shown += (sender, e) => inputBox.Select();
            if (findDialog.ShowDialog() == DialogResult.OK) return inputBox.Text;
            return "";
        }

        private void FindNext()
        {
            int index = ProcessLog.Text.IndexOf(searchTerm, lastSearchIndex, StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                ProcessLog.Select(index, searchTerm.Length);
                ProcessLog.HideSelection = false;
                ProcessLog.ScrollToCaret();
                lastSearchIndex = index + searchTerm.Length;
            }
            else
            {
                MessageBox.Show("No more occurrences found.", "Search Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lastSearchIndex = 0; // Reset search for next query
            }
        }

        int lastSearchIndex = 0;
        string searchTerm = "";
        private void FindButton_Click(object sender, EventArgs e)
        {
            string rs = SearchDialog(searchTerm);
            if (string.IsNullOrEmpty(rs)) return;
            searchTerm = rs;
            FindNext();

        }

        private void ProcessLog_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void ProcessLog_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                e.IsInputKey = true; // Indicates that the key is a regular input key
                FindButton_Click(sender, e);
            }

            if (e.KeyCode == Keys.F3)
            {
                e.IsInputKey = true;
                FindNext();
            }
        }


        private void MergeSegments(string smallSegmentId, string targetSegmentId, HashSet<int> runOrderComids)
        {
            AddToProcessLog($"INPUTS: Merging segment {smallSegmentId} into {targetSegmentId}");

            var newMergeEntry = new string[] { smallSegmentId, targetSegmentId };
            if (AQT2D.SN.merged == null) AQT2D.SN.merged = new string[][] { newMergeEntry };
            else AQT2D.SN.merged = AQT2D.SN.merged.Concat(new string[][] { newMergeEntry }).ToArray();

            // Update network structure
            var smallSegment = AQT2D.SN.network.First(n => n[0] == smallSegmentId);  // identify network array for small
            var targetSegment = AQT2D.SN.network.First(n => n[0] == targetSegmentId);

            targetSegment[4] = (double.Parse(targetSegment[4]) + double.Parse(smallSegment[4])).ToString(); // [4] is length, so sum the two
            targetSegment[5] = (double.Parse(targetSegment[5]) + double.Parse(smallSegment[5])).ToString(); // [5] is travel time, so sum the two

            // Remove small segment from network
            AQT2D.SN.network = AQT2D.SN.network.Where(n => n[0] != smallSegmentId).ToArray();

            // Replace all occurrences of smallSegmentId with targetSegmentId in Sources before removing smallSegmentId
            foreach (var key in AQT2D.SN.sources.Keys.ToList())
            {
                AQT2D.SN.sources[key] = AQT2D.SN.sources[key]
                    .Select(id => id == int.Parse(smallSegmentId) ? int.Parse(targetSegmentId) : id)
                    .ToArray();
            }

            // Update sources
            if (AQT2D.SN.sources.ContainsKey(smallSegmentId))
            {
                // Combine the sources of smallSegmentId into targetSegmentId
                if (AQT2D.SN.sources.ContainsKey(targetSegmentId))
                {
                    AQT2D.SN.sources[targetSegmentId] = AQT2D.SN.sources[targetSegmentId]
                        .Concat(AQT2D.SN.sources[smallSegmentId])
                        .Where(id => id != int.Parse(targetSegmentId)) // Prevent self-referencing
                        .Distinct()
                        .ToArray();
                }
                else
                {
                    AQT2D.SN.sources[targetSegmentId] = AQT2D.SN.sources[smallSegmentId]
                        .Where(id => id != int.Parse(targetSegmentId)) // Prevent self-referencing
                        .ToArray();
                }

                AQT2D.SN.sources.Remove(smallSegmentId);  // now remove the smallSegmentID key
            }

            RebuildOrderBasedOnSources(runOrderComids);  // do this once post merger?

            MergeGeoJsonFiles(BaseDir, smallSegmentId, targetSegmentId);
        }

        private void RebuildOrderBasedOnSources(HashSet<int> originalOrderComids)
        {
            var newOrder = new List<List<int>>();
            var processed = new HashSet<int>();
            var toProcess = new HashSet<int>(originalOrderComids);

            // Start with the last segment in the original order array
            var lastSegmentId = AQT2D.SN.order.Last().Last();
            newOrder.Add(new List<int> { lastSegmentId });
            processed.Add(lastSegmentId);
            toProcess.Remove(lastSegmentId);

            // Define the previous levels
            while (toProcess.Count > 0)
            {
                var previousLevel = new List<int>();

                // Check sources for all segments in the current level
                foreach (var segmentId in newOrder.Last())
                {
                    if (AQT2D.SN.sources.ContainsKey(segmentId.ToString()))
                    {
                        foreach (var sourceId in AQT2D.SN.sources[segmentId.ToString()])
                        {
                            if (originalOrderComids.Contains(sourceId) && !processed.Contains(sourceId))
                            {
                                previousLevel.Add(sourceId);
                                processed.Add(sourceId);
                                toProcess.Remove(sourceId);
                            }
                        }
                    }
                }

                if (previousLevel.Count > 0)
                {
                    newOrder.Add(previousLevel);
                }
                else
                {
                    break; // No more levels to process
                }
            }

            // Reverse the newOrder to have the correct order from first to last
            newOrder.Reverse();

            // Convert List<List<int>> to int[][] and update AQT2D.SN.order
            AQT2D.SN.order = newOrder.Select(step => step.ToArray()).ToArray();
        }

        public int[] GetDownstreamSegments(string segmentId)
        {
            return AQT2D.SN.sources
                .Where(entry => entry.Value.Contains(int.Parse(segmentId)))
                .Select(entry => int.Parse(entry.Key))
                .ToArray();
        }

        private void merge_button_Click(object sender, EventArgs e)
        {
            if (AQT2D.SN == null) return;

            var ttdata = AnalyzeTravelTimes();
            double TTThreshold;
            using (var form = new MergeForm(ttdata))
            {
                if (form.ShowDialog() != DialogResult.OK) return;
                TTThreshold = form.Threshold;
            }

            string PreMergedJSON = JsonConvert.SerializeObject(AQT2D.SN, Formatting.Indented);
            File.WriteAllText(BaseDir + "PreMergedStreamNetwork.JSON", PreMergedJSON);

            if (!VerifyStreamNetwork()) return;

            var runOrderComids = new HashSet<int>(AQT2D.SN.order.SelectMany(o => o).Select(o => int.Parse(o.ToString())));

            // Initialize small segments list
            var smallSegments = GetSmallSegments(TTThreshold, runOrderComids);

            // Process each small segment
            // while (smallSegments.Count > 0)
            {
                foreach (var segmentId in smallSegments.ToList())
                {
                    // Find adjacent segments
                    // var upstreamSegments = AQT2D.SN.sources.ContainsKey(segmentId) ? AQT2D.SN.sources[segmentId] : new int[] { };
                    var downstreamSegment = GetDownstreamSegments(segmentId);
                    //exclusively merge to downstream segment to ensure not merging a main stem to a small tributary.

                    // Filter valid upstream segments
                    //var validUpstreamSegments = upstreamSegments
                    //.Where(id => AQT2D.SN.network.Any(n => n[0] == id.ToString()))
                    //.ToArray();

                    // merge with the downstream segment to ensure flow >= current segment
                    if ((downstreamSegment != null) && (downstreamSegment.Length > 0))
                    {
                        MergeSegments(segmentId, downstreamSegment[0].ToString(), runOrderComids);
                    }
                    /*                    // Try to merge with the smallest valid upstream segment
                                        else if (validUpstreamSegments.Length > 0)
                                        {  // check upstream segments if merging pour point
                                            foreach (var upstreamSegment in validUpstreamSegments.OrderBy(id => double.Parse(AQT2D.SN.network.First(n => n[0] == id.ToString())[4])))
                                            {
                                                if (upstreamSegment.ToString() != segmentId)
                                                {
                                                    MergeSegments(segmentId, upstreamSegment.ToString(), runOrderComids);
                                                    break;
                                                }
                                            }
                                        }
                    */
                }
            }

            string MergedJSON = JsonConvert.SerializeObject(AQT2D.SN, Formatting.Indented);
            File.WriteAllText(BaseDir + "MergedStreamNetwork.JSON", MergedJSON);

            string snJson = JsonConvert.SerializeObject(AQT2D.SN, Formatting.Indented);
            File.WriteAllText(Path.Combine(BaseDir, "StreamNetwork.JSON"), snJson, Encoding.Default);
            AddToProcessLog("INFO: Merged stream network saved to " + Path.Combine(BaseDir, "StreamNetwork.JSON"));

            AQT2D.nSegs = AQT2D.SN.network.Count() - 1;
            PlotButton_Click(sender, e);
            UpdateScreen();

            string Msg = "Segments have been Merged.";
            if (executeButton.Enabled) Msg += "  Linked segments must be re-created before running the model.";
            MessageBox.Show(Msg);

        }

        private (int segmentCount, double averageTravelTime, double minTravelTime, double fifthPctileTravelTime, List<double> TTimes) AnalyzeTravelTimes()
        {
            List<double> travelTimes = new List<double>();

            for (int i = 1; i < AQT2D.SN.network.Length; i++) // element zero contains headers
            {
                double travelTime = double.Parse(AQT2D.SN.network[i][5]); // travel time in days
                travelTimes.Add(travelTime);
            }

            int segmentCount = travelTimes.Count;
            double averageTravelTime = travelTimes.Average();
            double minTravelTime = travelTimes.Min();
            double fifthPctileTravelTime = CalculatePercentile(travelTimes, 5);

            return (segmentCount, averageTravelTime, minTravelTime, fifthPctileTravelTime, travelTimes);
        }

        private double CalculatePercentile(List<double> sortedValues, double percentile)
        {
            sortedValues.Sort();
            double realIndex = percentile / 100.0 * (sortedValues.Count - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;

            if (index + 1 < sortedValues.Count)
            {
                return sortedValues[index] * (1 - frac) + sortedValues[index + 1] * frac;
            }
            else
            {
                return sortedValues[index];
            }
        }

        // get small segments list
        private List<string> GetSmallSegments(double lengthThreshold, HashSet<int> runOrderComids)
        {
            var smallSegments = new List<string>();
            for (int i = 1; i < AQT2D.SN.network.Length; i++) // element zero contains headers
            {
                double length = double.Parse(AQT2D.SN.network[i][5]); // travel time in days
                string comid = AQT2D.SN.network[i][0];
                if (length < lengthThreshold && runOrderComids.Contains(int.Parse(comid)))
                {
                    smallSegments.Add(comid);
                }
            }
            return smallSegments;
        }

        private void MergeGeoJsonFiles(string baseDir, string smallSegmentId, string targetSegmentId)
        {
            // Paths to the GeoJSON files
            string smallSegmentPath = Path.Combine(baseDir, $"{smallSegmentId}.GeoJSON");
            string targetSegmentPath = Path.Combine(baseDir, $"{targetSegmentId}.GeoJSON");

            // Read the GeoJSON files
            JObject smallSegmentGeoJson = JObject.Parse(File.ReadAllText(smallSegmentPath));
            JObject targetSegmentGeoJson = JObject.Parse(File.ReadAllText(targetSegmentPath));

            // Backup the target segment GeoJSON
            string backupPath = Path.Combine(baseDir, $"{targetSegmentId}_pre_merge.GeoJSON");
            File.Copy(targetSegmentPath, backupPath, overwrite: true);

            // Get the coordinates of both segments
            JArray smallSegmentCoordinates = (JArray)smallSegmentGeoJson["stream_geometry"]["features"][0]["geometry"]["coordinates"];
            JObject targetGeometry = (JObject)targetSegmentGeoJson["stream_geometry"]["features"][0]["geometry"];

            // Check if the target geometry is already a MultiLineString
            if (targetGeometry["type"].ToString() == "MultiLineString")
            {
                JArray targetCoordinates = (JArray)targetGeometry["coordinates"];
                targetCoordinates.Insert(0, smallSegmentCoordinates);
            }
            else if (targetGeometry["type"].ToString() == "LineString")
            {
                // Convert to MultiLineString and insert small segment coordinates first
                JArray targetCoordinates = new JArray { smallSegmentCoordinates, targetGeometry["coordinates"] };
                targetGeometry["type"] = "MultiLineString";
                targetGeometry["coordinates"] = targetCoordinates;
            }

            // Update GNIS_NAME
            string targetGnisName = (string)targetSegmentGeoJson["stream_geometry"]["features"][0]["properties"]["GNIS_NAME"];
            string newGnisName;

            if (string.IsNullOrEmpty(targetGnisName)) newGnisName = $" (merged with) {smallSegmentId}";
            else newGnisName = targetGnisName + $" (merged with) {smallSegmentId}";

            int firstIndex = newGnisName.IndexOf(" (merged with) "); // handle potential for names with multiple "(merged with)" substrings
            if (firstIndex != -1)
            {
                int secondIndex = newGnisName.IndexOf(" (merged with) ", firstIndex + 1);
                while (secondIndex != -1)
                {
                    newGnisName = newGnisName.Remove(secondIndex, " (merged with) ".Length).Insert(secondIndex, ", ");
                    secondIndex = newGnisName.IndexOf(" (merged with) ", secondIndex + 1);
                }
            }


            targetSegmentGeoJson["stream_geometry"]["features"][0]["properties"]["GNIS_NAME"] = newGnisName;

            // Save the updated target segment GeoJSON
            File.WriteAllText(targetSegmentPath, targetSegmentGeoJson.ToString());
        }

    }
}

