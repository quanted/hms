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
using System.Collections.Specialized;

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
            if (!parameters.ContainsKey("stationID"))
            {
                string location = "";
                if ((parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude")))
                {
                    if ((String.IsNullOrWhiteSpace(parameters["latitude"])) || (String.IsNullOrWhiteSpace(parameters["longitude"])))
                    {
                        location = "file";
                    }
                }
                else if (parameters.ContainsKey("filePath"))
                {
                    if ((String.IsNullOrWhiteSpace(parameters["filePath"])) && location.Contains("file") && !parameters.ContainsKey("geoJson"))
                    {
                        errorMsg += "Error: Invalid location details provided.";
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
                    errorMsg += "Error: All location details were not provided. Both a latitude and longitude value must be given OR a shapefile provided.\n";
                }
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
                if (String.IsNullOrWhiteSpace(parameters["source"]) || !parameters.ContainsKey("stationID"))
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
            if (!parameters.ContainsKey("dataset"))
            {
                errorMsg += "Error: A dataset must be provided.";
            }

        }
        
        /// <summary>
        /// POST method using WSHMS api, requires additional 'dataset' parameter.
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/WSHMS/")]
        //public async Task<string> Post()
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string guid = Guid.NewGuid().ToString();

        //    string fileSaveLocation = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + guid);
        //    Directory.CreateDirectory(fileSaveLocation);
        //    CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
        //    List<string> files = new List<string>();
        //    parameters = new Dictionary<string, string>();
        //    parameters.Add("id", guid);
        //    try
        //    {
        //        await Request.Content.ReadAsMultipartAsync(provider);
        //        foreach (var key in provider.FormData.AllKeys)
        //        {
        //            foreach (var val in provider.FormData.GetValues(key))
        //            {
        //                parameters.Add(key, val);
        //            }
        //        }

        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            files.Add(Path.GetFileName(file.LocalFileName));
        //            parameters.Add("filePath", file.LocalFileName);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e).ToString();
        //    }
        //    string errorMsg = "";
        //    HMSUtils.Utils util = new HMSUtils.Utils();
        //    util.ParameterValidation(out errorMsg, parameters);

        //    if (parameters.ContainsKey("filePath"))
        //    {
        //        util.UnzipShapefile(out errorMsg, parameters["filePath"], parameters["id"]);
        //    }
        //    if (errorMsg.Contains("Error"))
        //    {
        //        if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]))) { util.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]); }
        //        return errorMsg;
        //    }

        //    string data = RetrieveData();

        //    if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
        //    {
        //        util.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
        //    }
        //    return data;
        //}

        /// <summary>
        /// POST method using WSHMS api, requires additional 'dataset' parameter.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSHMS/")]
        public async Task<HMSJSON.HMSJSON.HMSData> Post()
        {

            string guid = Guid.NewGuid().ToString();

            string fileSaveLocation = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + guid);
            Directory.CreateDirectory(fileSaveLocation);
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            parameters = new Dictionary<string, string>();
            parameters.Add("id", guid);
            try
            {

                if (Request.Content.IsMimeMultipartContent())
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
                else if(Request.Content.IsFormData())
                {
                    NameValueCollection collection = await Request.Content.ReadAsFormDataAsync();
                    foreach(string key in collection)
                    {
                        parameters.Add(key, collection[key]);
                    }
                }
                else
                {
                    return new HMSJSON.HMSJSON.HMSData();
                }
            }
            catch (Exception e)
            {
                return new HMSJSON.HMSJSON.HMSData();
            }
            string errorMsg = "";
            HMSUtils.Utils util = new HMSUtils.Utils();
            util.ParameterValidation(out errorMsg, parameters);

            if (parameters.ContainsKey("filePath"))
            {
                util.UnzipShapefile(out errorMsg, parameters["filePath"], parameters["id"]);
            }
            if (errorMsg.Contains("Error"))
            {
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]))) { util.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]); }

                return new HMSJSON.HMSJSON.HMSData();
            }

            HMSJSON.HMSJSON.HMSData data = RetrieveDataObject();

            if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
            {
                util.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
            }
            return data;
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
                data = result.GetHMSData(out errorMsg, parameters["dataset"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"], parameters["layers"]);
            }
            else if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                data = result.GetHMSData(out errorMsg, parameters["dataset"], parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["layers"]);
            }
            else
            {
                return null;
            }
            if (errorMsg.Contains("Error")) { return errorMsg; }
            return data;
        }

        /// <summary>
        /// Executes methods for retieving data using the values in the parameters variable.
        /// </summary>
        /// <returns></returns>
        private HMSJSON.HMSJSON.HMSData RetrieveDataObject()
        {
            HMSJSON.HMSJSON.HMSData data;
            string errorMsg = "";
            WSHMS result = new WSHMS();

            if (parameters.ContainsKey("filePath"))
            {
                data = result.GetHMSDataObject(out errorMsg, parameters["dataset"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"], parameters["layers"]);
            }
            else if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                data = result.GetHMSDataObject(out errorMsg, parameters["dataset"], parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["layers"]);
            }
            else
            {
                return new HMSJSON.HMSJSON.HMSData();
            }
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            return data;
        }
    }
}