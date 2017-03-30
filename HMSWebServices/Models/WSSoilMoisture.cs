using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSSoilMoisture
    {

        public HMSJSON.HMSJSON.HMSData GetSoilMoistureData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // Soilmoisture Initialization
            HMSSoilMoisture.SoilMoisture soilM;
            int[] layers = ConvertLayersString(out errorMsg, parameters["layers"]);
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null, layers);
            }
            else if (parameters.ContainsKey("filePath"))
            {
                soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"], layers);
            }
            else if (parameters.ContainsKey("geojson"))
            {
                soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), layers);
                soilM.gdal.geoJSON = parameters["geojson"];
            }
            else
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
            }
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
            }

            soilM.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
            }
            return soilM.jsonData;
        }

        /// <summary>
        /// Converts the argument string that contains the selected layers to an int[]
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="layerString"></param>
        /// <returns></returns>
        private int[] ConvertLayersString(out string errorMsg, string layerString)
        {
            errorMsg = "";
            int[] layerArray = new int[layerString.Length];
            try
            {
                layerArray = layerString.Select(c => c - '0').ToArray();
            }
            catch
            {
                errorMsg = "ERROR: Failed to convert layers argument.";
                return null;
            }
            return layerArray;
        }
    }
}