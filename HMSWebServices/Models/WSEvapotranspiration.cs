using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Models
{
    public class WSEvapotranspiration
    {

        public HMSJSON.HMSJSON.HMSData GetEvapotranspirationData(out string errorMsg, Dictionary<string,string> parameters)
        {
            errorMsg = "";
            HMSUtils.Utils utils = new HMSUtils.Utils();

            // BaseFlow Initiailization
            HMSEvapotranspiration.Evapotranspiration evapo;
            if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
            {
                evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["startdate"], parameters["enddate"], parameters["source"], Convert.ToBoolean(parameters["localtime"]), null);
            }
            else if (parameters.ContainsKey("filePath"))
            {
                evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]), parameters["filePath"]);
            }
            else if (parameters.ContainsKey("geojson"))
            {
                evapo = new HMSEvapotranspiration.Evapotranspiration(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"], Convert.ToBoolean(parameters["localTime"]));
                evapo.gdal.geoJSON = parameters["geojson"];
            }
            else
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters.");
            }

            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: No valid geospatial information found in parameters. " + errorMsg);
            }
            evapo.GetDataSetsObject(out errorMsg);
            if (errorMsg.Contains("ERROR"))
            {
                return utils.ReturnError("ERROR: Unable to get requested data. " + errorMsg);
            }
            return evapo.jsonData;
        }
    }
}