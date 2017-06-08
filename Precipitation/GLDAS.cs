using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitation
{
    /// <summary>
    /// Precipitation GLDAS class.
    /// </summary>
    class GLDAS
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
            string data = gldas.GetData(out errorMsg, "PRECIP", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput gldasOutput = output;
            gldasOutput = gldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
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
            output.Metadata.Add("gldas_column_1", "Date");
            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "in"; }

            // NLDAS static methods used for aggregation as GLDAS is identical in function. Modifier refers to the 3hr different to nldas's hourly resolution.
            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("gldas_column_2", "Daily Total");
                    return output;
                case "weekly":
                    output.Data = NLDAS.WeeklyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("gldas_column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 3.0, output, input);
                    output.Metadata.Add("gldas_column_2", "Monthly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, 3.0, output, input) : output.Data;
                    output.Metadata.Add("gldas_column_2", "Hourly Average");
                    return output;
            }
        }
    }
}
