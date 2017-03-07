using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSPrecipitation
    {

        /// <summary>
        /// Gets Precipitation data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public string GetPrecipitationData(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return null; }
            string data = precip.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Precipitation data using shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public string GetPrecipitationData(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return null; }
            string data = precip.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Precipitation data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetPrecipitationDataObject(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            precip.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return precip.jsonData;
        }

        /// <summary>
        /// Gets Precipitation data using shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetPrecipitationDataObject(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSPrecipitation.Precipitation precip = new HMSPrecipitation.Precipitation(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            precip.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return precip.jsonData;
        }
    }
}