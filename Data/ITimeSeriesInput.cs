using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

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
        /// VALUES: "default", "hourly", "daily", "monthly"
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
        /// Description: Time-series data source;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas", "ncei", "daymet", "prism", "trmm", "nwm", "nwis"];
        /// Required: True;        
        /// </summary>
        [Required]
        public string Source { get; set; }

        /// <summary>
        /// Description: The start and end date for the requested time-series;
        /// Default: None;
        /// Options: None;
        /// Required: True;
        /// </summary>
        [Required]   
        public DateTimeSpan DateTimeSpan { get; set; }

        /// <summary>
        /// Description: The spatial area of interest of the requested time-series. One geometry identifier is required.;
        /// Default: None;
        /// Options: None;
        /// Required: True;
        /// </summary>
        [Required]
        public TimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// Description: Specifies the output format for the data values in the timeseries;
        /// Default: "E3";
        /// Options: https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx ;
        /// Required: False;
        /// </summary>
        public string DataValueFormat { get; set; }

        /// <summary>
        /// Description: The temporal resolution of the time series to be returned. Valid options dependent on the dataset and source of the timeseries.;
        /// Default: "default";
        /// Options: ["default", "hourly", "3hourly", "daily", "monthly"];
        /// Required: False;
        /// </summary>
        public string TemporalResolution { get; set; }

        /// <summary>
        /// Description: Indicates if the timezone of the geometry is used for the date/time values of the returned timeseries.;
        /// Default: True;
        /// Options: [True, False];
        /// Required: False;
        /// </summary>
        public bool TimeLocalized { get; set; }

        /// <summary>
        /// Description: Unit system of the output values.;
        /// Default: "metric";
        /// Options: ["metric", "imperial"];
        /// Required: False;
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// Description: Specifies output format type.;
        /// Default: "json";
        /// Options: "json";
        /// Required: False;
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// Internal: Holds base url for data retrieval depending on the specified source and dataset.
        /// </summary>
        public List<string> BaseURL { get; set; }

        //------------------------//
        /// <summary>
        /// Description: A dictionary for utilizing an ITimeSeriesOutput as an input variable, where the key is a provided identifier of the ITimeSeriesOutput.;
        /// Default: None;
        /// Options: None;
        /// Required: False;
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
    /// DateTimeSpan object for specifying the temporal timespan of the returned timeseries data.
    /// </summary>
    public class DateTimeSpan : IDateTimeSpan
    {
        /// <summary>
        /// Description: The start date for the requested time-series.;
        /// Default: None;
        /// Options: None;
        /// Required: True;        
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Description: The end date for the requested time-series.;
        /// Default: None;
        /// Options: None;
        /// Required: True;        
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Description: Format for the output of DateTime values.;
        /// Default: "yyyy-MM-dd HH;
        /// Options: https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx;
        /// Required: False;        
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
        /// Description: Time zone.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Description: Timezone offset from GMT.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        double Offset { get; set; }

        /// <summary>
        /// Description: Indicates if day light savings is active or not. Not currently used.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        bool DLS { get; set; }
    }

    /// <summary>
    /// Timezone information corresponding to the geospatial area of interest, specified within the Geometry object.
    /// </summary>
    public class Timezone : ITimezone
    {
        /// <summary>
        /// Description: Time zone.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description: Timezone offset from GMT.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Description: Indicates if day light savings is active or not. Not currently used.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
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
    /// The geospatial area of interest object for the returned time series data. 
    /// </summary>
    public class TimeSeriesGeometry : ITimeSeriesGeometry
    {

        /// <summary>
        /// Description: Description of the geometry, used to indicate details about the type of location the point represents.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Description: NHDPlus v2 catchment identifier.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public int ComID { get; set; }

        /// <summary>
        /// Description: NHDPlus v2 Hydrologic Unit Code idendifier, specifically a HUC8 or HUC12 ID.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public string HucID { get; set; }

        /// <summary>
        /// Description: NCEI weather station ID, supports GHCND and COOP stations. If station type is not prepended to the ID, assumed to be GHCND.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// Description: Point coordinate object for providing latitude and longitude coordinates.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public PointCoordinate Point { get; set; }

        /// <summary>
        /// Description: Dictionary for holding metadata and additional information about the provided geometry. ;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
        /// </summary>
        public Dictionary<string, string> GeometryMetadata { get; set; }

        /// <summary>
        /// Description: Timezone information for the input geometry.;
        /// Default: None;
        /// Options: None;
        /// Required: False;        
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
    /// Point geometry object for specifying a point geospatial area of interest.
    /// </summary>
    public class PointCoordinate : IPointCoordinate
    {

        /// <summary>
        /// Description: Latitude value of point geometry.;
        /// Default: None;
        /// Options: [-90.0,90.0];
        /// Required: False;        
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Description: Longitude value of point geometry.;
        /// Default: None;
        /// Options: [-180.0, 180.0];
        /// Required: False;        
        /// </summary>
        public double Longitude { get; set; }
    }
}