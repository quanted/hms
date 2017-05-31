using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSLandSurfaceFlow
    {
        /// <summary>
        /// Gets landsurfaceflow data using the parameters in the dictionary.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetLandSurfaceFlowData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // LandSurfaceFlow Initiailization
            HMSLandSurfaceFlow.LandSurfaceFlow sflow;
            if (parameters["source"] == "compare")
            {
                return GetLandSurfaceFlowCompareData(out errorMsg, parameters);
            }
            else
            {
                if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                {
                    sflow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                }
                else if (parameters.ContainsKey("filePath"))
                {
                    sflow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
                }
                else if (parameters.ContainsKey("geojson"))
                {
                    sflow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                    sflow.gdal.geoJSON = parameters["geojson"];
                }
                else
                {
                    return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                }

                if (errorMsg.Contains("ERROR"))
                {
                    return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
                }
                sflow.GetDataSetsObject(out errorMsg);
                if (errorMsg.Contains("ERROR"))
                {
                    return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
                }
                return sflow.jsonData;
            }
        }

        /// <summary>
        /// Gets landsurfaceflow comparision data using the latitude/longitude values in parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private HMSJSON.HMSJSON.HMSData GetLandSurfaceFlowCompareData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSJSON.HMSJSON.HMSData results = new HMSJSON.HMSJSON.HMSData();
            HMSUtils.Utils utils = new HMSUtils.Utils();

            if (utils.CompareDates(out errorMsg, parameters["startDate"], parameters["endDate"]) < 0)
            {
                return utils.ReturnError("ERROR: Date error, invalid date range provided.");
            }

            List<HMSJSON.HMSJSON.HMSData> list = new List<HMSJSON.HMSJSON.HMSData>();
            HMSJSON.HMSJSON totals = new HMSJSON.HMSJSON();

            // Get nldas precip data.
            HMSLandSurfaceFlow.LandSurfaceFlow nldasRunoff = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], "nldas", true, "");
            nldasRunoff.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, nldasRunoff.jsonData, "daily"));

            // Get gldas precip data.
            HMSLandSurfaceFlow.LandSurfaceFlow gldasRunoff = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], "gldas", true, "", nldasRunoff.gmtOffset.ToString(), nldasRunoff.tzName);
            gldasRunoff.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, gldasRunoff.jsonData, "daily"));

            // Get curve number data.
            HMSLandSurfaceFlow.LandSurfaceFlow cnRunoff = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], "curvenumber", true, "", nldasRunoff.gmtOffset.ToString(), nldasRunoff.tzName);
            cnRunoff.GetDataSetsObject(out errorMsg);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }
            list.Add(totals.CollectDataTotals(out errorMsg, cnRunoff.jsonData, "daily"));

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