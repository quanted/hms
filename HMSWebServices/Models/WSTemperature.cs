using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSTemperature
    {
        /// <summary>
        /// Gets termpature data using the parameters in the dictionary.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetTemperatureData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // Temperature Initiailization
            HMSTemperature.Temperature temp;
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                temp = new HMSTemperature.Temperature(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
            }
            else if (parameters.ContainsKey("filePath"))
            {
                temp = new HMSTemperature.Temperature(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
            }
            else if (parameters.ContainsKey("geojson"))
            {
                temp = new HMSTemperature.Temperature(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                temp.gdal.geoJSON = parameters["geojson"];
            }
            else if (parameters.ContainsKey("stationID"))
            {
                temp = new HMSTemperature.Temperature();
            }
            else
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
            }

            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
            }
            temp.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
            }
            return temp.jsonData;

        }

    }
}