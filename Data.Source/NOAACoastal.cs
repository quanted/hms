using Serilog;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class NOAACoastal
    {
        /// <summary>
        /// Default Coastal constructor
        /// </summary>
        public NOAACoastal()
        { }

        /// <summary>
        /// Get data function for noaa coastal.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset">nldas dataset parameter</param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, string dataset, ITimeSeriesOutput output, ITimeSeriesInput componentInput, int retries = 0)
        {
            ITimeSeriesOutput data = output;
            errorMsg = "";
            ValidateInput(componentInput);
  
            // Max size of request is 31 days. Get difference of begin and end dates in days.
            double totalDays = (componentInput.DateTimeSpan.EndDate - componentInput.DateTimeSpan.StartDate).TotalDays;

            // Divide total days by 31 and round up to nearest whole number. Then loop and concatenate data.
            int dayIncrements = (int)Math.Ceiling(totalDays / 31);
            DateTime begin = componentInput.DateTimeSpan.StartDate;
            DateTime end = begin.AddDays(31);
            for (int i = 0; i < dayIncrements; i++)
            {
                // Constructs the url for the data request and it's query string.
                string url = ConstructURL(begin, end, componentInput);
                if (errorMsg.Contains("ERROR")) { return null; }

                // Uses the constructed url to download time series data.
                // Parse json string into json object for easy concatenation of data
                data = SetDataToOutput(out errorMsg, dataset, DownloadData(url, retries).Result, data, componentInput);
                if (errorMsg.Contains("ERROR") || data == null) { return null; }

                begin = end;
                end = begin.AddDays(31);
            }

            return data;
        }

        private void ValidateInput(ITimeSeriesInput input)
        {
            if (input.Geometry.Timezone.Name != "LST" && input.TimeLocalized == true)
            {
                input.Geometry.Timezone.Name = "LST";
            }
            else if(input.Geometry.Timezone.Name == "LST" && input.TimeLocalized == false)
            {
                input.Geometry.Timezone.Name = "GMT";
            }
            else
            {
                input.Geometry.Timezone.Name = "GMT";
            }
        }

        /// <summary>
        /// Constructs the url for retrieving noaa coastal data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static string ConstructURL(DateTime begin, DateTime end, ITimeSeriesInput cInput)
        {
            StringBuilder sb = new StringBuilder();
            // Append noaa coastal url
            sb.Append(cInput.BaseURL[0]);

            // Add Begin and End Date
            string[] begin_date = begin.ToString("yyyyMMdd HH:mm").Split(' ');
            string[] end_date = end.ToString("yyyyMMdd HH:mm").Split(' ');
            sb.Append("begin_date=" + begin_date[0] + "%20" + begin_date[1]
                + @"&end_date=" + end_date[0] + "%20" + end_date[1]);

            // Set units to english if imperial is specified
            string units = cInput.Units;
            if(cInput.Units.Contains("imperial"))
            {
                units = "english";
            }

            // Add all other inputs to request url
            sb.Append(
                @"&station=" + cInput.Geometry.StationID +
                @"&product=" + cInput.Geometry.GeometryMetadata["product"] + 
                @"&datum=" + cInput.Geometry.GeometryMetadata["datum"] +
                @"&units=" + units +
                @"&time_zone=" + cInput.Geometry.Timezone.Name +
                @"&application=" + cInput.Geometry.GeometryMetadata["application"] +
                @"&format=json"
            );

            return sb.ToString();
        }



        /// <summary>
        /// Downloads noaa data from noaa tides and currents. If Http Request fails will retry up to 5 times.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> DownloadData(string url, int retries)
        {
            string data = "";
            HttpClient hc = new HttpClient();
            HttpResponseMessage wm = new HttpResponseMessage();
            int maxRetries = 10;

            try
            {
                string status = "";

                while (retries < maxRetries && !status.Contains("OK"))
                {
                    wm = await hc.GetAsync(url);
                    var response = wm.Content;
                    status = wm.StatusCode.ToString();
                    data = await wm.Content.ReadAsStringAsync();
                    retries += 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(1000 * retries);
                    }
                }
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries += 1;
                    Log.Warning("Error: Failed to download noaa coastal data. Retry {0}:{1}, Url: {2}", retries, maxRetries, url);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download noaa coastal data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }


        /// <summary>
        /// Takes the data recieved from noaaa_coastal and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            // Parse json string into json object for easy manipulation
            JsonElement root = JsonDocument.Parse(data).RootElement;

            // Set output dataset/source
            output.Dataset = dataset;
            output.DataSource = input.Source;

            // Check for error message
            if (root.TryGetProperty(@"error", out JsonElement value))
            {
                // Error found, set error msg to errorMsg
                value.TryGetProperty(@"message", out value);
                errorMsg = "ERROR: " + value.ToString();
            }
            
            // Check for metadata and set to output
            if (root.TryGetProperty(@"metadata", out value) && output.Metadata.Count == 0)
            {
                output.Metadata = SetMetadata(out errorMsg, JsonSerializer.Serialize<JsonElement>(value), output);
            }

            // Check for data and set to output
            if (root.TryGetProperty(@"data", out value))
            {
                // Set data to output data
                SetData(out errorMsg, value, input).ToList().ForEach(x => {
                    // Add data to ouput if key is not already there and if the key does not exceed the end date
                    if(!output.Data.ContainsKey(x.Key) && 
                    !(DateTime.ParseExact(x.Key, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) > input.DateTimeSpan.EndDate))
                    {
                        output.Data.Add(x.Key, x.Value);
                    }
                });
            }

            return output;
        }

        /// <summary>
        /// Parses data string from noaa and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns> 
        private Dictionary<string, List<string>> SetData(out string errorMsg, JsonElement data, ITimeSeriesInput input)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            double offset = (input.TimeLocalized == true) ? input.Geometry.Timezone.Offset : 0.0;

            // Loop over array of objects. Objects take form of:
            //
            // {"t":"2013-08-08 15:00", "v":"72.5", "f":"0,0,0"}
            //
            foreach (JsonElement item in data.EnumerateArray())
            {
                // Serialize to string then deserialize to dictionary of strings
                string itemString = JsonSerializer.Serialize<JsonElement>(item);
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(itemString);

                // Get time value and convert to string array
                string time = dict["t"];

                // Remove t and other unneccsarry values from data
                dict.Remove("t");

                // Assign list of stings of data to dict
                dataDict[time] = dict.Values.ToList();
            }

            return dataDict;
        }

        /// <summary>
        /// Parses data string from noaa and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string metaData, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> item in JsonSerializer.Deserialize<Dictionary<string, string>>(metaData))
            {
                meta.Add(output.DataSource + "_"+ item.Key, item.Value);
            }
            return meta;
        }
    }
}
