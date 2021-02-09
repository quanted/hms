using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ITimeSeriesOutput Clone();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput<List<double>> ToListDouble();


    }

    /// <summary>
    /// TimeSeriesOutput interface for output object.
    /// </summary>
    public interface ITimeSeriesOutput<T>
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
        Dictionary<string, T> Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ITimeSeriesOutput<T> Clone();

        /// <summary>
        /// Aggregate Data to hourly time intervals, no action taken if current interval is hourly or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToHourly(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean = true);

        /// <summary>
        /// Aggregate Data to daily time intervals, no action taken if current interval is daily or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToDaily(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean = true);

        /// <summary>
        /// Aggregate Data to monthly time intervals, no action taken if current interval is monthly or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToMonthly(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean = true);

        /// <summary>
        /// Convert ITimeSeriesOutput<T> to default ITimeSeriesOutput with string representation of numerical values.
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput ToDefault(string valueFormat = "E3");


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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput Clone()
        {
            TimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            TimeSeriesOutput newOutput = (TimeSeriesOutput)oFactory.Initialize();
            newOutput = this;
            return newOutput;
        }

        /// <summary>
        /// Convert ITimeSeriesOutput to ITimeSeriesOutput<list type="<double>"></list> with string representation of numerical values.
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput<List<double>> ToListDouble()
        {
            if (this.Data == null)
            {
                return null;
            }
            else
            {
                ITimeSeriesOutput<List<double>> output = new TimeSeriesOutput<List<double>>();
                output.DataSource = this.DataSource;
                output.Dataset = this.Dataset;
                output.Metadata = this.Metadata;
                output.Data = new Dictionary<string, List<double>>();
                foreach (var item in this.Data)
                {
                    List<double> values = new List<double>();
                    foreach (string v in item.Value)
                    {
                        values.Add(Convert.ToDouble(v));
                    }
                    output.Data.Add(item.Key, values);
                }
                return output;
            }
        }

    }

    /// <summary>
    /// Concrete TimeSeriesOutput class
    /// </summary>
    public class TimeSeriesOutput<T> : ITimeSeriesOutput<T>
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
        public Dictionary<string, T> Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput<T> Clone()
        {
            TimeSeriesOutputFactory<T> oFactory = new TimeSeriesOutputFactory<T>();
            TimeSeriesOutput<T> newOutput = (TimeSeriesOutput<T>)oFactory.Initialize();
            newOutput = this;
            return newOutput;
        }

        /// <summary>
        /// Cleanup timeseries data with filled in value.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        private Dictionary<DateTime, List<double>> CleanUp(ITimeSeriesInput input, Dictionary<DateTime, List<double>> data)
        {
            double value = -9999;

            if (input.Geometry.GeometryMetadata.ContainsKey("fill"))
            {
                value = Double.Parse(input.Geometry.GeometryMetadata["fill"]);
            }
            else
            {
                return data;
            }

            Dictionary<DateTime, List<double>> cleaned = new Dictionary<DateTime, List<double>>();
            DateTime d0 = input.DateTimeSpan.StartDate;
            List<double> values = new List<double>();
            int n = data.Values.First<List<double>>().Count;
            for(int i = 0; i < n; i++)
            {
                values.Add(value);
            }
            int missingN = 0;
            int totalN = 0;

            if (input.TemporalResolution == "hourly")
            {
                while (d0 <= input.DateTimeSpan.EndDate)
                {
                    if (!data.ContainsKey(d0))
                    {
                        cleaned.Add(d0, values);
                        missingN += 1;
                    }
                    else
                    {
                        cleaned.Add(d0, data[d0]);
                    }
                    d0 = d0.AddHours(1.0);
                    totalN += 1;
                }
            }
            else if (input.TemporalResolution == "daily")
            {
                while (d0 <= input.DateTimeSpan.EndDate)
                {
                    if (!data.ContainsKey(d0))
                    {
                        cleaned.Add(d0, values);
                        missingN += 1;
                    }
                    else
                    {
                        cleaned.Add(d0, data[d0]);
                    }
                    d0 = d0.AddDays(1.0);
                    totalN += 1;
                }
            }
            else if (input.TemporalResolution == "monthly")
            {
                while (d0 <= input.DateTimeSpan.EndDate)
                {
                    if (!data.ContainsKey(d0))
                    {
                        cleaned.Add(d0, values);
                        missingN += 1;
                    }
                    else
                    {
                        cleaned.Add(d0, data[d0]);
                    }
                    d0 = d0.AddMonths(1);
                    totalN += 1;
                }
            }
            else if (input.TemporalResolution == "yearly")
            {
                while (d0 <= input.DateTimeSpan.EndDate)
                {
                    if (!data.ContainsKey(d0))
                    {
                        cleaned.Add(d0, values);
                        missingN += 1;
                    }
                    else
                    {
                        cleaned.Add(d0, data[d0]);
                    }
                    d0 = d0.AddYears(1);
                    totalN += 1;
                }
            }
            double missingP = (double)missingN / (double)totalN;
            this.Metadata.Add("missing_data_value", input.Geometry.GeometryMetadata["fill"]);
            this.Metadata.Add("missing_values", missingN.ToString());
            this.Metadata.Add("missing_values_p", Math.Round(missingP, 3).ToString());
            return cleaned;
        }

        /// <summary>
        /// Aggregate Data to hourly time intervals, no action taken if current interval is hourly or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToHourly(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean=true)
        {
            if (this.Data.GetType() != typeof(Dictionary<string, List<double>>))
            {
                return null;
            }

            ITimeSeriesAggregationFactory iFactory = new TimeSeriesAggregationFactory();
            ITimeSeriesAggregation timeSeries = iFactory.Initialize(this);
            TimeSpan ts = timeSeries.TimeDifference();
            if (ts > TimeSpan.FromHours(1.0) || ts.Ticks == 0)
            {
                return null;
            }

            Dictionary<DateTime, List<double>> aggregated = new Dictionary<DateTime, List<double>>();
            DateTime d0 = timeSeries.Data.Keys.ElementAt(0);
            List<double> values = new List<double>();
            foreach (var item in timeSeries.Data)
            {
                if (item.Key.Subtract(d0) < TimeSpan.FromHours(1.0))
                {
                    if (values.Count == 0)
                    {
                        values = item.Value;
                    }
                    else
                    {
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            if (avg)
                            {
                                values[i] = (values[i] + item.Value[i]) / 2.0;
                            }
                            else
                            {
                                values[i] += item.Value[i];
                            }
                        }
                    }
                }
                else
                {
                    aggregated.Add(d0, values);
                    d0 = item.Key;
                    values = item.Value;
                }
            }
            if (!aggregated.ContainsKey(d0))
            {
                aggregated.Add(d0, values);
            }
            if (clean)
            {
                timeSeries.Data = this.CleanUp(input, aggregated);
            }
            else
            {
                timeSeries.Data = aggregated;
            }
            return this.ToDict(timeSeries.Data, input.DateTimeSpan.DateTimeFormat);
        }

        /// <summary>
        /// Aggregate Data to daily time intervals, no action taken if current interval is daily or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToDaily(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean = true)
        {
            if (this.Data.GetType() != typeof(Dictionary<string, List<double>>))
            {
                return null;
            }

            ITimeSeriesAggregationFactory iFactory = new TimeSeriesAggregationFactory();
            ITimeSeriesAggregation timeSeries = iFactory.Initialize(this);
            TimeSpan ts = timeSeries.TimeDifference();
            if (ts > TimeSpan.FromDays(1.0) || ts.Ticks == 0)
            {
                return null;
            }

            Dictionary<DateTime, List<double>> aggregated = new Dictionary<DateTime, List<double>>();
            DateTime d0 = timeSeries.Data.Keys.ElementAt(0);
            List<double> values = new List<double>();
            foreach (var item in timeSeries.Data)
            {
                if (item.Key.Day == d0.Day)
                {
                    if (values.Count == 0)
                    {
                        values = item.Value;
                    }
                    else
                    {
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            if (avg)
                            {
                                values[i] = (values[i] + item.Value[i]) / 2.0;
                            }
                            else
                            {
                                values[i] += item.Value[i];
                            }
                        }
                    }
                }
                else
                {
                    if (aggregated.ContainsKey(d0))
                    {
                        aggregated[d0] = values;
                    }
                    else
                    {
                        aggregated.Add(d0, values);
                    }
                    d0 = item.Key;
                    values = item.Value;
                }
            }
            if (!aggregated.ContainsKey(d0))
            {
                aggregated.Add(d0, values);
            }
            if (clean)
            {
                timeSeries.Data = this.CleanUp(input, aggregated);
            }
            else
            {
                timeSeries.Data = aggregated;
            }
            return this.ToDict(timeSeries.Data, input.DateTimeSpan.DateTimeFormat);
        }

        /// <summary>
        /// Aggregate Data to monthly time intervals, no action taken if current interval is monthly or greater.
        /// </summary>
        /// <param name="avg">averaging aggregation or sum aggregation</param>
        public Dictionary<string, List<double>> ToMonthly(string dateFormat, ITimeSeriesInput input, bool avg = false, bool clean = true)
        {
            if (this.Data.GetType() != typeof(Dictionary<string, List<double>>))
            {
                return null;
            }

            ITimeSeriesAggregationFactory iFactory = new TimeSeriesAggregationFactory();
            ITimeSeriesAggregation timeSeries = iFactory.Initialize(this);
            TimeSpan ts = timeSeries.TimeDifference();
            if (ts > TimeSpan.FromDays(31.0) || ts.Ticks == 0)
            {
                return null;
            }


            Dictionary<DateTime, List<double>> aggregated = new Dictionary<DateTime, List<double>>();
            DateTime d0 = timeSeries.Data.Keys.ElementAt(0);
            List<double> values = new List<double>();
            foreach (var item in timeSeries.Data)
            {
                if (item.Key.Month == d0.Month)
                {
                    if (values.Count == 0)
                    {
                        values = item.Value;
                    }
                    else
                    {
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            if (avg)
                            {
                                values[i] = (values[i] + item.Value[i]) / 2.0;
                            }
                            else
                            {
                                values[i] += item.Value[i];
                            }
                        }
                    }
                }
                else
                {
                    if (aggregated.ContainsKey(d0))
                    {
                        aggregated[d0] = values;
                    }
                    else
                    {
                        aggregated.Add(d0, values);
                    }
                    d0 = item.Key;
                    values = item.Value;
                }
            }
            if (!aggregated.ContainsKey(d0))
            {
                aggregated.Add(d0, values);
            }
            if (clean)
            {
                timeSeries.Data = this.CleanUp(input, aggregated);
            }
            else
            {
                timeSeries.Data = aggregated;
            }
            return this.ToDict(timeSeries.Data, input.DateTimeSpan.DateTimeFormat);
        }

        private Dictionary<string, List<double>> ToDict(Dictionary<DateTime, List<double>> timeseries, string dateFormat)
        {
            Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
            foreach(KeyValuePair<DateTime, List<double>> kv in timeseries)
            {
                data.Add(kv.Key.ToString(dateFormat), kv.Value);
            }
            return data;
        }

        /// <summary>
        /// Convert ITimeSeriesOutput<T> to default ITimeSeriesOutput with string representation of numerical values.
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesOutput ToDefault(string valueFormat = "E3")
        {
            if (this.Data == null)
            {
                return null;
            }
            else
            {
                ITimeSeriesOutput output = new TimeSeriesOutput();
                output.DataSource = this.DataSource;
                output.Dataset = this.Dataset;
                output.Metadata = this.Metadata;
                output.Data = new Dictionary<string, List<string>>();
                foreach (var item in this.Data)
                {
                    List<double> v = ((IEnumerable<double>)item.Value).Cast<double>().ToList();
                    List<string> values = new List<string>();
                    for (int i = 0; i < v.Count; i++)
                    {
                        values.Add(v[i].ToString(valueFormat));
                    }
                    output.Data.Add(item.Key, values);
                }
                return output;
            }
        }

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

    /// <summary>
    /// Abstract ITimeSeriesOutput Factory class
    /// </summary>
    public abstract class ITimeSeriesOutputFactory<T>
    {
        /// <summary>
        /// ITimeSeriesOutput Intializer.
        /// </summary>
        /// <returns></returns>
        public abstract ITimeSeriesOutput<T> Initialize();
    }

    /// <summary>
    /// Concrete ITimeSeriesOutput Factory Class
    /// </summary>
    public class TimeSeriesOutputFactory<T> : ITimeSeriesOutputFactory<T>
    {
        /// <summary>
        /// Initializer.
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesOutput<T> Initialize()
        {
            TimeSeriesOutput<T> output = new TimeSeriesOutput<T>()
            {
                Data = new Dictionary<string,T>(),
                Dataset = "",
                DataSource = "",
                Metadata = new Dictionary<string, string>()
            };
            return output;
        }
    }
}
