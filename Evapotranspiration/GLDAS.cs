using Data;
using System.Collections.Generic;

namespace Evapotranspiration
{
    /// <summary>
    /// Evapotranspiration GLDAS class.
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
            ITimeSeriesOutput gldasOutput = output;
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, "Evapotrans", input);
            if (errorMsg.Contains("ERROR")) { return null; }
            /*
            if (input.Algorithm != "gldas")
            {
                data = gldas.GetData(out errorMsg, "SurfaceTemp", input);
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            else
            {
                data = gldas.GetData(out errorMsg, "Evapotrans", input);
                if (errorMsg.Contains("ERROR")) { return null; }
            }*/
            gldasOutput = gldas.SetDataToOutput(out errorMsg, "Evapotranspiration", data, output, input);
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
            output.Metadata.Add("column_1", "Date");

            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "in"; }

            // NLDAS static methods used for aggregation as GLDAS is identical in function. Modifier refers to the 3hr difference to nldas's hourly resolution.
            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
                case "weekly":
                    output.Data = NLDAS.WeeklyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, 3.0, output, input) : output.Data;
                    output.Metadata.Add("column_2", "Hourly Average");
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
            return Data.Source.GLDAS.CheckStatus("Evapotranspiration", input);
        }
    }
}
