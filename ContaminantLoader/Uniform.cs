using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using Data;
using System.Globalization;
using System.Linq;

namespace ContaminantLoader
{
    public class Uniform
    {

        /// <summary>
        /// Given a startDate, endDate, min, max, and temporalResolution value from a json formated imput 
        /// string, a uniform distribution of random values is generated.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GenerateUniformDistribution(string inputString)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();

            output.Dataset = "random_uniform_distribution";
            output.DataSource = "Random";

            dynamic jsonInput = null;
            double min = 0.0;
            double max = 0.0;
            string startDateString = null;
            string endDateString = null;
            DateTime startDate;
            DateTime endDate;
            string temporalResolution;
            int seed = 42;

            try
            {
                jsonInput = System.Text.Json.JsonDocument.Parse(inputString);
                min = jsonInput.min;
                max = jsonInput.max;
                startDateString = jsonInput.startDate;
                endDateString = jsonInput.endDate;
                temporalResolution = jsonInput.temporalResolution;
            }
            catch(System.Text.Json.JsonException ex){
                Utilities.ErrorOutput error = new Utilities.ErrorOutput();
                return error.ReturnError("JSON input error - " + ex.ToString());
            }
            try
            {
                startDate = DateTime.ParseExact(jsonInput.startDate.ToString(), "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(jsonInput.endDate.ToString(), "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
            }
            catch(FormatException ex)
            {
                Utilities.ErrorOutput error = new Utilities.ErrorOutput();
                return error.ReturnError("JSON date error - " + ex.ToString());
            }
            try
            {
                seed = int.Parse(jsonInput.seed.ToString());
            }
            catch(Exception e)
            {
                seed = 42;
            }

            output.Data = this.GetRandomDistribution(startDate, endDate, temporalResolution, seed, min, max);
            output.Metadata.Add("minValue", min.ToString());
            output.Metadata.Add("maxValue", max.ToString());
            return output;
        }

        /// <summary>
        /// Using the input values parsed from the json string, generates and returns a date and value time-series
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="temporalResolution"></param>
        /// <param name="seed"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetRandomDistribution(DateTime startDate, DateTime endDate, string temporalResolution, int seed, double min, double max)
        {
            Dictionary<string, List<string>> output = new Dictionary<string, List<string>>();
            Random rnd = new MersenneTwister(seed);
            IEnumerable<double> randomDistribution = ContinuousUniform.Samples(rnd, min, max);
            DateTime iDate = new DateTime();
            iDate = startDate;
            int i = 0;
            while(iDate.CompareTo(endDate) < 1)
            {
                string date = iDate.ToString("yyyy-MM-dd HH");
                double value = randomDistribution.ElementAt(i);
                List<string> valueList = new List<string>();
                valueList.Add(value.ToString("E3"));
                output.Add(date, valueList);
                switch (temporalResolution)
                {
                    case ("hourly"):
                    default:
                        iDate = iDate.AddHours(1);
                        break;
                    case ("3hourly"):
                        iDate = iDate.AddHours(3);
                        break;
                    case ("6hourly"):
                        iDate = iDate.AddHours(6);
                        break;
                    case ("daily"):
                        iDate = iDate.AddDays(1);
                        break;
                    case ("weekly"):
                        iDate = iDate.AddDays(7);
                        break;
                    case ("monthly"):
                        iDate = iDate.AddMonths(1);
                        break;
                    case ("yearly"):
                        iDate = iDate.AddYears(1);
                        break;
                }
                i++;
            }
            return output;
        }
    }
}
