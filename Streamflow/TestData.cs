using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streamflow
{
    public class TestData
    {

        public ITimeSeriesOutput<List<double>> GenerateData(out string errorMsg, ITimeSeriesOutput<List<double>> output, ITimeSeriesInput input)
        {
            errorMsg = "";

            Dictionary<string, string> inputs = new Dictionary<string, string>();
            inputs.Add("min", 0.1.ToString());
            inputs.Add("max", 25.0.ToString());
            inputs.Add("startDate", input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH"));
            inputs.Add("endDate", input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd HH"));
            inputs.Add("temporalResolution", input.TemporalResolution);
            string jsonInput = System.Text.Json.JsonSerializer.Serialize(inputs);

            ContaminantLoader.ContaminantLoader cl = new ContaminantLoader.ContaminantLoader("uniform", null, jsonInput);

            return cl.Result;

        }

        private ITimeSeriesOutput<List<double>> ConvertOutput(ITimeSeriesOutput output)
        {
            ITimeSeriesOutput<List<double>> convertedOutput = new TimeSeriesOutput<List<double>>();
            convertedOutput.Dataset = "Streamflow";
            convertedOutput.DataSource = "test data";
            convertedOutput.Metadata = output.Metadata;
            convertedOutput.Data = new Dictionary<string, List<double>>();
            foreach(KeyValuePair<string, List<string>> step in output.Data)
            {
                List<double> values = new List<double>();
                for(int i = 0; i < step.Value.Count; i++)
                {
                    values.Add(Double.Parse(step.Value[i]));
                }
                convertedOutput.Data.Add(step.Key, values);
            }
            return convertedOutput;
        }
    }
}
