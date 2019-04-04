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
    public class WSPrecipExtraction
    {

        private enum PrecipSources { extraction, nldas, gldas, ncei, daymet, prism };

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWorkFlowData(PrecipitationExtractionInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.DataSource = string.Join(" - ", input.SourceList.ToArray());

            // Validate precipitation sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            List<Precipitation.Precipitation> precipList = new List<Precipitation.Precipitation>();
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

            if (input.SourceList.Contains("ncei"))
            {
                //Find nearest station to lat/long?
                if(input.Geometry.GeometryMetadata == null)
                {
                    input.Geometry.GeometryMetadata = new Dictionary<string, string>();
                }
                input.Geometry.GeometryMetadata["stationID"] = input.Geometry.StationID;
                input.Geometry.GeometryMetadata["token"] = (input.Geometry.GeometryMetadata.ContainsKey("token")) ? input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                input.Source = "ncei";
                
                // NCEI Call
                Precipitation.Precipitation ncei = new Precipitation.Precipitation();
                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory nFactory = new TimeSeriesInputFactory();
                ITimeSeriesInput nInput = nFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                // Set input to precip object.
                ncei.Input = nInput;
                //ncei.Input.TemporalResolution = "daily";
                ncei.Input.TemporalResolution = input.TemporalResolution;
                ncei.Input.Geometry.GeometryMetadata["token"] = (ncei.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? ncei.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                ITimeSeriesOutput nResult = ncei.GetData(out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
                outputList.Add(nResult);
                input.SourceList.Remove("ncei");
            }

            PointCoordinate point = new PointCoordinate()
            {
                Latitude = Double.Parse(outputList[0].Metadata["ncei_latitude"]),
                Longitude = Double.Parse(outputList[0].Metadata["ncei_longitude"])
            };

            // Construct Precipitation objects for Parallel execution in the preceeding Parallel.ForEach statement.
            foreach (string source in input.SourceList)
            {
                // Precipitation object
                Precipitation.Precipitation precip = new Precipitation.Precipitation();

                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                input.Source = source;
                ITimeSeriesInput sInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
                input.Geometry.Point = point;
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                // Set input to precip object.
                precip.Input = sInput;
                precip.Input.TemporalResolution = "daily";

                //precip.Input.DateTimeSpan.EndDate = precip.Input.DateTimeSpan.EndDate.AddDays(1);
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
                precip.Input.DateTimeSpan.EndDate = new DateTime(precip.Input.DateTimeSpan.EndDate.Year, precip.Input.DateTimeSpan.EndDate.Month, precip.Input.DateTimeSpan.EndDate.Day, 00, 00, 00);
                precip.Input.DateTimeSpan.StartDate = new DateTime(precip.Input.DateTimeSpan.StartDate.Year, precip.Input.DateTimeSpan.StartDate.Month, precip.Input.DateTimeSpan.StartDate.Day, 00, 00, 00);
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

            output = outputList.ElementAt(0);
            foreach (ITimeSeriesOutput result in outputList.Skip(1))
            {
                output = Utilities.Merger.MergeTimeSeries(output, result);
            }

            //output.Metadata.Add("column_1", "Date");
            output.Metadata["column_2"] = "ncei";
            output = Utilities.Statistics.GetStatistics(out errorMsg, input, output);

            return output;
        }
    }
}