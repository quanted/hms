using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiation
{
    public class GLDAS
    {

        private Dictionary<string, ITimeSeriesOutput> timeseriesData;


        /// <summary>
        /// Makes the GetData call to the base GLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (!validInputs){ return null; }

            this.timeseriesData = new Dictionary<string, ITimeSeriesOutput>();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output1 = oFactory.Initialize();
            ITimeSeriesOutput output2 = oFactory.Initialize();
            this.GetLongwaveComponent(out errorMsg, input, output1);
            this.GetShortwaveComponent(out errorMsg, input, output2);
            output = Utilities.Merger.MergeTimeSeries(this.timeseriesData["longwave"], this.timeseriesData["shortwave"]);

            output.Dataset = "DW Radiation";
            output.DataSource = "gldas";

            switch (input.TemporalResolution)
            {
                case "monthly":
                    output.Data = NLDAS.DailyAverage(out errorMsg, 7, 1.0, output, input);
                    output.Data = NLDAS.MonthlyAverage(out errorMsg, 7, 1.0, output, input);
                    break;
                case "daily":
                    output.Data = NLDAS.DailyAverage(out errorMsg, 7, 1.0, output, input);
                    break;
                case "default":
                default:
                    break;
            }
            output.Metadata["column_1"] = "date";
            output.Metadata["column_2"] = "longwave";
            output.Metadata["column_3"] = "shortwave";
            output.Metadata["column_2_units"] = "W/m^2";
            output.Metadata["column_3_units"] = "W/m^2";

            return output;

        }

        private void GetLongwaveComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            string title = "DW Longwave";
            input.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL(input.Source, "longwave_radiation") };
            input.Source = title;
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, title, input);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput gldasOutput = output.Clone();
            gldasOutput = gldas.SetDataToOutput(out errorMsg, title, data, output, input);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("longwave", gldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private void GetShortwaveComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            string title = "DW Shortwave";
            ITimeSeriesInput tempInput = input.Clone(new List<string>() { "radiation" });
            tempInput.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL("gldas", "shortwave_radiation") };
            tempInput.Source = title;
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, title, tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput gldasOutput = output.Clone();
            gldasOutput = gldas.SetDataToOutput(out errorMsg, title, data, output, tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("shortwave", gldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private Boolean ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // GLDAS 2.0 date range 1948-01-01 - 2010-12-31
            DateTime date0 = new DateTime(1948, 1, 1);
            DateTime tempDate = DateTime.Now;
            DateTime date1 = new DateTime(tempDate.Year, tempDate.Month, 1).AddMonths(-2);
            //DateTime date1 = new DateTime(2010, 12, 31);
            string dateFormat = "yyyy-MM-dd";
            if (DateTime.Compare(input.DateTimeSpan.StartDate, date0) < 0 || (DateTime.Compare(input.DateTimeSpan.StartDate, date1) > 0))
            {
                errors.Add("ERROR: Start date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". Start date provided: " + input.DateTimeSpan.StartDate.ToString(dateFormat));
            }
            if (DateTime.Compare(input.DateTimeSpan.EndDate, date0) < 0 ||DateTime.Compare(input.DateTimeSpan.EndDate, date1) > 0)
            {
                errors.Add("ERROR: End date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". End date provided: " + input.DateTimeSpan.EndDate.ToString(dateFormat));
            }

            // Validate Spatial range
            // GLDAS 2.0 spatial range 180W ~ 180E, 60S ~ 90N
            if (input.Geometry.Point.Latitude < -60 && input.Geometry.Point.Latitude > 90)
            {
                errors.Add("ERROR: Latitude is not valid. Latitude must be between -60 and 90. Latitude provided: " + input.Geometry.Point.Latitude.ToString());
            }
            if (input.Geometry.Point.Longitude < -180 && input.Geometry.Point.Longitude > 180) {
                errors.Add("ERROR: Longitude is not valid. Longitude must be between -180 and 180. Longitude provided: " + input.Geometry.Point.Longitude.ToString());
            }

            if(errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }

    }
}
