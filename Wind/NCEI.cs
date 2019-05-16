﻿using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Wind
{
    public class NCEI
    {
        /// <summary>
        /// Makes the GetData call to the base NCEI class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            Data.Source.NCDC ncdc = new Data.Source.NCDC();
            ITimeSeriesOutput ncdcOutput = output;

            // Make call to get station metadata and add to output.Metadata
            if (!input.Geometry.GeometryMetadata.ContainsKey("token"))
            {
                errorMsg = "ERROR: No NCEI token provided. Please provide a valid NCEI token.";
                return null;
            }
            string station_url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations/";
            if (!input.Geometry.GeometryMetadata.ContainsKey("stationID") && input.Geometry.StationID == null)
            {
                errorMsg = "ERROR: No NCEI stationID provided. Please provide a valid NCEI stationID.";
                return null;
            }
            else if (input.Geometry.StationID != null)
            {
                ncdcOutput.Metadata = SetMetadata(out errorMsg, "ncei", ncdc.GetStationDetails(out errorMsg, station_url, input.Geometry.StationID, input.Geometry.GeometryMetadata["token"]));
            }
            else
            {
                ncdcOutput.Metadata = SetMetadata(out errorMsg, "ncei", ncdc.GetStationDetails(out errorMsg, station_url, input.Geometry.GeometryMetadata["stationID"], input.Geometry.GeometryMetadata["token"]));
            }
            ncdcOutput.Metadata.Add("ncei", input.TemporalResolution);
            ncdcOutput.Metadata.Add("ncei_units", "m/s");

            // Data aggregation takes place within ncdc.GetData
            Dictionary<string, double> data = ncdc.GetData(out errorMsg, "AWND", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Set resulting data to output.Data
            ncdcOutput.Data = ConvertDict(out errorMsg, input.DataValueFormat, data);
            if (errorMsg.Contains("ERROR")) { return null; }

            ncdcOutput.DataSource = "ncei";
            ncdcOutput.Dataset = "Wind";
            ncdcOutput.Metadata.Add("column_1", "Date");
            ncdcOutput.Metadata.Add("column_2", "Wind");
            return ncdcOutput;
        }

        /// <summary>
        /// Converts the returned Dictionary into the appropriate format for ITimeSeriesOutput.Data
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> ConvertDict(out string errorMsg, string dataFormat, Dictionary<string, double> data)
        {
            errorMsg = "";

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var key in data)
            {
                result.Add(key.Key.ToString(), new List<string>() { data[key.Key.ToString()].ToString(dataFormat) });
            }
            return result;
        }

        /// <summary>
        /// Adds source_ to the metadata keys.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="source"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string source, Dictionary<string, string> metadata)
        {
            errorMsg = "";
            Dictionary<string, string> newMeta = new Dictionary<string, string>();
            foreach (var ele in metadata)
            {
                newMeta.Add(source + "_" + ele.Key, ele.Value);
            }
            return newMeta;
        }

        /// <summary>
        /// Calls the function in Data.Source.NCDC that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.NCDC.CheckStatus("Wind", input);
        }
    }
}