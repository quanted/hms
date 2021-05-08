using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wind
{
    class NOAACoastal
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
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, int retries = 0)
        {
            Data.Source.NOAACoastal noaaCoastal = new Data.Source.NOAACoastal();

            // Set metadata fields for url construction in data source
            input.Geometry.GeometryMetadata.Add("product", "wind");
            input.Geometry.GeometryMetadata.Add("datum", "");
            input.Geometry.GeometryMetadata.Add("application", "web_services");

            // Get data and set to output
            ITimeSeriesOutput noaaCoastalOutput = noaaCoastal.GetData(out errorMsg, "Wind", output, input, retries);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Prune data based on product and set correct metadata columns
            noaaCoastalOutput = SetDataColumns(noaaCoastalOutput, input);

            // Set the data to the correct temporal aggregation
            noaaCoastalOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return noaaCoastalOutput;
        }


        /// <summary>
        /// Sets the metadata columns.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput SetDataColumns(ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            // Date is always first data column
            output.Metadata.Add("column_1", "Date " + input.Geometry.Timezone.Name);
            output.Metadata.Add("column_2", "Avg Wind Speed");
            output.Metadata.Add("column_3", "Max Wind Speed");
            output.Metadata.Add("column_4", "Min Wind Speed");
            output.Metadata.Add("column_5", "Avg Wind Direction");
            output.Metadata.Add("column_6", "Max Wind Direction");
            output.Metadata.Add("column_7", "Min Wind Direction");
            output.Metadata.Add("column_8", "Avg Wind Gust");
            output.Metadata.Add("column_9", "Max Wind Gust");
            output.Metadata.Add("column_10", "Min Wind Gust");
            output.Metadata.Add("column_2_units", "m/s");
            output.Metadata.Add("column_3_units", "m/s");
            output.Metadata.Add("column_4_units", "m/s");
            output.Metadata.Add("column_5_units", "degrees");
            output.Metadata.Add("column_6_units", "degrees");
            output.Metadata.Add("column_7_units", "degrees");
            output.Metadata.Add("column_8_units", "m/s");
            output.Metadata.Add("column_9_units", "m/s");
            output.Metadata.Add("column_10_units", "m/s");
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
            List<double> secondValueList = new List<double>();
            List<double> thirdValueList = new List<double>();

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

                    if (thirdValueList.Count > 0)
                    {
                        values.Add(thirdValueList.Average().ToString());
                        values.Add(thirdValueList.Max().ToString());
                        values.Add(thirdValueList.Min().ToString());
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
                    // Add wind speed
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                    // Add wind direction
                    success = double.TryParse(item.Value[1], out value);
                    if (success)
                    {
                        secondValueList.Add(value);
                    }
                    // Add wind gust speed
                    success = double.TryParse(item.Value[3], out value);
                    if (success)
                    {
                        thirdValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }


        /// <summary>
        /// Aggregates 6-minute interval data into daily.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> DailyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();
            List<double> thirdValueList = new List<double>();

            // Get first hour and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previous = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[2]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current hour
                int current = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[2]);

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

                    if (thirdValueList.Count > 0)
                    {
                        values.Add(thirdValueList.Average().ToString());
                        values.Add(thirdValueList.Max().ToString());
                        values.Add(thirdValueList.Min().ToString());
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
                    thirdValueList.Clear();
                    previous = current;
                }
                else
                {
                    // Add wind speed
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                    // Add wind direction
                    success = double.TryParse(item.Value[1], out value);
                    if (success)
                    {
                        secondValueList.Add(value);
                    }
                    // Add wind gust speed
                    success = double.TryParse(item.Value[3], out value);
                    if (success)
                    {
                        thirdValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }

        /// <summary>
        /// Aggregates 6-minute interval data into monthly.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> MonthlyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();
            List<double> thirdValueList = new List<double>();

            // Get first hour and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previous = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[1]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current hour
                int current = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[1]);

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

                    if (thirdValueList.Count > 0)
                    {
                        values.Add(thirdValueList.Average().ToString());
                        values.Add(thirdValueList.Max().ToString());
                        values.Add(thirdValueList.Min().ToString());
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
                    thirdValueList.Clear();
                    previous = current;
                }
                else
                {
                    // Add wind speed
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                    // Add wind direction
                    success = double.TryParse(item.Value[1], out value);
                    if (success)
                    {
                        secondValueList.Add(value);
                    }
                    // Add wind gust speed
                    success = double.TryParse(item.Value[3], out value);
                    if (success)
                    {
                        thirdValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }


        /// <summary>
        /// Aggregates 6-minute interval data into yearly.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> YearlyAggregated(ITimeSeriesOutput output)
        {
            // Create new dictionary assign values and assign to output
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            List<double> firstValueList = new List<double>();
            List<double> secondValueList = new List<double>();
            List<double> thirdValueList = new List<double>();

            // Get first hour and set lastKey
            string lastKey = output.Data.Keys.ToArray()[0];
            KeyValuePair<string, List<string>> endItem = output.Data.Last();
            int previous = Convert.ToInt32(lastKey.Split(" ")[0].Split("-")[0]);
            foreach (KeyValuePair<string, List<string>> item in output.Data)
            {
                // Get the current hour
                int current = Convert.ToInt32(item.Key.Split(" ")[0].Split("-")[0]);

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

                    if (thirdValueList.Count > 0)
                    {
                        values.Add(thirdValueList.Average().ToString());
                        values.Add(thirdValueList.Max().ToString());
                        values.Add(thirdValueList.Min().ToString());
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
                    thirdValueList.Clear();
                    previous = current;
                }
                else
                {
                    // Add wind speed
                    bool success = double.TryParse(item.Value[0], out double value);
                    if (success)
                    {
                        firstValueList.Add(value);
                    }
                    // Add wind direction
                    success = double.TryParse(item.Value[1], out value);
                    if (success)
                    {
                        secondValueList.Add(value);
                    }
                    // Add wind gust speed
                    success = double.TryParse(item.Value[3], out value);
                    if (success)
                    {
                        thirdValueList.Add(value);
                    }
                }
                lastKey = item.Key;
            }
            return data;
        }
    }
}
