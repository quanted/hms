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
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (!validInputs) { return null; }

            Data.Source.GLDAS gldas = new Data.Source.GLDAS();

            ITimeSeriesOutput gldasOutput = output;
            ITimeSeriesOutput tempOutput = null;
            int retries = 0;
            bool dataComplete = false;
            Dictionary<string, bool> years = GetYears(input);

            while (!dataComplete)
            {

                List<string> data = gldas.GetData(out errorMsg, "PRECIP", input);

                if (errorMsg.Contains("ERROR"))
                {
                    Utilities.ErrorOutput err = new ErrorOutput();
                    output = err.ReturnError("Precipitation", "gldas", errorMsg);
                    errorMsg = "";
                    return output;
                }
                else
                {
                    if (retries > 0)
                    {
                        tempOutput = gldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
                        gldasOutput = gldas.MergeTimeseries(gldasOutput, tempOutput);
                    } 
                    else
                    {
                        gldasOutput = gldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
                    }
                    dataComplete = CheckYears(years, gldasOutput);
                    retries++;
                }
            }
            gldasOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return gldasOutput;
        }

        /// <summary>
        /// Gets a list of years between input start date and end date for gldas 2.0/2.1 validation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetYears(ITimeSeriesInput input)
        {
            Dictionary<string, bool> years = new Dictionary<string, bool>();
            DateTime startYear = new DateTime(input.DateTimeSpan.StartDate.Year, input.DateTimeSpan.StartDate.Month, input.DateTimeSpan.StartDate.Day);
            DateTime endYear = new DateTime(input.DateTimeSpan.EndDate.Year, input.DateTimeSpan.EndDate.Month, input.DateTimeSpan.EndDate.Day);
            if (startYear.Year == endYear.Year)
            {
                years.Add(startYear.Year.ToString(), false);
                return years;
            }
            while (startYear.Year != (endYear.Year + 1))
            {
                years.Add(startYear.Year.ToString(), false);
                startYear = startYear.AddYears(1);
            }
            return years;
        }

        /// <summary>
        /// Checks the list of years of the output to make sure all years are present from the expected list.
        /// </summary>
        /// <param name="years"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool CheckYears(Dictionary<string,bool> years, ITimeSeriesOutput output)
        {
            bool complete = true;
            bool checkComplete = false;
            int i = 0;
            List<string> keys = output.Data.Keys.ToList();
            int total = keys.Count;

            // Check last element 
            string lastKey = keys[keys.Count - 1].Split("-")[0];
            if (years.Last().Key == lastKey)
            {
                years[lastKey] = true;
            }

            while (!checkComplete)
            {
                string year = keys[i].Split("-")[0];
                years[year] = true;
                i = i + 2000;
                if(i > total)
                {
                    break;
                }
            }
            foreach(KeyValuePair<string, bool> keyValue in years)
            {
                if (keyValue.Value == false)
                    complete = false;
            }
            return complete;
        }

        /// <summary>
        /// Checks for temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("gldas_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");
            output.Data = ConvertToHourly(out errorMsg, output, input);
            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "in"; }

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
                case "yearly":
                    output.Data = NLDAS.YearlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Yearly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, 3.0, output, input) : ConvertToThreeHourly(out errorMsg, output, input);
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
            Dictionary<string, List<string>> tempData = output.Data;
            foreach(KeyValuePair<string, List<string>> kv in tempData)
            {
                tempData[kv.Key][0] = (modifier * Convert.ToDouble(kv.Value[0])).ToString(input.DataValueFormat);
            }
            return tempData;
        }

        /// <summary>
        /// Converts hourly to 3 hourly
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ConvertToThreeHourly(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            // 1hour to 3 hourly
            double modifier = 3;
            Dictionary<string, List<string>> tempData = output.Data;
            foreach (KeyValuePair<string, List<string>> kv in tempData)
            {
                tempData[kv.Key][0] = (modifier * Convert.ToDouble(kv.Value[0])).ToString(input.DataValueFormat);
            }
            return tempData;
        }

        /// <summary>
        /// Validate start/end date and lat/lon inputs
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private Boolean ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // GLDAS 2.0 date range 1948-01-01 - 2010-12-31
            // GLDAS 2.1 date range 2000-01-01 - Present(-2 month)
            DateTime date0 = new DateTime(1948, 1, 1);
            DateTime tempDate = DateTime.Now;
            DateTime date1 = new DateTime(tempDate.Year, tempDate.Month, 1).AddMonths(-2);
            string dateFormat = "yyyy-MM-dd";
            if (DateTime.Compare(input.DateTimeSpan.StartDate, date0) < 0 || (DateTime.Compare(input.DateTimeSpan.StartDate, date1) > 0))
            {
                errors.Add("ERROR: Start date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". Start date provided: " + input.DateTimeSpan.StartDate.ToString(dateFormat));
            }
            if (DateTime.Compare(input.DateTimeSpan.EndDate, date0) < 0 || DateTime.Compare(input.DateTimeSpan.EndDate, date1) > 0)
            {
                errors.Add("ERROR: End date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". End date provided: " + input.DateTimeSpan.EndDate.ToString(dateFormat));
            }

            // Validate Spatial range
            // GLDAS 2.0/2.1 spatial range 180W ~ 180E, 60S ~ 90N
            if (input.Geometry.Point.Latitude < -60 || input.Geometry.Point.Latitude > 90)
            {
                errors.Add("ERROR: Latitude is not valid. Latitude must be between -60 and 90. Latitude provided: " + input.Geometry.Point.Latitude.ToString());
            }
            if (input.Geometry.Point.Longitude < -180 || input.Geometry.Point.Longitude > 180)
            {
                errors.Add("ERROR: Longitude is not valid. Longitude must be between -180 and 180. Longitude provided: " + input.Geometry.Point.Longitude.ToString());
            }

            if (errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }
    }
}