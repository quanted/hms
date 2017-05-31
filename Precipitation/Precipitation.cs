using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HMSPrecipitation
{
    public class Precipitation : HMSLDAS.IHMSModule
    {

        // Class variables which define a precipitation object.
        public double latitude { get; set; }                            // Latitude for timeseries
        public double longitude { get; set; }                           // Longitude for timeseries
        public DateTime startDate { get; set; }                         // Start data for timeseries
        public DateTime endDate { get; set; }                           // End date for timeseries
        public double gmtOffset { get; set; }                           // Timezone offset from GMT
        public string tzName { get; set; }                              // Timezone name
        public string dataSource { get; set; }                          // NLDAS, GLDAS, or SWAT algorithm simulation
        public bool localTime { get; set; }                             // False = GMT time, true = local time.
        public List<HMSTimeSeries.HMSTimeSeries> ts { get; set; }       // TimeSeries Data, unaltered from source
        public double cellWidth { get; set; }                           // LDAS data point cell width, defined by source.
        public string shapefilePath { get; set; }                       // Path to shapefile, if provided. Used in place of coordinates.
        public HMSGDAL.HMSGDAL gdal { get; set; }                       // GDAL object for GIS operations.
        public string station { get; set; }                             // Station ID for NCDC data.
        public HMSJSON.HMSJSON.HMSData jsonData { get; set; }           // Post-Processed data object, ready to be serialized into json string.

        /// <summary>
        /// Default constructor
        /// </summary>
        public Precipitation() { }

        /// <summary>
        /// Constructor for a generic precip object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, bool local) : this(out errorMsg, startDate, endDate, source, local, null)
        {
        }

        /// <summary>
        /// Constructor for getting NCDC data with a station id.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="station"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, string station) : this(out errorMsg, startDate, endDate, source, false, "")
        {
            this.station = station;
        }

        /// <summary>
        /// Constructor using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="snow"></param>
        public Precipitation(out string errorMsg, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, "0.0", "0.0", startDate, endDate, source, local, sfPath)
        {
        }

        /// <summary>
        /// Constructor using latitude and longitude.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public Precipitation(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath) : this(out errorMsg, latitude, longitude, startDate, endDate, source, local, sfPath, "0.0", "NaN")
        {
        }

        /// <summary>
        /// Constructor using latitude and longitude, with gmtOffset already known.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="source"></param>
        /// <param name="local"></param>
        /// <param name="sfPath"></param>
        public Precipitation(out string errorMsg, string latitude, string longitude, string startDate, string endDate, string source, bool local, string sfPath, string gmtOffset, string tzName)
        {
            errorMsg = "";
            this.gmtOffset = Convert.ToDouble(gmtOffset);
            this.dataSource = source.ToLower();
            this.localTime = local;
            this.tzName = tzName;
            if (errorMsg.Contains("ERROR")) { return; }
            SetDates(out errorMsg, startDate, endDate);
            if (errorMsg.Contains("ERROR")) { return; }
            ts = new List<HMSTimeSeries.HMSTimeSeries>();
            if (string.IsNullOrWhiteSpace(sfPath))
            {
                this.shapefilePath = null;
                try
                {
                    this.latitude = Convert.ToDouble(latitude.Trim());
                    this.longitude = Convert.ToDouble(longitude.Trim());
                }
                catch
                {
                    errorMsg = "ERROR: Invalid latitude or longitude value.";
                    return;
                }
            }
            else
            {
                this.shapefilePath = sfPath;
                this.latitude = 0.0;
                this.longitude = 0.0;
            }
            if (this.dataSource == "nldas") { this.cellWidth = 0.12500; }
            else if (this.dataSource == "gldas") { this.cellWidth = 0.2500; }
            else if (this.dataSource == "daymet") { this.cellWidth = 0.01; }
            else { this.cellWidth = 0.0; }
            this.gdal = new HMSGDAL.HMSGDAL();
        }

        /// <summary>
        /// Sets startDate and endDate, checks that dates are valid (start date before end date, end date no greater than today, start dates are valid for data sources)
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void SetDates(out string errorMsg, string start, string end)
        {
            errorMsg = "";
            try
            {
                this.startDate = Convert.ToDateTime(start);
                this.endDate = Convert.ToDateTime(end);
                DateTime newStartDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day, 00, 00, 00);
                DateTime newEndDate = new DateTime(this.endDate.Year, this.endDate.Month, this.endDate.Day, 23, 00, 00);
                this.startDate = newStartDate;
                this.endDate = newEndDate;
            }
            catch
            {
                errorMsg = "ERROR: Invalid date format. Please provide a date as mm-dd-yyyy or mm/dd/yyyy.";
                return;
            }
            if (DateTime.Compare(this.endDate, DateTime.Today) > 0)   //If endDate is past today's date, endDate is set to 5 days prior to today.
            {
                this.endDate = DateTime.Today.AddDays(-5.0);
            }
            if (DateTime.Compare(this.startDate, this.endDate) > 0)
            {
                errorMsg = "ERROR: Invalid dates entered. Please enter an end date set after the start date.";
                return;
            }
            if (this.dataSource.Contains("nldas"))
            {
                DateTime minDate = new DateTime(1979, 01, 02);              //NLDAS data collection start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;                               //start date is set to NLDAS start date
                }
            }
            else if (this.dataSource.Contains("gldas"))
            {
                DateTime minDate = new DateTime(2000, 02, 25);              //GLDAS data collection start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;                               //start date is set to GLDAS start date
                }
            }
            else if (this.dataSource.Contains("daymet"))
            {
                DateTime minDate = new DateTime(1980, 01, 01);              //Daymet dataset start date
                if (DateTime.Compare(this.startDate, minDate) < 0)
                {
                    this.startDate = minDate;
                }
            }
        }

        /// <summary>
        /// Gets precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        public List<HMSTimeSeries.HMSTimeSeries> GetDataSets(out string errorMsg)
        {
            errorMsg = "";
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            double offset = gmtOffset;
            HMSTimeSeries.HMSTimeSeries newTS = new HMSTimeSeries.HMSTimeSeries();
            ts.Add(newTS);

            if (this.shapefilePath != null && this.dataSource.Contains("ldas"))
            {
                bool sourceNLDAS = true;
                if (this.dataSource.Contains("gldas")) { sourceNLDAS = false; }
                double[] center = gldas.DetermineReturnCoordinates(out errorMsg, gdal.ReturnCentroid(out errorMsg, this.shapefilePath), sourceNLDAS);
                this.latitude = center[0];
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
                if (errorMsg.Contains("ERROR")) { return null; }
            }
            else if (this.shapefilePath != null && this.dataSource.Contains("daymet"))
            {
                double[] center = gdal.ReturnCentroid(out errorMsg, this.shapefilePath);
                this.latitude = center[0];
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
            }
            else if (this.gdal.geoJSON != null)
            {
                double[] center = gdal.ReturnCentroidFromGeoJSON(out errorMsg);
                this.latitude = center[0];
                this.longitude = center[1];
                gdal.CellAreaInShapefileByGrid(out errorMsg, center, this.cellWidth);
            }


            // Updated to use call to google instead of using shapefile search. Commented code obsolete.
            if (this.localTime == true && !String.IsNullOrWhiteSpace(this.tzName))
            {
                HMSUtils.Utils utils = new HMSUtils.Utils();
                Dictionary<string, string> tzDetails = utils.GetTZInfo(out errorMsg, this.latitude, this.longitude);
                if (tzDetails.ContainsKey("rawOffset") && tzDetails.ContainsKey("timeZoneId"))
                {
                    this.gmtOffset = Convert.ToDouble(tzDetails["rawOffset"]) / 3600;
                    this.tzName = tzDetails["timeZoneId"];
                }
                else if (tzDetails.ContainsKey("tzOffset") && tzDetails.ContainsKey("tzName"))
                {
                    this.gmtOffset = Convert.ToDouble(tzDetails["tzOffset"]);
                    this.tzName = tzDetails["tzName"];
                }
                //this.gmtOffset = gdal.GetGMTOffset(out errorMsg, this.latitude, this.longitude, ts[0]);         //Gets the GMT offset
                //if (errorMsg.Contains("ERROR")) { return null; }
                //this.tzName = ts[0].tzName;                                                                     //Gets the Timezone name
                if (errorMsg.Contains("ERROR")) { return null; }
                this.startDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.startDate, true);
                this.endDate = gdal.AdjustDateByOffset(out errorMsg, this.gmtOffset, this.endDate, false);
            }

            // Define this from the utils class, possibly read from file.
            if (this.shapefilePath != null || this.gdal.geoJSON != null)
            {
                if (gdal.coordinatesInShapefile.Count > 30)
                {
                    errorMsg = "ERROR: Feature geometries containing more than 30 datapoints are prohibited. Current feature contains " + gdal.coordinatesInShapefile.Count + " " + this.dataSource + " data points."; return null;
                }
            }
            // TODO: Move data retrieval calls back into the precipitation class.
            if (this.dataSource.Contains("nldas") || this.dataSource.Contains("gldas"))         // LDAS Precipitation DATA
            {
                gldas.BeginLDASSequence(out errorMsg, this, "PRECIP", newTS);
            }
            else if (this.dataSource.Contains("daymet"))                                        // Daymet Precipitation DATA
            {
                HMSDaymet.HMSDaymet daymet = new HMSDaymet.HMSDaymet();
                daymet.GetDaymetData(out errorMsg, this, "Precip", newTS);
            }
            else if (this.dataSource.Contains("ncdc"))                                          // NCDC Precipitation DATA
            {
                HMSNCDC.HMSNCDC ncdc = new HMSNCDC.HMSNCDC();
                ncdc.BeginNCDCSequence(out errorMsg, this, "NCDC", this.station, newTS);
            }
            else if (this.dataSource.Contains("wgen"))
            {
                double[,] xrain = new double[20, 365];                      // Daymet data array initialization.

                // Daymet historic data retrieval setup
                DateTime endDateTemp = this.endDate;
                DateTime startDateTemp = this.startDate;
                this.endDate = this.startDate.AddDays(-1);                              // Historic end date set to given start date.
                this.startDate = this.startDate.AddYears(-20);              // Historic start date set to 20 years before given start date.

                // Daymet historic data retrieval
                HMSDaymet.HMSDaymet daymet = new HMSDaymet.HMSDaymet();
                daymet.GetDaymetData(out errorMsg, this, "Precip", newTS);
                string data = this.ts[0].timeSeries;
                // Daymet historic data processing
                //data = data.Remove(0, data.IndexOf(@"(mm/day") + 9);        // Removing metadata from returned time series.
                string[] dataRows = data.Split('\n');
                // Set all elements in xrain data array to 0.
                for (int year = 0; year < 20; year++)
                {
                    for (int day = 0; day < 365; day++)
                    {
                        xrain[year, day] = 0;
                    }
                }
                // Assignment of daymet data to xrain array.
                char[] delimiterChar = { ',' };
                int seqYear = 0;
                int prevYear = Convert.ToInt32(dataRows[0][0].ToString());
                int numRows = 0;
                foreach (string row in dataRows)
                {
                    numRows++;
                    string[] observation = row.Split(' ');
                    if ((observation == null) || (observation.Length < 3))
                    {
                        break;
                    }
                    if (prevYear != Convert.ToInt32(observation[0].ToString()))
                    {
                        prevYear = Convert.ToInt32(observation[0].ToString());
                        seqYear++;
                    }
                    xrain[seqYear - 1, Convert.ToInt32(observation[1].ToString()) - 1] = Convert.ToDouble(observation[2].ToString()) * 0.0393701;
                }

                double[,] ydata = new double[4, 12];                        // Precip parameters.

                WGEN wgen = new WGEN();
                wgen.getPrecipParameters(xrain, out ydata, out errorMsg);   // Gets precip parameters.

                // Populating variables from precip parameters
                double[] pww = new double[12];
                double[] pwd = new double[12];
                double[] alpha = new double[12];
                double[] beta = new double[12];
                for (int i = 0; i < 12; i++)
                {
                    pww[i] = ydata[0, i];
                    pwd[i] = ydata[1, i];
                    alpha[i] = ydata[2, i];
                    beta[i] = ydata[3, i];
                }

                int seed = 8888;                                            // Seed for random number generator.
                int numYears = this.endDate.Year - this.startDate.Year;     // Number of years to generate data.
                if (numYears < 1)
                {
                    numYears = 1;                                           // Minimum of 1 year of returned precip data.
                }
                int numDays = numYears * 365;                               // Number of days to generate data.
                double[] precip = new double[numDays];
                wgen.generatePrecip(seed, pww, pwd, alpha, beta, numDays, out precip, out errorMsg);

                // Replace historic dates with requested dates.
                this.startDate = startDateTemp;
                this.endDate = endDateTemp;

                HMSJSON.HMSJSON.HMSData tempOutput = new HMSJSON.HMSJSON.HMSData();
                tempOutput.source = "wgen";
                tempOutput.dataset = "precipitation";
                tempOutput.metadata = new Dictionary<string, string>()
                {
                    { "wgen_startDate", this.startDate.ToString() },
                    { "wgen_endDate", this.endDate.ToString() },
                    { "wgen_latitude", this.latitude.ToString() },
                    { "wgen_longitude", this.longitude.ToString() },
                    { "wgen_seed_value", seed.ToString() },
                    { "wgen_historic_years", (numYears + 1).ToString() },
                    { "wgen_units", "(mm/day)" }
                };
                Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
                for (int day = 0; day < precip.Length; day++)
                {
                    string date = this.startDate.AddDays(day).ToString("yyyy-MM-dd");
                    string dateData = (precip[day] * 25.4).ToString("0.0000E+00");
                    tempData.Add(date, new List<string>() { dateData });
                }
                tempOutput.data = tempData;
                this.jsonData = tempOutput;
                return null;
            }
            if (errorMsg.Contains("ERROR")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            this.jsonData = output.ConstructHMSDataFromTS(out errorMsg, this.ts, "Precipitation", this.dataSource, this.localTime, this.gmtOffset);
            return ts;
        }

        /// <summary>
        /// Gets the precipitaiton data using a shapefile.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public string GetDataSetsString(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            HMSJSON.HMSJSON output = new HMSJSON.HMSJSON();
            string combinedData = output.CombineTimeseriesAsJson(out errorMsg, this.jsonData);
            if (errorMsg.Contains("ERROR")) { return null; }
            return combinedData;
        }

        /// <summary>
        /// Get precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public void GetDataSetsObject(out string errorMsg)
        {
            errorMsg = "";
            GetDataSets(out errorMsg);
        }

    }
    //---------- WGEN ----------//
    public class WGEN
    {
        /// <summary>
        /// Returns precipitation generation parameters for WGEN.
        /// </summary>
        /// <param name="XRAIN">Two dimensional array(Year,Day) of historical precipitation data. Day: Year, e.g. 0,1,2, and Year: Day within a year, e.g 0,1,2..364.  </param>
        /// <param name="YDATA">Output. Two dimensional array(Month,Parametr) of monthly WGEN precipitation generation parameters.
        /// Month: month of year e.g. 0,1,...11 and Paramter: WGEN parameter, 0 - PWW (Probability of a wet day following a wet day;
        /// 1 - PWD (Probability of a wet day following a dry day; 
        /// 2 - Alpha (Shape parameters of Gamma Distribution; 3 - Beta (Scale paramter of Gamma Distribution </param>
        /// <param name="errorMsg">string containing an errorMsg.  Empty string means no errorMsg was generated.</param>
        public void getPrecipParameters(double[,] XRAIN, out double[,] YDATA, out string errorMsg)
        {
            errorMsg = "";
            YDATA = new double[4, 12];

            int[] NWD = new int[12];
            int[] NDD = new int[12];
            int[] NDW = new int[12];
            int[] NWW = new int[12];

            double[] SUM = new double[12];
            double[] SUM2 = new double[12];
            double[] SUM3 = new double[12];

            double[] SL = new double[12];
            double[] PWW = new double[12];
            double[] PWD = new double[12];
            double[] RBAR = new double[12];

            double[] ALPHA = new double[12];
            double[] BETA = new double[12];

            int[] NW = new int[12];
            int[] IC = new int[12];
            double[] SUML = new double[12];

            double[] RLBAR = new double[12];
            double[] AL2 = new double[12];
            double[] BE2 = new double[12];
            DateTime[] Date = new DateTime[12];

            double[] PPPW = new double[12];
            int[] ND = new int[12];

            for (int i = 0; i < 12; i++)
            {
                ND[i] = 0;
                PPPW[i] = 0;
                NWD[i] = 0;
                NWW[i] = 0;
                NDD[i] = 0;
                NDW[i] = 0;
                NW[i] = 0;
                SL[i] = 0;
                SUML[i] = 0;
                SUM[i] = 0;
                SUM2[i] = 0;
                PWW[i] = 0;
                PWD[i] = 0;
                ALPHA[i] = 0;
                BETA[i] = 0;
                SUM3[i] = 0;
            }
            int NYR = 20;
            double RIM1 = 0;
            int MO = 0;
            double RAIN = 0;
            for (int J = 0; J < NYR; J++)
            {
                for (int K = 0; K < 365; K++)
                {
                    if ((K >= 0) && (K < 31))
                    {
                        MO = 0;
                    }
                    else if ((K >= 31) && (K < 59))
                    {
                        MO = 1;
                    }
                    else if ((K >= 59) && (K < 90))
                    {
                        MO = 2;
                    }
                    else if ((K >= 90) && (K < 120))
                    {
                        MO = 3;
                    }
                    else if ((K >= 120) && (K < 151))
                    {
                        MO = 4;
                    }
                    else if ((K >= 151) && (K < 181))
                    {
                        MO = 5;
                    }
                    else if ((K >= 181) && (K < 212))
                    {
                        MO = 6;
                    }
                    else if ((K >= 212) && (K < 243))
                    {
                        MO = 7;
                    }
                    else if ((K >= 243) && (K < 273))
                    {
                        MO = 8;
                    }
                    else if ((K >= 273) && (K < 304))
                    {
                        MO = 9;
                    }
                    else if ((K >= 304) && (K < 334))
                    {
                        MO = 10;
                    }
                    else if ((K >= 334) && (K < 365))
                    {
                        MO = 11;
                    }
                    else
                    {
                        errorMsg = "Invalid data.  Day of year must be between 1 and 365.";
                        return;
                    }
                    RAIN = XRAIN[J, K];
                    if (RAIN > 0)
                        NW[MO] = NW[MO] + 1;
                    ND[MO] = ND[MO] + 1;
                    if (RAIN > 0.0)
                    {
                        if (RIM1 <= 0) //3
                        {
                            NWD[MO] = NWD[MO] + 1; //2
                        }
                        else //4 if RIM > 0
                        {
                            NWW[MO] = NWW[MO] + 1;
                        }
                        //6
                        SUML[MO] = SUML[MO] + Math.Log(RAIN);
                        SUM[MO] = SUM[MO] + RAIN;
                        SUM2[MO] = SUM2[MO] + RAIN * RAIN;
                        SUM3[MO] = SUM3[MO] + RAIN * RAIN * RAIN;
                        SL[MO] = SL[MO] + Math.Log(RAIN);
                    }
                    else //5 if RAIN <= 0
                    {
                        if (RIM1 <= 0) //7
                        {
                            NDD[MO] = NDD[MO] + 1;
                        }
                        else // 8 if RIM1 > 0
                        {
                            NDW[MO] = NDW[MO] + 1;
                        }
                    }
                    RIM1 = RAIN;    //9
                }
            }

            int XXND = 0;
            int YYNW = 0;
            int III = 0;
            int XNWW = 0;
            int XNWD = 0;
            int XXNW = 0;
            int XND = 0;
            int XNW = 0;
            double Y = 0;
            double ANUM = 0;
            double ADOM = 0;
            double ALPHA2 = 0;
            double BETA2 = 0;
            for (int I = 0; I < 12; I++)
            {
                XXND = ND[I];
                YYNW = NW[I];
                PPPW[I] = Convert.ToDouble(YYNW) / Convert.ToDouble(XXND);
                III = 1;
                if (NW[I] < 3)
                    III = 2;
                IC[I] = III;
                if (NW[I] >= 3)
                {
                    XNWW = NWW[I];
                    XNWD = NWD[I];
                    XXNW = NWW[I] + NDW[I];
                    XND = NDD[I] + NWD[I];
                    XNW = NW[I];
                    PWW[I] = Convert.ToDouble(XNWW) / Convert.ToDouble(XXNW);
                    PWD[I] = Convert.ToDouble(XNWD) / Convert.ToDouble(XND);
                    RBAR[I] = SUM[I] / Convert.ToDouble(XNW);
                    RLBAR[I] = SUML[I] / Convert.ToDouble(XNW);
                    Y = Math.Log(RBAR[I]) - RLBAR[I];
                    //Potetial References: 
                    //http://www.goldsim.com/Downloads/Library/Models/Applications/Hydrology/WGEN.pdf
                    //https://www.youtube.com/watch?v=ieKokNwvjew
                    //https://www.youtube.com/watch?v=ieKokNwvjew
                    //http://www.tandfonline.com/doi/pdf/10.4296/cwrj59
                    //https://www.nrcs.usda.gov/wps/portal/nrcs/detailfull/national/water/manage/hydrology/?cid=stelprdb1043611
                    //http://cola.siu.edu/geography/_common/documents/papers/wang.pdf
                    //http://modeling.bsyse.wsu.edu/CS_Suite/ClimGen/documentation/abstract/en.htm
                    //http://eprints.qut.edu.au/38134/1/WGENAridClim.pdf
                    //https://onlinecourses.science.psu.edu/stat414/node/193
                    //http://www.ism.ac.jp/editsec/aism/pdf/054_3_0565.pdf
                    //http://accord-framework.net/docs/html/M_Accord_Statistics_Distributions_Univariate_GammaDistribution_Estimate.htm
                    if ((0.5772 < Y) && (Y <= 17.0))
                    {
                        ANUM = 8.898919 + (9.05995 * Y) + (0.9775373 * Y * Y); //Reference: http://eprints.qut.edu.au/38134/1/WGENAridClim.pdf
                        ADOM = Y * (17.79728 + (11.968477 * Y) + (Y * Y));
                    }
                    else if ((0 <= Y) && (Y <= 0.5772))
                    {
                        ANUM = 0.5000876 + (0.1648852 * Y) - (0.0544274 * Y * Y);
                        ADOM = Y;
                    }
                    if (Y > 17.0)
                    {
                        ALPHA2 = 1.0 / Y; //Reference http://www.ism.ac.jp/editsec/aism/pdf/054_3_0565.pdf
                    }
                    else
                    {
                        ALPHA2 = ANUM / ADOM;
                        if (ALPHA2 >= 1.0)
                            ALPHA2 = 0.998;
                    }
                    BETA2 = RBAR[I] / ALPHA2;
                    ALPHA[I] = ALPHA2;
                    BETA[I] = BETA2;
                }
                //120
                for (int i = 0; i < 12; i++)
                {
                    YDATA[0, i] = PWW[i];
                    YDATA[1, i] = PWD[i];
                    YDATA[2, i] = ALPHA[i];
                    YDATA[3, i] = BETA[i];
                }
            }
            return;
        }
        /// <summary>
        /// Returns WGEN generated daily precipitation (inches) data for numDays number of days
        /// </summary>
        /// <param name="rndSeed">Random number generator seed</param>
        /// <param name="PWW">One dimensional array(Month) of probabilities of a wet day following a wet day.  Month: Month of the year.</param>
        /// <param name="PWD">One dimensional array(Month) of probabilities of a wet day following a dry day.  Month: Month of the year.</param>
        /// <param name="alpha">One dimensional arry(Month) of Gamma Distribution Shape parameter. Month: Month of the year.</param>
        /// <param name="beta">One dimensional arry(Month) of Gamma Distribution Scale parameter. Month: Month of the year</param>
        /// <param name="numDays">Number of days for which precipitation is to be generated</param>
        /// <param name="precip">One dimensional array[Day] of daily precipitation in inches</param>
        /// <param name="errorMsg"></param>
        public void generatePrecip(int rndSeed, double[] PWW, double[] PWD, double[] alpha, double[] beta, int numDays, out double[] precip, out string errorMsg)
        {
            errorMsg = "";
            precip = new double[numDays];

            int[] month = { 30, 58, 89, 119, 150, 180, 211, 242, 272, 303, 333, 364 };
            Random rnd = new Random(rndSeed);
            int mon = 0;

            Accord.Statistics.Distributions.Univariate.GammaDistribution gamma = new Accord.Statistics.Distributions.Univariate.GammaDistribution(alpha[0], beta[0]);
            bool previousDayWet = false;

            //Randomly decide if the first day is dry or wet
            double rnd1 = rnd.NextDouble();
            if (rnd1 > 0.5)
            {
                previousDayWet = true;
            }
            else
            {
                previousDayWet = false;
            }
            //If previous day is wet then assign it the precipitation amount by generating a random variate from Gamma Distribution
            if (previousDayWet == true)
            {
                precip[0] = gamma.Generate();
            }

            int year = 0;
            //Start the loop from 2nd day because the first day has already been decided
            for (int i = 1; i < numDays; i++)
            {
                if ((i - year * 365) > month[mon]) //Check if month has changed
                {
                    mon = mon + 1;
                    if (mon > 11)
                    {
                        mon = 1;
                        year = year + 1;
                    }
                    gamma = new Accord.Statistics.Distributions.Univariate.GammaDistribution(alpha[mon], beta[mon]); //Instantiate Gamma Distribution for the month
                }
                //Check if it is a wet or dry day
                rnd1 = rnd.NextDouble();
                if (previousDayWet == true)
                {
                    if (rnd1 <= PWW[mon]) //wet day followed by a wet day
                    {
                        previousDayWet = true; //set for the next iteration
                        precip[i] = gamma.Generate();
                    }
                    else //wet day followed by a dry day
                    {
                        previousDayWet = false; //set for the next iteration
                        precip[i] = 0;
                    }
                }
                else //Previous day is dry
                {
                    if (rnd1 <= PWD[mon]) //dry day follwoed by a wet day
                    {
                        previousDayWet = true; //set for the next iteration
                        precip[i] = gamma.Generate();
                    }
                    else //dry day followed by a dry day
                    {
                        previousDayWet = false;
                        precip[i] = 0; //set for the next iteration
                    }
                }
            }
            return;
        }

    }
}