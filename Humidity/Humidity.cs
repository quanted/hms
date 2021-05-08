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

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            if ((this.Input.Geometry.ComID > 1 && this.Input.Geometry.Point == null) || (this.Input.Geometry.ComID > 1 && this.Input.Geometry.Point.Latitude == -9999))
            {
                this.Input.Geometry.Point = Utilities.COMID.GetCentroid(this.Input.Geometry.ComID, out errorMsg);
                this.Output.Metadata.Add("catchment_comid", this.Input.Geometry.ComID.ToString());
            }

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0 && !this.Input.Source.Contains("noaa_coastal"))
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            if (this.Relative)
            {
                switch (this.Input.Source.ToLower())
                {
                    case "prism":
                        // PRISM relative humidity Data call
                        PRISM prism = new PRISM();
                        this.Output = prism.GetRelativeHumidityData(out errorMsg, this.Output, this.Input);
                        if (errorMsg.Contains("ERROR")) { return null; }
                        break;
                    case "noaa_coastal":
                        NOAACoastal noaaCoastal = new NOAACoastal();
                        this.Output = noaaCoastal.GetData(out errorMsg, this.Output, this.Input, this.Relative);
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
