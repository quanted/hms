using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI.AQUATOX

{
    public partial class NewSimForm : Form
    {
        private AQSim_2D AQT2D = null;
        public string COMID = ""; // user selected COMID Pour Point for StreamNetwork
        public string WBCOMID = ""; // user selected WBCOMID for 0-D lake/res model
        public string HUCChosen = ""; // the user selected HUC number for the model (8-14 characters)
        public Dictionary<string, string> GeoJSON = new();
        public string ExportSNJSON = "";
        public string SimName = "";
        public double SArea = -9999;  // surface area in square meters as taken from waterbodies object
        public string BaseJSON_FileN = "Default Lake.JSON";
        public AQTSim BSim = null;
        public DateTime StartDT;  // start and end simulation time 
        public DateTime EndDT;
        public bool SNPopulated = false;
        public bool fromtemplate = true;
        public string HUCStr = "14";

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
            //MessageBox.Show("WebView Process Failed");
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
                    webView_MouseDown((content.StartsWith("LAKE|")), false, content);
                }, null);
            }
            else if (content.StartsWith("H14"))
            {
                string filestr = @"N:\AQUATOX\CSRA\GIS\HUC_14\GeoJSON_split\H14_" + content.Substring(3, 2) + ".geojson";
                string HUC14 = File.ReadAllText(filestr);
                webView.CoreWebView2.PostWebMessageAsString("ADD|" + HUC14); // display layer
            }
            else if (content.StartsWith("DoneDraw"))
            {
                Application.DoEvents();
            }
            else if (content.StartsWith("HUC"))
            {
                System.Threading.SynchronizationContext.Current.Post((_) =>
                {
                    webView_MouseDown(false,true, content);
                }, null);
            }
            else
            {
                string[] split = content.Split('|');
                split[1] = split[1].Replace("\\\"", "\"");   // lake geojsons included \ character in strings
                if (!GeoJSON.ContainsKey(split[0])) GeoJSON.Add(split[0], split[1]);
            }

        }

        protected override void WndProc(ref Message m)
        {
            // Suppress the WM_UPDATEUISTATE message
            if (m.Msg == 0x128) return;
            base.WndProc(ref m);
            // https://stackoverflow.com/questions/8848203/alt-key-causes-form-to-redraw
        }

        private void UpdateLeftPanels()
        {
            ReadNetworkPanel.Visible = StreamButton.Checked;
            NetworkLabel.Visible = StreamButton.Checked;

            comidBox.Text = NScrSettings.COMIDstr;
            EndCOMIDBox.Text = NScrSettings.EndCOMIDstr;
            spanBox.Text = NScrSettings.UpSpanStr;

            if (LakeButton.Checked)
            {

                if (WBCOMID =="")
                {
                    Summary1Label.Text = "WB COMID:  (unselected)";
                    Summary2Label.Visible = false;
                }
                else
                {
                    Summary1Label.Text = "WBCOMID: " + WBCOMID;
                    SimNameEdit.Text = SimName;
                    if (SArea > 0)
                    {
                        Summary2Label.Visible = true;
                        Summary2Label.Text = "Surface Area (sq. km): " + (SArea / 1e6).ToString("N3");    //m3 to sq km.
                    }
                    else Summary2Label.Visible = false;
                }


            }
            else if (HUCButton.Checked)
            {
                SimNameEdit.Text = SimName;
                if (HUCChosen == "") Summary1Label.Text = "HUC" + HUCStr + ":  (unselected)";
                else Summary1Label.Text = "HUC"+": " + HUCChosen;
                Summary2Label.Visible = false;

            }
            else  // otherwise-- stream network selected
            {
                if (SNPopulated)
                {
                    Summary1Label.Text = "Pour Point: " + COMID;
                    Summary2Label.Text = AQT2D.SNStats();
                    Summary2Label.Visible = true;
                }
                else
                {
                    if (!SNPopulated) Summary1Label.Text = "Stream network has not been read";
                    Summary2Label.Visible = false;
                }
            };
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
            
            HighlightStreamNetwork();

            comidBox_Leave(sender, e); //update NScrSettings
            UpdateLeftPanels();
        }


        private void HighlightStreamNetwork()
        {
            for (int i = 0; i < AQT2D.SN.order.Length; i++)
                for (int j = 0; j < AQT2D.SN.order[i].Length; j++)
                {
                    bool lastshape = ((i == AQT2D.SN.order.Length - 1) && (j == AQT2D.SN.order[i].Length - 1));
                    webView.CoreWebView2.PostWebMessageAsString("FLCOLOR|" + AQT2D.SN.order[i][j] + "|" + lastshape.ToString()); // display all layers
                };
            webView.CoreWebView2.PostWebMessageAsString("ZOOM");
        }

        private static bool ReadLakeSurrogates()
        {
            if (LS != null) return true;  // don't need to re-read already in memory

            double LSVersionNum = 0;
            string fileN = Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "Multi_Seg_Surrogates.json");

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


        public static string ChooseJSONTemplate(out string sitename, out AQTSim BaseSim)
        {
            BaseSim = null;
            sitename = "";
            if (!ReadLakeSurrogates()) return "";

            GridForm gf = new GridForm();
            gf.Text = "Select Surrogate Simulation";

            if (gf.SelectRow(LS.table, "Select_Surrogate"))
            {
                sitename = gf.chosenlake;
                if (LS.Sims.TryGetValue(gf.chosenfileN, out BaseSim)) return gf.chosenfileN;
                else return "";
            }
            else return "";
        }

        private void Choose_from_Template_Click(object sender, EventArgs e)
        {
            string lakename;
            string lakefilen = ChooseJSONTemplate(out lakename, out BSim);
            if (lakefilen == "") return;

            fromtemplate = true;
            SimBaseLabel.Text = "Simulation Base: " + lakename;
            BaseJSON_FileN = lakefilen;
            SimJSONLabel.Text = "\"" + BaseJSON_FileN + "\"";
        }

        private void NewSimForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = false;  //12/11/2021
            StartDT = StartDate.Value;
            EndDT = EndDate.Value;

            if (DialogResult == DialogResult.OK)
            {
                if ((StreamButton.Checked && !SNPopulated)||(LakeButton.Checked && (WBCOMID==""))||(HUCButton.Checked && (HUCChosen=="")))
                {
                    string ErrStr = "To create a new simulation, populate a stream network (with \"Read Network\")";
                    if (LakeButton.Checked) ErrStr = "To create a 0-D Lake/Reservoir simulation, you must first click on a waterbody on the map";
                    if (HUCButton.Checked) ErrStr = "To create a HUC simulation, you must first click on one of the HUCs diaplsyed on the map";
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

                if (!e.Cancel)
                {
                    if (!StreamButton.Checked) COMID = "";
                    if (!HUCButton.Checked) HUCChosen = "";
                    if (!LakeButton.Checked) WBCOMID = "";
                }
            }

        }

        private void NewSimForm_Shown(object sender, EventArgs e)
        {
            //basedirBox.Text = Properties.Settings.Default.MS_Directory;
            UpdateLeftPanels();
        }


        private void HelpButton2_Click(object sender, EventArgs e)
        {
            string target = "New_Simulation";
            AQTMainForm.OpenUrl(target);
        }





        private void webView_MouseDown(bool lake, bool HUC, string COMIDstr)

        {
            string[] msg = COMIDstr.Split('|');
            SArea = 0;

            if (lake)
            {
                WBCOMID = msg[1];
                if (msg.Length > 2) SimName = msg[2];
                else SimName = "WBCOMID: " + WBCOMID;
                if (SimName == " ") SimName = "WBCOMID: " + WBCOMID;
                if (msg.Length > 3) if (!double.TryParse(msg[3], out SArea)) SArea = -9999;

                UpdateLeftPanels();

            }
            else if (HUC)
            {
                HUCChosen = msg[1];
                if (msg.Length > 2) SimName = "HUC" + HUCStr + ": "+msg[2];
                else SimName = "HUC" + HUCStr + ": " + HUCChosen;

                UpdateLeftPanels();
            }
            else  //flow lines
            {
                COMID = msg[1];
                if (msg[0] == "FL")
                {
                    if (msg.Length > 2) SimName = msg[2];
                    else SimName = "COMID: " + COMID;
                    if (SimName == " ") SimName = "COMID: " + COMID;
                    comidBox.Text = COMID;

                    comidLabel.ForeColor = System.Drawing.Color.DarkOrange;
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
            UpdateLeftPanels();
            if (StreamButton.Checked)
            {
                ShowH14Box.Visible = true;
                ShowH14Box.Checked = false;
                HUCSelectionPanel.Visible = false;
                webView.CoreWebView2.PostWebMessageAsString("STREAMMAP");
                SegLoadLabel.Text = "Zoom in to see stream segments.";
                SegLoadLabel.Visible = true;
                infolabel1.Text = "Click on a pour-point stream segment then right-click on an upstream";
                infolabel2.Text = "segment or input an up-river span in km and click \"Read Network\"";
            }
            else if (LakeButton.Checked)
            {
                ShowH14Box.Visible = false;
                HUCSelectionPanel.Visible = false;
                webView.CoreWebView2.PostWebMessageAsString("LAKEMAP");
                SegLoadLabel.Text = "Zoom in to see Lakes/Reservoirs.";
                SegLoadLabel.Visible = true;
                infolabel1.Text = "Click on a Lake/Reservoir to Select";
                infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom";
            }
            else //  HUCButton.checked
            {
                ShowH14Box.Visible = false;
                webView.CoreWebView2.PostWebMessageAsString("HUCMAP|" + HUCStr);
                HUCSelectionPanel.Visible = true;

                SegLoadLabel.Text = "Zoom in to see HUC14 segments.";
                SegLoadLabel.Visible = BHUC14.Checked;

                infolabel1.Text = "Click on one HUC to Select";
                infolabel2.Text = "Drag to pan the map, mouse-wheel to zoom";
            }
        }

        private void MS_Surrogate_Button_Click(object sender, EventArgs e)
        {
            // private functionality to create and write Multi-segment Surrogates object
            Lake_Surrogates LS = new Lake_Surrogates(Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "MS_Surrogates_Table.json"), "..\\..\\..\\Studies\\");
            string json = JsonConvert.SerializeObject(LS, LSJSONSettings());
            File.WriteAllText("..\\..\\..\\2D_Inputs\\" + "Multi_Seg_Surrogates.json", json);
            return;
        }

        private void BrowseJSONButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text File|*.txt;*.json";
            openFileDialog1.Title = "Open a JSON File";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;

            if (openFileDialog1.FileName != "")
            {
                try
                {
                    BSim = new AQTSim();
                    string err = BSim.Instantiate(File.ReadAllText(openFileDialog1.FileName));
                    if (err != "")
                    {
                        MessageBox.Show("Could not read JSON: " + err);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Could not read JSON " + openFileDialog1.FileName);
                    BSim = null;
                    return;
                }

                fromtemplate = false;
                string filen = Path.GetFileName(openFileDialog1.FileName);
                SimBaseLabel.Text = "Simulation Base: " + filen;
                BaseJSON_FileN = openFileDialog1.FileName;
                SimJSONLabel.Text = "\"" + filen + "\"";
            }
        }

        private void BHUC14_CheckedChanged(object sender, EventArgs e)
        {
            if (BHUC8.Checked) HUCStr = "8";
            else if (BHUC10.Checked) HUCStr = "10";
            else if (BHUC12.Checked) HUCStr = "12";
            else HUCStr = "14";
            MapType_CheckChanged(sender, e);
        }

        private void ShowH14Box_CheckedChanged(object sender, EventArgs e)
        {
            string showstr = "false";
            if (ShowH14Box.Checked) showstr = "true";
            webView.CoreWebView2.PostWebMessageAsString("SHOWH14|" + showstr);
        }
    }
}

