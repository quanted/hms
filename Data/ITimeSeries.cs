using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    /// <summary>
    /// Timeseries interface for output object.
    /// </summary>
    public interface ITimeSeries
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
}
