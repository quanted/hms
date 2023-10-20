using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQTSegment;
using AQUATOX.Volume;
using AQUATOX.Loadings;

using System.Linq;
using Newtonsoft.Json;
using Data;
using System.IO;
using AQUATOX.Plants;
using AQUATOX.Animals;
using Streamflow;
using System.Net;
using System.Threading;
using System.Data;

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
            public Dictionary<string, int[]> boundary;
            [JsonProperty("divergent-paths")] public Dictionary<string, int[]> divergentpaths;
            public cWaterbodies waterbodies;
        }

        public class cWaterbodies
        {
            [JsonProperty("comid-wbcomid")] public Dictionary<int, int> comid_wb;
            [JsonProperty("waterbody-table")] public string[][] wb_table;
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
        /// Returns a summary of stream segments and waterbodies in network
        /// </summary>
        public String SNStats()
        {
            if (SN == null) return "";
            int WBCount = 0;
            if (SN.waterbodies != null)
                for (int i = 1; i < SN.waterbodies.wb_table.Length; i++) WBCount++;

            int FLCount = 0;
            for (int i = 0; i < SN.order.Length; i++)
                for (int j = 0; j < SN.order[i].Length; j++)
                {
                    int COMID = SN.order[i][j];
                    string CString = COMID.ToString();
                    bool in_waterbody = false;
                    if (SN.waterbodies != null) in_waterbody = NWM_Waterbody(COMID);
                    if (!in_waterbody) FLCount++; // don't count segments that are superceded by their lake/reservoir waterbody.
                };
            string outstr = "A total of " + (WBCount+FLCount).ToString() + " segments";
            if (WBCount > 0) outstr += " including " + WBCount + " lake/reservoir segments.";
            return outstr;
        }

        /// <summary>
        /// Returns true if the COMID is located within an NWM waterbody and does not need running as a stream segment.
        /// </summary>
        public bool NWM_Waterbody(int COMID)
        {
            int WBCOMID;
            if (SN.waterbodies != null) if (SN.waterbodies.comid_wb.TryGetValue(COMID, out WBCOMID))
              for (int i = 1; i < SN.waterbodies.wb_table.GetLength(0); i++)
                    {
                    if (int.Parse(SN.waterbodies.wb_table[i][0]) == WBCOMID)
                        return true;
                    }

            return false;
        } 

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

        [JsonIgnore] public IProgress<int> ProgHandle = null;  // report individual simulation progress (Task.Run)
        [JsonIgnore] public CancellationToken CancelToken;  // report individual simulation progress (Task.Run)

        /// <summary>
        /// converts stream network json string into SN variable and saves number of segments
        /// </summary>
        /// <param name="SNJSON">input json</param>
        public void CreateStreamNetwork(string SNJSON)
        {
            SN = Newtonsoft.Json.JsonConvert.DeserializeObject<streamNetwork>(SNJSON);
            nSegs = SN.network.Count() - 1;
        }

        public static List<string> MultiSegSimFlags()
        {
            return new List<string>(new string[] { "Nitrogen", "Phosphorus", "Organic Matter" });
        }

        public static string MultiSegSimName(List<bool> flags)
        {
            if ((flags[2]) && (flags[1])) return "MS_OM.json";  // [2] is organic matter, organic matter simulations require nitrogen
            else if (flags[2]) return "MS_OM_NoP.json";  // [1] is phosphorus, and this is not selected 
            else if ((flags[0]) && (flags[1])) return "MS_Nutrients.json";  // [0] is nitrogen; [1] is phosphorus 
            else if (flags[0]) return "MS_Nitrogen.json"; //flag [0] for nitrogen is the only one selected
            else return "MS_Phosphorus.json"; //flag [1] for phosphorus is the only one selected
        }

        public void LakeFlowsFromNWM(TVolume TVol, Data.TimeSeriesOutput<List<double>> ATSO)  // time series output must currently be in m3/s
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

            InflowLoad.ITSI.InputTimeSeries.Add("input", (TimeSeriesOutput)ATSO.ToDefault());

            KnownValLoad.UseConstant = false;
            KnownValLoad.MultLdg = 1;
            KnownValLoad.NoUserLoad = false;
            // KnownValLoad.Hourly = true;  12/14/22 

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
                if (!(Double.TryParse(entry.Value[3], out double volume)))
                    throw new ArgumentException("Cannot convert '" + entry.Value + "' to Double");

                if (volume < Consts.Tiny) volume = TVol.AQTSeg.Location.Locale.SiteLength.Val * 1000; //default minimum volume (length * XSec 1 m2) for now
                KnownValLoad.list.Add(date, volume);
                if (firstvol) TVol.InitialCond = volume;

                firstvol = false;
            }

            InflowLoad.Translate_ITimeSeriesInput(0, 1000 / 86400);  // default minimum flow of 1000 cmd for now
            InflowLoad.MultLdg = 86400;  // seconds per day
            // InflowLoad.Hourly = true;  12/14/22
            InflowLoad.UseConstant = false;

            TVol.LoadNotes1 = "Volumes from NWM in m3";
            TVol.LoadNotes2 = "NWM inflow converted from m3/s using multiplier";
            InflowLoad.ITSI = null;

            TVol.AQTSeg.CalcVelocity = true;
        }


        public void StreamFlowsFromNWM(TVolume TVol, Data.TimeSeriesOutput<List<double>> ATSO, bool useVelocity)  // time series output must currently be in m3/s
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

                InflowLoad.ITSI.InputTimeSeries.Add("input", (TimeSeriesOutput)ATSO.ToDefault());

                KnownValLoad.UseConstant = false;
                KnownValLoad.MultLdg = 1;
                KnownValLoad.NoUserLoad = false;
                // KnownValLoad.Hourly = true;  12/14/22

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

                    double VolCalc = 0;
                    if (flow > Consts.Tiny)
                       VolCalc = (flow / velocity) * (TVol.AQTSeg.Location.Locale.SiteLength.Val) * 1000;
                    // known value(m3) = flow(m3/s) / velocity(m/s) * sitelength(km) * 1000 (m/km)
                    if (VolCalc < Consts.Tiny) VolCalc = TVol.AQTSeg.Location.Locale.SiteLength.Val *1000; //default minimum volume (length * XSec 1 m2) for now
                    KnownValLoad.list.Add(date,VolCalc);
                    
                    if (firstvol) TVol.InitialCond = VolCalc;
                    firstvol = false;
                }

                InflowLoad.Translate_ITimeSeriesInput(0, 1000 / 86400);  // default minimum flow of 1000 cmd for now
                InflowLoad.MultLdg = 86400;  // seconds per day
                // InflowLoad.Hourly = true; 12/14/22
                InflowLoad.UseConstant = false;
                
                TVol.LoadNotes1 = "Volumes from NWM using flows in m3/s";       
                TVol.LoadNotes2 = "NWM inflow converted from m3/d using multiplier";
                InflowLoad.ITSI = null;

                if (TVol.AQTSeg.DynVelocity==null) TVol.AQTSeg.DynVelocity = new TLoadings();
                TLoadings VelocityLoad = TVol.AQTSeg.DynVelocity;
                TVol.AQTSeg.CalcVelocity = false;

                if (VelocityLoad.ITSI == null)
                {
                    TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                    TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                    input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    VelocityLoad.ITSI = input;
                }

                VelocityLoad.ITSI.InputTimeSeries.Add("input", (TimeSeriesOutput)ATSO.ToDefault());
                VelocityLoad.Translate_ITimeSeriesInput(1,0);  //bank 1 for velocity;  minimum velocity of zero
                VelocityLoad.MultLdg = 100;  // m/s to cm/s
                // VelocityLoad.Hourly = true; 12/14/22
                VelocityLoad.UseConstant = false;
                VelocityLoad.ITSI = null;
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

                DischargeLoad.ITSI.InputTimeSeries.Add("input", (TimeSeriesOutput)ATSO.ToDefault());
                DischargeLoad.Translate_ITimeSeriesInput(0,1000/86400);  //default minimum flow of 1000 cu m /d for now
                DischargeLoad.MultLdg = 86400;  // seconds per day
                // DischargeLoad.Hourly = true;  12/14/22
                DischargeLoad.UseConstant = false;
                TVol.LoadNotes1 = "Discharge from NWM in m3/s";                      // Add flexibility here in case of alternative data source
                TVol.LoadNotes2 = "Converted to m3/d using multiplier";
                DischargeLoad.ITSI = null;
            }
        }


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
        /// Reads the GeoJSON for a comid from web services
        /// </summary>
        /// <param name="comid">comid</param>
        /// <returns>JSON or error message</returns>
        /// 
        public string ReadGeoJSON(string comid)
        {
            //string requestURL = "https://qedcloud.net/hms/rest/api/";
            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            //string requestURL = "https://qed.epa.gov/hms/rest/api/";

            string component = "info";
            string dataset = "catchment";

            try
            {
                string rurl = requestURL + "" + component + "/" + dataset + "?streamGeometry=true&comid=" + comid;
                var request = (HttpWebRequest)WebRequest.Create(rurl);
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /// <summary>
        /// Reads the GeoJSON for a waterbody comid from web services
        /// </summary>
        /// <param name="comid">comid</param>
        /// <returns>JSON or error message</returns>
        /// 
        public string ReadWBGeoJSON(string WBcomid)
    {

        string requestURL = "https://watersgeo.epa.gov/arcgis/rest/services/NHDPlus_NP21/NHDSnapshot_NP21/MapServer/1/query";
        // https://watersgeo.epa.gov/arcgis/rest/services/NHDPlus_NP21/NHDSnapshot_NP21/MapServer/1/query?where=COMID%3D167267891&f=geojson

        try
        {
            string rurl = requestURL + "?f=geojson&where=COMID%3D" + WBcomid;
            var request = (HttpWebRequest)WebRequest.Create(rurl);
            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }


    /// <summary>
    /// Reads the stream network data structure from web services
    /// </summary>
    /// <param name="comid">Primary comid</param>
    /// <param name="endComid">Optional PourID</param>
    /// <param name="span">Optional up-stream distance to search in km</param>
    /// <returns>JSON or error message</returns>
    public string ReadStreamNetwork(string comid, string endComid, string span)
        {
            //string requestURL = "https://qedcloud.net/hms/rest/api/";
            string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            //string requestURL = "https://qed.epa.gov/hms/rest/api/";
            string component = "info";
            string dataset = "streamnetwork";

            try
            {
                string rurl = requestURL + component + "/" + dataset + "?mainstem=false&comid=" + comid;
                if (endComid != "") rurl += "&endComid=" + endComid;
                if (span != "") rurl += "&maxDistance=" + span;
                var request = (HttpWebRequest)WebRequest.Create(rurl);
                request.Timeout = 600000;  //10 minutes
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private class HydrologyTSI : TimeSeriesInput
        {
            public HydrologyTSI()
            {
                Source = "nwm";
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),  // overwritten below
                    EndDate = new DateTime(2015, 12, 31),
                    DateTimeFormat = "yyyy-MM-dd HH"
                };
                Geometry = new TimeSeriesGeometry()
                {
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        ["waterbody"] = "false"
                    }
                };
                DataValueFormat = "E3";
                TemporalResolution = "hourly";
                Units = "metric";
                OutputFormat = "json";
            }
        };

 
        private AQTSim JSON_to_AQTSim(string baseJSON, string setupjson, string comid, double lenkm, out string err)
        {
            AQTSim Sim = new AQTSim();
            err = Sim.Instantiate(baseSimJSON);

            Sim.AQTSeg.PSetup = Newtonsoft.Json.JsonConvert.DeserializeObject<Setup_Record>(setupjson);

            if (err != "") { return null; }

            Sim.AQTSeg.SetMemLocRec();

            Sim.AQTSeg.StudyName = "COMID: " + comid;
            Sim.AQTSeg.FileName = "AQT_Input_" + comid + ".JSON";
            if (lenkm > 0)
            {
                Sim.AQTSeg.Location.Locale.SiteLength.Val = lenkm;
                Sim.AQTSeg.Location.Locale.SiteLength.Comment = "From Multi-Seg Linkage";
            }
            return Sim;
        }

        /// <summary>
        /// Submit POST request to HMS web API for stream flow
        /// </summary>
        private TimeSeriesOutput<List<double>> submitHydrologyRequest(HydrologyTSI TSI, out string errmsg)  
        {
            // ------- Use Streamflow.Streamflow for Flask Request ------- 

            // Environment.SetEnvironmentVariable("FLASK_SERVER", "https://qedcloud.net/hms/rest/api/v2"); 
            Environment.SetEnvironmentVariable("FLASK_SERVER", "https://ceamdev.ceeopdev.net/hms/rest/api/v2");
            TSI.BaseURL = new List<string> { "https://ceamdev.ceeopdev.net/hms/rest/api/v2/hms/nwm/data/?" };

            Streamflow.Streamflow sf = new();
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            sf.Input = iFactory.SetTimeSeriesInput(TSI, new List<string>() { "streamflow" }, out errmsg);

            // If error occurs in input validation and setup, errormsg is returned as out, and object is returned as null
            if (errmsg.Contains("ERROR")) { return null; };

            TimeSeriesOutput<List<double>> TSO = (TimeSeriesOutput <List<double>>) sf.GetData(out errmsg);
            return TSO;

            // ------- Older code below as temporary reference/archive ------- 

            // ------- Manual Flask Request ------- 
            //string flaskURL = "https://ceamdev.ceeopdev.net/hms/rest/api/v2/hms/nwm/data/?";
            //string dataRequest = "source=nwm&dataset=streamflow&comid=" + comid;
            //dataRequest += "&startDate=" + dates.StartDate.ToString("yyyy-MM-dd");
            //dataRequest += "&endDate=" + dates.EndDate.ToString("yyyy-MM-dd");
            //dataRequest += "&waterbody=" + isWaterbody.ToString();

            //string dataURL = "https://ceamdev.ceeopdev.net/hms/rest/api/v2/hms/data";
            //FlaskData<TimeSeriesOutput<List<double>>> results = Utilities.WebAPI.RequestData<FlaskData<TimeSeriesOutput<List<double>>>>(dataRequest, 100, flaskURL, dataURL).Result;
            //if (results.status != "SUCCESS") errmsg = results.status;
            //else errmsg = "";
            //return results.data;

            // -------  Non Flask Request ------- 
            //string requestURL = "https://ceamdev.ceeopdev.net/hms/rest/api/";
            //string component = "hydrology";
            //string dataset = "streamflow";
            //errmsg = "";

            //var request = (HttpWebRequest)WebRequest.Create(requestURL + component + "/" + dataset + "/");
            //string json = JsonConvert.SerializeObject(TSI);
            //var data = Encoding.ASCII.GetBytes(json);  //StreamFlowInput previously initialized
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.ContentLength = data.Length;

            //using (var stream = request.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}
            //var response = (HttpWebResponse)request.GetResponse();
            //string rstring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //if (rstring.IndexOf("ERROR") >= 0)
            //{
            //    errmsg = "Error from web service returned: " + rstring;
            //    return null;
            //}
            //return JsonConvert.DeserializeObject<TimeSeriesOutput<List<double>>>(rstring);
        }

        public string PopulateLakeRes(int WBComid, string setupjson, out string jsondata)
        {
            jsondata = "";
            string WBCstr = WBComid.ToString();

            string err;
            AQTSim Sim = JSON_to_AQTSim(baseSimJSON, setupjson, WBCstr, -9999, out err);
            if (err != "") { return err; }

            HydrologyTSI TSI = new();
            TSI.DateTimeSpan.StartDate = Sim.AQTSeg.PSetup.FirstDay.Val;
            TSI.DateTimeSpan.EndDate = Sim.AQTSeg.PSetup.LastDay.Val;
            TSI.Geometry.ComID = WBComid;
            TSI.Geometry.GeometryMetadata["waterbody"] = "true";
            TSI.Source = "nwm";

            try
            {
                TimeSeriesOutput<List<double>> TSO = submitHydrologyRequest(TSI, out string errstr);
                if (errstr != "") return errstr;
                if (TSO.Data.Count == 0) return "ERROR: No data records were returned.  Your date range may be outside available NWM data.";
                TStateVariable TSV = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
                TVolume TVol = TSV as TVolume;
                LakeFlowsFromNWM(TVol, TSO);

                Sim.AQTSeg.Location.Locale.ICZMean.Val = TVol.InitialCond / Sim.AQTSeg.Location.Locale.SurfArea.Val;   // Estimate mean depth from volume & surface area
                                                   //m                //m3               //m2
                Sim.AQTSeg.Location.Locale.ICZMean.Comment = "Estimated from NWM based on surface area and initial volume";

                Sim.AQTSeg.Location.Locale.ZMax.Val = Sim.AQTSeg.Location.Locale.ICZMean.Val * 2.0;  //approximation for now
                Sim.AQTSeg.Location.Locale.ZMax.Comment = "Estimated as 2.0 x mean depth";

                Sim.AQTSeg.Location.Locale.SiteLength.Val =  Math.Sqrt(2.0 * Sim.AQTSeg.Location.Locale.SurfArea.Val * 1e-6) ;  //approximation for now
                                                     //km                                            // m2        // km2/m2
                Sim.AQTSeg.Location.Locale.SiteLength.Comment = "Rough estimate from Surface Area"; 

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

            string err;
            AQTSim Sim = JSON_to_AQTSim(baseSimJSON, setupjson, comid, lenkm, out err);
            if (err != "") { return err; }

            HydrologyTSI TSI = new();
            TSI.DateTimeSpan.StartDate = Sim.AQTSeg.PSetup.FirstDay.Val;
            TSI.DateTimeSpan.EndDate = Sim.AQTSeg.PSetup.LastDay.Val;
            TSI.Geometry.ComID = int.Parse(comid);
            TSI.Geometry.GeometryMetadata["waterbody"] ="false"; 
            TSI.Source = "nwm"; 

            try
            {
                TimeSeriesOutput<List<double>> TSO = submitHydrologyRequest(TSI, out string errstr);
                if (errstr != "") return errstr;
                StreamFlowsFromNWM(Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume,TSO,true);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            jsondata = "";

            string errmessage = Sim.SaveJSON(ref jsondata);
            return errmessage;
        }

        /// <summary>
        /// After up-stream segments have been run, this procedure apportions masses of state variables flowing into the current stream segment from one identified upstream segment or waterbody 
        /// </summary>
        /// <param name="Sim">The AQUATOX segment that is having data passed to it</param>
        /// <param name="SrcID">COMID of the upstream segments from which data is being passed</param>
        /// <param name="ninputs">Number of upstream inputs</param>
        /// <param name="AR">Data structure of archived inputs from which to gather model results</param>
        /// <param name="divergence_flows">a list of any additional divergence flows from source segment (flows not to this segment), for the complete set of time-steps of the simulation in m3/s</param> 
        public string Pass_Data(AQTSim Sim, int SrcID, int ninputs, archived_results AR = null, List <ITimeSeriesOutput<List<double>>> divergence_flows = null )  
        {
            //archived_results AR;
            if (AR == null)    // (AR.Equals(null)) crashed
            {
                archive.TryGetValue(SrcID, out AR);
                if (AR == null)
                {  // check to see if upstream segment is null because it is actually a lake/reservoir
                    if (SN.waterbodies.comid_wb.ContainsKey(SrcID))
                    {
                        SN.waterbodies.comid_wb.TryGetValue(SrcID, out SrcID);  // translate SrcID to the relevant WBCOMID
                        archive.TryGetValue(SrcID, out AR);
                    }
                if (AR == null) return "WARNING: Segment "+Sim.AQTSeg.StudyName+" is missing expected linkage data from "+SrcID;
                } 
            }

            Sim.AQTSeg.PSetup.UseDetrInputRecInflow.Val = false;  // 10/4/2021 inflow from upstream for detritus, not from input record

            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];

                if (((TSV.NState >= AllVariables.H2OTox) && (TSV.NState < AllVariables.TSS)) ||    // Select which state variables move from segment to segment
                    ((TSV.NState >= AllVariables.DissRefrDetr) && (TSV.NState <= AllVariables.SuspLabDetr)) || 
                    ((TSV.IsPlant()) && ( ((TPlant)TSV).IsPhytoplankton() || (((TPlant)TSV).IsMacrophyte() && (((TPlant)TSV).MacroType == TMacroType.Freefloat))) ) ||
                    ((TSV.IsAnimal()) && ((TAnimal)TSV).IsPlanktonInvert()))
                {
                    TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                    int ndates = AR.dates.Count();

                    TLoadings InflowLoad = tvol.LoadsRec.Alt_Loadings[0];
                    SortedList<DateTime, double> newlist = new SortedList<DateTime, double>();

                    for (int i = 0; i < ndates; i++)
                    {
                        double OutVol = AR.washout[i];  // out volume to this segment from upstream segment

                        double frac_this_segment = 1.0;
                        double totOutVol = OutVol;
                        if (divergence_flows != null)
                            foreach (ITimeSeriesOutput<List<double>> its in divergence_flows)
                            {
                                totOutVol = totOutVol + Convert.ToDouble(its.Data.Values.ElementAt(i)[0]) * 86400 ;     //TODO FIXME potential issue if time-step chagnes or time-period is increased since NWM data gathering
                                // m3/d      m3/d                                         m3/s               s/d
                                frac_this_segment = OutVol / totOutVol;
                            }

                        double InVol = InflowLoad.ReturnLoad(AR.dates[i]);  // inflow volume to current segment,   If velocity is not used, must be estimated as current seg. outflow 

                        if (InVol < Consts.Tiny) newlist.Add(AR.dates[i], 0);
                        else if (ninputs == 1) newlist.Add(AR.dates[i], AR.concs[iTSV][i] * (OutVol / InVol)* frac_this_segment);  // first or only input
                        else newlist.Add(AR.dates[i], TSV.LoadsRec.Loadings.list.Values[i] + AR.concs[iTSV][i] * (OutVol / InVol) * frac_this_segment);  //adding second or third inputs

                    }

                    TSV.LoadsRec.Loadings.list = newlist;
                    TSV.LoadsRec.Loadings.UseConstant = false;
                    // TSV.LoadsRec.Loadings.Hourly = true; 12/14/22
                    TSV.LoadNotes1 = "Linkage Data from " + SrcID.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// Instantiates a json but overwrites the setup with the master setup record.
        /// </summary>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="errstr">string holding info about an error if present</param>
        /// <param name="jsonstring">the input json </param>
        public AQTSim Instantiate_with_setup(string setupjson, ref List<string> outstr, string jsonstring)
        {
            AQTSim Sim = new AQTSim();
            string errstr = Sim.Instantiate(jsonstring);
            if (errstr != "")
            {
                outstr.Add("ERROR: " + errstr);
                return null;
            }
            Sim.AQTSeg.SetMemLocRec();

            Sim.AQTSeg.PSetup = Newtonsoft.Json.JsonConvert.DeserializeObject<Setup_Record>(setupjson);
            return Sim;

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
        /// <param name="divergence_flows">a list of any additional divergence flows from source segment (flows not to this segment), for the complete set of time-steps of the simulation in m3/s</param> 
        /// <param name="outofnetwork">array of COMIDs that are out of the network water sources.</param>  
        /// <returns>boolean: true if the run was completed successfully</returns>/// 
        public bool executeModel(int comid, string setupjson, ref List<string> outstr, ref string jsonstring, List<ITimeSeriesOutput<List<double>>> divergence_flows = null, int[] outofnetwork = null)         
        {
            AQTSim Sim = Instantiate_with_setup(setupjson, ref outstr, jsonstring);
            Sim.AQTSeg.ProgHandle = this.ProgHandle;
            Sim.AQTSeg._ct = this.CancelToken;

            if (SVList == null)
            {
                SVList = new List<string>();
                foreach (TStateVariable SV in Sim.AQTSeg.SV)
                {
                    SVList.Add(SV.PName+" ("+SV.StateUnit+")");   //save list of SVs for output
                }
            }
                
            int nSources = 0;
            if (SN != null)
            {
                if (SN.sources != null)
                 if (SN.sources.TryGetValue(comid.ToString(), out int[] Sources))
                    foreach (int SrcID in Sources)
                    {
                        if ((SrcID != comid) && !outofnetwork.Contains(SrcID))  // don't pass data from out of network segments
                        {
                            nSources++;
                            string errstr = Pass_Data(Sim, SrcID, nSources, null, divergence_flows);
                            if (errstr != "") outstr.Add(errstr);
                                else outstr.Add("INFO: Passed data from Source " + SrcID.ToString() + " into COMID " + comid.ToString());
                        }
                    };
            
                if ((SN.waterbodies != null) && (SN.sources != null))
                {   // pass data into Waterbodies from adjacent stream segments
                    if (SN.waterbodies.comid_wb.ContainsValue(comid))  // if the comid is a waterbody
                      foreach (KeyValuePair<int, int> entry in SN.waterbodies.comid_wb)  
                        if (entry.Value == comid)  // for each stream segment in waterbody
                          if (SN.sources.TryGetValue(entry.Key.ToString(), out int[] Sources)) //get the sources for the stream segment
                            foreach (int SrcID in Sources) //loop through the sources
                              if ((SrcID != entry.Key) && !outofnetwork.Contains(SrcID))  // don't pass data from out of network segments
                                {
                                   archive.TryGetValue(SrcID, out archived_results AR);  // don't pass data from segments not run (e.g. internal segments in the waterbody that are irrelevant)
                                    if (AR != null)
                                    {
                                        nSources++;
                                        string errstr = Pass_Data(Sim, SrcID, nSources, null, divergence_flows);
                                        if (errstr != "") outstr.Add(errstr);
                                            else outstr.Add("INFO: Passed data from Source " + entry.Key.ToString() + " into WBCOMID " + comid.ToString());
                                    }
                                }
                }
            }


            Sim.AQTSeg.RunID = "Run: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string errmessage = Sim.Integrate();
            if (errmessage == "")
            {
                archiveOutput(comid, Sim);
                errmessage = Sim.SaveJSON(ref jsonstring);

                if (errmessage != "")
                {
                    outstr.Add("ERROR: " + errmessage);
                    return false;
                }
            }
            else {
                outstr.Add("ERROR: " + errmessage);
                return false;
                 };

            outstr.Add("INFO: " + "--> Executed COMID " + comid.ToString());
            return true;
        }


    }

    public class Lake_Surrogates
    {
        public double VersionNum = 1.0;
        public DataTable table;
        public Dictionary<string, AQTSim> Sims;

        public Lake_Surrogates(string tablefilen, string jsonDir)
        {
            if (!File.Exists(tablefilen)) return;
            string json = File.ReadAllText(tablefilen);
            table = JsonConvert.DeserializeObject<DataTable>(json);
            Sims = new Dictionary<string, AQTSim>();
            foreach (DataRow row in table.Rows)
            {
                string fileN = row.Field<string>("Filename");
                // if (!File.Exists(jsonDir+fileN)) continue;  raise error instead
                json = File.ReadAllText(jsonDir+fileN);
                AQTSim sim = JsonConvert.DeserializeObject<AQTSim>(json, AQTSim.AQTJSONSettings());
                sim.SavedRuns = null;
                sim.AQTSeg.ClearResults();
                Sims.Add(fileN,sim);
            }
        }
    }

    public class LSKnownTypesBinder : AQTKnownTypesBinder
    {
        public LSKnownTypesBinder(): base()
        {
            KnownTypes.Add(typeof(Lake_Surrogates));
            KnownTypes.Remove(typeof(Dictionary<string, AQUATOXSegment>));   //duplicate dictionaries confuse the binder
            KnownTypes.Add(typeof(Dictionary<string, AQTSim>)); 
        }
    }

}


