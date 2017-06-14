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

            // Array to hold the total values associated with the sources in the sources[]
            double[] totals = new double[data.Data[data.Data.Keys.ElementAt(0)].Count];

            // Array of sources in the order they were added to the Data object.
            string[] sources = data.DataSource.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            // Number of days of Data
            double nDays = Convert.ToDouble(data.Data.Keys.Count);

            Dictionary<string, double> dailyMeans = new Dictionary<string, double>();

            // Calculate totals and dailyMeans
            foreach(string key in data.Data.Keys)
            {
                double sum = 0.0;
                for(int i = 0; i < data.Data[key].Count; i++)
                {
                    totals[i] += Convert.ToDouble(data.Data[key][i]);
                    sum += Convert.ToDouble(data.Data[key][i]);
                }
                dailyMeans.Add(key, sum / Convert.ToDouble(data.Data[key].Count));
            }

            // Calculate average
            double[] average = new double[totals.Length];
            double averageSum = 0.0;
            double[] averageIndex = new double[totals.Length];
            for(int i = 0; i < average.Length; i++)
            {
                average[i] = totals[i] / nDays;
                averageSum += average[i];

                if (i != 0)
                {
                    averageIndex[i] += (totals[0] + totals[i])/ nDays;
                }
            }

            // Calculate total average
            double totalMean = averageSum / Convert.ToDouble(average.Length);
            double[] totalMeanIndexed = new double[average.Length];
            for(int i = 0; i < average.Length-1; i++)
            {
                totalMeanIndexed[i] = averageIndex[i] / 2;
            }

            // residual sum of squares
            double[] resSS = new double[average.Length];
            double[] resSSIndexed = new double[average.Length];
            double totSS = 0.0;
            double totSSIndexed = 0.0;
            
            // Standard Deviation Difference
            double[] stdDevD = new double[average.Length];
                        
            foreach(string key in data.Data.Keys)
            {
                // daily value
                double[] dailyValue = new double[average.Length];

                double dailyMean = 0.0;
                double[] dailyMeanIndexed = new double[average.Length];
                for(int i = 0; i < data.Data[key].Count; i++)
                {
                    dailyValue[i] = Convert.ToDouble(data.Data[key][i]);
                    dailyMean += Convert.ToDouble(data.Data[key][i]);
                    stdDevD[i] += Math.Pow(Convert.ToDouble(data.Data[key][i]) - average[i], 2.0);

                    if(i != 0)
                    {
                        dailyMeanIndexed[i] = Convert.ToDouble(data.Data[key][i]) + Convert.ToDouble(data.Data[key][0]) / 2.0;
                    }
                }
                // daily mean
                dailyMean = dailyMean / Convert.ToDouble(average.Length);

                // residual sum of squares calculated for each source
                for (int i = 0; i < data.Data[key].Count; i++)
                {
                    resSS[i] += Math.Pow(Math.Sqrt(dailyValue[i]) - Math.Sqrt(dailyMean), 2.0);
                    
                    if(i != 0)
                    {
                        resSSIndexed[i] += Math.Pow(Math.Sqrt(dailyValue[i]) - Math.Sqrt(Convert.ToDouble(data.Data[key][0])), 2.0);
                    }
                }
                totSSIndexed += Math.Pow(Math.Sqrt(Convert.ToDouble(data.Data[key][0])) - Math.Sqrt(average[0]), 2.0);

                // total sum of squares
                totSS += Math.Pow(Math.Sqrt(dailyMean) - Math.Sqrt(totalMean), 2.0);  
            }

            // calculated GORE value
            double[] goreValue = new double[average.Length];
            for (int i = 0; i < average.Length; i++)
            {
                goreValue[i] = 1 - (resSS[i] / totSS);
                data.Metadata.Add(sources[i].Trim() + "_gore", goreValue[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_average", average[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_standard_deviation", Math.Sqrt(stdDevD[i] / nDays).ToString());
                if (i != 0)
                {
                    double compareGoreValue = 1 - (resSSIndexed[i] / totSSIndexed);
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_gore", compareGoreValue.ToString());
                }
            }

            return data;
        }
    }
}
