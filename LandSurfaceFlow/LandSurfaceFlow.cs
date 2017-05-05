using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HMSLandSurfaceFlow
{
    public class LandSurfaceFlow : HMSLDAS.IHMSModule
    {

        // Class variables which define a landsurfaceflow object.
        public double latitude { get; set; }                            // Latitude for timeseries
        public double longitude { get; set; }                           // Longitude for timeseries
        public DateTime startDate { get; set; }                         // Start data for timeseries
        public DateTime endDate { get; set; }                           // End date for timeseries
        public double gmtOffset { get; set; }                           // Timezone offset from GMT
        public string tzName { get; set; }                              // Timezone name
        public string dataSource { get; set; }                          // NLDAS, GLDAS, or SWAT algorithm simulation
        public bool localTime { get; set; }                             // False = GMT time, true = local time.
        public List<HMSTimeSeries.HMSTimeSeries> ts { get; set; }       // TimeSeries Data
        public double cellWidth { get; set; }                           // LDAS data point cell width, defined by source.
        public string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
        public HMSGDAL.HMSGDAL gdal { get; set; }                       // GDAL object for GIS operations.
        public HMSJSON.HMSJSON.HMSData jsonData { get; set; }

        public LandSurfaceFlow() { }

        /// <summary>
        /// Constructor for a generic landsurfaceflow object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        public LandSurfaceFlow(out string errorMsg, string startDate, string endDate, string source, bool local) : this(out errorMsg, startDate, endDate, source, local, null)
        { }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="sfPath"></param>
        public LandSurfaceFlow(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, "0.0", "0.0", startDate, endDate, source, local, sfPath)
        {   
        }

        /// <summary>
        /// Constructor using latitude and longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="sfPath"></param>
        public LandSurfaceFlow(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, latitude, longitude, startDate, endDate, source, local, sfPath, "0.0", "NaN")
        { 
        }

        /// <summary>
        /// Constructor using latitude and longitude, with gmtOffset already known.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="sfPath"></param>
        public LandSurfaceFlow(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath, string gmtOffset, string tzName)
        {
            errorMsg = "";
            this.gmtOffset = Convert.ToDouble(gmtOffset);
            this.dataSource = source.ToLower();
            this.localTime = local;
            this.tzName = tzName;
            if (errorMsg.Contains("ERROR")) { return; }
            SetDates(out errorMsg, startDate, endDate);
            if (errorMsg.Contains("ERROR")) { return; }
            ts = new List<HMSTimeSeries.HMSTimeSeries>();
            if (string.IsNullOrWhiteSpace(sfPath))
            {
                this.shapefilePath = null;
                try
                {
                    this.latitude = Convert.ToDouble(latitude.Trim());
                    this.longitude = Convert.ToDouble(longitude.Trim());
                }
                catch
                {
                    errorMsg = "ERROR: Invalid latitude or longitude value.";
                    return;
                }
            }
            else
            {
                this.shapefilePath = sfPath;
                this.latitude = 0.0;
                this.longitude = 0.0;
            }
            if (this.dataSource == "nldas") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "gldas") { this.cellWidth = 0.2500; }
            this.gdal = new HMSGDAL.HMSGDAL();
        }
    
        /// <summary>
        /// Sets startDate and endDate, checks that dates are valid (start date before end date, end date no greater than today, start dates are valid for data sources)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void SetDates(out string errorMsg, string start, string end)
        {
            errorMsg = "";
            try
            {
                this.startDate = Convert.ToDateTime(start);
                this.endDate = Convert.ToDateTime(end);
                //Add Hours to start/end dates
                DateTime newStartDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day, 00, 00, 00);
                DateTime newEndDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 00, 00);
                this.startDate = newStartDate;
                this.endDate = newEndDate;
            }
            catch
            {
                errorMsg = "ERROR: Invalid data format. Please provide a date as mm-dd-yyyy or mm/dd/yyyy.";
                return;
            }
            if (DateTime.Compare(this.endDate, DateTime.Today) > 0)   //If endDate is past today's date, endDate is set to 2 days prior to today.
            {
                this.endDate = DateTime.Today.AddDays(-2.0);
            }
            if (DateTime.Compare(this.startDate, this.endDate) > 0)
            {
                errorMsg = "ERROR: Invalid dates entered. Please enter an end date set after the start date.";
                return;
            }
            if (this.dataSource.Contains("nldas"))   //NLDAS data collection start date
            {
                DateTime minDate = new DateTime(1979, 01, 02);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to NLDAS start date
                }
            }
            else if (this.dataSource.Contains("gldas"))   //GLDAS data collection start date
            {
                DateTime minDate = new DateTime(2000, 02, 25);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to GLDAS start date
                }
            }
        }

        /// <summary>
        /// Gets land surface runoff data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            double offset = gmtOffset;
            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);

            if (this.shapefilePath != null && this.dataSource.Contains("ldas"))
            {
                bool sourceNLDAS = true;
                if (this.dataSource.Contains("gldas")) { sourceNLDAS = false; }
                double[] center = gldas.DetermineReturnCoordinates(out errorMsg, gdal.ReturnCentroid(out errorMsg, this.shapefilePath), sourceNLDAS);
                this.latitude = center[0];   
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            else if (this.gdal.geoJSON != null)
            {
                double[] center = gdal.ReturnCentroidFromGeoJSON(out errorMsg);
                this.latitude = center[0];
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
            }
            else
            {
                ts[0].cellCoverage = 1.0;
            }

            // Updated to use call to google instead of using shapefile search. Commented code obsolete.
            if (this.localTime == true && !String.IsNullOrWhiteSpace(this.tzName))
            {
                HMSUtils.Utils utils = new HMSUtils.Utils();
                Dictionary<string, string> tzDetails = utils.GetTZInfo(out errorMsg, this.latitude, this.longitude);
                if (tzDetails.ContainsKey("rawOffset") && tzDetails.ContainsKey("timeZoneId"))
                {
                    this.gmtOffset = Convert.ToDouble(tzDetails["rawOffset"]) / 3600;
                    this.tzName = tzDetails["timeZoneId"];
                }
                else if (tzDetails.ContainsKey("tzOffset") && tzDetails.ContainsKey("tzName"))
                {
                    this.gmtOffset = Convert.ToDouble(tzDetails["tzOffset"]);
                    this.tzName = tzDetails["tzName"];
                }
                //this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);         //Gets the GMT offset
                //if (errorMsg.Contains("ERROR")) { return null; }
                //this.tzName = ts[0].tzName;                                                                     //Gets the Timezone name
                if (errorMsg.Contains("ERROR")) { return null; }
                this.startDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.startDate, true);
                this.endDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.endDate, false);
            }

            // Define this from the utils class, possibly read from file.
            if (this.shapefilePath != null || this.gdal.geoJSON != null)
            {
                if (gdal.coordinatesInShapefile.Count > 30)
                {
                    errorMsg = "ERROR: Feature geometries containing more than 30 datapoints are prohibited. Current feature contains " + gdal.coordinatesInShapefile.Count + " " + this.dataSource + " data points."; return null;
                }
            }
            if (this.dataSource.Contains("ldas"))                                   // LDAS RUNOFF DATA
            {
                //TODO: move ldas data retrieval for runoff to the landsurfaceflow class.
                gldas.BeginLDASSequence(out errorMsg, this, "Surface_Flow", newTS);
            }
            else if (this.dataSource.Contains("curvenumber"))                       // CURVE NUMBER DATA
            {
                int totalPoints = 0;
                if (this.shapefilePath != null || this.gdal.geoJSON != null)
                {
                    totalPoints = this.gdal.coordinatesInShapefile.Count;
                    for (int i = 0; i < totalPoints; i++)
                    {
                        if (i != 0) { newTS = new HMSTimeSeries.HMSTimeSeries(); this.ts.Add(newTS); }
                        string data = GetDataCurveNumber(out errorMsg);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        newTS.SetTimeSeriesVariables(out errorMsg, newTS, data, this.dataSource);
                        newTS.newMetaData = String.Concat(newTS.newMetaData, "percentInCell=", this.gdal.areaPrecentInGeometry[i], "\nareaInCell=", this.gdal.areaGeometryIntersection[i], "\n");
                        if (errorMsg.Contains("ERROR")) { return null; }
                    }
                }
                else
                {
                    string data = GetDataCurveNumber(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    this.ts[0].SetTimeSeriesVariables(out errorMsg, newTS, data, this.dataSource);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            this.jsonData = output.ConstructHMSDataFromTS(out errorMsg, this.ts, "SurfaceFlow", this.dataSource, this.localTime, this.gmtOffset);
            if (errorMsg.Contains("ERROR")) { return null; }
            return ts;
        }

        /// <summary>
        /// Gets the land surface runoff data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string GetDataSetsString(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            string combinedData = output.CombineTimeseriesAsJson(out errorMsg, this.jsonData);
            if (errorMsg.Contains("ERROR")) { return null; }
            return combinedData;
        }

        /// <summary>
        /// Get land surface runoff data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public void GetDataSetsObject(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
        }

        /// <summary>
        /// Constructs URL and retrieves data for curve number runoff.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string GetDataCurveNumber(out string errorMsg)
        {
            errorMsg = "";

            // NEED TO DO: test the return results for the POST request to correctly handle the information.

            // Constructing the data url
            string source_url = "CURVE_NUMBER_INT";
            Dictionary<string, string> urls = (Dictionary<string, string>)HttpContext.Current.Application["urlList"];
            string url = urls[source_url];

            // POST dictionary for lat/lon argument
            string parameters = "startdate={" + this.startDate.ToString() + "}&enddate={" + this.endDate.ToString() +
                "}&latitude={" + this.latitude.ToString() + "}&longitude={" + this.longitude.ToString() + "}";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);

            string data = "";
            try
            {
                int retries = 5;
                string status = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(url);

                    wr.Method = "POST";
                    wr.ContentType = "application/x-www-form-urlencoded";
                    wr.ContentLength = byteArray.Length;
                    using (Stream stream = wr.GetRequestStream())
                    {
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

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
                errorMsg = "ERROR: Unable to download requested curve number data. " + ex.Message;
                return null;
            }

            return data;
        }
    }
}
