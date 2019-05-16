﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radiation
{
    public class Radiation
    {

        private enum validSources { nldas, gldas, daymet };

        // -------------- Radiation Variables -------------- //

        // Radiation specific variables are listed here.

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }



        // -------------- Radiation Constructors -------------- //

        /// <summary>
        /// Default Radiation constructor
        /// </summary>
        public Radiation() { }


        // -------------- Radiation Functions -------------- //

        /// <summary>
        /// Get Radiation data function.
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
            if (String.IsNullOrWhiteSpace(this.Input.Units))
            {
                this.Input.Units = "metric";
            }
            this.Input.DataValueFormat = "E3";

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            switch (this.Input.Source)
            {
                case "nldas":
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    break;
                case "gldas":
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    break;
                case "daymet":
                    Daymet daymet = new Daymet();
                    this.Output = daymet.GetData(out errorMsg, this.Output, this.Input);
                    break;
                default:
                    if (errorMsg.Contains("ERROR")) { return null; }
                    errorMsg = "ERROR: 'Source' for wind was not found among available sources or is invalid. Valid sources: " + Enum.GetNames(typeof(validSources)).ToList();
                    break;
            }
            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            // Adds Geometry metadata to the output metadata. NOT WORKING
            // this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }

    }
}