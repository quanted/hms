using Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Precipitation
{
    /// <summary>
    /// Base precipitation ncdc class.
    /// </summary>
    public class NCDC
    {
        /// <summary>
        /// Makes the GetData call to the base NCDC class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            Data.Source.NCDC ncdc = new Data.Source.NCDC();
            ITimeSeriesOutput ncdcOutput = output;

            // Make call to get station metadata and add to output.Metadata
            string token = (input.Geometry.GeometryMetadata.ContainsKey("token")) ? input.Geometry.GeometryMetadata["token"] : (string)HttpContext.Current.Application["ncdc_token"];
            ncdcOutput.Metadata = SetMetadata(out errorMsg, "ncdc", ncdc.GetStationDetails(out errorMsg, input.Geometry.GeometryMetadata["stationID"], token));
            ncdcOutput.Metadata.Add("ncdc_temporalResolution", input.TemporalResolution);
            ncdcOutput.Metadata.Add("ncdc_units", "mm");

            // Data aggregation takes place within ncdc.GetData
            Dictionary<string, double> data = ncdc.GetData(out errorMsg, "NCDC", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Set resulting data to output.Data
            ncdcOutput.Data = ConvertDict(out errorMsg, input.DataValueFormat, data);
            if (errorMsg.Contains("ERROR")) { return null; }

            ncdcOutput.DataSource = "ncdc";
            ncdcOutput.Dataset = "Precipitation";

            return ncdcOutput;
        }

        /// <summary>
        /// Converts the returned Dictionary into the appropriate format for ITimeSeriesOutput.Data
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> ConvertDict(out string errorMsg, string dataFormat, Dictionary<string, double> data)
        {
            errorMsg = "";

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach(var key in data)
            {
                result.Add(key.Key.ToString(), new List<string>() { data[key.Key.ToString()].ToString(dataFormat) });
            }
            return result;
        }

        /// <summary>
        /// Adds source_ to the metadata keys.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="source"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string source, Dictionary<string, string> metadata)
        {
            errorMsg = "";
            Dictionary<string, string> newMeta = new Dictionary<string, string>();
            foreach (var ele in metadata)
            {
                newMeta.Add(source + "_" + ele.Key, ele.Value);
            }
            return newMeta;
        }

    }
}
