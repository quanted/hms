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
        /// Checks the data endpoints for the Precipitation component.
        /// </summary>
        /// <returns></returns>
        public async static Task<Dictionary<string, Dictionary<string, string>>> CheckPrecipEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<Precipitation.Precipitation> precips = new List<Precipitation.Precipitation>();
            List<string> sources = new List<string>() { "nldas", "gldas", "ncdc", "daymet"};
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
                precip.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "precipitation" }, out string errorMsg);
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

        /// <summary>
        /// Checks the data endpoints for the Evapotranspiration component.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, string>>> CheckEvapoEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<Evapotranspiration.Evapotranspiration> evapos = new List<Evapotranspiration.Evapotranspiration>();
            List<string> sources = new List<string>() { "nldas", "gldas" };
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
            foreach (string source in sources)
            {
                Evapotranspiration.Evapotranspiration evapo = new Evapotranspiration.Evapotranspiration();
                testInput.Source = source;
                evapo.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "evapotranspiration" }, out string errorMsg);
                evapos.Add(evapo);
            }

            Parallel.ForEach(evapos, (Evapotranspiration.Evapotranspiration evapo) =>
            {
                endpoints.Add(evapo.Input.Source, evapo.CheckEndpointStatus());
            });
            return endpoints;
        }

        /// <summary>
        /// Checks the data endpoints for the Soil Moisture component.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, string>>> CheckSoilMEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<SoilMoisture.SoilMoisture> soils = new List<SoilMoisture.SoilMoisture>();
            List<string> sources = new List<string>() { "nldas", "gldas" };
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
            foreach (string source in sources)
            {
                string[] validLayers = (source.Contains("nldas")) ? new string[] { "0-10", "10-40", "40-100", "100-200", "0-100", "0-200" } : new string[] { "0-10", "10-40", "40-100", "0-100" };
                foreach (string layer in validLayers)
                {
                    SoilMoisture.SoilMoisture soil = new SoilMoisture.SoilMoisture();
                    testInput.Source = source;
                    string l = layer.Replace('-', '_') + "_soilmoisture";
                    soil.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { l }, out string errorMsg);
                    soil.Input.Source = source + "_" + l;
                    soils.Add(soil);
                }
            }

            Parallel.ForEach(soils, (SoilMoisture.SoilMoisture soil) =>
            {
                endpoints.Add(soil.Input.Source, soil.CheckEndpointStatus());
            });
            return endpoints;
        }

        /// <summary>
        /// Checks the data endpoints for the Sub-Surface Flow component.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, string>>> CheckSubsurfaceEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<SubSurfaceFlow.SubSurfaceFlow> subsurfaces = new List<SubSurfaceFlow.SubSurfaceFlow>();
            List<string> sources = new List<string>() { "nldas", "gldas" };
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
            foreach (string source in sources)
            {
                SubSurfaceFlow.SubSurfaceFlow subsurface = new SubSurfaceFlow.SubSurfaceFlow();
                testInput.Source = source;
                subsurface.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "subsurfaceflow" }, out string errorMsg);
                subsurfaces.Add(subsurface);
            }

            Parallel.ForEach(subsurfaces, (SubSurfaceFlow.SubSurfaceFlow subsurface) =>
            {
                endpoints.Add(subsurface.Input.Source, subsurface.CheckEndpointStatus());
            });
            return endpoints;
        }

        /// <summary>
        /// Checks the data endpoints for the SurfaceRunoff component.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, string>>> CheckRunoffEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<SurfaceRunoff.SurfaceRunoff> runoffs = new List<SurfaceRunoff.SurfaceRunoff>();
            List<string> sources = new List<string>() { "nldas", "gldas" };
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
            foreach (string source in sources)
            {
                SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
                testInput.Source = source;
                runoff.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "surfacerunoff" }, out string errorMsg);
                runoffs.Add(runoff);
            }

            Parallel.ForEach(runoffs, (SurfaceRunoff.SurfaceRunoff runoff) =>
            {
                endpoints.Add(runoff.Input.Source, runoff.CheckEndpointStatus());
            });
            return endpoints;
        }

        /// <summary>
        /// Checks the data endpoints for the Temperature component.
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<string, string>>> CheckTempEndpoints()
        {
            Dictionary<string, Dictionary<string, string>> endpoints = new Dictionary<string, Dictionary<string, string>>();
            List<Temperature.Temperature> temps = new List<Temperature.Temperature>();
            List<string> sources = new List<string>() { "nldas", "gldas", "daymet" };
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
            foreach (string source in sources)
            {
                Temperature.Temperature temp = new Temperature.Temperature();
                testInput.Source = source;
                temp.Input = iFactory.SetTimeSeriesInput(testInput, new List<string>() { "temperature" }, out string errorMsg);
                temps.Add(temp);
            }

            Parallel.ForEach(temps, (Temperature.Temperature temp) =>
            {
                endpoints.Add(temp.Input.Source, temp.CheckEndpointStatus());
            });
            return endpoints;
        }

    }
}