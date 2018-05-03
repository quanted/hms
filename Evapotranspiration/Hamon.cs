using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using Data;

namespace Evapotranspiration
{
    public class Hamon
    {
        private double latitude;
        private double longitude;

        private int icounter = 0;
        private double mean1 = 0.0;

        public Hamon()
        {
            latitude = 33.925673;
            longitude = -83.355723;
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        public void HamonMethod(double mean, int jday, out double petHamon, out double dayLightHours, out string errorMsg)
        {
            petHamon = 0;
            dayLightHours = 0;
            errorMsg = "";

            double deg = latitude;  //  latitude is input from the GUI.

            double pi, esat, sunhours, number, angle, ACOS;
            double declination, sunsetangle;
            double pot_evap = 0.0;
            double MMEAN = 0.0;
            double JulianDay = Convert.ToDouble(jday);

            pi = Math.PI;
            const double IN_TO_CM = 2.54;


            try
            {
                number = (deg * pi / 180.0);

                if (Math.Abs(number) != 1)
                {
                    ACOS = (pi / 2.0) - Math.Atan(number / Math.Sqrt(1 - (number * number)));
                }


                if (number == -1.0) ACOS = pi;

                declination = (0.4093 * Math.Sin(2 * pi * JulianDay / 365.0 - 1.405));
                angle = (-Math.Tan(number) * Math.Tan(declination));
                sunsetangle = (pi / 2.0) - Math.Atan(angle / Math.Sqrt(1 - (angle * angle)));

                sunhours = 24.0 * sunsetangle / pi;
                dayLightHours = sunhours;
                esat = (0.6108 * Math.Exp(17.27 * mean / (237.3 + mean)));

                if ((icounter > 0) && (mean1 > 0.0) && (mean < 0.0)) //  Wilson Melendez: this part needs to be clarified with Yusuf.
                {
                    MMEAN = mean1;
                }
                //    else
                //  {
                //     mean = mean;
                //  } 

                icounter++;
                mean1 = mean;

                if (mean >= 0.0)
                    pot_evap = 0.21 * Math.Pow(sunhours, 2.0) * esat / (mean + 273.2);
                else
                    pot_evap = 0.21 * Math.Pow(sunhours, 2.0) * esat / (MMEAN + 273.2);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert cm/day to inches/day
            petHamon = pot_evap / IN_TO_CM;

        }

        public ITimeSeriesOutput Compute(double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";

            NLDAS2 nldas = new NLDAS2(lat, lon, startDate, endDate);
            double petHamon = 0;
            double sunshineHours = 0;

            DataTable dt = nldas.getData1(timeZoneOffset, out errorMsg);
           
            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            dt.Columns.Add("Sunshine_Hours");
            dt.Columns.Add("HamonPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "hamon";
            output.Metadata = new Dictionary<string, string>()
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Julian Day" },
                { "column_3", "Minimum Temperature" },
                { "column_4", "Maximum Temperature" },
                { "column_5", "Mean Temperature" },
                { "column_6", "Sunshine Hours" },
                { "column_7", "Potential Evapotranspiration" }
            };
            output.Data = new Dictionary<string, List<string>>();

            foreach (DataRow dr in dt.Rows)
            {
                double tmean = Convert.ToDouble(dr["TMean_C"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"].ToString());
                HamonMethod(tmean, jday, out petHamon, out sunshineHours, out errorMsg);
                dr["Sunshine_Hours"] = sunshineHours.ToString("F2", CultureInfo.InvariantCulture);
                dr["HamonPET_in"] = petHamon.ToString("F4", CultureInfo.InvariantCulture);
                List<string> lv = new List<string>();
                foreach (Object g in dr.ItemArray.Skip(1))
                {
                    lv.Add(g.ToString());
                }
                output.Data.Add(dr[0].ToString(), lv);
            }
            return output;
        }
    }
}