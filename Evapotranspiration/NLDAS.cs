﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evapotranspiration
{
    /// <summary>
    /// Evapotranspiration NLDAS class
    /// </summary>
    public class NLDAS
    {
        /// <summary>
        /// Makes the GetData call to the base NLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "Evapotrans", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput nldasOutput = output;
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "Evapotranspiration", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            nldasOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return nldasOutput;
        }

        /// <summary>
        /// Checks temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("nldas_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");

            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }
            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = DailyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, 1.0, output, input) : output.Data;
                    output.Metadata.Add("column_2", "Hourly Total");
                    return output;
            }
        }

        /// <summary>
        /// Converts metric ldas kg/m**2 (mm) units to imperial inches units.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> UnitConversion(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Unit conversion coefficient
            double unit = 0.0393701;
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                    ( modifier * unit * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0])).ToString(input.DataValueFormat)
                });
            }
            return tempData;
        }

        /// <summary>
        /// Daily aggregated sums for evapotranspiration data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Day != iDate.Day)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
                    iDate = date;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

        /// <summary>
        /// Weekly aggregated sums for evapotranspiration data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        //public static Dictionary<string, List<string>> WeeklyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        //{
        //    errorMsg = "";

        //    DateTime iDate = new DateTime();
        //    double sum = 0.0;

        //    // Unit conversion coefficient
        //    double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

        //    string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
        //    DateTime.TryParse(dateString0, out iDate);
        //    Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
        //    for (int i = 0; i < output.Data.Count; i++)
        //    {
        //        DateTime date = new DateTime();
        //        string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length) + ":00:00";
        //        DateTime.TryParse(dateString, out date);
        //        int dayDif = (int)(date - iDate).TotalDays;
        //        if (dayDif >= 7)
        //        {
        //            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
        //            iDate = date;
        //            sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
        //        }
        //        else
        //        {
        //            sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
        //        }
        //    }
        //    return tempData;
        //}

        /// <summary>
        /// Monthly aggregated sums for evapotranspiration data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
                    iDate = date;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

        /// <summary>
        /// Calls the function in Data.Source.NLDAS that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.NLDAS.CheckStatus("Evapotranspiration", input);
        }
    }
}
