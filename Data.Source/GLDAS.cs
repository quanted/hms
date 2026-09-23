using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

namespace Data.Source
{
    /// <summary>
    /// Base GLDAS class.
    /// </summary>
    public class GLDAS
    {

        private DateTime temporalStart = new DateTime(2000, 1, 1);          // GLDAS temporal coverage start date.
        private DateTime temporalEnd = DateTime.UtcNow.AddMonths(-4);       // GLDAS temporal coverage end date (approximate)
        private double maxLat = 90.0;                                       // GLDAS spatial coverage max latitude
        private double minLat = -60.0;                                      // GLDAS spatial coverage min latitude
        private double maxLng = 180.0;                                      // GLDAS spatial coverage max longitude
        private double minLng = -180.0;                                     // GLDAS spatial coverage min longitude

        /// <summary>
        /// Get data function for gldas.
        /// </summary>
        /// <param name="errorMsg"></param>s
        /// <param name="dataset">nldas dataset parameter</param>
        /// <param name="componentInput"></param>
        /// <param name="retries">Number of retries</param>
        /// <param name="accessToken">Optional EarthData access token for Giovanni API</param>
        /// <returns></returns>
        public List<string> GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput, int retries = 0, string accessToken = null)
        {
            errorMsg = "";

            if (!this.ValidateInput(out errorMsg, componentInput))
            {
                return null;
            }

            // Adjusts date/times by the timezone offset if timelocalized is set to true.
            componentInput.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, componentInput) as DateTimeSpan;

            if (componentInput.Geometry.GeometryMetadata.ContainsKey("StreamFlowEndDate"))
            {
                DateTime sfed = DateTime.ParseExact(componentInput.Geometry.GeometryMetadata["StreamFlowEndDate"], "MM/dd/yyyy HH:mm", null);
                TimeSpan ts = new TimeSpan(23, 00, 0);
                componentInput.DateTimeSpan.EndDate = sfed.Date.AddDays(1.0) + ts;
            }

            if (componentInput.BaseURL == null || componentInput.BaseURL.Count == 0 || string.IsNullOrWhiteSpace(componentInput.BaseURL[0]))
            {
                errorMsg = "ERROR: GLDAS base URL is missing or invalid.";
                return null;
            }

            if (componentInput.BaseURL[0].Contains("giovanni", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(accessToken))
            {
                errorMsg = "ERROR: GLDAS EarthData access token is required for Giovanni data retrieval.";
                return null;
            }

            // Constructs the url for the NLDAS data request and it's query string.
            List<string> url = ConstructURL(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            List<string> data = DownloadData(url, retries, accessToken).Result;
            if (data == null || data.Count == 0)
            {
                errorMsg = "ERROR: GLDAS download failed or returned no data.";
                return null;
            }

            return data;
        }

        private bool ValidateInput(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            StringBuilder errors = new StringBuilder();
            bool valid = true;
            // Temporal validation
            if (input.DateTimeSpan.StartDate < this.temporalStart)
            {
                valid = false;
                errors.Append("ERROR: Invalid start date, GLDAS temporal coverage starts at " + this.temporalStart.ToString("yyyy-MM-dd") + ", entered start date is " + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + ", ");
            }
            if (input.DateTimeSpan.EndDate > this.temporalEnd)
            {
                valid = false;
                errors.Append("ERROR: Invalid end date, GLDAS temporal coverage ends at " + this.temporalEnd.ToString("yyyy-MM-dd") + ", entered end date is " + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + ", ");
            }
            // Spatial calidation
            if (input.Geometry.Point.Latitude < this.minLat || input.Geometry.Point.Latitude > this.maxLat)
            {
                valid = false;
                errors.Append("ERROR: Invalid latitude, GLDAS spatial coverage is between latitudes " + this.minLat.ToString() + " and " + this.maxLat.ToString() + ", entered latitude is " + input.Geometry.Point.Latitude.ToString() + ", ");
            }
            if (input.Geometry.Point.Longitude < this.minLng || input.Geometry.Point.Longitude > this.maxLng)
            {
                valid = false;
                errors.Append("ERROR: Invalid longitude, GLDAS spatial coverage is between longitude " + this.minLng.ToString() + " and " + this.maxLng.ToString() + ", entered longitude is " + input.Geometry.Point.Longitude.ToString() + ", ");
            }

            if (!valid)
            {
                errorMsg = errors.ToString();
            }
            return valid;
        }

        /// <summary>
        /// Extracts the GLDAS data variable from the base URL or constructs it from the dataset name.
        /// </summary>
        /// <param name="dataset">Dataset name</param>
        /// <param name="baseUrl">Base URL containing the data parameter</param>
        /// <returns>Data variable string for Giovanni API</returns>
        private static string GetGldasDataVariable(string dataset, string baseUrl)
        {
            // Try to extract from base URL first (for Giovanni API URLs)
            if (!string.IsNullOrWhiteSpace(baseUrl) && baseUrl.Contains("data=", StringComparison.OrdinalIgnoreCase))
            {
                var dataParam = baseUrl.Split(new[] { "data=" }, StringSplitOptions.None);
                if (dataParam.Length > 1)
                {
                    var variable = dataParam[1].Split('&')[0];
                    return variable;
                }
            }

            // Default fallback - construct from dataset name
            return dataset;
        }

        private static string ExtractDataVariableFromBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl) || !baseUrl.Contains("data=", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var dataParam = baseUrl.Split(new[] { "data=" }, StringSplitOptions.None);
            if (dataParam.Length < 2)
            {
                return null;
            }

            return Uri.UnescapeDataString(dataParam[1].Split('&')[0]);
        }

        /// <summary>
        /// Constructs the url for retrieving GLDAS data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static List<string> ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            List<string> urls = new List<string>();

            // Check if using Giovanni API format
            if (cInput.BaseURL[0].Contains("giovanni", StringComparison.OrdinalIgnoreCase))
            {
                string baseUrl = cInput.BaseURL[0].Split('?')[0];
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    baseUrl = "https://api.giovanni.earthdata.nasa.gov/timeseries";
                }

                string dataVariable = GetGldasDataVariable(dataset, cInput.BaseURL[0]);

                // GLDAS version handling for Giovanni API
                DateTime gldas21 = new DateTime(2010, 01, 01);
                bool only21 = true;

                if (DateTime.Compare(cInput.DateTimeSpan.StartDate, gldas21) >= 0 || only21)
                {
                    // Case #3: both start and end are in GLDAS 2.1
                    string start = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-ddTHH:mm:ss");
                    DateTime tempDate = cInput.DateTimeSpan.EndDate.AddHours(3);
                    string end = tempDate.ToString("yyyy-MM-ddTHH:mm:ss");

                    string url = Utilities.clsNLDAS_GES_DISC.BuildTimeSeriesUrl(
                        cInput.Geometry.Point.Latitude, 
                        cInput.Geometry.Point.Longitude, 
                        start, 
                        end, 
                        dataVariable, 
                        baseUrl);
                    urls.Add(url);
                }
                else if (DateTime.Compare(cInput.DateTimeSpan.EndDate, gldas21) > 0 && DateTime.Compare(cInput.DateTimeSpan.StartDate, gldas21) < 0)
                {
                    // Case #2: start is in GLDAS 2.0, end is in GLDAS 2.1
                    string dataVariable20 = dataVariable.Replace("_v2_1_", "_v2_0_");

                    // GLDAS 2.0 portion
                    string start1 = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-ddTHH:mm:ss");
                    DateTime tempDate1 = gldas21.AddHours(3);
                    string end1 = tempDate1.ToString("yyyy-MM-ddTHH:mm:ss");

                    string url1 = Utilities.clsNLDAS_GES_DISC.BuildTimeSeriesUrl(
                        cInput.Geometry.Point.Latitude, 
                        cInput.Geometry.Point.Longitude, 
                        start1, 
                        end1, 
                        dataVariable20, 
                        baseUrl);
                    urls.Add(url1);

                    // GLDAS 2.1 portion
                    string start2 = gldas21.ToString("yyyy-MM-ddTHH:mm:ss");
                    DateTime tempDate2 = cInput.DateTimeSpan.EndDate.AddHours(3);
                    string end2 = tempDate2.ToString("yyyy-MM-ddTHH:mm:ss");

                    string url2 = Utilities.clsNLDAS_GES_DISC.BuildTimeSeriesUrl(
                        cInput.Geometry.Point.Latitude, 
                        cInput.Geometry.Point.Longitude, 
                        start2, 
                        end2, 
                        dataVariable, 
                        baseUrl);
                    urls.Add(url2);
                }
                else
                {
                    // Case #1: both start and end are in GLDAS 2.0
                    string dataVariable20 = dataVariable.Replace("_v2_1_", "_v2_0_");

                    string start = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-ddTHH:mm:ss");
                    string end = cInput.DateTimeSpan.EndDate.ToString("yyyy-MM-ddTHH:mm:ss");

                    string url = Utilities.clsNLDAS_GES_DISC.BuildTimeSeriesUrl(
                        cInput.Geometry.Point.Latitude, 
                        cInput.Geometry.Point.Longitude, 
                        start, 
                        end, 
                        dataVariable20, 
                        baseUrl);
                    urls.Add(url);
                }

                return urls;
            }

            // Legacy format support (old hydro1 URLs)
            // Example base url: https://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=GLDAS2:GLDAS_NOAH025_3H_v2.1:Rainf_f_tavg&location=GEOM:POINT
            // Cases:
            // #1 both start and end are in GLDAS 2.0
            // #2 start is in GLDAS 2.0, end is in GLDAS 2.1
            // #3 both are in GLDAS 2.1
            DateTime gldas21Legacy = new DateTime(2010, 01, 01);
            bool only21Legacy = true;
            var test = DateTime.Compare(cInput.DateTimeSpan.StartDate, gldas21Legacy);
            if (DateTime.Compare(cInput.DateTimeSpan.StartDate, gldas21Legacy) >= 0 || only21Legacy)            // #3
            {
                //Add Start and End Date
                string[] startDT = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
                DateTime tempDate = cInput.DateTimeSpan.EndDate.AddHours(3);
                string[] endDT = tempDate.ToString("yyyy-MM-dd HH").Split(' ');

                string url1 = cInput.BaseURL[0] +
                    @"%28" + cInput.Geometry.Point.Longitude.ToString() +
                    @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29" +
                    @"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1] + @"&type=asc2";
                urls.Add(url1);
            }
            else if (DateTime.Compare(cInput.DateTimeSpan.EndDate, gldas21Legacy) > 0 && DateTime.Compare(cInput.DateTimeSpan.StartDate, gldas21Legacy) < 0)          // #2
            {
                string gldas2Url = cInput.BaseURL[0].Replace("GLDAS_NOAH025_3H_v2.1", "GLDAS_NOAH025_3H_v2.0");

                //Add Start and End Date for GLDAS 2.0
                string[] startDT1 = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
                DateTime tempDate1 = gldas21Legacy.AddHours(3);
                string[] endDT1 = tempDate1.ToString("yyyy-MM-dd HH").Split(' ');

                string url1 = gldas2Url +
                    @"%28" + cInput.Geometry.Point.Longitude.ToString() +
                    @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29" +
                    @"&startDate=" + startDT1[0] + @"T" + startDT1[1] + @"&endDate=" + endDT1[0] + "T" + endDT1[1] + @"&type=asc2";
                urls.Add(url1);

                //Add Start and End Date for GLDAS 2.1
                string[] startDT2 = gldas21Legacy.ToString("yyyy-MM-dd HH").Split(' ');
                DateTime tempDate2 = cInput.DateTimeSpan.EndDate.AddHours(3);
                string[] endDT2 = tempDate2.ToString("yyyy-MM-dd HH").Split(' ');

                string url2 = cInput.BaseURL[0] +
                    @"%28" + cInput.Geometry.Point.Longitude.ToString() +
                    @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29" +
                    @"&startDate=" + startDT2[0] + @"T" + startDT2[1] + @"&endDate=" + endDT2[0] + "T" + endDT2[1] + @"&type=asc2";
                urls.Add(url2);
            }
            else
            {
                string gldas2Url = cInput.BaseURL[0].Replace("GLDAS_NOAH025_3H_v2.1", "GLDAS_NOAH025_3H_v2.0");

                //Add Start and End Date for GLDAS 2.0
                string[] startDT1 = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
                string[] endDT1 = cInput.DateTimeSpan.EndDate.ToString("yyyy-MM-dd HH").Split(' ');

                string url1 = gldas2Url +
                    @"%28" + cInput.Geometry.Point.Longitude.ToString() +
                    @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29" +
                    @"&startDate=" + startDT1[0] + @"T" + startDT1[1] + @"&endDate=" + endDT1[0] + "T" + endDT1[1] + @"&type=asc2";
                urls.Add(url1);
            }
            return urls;
        }

        /// <summary>
        /// Downloads gldas data from nasa servers. If Http Request fails will retry up to 5 times.
        /// TODO: Add in start date and end date check prior to the download call (Probably add to Validators class)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <param name="accessToken">Optional EarthData access token</param>
        /// <returns></returns>
        private async Task<List<string>> DownloadData(List<string> urls, int retries, string accessToken = null)
        {
            List<string> data = new List<string>();
            HttpClient hc = new HttpClient();

            // Add authorization header if access token is provided
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                hc.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }

            HttpResponseMessage wm = new HttpResponseMessage();
            foreach (string url in urls)
            {
                string _data = "";
                int maxRetries = 10;

                try
                {
                    string status = "";

                    while (retries < maxRetries && !status.Contains("OK"))
                    {
                        wm = await hc.GetAsync(url);
                        var response = wm.Content;
                        status = wm.StatusCode.ToString();
                        _data = await wm.Content.ReadAsStringAsync();
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
                        Log.Warning("Error: Failed to download gldas data. Retry {0}:{1}", retries, maxRetries);
                        Random r = new Random();
                        Thread.Sleep(5000 + (r.Next(10) * 1000));
                        return await this.DownloadData(urls, retries, accessToken);
                    }

                    wm.Dispose();
                    hc.Dispose();
                    Log.Warning(ex, "Error: Failed to download gldas data.");
                    return null;
                }
                data.Add(_data);
            }
            wm.Dispose();
            hc.Dispose();
            return data;           
        }

        /// <summary>
        /// Determines the coordinate values that corresponds to the closest coordinate point in the NLDAS grid.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="point">ICoordinate</param>
        /// <returns>[Latitude, Longitude]</returns>
        public double[] DetermineReturnCoordinates(out string errorMsg, IPointCoordinate point)
        {
            errorMsg = "";
            double[] coord = new double[2];
            double step = step = 0.25;
            double x = (point.Longitude + 179.8750) / step;
            coord[1] = (Math.Round(x, MidpointRounding.AwayFromZero) * step) - 179.8750;
            double y = (point.Latitude + 59.8750) / step;
            coord[0] = (Math.Round(y, MidpointRounding.AwayFromZero) * step) - 59.8750;
            return coord;
        }

        /// <summary>
        /// Takes the data recieved from nldas and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, List<string> data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            List<string> metadataList = new List<string>();
            List<string> dataList = new List<string>();

            if (data == null || data.Count == 0)
            {
                errorMsg = "ERROR: GLDAS response data is empty.";
                return output;
            }

            foreach (string _data in data)
            {
                string[] splitData = _data.Split(new string[] { "Data\r\n", "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitData.Length < 2)
                {
                    errorMsg = "ERROR: Unable to parse GLDAS response payload format.";
                    return output;
                }
                metadataList.Add(splitData[0]);
                dataList.Add(splitData[1]);
            }
            output.Dataset = dataset;
            output.DataSource = input.Source;

            string dataVariable = null;
            if (input?.BaseURL != null && input.BaseURL.Count > 0)
            {
                dataVariable = ExtractDataVariableFromBaseUrl(input.BaseURL[0]);
            }

            if (!string.IsNullOrWhiteSpace(dataVariable))
            {
                output.Metadata["gldas_data_variable"] = dataVariable;
            }

            output.Metadata = SetMetadata(out errorMsg, metadataList, output);
            output.Data = SetData(out errorMsg, dataList, input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);

            return output;
        }

        /// <summary>
        /// Parses data string from nldas and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, List<string> metaDataList, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = output.Metadata;
            foreach (string metaData in metaDataList)
            {
                string version = (metaData.Contains("v2.0")) ? "2.0" : "2.1";

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
                            string key = "gldas_" + version + "_" + line[0];
                            if (!meta.ContainsKey(key))
                            {
                                meta[key] = line[1];
                            }
                        }
                    }
                }
            }
            return meta;
        }

        /// <summary>
        /// Parses data string from nldas and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetData(out string errorMsg, List<string> dataLists, bool localTime, string dateFormat, string dataFormat, ITimezone tzDetails)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            List<string> timestepData;
            double offset = (localTime == true) ? tzDetails.Offset : 0.0;
            foreach (string data in dataLists)
            {
                string[] tsLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < tsLines.Length; i++)
                {
                    string line = tsLines[i].Trim();
                    if (line.Contains("MEAN") || string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }

                    if (line.StartsWith("Date", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string timestampString = "";
                    string valueString = "";

                    string[] csvParts = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (csvParts.Length >= 2)
                    {
                        timestampString = csvParts[0].Trim().Trim('"').Replace("T", " ").Replace("Z", "");
                        valueString = csvParts[1].Trim().Trim('"');
                    }
                    else
                    {
                        string[] whitespaceParts = line.Split(new string[] { "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (whitespaceParts.Length < 3)
                        {
                            continue;
                        }

                        timestampString = (whitespaceParts[0] + " " + whitespaceParts[1]).Trim().Trim('"').Replace("T", " ").Replace("Z", "");
                        valueString = whitespaceParts[2].Trim().Trim('"');
                    }

                    if (!DateTime.TryParse(timestampString, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime parsedTimestamp) &&
                        !DateTime.TryParse(timestampString, out parsedTimestamp))
                    {
                        continue;
                    }

                    if (!double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) &&
                        !double.TryParse(valueString, out value))
                    {
                        continue;
                    }

                    string key = NLDAS.SetDateToLocal(offset, parsedTimestamp.ToString("yyyy-MM-dd HH:mm:ss"), dateFormat);
                    if (!dataDict.ContainsKey(key))
                    {
                        timestepData = new List<string>();
                        timestepData.Add(value.ToString(dataFormat));
                        dataDict[key] = timestepData;
                    }
                }
            }

            if (dataDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to parse GLDAS response data values.";
            }

            return dataDict;
        }

        public ITimeSeriesOutput MergeTimeseries(ITimeSeriesOutput firstOutput, ITimeSeriesOutput secondOutput)
        {
            firstOutput.Data = firstOutput.Data.Concat(secondOutput.Data).GroupBy(k => k.Key).ToDictionary(g => g.Key, g => g.First().Value);
            return firstOutput;
        }


        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the GLDAS endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            try
            {
                WebRequest wr = WebRequest.Create(ConstructURL(out string errorMsg, dataset, testInput)[0]);
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
    }
}