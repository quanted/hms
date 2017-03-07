using System;
using System.Collections.Generic;
using System.Web.Http;
using HMSWebServices.Models;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace HMSWebServices.Controllers
{

    public class WSPrecipitationController : ApiController
    {
        public Dictionary<string, string> parameters;

        /// <summary>
        /// Gets precipitaiton data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSPrecipitation/{param}")]
        public HMSJSON.HMSJSON.HMSData Get(string param)
        {
            //string data = "";
            string errorMsg = "";
            HMSUtils.Utils util = new HMSUtils.Utils();
            parameters = new Dictionary<string, string>();
            parameters = util.ParseParameterString(out errorMsg, param);
            if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
            if (util.ParameterValidation(out errorMsg, parameters))
            {
                if (errorMsg.Contains("Error")) { return new HMSJSON.HMSJSON.HMSData(); }
                WSPrecipitation result = new WSPrecipitation();
                HMSJSON.HMSJSON.HMSData data = result.GetPrecipitationDataObject(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                return data;
            }
            else
            {
                return new HMSJSON.HMSJSON.HMSData();
            }
            //return data;
        }

        /// <summary>
        /// POST method for getting precipitation data.
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/WSPrecipitation/")]
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
        /// POST method for getting precipitation data.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WSPrecipitation/")]
        public async Task<HMSJSON.HMSJSON.HMSData> Post()
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
            catch
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

            HMSJSON.HMSJSON.HMSData data = RetrieveData();

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
        //private string RetrieveData()
        //{
        //    string data = "";
        //    string errorMsg = "";
        //    WSPrecipitation result = new WSPrecipitation();

        //    if (parameters.ContainsKey("filePath"))
        //    {
        //        data = result.GetPrecipitationData(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
        //    }
        //    else if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
        //    {
        //        data = result.GetPrecipitationData(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    if (errorMsg.Contains("Error")) { return errorMsg; }
        //    return data;
        //}

        /// <summary>
        /// Executes methods for retieving data using the values in the parameters variable.
        /// </summary>
        /// <returns></returns>
        private HMSJSON.HMSJSON.HMSData RetrieveData()
        {
            string errorMsg = "";
            WSPrecipitation result = new WSPrecipitation();
            HMSJSON.HMSJSON.HMSData data;
            if (parameters.ContainsKey("filePath"))
            {
                data = result.GetPrecipitationDataObject(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
            }
            else if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                data = result.GetPrecipitationDataObject(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
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