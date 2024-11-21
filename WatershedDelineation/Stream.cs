using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace WatershedDelineation
{
    public class Shape
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }
    }

    public class NtNavResultsStandard
    {
        public string feature_id { get; set; }
        public string permanent_identifier { get; set; }
        public int comid { get; set; }
        public string reachcode { get; set; }
        public DateTime reachsmdate { get; set; }
        public double fmeasure { get; set; }
        public double tmeasure { get; set; }
        public double totaldist { get; set; }
        public object totaltime { get; set; }
        public int hydroseq { get; set; }
        public int levelpathid { get; set; }
        public int terminalpathid { get; set; }
        public int uphydroseq { get; set; }
        public int dnhydroseq { get; set; }
        public double pathlength { get; set; }
        public double lengthkm { get; set; }
        public object pathtime { get; set; }
        public object travtime { get; set; }
        public string nhdplus_region { get; set; }
        public string nhdplus_version { get; set; }
        public int ftype { get; set; }
        public int fcode { get; set; }
        public string gnis_id { get; set; }
        public string wbarea_permanent_identifier { get; set; }
        public int? wbareacomi { get; set; }
        public string wbd_huc12 { get; set; }
        public int catchment_featureid { get; set; }
        public Shape shape { get; set; }
    }

    public class Output
    {
        public int return_code { get; set; }
        public object status_message { get; set; }
        public string start_permanent_identifier { get; set; }
        public int start_comid { get; set; }
        public double total_distance_km { get; set; }
        public object total_flowtime_day { get; set; }
        public List<NtNavResultsStandard> ntNavResultsStandard { get; set; }
    }

    public class Status
    {
        public object submission_id { get; set; }
        public int status_code { get; set; }
        public object status_message { get; set; }
        public double execution_time { get; set; }
        public int output_bytes { get; set; }
    }

    public class RootObject
    {
        public Output output { get; set; }
        public Status status { get; set; }
    }

    public class StreamSegment
    {
        public string comID { get; set; }
        public double length_km { get; set; }
        public Dictionary<string, List<double>> velocities_mPerSec { get; set; }  //(key, value) 	Key= timestep, value=velocity
        public Dictionary<string, List<double>> flows_m3PerSec { get; set; }      //(key, value) 	Key= timestep, value=flow
        public Dictionary<string, bool> contaminated { get; set; }	//(key, value) 	Key= timestep, value=contaminated?	
        public Dictionary<string, List<string>> timestepData { get; set; }

        public StreamSegment()
        {
            this.comID = "";
            this.length_km = 0;
            this.velocities_mPerSec = null;
            this.flows_m3PerSec = null;
            this.contaminated = null;
            this.timestepData = null;
        }
    }

    public class Streams
    {
        string startCOMID { get; set; }

        string stopCOMID { get; set; }

        LinkedList<StreamSegment> StreamSegments { get; set; }
                
        public Streams(string start, string end = null, LinkedList<StreamSegment> segs = null)
        {
            this.startCOMID = start;
            this.stopCOMID = end;
            this.StreamSegments = segs;
        }

        public object GetNetwork(double maxDistance=50.0, string endComid=null, bool mainstem=false)
        {
            string errorMsg = "";
            string data = GetStreamNetwork(out errorMsg, endComid, maxDistance, mainstem);
            object networkObject = System.Text.Json.JsonSerializer.Deserialize<object>(data);
            return networkObject;
        }

        public LinkedList<StreamSegment> GetStreams(ITimeSeriesInput input, List<List<object>> contaminantInflow, string inflowSource, out string comids)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            //Create a LinkedList of Streams
            LinkedList<StreamSegment> travelPath = new LinkedList<StreamSegment>();
            //Call EPA Waters web service to get path of travel
            //For example: Using the user specified (from map) start and stop COMIDs
            //The call returns JSON with COMIDs in the path
            string data = GetWatersData(out errorMsg, input.Geometry.GeometryMetadata["startCOMID"], input.Geometry.GeometryMetadata["endCOMID"]);

            RootObject networkObject = Utilities.JSON.Deserialize<RootObject>(data);

            comids = "";
            foreach(NtNavResultsStandard stream in networkObject.output.ntNavResultsStandard)
            {
                comids += stream.comid.ToString() + ",";
            }
            string flaskURL = (Environment.GetEnvironmentVariable("FLASK_SERVER") != null) ? Environment.GetEnvironmentVariable("FLASK_SERVER") : "http://localhost:7777";
            string nwmUrl = flaskURL + "/hms/nwm/forecast/short_term?comid=" + comids;
            nwmUrl = nwmUrl.TrimEnd(',');
            //string nwmUrl = flaskURL + "/hms/nwm/data/?dataset=streamflow&comid=" + seg.comID + "&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd");
            WatershedDelineation.NWM nwm = new WatershedDelineation.NWM();
            string nwmdata = nwm.GetData(out errorMsg, nwmUrl);
            NWMObject nwmObjectData = Utilities.JSON.Deserialize<NWMObject>(nwmdata);

            Dictionary<string, int> inflowDict = new Dictionary<string, int>();
            //Iterate over input table to format as dictionary
            //TODO: Can output object from Front end be formatted as a dictionary?
            if (inflowSource.Equals("Input Table"))
            {
                foreach (List<object> vals in contaminantInflow)
                {
                    string[] formats = { "yyyy-MM-dd HH", "yyyy-M-d H" };
                    string dt = vals[0].ToString() + " " + vals[1].ToString();
                    DateTime x = DateTime.ParseExact(dt, formats, CultureInfo.InvariantCulture);
                    inflowDict.Add(x.ToString("yyyy-MM-dd HH"), Int32.Parse(vals[2].ToString()));
                }
            }

            //Iterate through the COMIDs and stream lengths in the returned JSON and add them to the LinkedList
            foreach (NtNavResultsStandard stream in networkObject.output.ntNavResultsStandard)
            {
                StreamSegment seg = new StreamSegment(); //Instantiate Stream object from Stream Class
                seg.comID = stream.comid.ToString();
                seg.length_km = stream.lengthkm;
                //Use NWM forecast data for the stream to fill flow values
                string dat = nwmObjectData.data[seg.comID];
                NWMData parsedNwmData = Utilities.JSON.Deserialize<NWMData>(dat);

                seg.flows_m3PerSec = parsedNwmData.data;
                seg.velocities_mPerSec = parsedNwmData.data;
                //Initialize stream. Contaminated with 'false' values
                seg.contaminated = new Dictionary<string, bool>();
                seg.timestepData = new Dictionary<string, List<string>>();
                travelPath.AddLast(seg);
                Thread.Sleep(1000);
            }
            
            //Calculate which streams got contaminated at each time step
            //TODO: Algorithm does not incorporate flow values into contamination calculation
            for (int timeStep = 0; timeStep < 18; timeStep++)
            {
                string timeindex = travelPath.First.Value.velocities_mPerSec.Keys.ElementAt(timeStep);
                double totalTravelTime = 0;
                double traveltimetest = 0;
                for (LinkedListNode<StreamSegment> stream = travelPath.First; stream != null; stream = stream.Next)
                {
                    if (inflowSource.Equals("Input Table"))
                    {
                        stream.Value.flows_m3PerSec[timeindex][0] = inflowDict[timeindex];
                    }
                    totalTravelTime += ((stream.Value.length_km * 1000) / stream.Value.velocities_mPerSec[timeindex.ToString()][1]) / 3600;
                    if (totalTravelTime >= timeStep)
                    {
                        stream.Value.contaminated[timeindex.ToString()] = false;
                    }
                    else
                    {
                        stream.Value.contaminated[timeindex.ToString()] = true;
                    }                    
                    List<string> combined = new List<string> { stream.Value.comID, stream.Value.length_km.ToString(), stream.Value.velocities_mPerSec[timeindex][1].ToString(), stream.Value.flows_m3PerSec[timeindex][0].ToString(), stream.Value.contaminated[timeindex].ToString() };
                    stream.Value.timestepData.Add(timeindex, combined);
                }
            }
            return travelPath;
        }

        public string GetWatersData(out string errorMsg, string start, string stop)
        {
            errorMsg = "";
            string requestURL = "https://ofmpub.epa.gov/waters10/Navigation.Service?pNavigationType=PP&pStartComID=" + start + "&pStopCOMID=" + stop;
            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(requestURL);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download requested epa waters data. " + ex.Message;
            }
            return data;
        }

        private string GetStreamNetwork(out string errorMsg, string endComid=null, double maxDistance=50.0, bool mainstem=false)
        {
            errorMsg = "";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string requestURL = "https://api.epa.gov/waters/v2/navigation3?p_indexing_engine=DISTANCE&p_limit_innetwork=FALSE&p_limit_navigable=TRUE&p_fallback_limit_innetwork=FALSE&p_fallback_limit_navigable=TRUE&p_return_link_path=FALSE&p_use_simplified_catchments=TRUE&p_known_region=point";
            string apiKey = Environment.GetEnvironmentVariable("WATERS_APIKEY");
            if (mainstem && !string.IsNullOrEmpty(endComid))
            {
                // Downstream mainstem traversal, start and stop comids are switched. 
                //string query = "SELECT PB.ComID FROM PlusFlowlineVAA as PA join PlusFlowlineVAA as PB on PA.DnHydroseq=PB.Hydroseq AND PA.ComID=" + this.startCOMID;
                //Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                string comid = this.startCOMID;
                //if (sourceComid.ContainsKey("ComID"))
                //{
                //    comid = sourceComid["ComID"];
                //}
                // requestURL = "https://ofmpub.epa.gov/waters10/Navigation.Service?pNavigationType=DM&pStopComID=" + comid + "&pStartComID=" + endComid + "&pMaxDistanceKm=" + maxDistance.ToString();
                requestURL += "&p_search_type=DM&p_start_nhdplusid=" + endComid; 
            }
            else if (mainstem)
            {
                // Upstream mainstem traversal, stopping condition is maxDistance
                requestURL += "&p_search_type=UM&p_start_nhdplusid=" + this.startCOMID;
            }
            else if (!string.IsNullOrEmpty(endComid))
            {
                // Point to point traversal
                requestURL += "&p_search_type=PP&p_start_nhdplusid=" + this.startCOMID + "&p_stop_nhdplusid=" + endComid;
            }
            else
            {
                // Upstream with tributary traversal, stopping condition is maxDistance
                requestURL += "&p_search_type=UT&p_start_nhdplusid=" + this.startCOMID;
            }
            requestURL += "&p_search_max_distancekm=" + maxDistance.ToString();
            requestURL += "&p_network_resolution=MR&f=json&api_key=" + apiKey;

            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(requestURL);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download requested stream network data. " + ex.Message;
            }
            return data;
        }
    }
}
