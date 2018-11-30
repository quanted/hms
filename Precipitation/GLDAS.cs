using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Precipitation
{
    /// <summary>
    /// Precipitation GLDAS class.
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
            List<string> data = gldas.GetData(out errorMsg, "PRECIP", input);
            //if (errorMsg.Contains("ERROR")) { return null; }
            //if (data.Contains("ERROR"))
            //{
            //    string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //    errorMsg = lines[0] + " Dataset: precipitation; Source: " + input.Source;
            //    return null;
            //}

            ITimeSeriesOutput gldasOutput = output;
            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new ErrorOutput();
                output = err.ReturnError("Precipitation", "gldas", errorMsg);
                errorMsg = "";
                return output;
            }
            else
            {
                gldasOutput = gldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            }

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
            output.Data = ConvertToHourly(out errorMsg, output, input);
            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "in"; }
            output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, 3.0, output, input) : output.Data;

            // NLDAS static methods used for aggregation as GLDAS is identical in function. Modifier refers to the 3hr different to nldas's hourly resolution.
            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyAggregatedSum(out errorMsg, 7, 3.0, output, input);
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
            return Data.Source.GLDAS.CheckStatus("Precipitation", input);
        }

        /// <summary>
        /// Converts metric kg m-2 s-1 to kg m-2
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ConvertToHourly(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            // seconds to hours
            double modifier = 3600;
            //double modifier = 1;
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                    ( modifier * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0])).ToString(input.DataValueFormat)
                });
            }
            return tempData;
        }
    }
}