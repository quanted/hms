using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;

namespace GUI.AQUATOX

{
    public partial class NewSimForm : Form
    {
        // private BackgroundWorker Worker = new BackgroundWorker();  potentially to be added, to report progress
        private AQSim_2D AQT2D = null;

        public string COMID = "";
        public Dictionary<string, string> GeoJSON = new();
        public string ExportSNJSON = "";
        public string SimName = "";
        public double SArea = -9999;  // surface area in square meters as taken from waterbodies object
        public string BaseJSON_FileN = "Default Lake.JSON";
        public AQTSim BSim = null;
        public DateTime StartDT;  // start and end simulation time 
        public DateTime EndDT;
        public bool SNPopulated = false;
        public bool LakeSelected = false;

        static Lake_Surrogates LS = null;

        public NScreenSettings NScrSettings = new();

        static public JsonSerializerSettings LSJSONSettings()
        {
            LSKnownTypesBinder LSBinder = new LSKnownTypesBinder();
            return new JsonSerializerSettings()
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = LSBinder
            };
        }

        public class NScreenSettings
        {
            public string COMIDstr = "";
            public string EndCOMIDstr = "";
            public string UpSpanStr = "";
        }

        Task webviewready;
        TaskCompletionSource tcs = new TaskCompletionSource();
        Task mapReadyForRender; // JScript signals when map is ready to render

        public NewSimForm()
        {
            AutoScroll = true;
            InitializeComponent();

            mapReadyForRender = tcs.Task;
            webviewready = InitializeAsync();

            webView.Source = new Uri(Path.Combine(Environment.CurrentDirectory, @"html\newSimMap.html"));

            this.ResumeLayout(false);
            this.PerformLayout();
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
            String content = (args.WebMessageAsJson).Trim('"'); //  TryGetWebMessageAsString();

            if (content == "DOMContentLoaded")
            {
                tcs.SetResult();  //set mapreadyforrender as complete
            }
            else if ((content.StartsWith("LAKE|")) ||
                (content.StartsWith("FL|")) ||
                (content.StartsWith("FL2|")))
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    webView_MouseDown((content.StartsWith("LAKE|")), content);
                }, null);
            }
            else
            {
                string[] split = content.Split('|');
                split[1] = split[1].Replace("\\\"", "\"");   // lake geojsons included \ character in strings
                if (!GeoJSON.ContainsKey(split[0]))  GeoJSON.Add(split[0], split[1]);
            }
                
        }

        protected override void WndProc(ref Message m)  
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128) return;
            base.WndProc(ref m);
            // https://stackoverflow.com/questions/8848203/alt-key-causes-form-to-redraw
        }

        private void UpdateScreen()
        {
            ReadNetworkPanel.Visible = StreamButton.Checked;
            NetworkLabel.Visible = StreamButton.Checked;

            comidBox.Text = NScrSettings.COMIDstr;
            EndCOMIDBox.Text = NScrSettings.EndCOMIDstr;
            spanBox.Text = NScrSettings.UpSpanStr;
        }



        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void ReadNetwork_Click(object sender, EventArgs e) // initializes the AQT2D object, reads the stream network from web services, saves the stream network object
        {
                        
            if (!Int32.TryParse(comidBox.Text, out int COMID))
            {
                MessageBox.Show("Please either enter a COMID in the COMID box or click on a stream segment to select a pour point.");
                return;
            }

            bool hasendcomid = Int32.TryParse(EndCOMIDBox.Text, out int endCOMID);
            bool hasspan = Double.TryParse(spanBox.Text, out double Span);

            if ((!hasendcomid) && (!hasspan))
            {
                MessageBox.Show("Please either specify an up-river span, enter an up-river COMID in the endCOMID box, or right-click on a stream segment to select an upstream end point.");
                return;
            }

            if ((hasendcomid) && (hasspan)) spanBox.Text = ""; //end comid takes precedence so make this clear on the interface

            SegLoadLabel.Text = "Please Wait, Reading Stream Network...";
            SegLoadLabel.Visible = true;

            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            if (AQT2D == null) AQT2D = new();
            string SNJSON = AQT2D.ReadStreamNetwork(comidBox.Text, EndCOMIDBox.Text, spanBox.Text);
            SegLoadLabel.Visible = false;
            Cursor.Current = Cursors.Default;
            if (SNJSON == "")
            {
                MessageBox.Show("ERROR: web service returned empty JSON."); return;
            }
            if (SNJSON.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MessageBox.Show("Web service returned an error: " + SNJSON); return;
            }
            try
            { AQT2D.CreateStreamNetwork(SNJSON); }
            catch
            {
                Cursor.Current = Cursors.Default;
                SegLoadLabel.Visible = false;
                MessageBox.Show("ERROR converting JSON: " + SNJSON); 
                return;
            }

            ExportSNJSON = SNJSON;
            webView.CoreWebView2.PostWebMessageAsString("RESETCOLORS");
            comidLabel.ForeColor = System.Drawing.Color.Black;
            endCOMIDLabel.ForeColor = System.Drawing.Color.Black;

            SNPopulated = true;
            LakeSelected = false;
            Summary1Label.Text = "Pour Point: " + COMID;
            Summary2Label.Text = AQT2D.SNStats();
            Summary2Label.Visible = true;

            //if (AQT2D.SN.waterbodies != null)
            //    for (int i = 1; i < AQT2D.SN.waterbodies.wb_table.Length; i++)

            for (int i = 0; i < AQT2D.SN.order.Length; i++)
                for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                {
                    bool lastshape = ((i == AQT2D.SN.order.Length - 1) && (j == AQT2D.SN.order[i].Length - 1));
                    webView.CoreWebView2.PostWebMessageAsString("FLCOLOR|" + AQT2D.SN.order[i][j] + "|"+lastshape.ToString()); // display all layers

                    //bool in_waterbody = false;
                    //if (AQT2D.SN.waterbodies != null) in_waterbody = AQT2D.SN.waterbodies.comid_wb.ContainsKey(CID);
                    //if (!in_waterbody) webView.CoreWebView2.PostWebMessageAsString("FLCOLOR|" + AQT2D.SN.order[i][j]);    // don't color segments that are superceded by their lake/reservoir waterbody.
                };

            webView.CoreWebView2.PostWebMessageAsString("ZOOM");
            comidBox_Leave(sender, e); //update NScrSettings
            UpdateScreen();
        }


        private static bool ReadLakeSurrogates()
        {
            if (LS != null) return true;  // don't need to re-read already in memory

            double LSVersionNum = 0;
            string fileN = Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates.json");

            if (File.Exists(fileN))
            {
                string json = File.ReadAllText(fileN);
                LS = JsonConvert.DeserializeObject<Lake_Surrogates>(json, LSJSONSettings());
                LSVersionNum = LS.VersionNum;
            }

            // Add code to update file from EPA server here

            if (LSVersionNum == 0)
            {
                MessageBox.Show("Cannot find required file " + fileN, "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                return false;
            }
            else return true;

        }


        public static string ChooseLakeTemplate(out string lakename, out AQTSim BaseSim)
        {
            BaseSim = null;
            lakename = "";
            if (!ReadLakeSurrogates()) return "";

            GridForm gf = new GridForm();

            if (gf.SelectRow(LS.table, "SurrogateLakes"))
            {
                lakename = gf.chosenlake;
                if (LS.Sims.TryGetValue(gf.chosenfileN, out BaseSim)) return gf.chosenfileN;
                   else return "";
            }
            else return "";
        }

        private void Choose_from_Template_Click(object sender, EventArgs e)
        {
            string lakename;
            string lakefilen = ChooseLakeTemplate(out lakename, out BSim);    
            if (lakefilen == "") return;

            SimBaseLabel.Text = "Simulation Base: "+lakename;
            BaseJSON_FileN = lakefilen;
            SimJSONLabel.Text = "\"" + BaseJSON_FileN + "\"";
        }

        private void NewSimForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = false;  //12/11/2021
            StartDT = StartDate.Value;
            EndDT = EndDate.Value;

            if (DialogResult == DialogResult.OK)  {
                if (!LakeSelected && !SNPopulated)
                {
                    string ErrStr = "To create a new simulation, populate a stream network (with \"Read Network\")";
                    if (LakeButton.Checked) ErrStr = "To create a 0-D Lake/Reservoir simulation, you must first click on a waterbody on the map";
                    MessageBox.Show(ErrStr, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    e.Cancel = true;
                }

                if (StartDate.Value >= EndDate.Value)
                {
                    MessageBox.Show("End date must be after start date.", "Information",
                       MessageBoxButtons.OK, MessageBoxIcon.Warning,
                       MessageBoxDefaultButton.Button1);
                    e.Cancel = true;
                }
            }

        }

        private void NewSimForm_Shown(object sender, EventArgs e)
        {
            //basedirBox.Text = Properties.Settings.Default.MS_Directory;
            UpdateScreen();
        }


        private void HelpButton2_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.PostWebMessageAsString("ZOOM");

            //string target = "Multi_Segment_Runs";  //fixme new help topic
            //AQTTestForm.OpenUrl(target);
        }




        private void webView_MouseDown(bool lake, string COMIDstr)
            
        {
            string[] msg = COMIDstr.Split('|');
            COMID = msg[1];
            SArea = 0;
            Summary2Label.Visible = false;

            if (lake)
            {
                if (msg.Length > 2) SimName = msg[2];
                else SimName = "WBCOMID: " + COMID;
                if (SimName == " ") SimName = "WBCOMID: " + COMID;
                if (msg.Length > 3) if (!double.TryParse(msg[3], out SArea)) SArea = -9999;

                if (SArea > 0)
                {
                    Summary2Label.Visible = true;
                    Summary2Label.Text = "Surface Area (sq. km): " + (SArea / 1e6).ToString("N3");    //m3 to sq km.
                }
                else Summary2Label.Visible = false;
                Summary1Label.Text = "WBCOMID: " + COMID;
                SimNameEdit.Text = SimName;
                SNPopulated = false;
                LakeSelected = true;
            }
            else  //flow lines
            {
                if (msg[0] == "FL")
                {
                    if (msg.Length > 2) SimName = msg[2];
                    else SimName = "COMID: " + COMID;
                    if (SimName == " ") SimName = "COMID: " + COMID;
                    comidBox.Text = COMID;

                    comidLabel.ForeColor =  System.Drawing.Color.DarkOrange;
                    NScrSettings.COMIDstr = COMID;
                    SimNameEdit.Text = SimName;
                }
                else
                {
                    EndCOMIDBox.Text = COMID;
                    endCOMIDLabel.ForeColor = System.Drawing.Color.Green;
                    NScrSettings.EndCOMIDstr = COMID;
                }
            }
        }


        private void webView_MouseHover(object sender, EventArgs e)
        {
            webView.Focus();
        }



        private void comidBox_Leave(object sender, EventArgs e)
        {
            NScrSettings.COMIDstr = comidBox.Text;
            NScrSettings.EndCOMIDstr = EndCOMIDBox.Text;
            NScrSettings.UpSpanStr = spanBox.Text;
        }

        private void MapType_CheckChanged(object sender, EventArgs e)
        {
            UpdateScreen();
            if (StreamButton.Checked)
            {
                webView.CoreWebView2.PostWebMessageAsString("STREAMMAP");
                SegLoadLabel.Text = "Zoom in to see stream segments.";
                SegLoadLabel.Visible = true;
                infolabel1.Text = "Click on a pour-point stream segment then right-click on an upstream";
                infolabel2.Text = "segment or input an up-river span in km and click \"Read Network\"";
                if (!LakeSelected && !SNPopulated) Summary1Label.Text = "Stream network has not been read";
            }
            else
            {
                webView.CoreWebView2.PostWebMessageAsString("LAKEMAP");
                infolabel1.Text = "Click on a Lake/Reservoir to Select";
                infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom";
                if (!LakeSelected && !SNPopulated) Summary1Label.Text = "WB COMID:  (unselected)";
            }

        }

        private void LSButton_Click(object sender, EventArgs e)
        {
            // private funcitonality to create and write Lake_Surrogates object
            Lake_Surrogates LS = new Lake_Surrogates(Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates_Table.json"), "..\\..\\..\\Studies\\");
            string json = JsonConvert.SerializeObject(LS, LSJSONSettings());
            File.WriteAllText("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates.json", json);
            return;

        }

    }
}

