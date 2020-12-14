using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace Data.Source
{
    /// <summary>
    /// Base TRMM class.
    /// </summary>
    public class TRMM
    {

        private DateTime temporalStart = new DateTime(1998, 1, 1);          // TRMM temporal coverage start date.
        private DateTime temporalEnd = new DateTime(DateTime.UtcNow.Year, 12, 31).AddYears(-1);          // TRMM temporal coverage end date (approximate)
        private double maxLat = 50.0;                                       // TRMM spatial coverage max latitude
        private double minLat = -50.0;                                       // TRMM spatial coverage min latitude
        private double maxLng = 180.0;                                      // TRMM spatial coverage max longitude
        private double minLng = -180.0;                                     // TRMM spatial coverage min longitude

        /// <summary>
        /// Get data function for TRMM.
        /// </summary>
        /// <param name="errorMsg"></param>s
        /// <param name="dataset">trmm dataset parameter</param>
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
            componentInput.DateTimeSpan = TRMM.AdjustForOffset(out errorMsg, componentInput) as DateTimeSpan;

            // Constructs the url for the TRMM data request and it's query string.
            string url = ConstructURL(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Uses the constructed url to download time series data.
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
                errors.Append("ERROR: Invalid start date, TRMM temporal coverage starts at " + this.temporalStart.ToString("yyyy-MM-dd") + ", entered start date is " + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + ", ");
            }
            if (input.DateTimeSpan.EndDate > this.temporalEnd)
            {
                valid = false;
                errors.Append("ERROR: Invalid end date, TRMM temporal coverage ends at " + this.temporalEnd.ToString("yyyy-MM-dd") + ", entered end date is " + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + ", ");
            }
            // Spatial calidation
            if (input.Geometry.Point.Latitude < this.minLat || input.Geometry.Point.Latitude > this.maxLat)
            {
                valid = false;
                errors.Append("ERROR: Invalid latitude, TRMM spatial coverage is between latitudes " + this.minLat.ToString() + " and " + this.maxLat.ToString() + ", entered latitude is " + input.Geometry.Point.Latitude.ToString() + ", ");
            }
            if (input.Geometry.Point.Longitude < this.minLng || input.Geometry.Point.Longitude > this.maxLng)
            {
                valid = false;
                errors.Append("ERROR: Invalid longitude, TRMM spatial coverage is between longitude " + this.minLng.ToString() + " and " + this.maxLng.ToString() + ", entered longitude is " + input.Geometry.Point.Longitude.ToString() + ", ");
            }

            if (!valid)
            {
                errorMsg = errors.ToString();
            }
            return valid;
        }

        /// <summary>
        /// Adjusts the DateTimeSpan object to account for timezone offset. 
        /// TRMM Date/Times are in GMT.
        /// Adjustment effects range of the requested data but not the date values of the data. Date values are altered with SetDateToLocal()
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="cInput">ITimeSeriesInput</param>
        /// <returns></returns>
        public static IDateTimeSpan AdjustForOffset(out string errorMsg, ITimeSeriesInput cInput)
        {
            //TODO: Add error handling for Timezone.Offset 

            errorMsg = "";
            IDateTimeSpan dateTime = cInput.DateTimeSpan;

            if (cInput.Geometry.Timezone.Offset < 0.0 && cInput.TimeLocalized == true)
            {
                dateTime.StartDate = new DateTime(dateTime.StartDate.Year, dateTime.StartDate.Month, dateTime.StartDate.Day, 1 + Convert.ToInt16(System.Math.Abs(cInput.Geometry.Timezone.Offset)), 00, 00);
            }
            else if (cInput.Geometry.Timezone.Offset > 0.0 && cInput.TimeLocalized == true)
            {
                dateTime.StartDate = dateTime.StartDate.AddDays(-1.0);
                dateTime.StartDate = new DateTime(dateTime.StartDate.Year, dateTime.StartDate.Month, dateTime.StartDate.Day, 24 - Convert.ToInt16(cInput.Geometry.Timezone.Offset), 00, 00);
            }
            else
            {
                dateTime.StartDate = new DateTime(dateTime.StartDate.Year, dateTime.StartDate.Month, dateTime.StartDate.Day, 01, 00, 00);
            }

            if (cInput.Geometry.Timezone.Offset < 0.0 && cInput.TimeLocalized == true)
            {
                dateTime.EndDate = dateTime.EndDate.AddDays(1.0);
                dateTime.EndDate = new DateTime(dateTime.EndDate.Year, dateTime.EndDate.Month, dateTime.EndDate.Day, Convert.ToInt16(System.Math.Abs(cInput.Geometry.Timezone.Offset)), 00, 00);
            }
            else if (cInput.Geometry.Timezone.Offset > 0.0 && cInput.TimeLocalized == true)
            {
                dateTime.EndDate = new DateTime(dateTime.EndDate.Year, dateTime.EndDate.Month, dateTime.EndDate.Day, 24 - Convert.ToInt16(cInput.Geometry.Timezone.Offset), 00, 00);
            }
            else
            {
                dateTime.EndDate = dateTime.EndDate.AddDays(1.0);
                dateTime.EndDate = new DateTime(dateTime.EndDate.Year, dateTime.EndDate.Month, dateTime.EndDate.Day, 00, 00, 00);
            }
            return dateTime;
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

        /// <summary>
        /// Constructs the url for retrieving TRMM data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            // Example base url: https://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=TRMM:TRMM_3B42.7:precipitation&location=GEOM:POINT
            string[] startDT = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
            DateTime tempDate = cInput.DateTimeSpan.EndDate.AddHours(3);
            string[] endDT = tempDate.ToString("yyyy-MM-dd HH").Split(' ');

            string url = cInput.BaseURL[0] +
                @"%28" + cInput.Geometry.Point.Longitude.ToString() +
                @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29" +
                @"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1] + @"&type=asc2";

            return url;
        }

        /// <summary>
        /// Downloads TRMM data from nasa servers. If Http Request fails will retry up to 5 times.
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
                if(retries < maxRetries)
                {
                    retries += 1;
                    Log.Warning("Error: Failed to download trmm data. Retry {0}:{1}", retries, maxRetries);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download trmm data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }

        /// <summary>
        /// Determines the coordinate values that corresponds to the closest coordinate point in the TRMM grid.
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
        /// Takes the data recieved from trmm and sets the ITimeSeries object values.
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
            output.Data = SetData(out errorMsg, splitData[1], input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);
            return output;
        }

        /// <summary>
        /// Parses data string from trmm and sets the metadata for the ITimeSeries object.
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
                        meta["trmm_" + line[0]] = line[1];
                    }
                }
            }
            return meta;
        }

        /// <summary>
        /// Parses data string from trmm and sets the data for the ITimeSeries object.
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
                if (tsLines[i].Contains("MEAN"))
                {
                    break;
                }
                string[] lineData = tsLines[i].Split(new string[] { "T", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                string key = TRMM.SetDateToLocal(offset, lineData[0] + " " + lineData[1].Replace("Z", ""), dateFormat);
                if (!dataDict.ContainsKey(key))
                {
                    timestepData = new List<string>();
                    timestepData.Add(Convert.ToDouble(lineData[2]).ToString(dataFormat));
                    dataDict[key] = timestepData;
                }
            }
            return dataDict;
        }

        public ITimeSeriesOutput MergeTimeseries(ITimeSeriesOutput firstOutput, ITimeSeriesOutput secondOutput)
        {
            firstOutput.Data = firstOutput.Data.Concat(secondOutput.Data).GroupBy(k => k.Key).ToDictionary(g => g.Key, g => g.First().Value);
            return firstOutput;
        }


        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the trmm endpoints.
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
    }
}