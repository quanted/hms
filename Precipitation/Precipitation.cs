﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HMSPrecipitation
{
    public class Precipitation : HMSLDAS.IHMSModule
    {

        // Class variables which define a precipitation object.
        public double latitude { get; set; }                            // Latitude for timeseries
        public double longitude { get; set; }                           // Longitude for timeseries
        public DateTime startDate { get; set; }                         // Start data for timeseries
        public DateTime endDate { get; set; }                           // End date for timeseries
        public double gmtOffset { get; set; }                           // Timezone offset from GMT
        public string tzName { get; set; }                              // Timezone name
        public string dataSource { get; set; }                          // NLDAS, GLDAS, or SWAT algorithm simulation
        public bool localTime { get; set; }                             // False = GMT time, true = local time.
        public List<HMSTimeSeries.HMSTimeSeries> ts { get; set; }       // TimeSeries Data, unaltered from source
        public double cellWidth { get; set; }                           // LDAS data point cell width, defined by source.
        public string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
        public HMSGDAL.HMSGDAL gdal { get; set; }                       // GDAL object for GIS operations.
        public string station { get; set; }                             // Station ID for NCDC data.
        public HMSJSON.HMSJSON.HMSData jsonData { get; set; }           // Post-Processed data object, ready to be serialized into json string.

        /// <summary>
        /// Default constructor
        /// </summary>
        public Precipitation() { }

        /// <summary>
        /// Constructor for a generic precip object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, bool local) : this(out errorMsg, startDate, endDate, source, local, null)
        {
        }

        /// <summary>
        /// Constructor for getting NCDC data with a station id.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="station"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, string station) : this(out errorMsg, startDate, endDate, source, false, "")
        {
            this.station = station;
        }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="snow"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, "0.0", "0.0", startDate, endDate, source, local, sfPath)
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
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public Precipitation(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, latitude, longitude, startDate, endDate, source, local, sfPath, "0.0", "NaN")
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
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public Precipitation(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath, string gmtOffset, string tzName)
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
            else if (this.dataSource == "Daymet") { this.cellWidth = 0.01; }
            else { this.cellWidth = 0.0; }
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
                DateTime newStartDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day, 00, 00, 00);
                DateTime newEndDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 00, 00);
                this.startDate = newStartDate;
                this.endDate = newEndDate;
            }
            catch
            {
                errorMsg = "ERROR: Invalid date format. Please provide a date as mm-dd-yyyy or mm/dd/yyyy.";
                return;
            }
            if (DateTime.Compare(this.endDate, DateTime.Today) > 0)   //If endDate is past today's date, endDate is set to 5 days prior to today.
            {
                this.endDate = DateTime.Today.AddDays(-5.0);
            }
            if (DateTime.Compare(this.startDate, this.endDate) > 0)
            {
                errorMsg = "ERROR: Invalid dates entered. Please enter an end date set after the start date.";
                return;
            }
            if (this.dataSource.Contains("NLDAS"))
            {
                DateTime minDate = new DateTime(1979, 01, 02);              //NLDAS data collection start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;                               //start date is set to NLDAS start date
                }
            }
            else if (this.dataSource.Contains("GLDAS"))
            {
                DateTime minDate = new DateTime(2000, 02, 25);              //GLDAS data collection start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;                               //start date is set to GLDAS start date
                }
            }
            else if (this.dataSource.Contains("Daymet"))
            {
                DateTime minDate = new DateTime(1980, 01, 01);              //Daymet dataset start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;
                }
            }
        }

        /// <summary>
        /// Gets precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            double offset = gmtOffset;
            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);

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
            else if (this.shapefilePath != null && this.dataSource.Contains("Daymet"))
            {
                double[] center = gdal.ReturnCentroid(out errorMsg, this.shapefilePath);
                this.latitude = center[0];   
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
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
                this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);         //Gets the GMT offset
                if (errorMsg.Contains("ERROR")) { return null; }
                this.tzName = ts[0].tzName;                                                                     //Gets the Timezone name
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
            if (this.dataSource.Contains("NLDAS") || this.dataSource.Contains("GLDAS"))
            {
                gldas.BeginLDASSequence(out errorMsg, this, "PRECIP", newTS);
            }
            else if (this.dataSource.Contains("Daymet"))
            {
                HMSDaymet.HMSDaymet daymet = new HMSDaymet.HMSDaymet();
                daymet.GetDaymetData(out errorMsg, this, "Precip", newTS);
            }
            else if (this.dataSource.Contains("NCDC"))
            {
                HMSNCDC.HMSNCDC ncdc = new HMSNCDC.HMSNCDC();
                ncdc.BeginNCDCSequence(out errorMsg, this, "NCDC", this.station, newTS);
            }
            if (errorMsg.Contains("ERROR")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            this.jsonData = output.ConstructHMSDataFromTS(out errorMsg, this.ts, "Precipitation", this.dataSource, this.localTime, this.gmtOffset);
            return ts;
        }

        /// <summary>
        /// Gets the precipitaiton data using a shapefile.
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
        /// Get precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public void GetDataSetsObject(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
        }
    }
}