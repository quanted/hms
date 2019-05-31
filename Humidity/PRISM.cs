using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humidity
{
    public class PRISM
    {
        /// <summary>
        /// Makes the GetData call to the base PRISM class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetRelativeHumidityData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.PRISM prism = new Data.Source.PRISM();

            // ----------------- Dew Point Temperature Block ------------------- //
            // Dew Point object
            DewPoint.DewPoint dPoint = new DewPoint.DewPoint();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            dPoint.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "dewpoint" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return null; }

            // Gets the dew point temperature data.
            ITimeSeriesOutput dewPointData = dPoint.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }


            // ----------------- Temperature Block ----------------------- //
            // Temperature object
            Temperature.Temperature temp = new Temperature.Temperature();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            temp.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "temperature" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return null; }

            // Gets the Temperature data.
            ITimeSeriesOutput temperatureData = temp.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput prismOutput = this.CalculateRelativeHumidity(out errorMsg, input, dewPointData, temperatureData);
            if (input.Geometry.GeometryMetadata.ContainsKey("evapo"))
            {
                prismOutput = this.CalculateMinMaxRelativeHumidity(out errorMsg, input, dewPointData, temperatureData);
            }
            if (errorMsg.Contains("ERROR")) { return null; }

            return prismOutput;
        }

        /// <summary>
        /// Calculate relative humidity using temperature and dew point data and the August-Roche-Magnus approximation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="dewPoint"></param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        private ITimeSeriesOutput CalculateRelativeHumidity(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput dewPoint, ITimeSeriesOutput temperature)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.DataSource = "prism";
            output.Dataset = "relative humidity";
            output.Data = new Dictionary<string, List<string>>();
            for(int i = 0; i < dewPoint.Data.Count; i++)
            {
                string date = dewPoint.Data.Keys.ElementAt(i);
                double temp = Double.Parse(temperature.Data.Values.ElementAt(i)[2]); // 0: max temp, 1: min temp, 2: mean temp
                double dew = Double.Parse(dewPoint.Data.Values.ElementAt(i)[0]);

                // August-Roche-Magnus approximation
                double value = 100 * (Math.Exp((17.625 * dew) / (243.04 + dew)) / Math.Exp((17.625 * temp) / (243.04 + temp)));
                output.Data.Add(date, new List<string> { value.ToString(input.DataValueFormat), temp.ToString(input.DataValueFormat), dew.ToString(input.DataValueFormat) });
            }
            output.Metadata = new Dictionary<string, string>();
            output.Metadata.Add("column_1", "date");
            output.Metadata.Add("column_2", "relative_humidity");
            output.Metadata.Add("column_3", "mean_temperature");
            output.Metadata.Add("column_4", "dew_point");
            output.Metadata.Add("algorithm", "August-Roche-Magnus appromixmation");
            return output;
        }

        /// <summary>
        /// Calculate MIN AND MAX relative humidity using temperature and dew point data and the August-Roche-Magnus approximation.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="dewPoint"></param>
        /// <param name="temperature"></param>
        /// <returns></returns>
        private ITimeSeriesOutput CalculateMinMaxRelativeHumidity(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput dewPoint, ITimeSeriesOutput temperature)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.DataSource = "prism";
            output.Dataset = "relative humidity";
            output.Data = new Dictionary<string, List<string>>();
            for (int i = 0; i < dewPoint.Data.Count; i++)
            {
                string date = dewPoint.Data.Keys.ElementAt(i);
                double mintemp = Double.Parse(temperature.Data.Values.ElementAt(i)[1]); // 0: max temp, 1: min temp, 2: mean temp
                double maxtemp = Double.Parse(temperature.Data.Values.ElementAt(i)[0]); // 0: max temp, 1: min temp, 2: mean temp
                double temp = Double.Parse(temperature.Data.Values.ElementAt(i)[2]); // 0: max temp, 1: min temp, 2: mean temp
                double dew = Double.Parse(dewPoint.Data.Values.ElementAt(i)[0]);

                // August-Roche-Magnus approximation
                double min = 100 * (Math.Exp((17.625 * dew) / (243.04 + dew)) / Math.Exp((17.625 * mintemp) / (243.04 + mintemp)));
                double max = 100 * (Math.Exp((17.625 * dew) / (243.04 + dew)) / Math.Exp((17.625 * maxtemp) / (243.04 + maxtemp)));
                output.Data.Add(date, new List<string> { min.ToString(input.DataValueFormat), max.ToString(input.DataValueFormat), mintemp.ToString(input.DataValueFormat), maxtemp.ToString(input.DataValueFormat), temp.ToString(input.DataValueFormat), dew.ToString(input.DataValueFormat) });
            }
            return output;
        }
    }
}
