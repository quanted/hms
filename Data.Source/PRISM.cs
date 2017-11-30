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
            public string tstamp { get; set; }
            public string service_url { get; set; }
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
            public List<int> cell_index { get; set; }
            public List<List<object>> data { get; set; }
            public string gid { get; set; }
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


    public class PRISM
    {

        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";
            string url = componentInput.BaseURL[0];
            string parameters = ConstructParameterString(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(url, parameters).Result;
            if (errorMsg.Contains("ERROR")) { return null; }

            // TESTING
            //string data = "{\n  \"metainfo\": {\n    \"status\": \"Finished\",\n    \"suid\": \"f3d2cb7e-d480-11e7-9329-2917bf2217fc\",\n    \"cloud_node\": \"10.1.85.2\",\n    \"request_ip\": \"73.118.95.127\",\n    \"service_url\": \"http://csip.engr.colostate.edu:8083/csip-climate/m/prism/1.0\",\n    \"csip-climate.version\": \"$version: 2.0.13 28d288481f83 2017-11-03 Mark Haas <mark.haas@ars.usda.gov>, built at 2017-11-03 12:26 by jenkins$\",\n    \"csip.version\": \"$version: 2.2.1 7bf35fce9e13 2017-09-15 od, built at 2017-11-03 12:26 by jenkins$\",\n    \"tstamp\": \"2017-11-28 14:13:16\",\n    \"cpu_time\": 27,\n    \"expiration_date\": \"2017-11-28 14:13:47\"\n  },\n  \"parameter\": [\n    {\n      \"name\": \"input_zone_features\",\n      \"value\": {\n        \"type\": \"FeatureCollection\",\n        \"features\": [{\n          \"type\": \"Feature\",\n          \"properties\": {\n            \"name\": \"pt one\",\n            \"gid\": 1\n          },\n          \"geometry\": {\n            \"type\": \"Point\",\n            \"coordinates\": [\n              -83.355723,\n              33.925673\n            ],\n            \"crs\": {\n              \"type\": \"name\",\n              \"properties\": {\"name\": \"EPSG:4326\"}\n            }\n          }\n        }]\n      }\n    },\n    {\n      \"name\": \"units\",\n      \"value\": \"metric\"\n    },\n    {\n      \"name\": \"climate_data\",\n      \"value\": [\"ppt\"]\n    },\n    {\n      \"name\": \"start_date\",\n      \"value\": \"2015-01-01\"\n    },\n    {\n      \"name\": \"end_date\",\n      \"value\": \"2015-01-08\"\n    }\n  ],\n  \"result\": [{\n    \"name\": \"output\",\n    \"value\": [{\n      \"cell_index\": [\n        999,\n        384\n      ],\n      \"data\": [\n        [\n          \"date\",\n          \"ppt (mm)\"\n        ],\n        [\n          \"2015-01-01\",\n          0\n        ],\n        [\n          \"2015-01-02\",\n          1.46\n        ],\n        [\n          \"2015-01-03\",\n          15.35\n        ],\n        [\n          \"2015-01-04\",\n          17.16\n        ],\n        [\n          \"2015-01-05\",\n          12.98\n        ],\n        [\n          \"2015-01-06\",\n          0\n        ],\n        [\n          \"2015-01-07\",\n          0\n        ],\n        [\n          \"2015-01-08\",\n          0\n        ]\n      ],\n      \"name\": \"pt one\"\n    }]\n  }]\n}";

            return data;
        }

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

        private async Task<string> DownloadData(string url, string parameters)
        {
            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";

                var content = new StringContent(parameters, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    while (retries > 0 && !status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                        var response = await client.PostAsync(url, content);
                        data =  await response.Content.ReadAsStringAsync();
                        status = response.StatusCode.ToString();
                        retries -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return "ERROR: Unable to download requested nldas data.\n" + ex.Message;
            }
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
            PRISMData.PRISM content = Newtonsoft.Json.JsonConvert.DeserializeObject<PRISMData.PRISM>(data);

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
            for (int i = 1; i < content.result[0].value[0].data.Count - 1; i++)
            {
                DateTime.TryParseExact(content.result[0].value[0].data[i][0].ToString(), new string[] { "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime newDate);
                Double.TryParse(content.result[0].value[0].data[i][1].ToString(), out double dataValue);
                dataDict.Add(newDate.ToString(dateFormat), new List<string> { dataValue.ToString(dataFormat) });
            }
            return dataDict;
        }
    }
}
