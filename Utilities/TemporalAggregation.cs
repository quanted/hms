using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class TemporalAggregation
    {

        public static Dictionary<string, List<string>> Aggregation(string aggregation, int timestepHr, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            string errorMsg = "";
            int timesteps = (24 / timestepHr) - 1;
            switch (aggregation)
            {
                case "daily":
                default:
                    return DailyAggregation(out errorMsg, timesteps, 1.0, output, input);
                case "weekly":
                    return WeeklyAggregation(out errorMsg, 1.0, output, input);
                case "monthly":
                    return MonthlyAggregation(out errorMsg, 1.0, output, input);
                case "yearly":
                    return YearlyAggregation(out errorMsg, timesteps, 1.0, output, input);
            }
        }

        public static Dictionary<string, List<string>> DailyAggregation(out string errorMsg, int timesteps, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            if (output.Data.Keys.Last().Contains(" 00"))
            {
                output.Data.Remove(output.Data.Keys.Last());
            }
            TimeSpan t0 = (DateTime.UtcNow - new DateTime(1970, 1, 1));

            timesteps += 1;
            int totalDays = (output.Data.Keys.Count / timesteps) + 1;
            double[][] dValues = new double[totalDays][];
            for (int i = 0; i < totalDays; i++)
            {

                dValues[i] = new double[timesteps];
            }
            int hour = 0;
            int day = 0;
            int i0 = (output.DataSource.Equals("nldas")) ? 1 : 0;
            for (int i = i0; i < output.Data.Keys.Count; i++)
            {
                hour = i % (timesteps);
                day = i / (timesteps);
                dValues[day][hour] = modifier * Double.Parse(output.Data.Values.ElementAt(i - i0)[0]);
            }

            var m = Matrix<double>.Build;
            Matrix<double> matrix = m.DenseOfRowArrays(dValues);
            Vector<double> precipColumnValues = matrix.ColumnSums();
            Vector<double> precipRowValues = matrix.RowSums();
            Dictionary<string, List<string>> tempData0 = new Dictionary<string, List<string>>();
            var tempKeys = output.Data.Keys;
            int j = 0;
            for (int i = 0; i < output.Data.Count; i += timesteps)
            {
                tempData0.Add(tempKeys.ElementAt(i).Replace(" 01", " 00"), new List<string> { (precipRowValues.ElementAt(j)).ToString(input.DataValueFormat) });
                j++;
            }
            TimeSpan t1 = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            return tempData0;

        }

        public static Dictionary<string, List<string>> WeeklyAggregation(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            CultureInfo enUS = new CultureInfo("en-US");

            DateTime iDate = new DateTime();
            double sum = 0.0;

            DateTime.TryParseExact(output.Data.Keys.ElementAt(0), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string> { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime.TryParseExact(output.Data.Keys.ElementAt(i), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out date);
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    values = new List<string> { (modifier * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

        public static Dictionary<string, List<string>> MonthlyAggregation(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            CultureInfo enUS = new CultureInfo("en-US");

            DateTime iDate = new DateTime();
            double sum = 0.0;

            DateTime.TryParseExact(output.Data.Keys.ElementAt(0), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string>() { "" };
            DateTime date = new DateTime();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime.TryParseExact(output.Data.Keys.ElementAt(0), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    values = new List<string> { (modifier * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

        public static Dictionary<string, List<string>> YearlyAggregation(out string errorMsg, int lastHour, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            CultureInfo enUS = new CultureInfo("en-US");

            DateTime iDate = new DateTime();
            double sum = 0.0;

            DateTime.TryParseExact(output.Data.Keys.ElementAt(0), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            List<string> values = new List<string> { "" };
            DateTime date = new DateTime();
            bool last = false;
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime.TryParseExact(output.Data.Keys.ElementAt(0), input.DateTimeSpan.DateTimeFormat, enUS, DateTimeStyles.None, out date);
                last = (date.Month == 12 && date.Day == 31 && date.Hour == lastHour) ? true : false;
                if (last)
                {
                    sum += Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    values = new List<string> { (modifier * sum).ToString(input.DataValueFormat) };
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), values);
                    iDate = date;
                    sum = 0;
                    last = false;
                }
                else
                {
                    sum += Double.Parse(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

    }
}
