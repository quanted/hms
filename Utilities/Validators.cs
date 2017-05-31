using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class Validators
    {

        /// <summary>
        /// ENUM of valid sources.
        /// </summary>
        enum Sources
        {
            nldas, gldas, daymet, ncdc, wgen
        };


        /// <summary>
        /// Validates the parameters of a ITimeSeriesInput object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="parameters">ITimeSeriesInput object</param>
        /// <returns>true if parameters are valid.</returns>
        public bool ParameterValidation(out string errorMsg, ITimeSeriesInput parameters)
        {
            errorMsg = "";
            bool valid = true;

            // Check if Source parameter exists.
            if (String.IsNullOrEmpty(parameters.Source))
            {
                errorMsg += "ERROR: Source not found.\n";
                valid = false;
            }            
            else
            {
                Sources source;
                // Check if Source parameter is a valid source.
                if (!Enum.TryParse(parameters.Source, true, out source))
                {
                    errorMsg += "ERROR: Source is not valid. Provided source: " + parameters.Source + "\n";
                    valid = false;
                }
            }

            // Check if TimeSpan StartDate is valid.
            if (parameters.TimeSpan.StartDate == DateTime.MinValue)
            {
                errorMsg += "ERROR: Start date value is not valid. Provided start date: " + parameters.TimeSpan.StartDate.ToString() + "\n";
                valid = false;
            }
            // Check if TimeSpan EndDate is valid.
            if (parameters.TimeSpan.EndDate == DateTime.MinValue)
            {
                errorMsg += "ERROR: End date value is not valid. Provided end date: " + parameters.TimeSpan.EndDate.ToString() + "\n";
                valid = false;
            }

            // Check if TimeSpan StartDate is before EndDate.
            if (DateTime.Compare(parameters.TimeSpan.StartDate, parameters.TimeSpan.EndDate) >= 0)
            {
                errorMsg += "ERROR: End date is invalid, is equal or before start date. Provided start date: " + parameters.TimeSpan.StartDate.ToString() + 
                    " Provided end date: " + parameters.TimeSpan.EndDate.ToString() + "\n";
                valid = false;
            }

            return valid;
        }

    }
}
