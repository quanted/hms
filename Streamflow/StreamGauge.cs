using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Streamflow
{
    public class StreamGauge
    {

        /// <summary>
        /// Makes the GetData call to the base StreamGauge class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesOutput<List<double>> output, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";
            if (!input.Geometry.GeometryMetadata.ContainsKey("gaugestation"))
            {
                errorMsg = "Stream Gauge station id not found. 'gaugestation' required in Geometry MetaData input.";
                return null;
            }

            Data.Source.StreamGauge sg = new Data.Source.StreamGauge();
            List<string> data = sg.GetData(out errorMsg, input, retries);

            ITimeSeriesOutput<List<double>> nwisOutput = output.Clone();
            ITimeSeriesOutput test = new TimeSeriesOutput();
            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                output = (ITimeSeriesOutput<List<double>>)err.ReturnError("Streamflow", "nwis", errorMsg);
                errorMsg = "";
                return output ;
            }
            else
            {
                nwisOutput.Metadata = sg.SetMetadata(out errorMsg, input);
                // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
                if (input.Geometry.Timezone.Offset == 0)
                {
                    input.Geometry.Point = new PointCoordinate()
                    {
                        Latitude = Double.Parse(nwisOutput.Metadata["dec_lat_va"]),
                        Longitude = Double.Parse(nwisOutput.Metadata["dec_long_va"])
                    };
                    input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, input.Geometry.Point) as Timezone;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                nwisOutput = sg.SetDataToOutput(out errorMsg, data[0], output, input);
            }
            nwisOutput.Metadata.Add("timeseries_timezone", input.TimeLocalized ? "local": "GMT");
            nwisOutput.Metadata.Add("nwis_data_url", data[1]);
            switch (input.TemporalResolution)
            {
                case "hourly":
                    nwisOutput.Data = nwisOutput.ToHourly("yyyy-MM-dd HH", true);
                    nwisOutput.Metadata.Add("temporal_resolution", "hourly");
                    break;
                case "daily":
                    nwisOutput.Data = nwisOutput.ToDaily("yyyy-MM-dd HH", true);
                    nwisOutput.Metadata.Add("temporal_resolution", "daily");
                    break;
                case "monthly":
                    nwisOutput.Data = nwisOutput.ToMonthly("yyyy-MM-dd HH", true);
                    nwisOutput.Metadata.Add("temporal_resolution", "monthly");
                    break;
                default:
                    nwisOutput.Metadata.Add("temporal_resolution", "default");
                    break;
            }
            nwisOutput.Metadata.Add("units", "m^3/s");
            return nwisOutput;
        }
    }
}
