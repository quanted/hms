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
        private string listToString(List<string> lst, out string errorMsg)
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

        public List<string> prepareCOMID(string HUCNumber, out string errorMsg)
        {
            List<string> lst = new List<string>();
            //string lst = "";
            DataTable dt = new DataTable();
            errorMsg = "";
            if ((HUCNumber.Length != 12) && (HUCNumber.Length != 8))
            {
                //errorMsg = "Invalid HUC Number.  The system works with HUC 8 or HUC12 only.";
                foreach(string str in HUCNumber.Split(','))
                {
                    lst.Add(str);
                }
            }
            if (HUCNumber.Length == 12)
            {
                lst = SQLiteRequest("Select COMID From HUC12_PU_COMIDs_CONUS Where HUC12='" + HUCNumber + "'");
            }
            else if (HUCNumber.Length == 8)
            {
                lst = SQLiteRequest("Select COMID From PlusFlowlineVAA Where SUBSTR(ReachCode, 1, 8)='" + HUCNumber + "'");
            }
            return lst;
        }

        public List<string> SQLiteRequest(string query)
        {
            //Create SQLite connection
            SQLiteConnection sqlite = new SQLiteConnection("Data Source=database.sqlite;Version=3;"); //new SQLiteConnection("Data Source=M:\\StreamHydrologyFiles\\NHDPlusV2Data\\database.sqlite");
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand cmd;
                sqlite.Open();  //Initiate connection to the db
                cmd = sqlite.CreateCommand();
                cmd.CommandText = query;  //set the passed query
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt); //fill the datasource
            }
            catch (SQLiteException ex)
            {
                return null;//"ERROR: Unable to obtain data for the specified query." + ex.Message;
            }
            sqlite.Close();

            List<string> comIDs = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                comIDs.Add(dr["COMID"].ToString());
            }

            return comIDs;
        }

        public GeometryData getData(ITimeSeriesInput input, ITimeSeriesOutput result, List<string> coms, out string errorMsg)
        {
            /*Sample call of new utility in WS[Module].cs
            Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
            ITimeSeriesOutput outs = cd.getCatchmentAggregation(input, result);
            return outs;
            
            Next runoff Use case: Single catchment use case is already implemented.  
            Next use case: Calculate surface, subsurface, and total runoff for a collection 
            of catchments (e.g. HUC12, HUC8, or user supplied lists of NHDPlus catchments)?
            COMID   DateTime    Surface Runoff  Subsurface Runoff   Total Runoff 
            */

            /*
             * Possible workflow: 
             * 1. Each class would generate its own data (ie Surface Runoff class gets ITimeSeries) and call this method.
             * 2. Call % area flask endpoint using lat/long to get json of grid cell data
             *      Other cases like huc 8 and huc12 would need to have those values passed in through geometry metadata box (huc_8_num:01060002,com_id_num:9311911)
             * 3. Return table for each [hour, day, week, month] that gives % runoff of each catchment grid cell
            */
            errorMsg = "";
            //Parse geometry data from input page
            double lat = input.Geometry.Point.Latitude;
            double lon = input.Geometry.Point.Longitude;
            string baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?";
            Dictionary<string, string> metadata = input.Geometry.GeometryMetadata;
            List<string> comIDS = new List<string>();
            //Check for huc arguments otherwise use lat long

            string huc = "";

            string type = metadata["GeometryType"].ToString();
            huc = metadata[type].ToString();
            //List<string> coms = prepareCOMID(huc, out errorMsg);
            string comList = listToString(coms, out errorMsg);
            baseURL += "com_id_list=" + comList;
            if(input.Geometry.GeometryMetadata == null)
            {
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/" + "?lat_long_x=" + lon + "&lat_long_y=" + lat;
            }
            if(type == "huc_8_num")
            {
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?huc_8_num=" + huc;
            }

            WebClient myWC = new WebClient();
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            string data = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(baseURL);
                    wr.Timeout = 1200000;//20 min
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg =  "ERROR: Unable to obtain data for the specified query." + ex.Message;
            }
            GeometryData geodata = JsonConvert.DeserializeObject<GeometryData>(data);

            return geodata;
        }

        public ITimeSeriesOutput getCatchmentAggregation(ITimeSeriesInput input, ITimeSeriesOutput result, GeometryData geodata)
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

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            int i = 0;
            foreach (var entry in result.Data)
            {
                foreach(Tuple<string, Point> tup in ptsList)//foreach (Point pt in ptsList)
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
                    string key = i.ToString();//Arbitrary ID to keep track of dictionary values
                    output.Data.Add(key, outList);
                    i++;
                }
            }
                        
            output.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "COMID" },
                { "column_3", "Total Runoff" },
                { "column_4", "Percent Area" },
                { "column_5", "Percent Runoff" }
            };
            return output;
        }
    }
}