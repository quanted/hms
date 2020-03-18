using Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Globalization;

namespace Utilities
{
    public class Time
    {
        /// <summary>
        /// Gets timezone details, timezone name and offset, based on the coordinates in point.
        /// Function first attempts getting timezone details from google earth engine hms api, if it fails reverts to google maps api.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="point">IPointCoordinate containing a latitude and longitude value.</param>
        /// <returns></returns>
        public static ITimezone GetTimezone(out string errorMsg, IPointCoordinate point)
        {
            errorMsg = "";
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            string key = System.Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            string hereKey = System.Environment.GetEnvironmentVariable("HERE_API_KEY");
            if (key != null)
            {
                string baseUrl = "https://maps.googleapis.com/maps/api/timezone/json?";
                string location = "location=" + point.Latitude.ToString() + "," + point.Longitude.ToString();
                string timeStamp = "timestamp=1331161200";
                string completeUrl = baseUrl + location + "&" + timeStamp + "&key=" + key;
                try
                {
                    WebClient wc = new WebClient();
                    byte[] buffer = wc.DownloadData(completeUrl);
                    string resultString = Encoding.UTF8.GetString(buffer);
                    Dictionary<string, object> tz = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString);
                    return new Timezone() { Name = tz["timeZoneId"].ToString(), Offset = Convert.ToDouble(tz["rawOffset"].ToString()) / 3600, DLS = false };
                }
                catch (Exception ex)
                {
                    errorMsg = "ERROR: " + ex.Message;
                    return new Timezone();
                }
            }
            else if(hereKey != null)
            {
                string baseUrl = "https://reverse.geocoder.ls.hereapi.com/6.2/reversegeocode.json?";
                string location = "prox=" + point.Latitude.ToString() + "," + point.Longitude.ToString();
                string completeUrl = baseUrl + location + "&mode=retrieveAddresses&maxresults=1&gen=9&key=" + hereKey;
                try
                {
                    WebClient wc = new WebClient();
                    byte[] buffer = wc.DownloadData(completeUrl);
                    string resultString = Encoding.UTF8.GetString(buffer);
                    Dictionary<string, object> tz = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString);
                    return new Timezone() { Name = tz["timeZoneId"].ToString(), Offset = Convert.ToDouble(tz["rawOffset"].ToString()) / 3600, DLS = false };
                }
                catch (Exception ex)
                {
                    errorMsg = "ERROR: " + ex.Message;
                    return new Timezone();
                }
            }
            else
            {
                errorMsg = "ERROR: Unable to retrieve timezone data.";
                return new Timezone();
            }

        }

        /// <summary>
        /// Shift the timeseries by the specified shift, in hours.
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="data"></param>
        /// <param name="dateOutputFormat"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> TimeSeriesShift(double shift, Dictionary<string, List<string>> data, string dateOutputFormat)
        {
            CultureInfo enUS = new CultureInfo("en-US");
            Dictionary<string, List<string>> newTimeseries = new Dictionary<string, List<string>>();
            foreach(KeyValuePair<string, List<string>> d in data)
            {
                DateTime newDate = new DateTime();
                bool p = DateTime.TryParseExact(d.Key, dateOutputFormat, enUS, DateTimeStyles.None, out newDate);
                newDate = newDate.AddHours(shift);
                newTimeseries.Add(newDate.ToString(dateOutputFormat), d.Value);
            }
            return newTimeseries;
        }

    }
}
