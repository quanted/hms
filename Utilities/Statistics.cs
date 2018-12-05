using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;

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
            int notSkipped = 0;

            Matrix<double> matrix = BuildMatrix(data.Data, true);
            Vector<double> mSum = matrix.ColumnSums();
            double[] mMean = new double[matrix.ColumnCount];
            double[] mMax = new double[matrix.ColumnCount];
            double[] mSTD = new double[matrix.ColumnCount];
            double[] mVar = new double[matrix.ColumnCount];
            double[][] mPearson = new double[matrix.ColumnCount][];
            double[] mCovariance = new double[matrix.ColumnCount];
            double[] mEntropy = new double[matrix.ColumnCount];
            double[] mGeoMean = new double[matrix.ColumnCount];
            double[] mSkewness = new double[matrix.ColumnCount];
            double[] mRMS = new double[matrix.ColumnCount];
            double[] mR2 = new double[matrix.ColumnCount];
            double[] m95 = new double[matrix.ColumnCount];
            double[] m95Count = new double[matrix.ColumnCount];
            double[] m75 = new double[matrix.ColumnCount];
            double[] m75Count = new double[matrix.ColumnCount];

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                DescriptiveStatistics desc = new DescriptiveStatistics(matrix.Column(i));
                mMean[i] = desc.Mean;
                mMax[i] = desc.Maximum;
                mSTD[i] = desc.StandardDeviation;
                mVar[i] = desc.Variance;
                if(i == 0)
                {
                    mCovariance[i] = 0.0;
                    mR2[i] = 0.0;
                }
                else
                {
                    mCovariance[i] = MathNet.Numerics.Statistics.Statistics.Covariance(matrix.Column(0), matrix.Column(i));
                    mR2[i] = MathNet.Numerics.GoodnessOfFit.RSquared(matrix.Column(0), matrix.Column(i));
                }
                mEntropy[i] = MathNet.Numerics.Statistics.Statistics.Entropy(matrix.Column(i));
                mGeoMean[i] = MathNet.Numerics.Statistics.Statistics.GeometricMean(matrix.Column(i));
                mSkewness[i] = MathNet.Numerics.Statistics.Statistics.Skewness(matrix.Column(i));
                mRMS[i] = MathNet.Numerics.Statistics.Statistics.RootMeanSquare(matrix.Column(i));
                m95[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 95);
                m75[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 75);

                Func<double, bool> isAbove95 = (v) => (v >= m95[i]);
                Func<double, bool> isAbove75 = (v) => (v >= m75[i]);
                m95Count[i] = matrix.Column(i).Where(isAbove95).Count();
                m75Count[i] = matrix.Column(i).Where(isAbove75).Count();

                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if(j == 0)
                    {
                        mPearson[i] = new double[matrix.ColumnCount];
                    }
                    if (i == j)
                    {
                        mPearson[i][j] = 0.0;
                    }
                    else
                    {
                        mPearson[i][j] = MathNet.Numerics.Statistics.Correlation.Pearson(matrix.Column(i), matrix.Column(j));
                    }
                }
            }

            // Array of sources in the order they were added to the Data object.
            string[] sources = data.DataSource.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            double[] gore = CalculateGORE(mMean, data.Data);

            //double[] sums = CalculateSums(data.Data, out notSkipped);

            //double[] dailyAverage = CalculateDailyAverage(sums, data.Data, notSkipped);
            //double[] stdDeviation = CalculateStandardDeviation(dailyAverage, data.Data, notSkipped);
            //double[] goreAvg = CalculateAverageGORE(dailyAverage, data.Data);
            //double[] rSquared = CalculateDetermination(data.Data);

            // calculated GORE value
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                //data.Metadata.Add(sources[i].Trim() + "_gore", goreAvg[i].ToString());
                //data.Metadata.Add(sources[i].Trim() + "_average", dailyAverage[i].ToString());
                //data.Metadata.Add(sources[i].Trim() + "_sum", sums[i].ToString());
                //data.Metadata.Add(sources[i].Trim() + "_standard_deviation", stdDeviation[i].ToString());
                if (i != 0)
                {
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_gore", gore[i].ToString());
                    //    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_R-Squared", rSquared[i].ToString());
                }

                // MathNet.Numerics
                data.Metadata.Add(sources[i].Trim() + "_sum", mSum[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_mean", mMean[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_max", mMax[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_standard_deviation", mSTD[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_variance", mVar[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_entropy", mEntropy[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_geometric_mean", mGeoMean[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_skewness", mSkewness[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_root_mean_square", mRMS[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_95_percentile", m95[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_95_percentile_count", m95Count[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_75_percentile", m75[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_75_percentile_count", m75Count[i].ToString());
                if (i != 0)
                {
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_covariance", mCovariance[i].ToString());
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_r_squared", mR2[i].ToString());
                }
                for(int j = 0; j < matrix.ColumnCount; j++)
                {
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[j].Trim() + "_pearson_cofficient", mPearson[i][j].ToString());

                }
            }

            return data;
        }

        /// <summary>
        /// Build a MathNet.Numerics Matrix from timeseries from multiple sources
        /// </summary>
        /// <param name="data"></param>
        /// <param name="excludeAcrossSources">Exclude data if missing from any source</param>
        /// <returns></returns>
        private static Matrix<double> BuildMatrix(Dictionary<string, List<string>> data, bool excludeAcrossSources)
        {
            // calculate daily values to get sums
            if (data == null || data.Count < 1)
                return null;

            int rows = data.Count;
            int cols = data.First().Value.Count;            

            List<List<double>> lstTable = new List<List<double>>();
            
            foreach (KeyValuePair<string, List<string>> timeseries in data)
            {
                bool missingData = false;
                List<double> row = new List<double>();
                for (int i = 0; i < cols; i++)
                {
                    double dval;
                    if (Double.TryParse(timeseries.Value.ElementAt(i), out dval))
                        row.Add(dval);                        
                    else
                    {
                        missingData = true;
                        row.Add(-9999);
                    }
                }

                //If there are missing data and we want to exclude missing data
                if (missingData && excludeAcrossSources)
                    continue;
                else
                    lstTable.Add(row);               
            }

            double[][] arrayTable = new double[rows][];
            for (int i=0; i<rows; i++)            
                arrayTable[i] = lstTable[i].ToArray();
            

            var M = Matrix<double>.Build;
            var matrix = M.DenseOfRowArrays(arrayTable);
            return matrix;
        }

        /// <summary>
        /// Calculates sum for all value sources.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateSums(Dictionary<string, List<string>> data, out int notSkipped)
        {
            int p = 4; // Double decimal places
            notSkipped = 0; // Track amount of valid data entries for average

            // calculate daily values to get sums
            double[] sums = new double[data.Values.ElementAt(0).Count];
            foreach (var e in data)
            {
                for (int i = 0; i < e.Value.Count; i++)
                {
                    if (Convert.ToDouble(e.Value.ElementAt(i)) > -9999)
                    {
                        if (i == 0)
                        {
                            notSkipped += 1;
                        }
                        sums[i] += Math.Round(Convert.ToDouble(e.Value.ElementAt(i)), p);
                    }
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
        private static double[] CalculateDailyAverage(double[] sums, Dictionary<string, List<string>> data, int notSkipped)
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
            dailyAverage[0] = sums[0] / notSkipped;

            return dailyAverage;
        }

        /// <summary>
        /// Calculates standard deviation for all sources.
        /// </summary>
        /// <param name="averages"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateStandardDeviation(double[] averages, Dictionary<string, List<string>> data, int notSkipped)
        {

            int p = 4; // Double decimal places
            double[] sumDif = new double[data.Values.ElementAt(0).Count];
            foreach (var el in data)
            {
                for (int i = 0; i < el.Value.Count; i++)
                {
                    // A. Sum of (daily value - daily average) squared
                    if(Convert.ToDouble(el.Value[i]) > -9999)
                    {
                        sumDif[i] += Math.Pow(Math.Round(Convert.ToDouble(el.Value[i]), p) - averages[i], 2.0);
                    }
                }
            }

            //Parallel.ForEach(data, (KeyValuePair<string, List<string>> el) =>
            //{
            //    for (int i = 0; i < el.Value.Count; i++)
            //    {
            //        // A. Sum of (daily value - daily average) squared
            //        sumDif[i] += Math.Pow(Math.Round(Convert.ToDouble(el.Value[i]), p) - averages[i], 2.0);
            //    }
            //});

            double days = data.Keys.Count;
            double[] stdDev = new double[data.Values.ElementAt(0).Count];

            // Standard Deviation = A / #days 
            for (int i = 0; i < sumDif.Length; i++)
            {
                stdDev[i] = Math.Round(Math.Sqrt(sumDif[i] / days), p);
            }
            stdDev[0] = Math.Round(Math.Sqrt(sumDif[0] / notSkipped), p);

            //Parallel.For(0, sumDif.Length, i =>
            //{
            //    stdDev[i] = Math.Round(Math.Sqrt(sumDif[i] / days), p);
            //});

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

            for (int i = 1; i < gore.Length; i++)
            {
                // Calculate GORE value
                gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            }

            //Parallel.For(1, gore.Length, i =>
            //{
            //    gore[i] = 1.0 - (dailyDif[i] / dailyAvgDif);
            //});

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

        /// <summary>
        /// Calculates the coefficient of determination (R-Squared), where the first element is the actual and remaining elemnts are expected values.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static double[] CalculateDetermination(Dictionary<string, List<string>> data)
        {
            double[] r2 = new double[data.Values.ElementAt(0).Count];
            double[][] datasets = new double[data.Values.ElementAt(0).Count][];

            // Array initialization
            for(int i = 0; i < data.Values.ElementAt(0).Count; i++)
            {
                datasets[i] = new double[data.Count];
            }

            // Assigns each dataset from the Output dictionary to a a double[each dataset][each date value]
            for(int i = 0; i < data.Count; i++)
            {
                List<string> d = data.ElementAt(i).Value;
                for(int j = 0; j < d.Count; j++)
                {
                   datasets[j][i] = Double.Parse(d.ElementAt(j));
                }
            }

            // Calculates coefficient of determination using dataset array [0] against each other dataset and assigning resulting value to r2
            r2[0] = 0.0;
            for(int i = 1; i < r2.Length; i++)
            {
                //r2[i] = Accord.Statistics.Tools.Determination(datasets[0], datasets[i]);
                r2[i] = MathNet.Numerics.GoodnessOfFit.CoefficientOfDetermination(datasets[0], datasets[i]);
            }

            return r2;
        }
    }
}
