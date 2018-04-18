using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Data.Source
{
    /// <summary>
    /// Base GLDAS class.
    /// </summary>
    public class GLDAS
    {
        /// <summary>
        /// Get data function for gldas.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset">nldas dataset parameter</param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";

            // Adjusts date/times by the timezone offset if timelocalized is set to true.
            componentInput.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, componentInput) as DateTimeSpan;

            // Constructs the url for the NLDAS data request and it's query string.
            string url = ConstructURL(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Uses the constructed url to download time series data.
            string data = DownloadData(out errorMsg, url);
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Constructs the url for retrieving nldas data based on the given parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(cInput.BaseURL[0]);

            //Add X and Y coordinates
            sb.Append(@"%28" + cInput.Geometry.Point.Longitude.ToString() + @",%20" + cInput.Geometry.Point.Latitude.ToString() + @"%29");

            //Add Start and End Date
            string[] startDT = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
            DateTime tempDate = cInput.DateTimeSpan.EndDate.AddHours(3);
            string[] endDT = tempDate.ToString("yyyy-MM-dd HH").Split(' ');
            sb.Append(@"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1] + @"&type=asc2");

            return sb.ToString();
        }

        /// <summary>
        /// Downloads gldas data from nasa servers. If Http Request fails will retry up to 5 times.
        /// TODO: Add in start date and end date check prior to the download call (Probably add to Validators class)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download requested gldas data.\n" + ex.Message;
                return null;
            }
            return data;
        }

        /// <summary>
        /// Determines the coordinate values that corresponds to the closest coordinate point in the NLDAS grid.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="point">ICoordinate</param>
        /// <returns>[Latitude, Longitude]</returns>
        public double[] DetermineReturnCoordinates(out string errorMsg, IPointCoordinate point)
        {
            errorMsg = "";
            double[] coord = new double[2];
            double step = step = 0.25;
            double x = (point.Longitude + 179.8750) / step;
            coord[1] = (Math.Round(x, MidpointRounding.AwayFromZero) * step) - 179.8750;
            double y = (point.Latitude + 59.8750) / step;
            coord[0] = (Math.Round(y, MidpointRounding.AwayFromZero) * step) - 59.8750;
            return coord;
        }

        /// <summary>
        /// Takes the data recieved from nldas and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            string[] splitData = data.Split(new string[] { "Data\n" }, StringSplitOptions.RemoveEmptyEntries);
            output.Dataset = dataset;
            output.DataSource = input.Source;
            output.Metadata = SetMetadata(out errorMsg, splitData[0], output);
            output.Data = SetData(out errorMsg, splitData[1], input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);
            //output.Data = SetData(out errorMsg, splitData[1].Substring(0, splitData[1].IndexOf("MEAN")).Trim(), input.TimeLocalized, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat, input.Geometry.Timezone);
            return output;
        }

        /// <summary>
        /// Parses data string from nldas and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string metaData, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = output.Metadata;
            string[] metaDataLines = metaData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < metaDataLines.Length - 1; i++)
            {
                if (metaDataLines[i].Contains("="))
                {
                    string[] line = metaDataLines[i].Split('=');
                    if (line[0].Contains("column"))
                    {
                        meta[line[0]] = line[1];
                    }
                    else
                    {
                        meta["gldas_" + line[0]] = line[1];
                    }
                }
            }
            return meta;
        }

        /// <summary>
        /// Parses data string from nldas and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetData(out string errorMsg, string data, bool localTime, string dateFormat, string dataFormat, ITimezone tzDetails)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            List<string> timestepData;
            double offset = (localTime == true) ? tzDetails.Offset : 0.0;
            string[] tsLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < tsLines.Length; i++)
            for (int i = 0; i < tsLines.Length-1; i++)
            {
                timestepData = new List<string>();
                //string[] lineData = tsLines[i].Split(new string[] { "T", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                string[] lineData = tsLines[i].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                timestepData.Add(Convert.ToDouble(lineData[2]).ToString(dataFormat));
                dataDict[NLDAS.SetDateToLocal(offset, lineData[0] + " " + lineData[1], dateFormat)] = timestepData;
            }
            return dataDict;
        }

        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the GLDAS endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            try
            {
                WebRequest wr = WebRequest.Create(ConstructURL(out string errorMsg, dataset, testInput));
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                string status = response.StatusCode.ToString();
                string description = response.StatusDescription;
                response.Close();
                return new Dictionary<string, string>()
                {
                    { "status", status },
                    { "description", description}
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>()
                {
                    { "status", "ERROR" },
                    { "description", ex.Message }
                };
            }
        }
    }
}
