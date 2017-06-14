using Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
                // Validate precipitation sources.
                errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
                
                foreach (string source in input.SourceList)
                {
                    // Precipitation object
                    Precipitation.Precipitation precip = new Precipitation.Precipitation();

                    // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                    ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                    ITimeSeriesInput sInput = iFactory.SetTimeSeriesInput(input, out errorMsg);
                    sInput.Source = source;

                    // Set input to precip object.
                    precip.Input = sInput;
                    precip.Input.TemporalResolution = "daily";
                    // Assigns coordinates of the ncdc station to the point coordinate values for the other data source calls.
                    if (!source.Contains("ncdc"))
                    {
                        if (output.Metadata.ContainsKey("ncdc_latitude") && output.Metadata.ContainsKey("ncdc_longitude"))
                        {
                            precip.Input.Geometry.Point.Latitude = Convert.ToDouble(output.Metadata["ncdc_latitude"]);
                            precip.Input.Geometry.Point.Longitude = Convert.ToDouble(output.Metadata["ncdc_longitude"]);
                        }
                        else
                        {
                            errorMsg = "ERROR: Coordinate information was not found or is invalid for the specified NCDC station.";
                        }
                        precip.Input.DateTimeSpan.EndDate = precip.Input.DateTimeSpan.EndDate.AddDays(1);
                    }


                    // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
                    if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                    // Gets the Precipitation data.
                    ITimeSeriesOutput result = precip.GetData(out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                    if (source.Contains("ncdc"))
                    {
                        output = result;
                    }
                    else
                    {
                        output = Utilities.Merger.MergeTimeSeries(output, result);
                    }
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