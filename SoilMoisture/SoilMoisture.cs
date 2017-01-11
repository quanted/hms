﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSSoilMoisture
{
    public class SoilMoisture
    {

        // Class variables which define a SoilMoisture object.
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
        public int[] layersSelected { get; set; }                       // Layers selected is represented by an integer 1-6, 
        public List<HMSTimeSeries.HMSTimeSeries> layers { get; set; }

        public SoilMoisture() { }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="sfPath"></param>
        public SoilMoisture(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath, int[] soilLayers) : this(out errorMsg, "0.0", "0.0", startDate, endDate, source, local, sfPath, soilLayers)
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
        public SoilMoisture(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath, int[] soilLayers)
        {
            errorMsg = "";
            this.gmtOffset = 0.0;
            this.dataSource = source;
            this.localTime = local;
            this.tzName = "GMT";
            if (errorMsg.Contains("Error")) { return; }
            SetDates(out errorMsg, startDate, endDate);
            if (errorMsg.Contains("Error")) { return; }
            ts = new List<HMSTimeSeries.HMSTimeSeries>();
            if (string.IsNullOrWhiteSpace(sfPath))
            {
                this.shapefilePath = null;
                this.latitude = ConvertStringToDouble(out errorMsg, latitude);
                this.longitude = ConvertStringToDouble(out errorMsg, longitude);
            }
            else
            {
                this.shapefilePath = sfPath.Substring(0, sfPath.LastIndexOf('.'));
                this.latitude = 0.0;
                this.longitude = 0.0;
            }
            if (this.dataSource == "NLDAS") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "GLDAS") { this.cellWidth = 0.2500; }
            this.gdal = new HMSGDAL.HMSGDAL();

            this.layersSelected = soilLayers;
            this.layers = new List<HMSTimeSeries.HMSTimeSeries>();
        }

        /// <summary>
        /// Method first checks if the string is numeric then attemps to convert to a double.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private double ConvertStringToDouble(out string errorMsg, string str)
        {
            errorMsg = "";
            double result = 0.0;
            if (Double.TryParse(str, out result))
            {
                try
                {
                    return result = Convert.ToDouble(str);
                }
                catch
                {
                    errorMsg = "Error: Unable to convert string value to double.";
                    return result;
                }
            }
            else
            {
                errorMsg = "Error: Coordinates contain invalid characters.";
                return result;
            }
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
                errorMsg = "Error: Invalid data format. Please provide a date as mm-dd-yyyy or mm/dd/yyyy.";
                return;
            }
            if (DateTime.Compare(this.endDate, DateTime.Today) > 0)   //If endDate is past today's date, endDate is set to 2 days prior to today.
            {
                this.endDate = DateTime.Today.AddDays(-2.0);
            }
            if (DateTime.Compare(this.startDate, this.endDate) > 0)
            {
                errorMsg = "Error: Invalid dates entered. Please enter an end date set after the start date.";
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
        /// Returns the soil moisture level from index.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        private string GetSoilMoistureLayer(out string errorMsg, int layerIndex)
        {
            errorMsg = "";
            if (this.dataSource.Contains("NLDAS"))
            {
                switch (layerIndex)
                {
                    case 0:
                        return "0_10";  // 0-10cm layer
                    case 1:
                        return "10_40"; // 10-40cm layer
                    case 2:
                        return "40_100"; // 40-100cm layer
                    case 3:
                        return "100_200"; // 100-200cm layer
                    case 4:
                        return "0_100"; // 0-100cm layer
                    case 5:
                        return "0_200"; // 0-200cm layer
                    default:
                        errorMsg = "Error: Soil layer " + layerIndex + " is invalid.";
                        return null;
                }
            }
            else
            {
                switch (layerIndex)
                {
                    case 0:
                        return "0_10";  // 0-10cm layer
                    case 1:
                        return "10_40"; // 10-40cm layer
                    case 2:
                        return "40_100"; // 40-100cm layer
                    case 3:
                        return "0_100"; // 0-100cm layer
                    default:
                        errorMsg = "Error: Soil layer " + layerIndex + " is invalid.";
                        return null;
                }
            }   
        }

        /// <summary>
        /// Gets soil moisture data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            double offset = gmtOffset;
            string data = "";
            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);
            if (this.shapefilePath != null)
            {
                bool sourceNLDAS = true;
                if (this.dataSource.Contains("GLDAS")) { sourceNLDAS = false; }
                double[] center = gldas.DetermineReturnCoordinates(out errorMsg, gdal.ReturnCentroid(out errorMsg, this.shapefilePath), sourceNLDAS);
                this.latitude = center[0];   // coordinate values for LandSurfaceFlow objects are taken from the centroid of the shapefile.
                this.longitude = center[1];
                gdal.CellAreaInShapefile(out errorMsg, center, this.cellWidth);
            }

            if (this.localTime == true && offset == 0.0)
            {
                this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);    //Gets the GMT offset
                if (errorMsg.Contains("Error")) { return null; }
                this.tzName = ts[0].tzName;              //Gets the Timezone name
                if (errorMsg.Contains("Error")) { return null; }
                this.startDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.startDate, true);
                this.endDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.endDate, false);
            }

            int totalPoints = 0;

            if (this.shapefilePath != null)
            {
                totalPoints = gdal.coordinatesInShapefile.Count;

                for (int i = 0; i < totalPoints; i++)
                {
                    layers = new List<HMSTimeSeries.HMSTimeSeries>();
                    for (int j = 0; j < layersSelected.Length; j++)
                    {
                        string layerName = GetSoilMoistureLayer(out errorMsg, layersSelected[j]);
                        if (errorMsg.Contains("Error")) { return null; }

                        newTS = new HMSTimeSeries.HMSTimeSeries();
                        layers.Add(newTS);

                        if (dataSource.Contains("NLDAS"))
                        {
                            data = gldas.LDAS(out errorMsg, gdal.coordinatesInShapefile[i].Item1, gdal.coordinatesInShapefile[i].Item2, startDate, endDate, "NLDAS_" + layerName + "_Soil_Moisture", newTS, shapefilePath);
                            if (errorMsg.Contains("Error")) { return null; }
                        }
                        else if (dataSource.Contains("GLDAS"))
                        {
                            data = gldas.LDAS(out errorMsg, gdal.coordinatesInShapefile[i].Item1, gdal.coordinatesInShapefile[i].Item2, startDate, endDate, "GLDAS_" + layerName + "_Soil_Moisture", newTS, shapefilePath);
                            if (errorMsg.Contains("Error")) { return null; }
                        }
                        else
                        {
                            errorMsg = "Error: Invalid data source selected.";
                            return null;
                        }
                        layers[j].SetTimeSeriesVariables(out errorMsg, newTS, data);
                        if (errorMsg.Contains("Error")) { return null; }
                    }
                    string result = SetSoilMoistureTimeSeries(out errorMsg, ts.Count);
                    if (i != 0)
                    {
                        newTS = new HMSTimeSeries.HMSTimeSeries();
                        ts.Add(newTS);
                    }
                    ts[i].SetTimeSeriesVariables(out errorMsg, layers[0], result);
                    
                    //Setting metadata
                    for (int j = 0; j < layersSelected.Length; j++)
                    {
                        ts[i].newMetaData = String.Concat(ts[i].newMetaData, "layer", j+1, "_Name=", GetSoilMoistureLayer(out errorMsg, layersSelected[j]), "cm\n");
                    }
                    ts[i].newMetaData = String.Concat(ts[i].newMetaData, "precentInCell=", gdal.areaPrecentInGeometry[i], "\nareaInCell=", gdal.areaGeometryIntersection[i], "\n");
                    ts[i].newMetaData += "\n";
                    ts[i].newMetaData += @"  Date\Time" + "\t";
                    layers.Clear();
                }
            }
            else
            {
                for (int j = 0; j < layersSelected.Length; j++)
                {
                    string layerName = GetSoilMoistureLayer(out errorMsg, layersSelected[j]);
                    if (errorMsg.Contains("Error")) { return null; }

                    newTS = new HMSTimeSeries.HMSTimeSeries();
                    layers.Add(newTS);

                    if (dataSource.Contains("NLDAS"))
                    {
                        data = gldas.LDAS(out errorMsg, latitude, longitude, startDate, endDate, "NLDAS_" + layerName + "_Soil_Moisture", newTS, shapefilePath);
                        if (errorMsg.Contains("Error")) { return null; }
                    }
                    else if (dataSource.Contains("GLDAS"))
                    {
                        data = gldas.LDAS(out errorMsg, latitude, longitude, startDate, endDate, "GLDAS_" + layerName + "_Soil_Moisture", newTS, shapefilePath);
                        if (errorMsg.Contains("Error")) { return null; }
                    }
                    else
                    {
                        errorMsg = "Error: Invalid data source selected.";
                        return null;
                    }
                    layers[j].SetTimeSeriesVariables(out errorMsg, newTS, data);
                    if (errorMsg.Contains("Error")) { return null; }
                    ts[0].newMetaData += "layer" + (j + 1) + " Name=" + layerName + "cm\n";
                }
                string result = SetSoilMoistureTimeSeries(out errorMsg, 0);
                ts[0].SetTimeSeriesVariables(out errorMsg, layers[0], result);
                if (errorMsg.Contains("Error")) { return null; }
            }
            return ts;
        }

        /// <summary>
        /// Gets the soil moisture data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string GetDataSetsString(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            string combinedData = output.CombineDatasetsAsJSON(out errorMsg, ts, "SoilMoisture", this.dataSource, this.localTime, this.gmtOffset);
            if (errorMsg.Contains("Error")) { return null; }
            return combinedData;
        }

        /// <summary>
        /// Takes timeseries data from layers List and concatenates all layer data together into a single timeseries set as ts[].timeSeries.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string SetSoilMoistureTimeSeries(out string errorMsg, int index)
        {
            errorMsg = "";
            try
            {
                List<string[]> layerData = new List<string[]>();
                for (int i = 0; i < layers.Count; i++)
                {
                    layerData.Add(layers[i].timeSeries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
                }

                string timeSeries = "";
                for (int i = 0; i < layerData[0].Length; i++)
                {
                    string date = "";
                    string data = "";

                    for (int j = 0; j < layers.Count; j++)
                    {
                        string[] layerLineData = layerData[j][i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (j == 0)
                        {
                            date = layerLineData[0] + " " + layerLineData[1];
                        }
                        data += layerLineData[2] + " ";
                    }
                    timeSeries = String.Concat(timeSeries, date, " ", data, "\n");

                }
                return timeSeries = String.Concat(layers[0].metaData, "\n\n     Date&Time     Data\n", timeSeries, "MEAN");
            }
            catch (Exception ex)
            {
                errorMsg = "Error: " + ex;
                return null;
            }
        }

    }
}
