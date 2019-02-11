using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Data;

namespace Utilities
{
    public class Utility
    {
        public static string errorMessage = "";
        const double minLat = 25.0625;
        const double maxLat = 52.9375;
        const double minLon = -124.9375;
        const double maxLon = -67.0625;
        const double minCLon = 67.0625;
        const double maxCLon = 124.9375;
        const double minLatG = -59.875;
        const double maxLatG = 90.0;
        const double minLonG = -179.875;
        const double maxLonG = 180.0;
        const double minAlb = 0.0;
        const double maxAlb = 1.0;
        const double minSA = 0.001;
        const double maxSA = 82103.0;
        const double minLD = 0.01;
        const double maxLD = 594;
        const double minSAngle = 0.0;
        const double maxSAngle = 90.0;
        const double minAnnualPrecip = 0.0;
        const double maxAnnualPrecip = 10000;
        const double minAZenith = 0.0;
        const double maxAZenith = 1.0;
        const double minEmissivity = 0.0;
        const double maxEmissivity = 1.0;
        const double minRSS = 0.0;      // Units are in s/m
        const double maxRSS = 1000.0;   // Units are in s/m
        const double minRST = 0.0;     // Units are in s/m
        const double maxRST = 500.0;    // Units are in s/m
        const double minZog = 0.0;  // Units are in m
        const double maxZog = 3.0;  // Units are in m
        const double minLAI = 0.0;  // dimensionless
        const double maxLAI = 10.0;  // dimensionless
        const double minWLeaf = 0.001;
        const double maxWLeaf = 1.0;
        const double minVegHeight = 0.0;  // Units are in m
        const double maxVegHeight = 120.0;  // Units are in m
        public const string WSBaseURL = @"http://localhost:60607/api/";

        /*
        public static bool IsValidData(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon, TextBox txtSDate, string nameSDate,
                                TextBox txtEDate, string nameEDate)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsEndDateLaterStartDate(txtSDate, nameSDate, txtEDate, nameEDate);
            return result;
        }

        public static bool IsValidDataG(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon, TextBox txtSDate, string nameSDate,
                                TextBox txtEDate, string nameEDate)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLatG, maxLatG) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLonG, maxLonG) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsEndDateLaterStartDate(txtSDate, nameSDate, txtEDate, nameEDate);
            return result;
        }

        public static bool IsValidData(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon, TextBox txtSDate, string nameSDate,
                                TextBox txtEDate, string nameEDate, TextBox txtAlb, string nameAlb)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb);
            return result;
        }

        public static bool IsValidData(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon, TextBox txtSDate, string nameSDate,
                                TextBox txtEDate, string nameEDate, TextBox txtAlb, string nameAlb, TextBox txtSA, string nameSA,
                                TextBox txtLD, string nameLD,
                                TextBox txtFactorJan, string nameFactorJan, TextBox txtFactorFeb, string nameFactorFeb,
                                TextBox txtFactorMar, string nameFactorMar, TextBox txtFactorApr, string nameFactorApr,
                                TextBox txtFactorMay, string nameFactorMay, TextBox txtFactorJun, string nameFactorJun,
                                TextBox txtFactorJul, string nameFactorJul, TextBox txtFactorAug, string nameFactorAug,
                                TextBox txtFactorSep, string nameFactorSep, TextBox txtFactorOct, string nameFactorOct,
                                TextBox txtFactorNov, string nameFactorNov, TextBox txtFactorDec, string nameFactorDec)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb) &&
                     IsPresent(txtSA, nameSA) && IsDouble(txtSA, nameSA) && IsWithinRange(txtSA, nameSA, minSA, maxSA) &&
                     IsPresent(txtLD, nameLD) && IsDouble(txtLD, nameLD) && IsWithinRange(txtLD, nameLD, minLD, maxLD) &&
                     IsPresent(txtFactorJan, nameFactorJan) && IsDouble(txtFactorJan, nameFactorJan) &&
                     IsPresent(txtFactorFeb, nameFactorFeb) && IsDouble(txtFactorFeb, nameFactorFeb) &&
                     IsPresent(txtFactorMar, nameFactorMar) && IsDouble(txtFactorMar, nameFactorMar) &&
                     IsPresent(txtFactorApr, nameFactorApr) && IsDouble(txtFactorApr, nameFactorApr) &&
                     IsPresent(txtFactorMay, nameFactorMay) && IsDouble(txtFactorMay, nameFactorMay) &&
                     IsPresent(txtFactorJun, nameFactorJun) && IsDouble(txtFactorJun, nameFactorJun) &&
                     IsPresent(txtFactorJul, nameFactorJul) && IsDouble(txtFactorJul, nameFactorJul) &&
                     IsPresent(txtFactorAug, nameFactorAug) && IsDouble(txtFactorAug, nameFactorAug) &&
                     IsPresent(txtFactorSep, nameFactorSep) && IsDouble(txtFactorSep, nameFactorSep) &&
                     IsPresent(txtFactorOct, nameFactorOct) && IsDouble(txtFactorOct, nameFactorOct) &&
                     IsPresent(txtFactorNov, nameFactorNov) && IsDouble(txtFactorNov, nameFactorNov) &&
                     IsPresent(txtFactorDec, nameFactorDec) && IsDouble(txtFactorDec, nameFactorDec);
            return result;
        }

        public static bool IsValidData(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon,
                                TextBox txtSDate, string nameSDate, TextBox txtEDate, string nameEDate,
                                TextBox txtAlb, string nameAlb, TextBox txtCLon, string nameCLon, TextBox txtSAngle, string nameSAngle)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb) &&
                     IsPresent(txtCLon, nameCLon) && IsDouble(txtCLon, nameCLon) && IsWithinRange(txtCLon, nameCLon, minCLon, maxCLon) &&
                     IsPresent(txtSAngle, nameSAngle) && IsDouble(txtSAngle, nameSAngle) && IsWithinRange(txtSAngle, nameSAngle, minSAngle, maxSAngle);
            return result;
        }

        public static bool IsValidDataMCRAE(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon,
                                TextBox txtSDate, string nameSDate, TextBox txtEDate, string nameEDate,
                                TextBox txtAlb, string nameAlb, TextBox txtEmi, string nameEmi)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb) &&
                     IsPresent(txtEmi, nameEmi) && IsDouble(txtEmi, nameEmi) && IsWithinRange(txtEmi, nameEmi, minEmissivity, maxEmissivity);
            return result;
        }

        public static bool IsValidDataMCRWE(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon,
                                TextBox txtSDate, string nameSDate, TextBox txtEDate, string nameEDate,
                                TextBox txtAlb, string nameAlb, TextBox txtAZZ, string nameAZZ, TextBox txtEmi, string nameEmi)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb) &&
                     IsPresent(txtAZZ, nameAZZ) && IsDouble(txtAZZ, nameAZZ) && IsWithinRange(txtAZZ, nameAZZ, minAZenith, maxAZenith) &&
                     IsPresent(txtEmi, nameEmi) && IsDouble(txtEmi, nameEmi) && IsWithinRange(txtEmi, nameEmi, minEmissivity, maxEmissivity);
            return result;
        }

        public static bool IsValidDataSW(TextBox txtLat, string nameLat, TextBox txtLon, string nameLon,
                                 TextBox txtSDate, string nameSDate, TextBox txtEDate, string nameEDate,
                                 TextBox txtAlb, string nameAlb, TextBox txtRss, string nameRss, TextBox txtRst, string nameRst,
                                 TextBox txtZog, string nameZog, TextBox txtLAIJan, string nameLaiJan,
                                 TextBox txtLAIFeb, string nameLaiFeb, TextBox txtLAIMar, string nameLaiMar,
                                 TextBox txtLAIApr, string nameLaiApr, TextBox txtLAIMay, string nameLaiMay,
                                 TextBox txtLAIJun, string nameLaiJun, TextBox txtLAIJul, string nameLaiJul,
                                 TextBox txtLAIAug, string nameLaiAug, TextBox txtLAISep, string nameLaiSep,
                                 TextBox txtLAIOct, string nameLaiOct, TextBox txtLAINov, string nameLaiNov,
                                 TextBox txtLAIDec, string nameLaiDec,
                                 TextBox txtWleaf, string nameWleaf, TextBox txtVegHeight, string nameVegHeight)
        {
            bool result;
            result = IsPresent(txtLat, nameLat) && IsDouble(txtLat, nameLat) && IsWithinRange(txtLat, nameLat, minLat, maxLat) &&
                     IsPresent(txtLon, nameLon) && IsDouble(txtLon, nameLon) && IsWithinRange(txtLon, nameLon, minLon, maxLon) &&
                     IsPresent(txtSDate, nameSDate) && IsDate(txtSDate, nameSDate) && IsDateWithinRange(txtSDate, nameSDate) &&
                     IsPresent(txtEDate, nameEDate) && IsDate(txtEDate, nameEDate) && IsDateWithinRange(txtEDate, nameEDate) &&
                     IsPresent(txtAlb, nameAlb) && IsDouble(txtAlb, nameAlb) && IsWithinRange(txtAlb, nameAlb, minAlb, maxAlb) &&
                     IsPresent(txtRss, nameRss) && IsDouble(txtRss, nameRss) && IsWithinRange(txtRss, nameRss, minRSS, maxRSS) &&
                     IsPresent(txtRst, nameRst) && IsDouble(txtRst, nameRst) && IsWithinRange(txtRst, nameRst, minRST, maxRST) &&
                     IsPresent(txtZog, nameZog) && IsDouble(txtZog, nameZog) && IsWithinRange(txtZog, nameZog, minZog, maxZog) &&
                     IsPresent(txtLAIJan, nameLaiJan) && IsDouble(txtLAIJan, nameLaiJan) && IsWithinRange(txtLAIJan, nameLaiJan, minLAI, maxLAI) &&
                     IsPresent(txtLAIFeb, nameLaiFeb) && IsDouble(txtLAIFeb, nameLaiFeb) && IsWithinRange(txtLAIFeb, nameLaiFeb, minLAI, maxLAI) &&
                     IsPresent(txtLAIMar, nameLaiMar) && IsDouble(txtLAIMar, nameLaiMar) && IsWithinRange(txtLAIMar, nameLaiMar, minLAI, maxLAI) &&
                     IsPresent(txtLAIApr, nameLaiApr) && IsDouble(txtLAIApr, nameLaiApr) && IsWithinRange(txtLAIApr, nameLaiApr, minLAI, maxLAI) &&
                     IsPresent(txtLAIMay, nameLaiMay) && IsDouble(txtLAIMay, nameLaiMay) && IsWithinRange(txtLAIMay, nameLaiMay, minLAI, maxLAI) &&
                     IsPresent(txtLAIJun, nameLaiJun) && IsDouble(txtLAIJun, nameLaiJun) && IsWithinRange(txtLAIJun, nameLaiJun, minLAI, maxLAI) &&
                     IsPresent(txtLAIJul, nameLaiJul) && IsDouble(txtLAIJul, nameLaiJul) && IsWithinRange(txtLAIJul, nameLaiJul, minLAI, maxLAI) &&
                     IsPresent(txtLAIAug, nameLaiAug) && IsDouble(txtLAIAug, nameLaiAug) && IsWithinRange(txtLAIAug, nameLaiAug, minLAI, maxLAI) &&
                     IsPresent(txtLAISep, nameLaiSep) && IsDouble(txtLAISep, nameLaiSep) && IsWithinRange(txtLAISep, nameLaiSep, minLAI, maxLAI) &&
                     IsPresent(txtLAIOct, nameLaiOct) && IsDouble(txtLAIOct, nameLaiOct) && IsWithinRange(txtLAIOct, nameLaiOct, minLAI, maxLAI) &&
                     IsPresent(txtLAINov, nameLaiNov) && IsDouble(txtLAINov, nameLaiNov) && IsWithinRange(txtLAINov, nameLaiNov, minLAI, maxLAI) &&
                     IsPresent(txtLAIDec, nameLaiDec) && IsDouble(txtLAIDec, nameLaiDec) && IsWithinRange(txtLAIDec, nameLaiDec, minLAI, maxLAI) &&
                     IsPresent(txtWleaf, nameWleaf) && IsDouble(txtWleaf, nameWleaf) && IsWithinRange(txtWleaf, nameWleaf, minWLeaf, maxWLeaf) &&
                     IsPresent(txtVegHeight, nameVegHeight) && IsDouble(txtVegHeight, nameVegHeight) && IsWithinRange(txtVegHeight, nameVegHeight, minVegHeight, maxVegHeight);
            return result;
        }



        public static bool IsPresent(TextBox textBox, string name)
        {
            if (textBox.Text == "")
            {
                errorMessage = name + " is a required field.";
                textBox.Focus();
                return false;
            }
            return true;
        }

        public static bool IsDouble(TextBox textBox, string name)
        {
            double number = 0;
            if (double.TryParse(textBox.Text, out number))
            {
                return true;
            }
            else
            {
                errorMessage = name + " must be a double value.";
                textBox.Focus();
                return false;
            }
        }

        public static bool IsWithinRange(TextBox textBox, string name, double min, double max)
        {
            double number = Convert.ToDouble(textBox.Text);
            if (number < min || number > max)
            {
                errorMessage = name + " must be between " + min + " and " + max + ".";
                textBox.Focus();
                return false;
            }
            return true;
        }

        public static bool IsDate(TextBox textBox, string name)
        {
            DateTime varDate;
            string strDate;
            CultureInfo CInfoUS = new CultureInfo("en-US");
            try
            {
                strDate = textBox.Text.Trim();
                varDate = DateTime.ParseExact(strDate, "yyyy-mm-dd", CInfoUS);
            }
            catch (System.FormatException ex)
            {
                errorMessage = name + " is not in the correct format. " + ex.Message;
                return false;
            }
            return true;
        }

        public static bool IsDateWithinRange(TextBox textBox, string name)
        {
            DateTime varDate;
            DateTime currentDate = DateTime.Now;
            currentDate = currentDate.AddDays(-5.0);
            string cDate = String.Format("{0:yyyy-MM-dd}", currentDate);

            int requestedYear, currentYear;

            varDate = Convert.ToDateTime(textBox.Text);
            requestedYear = varDate.Year;
            currentYear = currentDate.Year;

            if (requestedYear < 1979 || requestedYear > currentYear)
            {
                errorMessage = name + " must be between 1979-01-01 and " + cDate + ".";
                return false;
            }
            return true;
        }

        public static bool IsEndDateLaterStartDate(TextBox textSdate, string nameSdate, TextBox textEdate, string nameEdate)
        {
            double elapsedJulianDate1 = Convert.ToDateTime(textSdate.Text.ToString()).ToOADate();
            double elapsedJulianDate2 = Convert.ToDateTime(textEdate.Text.ToString()).ToOADate();

            if (elapsedJulianDate2 < elapsedJulianDate1)
            {
                errorMessage = nameEdate + " must be equal to or later than " + nameSdate + ".";
                return false;
            }
            return true;
        }*/

        public static string ConvertDataTableToJSON(DataTable table)
        {
            StringBuilder jsonString = new StringBuilder();

            if (table.Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    jsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
                                              + "\":" + "\""
                                              + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
                                              + "\":" + "\""
                                              + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        jsonString.Append("}");
                    }
                    else
                    {
                        jsonString.Append("},");
                    }
                }
                jsonString.Append("]");
            }

            return jsonString.ToString();
        }

        public static double CalculateDewPoint(double tempC, double relHum)
        {

            double dpExp = 0.0;
            double dp1 = 0.0;
            double dp2 = 0.0;
            double dewPointC = 0.0;
            double dewPointFah = 0.0;

            // Calculate dewpoint
            dpExp = (7.5 * tempC) / (237.7 + tempC);
            dp1 = 6.11 * Math.Pow(10, dpExp);
            dp2 = dp1 * relHum / 100.0;
            dewPointC = (-430.22 + 237.7 * Math.Log(dp2)) / (-Math.Log(dp2) + 19.08);

            dewPointFah = ((9.0 / 5.0) * dewPointC) + 32.0;  // Convert Celsius to Fahrenheit

            return dewPointFah;
        }

        public static double CalculateCloudCover(double rs, double ra, double albedo, double nn)
        {
            // Calculate cloud cover.
            // Cloud  cover equation   0.0042 * C^2 - 0.048 * C - 0.9234 + NN/n = 0, where C = cloud cover
            // using NN/n ratio, cloud cover C is estimated using a quadratic equation.

            double n = 0.0;
            double coeff1 = 0.0;
            double coeff2 = 0.0;
            double coeff3 = 0.0;
            double coeff4 = 0.0;
            double root1 = 0.0;
            double discriminant;

            n = (((rs / ra) - albedo) / 0.5) * nn;
            coeff1 = 0.0042;
            coeff2 = 0.048;  // Wilson Melendez: is this coefficient positive or negative?

            if (n > nn)
            {
                n = nn;
            }

            coeff3 = -0.9234 + (n / nn);

            if ((n / nn) > 1)
            {
                root1 = 1.0;
                return root1;  // Wilson Melendez: added return statement here.
            }
            else if ((n / nn) < 0)
            {
                coeff3 = 0;
            }

            discriminant = (coeff2 * coeff2 - 4 * coeff1 * coeff3);
            coeff4 = Math.Sqrt(discriminant);

            if (discriminant >= 0.0)
            {
                root1 = (-coeff2 + coeff4) / (2.0 * coeff1);

                if (root1 < 0.0)
                {
                    root1 = 0.0;
                }
                else if (root1 > 10)
                {
                    root1 = 10.0;
                }
                root1 = Math.Round(root1, 1);
            }

            return root1;
        }

        public static double CalculateRH(double SpecificHumidity, double Temperature)
        {
            // Calculate relative humidity here.
            double RH = 0.0;
            double es = 6.112 * Math.Exp((17.67 * Temperature) / (Temperature + 243.5)); //double es = 0.6108 * Math.Exp(17.27 * Temperature / (Temperature + 237.3));
            double e = SpecificHumidity * 1013.25 / (0.378 * SpecificHumidity + 0.622);
            RH = 100 * (e / es);

            return RH;
        }

        public static int CalculateMortonMethod(string method)
        {
            switch (method)
            {
                case "ETP":
                    return 0;
                case "ETW":
                    return 1;
                case "ETA":
                    return 2;//Not in CRWE
            }
            return -1;
        }

        public static DataTable aggregateData(ITimeSeriesInput inpt, DataTable dt, string algo)
        {
            DataTable aggregated = dt.Clone();

            if (inpt.Source == "daymet")
            {
                DataRow dr2 = null;
                List<Double> tList = new List<double>();
                List<Double> sList = new List<double>();
                List<Double> vList = new List<double>();
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
                            sList = new List<double>();
                            vList = new List<double>();
                        }
                        tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                        tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                        if(algo != "hamon")
                        {
                            sList.Add(Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"].ToString()));
                        }
                        if (algo == "mortoncrae" || algo == "mortoncrwe")
                        {
                            vList.Add(Convert.ToDouble(dt.Rows[i]["VaPress"].ToString()));
                        }
                        if (j == 6 || i == dt.Rows.Count - 1)
                        {
                            dr2["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                            dr2["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                            dr2["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                            if (algo != "hamon")
                            {
                                dr2["SolarRadMean_MJm2day"] = Math.Round(sList.Average(), 2);
                            }
                            if (algo == "mortoncrae" || algo == "mortoncrwe")
                            {
                                dr2["VaPress"] = Math.Round(vList.Average(), 2);
                            }
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
                            sList = new List<double>();
                            vList = new List<double>();
                            newmonth = false;
                            curmonth = DateTime.ParseExact(dt.Rows[i]["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).Month;
                        }
                        tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                        tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                        if (algo != "hamon")
                        {
                            sList.Add(Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"].ToString()));
                        }
                        if (algo == "mortoncrae" || algo == "mortoncrwe")
                        {
                            vList.Add(Convert.ToDouble(dt.Rows[i]["VaPress"].ToString()));
                        }
                        if (i + 1 < dt.Rows.Count && (DateTime.ParseExact(dt.Rows[i + 1]["Date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture).Month != curmonth) || i == dt.Rows.Count - 1)
                        {
                            dr2["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                            dr2["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                            dr2["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                            if (algo != "hamon")
                            {
                                dr2["SolarRadMean_MJm2day"] = Math.Round(sList.Average(), 2);
                            }
                            if (algo == "mortoncrae" || algo == "mortoncrwe")
                            {
                                dr2["VaPress"] = Math.Round(vList.Average(), 2);
                            }
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
            }
            return aggregated;
        }
    }
}
