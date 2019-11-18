using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
        public Dictionary<string, double> velocities_mPerSec { get; set; }  //(key, value) 	Key= timestep, value=velocity
        public Dictionary<string, double> flows_m3PerSec { get; set; }      //(key, value) 	Key= timestep, value=flow
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
                
        public Streams(string start, string end, LinkedList<StreamSegment> segs)
        {
            this.startCOMID = start;
            this.stopCOMID = end;
            this.StreamSegments = segs;
        }

        public LinkedList<StreamSegment> GetStreams(string startDate, string endDate)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            //Get data from Contaminant Loader
            /*
            ContaminantLoader.ContaminantLoader cl = new ContaminantLoader.ContaminantLoader("generic", "csv", "Date-Time, TestValues\n2010-01-01 00, 1.0\n2010-01-01 01, 1.5\n2010-01-01 02, 2.0\n2010-01-01 03, 2.5\n2010-01-01 04, 3.0\n2010-01-01 05, 3.5\n2010-01-01 06, 4.0\n2010-01-01 07, 4.5\n2010-01-01 08, 5.0\n2010-01-01 09, 5.5\n2010-01-01 10, 6.0\n2010-01-01 11, 6.5\n2010-01-01 12, 6.0\n2010-01-01 13, 5.5\n2010-01-01 14, 5.0\n2010-01-01 15, 4.5\n2010-01-01 16, 4.0\n2010-01-01 17, 3.5\n2010-01-01 18, 3.0\n2010-01-01 19, 2.5\n2010-01-01 20, 2.0\n2010-01-01 21, 1.5\n2010-01-01 22, 1.0\n2010-01-01 23, 1.0");
            foreach(KeyValuePair<string, List<string>> contamination in cl.Result.Data)
            {
                Console.WriteLine(contamination.ToString());
            }
            ITimeSeriesOutput test = cl.Result;*/
            //Time = Distance / Velocity



            //Create a LinkedList of Streams
            LinkedList<StreamSegment> travelPath = new LinkedList<StreamSegment>();
            //Call EPA Waters web service to get path of travel
            //For example: Using the user specified (from map) start and stop COMIDs
            //The call returns JSON with COMIDs in the path
            string data = GetWatersData(out errorMsg);

            RootObject networkObject = Utilities.JSON.Deserialize<RootObject>(data);

            //Iterate through the COMIDs and stream lengths in the returned JSON and add them to the LinkedList
            foreach (NtNavResultsStandard stream in networkObject.output.ntNavResultsStandard)
            {
                StreamSegment seg = new StreamSegment(); //Instantiate Stream object from Stream Class
                seg.comID = stream.comid.ToString();
                seg.length_km = stream.lengthkm;
                //Use NWM forecast data for the stream to fill flow values
                WatershedDelineation.NWM nwm = new WatershedDelineation.NWM();
                string flaskURL = (Environment.GetEnvironmentVariable("FLASK_SERVER") != null) ? Environment.GetEnvironmentVariable("FLASK_SERVER") : "http://localhost:7777";
                string nwmUrl = flaskURL + "/hms/nwm/data/?dataset=streamflow&comid=" + seg.comID + "&startDate=" + startDate + "&endDate=" + endDate;
                string nwmdata = nwm.GetData(out errorMsg, nwmUrl);
                NWMObject flowData = Utilities.JSON.Deserialize<NWMObject>(nwmdata);
                NWMData parsedFlowData = Utilities.JSON.Deserialize<NWMData>(flowData.data);
                seg.flows_m3PerSec = parsedFlowData.data;

                //Use NWM forecast data for the stream to fill velocity values
                nwmUrl = flaskURL + "/hms/nwm/data/?dataset=velocity&comid=" + seg.comID + "&startDate=" + startDate + "&endDate=" + endDate;
                nwmdata = nwm.GetData(out errorMsg, nwmUrl);
                NWMObject velocityData = Utilities.JSON.Deserialize<NWMObject>(nwmdata);
                NWMData parsedVelocityData = Utilities.JSON.Deserialize<NWMData>(velocityData.data);
                seg.velocities_mPerSec = parsedVelocityData.data;
                //Initialize stream. Contaminated with false values
                seg.contaminated = new Dictionary<string, bool>();
                seg.timestepData = new Dictionary<string, List<string>>();
                /*foreach (string date in seg.velocities_mPerSec.Keys)
                {
                    List<string> combined = new List<string>{ seg.comID, seg.length_km.ToString(), seg.flows_m3PerSec[date].ToString(), seg.velocities_mPerSec[date].ToString(), "false"};
                    seg.contaminated.Add(date, false);
                    seg.timestepData.Add(date, combined);
                }*/
                travelPath.AddLast(seg);
            }

            //Calculate which streams got contaminated at each time step
            for (int timeStep = 0; timeStep <= 18; timeStep++)//Change timestep to be datetime for indexing  2019-11-02T04:00:00
            {
                string timeindex = startDate + "T" + timeStep.ToString("00") + ":00:00";
                double totalTravelTime = 0;
                for (LinkedListNode<StreamSegment> stream = travelPath.First; stream != null; stream = stream.Next)
                {                    
                    totalTravelTime += stream.Value.velocities_mPerSec[timeindex.ToString()] / stream.Value.length_km;
                    if (totalTravelTime >= timeStep)
                    {
                        stream.Value.contaminated[timeindex.ToString()] = false;
                    }
                    else
                    {
                        stream.Value.contaminated[timeindex.ToString()] = true;
                    }
                    //stream.Value.contaminated[timeindex.ToString()] = true;
                    List<string> combined = new List<string> { stream.Value.comID, stream.Value.length_km.ToString(), stream.Value.flows_m3PerSec[timeindex].ToString(), stream.Value.velocities_mPerSec[timeindex].ToString(), stream.Value.contaminated[timeindex].ToString() };
                    stream.Value.timestepData.Add(timeindex, combined);
                }
            }
            return travelPath;
        }

        public string GetWatersData(out string errorMsg)
        {
            errorMsg = "";
            string requestURL = "https://ofmpub.epa.gov/waters10/Navigation.Service?pNavigationType=PP&pStartComID=22338753&pStopCOMID=22340577";
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
                errorMsg = "ERROR: Unable to download requested nldas data. " + ex.Message;
            }
            return data;
        }
    }
}
