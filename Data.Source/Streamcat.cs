using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Data.Source
{
    public class Catchment
    {
        public int comid;
        public Dictionary<int, double> landcover;
        public Dictionary<string, double> soil;
        public string hsg;
    }

    public class Streamcat
    {

        /// <summary>
        /// Get catchment data for the specified comid.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        public Catchment GetCatchmentData(out string errorMsg, int comid)
        {
            errorMsg = "";
            string rawData = GetStreamcatData(out errorMsg, comid);
            dynamic streamcatData = JsonConvert.DeserializeObject<dynamic>(rawData);
            Catchment catchment = SetCatchmentData(out errorMsg, streamcatData.output);
            return catchment;
        }

        /// <summary>
        /// Create get query string to retrieve epa waters streamcat data. HTTP site: https://watersgeo.epa.gov/watershedreport/?comid=
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        private string GetStreamcatData(out string errorMsg, int comid)
        {
            errorMsg = "";
            string aoi = "Catchment%2FWatershed";
            string metrics = "Agriculture;Hydrology;Land%20Cover;Soils;Urban";
            string requestUrl = String.Format(
                "https://ofmpub.epa.gov/waters10/streamcat.jsonv25?pcomid={0}" +
                "&pAreaOfInterest={1}&pLandscapeMetricType={2}" +
                "&pLandscapeMetricClass=Disturbance;Natural&pFilenameOverride=AUTO", comid, aoi, metrics);

            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(requestUrl);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(200);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download requested nldas data. " + ex.Message;
                return null;
            }
            return data;
        }

        /// <summary>
        /// Reference landcover from nlcd 2011 data: https://www.mrlc.gov/nlcd11_leg.php
        /// Reference hydrologic soil group: https://daac.ornl.gov/SOILS/guides/Global_Hydrologic_Soil_Group.html
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static dynamic SetCatchmentData(out string errorMsg, dynamic output)
        {
            errorMsg = "";
            Catchment catchment = new Catchment();
            catchment.comid = output.comid;
            Dictionary<int, double> landcover = new Dictionary<int, double>()
            {
                { 11, -1.0 },
                { 12, -1.0 },
                { 21, -1.0 },
                { 22, -1.0 },
                { 23, -1.0 },
                { 24, -1.0 },
                { 31, -1.0 },
                { 41, -1.0 },
                { 42, -1.0 },
                { 43, -1.0 },
                { 51, -1.0 },
                { 52, -1.0 },
                { 71, -1.0 },
                { 72, -1.0 },
                { 73, -1.0 },
                { 74, -1.0 },
                { 81, -1.0 },
                { 82, -1.0 },
                { 90, -1.0 },
                { 95, -1.0 }
            };
            catchment.landcover = landcover;
            catchment.soil = new Dictionary<string, double>() { {"sand", -1.0 }, {"clay", -1.0 } };

            foreach(dynamic cMetric in output.metrics)
            {
                string id = cMetric.id;
                double value = cMetric.metric_value;
                switch (id)
                {
                    case "pctow2011cat":                // Open Water Land Cover Percentage; class 11
                        catchment.landcover[11] = value;
                        break;
                    case "pctice2011cat":               // Ice/Snow Cover Percentage; class 12
                        catchment.landcover[12] = value;
                        break;
                    case "pcturbop2011cat":             // Developed, Open Space Land Use Percentage; class: 21
                        catchment.landcover[21] = value;
                        break;
                    case "pcturblo2011cat":             // Developed, Low Intensity Land Use Percentage; class: 22
                        catchment.landcover[22] = value;
                        break;
                    case "pcturbmd2011cat":             // Developed, Medium Intensity Land Use Percentage; class: 23
                        catchment.landcover[23] = value;
                        break;
                    case "pcturbhi2011cat":             // Developed, High Intensity Land Use Percentage; class: 24
                        catchment.landcover[24] = value;
                        break;
                    case "pctbl2011cat":                // Bedrock and Similar Earthen Material Percentage; class: 31
                        catchment.landcover[31] = value;
                        break;
                    case "pctdecid2011cat":             // Deciduous Forest Land Cover Percentage; class: 41
                        catchment.landcover[41] = value;
                        break;
                    case "pctconif2011cat":             // Evergreen Forest Land Cover Percentage; class: 42
                        catchment.landcover[42] = value;
                        break;
                    case "pctmxfst2011cat":             // Mixed Deciduous/Evergreen Forest Land Cover Percentage; class 43
                        catchment.landcover[43] = value;
                        break;
                    // Class 51 Dwarf Scrub (Alaska only)
                    case "pctshrb2011cat":              // Shrub/Scrub Land Cover Percentage; class 52
                        catchment.landcover[52] = value;
                        break;
                    case "pctgrs2011cat":               // Grassland/Herbaceous Land Cover Percentage; class: 71
                        catchment.landcover[71] = value;
                        break;
                    // Class 72 Sedge/Herbaceous (Alaska only)
                    // Class 73 Lichens (Alaska only)
                    // Class 74 Moss (Alaska only)
                    case "pcthay2011cat":               // Pasture Hay Land Use Percentage; class: 81
                        catchment.landcover[81] = value;
                        break;
                    case "pctcrop2011cat":              // Row Crop Land Use Percentage; class 82
                        catchment.landcover[82] = value;
                        break;
                    case "pctwdwet2011cat":             // Woody Wetland Land Cover Percentage; class 90
                        catchment.landcover[90] = value;
                        break;
                    case "pcthbwet2011cat":             // Herbaceous Wetland Land Cover Percentage; class: 95
                        catchment.landcover[95] = value;
                        break;  
                    case "claycat":                     // Statsgo Catchment Clay Mean
                        catchment.soil["clay"] = value;
                        break;
                    case "sandcat":                     // Statsgo Catchment Sand Mean
                        catchment.soil["sand"] = value;
                        break;
                    default:
                        break;
                }
            }

            string hsg = "A";
            if (catchment.soil["sand"] > 90 && catchment.soil["clay"] < 10) {
                hsg = "A";
            }
            else if( catchment.soil["sand"] > 50 && catchment.soil["sand"] < 90 && catchment.soil["clay"] > 10 && catchment.soil["clay"] < 20)
            {
                hsg = "B";
            }
            else if(catchment.soil["sand"] < 50 && catchment.soil["clay"] > 20 && catchment.soil["clay"] < 40)
            {
                hsg = "C";
            }
            else if (catchment.soil["sand"] < 50 && catchment.soil["clay"] > 40 ){
                hsg = "D";
            }
            catchment.hsg = hsg;

            return catchment;
        }

    }
}
