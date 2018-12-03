using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Delineation Input that implements TimeSeriesInput object.
    /// </summary>
    public class WatershedDelineationInput : TimeSeriesInput
    {

    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle DelineationCompare POST request example
    /// </summary>
    public class WatershedDelineationInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WatershedDelineationInput example = new WatershedDelineationInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Description = "EPA Athens Office",
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "City", "Athens" },
                        { "State", "Georgia"},
                        { "Country", "United States" },
                        { "huc_12_num", "030502040102" }
                    },
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json"
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbucle DelineationCompare Output example
    /// </summary>
    public class WatershedDelineationOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Precipitation";
            output.DataSource = "ncdc, nldas, gldas, daymet";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_prod_name", "NLDAS_FORA0125_H.002" },
                { "nldas_param_short_name", "SSRUNsfc" },
                { "nldas_param_name", "Surface runoff (non-infiltrating)" },
                { "nldas_unit", "kg/m^2" },
                { "nldas_undef", "  9.9990e+20" },
                { "nldas_begin_time", "2015/01/01/00" },
                { "nldas_end_time", "2015/01/01/05" },
                { "nldas_time_interval[hour]", "1" },
                { "nldas_tot_record", "5" },
                { "nldas_grid_y", "71" },
                { "nldas_grid_x", "333" },
                { "nldas_elevation[m]", "219.065796" },
                { "nldas_dlat", "0.125000" },
                { "nldas_dlon", "0.125000" },
                { "nldas_ydim(original data set)", "224" },
                { "nldas_xdim(original data set)", "464" },
                { "nldas_start_lat(original data set)", "  25.0625" },
                { "nldas_start_lon(original data set)", "-124.9375" },
                { "nldas_Last_update", "Fri Jun  2 15:41:10 2017" },
                { "nldas_begin_time_index", "315563" },
                { "nldas_end_time_index", "315731" },
                { "nldas_lat", "  33.9375" },
                { "nldas_lon", " -83.3125" },
                { "nldas_Request_time", "Fri Jun  2 20:00:24 2017" }
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2010-04-08 000", new List<string>() { "10548330", "1.2207", "0.5263", "0.00642458", "33.8125", "-81.5625" } },
                { "2010-04-08 001", new List<string>() { "10548332", "1.2207", "0.05534", "0.00067557", "33.8125", "-81.5625" } },
                { "2010-04-08 002", new List<string>() { "10548346", "1.2207", "0.72801", "0.00888676", "33.8125", "-81.5625" } },
                { "2010-04-08 003", new List<string>() { "10548414", "1.2207", "0.66324", "0.0080962", "33.8125", "-81.5625" } },
                { "2010-04-08 004", new List<string>() { "10548428", "1.2207", "0.08131", "0.00099251", "33.8125", "-81.5625" } },
                { "2010-04-08 005", new List<string>() { "10548440", "1.2207", "0.00673", "8.215E-05", "33.8125", "-81.5625" } }
            };
            return output;
        }
    }


    // --------------- DelineationCompare Controller --------------- //

    /// <summary>
    /// DelineationCompare controller for HMS.
    /// </summary>
    [Route("api/hydrology/delineation/")]
    public class WSWatershedDelineationController : Controller
    {
        /// <summary>
        /// POST Method for getting DelineationCompare data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="watershedInput">Parameters for retrieving DelineationCompare data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("")]             // Default endpoint
        [Route("v1.0")]         // Version 1.0 endpoint
        [SwaggerRequestExample(typeof(WatershedDelineationInput), typeof(WatershedDelineationInputExample))]
        [SwaggerResponseExample(200, typeof(WatershedDelineationOutputExample))]
        public async Task<IActionResult> POST([FromBody]WatershedDelineationInput watershedInput)
        {
            WSWatershedDelineation watershed = new WSWatershedDelineation();
            ITimeSeriesOutput results = await watershed.GetDelineationData(watershedInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
