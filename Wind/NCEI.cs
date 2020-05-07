using System;
using System.Collections.Generic;
using Data;
using Data.Source;

namespace Wind
{
    public class NCEIWind
    {
        public string DATE { get; set; }
        public string STATION { get; set; }
        public double AWND { get; set; }
        public string AWND_ATTRIBUTES { get; set; }
    }
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

            ITimeSeriesOutput ncdcOutput = output;

            // Make call to get station metadata and add to output.Metadata
            if (input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                input.Geometry.StationID = input.Geometry.GeometryMetadata["stationID"];
            }
            else if (input.Geometry.StationID != null && !input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                input.Geometry.GeometryMetadata.Add("stationID", input.Geometry.StationID);
            }
            if (!input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                errorMsg = "ERROR: No NCEI stationID provided. Please provide a valid NCEI stationID.";
                return null;
            }
            string station_url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations/";
            ncdcOutput.Metadata = SetMetadata(out errorMsg, "ncei", Data.Source.NCEI<NCEIWind>.GetStationDetails(out errorMsg, station_url, input.Geometry.StationID, input.Geometry.GeometryMetadata["token"]));
            ncdcOutput.Metadata.Add("ncei", input.TemporalResolution);
            ncdcOutput.Metadata.Add("ncei_units", "m/s");

            // Data aggregation takes place within ncdc.GetData
            Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
            if (input.Geometry.StationID.Contains("GHCND"))
            {
                Data.Source.NCEI<NCEIWind> ncei = new Data.Source.NCEI<NCEIWind>();
                List<NCEIWind> preData = ncei.GetData(out errorMsg, "AWND", input);
                data = this.ParseData(out errorMsg, preData, input.DateTimeSpan.DateTimeFormat);
            }

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
        private Dictionary<string, List<string>> ConvertDict(out string errorMsg, string dataFormat, Dictionary<string, List<double>> data)
        {
            errorMsg = "";

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, List<double>> d in data)
            {
                List<string> dataList = new List<string>()
                {
                    d.Value[0].ToString(dataFormat),
                };
                result.Add(d.Key.ToString(), dataList);
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
        /// Parse the resulting NCEIWind list into a dictionary of timestamps and double values
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="rawdata"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        protected Dictionary<string, List<double>> ParseData(out string errorMsg, List<NCEIWind> rawdata, string dateFormat)
        {
            errorMsg = "";
            Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(rawdata[0].DATE, out newDate);
            double windSum = 0.0;
            int n = 0;
            for (int i = 0; i <= rawdata.Count - 1; i++)
            {
                DateTime.TryParse(rawdata[i].DATE, out iDate);
                if (iDate.Date == newDate.Date)
                {
                    double wind = NCEI<NCEIWind>.AttributeCheck(out errorMsg, rawdata[i].AWND, rawdata[i].AWND_ATTRIBUTES);
                    if (wind < 0)
                    {
                        windSum = wind;
                    }
                    else
                    {
                        windSum += wind;
                    }                   
                    if (errorMsg.Contains("ERROR")) { return null; }
                    n += 1;
                }
                else
                {
                    double windAvg = windSum / n;
                    newDate = newDate.AddHours(-newDate.Hour);
                    data.Add(newDate.ToString(dateFormat), new List<double>() {windAvg });
                    newDate = iDate;
                    windSum = NCEI<NCEIWind>.AttributeCheck(out errorMsg, rawdata[i].AWND, rawdata[i].AWND_ATTRIBUTES);
                    if (i == rawdata.Count - 1)
                    {
                        iDate = iDate.AddHours(-iDate.Hour);
                        data.Add(iDate.ToString(dateFormat), new List<double>() { windSum });
                    }
                    if (errorMsg.Contains("ERROR")) { return null; }
                    n = 1;
                }
            }
            return data;
        }


    }
}
