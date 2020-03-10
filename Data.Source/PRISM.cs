using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class PRISMData {
        public class Metainfo
        {
            public string status { get; set; }
            public string suid { get; set; }
            public string cloud_node { get; set; }
            public string request_ip { get; set; }
            public string service_url { get; set; }
            public string tstamp { get; set; }
            public int cpu_time { get; set; }
            public string expiration_date { get; set; }
        }

        public class Parameter
        {
            public string name { get; set; }
            public object value { get; set; }
        }

        public class Value
        {
            public string cell_index { get; set; }
            public string center_of_cell { get; set; }
            public List<List<object>> data { get; set; }
            public string name { get; set; }
        }

        public class Result
        {
            public string name { get; set; }
            public List<Value> value { get; set; }
        }

        public class PRISM
        {
            public Metainfo metainfo { get; set; }
            public List<Parameter> parameter { get; set; }
            public List<Result> result { get; set; }
        }
    }

    /// <summary>
    /// PRISM Data collector class, using CSU Proxy
    /// </summary>
    public class PRISM
    {

        /// <summary>
        /// Get PRISM data string, specifying dataset
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset">prism dataset</param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";
            string url = componentInput.BaseURL[0];
            string parameters = ConstructParameterString(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(url, parameters, 0).Result;
            if (data.Contains("ERROR")) { errorMsg = data; return null; }

            return data;
        }

        /// <summary>
        /// Create query string for PRISM request body
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private string ConstructParameterString(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("{'metainfo': { },'parameter': [{'name': 'input_zone_features','value': {'type':'FeatureCollection', 'features':" +
                    "[{'type':'Feature', 'properties':{'name': 'pt one','gid': 1},'geometry':{'type':'Point','coordinates':[");

                // Append point longitude and latitude values to string
                sb.Append(componentInput.Geometry.Point.Longitude.ToString() + ", " + componentInput.Geometry.Point.Latitude.ToString());
                sb.Append("],'crs':{ 'type':'name','properties':{ 'name':'EPSG:4326'} } } } ] } }, { " +
                    "'name': 'units', 'value': 'metric'}, {'name': 'climate_data','value': [");

                // Append dataset
                // Valid dataset: 'ppt', 'tmin', 'tmax', 'tdmean', 'vpdmin', vpdmax'
                sb.Append(dataset);
                sb.Append("] }, {'name': 'start_date','value': ");

                // Append start date
                sb.Append(componentInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd"));
                sb.Append("}, {'name': 'end_date','value': ");
                // Append end date
                sb.Append(componentInput.DateTimeSpan.EndDate.ToString("yyyy-MM-dd"));
                sb.Append("}]}");
            }
            catch(Exception ex)
            {
                errorMsg = "ERROR: " + ex.Message;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Download data from PRISM CSU servers
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<string> DownloadData(string url, string parameters, int retries)
        {
            string data = "";
            HttpClient hc = new HttpClient();
            HttpResponseMessage wm = new HttpResponseMessage();
            HttpContent content = new StringContent(parameters, Encoding.UTF8, "application/json");
            int maxRetries = 10;

            try
            {
                string status = "";

                while (retries < maxRetries && !status.Contains("OK"))
                {
                    wm = await hc.PostAsync(url, content);
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
                    Log.Warning("Error: Failed to download prism data. Retry {0}:{1}", retries, maxRetries);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, parameters, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download prism data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }

        /// <summary>
        /// Takes the data recieved from prism and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            PRISMData.PRISM content;
            try
            {
                content = System.Text.Json.JsonSerializer.Deserialize<PRISMData.PRISM>(data);
            }
            catch(System.Text.Json.JsonException ex)
            {
                errorMsg = "PRISM JSON Deserialization Error: " + ex.Message;
                return null;
            }
            output.Dataset = dataset;
            output.DataSource = input.Source;
            output.Metadata = SetMetadata(out errorMsg, content, input, output);
            output.Data = SetData(out errorMsg, content, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat);
            return output;
        }

        /// <summary>
        /// Parses data string from prism and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, PRISMData.PRISM content, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = output.Metadata;
            foreach(PropertyInfo p in typeof(PRISMData.Metainfo).GetProperties())
            {
                if (!meta.ContainsKey(p.Name) || !meta.ContainsKey("prism_" + p.Name))
                {
                    meta.Add("prism_" + p.Name, p.GetValue(content.metainfo, null).ToString());
                }
            }

            meta.Add("prism_start_date", input.DateTimeSpan.StartDate.ToString());
            meta.Add("prism_end_date", input.DateTimeSpan.EndDate.ToString());
            meta.Add("prism_point_longitude", input.Geometry.Point.Longitude.ToString());
            meta.Add("prism_point_latitude", input.Geometry.Point.Latitude.ToString());
            if (output.Dataset == "Precipitation") {
                meta.Add("prism_unit", "mm");
            }
            else
            {
                meta.Add("prism_unit", "degC");
            }
            return meta;
        }

        /// <summary>
        /// Parses data string from prism and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetData(out string errorMsg, PRISMData.PRISM content, string dateFormat, string dataFormat)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            for (int i = 1; i < content.result[0].value[0].data.Count; i++)
            {
                DateTime.TryParseExact(content.result[0].value[0].data[i][0].ToString(), new string[] { "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime newDate);
                List<string> values = new List<string>();
                for (int j = 1; j < content.result[0].value[0].data[i].Count; j++) {
                    values.Add(content.result[0].value[0].data[i][j].ToString());
                }
                
                dataDict.Add(newDate.ToString(dateFormat), values);
            }
            return dataDict;
        }
    }
}
