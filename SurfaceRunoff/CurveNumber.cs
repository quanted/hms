using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceRunoff
{
    /// <summary>
    /// SurfaceRunoff curve number class.
    /// </summary>
    class CurveNumber
    {

        /// <summary>
        /// GetData function for curvenumber.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            string data = cn.Simulate(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            precip.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            precip.Input.Geometry.GeometryMetadata.Add("stationID", "USW00013874");
            precip.Input.Geometry.GeometryMetadata.Add("token", "RUYNSTvfSvtosAoakBSpgxcHASBxazzP");
            ITimeSeriesOutput precipOutput = precip.GetData(out errorMsg);

            ITimeSeriesOutput cnOutput = getRunoffValue(precipOutput, data);
            cnOutput.Metadata.Remove(input.Source + "_timeZone");
            cnOutput.Metadata.Remove(input.Source + "_tz_offset");

            //TODO: validate temporal resolution function
            cnOutput = TemporalAggregation(out errorMsg, cnOutput, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return cnOutput;
        }

        public ITimeSeriesOutput getRunoffValue(ITimeSeriesOutput output, string data)
        {
            double converter = 1.0;
            if(output.DataSource == "gldas")
            {
                //kg/m^2/hr to in
                converter = 0.0393700787 / 24;
            }
            else
            {
                //mm to in (Daymet, NCDC, WGEN, Prism)
                //kg/m^2 to in (NLDAS)
                converter = 0.0393700787;  //1 mm = 0.039 in        1kg/m^2 = 1mm/day
            }

            foreach (var entry in output.Data)
            {
                string date = entry.Key;
                double P = Convert.ToDouble(entry.Value[0]) * converter;//call precip for P
                int CN = Convert.ToInt32(data);
                double S = (1000 / CN) - 10;//Calculate S using CN
                double runoff = Math.Pow((P - (0.2 * S)), 2) / (P + (0.8 * S));//runoff = (P - 0.2S)^2  / (P + 0.8S) in inches
                runoff = runoff * 25.4;//convert inches back to kg/m^2
                if(P <= 0.0)//if(P < (0.2 * S))
                {
                    runoff = 0.0;
                }
                entry.Value[0] = Math.Round(runoff, 5).ToString();
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
            //output.Metadata.Add("daymet_temporalresolution", input.TemporalResolution);
            //output.Metadata.Add("column_1", "Date");

            switch (input.TemporalResolution)
            {
                case "weekly":
                    output.Data = NLDAS.WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                case "daily":
                default:
                    output.Data = getDaymetDays(out errorMsg, output, input);
                    //output.Metadata.Add("column_2", "Daily Total");
                    return output;
            }
        }

        public static Dictionary<string, List<string>> getDaymetDays(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";         
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput newout = iFactory.Initialize();

            foreach (var entry in output.Data)//for (int i = inpt.DateTimeSpan.StartDate.DayOfYear-1; i < inpt.DateTimeSpan.EndDate.DayOfYear; i++)
            {
                DateTime date = new DateTime();
                string dateString = entry.Key.ToString().Substring(0, entry.Key.ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                Boolean julianflag = false;
                if (date.DayOfYear >= input.DateTimeSpan.StartDate.DayOfYear && date.Year == input.DateTimeSpan.StartDate.Year)
                {
                    julianflag = true;//newout.Data.Add(entry.Key, entry.Value);
                }
                if (date.DayOfYear > input.DateTimeSpan.EndDate.DayOfYear && date.Year == input.DateTimeSpan.EndDate.Year)
                {
                    julianflag = false;
                    break;//newout.Data.Add(entry.Key, entry.Value);
                }
                if (julianflag)
                {
                    newout.Data.Add(entry.Key, entry.Value);
                }
            }
            return newout.Data;
        }

    }
}