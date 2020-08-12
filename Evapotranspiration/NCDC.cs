using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Utilities;

namespace Evapotranspiration
{
    public class NCDCEntry
    {
        public string DATE { get; set; }
        public string STATION { get; set; }
        [JsonConverter(typeof(JSON.DoubleConverter))]
        public double TMAX { get; set; }
        [JsonConverter(typeof(JSON.DoubleConverter))]

        public double TMIN { get; set; }
        [JsonConverter(typeof(JSON.DoubleConverter))]

        public double AWND { get; set; }
        [JsonConverter(typeof(JSON.DoubleConverter))]

        public double PRCP { get; set; }
    }

    class NCDC
    {
        public DataTable DownloadData(ITimeSeriesOutput outpt, ITimeSeriesInput inpt)
        {
            string data = "";
            string errorMsg = "";
            string[] station = inpt.Geometry.GeometryMetadata["stationID"].ToString().Split(':');
            string url = "https://www.ncei.noaa.gov/access/services/data/v1?dataset=daily-summaries&dataTypes=TMAX,TMIN,AWND,PRCP,EVAP&stations=" + station[1] + "&startDate=" + inpt.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + inpt.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&format=json&units=metric";
            WebClient myWC = new WebClient();
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download data from Daymet. " + ex.Message;
                return null;
            }
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JSON.DoubleConverter());
            List<NCDCEntry> datalist = JsonSerializer.Deserialize<List<NCDCEntry>>(data, options);

            string date = "";
            DataTable dtMinMax = new DataTable();
            dtMinMax.Columns.Add("Date");
            dtMinMax.Columns.Add("Julian_Day");
            dtMinMax.Columns.Add("TMin_C");
            dtMinMax.Columns.Add("TMax_C");
            dtMinMax.Columns.Add("TMean_C");

            DateTime startDate = inpt.DateTimeSpan.StartDate;
            int startYear = startDate.Year - 1;
            double elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
            int julianDayOfYear = 0;
            foreach (NCDCEntry obj in datalist)
            {
                DataRow dr = dtMinMax.NewRow();
                date = obj.DATE;
                dr["Date"] = date;
                dr["TMin_C"] = Convert.ToDouble(obj.TMIN);// - 273.15;
                dr["TMax_C"] = Convert.ToDouble(obj.TMAX);// - 273.15;
                dr["TMean_C"] = (Convert.ToDouble(obj.TMAX) + Convert.ToDouble(obj.TMIN)) / 2.0;
                startDate = Convert.ToDateTime(date);
                startYear = startDate.Year - 1;
                elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
                julianDayOfYear = Convert.ToInt32(Convert.ToDateTime(date).ToOADate() - elapsedJulianDate);
                dr["Julian_Day"] = julianDayOfYear;
                dtMinMax.Rows.Add(dr);
            }
            return dtMinMax;
        }

        public DataTable Aggregate(ITimeSeriesInput inpt, DataTable dt)
        {
            DataTable aggregated = dt.Clone();
            DataRow dr2 = null;
            List<Double> tList = new List<double>();
            if (inpt.TemporalResolution == "weekly")
            {
                int j = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (j == 0)
                    {
                        dr2 = aggregated.NewRow();
                        dr2["Date"] = dt.Rows[i]["Date"].ToString();
                        dr2["Julian_Day"] = dt.Rows[i]["Julian_Day"].ToString();
                        tList = new List<double>();
                    }
                    tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                    tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                    if (j == 6 || i == dt.Rows.Count - 1)
                    {
                        dr2["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                        dr2["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                        dr2["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                        aggregated.Rows.Add(dr2);
                        j = -1;
                    }
                    j++;
                }
                dt = aggregated;
            }
            else if (inpt.TemporalResolution == "monthly")
            {
                int curmonth = inpt.DateTimeSpan.StartDate.Month;
                int j = 0;
                bool newmonth = true;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (newmonth)
                    {
                        dr2 = aggregated.NewRow();
                        dr2["Date"] = dt.Rows[i]["Date"].ToString();
                        dr2["Julian_Day"] = dt.Rows[i]["Julian_Day"].ToString();
                        tList = new List<double>();
                        newmonth = false;
                        curmonth = DateTime.ParseExact(dt.Rows[i]["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).Month;
                    }
                    tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                    tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                    if (i + 1 < dt.Rows.Count && (DateTime.ParseExact(dt.Rows[i + 1]["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).Month != curmonth) || i == dt.Rows.Count - 1)
                    {
                        dr2["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                        dr2["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                        dr2["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                        aggregated.Rows.Add(dr2);
                        j = -1;
                        newmonth = true;
                    }
                    j++;
                }
                dt = aggregated;
            }
            else if (inpt.TemporalResolution == "default" || inpt.TemporalResolution == "daily")
            {
                return dt;
            }
            return dt;
        }
    }
}
