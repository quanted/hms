﻿using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service SurfaceRunoff Model
    /// </summary>
    public class WSSurfaceRunoff
    {

        private enum RunoffSources { nldas, gldas, curvenumber }

        /// <summary>
        /// Gets SurfaceRunoff data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetSurfaceRunoff(ITimeSeriesInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // SurfaceRunoff object
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the SurfaceRunoff data.
            ITimeSeriesOutput result = runoff.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;

        }
    }
}