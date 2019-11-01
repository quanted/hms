using System;
using System.Collections.Generic;
using Solar;
using Data;
using System.Globalization;
using System.Threading.Tasks;


namespace Web.Services.Models
{
    /// <summary>
    /// Solar Calculator Input object
    /// </summary>
    public class SolarCalculatorInput : TimeSeriesInput
    {
        /// <summary>
        /// Calculator model: 'Day' or 'Year'
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Calculation localtime for when model='year', default='12:00:00'
        /// </summary>
        public string LocalTime { get; set; }

    }

    /// <summary>
    /// Model for the WSSolar controller
    /// </summary>
    public class WSSolar
    {

        /// <summary>
        /// Calls into the Solar module and gets the default input data,
        /// equivlanet to selectin the first option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetGCSolarDefaultInput()
        {
            GCSolar gcS = new GCSolar();
            return gcS.GetDefaultInputs();
        }

        /// <summary>
        /// Calls into the Solar module and gets the default data, 
        /// equivalent to selecting the third option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetGCSolarOutput()
        {
            GCSolar gcS = new GCSolar();
            return gcS.GetOutput();
        }

        /// <summary>
        /// Calls into the Solar module and sets the Common variables using the input object and returns the data based on those inputs.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetGCSolarOutput(Dictionary<string, object> input)
        {
            GCSolar gcS = new GCSolar();
            List<string> errors = new List<string>();
            gcS.SetCommonVariables(input, out errors);
            if (errors.Count > 0)
            {
                return new Dictionary<string, object>()
                {
                    { "Input Errors", errors }
                };
            }
            return gcS.GetOutput();
        }

        /// <summary>
        /// Constructs input metadata for GCSolar module.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetMetadata()
        {
            GCSolar gcS = new GCSolar();

            double[] waves = gcS.common.getWave();
            double[] dLats = gcS.common.ilattm;
            string[] seasons = gcS.common.sease;
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("Notice", "Metadata is in development.");
            metadata.Add("Contaminant Name", new Dictionary<string, string>()
            {
                { "Default Value", "Methoxyclor" },
                { "Possible Values", "null" },
                { "Description", "null" }
            });
            metadata.Add("Contaminant Type", new Dictionary<string, string>()
            {
                { "Default Value", "Chemical" },
                { "Possible Values", "Chemical, Biological" },
                { "Description", "Corresponds to contaminant name." }
            });
            metadata.Add("Water Type Name", new Dictionary<string, string>()
            {
                { "Default Value", "Pure Water" },
                { "Possible Values", "Pure Water, Natural Water" },
                { "Description", "null" }
            });
            metadata.Add("Min Wavelength", new Dictionary<string, object>()
            {
                { "Default Value", 297.5 },
                { "Possible Values", waves },
                { "Description", "Minimum wavelength for the calculated solar data. Must be less than maximum wavelength." }
            });
            metadata.Add("Max Wavelength", new Dictionary<string, object>()
            {
                { "Default Value", 330 },
                { "Possible Values", waves },
                { "Description", "Maximum wavelength for the calculated solar data. Must be greater than minimum wavelength." }
            });
            metadata.Add("Longitude", new Dictionary<string, object>()
            {
                { "Default Value", 83.2 },
                { "Possible Values", "null" },
                { "Description", "Longitude for the calculated solar data." }
            });
            metadata.Add("Latitude(s)", new Dictionary<string, object>()
            {
                { "Default Value", dLats },
                { "Possible Values", null },
                { "Description", "List of latitude values for the calcualted solar data. Unused values are listed as -99. If custom ephemeride values are used, switch out the Latitude(s) list variable with the single Latitude variable." }
            });
            string[] seasonsList = new string[4] { "Spring", "Summer", "Fall", "Winter" };
            metadata.Add("Season(s)", new Dictionary<string, object>()
            {
                { "Default Value",  seasons },
                { "Possible Values", seasonsList },
                { "Description", "List of seasons for the calculated solar data. If custom ephemeride values are used, do not include Season(s) variable." }

            });
            metadata.Add("Latitude", new Dictionary<string, object>()
            {
                { "Default Value", 40 },
                { "Possible Values", null },
                { "Description", "Used only when custom ephemeride values are desired, where the Latitude variable replaces the Latitude(s) variable." }
            });
            double[] sDec = new double[3] { 23, 26, 24.1 };
            metadata.Add("Solar Declination", new Dictionary<string, object>()
            {
                { "Default Value", sDec },
                { "Possible Values", null },
                { "Description", "Used only when custom ephemeride values are desired. Format of input is [Degrees, Minutes, Seconds]." }
            });
            double[] rAsc = new double[3] { 5, 58, 53.26 };
            metadata.Add("Right Ascension", new Dictionary<string, object>()
            {
                { "Default Value", rAsc },
                { "Possible Values", null },
                { "Description", "Used only when custom ephemeride values are desired. Format of input is [Degrees, Minutes, Seconds]." }
            });
            double[] sRea = new double[3] { 17, 57, 16.047 };
            metadata.Add("Sidereal Time", new Dictionary<string, object>()
            {
                { "Default Value", sRea },
                { "Possible Values", null },
                { "Description", "Used only when custom ephemeride valus are desired. Format of input is [Degrees, Minutes, Seconds]." }
            });
            metadata.Add("Atmospheric Ozone Layer", new Dictionary<string, object>()
            {
                { "Default Value", 0.3 },
                { "Possible Values", null },
                { "Description", "Thickness of atmospheric ozone layer." }
            });
            metadata.Add("Initial Depth (cm)", new Dictionary<string, object>()
            {
                { "Default Value", 0.001 },
                { "Possible Values", null },
                { "Description", "" }
            });
            metadata.Add("Final Depth (cm)", new Dictionary<string, object>()
            {
                { "Default Value", 5 },
                { "Possible Values", null },
                { "Description", "" }
            });
            metadata.Add("Depth Increment (cm)", new Dictionary<string, object>()
            {
                { "Default Value", 10 },
                { "Possible Values", null },
                { "Description", "" }
            });
            metadata.Add("Quantum Yield", new Dictionary<string, object>()
            {
                { "Default Value", 0.32 },
                { "Possible Values", null },
                { "Description", "" }
            });
            metadata.Add("Refractive Index", new Dictionary<string, object>()
            {
                { "Default Value", 1.34 },
                { "Possible Values", null },
                { "Description", "" }
            });
            metadata.Add("Elevation (km)", new Dictionary<string, object>()
            {
                { "Default Value", 0 },
                { "Possible Values", null },
                { "Description", "" }
            });
            return metadata;
        }

        /// <summary>
        /// Model for accessing NOAA Solar Calculator 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> RunSolarCalculator(SolarCalculatorInput input)
        {
            Solar.SolarCalculator soCal = new SolarCalculator();
            soCal.Input = input;
            soCal.Model = input.Model.ToLower();
            Utilities.ErrorOutput error = new Utilities.ErrorOutput();
            //Validate unique solar calculator inputs
            if (String.IsNullOrWhiteSpace(input.DateTimeSpan.StartDate.ToString()) || input.DateTimeSpan.StartDate == DateTime.MinValue)
            {
                return error.ReturnError("DateTimeSpan has no StartDate. StartDate is a required parameter.");
            }
            if (string.IsNullOrWhiteSpace(input.DateTimeSpan.EndDate.ToString()) || input.DateTimeSpan.EndDate == DateTime.MinValue)
            {
                soCal.Input.DateTimeSpan.EndDate = soCal.Input.DateTimeSpan.StartDate.AddYears(1);
            }
            if (String.IsNullOrWhiteSpace(input.Model))
            {
                return error.ReturnError("Model parameter not defined in request.");
            }
            if (String.IsNullOrWhiteSpace(input.LocalTime) && input.Model.ToLower().Equals("year"))
            {
                soCal.LocalTime = "12:00:00";
            }
            else if(input.Model.ToLower().Equals("year"))
            {
                try
                {
                    DateTime testDate = DateTime.ParseExact(input.LocalTime, "hh:mm:ss", CultureInfo.InvariantCulture);
                }
                catch(FormatException fe){
                    return error.ReturnError("LocalTime parameter provided is a not a valid hour time. LocalTime format must be hh:mm:ss");
                }
                soCal.LocalTime = input.LocalTime;
            }

            soCal.GetCalculatorData();

            return soCal.Output;
        }
    }
}