using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class Daymet
    {

        private DateTime temporalStart = new DateTime(1980, 1, 1);          // Daymet temporal coverage start date.
        private DateTime temporalEnd = new DateTime(DateTime.UtcNow.Year, 12, 31).AddYears(-1);      // Daymet temporal coverage end date (approximate)
        private double maxLat = 52.0;                                       // Daymet spatial coverage max latitude
        private double minLat = 14.5;                                       // Daymet spatial coverage min latitude
        private double maxLng = -53.0;                                      // Daymet spatial coverage max longitude
        private double minLng = -131.0;                                     // Daymet spatial coverage min longitude

        /// <summary>
        /// GetData function for Daymet base class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetData(out string errorMsg, string dataSet, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";

            if (!this.ValidateInput(out errorMsg, input))
            {
                return null;
            }

            string url = ConstructURL(out errorMsg, dataSet, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(url, retries).Result;
            if (errorMsg.Contains("ERROR")) { return null; }

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
                errors.Append("ERROR: Invalid start date, Daymet temporal coverage starts at " + this.temporalStart.ToString("yyyy-MM-dd") + ", entered start date is " + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + ", ");
            }
            if (input.DateTimeSpan.EndDate > this.temporalEnd)
            {
                valid = false;
                errors.Append("ERROR: Invalid end date, Daymet temporal coverage ends at " + this.temporalEnd.ToString("yyyy-MM-dd") + ", entered end date is " + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + ", ");
            }
            // Spatial calidation
            if (input.Geometry.Point.Latitude < this.minLat || input.Geometry.Point.Latitude > this.maxLat)
            {
                valid = false;
                errors.Append("ERROR: Invalid latitude, Daymet spatial coverage is between latitudes " + this.minLat.ToString() + " and " + this.maxLat.ToString() + ", entered latitude is " + input.Geometry.Point.Latitude.ToString() + ", ");
            }
            if (input.Geometry.Point.Longitude < this.minLng || input.Geometry.Point.Longitude > this.maxLng)
            {
                valid = false;
                errors.Append("ERROR: Invalid longitude, Daymet spatial coverage is between longitude " + this.minLng.ToString() + " and " + this.maxLng.ToString() + ", entered longitude is " + input.Geometry.Point.Longitude.ToString() + ", ");
            }

            if (!valid)
            {
                errorMsg = errors.ToString();
            }
            return valid;
        }

        /// <summary>
        /// Downloads the Daymet data using the constructed URL.
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
                    Log.Warning("Error: Failed to download daymet data. Retry {0}:{1}", retries, maxRetries);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download daymet data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }

        /// <summary>
        /// Constructs the url for Daymet rest API.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, string dataSet, ITimeSeriesInput input)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            if(input.BaseURL == null)
            {
                errorMsg = "ERROR: Unable to build url for dataset = " + dataSet + " and source = " + input.Source;
                return null;
            }
            sb.Append(input.BaseURL[0]);
            sb.Append("lat=" + input.Geometry.Point.Latitude + "&lon=" + input.Geometry.Point.Longitude);                   // Adds coordinates to url variable string
            sb.Append("&measuredParams=" + GetMeasuredParam(out errorMsg, dataSet));                                        // Adds dataset variable to string
            string years = GetListOfYears(out errorMsg, input.DateTimeSpan.StartDate, input.DateTimeSpan.EndDate);
            sb.Append("&year=" + years);                                                                                    // Adds start and end dates, only takes years
            if (errorMsg.Contains("ERROR")) { return null; }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the measured parameter string used in constructing the URL for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private static string GetMeasuredParam(out string errorMsg, string dataset)
        {
            errorMsg = "";
            switch (dataset.ToLower())
            {
                case "precip":
                case "precipitation":
                    return "prcp";
                case "temp":
                case "temperature":
                    return "tmax,tmin";
                case "radiation":
                case "rad":
                    return "srad";
                default:
                    return dataset;
            }
        }

        /// <summary>
        /// Converts date parameters to list of years.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static string GetListOfYears(out string errorMsg, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            StringBuilder st = new StringBuilder();
            int yearDif = (endDate.Year - startDate.Year);
            for (int i = 0; i <= yearDif; i++)
            {
                string year = startDate.AddYears(i).Year.ToString();
                st.Append(year + ",");
            }
            st.Remove(st.Length - 1, 1);
            return st.ToString();
        }

        /// <summary>
        /// Constructs the ITimeSeriesOutput Data and MetaData object from the data string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataSet, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            List<string> splitData = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int j = splitData.FindIndex(x => x.Contains("year,yday"));

            //string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)" }, StringSplitOptions.RemoveEmptyEntries);
            output.Metadata = SetMetaData(out errorMsg, String.Join("\n", splitData.GetRange(0, j)));
            if (errorMsg.Contains("ERROR")) { return null; }

            double modifier = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            Dictionary<DateTime, List<string>> outputTemp = this.SetData(out errorMsg, splitData, j, input.DataValueFormat, input.DateTimeSpan.DateTimeFormat, input.Geometry.GeometryMetadata, modifier, input.DateTimeSpan);
            if (errorMsg.Contains("ERROR")) { return null; }

            SortedDictionary<DateTime, List<string>> sortedData = new SortedDictionary<DateTime, List<string>>(outputTemp);
            if (input.Geometry.GeometryMetadata.ContainsKey("leapYear"))
            {
                // Daymet Leap Year MESS!
                // Inserts Dec 31st for leap years.
                for (int i = 0; i <= (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year); i++)
                {
                    DateTime date = sortedData.Keys.First().AddYears(i);

                    if (DateTime.IsLeapYear(date.Year) && sortedData.ContainsKey(new DateTime(date.Year, 12, 30)))
                    {
                        sortedData.Add(new DateTime(date.Year, 12, 31), new List<string>() { (0).ToString(input.DataValueFormat) });
                    }
                    if (i == (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year) - 1) { break; }
                }
            }
            Dictionary<string, List<string>> outputFinal = new Dictionary<string, List<string>>();
            foreach (DateTime key in sortedData.Keys)
            {
                outputFinal.Add(key.ToString(input.DateTimeSpan.DateTimeFormat), sortedData[key]);
            }
            int h = 2;
            foreach(string s in splitData[j].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                if(s != "year" && s != "yday")
                {
                    output.Metadata.Add("column_" + h, s);
                    h += 1;
                }
            }

            output.Data = outputFinal;
            return output;
        }

        /// <summary>
        /// Creates the metaData dictionary for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetaData(out string errorMsg, string metadata)
        {
            errorMsg = "";
            Dictionary<string, string> daymetMetadata = new Dictionary<string, string>();
            string[] metaLines = metadata.Split(new string[] { "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < metaLines.Length; i++)
            {
                if (metaLines[i].Contains("http"))
                {
                    daymetMetadata.Add("daymet_url_reference:", metaLines[i].Trim());
                }
                else if (metaLines[i].Contains(':'))
                {
                    string[] lineData = metaLines[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    daymetMetadata.Add("daymet_" + lineData[0].Trim(), lineData[1].Trim());
                }
            }
            return daymetMetadata;
        }

        /// <summary>
        /// Creates the timeseries dictionary for the daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="timeseries"></param>
        /// <param name="dataFormat"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        private Dictionary<DateTime, List<string>> SetData(out string errorMsg, List<string> timeseries, int index, string dataFormat, string dateFormat, Dictionary<string, string> geoMeta, double modifier, IDateTimeSpan dateSpan)
        {
            errorMsg = "";
            Dictionary<DateTime, List<string>> data = new Dictionary<DateTime, List<string>>();
            if (geoMeta.ContainsKey("leapYear"))
            {
                for (int i = index+1; i < timeseries.Count; i++)
                {
                    string[] lineData = timeseries[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime date = new DateTime(Convert.ToInt16(Convert.ToDouble(lineData[0])), 1, 1);

                    // Leap year dates have to be shifted by -1 day after Feb 28, due to Daymet not including Feb 29th
                    if (DateTime.IsLeapYear(date.Year) && date > new DateTime(date.Year, 2, 28))
                    {
                        date = date.AddDays(-1.0);
                    };

                    DateTime date2 = new DateTime();
                    date2 = date;
                    if (i > 0) { date2 = date.AddDays(Convert.ToDouble(lineData[1]) - 1); }
                    else { date2 = date; }
                    List<string> dset = new List<string>();
                    for(int j = 2; j < lineData.Length; j++)
                    {
                        dset.Add((modifier * Convert.ToDouble(lineData[j])).ToString(dataFormat));
                    }
                    if (date2.Date >= dateSpan.StartDate.Date && date2.Date <= dateSpan.EndDate.Date)
                    {
                        data.Add(date2, dset);
                    }
                }
            }
            else
            {
                for (int i = index+1; i < timeseries.Count; i++)
                {
                    string[] lineData = timeseries[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime date = new DateTime(Convert.ToInt16(Convert.ToDouble(lineData[0])), 1, 1);
                    DateTime date2;
                    List<string> dset = new List<string>();

                    if (i > 0) { date2 = date.AddDays(Convert.ToDouble(lineData[1]) - 1); }
                    else { date2 = date; }
                    for (int j = 2; j < lineData.Length; j++)
                    {
                        dset.Add((modifier * Convert.ToDouble(lineData[j])).ToString(dataFormat));
                    }
                    if (date2 >= dateSpan.StartDate && date2 <= dateSpan.EndDate)
                    {
                        data.Add(date2, dset);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the GLDAS endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            string url = ConstructURL(out string errorMsg, dataset, testInput);
            try
            {
                WebRequest wr = WebRequest.Create(url);
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
