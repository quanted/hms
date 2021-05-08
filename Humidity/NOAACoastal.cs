using Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Humidity
{
    public class NOAACoastal
    {
        /// <summary>
        /// Default Coastal constructor
        /// </summary>
        public NOAACoastal()
        { }

        /// <summary>
        /// Makes the GetData call to the base NOAACoastal class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, bool Relative, int retries = 0)
        {
            Data.Source.NOAACoastal noaaCoastal = new Data.Source.NOAACoastal();

            // Check input
            ValidateInput(out errorMsg, input, Relative);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Set metadata fields for url construction
            input.Geometry.GeometryMetadata.Add("product", "humidity");
            input.Geometry.GeometryMetadata.Add("datum", "");
            input.Geometry.GeometryMetadata.Add("application", "web_services");

            // Get data and set to output
            ITimeSeriesOutput noaaCoastalOutput = noaaCoastal.GetData(out errorMsg, "Humidity", output, input, retries);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Prune data based on product and set correct metadata columns
            noaaCoastalOutput = SetDataColumns(noaaCoastalOutput, input);

            // Set the data to the correct temporal aggregation
            noaaCoastalOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return noaaCoastalOutput;
        }

        private void ValidateInput(out string errorMsg, ITimeSeriesInput input, bool Relative)
        {
            // Must have relative
            errorMsg = Relative ? "" : "ERROR: Specific humidity not supported by the noaa_coastal data source.";
        }

        /// <summary>
        /// Sets the metadata columns.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput SetDataColumns(ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            // Date is always first data column
            output.Metadata.Add("column_1", "Date");
            output.Metadata.Add("column_2", "Avg Humidity");
            output.Metadata.Add("column_3", "Max Humidity");
            output.Metadata.Add("column_4", "Min Humidity");
            output.Metadata.Add("noaa_coastal_units", "percent");
            return output;
        }

        /// <summary>
        /// Checks temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        ///
        public ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            switch (input.TemporalResolution.Trim().ToLower())
            {
                case "hourly":
                    output.Data = HourlyAggregated(output);
                    return output;
                case "daily":
                    output.Data = DailyAggregated(output);
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregated(output);
                    return output;
                case "yearly":
                    output.Data = YearlyAggregated(output);
                    return output;
                // Default is hourly
                default:
                    output.Data = HourlyAggregated(output);
                    return output;
            }
        }

        /// <summary>
        /// Aggregates 6-minute interval data into hourly.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> HourlyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();

            // Get first hour and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previous = Convert.ToInt32(lastKey.Split(" ")[1].Split(":")[0]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current hour
                int current = Convert.ToInt32(item.Key.Split(" ")[1].Split(":")[0]);

                // If days are different then we have a new day. Set values to data dict and 
                // set previous day to current day.
                if (current != previous || item.Key == endItem.Key)
                {
                    List<string> values = new List<string>();
                    if (firstValueList.Count > 0)
                    {
                        values.Add(firstValueList.Average().ToString());
                        values.Add(firstValueList.Max().ToString());
                        values.Add(firstValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    data.Add(lastKey.Split(":")[0], values);
                    firstValueList.Clear();
                    previous = current;
                }
                else
                {
                    // Add value to list
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }


        /// <summary>
        /// Aggregates 6-minute interval data into daily data.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> DailyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();

            // Get first day and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previousDay = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[2]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current day
                int currentDay = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[2]);

                // If days are different then we have a new day. Set values to data dict and 
                // set previous day to current day.
                if (currentDay != previousDay || item.Key == endItem.Key)
                {
                    List<string> values = new List<string>();
                    if (firstValueList.Count > 0)
                    {
                        values.Add(firstValueList.Average().ToString());
                        values.Add(firstValueList.Max().ToString());
                        values.Add(firstValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    data.Add(lastKey.Split(" ")[0], values);
                    firstValueList.Clear();
                    previousDay = currentDay;
                }
                else
                {
                    // Add value to list
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }


        /// <summary>
        /// Aggregates 6-minute interval data into monthly data.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> MonthlyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();

            // Get first month and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previousMonth = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[1]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current month
                int currentMonth = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[1]);

                // If months are different then we have a new month. Set values to data dict and 
                // set previous month to current month.
                if (currentMonth != previousMonth || item.Key == endItem.Key)
                {
                    List<string> values = new List<string>();
                    if (firstValueList.Count > 0)
                    {
                        values.Add(firstValueList.Average().ToString());
                        values.Add(firstValueList.Max().ToString());
                        values.Add(firstValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    string[] date = lastKey.Split(" ")[0].Split("-");
                    data.Add(date[0] + "-" + date[1], values);
                    firstValueList.Clear();
                    previousMonth = currentMonth;
                }
                else
                {
                    // Add value to list
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }



        /// <summary>
        /// Aggregates 6-minute interval data into yearly data.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> YearlyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();

            // Get first year and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previousYear = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[0]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current month
                int currentYear = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[0]);

                // If months are different then we have a new month. Set values to data dict and 
                // set previous month to current month.
                if (currentYear != previousYear || item.Key == endItem.Key)
                {
                    List<string> values = new List<string>();
                    if (firstValueList.Count > 0)
                    {
                        values.Add(firstValueList.Average().ToString());
                        values.Add(firstValueList.Max().ToString());
                        values.Add(firstValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    string[] date = lastKey.Split(" ")[0].Split("-");
                    data.Add(date[0], values);
                    firstValueList.Clear();
                    previousYear = currentYear;
                }
                else
                {
                    // Add value to list
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }
    }
}