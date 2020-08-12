using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Drawing.Charts;

namespace Web.Services.Models
{
    /// <summary>
    /// Web Service Utilities Class
    /// </summary>
    public class WSUtilities
    {

        /// <summary>
        /// Checks the external data endpoints
        /// </summary>
        /// <returns></returns>
        public async static Task<Dictionary<string, bool>> ServiceCheck()
        {
            Dictionary<string,bool> endpoints = new Dictionary<string, bool>();
            List<Precipitation.Precipitation> precips = new List<Precipitation.Precipitation>();
            List<string> sources = new List<string>() { "nldas", "gldas", "ncei", "daymet", "prism", "trmm" };
            ITimeSeriesInput testInput = new TimeSeriesInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2005, 01, 01),
                    EndDate = new DateTime(2005, 12, 31)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    }
                },
                TemporalResolution = "daily"
            };
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            foreach (string source in sources)
            {
                Precipitation.Precipitation precip = new Precipitation.Precipitation();
                testInput.Source = source;
                precip.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "precipitation" }, out string errorMsg);
                if (source.Contains("ncei"))
                {
                    precip.Input.Geometry.GeometryMetadata["stationID"] = "GHCND:USW00013874";
                    precip.Input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                }
                precips.Add(precip);
            }


            Parallel.ForEach(precips, (Precipitation.Precipitation p) =>
            //foreach(Precipitation.Precipitation p in precips)
            {
                bool result = true;
                try
                {
                    ITimeSeriesOutput o = p.GetData(out string errorMsg, 9);
                    if (p.Output.Metadata.ContainsKey("ERROR") || p.Output.Data.Count < 365)
                    {
                        result = false;
                    }

                }
                catch (Exception)
                {
                    result = false;
                }
                endpoints.Add(p.Input.Source, result);
            }
            );
            return endpoints;
        }
    }
}