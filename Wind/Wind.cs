﻿using Data;
using System;
using System.Linq;

namespace Wind
{
    public class Wind
    {

        private enum validSources { nldas, gldas, ncei, noaa_coastal };

        // -------------- Wind Variables -------------- //

        // Wind specific variables are listed here.

        // Wind component
        // User request for U, V values and/or speed, direction. All input is made upper case for comparison.
        // Valid values: U/V, SPEED/DIR (SPEED/DIRECTION), ALL
        // Defaults: ALL 
        public string component = "ALL";


        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }



        // -------------- Wind Constructors -------------- //

        /// <summary>
        /// Default Wind constructor
        /// </summary>
        public Wind() { }


        // -------------- Wind Functions -------------- //

        /// <summary>
        /// Get Wind data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            if ((this.Input.Geometry.ComID > 1 && this.Input.Geometry.Point == null) 
                || (this.Input.Geometry.ComID > 1 && this.Input.Geometry.Point.Latitude == -9999))
            {
                this.Input.Geometry.Point = Utilities.COMID.GetCentroid(this.Input.Geometry.ComID, out errorMsg);
                this.Output.Metadata.Add("catchment_comid", this.Input.Geometry.ComID.ToString());
            }

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0 
                && !this.Input.Source.Contains("ncei") && !this.Input.Source.Contains("noaa_coastal"))
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            if (String.IsNullOrWhiteSpace(this.Input.Units))
            {
                this.Input.Units = "metric";
            }
            //this.Input.DataValueFormat = "E3";

            switch (this.Input.Source.ToLower())
            {
                case "noaa_coastal":
                    NOAACoastal noaaCoastal = new NOAACoastal();
                    this.Output = noaaCoastal.GetData(out errorMsg, this.Output, this.Input);
                    break;
                case "nldas":
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.component, this.Output, this.Input);
                    break;
                case "gldas":
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    break;
                case "ncei":
                    NCEI ncei = new NCEI();
                    this.Input.TemporalResolution = (this.Input.TemporalResolution == "default") ? "daily" : this.Input.TemporalResolution;
                    this.Output = ncei.GetData(out errorMsg, this.Output, this.Input);
                    break;
                default:
                    if (errorMsg.Contains("ERROR")) { return null; }
                    errorMsg = "ERROR: 'Source' for wind was not found among available sources or is invalid. Valid sources: " + Enum.GetNames(typeof(validSources)).ToList();
                    break;
            }
            if (errorMsg.Contains("ERROR")) {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

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
