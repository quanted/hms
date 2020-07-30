using Data;
using Data.Source;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Temperature
{
    public class NCEITemperature
    {
        public string DATE { get; set; }
        public string STATION { get; set; }
        public double TMAX { get; set; }
        public string TMAX_ATTRIBUTES { get; set; }
        public double TMIN { get; set; }
        public string TMIN_ATTRIBUTES { get; set; }
    }

    /// <summary>
    /// Base temperature ncei class.
    /// </summary>
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

            ITimeSeriesOutput nceiOutput = output;

            // Make call to get station metadata and add to output.Metadata
            if (!input.Geometry.GeometryMetadata.ContainsKey("token"))
            {
                input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }
            //    errorMsg = "ERROR: No NCEI token provided. Please provide a valid NCEI token.";
            //    return null;
            //}
            string station_url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations/";
            if(input.Geometry.StationID != null && !input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                input.Geometry.GeometryMetadata.Add("stationID", input.Geometry.StationID);
            }
            else if (input.Geometry.StationID == null && input.Geometry.GeometryMetadata.ContainsKey(("stationID")))
            {
                input.Geometry.StationID = input.Geometry.GeometryMetadata["stationID"];
            }

            if (!input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                errorMsg = "ERROR: No NCEI stationID provided. Please provide a valid NCEI stationID.";
                return null;
            }
            nceiOutput.Metadata = SetMetadata(out errorMsg, "ncei", NCEI<NCEITemperature>.GetStationDetails(out errorMsg, station_url, input.Geometry.GeometryMetadata["stationID"], input.Geometry.GeometryMetadata["token"]));
            nceiOutput.Metadata.Add("ncei", input.TemporalResolution);
            nceiOutput.Metadata.Add("ncei_units", "C");

            // Data aggregation takes place within ncdc.GetData

            Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
            if (input.Geometry.StationID.Contains("GHCND"))
            {
                NCEI<NCEITemperature> ncei = new NCEI<NCEITemperature>();
                List<NCEITemperature> preData = ncei.GetData(out errorMsg, "TMIN,TMAX", input);
                data = this.ParseData(out errorMsg, preData, input.DateTimeSpan.DateTimeFormat);
            }

            // Set resulting data to output.Data
            nceiOutput.Data = this.ConvertDict(out errorMsg, input.DataValueFormat, data);
            if (errorMsg.Contains("ERROR")) { return null; }

            nceiOutput.DataSource = "ncei";
            nceiOutput.Dataset = "Temperature";
            nceiOutput.Metadata.Add("column_1", "Date");
            if (input.TemporalResolution == "monthly")
            {
                nceiOutput.Data = MonthlyValues(out errorMsg, nceiOutput, input, "all");
                nceiOutput.Metadata.Add("column_2", "Avg Max Temp");
                nceiOutput.Metadata.Add("column_3", "Avg Min Temp");
                nceiOutput.Metadata.Add("column_4", "Avg Temp");
            }
            else
            {
                nceiOutput.Metadata.Add("column_2", "Max Temp");
                nceiOutput.Metadata.Add("column_3", "Min Temp");
                nceiOutput.Metadata.Add("column_4", "Avg Temp");
                nceiOutput.Metadata.Add("disclaimer_column_4", "Average temperature is the midrange of the tmax and tmin values.");
            }
            return nceiOutput;
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
            foreach(KeyValuePair<string, List<double>> d in data)
            {
                List<string> dataList = new List<string>()
                {
                    d.Value[0].ToString(dataFormat),
                    d.Value[1].ToString(dataFormat),
                    ((d.Value[0] + d.Value[1])/2).ToString(dataFormat)
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
        protected Dictionary<string, List<double>> ParseData(out string errorMsg, List<NCEITemperature> rawdata, string dateFormat)
        {
            errorMsg = "";
            Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(rawdata[0].DATE, out newDate);
            double tmaxSum = 0.0;
            double tminSum = 0.0;
            int n = 0;
            for (int i = 0; i <= rawdata.Count - 1; i++)
            {
                DateTime.TryParse(rawdata[i].DATE, out iDate);
                if (iDate.Date == newDate.Date)
                {
                    double tmax = NCEI<NCEITemperature>.AttributeCheck(out errorMsg, rawdata[i].TMAX, rawdata[i].TMAX_ATTRIBUTES);
                    double tmin = NCEI<NCEITemperature>.AttributeCheck(out errorMsg, rawdata[i].TMIN, rawdata[i].TMIN_ATTRIBUTES);
                    if (tmax < 0)
                    {
                        tmaxSum = tmax;
                    }
                    else
                    {
                        tmaxSum += tmax;
                    }
                    if (tmin < 0)
                    {
                        tminSum = tmin;
                    }
                    else
                    {
                        tminSum += tmin;
                    }
                    if (errorMsg.Contains("ERROR")) { return null; }
                    n += 1;
                }
                else
                {
                    double tmaxAvg = tmaxSum / n;
                    double tminAvg = tminSum / n;
                    newDate = newDate.AddHours(-newDate.Hour);
                    data.Add(newDate.ToString(dateFormat), new List<double>() { tmaxAvg, tminAvg });
                    newDate = iDate;
                    tmaxSum = NCEI<NCEITemperature>.AttributeCheck(out errorMsg, rawdata[i].TMAX, rawdata[i].TMAX_ATTRIBUTES);
                    tminSum = NCEI<NCEITemperature>.AttributeCheck(out errorMsg, rawdata[i].TMIN, rawdata[i].TMIN_ATTRIBUTES);
                    if (i == rawdata.Count - 1)
                    {
                        iDate = iDate.AddHours(-iDate.Hour);
                        data.Add(iDate.ToString(dateFormat), new List<double>() { tmaxAvg, tminAvg });
                    }
                    if (errorMsg.Contains("ERROR")) { return null; }
                    n = 1;
                }
            }
            return data;
        }


        /// <summary>
        /// Gets monthly temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, string type)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double avgSum = 0.0;
            double lowSum = 0.0;
            double hiSum = 0.0;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    double average = avgSum / dayIndex;
                    double lAverage = lowSum / dayIndex;
                    double hAverage = hiSum / dayIndex;

                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (hAverage).ToString(input.DataValueFormat),
                                    (lAverage).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                    );

                    avgSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    hiSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    lowSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                    iDate = date;
                    dayIndex = 1;
                }
                else
                {
                    avgSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    hiSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    lowSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);

                    dayIndex++;
                }
            }
            return tempData;
        }

    }

}
