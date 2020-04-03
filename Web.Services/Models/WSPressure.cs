using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Pressure Model
    /// </summary>
    public class WSPressure
    {

        /// <summary>
        /// Gets pressure data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetPressure(PressureInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Pressure object
            Pressure.Pressure press = new Pressure.Pressure();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            press.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacepressure" }, out errorMsg);

            // Gets the Pressure data.
            ITimeSeriesOutput result = press.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, press.Input, result);

            return result;
        }
    }
}