using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Data
{
    /// <summary>
    /// Interface for timeseries inputs.
    /// </summary>
    public interface ITimeSeriesInput
    {
        /// <summary>
        /// Data source for the timeseries.
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// Gets date object for time series.
        /// </summary>
        DateTimeSpan DateTimeSpan { get; set; }

        /// <summary>
        /// Gets geometry object for time series.
        /// </summary>
        TimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// Specifies output format for the data values.
        /// Format Reference: https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
        /// </summary>
        string DataValueFormat { get; set; }

        /// <summary>
        /// The temporal resolution of the time series to be returned.
        /// </summary>
        string TemporalResolution { get; set; }

        /// <summary>
        /// Indicates whether the output time values are used for the timezone of the input geometry.
        /// </summary>
        bool TimeLocalized { get; set; }

        /// <summary>
        /// Unit system applied to output.
        /// </summary>
        string Units { get; set; }

        /// <summary>
        /// Indicates the type of output for the given spatial data.
        /// Default: Return all datapoints with no aggregation.
        /// Options: 1. Return a subset of all datapoints (for handling large input geometries)
        ///          2. Type of spatial aggregation (total of area, average of area... )
        /// </summary>
        //string SpatialAggregation { get; set; }

        /// <summary>
        /// Specifies output format type.
        /// JSON, XML... other.
        /// </summary>
        string OutputFormat { get; set; }
    }

    /// <summary>
    /// Concrete class for timeseries inputs.
    /// </summary>
    public class TimeSeriesInput : ITimeSeriesInput
    {
        /// <summary>
        /// Data source for the timeseries.
        /// </summary>
        [Required] 
        public string Source { get; set; }

        /// <summary>
        /// Gets date object for time series.
        /// </summary>
        [Required]
        public DateTimeSpan DateTimeSpan { get; set; }

        /// <summary>
        /// Gets geometry object for time series.
        /// </summary>
        public TimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// Specifies output format for the data values.
        /// Format Reference: https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
        /// </summary>
        public string DataValueFormat { get; set; }

        /// <summary>
        /// The temporal resolution of the time series to be returned.
        /// </summary>
        public string TemporalResolution { get; set; }

        /// <summary>
        /// Indicates whether the output time values are used for the timezone of the input geometry.
        /// </summary>
        public bool TimeLocalized { get; set; }

        /// <summary>
        /// Unit system applied to output.
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// Indicates the type of output for the given spatial data.
        /// Default: Return all datapoints with no aggregation.
        /// Options: 1. Return a subset of all datapoints (for handling large input geometries)
        ///          2. Type of spatial aggregation (total of area, average of area... )
        /// </summary>
        // public string SpatialAggregation { get; set; }

        /// <summary>
        /// Specifies output format type.
        /// JSON, XML... other.
        /// </summary>
        public string OutputFormat { get; set; }
    }


    // ----------------- TimeSeriesInputFactory Object ----------------- //

    /// <summary>
    /// Creator abstract class for TimeSeriesInputFactory
    /// </summary>
    public abstract class ITimeSeriesInputFactory
    {
        /// <summary>
        /// TimeSeriesInput setter abstract function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public abstract ITimeSeriesInput SetTimeSeriesInput(ITimeSeriesInput input, out string errorMsg);
    }

    /// <summary>
    /// Creator concrete class for TimeSeriesInputFactory
    /// </summary>
    public class TimeSeriesInputFactory : ITimeSeriesInputFactory
    {
        /// <summary>
        /// TimeSeriesInputFactory function for validating and setting TimeSeriesInput objects.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public override ITimeSeriesInput SetTimeSeriesInput(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            TimeSeriesInput newInput = new TimeSeriesInput();

            // Below preforms validation of required parameters when attempting to initialize dataset component inputs.

            // Validates that the source string is not null or empty.
            if (String.IsNullOrWhiteSpace(input.Source))
            {
                errorMsg += "ERROR: Required 'Source' parameter was not found or is invalid.";
            }
            else
            {
                newInput.Source = input.Source;
            }

            // Validating Geometry object
            // Validates that the Latitude parameter is not invalid
            if (Double.IsNaN(input.Geometry.Point.Latitude))
            {
                errorMsg += "ERROR: Required 'Latitude' parameter was not found or is invalid.";
            }
            // Validates that the Longitude parameter is not invalid
            if (Double.IsNaN(input.Geometry.Point.Longitude))
            {
                errorMsg += "ERROR: Required 'Latitude' parameter was not found or is invalid.";
            }
            if (!errorMsg.Contains("Latitude") || !errorMsg.Contains("Longitude"))
            {
                if (input.Geometry.Point.Latitude > -90 && input.Geometry.Point.Latitude < 90 &&
                input.Geometry.Point.Longitude > -180 && input.Geometry.Point.Longitude < 180)
                {
                    IPointCoordinate pC = new PointCoordinate()
                    {
                        Latitude = input.Geometry.Point.Latitude,
                        Longitude = input.Geometry.Point.Longitude
                    };
                    newInput.Geometry = new TimeSeriesGeometry()
                    {
                        Point = (PointCoordinate)pC
                    };
                    
                }
                else
                {
                    errorMsg += "ERROR: Latitude or Longitude value is not a valid coordinate.";
                }
            }

            newInput.Geometry.GeometryMetadata = (input.Geometry.GeometryMetadata == null) ? new Dictionary<string, string>() : input.Geometry.GeometryMetadata;
            newInput.Geometry.Description = (input.Geometry.Description == null) ? "" : input.Geometry.Description;
            // Validates and sets Timezone information
            if (input.Geometry.Timezone == null)
            {
                newInput.Geometry.Timezone = new Timezone()
                {
                    Name = "",
                    Offset = 0.0,
                    DLS = false
                };
            }
            else
            {
                newInput.Geometry.Timezone = new Timezone() { };
                newInput.Geometry.Timezone.Name = (String.IsNullOrWhiteSpace(input.Geometry.Timezone.Name)) ? "TZNotSet" : input.Geometry.Timezone.Name;
                newInput.Geometry.Timezone.Offset = (Double.IsNaN(input.Geometry.Timezone.Offset)) ? 0.0 : input.Geometry.Timezone.Offset;
                newInput.Geometry.Timezone.DLS = (input.Geometry.Timezone.DLS == true) ? true : false;
            }
            // Validates that the StartDate parameter is not invalid
            if (input.DateTimeSpan.StartDate.Equals(DateTime.MinValue))
            {
                errorMsg += "ERROR: Required 'StartDate' parameter was not found or is invalid.";
            }
            // Validates that the DndDate parameter is not invalid
            if (input.DateTimeSpan.EndDate.Equals(DateTime.MinValue))
            {
                errorMsg += "ERROR: Required 'EndDate' parameter was not found or is invalid.";
            }
            if (!errorMsg.Contains("StartDate") || !errorMsg.Contains("EndDate"))
            {
                newInput.DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = input.DateTimeSpan.StartDate,
                    EndDate = input.DateTimeSpan.EndDate
                };
            }
            // Validates DateTime output format
            newInput.DateTimeSpan.DateTimeFormat = (String.IsNullOrWhiteSpace(input.DateTimeSpan.DateTimeFormat)) ? "yyyy-MM-dd HH": input.DateTimeSpan.DateTimeFormat;
            // TODO: Add validation of the given format.

            // Validates the DataValueFormat parameter
            newInput.DataValueFormat = (String.IsNullOrWhiteSpace(input.DataValueFormat)) ? "default" : input.DataValueFormat;
            // TODO: Add validation of the given format.

            // Validates TemporalResolution parameter
            newInput.TemporalResolution = (String.IsNullOrWhiteSpace(input.TemporalResolution)) ? "default" : input.TemporalResolution;
            // TODO: Add validation of the provided value.

            // Validates TimeLocalized parameter
            newInput.TimeLocalized = (input.TimeLocalized == true) ? true : false;

            // Validates Units parameter
            newInput.Units = (String.IsNullOrWhiteSpace(input.Units)) ? "metric" : input.Units;
            // TODO: Add validation of the provided value.

            // Validates OutputFormat parameter
            newInput.OutputFormat = (String.IsNullOrWhiteSpace(input.OutputFormat)) ? "json" : input.OutputFormat;
            // TODO: Add validation of the provided value.

            return newInput;
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
        /// Description of the geometry, used to indicate details about the type of location the point represents.
        /// </summary>
        string Description { get; set; }

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
        /// Lat/lon point for when a coordinates are used as the geometry type.
        /// </summary>
        [Required]
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