using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSSoilMoisture
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string dataSource { get; set; }
        public bool localTime { get; set; }
        public int[] layers { get; set; }


        /// <summary>
        /// Gets Soil Moisture data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public string GetSoilMoistureData(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName, string layers)
        {
            errorMsg = "";
            int[] layersArray = ConvertLayersString(out errorMsg, layers);
            HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName, layersArray);
            if (errorMsg.Contains("Error")) { return null; }
            string data = sm.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Soil Moisture data using latitude, longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public string GetSoilMoistureData(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime, string layers)
        {
            errorMsg = "";
            //string wsPath = HttpRuntime.AppDomainAppPath;
            int[] layersArray = ConvertLayersString(out errorMsg, layers);
            HMSSoilMoisture.SoilMoisture sm = new HMSSoilMoisture.SoilMoisture(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null, layersArray);
            if (errorMsg.Contains("Error")) { return null; }
            string data = sm.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
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