using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    }

    /// <summary>
    /// Response from HMS-GIS
    /// </summary>
    public class GeometryResponse
    {
        /// <summary>
        /// Geometry component of HMS-GIS response
        /// </summary>
        public Dictionary<string, List<CatchmentPoint>> Geometry;

        /// <summary>
        /// Metadata component of HMS-GIS response
        /// </summary>
        public Dictionary<string, string> Metadata;
    }

    /// <summary>
    /// Catchment point data
    /// </summary>
    public class CatchmentPoint
    {
        /// <summary>
        /// Latitude of centroid
        /// </summary>
        public double Latitude;
        /// <summary>
        /// Longitude of centroid
        /// </summary>
        public double Longitude;
        /// <summary>
        /// Total cell area
        /// </summary>
        public double CellArea;
        /// <summary>
        /// Cell area that intersects the catchment
        /// </summary>
        public double ContainedArea;
        /// <summary>
        /// Percent coverage of the intersection
        /// </summary>
        public double PercentArea;
    }

    /// <summary>
    /// Point object
    /// </summary>
    public class Point
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
        public ITimeSeriesOutput GetTotalFlowData(TotalFlowInput input)
        {
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

            // TODO: Add metadata
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            GeometryResponse geo = new GeometryResponse();
            string baseUrl = "";
            switch (input.GeometryType.ToLower())
            {
                case "huc":
                    // use case 1
                    // use case 2
                    string hucID = input.GeometryInput;
                    using( var client = new HttpClient())
                    {
                        geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(baseUrl + "/api/GridCell/" + hucID).Result);
                    }
                    break;
                case "commid":
                    // use case 3
                    string commid = input.GeometryInput;
                    using (var client = new HttpClient())
                    {
                        geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(baseUrl + "/api/GridCell/commid/" + commid).Result);
                    }
                    break;
                case "catchmentid":
                    // use case 3
                    string catchmentID = input.GeometryInput;
                    using (var client = new HttpClient())
                    {
                        geo = JsonConvert.DeserializeObject<GeometryResponse>(client.GetStringAsync(baseUrl + "/api/GridCell/catchmentid/" + catchmentID).Result);
                    }
                    break;
                case "catchment":
                    // use case 4
                    // Use POST call with geometry 
                    break;
                case "flowline":
                    // use case 5
                    // Use POST call with geometry
                    break;
                case "points":
                    // use case 6
                    // use case 7
                    // GET call with points, hms-gis will get geometries
                    break;
                case "test":
                    string testGeometry = "{\"Geometry\": {\"02080107\": [{\"Latitude\": 37.437499999999972,\"Longitude\": -76.687499999999972,\"CellArea\": 0.0156250000000036,\"ContainedArea\": 0.00559514073505796,\"PercentArea\": 35.8089007043628},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.5625,\"CellArea\": 0.0156250000000053,\"ContainedArea\": 0.0100796700330301,\"PercentArea\": 64.5098882113707},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.437499999999972,\"CellArea\": 0.0156250000000142,\"ContainedArea\": 0.00780989236262298,\"PercentArea\": 49.9833111207416},{\"Latitude\": 37.437499999999972,\"Longitude\": -76.312499999999957,\"CellArea\": 0.0156250000000036,\"ContainedArea\": 0.0012605882348706,\"PercentArea\": 8.06776470316997}]},\"Metadata\": {}}";
                    geo = JsonConvert.DeserializeObject<GeometryResponse>(testGeometry);
                    break;
                default:
                    return err.ReturnError("Input error - GeometryType provided is invalid. Provided value = " + input.GeometryType);
            }

            // Collect all unique points in Catchment
            List<Point> points = new List<Point>();
            List<string> pointsSTR = new List<string>();
            foreach (KeyValuePair<string, List<CatchmentPoint>> k in geo.Geometry)
            {
                foreach (CatchmentPoint cp in k.Value)
                {
                    Point p = new Point();
                    p.Latitude = cp.Latitude;
                    p.Longitude = cp.Longitude;
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
            foreach (Point point in points)
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
            Parallel.ForEach(geo.Geometry, options2, (KeyValuePair<string, List<CatchmentPoint>> catchment) => {
                string catchmentID = catchment.Key;
                List<ITimeSeriesOutput> catchmentPoints = new List<ITimeSeriesOutput>();
                foreach (CatchmentPoint cp in catchment.Value)
                {
                    IPointCoordinate point = new PointCoordinate()
                    {
                        Latitude = cp.Latitude,
                        Longitude = cp.Longitude
                    };
                    string key = point.Latitude.ToString() + "," + point.Longitude.ToString();
                    ITimeSeriesOutput output1 = Utilities.Merger.ModifyTimeSeries(surfaceFlow[key].Output, cp.PercentArea / 100);
                    ITimeSeriesOutput output2 = Utilities.Merger.ModifyTimeSeries(subsurfaceFlow[key].Output, cp.PercentArea / 100);

                    if(iOutput.Data.Count == 0)
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

            return output;
        }
    }
}
