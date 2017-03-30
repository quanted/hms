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
        public Dictionary<DateTime, double> timeSeriesDict { get; set; }


        /// <summary>
        /// Parses the data downloaded, removes meta data and MEAN value.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private string ParseForData(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string dataResults = "";
        //    if (data.Contains("Data"))
        //    {
        //        string[] parseData = data.Split(new string[] { "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
        //        if (data.Contains("MEAN"))
        //        {
        //            string[] parseData2 = parseData[1].Trim().Split(new string[] { "MEAN" }, StringSplitOptions.RemoveEmptyEntries);
        //            dataResults = parseData2[0];
        //        }
        //        else
        //        {
        //            errorMsg = "ERROR: Failed to parse data.";
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Requested data not found.";
        //        return null;
        //    }
        //    return dataResults;
        //}

        /// <summary>
        /// Parses the Daymet data downloaded and returns the timeseries data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ParseForDaymetData(out string errorMsg, string data)
        {
            errorMsg = "";
            StringBuilder builder = new StringBuilder();
            if (data.Contains("year,yday,prcp (mm/day)"))
            {
                string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)" }, StringSplitOptions.RemoveEmptyEntries);
                string[] lines = splitData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] lineData = lines[i].Split(',');
                    builder.Append(lineData[0] + " " + lineData[1] + " " + lineData[2] + "\n");
                }
                return builder.ToString();
            }
            else if (data.Contains("year,yday,tmax (deg c),tmin (deg c)"))
            {
                string[] splitData = data.Split(new string[] { "year,yday,tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
                string[] lines = splitData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] lineData = lines[i].Split(',');
                    builder.Append(lineData[0] + " " + lineData[1] + " " + lineData[2] + "," + lineData[3] +" \n");
                }
                return builder.ToString();
            }
            else
            {
                errorMsg = "ERROR: Unable to parse out Daymet data.";
                return null;
            }
        }

        /// <summary>
        /// Parses the data downloaded, collects the meta data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private string ParseForMetaData(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string metaData = "";
        //    if (data.Contains("Date&Time"))
        //    {
        //        string[] parseData = data.Split(new string[] { "Date&Time" }, StringSplitOptions.RemoveEmptyEntries);
        //        metaData = parseData[0].Trim();
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Requested data not found.";
        //        return null;
        //    }
        //    return metaData;
        //}

        /// <summary>
        /// Parses the Daymet data downloaded and returns the metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ParseForDaymetMetaData(out string errorMsg, string data)
        {
            errorMsg = "";
            if (data.Contains("year,yday,prcp (mm/day)"))
            {
                string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)" }, StringSplitOptions.RemoveEmptyEntries);
                string metaData = splitData[0];
                metaData += "column_1: year yday\n";
                metaData += "column_2: prcp (mm/day)";
                return metaData;
            }
            else if (data.Contains("year,yday,tmax (deg c),tmin (deg c)"))
            {
                string[] splitData = data.Split(new string[] { "year,yday,tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
                string metaData = splitData[0];
                metaData += "column_1: year yday\n";
                metaData += "column_2: tmax (deg c), tmin (deg c)";
                return metaData;
             }
            else
            {
                errorMsg = "ERROR: Unable to parse out Daymet metadata.";
                return null;
            }
        }

        /// <summary>
        /// Parses the meta data for the elevation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private double GetElevation(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string elevation = "";
        //    if (data.Contains("elevation"))
        //    {
        //        string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            if (lines[i].Contains("elevation"))
        //            {
        //                string[] singleLine = lines[i].Split('=');
        //                elevation = singleLine[1].Trim();
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Unable to find elevation in meta data.";
        //        return 0;
        //    }
        //    return Convert.ToDouble(elevation);
        //}

        /// <summary>
        /// Parses the meta data for the latitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private double GetLatitude(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string latitude = "";
        //    if (data.Contains("Metadata"))
        //    {
        //        string[] metaData = data.Split(new string[] { "Metadata for Requested Time Series:" }, StringSplitOptions.RemoveEmptyEntries);

        //        if (metaData[1].Contains("lat="))
        //        {
        //            string[] lines = metaData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //            for (int i = 0; i < lines.Length; i++)
        //            {
        //                if (lines[i].Contains("lat="))
        //                {
        //                    string[] singleLine = lines[i].Split('=');
        //                    latitude = singleLine[1].Trim();
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            errorMsg = "ERROR: Unable to find latitude in meta data.";
        //            return 0;
        //        }
        //        return Convert.ToDouble(latitude);
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Metadata was not found.";
        //        return 0;
        //    }
        //}

        /// <summary>
        /// Parses the meta data for the longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private double GetLongitude(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string longitude = "";
        //    if (data.Contains("Metadata"))
        //    {
        //        string[] metaData = data.Split(new string[] { "Metadata for Requested Time Series:" }, StringSplitOptions.RemoveEmptyEntries);

        //        if (metaData[1].Contains("lon="))
        //        {
        //            string[] lines = metaData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //            for (int i = 0; i < lines.Length; i++)
        //            {
        //                if (lines[i].Contains("lon="))
        //                {
        //                    string[] singleLine = lines[i].Split('=');
        //                    longitude = singleLine[1].Trim();
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            errorMsg = "ERROR: Unable to find longitude in meta data.";
        //            return 0;
        //        }
        //        return Convert.ToDouble(longitude);
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Metadata was not found.";
        //        return 0;
        //    }
        //}

        /// <summary>
        /// Parses the meta data for the elevation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //private double GetCellSize(out string errorMsg, string data)
        //{
        //    errorMsg = "";
        //    string size = "";
        //    if (data.Contains("dlat"))
        //    {
        //        string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            if (lines[i].Contains("dlat"))
        //            {
        //                string[] singleLine = lines[i].Split('=');
        //                size = singleLine[1].Trim();
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        errorMsg = "ERROR: Unable to find cell size in meta data.";
        //        return 0;
        //    }
        //    return Convert.ToDouble(size);
        //}

        /// <summary>
        /// Parses through the Daymet metadata to determine the timeseries variable values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        private void SetDaymetVariables(out string errorMsg, string data)
        {
            errorMsg = "";
            try
            {
                string[] lines = data.Split(new string[] { "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("Elevation"))
                    {
                        string[] singleLine = lines[i].Split(':', ' ');
                        this.metaElev = Convert.ToDouble(singleLine[2].Trim());
                    }
                    else if (lines[i].Contains("Latitude"))
                    {
                        string[] singleLine = lines[i].Split(':');
                        this.metaLat = Convert.ToDouble(singleLine[1].Trim());
                    }
                    else if (lines[i].Contains("Longitude"))
                    {
                        string[] singleLine = lines[i].Split(':');
                        this.metaLon = Convert.ToDouble(singleLine[1].Trim());
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to parse metadata.";
                return;
            }
        }

        /// <summary>
        /// Sets the class object variables for the provided TimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="data"></param>
        public void SetTimeSeriesVariables(out string errorMsg, HMSTimeSeries ts, string data, string source)
        {
            errorMsg = "";
            if (source.Contains("NLDAS") || source.Contains("GLDAS"))           //Data parsing is unique to the datasource.
            {
                SetLDASTimeSeries(out errorMsg, ts, data);
                if (errorMsg.Contains("ERROR")) { return; }
            }
            else if (source.Contains("Daymet"))
            {
                this.timeSeries = ts.ParseForDaymetData(out errorMsg, data);
                this.metaData = ts.ParseForDaymetMetaData(out errorMsg, data);
                SetDaymetVariables(out errorMsg, this.metaData);
                if (errorMsg.Contains("ERROR")) { return; }
            }
        }

        /// <summary>
        /// Parses LDAS return data and assigns to timeseries, metadata, metaLat, metaLon, and elevation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="data"></param>
        private void SetLDASTimeSeries(out string errorMsg, HMSTimeSeries ts, string data)
        {
            errorMsg = "";

            if (data.Contains("Data"))
            {
                string[] splitData = data.Split(new string[] { "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitData[1].Contains("MEAN"))              // Collect LDAS data 
                {
                    this.timeSeries = splitData[1].Substring(0, splitData[1].IndexOf("MEAN")).Trim();
                }
                else
                {
                    errorMsg = "ERROR: LDAS data was not found.";
                }

                if (splitData[0].Contains("Date&Time"))         // Collect LDAS metadata
                {
                    this.metaData = splitData[0].Trim();
                    this.newMetaData = "\n" + this.newMetaData;
                    string[] splitMeta = splitData[0].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = splitMeta.Length - 1; i != 0; i--)
                    {
                        if(splitMeta[i].Contains("="))
                        {
                            string[] metaLine = splitMeta[i].Split('=');
                            switch (metaLine[0])
                            {
                                case "elevation[m]":
                                    this.metaElev = Convert.ToDouble(metaLine[1]);
                                    break;
                                case "lat":
                                    this.metaLat = Convert.ToDouble(metaLine[1]);
                                    break;
                                case "lon":
                                    this.metaLon = Convert.ToDouble(metaLine[1]);
                                    break;
                                case "dlat":
                                    this.cellSize = Convert.ToDouble(metaLine[1]);
                                    this.newMetaData += "cell_width[deg]=" + this.cellSize + "\n";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                errorMsg = "ERROR: LDAS data was not found.";
            }
        }

        public void ConvertTSDictToTS(out string errorMsg)
        {
            errorMsg = "";
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < this.timeSeriesDict.Count; i++)
            {
                DateTime date = this.timeSeriesDict.Keys.ElementAt(i);
                st.Append(date.ToString("yyyy-MM-dd HH") + "Z " + this.timeSeriesDict[date].ToString("0.0000####E+00") + "\n");
            }
            this.timeSeries = st.ToString();
        }
    }
}
