using Data;
using System;
using System.Collections.Generic;

namespace Precipitation
{
    /// <summary>
    /// Precipitation dataset class: Implemented ITimeSeriesComponent
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
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            //TODO: If Timezone data is not present make call to function to Get Timezone data: set to Geometry.TimeZone
            //TODO: Check Source and run specific subcomponent class for source

            // NLDAS Precipitation Data call
            NLDAS nldas = new NLDAS();

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
            if (errorMsg.Contains("ERROR")) { return null; }
            return this.Output;
        }
    }
}