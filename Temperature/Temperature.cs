using Data;
using System;
using System.Linq;

namespace Temperature
{
    /// <summary>
    /// Temperature dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class Temperature : ITimeSeriesComponent
    {

        // -------------- Temperature Variables -------------- //

        // Temperature specific variables are listed here.


        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- Temperature Constructors -------------- //

        /// <summary>
        /// Default Temperature constructor
        /// </summary>
        public Temperature() { }


        // -------------- Temperature Functions -------------- //

        /// <summary>
        /// Get Temperature data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0)
            {
                Utilities.Time tz = new Utilities.Time();
                this.Input.Geometry.Timezone = tz.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            //TODO: Check Source and run specific subcomponent class for source
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            switch (this.Input.Source)
            {
                case "nldas":
                    // NLDAS Temperature Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS Temperature Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "daymet":
                    // daymet Temperature Data call
                    Daymet daymet = new Daymet();
                    this.Output = daymet.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for temperature was not found among available sources or is invalid.";
                    break;
            };

            // Adds Geometry metadata to the output metadata. NOT WORKING
            this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }
    }
}
