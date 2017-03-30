using HMSWebServices.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HMSWebServices.Controllers
{
    public class WSTotalFlowController : ApiController
    {

        public Dictionary<string, string> parameters;

        /// <summary>
        /// Gets total flow data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSTotalFlow/{param}")]
        public HMSJSON.HMSJSON.HMSData Get(string param)
        {
            string errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();
            parameters = utils.ParseParameterString(out errorMsg, param);
            parameters["dataset"] = "totalflow";
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError(errorMsg);
            }
            if (utils.ParameterValidation(out errorMsg, parameters))
            {
                if (errorMsg.Contains("ERROR")) { return utils.ReturnError(errorMsg); }
                WSTotalFlow result = new WSTotalFlow();
                HMSJSON.HMSJSON.HMSData data = result.GetTotalFlowData(out errorMsg, parameters);
                return data;
            }
            else
            {
                return utils.ReturnError("ERROR: Errors found in parameters. " + errorMsg);
            }
        }

        /// <summary>
        /// POST method for getting total flow data.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSTotalFlow/")]
        public async Task<HMSJSON.HMSJSON.HMSData> Post()
        {
            string guid = Guid.NewGuid().ToString();

            HMSUtils.Utils utils = new HMSUtils.Utils();

            List<string> files = new List<string>();
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parameters.Add("id", guid);
            parameters["dataset"] = "totalflow";
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
                if (Directory.Exists(HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"])))
                {
                    utils.DeleteTempGUIDDirectory(HttpContext.Current.Server.MapPath("~\\TransientStorage\\"), parameters["id"]);
                }
                return utils.ReturnError(errorMsg);
            }

            WSTotalFlow result = new WSTotalFlow();
            HMSJSON.HMSJSON.HMSData data = result.GetTotalFlowData(out errorMsg, parameters);
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