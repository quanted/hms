using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// WorkFlow Input that implements TimeSeriesInput object.
    /// </summary>
    public class WorkFlowCompareInput : TimeSeriesInput
    {
        /// <summary>
        /// Specified dataset for the workflow
        /// </summary>
        [Required]
        public string Dataset { get; set; }

        /// <summary>
        /// List of sources for the workflow.
        /// </summary>
        [Required]
        public List<string> SourceList { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle WorkFlowCompare POST request example
    /// </summary>
    public class WorkFlowCompareInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WorkFlowCompareInput example = new WorkFlowCompareInput()
            {
                Dataset = "Precipitation",
                SourceList = new List<string>()
                {
                    { "ncdc" },
                    { "nldas" },
                    { "gldas" },
                    { "daymet" }
                },
                Source = "compare",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "stationID", "GHCND:USW00013874" }
                    },
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = true
                    }
                },
                TimeLocalized = true
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbucle WorkFlowCompare Output example
    /// </summary>
    public class WorkFlowCompareOutputExample : IExamplesProvider
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
                {"ncdc_elevation", "307.8"},
                {"ncdc_mindate", "1930-01-01"},
                {"ncdc_maxdate", "2017-06-07"},
                {"ncdc_latitude", "33.6301"},
                {"ncdc_name", "ATLANTA HARTSFIELD INTERNATIONAL AIRPORT}, GA US"},
                {"ncdc_datacoverage", "1"},
                {"ncdc_id", "GHCND,USW00013874"},
                {"ncdc_elevationUnit", "METERS"},
                {"ncdc_longitude", "-84.4418"},
                {"ncdc_temporalResolution", "daily"},
                {"ncdc_units", "mm"},
                {"ncdc_timeZone", "EST"},
                {"ncdc_tz_offset", "-5"},
                {"column_3", "nldas"},
                {"nldas_prod_name", "NLDAS_FORA0125_H.002"},
                {"nldas_param_short_name", "APCPsfc"},
                {"nldas_param_name", "Precipitation hourly total"},
                {"nldas_unit", "kg/m^2"},
                {"nldas_undef", "  9.9990e+20"},
                {"nldas_begin_time", "2010/01/01/05"},
                {"nldas_end_time", "2010/01/11/04"},
                {"nldas_time_interval[hour]", "1"},
                {"nldas_tot_record", "240"},
                {"nldas_grid_y", "69"},
                {"nldas_grid_x", "324"},
                {"nldas_elevation[m]", "293.524200"},
                {"nldas_dlat", "0.125000"},
                {"nldas_dlon", "0.125000"},
                {"nldas_ydim(original data set)", "224"},
                {"nldas_xdim(original data set)", "464"},
                {"nldas_start_lat(original data set)", "  25.0625"},
                {"nldas_start_lon(original data set)", "-124.9375"},
                {"nldas_Last_update", "Wed Jun 14 15,41,26 2017"},
                {"nldas_begin_time_index", "271744"},
                {"nldas_end_time_index", "271983"},
                {"nldas_lat", "  33.6875"},
                {"nldas_lon", " -84.4375"},
                {"nldas_Request_time", "Wed Jun 14 21,05,16 2017"},
                {"nldas_temporalresolution", "daily"},
                {"nldas_timeZone", "EST"},
                {"nldas_tz_offset", "-5"},
                {"column_4", "gldas"},
                {"gldas_prod_name", "GLDAS_NOAH025_3H.001"},
                {"gldas_param_short_name", "precip"},
                {"gldas_param_name", "Precipitation rate"},
                {"gldas_unit", "kg/m^2/hr"},
                {"gldas_undef", "  9.9990e+20"},
                {"gldas_begin_time", "2010/01/01/05"},
                {"gldas_end_time", "2010/01/11/04"},
                {"gldas_time_interval[hour]", "3"},
                {"gldas_tot_record", "81"},
                {"gldas_grid_y", "374"},
                {"gldas_grid_x", "382"},
                {"gldas_elevation[m]", "276.221100"},
                {"gldas_dlat", "0.250000"},
                {"gldas_dlon", "0.250000"},
                {"gldas_ydim(original data set)", "600"},
                {"gldas_xdim(original data set)", "1440"},
                {"gldas_start_lat(original data set)", " -59.8750"},
                {"gldas_start_lon(original data set)", "-179.8750"},
                {"gldas_Last_update", "Sat Jan 14 07,30,13 2017"},
                {"gldas_begin_time_index", "28793"},
                {"gldas_end_time_index", "28873"},
                {"gldas_lat", "  33.6250"},
                {"gldas_lon", " -84.3750"},
                {"gldas_Request_time", "Wed Jun 14 21,05,16 2017"},
                {"gldas_temporalresolution", "daily"},
                {"gldas_timeZone", "EST"},
                {"gldas_tz_offset", "-5"},
                {"column_5", "daymet"},
                {"daymet_Latitude", "33.6301"},
                {"daymet_Longitude", "-84.4418"},
                {"daymet_X & Y on Lambert Conformal Conic", "1387382.75 -813758.01"},
                {"daymet_Tile", "11028"},
                {"daymet_Elevation", "302 meters"},
                {"daymet_url_reference,", "How to cite, Thornton; P.E.; M.M. Thornton; B.W. Mayer; Y. Wei; R. Devarakonda; R.S. Vose; and R.B. Cook. 2016. Daymet, Daily Surface Weather Data on a 1-km Grid for North America; Version 3. ORNL DAAC; Oak Ridge; Tennessee; USA. http,//dx.doi.org/10.3334/ORNLDAAC/1328"},
                {"daymet_unit", "mm"},
                {"daymet_temporalresolution", "daily"},
                {"daymet_timeZone", "EST"},
                {"daymet_tz_offset", "-5"},
                {"column_1", "date"},
                {"column_2", "ncdc"},
                {"ncdc_gore", "0.244015309955546"},
                {"ncdc_average", "0.166666666666667"},
                {"ncdc_standard_deviation", "0.471404520791032"},
                {"nldas_gore", "0.672765848579113"},
                {"nldas_average", "0.0750555555555556"},
                {"nldas_standard_deviation", "0.166841462413415"},
                {"nldas_ncdc_gore", "0.805642410981871"},
                {"gldas_gore", "0.940240702844843"},
                {"gldas_average", "0.233811111111111"},
                {"gldas_standard_deviation", "0.361684105399678"},
                {"gldas_ncdc_gore", "0.282008659975559"},
                {"daymet_gore", "0.296650173373953"},
                {"daymet_average", "0.222222222222222"},
                {"daymet_standard_deviation", "0.415739709641549"},
                {"daymet_ncdc_gore", "-0.75"}
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2010-01-01 00", new List<string>() { "0.000E+000", "1.342E-001", "3.577E-001", "1.000E+000" } },
                { "2010-01-02 00", new List<string>() { "0.000E+000", "0.000E+000", "0.000E+000",  "0.000E+000" } },
                { "2010-01-03 00", new List<string>() { "0.000E+000", "0.000E+000", "0.000E+000",  "0.000E+000" } },
                { "2010-01-04 00", new List<string>() { "0.000E+000", "0.000E+000", "0.000E+000",  "0.000E+000" } },
                { "2010-01-05 00", new List<string>() { "0.000E+000", "0.000E+000", "0.000E+000",  "0.000E+000" } },
                { "2010-01-06 00", new List<string>() { "0.000E+000", "0.000E+000", "0.000E+000",  "0.000E+000" } },
            };
            return output;
        }
    }


    // --------------- WorkFlowCompare Controller --------------- //

    /// <summary>
    /// WorkFlowCompare controller for HMS.
    /// </summary>
    [Route("api/workflow/compare")]
    public class WSWorkFlowCompareController : Controller
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
        [SwaggerRequestExample(typeof(WorkFlowCompareInput), typeof(WorkFlowCompareInputExample))]
        [SwaggerResponseExample(200, typeof(WorkFlowCompareOutputExample))]
        public ITimeSeriesOutput POST([FromBody]WorkFlowCompareInput workflowInput)
        {

            WSWorkFlow workFlow = new WSWorkFlow();
            ITimeSeriesOutput results = workFlow.GetWorkFlowData(workflowInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return results;
        }
    }
}
