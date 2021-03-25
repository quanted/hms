using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coastal
{
    public class NOAACoastal
    {
        /// <summary>
        /// Default Coastal constructor
        /// </summary>
        public NOAACoastal()
        { }

        /// <summary>
        /// Makes the GetData call to the base NOAACoastal class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";
            Data.Source.NOAACoastal noaaCoastal = new Data.Source.NOAACoastal();

            // bool validInputs = ValidateInputs(input, out errorMsg);
            // if (errorMsg.Contains("ERROR")) { return null; }
            string data = noaaCoastal.GetData(out errorMsg, "COAST", input, retries);


            ITimeSeriesOutput noaaCoastalOutput = output;
            if (errorMsg.Contains("ERROR")) { return null; }
            else
            {
                noaaCoastalOutput = noaaCoastal.SetDataToOutput(out errorMsg, "Coastal", data, output, input);
            }
            // noaaCoastalOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return noaaCoastalOutput;
        }


        /// <summary>
        /// Validate input dates and coordinates for noaa coastal data.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // NDLAS date range 1979-01-01T13 - Present(- 5 days)
            DateTime date0 = new DateTime(1979, 1, 1);
            DateTime date1 = DateTime.Now.AddDays(-5);
            string dateFormat = "yyyy-MM-dd";
            if (DateTime.Compare(input.DateTimeSpan.StartDate, date0) < 0 || (DateTime.Compare(input.DateTimeSpan.StartDate, date1) > 0))
            {
                errors.Add("ERROR: Start date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". Start date provided: " + input.DateTimeSpan.StartDate.ToString(dateFormat));
            }
            if (DateTime.Compare(input.DateTimeSpan.EndDate, date0) < 0 || DateTime.Compare(input.DateTimeSpan.EndDate, date1) > 0)
            {
                errors.Add("ERROR: End date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". End date provided: " + input.DateTimeSpan.EndDate.ToString(dateFormat));
            }

            // TODO: Add validation for other NOAA inputs //
            ////////////////////////////////////////////////


            if (errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }

        /// <summary>
        /// Calls the function in Data.Source.NOAACoastal that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.NOAACoastal.CheckStatus("Coastal", input);
        }


        /// <summary>
        /// Checks temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /*
        public ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("nldas_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");
            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = DailyAggregatedSum(out errorMsg, 23, 1.0, output, input);
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                case "yearly":
                    output.Data = YearlyAggregatedSum(out errorMsg, 23, 1.0, output, input);
                    output.Metadata.Add("column_2", "Yearly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, 1.0, output, input) : output.Data;
                    output.Metadata.Add("column_2", "Hourly Total");
                    return output;
            }
        }
        */
    }
}