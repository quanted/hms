using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Precipitation
{
    /// <summary>
    /// Base precipitation wgen class.
    /// </summary>
    public class WGEN
    {
        /// <summary>
        /// Initiates the wgen precip generator.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Simulate.WGEN wgen = new Data.Simulate.WGEN();

            // The number of years of historic data can be customized by includead 'yearsHistoric' in Geometry.GeometryMetadata
            int years = (input.Geometry.GeometryMetadata.ContainsKey("yearsHistoric")) ? Convert.ToInt16(input.Geometry.GeometryMetadata["yearsHistoric"]) : 20;
            // Historic Precipitation data.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput historicInput = iFactory.SetTimeSeriesInput(input, out errorMsg);
            ITimeSeriesOutput historicData = GetHistoricData(out errorMsg, years, historicInput, output);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Run wgen
            ITimeSeriesOutput outputData = output;
            outputData = wgen.Simulate(out errorMsg, years, input, output, historicData);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Unit conversion and temporal aggregation
            outputData = TemporalAggregation(out errorMsg, outputData, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return outputData;
        }

        /// <summary>
        /// Gets the historic precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="years">Years of historic data.</param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput GetHistoricData(out string errorMsg, int years, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";

            Precipitation precip = new Precipitation();
            precip.Input = input;
            precip.Output = output;

            // Historic end date set to simulated data start date minus one day.
            precip.Input.DateTimeSpan.EndDate = precip.Input.DateTimeSpan.StartDate.AddDays(-1);
            // Historic start date set to 20 years before simulated start date.
            precip.Input.DateTimeSpan.StartDate = precip.Input.DateTimeSpan.StartDate.AddYears(-1 * Math.Abs(years));

            if (input.Geometry.GeometryMetadata.ContainsKey("historicSource"))
            {
                switch (input.Geometry.GeometryMetadata["historicSource"])
                {
                    case "nldas":
                        precip.Input.Source = "nldas";
                        precip.Input.TemporalResolution = "daily";
                        precip.Output = precip.GetData(out errorMsg);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        return precip.Output; ;
                    case "gldas":
                        precip.Input.Source = "gldas";
                        precip.Input.TemporalResolution = "daily";
                        precip.Output = precip.GetData(out errorMsg);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        return precip.Output;
                    case "daymet":
                    default:
                        precip.Input.Source = "daymet";
                        precip.Output = precip.GetData(out errorMsg);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        return precip.Output; ;
                }
            }
            else
            {
                precip.Input.Source = "daymet";
                precip.Output = precip.GetData(out errorMsg);
                if (errorMsg.Contains("ERROR")) { return null; }
                return precip.Output;
            }
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
            output.Metadata.Add("wgen_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("wgen_column_1", "Date");

            if (!input.Units.Contains("imperial")) { output.Metadata["wgen_unit"] = "mm"; }

            switch (input.TemporalResolution)
            {
                case "weekly":
                    output.Data = WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("wgen_column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("wgen_column_2", "Monthly Total");
                    return output;
                default:
                    output.Data = (!input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, 1.0, output, input) : output.Data;
                    output.Metadata.Add("wgen_column_2", "Daily Total");
                    return output;
            }
        }

        /// <summary>
        /// Converts wgen inches to mm.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> UnitConversion(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Unit conversion coefficient
            double unit = 25.4;
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
        /// Weekly aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> WeeklyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (!input.Units.Contains("imperial")) ? 25.4 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length) + ":00:00";
                DateTime.TryParse(dateString, out date);
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
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
        /// Monthly aggregated sums for precipitation data.
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
            double unit = (!input.Units.Contains("imperial")) ? 25.4 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month)
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
    }
}
