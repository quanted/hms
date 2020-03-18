using Data;
using System;
using System.Collections.Generic;

namespace ContaminantLoader
{
    public class Generic
    {

        /// <summary>
        /// Converts generic contaminant input to ITimeSeriesOutput, valid inputTypes are json and csv.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public ITimeSeriesOutput ConvertGenericInput(string input, string inputType)
        {
            switch (inputType)
            {
                case ("json"):
                    return this.ConvertJSON(input);
                default:
                case ("csv"):
                    return this.ConvertCSV(input);
            }
        }

        /// <summary>
        /// Converts input string of csv, formatted with ', ' or ',' delimiters. Column titles are checked for by assuming the title can't be parsed to a double.
        /// Titles, if present, are added to metadata.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput ConvertCSV(string input)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            string[] lines = input.Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.None);
            int i = 0;

            double testInt = -99999.999;
            bool columnTitles = !double.TryParse(lines[0].Split(new string[] { ",", ", " }, StringSplitOptions.None)[1], out testInt);
            foreach(string line in lines)
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split(new string[] { ",", ", " }, StringSplitOptions.None);
                    if (columnTitles)
                    {
                        for (int j = 1; j < values.Length; j++)
                        {
                            output.Metadata.Add("column_" + j, values[j]);
                        }
                        columnTitles = false;
                    }
                    else
                    {
                        List<string> cValues = new List<string>();
                        for (int j = 1; j < values.Length; j++)
                        {
                            cValues.Add(values[j]);
                        }
                        output.Data.Add(values[0], cValues);
                    }
                }
                i++;
            }
            output.Dataset = "Generic Contaminant";
            output.DataSource = "User Input";
            return output;
        }

        /// <summary>
        /// Converts input string of json, formatted like {'Data':[[date1, value1, value1_2], [date2, value2, value2_2], ...]}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput ConvertJSON(string input)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();

            try
            {
                dynamic jsonInput = System.Text.Json.JsonDocument.Parse(input);
                var data = jsonInput.Data;
                if (data != null)
                {
                    for(int i = 0; i < data.Count; i++)
                    {
                        List<string> cValues = new List<string>();
                        for(int j = 1; j < data[i].Count; j++)
                        {
                            cValues.Add(data[i][j].ToString());
                        }
                        output.Data.Add(data[i][0].ToString(), cValues);
                    }
                }
                else
                {
                    output.Metadata.Add("ERROR", "Contaminant Loader needs Data block containing timeseries data.");
                }
            }
            catch (System.Text.Json.JsonException ex)
            {
                output.Metadata.Add("ERROR", ex.ToString());
                output.Metadata.Add("Valid-Format-Error", "JSON input contaminant format must be structured as: 'Data':[[date1, value1],...]");
            }
            output.Dataset = "Generic Contaminant";
            output.DataSource = "User Input";
            return output;
        }
    }

}
