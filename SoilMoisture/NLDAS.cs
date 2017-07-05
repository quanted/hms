using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoilMoisture
{
    public class NLDAS
    {
        /// <summary>
        /// Makes the GetData call to the base NLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, SoilMoisture input)
        {
            errorMsg = "";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            ITimeSeriesOutput nldasOutput = input.Output;
            List<ITimeSeriesOutput> layersData = new List<ITimeSeriesOutput>();
            List<string> urls = input.Input.BaseURL;
            for (int i = 0; i < input.Layers.Count; i++)
            {
                input.Input.BaseURL = new List<string>() { urls[i] };
                ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
                ITimeSeriesOutput tempOutput = new TimeSeriesOutput();
                tempOutput = oFactory.Initialize();
                string data = nldas.GetData(out errorMsg, input.Layers[i].Replace('-', '_') + "_SOILM", input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }

                tempOutput = nldas.SetDataToOutput(out errorMsg, "SoilMoisture", data, tempOutput, input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }

                tempOutput = TemporalAggregation(out errorMsg, tempOutput, input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }
                layersData.Add(tempOutput);
            }

            nldasOutput = MergeLayers(out errorMsg, layersData, "nldas");
            if (errorMsg.Contains("ERROR")) { return null; }

            return nldasOutput;
        }

        /// <summary>
        /// Merges all of the soil moisture layers data into a single ITimeSeriesOutput instance.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="layersData"></param>
        /// <returns></returns>
        public static ITimeSeriesOutput MergeLayers(out string errorMsg, List<ITimeSeriesOutput> layersData, string source)
        {
            errorMsg = "";
            ITimeSeriesOutput output = new TimeSeriesOutput();
            output.Data = new Dictionary<string, List<string>>();
            for( int i = 0; i < layersData[0].Data.Count; i++)
            {
                List<string> data = new List<string>();
                for (int j = 0; j < layersData.Count; j++)
                {
                    data.Add(layersData[j].Data[layersData[j].Data.Keys.ElementAt(i)][0].ToString());
                }
                output.Data.Add(layersData[0].Data.Keys.ElementAt(i), data);
            }
            output.DataSource = source;
            output.Dataset = "Soil Moisture";
            output.Metadata = layersData[0].Metadata;
            for (int i = 0; i < layersData.Count; i++)
            {
                string layerName = layersData[i].Metadata[source + "_param_short_name"].Substring(layersData[i].Metadata[source + "_param_short_name"].IndexOf('M') + 1);
                output.Metadata.Add("column_" + (i + 2), layerName);
            }

            return output;
        }

        /// <summary>
        /// Checks temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            if (!output.Metadata.ContainsKey("nldas_temporalresolution"))
            {
                output.Metadata.Add("nldas_temporalresolution", input.TemporalResolution);
            };

            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }
            output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, output, input) : output.Data;

            if (!output.Metadata.ContainsKey("column_1"))
            {
                output.Metadata.Add("column_1", "Date");
            };
            

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = DailyValues(out errorMsg, output, input);
                    output.Metadata.Add("column_2", "Daily Average");
                    return output;
                case "weekly":
                    output.Data = WeeklyValues(out errorMsg, output, input);
                    output.Metadata.Add("column_2", "Weekly Average");
                     return output;
                case "monthly":
                    output.Data = MonthlyValues(out errorMsg, output, input);
                    output.Metadata.Add("column_2", "Monthly Average");
                    return output;
                default:
                    return output;
            }
        }

        /// <summary>
        /// Converts metric ldas kg/m**2 (mm) units to imperial inches units.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> UnitConversion(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Unit conversion coefficient
            double unit = 0.0393701;
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                    ( unit * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0])).ToString(input.DataValueFormat)
                });
            }
            return tempData;
        }

        /// <summary>
        /// Gets daily temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double sum = 0.0;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Day != iDate.Day)
                {
                    double average = 0.0;
                    average = sum / dayIndex;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum = value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum += value;
                    dayIndex++;
                }
            }
            return tempData;
        }

        /// <summary>
        /// Gets weekly temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> WeeklyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double sum = 0.0;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    double average = 0.0;
                    average = sum / dayIndex;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]); ;
                    sum = value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum += value;
                    dayIndex++;
                }
            }
            return tempData;
        }

        /// <summary>
        /// Gets monthly temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double sum = 0.0;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month)
                {
                    double average = 0.0;
                    average = sum / dayIndex;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum = value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum += value;
                    dayIndex++;
                }
            }
            return tempData;
        }
    }
}
