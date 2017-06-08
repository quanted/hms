using Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

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
        public ITimezone GetTimezone(out string errorMsg, IPointCoordinate point)
        {
            errorMsg = "";
            Dictionary<string, string> urls = (Dictionary<string, string>)HttpContext.Current.Application["urlList"];
            string url = urls["TIMEZONE_GEE_INT"];
            string queryString = "latitude=" + point.Latitude.ToString() + "&longitude=" + point.Longitude.ToString();
            string completeUrl = url + queryString;
            try
            {
                WebClient wc = new WebClient();
                byte[] buffer = wc.DownloadData(completeUrl);
                string resultString = Encoding.UTF8.GetString(buffer);
                Dictionary<string, string> tz = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resultString);
                return new Timezone() { Name = tz["tzName"], Offset = Convert.ToDouble(tz["tzOffset"]), DLS = false };
            }
            catch
            {
                string key = "AIzaSyDUdVJFt_SUwqfNTfziXXUFK7gkHxTnRIE";     // Personal google api key, to be replaced by project key
                string baseUrl = "https://maps.googleapis.com/maps/api/timezone/json?";
                string location = "location=" + point.Latitude.ToString() + "," + point.Longitude.ToString();
                string timeStamp = "timestamp=1331161200";
                completeUrl = baseUrl + location + "&" + timeStamp + "&key=" + key;
                try
                {
                    WebClient wc = new WebClient();
                    byte[] buffer = wc.DownloadData(completeUrl);
                    string resultString = Encoding.UTF8.GetString(buffer);
                    Dictionary<string, string> tz = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(resultString);
                    return new Timezone() { Name = tz["timeZoneId"], Offset = Convert.ToDouble(tz["rawOffset"]) / 3600, DLS = false };
                }
                catch (Exception ex)
                {
                    errorMsg = "ERROR: " + ex.Message;
                    return new Timezone();
                }
            }
        }
    }
}
