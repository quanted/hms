using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Utilities;
using Web.Services.Controllers;
using System.Diagnostics;
using System.Text.Json;

namespace Web.Services.Models
{
    /// <summary>
    /// Result structure of the json string retrieved from ncei.
    /// </summary>
    public class Result
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<ResultData> data { get; set; }
    }

    /// <summary>
    /// Result structure of the json string retrieved from ncei.
    /// </summary>
    public class ResultString
    {
        public string id { get; set; }
        public string status { get; set; }
        public string data { get; set; }
    }

    /// <summary>
    /// ResultData structure of the json string retrieved from ncei.
    /// </summary>
    public class ResultData
    {
        public string id { get; set; }
        public double distance { get; set; }
        public StationData data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }

    /// <summary>
    /// StationData structure of the json string retrieved from ncei.
    /// </summary>
    public class StationData
    {
        public double elevation { get; set; }
        public string mindate { get; set; }
        public string maxdate { get; set; }
        public double latitude { get; set; }
        public string name { get; set; }
        public double datacoverage { get; set; }
        public string id { get; set; }
        public string elevationUnit { get; set; }
        public double longitude { get; set; }
    }

    /// <summary>
    /// HMS Web Service PrecipCompare Model
    /// </summary>
    public class WSPrecipCompare
    {

        private enum PrecipSources { compare, nldas, gldas, trmm, ncei, daymet, wgen };

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetPrecipCompareData(PrecipitationCompareInput input)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            if (input == null)
            {
                errorMsg = "ERROR: Invalid input parameters.";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            }
            
            Debug.WriteLine("Precip compare request recieved...");
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            //input.SourceList.Add("ncei");
            output.DataSource = string.Join(" - ", input.SourceList.ToArray());

            if(input.Geometry.GeometryMetadata == null)
            {
                input.Geometry.GeometryMetadata = new Dictionary<string, string>();
            }

            if (input.Weighted && input.Geometry.ComID <= 0)
            {
                errorMsg = "ERROR: ComID required for spatial weighted average.";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            }
            
            //In case of annual comparison, make sure that a valid input must have more than one year (i.e. start and end year are not the same).
            if(input.TemporalResolution == "annual" && input.DateTimeSpan.StartDate.Year == input.DateTimeSpan.EndDate.Year)
            {
                errorMsg = "ERROR: More than one year must be given for annual comparison.";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            }

            //NCEI station is required. If only a ComID is provided, use catchment centroid coordinates to find nearest NCEI station.
            if (input.Geometry.StationID == null)
            {


                errorMsg = "";
                string dbPath = "./App_Data/catchments.sqlite";
                string query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + input.Geometry.ComID.ToString();
                Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
                if (centroidDict.Count == 0)
                {
                    errorMsg = "ERROR: Unable to find catchment in database. ComID: " + input.Geometry.ComID.ToString();
                    return null;
                }

                IPointCoordinate centroid = new PointCoordinate()
                {
                    Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                    Longitude = double.Parse(centroidDict["CentroidLongitude"])
                };
                string nceiQuery = "/hms/gis/ncdc/stations/?latitude=" + centroid.Latitude.ToString() + "&longitude=" + centroid.Longitude.ToString() + "&geometry=point&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326";//"&comid=" + input.Geometry.ComID.ToString() +

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                
                // Calling to HMS Flask web service.
                Utilities.NCEIResult result = Utilities.WebAPI.RequestData<Utilities.NCEIResult>(nceiQuery).Result;

                ////Set NCEI station to closest station regardless of type
                double bestCoverage = 0.0;
                double closestDistance = 10000.0;
                foreach (NCEIStations details in result.data)//Opt for closest GHCND station, if any
                {
                    if (details.id.Contains("GHCND"))
                    {
                        if (details.distance <= closestDistance && details.data.datacoverage > bestCoverage)
                        {
                            closestDistance = details.distance;
                            bestCoverage = details.data.datacoverage;
                            input.Geometry.StationID = details.id.ToString();
                            input.Geometry.Point = new PointCoordinate() { Latitude = details.data.latitude, Longitude = details.data.longitude };
                        }
                    }
                }
            }
            input.Geometry.GeometryMetadata.Add("stationID", input.Geometry.StationID);

            //Check for extreme events and required parameters
            if (input.TemporalResolution == "extreme_5")
            {
                try
                {
                    input.Geometry.GeometryMetadata.Add("dailyThreshold", input.ExtremeDaily.ToString());
                    input.Geometry.GeometryMetadata.Add("totalThreshold", input.ExtremeTotal.ToString());
                }
                catch(Exception ex)
                {
                    errorMsg = "ERROR: One or more threshold values for extreme events were not provided.";
                    if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
                }
            }
            
            input.Source = "ncei";
            input.SourceList.Remove("ncei");
            // Validate precipitation sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            List<Precipitation.Precipitation> precipList = new List<Precipitation.Precipitation>();
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();
                        
            // NCEI Call
            Precipitation.Precipitation ncei = new Precipitation.Precipitation();
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory nFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput nInput = nFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Set input to precip object.
            // NCEI local time correction to GMT
            ncei.Input = nInput;
            ncei.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, ncei.Input.Geometry.Point) as Data.Timezone;
            //ncei.Input.DateTimeSpan.StartDate = ncei.Input.DateTimeSpan.StartDate.AddHours(ncei.Input.Geometry.Timezone.Offset);
            //ncei.Input.DateTimeSpan.EndDate = ncei.Input.DateTimeSpan.EndDate.AddHours(ncei.Input.Geometry.Timezone.Offset);

            ncei.Input.TemporalResolution = "daily";
            ncei.Input.Geometry.GeometryMetadata["token"] = (ncei.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? ncei.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            ITimeSeriesOutput nResult = ncei.GetData(out errorMsg);
            //nResult.Data = Utilities.Time.TimeSeriesShift(-1.0 * ncei.Input.Geometry.Timezone.Offset, nResult.Data, ncei.Input.DateTimeSpan.DateTimeFormat);
            //nResult.Data = Utilities.TemporalAggregation.Aggregation(input.TemporalResolution, 1, nResult, ncei.Input);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            output = nResult;
            Debug.WriteLine("Data retrieved for: NCEI");

            // Construct Precipitation objects for Parallel execution in the preceeding Parallel.ForEach statement.
            foreach (string source in input.SourceList)
            {
                // Precipitation object
                Precipitation.Precipitation precip = new Precipitation.Precipitation();
                PointCoordinate point = new PointCoordinate();
                if (output.Metadata.ContainsKey("ncei_latitude") && output.Metadata.ContainsKey("ncei_longitude"))
                {
                    point.Latitude = Convert.ToDouble(output.Metadata["ncei_latitude"]);
                    point.Longitude = Convert.ToDouble(output.Metadata["ncei_longitude"]);
                    input.Geometry.Point = point;
                }
                else
                {
                    errorMsg = "ERROR: Coordinate information was not found or is invalid for the specified NCEI station.";
                }
                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                input.Source = source;
                ITimeSeriesInput sInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                // Set input to precip object.
                precip.Input = sInput;
                precip.Input.TemporalResolution = input.TemporalResolution;
                if(precip.Input.TemporalResolution == "extreme_5")
                {
                    precip.Input.TemporalResolution = "daily";
                }
                else if(precip.Input.TemporalResolution == "annual")
                {
                    precip.Input.TemporalResolution = "yearly";
                }
                precip.Input.DateTimeSpan.EndDate = precip.Input.DateTimeSpan.EndDate.AddDays(1);
                if (!precip.Input.Geometry.GeometryMetadata.ContainsKey("leapYear"))
                {
                    precip.Input.Geometry.GeometryMetadata.Add("leapYear", "correction");
                }

                precipList.Add(precip);
                string debugString = "Input object constructed for: " + source;
                Debug.WriteLine(debugString);
            }

            List<string> errorList = new List<string>();
            object outputListLock = new object();
            var options = new ParallelOptions { MaxDegreeOfParallelism = -1 };

            Parallel.ForEach(precipList, options, (Precipitation.Precipitation precip) =>
            {
                // Gets the Precipitation data.
                string errorM = "";
                ITimeSeriesOutput result = precip.GetData(out errorM);
                lock (outputListLock)
                {
                    errorList.Add(errorM);
                    outputList.Add(result);
                }
                string debugString = "Data retrieved for: " + precip.Input.Source;
                Debug.WriteLine(debugString);
            });

            if (errorList.FindIndex(errorStr => errorStr.Contains("ERROR")) != -1)
            {
                return err.ReturnError(string.Join(",", errorList.ToArray()));
            }

            Debug.WriteLine("Precip Compare data has been collected");
            foreach (ITimeSeriesOutput result in outputList)
            {
                string debugString = "Total data points from source: " + result.DataSource + " = " + result.Data.Count.ToString();
                Debug.WriteLine(debugString);
                ITimeSeriesOutput aggregated = oFactory.Initialize();
                //Spatial weighted average aggregation
                if (input.Weighted && result.Data.Count > 0)
                {
                    Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
                    Utilities.GeometryData gd = null;
                    input.Geometry.GeometryMetadata["precipSource"] = result.DataSource.ToString();
                    gd = cd.getData(input, new List<string> { input.Geometry.ComID.ToString() }, out errorMsg);
                    aggregated = cd.getCatchmentAggregation(input, result, gd, input.Weighted);
                    result.Data = aggregated.Data;
                }
                output = Utilities.Merger.MergeTimeSeries(output, result);
                if (result.Metadata.Values.Contains("ERROR"))
                {
                    output.Metadata.Add(result.DataSource.ToString() + " ERROR", "The service is unavailable or returned no valid data.");
                }
            }
            //output.Metadata.Add("column_1", "Date");
            //output.Metadata.Add("column_2", "ncei");
            output = Utilities.Statistics.GetCompareStatistics(out errorMsg, input, output);
            
            return output;
        }
    }
}