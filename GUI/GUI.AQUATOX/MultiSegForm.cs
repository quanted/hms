using AQUATOX.AQTSegment;
using AQUATOX.Loadings;
using AQUATOX.Volume;
using AQUATOX.Animals;
using AQUATOX.Plants;
using Data;
using Globals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Web.Services.Controllers;

namespace GUI.AQUATOX
{
    public partial class MultiSegForm : Form
    {

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that 
        /// window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        private const int WM_SETREDRAW = 11;


        public MultiSegForm()
        {
            AutoScroll = true;
            InitializeComponent();
        }


        private void cancel_click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Cancel any changes to inputs?", "Confirm",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                // UserCanceled = true;
                this.Close();
            }
        }

        private void OK_click(object sender, EventArgs e)
        {
            this.Close();
        }

        class streamNetwork
        {
            public string[][] network;
            public int[][] order;
            public Dictionary<string, int[]> sources;
        }

        private StreamflowInput sfi;
        private TimeSeriesOutput ATSO;

        /// <summary>
        /// Submit POST request to HMS web API
        /// </summary>
        public void submitRequest(string comid)
        {

            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            // string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "hydrology";
            string dataset = "streamflow";

            var request = (HttpWebRequest)WebRequest.Create(requestURL + "" + component + "/" + dataset + "/");
            var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(sfi));  //StreamFlowInput previously initialized
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            string rstring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            ATSO = JsonConvert.DeserializeObject<TimeSeriesOutput>(rstring);

        }

        private void AddToTextBox(string msg)
        {
            textBox3.AppendText(msg + Environment.NewLine);
            Application.DoEvents();
        }

        private void TSafeAddToTextBox(string msg)
        {

            if (InvokeRequired)
                textBox3.BeginInvoke((MethodInvoker)delegate ()
                {
                    textBox3.AppendText(msg + Environment.NewLine);
                });
            else AddToTextBox(msg);


        }


        private void UpdateDischarge(TVolume TVol)
        {
            TVol.Calc_Method = VolumeMethType.Manning;

            // 1 is discharge in this case
            TLoadings DischargeLoad = TVol.LoadsRec.Alt_Loadings[1];

            if (DischargeLoad.ITSI == null)
            {
                TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                DischargeLoad.ITSI = input;
            }

            DischargeLoad.ITSI.InputTimeSeries.Add("input", ATSO);
            DischargeLoad.Translate_ITimeSeriesInput();
            DischargeLoad.MultLdg = 1e6;
            DischargeLoad.UseConstant = false;
            TVol.LoadNotes1 = "From 2-D Linkage";
            TVol.LoadNotes2 = "";
            DischargeLoad.ITSI = null;

        }

        streamNetwork SN = null;
        private void createButton_Click(object sender, EventArgs e)
        {
            StreamflowInputExample sfie = new StreamflowInputExample();

            string SNJSON = System.IO.File.ReadAllText(@"C:\Users\cloug\Downloads\response_1618863788756.json", Encoding.Default);  //C:\Users\cloug\Downloads\response_1617838994520.json
            SN = Newtonsoft.Json.JsonConvert.DeserializeObject<streamNetwork>(SNJSON);

            int nSegs = SN.network.Count() - 1;
            AddToTextBox("System has " + nSegs.ToString() + " segments");
            string BaseFileN = @"C:\work\AQUATOX\CSRA\LBR Glenwood 4.JSON";
            AddToTextBox(" Basefile = " + BaseFileN);

            string BaseDir = basedirBox.Text;
            AddToTextBox(" BaseDir = " + BaseDir);

            File.WriteAllText(BaseDir + "StreamNetwork.JSON", SNJSON);

            for (int iSeg = 1; iSeg <= nSegs; iSeg++)
            {
                string comid = SN.network[iSeg][0];
                double lenkm = double.Parse(SN.network[iSeg][4]);

                string json = File.ReadAllText(BaseFileN);
                AQTSim Sim = new AQTSim();
                string err = Sim.Instantiate(json);
                if (err != "") { MessageBox.Show(err); return; }
                Sim.AQTSeg.SetMemLocRec();

                Sim.AQTSeg.Location.Locale.SiteLength.Val = lenkm;
                Sim.AQTSeg.Location.Locale.SiteLength.Comment = "From 2-D Linkage";

                sfi = sfie.GetExamples();
                sfi.DateTimeSpan.StartDate = Sim.AQTSeg.PSetup.FirstDay.Val;
                sfi.DateTimeSpan.EndDate = Sim.AQTSeg.PSetup.LastDay.Val;
                sfi.TemporalResolution = "daily";
                sfi.Geometry.ComID = int.Parse(comid);
                sfi.Source = "test";
                submitRequest(comid);
                UpdateDischarge(Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume);

                AddToTextBox("Imported Flow Data for " + comid);

                string jsondata = "";
                string errmessage = Sim.SaveJSON(ref jsondata);
                if (errmessage == "")
                {
                    Sim.AQTSeg.FileName = BaseDir + "AQT_Nutrient_" + comid + ".JSON";
                    File.WriteAllText(Sim.AQTSeg.FileName, jsondata);
                }
                else MessageBox.Show(errmessage);

                AddToTextBox("Saved JSON for " + comid);

                AddToTextBox("");
            }

            if (SN.sources.TryGetValue("boundaries", out int[] boundaries))
            {
                string bnote = "Note: Boundary Condition Flows and State Variables should be added to COMIDs: ";
                foreach (long bid in boundaries) bnote = bnote + bid.ToString() + ", ";
            }
        }

        Dictionary<int, archived_results> archive = new Dictionary<int, archived_results>();

        class archived_results
        {
            public DateTime[] dates;
            public double[] washout;  // m3
            public double[][] concs; // g/m3 or mg/m3 depending on state var
        }

        private void archiveOutput(int comid, AQTSim Sim)
        {
            archived_results AR = new archived_results();
            TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
            int ndates = tvol.SVoutput.Data.Keys.Count;
            AR.dates = new DateTime[ndates];
            AR.washout = new double[ndates];
            for (int i = 0; i < ndates; i++)
            {
                ITimeSeriesOutput ito = tvol.SVoutput;
                string datestr = ito.Data.Keys.ElementAt(i).ToString();
                Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                AR.dates[i] = Convert.ToDateTime(datestr);
                AR.washout[i] = Val;  // fixme, convert to washout
            }

            AR.concs = new double[Sim.AQTSeg.SV.Count()][];
            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                AR.concs[iTSV] = new double[ndates];
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];
                for (int i = 0; i < ndates; i++)
                {
                    ITimeSeriesOutput ito = TSV.SVoutput;
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                    AR.concs[iTSV][i] = Val;
                }
            }

            archive.Add(comid, AR);
        }

        private void Pass_Data(AQTSim Sim, int SrcID, int ninputs)
        {
            archived_results AR;
            archive.TryGetValue(SrcID, out AR);

            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];

                if (((TSV.NState >= AllVariables.H2OTox) && (TSV.NState <= AllVariables.TSS)) ||   //fixme improve conditions for drift
                    ((TSV.NState >= AllVariables.DissRefrDetr) && (TSV.NState <= AllVariables.SuspLabDetr)) || // fixme macrophytes 
                    ((TSV.IsPlant()) && ((TPlant)TSV).IsPhytoplankton()) ||
                    ((TSV.IsAnimal()) && ((TAnimal)TSV).IsPlanktonInvert()))
                {
                    TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                    int ndates = AR.dates.Count();
                    TLoadings DischargeLoad = tvol.LoadsRec.Alt_Loadings[1];  //fixme refine with inflow load?
                    SortedList<DateTime, double> newlist = new SortedList<DateTime, double>();

                    for (int i = 0; i < ndates; i++)
                    {
                        double InVol = AR.washout[i];  // fixme replace with calculated inflow volume using Mannings N;
                        double OutVol = AR.washout[i];   
                                                
                        if (ninputs == 1) newlist.Add(AR.dates[i] , AR.concs[iTSV][i] * (OutVol / InVol));  // first or only input
                        else newlist.Add(AR.dates[i], TSV.LoadsRec.Loadings.list.Values[i] + AR.concs[iTSV][i] * (OutVol / InVol));  //adding second or third inputs

                    }

                    TSV.LoadsRec.Loadings.list = newlist;
                }

            }
        }


            private string executeModel(int comid)
        {
            string strout = "";
            string BaseDir = basedirBox.Text;
            string json = File.ReadAllText(BaseDir+ "AQT_Nutrient_" + comid.ToString() + ".JSON");
            AQTSim Sim = new AQTSim();
            string err = Sim.Instantiate(json);
            if (err != "") { MessageBox.Show(err); return(err); }
            Sim.AQTSeg.SetMemLocRec();

            int nSources = 0;
            if (SN.sources.TryGetValue(comid.ToString(), out int[] Sources))
                foreach (int SrcID in Sources)
                { 
                  if (SrcID != comid)  // set to itself in boundaries 
                    {
                        nSources++;
                        Pass_Data(Sim, SrcID, nSources);
                        strout = strout + "Passed data from Source "+ SrcID.ToString() + " into COMID " + comid.ToString() + Environment.NewLine;
                    }
                };

            Sim.AQTSeg.RunID = "2D Run: "+ DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string errmessage = Sim.Integrate();
            if (errmessage == "")
            {
                archiveOutput(comid, Sim);

                string jsondata = "";
                errmessage = Sim.SaveJSON(ref jsondata);
                if (errmessage == "")
                {
                    Sim.AQTSeg.FileName = BaseDir + "AQT_Run_" + comid + ".JSON";
                    File.WriteAllText(Sim.AQTSeg.FileName, jsondata);
                }
                else strout = strout + errmessage + Environment.NewLine;
            }
            else strout = strout + errmessage +Environment.NewLine;


            return (strout + "--> Executed COMID " + comid.ToString());
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (SN == null)
            {
                string BaseDir = basedirBox.Text;
                string SNJSON = System.IO.File.ReadAllText(BaseDir + "StreamNetwork.JSON", Encoding.Default);  //C:\Users\cloug\Downloads\response_1617838994520.json
                SN = Newtonsoft.Json.JsonConvert.DeserializeObject<streamNetwork>(SNJSON);
                AddToTextBox("Read stream network from "+ BaseDir + "StreamNetwork.JSON");
            }

            archive.Clear();
            for (int ordr = 0; ordr < SN.order.Length; ordr++)
            {
                 Parallel.ForEach(SN.order[ordr], runID =>
                 {
                    TSafeAddToTextBox(executeModel(runID) );
                 });
                Application.DoEvents();
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            int SVIndex = int.Parse(SVBox.Text);
            //StreamWriter file = new StreamWriter("N:\\AQUATOX\\CSRA\\" + "Test2" + ".csv");
            string csv = "";

            int NDates = 0;

            foreach (KeyValuePair<int, archived_results> entry in archive)
            {
                csv += (entry.Key)+",";
                NDates = entry.Value.dates.Count();
            }
            csv += Environment.NewLine;

            for (int i = 0; i < NDates; i++)
            {
                foreach (KeyValuePair<int, archived_results> entry in archive)
                {
                    csv += (entry.Value.concs[SVIndex][i])+ ",";
                }
                csv += Environment.NewLine;
            }

            textBox3.Text = csv;
            //file.Close();
            //MessageBox.Show("Exported N:\\AQUATOX\\CSRA\\" + "Test2" + ".csv");

        }
    }
}
