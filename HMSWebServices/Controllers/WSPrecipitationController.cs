using System;
using System.Collections.Generic;
using System.Web.Http;
using HMSWebServices.Models;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace HMSWebServices.Controllers
{
    public class Precipitation
    {
        public string ID { get; set; }                                  // GUID for specific session
        public string latitude { get; set; }                            // Latitude for timeseries
        public string longitude { get; set; }                           // Longitude for timeseries
        public string startDate { get; set; }                           // Start data for timeseries
        public string endDate { get; set; }                             // End date for timeseries
        public string source { get; set; }                              // NLDAS, GLDAS, or SWAT algorithm simulation
        public string localTime { get; set; }                           // False = GMT time, true = local time
        public string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
    }

    public class WSPrecipitationController : ApiController
    {
        /// <summary>
        /// Gets precipitaiton data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSPrecipitation/{param}")]
        public string Get(string param)
        {
            string data = "";
            string errorMsg = "";
            Dictionary<string, string> parameters = ParseParameters(out errorMsg, param);
            if (errorMsg.Contains("Error")) { return errorMsg; }
            WSPrecipitation result = new WSPrecipitation();

            if (parameters.ContainsKey("latitude"))             //Location provided by latitude, longitude
            {
                data = result.GetPrecipitationData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
            }
            else if (parameters.ContainsKey("shapefilePath"))       //Location provided by shapefile.
            {
                string shapefile = HttpContext.Current.Server.MapPath("~/TransientStorage/") + parameters["ID"] + ".zip";
                UnzipFile(out errorMsg, shapefile, parameters["ID"]);
                if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["ID"]); return errorMsg; }
                string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["ID"] + "\\" + parameters["shapefilePath"] + ".shp";
                data = result.GetPrecipitationData(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), unzippedShapefile);
                DeleteTempShapefiles(parameters["ID"]);
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["ID"]); return errorMsg; }
            return data;
        }

        /// <summary>
        /// Gets precipitaiton data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSPrecipitation/")]
        public string Post([FromBody]Precipitation param)
        {
            string data = "";
            string errorMsg = "";
            if (errorMsg.Contains("Error")) { return errorMsg; }
            WSPrecipitation result = new WSPrecipitation();

            if (String.IsNullOrWhiteSpace(param.shapefilePath))
            {
                data = result.GetPrecipitationData(out errorMsg, param.latitude, param.longitude, param.startDate, param.endDate, param.source, Convert.ToBoolean(param.localTime));
            }
            else
            {
                string shapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + param.ID + ".zip";
                UnzipFile(out errorMsg, shapefile, param.ID);
                if (errorMsg.Contains("Error")) { DeleteTempShapefiles(param.ID); return errorMsg; }
                string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + param.ID + "\\" + param.shapefilePath + ".shp";
                data = result.GetPrecipitationData(out errorMsg, param.startDate.ToString(), param.endDate.ToString(), param.source, Convert.ToBoolean(param.localTime), unzippedShapefile);
                DeleteTempShapefiles(param.ID);
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(param.ID); return errorMsg; }
            return data;
        }

        /// <summary>
        /// Parses the parameters in the param string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="paramOne"></param>
        /// <returns></returns>
        private Dictionary<string,string> ParseParameters(out string errorMsg, string param)
        {
            errorMsg = "";
            Dictionary<string, string> variables = new Dictionary<string, string>();
            string[] values = param.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < values.Length; i++)
            {
                string[] line = values[i].Split('=');
                variables.Add(line[0], line[1]);
            }
            return variables;
        }

        /// <summary>
        /// Extracts the contents of the zip file containing the shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="zipPath"></param>
        private void UnzipFile(out string errorMsg, string zipPath, string sessionGUID)
        {
            errorMsg = "";
            string extractPath = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID );
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                errorMsg = "Error: " + ex;
                return;
            }
        }

        /// <summary>
        /// Deletes extracted shapefiles for cleanup.
        /// </summary>
        private void DeleteTempShapefiles(string sessionGUID)
        {
            try
            {
                Directory.Delete(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID + "\\"), true);
            }
            catch
            {}
        }
    }
}