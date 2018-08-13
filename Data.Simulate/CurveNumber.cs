using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Data.Simulate
{
    /// <summary>
    /// Curve Number base class.
    /// Classified as simulation data, simulation takes place on a different server so only a standard data call is made.
    /// </summary>
    public class CurveNumber
    {

        /// <summary>
        /// Get simulated curve number data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public string Simulate(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput precipData)
        {
            errorMsg = "";

            // Procedure
            // Get comid from lat/lon values
            // Get CN for comid
            // Runoff calculation: https://en.wikipedia.org/wiki/Runoff_curve_number
            // Calculate soil moisture retention (S): S = 1000/CN - 10
            // Calculate initial abstraction (Ia): Ia = 0.2S (old initial abstraction was Ia = 0.05S)
            // Iterate over precipitation data (P) by date
            // If precipitation <= Ia: Runoff (Q) = 0
            // Else precipitation > Ia: Runoff (Q) = (P - Ia)^2/(P- Ia + S)
            // Output resulting data string

            string data = "";
            return data;
        }
    }
}
