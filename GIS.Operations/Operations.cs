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

        public static double CalculateRadialDistance(double lat1, double lng1, double lat2, double lng2, bool km=false)
        {
            // returns distance in meters
            double r = 6371e3;
            double t1 = lat1 * Math.PI / 180.0;
            double t2 = lat2 * Math.PI / 180.0;
            double dt = (lat2 - lat1) * Math.PI / 180.0;
            double g = (lng2 - lng1) * Math.PI / 180.0;
            double a = Math.Sin(dt / 2.0) * Math.Sin(dt / 2.0) + Math.Cos(t1) * Math.Cos(t2) * Math.Sin(g / 2.0) * Math.Sin(g / 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            double d = r * c;
            if (km)
            {
                d = d / 1000.0;
            }
            return d;
        }
    }
}
