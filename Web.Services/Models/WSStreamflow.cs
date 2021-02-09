using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Streamflow Model
    /// </summary>
    public class WSStreamflow
    {

        /// <summary>
        /// Gets streamflow data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input">ITimeSeriesInput</param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetStreamflow(StreamflowInput input)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            List<string> srcGauges = new List<string>()
            {
                "usgs",
                "streamgauge",
                "nwis"
            };

            // Streamflow object
            Streamflow.Streamflow sf = new Streamflow.Streamflow();
            if (srcGauges.Contains(input.Source))
            {
                input.Source = "nwis";
            }
            if (input.precipSource != null)
            {
                sf.precipSource = input.precipSource;
            }
            if (input.runoffSource != null)
            {
                sf.runoffSource = input.runoffSource;
            }
            if (input.streamBoundarySource != null)
            {
                sf.streamBoundarySource = input.streamBoundarySource;
            }
            if (input.precipTS != null)
            {
                sf.precipTS = input.precipTS;
            }
            if (input.runoffTS != null)
            {
                sf.runoffTS = input.runoffTS;
            }
            if (input.baseflowTS != null)
            {
                sf.baseflowTS = input.baseflowTS;
            }
            if (input.streamTS != null)
            {
                sf.streamTS = input.streamTS;
            }
            sf.constantLoading = input.constantLoading;
            if (input.loadings != null)
            {
                sf.loadings = input.loadings;
            }

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            sf.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "streamflow" }, out errorMsg);
           

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the Precipitation data.
            ITimeSeriesOutput<List<double>> result = sf.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            //result = Utilities.Statistics.GetStatistics(out errorMsg, sf.Input, result);
            return result.ToDefault(input.DataValueFormat);
        }
    }
}