using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class StreamGauge
    {
        /// <summary>
        /// Get data function for stream gauge.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        public List<string> GetData(out string errorMsg, ITimeSeriesInput componentInput, int retries = 0)
        {
            errorMsg = "";
            
            // Constructs the url for the USGS stream gauge data request and it's query string.
            string url = ConstructURL(out errorMsg, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Uses the constructed url to download time series data.
            string data = DownloadData(url, retries).Result;
            if (errorMsg.Contains("ERROR") || data == null) { return null; }

            return new List<string>() {data, url};
        }

        /// <summary>
        /// Constructs the url for retrieving usgs stream gauge as data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            if (!cInput.Geometry.GeometryMetadata.ContainsKey("gaugestation"))
            {
                errorMsg = "Stream Gauge station id not found. 'gaugestation' required in Geometry MetaData.";
                return null;
            }
            string stationID = cInput.Geometry.GeometryMetadata["gaugestation"];

            StringBuilder sb = new StringBuilder();
            sb.Append(cInput.BaseURL[0]);
            sb.Append(@"search_site_no=" + stationID + "&");
            sb.Append(@"search_site_no_match_type=exact&");
            sb.Append(@"group_key=NONE&");
            sb.Append(@"index_pmcode_00060=1&");
            sb.Append(@"sitefile_output_format=html_table&");
            sb.Append(@"column_name=agency_cd&");
            sb.Append(@"column_name=site_no&");
            sb.Append(@"column_name=station_nm&");
            sb.Append(@"range_selection=date_range&");
            sb.Append(@"begin_date=" + cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&");
            sb.Append(@"end_date=" + cInput.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&");
            sb.Append(@"format=rdb&");
            sb.Append(@"date_format=YYYY-MM-DD&");
            sb.Append(@"rdb_compression=value&");
            sb.Append(@"list_of_search_criteria=search_site_no%2Crealtime_parameter_selection");

            return sb.ToString();
        }

        /// <summary>
        /// Downloads stream gauge data from usgs servers. If Http Request fails will retry up to 10 times.
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
                    Log.Warning("Error: Failed to download usgs stream gauge data. Retry {0}:{1}, Url: {2}", retries, maxRetries, url);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download usgs stream gauge data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }
        
        /// <summary>
        /// Takes the data recieved from usgs stream gauge and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput<List<double>> SetDataToOutput(out string errorMsg, string data, ITimeSeriesOutput<List<double>> output, ITimeSeriesInput input)
        {
            errorMsg = "";

            string[] dataLines = data.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.None);
            output.Dataset = "streamflow";
            output.DataSource = "usgs";
            // output.Metadata = SetMetadata(out errorMsg, input, output);
            // Time series is the nwis default of 15min intervals, date-time has been updated to the time localized preference.
            output.Data = SetData(out errorMsg, dataLines, input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);
            return output;
        }

        /// <summary>
        /// Parses data string from usgs stream gauge and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public Dictionary<string, string> SetMetadata(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";

            if (!input.Geometry.GeometryMetadata.ContainsKey("gaugestation"))
            {
                errorMsg = "Stream Gauge station id not found. 'gaugestation' required in Geometry MetaData.";
                return null;
            }
            string stationID = input.Geometry.GeometryMetadata["gaugestation"];

            StringBuilder sb = new StringBuilder();
            sb.Append(input.BaseURL[0]);
            sb.Append(@"search_site_no=" + stationID + "&");
            sb.Append(@"&20&search_site_no_match_type=exact&group_key=NONE&format=sitefile_output&sitefile_output_format=rdb&column_name=agency_cd&column_name=site_no&column_name=station_nm&column_name=dec_lat_va&column_name=dec_long_va&column_name=huc_cd&range_selection=date_range&begin_date=2000-01-01&end_date=2010-12-31&date_format=YYYY-MM-DD&rdb_compression=value&list_of_search_criteria=search_site_no%2Crealtime_parameter_selection");
            string metadata = DownloadData(sb.ToString(), 0).Result;

            Dictionary<string, string> meta = new Dictionary<string, string>();
            string[] metaDataLines = metadata.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> keys = null;
            List<string> values = null;
            for (int i = 0; i < metaDataLines.Length; i++)
            {
                string[] s = metaDataLines[i].Replace("\r", "").Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (s[0].Contains("#"))
                {
                    continue;   // Comments
                }
                if (keys == null)
                {
                    keys = s.ToList();
                    i += 1;
                }
                else
                {
                    values = s.ToList();
                }
            }
            for(int i = 0; i < keys.Count; i++)
            {
                meta.Add(keys[i], values[i]);
            }
            meta.Add("nwis_metadata_url", sb.ToString());
            return meta;
        }

        /// <summary>
        /// Parses data string from usgs stream gauge and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<double>> SetData(out string errorMsg, string[] data, bool local, string dateFormat, string dataFormat, ITimezone tzDetails)
        {
            errorMsg = "";
            Dictionary<string, List<double>> dataDict = new Dictionary<string, List<double>>();
            double conversion = 0.0283168;

            int rowCount = 0;
            DateTime date1 = new DateTime();
            for(int i = 0; i < data.Length - 1; i++)
            {
                string[] s = data[i].Split(new string[] {"\t"}, StringSplitOptions.RemoveEmptyEntries);
                if (s[0].Contains("#"))
                {
                    continue;   // Comments
                }
                else if (rowCount >= 2)
                {
                    DateTime date = DateTime.ParseExact(s[2], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    if (!local)
                    {
                        date = date.AddHours(tzDetails.Offset);
                    }
                    double flow = Double.Parse(s[4]) * conversion;
                    string dateStr = date.ToString("yyyy-MM-dd HH:mm");
                    if (dataDict.ContainsKey(dateStr))
                    {
                        dataDict[dateStr] = new List<double>() {flow};
                    }
                    else
                    {
                        dataDict.Add(dateStr, new List<double>() {flow});
                    }
                }
                rowCount += 1;
            }
            return dataDict;
        }
    }
}
