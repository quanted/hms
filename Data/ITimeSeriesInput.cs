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
    }

    /// <summary>
    /// Concrete class for timeseries inputs.
    /// </summary>
    public class TimeSeriesInput : ITimeSeriesInput
    {
        /// <summary>
        /// REQUIRED: Data source of the timeseries.
        /// </summary>
        [Required] 
        public string Source { get; set; }

        /// <summary>
        /// REQUIRED: Contains a start date and end date for the timeseries request.
        /// </summary>
        [Required]
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
            if (input.Geometry == null)
            {
                errorMsg += "ERROR: No geometry values found in the provided parameters.";
                return null;
            }
            // Validates that the Latitude parameter is not invalid
            if (!input.Source.Contains("ncdc") && !input.Source.Contains("compare"))
            {
                if (input.Geometry.Point == null)
                {
                    errorMsg += "ERROR: No geometry values found in the provided parameters.";
                    return null;
                }

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
            }
            else
            {
                if (!input.Geometry.GeometryMetadata.ContainsKey("stationID"))
                {
                    errorMsg += "ERROR: " + input.Source + " used as source but no stationID value was found in Geometry.GeometryMetadata.";
                }

                IPointCoordinate pC = new PointCoordinate()
                {
                    Latitude = 0.0,
                    Longitude = 0.0
                };
                newInput.Geometry = new TimeSeriesGeometry() { Point = (PointCoordinate)pC };
            }

            newInput.Geometry.GeometryMetadata = input.Geometry.GeometryMetadata ?? new Dictionary<string, string>();
            newInput.Geometry.Description = input.Geometry.Description ?? "";
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
            if (!errorMsg.Contains("ERROR"))
            {
                if (DateTime.Compare(newInput.DateTimeSpan.StartDate, newInput.DateTimeSpan.EndDate) >= 0)
                {
                    errorMsg += "ERROR: Start date must be before end date.";
                }
            }


            // Validates DateTime output format
            newInput.DateTimeSpan.DateTimeFormat = (String.IsNullOrWhiteSpace(input.DateTimeSpan.DateTimeFormat)) ? "yyyy-MM-dd HH": input.DateTimeSpan.DateTimeFormat;
            // TODO: Add validation of the given datetimeformat. If not valid set to default, TBD

            // Validates the DataValueFormat parameter
            newInput.DataValueFormat = (String.IsNullOrWhiteSpace(input.DataValueFormat)) ? "E3" : input.DataValueFormat;
            // TODO: Add validation of the given datavalueformat. If not valid set to default, TBD

            // Validates TemporalResolution parameter
            newInput.TemporalResolution = (String.IsNullOrWhiteSpace(input.TemporalResolution)) ? "default" : input.TemporalResolution;
            // TODO: Add validation of the provided temporalresolution. If not valid set to default

            // Validates TimeLocalized parameter
            newInput.TimeLocalized = (input.TimeLocalized == true) ? true : false;
            // TODO: Add validation of the provided timelocalized. If not valid set to true

            // Validates Units parameter
            newInput.Units = (String.IsNullOrWhiteSpace(input.Units)) ? "metric" : input.Units;
            // TODO: Add validation of the provided units. If not valid set to "metric"

            // Validates OutputFormat parameter
            newInput.OutputFormat = (String.IsNullOrWhiteSpace(input.OutputFormat)) ? "json" : input.OutputFormat;
            // TODO: Add validation of the provided output. If not valid set to "json"

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