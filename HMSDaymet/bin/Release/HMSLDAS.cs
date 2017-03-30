using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Security;

namespace HMSLDAS
{
    public interface IHMSModule
    {
        double latitude { get; set; }                            // Latitude for timeseries
        double longitude { get; set; }                           // Longitude for timeseries
        DateTime startDate { get; set; }                         // Start data for timeseries
        DateTime endDate { get; set; }                           // End date for timeseries
        double gmtOffset { get; set; }                           // Timezone offset from GMT
        string tzName { get; set; }                              // Timezone name
        string dataSource { get; set; }                          // NLDAS, GLDAS, or SWAT algorithm simulation
        bool localTime { get; set; }                             // False = GMT time, true = local time.
        List<HMSTimeSeries.HMSTimeSeries> ts { get; set; }       // TimeSeries Data
        double cellWidth { get; set; }                           // LDAS data point cell width, defined by source.
        string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
        HMSGDAL.HMSGDAL gdal { get; set; }                       // GDAL object for GIS operations.
        HMSJSON.HMSJSON.HMSData jsonData { get; set; }
    }

    public class HMSLDAS
    {

        /// <summary>
        /// Retrieves the data based upon the argument variables. 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string LDAS(out string errorMsg, double latitude, double longitude, DateTime startDate, DateTime endDate, string source, HMSTimeSeries.HMSTimeSeries ts, string wsPath)
        {
            errorMsg = "";
            string data = GetData(out errorMsg, latitude, longitude, startDate, endDate, source, ts, wsPath);
            return data;
        }
        
        /// <summary>
        /// Retrieves the data based upon the argument variables. Also executes functions from HMSGDAL.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="local"></param>
        /// <param name="source"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        public string LDAS(out string errorMsg, double latitude, double longitude, DateTime startDate, DateTime endDate, bool local, string source, ref double gmtOffset, HMSTimeSeries.HMSTimeSeries ts, string wsPath)
        {
            errorMsg = "";

            string data = GetData(out errorMsg, latitude, longitude, startDate, endDate, local, source, ref gmtOffset, ts, wsPath);
            return data;
        }

        /// <summary>
        /// Retrieves the data based upon the argument variables. Calculates offset and timezone.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="local"></param>
        /// <param name="source"></param>
        /// <param name="gmtOffset"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        private string GetData(out string errorMsg, double latitude, double longitude, DateTime startDate, DateTime endDate, bool local, string source, ref double gmtOffset, HMSTimeSeries.HMSTimeSeries ts, string wsPath) 
        {
            errorMsg = "";

            double offset = 0.0;
            DateTime newStartDate = new DateTime();
            DateTime newEndDate = new DateTime();

            //Adjusts the Date by the GMT offset, data is retrieved in GMT time.
            if (local == true)
            {
                HMSGDAL.HMSGDAL gdal = new HMSGDAL.HMSGDAL();
                if (gmtOffset == 0.0)
                {
                    offset = gdal.GetGMTOffset(out errorMsg, latitude, longitude, ts);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                gmtOffset = offset;
                newStartDate = gdal.AdjustDateByOffset(out errorMsg, offset, startDate, true);
                newEndDate = gdal.AdjustDateByOffset(out errorMsg, offset, endDate, false);
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            else
            {
                newStartDate = startDate;
                newEndDate = endDate;
            }

            string url = ConstructURL(out errorMsg, latitude, longitude, newStartDate, newEndDate, source, wsPath);
            if (errorMsg.Contains("ERROR")) { return null; }
            ts.newMetaData += "url=" + url;
            string data = RetrieveData(out errorMsg, url);
            if (errorMsg.Contains("ERROR")) { return null; }
            return data;
        }

        /// <summary>
        /// Retrieves the data based upon the argument varaibles. Executes no GDAL funtions.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetData(out string errorMsg, double latitude, double longitude, DateTime startDate, DateTime endDate, string source, HMSTimeSeries.HMSTimeSeries ts, string wsPath)
        {
            errorMsg = "";
            string url = ConstructURL(out errorMsg, latitude, longitude, startDate, endDate, source, wsPath);
            if (errorMsg.Contains("ERROR")) { return null; }
            ts.newMetaData += "url=" + url + "\n";
            string data = RetrieveData(out errorMsg, url);
            if (errorMsg.Contains("ERROR")) { return null; }
            return data;
        }

        /// <summary>
        /// Constructs the URL for retrieving the data selected by the varaible 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private string ConstructURL(out string errorMsg, double latitude, double longitude, DateTime startDate, DateTime endDate, string source, string wsPath)
        {
            errorMsg = "";
            //string prepInfo = System.IO.Directory.GetCurrentDirectory() + @"\url_info.txt";  // URL configuration info.
            string prepInfo = System.AppDomain.CurrentDomain.BaseDirectory + @"bin\url_info.txt";  // URL configuration info.
            StringBuilder builder = new StringBuilder();
            //string urlStr = "";
            string[] lineData;
            try
            {
                foreach (string line in File.ReadLines(prepInfo))
                {
                    lineData = line.Split(' ');
                    if (lineData[0].Equals(source + "_URL", StringComparison.OrdinalIgnoreCase))
                    {
                        //urlStr = lineData[1];
                        builder.Append(lineData[1]);
                        break;
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to load URL details from configuration file.";
                return null;
            }
            if (source.Contains("NLDAS"))
            {
                //Add X and Y coordinates
                string[] xy = GetXYNLDAS(out errorMsg, longitude, latitude); // [0] = x, [1] = y
                if (errorMsg.Contains("ERROR")) { return null; }
                builder.Append("X" + xy[0] + "-" + "Y" + xy[1]);
                //urlStr = urlStr + "X" + xy[0] + "-" + "Y" + xy[1];
            }
            else if(source.Contains("GLDAS"))
            {
                //Add latitude/longitude points
                builder.Append(@"%28" + longitude + @",%20" + latitude + @"%29");
                //urlStr = urlStr + @"%28" + longitude + @",%20" + latitude + @"%29";
            }
            //Add Start and End Date
            string[] startDT = startDate.ToString("yyyy-MM-dd HH").Split(' ');
            string[] endDT = endDate.ToString("yyyy-MM-dd HH").Split(' ');
            //urlStr = urlStr + @"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1];
            builder.Append(@"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1] + @"&type=asc2");
            //Add format type 
            //return urlStr + @"&type=asc2";
            return builder.ToString();
        }

        /// <summary>
        /// Downloads the data using the constructed URL.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RetrieveData(out string errorMsg, string url)
        {
            errorMsg = "";
            string data = "";
            WebClient myWC = new WebClient();
            string[] checkData;
            try
            {
                //https certification ERROR requires the following statement to successfully retrieve data
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                byte[] dataBuffer = myWC.DownloadData(url);
                data = Encoding.UTF8.GetString(dataBuffer);
            }
            catch
            {
                errorMsg = "ERROR: Unable to download requested data.";
                return null;
            }
            if (data.Contains("ERROR"))
            {
                if (data.Contains("Specified end time is greater than data end time"))
                {
                    checkData = data.Split(new[] { "\n" }, StringSplitOptions.None);
                    for (int i = 0; i < checkData.Length; i++)
                    {
                        if (checkData[i].Contains("end_time"))
                        {
                            checkData = checkData[i].Split('=');
                            errorMsg = "ERROR: Specified end time is greater than data end time. Data end date: " + checkData[1];
                            return null;
                        }
                    }
                }
                else
                {
                    errorMsg = "ERROR: Failed to return data for the selected date and location.";
                    return null;
                }
            }
            myWC.Dispose();
            return data;
        }

        /// <summary>
        /// Converts latitude/longitude to X/Y values for NLDAS location.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string[] GetXYNLDAS(out string errorMsg, double longitude, double latitude)
        {
            errorMsg = "";
            double xMax = 463.0;
            double yMax = 223.0;
            double x, y = 0.0;
            string[] results = new string[2];
            x = (longitude + 124.9375) / 0.125;
            y = (latitude - 25.0625) / 0.125;
            if (x > xMax || x < 0)
            {
                errorMsg = "ERROR: Latitude or longitude value outside accepted range for chosen data source or is invalid.";
                return null;
            }
            if (y > yMax || y < 0)
            {
                errorMsg = "ERROR: Latitude or longitude value outside accepted range for chosen data source or is invalid.";
                return null;
            }
            results[0] = Convert.ToString(Math.Round(x, MidpointRounding.AwayFromZero));
            results[1] = Convert.ToString(Math.Round(y, MidpointRounding.AwayFromZero));
            return results;

        }

        /// <summary>
        /// Determines the coordinate values that coorespond to the closest coordinate point in the NLDAS/GLDAS grid.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="nldas"></param>
        /// <returns></returns>
        public double[] DetermineReturnCoordinates(out string errorMsg, double[] givenCoordinates, bool nldas)
        {
            errorMsg = "";
            double latitude = givenCoordinates[0];
            double longitude = givenCoordinates[1];
            double[] coord = new double[2];
            double step = 0.0;
            if (nldas == true)
            {
                step = 0.125;
                double x = (longitude + 124.9375) / step;
                coord[1] = (Math.Round(x, MidpointRounding.AwayFromZero) * step) - 124.9375;
                double y = (latitude - 25.0625) / step;
                coord[0] = (Math.Round(y, MidpointRounding.AwayFromZero) * step) + 25.0625;
            }
            else
            {
                step = 0.25;
                double x = (longitude + 179.8750) / step;
                coord[1] = (Math.Round(x, MidpointRounding.AwayFromZero) * step) - 179.8750;
                double y = (latitude + 59.8750) / step;
                coord[0] = (Math.Round(y, MidpointRounding.AwayFromZero) * step) - 59.8750;
            }
            return coord;
        }

        /// <summary>
        /// Function begins sequence of method calls to retrieve LDAS data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="module"></param>
        /// <param name="dataset"></param>
        /// <param name="newTS"></param>
        public void BeginLDASSequence(out string errorMsg, IHMSModule module, string dataset, HMSTimeSeries.HMSTimeSeries newTS)
        {
            errorMsg = "";
            int totalPoints = 0; 
            string data = "";

            if (module.shapefilePath != null || module.gdal.geoJSON != null)
            {
                totalPoints = module.gdal.coordinatesInShapefile.Count;
                for (int i = 0; i < totalPoints; i++)
                {
                    if (i != 0) { newTS = new HMSTimeSeries.HMSTimeSeries(); module.ts.Add(newTS); }
                    data = LDAS(out errorMsg, module.gdal.coordinatesInShapefile[i].Item1, module.gdal.coordinatesInShapefile[i].Item2, module.startDate, module.endDate, module.dataSource + "_" + dataset, newTS, module.shapefilePath);
                    if (errorMsg.Contains("ERROR")) { return; }
                    newTS.SetTimeSeriesVariables(out errorMsg, newTS, data, module.dataSource);
                    newTS.newMetaData = String.Concat(newTS.newMetaData, "percentInCell=", module.gdal.areaPrecentInGeometry[i], "\nareaInCell=", module.gdal.areaGeometryIntersection[i], "\n");
                    if (errorMsg.Contains("ERROR")) { return; }
                }
            }
            else
            {
                data = LDAS(out errorMsg, module.latitude, module.longitude, module.startDate, module.endDate, module.dataSource + "_" + dataset, module.ts[0], module.shapefilePath);
                if (errorMsg.Contains("ERROR")) { return; }
                module.ts[0].SetTimeSeriesVariables(out errorMsg, newTS, data, module.dataSource);
                if (errorMsg.Contains("ERROR")) { return; }
            }
        } 
    }
}
