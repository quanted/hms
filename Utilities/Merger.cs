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
            ITimeSeriesOutput result = oFactory.Initialize();

            result = primary;

            result.DataSource = result.DataSource + ", " + secondary.DataSource;

            int columns = primary.Data[primary.Data.Keys.ElementAt(0)].Count();

            result.Metadata.Add("column_" + (columns + 2), secondary.DataSource);
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
            foreach (string date in result.Data.Keys)
            {
                result.Data[date].Add(secondary.Data[date][0]);
                //result.Data[date].Concat(secondary.Data[date]);
            }

            return result;
        }
    }
}
