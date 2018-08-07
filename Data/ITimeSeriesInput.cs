using System;
using System.Collections;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public Dictionary<string, TimeSeriesOutput> InputTimeSeries { get; set; }

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
        /// TimeSeriesInputFactory function for validating and setting TimeSeriesInput objects.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dataset"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public override ITimeSeriesInput SetTimeSeriesInput(ITimeSeriesInput input, List<string> dataset, out string errorMsg)
        {
            errorMsg = "";
            TimeSeriesInput newInput = new TimeSeriesInput();

            // Below preforms validation of required parameters when attempting to initialize dataset component inputs.
            // TODO: Append error messages to array and output to errorMsg on return.
            List<string> errors = new List<string>();

            // Validates that the source string is not null or empty.
            if (String.IsNullOrWhiteSpace(input.Source))
            {
                errors.Add("ERROR: Required 'Source' parameter was not found or is invalid.");
                //errorMsg += "ERROR: Required 'Source' parameter was not found or is invalid.";
                //return newInput;
            }
            else
            {
                newInput.Source = input.Source;
            }

            // Validating Geometry object
            if (input.Geometry == null)
            {
                errors.Add("ERROR: No geometry values found in the provided parameters.");
                //errorMsg += "ERROR: No geometry values found in the provided parameters.";
                //return newInput;
            }
            else
            {
                // Validates that the Latitude parameter is not invalid
                if (!input.Source.Contains("ncdc") && !input.Source.Contains("compare"))
                {
                    if (input.Geometry.Point == null)
                    {
                        errors.Add("ERROR: No geometry values found in the provided parameters.");
                        //errorMsg += "ERROR: No geometry values found in the provided parameters.";
                        //return newInput;
                    }
                    else if (Double.IsNaN(input.Geometry.Point.Latitude))
                    {
                        errors.Add("ERROR: Required 'Latitude' parameter was not found or is invalid.");
                        //errorMsg += "ERROR: Required 'Latitude' parameter was not found or is invalid.";
                    }
                    // Validates that the Longitude parameter is not invalid
                    else if (Double.IsNaN(input.Geometry.Point.Longitude))
                    {
                        errors.Add("ERROR: Required 'Longitude' parameter was not found or is invalid.");
                        //errorMsg += "ERROR: Required 'Longitude' parameter was not found or is invalid.";
                    }

                    if (!(errors.Any(s => s.Contains("Latitude")) || errors.Any(s => s.Contains("Longitude")) || errors.Any(s => s.Contains("geometry"))))
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
                            IPointCoordinate pC = new PointCoordinate()
                            {
                                Latitude = 0,
                                Longitude = 0
                            };
                            newInput.Geometry = new TimeSeriesGeometry()
                            {
                                Point = (PointCoordinate)pC
                            };
                            errors.Add("ERROR: Latitude or Longitude value is not a valid coordinate.");
                        }
                    }
                    else
                    {
                        IPointCoordinate pC = new PointCoordinate()
                        {
                            Latitude = 0,
                            Longitude = 0
                        };
                        newInput.Geometry = new TimeSeriesGeometry()
                        {
                            Point = (PointCoordinate)pC
                        };
                        errors.Add("ERROR: Latitude or Longitude value is not a valid coordinate.");
                    }
                }
                else
                {
                    if (!input.Geometry.GeometryMetadata.ContainsKey("stationID"))
                    {
                        errors.Add("ERROR: " + input.Source + " used as source but no stationID value was found in Geometry.GeometryMetadata.");
                        //errorMsg += "ERROR: " + input.Source + " used as source but no stationID value was found in Geometry.GeometryMetadata.";
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
                    if (newInput.Geometry.Timezone.Offset > 12 || newInput.Geometry.Timezone.Offset < -12)
                    {
                        errors.Add("ERROR: Timezone offset value is not a valid timezone. Timezone offset provided: " + newInput.Geometry.Timezone.Offset.ToString());
                        //errorMsg += "ERROR: Timezone offset value is not a valid timezone. Timezone offset provided: " + newInput.Geometry.Timezone.Offset.ToString();
                    }
                    newInput.Geometry.Timezone.DLS = (input.Geometry.Timezone.DLS == true) ? true : false;
                }
            }

            if (input.DateTimeSpan == null)
            {
                errors.Add("ERROR: DateTimeSpan object is null. DateTimeSpan, with a StartDate and EndDate, is required.");
                //errorMsg += "ERROR: DateTimeSpan object is null. DateTimeSpan, with a StartDate and EndDate, is required.";
                //return newInput;
            }
            else
            {
                // Validates that the StartDate parameter is not invalid
                if (input.DateTimeSpan.StartDate.Equals(DateTime.MinValue))
                {
                    errors.Add("ERROR: Required 'StartDate' parameter was not found or is invalid.");
                    //errorMsg += "ERROR: Required 'StartDate' parameter was not found or is invalid.";
                }
                // Validates that the EndDate parameter is not invalid
                if (input.DateTimeSpan.EndDate.Equals(DateTime.MinValue))
                {
                    errors.Add("ERROR: Required 'EndDate' parameter was not found or is invalid.");
                    //errorMsg += "ERROR: Required 'EndDate' parameter was not found or is invalid.";
                }
                if (!errorMsg.Contains("StartDate") || !errorMsg.Contains("EndDate"))
                {
                    newInput.DateTimeSpan = new DateTimeSpan()
                    {
                        StartDate = input.DateTimeSpan.StartDate,
                        EndDate = input.DateTimeSpan.EndDate
                    };
                }
                if (DateTime.Compare(newInput.DateTimeSpan.StartDate, newInput.DateTimeSpan.EndDate) >= 0)
                {
                    errors.Add("ERROR: Start date must be before end date.");
                    //errorMsg += "ERROR: Start date must be before end date.";
                }

                // Validates DateTime output format
                newInput.DateTimeSpan.DateTimeFormat = (String.IsNullOrWhiteSpace(input.DateTimeSpan.DateTimeFormat)) ? "yyyy-MM-dd HH" : input.DateTimeSpan.DateTimeFormat;
                try
                {
                    string dateTest = newInput.DateTimeSpan.StartDate.ToString(newInput.DateTimeSpan.DateTimeFormat);
                }
                catch (FormatException fe)
                {
                    errors.Add("ERROR: Problem with the DateTimeFormat. Provided DateTimeFormat: " + newInput.DateTimeSpan.DateTimeFormat + ". Error Message: " + fe.Message);
                    //errorMsg += "ERROR: Problem with the DateTimeFormat. Provided DateTimeFormat: " + newInput.DateTimeSpan.DateTimeFormat + ". Error Message: " + fe.Message;
                }
            }


            // Validates the DataValueFormat parameter
            newInput.DataValueFormat = (String.IsNullOrWhiteSpace(input.DataValueFormat)) ? "E3" : input.DataValueFormat;
            try
            {
                double testValue = 12345.678901;
                string testString = testValue.ToString(newInput.DataValueFormat);
            }
            catch (FormatException fe)
            {
                errors.Add("ERROR: Problem with the DateValueFormat. Provded DataValueFormat: " + newInput.DataValueFormat + ". ErrorMessage: " + fe.Message);
                //errorMsg += "ERROR: Problem with the DateValueFormat. Provded DataValueFormat: " + newInput.DataValueFormat + ". ErrorMessage: " + fe.Message;
            }

            // Validates TemporalResolution parameter
            newInput.TemporalResolution = (String.IsNullOrWhiteSpace(input.TemporalResolution)) ? "default" : input.TemporalResolution.ToLower();
            string[] validTemporalResolutions = new string[] { "hourly", "daily", "weekly", "monthly", "seasonal", "yearly", "default" };
            // For non-uniform timeseries, leave as default.
            if (!Array.Exists(validTemporalResolutions, element => element == newInput.TemporalResolution))
            {
                newInput.TemporalResolution = "default";
            }

            // Validates TimeLocalized parameter, validation provided in the conditional check.
            newInput.TimeLocalized = (input.TimeLocalized == true) ? true : false;

            // Validates Units parameter
            newInput.Units = (String.IsNullOrWhiteSpace(input.Units)) ? "metric" : input.Units;
            string[] validUnits = new string[] { "metric", "imperial", "default" };
            if (!Array.Exists(validUnits, element => element == newInput.Units))
            {
                newInput.Units = "default";
            }

            // Validates OutputFormat parameter
            newInput.OutputFormat = (String.IsNullOrWhiteSpace(input.OutputFormat)) ? "json" : input.OutputFormat;
            string[] validOutputFormat = new string[] { "json", "default" };
            if (!Array.Exists(validOutputFormat, element => element == newInput.OutputFormat))
            {
                newInput.OutputFormat = "default";
            }

            newInput.BaseURL = new List<string>();
            foreach (string ds in dataset)
            {
                string tempError = "";
                newInput.BaseURL.Add(GetBaseURL(input, ds, out tempError));
                if (tempError.Contains("ERROR"))
                {
                    //errorMsg += tempError;
                    errors.Add(tempError);
                }
            }

            // Assign ITimeSeriesInput, if null assign empty ITimeSeriesOutput
            if (input.InputTimeSeries == null)
            {
                ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
                newInput.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
            }
            else
            {
                newInput.InputTimeSeries = input.InputTimeSeries;
            }
            errorMsg = string.Join(" ", errors.ToArray());

            return newInput;
        }


        private static string GetBaseURL(ITimeSeriesInput input, string dataset, out string errorMsg)
        {
            errorMsg = "";
            Dictionary<string, string> urls = new Dictionary<string, string>();

            try
            {
                urls = Data.Files.FileToDictionary(@".\App_Data\" + "url_info.txt");
            }
            catch (FileNotFoundException)
            {
                urls = Data.Files.FileToDictionary("/app/App_Data/url_info.txt");
            }

            Dictionary<string, string> caselessUrls = new Dictionary<string, string>(urls, StringComparer.OrdinalIgnoreCase);
            string source = input.Source.ToLower();

            string src = "";
            switch (source)
            {
                case "nldas":
                    src = "NLDAS";
                    break;
                case "gldas":
                    src = "GLDAS";
                    break;
                case "daymet":
                    src = "DAYMET";
                    break;
                case "ncdc":
                    src = "NCDC";
                    break;
                case "wgen":
                    return "";
                case "prism":
                    src = "PRISM";
                    break;
                default:
                    errorMsg = "ERROR: Provided source is not valid. Unable to construct base url.";
                    return "";
            }
            string url_key = (src == "PRISM") ? src + "_URL" : src + "_" + dataset + "_URL";
            try
            {
                return caselessUrls[url_key];
            }
            catch
            {
                errorMsg = "ERROR: Unable to construct base url from the specified dataset and provided data source.";
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