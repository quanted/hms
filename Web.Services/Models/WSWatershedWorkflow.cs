using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Utilities;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Serivce WorkFlow Model
    /// </summary>
    public class WSWatershedWorkFlow
    {
        private enum surfaceSources { nldas, gldas, nwm, curvenumber }
        private enum subSources { nldas, gldas }
        private enum precipSources { nldas, gldas, ncei, daymet, wgen, prism, nwm }
        private enum algorithms { constantvolume }//, changingvolume, kinematicwave }

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWorkFlowData(WatershedWorkflowInput input)
        {
            DateTime start = input.DateTimeSpan.StartDate;
            DateTime end = input.DateTimeSpan.EndDate;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate all sources.
            //errorMsg = (!Enum.TryParse(input.StreamHydrology, true, out algorithms sAlgos)) ? "ERROR: Algorithm is not currently supported." : "";
            //if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.RunoffSource, true, out surfaceSources sSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.Geometry.GeometryMetadata["precipSource"], true, out precipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SET Attributes to specific values until stack works
            input.TemporalResolution = "daily";
            if(input.RunoffSource == "nldas" || input.RunoffSource == "gldas")
            {
                input.Geometry.GeometryMetadata["precipSource"] = input.RunoffSource;
            }

            //Stream Network Delineation
            List<string> lst = new List<string>();
            WatershedDelineation.StreamNetwork sn = new WatershedDelineation.StreamNetwork();
            DataTable dt = new DataTable();
            if (input.Geometry.ComID > 0)
            {
                dt = sn.prepareStreamNetworkForHUC(input.Geometry.ComID.ToString(), "com_id_num", out errorMsg, out lst);
            }
            else if (input.Geometry.HucID != null)
            {
                string len = input.Geometry.HucID.Length.ToString();
                dt = sn.prepareStreamNetworkForHUC(input.Geometry.HucID.ToString(), "huc_" + len + "_num", out errorMsg, out lst);
            }
            else
            {
                errorMsg = "ERROR: No valid geometry was found";
            }
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            //NCEI station attributes
            if (input.Geometry.GeometryMetadata["precipSource"] == "ncei")
            {                
                input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                input.Geometry.GeometryMetadata["stationID"] = nceiStation(out errorMsg, input);

                
            }

            //Getting Stream Flow data
            input.Source = input.RunoffSource;
            List<string> validList = new List<string>();
            DataSet ds = WatershedDelineation.FlowRouting.calculateStreamFlows(start.ToShortDateString(), end.ToShortDateString(), dt, lst, out validList, input, input.StreamHydrology, out errorMsg);
            lst = validList;

            Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
            Utilities.GeometryData gd = null;
            if (input.Aggregation)
            {
                gd = cd.getData(input, lst, out errorMsg);
            }
            
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            Web.Services.Controllers.WatershedWorkflowOutput totalOutput = new Web.Services.Controllers.WatershedWorkflowOutput();
            totalOutput.data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>();
            int x = 1;
            Dictionary<string, string> meta = new Dictionary<string, string>();
            foreach (string com in lst)
            {
                //Setting all to ITimeSeriesOutput
                input.Source = input.Geometry.GeometryMetadata["precipSource"];
                ITimeSeriesOutput precipOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[3], x), gd, input.Aggregation);//cd.getCatchmentAggregation(input, precipResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput surfOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[0], x), gd, input.Aggregation); //dtToITSOutput(ds.Tables[0]); //cd.getCatchmentAggregation(input, surfResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput subOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[1], x), gd, input.Aggregation);//dtToITSOutput(ds.Tables[1]);//cd.getCatchmentAggregation(input, subResult, gd, input.Aggregation);
                input.Source = input.StreamHydrology;
                ITimeSeriesOutput hydrologyOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2], x), gd, input.Aggregation);// dtToITSOutput(ds.Tables[2]);//cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2]), gd, input.Aggregation);

                Dictionary<string, ITimeSeriesOutput> timeSeriesDict = new Dictionary<string, ITimeSeriesOutput>();
                timeSeriesDict.Add("Precipitation", precipOutput);
                timeSeriesDict.Add("SurfaceRunoff", surfOutput);
                timeSeriesDict.Add("SubsurfaceRunoff", subOutput);
                timeSeriesDict.Add("StreamHydrology", hydrologyOutput);
                totalOutput.data.Add(Int32.Parse(com), timeSeriesDict);
                meta = precipOutput.Metadata;
                x++;
            }

            //Turn delineation table to ITimeseries
            totalOutput.table = new Dictionary<string, Dictionary<string, string>>();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                //List<string> lv = new List<string>();
                Dictionary<string, string> lv = new Dictionary<string, string>();
                int j = 0;
                string com = dr["COMID"].ToString();
                foreach (Object g in dr.ItemArray)
                {
                    lv.Add(dt.Columns[j++].ToString(), g.ToString());
                }
                if (totalOutput.table.ContainsKey(com))
                {
                    continue;
                }
                else
                {
                    totalOutput.table.Add(com, lv);
                }
            }

            //Adding delineation data to output
            totalOutput.Metadata = meta;
            totalOutput.metadata = new Dictionary<string, string>()
            {
                { "request_url", "api/workflow/watershed" },
                { "start_date", input.DateTimeSpan.StartDate.ToString() },
                { "end_date", input.DateTimeSpan.EndDate.ToString() },
                { "timestep", input.TemporalResolution.ToString() },
                { "errors", errorMsg },
                { "catchments", cd.listToString(lst, out errorMsg) },
                { "connectivity_table_source", "PlusFlowlineVAA" },
                { "NHDPlus_url" , "http://www.horizon-systems.com/nhdplus/NHDPlusV2_data.php"}
            };
            totalOutput.Dataset = "Precipitation, SurfaceRunoff, SubsurfaceRunoff, StreamHydrology";
            totalOutput.DataSource = input.Geometry.GeometryMetadata["precipSource"].ToString() + ", " + input.RunoffSource.ToString() + ", " + input.RunoffSource.ToString() + ", " + input.StreamHydrology.ToString();
            watch.Stop();
            string elapsed = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes.ToString();
            totalOutput.metadata.Add("Time_elapsed", elapsed);
            return totalOutput;
        }

        public ITimeSeriesOutput dtToITSOutput(DataTable dt, int x)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput itimeoutput = oFactory.Initialize();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> lv = new List<string>();
                lv.Add(dr[x].ToString());
                itimeoutput.Data.Add(dr[0].ToString(), lv);
            }

            itimeoutput.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Stream Flow" },
                { "units", "cubic meters" }
            };
            itimeoutput.Dataset = "Stream Flow";
            itimeoutput.DataSource = "curvenumber";
            return itimeoutput;
        }


        private string nceiStation(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            string dbPath = "./App_Data/catchments.sqlite";
            string query = "";
            //NCEI station is required. If only a ComID is provided, use catchment centroid coordinates to find nearest NCEI station.
            if (input.Geometry.HucID != null)
            {
                //NEED TO CHANGE THESE QUERIES TO ONLY ADD ONE POINT
                if(input.Geometry.HucID.Length == 8)
                {
                    query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID in (SELECT COMID From HUC12_PU_COMIDs_CONUS Where HUC12 LIKE '" + input.Geometry.HucID.ToString() + "%') AND CentroidLatitude IS NOT NULL AND CentroidLongitude IS NOT NULL";
                }
                else if (input.Geometry.HucID.Length == 12)
                {
                    query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID in (SELECT COMID From HUC12_PU_COMIDs_CONUS Where HUC12='" + input.Geometry.HucID.ToString() + "') AND CentroidLatitude IS NOT NULL AND CentroidLongitude IS NOT NULL";
                }
            }            
            if (input.Geometry.ComID > 0)
            {
                query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + input.Geometry.ComID.ToString();        
            }

            Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
            if (centroidDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database.";
                return null;
            }

            IPointCoordinate centroid = new PointCoordinate()
            {
                Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                Longitude = double.Parse(centroidDict["CentroidLongitude"])
            };

            string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            if (flaskURL == null)
            {
                flaskURL = "http://localhost:7777";
            }
            Debug.WriteLine("Flask Server URL: " + flaskURL);

            string nceiBaseURL = flaskURL + "/hms/gis/ncdc/stations/?latitude=" + centroid.Latitude.ToString() + "&longitude=" + centroid.Longitude.ToString() + "&geometry=point&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326";//"&comid=" + input.Geometry.ComID.ToString() +
                                                                                                                                                                                                                                                                                                                                    //string nceiBaseURL = flaskURL + "/hms/gis/ncdc/stations/?startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326&comid=" + input.Geometry.ComID.ToString();

            //Using FLASK NCDC webservice            
            string data = DownloadData(out errorMsg, nceiBaseURL);
            Result result = JSON.Deserialize<Result>(data);
            //Set NCEI station to closest station regardless of type
            input.Geometry.StationID = result.data[0].id.ToString();
            foreach (ResultData details in result.data)//Opt for closest GHCND station, if any
            {
                if (details.id.Contains("GHCND"))
                {
                    input.Geometry.StationID = details.id.ToString();
                    break;
                }
            }
            return input.Geometry.StationID;
        }

        /// <summary>
        /// Pulls NCEI station details from flask webservice.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            if (flaskURL == null)
            {
                flaskURL = "http://localhost:7777";
            }
            Debug.WriteLine("Flask Server URL: " + flaskURL);

            string dataURL = flaskURL + "/hms/data?job_id=";
            WebClient myWC = new WebClient();
            string data = "";
            dynamic taskData = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string jobID = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    jobID = JSON.Deserialize<Dictionary<string, string>>(reader.ReadToEnd())["job_id"];
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }

                retries = 50;
                status = "";
                taskData = "";
                bool success = false;
                while (retries > 0 && !success && !jobID.Equals(""))
                {
                    Thread.Sleep(6000);
                    WebRequest wr = WebRequest.Create(dataURL + jobID);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    taskData = JSON.Deserialize<dynamic>(data);
                    if (taskData["status"] == "SUCCESS")
                    {
                        success = true;
                    }
                    else if (taskData["status"] == "FAILURE" || taskData["status"] == "PENDING")
                    {
                        break;
                    }
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Could not find NCEI stations for the given geometry." + ex.Message;
            }
            return data;
        }
    }
}