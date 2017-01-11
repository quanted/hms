using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;

namespace HMSJSON
{
    public class HMSJSON
    {
        public string timeseriesJSON { get; set; }
        public string metadataJSON { get; set; }
        public string datasetJSON { get; set; }
        public string sourceJSON { get; set; }
        public string newMetadataJSON { get; set; }
        private Dictionary<string, string> metaDictionary { get; set; }
        
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
        public string ConvertDataToJSON(out string errorMsg, string data, string dataset)
        {
            errorMsg = "";
            string[] dataLine = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string jsonString = "";

            jsonString = String.Concat("{\"", dataset,"\":[");

            for (int i = 0; i < dataLine.Length; i++)
            {
                string[] dataArray = dataLine[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                //Explicitly stating data types
                //jsonString += "{\"date/hour\":\"" + dataArray[0] + " " + dataArray[1] + "\",";
                //jsonString += "\"data\":\"" + dataArray[2] + "\"}";

                //Implicitly stating data types
                jsonString = String.Concat(jsonString, "{\"", dataArray[0], " ", dataArray[1], "\":\"", dataArray[2], "\"}");
                //jsonString += @"{""" + dataArray[0] + " " + dataArray[1] + @""":""" + dataArray[2] + @"""}";

                if (i+3 != dataLine.Length)
                {
                    jsonString += @",";
                }
                i += 2;
            }
            jsonString += @"]}";
            int count = Encoding.ASCII.GetCharCount(Encoding.ASCII.GetBytes(jsonString));
            return jsonString;
        }

        /// <summary>
        /// Returns a single string containing all the individual timeseries data with their metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="ts"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public string CombineDatasetsAsJSON(out string errorMsg, List<HMSTimeSeries.HMSTimeSeries> ts, string dataset, string source, bool localtime, double gmtOffset)
        {
            errorMsg = "";
            string data = "";
            for (int i = 0; i < ts.Count; i++)
            {
                // "DataSetBlock" is used to parse the data on the client side, where it is saved as individual files.
                string jsonString = GetJSONString(out errorMsg, ts[i].timeSeries, ts[i].newMetaData, ts[i].metaData, dataset, source, localtime, gmtOffset);
                data = String.Concat(data, "DataSetBlock ", jsonString);
            }
            return data;
        }

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
        public string GetJSONString(out string errorMsg, string timeseries, string newMetaData, string metadata, string dataset, string source, bool localtime, double gmtOffset)
        {
            errorMsg = "";
            HMSData output = new HMSData();
            output.dataset = dataset;
            output.source = source;
            output.metadata = SetHMSDataMetaData(out errorMsg, output, newMetaData, metadata);
            output.data = SetHMSDataTS(out errorMsg, output, timeseries, localtime, gmtOffset);
            string result = JsonConvert.SerializeObject(output);
            return result;
        }

        /// <summary>
        /// Creates a dictionary from the input metadata.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetHMSDataMetaData(out string errorMsg, HMSData output, string newMetaData, string metadata)
        {
            errorMsg = "";
            Dictionary<string, string> metaDict = new Dictionary<string, string>();

            string[] keys = ConfigurationManager.AppSettings["metadataConfig"].ToString().Split(',');
            string[] metaLines = metadata.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = metaLines.Length - 1; i > 0; i--)
            {
                string[] metadataLineArray = metaLines[i].Split(new char[] { '=' }, 2);
                if (keys.Contains(metadataLineArray[0]) && !metaDict.ContainsKey(metadataLineArray[0]))
                {
                    metaDict.Add(metadataLineArray[0], metadataLineArray[1].Trim());
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
                        metaDict.Add(line[0], line[1].Trim());
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
        private Dictionary<string, List<string>> SetHMSDataTS(out string errorMsg, HMSData output, string timeseries, bool localtime, double gmtOffset)
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
                for (int j = 2; j < lineData.Length; j++)
                {
                    timestepData.Add(lineData[j]);
                }
                data.Add(date, timestepData);
            }
            return data;
        }

    }
}
