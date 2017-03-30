using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSBaseFlow
    {
        /// <summary>
        /// Gets baseflow data using the parameters in the dictionary .
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetBaseFlowData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // BaseFlow Initiailization
            HMSBaseFlow.BaseFlow bFlow;
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
            }
            else if (parameters.ContainsKey("filePath"))
            {
                bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
            }
            else if (parameters.ContainsKey("geojson"))
            {
                bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                bFlow.gdal.geoJSON = parameters["geojson"];
            }
            else
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
            }

            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
            }
            bFlow.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
            }
            return bFlow.jsonData;
        }
    }
}