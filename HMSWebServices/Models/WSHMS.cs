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
            if (dataset.Contains("BaseFlow"))
            {
                HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = bFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("Evapotranspiration"))
            {
                HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = evapo.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("LandSurfaceFlow"))
            {
                HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = lsFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("Precipitation"))
            {
                HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = precip.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("SoilMoisture"))
            {
                int[] layersArray = ConvertLayersString(out errorMsg, layers);
                HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null, layersArray);
                if (errorMsg.Contains("Error")) { return null; }
                data = sm.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("Temperature"))
            {
                HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = temp.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("TotalFlow"))
            {
                HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
                if (errorMsg.Contains("Error")) { return null; }
                data = tFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            return data;
        }

        public string GetHMSData(out string errorMsg, string dataset, string startDate, string endDate, string dataSource, bool localTime, string shapefileName, string layers)
        {
            errorMsg = "";
            string data = "";
            if (dataset.Contains("BaseFlow"))
            {
                HMSBaseFlow.BaseFlow bFlow = new HMSBaseFlow.BaseFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = bFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("Evapotranspiration"))
            {
                HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = evapo.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("LandSurfaceFlow"))
            {
                HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = lsFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("Precipitation"))
            {
                HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = precip.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("SoilMoisture"))
            {
                int[] layersArray = ConvertLayersString(out errorMsg, layers);
                HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName, layersArray);
                if (errorMsg.Contains("Error")) { return null; }
                data = sm.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("TotalFlow"))
            {
                HMSTotalFlow.TotalFlow tFlow = new HMSTotalFlow.TotalFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = tFlow.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            else if (dataset.Contains("TotalFlow"))
            {
                HMSTemperature.Temperature temp = new HMSTemperature.Temperature(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
                if (errorMsg.Contains("Error")) { return null; }
                data = temp.GetDataSetsString(out errorMsg);
                if (errorMsg.Contains("Error")) { return null; }
            }
            return data;
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