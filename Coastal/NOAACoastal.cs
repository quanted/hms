using Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Coastal
{
    public class NOAACoastal
    {
        private enum ValidCoastalProducts { water_level, conductivity, visibility, currents };

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
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, int retries = 0)
        {
            Data.Source.NOAACoastal noaaCoastal = new Data.Source.NOAACoastal();

            // Validate Coastal inputs
            ValidateInputs(input, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Get data and set to output
            ITimeSeriesOutput noaaCoastalOutput = noaaCoastal.GetData(out errorMsg, "Coastal", output, input, retries);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Prune data based on product and set correct metadata columns
            noaaCoastalOutput = SetDataColumns(noaaCoastalOutput, input);
  
            // Set the data to the correct temporal aggregation
            noaaCoastalOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return noaaCoastalOutput;
        }

        /// <summary>
        /// Sets the metadata columns based on product.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput SetDataColumns(ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            // Date is always first data column
            output.Metadata.Add("column_1", "Date " + input.Geometry.Timezone.Name);
            switch (input.Geometry.GeometryMetadata["product"])
            {
                case "water_level":
                    output.Metadata.Add("column_2", "Avg Water Level");
                    output.Metadata.Add("column_3", "Max Water Level");
                    output.Metadata.Add("column_4", "Min Water Level");
                    output.Metadata.Add("noaa_coastal_units", input.Units == "metric" ? "m" : "ft");
                    break;
                case "currents":
                    output.Metadata.Add("column_2", "Avg Speed");
                    output.Metadata.Add("column_3", "Max Speed");
                    output.Metadata.Add("column_4", "Min Speed");
                    output.Metadata.Add("column_5", "Avg Direction");
                    output.Metadata.Add("column_6", "Max Direction");
                    output.Metadata.Add("column_7", "Min Direction");
                    string unit = (input.Units == "metric") ? @"cm/s" : "knots";
                    output.Metadata.Add("column_2_units", unit);
                    output.Metadata.Add("column_3_units", unit);
                    output.Metadata.Add("column_4_units", unit);
                    output.Metadata.Add("column_5_units", "degrees");
                    output.Metadata.Add("column_6_units", "degrees");
                    output.Metadata.Add("column_7_units", "degrees");
                    break;
                default:
                    break;
            }
            return output;
        }

        /// <summary>
        /// Validate inputs for noaa coastal data.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private void ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";

            // Check that metadata is not null
            if(input.Geometry.GeometryMetadata.Count < 1)
            {
                errorMsg = "ERROR: Missing all required metadata for making coastal request.";
            }

            // Validate product
            if (input.Geometry.GeometryMetadata.ContainsKey("product"))
            {
                // Validate coastal sources.
                errorMsg = (!Enum.TryParse(input.Geometry.GeometryMetadata["product"], true, out ValidCoastalProducts _))
                    ? "ERROR: 'product' was not found or is invalid." : "";
            } 
            else
            {
                errorMsg = "ERROR: Missing required \"product\" key in metadata.";
            }
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
            string product = input.Geometry.GeometryMetadata["product"];
            switch (input.TemporalResolution.Trim().ToLower())
            {
                case "hourly": 
                    output.Data = HourlyAggregated(output, product);
                    return output;
                case "daily":
                    output.Data = DailyAggregated(output, product);
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregated(output, product);
                    return output;
                case "yearly":
                    output.Data = YearlyAggregated(output, product);
                    return output;
                // Default is hourly
                default:
                    output.Data = HourlyAggregated(output, product);
                    return output;
            }
        }

        /// <summary>
        /// Aggregates 6-minute interval data into hourly.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> HourlyAggregated(ITimeSeriesOutput output, string product)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();

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
                if ((current != previous || item.Key == endItem.Key)
                    && product != "currents")
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
                else if ((current != previous || item.Key == endItem.Key)
                    && product == "currents")
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

                    if (secondValueList.Count > 0)
                    {
                        values.Add(secondValueList.Average().ToString());
                        values.Add(secondValueList.Max().ToString());
                        values.Add(secondValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    data.Add(lastKey.Split(" ")[0], values);
                    firstValueList.Clear();
                    secondValueList.Clear();
                    previous = current;
                }
                // Day is still the same so keep appending values to lists
                else if (current == previous && product == "currents")
                {
                    // Add value to list
                    bool success1 = double.TryParse(item.Value[0], out double value1);
                    bool success2 = double.TryParse(item.Value[0], out double value2);
                    if (success1 && success2)
                    {
                        firstValueList.Add(value1);
                        secondValueList.Add(value2);
                    }
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
        private Dictionary<string, List<string>> DailyAggregated(ITimeSeriesOutput output, string product)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();

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
                if ((currentDay != previousDay || item.Key == endItem.Key)
                    && product != "currents")
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
                else if ((currentDay != previousDay || item.Key == endItem.Key)
                    && product == "currents")
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

                    if (secondValueList.Count > 0)
                    {
                        values.Add(secondValueList.Average().ToString());
                        values.Add(secondValueList.Max().ToString());
                        values.Add(secondValueList.Min().ToString());
                    }
                    else
                    {
                        values.Add("No Value");
                        values.Add("No Value");
                        values.Add("No Value");
                    }
                    data.Add(lastKey.Split(" ")[0], values);
                    firstValueList.Clear();
                    secondValueList.Clear();
                    previousDay = currentDay;
                }
                // Day is still the same so keep appending values to lists
                else if(currentDay == previousDay && product == "currents")
                {
                    // Add value to list
                    bool success1 = double.TryParse(item.Value[0], out double value1);
                    bool success2 = double.TryParse(item.Value[0], out double value2);
                    if (success1 && success2)
                    {
                        firstValueList.Add(value1);
                        secondValueList.Add(value2);
                    }
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
        private Dictionary<string, List<string>> MonthlyAggregated(ITimeSeriesOutput output, string product)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();

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
                if ((currentMonth != previousMonth || item.Key == endItem.Key)
                    && product != "currents")
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
                else if ((currentMonth != previousMonth || item.Key == endItem.Key)
                    && product == "currents")
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

                    if (secondValueList.Count > 0)
                    {
                        values.Add(secondValueList.Average().ToString());
                        values.Add(secondValueList.Max().ToString());
                        values.Add(secondValueList.Min().ToString());
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
                    secondValueList.Clear();
                    previousMonth = currentMonth;
                }
                // Day is still the same so keep appending values to lists
                else if (currentMonth == previousMonth && product == "currents")
                {
                    // Add value to list
                    bool success1 = double.TryParse(item.Value[0], out double value1);
                    bool success2 = double.TryParse(item.Value[0], out double value2);
                    if (success1 && success2)
                    {
                        firstValueList.Add(value1);
                        secondValueList.Add(value2);
                    }
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
        private Dictionary<string, List<string>> YearlyAggregated(ITimeSeriesOutput output, string product)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();

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
                if ((currentYear != previousYear || item.Key == endItem.Key) 
                    && product != "currents" )
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
                else if ((currentYear != previousYear || item.Key == endItem.Key)
                    && product == "currents")
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

                    if (secondValueList.Count > 0)
                    {
                        values.Add(secondValueList.Average().ToString());
                        values.Add(secondValueList.Max().ToString());
                        values.Add(secondValueList.Min().ToString());
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
                    secondValueList.Clear();
                    previousYear = currentYear;
                }
                // Day is still the same so keep appending values to lists
                else if (currentYear == previousYear && product == "currents")
                {
                    // Add value to list
                    bool success1 = double.TryParse(item.Value[0], out double value1);
                    bool success2 = double.TryParse(item.Value[0], out double value2);
                    if (success1 && success2)
                    {
                        firstValueList.Add(value1);
                        secondValueList.Add(value2);
                    }
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