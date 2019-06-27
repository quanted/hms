using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace ContaminantLoader
{
    /// <summary>
    /// Base ContaminantLoader class
    /// </summary>
    public class ContaminantLoader
    {

        /// <summary>
        /// Resulting ITimeSeriesOutput from the InputString
        /// </summary>
        public ITimeSeriesOutput Result { get; set; }

        /// <summary>
        /// The type of contaminant to be converted, different contaminants may require special treatment or handling.
        /// </summary>
        public string ContaminantType { get; set; }

        /// <summary>
        /// The format type of the InputString (valid options include json and csv) Others to be added in the future.
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// String values of the contaminant to be converted into ITimeSeriesOutput
        /// </summary>
        public string InputString { get; set; }

        /// <summary>
        /// Initialize Contaminant with type, inputType and input
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inputType"></param>
        /// <param name="input"></param>
        public ContaminantLoader(string type, string inputType, string input)
        {
            this.ContaminantType = type;
            this.InputType = inputType;
            this.InputString = input;
            this.SetResults();
        }

        /// <summary>
        /// Convert contaminant input to HMS compatible format.
        /// </summary>
        private void SetResults()
        {
            switch (this.ContaminantType)
            {
                default:
                case ("generic"):
                    Generic generic = new Generic();
                    this.Result = generic.ConvertGenericInput(this.InputString, this.InputType);
                    break;
            }
            this.TimeSeriesAnalysis();
        }

        /// <summary>
        /// Calculate timestep, specify start and end date, state timestep count. Calculations added to Result.Metadata
        /// </summary>
        private void TimeSeriesAnalysis()
        {
            List<string> timesteps = new List<string>();
            DateTime date0;
            bool dateParse0 = DateTime.TryParse(this.Result.Data.Keys.ElementAt(0), out date0);
            if (!dateParse0)
            {
                DateTime.TryParseExact(this.Result.Data.Keys.ElementAt(0), "yyyy-MM-dd HH", null, System.Globalization.DateTimeStyles.None, out date0);
            }
            this.Result.Metadata.Add("start_date", date0.ToString("yyyy-MM-dd HH"));

            foreach(KeyValuePair<string, List<string>> data in this.Result.Data)
            {
                DateTime date;
                bool dateParse = DateTime.TryParse(data.Key, out date);
                if (!dateParse)
                {
                    DateTime.TryParseExact(data.Key, "yyyy-MM-dd HH", null, System.Globalization.DateTimeStyles.None, out date);
                }
                if (date != date0 )
                {
                    double hourDiff = (date - date0).TotalHours;
                    if(hourDiff < 24)
                    {
                        timesteps.Add(hourDiff + " hour(s)");
                    }
                    else if(hourDiff >= 24)
                    {
                        timesteps.Add(hourDiff / 24 + " day(s)");
                    }
                    else
                    {
                        timesteps.Add("unknown");
                    }
                }
                date0 = date;
            }
            this.Result.Metadata.Add("end_date", date0.ToString("yyyy-MM-dd HH"));
            bool samestep = true;
            foreach(string step in timesteps)
            {
                if (!step.Equals(timesteps[0]))
                {
                    samestep = false;
                }
            }
            if (samestep)
            {
                this.Result.Metadata.Add("timestep", timesteps[0]);
            }
            else
            {
                this.Result.Metadata.Add("timestep", "variable");
            }
            this.Result.Metadata.Add("count", this.Result.Data.Count.ToString());
        }
    }
}
