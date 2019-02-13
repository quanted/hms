using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Utilities;

namespace Precipitation
{
    /// <summary>
    /// Precipitation NLDAS class
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
            string data = nldas.GetData(out errorMsg, "PRECIP", input);
            //if (errorMsg.Contains("ERROR")) { return null; }
            //if (data.Contains("ERROR"))
            //{
            //    string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //    errorMsg = lines[0] + " Dataset: precipitation, Source: " + input.Source;
            //    return null;
            //}
            
            ITimeSeriesOutput nldasOutput = output;
            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                output = err.ReturnError("Precipitation", "nldas", errorMsg);
                errorMsg = "";
                return output;
            }
            else
            {
                nldasOutput = nldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            }
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
                    output.Data = DailyAggregatedSum(out errorMsg, 23, 1.0, output, input);
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
                case "weekly":
                    output.Data = WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                case "yearly":
                    output.Data = YearlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Yearly Total");
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
            Dictionary<string, List<string>> tempData = output.Data;

            foreach (KeyValuePair<string, List<string>> kv in tempData)
            {
                tempData[kv.Key][0] = (modifier * unit * Convert.ToDouble(kv.Value[0])).ToString(input.DataValueFormat);
            }
            return tempData;
        }

        /// <summary>
        /// Daily aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyAggregatedSum(out string errorMsg, int hours, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            ITimeSeriesAggregation aggOut = ConvertTimeSeries(output);

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }


            iDate = aggOut.Data.Keys.ElementAt(0).Date;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            int nHourly = 0;
            List<string> values = new List<string> { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < aggOut.Data.Count; i++)
            {
                date = aggOut.Data.Keys.ElementAt(i).Date;
                if (date.Day != iDate.Day || (nHourly >= hours && i > nHourly))
                {
                    values = new List<string> { (modifier * unit * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0];
                    nHourly = 0;
                }
                else
                {
                    sum += aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0];
                    nHourly++;
                }
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
            ITimeSeriesAggregation aggOut = ConvertTimeSeries(output);

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            iDate = aggOut.Data.Keys.ElementAt(0).Date;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string> { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < aggOut.Data.Count; i++)
            {
                date = aggOut.Data.Keys.ElementAt(i).Date;
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    values = new List<string> { (modifier * unit * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    if (input.Source == "gldas")
                    {
                        iDate = iDate.AddHours(-iDate.Hour);//Fixes issue with GLDAS keys being not compatible with other time series in precip compare
                    }
                    sum = aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
                }
                else
                {
                    sum += aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
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
            ITimeSeriesAggregation aggOut = ConvertTimeSeries(output);


            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            iDate = aggOut.Data.Keys.ElementAt(0).Date;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string>() { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < aggOut.Data.Count; i++)
            {
                date = aggOut.Data.Keys.ElementAt(i).Date;
                if (date.Month != iDate.Month)
                {
                    values = new List<string> { (modifier * unit * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
                }
                else
                {
                    sum += aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
                }
            }
            return tempData;
        }

        /// <summary>
        /// Yearly aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> YearlyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            ITimeSeriesAggregation aggOut = ConvertTimeSeries(output);

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            iDate = aggOut.Data.Keys.ElementAt(0).Date;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string> { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < aggOut.Data.Count; i++)
            {
                date = aggOut.Data.Keys.ElementAt(i).Date;
                if (date.Year != iDate.Year)
                {
                    values = new List<string> { (modifier * unit * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
                }
                else
                {
                    sum += aggOut.Data[aggOut.Data.Keys.ElementAt(i)][0]; 
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
            return Data.Source.NLDAS.CheckStatus("Precipitation", input);
        }



        public static ITimeSeriesAggregation ConvertTimeSeries(ITimeSeriesOutput output)
        {
            //Create new ITimeSeries and convert Dictionary<string, List<string>> to Dictionary<DateTime, List<double>>
            ITimeSeriesAggregationFactory aFactory = new TimeSeriesAggregationFactory();
            ITimeSeriesAggregation aggOutput = aFactory.Initialize();
            foreach (KeyValuePair<string, List<string>> kv in output.Data)
            {
                DateTime iDate = new DateTime();
                iDate = DateTime.ParseExact(kv.Key, "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
                aggOutput.Data.Add(iDate, kv.Value.ConvertAll(x => double.Parse(x)));
            }
            aggOutput.Dataset = output.Dataset;
            aggOutput.DataSource = output.DataSource;
            aggOutput.Metadata = output.Metadata;

            return aggOutput;
        }
    }
}