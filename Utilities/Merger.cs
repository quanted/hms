using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Merger
    {

        /// <summary>
        /// Merges secondary timeseries into primary timeseries. Preference given to primary timeseries.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput MergeTimeSeries(ITimeSeriesOutput primary, ITimeSeriesOutput secondary)
        {

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = new TimeSeriesOutput();
            result = primary.Clone();

            if (!secondary.Metadata.ContainsKey(secondary.DataSource + "_ERROR"))
            {
                result.DataSource = result.DataSource + ", " + secondary.DataSource;

                int columns = primary.Data[primary.Data.Keys.ElementAt(0)].Count();
                if (!result.Metadata.ContainsKey("column_" + (columns + 2)))
                {
                    result.Metadata.Add("column_" + (columns + 2), secondary.DataSource);
                }
            }
            // Copies keys from secondary into primary.
            foreach (string key in secondary.Metadata.Keys)
            {
                if (!result.Metadata.ContainsKey(key) && !key.Contains("column"))
                {
                    result.Metadata.Add(key, secondary.Metadata[key]);
                }
            }

            // Assumption: secondary timeseries only has a single value for each date/data entry.
            // Merges data values for each date key in secondary into primary.
            if (secondary.Data.Keys.Count > 0)
            {
                foreach (string date in primary.Data.Keys)
                {
                    if (secondary.Data.ContainsKey(date))
                    {
                        foreach (string value in secondary.Data[date]) { 
                            result.Data[date].Add(value);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Addition merge list of TimeSeriesOutput data
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput MergeTimeSeries(List<ITimeSeriesOutput> data)
        {

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();
            result.Metadata = new Dictionary<string, string>();
            result.Data = new Dictionary<string, List<string>>();

            foreach (string date in data[0].Data.Keys) {
                double total = 0.0;
                foreach (ITimeSeriesOutput d in data)
                {
                    double point = 0.0;
                    Double.TryParse(d.Data[date][0], out point);
                    total += point;
                }
                result.Data[date] = new List<string>() { total.ToString("E3") };              
            }
            return result;
        }

        /// <summary>
        /// Adds secondary timeseries to primary timeseries. Preference given to primary timeseries.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput AddTimeSeries(ITimeSeriesOutput primary, ITimeSeriesOutput secondary, string columnName)
        {

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();

            //result = primary;

            if (!result.Metadata.ContainsKey("column_1"))
            {
                result.Metadata.Add("column_1", columnName);
            }
            // Copies keys from secondary into primary.
            foreach (string key in secondary.Metadata.Keys)
            {
                if (!result.Metadata.ContainsKey(key) && !key.Contains("column"))
                {
                    result.Metadata.Add(key, secondary.Metadata[key]);
                }
            }

            // Assumption: secondary timeseries only has a single value for each date/data entry.
            // Merges data values for each date key in secondary into primary.
            if (secondary.Data.Keys.Count > 0)
            {
                foreach (string date in primary.Data.Keys)
                {
                    double firstValue = 0.0;
                    Double.TryParse(primary.Data[date][0], out firstValue);
                    double secondValue = 0.0;
                    Double.TryParse(secondary.Data[date][0], out secondValue);
                    result.Data[date] = new List<string>{ (firstValue + secondValue).ToString("E3") };
                }
            }

            return result;
        }

        /// <summary>
        /// Multiply a timeseries by a modifier.
        /// </summary>
        /// <param name="timeseries"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput ModifyTimeSeries(ITimeSeriesOutput timeseries, double modifier)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();

            //result = timeseries;
            result.Metadata = timeseries.Metadata;
            result.Data = new Dictionary<string, List<string>>();

            foreach (string date in timeseries.Data.Keys)
            {
                double value = 0.0;
                Double.TryParse(timeseries.Data[date][0], out value);
                result.Data[date] = new List<string> { (modifier * value).ToString("E3") };
            }
            return result;
        }
    }
}
