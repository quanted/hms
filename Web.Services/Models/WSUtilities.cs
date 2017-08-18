using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    /// <summary>
    /// Web Service Utilities Class
    /// </summary>
    public class WSUtilities
    {
        
        /// <summary>
        /// Checks the data endpoints for the precipitation component.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> CheckPrecipEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<Precipitation.Precipitation> precips = new List<Precipitation.Precipitation>();
            List<string> sources = new List<string>() { "nldas", "gldas", "ncdc", "daymet" };
            ITimeSeriesInput testInput = new TimeSeriesInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2005, 01, 01),
                    EndDate = new DateTime(2005, 01, 05)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                }
            };
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            foreach(string source in sources)
            {
                Precipitation.Precipitation precip = new Precipitation.Precipitation();
                testInput.Source = source;
                precip.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "precip" }, out string errorMsg);
                if (source.Contains("ncdc"))
                {
                    precip.Input.Geometry.GeometryMetadata["stationID"] = "GHCND:USW00013874";
                    precip.Input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                }
                precips.Add(precip);
            }

            Parallel.ForEach(precips, (Precipitation.Precipitation precip) =>
            {
                endpoints.Add(precip.Input.Source, precip.CheckEndpointStatus());
            });         
            return endpoints;
        }
    }
}