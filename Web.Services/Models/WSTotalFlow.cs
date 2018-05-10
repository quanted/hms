using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        public string GeometryType;

        /// <summary>
        /// Contains the geometry data, type specified by geometry Type. 
        /// Valid formats are: an ID for type huc, commid, and catchmentid; geojson for types catchment and flowline; and points for type points
        /// </summary>
        public string GeometryInput;

        /// <summary>
        /// Contains the type as key and input as value, used when multiple inputs are needed for a request
        /// </summary>
        public Dictionary<string, string> GeometryInputs;
    }

    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryResponse
    {
        public string id;
        public string status;
        public GeometryData data;
    }


    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryData
    {
        /// <summary>
        /// Geometry component of HMS-GIS response
        /// </summary>
        public Dictionary<string, Catchment> geometry;

        /// <summary>
        /// Metadata component of HMS-GIS response
        /// </summary>
        public Dictionary<string, string> metadata;
    }

    /// <summary>
    /// Catchments
    /// </summary>
    public class Catchment
    {
        /// <summary>
        /// List of points in the Catchment
        /// </summary>
        public List<Point> points;
    }

    /// <summary>
    /// Catchment point data
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Latitude of centroid
        /// </summary>
        public double latitude;
        /// <summary>
        /// Longitude of centroid
        /// </summary>
        public double longitude;
        /// <summary>
        /// Total cell area
        /// </summary>
        public double cellArea;
        /// <summary>
        /// Cell area that intersects the catchment
        /// </summary>
        public double containedArea;
        /// <summary>
        /// Percent coverage of the intersection
        /// </summary>
        public double percentArea;
    }

    /// <summary>
    /// Point object
    /// </summary>
    public class GeometryPoint
    {
        /// <summary>
        /// Point latitude
        /// </summary>
        public double Latitude;
        /// <summary>
        /// Point longitude
        /// </summary>
        public double Longitude;
    
    }


    /// <summary>
    /// Model for Total Flow controller
    /// </summary>
    public class WSTotalFlow
    {

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
            // local testing
            //string baseUrl = "http://localhost:8000/hms/rest/api/v2/hms/gis/percentage/";
            // qedinternal url
            //string baseUrl = "https://qedinternal.epa.gov/hms/rest/api/v2/hms/gis/percentage/";
            // deployment url
            string baseUrl = "http://172.20.100.11/hms/rest/api/v2/hms/gis/percentage/";
            if (input.GeometryInputs != null)
            {
                if(input.GeometryInputs.ContainsKey("huc8") && input.GeometryInputs.ContainsKey("commid"))
                {
                    Dictionary<string, string> taskID;
                    string queryUrl = baseUrl + "?huc_8_num=" + input.GeometryInputs["huc8"] + "&com_id_num=" + input.GeometryInputs["commid"];
                    using (var client = new HttpClient())
                    {
                        taskID = JsonConvert.DeserializeObject<Dictionary<string, string>>(client.GetStringAsync(queryUrl).Result);
                    }
                    geo = this.RequestData(taskID["job_id"], out error);

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
                            string queryUrl = baseUrl + "?huc_8_id=" + hucID;
                            client.Timeout = TimeSpan.FromMinutes(10);
                            geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(queryUrl).Result);
                        }
                        //goto default;
                        break;
                    case "commid":
                        // use case 3
                        //string commid = input.GeometryInput;
                        //using (var client = new HttpClient())
                        //{
                        //    string queryUrl = baseUrl + "?com_id=" + commid;
                        //    client.Timeout = TimeSpan.FromMinutes(10);
                        //    geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(queryUrl).Result);
                        //}
                        goto default;
                        break;
                    case "catchmentid":
                        // use case 3
                        // string catchmentID = input.GeometryInput;
                        //using (var client = new HttpClient())
                        //{
                        //    geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(baseUrl + "/api/GridCell/catchmentid/" + catchmentID).Result);
                        //}
                        goto default;
                        break;
                    case "catchment":
                        // use case 4
                        // Use POST call with geometry 
                        goto default;
                        break;
                    case "flowline":
                        // use case 5
                        // Use POST call with geometry
                        goto default;
                        break;
                    case "points":
                        // use case 6
                        // use case 7
                        // GET call with points, hms-gis will get geometries
                        goto default;
                        break;
                    case "test":
                        string testGeometry = "{\"geometry\":{\"9311911\": { \"points\": [ { \"cellArea\": 0.015624999999992895,  \"containedArea\": 4.178630503273804e-05,  \"longitude\": -71.43749999999996,  \"latitude\": 44.18749999999999,  \"percentArea\": 0.26743235220964506 },  { \"cellArea\": 0.015624999999996447,  \"containedArea\": 0.005083393397351494,  \"longitude\": -71.31249999999997,  \"latitude\": 44.18750000000001,  \"percentArea\": 32.53371774305696 },  { \"cellArea\": 0.015624999999996447,  \"containedArea\": 0.0002419268603100419,  \"longitude\": -71.31249999999997,  \"latitude\": 44.31249999999997,  \"percentArea\": 1.5483319059846201 } ] } },  \"metadata\": { \"execution time\": 86.99717831611633,  \"nldas source\": \"https://ldas.gsfc.nasa.gov/nldas/gis/NLDAS_Grid_Reference.zip\",  \"number of points\": 3,  \"request date\": \"Thu, 22 Mar 2018 11:46:44 GMT\",  \"shapefile source\": \"ftp://newftp.epa.gov/exposure/BasinsData/NHDPlus21/NHDPlus01060002.zip\" } }";
                        //string testGeometry = "{\"Geometry\": {\"02080107\": [{\"Latitude\": 37.437499999999972,\"Longitude\": -76.687499999999972,\"CellArea\": 0.0156250000000036,\"ContainedArea\": 0.00559514073505796,\"PercentArea\": 35.8089007043628},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.5625,\"CellArea\": 0.0156250000000053,\"ContainedArea\": 0.0100796700330301,\"PercentArea\": 64.5098882113707},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.437499999999972,\"CellArea\": 0.0156250000000142,\"ContainedArea\": 0.00780989236262298,\"PercentArea\": 49.9833111207416},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.312499999999957,\"CellArea\": 0.0156250000000036,\"ContainedArea\": 0.0012605882348706,\"PercentArea\": 8.06776470316997}]},\"Metadata\": {}}";
                        geo = JsonConvert.DeserializeObject<GeometryResponse>(testGeometry);
                        break;
                    default:
                        return err.ReturnError("Input error - GeometryType provided is invalid. Provided value = " + input.GeometryType);
                }
            }
            // Check for any errors
            if (!String.IsNullOrWhiteSpace(error))
            {
                return err.ReturnError(error);
            }

            // Collect all unique points in Catchment
            List<GeometryPoint> points = new List<GeometryPoint>();
            List<string> pointsSTR = new List<string>();
            foreach (KeyValuePair<string, Catchment> k in geo.data.geometry)
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

                // Initialize surfaceFlow catchment point object
                errorMsg = "";
                TimeSeriesInput surfaceTempInput = new TimeSeriesInput();
                surfaceTempInput = input;
                surfaceTempInput.Geometry = tsGeometry;            
                SurfaceRunoff.SurfaceRunoff sFlow = new SurfaceRunoff.SurfaceRunoff();
                sFlow.Input = inputFactory.SetTimeSeriesInput(surfaceTempInput, new List<string>() { "SURFFLOW" }, out errorMsg);
                surfaceFlow.Add(key, sFlow);
                errorMessages.Add(errorMsg);

                // Initialize subsurfaceFlow catchment point object
                errorMsg = "";
                TimeSeriesInput subSurfaceTempInput = new TimeSeriesInput();
                subSurfaceTempInput = input;
                subSurfaceTempInput.Geometry = tsGeometry;
                SubSurfaceFlow.SubSurfaceFlow subFlow = new SubSurfaceFlow.SubSurfaceFlow();
                subFlow.Input = inputFactory.SetTimeSeriesInput(subSurfaceTempInput, new List<string>() { "BASEFLOW" }, out errorMsg);
                subsurfaceFlow.Add(key, subFlow);
                errorMessages.Add(errorMsg);
            }
            
            // TODO: merge parallelized calls to surfaceRunoff and subsurfaceFlow
            // Parallelized surfaceRunoff
            object outputListLock = new object();
            List<string> surfaceError = new List<string>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = -1 };
            Parallel.ForEach(surfaceFlow, options, (KeyValuePair<string, SurfaceRunoff.SurfaceRunoff> surF) =>
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
            Parallel.ForEach(subsurfaceFlow, options, (KeyValuePair<string, SubSurfaceFlow.SubSurfaceFlow> subF) =>
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
            Parallel.ForEach(geo.data.geometry, options2, (KeyValuePair<string, Catchment> catchments) => {
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
                
                output.Data = (Utilities.Merger.MergeTimeSeries(catchmentPoints).Data);
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
            foreach(KeyValuePair<string, string> kv in geo.data.metadata)
            {
                if (!output.Metadata.ContainsKey(kv.Key))
                {
                    output.Metadata.Add(kv.Key, kv.Value);
                }
            }

            // Cleaning up output metadata
            Dictionary<string, string> cleanedMetaData = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> kv in output.Metadata)
            {
                if (!kv.Key.Contains("column"))
                {
                    cleanedMetaData.Add(kv.Key, kv.Value);
                }
            }
            output.Metadata = cleanedMetaData;

            // Add catchments metadata to output metadata
            foreach(KeyValuePair<string, string> kv in catchmentMeta)
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
        public GeometryResponse RequestData(string taskID, out string error)
        {
            error = "";

            GeometryResponse result = new GeometryResponse();

            // local testing
            //string dataUrl = "http://localhost:8000/hms/rest/api/v2/hms/data" + "?job_id=" + taskID;
            // deployment url
            string dataUrl = "https://qedinternal.epa.gov/hms/rest/api/v2/hms/data" + "?job_id=" + taskID;
            string status = "PENDING";
            int maxCount = 50;
            int count = 0;

            while( !(status == "SUCCESS") && !(status == "FAILURE") && count < maxCount)
            {
                if(count > 0)
                {
                    Task.Delay(5000).Wait();
                }

                using (var client = new HttpClient())
                {
                    result = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(dataUrl).Result);
                }
                status = result.status;
                count++;
            }
            if(count >= maxCount)
            {
                error = "Max number of requests reached in attempting to get geometry results. Max count:" + maxCount.ToString();
            }
            if (status.Equals("FAILURE"))
            {
                error = "Task to retrieve all gridpoints encountered an error.";
            }

            return result;
        }
    }
}
