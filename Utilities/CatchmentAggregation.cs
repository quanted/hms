using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Data.SQLite;

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

        public ITimeSeriesOutput getCatchmentAggregation(ITimeSeriesInput input, ITimeSeriesOutput result)
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
            bool entireh8 = false;
            Dictionary<string, string> metadata = input.Geometry.GeometryMetadata;
            List<string> comIDS = new List<string>();
            //Check for huc arguments otherwise use lat long
            if (metadata.ContainsKey("huc_8_num"))
            {
                //baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?huc_8_num=01060002&com_id_num=9311911";
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?huc_8_num=" + metadata["huc_8_num"];

                if (metadata.ContainsKey("com_id_num") && metadata.Count == 2)
                {
                    baseURL += "&com_id_num=" + metadata["com_id_num"];
                    comIDS.Add(metadata["com_id_num"]);
                }
                else if (metadata.Count > 2)
                {
                    foreach (string val in metadata.Values)
                    {
                        if (val != metadata["huc_8_num"])
                        {
                            comIDS.Add(val);
                        }
                    }
                }
            }
            else if (metadata.ContainsKey("huc_12_num"))
            {
                baseURL = "http://127.0.0.1:5000/gis/rest/hms/percentage/?huc_12_num=" + metadata["huc_12_num"];
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
                return err.ReturnError("ERROR: Unable to download data. " + ex.Message);
            }
            GeometryData geodata = JsonConvert.DeserializeObject<GeometryData>(data);

            if (comIDS.Count > 0)
            {
                Dictionary<string, Catchment> newgeodata = new Dictionary<string, Catchment>();
                for (int x = 0; x < comIDS.Count; x++)
                {
                    string key = comIDS[x].ToString();
                    Catchment val = geodata.geometry[key];
                    newgeodata.Add(key, val);
                }
                geodata.geometry = newgeodata;
            }
            else
            {
                entireh8 = true;
            }

            //
            List<Point> ptsList = new List<Point>();
            foreach (KeyValuePair<string, Catchment> points in geodata.geometry)
            {
                foreach (Point point in points.Value.points)
                {
                    if (entireh8)
                    {
                        comIDS.Add(points.Key);
                    }
                    ptsList.Add(point);
                }
            }

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            int i = 0;
            foreach (var entry in result.Data)
            {
                int j = 0;
                foreach (Point pt in ptsList)
                {
                    List<string> outList = new List<string>();
                    //outList.Add(entry.Key.ToString());//Date
                    outList.Add(comIDS[j].ToString());//ComID
                    outList.Add((Convert.ToDouble(entry.Value[0])).ToString());//runoff
                    outList.Add((Math.Round(pt.percentArea, 5).ToString()));//% area
                    outList.Add(Math.Round((Convert.ToDouble(entry.Value[0]) * pt.percentArea / 100), 8).ToString());//% runoff
                    outList.Add(pt.latitude.ToString());//lat
                    outList.Add(pt.longitude.ToString());//long
                    string key = entry.Key.ToString() + i.ToString();
                    //string key = i.ToString();//Index
                    output.Data.Add(key, outList);
                    i++;
                    j++;
                }
            }


            /*
            Dictionary<string, List<Point>> ptsList = new Dictionary<string, List<Point>>();
            //Dictionary<string, Point> ptsList = new Dictionary<string, Point>();
            //List<Point> ptsList = new List<Point>();
            foreach (KeyValuePair<string, Catchment> points in geodata.geometry)
            {
                ptsList.Add(points.Key, points.Value.points);
            }

            //Calculating total % area of a catchment(Total Area of catchment / total area of intersecting cells)
            Dictionary<string, double> totalPercentAreaList = new Dictionary<string, double>();
            foreach (KeyValuePair<string, List<Point>> pt in ptsList)
            {
                string comIDKey = pt.Key;
                double totalContainedArea = 0.0;
                double totalCellAreas = 0.0;
                double totalPercentArea = 0.0;
                foreach (Point p in pt.Value)
                {
                    totalContainedArea += p.containedArea;
                    totalCellAreas += p.cellArea;
                }
                totalPercentArea = Math.Round(totalContainedArea / totalCellAreas, 5);
                totalPercentAreaList.Add(comIDKey, totalPercentArea);
            }

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();

            int i = 0;
            foreach (var entry in result.Data)
            {
                foreach (KeyValuePair<string, double> pt in totalPercentAreaList)
                {
                    List<string> outList = new List<string>();
                    string dateKey = entry.Key;
                    double totalRunoff = Convert.ToDouble(entry.Value[0]);
                    outList.Add(pt.Key);
                    outList.Add(dateKey);
                    outList.Add(totalRunoff.ToString());
                    outList.Add((totalRunoff * pt.Value).ToString());
                    output.Data.Add(i.ToString(), outList);
                    i++;
                }
            }*/

            string types = input.GetType().Name;
            types = types.Remove(types.Length - 5);


            output.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "COMID" },
                { "column_3", types },
                { "column_4", "Percent Area" },
                { "column_5", "Percent " + types },
                { "column_6", "Latitude" },
                { "column_7", "Longitude" }
            };
            return output;
        }
    }
}
