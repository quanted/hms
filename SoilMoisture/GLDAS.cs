using Data;
using System.Collections.Generic;

namespace SoilMoisture
{
    public class GLDAS
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
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            ITimeSeriesOutput gldasOutput = input.Output;
            List<ITimeSeriesOutput> layersData = new List<ITimeSeriesOutput>();
            List<string> urls = input.Input.BaseURL;
            for (int i = 0; i < input.Layers.Count; i++)
            {
                input.Input.BaseURL = new List<string>() { urls[i] };
                ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
                ITimeSeriesOutput tempOutput = new TimeSeriesOutput();
                tempOutput = oFactory.Initialize();
                List<string> data = gldas.GetData(out errorMsg, input.Layers[i].Replace('-', '_') + "_Soil_Moisture", input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }

                tempOutput = gldas.SetDataToOutput(out errorMsg, "SoilMoisture", data, tempOutput, input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }

                tempOutput = TemporalAggregation(out errorMsg, tempOutput, input.Input);
                if (errorMsg.Contains("ERROR")) { return null; }
                layersData.Add(tempOutput);
            }

            gldasOutput = NLDAS.MergeLayers(out errorMsg, layersData, "gldas");
            if (errorMsg.Contains("ERROR")) { return null; }

            return gldasOutput;
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
            output.Metadata.Add("gldas_temporalresolution", input.TemporalResolution);

            if (input.Units.Contains("imperial")) { output.Metadata["gldas_unit"] = "in"; }
            output.Data = (input.Units.Contains("imperial")) ? NLDAS.UnitConversion(out errorMsg, output, input) : output.Data;

            output.Metadata.Add("column_1", "Date");

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyValues(out errorMsg, output, input);
                    //output.Metadata.Add("column_2", "Daily Average");
                    return output;
                case "weekly":
                    output.Data = NLDAS.WeeklyValues(out errorMsg, output, input);
                    //output.Metadata.Add("column_2", "Weekly Average");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyValues(out errorMsg, output, input);
                    //output.Metadata.Add("column_2", "Monthly Average");
                    return output;
                default:
                    return output;
            }
        }

        /// <summary>
        /// Calls the function in Data.Source.GLDAS that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.GLDAS.CheckStatus("SoilMoisture", input);
        }
    }
}