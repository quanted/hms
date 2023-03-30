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
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

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

                Dictionary<string, object> comidGages = Utilities.COMID.GetGageID(input.Geometry.ComID, out errorMsg);
                if (comidGages.ContainsKey("GAGEID"))
                {
                    input.Geometry.GeometryMetadata.Add("gaugestation", comidGages["GAGEID"].ToString());
                    output.Metadata = Utilities.Metadata.MergeMetadata(output.Metadata, comidGages, "NWIS");
                }
                else
                {
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
                        output.Metadata.Add("NWIS_GAGE_NOTE", "Direct linkage between NWIS gage ID and NHDPlus catchment COMID was not found, gage determined from catchment bounds.");
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
                        output.Metadata.Add("gage_distance", Math.Round(min, 5).ToString() + "");
                        output.Metadata.Add("gage_distance_units", "(km) from catchment centroid to gauge latitude/longitude.");
                        output.Metadata.Add("NWIS_GAGE_NOTE", "Direct linkage between NWIS gage ID and NHDPlus catchment COMID was not found, gage determined from expanding bounds of catchment.");
                    }
                }
            }
            else if (!input.Geometry.GeometryMetadata.ContainsKey("gaugestation"))
            {
                errorMsg = "Stream Gauge station id not found. 'gaugestation' required in Geometry MetaData input.";
                return null;
            }
            else
            {
                int gageID = -1;
                try
                {
                    gageID = Int32.Parse(input.Geometry.GeometryMetadata["gaugestation"]);
                }
                catch (FormatException)
                {
                    errorMsg = "Invalid stream GageID provided in the geometry metadata for gaugestation. Value must be an integer.";
                    return null;
                }

                Dictionary<string, object> comidGages = Utilities.COMID.GetGageInfo(gageID, out errorMsg);
                if (comidGages.ContainsKey("GAGEID"))
                {
                    output.Metadata = Utilities.Metadata.MergeMetadata(output.Metadata, comidGages, "NWIS");
                }
            }

            List<string> data = sg.GetData(out errorMsg, input, retries);

            ITimeSeriesOutput<List<double>> nwisOutput = output.Clone();
            ITimeSeriesOutput test = new TimeSeriesOutput();
            if (errorMsg.Contains("ERROR"))
            {
                output = err.ReturnError<List<double>>("Streamflow", "nwis", errorMsg);
                errorMsg = "";
                return output ;
            }
            else if(data[0].Contains("No sites/data found"))
            {
                output = err.ReturnError<List<double>>("Streamflow", "nwis", data[0]);
                errorMsg = "";
                output.Metadata.Add("nwis_url", data[1]);
                return output;
            }
            else
            {
                nwisOutput.Metadata = sg.SetMetadata(out errorMsg, input, output.Metadata);
                // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
                if (input.Geometry.Timezone.Offset == 0)
                {
                    if (nwisOutput.Metadata.ContainsKey("NWIS_LatSite") && nwisOutput.Metadata.ContainsKey("NWIS_LonSite"))
                    {
                        input.Geometry.Point = new PointCoordinate()
                        {
                            Latitude = Double.Parse(nwisOutput.Metadata["NWIS_LatSite"]),
                            Longitude = Double.Parse(nwisOutput.Metadata["NWIS_LonSite"])
                        };
                    }
                    else
                    {
                        input.Geometry.Point = new PointCoordinate()
                        {
                            Latitude = Double.Parse(nwisOutput.Metadata["dec_lat_va"]),
                            Longitude = Double.Parse(nwisOutput.Metadata["dec_long_va"])
                        };
                    }
                    input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, input.Geometry.Point) as Timezone;
                    if (errorMsg.Contains("ERROR")) 
                    {
                        output = err.ReturnError<List<double>>("Streamflow", "nwis", errorMsg);
                        errorMsg = "";
                        return output;
                    }
                }
                nwisOutput = sg.SetDataToOutput(out errorMsg, data[0], output, input);
            }
            nwisOutput.Metadata.Add("timeseries_timezone", input.TimeLocalized ? "local": "GMT");
            nwisOutput.Metadata.Add("nwis_data_url", data[1]);
            if (!input.Geometry.GeometryMetadata.ContainsKey("fill"))
            {
                input.Geometry.GeometryMetadata["fill"] = "-9999";
            }

            switch (input.TemporalResolution)
            {
                case "hourly":
                    nwisOutput.Data = nwisOutput.ToHourly("yyyy-MM-dd HH", input, true);
                    nwisOutput.Metadata.Add("temporal_resolution", "hourly");
                    break;
                case "daily":
                    nwisOutput.Data = nwisOutput.ToDaily("yyyy-MM-dd HH", input, true);
                    nwisOutput.Metadata.Add("temporal_resolution", "daily");
                    break;
                case "monthly":
                    nwisOutput.Data = nwisOutput.ToMonthly("yyyy-MM-dd HH", input, true);
                    nwisOutput.Metadata.Add("temporal_resolution", "monthly");
                    break;
                default:
                    nwisOutput.Metadata.Add("temporal_resolution", "default");
                    break;
            }
            nwisOutput.Metadata.Add("units", "m^3/s");
            if(nwisOutput.Metadata.Any(kv => kv.Key.Contains("<!"))) 
            { 
                foreach(KeyValuePair<string, string> kv in nwisOutput.Metadata)
                {
                    if (kv.Key.Contains("<!"))
                    {
                        nwisOutput.Metadata.Remove(kv.Key);
                    }
                }
            }
            return nwisOutput;
        }
    }
}
