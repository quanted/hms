using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace Utilities
{
    public class Statistics
    {

        /// <summary>
        /// Generic statistics calculation, dataset/source independent
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput GetStatistics(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput data)
        {
            errorMsg = "";
            int missingDays = 0;

            Matrix<double> matrix = BuildMatrix(data.Data, true, out missingDays);
            Vector<double> mSum = matrix.ColumnSums();
            double[] mMean = new double[matrix.ColumnCount];
            double[] mMax = new double[matrix.ColumnCount];
            double[] mSTD = new double[matrix.ColumnCount];
            double[] mVar = new double[matrix.ColumnCount];
            double[] mMedian = new double[matrix.ColumnCount];
            double[] mEntropy = new double[matrix.ColumnCount];
            double[] mGeoMean = new double[matrix.ColumnCount];
            double[] mSkewness = new double[matrix.ColumnCount];
            double[] mRMS = new double[matrix.ColumnCount];
            double[] m99 = new double[matrix.ColumnCount];
            double[] m99Count = new double[matrix.ColumnCount];
            double[] m95 = new double[matrix.ColumnCount];
            double[] m95Count = new double[matrix.ColumnCount];
            double[] m75 = new double[matrix.ColumnCount];
            double[] m75Count = new double[matrix.ColumnCount];
            double[] mZeroCount = new double[matrix.ColumnCount];

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                DescriptiveStatistics desc = new DescriptiveStatistics(matrix.Column(i));
                mMean[i] = desc.Mean;
                mMax[i] = desc.Maximum;
                mSTD[i] = desc.StandardDeviation;
                mVar[i] = desc.Variance;
                mMedian[i] = matrix.Column(i).Median();
                mEntropy[i] = MathNet.Numerics.Statistics.Statistics.Entropy(matrix.Column(i));
                mGeoMean[i] = MathNet.Numerics.Statistics.Statistics.GeometricMean(matrix.Column(i));
                mSkewness[i] = MathNet.Numerics.Statistics.Statistics.Skewness(matrix.Column(i));
                mRMS[i] = MathNet.Numerics.Statistics.Statistics.RootMeanSquare(matrix.Column(i));
                m99[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 99);
                m95[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 95);
                m75[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 75);

                Func<double, bool> isAbove99 = (v) => (v >= m99[i]);
                Func<double, bool> isAbove95 = (v) => (v >= m95[i]);
                Func<double, bool> isAbove75 = (v) => (v >= m75[i]);
                Func<double, bool> isZero = (v) => (v == 0);

                m99Count[i] = matrix.Column(i).Where(isAbove99).Count();
                m95Count[i] = matrix.Column(i).Where(isAbove95).Count();
                m75Count[i] = matrix.Column(i).Where(isAbove75).Count();
                mZeroCount[i] = matrix.Column(i).Where(isZero).Count();
            }

            List<string> columns = new List<string>();
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                if(data.Metadata.Keys.Contains("column_" + (i + 2)))
                {
                    columns.Add(data.Metadata["column_" + (i + 2)] + "_");
                }
                else if (columns.Count != i + 1 && matrix.ColumnCount > 1)
                {
                    columns.Add("column_" + (i + 2) + "_");
                }
                //else if (matrix.ColumnCount == 1)
                //{
                //    columns.Add("");
                //}
            }

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                // MathNet.Numerics
                data.Metadata.Add(columns[i].Trim() + "sum", mSum[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "mean", mMean[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "max", mMax[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "standard_deviation", mSTD[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "variance", mVar[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "median", mMedian[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "entropy", mEntropy[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "geometric_mean", mGeoMean[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "skewness", mSkewness[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "root_mean_square", mRMS[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "99_percentile", m99[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "99_percentile_count", m99Count[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "95_percentile", m95[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "95_percentile_count", m95Count[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "75_percentile", m75[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "75_percentile_count", m75Count[i].ToString());
                data.Metadata.Add(columns[i].Trim() + "zero_count", mZeroCount[i].ToString());
            }
            return data;
        }

        /// <summary>
        /// Comparison workflow statistics calculations
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput GetCompareStatistics(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput data)
        {
            errorMsg = "";
            int missingDays = 0;

            // Array of sources in the order they were added to the Data object.
            string[] sources = data.DataSource.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            Matrix<double> matrix = BuildMatrix(data.Data, true, out missingDays);
            Vector<double> mSum = matrix.ColumnSums();
            double[] mMean = new double[matrix.ColumnCount];
            double[] mMax = new double[matrix.ColumnCount];
            double[] mSTD = new double[matrix.ColumnCount];
            double[] mVar = new double[matrix.ColumnCount];
            double[][] mPearson = new double[matrix.ColumnCount][];
            double[] mCovariance = new double[matrix.ColumnCount];
            double[] mMedian = new double[matrix.ColumnCount];
            double[] mEntropy = new double[matrix.ColumnCount];
            double[] mGeoMean = new double[matrix.ColumnCount];
            double[] mSkewness = new double[matrix.ColumnCount];
            double[] mRMS = new double[matrix.ColumnCount];
            double[] mR2 = new double[matrix.ColumnCount];
            double[] m99 = new double[matrix.ColumnCount];
            double[] m99Count = new double[matrix.ColumnCount];
            double[] m95 = new double[matrix.ColumnCount];
            double[] m95Count = new double[matrix.ColumnCount];
            double[] m75 = new double[matrix.ColumnCount];
            double[] m75Count = new double[matrix.ColumnCount];
            double[] mZeroCount = new double[matrix.ColumnCount];

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                DescriptiveStatistics desc = new DescriptiveStatistics(matrix.Column(i));
                mMean[i] = desc.Mean;
                mMax[i] = desc.Maximum;
                mSTD[i] = desc.StandardDeviation;
                mVar[i] = desc.Variance;
                if (i == 0)
                {
                    mCovariance[i] = 0.0;
                    mR2[i] = 0.0;
                }
                else
                {
                    mCovariance[i] = MathNet.Numerics.Statistics.Statistics.Covariance(matrix.Column(0), matrix.Column(i));
                    mR2[i] = MathNet.Numerics.GoodnessOfFit.RSquared(matrix.Column(0), matrix.Column(i));
                }
                mMedian[i] = matrix.Column(i).Median();
                mEntropy[i] = MathNet.Numerics.Statistics.Statistics.Entropy(matrix.Column(i));
                mGeoMean[i] = MathNet.Numerics.Statistics.Statistics.GeometricMean(matrix.Column(i));
                mSkewness[i] = MathNet.Numerics.Statistics.Statistics.Skewness(matrix.Column(i));
                mRMS[i] = MathNet.Numerics.Statistics.Statistics.RootMeanSquare(matrix.Column(i));
                m99[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 99);
                m95[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 95);
                m75[i] = MathNet.Numerics.Statistics.Statistics.Percentile(matrix.Column(i), 75);

                Func<double, bool> isAbove99 = (v) => (v >= m99[i]);
                Func<double, bool> isAbove95 = (v) => (v >= m95[i]);
                Func<double, bool> isAbove75 = (v) => (v >= m75[i]);
                Func<double, bool> isZero = (v) => (v == 0);

                m99Count[i] = matrix.Column(i).Where(isAbove99).Count();
                m95Count[i] = matrix.Column(i).Where(isAbove95).Count();
                m75Count[i] = matrix.Column(i).Where(isAbove75).Count();
                mZeroCount[i] = matrix.Column(i).Where(isZero).Count();

                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (j == 0)
                    {
                        mPearson[i] = new double[matrix.ColumnCount];
                    }
                    if (i == j)
                    {
                        mPearson[i][j] = 1.0;
                    }
                    else
                    {
                        int test1 = matrix.Column(i).Count;
                        int test2 = matrix.Column(j).Count;
                        mPearson[i][j] = MathNet.Numerics.Statistics.Correlation.Pearson(matrix.Column(i), matrix.Column(j));
                    }
                }
            }

            double[] datasetCompare = new double[0];

            // DatasetCompare set to Gore for precipitation
            if (data.Dataset == "Precipitation")
            {
                datasetCompare = CalculateGORE(mMean, data.Data);
            }

            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                // MathNet.Numerics
                data.Metadata.Add(sources[i].Trim() + "_sum", mSum[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_mean", mMean[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_max", mMax[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_standard_deviation", mSTD[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_variance", mVar[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_median", mMedian[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_entropy", mEntropy[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_geometric_mean", mGeoMean[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_skewness", mSkewness[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_root_mean_square", mRMS[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_99_percentile", m99[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_99_percentile_count", m99Count[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_95_percentile", m95[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_95_percentile_count", m95Count[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_75_percentile", m75[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_75_percentile_count", m75Count[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_zero_count", mZeroCount[i].ToString());
                data.Metadata.Add(sources[i].Trim() + "_missing_days", missingDays.ToString());
                if (i != 0)
                {
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_covariance", mCovariance[i].ToString());
                    data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_r_squared", mR2[i].ToString());
                }
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    string key = sources[i].Trim() + "_" + sources[j].Trim() + "_pearson_coefficient";
                    if (!data.Metadata.Keys.Contains(key))
                    {
                        data.Metadata.Add(key, mPearson[i][j].ToString());
                    }
                }

                if (data.Dataset == "Precipitation")
                {
           
                    // Precipitation Comparison Specific Stats
                    double[] pDryDays = new double[matrix.ColumnCount];
                    double[] pWetDays = new double[matrix.ColumnCount];
                    double[] pHeavyDays = new double[matrix.ColumnCount];

                    // dry, wet, heavy values are only used on precipitation, the cutoff values can be customized by setting the following keys in data.Metadata ["dryDay", "wetDay", "heavyDay"]
                    // TODO: Add documentation to input request and if precip copy over keys corresponding to these values to the output metadata
                    double dryValue = (data.Metadata.ContainsKey("dryDay")) ? Double.Parse(data.Metadata["dryDay"]) : 1.0;
                    double wetValue = (data.Metadata.ContainsKey("wetDay")) ? Double.Parse(data.Metadata["wetDay"]) : 1.0;
                    double heavyValue = (data.Metadata.ContainsKey("heavyDay")) ? Double.Parse(data.Metadata["heavyDay"]) : 10.0;

                    Func<double, bool> isDryDay = (v) => (v < dryValue);
                    Func<double, bool> isWetDay = (v) => (v >= wetValue);
                    Func<double, bool> isHeavyDay = (v) => (v >= heavyValue);
                    pDryDays[i] = matrix.Column(i).Where(isDryDay).Count();
                    pWetDays[i] = matrix.Column(i).Where(isWetDay).Count();
                    pHeavyDays[i] = matrix.Column(i).Where(isHeavyDay).Count();

                    if (i != 0)
                    {
                        data.Metadata.Add(sources[i].Trim() + "_" + sources[0].Trim() + "_gore", datasetCompare[i].ToString());
                    }

                    data.Metadata.Add(sources[i].Trim() + "_dry_days", pDryDays[i].ToString());
                    data.Metadata.Add(sources[i].Trim() + "_wet_days", pWetDays[i].ToString());
                    data.Metadata.Add(sources[i].Trim() + "_heavy_days", pHeavyDays[i].ToString());
                }

            }

            data.Metadata.Add("missing_days", missingDays.ToString());

            if (input.TemporalResolution == "extreme_5")
            {
                SumExtremeValues(out errorMsg, input, ref data);
            }

            return data;
        }

        /// <summary>
        /// Build a MathNet.Numerics Matrix from timeseries from multiple sources
        /// </summary>
        /// <param name="data"></param>
        /// <param name="excludeAcrossSources">Exclude data if missing from any source</param>
        /// <returns></returns>
        private static Matrix<double> BuildMatrix(Dictionary<string, List<string>> data, bool excludeAcrossSources, out int missingDays)
        {
            missingDays = 0;

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
                if (timeseries.Key.Contains("Total"))
                {
                    break;
                }
                for (int i = 0; i < cols; i++)
                {
                    double dval;
                    if (i >= timeseries.Value.Count)
                    {
                        missingData = true;
                        row.Add(-9999);
                        continue;
                    }
                    if (Double.TryParse(timeseries.Value.ElementAt(i), out dval) && dval >= 0)
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

            missingDays = rows - lstTable.Count;
            rows = lstTable.Count;


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

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of values based on extreme event parameters.
        /// Requires summing of daily values first.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static void SumExtremeValues(out string errorMsg, ITimeSeriesInput inputData, ref ITimeSeriesOutput dailyData)
        {
            errorMsg = "";
            List<string> removeDates = new List<string>();
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            string dateString0 = dailyData.Data.Keys.ElementAt(0).ToString().Substring(0, dailyData.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            string endstring = dailyData.Data.Keys.ElementAt(dailyData.Data.Keys.Count - 1).ToString().Substring(0, dailyData.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out newDate);
            double sum = 0.0;
            double fiveDaySum = 0.0;
            double dailySum = 0.0;

            Queue<KeyValuePair<string, double>> previousFiveDays = new Queue<KeyValuePair<string, double>>();
            previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.ElementAt(0).Key, double.Parse(dailyData.Data.ElementAt(0).Value[0])));
            previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.ElementAt(1).Key, double.Parse(dailyData.Data.ElementAt(1).Value[0])));
            previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.ElementAt(2).Key, double.Parse(dailyData.Data.ElementAt(2).Value[0])));
            previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.ElementAt(3).Key, double.Parse(dailyData.Data.ElementAt(3).Value[0])));
            previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.ElementAt(4).Key, double.Parse(dailyData.Data.ElementAt(4).Value[0])));
            //Add new value at every iteration, and take sum of all items to find the total

            for (int i = 0; i < dailyData.Data.Count; i++)
            {
                fiveDaySum = 0.0;
                if (i > 4)//Skip first five checks
                {
                    foreach (KeyValuePair<string, double> pair in previousFiveDays)
                    {
                        double val = pair.Value;
                        if (val < -1.0)
                        {
                            //If a value is missing for ncdc, take average of all other sources, if that value is negative, just use 0
                            val = 0.0;    //Double arithmetic here may be causing issues with travis build
                            for (int x = 1; x < dailyData.Data.ElementAt(i).Value.Count - 1; x++)
                            {
                                val += double.Parse(dailyData.Data.ElementAt(i).Value[x]);
                            }
                            val /= dailyData.Data.ElementAt(i).Value.Count - 1;
                            if (val < -1.0)
                            {
                                val = 0.0;
                            }
                        }
                        fiveDaySum += val;
                    }
                }
                dailySum = double.Parse(dailyData.Data.ElementAt(i).Value[0]);
                if (dailySum < -1)
                {
                    dailySum = 0.0;
                }
                string dateString = dailyData.Data.Keys.ElementAt(i).ToString().Substring(0, dailyData.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (dailySum >= Convert.ToDouble(inputData.Geometry.GeometryMetadata["dailyThreshold"]) && fiveDaySum >= Convert.ToDouble(inputData.Geometry.GeometryMetadata["totalThreshold"]))
                {
                    //Daily >= thresh, AND 5days >= thresh
                    sum = dailySum + fiveDaySum;
                    dict.Add(iDate.ToString(inputData.DateTimeSpan.DateTimeFormat), sum);
                    newDate = iDate;
                    sum = double.Parse(dailyData.Data.ElementAt(i).Value[0]);
                }
                else if (dailySum < Convert.ToDouble(inputData.Geometry.GeometryMetadata["dailyThreshold"]) && fiveDaySum >= Convert.ToDouble(inputData.Geometry.GeometryMetadata["totalThreshold"]))
                {
                    //daily < thresh AND 5days >= thresh
                    sum = fiveDaySum;
                    dict.Add(iDate.ToString(inputData.DateTimeSpan.DateTimeFormat), sum);
                    newDate = iDate;
                    sum = double.Parse(dailyData.Data.ElementAt(i).Value[0]);
                }
                else if (dailySum >= Convert.ToDouble(inputData.Geometry.GeometryMetadata["dailyThreshold"]) && fiveDaySum < Convert.ToDouble(inputData.Geometry.GeometryMetadata["totalThreshold"]))
                {
                    //daily >= thresh AND 5days < thresh
                    sum = dailySum;
                    dict.Add(iDate.ToString(inputData.DateTimeSpan.DateTimeFormat), sum);
                    newDate = iDate;
                    sum = double.Parse(dailyData.Data.ElementAt(i).Value[0]);
                }
                else
                {
                    removeDates.Add(dailyData.Data.Keys.ElementAt(i));//sum += dailySum;
                }
                previousFiveDays.Dequeue();
                previousFiveDays.Enqueue(new KeyValuePair<string, double>(dailyData.Data.Keys.ElementAt(i), double.Parse(dailyData.Data.ElementAt(i).Value[0])));
            }

            foreach (KeyValuePair<string, double> pair in dict)
            {
                dailyData.Data[pair.Key][0] = pair.Value.ToString();
            }

            foreach (string date in removeDates)
            {
                dailyData.Data.Remove(date);
            }
        }
    }
}