using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSTotalFlow
    {

        /// <summary>
        /// Gets total flow data using the parameters in the dictionary.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetTotalFlowData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // TotalFlow Initialization
            HMSTotalFlow.TotalFlow tflow;
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                tflow = new HMSTotalFlow.TotalFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
            }
            else if (parameters.ContainsKey("filePath"))
            {
                tflow = new HMSTotalFlow.TotalFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
            }
            else if (parameters.ContainsKey("geojson"))
            {
                tflow = new HMSTotalFlow.TotalFlow(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                tflow.gdal.geoJSON = parameters["geojson"];
            }
            else
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
            }

            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
            }
            tflow.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
            }
            return tflow.jsonData;
        }
    }
}