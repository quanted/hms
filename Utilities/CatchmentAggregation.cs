using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Utilities
{
    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryData
    {
        /// <summary>
        /// Geometry component of HMS-GIS response
        /// </summary>
        public Dictionary<string, Catchment> geometry;

        /// <summary>
        /// Metadata component of HMS-GIS response
        /// </summary>
        public Dictionary<string, string> metadata;
    }

    /// <summary>
    /// Catchments
    /// </summary>
    public class Catchment
    {
        /// <summary>
        /// List of points in the Catchment
        /// </summary>
        public List<Point> points;
    }

    /// <summary>
    /// Catchment point data
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Latitude of centroid
        /// </summary>
        public double latitude;
        /// <summary>
        /// Longitude of centroid
        /// </summary>
        public double longitude;
        /// <summary>
        /// Total cell area
        /// </summary>
        public double cellArea;
        /// <summary>
        /// Cell area that intersects the catchment
        /// </summary>
        public double containedArea;
        /// <summary>
        /// Percent coverage of the intersection
        /// </summary>
        public double percentArea;
    }

    public class CatchmentAggregation
    {
        public string listToString(List<string> lst, out string errorMsg)
        {
            errorMsg = "";
            if (lst.Count <= 0)
            {
                errorMsg = "The list must contain at least one COMID";
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (string comid in lst)
            {
                sb.Append(comid);
                sb.Append(",");
            }
            string comIDs = sb.ToString();
            int index = comIDs.LastIndexOf(",");
            if (index > 0)
            {
                comIDs.Remove(index, 1);
            }
            comIDs = comIDs.Remove(index, 1);
            return comIDs;
        }

        public List<string> prepareCOMID(string geoNumber, string geo, out string errorMsg)
        {
            List<string> lst = new List<string>();
            //string lst = "";
            DataTable dt = new DataTable();
            errorMsg = "";
            if (geo == "com_id_list" || geo == "com_id_num")
            {
                //errorMsg = "Invalid geometry.  The system works with HUC 8 or HUC12 only.";
                foreach(string str in geoNumber.Split(','))
                {
                    lst.Add(str);
                }
            }
            if (geo == "huc_12_num")
            {
                lst = SQLiteRequest("Select COMID From HUC12_PU_COMIDs_CONUS Where HUC12='" + geoNumber + "'");
            }
            else if (geo == "huc_8_num")
            {
                lst = SQLiteRequest("Select COMID From PlusFlowlineVAA Where SUBSTR(ReachCode, 1, 8)='" + geoNumber + "'");
            }
            return lst;
        }

        public List<string> SQLiteRequest(string query)
        {
            string dbPath = "/app/App_Data/catchments.sqlite";
            if (!File.Exists(dbPath))
            {
                dbPath = @".\App_Data\catchments.sqlite";
            }
            
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = dbPath;
            DataTable dt = new DataTable();

            using (SQLiteConnection con = new SQLiteConnection(connectionStringBuilder.ConnectionString))
            {
                con.Open();
                SQLiteCommand com = con.CreateCommand();
                com.CommandText = query;
                using (SQLiteDataAdapter dr = new SQLiteDataAdapter(com))
                {
                    dr.Fill(dt);
                }
                con.Close();
            }

            List<string> comIDs = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                comIDs.Add(dr["COMID"].ToString());
            }

            return comIDs;
        }

        public GeometryData getData(ITimeSeriesInput input, List<string> coms, out string errorMsg)
        {
            errorMsg = "";
            string baseURL = "http://localhost:7777/hms/gis/percentage/?";//"http://127.0.0.1:5000/gis/rest/hms/percentage/?";
            Dictionary<string, string> metadata = input.Geometry.GeometryMetadata;
            //Check for huc arguments 
            if(input.Geometry.HucID != null)
            {
                baseURL += "huc_" + input.Geometry.HucID.Length + "_num=" + input.Geometry.HucID;
            }
            else
            {
                string comList = listToString(coms, out errorMsg);
                baseURL += "com_id_list=" + comList;
            }

            if(input.Geometry.GeometryMetadata["precipSource"] != null)
            {
                baseURL += "&grid_source=" + input.Geometry.GeometryMetadata["precipSource"];
            }
            else
            {
                baseURL += "&grid_source=nldas";
            }

            string dataURL = "http://localhost:7777/hms/data?job_id=";
            WebClient myWC = new WebClient();
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            string data = "";
            dynamic taskData = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string jobID = "";
                while(retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(baseURL);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    jobID = JSON.Deserialize<Dictionary<string,string>>(reader.ReadToEnd())["job_id"];
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
                    if(taskData["status"] == "SUCCESS")
                    {
                        success = true;
                    }
                    else if(taskData["status"] == "FAILURE" || taskData["status"] == "PENDING")
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
                errorMsg =  "ERROR: Unable to obtain data for the specified query." + ex.Message;
            }
            GeometryData geodata = JsonConvert.DeserializeObject<GeometryData>(JsonConvert.SerializeObject(taskData["data"]));
            
            return geodata;
        }

        public ITimeSeriesOutput getCatchmentAggregation(ITimeSeriesInput input, ITimeSeriesOutput result, GeometryData geodata, Boolean agg)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            int i = 0;

            if (agg)
            {
                List<Tuple<string, Point>> ptsList = new List<Tuple<string, Point>>();

                foreach (KeyValuePair<string, Catchment> points in geodata.geometry)
                {
                    foreach (Point point in points.Value.points)
                    {
                        var tuple = new Tuple<string, Point>(points.Key, point);
                        ptsList.Add(tuple);
                    }
                }

                foreach (var entry in result.Data)
                {
                    double average = 0.0;
                    foreach (Tuple<string, Point> tup in ptsList)//foreach (Point pt in ptsList)
                    {
                        List<string> outList = new List<string>();
                        Point pt = tup.Item2;
                        DateTime date = new DateTime();
                        string dateString = entry.Key.ToString().Substring(0, entry.Key.ToString().Length - 3);
                        DateTime.TryParse(dateString, out date);
                        outList.Add(dateString);//Date
                        outList.Add(tup.Item1.ToString());//ComID
                        outList.Add((Convert.ToDouble(entry.Value[0])).ToString());//runoff
                        //outList.Add((Math.Round(pt.percentArea, 5).ToString()));//% area
                        outList.Add(Math.Round((Convert.ToDouble(entry.Value[0]) * (pt.percentArea / 100)), 8).ToString());//% runoff
                        average += Math.Round((Convert.ToDouble(entry.Value[0]) * (pt.percentArea / 100)), 8);
                        string key = i.ToString();//Arbitrary ID to keep track of dictionary values
                        //output.Data.Add(key, outList);
                        i++;
                    }
                    output.Data.Add(entry.Key.ToString(), new List<string> { (average / ptsList.Count).ToString("E3") });
                }

                output.Dataset = result.Dataset.ToString();
                output.DataSource = input.Source.ToString();

                output.Metadata = new Dictionary<string, string>()
                {
                    { "request_time", DateTime.Now.ToString() },
                    { "column_1", "Date" },
                    { "column_2", "COMID" },
                    { "column_3", "Total " + result.Dataset.ToString() },
                    { "column_5", "Percent " + result.Dataset.ToString() }
                };
            }
            else
            {
                foreach (var entry in result.Data)
                {
                    List<string> outList = new List<string>();
                    DateTime date = new DateTime();
                    string dateString = entry.Key.ToString();//.Substring(0, entry.Key.ToString().Length - 3);
                    DateTime.TryParse(dateString, out date);
                    outList.Add((Convert.ToDouble(entry.Value[0])).ToString());//runoff
                    output.Data.Add(date.ToShortDateString(), outList);
                }

                output.Metadata = new Dictionary<string, string>()
                {
                    { "request_time", DateTime.Now.ToString() },
                    { "column_1", "Date" },
                    { "column_2", result.Dataset.ToString() },
                    { "units", "mm" }
                };
            }
            output.Dataset = result.Dataset.ToString();
            output.DataSource = input.Source.ToString();
            return output;
        }
    }
}
