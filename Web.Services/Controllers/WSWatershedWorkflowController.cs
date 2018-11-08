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
    interface WorkflowSources
    {
        string RunoffSource { get; set; }
        string StreamHydrology { get; set; }
    }


    /// <summary>
    /// WorkFlow Input that implements TimeSeriesInput object.
    /// </summary>
    public class WatershedWorkflowInput : TimeSeriesInput, WorkflowSources
    {
        /// <summary>
        /// OPTIONAL: Specifies the requested source for Runoff Data
        /// </summary>
        public string RunoffSource { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies the requested Stream Hydrology Algorithm to use
        /// </summary>
        public string StreamHydrology { get; set; }

        /// <summary>
        /// OPTIONAL: States whether or not runoff should be aggregated by catchments
        /// </summary>
        public Boolean Aggregation { get; set; }
        // Add extra SurfaceRunoff specific variables here
    }

    /// <summary>
    /// WorkFlow Output object
    /// </summary>
    public class WatershedWorkflowOutput : TimeSeriesOutput
    {
        public Dictionary<int, Dictionary<string, ITimeSeriesOutput>> data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public new Dictionary<string, Dictionary<string, string>> table { get; set; }
        //public Dictionary<string, Dictionary<string, string>> table { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle WorkFlowCompare POST request example
    /// </summary>
    public class WatershedWorkflowInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WatershedWorkflowInput example = new WatershedWorkflowInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2014, 07, 01),
                    EndDate = new DateTime(2014, 07, 31),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    HucID = "030502040102",
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "precipSource", "daymet"}
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
                OutputFormat = "json",
                Aggregation = false,
                RunoffSource = "curvenumber",
                StreamHydrology = "constant"
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbucle WorkFlowCompare Output example
    /// </summary>
    public class WatershedWorkflowOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            WatershedWorkflowOutput wsoutput = new WatershedWorkflowOutput();
            wsoutput.Dataset = "Precipitation";
            wsoutput.DataSource = "ncdc, nldas, gldas, daymet";
            wsoutput.metadata = new Dictionary<string, string>()
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
            wsoutput.data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>()
            {
                { 9311817, new Dictionary<string, ITimeSeriesOutput>(){ { "Precip", output }, { "Subsurf", output }, { "Surface", output }, { "Stream Flow", output } } },
                { 9311819, new Dictionary<string, ITimeSeriesOutput>(){ { "Precip", output }, { "Subsurf", output }, { "Surface", output }, { "Stream Flow", output } } }
            };
            return wsoutput;
        }
    }


    // --------------- WorkFlowCompare Controller --------------- //

    /// <summary>
    /// WorkFlowCompare controller for HMS.
    /// </summary>
    [Route("api/workflow/watershed")]
    public class WSWatershedWorkflowController : Controller
    {
        /// <summary>
        /// POST Method for getting WorkFlowCompare data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="workflowInput">Parameters for retrieving WorkFlowCompare data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("")]             // Default endpoint
        [Route("v1.0")]         // Version 1.0 endpoint
        [SwaggerRequestExample(typeof(WatershedWorkflowInput), typeof(WatershedWorkflowInputExample))]
        [SwaggerResponseExample(200, typeof(WatershedWorkflowOutputExample))]
        public async Task<IActionResult> POST([FromBody]WatershedWorkflowInput workflowInput)
        {
            WSWatershedWorkFlow workFlow = new WSWatershedWorkFlow();
            ITimeSeriesOutput results = await workFlow.GetWorkFlowData(workflowInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}