using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfaceRunoff
{
    /// <summary>
    /// SurfaceRunoff dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class SurfaceRunoff : ITimeSeriesComponent
    {

        // -------------- SurfaceRunoff Variables -------------- //

        // SurfaceRunoff specific variables are listed here.
        /// <summary>
        /// OPTIONAL: Precipitation data source for Curve Number (NLDAS, GLDAS, NCDC, DAYMET, PRISM, WGEN)
        /// </summary>
        public string CurveSource { get; set; }
        
        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- SurfaceRunoff Constructors -------------- //

        /// <summary>
        /// Default SurfaceRunoff constructor
        /// </summary>
        public SurfaceRunoff() { }


        // -------------- SurfaceRunoff Functions -------------- //

        /// <summary>
        /// Get SurfaceRunoff data function.
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

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            switch (this.Input.Source)
            {
                case "nldas":
                    // NLDAS SurfaceRunoff Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS SurfaceRunoff Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "curvenumber":
                    //Curve Number SurfaceRunoff not yet working
                    CurveNumber curve = new CurveNumber();
                    this.Input.Source = this.CurveSource;
                    this.Output = curve.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for surfacerunoff was not found among available sources or is invalid.";
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
        /// Check surface runoff data endpoints.
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
