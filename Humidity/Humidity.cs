using System;
using Data;

namespace Humidity
{
    public class Humidity : ITimeSeriesComponent
    {
        // -------------- Humidity Variables -------------- //

        // Humidity specific variables are listed here.
        bool Relative;  // Specifiy if relative humidity or not (specific humidity)

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }

        // -------------- Humidity Constructors -------------- //

        /// <summary>
        /// Default Humidity constructor
        /// </summary>
        public Humidity(bool relative)
        {
            this.Relative = relative;
        }


        // -------------- Humidity Functions -------------- //

        /// <summary>
        /// Get Humidity data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0)
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            //TODO: Check Source and run specific subcomponent class for source
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            if (this.Relative)
            {
                switch (this.Input.Source)
                {
                    case "prism":
                        // PRISM relative humidity Data call
                        PRISM prism = new PRISM();
                        this.Output = prism.GetRelativeHumidityData(out errorMsg, this.Output, this.Input);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        break;
                    default:
                        errorMsg = "ERROR: 'Source' for relative humidity was not found among available sources or is invalid.";
                        break;
                };
            }
            else
            {
                errorMsg = "ERROR: Specific humidity component not yet added.";
            }

            // Adds Geometry metadata to the output metadata. NOT WORKING
            //this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }

    }
}
