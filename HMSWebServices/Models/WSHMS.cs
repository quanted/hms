using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSHMS
    {
        /// <summary>
        /// Gets the specified data, as 'dataset', using the parameters in the dictionary.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetHMSData(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            switch (parameters["dataset"])
            {
                case "baseflow":
                    HMSBaseFlow.BaseFlow bFlow;
                    if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                    {
                        bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        bFlow.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    bFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return bFlow.jsonData;
                case "evapotranspiration":
                    HMSEvapotranspiration.Evapotranspiration evapo;
                    if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                    {
                        evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        evapo.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    evapo.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return evapo.jsonData;
                case "landsurfaceflow":
                case "surfacerunoff":
                    HMSLandSurfaceFlow.LandSurfaceFlow sFlow;
                    if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                    {
                        sFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        sFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        sFlow.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    sFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return sFlow.jsonData;
                case "precipitaiton":
                    HMSPrecipitation.Precipitation precip;
                    if (parameters.ContainsKey("latitude"))
                    {
                        precip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        precip = new HMSPrecipitation.Precipitation(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        precip.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    precip.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return precip.jsonData;
                case "soilmoisture":
                    HMSSoilMoisture.SoilMoisture soilM;
                    int[] layers = ConvertLayersString(out errorMsg, parameters["layers"]);
                    if (parameters.ContainsKey("latitude"))
                    {
                        soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null, layers);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        soilM = new HMSSoilMoisture.SoilMoisture(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), layers);
                        soilM.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    soilM.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return soilM.jsonData;
                case "temperature":
                    HMSTemperature.Temperature temp;
                    if (parameters.ContainsKey("latitude"))
                    {
                        temp = new HMSTemperature.Temperature(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        temp = new HMSTemperature.Temperature(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        temp.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    temp.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return temp.jsonData;
                case "totalflow":
                    HMSTotalFlow.TotalFlow tFlow;
                    if (parameters.ContainsKey("latitude"))
                    {
                        tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
                    }
                    else if (parameters.ContainsKey("geojson"))
                    {
                        tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                        tFlow.gdal.geoJSON = parameters["geojson"];
                    }
                    else
                    {
                        return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
                    }
                    tFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                    return tFlow.jsonData;
                default:
                    return utils.ReturnError("ERROR: dataset provided not found.");
            }
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