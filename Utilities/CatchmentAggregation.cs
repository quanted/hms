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

        public string SQLiteRequest(string query)
        {
            //Create SQLite connection
            SQLiteConnection sqlite = new SQLiteConnection("Data Source=M:\\StreamHydrologyFiles\\NHDPlusV2Data\\database.sqlite");
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
                return null;
            }
            sqlite.Close();
            List<string> comIDs = new List<string>();
            string comList = "";
            foreach (DataRow dr in dt.Rows)
            {
                comList += dr["COMID"].ToString() + ",";
            }
            comList = comList.TrimEnd(comList[comList.Length - 1]);

            return comList;
        }

        public GeometryData getData(ITimeSeriesInput input, ITimeSeriesOutput result)
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

            //Parse geometry data from input page
            double lat = input.Geometry.Point.Latitude;
            double lon = input.Geometry.Point.Longitude;
            string baseURL = "";
            Dictionary<string, string> metadata = input.Geometry.GeometryMetadata;
            List<string> comIDS = new List<string>();
            //Check for huc arguments otherwise use lat long
            if (metadata.ContainsKey("huc_8_num") && metadata["huc_8_num"].Length == 8)
            {
                //https://ofmpub.epa.gov/waters10/NavigationDelineation.Service?pNavigationType=PP&pStartComid=9310951&pStopComid=166443305
                //huc_8_num:01060002
                string query = "SELECT COMID FROM HUC12_PU_COMIDs_CONUS WHERE HUC12 LIKE '" + metadata["huc_8_num"].ToString() + "%' ORDER BY COMID";
                string queryResult = SQLiteRequest(query);
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?huc_8_num=" + metadata["huc_8_num"];
            }
            else if (metadata.ContainsKey("huc_12_num") && metadata["huc_12_num"].Length == 12)
            {
                //huc_12_num:030502040102   huc_12_num:051202070802
                string query = "SELECT COMID FROM HUC12_PU_COMIDs_CONUS WHERE HUC12 = '" + metadata["huc_12_num"].ToString() + "' ORDER BY COMID";
                string queryResult = SQLiteRequest(query);
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?com_id_list=" + queryResult;
            }
            else if (metadata.ContainsKey("com_id_list"))
            {
                string comList = metadata["com_id_list"].Replace(';', ',');
                foreach (string val in comList.Split(','))
                {
                    comIDS.Add(val);
                    //comList += val + ",";
                }
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?com_id_list=" + comList;
            }
            else if (metadata.ContainsKey("com_id_num") && metadata.Count == 1)
            {
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?&com_id_num=" + metadata["com_id_num"];
                comIDS.Add(metadata["com_id_num"]);
            }
            else
            {
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/" + "?lat_long_x=" + lon + "&lat_long_y=" + lat;
                //string baseURL = "http://172.20.100.15/hms/rest/api/v2/hms/gis/percentage/" + "?lat_long_x=" + lon + "&lat_long_y=" + lat;
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
                    wr.Timeout = 600000;//10 min
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
                return null;
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