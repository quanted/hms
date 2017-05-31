using Data;
using System;
using System.Collections.Generic;

namespace Precipitation
{
    public class Precipitation : ITimeSeriesComponent
    {

        // Implement interfaces
        public ITimeSeries Output { get; set; }
        public ITimeSeriesInput Input { get; set; }


        public Precipitation() { }

        /// <summary>
        /// Default Precipitation constructor
        /// </summary>
        /// <param name="input"></param>
        public Precipitation(ITimeSeriesInput input)
        {
            this.Input.Source = input.Source;
            this.Input.TimeSpan = input.TimeSpan;
            this.Input.Geometry = input.Geometry;
            this.Input.TemporalResolution = input.TemporalResolution;
            this.Input.TimeLocalized = input.TimeLocalized;
            this.Input.Units = input.Units;
            this.Input.SpatialAggregation = input.SpatialAggregation;
            this.Input.OutputFormat = input.OutputFormat;
        }

        public ITimeSeries GetData(out string errorMsg)
        {
            errorMsg = "";

            // Check Geometry type: if not coordinate, get total number of points
            // Get Timezone data: set to Geometry.TimeZone
            // Check Source: run specific subcomponent class for source

            NLDAS nldas = new NLDAS();
            this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
            if (errorMsg.Contains("ERROR")) { return null; }
            return this.Output;
        }
    }
}
