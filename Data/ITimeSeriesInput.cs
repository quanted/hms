using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Data
{
    /// <summary>
    /// Interface for timeseries inputs.
    /// </summary>
    public interface ITimeSeriesInput
    {
        /// <summary>
        /// REQUIRED: Data source of the timeseries.
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// REQUIRED: Contains a start date and end date for the timeseries request.
        /// </summary>
        DateTimeSpan DateTimeSpan { get; set; }

        /// <summary>
        /// REQUIRED: Contains the point, latitude/longitude, for the timeseries request. Metadata may be provided for the geometry.
        /// </summary>
        TimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies the output format for the data values in the timeseries.
        /// DEFAULT: 
        /// Format Reference: https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
        /// </summary>
        string DataValueFormat { get; set; }

        /// <summary>
        /// OPTIONAL: The temporal resolution of the time series to be returned. Valid options dependent on the dataset and source of the timeseries.
        /// DEFAULT: "default"
        /// VALUES: "default", "hourly", "daily", "weekly", "monthly"
        /// </summary>
        string TemporalResolution { get; set; }

        /// <summary>
        /// OPTIONAL: Indicates if the timezone of the geometry is used for the date/time values of the timeseries.
        /// DEFAULT: True
        /// </summary>
        bool TimeLocalized { get; set; }

        /// <summary>
        /// OPTIONAL: Unit system of the output values.
        /// DEFAULT: "metric"
        /// VALUES: "metric", "imperial"
        /// </summary>
        string Units { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies output format type.
        /// DEFAULT: "json"
        /// VALUES: "json", "xml", "csv"
        /// </summary>
        string OutputFormat { get; set; }

        /// <summary>
        /// Internal: Base url for data retrieval depending on the specified source and dataset.
        /// </summary>
        List<string> BaseURL { get; set; }

        //------------------------//
        /// <summary>
        /// Optional: A dictionary for utilizing an ITimeSeriesOutput as an input variable, where the key is a provided identifier of the ITimeSeriesOutput.
        /// </summary>
        Dictionary<string, TimeSeriesOutput> InputTimeSeries { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        ITimeSeriesInput Clone(List<string> dataset);

    }


    /// <summary>
    /// Concrete class for timeseries inputs.
    /// </summary>
    public class TimeSeriesInput : ITimeSeriesInput
    {
        /// <summary>
        /// REQUIRED: Data source of the timeseries.
        /// </summary>
#if RUNNING_ON_4  // JSC 1/22/2018
        [Required]   
#endif
        public string Source { get; set; }

        /// <summary>
        /// REQUIRED: Contains a start date and end date for the timeseries request.
        /// </summary>
#if RUNNING_ON_4  // JSC 1/22/2018
        [Required]   
#endif
        public DateTimeSpan DateTimeSpan { get; set; }

        /// <summary>
        /// REQUIRED: Contains the point, latitude/longitude, for the timeseries request. Metadata may be provided for the geometry.
        /// </summary>
        public TimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies the output format for the data values in the timeseries.
        /// DEFAULT: 
        /// Format Reference: https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
        /// </summary>
        public string DataValueFormat { get; set; }

        /// <summary>
        /// OPTIONAL: The temporal resolution of the time series to be returned. Valid options dependent on the dataset and source of the timeseries.
        /// DEFAULT: "default"
        /// VALUES: "default", "hourly", "daily", "weekly", "monthly"
        /// </summary>
        public string TemporalResolution { get; set; }

        /// <summary>
        /// OPTIONAL: Indicates if the timezone of the geometry is used for the date/time values of the timeseries.
        /// DEFAULT: True
        /// </summary>
        public bool TimeLocalized { get; set; }

        /// <summary>
        /// OPTIONAL: Unit system of the output values.
        /// DEFAULT: "metric"
        /// VALUES: "metric", "imperial"
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies output format type.
        /// DEFAULT: "json"
        /// VALUES: "json", "xml", "csv"
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Internal: Holds base url for data retrieval depending on the specified source and dataset.
        /// </summary>
        public List<string> BaseURL { get; set; }

        //------------------------//
        /// <summary>
        /// Optional: A dictionary for utilizing an ITimeSeriesOutput as an input variable, where the key is a provided identifier of the ITimeSeriesOutput.
        /// </summary>
        public Dictionary<string, TimeSeriesOutput> InputTimeSeries { get; set; }

        /// <summary>
        /// Creates a new ITimeSeriesInput from the current object.
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesInput Clone(List<string> dataset)
        {
            string errorMsg = "";
            TimeSeriesInputFactory factory = new TimeSeriesInputFactory();
            TimeSeriesInput newInput = (TimeSeriesInput)factory.SetTimeSeriesInput(this, dataset, out errorMsg);
            return newInput;
        }

    }


    // ----------------- TimeSeriesInputFactory Object ----------------- //

    /// <summary>
    /// Creator abstract class for TimeSeriesInputFactory
    /// </summary>
    public abstract class ITimeSeriesInputFactory
    {

        /// <summary>
        /// Constructor for empty, non-null, ITimeSeriesInput, abstract function
        /// </summary>
        /// <returns></returns>
        public abstract ITimeSeriesInput Initialize();

        /// <summary>
        /// TimeSeriesInput setter abstract function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dataset"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public abstract ITimeSeriesInput SetTimeSeriesInput(ITimeSeriesInput input, List<string> dataset, out string errorMsg);
    }

    /// <summary>
    /// Creator concrete class for TimeSeriesInputFactory
    /// </summary>
    public class TimeSeriesInputFactory : ITimeSeriesInputFactory
    {
        /// <summary>
        /// Construct empty, non-null, ITimeSeriesInput
        /// </summary>
        /// <returns></returns>
        public override ITimeSeriesInput Initialize()
        {
            ITimeSeriesInput defaultInput = new TimeSeriesInput()
            {
                Source = "",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = DateTime.Today.AddDays(-1.0),
                    EndDate = DateTime.Today,
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    ComID = -1,
                    HucID = "-1",
                    StationID = "",
                    Point = new PointCoordinate()
                    {
                        Latitude = -9999,
                        Longitude = -9999,
                    },
                    Timezone = new Timezone
                    {
                        DLS = false,
                        Name = "",
                        Offset = 0
                    },
                    GeometryMetadata = new Dictionary<string, string>(),
                    Description = ""
                },
                TemporalResolution = "default",
                TimeLocalized = false,
                Units = "",
                BaseURL = new List<string>(),
                DataValueFormat = "E3",
                OutputFormat = "json",
                InputTimeSeries = null 
            };
            return defaultInput;
        }

        /// <summary>
        /// Refactored TimeseriesInput factory builder method.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dataset"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public override ITimeSeriesInput SetTimeSeriesInput(ITimeSeriesInput input, List<string> dataset, out string errorMsg)
        {
            errorMsg = "";
            TimeSeriesInput validatedInput = ITimeSeriesValidation.Validate(out errorMsg, dataset, input) as TimeSeriesInput;
            if (errorMsg.Contains("ERROR"))
            {
                Log.Warning(errorMsg);
                return input;
            }
            return validatedInput;
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static string GetBaseURL(string source, string dataset)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            if(File.Exists(@".\App_Data\" + "url_info.txt"))
            {
                urls = Data.Files.FileToDictionary(@".\App_Data\" + "url_info.txt");
            }
            else
            {
                urls = Data.Files.FileToDictionary("/app/App_Data/url_info.txt");
            }

            Dictionary<string, string> caselessUrls = new Dictionary<string, string>(urls, StringComparer.OrdinalIgnoreCase);
            string url_key = source.ToLower() + "_" + dataset.ToLower() + "_url";
            try
            {
                return caselessUrls[url_key];
            }
            catch
            {
                Log.Warning("ERROR: Unable to construct base url from dataset: {0}, and source: {1}.", dataset, source);
                return "";
            }
        }

    }


    // ----------------- DateTimeSpan Object ----------------- //

    /// <summary>
    /// DateTimeSpan interface for time series components.
    /// </summary>
    public interface IDateTimeSpan
    {
        /// <summary>
        /// Start date of the time series.
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the time series.
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// Format for the output of DateTime values.
        /// Format Reference: https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
        /// </summary>
        string DateTimeFormat { get; set; }
    }

    /// <summary>
    /// DateTimeSpan concrete class.
    /// </summary>
    public class DateTimeSpan : IDateTimeSpan
    {
        /// <summary>
        /// Start date of the time series.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the time series.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Format for the output of DateTime values.
        /// Format Reference: https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
        /// </summary>
        public string DateTimeFormat { get; set; }
    }


    // ----------------- Timezone Object ----------------- //

    /// <summary>
    /// Timezone interface for timezone information of input geometry.
    /// </summary>
    public interface ITimezone
    {
        /// <summary>
        /// Timezone name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Timezone offset from GMT.
        /// </summary>
        double Offset { get; set; }

        /// <summary>
        /// Indicates if day light savings is active or not.
        /// </summary>
        bool DLS { get; set; }
    }

    /// <summary>
    /// Timezone concrete class for timezone information of input geometry.
    /// </summary>
    public class Timezone : ITimezone
    {
        /// <summary>
        /// Timezone name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timezone offset from GMT.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Indicates if day light savings is active or not.
        /// </summary>
        public bool DLS { get; set; }
    }


    // ----------------- TimeSeriesGeometry Object ----------------- //

    /// <summary>
    /// Geometry interface for time series components.
    /// </summary>
    public interface ITimeSeriesGeometry
    {
        /// <summary>
        /// Description of the geometry type being provided. Valid descriptions: "Point", "HucID", "ComID", and "StationID" in that order.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Catchment comid
        /// </summary>
        int ComID { get; set; }

        /// <summary>
        /// ID for NHDPlus boundaries
        /// </summary>
        string HucID { get; set; }

        /// <summary>
        /// ID for NCDC Stations
        /// </summary>
        string StationID { get; set; }

        /// <summary>
        /// Lat/lon point for when a coordinates are used as the geometry type.
        /// </summary>
        PointCoordinate Point { get; }

        /// <summary>
        /// Dictionary for holding metadata and additional information about the provided geometry. 
        /// </summary>
        Dictionary<string, string> GeometryMetadata { get; set; }

        /// <summary>
        /// Timezone information for the input geometry.
        /// </summary>
        Timezone Timezone { get; set; }
    }

    /// <summary>
    /// TimeSeries Geometry concrete class for time series components.
    /// </summary>
    public class TimeSeriesGeometry : ITimeSeriesGeometry
    {
        /// <summary>
        /// Description of the geometry, used to indicate details about the type of location the point represents.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Catchment comid
        /// </summary>
        public int ComID { get; set; }

        /// <summary>
        /// ID for NHDPlus boundaries
        /// </summary>
        public string HucID { get; set; }

        /// <summary>
        /// ID for NCDC Stations
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// Lat/lon point for when a coordinates are used as the geometry type.
        /// </summary>
        /// 
#if RUNNING_ON_4  // JSC 1/22/2018
        [Required]   
#endif
        public PointCoordinate Point { get; set; }

        /// <summary>
        /// Dictionary for holding metadata and additional information about the provided geometry. 
        /// </summary>
        public Dictionary<string, string> GeometryMetadata { get; set; }

        /// <summary>
        /// Timezone information for the input geometry.
        /// </summary>
        public Timezone Timezone { get; set; }
    }


    // ----------------- PointCoordinate Object ----------------- //

    /// <summary>
    /// Point geometry object interface.
    /// </summary>
    public interface IPointCoordinate
    {
        /// <summary>
        /// Latitude value of point geometry.
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        /// Longitude value of point geometry.
        /// </summary>
        double Longitude { get; set; }
    }

    /// <summary>
    /// Point geometry object interface.
    /// </summary>
    public class PointCoordinate : IPointCoordinate
    {
        /// <summary>
        /// Latitude value of point geometry.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude value of point geometry.
        /// </summary>
        public double Longitude { get; set; }
    }
}