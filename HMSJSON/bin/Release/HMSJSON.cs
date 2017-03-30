using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Globalization;

namespace HMSJSON
{
    public class HMSJSON
    {
        //public string timeseriesJSON { get; set; }
        //public string metadataJSON { get; set; }
        //public string datasetJSON { get; set; }
        //public string sourceJSON { get; set; }
        //public string newMetadataJSON { get; set; }
        //private Dictionary<string, string> metaDictionary { get; set; }
        
        public struct HMSData
        {
            public string dataset;
            public string source;
            public Dictionary<string, string> metadata;
            public Dictionary<string, List<string>> data;
        };

        /// <summary>
        /// Returns a space separated list of data as a json format string. OBSOLETE
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        //public string ConvertDataToJSON(out string errorMsg, string data, string dataset)
        //{
        //    errorMsg = "";
        //    string[] dataLine = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //    string jsonString = "";

        //    jsonString = String.Concat("{\"", dataset,"\":[");

        //    for (int i = 0; i < dataLine.Length; i++)
        //    {
        //        string[] dataArray = dataLine[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        //        //Explicitly stating data types
        //        //jsonString += "{\"date/hour\":\"" + dataArray[0] + " " + dataArray[1] + "\",";
        //        //jsonString += "\"data\":\"" + dataArray[2] + "\"}";

        //        //Implicitly stating data types
        //        jsonString = String.Concat(jsonString, "{\"", dataArray[0], " ", dataArray[1], "\":\"", dataArray[2], "\"}");
        //        //jsonString += @"{""" + dataArray[0] + " " + dataArray[1] + @""":""" + dataArray[2] + @"""}";

        //        if (i+3 != dataLine.Length)
        //        {
        //            jsonString += @",";
        //        }
        //        i += 2;
        //    }
        //    jsonString += @"]}";
        //    int count = Encoding.ASCII.GetCharCount(Encoding.ASCII.GetBytes(jsonString));
        //    return jsonString;
        //}

        /// <summary>
        /// Returns a single string containing all the individual timeseries data with their metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        //public string CombineDatasetsAsJSON(out string errorMsg, List<HMSTimeSeries.HMSTimeSeries> ts, string dataset, string source, bool localtime, double gmtOffset)
        //{
        //    errorMsg = "";
        //    //string data = "";
        //    StringBuilder stBuilder = new StringBuilder();
        //    for (int i = 0; i < ts.Count; i++)
        //    {
        //        // "DataSetBlock" is used to parse the data on the client side, where it is saved as individual files.
        //        string jsonString = GetJSONString(out errorMsg, ts[i].timeSeries, ts[i].newMetaData, ts[i].metaData, dataset, source, localtime, gmtOffset);
        //        stBuilder.Append("DataSetBlock");
        //        stBuilder.Append(jsonString);
        //    }
        //    return stBuilder.ToString();
        //}

        /// <summary>
        /// Generates a HMSData object from the argument parameters and converts the object into a json string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="timeseries"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <param name="dataset"></param>
        /// <param name="source"></param>
        /// <param name="localtime"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        public string GetJSONString(out string errorMsg, string timeseries, string newMetaData, string metadata, string dataset, string source, bool localtime, double gmtOffset, double coverage)
        {
            errorMsg = "";
            HMSData output = new HMSData();
            output.dataset = dataset;
            output.source = source;
            if (source.Contains("NLDAS") || source.Contains("GLDAS"))
            {
                //output.metadata = SetHMSDataMetaData(out errorMsg, newMetaData, metadata);
                output.metadata = SetHMSDataMetaData2(out errorMsg, newMetaData, metadata, source, 1);
                output.data = SetHMSDataTS(out errorMsg, timeseries, source, dataset, localtime, gmtOffset, coverage);
            }
            else if (source.Contains("Daymet"))
            {
                output.metadata = SetHMSDaymetMetaData(out errorMsg, newMetaData, metadata);
                output.data = SetHMSDaymetDataTS(out errorMsg, timeseries);
            }
            else
            {
                errorMsg = "ERROR: Unable to create JSON from data.";
                return null;
            }
            return JsonConvert.SerializeObject(output);
        }

        //TODO: Create a single SetHMSMetadata method
        /// <summary>
        /// Creates a dictionary from the input metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        //private Dictionary<string, string> SetHMSDataMetaData(out string errorMsg, string newMetaData, string metadata, string source)
        //{
        //    errorMsg = "";
        //    Dictionary<string, string> metaDict = new Dictionary<string, string>();

        //    string[] keys = ConfigurationManager.AppSettings["metadataConfig"].ToString().Split(',');
        //    string[] metaLines = metadata.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = metaLines.Length - 1; i > 0; i--)
        //    {
        //        string[] metadataLineArray = metaLines[i].Split(new char[] { '=' }, 2);
        //        if (keys.Contains(metadataLineArray[0]) && !metaDict.ContainsKey(metadataLineArray[0]) && !metaDict.Keys.Contains(metadataLineArray[0] + "[GMT]"))
        //        {
        //            if (metadataLineArray[0].Contains("begin_time") || metadataLineArray[0].Contains("end_time"))
        //            {
        //                metaDict.Add(source + "_" + metadataLineArray[0] + "[GMT]" , metadataLineArray[1].Trim());
        //            }
        //            else
        //            {
        //                metaDict.Add(source + "_" + metadataLineArray[0], metadataLineArray[1].Trim());
        //            }
        //        }
        //    }
        //    string[] dataLines = newMetaData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < dataLines.Length; i++)
        //    {
        //        if (dataLines[i].Contains("="))
        //        {
        //            string[] line = dataLines[i].Split(new char[] { '=' }, 2);
        //            if (!metaDict.ContainsKey(line[0]))
        //            {
        //                metaDict.Add(source + "_" + line[0], line[1].Trim());
        //            }
        //        }
        //    }
        //    return metaDict;
        //}

        /// <summary>
        /// Creates a dictionary from the input metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetHMSDataMetaData2(out string errorMsg, string newMetaData, string metadata, string source, int index)
        {
            errorMsg = "";
            Dictionary<string, string> metaDict = new Dictionary<string, string>();
            if (index > 1)
            {
                source = source + "_" + index.ToString();
            }
            //string[] keys = ConfigurationManager.AppSettings["metadataConfig"].ToString().Split(',');
            string[] metaLines = metadata.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = metaLines.Length - 1; i > 0; i--)
            {
                string[] metadataLineArray = metaLines[i].Split(new char[] { '=' }, 2);
                if (!metaDict.ContainsKey(source + "_" + metadataLineArray[0]) && !metaDict.ContainsKey(metadataLineArray[0]) && metadataLineArray.Length > 1)
                {
                    try
                    {
                        if (metadataLineArray[0].Contains("begin_time") || metadataLineArray[0].Contains("end_time"))
                        {
                            metaDict.Add(source + "_" + metadataLineArray[0] + "[GMT]", metadataLineArray[1].Trim());
                        }
                        else
                        {
                            metaDict.Add(source + "_" + metadataLineArray[0], metadataLineArray[1].Trim());
                        }
                    }
                    catch { }
                }
            }
            string[] dataLines = newMetaData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < dataLines.Length; i++)
            {
                if (dataLines[i].Contains("="))
                {
                    string[] line = dataLines[i].Split(new char[] { '=' }, 2);
                    if (!metaDict.ContainsKey(line[0]))
                    {
                        metaDict.Add(source + "_" + line[0], line[1].Trim());
                    }
                }
            }
            return metaDict;
        }

        /// <summary>
        /// Creates the timeseries dictionary containing the date/hour as the key and an array for the data values, depending on the total number of datasets provided.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="timeseries"></param>
        /// <param name="localtime"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetHMSDataTS(out string errorMsg, string timeseries, string dataset, string source, bool localtime, double gmtOffset, double coverage)
        {
            errorMsg = "";
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<string> timestepData;
            string[] tsLines = timeseries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            HMSGDAL.HMSGDAL gdal = new HMSGDAL.HMSGDAL();
            
            for (int i = 0; i < tsLines.Length; i++)
            {
                timestepData = new List<string>();
                string date = "";
                string[] lineData = tsLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (localtime == true) { date = gdal.SetDateToLocal(out errorMsg, lineData[0] + " " + lineData[1], gmtOffset); }
                else { date = lineData[0] + " " + lineData[1]; }
                string values = "";
                for (int j = 2; j < lineData.Length; j++)
                {
                    values = values + CoverageAdjust(out errorMsg, lineData[j], coverage, dataset, source);
                    if (j != lineData.Length - 1) { values += ", "; }
                }
                timestepData.Add(values);
                data.Add(date, timestepData);
            }
            return data;
        }

        /// <summary>
        /// Creates the metaData dictionary for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetHMSDaymetMetaData(out string errorMsg, string newMetaData, string metadata)
        {
            errorMsg = "";
            Dictionary<string, string> daymetMeta = new Dictionary<string, string>();
            string[] metaLines = metadata.Split(new string[] { "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < metaLines.Length; i++)
            {
                if (metaLines[i].Contains("http"))
                {
                    daymetMeta.Add("Daymet_url_reference:", metaLines[i].Trim());
                }
                else if(metaLines[i].Contains(':'))
                {
                    string[] lineData = metaLines[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    daymetMeta.Add("Daymet_" + lineData[0].Trim(), lineData[1].Trim());
                }
            }
            if (newMetaData == null) { return daymetMeta; }
            string[] dataLines = newMetaData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < dataLines.Length; i++)
            {
                if (dataLines[i].Contains("="))
                {
                    string[] line = dataLines[i].Split(new char[] { '=' }, 2);
                    if (!daymetMeta.ContainsKey(line[0]))
                    {
                        daymetMeta.Add("Daymet_" + line[0], line[1].Trim());
                    }
                }
            }
            return daymetMeta;
        }

        /// <summary>
        /// Creates the timeseries dictionary for the daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="timeseries"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetHMSDaymetDataTS(out string errorMsg, string timeseries)
        {
            errorMsg = "";
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            string[] tsLines = timeseries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < tsLines.Length; i++)
            {
                string[] lineData = tsLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                DateTime date = new DateTime(Convert.ToInt16(lineData[0]), 1, 1);
                DateTime date2;
                if (i > 0) { date2 = date.AddDays(Convert.ToInt16(lineData[1]) - 1); }
                else { date2 = date; }
                data.Add(date2.Year + "-" + date2.Month.ToString("00") + "-" + date2.Day.ToString("00"), new List<string> { lineData[2] });
            }
            return data;
        }

        /// <summary>
        /// Returns a single string containing all the retrieved timeseries data, merged together. The metadata for the location and elevation of each point is added to the output metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public string CombineTimeseriesAsJson(out string errorMsg, HMSData data)
        {
            errorMsg = "";
            return JsonConvert.SerializeObject(data);
        }
        
        /// <summary>
        /// Takes an existing timeseries and adds on to the data array for each time entry.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="baseTS"></param>
        /// <param name="newTS"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> MergeJsonTS(out string errorMsg, string dataset, string source, Dictionary<string, List<string>> baseTS, string newTS, double coverage)
        {
            errorMsg = "";
            string[] newTSLines = newTS.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newTSLines.Length; i++ )
            {
                string[] lineData = newTSLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string values = "";
                for (int j = 2; j < lineData.Length; j++)
                {
                    values = values + CoverageAdjust(out errorMsg, lineData[j], coverage, dataset, source);
                    if (j != lineData.Length - 1) { values += ", "; }
                }
                baseTS[lineData[0] + " " + lineData[1]].Add(values);
            }
            return baseTS;
        }

        /// <summary>
        /// Constructs a HMSData object from the timeSeries List provided. If localtime is true and source is NLDAS/GLDAS, date/time is adjusted for gmtOffset.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="dataset"></param>
        /// <param name="source"></param>
        /// <param name="localtime"></param>
        /// <param name="gmtOffset"></param>
        /// <returns></returns>
        public HMSData ConstructHMSDataFromTS(out string errorMsg, List<HMSTimeSeries.HMSTimeSeries> ts, string dataset, string source, bool localtime, double gmtOffset)
        {
            errorMsg = "";
            HMSData json = new HMSData();
            if (source.Contains("GLDAS") || source.Contains("NLDAS"))
            {
                json.metadata = SetHMSDataMetaData2(out errorMsg, ts[0].newMetaData, ts[0].metaData, source, 1);

                if (ts.Count > 1)
                {
                    json.metadata.Add(source + "_timeseries_1", ts[0].metaLat + "," + ts[0].metaLon);
                    json.metadata.Add(source + "_elevation[m]_1", ts[0].metaElev.ToString());
                    json.metadata.Add(source + "_percentInCell_1", json.metadata[source +"_percentInCell"]);
                    json.metadata.Add(source + "_areaInCell_1", json.metadata[source + "_areaInCell"]);
                    json.metadata.Remove("elevation[m]");
                    json.data = SetHMSDataTS(out errorMsg, ts[0].timeSeries, dataset, source, localtime, gmtOffset, Convert.ToDouble(json.metadata[source + "_percentInCell_1"]));
                }
                else
                {
                    json.metadata.Add(source + "_timeseries_1", ts[0].metaLat + "," + ts[0].metaLon);
                    json.data = SetHMSDataTS(out errorMsg, ts[0].timeSeries, dataset, source, localtime, gmtOffset, ts[0].cellCoverage);
                }


                for (int i = 1; i < ts.Count; i++)
                {
                    string tempSource = source + "_" + (i+1).ToString();
                    Dictionary<string, string> tempMeta = SetHMSDataMetaData2(out errorMsg, ts[i].newMetaData, ts[i].metaData, source, i+1);
                    json.metadata.Add(tempSource + "_timeseries_" + (i + 1), ts[i].metaLat + "," + ts[i].metaLon);
                    json.metadata.Add(tempSource + "_elevation[m]_" + (i + 1), ts[i].metaElev.ToString());
                    json.metadata.Add(tempSource + "_percentInCell_" + (i + 1), tempMeta[tempSource + "_percentInCell"]);
                    json.metadata.Add(tempSource + "_areaInCell_" + (i + 1), tempMeta[tempSource + "_areaInCell"]);
                    Dictionary<string, List<string>> values = MergeJsonTS(out errorMsg, dataset, source, json.data, ts[i].timeSeries, Convert.ToDouble(json.metadata[tempSource + "_percentInCell_" + (i+1)]));
                    json.data = values;
                }
            }
            else if (source.Contains("Daymet"))
            {
                json.metadata = SetHMSDaymetMetaData(out errorMsg, ts[0].newMetaData, ts[0].metaData);
                json.metadata.Add(source + "_unit", "mm/day");
                json.metadata.Add(source + "_timeseries_1", json.metadata[source + "_Latitude"] + ", " + json.metadata[source + "_Longitude"]);
                json.data = SetHMSDaymetDataTS(out errorMsg, ts[0].timeSeries);
            }
            else if (source.Contains("NCDC"))
            {
                json.data = SetHMSDataTS(out errorMsg, ts[0].timeSeries, dataset, source, localtime, gmtOffset, ts[0].cellCoverage);
            }
            else
            {
                return new HMSData();
            }
            json.dataset = dataset;
            json.source = source;
            return json;
        }

        private string CoverageAdjust(out string errorMsg, string value, double coverage, string dataset, string source)
        {
            errorMsg = "";
            double modifier = 1.0;
            if (dataset.Contains("Precipitation") && source.Contains("GLDAS")) { modifier = 3; }     //Convert GLDAS hr to 3hr
            try
            {
                if (dataset.Contains("Baseflow") || dataset.Contains("SurfaceFlow"))
                {
                    string adjustedValue = (Convert.ToDouble(value) * coverage * modifier).ToString("0.0000####E+00");
                    return adjustedValue;
                }
                else
                {
                    return (Convert.ToDouble(value) * modifier).ToString("0.0000####E+00");
                }
            }
            catch
            {
                return (Convert.ToDouble(value) * modifier).ToString("0.0000####E+00");

            }
        }

        public HMSData CollectDataTotals(out string errorMsg, HMSData jsonData, string frequency)
        {
            errorMsg = "";
            HMSData totals = new HMSData();
            totals.dataset = jsonData.dataset;
            totals.source = jsonData.source;
            totals.metadata = jsonData.metadata;
            DateTime iDate = new DateTime();
            double sum = 0.0;
            if (totals.source.Contains("LDAS") || totals.source.Contains("NCDC"))
            {
                string dateString1 = jsonData.data.Keys.ElementAt(0).ToString().Substring(0, jsonData.data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString1, out iDate);
            }
            else if (totals.source.Contains("Daymet"))
            {
                string dateString1 = jsonData.data.Keys.ElementAt(0).ToString();
                DateTime.TryParse(dateString1, out iDate);
            }

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                DateTime date = new DateTime();
                if (totals.source.Contains("LDAS") || totals.source.Contains("NCDC"))
                {
                    string dateString = jsonData.data.Keys.ElementAt(i).ToString().Substring(0, jsonData.data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                    DateTime.TryParse(dateString, out date);
                }
                else if (totals.source.Contains("Daymet"))
                {
                    string dateString = jsonData.data.Keys.ElementAt(i).ToString();
                    DateTime.TryParse(dateString, out date);
                }
                switch (frequency)
                {
                    case "daily":
                        if (date.Day != iDate.Day)
                        {
                            tempData.Add(iDate.ToString("yyyy-MM-dd"), new List<string>() { sum.ToString("0.0000####E+00") });
                            iDate = date;
                            sum = Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        else
                        {
                            sum += Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        break;
                    case "weekly":
                        int dayDif = (int)(date - iDate).TotalDays;
                        if (dayDif >= 7)
                        {
                            tempData.Add(iDate.ToString("yyyy-MM-dd"), new List<string>() { sum.ToString("0.0000####E+00") });
                            iDate = date;
                            sum = Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        else
                        {
                            sum += Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        break;
                    case "monthly":
                        if (date.Month != iDate.Month)
                        {
                            tempData.Add(iDate.ToString("yyyy-MM-dd"), new List<string>() { sum.ToString("0.0000####E+00") });
                            iDate = date;
                            sum = Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        else
                        {
                            sum += Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        break;
                    case "yearly":
                        int yearDif = (date.Year - iDate.Year);
                        if (yearDif > 0)
                        {
                            tempData.Add(iDate.ToString("yyyy-MM-dd"), new List<string>() { sum.ToString("0.0000####E+00") });
                            iDate = date;
                            sum = Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        else
                        {
                            sum += Convert.ToDouble(jsonData.data[jsonData.data.Keys.ElementAt(i)][0]);
                        }
                        break;
                    default:
                        break;
                }
            }
            totals.data = tempData;
            return totals;
        }

        public HMSData MergeHMSDataList(out string errorMsg, List<HMSData> list)
        {
            errorMsg = "";
            HMSData results = new HMSData();
            results = list[0];
            results.dataset = "Precipitation Compare";
            results.metadata.Add("column_1", "date/time");
            results.metadata.Add("column_2", list[0].source);
            for(int i = 1; i < list.Count; i++){
                results.source += "-" + list[i].source;
                if (list[i].metadata != null)
                {
                    for (int j = 0; j < list[i].metadata.Count; j++)
                    {
                        string metKey = list[i].metadata.Keys.ElementAt(j);
                        if (!results.metadata.ContainsKey(metKey) || !metKey.Contains("column"))
                        {
                            results.metadata.Add(metKey, list[i].metadata.Values.ElementAt(j));
                        }
                    }
                    results.metadata.Add("column_" + (i + 2), list[i].source);
                }
                else
                {
                    results.metadata.Add("column_" + (i + 2), list[i].source);
                }
                if (list[i].data != null)
                {
                    for (int j = 0; j < list[0].data.Count; j++) //First object in list determines length of data dictionary.
                    {
                        string date = list[i].data.Keys.ElementAt(j);
                        string data = list[i].data[date][0];
                        results.data[date].Add(data);
                    }
                }
            }
            return results;
        }

    }
}
