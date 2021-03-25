using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class NOAACoastal
    {

        private DateTime temporalStart = new DateTime(1979, 1, 1);
        private DateTime temporalEnd = DateTime.UtcNow;

        // May need these later for getting nearest station?
        private double maxLat = 53.0;                                       // NLDAS spatial coverage max latitude
        private double minLat = 25.0;                                       // NLDAS spatial coverage min latitude
        private double maxLng = -63.0;                                      // NLDAS spatial coverage max longitude
        private double minLng = -125.0;                                     // NLDAS spatial coverage min longitude

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
        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput, int retries = 0)
        {
            errorMsg = "";

            if (!this.ValidateInput(out errorMsg, componentInput))
            {
                return null;
            }

            // Adjusts date/times by the timezone offset if timelocalized is set to true.
            // componentInput.DateTimeSpan = AdjustForOffset(out errorMsg, componentInput) as DateTimeSpan;

            // Constructs the url for the NLDAS data request and it's query string.
            string url = ConstructURL(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Uses the constructed url to download time series data.
            string data = DownloadData(url, retries).Result;
            if (errorMsg.Contains("ERROR") || data == null) { return null; }

            return data;
        }

        // TODO: Implement
        private bool ValidateInput(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            StringBuilder errors = new StringBuilder();
            bool valid = true;
            return valid;
        }

        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the NOAACoastal endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            try
            {
                WebRequest wr = WebRequest.Create(ConstructURL(out string errorMsg, dataset, testInput));
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                string status = response.StatusCode.ToString();
                string description = response.StatusDescription;
                response.Close();
                return new Dictionary<string, string>()
                {
                    { "status", status },
                    { "description", description}
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>()
                {
                    { "status", "ERROR" },
                    { "description", ex.Message }
                };
            }
        }

        /// <summary>
        /// Constructs the url for retrieving noaa coastal data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            // Append noaa coastal url
            sb.Append(cInput.BaseURL[0]);

            // Add Begin and End Date
            string[] begin_date = cInput.DateTimeSpan.StartDate.ToString("yyyyMMdd HH:mm").Split(' ');
            string[] end_date = cInput.DateTimeSpan.EndDate.ToString("yyyyMMdd HH:mm").Split(' ');
            sb.Append("begin_date=" + begin_date[0] + "%20" + begin_date[1]
                + @"&end_date=" + end_date[0] + "%20" + end_date[1]);

            // Add all other inputs
            sb.Append(
                @"&station=" + cInput.Geometry.StationID +
                @"&product=" + cInput.Geometry.GeometryMetadata["product"] +
                @"&datum=" + cInput.Geometry.GeometryMetadata["datum"] +
                @"&units=" + cInput.Units +
                @"&time_zone=" + cInput.Geometry.Timezone.Name +
                @"&application=" + cInput.Geometry.GeometryMetadata["application"] +
                @"&format=" + cInput.OutputFormat
            );

            return sb.ToString();
        }



        /// <summary>
        /// Downloads noaa data from noaa tides and currents. If Http Request fails will retry up to 5 times.
        /// TODO: Add in start date and end date check prior to the download call (Probably add to Validators class)
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
                    Log.Warning("Error: Failed to download nldas data. Retry {0}:{1}, Url: {2}", retries, maxRetries, url);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download nldas data.");
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
            string[] splitData = data.Split(new string[] { "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitData.Length <= 1)
            {
                splitData = data.Split(new string[] { "Data\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            output.Dataset = dataset;
            output.DataSource = input.Source;
            output.Metadata = SetMetadata(out errorMsg, splitData[0], output);
            output.Data = SetData(out errorMsg, splitData[1].Substring(0, splitData[1].IndexOf("MEAN")).Trim(), input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);
            return output;
        }


        /// <summary>
        /// Parses data string from nldas and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetData(out string errorMsg, string data, bool localTime, string dateFormat, string dataFormat, ITimezone tzDetails)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            List<string> timestepData;
            double offset = (localTime == true) ? tzDetails.Offset : 0.0;
            string[] tsLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tsLines.Length; i++)
            {
                timestepData = new List<string>();
                string[] lineData = tsLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                timestepData.Add(Convert.ToDouble(lineData[2]).ToString(dataFormat));
                dataDict[SetDateToLocal(offset, lineData[0] + " " + lineData[1], dateFormat)] = timestepData;
            }
            return dataDict;
        }


        /// <summary>
        /// Parses data string from nldas and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string metaData, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = output.Metadata;
            string[] metaDataLines = metaData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < metaDataLines.Length - 1; i++)
            {
                if (metaDataLines[i].Contains("="))
                {
                    string[] line = metaDataLines[i].Split('=');
                    if (line[0].Contains("column"))
                    {
                        meta[line[0]] = line[1];
                    }
                    else
                    {
                        meta["nldas_" + line[0]] = line[1];
                    }
                }
            }
            return meta;
        }


        /// <summary>
        /// Adjusts the Date/Time value to the local time, using the offset.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="offset"></param>
        /// <param name="dateHour"></param>
        /// <returns></returns>
        public static string SetDateToLocal(double offset, string dateHour, string dateFormat)
        {

            string[] date = dateHour.Split(' ');
            string hourStr = date[1].Substring(0, 2);
            string dateHourStr = date[0] + " " + hourStr;
            double adjustedOffset = offset + 1;
            DateTime newDate = new DateTime();
            DateTime.TryParseExact(dateHourStr, new string[] { "yyyy-MM-dd HH" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDate);
            newDate = (offset != 0.0) ? newDate.AddHours(offset) : newDate;
            string newDateString = newDate.ToString(dateFormat);
            return newDateString;
        }
    }
}
