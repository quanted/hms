using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSLandSurfaceFlow
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string dataSource { get; set; }
        public bool localTime { get; set; }

        /// <summary>
        /// Gets Land Surface Flow data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <param name="shapefileName"></param>
        /// <returns></returns>
        public string GetLandSurfaceFlowData(out string errorMsg, string startDate, string endDate, string dataSource, bool localTime, string shapefileName)
        {
            errorMsg = "";
            HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, startDate, endDate, dataSource, localTime, shapefileName);
            if (errorMsg.Contains("Error")) { return null; }
            string data = lsFlow.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }

        /// <summary>
        /// Gets Land Surface Flow data using latitude, longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dataSource"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public string GetLandSurfaceFlowData(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string dataSource, bool localTime)
        {
            errorMsg = "";
            //string wsPath = HttpRuntime.AppDomainAppPath;
            HMSLandSurfaceFlow.LandSurfaceFlow lsFlow = new HMSLandSurfaceFlow.LandSurfaceFlow(out errorMsg, latitude, longitude, startDate, endDate, dataSource, localTime, null);
            if (errorMsg.Contains("Error")) { return null; }
            string data = lsFlow.GetDataSetsString(out errorMsg);
            if (errorMsg.Contains("Error")) { return null; }
            return data;
        }
    }
}