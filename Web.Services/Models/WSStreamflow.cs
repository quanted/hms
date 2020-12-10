using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ITimeSeriesOutput> GetStreamflow(ITimeSeriesInput input)
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