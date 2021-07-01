using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQSite;
using AQUATOX.AQTSegment;
using AQUATOX.Volume;
using AQUATOX.Loadings;

using System.Threading;
using System.Threading.Tasks;

using System.Linq;
using Newtonsoft.Json;
using Data;
using System.ComponentModel;
using System.IO;
using System.Data;
using AQUATOX.Plants;
using AQUATOX.Animals;
using System.Net;
using System.Text;

namespace AQUATOX.AQSim_2D

{
    public class AQSim_2D

    {
        /// <summary>
        /// holds results from streamNetwork web service
        /// </summary>
        public class streamNetwork
        {
            public string[][] network;
            public int[][] order;
            public Dictionary<string, int[]> sources;
        }

        /// <summary>
        /// current stream network object
        /// </summary>
        public streamNetwork SN = null;

        /// <summary>
        /// number of stream segments in stream network
        /// </summary>
        public int nSegs;

        /// <summary>
        /// JSON string holds the base AQUATOX simulation that is propagated to create a 2D network
        /// </summary>
        public string baseSimJSON = "";


        /// <summary>
        /// list of state variables in simulation for summarizing output and verifying the list has not changed
        /// </summary>
        public List<string> SVList = null;

        /// <summary>
        /// Dictionary of archived_results organized by COMID.  Used for routing state variables and summarizing 2-D results.
        /// </summary>
        public Dictionary<int, archived_results> archive = new Dictionary<int, archived_results>();

        /// <summary>
        /// Archived results for a single COMID;  
        /// array of dates, water-volume discharge on the main stem, and 2-D array of state variable concentrations
        /// </summary>
        public class archived_results
        {
            public DateTime[] dates;
            public double[] washout;  // m3
            public double[][] concs; // g/m3 or mg/m3 depending on state var
        }


        /// <summary>
        /// converts stream network json string into SN variable and saves number of segments
        /// </summary>
        /// <param name="SNJSON">input json</param>
        public void CreateStreamNetwork(string SNJSON)
        {
            SN = Newtonsoft.Json.JsonConvert.DeserializeObject<streamNetwork>(SNJSON);
            nSegs = SN.network.Count() - 1;
        }

        public List<string> MultiSegSimFlags()
        {
            return new List<string>(new string[] { "Nitrogen", "Phosphorus", "Organic Matter" });
        }

        public string MultiSegSimName(List<bool> flags)
        {
            if ((flags[2]) && (flags[1])) return "MS_OM.json";  // [2] is organic matter, organic matter simulations require nitrogen
            else if (flags[2]) return "MS_OM_NoP.json";  // [1] is phosphorus, and this is not selected 
            else if ((flags[0]) && (flags[1])) return "MS_Nutrients.json";  // [0] is nitrogen; [1] is phosphorus 
            else if (flags[0]) return "MS_Nitrogen.json"; //flag [0] for nitrogen is the only one selected
            else return "MS_Phosphorus.json"; //flag [1] for phosphorus is the only one selected
        }

        private void FlowsFromNWM(TVolume TVol, Data.TimeSeriesOutput ATSO, bool useVelocity)  // time series output must currently be in m3/s
        {
            if (useVelocity)
            {
                TVol.Calc_Method = VolumeMethType.KnownVal;

                TLoadings KnownValLoad = TVol.LoadsRec.Loadings;
                TLoadings InflowLoad = TVol.LoadsRec.Alt_Loadings[0];  // array slot [0] is "inflow" in this case (TVolume)

                if (InflowLoad.ITSI == null)
                {
                    TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                    TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                    input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    InflowLoad.ITSI = input;
                }

                InflowLoad.ITSI.InputTimeSeries.Add("input", ATSO);

                KnownValLoad.UseConstant = false;
                KnownValLoad.MultLdg = 1;
                KnownValLoad.NoUserLoad = false;
                KnownValLoad.Hourly = true;  

                ITimeSeriesOutput TSO = InflowLoad.ITSI.InputTimeSeries.FirstOrDefault().Value;

                KnownValLoad.list.Clear();
                KnownValLoad.list.Capacity = TSO.Data.Count;

                bool firstvol = true;
                foreach (KeyValuePair<string, List<string>> entry in TSO.Data)
                {
                    string dateString = entry.Key + ":00"; //  (entry.Key.Count() == 13) ? entry.Key.Split(" ")[0] : entry.Key; make flexible?
                    if (!(DateTime.TryParse(dateString, out DateTime date)))
                        throw new ArgumentException("Cannot convert '" + entry.Key + "' to TDateTime");
                    if (!(Double.TryParse(entry.Value[0], out double flow)))
                        throw new ArgumentException("Cannot convert '" + entry.Value + "' to Double");
                    if (!(Double.TryParse(entry.Value[1], out double velocity)))
                        throw new ArgumentException("Cannot convert '" + entry.Value + "' to Double");

                    if (velocity < Consts.Tiny) velocity = flow; // default to 1 m2 for now
                    double VolCalc = (flow / velocity) * (TVol.AQTSeg.Location.Locale.SiteLength.Val) * 1000;
                    // known value(m3) = flow(m3/s) / velocity(m/s) * sitelength(km) * 0.001 (m/km)

                    if (flow > Consts.Tiny) 
                        KnownValLoad.list.Add(date,VolCalc);
                    
                    if ((firstvol) && (VolCalc > Consts.Tiny)) TVol.InitialCond = VolCalc;
                }

                InflowLoad.Translate_ITimeSeriesInput(0);
                InflowLoad.MultLdg = 86400;  // seconds per day
                InflowLoad.Hourly = true;
                InflowLoad.UseConstant = false;
                TVol.LoadNotes1 = "Volumes from NWM using discharge in m3/s";       
                TVol.LoadNotes2 = "NWM inflow converted from m3/d using multiplier";
                InflowLoad.ITSI = null;
            }
            else  // use discharge and manning's equation
            {
                TVol.Calc_Method = VolumeMethType.Manning;

                // array slot [1] is "discharge" in this case (TVolume)
                TLoadings DischargeLoad = TVol.LoadsRec.Alt_Loadings[1];

                if (DischargeLoad.ITSI == null)
                {
                    TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                    TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                    input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    DischargeLoad.ITSI = input;
                }


                DischargeLoad.ITSI.InputTimeSeries.Add("input", ATSO);
                DischargeLoad.Translate_ITimeSeriesInput(0);
                DischargeLoad.MultLdg = 86400;  // seconds per day
                DischargeLoad.Hourly = true;
                DischargeLoad.UseConstant = false;
                TVol.LoadNotes1 = "Discharge from NWM in m3/s";                      // Add flexibility here in case of alternative data source
                TVol.LoadNotes2 = "Converted to m3/d using multiplier";
                DischargeLoad.ITSI = null;
            }
        }

    //  public DataTable GetOverlandTable(string BaseJSON)() {}              WORKHERE


        public void archiveOutput(int comid, AQTSim Sim)
        {
            archived_results AR = new archived_results();
            TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;

            ITimeSeriesOutput ito = tvol.SVoutput;

            int washoutindex = -1;
            int ccount = ito.Data.Values.ElementAt(0).Count();
            for (int col = 2; col <= ccount; col++)
            {
                if ((ito.Metadata["Name_" + col.ToString()] == "Discharge")
                    && (ito.Metadata["Unit_" + col.ToString()] == "m3/d")) washoutindex = col;
            }

            int ndates = tvol.SVoutput.Data.Keys.Count;
            AR.dates = new DateTime[ndates];
            AR.washout = new double[ndates];

            for (int i = 0; i < ndates; i++)
            {
                string datestr = ito.Data.Keys.ElementAt(i).ToString();
                Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[washoutindex - 1]);
                AR.dates[i] = Convert.ToDateTime(datestr);
                AR.washout[i] = Val;  // m3/d
            }

            AR.concs = new double[Sim.AQTSeg.SV.Count()][];
            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                AR.concs[iTSV] = new double[ndates];
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];
                for (int i = 0; i < ndates; i++)
                {
                    ito = TSV.SVoutput;
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                    AR.concs[iTSV][i] = Val;
                }
            }

            archive.Add(comid, AR);
        }


        /// <summary>
        /// Reads the stream network data structure from web services
        /// </summary>
        /// <param name="comid">Primary comid</param>
        /// <param name="pourID">Optional PourID</param>
        /// <param name="span">Optional up-stream distance to search in km</param>
        /// <returns>JSON or error message</returns>
        public string ReadStreamNetwork(string comid, string pourID, string span)
        {
            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            //string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "info";
            string dataset = "streamnetwork";

            try
            {
                string rurl = requestURL + "" + component + "/" + dataset + "?mainstem=false&comid=" + comid;
                if (pourID != "") rurl += "&endComid=" + pourID;
                if (span != "") rurl += "&maxDistance=" + span;
                var request = (HttpWebRequest)WebRequest.Create(rurl);
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private TimeSeriesInput TSI = new TimeSeriesInput()
        {
            Source = "nwis",
            DateTimeSpan = new DateTimeSpan()
            {
                StartDate = new DateTime(2015, 01, 01),  // overwritten below
                EndDate = new DateTime(2015, 12, 31),
                DateTimeFormat = "yyyy-MM-dd HH"
            },
            Geometry = new TimeSeriesGeometry()
            {
                GeometryMetadata = new Dictionary<string, string>()
                {
                }
            },
            DataValueFormat = "E3",
            TemporalResolution = "hourly",
            Units = "metric",
            OutputFormat = "json"
        };


        /// <summary>
        /// Submit POST request to HMS web API for stream flow
        /// </summary>
        private TimeSeriesOutput submitHydrologyRequest(string comid, out string errmsg)  
        {

            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            //string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "hydrology";
            string dataset = "streamflow";
            errmsg = "";

            var request = (HttpWebRequest)WebRequest.Create(requestURL + "" + component + "/" + dataset + "/");
            string json = JsonConvert.SerializeObject(TSI);
            var data = Encoding.ASCII.GetBytes(json);  //StreamFlowInput previously initialized
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            string rstring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (rstring.IndexOf("ERROR") >= 0)
            {
                errmsg = "Error from web service returned: " + rstring; 
                return null;
            }
            return JsonConvert.DeserializeObject<TimeSeriesOutput>(rstring);
        }

        /// <summary>
        /// After the SN streamnetwork object has been initialized, this method is iterated through for each segment 
        /// to read physical parameters from the streamnetwork, read updated flow data from web services, and save the
        /// resulting segment to a unique json string
        /// </summary>
        /// <param name="iSeg">index of segment being set up</param>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="jsondata">a json string holding the modified AQUATOX segment</param>
        /// <returns>a blank string if no error, or error details otherwise</returns>
        public string PopulateStreamNetwork(int iSeg, string setupjson, out string jsondata) 
        {
            jsondata = "";
            string comid = SN.network[iSeg][0];
            double lenkm = double.Parse(SN.network[iSeg][4]);

            AQTSim Sim = new AQTSim();
            string err = Sim.Instantiate(baseSimJSON);

            Sim.AQTSeg.PSetup = Newtonsoft.Json.JsonConvert.DeserializeObject<Setup_Record>(setupjson);

            if (err != "") { return err; }

            Sim.AQTSeg.SetMemLocRec();

            Sim.AQTSeg.Location.Locale.SiteLength.Val = lenkm;
            Sim.AQTSeg.Location.Locale.SiteLength.Comment = "From Multi-Seg Linkage";

            // create a new itimeseries
            TSI.DateTimeSpan.StartDate = Sim.AQTSeg.PSetup.FirstDay.Val;
            TSI.DateTimeSpan.EndDate = Sim.AQTSeg.PSetup.LastDay.Val;
            TSI.Geometry.ComID = int.Parse(comid);
            TSI.Source = "nwm"; //  "test";

            try
            {
                TimeSeriesOutput TSO = submitHydrologyRequest(comid, out string errstr);
                if (errstr != "") return errstr;
                FlowsFromNWM(Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume,TSO,true);
                // Could add to Log -- "Imported Flow Data for " + comid 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            jsondata = "";

            string errmessage = Sim.SaveJSON(ref jsondata);
            return errmessage;
        }

        public void Pass_Data(AQTSim Sim, int SrcID, int ninputs, archived_results AR = null)
        {
            //archived_results AR;
            if (AR == null)    // (AR.Equals(null)) crashed
            {
                archive.TryGetValue(SrcID, out AR);
                if (AR == null) return;
            }

            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];

                if (((TSV.NState >= AllVariables.H2OTox) && (TSV.NState < AllVariables.TSS)) ||   
                    ((TSV.NState >= AllVariables.DissRefrDetr) && (TSV.NState <= AllVariables.SuspLabDetr)) || 
                    ((TSV.IsPlant()) && ( ((TPlant)TSV).IsPhytoplankton() || (((TPlant)TSV).IsMacrophyte() && (((TPlant)TSV).MacroType == TMacroType.Freefloat))) ) ||
                    ((TSV.IsAnimal()) && ((TAnimal)TSV).IsPlanktonInvert()))
                {
                    TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                    int ndates = AR.dates.Count();
                    // TLoadings DischargeLoad = tvol.LoadsRec.Alt_Loadings[1];    
                    TLoadings InflowLoad = tvol.LoadsRec.Alt_Loadings[0];
                    SortedList<DateTime, double> newlist = new SortedList<DateTime, double>();

                    for (int i = 0; i < ndates; i++)
                    {
                        double OutVol = AR.washout[i];  // out volume from upstream segment
                        double InVol = InflowLoad.ReturnLoad(AR.dates[i]);  // inflow volume to current segment,   If velocity is not used, must be estimated as current seg. outflow 

                        if (InVol < Consts.Tiny) newlist.Add(AR.dates[i], 0);
                        else if (ninputs == 1) newlist.Add(AR.dates[i], AR.concs[iTSV][i] * (OutVol / InVol));  // first or only input
                        else newlist.Add(AR.dates[i], TSV.LoadsRec.Loadings.list.Values[i] + AR.concs[iTSV][i] * (OutVol / InVol));  //adding second or third inputs

                    }

                    TSV.LoadsRec.Loadings.list = newlist;
                    TSV.LoadsRec.Loadings.UseConstant = false;
                    TSV.LoadsRec.Loadings.Hourly = true;
                    TSV.LoadNotes1 = "Linkage Data from " + SrcID.ToString();
                }
            }
        }

        /// <summary>
        /// Runs one segment of the 2D simulation given the comid.  Reads loadings from upstream reach prior to execution.
        /// Segments must be executed in the order and with the parallel processing specified within SN streamnetwork object.
        /// Results of the simulation are passed back in a json and stored in the archive Dictionary.
        /// </summary>
        /// <param name="comid">integer comid of segment in network</param>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="outstr">information about the status of the run for the user's log</param>
        /// <param name="jsonstring">the completed simulation with results </param>
        /// <returns>boolean: true if the run was completed successfully</returns>
        public bool executeModel(int comid, string setupjson, ref string outstr, ref string jsonstring)  
        {
            AQTSim Sim = new AQTSim();
            outstr = Sim.Instantiate(jsonstring);

            if (outstr != "")  return false; 
            Sim.AQTSeg.SetMemLocRec();

            Sim.AQTSeg.PSetup = Newtonsoft.Json.JsonConvert.DeserializeObject<Setup_Record>(setupjson);   

            if (SVList == null)
            {
                SVList = new List<string>();
                foreach (TStateVariable SV in Sim.AQTSeg.SV)
                {
                    SVList.Add(SV.PName);   //save list of SVs for output
                }
            }
                

            int nSources = 0;
            if (SN.sources.TryGetValue(comid.ToString(), out int[] Sources))
                foreach (int SrcID in Sources)
                {
                    if (SrcID != comid)  // set to itself in boundaries 
                    {
                        nSources++;
                        Pass_Data(Sim, SrcID, nSources);
                        outstr = outstr + "Passed data from Source " + SrcID.ToString() + " into COMID " + comid.ToString() + Environment.NewLine;
                    }
                };

            Sim.AQTSeg.RunID = "2D Run: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string errmessage = Sim.Integrate();
            if (errmessage == "")
            {
                archiveOutput(comid, Sim);
                errmessage = Sim.SaveJSON(ref jsonstring);

                if (errmessage != "")
                {
                    outstr += errmessage + Environment.NewLine;
                    return false;
                }
            }
            else { 
                   outstr += errmessage + Environment.NewLine;
                   return false;
                 };

            outstr += "--> Executed COMID " + comid.ToString();
            return true;
        }

    }

}


