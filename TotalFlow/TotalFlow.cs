using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSTotalFlow
{
    public class TotalFlow : HMSLDAS.IHMSModule
    {

        // Class variables which define a TotalFlow object.
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

        public HMSLandSurfaceFlow.LandSurfaceFlow landSurfaceFlow { get; set; }             // LandSurfaceFlow object
        public HMSBaseFlow.BaseFlow baseFlow { get; set; }                                  // BaseFlow object
        public HMSJSON.HMSJSON.HMSData jsonData { get; set; }

        public TotalFlow() { }

        /// <summary>
        /// Constructor for a generic totalflow object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        public TotalFlow(out string errorMsg, string startDate, string endDate, string source, bool local) : this(out errorMsg, startDate, endDate, source, local, null)
        { }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public TotalFlow(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, "0.0", "0.0", startDate, endDate, source, local, sfPath)
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
        public TotalFlow(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, latitude, longitude, startDate, endDate, source, local, sfPath, "0.0", "NaN")
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
        public TotalFlow(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath, string gmtOffset, string tzName)
        {
            errorMsg = "";
            this.gmtOffset = Convert.ToDouble(gmtOffset);
            this.dataSource = source;
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
            if (this.dataSource == "NLDAS") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "GLDAS") { this.cellWidth = 0.2500; }
            this.gdal = new HMSGDAL.HMSGDAL();

            this.landSurfaceFlow = new HMSLandSurfaceFlow.LandSurfaceFlow();
            this.baseFlow = new HMSBaseFlow.BaseFlow();
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
            if (this.dataSource.Contains("NLDAS"))   //NLDAS data collection start date
            {
                DateTime minDate = new DateTime(1979, 01, 02);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to NLDAS start date
                }
            }
            else if (this.dataSource.Contains("GLDAS"))   //GLDAS data collection start date
            {
                DateTime minDate = new DateTime(2000, 02, 25);
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;   //start date is set to GLDAS start date
                }
            }
        }
        
        /// <summary>
        /// Copies TotalFlow variable values to BaseFlow variables.
        /// </summary>
        /// <param name="newBaseFlow"></param>
        private void SetBaseFlow(HMSBaseFlow.BaseFlow newBaseFlow)
        {
            newBaseFlow.latitude = this.latitude;
            newBaseFlow.longitude = this.longitude;
            newBaseFlow.startDate = this.startDate;
            newBaseFlow.endDate = this.endDate;
            newBaseFlow.gmtOffset = this.gmtOffset;
            newBaseFlow.tzName = this.tzName;
            newBaseFlow.dataSource = this.dataSource;
            newBaseFlow.localTime = this.localTime;
            newBaseFlow.ts = new List<HMSTimeSeries.HMSTimeSeries>();
            newBaseFlow.cellWidth = this.cellWidth;
            newBaseFlow.shapefilePath = this.shapefilePath;
            newBaseFlow.gdal = this.gdal;
        }

        /// <summary>
        /// Copies TotalFlow variable values to BaseFlow variables.
        /// </summary>
        /// <param name="newSurfaceFlow"></param>
        private void SetSurfaceFlow(HMSLandSurfaceFlow.LandSurfaceFlow newSurfaceFlow)
        {
            newSurfaceFlow.latitude = this.latitude;
            newSurfaceFlow.longitude = this.longitude;
            newSurfaceFlow.startDate = this.startDate;
            newSurfaceFlow.endDate = this.endDate;
            newSurfaceFlow.gmtOffset = this.gmtOffset;
            newSurfaceFlow.tzName = this.tzName;
            newSurfaceFlow.dataSource = this.dataSource;
            newSurfaceFlow.localTime = this.localTime;
            newSurfaceFlow.ts = new List<HMSTimeSeries.HMSTimeSeries>();
            newSurfaceFlow.cellWidth = this.cellWidth;
            newSurfaceFlow.shapefilePath = this.shapefilePath;
            newSurfaceFlow.gdal = this.gdal;
        }

        /// <summary>
        /// Gets total flow data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            double offset = gmtOffset;
            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);

            SetBaseFlow(baseFlow);
            SetSurfaceFlow(landSurfaceFlow);

            baseFlow.ts.Add(newTS);
            landSurfaceFlow.ts.Add(newTS);

            if (this.shapefilePath != null && this.dataSource.Contains("LDAS"))
            {
                bool sourceNLDAS = true;
                if (this.dataSource.Contains("GLDAS")) { sourceNLDAS = false; }
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

            if (this.localTime == true && !String.IsNullOrWhiteSpace(this.tzName))
            {
                this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);     //Gets the GMT offset
                if (errorMsg.Contains("ERROR")) { return null; }
                this.tzName = ts[0].tzName;                                                                 //Gets the Timezone name
                if (errorMsg.Contains("ERROR")) { return null; }
                this.startDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.startDate, true);
                this.endDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.endDate, false);
            }
            gldas.BeginLDASSequence(out errorMsg, this.baseFlow, "BaseFlow", newTS);
            if (errorMsg.Contains("ERROR")) { return null; }
            gldas.BeginLDASSequence(out errorMsg, this.landSurfaceFlow, "Surface_Flow", newTS);
            if (errorMsg.Contains("ERROR")) { return null; }
            SetTotalFlowTimeSeries(out errorMsg, newTS);
            if (errorMsg.Contains("ERROR")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            this.jsonData = output.ConstructHMSDataFromTS(out errorMsg, this.ts, "TotalFlow", this.dataSource, this.localTime, this.gmtOffset);
            return ts;
        }

        /// <summary>
        /// Combines both land surface flow and base flow data to calculate total flow.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="newTS"></param>
        private void SetTotalFlowTimeSeries(out string errorMsg, HMSTimeSeries.HMSTimeSeries newTS)
        {
            errorMsg = "";
            try
            {
                for (int i = 0; i < baseFlow.ts.Count; i++)
                {
                    newTS = new HMSTimeSeries.HMSTimeSeries();
                    string data = "";
                    string[] surfaceflowData = landSurfaceFlow.ts[i].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                    string[] baseflowData = baseFlow.ts[i].timeSeries.Split('\n').Select(s => s.Trim()).ToArray();
                    for (int j = 0; j < surfaceflowData.Length - 1; j++)
                    {
                        string[] dataArrayLine = surfaceflowData[j].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        string[] baseflowLine = baseflowData[j].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        double value = Convert.ToDouble(dataArrayLine[2].Trim()) + Convert.ToDouble(baseflowLine[2].Trim());
                        data += dataArrayLine[0] + " " + dataArrayLine[1] + " " + string.Format("{0:0.0000E+00}", value) + "\n";
                    }
                    if (i != 0) { ts.Add(newTS); }
                    ts[i].SetTimeSeriesVariables(out errorMsg, newTS, String.Concat(landSurfaceFlow.ts[i].metaData, "\n", baseFlow.ts[i].metaData, "\n\n     Date&Time     Data\n", data, "MEAN"), dataSource);
                    ts[i].metaData = String.Concat(landSurfaceFlow.ts[i].metaData, "\n", baseFlow.ts[i].metaData);
                    ts[i].newMetaData = String.Concat(landSurfaceFlow.ts[i].newMetaData, "\n", baseFlow.ts[i].newMetaData);
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: " + ex;
                return;
            }
        }

        /// <summary>
        /// Gets the total flow data using a shapefile.
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
        /// Get total flow data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public void GetDataSetsObject(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
        }
    }
}
