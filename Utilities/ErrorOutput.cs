using Data;
using System.Collections.Generic;

namespace Utilities
{

    public class ErrorOutput<T> : ITimeSeriesOutput<T>
    {

        // Implement ITimeSeries interface
        public string Dataset { get; set; }
        public string DataSource { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public Dictionary<string, T> Data { get; set; }

        public ErrorOutput()
        {
            this.Dataset = "";
            this.DataSource = "";
            this.Metadata = new Dictionary<string, string>();
            this.Data = new Dictionary<string, T>();
        }

        ITimeSeriesOutput<T> ITimeSeriesOutput<T>.Clone()
        {
            throw new System.NotImplementedException();
        }

        Dictionary<string, List<double>> ITimeSeriesOutput<T>.ToHourly(string dateFormat, ITimeSeriesInput input, bool avg, bool clean)
        {
            throw new System.NotImplementedException();
        }

        Dictionary<string, List<double>> ITimeSeriesOutput<T>.ToDaily(string dateFormat, ITimeSeriesInput input, bool avg, bool clean)
        {
            throw new System.NotImplementedException();
        }

        Dictionary<string, List<double>> ITimeSeriesOutput<T>.ToMonthly(string dateFormat, ITimeSeriesInput input, bool avg, bool clean)
        {
            throw new System.NotImplementedException();
        }

        ITimeSeriesOutput ITimeSeriesOutput<T>.ToDefault(string valueFormat)
        {
            ITimeSeriesOutput<T> output = new TimeSeriesOutput<T>()
            {
                Metadata = this.Metadata,
                DataSource = this.DataSource,
                Dataset = this.Dataset
            };
            output.Data = new Dictionary<string, T>();

            return output.ToDefault(valueFormat);
        }
    }


        public class ErrorOutput : ITimeSeriesOutput
    {

        // Implement ITimeSeries interface
        public string Dataset { get; set; }
        public string DataSource { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public Dictionary<string, List<string>> Data { get; set; }

        /// <summary>
        /// Default ErrorOutput constructor
        /// </summary>
        public ErrorOutput()
        {
            this.Dataset = "";
            this.DataSource = "";
            this.Metadata = new Dictionary<string, string>();
            this.Data = new Dictionary<string, List<string>>();
        }


        /// <summary>
        /// Returns ITimeSeries object containing the errorMsg in the metadata.
        /// </summary>
        /// <param name="errorMsg">Error message to be pasted to the MetaData in the output.</param>
        /// <returns>ITimeSeries</returns>
        public ITimeSeriesOutput ReturnError(string errorMsg)
        {
            ITimeSeriesOutput output = new ErrorOutput();
            output.Metadata["ERROR"] = errorMsg;
            return output;
        }

        /// <summary>
        /// Returns ITimeSeries object containing the errorMsg in the metadata.
        /// </summary>
        /// <param name="errorMsg">Error message to be pasted to the MetaData in the output.</param>
        /// <returns>ITimeSeries</returns>
        public ITimeSeriesOutput ReturnError(string dataset, string source, string errorMsg)
        {
            ITimeSeriesOutput output = new ErrorOutput
            {
                Dataset = dataset,
                DataSource = source
            };
            output.Metadata[source + "_ERROR"] = errorMsg;
            return output;
        }

        /// <summary>
        /// Returns ITimeSeries object containing the errorMsg in the metadata.
        /// </summary>
        /// <param name="errorMsg">Error message to be pasted to the MetaData in the output.</param>
        /// <returns>ITimeSeries</returns>
        public ITimeSeriesOutput<T> ReturnError<T>(string dataset, string source, string errorMsg)
        {
            ITimeSeriesOutput<T> output = new ErrorOutput<T>
            {
                Dataset = dataset,
                DataSource = source
            };
            output.Metadata[source + "_ERROR"] = errorMsg;
            return output;
        }

        public ITimeSeriesOutput Clone()
        {
            return this.ReturnError("ERROR: Problem setting up output.");
        }

        public ITimeSeriesOutput<List<double>> ToListDouble()
        {
            throw new System.NotImplementedException();
        }
    }

    public class MetaErrorOutput
    {

        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Default ErrorOutput constructor
        /// </summary>
        public MetaErrorOutput()
        {
            this.Metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns ITimeSeries object containing the errorMsg in the metadata.
        /// </summary>
        /// <param name="errorMsg">Error message to be pasted to the MetaData in the output.</param>
        /// <returns>ITimeSeries</returns>
        public Dictionary<string, string> ReturnError(string errorMsg)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            output["ERROR"] = errorMsg;
            return output;
        }
    }

}
