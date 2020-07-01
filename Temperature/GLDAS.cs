using Data;
using System.Collections.Generic;

namespace Temperature
{
    /// <summary>
    /// Temperature GLDAS class.
    /// </summary>
    public class GLDAS
    {

        /// <summary>
        /// Makes the GetData call to the base GLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>S
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, "Temp", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput gldasOutput = output;
            gldasOutput = gldas.SetDataToOutput(out errorMsg, "Temperature", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            gldasOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return gldasOutput;
        }

        /// <summary>
        /// Checks for temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("gldas_temporalresolution", input.TemporalResolution);

            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "F"; }
            output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, output, input) : output.Data;

            output.Metadata.Add("column_1", "date");

            switch (input.TemporalResolution)
            {
                case "daily":
                    // Combined high/low/average
                    output.Data = NLDAS.DailyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Max Temp");
                    output.Metadata.Add("column_3", "Min Temp");
                    output.Metadata.Add("column_4", "Avg Temp");
                    return output;
                case "monthly":
                    // Combined high/low/average
                    output.Data = NLDAS.DailyValues(out errorMsg, output, input, "all");
                    output.Data = NLDAS.MonthlyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Avg Max Temp");
                    output.Metadata.Add("column_3", "Avg Min Temp");
                    output.Metadata.Add("column_4", "Avg Average Temp");
                    return output;
                default:
                case "default":
                    return output;
            }
        }

        /// <summary>
        /// Calls the function in Data.Source.GLDAS that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.GLDAS.CheckStatus("Temperature", input);
        }
    }
}
