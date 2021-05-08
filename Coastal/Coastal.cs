using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coastal
{
    public class Coastal : ITimeSeriesComponent
    {
        // -------------- Coastal Variables -------------- //

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- Coastal Constructor -------------- //

        /// <summary>
        /// Default Coastal constructor
        /// </summary>
        public Coastal()
        { }


        // -------------- Coastal Functions -------------- //

        /// <summary>
        /// Get Coastal data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, int retries = 0)
        {
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone variable.
            if (this.Input.Geometry.Timezone.Offset == 0 && !this.Input.Source.Contains("noaa_coastal"))
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            switch (this.Input.Source.ToLower())
            {
                case "noaa_coastal":
                    // NOAA Coastal Data call
                    NOAACoastal noaaCoastal = new NOAACoastal();
                    this.Output = noaaCoastal.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for coastal was not found among available sources or is invalid.";
                    break;
            };

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            return this.Output;
        }
    }
}
