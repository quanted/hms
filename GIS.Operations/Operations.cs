using System;
using System.Collections.Generic;
using System.Text;

namespace GIS.Operations
{
    public class Operations
    {

        public static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double p1 = Math.Pow(lat2 - lat1, 2.0);
            double p2 = Math.Pow(lng2 - lng1, 2.0);
            return Math.Sqrt(p1 + p2);
        }
    }
}
