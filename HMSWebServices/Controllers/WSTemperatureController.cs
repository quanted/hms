using HMSWebServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMSWebServices.Controllers
{
    public class WSTemperatureController : ApiController
    {
        public Dictionary<string, string> parameters;

        /// <summary>
        /// Evaluates the parameters provided by the POST body, ensuring all required parameters are present.
        /// </summary>
        /// <param name="errorMsg"></param>
        private void ParameterCheck(out string errorMsg)
        {
            errorMsg = "";
            if ((parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude")))
            {
                if ((String.IsNullOrWhiteSpace(parameters["latitude"])) || (String.IsNullOrWhiteSpace(parameters["longitude"])))
                {
                    errorMsg = "Error: No latitude or longitude value provided.";
                }
            }
            else if (parameters.ContainsKey("filePath"))
            {
                if ((String.IsNullOrWhiteSpace(parameters["filePath"])) && !parameters.ContainsKey("geoJson"))
                {
                    errorMsg += "Error: No file provided.";
                }
            }
            else if (parameters.ContainsKey("geoJson"))
            {
                if (parameters.ContainsKey("geoJson"))
                {
                    parameters.Add("filePath", HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]) + "\\geo.json");
                    System.IO.File.WriteAllText(parameters["filePath"], parameters["geoJson"]);
                }
            }
            else
            {
                errorMsg += "Error: No valid data location values provided. Both a latitude and longitude value must be given OR a shapefile provided OR a geoJson (as a parameter or json file).\n";
            }
            if (!(parameters.ContainsKey("startDate")))
            {
                errorMsg += "Error: A start date must be provided.\n";
            }
            if (!(parameters.ContainsKey("endDate")))
            {
                errorMsg += "Error: An end date must be provided.\n";
            }
            if (parameters.ContainsKey("source"))
            {
                if (String.IsNullOrWhiteSpace(parameters["source"]))
                {
                    errorMsg += "Error: A valid data source must be provided.";
                }
            }
            else
            {
                errorMsg += "Error: A data source must be provided.";
            }
            if (!parameters.ContainsKey("localTime"))
            {
                parameters.Add("localTime", "false");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(parameters["localTime"]))
                {
                    parameters["localTime"] = "false";
                }
            }
            if (!parameters.ContainsKey("layers"))
            {
                parameters.Add("layers", "0");
            }

        }

        /// <summary>
        /// Gets temperature data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSTemperature/{param}")]
        public string Get(string param)
        {
            string data = "";
            string errorMsg = "";
            Dictionary<string, string> parameters = ParseParameters(out errorMsg, param);
            if (errorMsg.Contains("Error")) { return errorMsg; }
            WSTemperature result = new WSTemperature();
            if (!parameters.ContainsKey("localTime")) { parameters.Add("localTime", "false"); }
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))             //Location provided by latitude, longitude
                {
                    data = result.GetTemperatureData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                }
                else
                {
                    errorMsg = "Error: Latitude and longitude must be provided.";
                }
            }
            else
            {
                errorMsg = "Error: Latitude and longitude must be provided.";
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["ID"]); return errorMsg; }
            return data;
        }

        [HttpPost]
        [Route("api/WSTemperature/")]
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
                    parameters.Add("filePath", file.LocalFileName);
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e).ToString();
            }
            string errorMsg = "";
            ParameterCheck(out errorMsg);
            if (errorMsg.Contains("Error"))
            {
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]))) { DeleteTempShapefiles(parameters["id"]); }
                return errorMsg;
            }
            string data = RetrieveData();
            if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
            {
                DeleteTempShapefiles(parameters["id"]);
            }
            return data;
        }

        /// <summary>
        /// Parses the parameters in the param string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="paramOne"></param>
        /// <returns></returns>
        private Dictionary<string, string> ParseParameters(out string errorMsg, string param)
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
        /// Checks file type, if zip will extract and check contents.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="zipPath"></param>
        private static void CheckFile(out string errorMsg, string filePath, string sessionGUID)
        {
            errorMsg = "";
            if (Path.GetExtension(filePath).Contains("zip"))
            {
                string extractPath = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID);
                try
                {
                    ZipFile.ExtractToDirectory(filePath, extractPath);
                    File.Delete(filePath);
                    CheckShapefiles(out errorMsg, sessionGUID);
                    if (errorMsg.Contains("Error")) { return; }
                }
                catch (Exception ex)
                {
                    errorMsg = "Error: " + ex;
                    return;
                }
            }
            else if (Path.GetExtension(filePath).Contains("json")) { return; }
            else { errorMsg = "Error: Invalid file provided. Accepted file types are: zipped shapefile (containing a shp, prj, and dbf file) or a json (containing geojson data)."; return; }
        }

        /// <summary>
        /// Deletes extracted shapefiles for cleanup.
        /// </summary>
        private static void DeleteTempShapefiles(string sessionGUID)
        {
            try
            {
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID + "\\")))
                {
                    Directory.Delete(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID + "\\"), true);
                }
            }
            catch
            {
                // write to log?
            }
        }

        /// <summary>
        /// Checks for required files for shapefile within the provided zip.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="sessionGUID"></param>
        private static void CheckShapefiles(out string errorMsg, string sessionGUID)
        {
            errorMsg = "";
            Dictionary<string, bool> requiredFiles = new Dictionary<string, bool>
            {
                {".shp", false},
                {".prj", false},
                {".dbf", false}
            };

            foreach (string file in Directory.GetFiles(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID)))
            {
                string ext = Path.GetExtension(file);
                if (requiredFiles.ContainsKey(ext))
                {
                    requiredFiles[ext] = true;
                }
            }
            if (requiredFiles.ContainsValue(false)) { errorMsg = "Error: Zipped Shapefile did not contain all required files. Zip must contain shp, prj, and dbf files."; }
        }

        /// <summary>
        /// Executes methods for retieving data using the values in the parameters variable.
        /// </summary>
        /// <returns></returns>
        private string RetrieveData()
        {
            string data = "";
            string errorMsg = "";
            WSTemperature result = new WSTemperature();

            if (parameters.ContainsKey("filePath"))
            {
                if (!String.IsNullOrWhiteSpace(parameters["filePath"]))
                {
                    CheckFile(out errorMsg, parameters["filePath"], parameters["id"]);
                    if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["id"]); return errorMsg; }
                    data = result.GetTemperatureData(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
                }
                else
                {
                    errorMsg = "Error: Invalid file provided."; return errorMsg;
                }
            }
            else if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))
                {
                    data = result.GetTemperatureData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                }
                else
                {
                    errorMsg = "Error: Invalid latitude or longitude values provided."; return errorMsg;
                }
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
