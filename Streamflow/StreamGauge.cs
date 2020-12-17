using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using GIS.Operations;

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
            Data.Source.StreamGauge sg = new Data.Source.StreamGauge();

            if (!input.Geometry.GeometryMetadata.ContainsKey("gaugestation") && input.Geometry.ComID > 0)
            {
                input.Geometry.Point = Utilities.COMID.GetCentroid(input.Geometry.ComID, out errorMsg);
                output.Metadata.Add("catchment_comid", input.Geometry.ComID.ToString());

                Catchment catchment = new Catchment(input.Geometry.ComID.ToString());
                if (catchment.data is null || catchment.data.features.Count == 0)
                {
                    errorMsg = "ERROR: Unable to get catchment data for COMID: " + input.Geometry.ComID.ToString();
                    return null;
                }

                foreach (KeyValuePair<string, object> kv in catchment.data.features[0].properties)
                {
                    output.Metadata.Add("NHDPlusV2_" + kv.Key, kv.Value.ToString());
                }

                double max = 1.0;
                Dictionary<string, Dictionary<string, string>> stations = sg.FindStation(catchment.GetBounds(), true, 0.0, 0.1, max);
                // If the catchment centroid is null the first station is selected, otherwise the station closest to the centroid.
                if (stations is null || stations.Count == 0)
                {
                    errorMsg = "ERROR: No stations found within a region expanded by " + max.ToString() + " degree(s) on the bounds of the catchment geometry";
                    return null;
                }
                else if (input.Geometry.Point is null)
                {
                    input.Geometry.GeometryMetadata.Add("gaugestation", stations.Keys.ToList()[0]);
                }
                else
                {
                    double min = 10000000.0;
                    string station = null;
                    foreach (KeyValuePair<string, Dictionary<string, string>> keyValue in stations)
                    {
                        double d = Operations.CalculateDistance(input.Geometry.Point.Latitude, input.Geometry.Point.Longitude, Double.Parse(keyValue.Value["dec_lat_va"]), Double.Parse(keyValue.Value["dec_long_va"]));
                        if (d < min)
                        {
                            min = d;
                            station = keyValue.Key;
                        }
                    }
                    input.Geometry.GeometryMetadata.Add("gaugestation", station);
                }
            }

            else if (!input.Geometry.GeometryMetadata.ContainsKey("gaugestation"))
            {
                errorMsg = "Stream Gauge station id not found. 'gaugestation' required in Geometry MetaData input.";
                return null;
            }

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
                nwisOutput.Metadata = sg.SetMetadata(out errorMsg, input, output.Metadata);
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
