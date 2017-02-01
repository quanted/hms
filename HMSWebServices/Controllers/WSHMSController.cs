using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HMSWebServices.Models;

namespace HMSWebServices.Controllers
{

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }

    public class WSHMSController : ApiController
    {
        public Dictionary<string, string> parameters;

        /// <summary>
        /// Evaluates the parameters provided by the POST body, ensuring all required parameters are present.
        /// </summary>
        /// <param name="errorMsg"></param>
        private void ParameterCheck(out string errorMsg)
        {
            errorMsg = "";
            string location = "";
            if ((parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude")))
            {
                if ((String.IsNullOrWhiteSpace(parameters["latitude"])) || (String.IsNullOrWhiteSpace(parameters["longitude"])))
                {
                    location = "file";
                }
            }
            if (parameters.ContainsKey("filePath") )
            {
                if ((String.IsNullOrWhiteSpace(parameters["filePath"])) && location.Contains("file") && !parameters.ContainsKey("geoJson"))
                {
                    errorMsg += "Error: Invalid location details provided.";
                }
            }
            else if(parameters.ContainsKey("geoJson"))
            {
                if (parameters.ContainsKey("geoJson"))
                {
                    parameters.Add("filePath", HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]) + "\\geo.json");
                    System.IO.File.WriteAllText(parameters["filePath"], parameters["geoJson"]);
                }
            }
            else
            {
                errorMsg += "Error: All location details were not provided. Both a latitude and longitude value must be given OR a shapefile provided.\n";
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
        
        [HttpPost]
        [Route("api/WSHMS/")]
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
                Directory.Delete(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + sessionGUID + "\\"), true);
            }
            catch
            { }
        }

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
                if(requiredFiles.ContainsKey(ext))
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
            WSHMS result = new WSHMS();

            if (parameters.ContainsKey("filePath"))
            {
                if (!String.IsNullOrWhiteSpace(parameters["filePath"]))
                {
                    //string shapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["id"] + "\\" + parameters["shapefile"] + ".zip";
                    CheckFile(out errorMsg, parameters["filePath"], parameters["id"]);
                    if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["id"]); return errorMsg; }
                    //string unzippedShapefile = HttpContext.Current.Server.MapPath("~\\TransientStorage\\") + parameters["id"] + "\\" + parameters["shapefile"] + ".shp";
                    data = result.GetHMSData(out errorMsg, parameters["dataset"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"], parameters["layers"]);
                }
                else if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))
                {
                    data = result.GetHMSData(out errorMsg, parameters["dataset"], parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["layers"]);
                }
                else
                {
                    errorMsg = "Error: Valid location parameters must be provided."; return errorMsg;
                }
            }
            else if (!String.IsNullOrWhiteSpace(parameters["latitude"]) && !String.IsNullOrWhiteSpace(parameters["longitude"]))
            {
                data = result.GetHMSData(out errorMsg, parameters["dataset"], parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["layers"]);
            }
            else
            {
                errorMsg = "Error: Valid location parameters must be provided."; return errorMsg;
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(parameters["id"]); return errorMsg; }
            return data;
        }
    }
}
