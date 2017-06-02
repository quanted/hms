using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Threading;
using System.IO;

namespace Data.Source
{
    /// <summary>
    /// Base NLDAS class
    /// </summary>
    public class NLDAS
    {
        /// <summary>
        /// Get data function for nldas.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="componentInput"></param>
        /// <returns></returns>
        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";

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
        private string ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                // Reading value from Application variables
                Dictionary<string, string> urls = (Dictionary<string, string>)HttpContext.Current.Application["urlList"];
                Dictionary<string, string> caselessUrls = new Dictionary<string, string>(urls, StringComparer.OrdinalIgnoreCase);
                sb.Append(caselessUrls[cInput.Source + "_" + dataset + "_URL"]);
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to load NLDAS url details from configuration file.\n" + ex.Message;
                return null;
            }

            //Add X and Y coordinates
            string[] xy = GetXYCoordinate(out errorMsg, cInput.Geometry.Point); // [0] = x, [1] = y
            if (errorMsg.Contains("ERROR")) { return null; }
            sb.Append("X" + xy[0] + "-" + "Y" + xy[1]);

            //Add Start and End Date
            string[] startDT = cInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH").Split(' ');
            string[] endDT = cInput.DateTimeSpan.EndDate.ToString("yyyy-MM-dd HH").Split(' ');
            sb.Append(@"&startDate=" + startDT[0] + @"T" + startDT[1] + @"&endDate=" + endDT[0] + "T" + endDT[1] + @"&type=asc2");
            
            return sb.ToString();
        }

        /// <summary>
        /// Downloads nldas data from nasa servers. If Http Request fails will retry up to 5 times.
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
                errorMsg = "ERROR: Unable to download requested nldas data.\n" + ex.Message;
                return null;
            }
            return data;
        }

        /// <summary>
        /// Converts latitude/longitude to X/Y values for NLDAS location.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="point">ICoordinate</param>
        /// <returns></returns>
        private string[] GetXYCoordinate(out string errorMsg, IPointCoordinate point)
        {
            errorMsg = "";
            double xMax = 463.0;
            double yMax = 223.0;
            double x, y = 0.0;
            string[] results = new string[2];
            x = (point.Longitude + 124.9375) / 0.125;
            y = (point.Latitude - 25.0625) / 0.125;
            if (x > xMax || x < 0)
            {
                errorMsg = "ERROR: Longitude value outside accepted range for NLDAS or is invalid. Provided longitude: " + point.Longitude.ToString() + "\n";
                return null;
            }
            if (y > yMax || y < 0)
            {
                errorMsg = "ERROR: Latitude value outside accepted range for NLDAS or is invalid. Provided latitude: " + point.Latitude.ToString() + "\n";
                return null;
            }
            results[0] = Convert.ToString(Math.Round(x, MidpointRounding.AwayFromZero));
            results[1] = Convert.ToString(Math.Round(y, MidpointRounding.AwayFromZero));
            return results;
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
            double step = 0.125;
            double x = (point.Longitude + 124.9375) / step;
            coord[1] = (Math.Round(x, MidpointRounding.AwayFromZero) * step) - 124.9375;
            double y = (point.Latitude - 25.0625) / step;
            coord[0] = (Math.Round(y, MidpointRounding.AwayFromZero) * step) + 25.0625;
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
            output.Data = SetData(out errorMsg, splitData[1].Substring(0, splitData[1].IndexOf("MEAN")).Trim());
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
                    output.Metadata["nldas_" + line[0]] = line[1];
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
        private Dictionary<string, List<string>> SetData(out string errorMsg, string data)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            List<string> timestepData;
            string[] tsLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tsLines.Length; i++)
            {
                timestepData = new List<string>();
                string[] lineData = tsLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                timestepData.Add(lineData[2]);
                dataDict[lineData[0] + " " + lineData[1]] = timestepData;
            }
            return dataDict;
            }

    }
}
