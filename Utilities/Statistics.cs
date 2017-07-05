using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Statistics
    {

        /// <summary>
        /// Gets the statistic details for the given data parameter.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="compareIndex">Index of the source to be compared against.</param>
        /// <returns></returns>
        public static ITimeSeriesOutput GetStatistics(out string errorMsg, ITimeSeriesOutput data)
        {
            errorMsg = "";

            // Array of sources in the order they were added to the Data object.
            string[] sources = data.DataSource.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            double[] sums = CalculateSums(data.Data);
            double[] dailyAverage = CalculateDailyAverage(sums, data.Data);
            double[] stdDeviation = CalculateStandardDeviation(dailyAverage, data.Data);
            double[] gore = CalculateGORE(dailyAverage, data.Data);
            double[] goreAvg = CalculateAverageGORE(dailyAverage, data.Data);

            // calculated GORE value
            for (int i = 0; i < dailyAverage.Length; i++)
            {
                data.Metadata.Add(sources[i].Trim() + "_gore", goreAvg[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_average", dailyAverage[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_sum", sums[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_standard_deviation", stdDeviation[i].ToString());
                if (i != 0)
                {
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_gore", gore[i].ToString());
                }
            }

            return data;
        }

        /// <summary>
        /// Calculates sum for all value sources.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateSums(Dictionary<string, List<string>> data)
        {
            // calculate daily values to get sums
            double[] sums = new double[data.Values.ElementAt(0).Count];
            foreach (var e in data)
            {
                for (int i = 0; i < e.Value.Count; i++)
                {
                    sums[i] += Convert.ToDouble(e.Value.ElementAt(i));
                }
            }

            //Parallel.ForEach(data, (KeyValuePair<string, List<string>> e) =>
            //{
            //    for (int i = 0; i < e.Value.Count; i++)
            //    {
            //        sums[i] += Convert.ToDouble(e.Value.ElementAt(i));
            //    }
            //});

            return sums;
        }

        /// <summary>
        /// Calculates daily average for all value sources.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateDailyAverage(double[] sums, Dictionary<string, List<string>> data)
        {
            // calculate daily average
            double[] dailyAverage = new double[data.Values.ElementAt(0).Count];

            //for (int i = 0; i < sums.Length; i++)
            //{
            //    dailyAverage[i] = sums[i] / Convert.ToDouble(data.Values.Count);
            //}

            double count = Convert.ToDouble(data.Values.Count);
            Parallel.For(0, sums.Length, i =>
            {
                dailyAverage[i] = sums[i] / count;
            });

            return dailyAverage;
        }

        /// <summary>
        /// Calculates standard deviation for all sources.
        /// </summary>
        /// <param name="averages"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateStandardDeviation(double[] averages, Dictionary<string, List<string>> data)
        {

            double[] sumDif = new double[data.Values.ElementAt(0).Count];
            //foreach (var el in data)
            //{
            //    for(int i = 0; i < el.Value.Count; i++)
            //    {
            //        // A. Sum of (daily value - daily average) squared
            //        sumDif[i] += Math.Pow(Convert.ToDouble(el.Value[i]) - averages[i], 2.0);
            //    }
            //}

            Parallel.ForEach(data, (KeyValuePair<string, List<string>> el) =>
            {
                for (int i = 0; i < el.Value.Count; i++)
                {
                    // A. Sum of (daily value - daily average) squared
                    sumDif[i] += Math.Pow(Convert.ToDouble(el.Value[i]) - averages[i], 2.0);
                }
            });

            double days = data.Keys.Count;
            double[] stdDev = new double[data.Values.ElementAt(0).Count];

            // Standard Deviation = A / #days 
            //for(int i = 0; i < sumDif.Length; i++)
            //{
            //    stdDev[i] = Math.Sqrt(sumDif[i] / days);
            //}

            Parallel.For(0, sumDif.Length, i =>
            {
                stdDev[i] = Math.Sqrt(sumDif[i] / days);
            });
            
            return stdDev;
        }

        /// <summary>
        /// Caculates GORE value for each value source. Value at index 0 is set as primary.
        /// </summary>
        /// <param name="dailyAverage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateGORE(double[] dailyAverage, Dictionary<string, List<string>> data)
        {

            double[] dailyDif = new double[data.Values.ElementAt(0).Count];
            double dailyAvgDif = 0.0;

            foreach (var el in data)
            {
                for (int i = 1; i < el.Value.Count; i++) { 
                    // Sum of (square root of sourceValue - square root of ncdcValue) squared
                    dailyDif[i] += Math.Pow(Math.Sqrt(Convert.ToDouble(el.Value[i])) - Math.Sqrt(Convert.ToDouble(el.Value[0])), 2.0);
                }
                // Sum of (square root of ncdcValue - square root of ncdc dailyAverage) squared
                dailyAvgDif += Math.Pow(Math.Sqrt(Convert.ToDouble(el.Value[0])) - Math.Sqrt(Convert.ToDouble(dailyAverage[0])), 2.0);                  
            }

            double[] gore = new double[data.Values.ElementAt(0).Count];

            //for(int i = 1; i < gore.Length; i++)
            //{
            //    // Calculate GORE value
            //    gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            //}

            Parallel.For(1, gore.Length, i =>
            {
                gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            });

            return gore;
        }

        /// <summary>
        /// Caculates average GORE value for each value source. Primary source is the average of all sources.
        /// </summary>
        /// <param name="dailyAverage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateAverageGORE(double[] dailyAverage, Dictionary<string, List<string>> data)
        {

            double[] dailyDif = new double[data.Values.ElementAt(0).Count];
            double dailyAvgDif = 0.0;

            // calculate average daily of all sources
            double dailyAvgTot = 0.0;

            for (int i = 0; i < dailyAverage.Length; i++)
            {
                dailyAvgTot += dailyAverage[i];
            }
            //Parallel.For(0, dailyAverage.Length, i =>
            //{
            //    dailyAvgTot += dailyAverage[i];
            //});

            dailyAvgTot = dailyAvgTot / Convert.ToDouble(dailyAverage.Length);

            //foreach (var el in data)
            //{
            Parallel.ForEach(data, (KeyValuePair<string, List<string>> el) => 
                {
                double dailySum = 0.0;
                for (int i = 0; i < el.Value.Count; i++)
                {
                    dailySum += Convert.ToDouble(el.Value[i]);
                }
                double dailySumAverage = dailySum / Convert.ToDouble(el.Value.Count);

                for (int i = 0; i < el.Value.Count; i++)
                {
                    // Sum of (square root of sourceValue - square root of ncdcValue) squared
                    dailyDif[i] += Math.Pow(Math.Sqrt(Convert.ToDouble(el.Value[i])) - Math.Sqrt(dailySumAverage), 2.0);
                }
                // Sum of (square root of ncdcValue - square root of ncdc dailyAverage) squared
                dailyAvgDif += Math.Pow(Math.Sqrt(dailySumAverage) - Math.Sqrt(Convert.ToDouble(dailyAvgTot)), 2.0);
                //}
            });

            double[] gore = new double[data.Values.ElementAt(0).Count];
            //for (int i = 0; i < gore.Length; i++)
            //{
            //    // Calculate GORE value
            //    gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            //}

            Parallel.For(0, gore.Length, i =>
            {
                //    // Calculate GORE value
                gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            });

            return gore;
        }
    }
}
