using HMSLDAS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HMSDaymet
{
    public class HMSDaymet
    {
         
        /// <summary>
        /// Gets Daymet data for the specified dataset using the variables provided by the IHMSModule object, and sets the value to the HMSTimeseries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="module"></param>
        /// <param name="dataset"></param>
        /// <param name="timeseries"></param>
        public void GetDaymetData(out string errorMsg, IHMSModule module, string dataset, HMSTimeSeries.HMSTimeSeries timeseries)
        {
            errorMsg = "";
            int totalPoints = 0;
            string data = "";
            if (module.shapefilePath != null)
            {
                totalPoints = module.gdal.coordinatesInShapefile.Count;
                for (int i = 0; i < totalPoints; i++)
                {
                    if (i != 0) { timeseries = new HMSTimeSeries.HMSTimeSeries(); module.ts.Add(timeseries); }
                    string url = GetDaymetURL(out errorMsg, module.dataSource, dataset) + BuildURLVariables(out errorMsg, module.startDate, module.endDate, module.gdal.coordinatesInShapefile[i].Item1, module.gdal.coordinatesInShapefile[i].Item2, dataset);             // Base url + variable string
                    data = RetrieveData(out errorMsg, url);
                    if (errorMsg.Contains("ERROR")) { return; }
                    timeseries.SetTimeSeriesVariables(out errorMsg, timeseries, data, module.dataSource);
                    timeseries.newMetaData = String.Concat(timeseries.newMetaData, "percentInCell=", module.gdal.areaPrecentInGeometry[i], "\nareaInCell=", module.gdal.areaGeometryIntersection[i], "\n");
                    if (errorMsg.Contains("ERROR")) { return; }
                }
            }
            else
            {
                string url = GetDaymetURL(out errorMsg, module.dataSource, dataset) + BuildURLVariables(out errorMsg, module.startDate, module.endDate, module.latitude, module.longitude, dataset);             // Base url + variable string
                if (errorMsg.Contains("ERROR")) { return; }
                data = RetrieveData(out errorMsg, url);
                if (errorMsg.Contains("ERROR")) { return; }
                timeseries.SetTimeSeriesVariables(out errorMsg, timeseries, data, module.dataSource);
                if (errorMsg.Contains("ERROR")) { return; }
            }
        }

        /// <summary>
        /// Returns the base url for the daymet data from the external URL file.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="datasource"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private string GetDaymetURL(out string errorMsg, string datasource, string dataSet)
        {
            errorMsg = "";
            string prepInfo = System.AppDomain.CurrentDomain.BaseDirectory + @"bin\url_info.txt";  // URL configuration info.
            string urlStr = "";
            string[] lineData;
            try
            {
                foreach( string line in File.ReadLines(prepInfo))
                {
                    lineData = line.Split(' ');
                    if (lineData[0].Equals(datasource +"_" + dataSet + "_URL", StringComparison.OrdinalIgnoreCase))
                    {
                        urlStr = lineData[1];
                        break;
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to load url details for Daymet.";
                return null;
            }
            return urlStr;
        }

        /// <summary>
        /// Constructs the query string for the Daymet url from the given variables.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private string BuildURLVariables(out string errorMsg, DateTime startDate, DateTime endDate, double latitude, double longitude, string dataset)
        {
            errorMsg = "";
            StringBuilder builder = new StringBuilder();
            builder.Append("lat=" + latitude + "&lon=" + longitude);                // Adds coordinates to url variable string
            builder.Append("&measuredParams=" + GetMeasuredParam(out errorMsg, dataset));                           // Adds dataset variable to string
            string years = GetListOfYears(out errorMsg, startDate, endDate);
            builder.Append("&year=" + years);         // Adds start and end dates, only takes years
            if (errorMsg.Contains("ERROR")) { return null; }
            return builder.ToString();
        }

        /// <summary>
        /// Downloads the Daymet data using the constructed URL.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RetrieveData(out string errorMsg, string url)
        {
            errorMsg = "";
            string data = "";
            WebClient myWC = new WebClient();
            try
            {
                byte[] dataBuffer = myWC.DownloadData(url);
                data = Encoding.UTF8.GetString(dataBuffer);
                myWC.Dispose();
            }
            catch
            {
                errorMsg = "ERROR: Unable to download data from Daymet.";
                return null;
            }
            return data;
        }

        /// <summary>
        /// Returns the measured parameter string used in constructing the URL for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private string GetMeasuredParam(out string errorMsg, string dataset)
        {
            errorMsg = "";
            switch (dataset)
            {
                case "Precip":
                    return "prcp";
                case "Temp":
                    return "tmax,tmin";
                default:
                    errorMsg = "ERROR: Parameter for Daynet did not load.";
                    return null;
            }
        }     

        private string GetListOfYears(out string errorMsg, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            StringBuilder st = new StringBuilder();
            int yearDif = (endDate.Year - startDate.Year);
            for (int i = 0; i <= yearDif; i++)
            {
                string year = startDate.AddYears(i).Year.ToString();
                st.Append(year + ",");
            }
            st.Remove(st.Length - 1, 1);
            return st.ToString();
        }
    }
}
