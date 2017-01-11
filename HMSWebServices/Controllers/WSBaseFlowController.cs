using HMSWebServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HMSWebServices.Controllers
{
    public class WSBaseFlowController : ApiController
    {
        /// <summary>
        /// Gets base flow data using the parameters provided in the param string.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WSBaseFlow/{param}")]
        public string Get(string param)
        {
            string data = "";
            string errorMsg = "";
            string[] parameters = ParseParameters(out errorMsg, param);
            if (errorMsg.Contains("Error")) { return errorMsg; }
            WSBaseFlow result = new WSBaseFlow();

            if (param.Contains("Location"))             //Location provided by latitude, longitude
            {
                data = result.GetBaseFlowData(out errorMsg, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], Convert.ToBoolean(parameters[5]));
            }
            else if (param.Contains("ShapeFile"))       //Location provided by shapefile.
            {
                string shapefilePath = HttpContext.Current.Server.MapPath("~/TransientStorage/") + parameters[0] + ".zip";
                UnzipFile(out errorMsg, shapefilePath);
                if (errorMsg.Contains("Error")) { DeleteTempShapefiles(); return errorMsg; }
                data = result.GetBaseFlowData(out errorMsg, parameters[1], parameters[2], parameters[3], Convert.ToBoolean(parameters[4]), shapefilePath);
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(); return errorMsg; }
            DeleteTempShapefiles();
            return data;
        }

        [HttpPost]
        [Route("api/WSBaseFlow/{param}")]
        public string Post(string param)
        {
            string data = "";
            string errorMsg = "";
            string[] parameters = ParseParameters(out errorMsg, param);
            if (errorMsg.Contains("Error")) { return errorMsg; }
            WSBaseFlow result = new WSBaseFlow();

            if (param.Contains("Location"))             //Location provided by latitude, longitude
            {
                data = result.GetBaseFlowData(out errorMsg, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], Convert.ToBoolean(parameters[5]));
            }
            else if (param.Contains("ShapeFile"))       //Location provided by shapefile.
            {
                string shapefilePath = HttpContext.Current.Server.MapPath("~/TransientStorage/") + parameters[0] + ".zip";
                UnzipFile(out errorMsg, shapefilePath);
                if (errorMsg.Contains("Error")) { DeleteTempShapefiles(); return errorMsg; }
                data = result.GetBaseFlowData(out errorMsg, parameters[1], parameters[2], parameters[3], Convert.ToBoolean(parameters[4]), shapefilePath);
            }
            if (errorMsg.Contains("Error")) { DeleteTempShapefiles(); return errorMsg; }
            DeleteTempShapefiles();
            return data;
        }

        /// <summary>
        /// Parses the parameters in the param string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="paramOne"></param>
        /// <returns></returns>
        private string[] ParseParameters(out string errorMsg, string param)
        {
            errorMsg = "";
            string[] result = new string[6];
            string[] parsedValue = param.Split('&');
            int j = 0;
            string[] value = new string[2];
            for (int i = 0; i <= parsedValue.Length; i++)
            {
                if (j < parsedValue.Length) { value = parsedValue[j].Split('='); }
                else { return result; }
                try
                {

                    if (value[0].Contains("Location"))
                    {
                        string[] coord = value[1].Split(',');
                        result[i] = coord[0];           //Latitude
                        result[i + 1] = coord[1];       //Longitude
                        i++;
                    }
                    else
                    {
                        result[i] = value[1];
                    }
                    j++;
                }
                catch (Exception ex)
                {
                    errorMsg = "Error: " + ex;
                    return null;
                }
            }
            return result;
        }

        /// <summary>
        /// Extracts the contents of the zip file containing the shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="zipPath"></param>
        private void UnzipFile(out string errorMsg, string zipPath)
        {
            errorMsg = "";
            string extractPath = HttpContext.Current.Server.MapPath("~/TransientStorage/");
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
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
        private void DeleteTempShapefiles()
        {
            string[] filePaths = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/TransientStorage/"));
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }
    }
}
