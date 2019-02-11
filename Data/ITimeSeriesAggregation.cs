using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
