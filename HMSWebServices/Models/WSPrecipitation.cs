using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSPrecipitation
    {

        /// <summary>
        /// Gets Precipitation data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public string GetPrecipitationData(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return null; }
            string data = precip.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Precipitation data using shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public string GetPrecipitationData(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return null; }
            string data = precip.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Precipitation data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetPrecipitationDataObject(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            precip.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return precip.jsonData;
        }

        /// <summary>
        /// Gets Precipitation data using shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetPrecipitationDataObject(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            precip.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return precip.jsonData;
        }

        public HMSJSON.HMSJSON.HMSData GetPrecipitationCompareDataObject(out string errorMsg, string startDate, string endDate, string stationID)
        {
            errorMsg = "";
            HMSJSON.HMSJSON.HMSData results = new HMSJSON.HMSJSON.HMSData();
            HMSUtils.Utils util = new HMSUtils.Utils();

            // Get station details 
            Dictionary<string, string> stationDetails = util.GetNCDCStationDetails(out errorMsg, stationID);
            // If error in retrieving station details, returns errorMsg in json object
            if (stationDetails.ContainsKey("errorMsg") || stationDetails.Count == 0)                     
            {
                if (!stationDetails.ContainsKey("errorMsg"))
                {
                    results.metadata = new Dictionary<string, string>{
                        { "errorMsg", "ERROR: Invalid stationID."}
                    };
                }
                else
                {
                    results.metadata = new Dictionary<string, string>{
                        { "errorMsg", stationDetails["errorMsg"]}
                    };
                }
                return results;
            }
            // Compare date values from station details to start and end date
            if (util.CompareDates(out errorMsg, startDate, stationDetails["mindate"]) < 0 || util.CompareDates(out errorMsg, endDate, stationDetails["maxdate"]) > 0)
            {
                if (errorMsg != "")
                {
                    results.metadata.Add("errorMsg", errorMsg);
                    return results;
                }
                results.metadata.Add("errorMsg", "ERROR: Date error, invalid date range provided for the selected station.");
                results.metadata.SelectMany(dict => stationDetails).ToDictionary(pair => "ncdc_" + pair.Key, pair => pair.Value);
                return results;
            }

            List<HMSJSON.HMSJSON.HMSData> list = new List<HMSJSON.HMSJSON.HMSData>();
            HMSJSON.HMSJSON totals = new HMSJSON.HMSJSON();

            // Get nldas precip data.
            HMSPrecipitation.Precipitation nldasPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], startDate, endDate, "NLDAS", true, null);
            nldasPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                results.metadata.Add("errorMsg", errorMsg);
                return results;
            }
            list.Add(totals.CollectDataTotals(out errorMsg, nldasPrecip.jsonData, "daily"));
            // Get gldas precip data.
            HMSPrecipitation.Precipitation gldasPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], startDate, endDate, "GLDAS", true, "", nldasPrecip.gmtOffset.ToString(), nldasPrecip.tzName);
            gldasPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                results.metadata.Add("errorMsg", errorMsg);
                return results;
            }
            list.Add(totals.CollectDataTotals(out errorMsg, gldasPrecip.jsonData, "daily"));
            // Get daymet precip data.
            HMSPrecipitation.Precipitation daymetPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], startDate, endDate, "Daymet", true, "", nldasPrecip.gmtOffset.ToString(), nldasPrecip.tzName);
            daymetPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                results.metadata.Add("errorMsg", errorMsg);
                return results;
            }
            list.Add(daymetPrecip.jsonData);
            // Get ncdc precip data.
            HMSPrecipitation.Precipitation ncdcPrecip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, "NCDC", stationID);
            ncdcPrecip.GetDataSetsObject(out errorMsg);
            //HMSJSON.HMSJSON.HMSData temp = new HMSJSON.HMSJSON.HMSData();
            //for (int i = 0; i < stationDetails.Count; i++)
            //{
            //   temp.metadata.Add("NCDC_" + stationDetails.Keys.ElementAt(i), stationDetails.Values.ElementAt(i));
            //}
            //temp.data = ncdcPrecip.jsonData.data;
            //temp.dataset = ncdcPrecip.jsonData.dataset;
            //temp.source = ncdcPrecip.jsonData.source;
            //ncdcPrecip.jsonData = temp;
            if (errorMsg != "")
            {
                results.metadata.Add("errorMsg", errorMsg);
                return results;
            }
            list.Add(totals.CollectDataTotals(out errorMsg, ncdcPrecip.jsonData, "daily"));

            if (errorMsg != "")
            {
                results.metadata.Add("errorMsg", errorMsg);
                return results;
            }
            HMSJSON.HMSJSON result = new HMSJSON.HMSJSON();
            results = result.MergeHMSDataList(out errorMsg, list);

            return results;
        }
    }
}