using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// Merges secondary timeseries into primary timeseries. Preference given to primary timeseries.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput MergeTimeSeries(List<ITimeSeriesOutput> outputs)
        {

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = new TimeSeriesOutput();
            result.Metadata = new Dictionary<string, string>();
            result.Data = new Dictionary<string, List<string>>();

            // Assumption: secondary timeseries only has a single value for each date/data entry.
            // Merges data values for each date key in secondary into primary.
            foreach (ITimeSeriesOutput output in outputs) {
                foreach (KeyValuePair<string, string> meta in outputs.ElementAt(0).Metadata)
                {
                    string key = output.Dataset + "_" + meta.Key;
                    if (!result.Metadata.ContainsKey(key)) {
                        result.Metadata.Add(key, meta.Value);
                    }
                }
            }


            // Assumption: secondary timeseries only has a single value for each date/data entry.
            // Merges data values for each date key in secondary into primary.
            foreach (KeyValuePair<string, List<string>> timestep in outputs.ElementAt(0).Data)
            {
                List<string> values = new List<string>();
                foreach (ITimeSeriesOutput o in outputs)
                {
                    string key = (o.Data.ContainsKey(timestep.Key)) ? timestep.Key : timestep.Key.Split(" ")[0] + "T00:00:00";
                    for (int i = 0; i < o.Data[key].Count; i++)
                    {
                        values.Add(o.Data[key][i]);
                    }
                }
                result.Data.Add(timestep.Key, values );
            }
            return result;
        }

        /// <summary>
        /// Addition merge list of TimeSeriesOutput data
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput AddTimeSeries(List<ITimeSeriesOutput> data)
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
                List<string> values = new List<string>();
                foreach (string v in timeseries.Data[date])
                {
                    double value = 0.0;
                    Double.TryParse(v, out value);
                    values.Add((value * modifier).ToString("E3"));
                }
                result.Data[date] = values;
            }
            return result;
        }

        /// <summary>
        /// Sums the timestep value by column/row index by each TimeSeries in output. Assumes all timeseries data are the same size.
        /// Resulting timeseries will contain the same number of rows and columns as the input timeseries.
        /// Multiply the result by the modifer for unit conversion.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput SumTimeSeriesByColumn(List<ITimeSeriesOutput> output, double modifier)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();
            result.Dataset = output.ElementAt(0).Dataset;
            result.DataSource = output.ElementAt(0).DataSource;
            foreach (KeyValuePair<string, List<string>> timestep in output.ElementAt(0).Data)
            {
                double[] value = new double[timestep.Value.Count];
                foreach(ITimeSeriesOutput o in output)
                {
                   for(int i = 0; i < timestep.Value.Count; i++)
                    {
                        value[i] += modifier * double.Parse(o.Data[timestep.Key][i]);
                    }
                }
                result.Data.Add(timestep.Key, value.Select(v => v.ToString("E5")).ToList());
            }
            return result;
        }

        /// <summary>
        /// Sums the values for each timestep from all timeseries in the output.
        /// Resulting timeseries will contain just one value for each timestep.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="datasource"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput SumTimeSeriesByRow(string dataset, string datasource, List<ITimeSeriesOutput> output)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();
            result.Dataset = dataset;
            result.DataSource = datasource;
            OrderedDictionary orderedData = new OrderedDictionary();
            foreach (KeyValuePair<string, List<string>> timestep in output.ElementAt(0).Data)
            {
                double value = 0.0;
                foreach (ITimeSeriesOutput o in output)
                {
                    for (int i = 0; i < o.Data[timestep.Key].Count; i++)
                    {
                        value += double.Parse(o.Data[timestep.Key][i]);
                    }
                }
                result.Data.Add(timestep.Key, new List<string>() { value.ToString("E5") });
            }
            return result;
        }

        /// <summary>
        /// Sums the values for each timestep from all timeseries in the output.
        /// Resulting timeseries will contain just one value for each timestep.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="datasource"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> SumDictionaryByRow(Dictionary<string, List<string>> output0, Dictionary<string, List<string>> output1)
        {
            Dictionary<string, List<string>> newOutput = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, List<string>> timestep in output0)
            {
                double result = 0.0;
                foreach (string value in timestep.Value)
                {
                    result += double.Parse(value);
                }
                string key = (output1.ContainsKey(timestep.Key)) ? timestep.Key : timestep.Key.Split("T")[0] + " 00";
                foreach (string value in output1[key])
                {
                    result += double.Parse(value);
                }
                newOutput.Add(timestep.Key, new List<string>() { result.ToString("E3") });
            }
            return newOutput;
        }
    }
}
