using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSHMS
    {

        public string GetHMSData(out string errorMsg, string dataset, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime, string layers)
        {
            errorMsg = "";
            string data = "";
            switch (dataset)
            {
                case "BaseFlow":
                    HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = bFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Evapotranspiration":
                    HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = evapo.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "LandSurfaceFlow":
                    HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = lsFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Precipitation":
                    HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = precip.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "SoilMoisture":
                    int[] layersArray = ConvertLayersString(out errorMsg, layers);
                    HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null, layersArray);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = sm.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Temperature":
                    HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = temp.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "TotalFlow":
                    HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = tFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                default:
                    return data;
            }
        }

        public string GetHMSData(out string errorMsg, string dataset, string startDate, string endDate, string dataSource, bool localTime, string shapefileName, string layers)
        {
            errorMsg = "";
            string data = "";
            switch (dataset)
            {
                case "BaseFlow":
                    HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = bFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Evapotranspiration":
                    HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = evapo.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "LandSurfaceFlow":
                    HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = lsFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Precipitation":
                    HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = precip.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "SoilMoisture":
                    int[] layersArray = ConvertLayersString(out errorMsg, layers);
                    HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName, layersArray);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = sm.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "TotalFlow":
                    HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = tFlow.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                case "Temperature":
                    HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return null; }
                    data = temp.GetDataSetsString(out errorMsg);
                    if (errorMsg.Contains("Error")) { return null; }
                    return data;
                default:
                    return data;
            }
        }

        public HMSJSON.HMSJSON.HMSData GetHMSDataObject(out string errorMsg, string dataset, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime, string layers)
        {
            errorMsg = "";
            switch (dataset) {
                case "BaseFlow":
                case "baseflow":
                    HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    bFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return bFlow.jsonData;
                case "Evapotranspiration":
                case "evapotranspiration":
                    HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    evapo.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return evapo.jsonData;
                case "LandSurfaceFlow":
                case "landsurfaceflow":
                case "surfacerunoff":
                    HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    lsFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return lsFlow.jsonData;
                case "Precipitation":
                case "precipitation":
                    HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    precip.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return precip.jsonData;
                case "SoilMoisture":
                case "soilmoisture":
                    int[] layersArray = ConvertLayersString(out errorMsg, layers);
                    HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null, layersArray);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    sm.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return sm.jsonData;
                case "Temperature":
                case "temperature":
                    HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    temp.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return temp.jsonData;
                case "TotalFlow":
                case "totalflow":
                    HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    tFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return tFlow.jsonData;
                default:
                    return new HMSJSON.HMSJSON.HMSData();

            }
        }

        public HMSJSON.HMSJSON.HMSData GetHMSDataObject(out string errorMsg, string dataset, string startDate, string endDate, string dataSource, bool localTime, string shapefileName, string layers)
        {
            errorMsg = "";
            switch (dataset)
            {
                case "BaseFlow":
                    HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    bFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return bFlow.jsonData;
                case "Evapotranspiration":
                    HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    evapo.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return evapo.jsonData;
                case "LandSurfaceFlow":
                    HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    lsFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return lsFlow.jsonData;
                case "Precipitation":
                    HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    precip.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return precip.jsonData;
                case "SoilMoisture":
                    int[] layersArray = ConvertLayersString(out errorMsg, layers);
                    HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName, layersArray);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    sm.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return sm.jsonData;
                case "TotalFlow":
                    HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    tFlow.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return tFlow.jsonData;
                case "Temperature":
                    HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    temp.GetDataSetsObject(out errorMsg);
                    if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                    return temp.jsonData;
                default:
                    return new HMSJSON.HMSJSON.HMSData();
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
                errorMsg = "Error: Failed to convert layers argument.";
                return null;
            }
            return layerArray;
        }
    }
}