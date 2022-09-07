using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Utilities;
//using Web.Services.Controllers;

namespace GUI.AQUATOX

{
    public partial class NewSimForm : Form
    {
        // private BackgroundWorker Worker = new BackgroundWorker();  potentially to be added, to report progress
        private AQSim_2D AQT2D = null;

        public int SimType;
        public string COMID = "";
        public string GeoJSON = "";
        public string SimName = "";
        public double SArea = -9999;  // surface area in square meters as taken from waterbodies object
        public string BaseJSON_FileN = "Default Lake.JSON";
        public AQTSim BSim = null;
        public DateTime StartDT;  // start and end simulation time 
        public DateTime EndDT;

        static Lake_Surrogates LS = null;

        private ScreenSettings ScrSettings = new();
        private StringCollection ShortDirNames = new();

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

        public class ScreenSettings
        {
            public string BaseJSONstr = "";
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
            String content = args.WebMessageAsJson; //  TryGetWebMessageAsString();

            if (content == "\"DOMContentLoaded\"")
            {
                tcs.SetResult();  //set mapreadyforrender as complete
            }
            else if ((content.StartsWith("\"LAKE|"))||
                (content.StartsWith("\"FL|")) ||
                (content.StartsWith("\"FL2|")))
            {
                System.Threading.SynchronizationContext.Current.Post((_) => {
                    webView_MouseDown((content.StartsWith("\"LAKE|")), content.Trim('"')); }, null);
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
            ReadNetworkPanel.Visible = StreamButton.Checked;
            NetworkLabel.Visible = StreamButton.Checked;

            comidBox.Text = ScrSettings.COMIDstr;
            EndCOMIDBox.Text = ScrSettings.EndCOMIDstr;
            spanBox.Text = ScrSettings.UpSpanStr;
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

            UpdateScreen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // code to create and write Lake_Surrogates object
            Lake_Surrogates LS = new Lake_Surrogates(Path.GetFullPath("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates_Table.json"), "..\\..\\..\\Studies\\");
            string json = JsonConvert.SerializeObject(LS, LSJSONSettings());
            File.WriteAllText("..\\..\\..\\2D_Inputs\\" + "Lake_Surrogates.json", json);
            return;

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

            if (StartDate.Value>=EndDate.Value)
            {
                MessageBox.Show("End date must be after start date.", "Information",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning,
                   MessageBoxDefaultButton.Button1);
                e.Cancel = true;
            }

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




        private void webView_MouseDown(bool lake, string COMIDstr)
            
        {
            string[] msg = COMIDstr.Split('|');
            COMID = msg[1];
            SArea = 0;
            SAreaLabel.Visible = false;

            if (lake)
            {
                if (msg.Length > 2) SimName = msg[2];
                else SimName = "WBCOMID: " + COMID;
                if (SimName == " ") SimName = "WBCOMID: " + COMID;
                if (msg.Length > 3) if (!double.TryParse(msg[3], out SArea)) SArea = -9999;

                if (SArea > 0)
                {
                    SAreaLabel.Visible = true;
                    SAreaLabel.Text = "Surface Area (sq. km): " + (SArea / 1e6).ToString("N3");    //m3 to sq km.
                }
                else SAreaLabel.Visible = false;
                WBCLabel.Text = "WBCOMID: " + COMID;
                SimNameEdit.Text = SimName;
            }
            else  //flow lines
            {
                if (msg[0] == "FL")
                {
                    if (msg.Length > 2) SimName = msg[2];
                    else SimName = "COMID: " + COMID;
                    if (SimName == " ") SimName = "COMID: " + COMID;
                    comidBox.Text = COMID;
                    WBCLabel.Text = "Pour Point: " + COMID;
                    SimNameEdit.Text = SimName;
                }
                else
                {
                    EndCOMIDBox.Text = COMID;
                }
            }
        }



        private void webView_MouseHover(object sender, EventArgs e)
        {
            webView.Focus();
        }



        private void comidBox_Leave(object sender, EventArgs e)
        {
            ScrSettings.COMIDstr = comidBox.Text;
            ScrSettings.EndCOMIDstr = EndCOMIDBox.Text;
            ScrSettings.UpSpanStr = spanBox.Text;

            //SaveScreenSettings();
        }

        private void MapType_CheckChanged(object sender, EventArgs e)
        {
            UpdateScreen();
            if (StreamButton.Checked)
            {
                webView.CoreWebView2.PostWebMessageAsString("STREAMMAP");
                SegLoadLabel.Visible = true;
            }
            else webView.CoreWebView2.PostWebMessageAsString("LAKEMAP");

        }

    }
}

