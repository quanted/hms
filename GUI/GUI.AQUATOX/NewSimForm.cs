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
using System.Runtime.InteropServices;
using System.Collections.Specialized;
//using Web.Services.Controllers;

namespace GUI.AQUATOX

{
    public partial class NewSimForm : Form
    {
        // private BackgroundWorker Worker = new BackgroundWorker();  potentially to be added, to report progress
        private AQSim_2D AQT2D = null;

        public int SimType;
        public string WBCOMID = "";
        public string GeoJSON = "";
        public string SimName = "";
        public string BaseJSON = "Default Lake.JSON";

        private FormWindowState LastWindowState = FormWindowState.Minimized;
        private Chart chart1 = new Chart();
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        private System.Drawing.Graphics graphics;
        private ScreenSettings ScrSettings = new();
        private StringCollection ShortDirNames = new();
        private List<int> executed = new List<int>(); // list of comids that have been asked to execute
        private string BaseDir;

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

        Task webviewready;
        TaskCompletionSource tcs = new TaskCompletionSource();
        Task mapReadyForRender; // JScript signals when map is ready to render

        public NewSimForm()
        {
            AutoScroll = true;
            InitializeComponent();

            this.Resize += new System.EventHandler(this.FormResize);

            mapReadyForRender = tcs.Task;
            webviewready = InitializeAsync();

            webView.Source = new Uri(Path.Combine(Environment.CurrentDirectory, @"html\newSimMap.html"));

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
            String content = args.WebMessageAsJson; //  TryGetWebMessageAsString();

            if (content == "\"DOMContentLoaded\"")
            {
                tcs.SetResult();  //set mapreadyforrender as complete
            }
            else if (content.StartsWith("\"INFO|"))
            {
                System.Threading.SynchronizationContext.Current.Post((_) => {
                    webView_MouseDown(content.Trim('"')); }, null);
            }
            else GeoJSON = content; 
                
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

        }


        private void UpdateScreen()
        {
            comidBox.Text = ScrSettings.COMIDstr;
            EndCOMIDBox.Text = ScrSettings.EndCOMIDstr;
            spanBox.Text = ScrSettings.UpSpanStr;
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


        private void ReadNetwork_Click(object sender, EventArgs e) // initializes the AQT2D object, reads the stream network from web services, saves the stream network object
        {
            // if (VerifyStreamNetwork())
                if (MessageBox.Show("Overwrite the existing stream network and any inputs and outputs?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No) return;

            string[] directoryFiles = System.IO.Directory.GetFiles(BaseDir, "*.JSON");
            foreach (string directoryFile in directoryFiles)
            {
                System.IO.File.Delete(directoryFile);
            }

            UpdateScreen();
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

        public static string ChooseLakeTemplate(out string lakename)
        {
            lakename = "";
            string fileN = Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates.json");
            if (!File.Exists(fileN)) return "";
            string json = File.ReadAllText(fileN);

            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            GridForm gf = new GridForm();

            if (gf.SelectRow(table, "SurrogateLakes"))
            {
                lakename = gf.chosenlake;
                return gf.chosenfileN;
            }
            else return "";
        }

        private void Choose_from_Template_Click(object sender, EventArgs e)
        {
            string lakename;
            string lakefilen = ChooseLakeTemplate(out lakename);    
            if (lakefilen == "") return;

            SimBaseLabel.Text = "Simulation Base: "+lakename;
            BaseJSON = lakefilen;
            SimJSONLabel.Text = "\"" + BaseJSON + "\"";
        }

        private void NewSimForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // SaveScreenSettings();
            // SaveBaseDir();


        }

        private void NewSimForm_Shown(object sender, EventArgs e)
        {
            //basedirBox.Text = Properties.Settings.Default.MS_Directory;
            UpdateScreen();
        }

        private void HelpButton2_Click(object sender, EventArgs e)
        {
            string target = "Multi_Segment_Runs";  //fixme new help topic
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

        async void PlotCOMIDMap()
        {
            string GeoJSON;
            double[][] polyline = null;


            await mapReadyForRender; // segments can't render until page is loaded

//          if (webView.CoreWebView2 == null)
//          { await webviewready; } //ensure webview initialized
//          await webView.CoreWebView2.DOMContentLoaded(null);
//          MessageBox.Show("Rendering Now");

            webView.Visible = true;
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
                            //AddToProcessLog("Reading GEOJSON (map data) from webservice for WB_COMID " + WBString);
                            GeoJSON = AQT2D.ReadWBGeoJSON(WBString);  // read from web service

                            if (GeoJSON.IndexOf("ERROR") >= 0)
                            {
                              //  AddToProcessLog("Error reading GeoJSON: web service returned: " + GeoJSON);
                                // show process log 
                                if (GeoJSON.IndexOf("Unable to find catchment in database") >= 0) System.IO.File.WriteAllText(BaseDir + WBString + ".GeoJSON", "{}");  //  write to disk
                                continue;
                                // return false;
                            }
                            System.IO.File.WriteAllText(BaseDir + WBString + ".GeoJSON", GeoJSON);  //  write to disk
                        }

                        if ((GeoJSON != "{}") && (webView != null && webView.CoreWebView2 != null))
                        {
                            int IDIndex = GeoJSON.IndexOf("\"COMID\":");
                            if (IDIndex == -1) { 
                                int IDLoc = GeoJSON.IndexOf("\"GNIS_NAME\"");
                                if (IDLoc > -1) GeoJSON = GeoJSON.Insert(IDLoc, "\"COMID\":"+WBString+",");
                            } 

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
                        //AddToProcessLog("Reading GEOJSON (map data) from webservice for COMID " + CString);
                        GeoJSON = AQT2D.ReadGeoJSON(CString);  // read from web service
                        if (GeoJSON.IndexOf("ERROR") >= 0)
                        {
                            //AddToProcessLog("Error reading GeoJSON: web service returned: " + GeoJSON);
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

                    if (GeoJSON == "{}") polyline = null;
                    else
                    {
                        GeoJSonType coords = JsonConvert.DeserializeObject<GeoJSonType>(GeoJSON);
                        try
                        {
                            polyline = coords.stream_geometry.features[0].geometry.coordinates;
                        }
                        catch
                        {
                            //AddToProcessLog("Error deserializing GeoJSON  " + BaseDir + CString + ".GeoJSON"); return;
                        }
                    } 

                    List<PointF> startpoints = new List<PointF>();
                    List<PointF> endpoints = new List<PointF>();

                    bool in_waterbody = false;
                    if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.SN.waterbodies.comid_wb.ContainsKey(COMID);

                    if (polyline != null)
                    {
                        if (AQT2D.SN.sources.TryGetValue(CString, out int[] Sources))
                            foreach (int SrcID in Sources)
                                if (boundaries.Contains(SrcID))  //ID inflow points with green circles
                                {
                                    webView.CoreWebView2.PostWebMessageAsString("MARKER|green|" + polyline[0][0] + "|" + polyline[0][1]+ "|boundry condition inflow from "+SrcID);
                                }

                        if (i == AQT2D.SN.order.Length-1) //ID pour point with red circle
                        {
                            webView.CoreWebView2.PostWebMessageAsString("MARKER|red|" + polyline[polyline.Length - 1][0] + "|" +polyline[polyline.Length - 1][1] + "|pour point");
                        }

                    }
                }

            webView.CoreWebView2.PostWebMessageAsString("RENDER");

            webView.Visible = true;
            chart1.Visible = false;
           
        }


        private void RedrawShapes()
        {
            if (AQT2D == null) return; 

            PlotCOMIDMap(); 
        }


        private void NewSimForm_ResizeEnd(object sender, EventArgs e)
        {
        }


        private void webView_MouseDown(string COMIDstr)
            
        {
            string[] msg = COMIDstr.Split('|');
            WBCOMID = msg[1];

            if (msg.Length > 2) SimName = msg[2];
            else SimName = "WBCOMID: " + WBCOMID;
            if (SimName == " ") SimName = "WBCOMID: " + WBCOMID;


            SimNameEdit.Text = SimName;
            WBCLabel.Text = "WBCOMID: "+ WBCOMID;
        }



        private void webView_MouseHover(object sender, EventArgs e)
        {
            webView.Focus();
        }

        private void NewSimForm_Resize(object sender, EventArgs e)
        {
            // When window state changes
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                NewSimForm_ResizeEnd(sender, e);
            }

        }

        private void showCOMIDsBox_CheckedChanged(object sender, EventArgs e)
        {
            NewSimForm_ResizeEnd(sender, e);
        }


        private void BaseJSONBox_Leave(object sender, EventArgs e)
        {
            
        }

        private void comidBox_Leave(object sender, EventArgs e)
        {
            ScrSettings.COMIDstr = comidBox.Text;
            ScrSettings.EndCOMIDstr = EndCOMIDBox.Text;
            ScrSettings.UpSpanStr = spanBox.Text;

            //SaveScreenSettings();
        }
               


    }
}

