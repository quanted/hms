﻿using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Wind
{
    /// <summary>
    /// Precipitation GLDAS class
    /// </summary>
    public class GLDAS
    {
        /// <summary>
        /// Makes the GetData call to the base GLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, "Wind", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput gldasOutput = output;
            gldasOutput = gldas.SetDataToOutput(out errorMsg, "Wind", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyAverage(out errorMsg, 7, 1.0, output, input);
                    break;
                case "default":
                default:
                    break;
            }
            output.Metadata["column_1"] = "date";
            output.Metadata["column_2"] = "velocity";
            output.Metadata["column_2_units"] = "m/s";

            return gldasOutput;
        }
    }
}
