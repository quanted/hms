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
        /// POST method using WSHMS api, requires additional 'dataset' parameter.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSHMS/")]
        public async Task<HMSJSON.HMSJSON.HMSData> Post()
        {
            string guid = Guid.NewGuid().ToString();

            HMSUtils.Utils utils = new HMSUtils.Utils();

            List<string> files = new List<string>();
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parameters.Add("id", guid);
            try
            {
                if (Request.Content.IsMimeMultipartContent())
                {
                    string fileSaveLocation = HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + guid);
                    CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);

                    await Request.Content.ReadAsMultipartAsync(provider);
                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            parameters.Add(key, val);
                        }
                    }
                    if (parameters.ContainsKey("filePath"))
                    {
                        Directory.CreateDirectory(fileSaveLocation);
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
                    return utils.ReturnError("ERROR: Invalid request content-type.");
                }
            }
            catch (Exception e)
            {
                return utils.ReturnError("ERROR: " + e.Message);
            }
            string errorMsg = "";
            utils.ParameterValidation(out errorMsg, parameters);

            if (parameters.ContainsKey("filePath"))
            {
                utils.UnzipShapefile(out errorMsg, parameters["filePath"], parameters["id"]);
            }
            if (errorMsg.Contains("ERROR"))
            {
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]))) {
                    utils.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
                }
                return utils.ReturnError(errorMsg);
            }

            WSHMS results = new WSHMS();
            HMSJSON.HMSJSON.HMSData data = results.GetHMSData(out errorMsg, parameters);
            if (errorMsg != "")
            {
                return utils.ReturnError(errorMsg);
            }

            if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
            {
                utils.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
            }
            return data;
        }

    }
}