﻿using Data;
using System.Collections.Generic;

namespace Precipitation
{
    /// <summary>
    /// Precipitation dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class Precipitation : ITimeSeriesComponent
    {

        // -------------- Precipitation Variables -------------- //

        // Precipitation specific variables are listed here.


        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- Precipitation Constructors -------------- //

        /// <summary>
        /// Default Precipitation constructor
        /// </summary>
        public Precipitation() { }


        // -------------- Precipitation Functions -------------- //

        /// <summary>
        /// Get Precipitation data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, int retries = 0)
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
            if (this.Input.Geometry.Timezone.Offset == 0 && !this.Input.Source.Contains("ncei"))
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            switch (this.Input.Source.ToLower()) {
                case "nldas":
                    // NLDAS Precipitation Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS Precipitation Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "ncei":
                    // NCDC Precipitation Data call
                    NCDC ncdc = new NCDC();
                    this.Output = ncdc.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    this.Input.Source = "ncei";
                    break;
                case "daymet":
                    // daymet Precipitation Data call
                    Daymet daymet = new Daymet();
                    this.Output = daymet.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "wgen":
                    // wgen Precipitation Data call
                    WGEN wgen = new WGEN();
                    this.Output = wgen.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "prism":
                    // PRISM Precipitation Data call
                    PRISM prism = new PRISM();
                    this.Output = prism.GetData(out errorMsg, this.Output, this.Input, retries);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "trmm":
                    // TRMM Precipitation Data call
                    TRMM trmm = new TRMM();
                    this.Output = trmm.GetData(out errorMsg, this.Output, this.Input, retries);
                    if(errorMsg.Contains("ERROR")) { return null; }
                    break;

                case "nwm":
                    // wgen Precipitation Data call
                    NWM nwm = new NWM();
                    this.Output = nwm.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for precipitation was not found among available sources or is invalid.";
                    break;
            };

            // Adds Geometry metadata to the output metadata. NOT WORKING
            //this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }

        /// <summary>
        /// Check precipitation data endpoints.
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
                case "daymet":
                    return Daymet.CheckStatus(this.Input);
                case "ncei":
                    return NCDC.CheckStatus(this.Input);
                // TODO: Add check status function for PRISM, nwm, trmm
                default:
                    return new Dictionary<string, string>() { { "status", "invalid source" } };
            }
        }
    }
}