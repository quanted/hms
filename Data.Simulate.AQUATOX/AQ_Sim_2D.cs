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
using System.Net;
using System.Threading;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using AQUATOX.OrgMatter;
using System.Globalization;
using System.Data.SQLite;

namespace AQUATOX.AQSim_2D

{
    public class AQSim_2D
    {

        public class webServiceURLsClass
        {
            public string hmsRest = "https://qedcloud.net/hms/rest/api/";  //defaults, this class is read from json for GUI.AQUATOX
            public string nationalMap = "https://hydro.nationalmap.gov/arcgis/rest/services/";
            public string watersGeo = "https://watersgeo.epa.gov/arcgis/rest/services/";
            public string hawqsAPI = "https://dev-api.hawqs.tamu.edu/";
        }

        public static webServiceURLsClass webServiceURLs = new();

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

        public class HAWQSInfo
        {
            public List<string> upriverHUCs;
            public List<string> HAWQSboundaryHUCs;
            public string[] colnames;
            public List<string> modelDomain;  // var for multi-seg runs ensure that boundary conditions are not set within model domain

            private Dictionary<string, List<string>> fromtoData = new Dictionary<string, List<string>>();
            private string fromtofilen = "";

            public void LoadFromtoData(string HUC_ID)
            {
                string HUCStr = HUC_ID.Length.ToString();
                string fromtopath = $@"..\2D_Inputs\HAWQS_Data\FromTo\fromto{HUC_ID.Substring(0, 2)}_{HUCStr}.csv";
                if (fromtofilen == fromtopath) return;  //already loaded

                if (File.Exists(fromtopath))  //csv file with "OutletHUC, InletHUC"
                {
                    using (TextFieldParser parser = new TextFieldParser(fromtopath))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");

                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();
                            if (fields.Length >= 2)
                            {
                                if (!fromtoData.ContainsKey(fields[1]))
                                {
                                    fromtoData[fields[1]] = new List<string>();
                                }
                                fromtoData[fields[1]].Add(fields[0]);
                            }
                        }
                    }
                    fromtofilen = fromtopath;
                }
                else throw new Exception("File Not Found: " + fromtopath);
            }

            public List<string> boundaryHUCs(string HUC_ID, bool include_self)  //returns the upstream boundaries for the given HUC_ID
            {
                if (HUC_ID == "") return null;
                if (fromtoData.TryGetValue(HUC_ID, out List<string> matchingHUCs))
                {
                    if (include_self) matchingHUCs.Add(HUC_ID);
                    return matchingHUCs;
                }
                else
                {
                    List<string> boundary = new List<string>();
                    if (include_self) boundary.Add(HUC_ID);
                    return boundary;
                }
            }


            public void AddSourceHUCs(string HUC_ID)
            {
                // function to check if the HUC starts with the same first eight characters
                bool IsSameRegion(string huc1, string huc2) => huc1.Substring(0, 8) == huc2.Substring(0, 8);
                // function to check if the HUC is located within the model domain (may span multiple HUC8s)
                bool InModelDomain(string huc) => modelDomain.Contains(huc);

                foreach (var boundaryHuc in boundaryHUCs(HUC_ID, false))  // Add the initial HUC_ID's BoundaryHUCs to the list if they aren't already present
                {
                    if (!upriverHUCs.Contains(boundaryHuc))
                    {
                        upriverHUCs.Add(boundaryHuc);

                        if (IsSameRegion(boundaryHuc, HUC_ID))
                            AddSourceHUCs(boundaryHuc); // Recursively add BoundaryHUCs if the identified boundaryHuc is in the same region
                        else if ((!HAWQSboundaryHUCs.Contains(boundaryHuc)) && (!InModelDomain(boundaryHuc)))
                        {
                            HAWQSboundaryHUCs.Add(boundaryHuc);  //identify those segments out of the region as upstream bound
                        }
                    }
                }
            }
        }

        public class HAWQSInput
        {
            public string dataset { get; set; } = "HUC14";
            public string downstreamSubbasin { get; set; } = "01010002010504";
            public string[] upstreamSubbasins { get; set; }
            public string upstreamDataSource { get; set; } = "NWM";
            public SetHrus setHrus { get; set; } = new SetHrus();
            public string weatherDataset { get; set; } = "PRISM";
            public string startingSimulationDate { get; set; } = "1981-01-01";
            public string endingSimulationDate { get; set; } = "1984-12-31";
            public int warmupYears { get; set; } = 2;
            public string outputPrintSetting { get; set; } = "daily";
            public ReportData reportData { get; set; } = new ReportData();
            public bool disaggregateComids { get; set; } = false;
        }

        public class SetHrus
        {
            public string method { get; set; } = "none";
            public double target { get; set; } = 0;
            public string units { get; set; } = "none";
            public string[] exemptLanduse { get; set; } = new string[] { "AGWF", "AGWR", "AGWT", "RIWF", "RIWN", "UPWF", "UPWN", "WATR", "WETF", "WETL", "WETN" };
            public string[] noAreaRedistribution { get; set; } = new string[] { "AGWF", "AGWR", "AGWT", "RIWF", "RIWN", "UPWF", "UPWN", "WATR", "WETF", "WETL", "WETN" };
        }

        public class ReportData
        {
            public string[] formats { get; set; } = new string[] { "csv" };
            public string units { get; set; } = "metric";
            public Outputs outputs { get; set; } = new Outputs();
        }

        public class Outputs
        {
            public Rch rch { get; set; } = new Rch();
            public Sub sub { get; set; } = new Sub();
        }

        public class Rch
        {
            public string[] statistics { get; set; } = new string[] { "daily" };

            public string[] subbasins { get; set; }
        }

        public class Sub
        {
            public string[] statistics { get; set; } = new string[] { "daily" };
        }




        /// <summary>
        /// Info linked from HAWQS
        /// </summary>
        public HAWQSInfo HAWQSInf = null;

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
            string outstr = "A total of " + (WBCount + FLCount).ToString() + " segments";
            if (WBCount > 0) outstr += " including " + WBCount + " lake/reservoir segments.";
            return outstr;
        }

        /// <summary>
        /// Returns the WBCOMID associated with the streamflow COMID or -1 if the COMID is not located within an NWM waterbody
        /// </summary>
        public int NWM_WaterbodyID(int COMID)
        {
            int WBCOMID;
            if (SN.waterbodies != null) if (SN.waterbodies.comid_wb.TryGetValue(COMID, out WBCOMID))
                    for (int i = 1; i < SN.waterbodies.wb_table.GetLength(0); i++)
                    {
                        if (int.Parse(SN.waterbodies.wb_table[i][0]) == WBCOMID)
                            return WBCOMID;
                    }

            return -1;
        }


        /// <summary>
        /// Returns true if the COMID is located within an NWM waterbody and does not need running as a stream segment.
        /// </summary>
        public bool NWM_Waterbody(int COMID)
        {
            return NWM_WaterbodyID(COMID) != -1;
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
            public double[] washout; // m3
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

            InflowLoad.Translate_ITimeSeriesInput(0, 86400, 1000);  // default minimum flow of 1000 cmd for now, multiplier of seconds/day
            InflowLoad.MultLdg = 1;  // seconds per day
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
                    if (VolCalc < Consts.Tiny) VolCalc = TVol.AQTSeg.Location.Locale.SiteLength.Val * 1000; //default minimum volume (length * XSec 1 m2) for now
                    KnownValLoad.list.Add(date, VolCalc);

                    if (firstvol) TVol.InitialCond = VolCalc;
                    firstvol = false;
                }

                InflowLoad.Translate_ITimeSeriesInput(0, 86400, 1000);  // default minimum flow of 1000 cmd for now  multiplier is seconds per day
                InflowLoad.MultLdg = 1;
                // InflowLoad.Hourly = true; 12/14/22
                InflowLoad.UseConstant = false;

                TVol.LoadNotes1 = "Volumes from NWM using flows in m3/s";
                TVol.LoadNotes2 = "NWM inflow converted from m3/d using multiplier";
                InflowLoad.ITSI = null;

                if (TVol.AQTSeg.DynVelocity == null) TVol.AQTSeg.DynVelocity = new TLoadings();
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
                VelocityLoad.Translate_ITimeSeriesInput(1, 1, 0);  //bank 1 for velocity;  minimum velocity of zero
                VelocityLoad.MultLdg = 100;  // m/s to cm/s
                // VelocityLoad.Hourly = true; 12/14/22
                VelocityLoad.UseConstant = false;
                VelocityLoad.ITSI = null;
            }
            else  // don't use velocity, use discharge and manning's equation
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
                DischargeLoad.Translate_ITimeSeriesInput(0, 86400, 1000);  //default minimum flow of 1000 cu m /d for now, multiplier is seconds/day
                DischargeLoad.MultLdg = 1;
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
            string requestURL = webServiceURLs.hmsRest;

            string component = "info";
            string dataset = "catchment";

            try
            {
                string rurl = requestURL + "" + component + "/" + dataset + "?streamGeometry=true&comid=" + comid;
                var request = (HttpWebRequest)WebRequest.Create(rurl);  // replace with HttpClient constructs
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
        public async Task<string> ReadWBGeoJSON(string WBcomid)
        {
            string requestURL = webServiceURLs.watersGeo + "NHDPlus_NP21/NHDSnapshot_NP21/MapServer/1/query";

            try
            {
                string rurl = requestURL + "?f=geojson&where=COMID%3D" + WBcomid;
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(rurl))
                using (HttpContent content = response.Content)
                {
                    // Check if the response is successful
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"Error: {response.StatusCode}, {response.ReasonPhrase}");

                    string result = await content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ReadHUCGeoJSON(string HUCStr, string HUCID)
        {
            try
            {
                string requestURL = webServiceURLs.nationalMap + "wbd/MapServer";

                // Determine the layer ID based on HUCStr
                int layerID = -1;
                switch (HUCStr)
                {
                    case "HUC8":
                        layerID = 4; // Replace with the correct layer ID for HUC8
                        break;
                    case "HUC10":
                        layerID = 5; // Replace with the correct layer ID for HUC10
                        break;
                    case "HUC12":
                        layerID = 6; // Replace with the correct layer ID for HUC12
                        break;
                    case "HUC14":
                        return "ERROR: HUC14 GEOJSON web service is not available";
                    default:
                        return "ERROR: Invalid HUCStr specified";
                }

                // Construct the URL with the layer ID and HUCID
                string queryParams = $"f=geojson&where={HUCStr}='{HUCID}'";
                string fullURL = $"{requestURL}/{layerID}/query?{queryParams}";

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(fullURL))
                using (HttpContent content = response.Content)
                {
                    // Check if the response is successful
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"Error: {response.StatusCode}, {response.ReasonPhrase}");

                    string result = await content.ReadAsStringAsync();
                    return result;
                }
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
            string requestURL = webServiceURLs.hmsRest;
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


        private AQTSim JSON_to_AQTSim(string baseJSON, string setupjson, string segtype, string IDstr, double lenkm, out string err)
        {
            AQTSim Sim = new AQTSim();
            err = Sim.Instantiate(baseSimJSON);

            Sim.AQTSeg.PSetup = Newtonsoft.Json.JsonConvert.DeserializeObject<Setup_Record>(setupjson);

            if (err != "") { return null; }

            Sim.AQTSeg.SetMemLocRec();

            Sim.AQTSeg.StudyName = segtype + ": " + IDstr;
            Sim.AQTSeg.FileName = "AQT_Input_" + IDstr + ".JSON";
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

            Environment.SetEnvironmentVariable("FLASK_SERVER", webServiceURLs.hmsRest + "v2");
            TSI.BaseURL = new List<string> { webServiceURLs.hmsRest + "v2/hms/nwm/data/?" };

            Streamflow.Streamflow sf = new();
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            sf.Input = iFactory.SetTimeSeriesInput(TSI, new List<string>() { "streamflow" }, out errmsg);

            // If error occurs in input validation and setup, errormsg is returned as out, and object is returned as null
            if (errmsg.Contains("ERROR")) { return null; };

            TimeSeriesOutput<List<double>> TSO = (TimeSeriesOutput<List<double>>)sf.GetData(out errmsg);
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

        public void setupLoad(TLoadings load, int count)
        {
            load.list.Clear();
            load.list.Capacity = count;
            load.UseConstant = false;
            load.MultLdg = 1;
            load.NoUserLoad = false;
            load.ITSI = null;
        }

        /// <summary>
        /// Creates inputs for a Volume model for the AQUATOX segment using HAWQS flows, Manning's Equation, and site-specific geometry
        /// </summary>
        /// <param name="TVol">The AQUATOX Volume object</param>
        /// <param name="ThisSeg">The full set of HAWQS daily results for this segment</param>
        /// <param name="FirstDate">The first date fo the AQUATOX simulation</param>
        public void VolFlowFromHAWQS(TVolume TVol, Dictionary<DateTime, HAWQSRCHRow> ThisSeg, DateTime FirstDate)
        {
            TVol.Discharg = ThisSeg[FirstDate].vals[1] * 86400; // convert to m3/day
            TVol.InitialCond = Math.Max(TVol.Manning_Volume(), TVol.AQTSeg.Location.Locale.SiteLength.Val * 1000); //default minimum volume (length * XSec 1 m2) for now ;     

            TVol.Calc_Method = VolumeMethType.Manning;

            TLoadings DischargLoad = TVol.LoadsRec.Alt_Loadings[1];   // array slot [0] is "inflow" in this case (TVolume)

            setupLoad(DischargLoad, ThisSeg.Count);

            foreach (KeyValuePair<DateTime, HAWQSRCHRow> pair in ThisSeg)
            {
                double disch = (pair.Value.vals[0] * 86400 > 0) ? pair.Value.vals[0] * 86400 : 0 ;
                DischargLoad.list.Add(pair.Key, disch);  //flow from RCH_Daily file -- converted from cms to m3/d
            }

            TVol.LoadNotes1 = "Flows from HAWQS converted to m3/day";
            TVol.LoadNotes2 = "Manning's Equation Used to Estimate Volume";
            TVol.AQTSeg.CalcVelocity = true;
        }

        /// <summary>
        /// Link COMID data from HAWQS into this lake/reservoir JSON
        /// Add any boundary conditions to point-source loads, inflow loads will come from linkage of other in-model-domain COMIDS
        /// </summary>
        /// <param name="HRD">HAWQS Reach Data -- a nested dictionary of relevant inputs for each HAWQS subbasin</param>
        /// <param name="Boundaries">The list of upstream boundaries that have data relevant to the SegID segment unit</param>
        /// <param name="SegID">The identifying string for the unit-- can be COMID, or HUC8-HUC14 identifier</param>
        /// <param name="SegIndex">Count of the number of COMIDs that have passed data to this Lake/Reservoir, including this one</param>
        /// <param name="WBJSON">The input waterbody JSON that will be modified with this COMID data from HAWQS</param>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="link_boundary">should boundary condition inflows from HAWQS be added to this segment-- i.e. is the up-river segment out of the AQUATOX domain?</param>
        /// <param name="link_overland">should overland flows be added to this segment</param>
        /// <returns>jsondata (the AQUATOX simulation JSON after HAWQS data has been linked), errors</returns>
        public async Task<(string, string)> HAWQS_add_COMID_to_WB(Dictionary<long, Dictionary<DateTime, HAWQSRCHRow>> HRD, List<string> Boundaries, string SegID, int SegIndex, string WBJSON, bool link_boundary, bool link_overland)
        {
            string jsondata = "";
            long ID = long.Parse(SegID);

            Dictionary<DateTime, HAWQSRCHRow> ThisSeg = HRD[ID];
            List<Dictionary<DateTime, HAWQSRCHRow>> BoundSegs = new();
            foreach (string bound in Boundaries)
            {
                long parsedBound = long.Parse(bound);
                if (HRD.ContainsKey(parsedBound)) BoundSegs.Add(HRD[long.Parse(bound)]);  // identify the relevant boundary condition inputs
                else return ("", "ERROR: boundary condition " + bound + " missing for Segment ID " + SegID);
            }

            string HUCStr = SegID.Length.ToString();

            AQTSim Sim = JsonConvert.DeserializeObject<AQTSim>(WBJSON, AQTSim.AQTJSONSettings());

            // Volume has already been loaded from NWM.

            DateTime FirstDay = Sim.AQTSeg.PSetup.FirstDay.Val;

            string[] components = { "SED", "NO3", "NH4", "MINP", "CHLA", "CBOD", "DISOX" };
            AllVariables[] SVs = { AllVariables.TSS, AllVariables.Nitrate, AllVariables.Ammonia, AllVariables.Phosphate, AllVariables.NullStateVar, AllVariables.DissRefrDetr, AllVariables.Oxygen, AllVariables.Temperature };

            TPlant PPhyto = null;
            for (AllVariables AlgLoop = Consts.FirstAlgae; AlgLoop <= Consts.LastAlgae; AlgLoop++)  //identify any phytoplankton in the simulation for CHLA linkage
            {
                TPlant TP = Sim.AQTSeg.GetStatePointer(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (TP != null)
                {
                    if (TP.IsPhytoplankton())  //Chl-a is assigned to first phytoplankton in the simulation 
                    {
                        PPhyto = TP;
                        SVs[4] = TP.NState;
                        break;
                    }
                }
            }

            double InitFlow = ThisSeg[FirstDay].vals[1];  //column for flow_OUT
            for (int i = 0; i < components.Length; i++)
            {
                bool isSED = (components[i] == "SED");  //sediment is handled differently in units and modeling
                bool isCBOD = (components[i] == "CBOD"); //unique input data structure in AQUATOX for CBOD
                bool isCHLA = (components[i] == "CHLA");  //Convert CHLA to biomass units

                int col_IN = Array.IndexOf(HAWQSInf.colnames, components[i] + "_IN");  //identify relevant columns in RCH_OUT file
                int col_OUT = col_IN + 1;

                TStateVariable TSV = Sim.AQTSeg.GetStatePointer(SVs[i], T_SVType.StV, T_SVLayer.WaterCol);
                if (TSV != null)
                {
                    double COMID_IC = (InitFlow <= 0) ? 0 : ThisSeg[FirstDay].vals[col_OUT] / InitFlow * 0.0115740;  // (kg/d) / (m3/s) * (mg/kg * d/s * m3/L)
                    if (isSED) COMID_IC *= 1000;  // sed is in units of tons
                    if (isCHLA) COMID_IC = COMID_IC * PPhyto.PAlgalRec.Plant_to_Chla.Val / 1.90;
                    //         mg/L OM  =   mg/L chl-a              C to chl-a          g OM / g OC

                    if (SegIndex == 1) TSV.InitialCond = COMID_IC;
                    else TSV.InitialCond = (COMID_IC + TSV.InitialCond * (SegIndex - 1)) / SegIndex;  //Average initial condition for all COMIDs within the WBCOMID

                    TLoadings PSLoad = TSV.LoadsRec.Alt_Loadings[0];  // Alt_Loadings[0] is point source loadings, used for boundary condition rivers in this case
                    TLoadings NPSLoad = TSV.LoadsRec.Alt_Loadings[2];  // Alt_Loadings[2] is non-point source loadings

                    if (isCBOD)
                    {
                        DetritalInputRecordType DIR = ((TDissRefrDetr)TSV).InputRecord;
                        DIR.InitCond = TSV.InitialCond;
                        DIR.DataType = DetrDataType.CBOD;
                        PSLoad = DIR.Load.Alt_Loadings[0];
                        NPSLoad = DIR.Load.Alt_Loadings[2];  //deal with different data structure for susp&dissolved detritus (CBOD)
                    }

                    bool overlandrelevant = (!isSED && !isCHLA);  //non-point source loads not relevant in AQUATOX for these state vars.

                    if (SegIndex == 1)
                    {
                        TSV.LoadNotes1 = "";
                        TSV.LoadNotes2 = "";
                    }

                    if (link_boundary || isSED)
                    {
                        if (SegIndex == 1) setupLoad(PSLoad, ThisSeg.Count);  //get PS loads ready to receive inputs 
                        TSV.LoadNotes1 = "Inflow Loads from HAWQS Linkage (Daily RCH file)";
                        if (isSED) TSV.LoadNotes1 = "In-reach Sediment Values from HAWQS Linkage (Daily RCH file)";
                    }


                    if (link_overland && overlandrelevant)
                    {
                        if (SegIndex == 1) setupLoad(NPSLoad, ThisSeg.Count);  //get non-point source loads ready to receive inputs
                        TSV.LoadNotes2 = "Non-Point Source Loads from HAWQS Simulation";
                    }

                    foreach (KeyValuePair<DateTime, HAWQSRCHRow> pair in ThisSeg)  //for each date
                    {
                        double boundInput = 0;
                        double boundflow = 0;

                        foreach (Dictionary<DateTime, HAWQSRCHRow> BoundSeg in BoundSegs)
                        {
                            boundInput += BoundSeg[pair.Key].vals[col_OUT];  // kg/d
                            boundflow += BoundSeg[pair.Key].vals[1];  // flow out in m3/s
                        }

                        double inflowMass = 0;  //inflow mass in g for boundary conditions (out of linked-segment domain)
                        if (boundflow > Consts.Tiny)
                            inflowMass = boundInput * 1000;  //  (kg/d) / (g / kg) 
                        if (components[i] == "SED") inflowMass *= 1000;   // in HAWQS sed is in units of tons, other components in kg

                        if (link_boundary || isSED)
                        {
                            if (!PSLoad.list.ContainsKey(pair.Key)) PSLoad.list.Add(pair.Key, inflowMass);
                            else PSLoad.list[pair.Key] = PSLoad.list[pair.Key] + inflowMass;  //sum all boundary condition inputs
                        }

                        if (link_overland && overlandrelevant)
                        {
                            double comp_IN = ThisSeg[pair.Key].vals[col_IN];      // kg/d
                            double overland_flow = Math.Max(0, (comp_IN - boundInput) * 1000);   // g/d = kg/d * 1000 g/kg -- Math.Max ensures no zeros due to rounding error
                            if (!NPSLoad.list.ContainsKey(pair.Key)) NPSLoad.list.Add(pair.Key, overland_flow);
                            else NPSLoad.list[pair.Key] = NPSLoad.list[pair.Key] + overland_flow;  //sum all overland inputs
                        }
                    }
                }
            }

            // Water Temperature Linkage Here
            TStateVariable TTemp = Sim.AQTSeg.GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            int col_WTMP = Array.IndexOf(HAWQSInf.colnames, "WTMP");  //identify relevant columns in RCH_OUT file
            if ((TTemp != null) && (col_WTMP >= 0))
            {
                TTemp.InitialCond = ThisSeg[FirstDay].vals[col_WTMP];
                TTemp.LoadNotes1 = "Temperature from HAWQS RCH data";
                TTemp.LoadNotes2 = "";
                setupLoad(TTemp.LoadsRec.Loadings, ThisSeg.Count);
                foreach (KeyValuePair<DateTime, HAWQSRCHRow> pair in ThisSeg)  //for each date
                {
                    TTemp.LoadsRec.Loadings.list.Add(pair.Key, pair.Value.vals[col_WTMP]);  // add daily WT in deg C
                }
            }

            string errmessage = Sim.SaveJSON(ref jsondata);
            return (jsondata, errmessage);
        }

        public bool ReadHUCGeometry(AQTSim Sim, string HUCID)
        {
            int HUClen = HUCID.Length;
            string rtesFilen;
            string subbasinFilen;

            if (HUClen == 8)
            {
                rtesFilen = "rtes-huc8-2024-03-01-071355.csv";
                subbasinFilen = "subbasins-huc8-2023-05-02-124826.csv";
            }
            else
            if (HUClen == 10)
            {
                rtesFilen = "rtes-huc10-2024-03-01-071352.csv";
                subbasinFilen = "subbasins-huc10-2023-05-02-124829.csv";
            }
            else
            {
                rtesFilen = "rtes-huc12-2024-03-01-070604.csv";
                subbasinFilen = "subbasins-huc12-2023-05-23-115646.csv";
            }

            string DBDir = @"..\2D_Inputs\HAWQS_data\RTE_SUB\";

            double[] ExtractFromCSV(string filePath, string uniqueId, int[] columnIndices)
            {
                double[] result = new double[columnIndices.Length];

                using (var reader = new StreamReader(filePath))
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values[0] == uniqueId)
                        {
                            for (int i = 0; i < columnIndices.Length; i++)
                            {
                                if (columnIndices[i] < values.Length)
                                    result[i] = double.Parse(values[columnIndices[i]], CultureInfo.InvariantCulture);
                            }
                            return result;
                        }
                    }

                return result;
            }

            try
            {

                double[] SubData = ExtractFromCSV(DBDir + subbasinFilen, HUCID, new int[] { 3, 4, 8 });
                double CumulativeArea = SubData[0];
                double Lat = SubData[1];
                double SubElev = SubData[2];

                double[] RTEData = ExtractFromCSV(DBDir + rtesFilen, HUCID, new int[] { 1, 2 });
                double Slope = RTEData[0];
                double Length = RTEData[1];

                double CH_W2 = 1.29d * Math.Pow(CumulativeArea, 0.6);  //estimates based on equations from TAMU/HAWQS
                double CH_D = 0.13d * Math.Pow(CumulativeArea, 0.4);

                Sim.AQTSeg.Location.Locale.SiteLength.Val = Length;  //km
                Sim.AQTSeg.Location.Locale.SiteLength.Comment = "RTE data inputs from HAWQS/SWAT";

                Sim.AQTSeg.Location.Locale.Channel_Slope.Val = Slope;  //m/m
                Sim.AQTSeg.Location.Locale.Channel_Slope.Comment = "RTE data inputs from HAWQS/SWAT";

                Sim.AQTSeg.Location.Locale.ICZMean.Val = CH_D;  //m
                Sim.AQTSeg.Location.Locale.ICZMean.Comment = "Estimated based on Subbasin Cumulative Area using eqn. from HAWQS/SWAT";
                Sim.AQTSeg.Location.Locale.ZMax.Val = Sim.AQTSeg.Location.Locale.ICZMean.Val * 2.0;  //approximation for now
                Sim.AQTSeg.Location.Locale.ZMax.Comment = "Estimated as 2.0 x mean depth";

                Sim.AQTSeg.Location.Locale.SurfArea.Val = CH_W2 * Length * 1000d;  //m2
                Sim.AQTSeg.Location.Locale.SurfArea.Comment = "Estimated based on Subbasin Cumulative Area using eqn. from HAWQS/SWAT";

                Sim.AQTSeg.Location.Locale.Latitude.Val = Lat;
                Sim.AQTSeg.Location.Locale.Latitude.Comment = "SUB data inputs from HAWQS/SWAT";
                Sim.AQTSeg.Location.Locale.Altitude.Val = SubElev;
                Sim.AQTSeg.Location.Locale.Altitude.Comment = "SUB data inputs from HAWQS/SWAT";

                Sim.AQTSeg.Location.Locale.UseEnteredManning.Val = true;
                Sim.AQTSeg.Location.Locale.EnteredManning.Val = 0.040; //natural channel default
                Sim.AQTSeg.Location.Locale.EnteredManning.Comment = "natural channel default";

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadHUCGeometry Error occurred: " + ex.Message);
                return false;
            }
        }


        /// <summary>
        /// Updates the AQTSim geometry using geometry from Local Database
        /// </summary>
        /// <param name="Sim">Simulation to be updated</param>
        /// <param name="COMID">NHD COMID</param>
        /// <returns>success of update</returns>
        public async Task<bool> ReadCOMIDGeometry(AQTSim Sim, string COMID)
        {
            try
            {
                string dbpath = @"..\2D_Inputs\HAWQS_data\COMID_HUC14_Unique.sqlite";

                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbpath))
                {
                    // Use the connection for database operations
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM COMID_to_HUC14 WHERE COMID = @Index", connection))
                    {
                        command.Parameters.AddWithValue("@Index", COMID); // assuming COMID is your input parameter

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // if there's a row
                            {
                                double bankfullWidth = reader.IsDBNull(reader.GetOrdinal("BANKFULL_WIDTH")) ? 0 : reader.GetDouble(reader.GetOrdinal("BANKFULL_WIDTH"));
                                double bankfullDepth = reader.IsDBNull(reader.GetOrdinal("BANKFULL_DEPTH")) ? 0 : reader.GetDouble(reader.GetOrdinal("BANKFULL_DEPTH"));
                                double lengthKm = reader.IsDBNull(reader.GetOrdinal("LENGTHKM")) ? -9998 : reader.GetDouble(reader.GetOrdinal("LENGTHKM"));
                                double slope = reader.IsDBNull(reader.GetOrdinal("SLOPE")) ? -9998 : reader.GetDouble(reader.GetOrdinal("SLOPE"));
                                double centroidLatitude = reader.IsDBNull(reader.GetOrdinal("CentroidLatitude")) ? -9998 : reader.GetDouble(reader.GetOrdinal("CentroidLatitude"));
                                double maxElevSMO = reader.IsDBNull(reader.GetOrdinal("MAXELEVSMO")) ? -9998 : reader.GetDouble(reader.GetOrdinal("MAXELEVSMO"));

                                Sim.AQTSeg.Location.Locale.SiteLength.Val = lengthKm > 0 ? lengthKm : 1;  //km
                                Sim.AQTSeg.Location.Locale.SiteLength.Comment = lengthKm > 0 ? "from NHD+ Catchment SQLite Database" : "default;no data in the WSCatchment web service";
                                lengthKm = Sim.AQTSeg.Location.Locale.SiteLength.Val;

                                Sim.AQTSeg.Location.Locale.Channel_Slope.Val = slope > 0 ? slope : 0.001;  //m/m
                                Sim.AQTSeg.Location.Locale.Channel_Slope.Comment = slope > 0 ? "from NHD+ Catchment SQLite Database" : "default; no data in the WSCatchment web service";

                                Sim.AQTSeg.Location.Locale.ICZMean.Val = bankfullDepth > 0 ? bankfullDepth : 1;  //m
                                Sim.AQTSeg.Location.Locale.ICZMean.Comment = bankfullDepth > 0 ? "from NHD+ Catchment SQLite Database" : "default; no data in the WSCatchment web service";

                                Sim.AQTSeg.Location.Locale.ZMax.Val = Sim.AQTSeg.Location.Locale.ICZMean.Val * 1.5;  //approximation for now
                                Sim.AQTSeg.Location.Locale.ZMax.Comment = "Estimated as 1.5 x BANKFULL_DEPTH";

                                Sim.AQTSeg.Location.Locale.Latitude.Val = centroidLatitude > 0 ? centroidLatitude : 40;  //m
                                Sim.AQTSeg.Location.Locale.Latitude.Comment = centroidLatitude > 0 ? "from NHD+ Catchment SQLite Database" : "default; no data in the WSCatchment web service";

                                Sim.AQTSeg.Location.Locale.SurfArea.Val = bankfullWidth > 0 ? bankfullWidth * lengthKm * 1000 : 5 * lengthKm * 1000;  //m/m
                                Sim.AQTSeg.Location.Locale.SurfArea.Comment = bankfullWidth > 0 ? "site length * bankfill width from NHD+ Catchment SQLite Database" : "Assumes width of 5 meters, no data in the WSCatchment web service";

                                Sim.AQTSeg.Location.Locale.Altitude.Val = maxElevSMO > 0 ? maxElevSMO / 100 : 300;  //altitude in m converted from cm
                                Sim.AQTSeg.Location.Locale.Altitude.Comment = maxElevSMO > 0 ? "Altitude from NHD+ Catchment SQLite Database" : "Default altitude of 300 meters, no data in the WSCatchment web service";

                                Sim.AQTSeg.Location.Locale.UseEnteredManning.Val = true;
                                Sim.AQTSeg.Location.Locale.EnteredManning.Val = 0.040; //natural channel default
                                Sim.AQTSeg.Location.Locale.EnteredManning.Comment = "natural channel default";
                            }
                        }
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<bool> ReadCOMIDGeometry(AQTSim Sim, string COMID)
        //Old code to read from NHD+ via EPA webservice. -- was slow, up to 12 seconds per query
        //Dictionary<string, string> ExtractMetadata(string json)
        //{
        //    var result = new Dictionary<string, string>();
        //    JObject jsonObject = JObject.Parse(json);
        //    var metadata = jsonObject["metadata"] as JObject;

        //    // List of parameters to extract
        //    string[] parameters = { "LengthKM",  "SLOPE", "BANKFULL_WIDTH", "BANKFULL_DEPTH", "CentroidLatitude" };

        //    // Loop through each parameter
        //    foreach (var param in parameters)
        //    {
        //        // Get the value if it exists, otherwise -9999
        //        var value = metadata[param]?.ToObject<object>() ?? -9999;
        //        result.Add(param, value.ToString());
        //    }

        //    return result;
        //}

        //string baseUrl = webServiceURLs.hmsRest+"info/catchment";
        //string parameters = "?comid="+COMID+"&streamcat=false&geometry=false&nwis=false&streamGeometry=false&cn=false&network=false";
        //string fullUrl = baseUrl + parameters;

        //Dictionary<string, string> GeoVals;

        //try

        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        var response = await client.GetAsync(fullUrl);
        //        var contentString = await response.Content.ReadAsStringAsync();
        //        string deserializedObject = JsonConvert.DeserializeObject<object>(contentString).ToString();
        //        GeoVals = ExtractMetadata(deserializedObject);
        //    }

        //    double sitelength = double.Parse(GeoVals["LengthKM"]);
        //    Sim.AQTSeg.Location.Locale.SiteLength.Val = sitelength;  //km
        //    Sim.AQTSeg.Location.Locale.SiteLength.Comment = "from WSCatchment web service";

        //    Sim.AQTSeg.Location.Locale.Channel_Slope.Val = double.Parse(GeoVals["SLOPE"]);  //m/m
        //    Sim.AQTSeg.Location.Locale.Channel_Slope.Comment = "from WSCatchment web service";

        //    Sim.AQTSeg.Location.Locale.ICZMean.Val = double.Parse(GeoVals["BANKFULL_DEPTH"]);  //m
        //    Sim.AQTSeg.Location.Locale.ICZMean.Comment = "BANKFULL_DEPTH from WSCatchment web service";
        //    Sim.AQTSeg.Location.Locale.ZMax.Val = Sim.AQTSeg.Location.Locale.ICZMean.Val * 1.5;  //approximation for now
        //    Sim.AQTSeg.Location.Locale.ZMax.Comment = "Estimated as 1.5 x BANKFULL_DEPTH";

        //    Sim.AQTSeg.Location.Locale.Latitude.Val = double.Parse(GeoVals["CentroidLatitude"]);
        //    Sim.AQTSeg.Location.Locale.Latitude.Comment = "from WSCatchment web service";

        //    Sim.AQTSeg.Location.Locale.SurfArea.Val = double.Parse(GeoVals["BANKFULL_WIDTH"])*sitelength*1000;  //m2
        //    Sim.AQTSeg.Location.Locale.SurfArea.Comment = "calculated from from WSCatchment web service";

        //    Sim.AQTSeg.Location.Locale.UseEnteredManning.Val = true;
        //    Sim.AQTSeg.Location.Locale.EnteredManning.Val = 0.040; //natural channel default
        //    Sim.AQTSeg.Location.Locale.EnteredManning.Comment = "natural channel default";
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("ReadCOMIDGeometry Error occurred: " + ex.Message);
        //    return false;
        //}
        //return true;
        //}

        /// <summary>
        /// Adds HAWQS daily outputs for overland flows and inflows (where relevant) to the simulation unit identified with SegID
        /// </summary>
        /// <param name="HRD">HAWQS Reach Data -- a nested dictionary of relevant inputs for each HAWQS subbasin</param>
        /// <param name="Boundaries">The list of upstream boundaries that have data relevant to the SegID segment unit</param>
        /// <param name="SegID">The identifying string for the unit-- can be COMID, or HUC8-HUC14 identifier</param>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="link_boundary">should boundary condition inflows from HAWQS be added to this segment-- i.e. is the up-river segment out of the AQUATOX domain?</param>
        /// <param name="link_overland">should overland flows be added to this segment</param>
        /// <returns>jsondata (the AQUATOX simulation JSON after HAWQS data has been linked), errors</returns>
public async Task<(string,string)> HAWQSRead(Dictionary<long, Dictionary<DateTime, HAWQSRCHRow>> HRD, List <string> Boundaries, string SegID, string setupjson, bool link_boundary, bool link_overland, bool is_COMID)
        {
            string jsondata = "";
            long ID = long.Parse(SegID);

            Dictionary<DateTime, HAWQSRCHRow> ThisSeg;
            if (HRD.ContainsKey(ID)) ThisSeg = HRD[ID];
            else return ("","ERROR: Segment " + SegID + " missing from HAWQS reach output");

            List<Dictionary<DateTime, HAWQSRCHRow>> BoundSegs = new();
            foreach (string bound in Boundaries)
            {
                long parsedBound = long.Parse(bound);
                if (HRD.ContainsKey(parsedBound)) BoundSegs.Add(HRD[long.Parse(bound)]);  // identify the relevant boundary condition inputs
                else return ("", "ERROR: boundary condition " + bound + " missing for Segment ID " + SegID);
            }

            string SegTypeStr;
            if (is_COMID) SegTypeStr = "COMID ";
            else SegTypeStr = "HUC" + SegID.Length.ToString();

            string err;
            AQTSim Sim = JSON_to_AQTSim(baseSimJSON, setupjson, SegTypeStr, SegID, -9999, out err);  //COMID length from ReadCOMIDGeometry
            if (err != "") { return ("", err); }

            if (is_COMID)
            {
                if (!await ReadCOMIDGeometry(Sim, SegID)) return ("", "ERROR: Cannot read COMID " + SegID + " geometry parameters from webserivce");
            }
            else if (!ReadHUCGeometry(Sim, SegID)) return ("", "ERROR: Cannot read HUC Geometry for " + SegID + " from local databases"); ;

            TVolume TVol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
            DateTime FirstDay = Sim.AQTSeg.PSetup.FirstDay.Val;
            VolFlowFromHAWQS(TVol, ThisSeg, FirstDay);  // read data for volume/flow model from HAWQS Linkage

            string[] components = { "SED", "NO3", "NH4", "MINP", "CHLA", "CBOD", "DISOX" };  
            AllVariables[] SVs = { AllVariables.TSS, AllVariables.Nitrate, AllVariables.Ammonia, AllVariables.Phosphate, AllVariables.NullStateVar, AllVariables.DissRefrDetr, AllVariables.Oxygen, AllVariables.Temperature };

            TPlant PPhyto = null; 
            for (AllVariables AlgLoop = Consts.FirstAlgae; AlgLoop <= Consts.LastAlgae; AlgLoop++)  //identify any phytoplankton in the simulation for CHLA linkage
            {
                TPlant TP = Sim.AQTSeg.GetStatePointer(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (TP != null)
                {
                    if (TP.IsPhytoplankton())  //Chl-a is assigned to first phytoplankton in the simulation 
                    {
                        PPhyto = TP;  
                        SVs[4] = TP.NState;
                        break;
                    }
                }
            }  

            double InitFlow = ThisSeg[FirstDay].vals[1];  //column for flow_OUT
            for (int i = 0; i < components.Length; i++)
            {
                bool isSED = (components[i] == "SED");  //sediment is handled differently in units and modeling
                bool isCBOD = (components[i] == "CBOD"); //unique input data structure in AQUATOX for CBOD
                bool isCHLA = (components[i] == "CHLA");  //Convert CHLA to biomass units

                int col_IN = Array.IndexOf(HAWQSInf.colnames, components[i] + "_IN");  //identify relevant columns in RCH_OUT file
                if (col_IN < 0) return ("", "ERROR: Missing column " + components[i] + "_IN from HAWQS RCH output.");

                int col_OUT = col_IN + 1;

                TStateVariable TSV = Sim.AQTSeg.GetStatePointer(SVs[i], T_SVType.StV, T_SVLayer.WaterCol);
                if (TSV != null) 
                {
                    TSV.InitialCond = (InitFlow <= 0) ? 0 : ThisSeg[FirstDay].vals[col_OUT] / InitFlow * 0.0115740;  // (kg/d) / (m3/s) * (mg/kg * d/s * m3/L)
                    if (isSED) TSV.InitialCond *= 1000;  // sed is in units of tons
                    if (isCHLA) TSV.InitialCond = TSV.InitialCond * PPhyto.PAlgalRec.Plant_to_Chla.Val / 1.90 ;
                    //             mg/L OM      =   mg/L chl-a            C to chl-a                    g OM / g OC

                    TLoadings InflowLoad = TSV.LoadsRec.Loadings;     // inflow loadings for boundary condition segments
                    TLoadings NPSLoad = TSV.LoadsRec.Alt_Loadings[2];  // Alt_Loadings[2] is non - point source loadings

                    if (isCBOD)
                    {
                        DetritalInputRecordType DIR = ((TDissRefrDetr)TSV).InputRecord;
                        DIR.InitCond = TSV.InitialCond;
                        DIR.DataType = DetrDataType.CBOD;
                        InflowLoad = DIR.Load.Loadings; 
                        NPSLoad = DIR.Load.Alt_Loadings[2];  //deal with different data structure for susp&dissolved detritus (CBOD)
                    }

                    bool overlandrelevant = (!isSED && !isCHLA);  //non-point source loads not relevant in AQUATOX for these state vars.

                    TSV.LoadNotes1 = "";
                    TSV.LoadNotes2 = "";
                    if (link_boundary || isSED) 
                    {
                        setupLoad(InflowLoad, ThisSeg.Count);  //get inflow loads ready to receive inputs
                        TSV.LoadNotes1 = "Inflow Loads from HAWQS Linkage (Daily RCH file)";
                        if (isSED) TSV.LoadNotes1 = "In-reach Sediment Values from HAWQS Linkage (Daily RCH file)";
                    }
                    

                    if (link_overland && overlandrelevant)
                    {
                        setupLoad(NPSLoad, ThisSeg.Count);  //get non-point source loads ready to receive inputs
                        TSV.LoadNotes2 = "Non-Point Source Loads from HAWQS Simulation";
                    }

                    foreach (KeyValuePair<DateTime, HAWQSRCHRow> pair in ThisSeg)  //for each date
                    {
                        double boundInput = 0;
                        double boundflow = 0;

                        foreach (Dictionary<DateTime, HAWQSRCHRow> BoundSeg in BoundSegs)
                        {
                            boundInput += BoundSeg[pair.Key].vals[col_OUT];  // kg/d
                            boundflow += BoundSeg[pair.Key].vals[1];  // flow out in m3/s
                        }

                        double inflowConc = 0;
                        if (boundflow > Consts.Tiny)
                             inflowConc = boundInput / boundflow * 0.0115740;  //  (kg/d) / (m3/s) * (mg/kg * d/s)
                        if (components[i] == "SED") inflowConc *= 1000;        // in HAWQS sed is in units of tons, other components in kg

                        if (link_boundary || isSED)
                        {
                            InflowLoad.list.Add(pair.Key, inflowConc);
                        }

                        if (link_overland && overlandrelevant)
                        {
                            double comp_IN = ThisSeg[pair.Key].vals[col_IN];      // kg/d
                            double overland_flow = Math.Max(0,(comp_IN - boundInput) * 1000);   // g/d = kg/d * 1000 g/kg -- Math.Max ensures no zeros due to rounding error
                            NPSLoad.list.Add(pair.Key, overland_flow);   // add daily NPS load in grams/d 
                        }
                    }
                }
            }

            // Water Temperature Linkage Here
            TStateVariable TTemp = Sim.AQTSeg.GetStatePointer( AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            int col_WTMP = Array.IndexOf(HAWQSInf.colnames, "WTMP");  //identify relevant columns in RCH_OUT file
            if ((TTemp != null) && (col_WTMP >= 0))
            {
                TTemp.InitialCond = ThisSeg[FirstDay].vals[col_WTMP];
                TTemp.LoadNotes1 = "Temperature from HAWQS RCH data";
                TTemp.LoadNotes2 = "";
                setupLoad(TTemp.LoadsRec.Loadings, ThisSeg.Count);
                foreach (KeyValuePair<DateTime, HAWQSRCHRow> pair in ThisSeg)  //for each date
                {
                    TTemp.LoadsRec.Loadings.list.Add(pair.Key, pair.Value.vals[col_WTMP]);  // add daily WT in deg C
                }
            }

            string errmessage = Sim.SaveJSON(ref jsondata);
            return (jsondata,errmessage);
        }

        /// <summary>
        /// Reads NWM Volumes and Flows and Saved JSON for WaterBody associated with WBCOMID
        /// </summary>
        /// <param name="WBComid">The waterbody COMID</param>
        /// <param name="setupjson">A JSON with the relevant setup parameters for the linked simulation</param>
        /// <param name="daily">Return daily rather than hourly results?</param>
        /// <param name="jsondata">Return of the JSON with the NWM volumes, flows, and simplified geometry parameters</param>
        /// <returns></returns>
        public string PopulateLakeRes(int WBComid, string setupjson, bool daily, out string jsondata)
        {
            jsondata = "";
            string WBCstr = WBComid.ToString();

            string err;
            AQTSim Sim = JSON_to_AQTSim(baseSimJSON, setupjson, "WBCOMID", WBCstr, -9999, out err);
            if (err != "") { return err; }

            HydrologyTSI TSI = new();
            TSI.DateTimeSpan.StartDate = Sim.AQTSeg.PSetup.FirstDay.Val;
            TSI.DateTimeSpan.EndDate = Sim.AQTSeg.PSetup.LastDay.Val;
            TSI.Geometry.ComID = WBComid;
            TSI.Geometry.GeometryMetadata["waterbody"] = "true";
            TSI.Source = "nwm";
            if (daily) TSI.TemporalResolution = "daily";

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
            AQTSim Sim = JSON_to_AQTSim(baseSimJSON, setupjson, "COMID", comid, lenkm, out err);
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
        /// <param name="passMass">if true passes mass, if false, passes the water concentration which can be done if water models may differ between segments e.g. reservoir to stream</param> 
        /// <param name="previous_flows">a list containing the sum of the previous flows linked to this segment for each time step.  Needed for weighted averaging.</param>
        /// <param name="divergence_flows">a list of any additional divergence flows from source segment (flows not to this segment), for the complete set of time-steps of the simulation in m3/s</param> 
        /// <returns>errors, warnings</returns>
        public string Pass_Data(AQTSim Sim, int SrcID, int ninputs, bool passMass, ref SortedList<DateTime, double> previous_flows, archived_results AR = null, List <ITimeSeriesOutput<List<double>>> divergence_flows = null)  
        {

            //archived_results AR;
            if (AR == null)    
                { 
                archive.TryGetValue(SrcID, out AR);
                if (AR == null)
                    {  // check to see if upstream segment is null because it is actually a lake/reservoir
                    if (SN.waterbodies.comid_wb.ContainsKey(SrcID))
                        {
                        SN.waterbodies.comid_wb.TryGetValue(SrcID, out SrcID);  // translate SrcID to the relevant WBCOMID
                        archive.TryGetValue(SrcID, out AR);
                        }
                if (AR == null) return "ERROR: Segment "+Sim.AQTSeg.StudyName+" is missing expected linkage data from "+SrcID;
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

                    TLoadings flowLoad;
                    if (tvol.Calc_Method == VolumeMethType.Manning) flowLoad = tvol.LoadsRec.Alt_Loadings[1];  //manning's input is "discharge load" or [1]
                    else flowLoad = tvol.LoadsRec.Alt_Loadings[0];  //[0] is inflow

                    SortedList<DateTime, double> newlist = new SortedList<DateTime, double>();
                    if (ninputs == 1) previous_flows = new SortedList<DateTime, double>();  // first or only loading, start saving flows for weighted averaging with potential future loadings

                    for (int i = 0; i < ndates; i++)
                    {
                        double OutVol = AR.washout[i];  // out volume to this segment from upstream segment

                        double frac_this_segment = 1.0;
                        double totOutVol = OutVol;
                        if (divergence_flows != null)
                            foreach (ITimeSeriesOutput<List<double>> its in divergence_flows)
                            {
                                totOutVol = totOutVol + Convert.ToDouble(its.Data.Values.ElementAt(i)[0]) * 86400;     //fixme potential issue if master setup time-step changes or simulation time-period is increased since NWM data gathering
                                                                                                                       // m3/d       m3/d                                        m3/s               s/d
                                frac_this_segment = totOutVol > 0 ? OutVol / totOutVol : 1;
                            }

                        double InVol = flowLoad.ReturnLoad(AR.dates[i]);  // inflow volume to current segment,   If velocity is not used, must be estimated as current seg. outflow 

                        if (passMass)
                        {
                            if (InVol < Consts.Tiny) newlist.Add(AR.dates[i], 0);
                            else if (ninputs == 1) newlist.Add(AR.dates[i], AR.concs[iTSV][i] * (OutVol / InVol) * frac_this_segment);  // first or only input
                            else newlist.Add(AR.dates[i], TSV.LoadsRec.Loadings.list.Values[i] + AR.concs[iTSV][i] * (OutVol / InVol) * frac_this_segment);  //adding second or third inputs
                        }
                        else // pass concentration
                        {
                            if (ninputs == 1)
                            {
                                newlist.Add(AR.dates[i], AR.concs[iTSV][i]);  // first or only input
                                previous_flows.Add(AR.dates[i], OutVol * frac_this_segment);  //water flowing into this segment at this time step corrected for divergences
                            }
                            else
                            {
                                double otherSegFlows = previous_flows.Values[i];  //sum of other-segment flows into this segment in this time step
                                double thisSegFlows = OutVol * frac_this_segment; //flows into this segment from passage segment this time step
                                double totFlows = otherSegFlows + thisSegFlows;   //total flows into this segment this time step
                                double weightavg = totFlows > 0 ? (TSV.LoadsRec.Loadings.list.Values[i] * otherSegFlows + AR.concs[iTSV][i] * thisSegFlows) / totFlows : 0;
                                newlist.Add(AR.dates[i], weightavg);  //weighted-averaging second or third inputs by volume of water.  
                            }
                        }
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
        /// <param name="segID">integer ID (comid, HUC, WBcomid) of segment in network</param>
        /// <param name="setupjson">string holding the master setup record</param>
        /// <param name="outstr">information about the status of the run for the user's log</param>
        /// <param name="jsonstring">the completed simulation with results </param>
        /// <param name="divergence_flows">a list of any additional divergence flows from source segment (flows not to this segment), for the complete set of time-steps of the simulation in m3/s</param> 
        /// <param name="outofnetwork">array of COMIDs that are out of the network water sources.</param>  
        /// <returns>boolean: true if the run was completed successfully</returns>/// 
        public bool executeModel(long segID, string setupjson, ref List<string> outstr, ref string jsonstring, List<ITimeSeriesOutput<List<double>>> divergence_flows = null, int[] outofnetwork = null)         
        {
            AQTSim Sim = Instantiate_with_setup(setupjson, ref outstr, jsonstring);
            Sim.AQTSeg.ProgHandle = this.ProgHandle;
            Sim.AQTSeg._ct = this.CancelToken;

            if (SVList == null)
            {
                SVList = new List<string>();
                SVList.Clear();  //workaround, fixes strange behavior before this was added
                foreach (TStateVariable SV in Sim.AQTSeg.SV)
                {
                    SVList.Add(SV.PName+" ("+SV.StateUnit+")");   //save list of SVs for output
                }
            }

            SortedList<DateTime, double> previous_flows = null;
            int nSources = 0;
            if (SN != null)
            {
                if (SN.sources != null)
                 if (SN.sources.TryGetValue(segID.ToString(), out int[] Sources))
                    foreach (int SrcID in Sources)
                    {
                        if ((SrcID != segID) && !outofnetwork.Contains(SrcID))  // don't pass data from out of network segments
                        {
                            nSources++;
                            string errstr = Pass_Data(Sim, SrcID, nSources, false, ref previous_flows, null, divergence_flows);
                    if (errstr != "") outstr.Add(errstr);
                                else outstr.Add("INFO: Passed data from Source " + SrcID.ToString() + " into COMID " + segID.ToString());
                }
                    };
            
                if ((SN.waterbodies != null) && (SN.sources != null))
                {   // pass data into Waterbodies from adjacent stream segments
                    if (SN.waterbodies.comid_wb.ContainsValue((int)segID))  // if the comid is a waterbody
                      foreach (KeyValuePair<int, int> entry in SN.waterbodies.comid_wb)  
                        if (entry.Value == segID)  // for each stream segment in waterbody
                          if (SN.sources.TryGetValue(entry.Key.ToString(), out int[] Sources)) //get the sources for the stream segment
                            foreach (int SrcID in Sources) //loop through the sources
                              if ((SrcID != entry.Key) && !outofnetwork.Contains(SrcID))  // don't pass data from out of network segments
                                {
                                   archive.TryGetValue(SrcID, out archived_results AR);  // don't pass data from segments not run (e.g. internal segments in the waterbody that are irrelevant)
                                    if (AR != null)
                                    {
                                        nSources++;
                                        string errstr = Pass_Data(Sim, SrcID, nSources, false, ref previous_flows, null, divergence_flows);
                                        if (errstr != "") outstr.Add(errstr);
                                            else outstr.Add("INFO: Passed data from Source " + entry.Key.ToString() + " into WBCOMID " + segID.ToString());
                                    }
                              }
                }

            }


            Sim.AQTSeg.RunID = "Run: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string errmessage = Sim.Integrate();
            if (errmessage == "")
            {
                archiveOutput((int)segID, Sim);
                errmessage = Sim.SaveJSON(ref jsonstring);

                if (errmessage != "")
                {
                    outstr.Add("ERROR running segment " + segID + ": " + errmessage);
                    return false;
                }
            }
            else {
                outstr.Add("ERROR running segment " + segID + ": " + errmessage);
                return false;
                 };

            outstr.Add("INFO: " + "--> Executed Segment " + segID.ToString());
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

    public class HAWQSRCHRow
    {
        public double lat, lon;
        public double[] vals;
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


