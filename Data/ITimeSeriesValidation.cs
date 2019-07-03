using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Data
{
    /// <summary>
    /// Input validation class, contains all static validation functions initiated from Validate
    /// </summary>
    public class ITimeSeriesValidation
    {
        static string[] validDatasets = {
            "precipitation", "evapotranspiration", "nutrients", "organicmatter", "radiation",
            "soilmoisture", "solar", "streamhydrology", "subsurfaceflow", "surfacerunoff", "surfacepressure",
            "temperature", "wind", "dewpoint", "humidity"
        };

        static Dictionary<string, List<string>> validSources = new Dictionary<string, List<string>>()
        {
            ["precipitation"] =  new List<string>{ "nldas", "gldas", "daymet", "ncei", "prism", "wgen", "nwm" },
            ["evapotranspiration"] = new List<string> { "nldas", "gldas", "daymet", "prism", "grangergray", "hamon", "hspf",
                "mcjannett", "mortoncrae", "mortoncrwe", "ncdc", "penmandaily", "penmanhourly",
                "penmanopenwater", "penpan", "priestlytaylor", "shuttleworthwallace" }, 
            ["nutrients"] = new List<string> { "aquatox" },
            ["organicmatter"] = new List<string> { "aquatox"},
            ["radiation"]  = new List<string> { "nldas", "gldas", "daymet"},
            ["soilmoisture"] = new List<string> { "nldas", "gldas" },
            ["solar"] = new List<string> { "gcsolar", "solarcalcualtor" },
            ["streamhydrology"] = new List<string> { "aquatox" },
            ["subsurfaceflow"] = new List<string> { "nldas", "gldas", "curvenumber" },
            ["surfacerunoff"] = new List<string> { "nldas", "gldas", "curvenumber" },
            ["temperature"] = new List<string> { "nldas", "gldas", "daymet", "prism" },
            ["workflow"] = new List<string> { "nldas", "gldas", "ncei", "daymet" },
            ["wind"] = new List<string> { "nldas", "gldas", "ncei" },
            ["dewpoint"] = new List<string> { "prism" },
            ["humidity"] = new List<string> { "prism", "nldas", "gldas" },
            ["surfacepressure"] = new List<string> { "gldas" }
        };

        static string[] validRemoteData =
        {
            "nldas", "gldas", "ncei", "daymet", "prism"
        };

        /// <summary>
        /// Primary input validate method, initiates all validation methods.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ITimeSeriesInput Validate(out string errorMsg, List<string> dataset, ITimeSeriesInput input)
        {
            errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput validInput = iFactory.Initialize();
            List<string> errors = new List<string>();
            List<string> errorTemp = new List<string>();

            bool validDataset = ValidateDataset(out errorTemp, dataset);
            errors = errors.Concat(errorTemp).ToList();
            errorTemp = new List<string>();

            if (errors.Count == 0 && ValidateSource(out errorTemp, dataset, input.Source))
            {
                validInput.Source = input.Source;
            }
            errors = errors.Concat(errorTemp).ToList();
            errorTemp = new List<string>();

            if (errors.Count == 0 && ValidateGeometry(out errorTemp, input.Geometry))
            {
                validInput.Geometry = input.Geometry;
                validInput.Geometry.Timezone = ValidateTimezone(out errorTemp, input.Geometry.Timezone);
                validInput.Geometry.GeometryMetadata = (input.Geometry.GeometryMetadata == null) ? new Dictionary<string, string>() : input.Geometry.GeometryMetadata;
            }
            errors = errors.Concat(errorTemp).ToList();
            errorTemp = new List<string>();

            if (errors.Count == 0 && ValidateDates(out errorTemp, input.DateTimeSpan))
            {
                validInput.DateTimeSpan = input.DateTimeSpan;
                if (input.DateTimeSpan.DateTimeFormat == null)
                {
                    validInput.DateTimeSpan.DateTimeFormat = "yyyy-MM-dd HH";
                }
            }
            errors = errors.Concat(errorTemp).ToList();
            errorTemp = new List<string>();

            validInput.BaseURL = GetBaseUrl(out errorTemp, input.Source, dataset);
            errors = errors.Concat(errorTemp).ToList();
            errorTemp = new List<string>();

            validInput.TemporalResolution = (string.IsNullOrWhiteSpace(input.TemporalResolution)) ? "default" : input.TemporalResolution;
            validInput.TimeLocalized = (string.IsNullOrWhiteSpace(input.TimeLocalized.ToString())) ? false : true;
            validInput.Units = (string.IsNullOrWhiteSpace(input.Units)) ? "metric" : input.Units;
            validInput.OutputFormat = (string.IsNullOrWhiteSpace(input.OutputFormat)) ? "json" : input.OutputFormat;
            validInput.DataValueFormat = (string.IsNullOrWhiteSpace(input.DataValueFormat)) ? "E3" : input.DataValueFormat;

            errorMsg = string.Join(", ", errors.ToArray());
            return validInput;
        }
        
        /// <summary>
        /// Validates the timezone attribute for ITimeSeriesInput
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Timezone ValidateTimezone(out List<string> errors, Timezone input)
        {
            errors = new List<string>();
            Timezone validTZ = new Timezone(); 
            if (input == null)
            {
                validTZ = new Timezone()
                {
                    Name = "",
                    Offset = 0.0,
                    DLS = false
                };
            }
            else
            {
                validTZ = new Timezone() { };
                validTZ.Name = (String.IsNullOrWhiteSpace(input.Name)) ? "TZNotSet" : input.Name;
                validTZ.Offset = (Double.IsNaN(input.Offset)) ? 0.0 : input.Offset;
                if (validTZ.Offset > 12 || validTZ.Offset < -12)
                {
                    errors.Add("ERROR: Timezone offset value is not a valid timezone. Timezone offset provided: " + validTZ.Offset.ToString());
                }
                validTZ.DLS = (input.DLS == true) ? true : false;
            }
            return validTZ;
        }

        /// <summary>
        /// Validates the list of input dataset values against the list of valid datasets.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static bool ValidateDataset(out List<string> errors, List<string> dataset)
        {
            errors = new List<string>();
            bool valid = true;
            if (!dataset.Any() || dataset == null || dataset.Count == 0)
            {
                errors.Add("ERROR: Dataset is invalid or not found.");
                return false;
            }
            foreach(string ds in dataset)
            {
                string dss = ds.Split(new char[] { '_' }).Last();
                valid = (validDatasets.Contains(dss.ToLower())) ? true : false;
            }
            if (!valid)
            {
                errors.Add("ERROR: Dataset is not valid. Dataset value(s): " + string.Join(", ", dataset.ToArray()));
            }
            return valid;
        }

        /// <summary>
        /// Validates the input source against the list of valid sources.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="dataset"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool ValidateSource(out List<string> errors, List<string> dataset, string source)
        {
            errors = new List<string>();
            bool valid = true;
            if (string.IsNullOrWhiteSpace(source))
            {
                errors.Add("ERROR: Source not found in input.");
                return false;
            }
            foreach (string ds in dataset)
            {
                string dss = ds.Split(new char[] { '_' }).Last();
                valid = (validSources[dss].Contains(source.ToLower())) ? true : false;
            }
            if (!valid)
            {
                errors.Add("ERROR: Source is not valid or not compatible with dataset. Dataset value(s): "
                    + string.Join(", ", dataset.ToArray()) + ". Source value: " + source);
            }
            return valid;
        }

        /// <summary>
        /// Validates input geometry.
        /// Valid inputs: Point.Latitude/Point.Longitude, HucID, ComID, StationID
        /// One valid input must be provided.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static bool ValidateGeometry(out List<string> errors, ITimeSeriesGeometry geometry)
        {
            errors = new List<string>();
            List<string> errorTemp = new List<string>();
            bool validGeom = true;
            if (geometry == null)
            {
                errors.Add("ERROR: No input geometry was found.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(geometry.Description))
            {
                if (geometry.Point != null)
                {
                    validGeom = ValidatePoint(out errorTemp, geometry.Point);
                    errors = errors.Concat(errorTemp).ToList();
                }
                if (!validGeom && !string.IsNullOrWhiteSpace(geometry.HucID))
                {
                    if (string.Equals("-1", geometry.HucID))
                    {
                        validGeom = ValidateHucID(out errorTemp, geometry.HucID);
                        errors = errors.Concat(errorTemp).ToList();
                    }
                }
                if (!validGeom && !string.IsNullOrWhiteSpace(geometry.ComID.ToString()))
                {
                    if (geometry.ComID > -1)
                    {
                        validGeom = ValidateComID(out errorTemp, geometry.ComID);
                        errors = errors.Concat(errorTemp).ToList();
                    }
                }
                if (!validGeom && !string.IsNullOrWhiteSpace(geometry.StationID))
                {
                    validGeom = ValidateStationID(out errorTemp, geometry.StationID);
                    errors = errors.Concat(errorTemp).ToList();
                }
            }
            else
            {
                switch (geometry.Description.ToLower())
                {
                    case "point":
                        validGeom = ValidatePoint(out errors, geometry.Point);
                        break;
                    case "hucid":
                        validGeom = ValidateHucID(out errors, geometry.HucID);
                        break;
                    case "comid":
                        validGeom = ValidateComID(out errors, geometry.ComID);
                        break;
                    case "stationid":
                        validGeom = ValidateStationID(out errors, geometry.StationID);
                        break;
                    default:
                        geometry.Description = "";
                        validGeom = ValidateGeometry(out errors, geometry);
                        break;
                }
            }
            return validGeom;
        }

        /// <summary>
        /// Validate geometry Point input.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool ValidatePoint(out List<string> errors, IPointCoordinate point)
        {
            errors = new List<string>();
            bool validPoint = true;
            if (point == null)
            {
                errors.Add("ERROR: Valid geometry point input not found.");
                return false;
            }
            if (point.Latitude > 90.0 || point.Latitude < -90.0)
            {
                errors.Add("ERROR: Latitude value is not valid. Latitude must be between -90 and 90. Latitude: " + point.Latitude.ToString());
                validPoint = false;
            }
            if (point.Longitude > 180.0 || point.Longitude < -180.0)
            {
                errors.Add("ERROR: Longitude value is not valid. Longitude must be between -180 and 180. Longitude: " + point.Longitude.ToString());
                validPoint = false;
            }
            return validPoint;
        }

        /// <summary>
        /// Validate geometry HucID input.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="hucID"></param>
        /// <returns></returns>
        private static bool ValidateHucID(out List<string> errors, string hucID)
        {
            errors = new List<string>();
            if (string.IsNullOrWhiteSpace(hucID))
            {
                errors.Add("ERROR: A valid HUC ID was not found.");
                return false;
            }
            int hucType = hucID.Length;
            if (hucType != 8 || hucType != 12)
            {
                errors.Add("ERROR: HucID provided is not valid. Only HucID's for HUC8 or HUC12 are accepted. HucID: " + hucID);
                return false;
            }
            return true;

        }

        /// <summary>
        /// Validate geometry ComID input.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="comID"></param>
        /// <returns></returns>
        private static bool ValidateComID(out List<string> errors, int comID)
        {
            errors = new List<string>();
            if (string.IsNullOrWhiteSpace(comID.ToString()) || comID <= 0)
            {
                errors.Add("ERROR: A valid COM ID was not found.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate geometry StationID input.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="stationID"></param>
        /// <returns></returns>
        private static bool ValidateStationID(out List<string> errors, string stationID)
        {
            errors = new List<string>();
            if (string.IsNullOrWhiteSpace(stationID))
            {
                errors.Add("ERROR: A valid stationID was not found.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates DateTimeSpan input.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool ValidateDates(out List<string> errors, IDateTimeSpan dateTime)
        {
            errors = new List<string>();
            bool nullDates = false;
            if (dateTime == null)
            {
                errors.Add("ERROR: No DateTimeSpan found.");
                return false;
            }
            if (dateTime.StartDate == null)
            {
                errors.Add("ERROR: Start date was not found.");
                nullDates = true;
            }
            if (dateTime.EndDate == null)
            {
                errors.Add("ERROR: End date was not found.");
                nullDates = true;
            }
            if (nullDates)
            {
                return false;
            }
            else
            {
                if (dateTime.StartDate.Equals(DateTime.MinValue))
                {
                    errors.Add("ERROR: Required 'StartDate' parameter was not found or is invalid.");
                    nullDates = true;
                }
                if (dateTime.EndDate.Equals(DateTime.MinValue))
                {
                    errors.Add("ERROR: Required 'EndDate' parameter was not found or is invalid.");
                    nullDates = true;
                }
                if (nullDates)
                {
                    return false;
                }
                if (DateTime.Compare(dateTime.StartDate, dateTime.EndDate) >= 0)
                {
                    errors.Add("ERROR: Start date must be before end date.");
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Get and assign base url for remote data sources.
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="source"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private static List<string> GetBaseUrl(out List<string> errors, string source, List<string> dataset)
        {
            errors = new List<string>();
            List<string> baseUrls = new List<string>();
            if (validRemoteData.Contains(source.ToLower()))
            {
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
                foreach (string ds in dataset)
                {
                    string url_key = source.ToLower() + "_" + ds.ToLower() + "_url";
                    try
                    {
                        baseUrls.Add(caselessUrls[url_key]);
                    }
                    catch
                    {
                        errors.Add("ERROR: Unable to construct base url from the specified dataset and provided data source.");
                    }
                }
            }
            return baseUrls;
        }
    }
}
