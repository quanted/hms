using Data;
using System.Collections.Generic;

namespace Streamflow
{
    /// <summary>
    /// Streamflow dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class Streamflow : ITimeSeriesComponent<List<double>>
    {

        // -------------- Streamflow Variables -------------- //

        // Streamflow specific variables are listed here.
        public string precipSource { get; set; }
        public string runoffSource { get; set; }            // baseflow is assumed to have the same source as surface runoff

        public string streamBoundarySource { get; set; }

        public ITimeSeriesOutput<List<double>> precipTS { get; set; }
        public ITimeSeriesOutput<List<double>> runoffTS { get; set; }
        public ITimeSeriesOutput<List<double>> baseflowTS { get; set; }

        public ITimeSeriesOutput<List<double>> streamTS { get; set; }


        // TimeSeries Output variable 
        public ITimeSeriesOutput<List<double>> Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- Streamflow Constructors -------------- //

        /// <summary>
        /// Default Streamflow constructor
        /// </summary>
        public Streamflow() { }

        public Streamflow(string precipSource, string runoffSource, string streamBoundarySource)
        {
            this.precipSource = precipSource;
            this.runoffSource = runoffSource;
            this.streamBoundarySource = streamBoundarySource;
        }

        public Streamflow(ITimeSeriesOutput<List<double>> precipTS, ITimeSeriesOutput<List<double>> runoffTS, ITimeSeriesOutput<List<double>> baseflowTS)
        {
            this.precipTS = precipTS;
            this.runoffTS = runoffTS;
            this.baseflowTS = baseflowTS;
            this.streamTS = null;
        }

        public Streamflow(ITimeSeriesOutput<List<double>> precipTS, ITimeSeriesOutput<List<double>> runoffTS, ITimeSeriesOutput<List<double>> baseflowTS, ITimeSeriesOutput<List<double>> streamTS)
        {
            this.precipTS = precipTS;
            this.runoffTS = runoffTS;
            this.baseflowTS = baseflowTS;
            this.streamTS = streamTS;
        }

        // -------------- Streamflow Functions -------------- //

        /// <summary>
        /// Get Streamflow data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, int retries = 0)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory<List<double>> iFactory = new TimeSeriesOutputFactory<List<double>>();
            this.Output = iFactory.Initialize();


            switch (this.Input.Source.ToLower())
            {
                case "nwis":
                case "usgs":
                case "streamgauge":
                    StreamGauge sg = new StreamGauge();
                    this.Output = sg.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "constantvolume":
                case "cv":
                    ConstantVolume cv = new ConstantVolume();
                    if (!System.String.IsNullOrEmpty(this.precipSource) && !System.String.IsNullOrEmpty(this.runoffSource))
                    {
                        // Retrieve data
                    }
                    else if (!(this.precipTS == null && this.runoffTS == null && this.baseflowTS == null))
                    {
                        // Combine data
                    }

                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for Streamflow was not found among available sources or is invalid.";
                    break;
            };

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            return this.Output;
        }
    }
}