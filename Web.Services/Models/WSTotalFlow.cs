using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace Web.Services.Models
{

    /// <summary>
    /// Input class for Total Flow (subsurface flow and surface flow)
    /// </summary>
    public class TotalFlowInput : TimeSeriesInput
    {
        // TODO: Possibly add geometryType parameter to TimeSeriesInput and adjust factory method accordingly
        /// <summary>
        /// Specifies the type of geometry provided
        /// Valid values: "huc", "commid", "catchmentid", "catchment", "flowline", "points"
        /// </summary>
        public string GeometryType { get; set; }

        /// <summary>
        /// Contains the geometry data, type specified by geometry Type. 
        /// Valid formats are: an ID for type huc, commid, and catchmentid; geojson for types catchment and flowline; and points for type points
        /// </summary>
        public string GeometryInput { get; set; }

        /// <summary>
        /// Contains the type as key and input as value, used when multiple inputs are needed for a request
        /// </summary>
        public Dictionary<string, string> GeometryInputs { get; set; }
    }

    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryResponse
    {
        public string job_id { get; set; }
        public string status { get; set; }
        public string data { get; set; }
        public GeometryData geometryData { get; set; }
    }


    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryData
    {
        /// <summary>
        /// Geometry component of HMS-GIS response
        /// </summary>
        public Dictionary<string, Catchment> geometry { get; set; }

        /// <summary>
        /// Metadata component of HMS-GIS response
        /// </summary>
        public Dictionary<string, object> metadata { get; set; }
    }

    /// <summary>
    /// Catchments
    /// </summary>
    public class Catchment
    {
        /// <summary>
        /// List of points in the Catchment
        /// </summary>
        public List<Point> points { get; set; }
    }

    /// <summary>
    /// Catchment point data
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Latitude of centroid
        /// </summary>

        public double latitude { get; set; }
        /// <summary>
        /// Longitude of centroid
        /// </summary>
        public double longitude { get; set; }
        /// <summary>
        /// Total cell area
        /// </summary>
        public double cellArea { get; set; }
        /// <summary>
        /// Cell area that intersects the catchment
        /// </summary>
        public double containedArea { get; set; }
        /// <summary>
        /// Percent coverage of the intersection
        /// </summary>
        public double percentArea { get; set; }
    }

    /// <summary>
    /// Point object
    /// </summary>
    public class GeometryPoint
    {
        /// <summary>
        /// Point latitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Point longitude
        /// </summary>
        public double Longitude { get; set; }

    }


    /// <summary>
    /// Model for Total Flow controller
    /// </summary>
    public class WSTotalFlow
    {

        // local testing
        private string baseUrl = "http://localhost:8000";
        // qedinternal url
        // private string baseUrl = "https://qedinternal.epa.gov";
        // deployment url
        // private string baseUrl = "http://172.20.100.15";



        /// <summary>
        /// Default function for retrieving Total Flow data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetTotalFlowData(TotalFlowInput input)
        {
            //TODO: Extract the geometry percentage from following code and place in Utility class, for possible use by all controllers.

            // Steps:
            // 1 - determine geometry
            // type case ID: is equal to "huc", "commid", or "catchmentid"
            // type case ID action: send request to HMS-GIS /rest/catchment?source=SOURCE%type=TYPE%id=ID for geometry
            // type case geojson: is equal to "catchment", or "flowline"
            // type case geojson action: send request to HMS-GIS /rest/catchment/geojson/ (geojson in body) OR
            //                         : send request to HMS-GIS /rest/catchment/flowlines/ (flowline geojson in body)
            // type case points: is equal to "points"
            // type case points action: if request is one point - send request to EPA waters for watershed geometry, if request is two points
            // 2 - retrieve catchments-points table 
            // 3 - collect all points catchment table (this resolves issue of possible duplicate calls on catchment edge)
            // 4 - retrieve data for all points in catchment table [Parallel]
            // 5 - iterate through catchments and assign data to appropriate catchment 
            // 6 - aggregate the data for each catchment based upon how much area that cell has in the catchment [Parallel]
            // 7 - return aggregated data as a timeseries in the form of { datetime : [ catchment1, catchment2, catchment3, ... ]}
            // 8 - set metadata (request inputs, catchment data, aggregation data, output structure)
            // 9 - return output
            string error = "";
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            GeometryResponse geo = new GeometryResponse();
            this.baseUrl = (Environment.GetEnvironmentVariable("FLASK_SERVER") != null) ? Environment.GetEnvironmentVariable("FLASK_SERVER") : this.baseUrl;
            string requestUrl = this.baseUrl + "/hms/rest/api/v2/hms/gis/percentage/";

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            if (input.GeometryInputs != null)
            {
                if (input.GeometryInputs.ContainsKey("huc8") && input.GeometryInputs.ContainsKey("commid"))
                {
                    Dictionary<string, string> taskID;
                    string queryUrl = requestUrl + "?huc_8_num=" + input.GeometryInputs["huc8"] + "&com_id_num=" + input.GeometryInputs["commid"] + "&grid_source=" + input.Source;
                    using (var httpClientHandler = new HttpClientHandler())
                    {
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                        using (var client = new HttpClient(httpClientHandler))
                        {
                            var result = client.GetStringAsync(queryUrl);
                            taskID = JsonSerializer.Deserialize<Dictionary<string, string>>(result.Result, options);
                        }
                        geo = await this.RequestData(taskID["job_id"]);
                    }

                }
                else
                {
                    return err.ReturnError("Input error - GeometryInputs provided is invalid.");
                }
            }
            else
            {
                switch (input.GeometryType.ToLower())
                {
                    case "huc":
                        // use case 1
                        // use case 2
                        string hucID = input.GeometryInput;
                        using (var client = new HttpClient())
                        {
                            string queryUrl = requestUrl + "?huc_8_num=" + hucID + "&grid_source=" + input.Source;
                            client.Timeout = TimeSpan.FromMinutes(10);
                            Task<string> request = client.GetStringAsync(queryUrl);
                            var result = request.Result;
                            GeometryResponse resp = JsonSerializer.Deserialize<GeometryResponse>(result, options);
                            Thread.Sleep(500);
                            geo = await this.RequestData(resp.job_id);
                        }
                        break;
                    case "comid":
                        // use case 3
                        string comID = input.GeometryInput;
                        using (var client = new HttpClient())
                        {
                            string queryUrl = requestUrl + "?com_id_list=" + comID + "&grid_source=" + input.Source;
                            client.Timeout = TimeSpan.FromMinutes(10);
                            Task<string> request = client.GetStringAsync(queryUrl);
                            var result = request.Result;
                            GeometryResponse resp = JsonSerializer.Deserialize<GeometryResponse>(result, options);
                            Thread.Sleep(500);
                            geo = await this.RequestData(resp.job_id);
                        }
                        break;
                    case "catchmentid":
                        // use case 3
                        // string catchmentID = input.GeometryInput;
                        //using (var client = new HttpClient())
                        //{
                        //    geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(baseUrl + "/api/GridCell/catchmentid/" + catchmentID).Result);
                        //}
                        goto default;
                        //break;
                    case "catchment":
                        // use case 4
                        // Use POST call with geometry 
                        goto default;
                        //break;
                    case "flowline":
                        // use case 5
                        // Use POST call with geometry
                        goto default;
                        //break;
                    case "points":
                        // use case 6
                        // use case 7
                        // GET call with points, hms-gis will get geometries
                        goto default;
                        //break;
                    case "test":
                        try
                        {
                            string testGeometry = "{\"geometry\":{\"9311911\": { \"points\": [ { \"cellArea\": 0.015624999999992895,  \"containedArea\": 4.178630503273804e-05,  \"longitude\": -71.43749999999996,  \"latitude\": 44.18749999999999,  \"percentArea\": 0.26743235220964506 },  { \"cellArea\": 0.015624999999996447,  \"containedArea\": 0.005083393397351494,  \"longitude\": -71.31249999999997,  \"latitude\": 44.18750000000001,  \"percentArea\": 32.53371774305696 },  { \"cellArea\": 0.015624999999996447,  \"containedArea\": 0.0002419268603100419,  \"longitude\": -71.31249999999997,  \"latitude\": 44.31249999999997,  \"percentArea\": 1.5483319059846201 } ] } },  \"metadata\": { \"execution time\": 86.99717831611633,  \"nldas source\": \"https://ldas.gsfc.nasa.gov/nldas/gis/NLDAS_Grid_Reference.zip\",  \"number of points\": 3,  \"request date\": \"Thu, 22 Mar 2018 11:46:44 GMT\",  \"shapefile source\": \"ftp://newftp.epa.gov/exposure/BasinsData/NHDPlus21/NHDPlus01060002.zip\" } }";
                            string testResponse = System.IO.File.ReadAllText(@"App_Data\test_nldas_grid.json");
                            geo = JsonSerializer.Deserialize<GeometryResponse>(testResponse, options);
                            geo.geometryData = JsonSerializer.Deserialize<GeometryData>(geo.data, options);
                        }
                        catch(JsonException ex)
                        {
                            string jsonError = ex.Message;
                        }
                        break;
                    default:
                        return err.ReturnError("Input error - GeometryType provided is invalid. Provided value = " + input.GeometryType);
                }
            }
            // Check for any errors
            if (!String.IsNullOrWhiteSpace(error) || !geo.status.Equals("SUCCESS"))
            {
                return err.ReturnError(error);
            }

            // Collect all unique points in Catchment
            List<GeometryPoint> points = new List<GeometryPoint>();
            List<string> pointsSTR = new List<string>();
            foreach (KeyValuePair<string, Catchment> k in geo.geometryData.geometry)
            {
                foreach (Point cp in k.Value.points)
                {
                    GeometryPoint p = new GeometryPoint();
                    p.Latitude = cp.latitude;
                    p.Longitude = cp.longitude;
                    if (!points.Contains(p))
                    {
                        points.Add(p);
                        pointsSTR.Add("[" + p.Latitude.ToString() + ", " + p.Longitude.ToString() + "]");
                    }
                }
            }

            ITimeSeriesInputFactory inputFactory = new TimeSeriesInputFactory();
            List<string> errorMessages = new List<string>();
            string errorMsg = "";
            // Initialize all surface and subsurface points
            Dictionary<string, SurfaceRunoff.SurfaceRunoff> surfaceFlow = new Dictionary<string, SurfaceRunoff.SurfaceRunoff>();
            Dictionary<string, SubSurfaceFlow.SubSurfaceFlow> subsurfaceFlow = new Dictionary<string, SubSurfaceFlow.SubSurfaceFlow>();
            foreach (GeometryPoint point in points)
            {
                string key = point.Latitude.ToString() + "," + point.Longitude.ToString();
                TimeSeriesGeometry tsGeometry = new TimeSeriesGeometry();
                tsGeometry.Point = new PointCoordinate()
                {
                    Latitude = point.Latitude,
                    Longitude = point.Longitude
                };
                if (surfaceFlow.Keys.Contains(key) || subsurfaceFlow.Keys.Contains(key))
                {
                    continue;
                }

                // Initialize surfaceFlow catchment point object
                errorMsg = "";
                TimeSeriesInput surfaceTempInput = new TimeSeriesInput();
                surfaceTempInput = input;
                surfaceTempInput.Geometry = tsGeometry;
                SurfaceRunoff.SurfaceRunoff sFlow = new SurfaceRunoff.SurfaceRunoff();
                sFlow.Input = inputFactory.SetTimeSeriesInput(surfaceTempInput, new List<string>() { "surfacerunoff" }, out errorMsg);
                surfaceFlow.Add(key, sFlow);
                errorMessages.Add(errorMsg);

                // Initialize subsurfaceFlow catchment point object
                errorMsg = "";
                TimeSeriesInput subSurfaceTempInput = new TimeSeriesInput();
                subSurfaceTempInput = input;
                subSurfaceTempInput.Geometry = tsGeometry;
                SubSurfaceFlow.SubSurfaceFlow subFlow = new SubSurfaceFlow.SubSurfaceFlow();
                subFlow.Input = inputFactory.SetTimeSeriesInput(subSurfaceTempInput, new List<string>() { "subsurfaceflow" }, out errorMsg);
                subsurfaceFlow.Add(key, subFlow);
                errorMessages.Add(errorMsg);
            }

            // TODO: merge parallelized calls to surfaceRunoff and subsurfaceFlow
            // Parallelized surfaceRunoff
            object outputListLock = new object();
            List<string> surfaceError = new List<string>();
            var pOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            Parallel.ForEach(surfaceFlow, pOptions, (KeyValuePair<string, SurfaceRunoff.SurfaceRunoff> surF) =>
            {
                string errorM = "";
                surF.Value.GetData(out errorM);
                lock (outputListLock)
                {
                    surfaceError.Add(errorM);
                }
            });

            // Parallelized subsurfaceFlow
            List<string> subsurfaceError = new List<string>();
            Parallel.ForEach(subsurfaceFlow, pOptions, (KeyValuePair<string, SubSurfaceFlow.SubSurfaceFlow> subF) =>
            {
                string errorM = "";
                subF.Value.GetData(out errorM);
                lock (outputListLock)
                {
                    subsurfaceError.Add(errorM);
                }
            });

            ITimeSeriesOutputFactory outputFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = outputFactory.Initialize();

            // Aggregate catchments
            List<ITimeSeriesOutput> catchmentFlow = new List<ITimeSeriesOutput>();
            ITimeSeriesOutput iOutput = outputFactory.Initialize();
            var options2 = new ParallelOptions { MaxDegreeOfParallelism = 1 };
            Dictionary<string, string> catchmentMeta = new Dictionary<string, string>();
            Parallel.ForEach(geo.geometryData.geometry, options2, (KeyValuePair<string, Catchment> catchments) => {
                string catchmentID = catchments.Key;
                List<ITimeSeriesOutput> catchmentPoints = new List<ITimeSeriesOutput>();
                foreach (Point cp in catchments.Value.points)
                {
                    IPointCoordinate point = new PointCoordinate()
                    {
                        Latitude = cp.latitude,
                        Longitude = cp.longitude
                    };
                    string key = point.Latitude.ToString() + "," + point.Longitude.ToString();
                    if (subsurfaceFlow[key].Output == null || surfaceFlow[key].Output == null)
                    {
                        Debug.WriteLine("Key:" + key + " was not found");
                        continue;
                    }

                    ITimeSeriesOutput output1 = Utilities.Merger.ModifyTimeSeries(surfaceFlow[key].Output, cp.percentArea / 100);
                    ITimeSeriesOutput output2 = Utilities.Merger.ModifyTimeSeries(subsurfaceFlow[key].Output, cp.percentArea / 100);

                    if (iOutput.Data.Count == 0)
                    {
                        iOutput = output1;
                        iOutput = Utilities.Merger.MergeTimeSeries(iOutput, output2);
                    }
                    else
                    {
                        iOutput = Utilities.Merger.MergeTimeSeries(iOutput, output1);
                        iOutput = Utilities.Merger.MergeTimeSeries(iOutput, output2);
                    }
                    catchmentPoints.Add(Utilities.Merger.AddTimeSeries(output1, output2, "TotalFlow"));
                }

                output.Data = (Utilities.Merger.AddTimeSeries(catchmentPoints).Data);
                int currentCatchment = catchmentMeta.Count + 1;
                catchmentMeta.Add("catchment_column_" + currentCatchment.ToString() + "_ID", catchments.Key);
            });
            // Testing
            // output.Data = iOutput.Data;

            output.DataSource = input.Source;
            output.Dataset = "Total Flow";

            output.Metadata = surfaceFlow.First().Value.Output.Metadata;
            output.Metadata.Remove(input.Source + "_lat");
            output.Metadata.Remove(input.Source + "_lon");
            output.Metadata.Remove(input.Source + "_prod_name");
            output.Metadata.Remove(input.Source + "_param_short_name");
            output.Metadata.Remove(input.Source + "_param_name");
            output.Metadata[input.Source + "_points"] = string.Join(", ", pointsSTR);

            // Adding geometry metadata to output metadata
            foreach (KeyValuePair<string, object> kv in geo.geometryData.metadata)
            {
                if (!output.Metadata.ContainsKey(kv.Key))
                {
                    output.Metadata.Add(kv.Key, kv.Value.ToString());
                }
            }

            // Cleaning up output metadata
            Dictionary<string, string> cleanedMetaData = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in output.Metadata)
            {
                if (!kv.Key.Contains("column"))
                {
                    cleanedMetaData.Add(kv.Key, kv.Value);
                }
            }
            output.Metadata = cleanedMetaData;

            // Add catchments metadata to output metadata
            foreach (KeyValuePair<string, string> kv in catchmentMeta)
            {
                output.Metadata.Add(kv.Key, kv.Value);
            }

            return output;
        }


        /// <summary>
        /// Using the received taskID, the function long-polls the server until the max requests is reached or the status returns success.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public async Task<GeometryResponse> RequestData(string taskID)
        {
            string error = "";
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            GeometryResponse result = new GeometryResponse();

            string dataUrl = this.baseUrl + "/hms/rest/api/v2/hms/data" + "?job_id=" + taskID;

            string status = "PENDING";
            int maxCount = 50;
            int count = 0;

            while (!(status == "SUCCESS") && !(status == "FAILURE") && count < maxCount)
            {
                if (count > 0)
                {
                    Task.Delay(5000).Wait();
                }
                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                    using (var client = new HttpClient(httpClientHandler))
                    {
                        var r = client.GetStringAsync(dataUrl);
                        await r;
                        try
                        {
                            result = JsonSerializer.Deserialize<GeometryResponse>(r.Result, options);
                        }
                        catch(JsonException ex)
                        {
                            string test = ex.Message;
                        }
                    }
                }
                status = result.status;
                count++;
            }
            if (count >= maxCount)
            {
                error = "Max number of requests reached in attempting to get geometry results. Max count:" + maxCount.ToString();
            }
            if (status.Equals("FAILURE"))
            {
                error = "Task to retrieve all gridpoints encountered an error.";
            }
            else
            {
                try
                {
                    result.geometryData = JsonSerializer.Deserialize<GeometryData>(result.data, options);
                }
                catch (JsonException ex)
                {
                    string test = ex.Message;
                }
            }

            return result;
        }
    }
}