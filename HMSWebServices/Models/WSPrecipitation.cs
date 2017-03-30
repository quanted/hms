using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSPrecipitation
    {
        /// <summary>
        /// Gets precipitation data using the parameters in the dictionary.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetPrecipitationData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // Precipitation Initialization
            HMSPrecipitation.Precipitation precip;
            if (parameters["source"] == "compare")
            {
                return GetPrecipitationCompareData(parameters);
            }
            else
            {
                if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                {
                    precip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                }
                else if (parameters.ContainsKey("filePath"))
                {
                    precip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
                }
                else if (parameters.ContainsKey("geojson"))
                {
                    precip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                    precip.gdal.geoJSON = parameters["geojson"];
                }
                else if (parameters.ContainsKey("stationID"))
                {
                    precip = new HMSPrecipitation.Precipitation();
                    // Place methods for retrieving ncdc data
                }
                else
                {
                    return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                }

                if (errorMsg.Contains("ERROR"))
                {
                    return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
                }
                precip.GetDataSetsObject(out errorMsg);
                if (errorMsg.Contains("ERROR"))
                {
                    return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
                }
                return precip.jsonData;
            }
        }

        /// <summary>
        /// Gets precipitation comparision data using the stationID provided in parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private HMSJSON.HMSJSON.HMSData GetPrecipitationCompareData(Dictionary<string, string> parameters)
        {
            string errorMsg = "";
            HMSJSON.HMSJSON.HMSData results = new HMSJSON.HMSJSON.HMSData();
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // Get station details 
            Dictionary<string, string> stationDetails = utils.GetNCDCStationDetails(out errorMsg, parameters["stationID"]);
            // If ERROR in retrieving station details, returns errorMsg in json object
            if (stationDetails.ContainsKey("errorMsg") || stationDetails.Count == 0)
            {
                if (!stationDetails.ContainsKey("errorMsg"))
                {
                    return utils.ReturnError("ERROR: Invalid stationID.");
                }
                else
                {
                    return utils.ReturnError(errorMsg);

                }
            }
            // Compare date values from station details to start and end date
            if (utils.CompareDates(out errorMsg, parameters["startDate"], stationDetails["mindate"]) < 0 || utils.CompareDates(out errorMsg, parameters["endDate"], stationDetails["maxdate"]) > 0)
            {
                if (errorMsg != "")
                {
                    return utils.ReturnError(errorMsg);
                }
                return utils.ReturnError("ERROR: Date ERROR, invalid date range provided for the selected station.");
              
            }

            List<HMSJSON.HMSJSON.HMSData> list = new List<HMSJSON.HMSJSON.HMSData>();
            HMSJSON.HMSJSON totals = new HMSJSON.HMSJSON();

            // Get nldas precip data.
            HMSPrecipitation.Precipitation nldasPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], parameters["startDate"], parameters["endDate"], "NLDAS", true, null);
            nldasPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, nldasPrecip.jsonData, "daily"));
            // Get gldas precip data.
            HMSPrecipitation.Precipitation gldasPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], parameters["startDate"], parameters["endDate"], "GLDAS", true, "", nldasPrecip.gmtOffset.ToString(), nldasPrecip.tzName);
            gldasPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, gldasPrecip.jsonData, "daily"));
            // Get daymet precip data.
            HMSPrecipitation.Precipitation daymetPrecip = new HMSPrecipitation.Precipitation(out errorMsg, stationDetails["latitude"], stationDetails["longitude"], parameters["startDate"], parameters["endDate"], "Daymet", true, "", nldasPrecip.gmtOffset.ToString(), nldasPrecip.tzName);
            daymetPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(daymetPrecip.jsonData);
            // Get ncdc precip data.
            HMSPrecipitation.Precipitation ncdcPrecip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["startDate"], parameters["endDate"], "NCDC", parameters["stationID"]);
            ncdcPrecip.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, ncdcPrecip.jsonData, "daily"));

            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            HMSJSON.HMSJSON result = new HMSJSON.HMSJSON();
            results = result.MergeHMSDataList(out errorMsg, list);

            return results;
        }
    }
}