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

namespace Web.Services.Models
{
    /// <summary>
    /// Result structure of the json string retrieved from ncdc.
    /// </summary>
    public class Result
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<ResultData> data { get; set; }
    }

    /// <summary>
    /// ResultData structure of the json string retrieved from ncdc.
    /// </summary>
    public class ResultData
    {
        public string id { get; set; }
        public double distance { get; set; }
        public StationData data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }

    /// <summary>
    /// StationData structure of the json string retrieved from ncdc.
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

        private enum PrecipSources { compare, nldas, gldas, ncdc, daymet, wgen };

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetPrecipCompareData(PrecipCompareInput input)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();


            /*TODO:
            1. DONE: Only make calls to data sources specified by user
            2. DONE: Find NCDC station nearest to COMID centroid if no station is provided
            3. Aggregate by weighted spatial average if user chooses to do so
            4. Aggregate daily/monthly/yearly/extreme events
               a. If extreme event, handle cases with daily/5 day total thresholds
            5. Check for errors such as missing NCDC data
               a. Possibly add notes to metadata if some data is missing from output
            */

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            input.SourceList.Add("ncdc");
            output.DataSource = string.Join(" - ", input.SourceList.ToArray());


            if (input.Weighted)
            {
                errorMsg = "ERROR: Weighted spatial average is currently unavailable.";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            }
            input.Weighted = false;

            //NCDC station is required. If only a ComID is provided, use catchment centroid coordinates to find nearest NCDC station.
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

                string ncdcBaseURL = "http://localhost:7777/hms/gis/ncdc/stations/?latitude=" + centroid.Latitude.ToString() + "&longitude=" + centroid.Longitude.ToString() + "&geometry=point&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326";//"&comid=" + input.Geometry.ComID.ToString() +
                //string ncdcBaseURL = "http://localhost:7777/hms/gis/ncdc/stations/?startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326&comid=" + input.Geometry.ComID.ToString();

                //Using FLASK NCDC webservice            
                string data = DownloadData(out errorMsg, ncdcBaseURL);
                //return null;
                Result result = JSON.Deserialize<Result>(data);
                //Set NCDC station to closest station regardless of type
                input.Geometry.StationID = result.data[0].id.ToString();
                foreach (ResultData details in result.data)//Opt for closest GHCND station, if any
                {
                    if (details.id.Contains("GHCND"))
                    {
                        input.Geometry.StationID = details.id.ToString();
                        break;
                    }
                }
            }
            input.Geometry.GeometryMetadata.Add("stationID", input.Geometry.StationID);

            //Check for extreme events and required parameters
            if (input.TemporalResolution != "daily" && input.TemporalResolution != "monthly" && input.TemporalResolution != "yearly")
            {
                //Extreme Precipitation Events are essentially combined. 
                if(input.DailyThreshold > input.FiveDayThreshold)
                {

                }
            }      

            input.Source = "ncdc";
            input.SourceList.Remove("ncdc");
            // Validate precipitation sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            List<Precipitation.Precipitation> precipList = new List<Precipitation.Precipitation>();
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

            // NCDC Call
            Precipitation.Precipitation ncdc = new Precipitation.Precipitation();
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory nFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput nInput = nFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Set input to precip object.
            ncdc.Input = nInput;
            //ncdc.Input.TemporalResolution = "daily";
            ncdc.Input.TemporalResolution = input.TemporalResolution;
            ncdc.Input.Geometry.GeometryMetadata["token"] = (ncdc.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? ncdc.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            ITimeSeriesOutput nResult = ncdc.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            output = nResult;

            // Construct Precipitation objects for Parallel execution in the preceeding Parallel.ForEach statement.
            foreach (string source in input.SourceList)
            {
                // Precipitation object
                Precipitation.Precipitation precip = new Precipitation.Precipitation();
                PointCoordinate point = new PointCoordinate();
                if (output.Metadata.ContainsKey("ncdc_latitude") && output.Metadata.ContainsKey("ncdc_longitude"))
                {
                    point.Latitude = Convert.ToDouble(output.Metadata["ncdc_latitude"]);
                    point.Longitude = Convert.ToDouble(output.Metadata["ncdc_longitude"]);
                    input.Geometry.Point = point;
                }
                else
                {
                    errorMsg = "ERROR: Coordinate information was not found or is invalid for the specified NCDC station.";
                }
                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                input.Source = source;
                ITimeSeriesInput sInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                // Set input to precip object.
                precip.Input = sInput;
                //precip.Input.TemporalResolution = "daily";
                precip.Input.TemporalResolution = input.TemporalResolution;

                precip.Input.DateTimeSpan.EndDate = precip.Input.DateTimeSpan.EndDate.AddDays(1);
                if (!precip.Input.Geometry.GeometryMetadata.ContainsKey("leapYear"))
                {
                    precip.Input.Geometry.GeometryMetadata.Add("leapYear", "correction");
                }

                precipList.Add(precip);
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
            });

            if (errorList.FindIndex(errorStr => errorStr.Contains("ERROR")) != -1)
            {
                return err.ReturnError(string.Join(",", errorList.ToArray()));
            }


            foreach (ITimeSeriesOutput result in outputList)
            {
                /*Spatial weighted average aggregation
                ITimeSeriesOutput updated = result;
                if (input.Weighted && updated.Data.Count > 0)
                {
                    Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
                    Utilities.GeometryData gd = null;
                    input.Geometry.GeometryMetadata["precipSource"] = result.DataSource.ToString();
                    gd = cd.getData(input, new List<string> { input.Geometry.ComID.ToString() }, out errorMsg);
                    ITimeSeriesOutput aggregated = cd.getCatchmentAggregation(input, result, gd, input.Weighted);
                    ITimeSeriesOutput reduced = oFactory.Initialize();
                    foreach (var data in aggregated.Data)
                    {
                        //TODO: Reformat this to take sum and average of all cell values and use that as data for timeseries.
                        reduced.Data.Add(data.Value[0], new List<string> { data.Value[3].ToString() });//Will cause crashes
                    }
                }
                output = Utilities.Merger.MergeTimeSeries(output, updated);*/
                output = Utilities.Merger.MergeTimeSeries(output, result);
            }

            output.Metadata.Add("column_1", "Date");
            output.Metadata.Add("column_2", "ncdc");
            //TODO: Update Statistics 
            //output = Utilities.Statistics.GetStatistics(out errorMsg, output);

            return output;
        }

        /// <summary>
        /// Pulls NCDC station details from flask webservice.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string dataURL = "http://localhost:7777/hms/data?job_id=";
            WebClient myWC = new WebClient();
            string data = "";
            dynamic taskData = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string jobID = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    jobID = JSON.Deserialize<Dictionary<string, string>>(reader.ReadToEnd())["job_id"];
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }

                retries = 50;
                status = "";
                taskData = "";
                bool success = false;
                while (retries > 0 && !success && !jobID.Equals(""))
                {
                    Thread.Sleep(6000);
                    WebRequest wr = WebRequest.Create(dataURL + jobID);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    taskData = JSON.Deserialize<dynamic>(data);
                    if (taskData["status"] == "SUCCESS")
                    {
                        success = true;
                    }
                    else if (taskData["status"] == "FAILURE" || taskData["status"] == "PENDING")
                    {
                        break;
                    }
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Could not find NCDC stations for the given geometry." + ex.Message;
            }
            return data;
        }
    }
}