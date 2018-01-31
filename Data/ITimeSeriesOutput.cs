using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    /// <summary>
    /// TimeSeriesOutput interface for output object.
    /// </summary>
    public interface ITimeSeriesOutput
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
        Dictionary<string, List<string>> Data { get; set; }
    }

    /// <summary>
    /// Concrete TimeSeriesOutput class
    /// </summary>
    public class TimeSeriesOutput : ITimeSeriesOutput
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
        public Dictionary<string, List<string>> Data { get; set; }
    }

    /// <summary>
    /// Abstract ITimeSeriesOutput Factory class
    /// </summary>
    public abstract class ITimeSeriesOutputFactory
    {
        /// <summary>
        /// ITimeSeriesOutput Intializer.
        /// </summary>
        /// <returns></returns>
        public abstract ITimeSeriesOutput Initialize();
    }

    /// <summary>
    /// Concrete ITimeSeriesOutput Factory Class
    /// </summary>
    public class TimeSeriesOutputFactory : ITimeSeriesOutputFactory
    {
        /// <summary>
        /// Initializer.
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesOutput Initialize()
        {
            TimeSeriesOutput output = new TimeSeriesOutput()
            {
                Data = new Dictionary<string, List<string>>(),
                Dataset = "",
                DataSource = "",
                Metadata = new Dictionary<string, string>()
            };
            return output;
        }
    }
}
