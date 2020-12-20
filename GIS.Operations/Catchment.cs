using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Net.Mime;

namespace GIS.Operations
{
    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public int id { get; set; }
        public Geometry geometry { get; set; }
        public Dictionary<string, object> properties { get; set; }
    }

    public class EPAWaters
    {
        public string type { get; set; }
        public Dictionary<string, object> crs { get; set; }
        
        public List<Feature> features { get; set; }
    }

    public class Streamcat
    {
        public Dictionary<string, object> output { get; set; }
        public Dictionary<string, object> status { get; set; }
    }

    public class Catchment
    {

        private string epaWatersURL = "https://watersgeo.epa.gov/arcgis/rest/services/NHDPlus_NP21/Catchments_NP21_Simplified/MapServer/0/query?";
        private string comid;
        private string url;
        public EPAWaters data;


        public Catchment(string comid)
        {
            this.comid = comid;
            this.url = this.BuildURL();
            try
            {
                this.data = JsonSerializer.Deserialize<EPAWaters>(this.DownloadData(this.url, 5).Result);
            }
            catch(JsonException js)
            {
                Log.Warning("Error: Failed to load data from EPA waters. Message: ", js.Message);
                this.data = null;
            }
        }

        public Dictionary<string, string> GetMetadata()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            if (this.data is null)
            {
                metadata.Add("ERROR", "Unable to retrieve EPA Waters catchment data for COMID: " + this.comid);
                return metadata;
            }

            foreach (KeyValuePair<string, object> keyValue in this.data.features[0].properties)
            {
                metadata.Add(keyValue.Key, keyValue.Value.ToString());
            }
            return metadata;
        }

        public Dictionary<string, double> GetBounds()
        {
            double maxLat = -90.0;
            double minLat = 90.0;
            double maxLng = -180.0;
            double minLng = 180.0;

            foreach (List<double> point in this.data.features[0].geometry.coordinates[0])
            {
                if(point[0] > maxLng)
                {
                    maxLng = point[0];
                }
                if(point[0] < minLng)
                {
                    minLng = point[0];
                }
                if(point[1] > maxLat)
                {
                    maxLat = point[1];
                }
                if(point[1] < minLat)
                {
                    minLat = point[1];
                }
            }

            return new Dictionary<string, double>()
            {
                { "max_latitude", maxLat },
                { "min_latitude", minLat },
                { "max_longitude", maxLng },
                { "min_longitude", minLng }
            };
        }


        private string BuildURL()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.epaWatersURL);
            sb.Append("where=FEATUREID=" + this.comid + "&");
            sb.Append("text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&");
            sb.Append("returnGeometry=true&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&");
            sb.Append("outSR=%7B%22wkt%22+%3A+%22GEOGCS%5B%5C%22GCS_WGS_1984%5C%22%2CDATUM%5B%5C%22D_WGS_1984%5C%22%2C+SPHEROID%5B%5C%22WGS_1984%5C%22%2C6378137%2C298.257223563%5D%5D%2CPRIMEM%5B%5C%22Greenwich%5C%22%2C0%5D%2C+UNIT%5B%5C%22Degree%5C%22%2C0.017453292519943295%5D%5D%22%7D&");
            sb.Append("returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&");
            sb.Append("outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&");
            sb.Append("resultOffset=&resultRecordCount=&queryByDistance=&returnExtentsOnly=false&datumTransformation=&");
            sb.Append("parameterValues=&rangeValues=&f=geojson");
            return sb.ToString();
        }

        private async Task<string> DownloadData(string url, int retries, string requestData=null)
        {
            string data = "";
            HttpClient hc = new HttpClient();
            HttpRequestMessage rm = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            if(requestData != null)
            {
                rm.Content = new StringContent(requestData, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage wm = new HttpResponseMessage();
            int maxRetries = 10;

            try
            {
                string status = "";

                while (retries < maxRetries && !status.Contains("OK"))
                {
                    wm = await hc.SendAsync(rm);
                    var response = wm.Content;
                    status = wm.StatusCode.ToString();
                    data = await wm.Content.ReadAsStringAsync();
                    retries += 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(1000 * retries);
                    }
                }
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries += 1;
                    Log.Warning("Error: Failed to download epa waters catchment geometry data. Retry {0}:{1}, Url: {2}", retries, maxRetries, url);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download epa waters catchment geometry data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }


        public Dictionary<string, object> GetStreamcatData()
        {
            string scURL = "https://ofmpub.epa.gov/waters10/streamcat.jsonv25?pcomid=" + this.comid + "&pAreaOfInterest=Catchment%2FWatershed;Riparian%20Buffer%20(100m)&pLandscapeMetricType=Agriculture;Climate;Disturbance;Hydrology;Infrastructure;Land%20Cover;Lithology;Mines;Pollution;Riparian;Soils;Topography;Urban;Wetness&pLandscapeMetricClass=Disturbance;Natural&pFilenameOverride=AUTO";
            string data = this.DownloadData(scURL, 5).Result;
            Streamcat sc = JsonSerializer.Deserialize<Streamcat>(data);
            return sc.output;
        }

        public Dictionary<string, Dictionary<string, string>> GetNWISGauges(List<double> coord = null)
        {
            Data.Source.StreamGauge sg = new Data.Source.StreamGauge();
            Dictionary<string, Dictionary<string, string>> stations = sg.FindStation(this.GetBounds(), true, 0.0, 0.1, 1.0);
            if (coord != null)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> kv in stations)
                {
                    double distance = Operations.CalculateRadialDistance(coord[0], coord[1], Double.Parse(kv.Value["dec_lat_va"]), Double.Parse(kv.Value["dec_long_va"]), true);
                    stations[kv.Key].Add("distance", distance.ToString());
                    stations[kv.Key].Add("distance_units", "(km) from catchment centroid to gauge latitude/longitude");
                }
            }
            return stations;
        }

        public object GetStreamGeometry(double latitude, double longitude)
        {
            string url = "https://ofmpub.epa.gov/waters10/PointIndexing.Service";
            string dataRequest = @"{'pGeometry': 'POINT(" + longitude + " " + latitude + ")', 'pGeometryMod': 'WKT,SRSNAME=urn:ogc:def:crs:OGC::CRS84', 'pPointIndexingMethod': 'DISTANCE', 'pPointIndexingMaxDist': 25, 'pOutputPathFlag': 'TRUE', 'pReturnFlowlineGeomFlag': 'TRUE', 'optOutCS': 'SRSNAME=urn:ogc:def:crs:OGC::CRS84', 'optOutPrettyPrint': 0}";
            string data = this.DownloadData(url, 5, dataRequest).Result;
            object streamData = JsonSerializer.Deserialize<object>(data);
            return streamData;
        }

    }
}
