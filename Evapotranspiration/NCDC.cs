using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Evapotranspiration
{
    public class NCDCEntry
    {
        public string DATE { get; set; }
        public string STATION { get; set; }
        public double TMAX { get; set; }
        public double TMIN { get; set; }
        public double AWND { get; set; }
        public double PRCP { get; set; }
    }

    class NCDC
    {
        public DataTable DownloadData(ITimeSeriesOutput outpt, ITimeSeriesInput inpt)
        {
            string data = "";
            string errorMsg = "";
            string[] station = inpt.Geometry.GeometryMetadata["stationID"].ToString().Split(':');
            string url = "https://www.ncdc.noaa.gov/access-data-service/api/v1/data?dataset=daily-summaries&dataTypes=TMAX,TMIN,AWND,PRCP,EVAP&stations=" + station[1] + "&startDate=" + inpt.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + inpt.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&format=json&units=metric";
            WebClient myWC = new WebClient();
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

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
                errorMsg = "ERROR: Unable to download data from Daymet. " + ex.Message;
                return null;
            }

            List<NCDCEntry> datalist = JsonConvert.DeserializeObject<List<NCDCEntry>>(data);
            
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
    }
}
