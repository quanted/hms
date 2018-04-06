using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Serivce WorkFlow Model
    /// </summary>
    public class WSWorkFlow
    {

        private enum PrecipSources { compare, nldas, gldas, ncdc, daymet, wgen };

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetWorkFlowData(WorkFlowCompareInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            input.SourceList = new List<string>()
            {
                { "ncdc" },
                { "nldas" },
                { "gldas" },
                { "daymet" }
            };

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.DataSource = string.Join(" - ", input.SourceList.ToArray());

            if (input.Dataset.Contains("Precipitation"))
            {
                input.SourceList = new List<string>()
                {
                    { "nldas" },
                    { "gldas" },
                    { "daymet" }
                };
                input.Source = "ncdc";
                // Validate precipitation sources.
                errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                List<Precipitation.Precipitation> precipList = new List<Precipitation.Precipitation>();
                List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

                // NCDC Call
                Precipitation.Precipitation ncdc = new Precipitation.Precipitation();
                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory nFactory = new TimeSeriesInputFactory();
                ITimeSeriesInput nInput = nFactory.SetTimeSeriesInput(input, new List<string>() { "PRECIP" }, out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
                
                // Set input to precip object.
                ncdc.Input = nInput;
                ncdc.Input.TemporalResolution = "daily";
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
                    ITimeSeriesInput sInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "PRECIP" }, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                    // Set input to precip object.
                    precip.Input = sInput;
                    precip.Input.TemporalResolution = "daily";

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


                foreach (ITimeSeriesOutput result in outputList)
                {
                    output = Utilities.Merger.MergeTimeSeries(output, result);
                }

                output.Metadata.Add("column_1", "date");
                output.Metadata.Add("column_2", "ncdc");
                output = Utilities.Statistics.GetStatistics(out errorMsg, output);

                return output;
            }
            else if (input.Dataset.Contains("SurfaceRunoff"))
            {
                //TODO: Do runoff source iteration.
                return err.ReturnError("ERROR. Workflow has not yet been implemented.");
            }
            else
            {
                return err.ReturnError("ERROR: WorkFlow source not found or is invalid.");
            }
        }
    }
}