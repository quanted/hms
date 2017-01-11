using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSTimeSeries
{
    public class HMSTimeSeries
    {

        public string timeSeries { get; set; }
        public string metaData { get; set; }
        public string newMetaData { get; set; }
        public double metaLat { get; set; }                 //latitude of the returned data
        public double metaLon { get; set; }                 //longitude of the returned data
        public double metaElev { get; set; }                //elevation of the location returned
        public double cellSize { get; set; }                //cellSize for grid
        public string tzName { get; set; }
        public double gmtOffset { get; set; }
        public double cellCoverage { get; set; }            // % of the NLDAS, GLDAS cell that intersects with the shapefile.
        public double mean { get; set; }

        /// <summary>
        /// Parses the data downloaded, removes meta data and MEAN value.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ParseForData(out string errorMsg, string data)
        {
            errorMsg = "";
            string dataResults = "";
            if (data.Contains("Data"))
            {
                string[] parseData = data.Split(new string[] { "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Contains("MEAN"))
                {
                    string[] parseData2 = parseData[1].Trim().Split(new string[] { "MEAN" }, StringSplitOptions.RemoveEmptyEntries);
                    dataResults = parseData2[0];

                }
                else
                {
                    errorMsg = "Error: Failed to parse data.";
                }
            }
            else
            {
                errorMsg = "Error: Requested data not found.";
                return null;
            }
            return dataResults;
        }

        /// <summary>
        /// Parses the data downloaded, collects the meta data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ParseForMetaData(out string errorMsg, string data)
        {
            errorMsg = "";
            string metaData = "";
            if (data.Contains("Date&Time"))
            {
                string[] parseData = data.Split(new string[] { "Date&Time" }, StringSplitOptions.RemoveEmptyEntries);
                metaData = parseData[0].Trim();
            }
            else
            {
                errorMsg = "Error: Requested data not found.";
                return null;
            }
            return metaData;
        }

        /// <summary>
        /// Parses the meta data for the elevation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetElevation(out string errorMsg, string data)
        {
            errorMsg = "";
            string elevation = "";
            if (data.Contains("elevation[m]"))
            {
                string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("elevation[m]"))
                    {
                        string[] singleLine = lines[i].Split('=');
                        elevation = singleLine[1].Trim();
                    }
                }
            }
            else
            {
                errorMsg = "Error: Unable to find elevation in meta data.";
                return 0;
            }
            return Convert.ToDouble(elevation);
        }

        /// <summary>
        /// Parses the meta data for the latitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetLatitude(out string errorMsg, string data)
        {
            errorMsg = "";
            string latitude = "";
            if (data.Contains("Metadata"))
            {
                string[] metaData = data.Split(new string[] { "Metadata for Requested Time Series:" }, StringSplitOptions.RemoveEmptyEntries);

                if (metaData[1].Contains("lat"))
                {
                    string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("lat"))
                        {
                            string[] singleLine = lines[i].Split('=');
                            latitude = singleLine[1].Trim();
                        }
                    }
                }
                else
                {
                    errorMsg = "Error: Unable to find latitude in meta data.";
                    return 0;
                }
                return Convert.ToDouble(latitude);
            }
            else
            {
                errorMsg = "Error: Metadata was not found.";
                return 0;
            }
        }

        /// <summary>
        /// Parses the meta data for the longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetLongitude(out string errorMsg, string data)
        {
            errorMsg = "";
            string longitude = "";
            if (data.Contains("Metadata"))
            {
                string[] metaData = data.Split(new string[] { "Metadata for Requested Time Series:" }, StringSplitOptions.RemoveEmptyEntries);

                if (metaData[1].Contains("lon"))
                {
                    string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("lon"))
                        {
                            string[] singleLine = lines[i].Split('=');
                            longitude = singleLine[1].Trim();
                        }
                    }
                }
                else
                {
                    errorMsg = "Error: Unable to find longitude in meta data.";
                    return 0;
                }
                return Convert.ToDouble(longitude);
            }
            else
            {
                errorMsg = "Error: Metadata was not found.";
                return 0;
            }
        }

        /// <summary>
        /// Parses the meta data for the elevation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public double GetCellSize(out string errorMsg, string data)
        {
            errorMsg = "";
            string size = "";
            if (data.Contains("dlat"))
            {
                string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("dlat"))
                    {
                        string[] singleLine = lines[i].Split('=');
                        size = singleLine[1].Trim();
                    }
                }
            }
            else
            {
                errorMsg = "Error: Unable to find cell size in meta data.";
                return 0;
            }
            return Convert.ToDouble(size);
        }

        /// <summary>
        /// Sets the class object variables for the provided TimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="data"></param>
        public void SetTimeSeriesVariables(out string errorMsg, HMSTimeSeries ts, string data)
        {
            errorMsg = "";
            this.timeSeries = ts.ParseForData(out errorMsg, data);
            this.metaData = ts.ParseForMetaData(out errorMsg, data);
            this.metaElev = ts.GetElevation(out errorMsg, this.metaData);
            this.metaLat = ts.GetLatitude(out errorMsg, this.metaData);
            this.metaLon = ts.GetLongitude(out errorMsg, this.metaData);
            this.cellSize = ts.GetCellSize(out errorMsg, this.metaData);
            this.newMetaData ="\n" + this.newMetaData + "cell_width[deg]=" + this.cellSize + "\n";
            if (errorMsg.Contains("Error")) { return; }
        }

    }
}
