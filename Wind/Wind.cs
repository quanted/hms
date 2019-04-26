﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wind
{
    public class Wind
    {

        private enum validSources { nldas, gldas, ncei };

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
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.component, this.Output, this.Input);
                    break;
                default:
                    if (errorMsg.Contains("ERROR")) { return null; }
                    errorMsg = "ERROR: 'Source' for wind was not found among available sources or is invalid. Valid sources: " + Enum.GetNames(typeof(validSources)).ToList();
                    break;
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