using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Data
{
    /// <summary>
    /// ITimeSeriesAggregation interface for output object.
    /// </summary>
    public interface ITimeSeriesAggregation
    {
        /// <summary>
        /// Dataset for the time series.
        /// </summary>
        string Dataset { get; set; }

        /// <summary>
        /// Source of the dataset.
        /// </summary>
        string DataSource { get; set; }

        /// <summary>
        /// Metadata dictionary providing details for the time series.
        /// </summary>
        Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Time series data.
        /// </summary>
        Dictionary<DateTime, List<double>> Data { get; set; }

        /// <summary>
        /// Determines temporal interval of time-series, assuming uniform distribution.
        /// </summary>
        /// <returns></returns>
        public TimeSpan TimeDifference();
    }

    /// <summary>
    /// Concrete TimeSeriesAggregation class
    /// </summary>
    public class TimeSeriesAggregation : ITimeSeriesAggregation
    {
        /// <summary>
        /// Dataset for the time series.
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Source of the dataset.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Metadata dictionary providing details for the time series.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Time series data.
        /// </summary>
        public Dictionary<DateTime, List<double>> Data { get; set; }

        /// <summary>
        /// Determines temporal interval of time-series, assuming uniform distribution.
        /// </summary>
        /// <returns>TimeSpan of the temporal interval.</returns>
        public TimeSpan TimeDifference()
        {
            if(this.Data.Count >= 2)
            {
                List<DateTime> dates = Data.Keys.ToList();
                return dates[1].Subtract(dates[0]);
            }
            else
            {
                return new TimeSpan(0);
            }
        }
    }

    /// <summary>
    /// Abstract ITimeSeriesAggregation Factory class
    /// </summary>
    public abstract class ITimeSeriesAggregationFactory
    {
        /// <summary>
        /// ITimeSeriesAggregation Intializer.
        /// </summary>
        /// <returns></returns>
        public abstract ITimeSeriesAggregation Initialize();

        /// <summary>
        /// Convert existing ITimeSeriesOutput to ITimeSeriesAggregation for temporal aggregation functions.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract ITimeSeriesAggregation Initialize(ITimeSeriesOutput output);


        /// <summary>
        /// Convert existing ITimeSeriesOutput to ITimeSeriesAggregation for temporal aggregation functions.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract ITimeSeriesAggregation Initialize<T>(ITimeSeriesOutput<T> output);
    }

    /// <summary>
    /// Concrete ITimeSeriesAggregation Factory Class
    /// </summary>
    public class TimeSeriesAggregationFactory : ITimeSeriesAggregationFactory
    {
        /// <summary>
        /// Initializer.
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesAggregation Initialize()
        {
            TimeSeriesAggregation output = new TimeSeriesAggregation()
            {
                Data = new Dictionary<DateTime, List<double>>(),
                Dataset = "",
                DataSource = "",
                Metadata = new Dictionary<string, string>()
            };
            return output;
        }

        /// <summary>
        /// Initializer.
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesAggregation Initialize(ITimeSeriesOutput output)
        {
            TimeSeriesAggregation transformed = new TimeSeriesAggregation()
            {
                Data = new Dictionary<DateTime, List<double>>(),
                Dataset = output.Dataset,
                DataSource = output.DataSource,
                Metadata = output.Metadata
            };
            foreach(var item in output.Data)
            {
                DateTime date = DateTime.ParseExact(item.Key, "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
                List<double> values = new List<double>();
                for(int i = 0; i < item.Value.Count; i++)
                {
                    values.Add(Double.Parse(item.Value[i]));
                }
                transformed.Data.Add(date, values);
            }
            return transformed;
        }

        /// <summary>
        /// Initializer.
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesAggregation Initialize<T>(ITimeSeriesOutput<T> output)
        {
            TimeSeriesAggregation transformed = new TimeSeriesAggregation()
            {
                Data = new Dictionary<DateTime, List<double>>(),
                Dataset = output.Dataset,
                DataSource = output.DataSource,
                Metadata = output.Metadata
            };
            foreach (var item in output.Data)
            {
                DateTime date = DateTime.ParseExact(item.Key, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                List<double> values = new List<double>();
                List<double> v = ((IEnumerable<double>)item.Value).Cast<double>().ToList();
                for (int i = 0; i < v.Count; i++)
                {
                    values.Add(Convert.ToDouble(v[i]));
                }
                transformed.Data.Add(date, values);
            }
            return transformed;
        }

    }
}
