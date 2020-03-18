using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Simulate
{
    /// <summary>
    /// Base wgen class.
    /// CURRENTLY only generates precipitation data.
    /// </summary>
    public class WGEN
    {
        /// <summary>
        /// Starts process for generating wgen precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public ITimeSeriesOutput Simulate(out string errorMsg, int years, ITimeSeriesInput input, ITimeSeriesOutput output, ITimeSeriesOutput historicData)
        {
            errorMsg = "";

            // Seed for random number generator, can be custom selected if added to Geometry.GeometryMetadata as 'seed'
            int seed = (input.Geometry.GeometryMetadata.ContainsKey("seed")) ? Convert.ToInt32(input.Geometry.GeometryMetadata["seed"]) : 8888;

            // Run wgen simulation.
            double[] precipData = BeginSimulation(out errorMsg, years, seed, input, output, historicData);

            output = FormatOutputData(out errorMsg, years, seed, precipData, input, output);
            output.Metadata["wgen_historic_datasource"] = (input.Geometry.GeometryMetadata.ContainsKey("historicSource")) ? input.Geometry.GeometryMetadata["historicSource"] : "daymet";

            return output;
        }

        /// <summary>
        /// Executes wgen precipitation genetor.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="years"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="historicData"></param>
        /// <returns></returns>
        private double[] BeginSimulation(out string errorMsg, int years, int seed, ITimeSeriesInput input, ITimeSeriesOutput output, ITimeSeriesOutput historicData)
        {
            // Setting xrain array to 0.
            double[,] xrain = new double[years, 365];
            for (int year = 0; year < years; year++)
            {
                for (int day = 0; day < 365; day++)
                {
                    xrain[year, day] = 0;
                }
            }

            // Assign historic data to xrain array.
            int seqYear = 0;
            int dayIndex = 0;
            DateTime pDate = new DateTime();
            DateTime.TryParse(historicData.Data.Keys.ElementAt(0).Substring(0, 10), out pDate);
            int prevYear = Convert.ToInt32(pDate.Year);
            foreach (var key in historicData.Data)
            {
                DateTime date = new DateTime();
                DateTime.TryParse(key.Key.ToString().Substring(0, 10), out date);
                if (prevYear != Convert.ToInt32(date.Year))
                {
                    prevYear = date.Year;
                    dayIndex = 0;
                    seqYear++;
                }
                else
                {
                    dayIndex++;
                }
                if (dayIndex != 365)
                {
                    xrain[seqYear, dayIndex] = Convert.ToDouble(historicData.Data[key.Key][0].ToString()) * 0.0393701;
                }
            }

            // Precip parameters
            double[,] ydata = new double[4, 12];
            SetParameters(out errorMsg, xrain, out ydata);

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

          
            int numYears = input.DateTimeSpan.EndDate.Year - input.DateTimeSpan.StartDate.Year;     // Number of years to generate data.
            if (numYears < 1)
            {
                numYears = 1;                                           // Minimum of 1 year of returned precip data.
            }
            int numDays = numYears * 365;                               // Number of days to generate data.
            double[] precip = new double[numDays];

            // Run precipitaiton generator
            GeneratePrecip(seed, pww, pwd, alpha, beta, numDays, out precip, out errorMsg);
            return precip;
        }

        /// <summary>
        /// Returns precipitation generation parameters for WGEN.
        /// </summary>
        /// <param name="XRAIN">Two dimensional array(Year,Day) of historical precipitation data. Day: Year, e.g. 0,1,2, and Year: Day within a year, e.g 0,1,2..364.  </param>
        /// <param name="YDATA">Output. Two dimensional array(Month,Parametr) of monthly WGEN precipitation generation parameters.
        /// Month: month of year e.g. 0,1,...11 and Paramter: WGEN parameter, 0 - PWW (Probability of a wet day following a wet day;
        /// 1 - PWD (Probability of a wet day following a dry day; 
        /// 2 - Alpha (Shape parameters of Gamma Distribution; 3 - Beta (Scale paramter of Gamma Distribution </param>
        /// <param name="errorMsg">string containing an errorMsg.  Empty string means no errorMsg was generated.</param>
        private void SetParameters(out string errorMsg, double[,] XRAIN, out double[,] YDATA)
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
        private void GeneratePrecip(int rndSeed, double[] PWW, double[] PWD, double[] alpha, double[] beta, int numDays, out double[] precip, out string errorMsg)
        {
            errorMsg = "";
            precip = new double[numDays];

            int[] month = { 30, 58, 89, 119, 150, 180, 211, 242, 272, 303, 333, 364 };
            Random rnd = new Random(rndSeed);
            int mon = 0;

            Accord.Statistics.Distributions.Univariate.GammaDistribution gamma = new Accord.Statistics.Distributions.Univariate.GammaDistribution(alpha[0], beta[0]);
            Accord.Math.Random.Generator.Seed = rndSeed;
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

        /// <summary>
        /// Constructs the output object from the simulated data and input parameters.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="years"></param>
        /// <param name="seed"></param>
        /// <param name="data"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput FormatOutputData(out string errorMsg, int years, int seed, double[] data, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";

            ITimeSeriesOutput precipOutput = output;
            DateTime date = input.DateTimeSpan.StartDate;
            Dictionary<string, List<string>> finalOutput = new Dictionary<string, List<string>>();

            // Add details to output object.
            precipOutput.Dataset = "precipitation";
            precipOutput.DataSource = "wgen";

            // Add metadata details
            precipOutput.Metadata = new Dictionary<string, string>()
                {
                    { "wgen_startDate", input.DateTimeSpan.StartDate.ToString() },
                    { "wgen_endDate", input.DateTimeSpan.EndDate.ToString() },
                    { "wgen_latitude", input.Geometry.Point.Latitude.ToString() },
                    { "wgen_longitude", input.Geometry.Point.Longitude.ToString() },
                    { "wgen_seed_value", seed.ToString() },
                    { "wgen_historic_years", (years).ToString() },
                    { "wgen_unit", "(in/day)" }
                };

            // Add data to output.data object
            for (int i = 0; i < data.Length; i++)
            {
                date = input.DateTimeSpan.StartDate.AddDays(i);
                finalOutput.Add(date.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { data[i].ToString(input.DataValueFormat) });
            }
            precipOutput.Data = finalOutput;
            return precipOutput;
        }
    }
}
