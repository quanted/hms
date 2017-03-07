using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSEvapotranspiration
    {

        /// <summary>
        /// Gets Evapotranspiration data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public string GetEvapotranspirationData(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return null; }
            string data = evapo.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Evapotranspiration data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public string GetEvapotranspirationData(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return null; }
            string data = evapo.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Evapotranspiration data using latitude/longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetEvapotranspirationDataObject(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            evapo.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return  new HMSJSON.HMSJSON.HMSData(); }
            return evapo.jsonData;
        }

        /// <summary>
        /// Gets Evapotranspiration data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData GetEvapotranspirationDataObject(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSEvapotranspiration.Evapotranspiration evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            evapo.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return evapo.jsonData;
        }
    }
}