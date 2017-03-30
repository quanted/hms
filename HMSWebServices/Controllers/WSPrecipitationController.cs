using System;
using System.Collections.Generic;
using System.Web.Http;
using HMSWebServices.Models;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Specialized;

namespace HMSWebServices.Controllers
{
    public class WSPrecipitationController : ApiController
    {
        public Dictionary<string, string> parameters;

        /// <summary>
        /// GET method for retrieving precipitaiton data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSPrecipitation/{param}")]
        public HMSJSON.HMSJSON.HMSData Get(string param)
        {
            string errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();
            parameters = utils.ParseParameterString(out errorMsg, param);
            parameters["dataset"] = "precipitation";
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError(errorMsg);
            }
            if (utils.ParameterValidation(out errorMsg, parameters))
            {
                if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                WSPrecipitation result = new WSPrecipitation();
                HMSJSON.HMSJSON.HMSData data = result.GetPrecipitationData(out errorMsg, parameters);
                return data;
            }
            else
            {
                return utils.ReturnError("ERROR: Errors found in parameters. " + errorMsg);
            }
        }

        /// <summary>
        /// POST method for retrieving precipitation data.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSPrecipitation/")]
        public async Task<HMSJSON.HMSJSON.HMSData> Post()
        {
            string guid = Guid.NewGuid().ToString();

            HMSUtils.Utils utils = new HMSUtils.Utils();    

            List<string> files = new List<string>();
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parameters.Add("id", guid);
            parameters["dataset"] = "precipitation";
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
                else if (Request.Content.IsFormData())
                {
                    NameValueCollection collection = await Request.Content.ReadAsFormDataAsync();
                    foreach (string key in collection)
                    {
                        parameters.Add(key, collection[key]);
                    }
                }
                else
                {
                    return utils.ReturnError("ERROR: Invalid request content-type.");
                }
            }
            catch(Exception e)
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
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
                {
                    utils.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
                }
                return utils.ReturnError(errorMsg);
            }

            WSPrecipitation result = new WSPrecipitation();
            HMSJSON.HMSJSON.HMSData data = result.GetPrecipitationData(out errorMsg, parameters);
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