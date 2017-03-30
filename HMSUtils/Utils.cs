using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HMSUtils
{
    public class Utils
    {
        enum States
        {
            AK, AL, AR, AZ, CA, CO, CT, DC, DE, FL, GA, HI, IA, ID, IL, IN, KS, KY, LA, MA, MD, ME, MI, MN, MO, MS, MT, NC, ND, NE, NH, NJ, NM, NV, NY, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VA, VT, WA, WI, WV, WY
        };
        enum Sources
        {
            NLDAS, GLDAS, Daymet, NCDC, compare
        };

        /// <summary>
        /// Evaluations a dictionary of parameters to ensure valid values are given, returns false if a parameter is found to be invalid. Specific details are returned in errorMsg.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ParameterValidation(out string errorMsg, Dictionary<string, string> parameters)
        {
            errorMsg = "";
            bool valid = true;

            if (parameters.ContainsKey("dataset"))
            {
                parameters["dataset"] = parameters["dataset"].ToLower();
            }
            else
            {
                errorMsg = "ERROR: dataset parameter was not found.\n";
                valid = false;
            }

            //Source test
            if (parameters.ContainsKey("source"))
            {
                Sources source;
                if (!Enum.TryParse(parameters["source"], out source))
                {
                    errorMsg += "ERROR: source parameter was not found.\n";
                    valid = false;
                }
            }
            else
            {
                errorMsg += "ERROR: source parameter not found, source is required.\n";
                valid = false;
            }

            //startDate and endDate test, checks if valid DateTime
            if (parameters.ContainsKey("endDate") && parameters.ContainsKey("startDate"))
            {
                if (valid == true)
                {
                    valid = DateValidation(out errorMsg, parameters["startDate"], parameters["endDate"], parameters["source"]);
                }
            }
            else
            {
                errorMsg += "ERROR: startDate and endDate parameters not found, startDate and endDate are required.\n";
                valid = false;
            }

            //Checks coordinate values for the selected source
            if (!parameters.ContainsKey("stationID"))
            {
                if (parameters.ContainsKey("latitude") && parameters.ContainsKey("longitude"))
                {
                    if (valid == true)
                    {
                        valid = CoordinatesValidation(out errorMsg, parameters["latitude"], parameters["longitude"], parameters["source"]);
                    }
                }
                else if (parameters.ContainsKey("filePath"))        // If shapefile provided
                {
                    if (String.IsNullOrWhiteSpace(parameters["filePath"]))
                    {
                        errorMsg += "ERROR: No file provided.\n";
                        valid = false;
                    }
                }
                else if (parameters.ContainsKey("geojson"))         // if geoJSON provided       
                {
                    //if (String.IsNullOrWhiteSpace(parameters["geoJSON"]))
                    //{
                    //    parameters.Add("filePath", HttpContext.Current.Server.MapPath("~\\TransientStorage\\" + parameters["id"]) + "\\geo.json");
                    //    System.IO.File.WriteAllText(parameters["filePath"], parameters["geoJSON"]);
                    //}
                }
                else
                {
                    errorMsg += "ERROR: No valid spatial location values provided. Must provide one of the follow: latitude and longitude coordinates, zipped shapefile, or geoJson (file or as a parameter).\n";
                    valid = false;
                }
            }

            // Checking for optional parameters and setting to default if not found.
            if (!parameters.ContainsKey("localTime"))
            {
                parameters.Add("localTime", "false");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(parameters["localTime"]))
                {
                    parameters["localTime"] = "false";
                }
            }
            if (!parameters.ContainsKey("layers"))
            {
                parameters.Add("layers", "0");
            }

            return valid;
        }

        /// <summary>
        /// Checks that the start date and end date are within the bounds of the source date range.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool DateValidation(out string errorMsg, string startDate, string endDate, string source)
        {
            errorMsg = "";
            bool valid = true;
            DateTime start;
            DateTime end;
            if (!DateTime.TryParse(startDate, out start))
            {
                errorMsg = "ERROR: startDate is not a valid date.\n";
                valid = false;
            }
            if (!DateTime.TryParse(endDate, out end))
            {
                errorMsg += "ERROR: endDate is not a valid date.\n";
                valid = false;
            }
            if (start != DateTime.MinValue && end != DateTime.MinValue)
            {
                if (DateTime.Compare(start, end) > 0)
                {
                    errorMsg += "ERROR: endDate must be a date after startDate.\n";
                    valid = false;
                }
                if (DateTime.Compare(start, DateTime.Now) > 0)
                {
                    errorMsg += "ERROR: startDate must be a date before today.\n";
                    valid = false;
                }
                if (DateTime.Compare(end, DateTime.Now) > 0)
                {
                    errorMsg += "ERROR: endDate must be a date before today.\n";
                    valid = false;
                }
            }
            else
            {
                errorMsg += "ERROR: startDate and endDate parameters do not contain valid dates.\n";
                valid = false;
            }
            if (valid == true)
            {
                valid = DateValidation(out errorMsg, start, end, source);
            }
            
            return valid;
        }

        /// <summary>
        /// Checks that the start date and end date are within the bounds of the source date range.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool DateValidation(out string errorMsg, DateTime start, DateTime end, string source)
        {
            errorMsg = "";
            bool valid = true;
            switch (source)
            {
                case "NLDAS":
                    DateTime minNLDASDate = new DateTime(1979, 01, 02);
                    if (DateTime.Compare(start, minNLDASDate) < 0)
                    {
                        errorMsg = "ERROR: Startdate value is before NLDAS min date. NLDAS min date is: " + minNLDASDate.ToString() + "\n";
                        valid = false;
                    }
                    if (DateTime.Compare(end, DateTime.Now.AddDays(-4)) > 0)
                    {
                        errorMsg += "ERROR: Enddate value is after latest NLDAS data release date. NLDAS latency is about 4 days.\n";
                        valid = false;
                    }
                    return valid;
                case "GLDAS":
                    DateTime minGLDASDate = new DateTime(2000, 02, 25);
                    if (DateTime.Compare(start, minGLDASDate) < 0)
                    {
                        errorMsg = "ERROR: Startdate value is before GLDAS min date. GLDAS min date is: " + minGLDASDate.ToString() + "\n";
                        valid = false;
                    }
                    if (DateTime.Compare(end, DateTime.Now.AddDays(-60)) > 0)
                    {
                        errorMsg += "ERROR: Enddate value is after latest GLDAS data release date. GLDAS latency is about 2 months.\n";
                        valid = false;
                    }
                    return valid;
                case "Daymet":
                    DateTime minDaymetDate = new DateTime(1980, 01, 01);
                    if (DateTime.Compare(start, minDaymetDate) < 0)
                    { 
                        errorMsg = "ERROR: Startdate value is before Daymet min date. Daymet min date is: " + minDaymetDate.ToString() + "\n";
                        valid = false;
                    }
                    if (DateTime.Compare(end, DateTime.Now.AddMonths(-18)) > 0)
                    {
                        errorMsg += "ERROR: Enddate value is after latest Daymet data release date. Daymet latency is about 18 months.\n";
                        valid = false;
                    }
                    //Daymet requires at least one year between start and end dates.
                    if ((end - start).Days <= 364)
                    {
                        errorMsg += "ERROR: Daymet requires start and end date must be equal to or greater than 1 year.\n";
                        valid = false;
                    }
                    return valid;
                case "NCDC":
                    // Will need to load station data and compare dates with that data.
                    return valid;
                default:
                    return false;
            }
        }

        public int CompareDates(out string errorMsg, string date1, string date2)
        {
            errorMsg = "";
            DateTime firstDate;
            DateTime secondDate;
            if(!DateTime.TryParse(date1, out firstDate))
            {
                errorMsg = "ERROR: Invalid firstDate, unable to parse date from string.";
                return 0;
            }
            if (!DateTime.TryParse(date2, out secondDate))
            {
                errorMsg = "ERROR: Invalid secondDate, unable to parse date from string.";
                return 0;
            }

            return DateTime.Compare(firstDate, secondDate);
        }

        /// <summary>
        /// Checks that latitude and longitude values are within the spatial coverage of the source.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool CoordinatesValidation(out string errorMsg, string latitude, string longitude, string source)
        {
            errorMsg = "";
            bool valid = true;
            double lat;
            double lon;

            if (!Double.TryParse(latitude, out lat))
            {
                errorMsg = "ERROR: latitude is not a valid numeric value.\n";
                valid = false;
            }
            else
            {
                if (Math.Abs(lat) > 90)
                {
                    errorMsg += "ERROR: latitude value is outside latitude range. Max/min latitude value is 90/-90.\n";
                    valid = false;
                }
            }
            if (!Double.TryParse(longitude, out lon))
            {
                errorMsg += "ERROR: longitude is not a valid numeric value.\n";
                valid = false;
            }
            else
            {
                if (Math.Abs(lon) > 180)
                {
                    errorMsg += "ERROR: longitude value is outside longitude range. Max/min longitude value is 180/-180.\n";
                    valid = false;
                }
            }
            if (valid == true)
            {
                valid = CoordinatesValidation(out errorMsg, lat, lon, source);
            }
            return valid;
        }

        /// <summary>
        /// Checks that latitude and longitude values are within the spatial coverage of the source.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool CoordinatesValidation(out string errorMsg, double latitude, double longitude, string source)
        {
            errorMsg = "";
            bool valid = true;
            // Coordinate values have already been evaluated to be within the required -90:90, -180:180 range
            switch (source)
            {
                case "NLDAS":
                    if (latitude > 53 || latitude < 25)     //NLDAS spatial bounds for latitude
                    {
                        errorMsg = "ERROR: Latitude value is outside of the spatial coverage for NLDAS. NLDAS spatial coverage is: 125W ~ 63W, 25N ~ 53N.\n";
                        valid = false;
                    }
                    if (longitude > -63 || longitude < -125)        //NLDAS spatial bounds for longitude
                    {
                        errorMsg += "ERROR: Longitude value is outside of the spatial coverage for NLDAS. NLDAS spatial coverage is: 125W ~ 63W, 25N ~ 53N.\n";
                        valid = false;
                    }
                    return valid;
                case "GLDAS":
                    if (latitude < -60)
                    {
                        errorMsg = "ERROR: Latitude value is outside of the spatial coverage for GLDAS. GLDAS spatial coverage is: 180W ~ 180E, 60S ~ 90N.\n";
                        valid = false;
                    }
                    return valid;
                default:
                    return valid;
            }
        }

        /// <summary>
        /// Unzips zip files to 'filePath + sessionsGUID' directory and checks for required files.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="filePath"></param>
        /// <param name="sessionGUID"></param>
        public void UnzipShapefile(out string errorMsg, string filePath, string sessionGUID)
        {
            errorMsg = "";
            if (Path.GetExtension(filePath).Contains("zip"))
            {
                // Web service path should look like HttpContext.Current.Server.MapPath("~\\TransientStorage\\")
                string extractPath = filePath.Substring(0, filePath.LastIndexOf("\\"));
                try
                {
                    ZipFile.ExtractToDirectory(filePath, extractPath);
                    File.Delete(filePath);
                    CheckUnzippedShapefiles(out errorMsg, filePath, sessionGUID);
                    if (errorMsg.Contains("ERROR")) { return; }
                }
                catch (Exception ex)
                {
                    errorMsg = "ERROR: " + ex;
                    return;
                }
            }
            else if (Path.GetExtension(filePath).Contains("json")) { return; }
            else { errorMsg = "ERROR: Invalid file provided. Accepted file types are: zipped shapefile (containing a shp, prj, and dbf file) or a json (containing geojson data)."; return; }

        }

        /// <summary>
        /// Checks for all required files for shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="path"></param>
        /// <param name="sessionGUID"></param>
        /// <returns></returns>
        public bool CheckUnzippedShapefiles(out string errorMsg, string path, string sessionGUID)
        {
            errorMsg = "";
            Dictionary<string, bool> requiredFiles = new Dictionary<string, bool>
            {
                {".shp", false},
                {".prj", false},
                {".dbf", false}
            };
            foreach (string file in Directory.GetFiles(path.Substring(0, path.LastIndexOf("\\"))))
            {
                string ext = Path.GetExtension(file);
                if (requiredFiles.ContainsKey(ext))
                {
                    requiredFiles[ext] = true;
                }
            }
            if (requiredFiles.ContainsValue(false))
            {
                errorMsg = "ERROR: Zipped shapefile did not contain all required files. Zip must contain shp, prj, and dbf files.";
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Deletes the temp directory associated with the specified GUID.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sessionGUID"></param>
        public void DeleteTempGUIDDirectory(string path, string sessionGUID)
        {
            try
            {
                if (Directory.Exists(path + sessionGUID + "\\"))
                {
                    Directory.Delete(path + sessionGUID + "\\", true);
                }
            }
            catch
            {
                // write to log?
            }
        }

        /// <summary>
        /// Collects the parameters contained in the paramString.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public Dictionary<string, string> ParseParameterString(out string errorMsg, string paramString)
        {
            errorMsg = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                string[] values = paramString.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < values.Length; i++)
                {
                    string[] line = values[i].Split('=');
                    parameters.Add(line[0], line[1]);
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to collect parameters.\n";
            }


            return parameters;
        }

        public Dictionary<string, string> GetNCDCStationDetails(out string errorMsg, string stationID)
        {
            errorMsg = "";
            HMSNCDC.HMSNCDC details = new HMSNCDC.HMSNCDC();
            Dictionary<string, string> stationDetails = details.GetStationDetails(out errorMsg, stationID);
            if(errorMsg != "")
            {
                stationDetails.Add("errorMsg", errorMsg);
            }
            return stationDetails;
        }

        //---------------------- Updated Util Methods ---------------------//

        /// <summary>
        /// Creates HMSData object with a metadata property containing an ERROR message to be returned.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public HMSJSON.HMSJSON.HMSData ReturnError(string errorMsg)
        {
            HMSJSON.HMSJSON.HMSData reply = new HMSJSON.HMSJSON.HMSData();
            Dictionary<string, string> meta = new Dictionary<string, string>()
            {
                { "errorMsg", errorMsg }
            };
            reply.metadata = meta;
            return reply;
        }
    }
}