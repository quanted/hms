using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoilMoisture
{
    /// <summary>
    /// SoilMoisture dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class SoilMoisture : ITimeSeriesComponent
    {

        // -------------- SoilMoisture Variables -------------- //

        // SoilMoisture specific variables are listed here.
        public List<string> Layers { get; set; }

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- SoilMoisture Constructors -------------- //

        /// <summary>
        /// Default SoilMoisture constructor
        /// </summary>
        public SoilMoisture() { }


        // -------------- SoilMoisture Functions -------------- //

        /// <summary>
        /// Get SoilMoisture data function.
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

            if (!CheckLayers(out errorMsg)) { return null; }

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            switch (this.Input.Source)
            {
                case "nldas":
                    // NLDAS SoilMoisture Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS SoilMoisture Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for SoilMoisture was not found among available sources or is invalid.";
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
        /// Verifies that the provided layers are valid for the specified source.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool CheckLayers(out string errorMsg)
        {
            errorMsg = "";
            string[] validLayers = (this.Input.Source.Contains("nldas")) ? new string[] { "0-10", "10-40", "40-100", "100-200", "0-100", "0-200" } : new string[] { "0-10", "10-40", "40-100", "0-100" };

            for( int i = 0; i < this.Layers.Count; i++)
            {
                if (!validLayers.Contains(this.Layers[i]))
                {
                    errorMsg = "ERROR: " + this.Layers[i] + " is not a valid layer for soil moisture from " + this.Input.Source + ".";
                    return false;
                }
            }
            return true;
        }
    }
}
