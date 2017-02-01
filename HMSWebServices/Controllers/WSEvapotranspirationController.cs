using HMSWebServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMSWebServices.Controllers
{
    //public class Evapotranspiration
    //{
    //    public string ID { get; set; }                                  // GUID for specific session
    //    public string latitude { get; set; }                            // Latitude for timeseries
    //    public string longitude { get; set; }                           // Longitude for timeseries
    //    public string startDate { get; set; }                           // Start data for timeseries
    //    public string endDate { get; set; }                             // End date for timeseries
    //    public string source { get; set; }                              // NLDAS, GLDAS, or SWAT algorithm simulation
    //    public string localTime { get; set; }                           // False = GMT time, true = local time
    //    public string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
    //}

    public class WSEvapotranspirationController : ApiController
    {
        public Dictionary<string, string> parameters;

        /// <summary>
        /// Gets evapotranspiration data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("api/WSEvapotranspiration/{param}")]
        //public string Get(string param)
        //{
        //    string data = "";
        //    string errorMsg = "";
        //    Dictionary<string, string> parameters = ParseParameters(out errorMsg, param);
        //    if (errorMsg.Contains("Error")) { return errorMsg; }
        //    WSEvapotranspiration result = new WSEvapotranspiration();

        //    if (parameters.ContainsKey("latitude"))             //Location provided by latitude, longitude
        //    {
        //        data = result.GetEvapotranspirationData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
        //    }
        //    else if (param.Contains("ShapeFile"))       //Location provided by shapefile.
        //    {
        //        string shapefile = HttpContext.Current.Server.MapPath("~/TransientStorage/") + parameters["ID"] + ".zip";
        //        UnzipFile(out errorMsg, shapefile, parameters["ID"]);
        //        if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["ID"]); return errorMsg; }
        //        string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["ID"] + "\\" + parameters["shapefilePath"] + ".shp";
        //        data = result.GetEvapotranspirationData(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), unzippedShapefile);
        //        DeleteTempShapefiles(parameters["ID"]);
        //    }
        //    if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["ID"]); return errorMsg; }
        //    return data;
        //}

        /// <summary>
        /// Gets evapotranspiration data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/WSEvapotranspiration/")]
        //public string Post([FromBody]Evapotranspiration param)
        //{
        //    string data = "";
        //    string errorMsg = "";
        //    if (errorMsg.Contains("Error")) { return errorMsg; }
        //    WSEvapotranspiration result = new WSEvapotranspiration();

        //    if (String.IsNullOrWhiteSpace(param.shapefilePath))
        //    {
        //        data = result.GetEvapotranspirationData(out errorMsg, param.latitude, param.longitude, param.startDate, param.endDate, param.source, Convert.ToBoolean(param.localTime));
        //    }
        //    else
        //    {
        //        string shapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + param.ID + ".zip";
        //        UnzipFile(out errorMsg, shapefile, param.ID);
        //        if (errorMsg.Contains("Error")) { DeleteTempShapefiles(param.ID); return errorMsg; }
        //        string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + param.ID + "\\" + param.shapefilePath + ".shp";
        //        data = result.GetEvapotranspirationData(out errorMsg, param.startDate.ToString(), param.endDate.ToString(), param.source, Convert.ToBoolean(param.localTime), unzippedShapefile);
        //        DeleteTempShapefiles(param.ID);
        //    }
        //    if (errorMsg.Contains("Error")) { DeleteTempShapefiles(param.ID); return errorMsg; }
        //    return data;
        //}

        [HttpPost]
        [Route("api/WSEvapotranspiration/")]
        public async Task<string> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string guid = Guid.NewGuid().ToString();

            string fileSaveLocation = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + guid);
            Directory.CreateDirectory(fileSaveLocation);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            parameters = new Dictionary<string, string>();
            parameters.Add("id", guid);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        parameters.Add(key, val);
                    }
                }

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e).ToString();
            }
            string data = RetrieveData();
            Directory.Delete(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]), true);
            return data;
        }

        /// <summary>
        /// Parses the parameters in the param string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="paramOne"></param>
        /// <returns></returns>
        //private Dictionary<string, string> ParseParameters(out string errorMsg, string param)
        //{
        //    errorMsg = "";
        //    Dictionary<string, string> variables = new Dictionary<string, string>();
        //    string[] values = param.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        string[] line = values[i].Split('=');
        //        variables.Add(line[0], line[1]);
        //    }
        //    return variables;
        //}

        /// <summary>
        /// Extracts the contents of the zip file containing the shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="zipPath"></param>
        private void UnzipFile(out string errorMsg, string zipPath, string sessionGUID)
        {
            errorMsg = "";
            string extractPath = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID);
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
            { }
        }

        /// <summary>
        /// Executes methods for retieving data using the values in the parameters variable.
        /// </summary>
        /// <returns></returns>
        private string RetrieveData()
        {
            string data = "";
            string errorMsg = "";
            WSEvapotranspiration result = new WSEvapotranspiration();

            if (parameters.ContainsKey("shapefile"))
            {
                if (!String.IsNullOrWhiteSpace(parameters["shapefile"]))
                {
                    string shapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["id"] + "\\" + parameters["shapefile"] + ".zip";
                    UnzipFile(out errorMsg, shapefile, parameters["id"]);
                    if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["id"]); return errorMsg; }
                    string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["id"] + "\\" + parameters["shapefile"] + ".shp";
                    data = result.GetEvapotranspirationData(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), unzippedShapefile);
                }
                else if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))
                {
                    data = result.GetEvapotranspirationData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                }
                else
                {
                    data = "Error: Valid location parameters must be provided."; return data;
                }
            }
            else if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))
            {
                data = result.GetEvapotranspirationData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
            }
            else
            {
                data = "Error: Valid location parameters must be provided."; return data;
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["id"]); return errorMsg; }
            return data;
        }
    }
}