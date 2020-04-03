using Data;
using System.Collections.Generic;
using System.Linq;

namespace SubSurfaceFlow
{
    /// <summary>
    /// SubSurfaceFlow dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class SubSurfaceFlow : ITimeSeriesComponent
    {

        // -------------- SubSurfaceFlow Variables -------------- //

        // SubSurfaceFlow specific variables are listed here.


        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- SubSurfaceFlow Constructors -------------- //

        /// <summary>
        /// Default SubSurfaceFlow constructor
        /// </summary>
        public SubSurfaceFlow() { }


        // -------------- SubSurfaceFlow Functions -------------- //

        /// <summary>
        /// Get SubSurfaceFlow data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0 && this.Input.Geometry.Point != null)
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            else
            {
                this.Input.TimeLocalized = false;
            }

            //TODO: Check Source and run specific subcomponent class for source
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            switch (this.Input.Source)
            {
                case "nldas":
                    // NLDAS SubSurfaceFlow Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS SubSurfaceFlow Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "curvenumber":
                    // Curve number calculation using surface runoff and baseflow % by comid from streamcat
                    CurveNumber cn = new CurveNumber();
                    this.Output = cn.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for subsurfaceflow was not found among available sources or is invalid.";
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

        /// <summary>
        /// Check subsurface data endpoints.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> CheckEndpointStatus()
        {
            switch (this.Input.Source)
            {
                case "nldas":
                    return NLDAS.CheckStatus(this.Input);
                case "gldas":
                    return GLDAS.CheckStatus(this.Input);
                default:
                    return new Dictionary<string, string>() { { "status", "invalid source" } };
            }
        }
    }
}