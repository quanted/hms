using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using Globals;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AQUATOX.Volume
{

    public enum VolumeMethType
    {
        Manning,    // 0
        KeepConst,  // 1
        Dynam,      // 2
        KnownVal    // 3
    } // end VolumeMethType

   
    public class TVolume : TStateVariable
    {
        // [JsonIgnore] double // LastCalcTA ;  
        // [JsonIgnore] double // LastTimeTA ;        // don't need saving
        [JsonIgnore] double Inflow = 0.0;
        [JsonIgnore] double Discharg = 0.0;          // don't need saving
        [JsonIgnore] double InflowLoad = 0.0;
        [JsonIgnore] double DischargeLoad = 0.0;
        [JsonIgnore] double KnownValueLoad = 0.0;
        // [JsonIgnore] double OOSDischFracLoad = 0;
        // [JsonIgnore] double OOSInflowFracLoad = 0;  // don't need saving

        public override List<string> GUIRadioButtons()
        {
            return new List<string>(new string[] { "Manning's Eqn. (vol. is fn. discharge)", "Keep Constant (discharge is fn. inflow)", "Calculate (vol. is fn. inflow, discharge, evap.)","Use Known Vals. (discharge is fn. inflow)" });
        }

        public override int RadioButtonState()
        {
            if (Calc_Method == VolumeMethType.Manning) return 0;
            if (Calc_Method == VolumeMethType.KeepConst) return 1;
            if (Calc_Method == VolumeMethType.Dynam) return 2;
            return 3;
        }

        public override void SetVarFromRadioButton(int iButton)
        {
            if (iButton == 0) Calc_Method = VolumeMethType.Manning;
             else if (iButton == 1) Calc_Method = VolumeMethType.KeepConst;
              else if (iButton == 2) Calc_Method = VolumeMethType.Dynam;
               else Calc_Method = VolumeMethType.KnownVal;
        }

        public VolumeMethType Calc_Method;

        //            public FlowType StratInflow;
        //            public FlowType StratOutflow;
        //            public bool StratAutomatically = false;
        //            public TLoadings StratDates = null;
        // -------------------------------------------------------
        public TVolume(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            Inflow = 0;
            Discharg = 0;
            InflowLoad = 0;
            DischargeLoad = 0;
            // OOSDischFracLoad = 1;
            // OOSInflowFracLoad = 1;
            KnownValueLoad = 0;
            Calc_Method = VolumeMethType.KeepConst;
            //                StratInflow = FlowType.FTBoth;
            //                StratOutflow = FlowType.FTBoth;
            //                StratAutomatically = true;
            //                StratDates = new TLoadings(10, 50);
        }

        ~TVolume()
        {
            //            StratDates.Destroy();
            //            base.Destroy();
        }

        public override void SetToInitCond() 
        {
            base.SetToInitCond();
            Inflow = 0;
            //LastTimeTA = 0;
            //LastCalcTA = 0;
            AQTSeg.Volume_Last_Step = InitialCond;

//            AQSite.VolFrac_Last_Step:= VolFrac(MaxEpiThick, Locale.ZMax, P_Shape);
//            If EstuarySegment then VolFrac_Last_Step:= TVolume(P).FracUpper;
          }

        public double Manning_Volume()
        {
            double result;
            // WATER VOLUME (using Manning's eq.) in cu/m
            double Q;
            double Y;
            double Width;
            double CLength;
            CLength = Location.Locale.SiteLength.Val * 1000;
            // m                        // km     // m/km
            // AVERAGE FLOW DISCHARGE
            Q = Discharg / 86400.0;
            // m3/s // m3/d  // s/d
            Width = Location.Locale.SurfArea.Val / (Location.Locale.SiteLength.Val * 1000.0);
            // m                   // sq.m                      // km       // m/km
            Y = Math.Pow((Q * Location.ManningCoeff()) / (Math.Sqrt(Location.Locale.Channel_Slope.Val) * Width), 0.6);
            // m      // m3/s            // s/ m^1/3                                   // m/m         // m
            result = Y * CLength * Width;
            // cu. m   // m  // m     // m

            return result;
        }

        // THIS PROCEDURE (VolFrac) ONLY USED IN BATHYMETRY MODE, NOT FOR RIVERS
        public double VolFrac(double Z, double ZMx, double P)
        {
            double result;
            double ZZMax3;
            if (P == -3.0)
            {
                P = -2.99;
            }                // to avoid zero divide
            ZZMax3 = AQMath.Cube(Z / ZMx);
            // fraction       m  m          unitless  m   m    m   m
            result = (6.0 * Z / ZMx - 3.0 * (1.0 - P) * (Z / ZMx) * (Z / ZMx) - 2.0 * P * ZZMax3) / (3.0 + P);
            // unitless
            // assuming generalized morphometry

            return result;
        }

        // volfrac
        // ------------------------------------------------------------------------------------------------
        public double Evaporation()
        {
            if (AQTSeg.UseConstEvap)
            {
                return Location.Locale.MeanEvap.Val * (0.0254 / 365.0) * Location.Locale.SurfArea.Val;
                // cu m/d             // in/yr    // m/in // d/yr                // sq m
            }
            else
            {
                // Use Time-Series ("dynamic") Evaporation
                return AQTSeg.DynEvap.ReturnLoad(AQTSeg.TPresent);
                // cu m/d
            }
        }
        // ------------------------------------------------------------------------------------------------


        public double ResidFlow()
        {
            double result;
            result = InflowLoad - Evaporation();
            // Load = Inflow load + Direct Precip + PS. + N.P.S.
            // m3/d  // m3/d           // m3/d

            return result;
        }

        // ------------------------------------------------------------------------------------------------
        public void DeltaVolume()
        {
            // ************************************************************************************************
            // *                                                                                              *
            // *  This procedure replaces DeltaMorph as of 12-9-99.                                           *
            // *                                                                                              *
            // *  This procedure ensures all dynamic Morph data is properly set, including:                   *
            // *         Volume, InflowH2O, Discharge, ZMix, MeanThick, TotDischarge, retentiontime           *
            // *                                                                                              *
            // *  Other Morph variables are not dynamic over a study run, such as                             *
            // *         P_Shape, ECoeffWater.  They are not modified by this Procedure                       *
            // *                                                                                              *
            // *  DeltaVolume is called whenever the Volume S.V. or Volume Loadings Data have changed         *
            // *                                                                                              *
            // ************************************************************************************************
            double Avg_Disch;
            double WidthCalc;
            double Channel_Depth;
            // ----------------------------------------------------------
            if (State < Consts.Tiny)
            {
                State = 0;
            }            // Volume cannot be negative

            // not Stratified
            Location.Morph.SegVolum = AQTSeg.Volume_Last_Step;

            Location.MeanThick = AQTSeg.DynamicZMean();
            Location.Discharge = Discharg;
            Location.Morph.InflowH2O = Inflow;
            Location.TotDischarge = Discharg;  // used for summing epilimnion and hypolimnion if stratification code enabled
            AQTSeg.residence_time = AQTSeg.Volume_Last_Step / Discharg;
             // water res time in d  =               m3     /   m3/d   

            if ((Location.SiteType == SiteTypes.Stream))
            {
                WidthCalc = Location.Locale.SurfArea.Val / (Location.Locale.SiteLength.Val * 1000);
                // m                        // sq.m                      // km     // m/km

                Avg_Disch = Location.Discharge / 86400.0;
                Channel_Depth = Math.Pow(Avg_Disch * Location.ManningCoeff() / (Math.Sqrt(Location.Locale.Channel_Slope.Val)) * WidthCalc, 3.0 / 5.0);
                Location.Morph.XSecArea = WidthCalc * Channel_Depth;
                // m2       // m          // m
            }
            else
            {
                Location.Morph.XSecArea = AQTSeg.Volume_Last_Step / (Location.Locale.SiteLength.Val * 1000.0);
            }                                         // m3                           // km    // m/km

        }


        public override void CalculateLoad(DateTime TimeIndex)
        {

            // --------------------------------------------------------------------------

            InflowLoad = 0;
            DischargeLoad = 0;
            // OOSDischFracLoad = 1;
            // OOSInflowFracLoad = 1;

            // not Linked Mode or "calibration" linked mode
            // Calculate Inflow
            if (!(Calc_Method == VolumeMethType.Manning))
            {
                // 0 is inflow in this case
                InflowLoad = (LoadsRec.Alt_Loadings[0].ReturnLoad(TimeIndex));
            }

            // Calculate Discharge
            if ((Calc_Method == VolumeMethType.Manning) || (Calc_Method == VolumeMethType.Dynam))
            {
                // 1 is discharge in this case
                DischargeLoad = (LoadsRec.Alt_Loadings[1].ReturnLoad(TimeIndex));
            }


            // Calculate Known Value
            KnownValueLoad = 0;
            if (Calc_Method == VolumeMethType.KnownVal)
            {
                KnownValueLoad = LoadsRec.Loadings.ReturnTSLoad(TimeIndex);  // time series inputs only
            }

            DeltaVolume();
        }  // CalculateLoad


        // ************************************************************
        // *                                                          *
        // *   NEW ESTUARY CODE,  AS SPECIFIED IN OCTOBER 2002 QAPP   *
        // *                                                          *
        // ************************************************************
        public TVolume EpiVol()
        {
            if (AQTSeg.UpperSeg) return this;
            return AQTSeg.otherseg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
        }
        
        public double FreshwaterHead()
        {
            return EpiVol().ResidFlow() / AQTSeg.Location.Locale.SurfArea.Val;
            //m on a given day       // m3/d          // m2
        }

        public double FracUpper()  // not used, use fresh water head for pulsing and if negative entrain and send out fresh?
        {
            double result;
            double FWH;
            FWH = FreshwaterHead();  // m
            if ((TidalAmplitude(this.AQTSeg.TPresent) + FWH) == 0)
            {
                result = 0.99; // avoid a crash
            }
            else
            {
                result = 1.5 * (FWH / (TidalAmplitude(this.AQTSeg.TPresent) + FWH));
            } // unitless  // m  // m        // m

            if (result > 1)  result = 0.99;
            if (result <= 0) result = 0.01;
            return result;
        }

        public void GetSalinities(out double SalUp, out double SalLow)
        {
            if (AQTSeg.UpperSeg)
            {
                SalUp = AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
                SalLow = AQTSeg.otherseg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            }
            else
            {
                SalUp = AQTSeg.otherseg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
                SalLow = AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            };
        }

        public double SaltWaterInflow()
        {
            double SalinityUpper, SalinityLower;
            GetSalinities(out SalinityUpper, out SalinityLower);

            if (SalinityUpper <= 0) 
                throw new Exception("Upper Salinity must always be greater than zero for calculations in \"salt balance\" approach.");

            if (SalinityUpper > (0.99 * SalinityLower)) return 99.0 * EpiVol().ResidFlow();
            else return EpiVol().ResidFlow() / ((SalinityLower / SalinityUpper) - 1);
        }

        public double UpperOutflow()
        {
            double SalinityUpper, SalinityLower;
            GetSalinities(out SalinityUpper, out SalinityLower);
            
            if (SalinityUpper > (0.99 * SalinityLower))
                return 100 * EpiVol().ResidFlow();
            else
                return EpiVol().ResidFlow() / (1 - (SalinityUpper / SalinityLower));
        }

        // ----------------------------------------------------------------------------------------------------------
        public double TidalAmplitude(DateTime TimeIndex)
        {
            double result;
            const int NUM_CONSTITUENTS = 8;
            double Height;
            int n;
            int ThisYr;
            double[] amplitude = new double[NUM_CONSTITUENTS + 1];
            double[] k = new double[NUM_CONSTITUENTS + 1];
            double THrs, TStart, HeightMax, HeightMin;

            // Speeds of each constituent in degrees per hour
            double[] cspeed = { 28.9841042, 30.0000000, 28.4397295, 15.0410686, 13.9430356, 0.0821373, 0.0410686, 14.9589314 };
            // node factor for each constituent for each year
            double[,] nodefactor = { { 0.9665, 0.9734, 0.9833, 0.9953, 1.0078, 1.0195, 1.0291, 1.0355, 1.0378, 1.0359, 1.0299, 1.0205, 1.0089, 0.9964, 0.9844, 0.9742, 0.9670, 0.9635, 0.9641, 0.9687, 0.9769, 0.9878, 1.0001, 1.0125, 1.0236, 1.0320, 1.0369, 1.0376, 1.0340, 1.0266, 1.0162, 1.0041, 0.9916, 0.9802, 0.9710, 0.9651, 0.9632, 0.9654, 0.9715, 0.9809, 0.9925, 1.0050, 1.0170, 1.0272, 1.0344, 1.0377, 1.0367, 1.0315, 1.0229, 1.0117, 0.9992, 0.9870, 0.9763, 0.9683, 0.9639, 0.9636, 0.9674, 0.9748, 0.9852, 0.9973, 1.0098, 1.0212, 1.0304, 1.0361, 1.0378, 1.0352, 1.0286, 1.0188 }, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                { 0.9665, 0.9734, 0.9833, 0.9953, 1.0078, 1.0195, 1.0291, 1.0355, 1.0378, 1.0359, 1.0299, 1.0205, 1.0089, 0.9964, 0.9844, 0.9742, 0.9670, 0.9635, 0.9641, 0.9687, 0.9769, 0.9878, 1.0001, 1.0125, 1.0236, 1.0320, 1.0369, 1.0376, 1.0340, 1.0266, 1.0162, 1.0041, 0.9916, 0.9802, 0.9710, 0.9651, 0.9632, 0.9654, 0.9715, 0.9809, 0.9925, 1.0050, 1.0170, 1.0272, 1.0344, 1.0377, 1.0367, 1.0315, 1.0229, 1.0117, 0.9992, 0.9870, 0.9763, 0.9683, 0.9639, 0.9636, 0.9674, 0.9748, 0.9852, 0.9973, 1.0098, 1.0212, 1.0304, 1.0361, 1.0378, 1.0352, 1.0286, 1.0188 }, 
                { 1.1052, 1.0884, 1.0627, 1.0294, 0.9910, 0.9514, 0.9161, 0.8912, 0.8817, 0.8897, 0.9133, 0.9479, 0.9873, 1.0260, 1.0600, 1.0864, 1.1040, 1.1122, 1.1108, 1.0998, 1.0795, 1.0507, 1.0150, 0.9755, 0.9369, 0.9050, 0.8855, 0.8827, 0.8972, 0.9257, 0.9629, 1.0027, 1.0399, 1.0712, 1.0943, 1.1083, 1.1128, 1.1077, 1.0930, 1.0693, 1.0375, 0.9999, 0.9602, 0.9234, 0.8957, 0.8824, 0.8864, 0.9068, 0.9394, 0.9782, 1.0176, 1.0529, 1.0812, 1.1008, 1.1112, 1.1119, 1.1031, 1.0849, 1.0578, 1.0235, 0.9845, 0.9453, 0.9113, 0.8886, 0.8818, 0.8925, 0.9183, 0.9541 }, 
                { 1.1702, 1.1428, 1.1010, 1.0470, 0.9849, 0.9207, 0.8629, 0.8216, 0.8057, 0.8190, 0.8582, 0.9150, 0.9789, 1.0415, 1.0965, 1.1395, 1.1683, 1.1818, 1.1794, 1.1613, 1.1282, 1.0814, 1.0237, 0.9598, 0.8970, 0.8444, 0.8121, 0.8074, 0.8315, 0.8786, 0.9394, 1.0038, 1.0640, 1.1146, 1.1524, 1.1754, 1.1828, 1.1743, 1.1503, 1.1115, 1.0601, 0.9994, 0.9349, 0.8748, 0.8290, 0.8068, 0.8135, 0.8475, 0.9011, 0.9643, 1.0279, 1.0850, 1.1309, 1.1631, 1.1801, 1.1813, 1.1668, 1.1370, 1.0930, 1.0374, 0.9745, 0.9107, 0.8549, 0.8171, 0.8059, 0.8237, 0.8665, 0.9250 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
            // equilibrium argument for each constituent for each year in degrees for the meridian of Greenwich
            double[,] equilarg = { { 165.43, 266.78, 7.96, 84.54, 185.26, 285.75, 26.03, 101.78, 201.83, 301.88, 42.01, 117.89, 218.36, 319.06, 60.00, 136.77, 238.11, 339.56, 81.04, 158.08, 259.37, 0.47, 101.33, 177.58, 277.98, 18.20, 118.30, 193.96, 294.03, 34.20, 134.53, 210.71, 311.50, 52.53, 153.76, 230.77, 332.24, 73.70, 175.08, 251.92, 352.94, 93.71, 194.25, 270.19, 10.35, 110.42, 210.46, 286.18, 26.41, 126.83, 227.47, 303.98, 45.09, 146.39, 247.82, 324.92, 66.36, 167.68, 268.83, 345.37, 86.05, 186.50, 286.76, 2.49, 102.54, 202.60, 302.74, 18.65 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 270.11, 282.73, 295.19, 269.98, 281.98, 293.74, 305.31, 279.27, 290.60, 301.93, 313.33, 287.43, 299.17, 311.15, 323.36, 298.35, 310.97, 323.69, 336.45, 311.70, 324.27, 336.65, 348.79, 323.25, 334.93, 346.43, 357.80, 331.67, 343.02, 354.47, 6.08, 340.47, 352.54, 4.84, 17.36, 352.58, 5.32, 18.07, 30.72, 5.78, 18.07, 30.12, 41.94, 16.09, 27.53, 38.87, 50.20, 24.13, 35.63, 47.33, 59.25, 33.97, 46.36, 58.94, 71.64, 46.95, 59.67, 72.28, 84.70, 59.45, 71.41, 83.14, 94.67, 68.62, 79.95, 91.28, 102.70, 76.83 },
                { 13.44, 15.49, 17.10, 19.05, 19.16, 18.22, 16.17, 14.11, 10.46, 6.78, 3.66, 2.49, 1.46, 1.47, 2.36, 4.91, 6.93, 9.21, 11.55, 14.76, 16.66, 18.05, 18.71, 19.41, 18.04, 15.58, 12.24, 9.48, 5.96, 3.18, 1.46, 1.83, 2.21, 3.38, 5.15, 8.28, 10.60, 12.92, 15.04, 17.76, 18.89, 19.20, 18.51, 17.70, 14.86, 11.30, 7.56, 5.26, 2.87, 1.58, 1.37, 3.07, 4.49, 6.43, 8.66, 12.00, 14.26, 16.25, 17.78, 19.60, 19.55, 18.43, 16.21, 14.02, 10.32, 6.69, 3.71, 2.72 },
                { 150.70, 249.04, 347.76, 61.72, 161.97, 263.47, 6.50, 85.70, 191.28, 296.92, 41.60, 119.41, 221.05, 321.40, 60.80, 134.19, 232.56, 330.72, 68.82, 141.65, 240.11, 339.03, 78.68, 154.01, 256.08, 359.72, 104.77, 185.15, 290.51, 34.64, 137.21, 212.94, 312.88, 52.01, 150.59, 223.48, 321.60, 59.73, 158.00, 231.24, 330.41, 70.43, 171.62, 248.92, 353.16, 98.58, 204.31, 283.91, 27.45, 129.41, 230.02, 304.24, 43.13, 141.57, 239.75, 312.48, 50.65, 149.04, 247.83, 321.92, 62.35, 164.09, 267.37, 346.78, 92.45, 197.99, 302.46, 20.01 },
                { 200.47, 199.99, 199.52, 201.01, 200.53, 200.05, 199.58, 201.07, 200.59, 200.12, 199.64, 201.13, 200.65, 200.18, 199.70, 201.19, 200.72, 200.24, 199.76, 201.26, 200.78, 200.30, 199.82, 201.32, 200.84, 200.36, 199.88, 201.38, 200.90, 200.42, 199.95, 201.44, 200.96, 200.48, 200.01, 201.50, 201.02, 200.55, 200.07, 201.56, 201.09, 200.61, 200.13, 201.62, 201.15, 200.67, 200.19, 201.69, 201.21, 200.73, 200.25, 201.75, 201.27, 200.79, 200.32, 201.81, 201.33, 200.85, 200.38, 201.87, 201.39, 200.92, 200.44, 201.93, 201.45, 200.98, 200.50, 201.99 },
                { 280.24, 280.00, 279.76, 280.50, 280.27, 280.03, 279.79, 280.54, 280.30, 280.06, 279.82, 280.57, 280.33, 280.09, 279.85, 280.60, 280.36, 280.12, 279.88, 280.63, 280.39, 280.15, 279.91, 280.66, 280.42, 280.18, 279.94, 280.69, 280.45, 280.21, 279.97, 280.72, 280.48, 280.24, 280.00, 280.75, 280.51, 280.27, 280.03, 280.78, 280.54, 280.30, 280.07, 280.81, 280.57, 280.33, 280.10, 280.84, 280.60, 280.37, 280.13, 280.87, 280.64, 280.40, 280.16, 280.90, 280.67, 280.43, 280.19, 280.94, 280.70, 280.46, 280.22, 280.97, 280.73, 280.49, 280.25, 281.00 },
                { 349.76, 350.00, 350.24, 349.50, 349.73, 349.97, 350.21, 349.46, 349.70, 349.94, 350.18, 349.43, 349.67, 349.91, 350.15, 349.40, 349.64, 349.88, 350.12, 349.37, 349.61, 349.85, 350.09, 349.34, 349.58, 349.82, 350.06, 349.31, 349.55, 349.79, 350.03, 349.28, 349.52, 349.76, 350.00, 349.25, 349.49, 349.73, 349.97, 349.22, 349.46, 349.70, 349.93, 349.19, 349.43, 349.67, 349.90, 349.16, 349.40, 349.63, 349.87, 349.13, 349.36, 349.60, 349.84, 349.10, 349.33, 349.57, 349.81, 349.06, 349.30, 349.54, 349.78, 349.03, 349.27, 349.51, 349.75, 349.00 } };

            // optimization
            //if (Convert.ToInt64(TimeIndex) == LastTimeTA)
            //{
            //    // calculate only once per day
            //    result = LastCalcTA;
            //    return result;
            //}

            ThisYr = TimeIndex.Year;

            if ((ThisYr < 1970) || (ThisYr > 2037))
                throw new Exception("TidalAmplitude only works in a date range of 1970-2037");

            TStart = (TimeIndex.DayOfYear - 1) * 24;
            THrs = TStart;

            SiteRecord loc = AQTSeg.Location.Locale;
            amplitude[1] = loc.amplitude1.Val; k[1] = loc.k1.Val; // m2
            amplitude[2] = loc.amplitude2.Val; k[2] = loc.k2.Val; // s2
            amplitude[3] = loc.amplitude3.Val; k[3] = loc.k3.Val; // n2
            amplitude[4] = loc.amplitude4.Val; k[4] = loc.k4.Val; // k1
            amplitude[5] = loc.amplitude5.Val; k[5] = loc.k5.Val; // o1
            amplitude[6] = loc.amplitude6.Val; k[6] = loc.k6.Val; //SSA
            amplitude[7] = loc.amplitude7.Val; k[7] = loc.k7.Val; //SA
            amplitude[8] = loc.amplitude8.Val; k[8] = loc.k8.Val; //P1

            HeightMax = -9e99;
            HeightMin = 9e99;
            do
            {
                // Sum each factor
                Height = 0;
                // Datum irrelevant for amplitude calculation
                for (n = 1; n <= NUM_CONSTITUENTS; n++)
                    Height = Height + (amplitude[n] * nodefactor[n, ThisYr] * Math.Cos(((cspeed[n] * THrs) + equilarg[n, ThisYr] - k[n] * (Math.PI / 180))));

                THrs = THrs + 0.1;
                if (Height < HeightMin) HeightMin = Height;

                if (Height > HeightMax) HeightMax = Height;

            } while (!(THrs > TStart + 24));

            result = (HeightMax - HeightMin) / 2;
            // m

            //LastTimeTA = TimeIndex;
            //LastCalcTA = result;

            return result;
        }


        // --------------------------------------------------------------------------------------
        public override void Derivative(ref double DB)
        {
            // ************************************
            // Calculate a change in volume
            // coded by JSC,
            // Modified by JSC 7/20/98 (% Change)
            // ************************************
            // Does not account for pore water exchanges in the overall water volume, they are assumed
            // to be negligable.  The toxicant in the pore waters is tracked
            double Evap = Evaporation();
            double Entr = 0;

            void Derivative_WriteRates()
            {
                // if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))  always save rates for volume, for data passage
                {
                    ClearRate();
                    SaveRate("Inflow", Inflow);
                    SaveRate("Discharge", Discharg);
                    SaveRate("Evap", Evap);
                    if (AQTSeg.EstuarySeg) SaveRate("Entrainment", Entr);
                }
            }

            if (AQTSeg.EstuarySeg)
            {
                DeltaVolume();
                if (AQTSeg.UpperSeg)
                {
                    Inflow = ResidFlow();  // inflow - evaporation
                    Entr = SaltWaterInflow(); // entrainment = salt water inflow
                    Discharg = UpperOutflow();
                }
                else // lower segment in estuary
                {
                    Evap = 0;
                    Inflow = SaltWaterInflow();
                    Entr = - Inflow;  // entrainment = salt water inflow
                    Discharg = 0;
                }
            }

            else // not estuary segment
            {
                switch (Calc_Method)
                {
                    case VolumeMethType.KeepConst:
                        // db = 0
                        Inflow = InflowLoad;
                        Discharg = Inflow - Evap;
                        if (Discharg < 0)
                        {
                            Discharg = 0;
                            Inflow = Evap;
                        }
                        break;
                    case VolumeMethType.Dynam:
                        Inflow = InflowLoad;
                        Discharg = DischargeLoad;
                        break;
                    case VolumeMethType.KnownVal:
                        // db = knownvalload-state, inflow, evap known
                        Inflow = InflowLoad;
                        Discharg = Inflow - Evap + State - KnownValueLoad;
                        if (Discharg < 0)
                        {
                            Inflow = Inflow - Discharg;
                            Discharg = 0;
                        }
                        break;
                    case VolumeMethType.Manning:
                        // db = Manning_Volume-state, discharge, evap known
                        Discharg = DischargeLoad;
                        Inflow = Manning_Volume() - State + Discharg + Evap;

                        if (Discharg < Consts.Tiny)   // new code 6/3/2021, hold things constant at state where flows = 0 for NWM linkage
                        {
                            Inflow = 0;
                            Discharg = 0;  // handle negative case
                            Evap = 0;
                        }
                        else if (Inflow < 0)
                        {
                            Discharg = Discharg - Inflow;
                            Inflow = 0;
                        }
                        break;
                }    // Switch

                DeltaVolume();
                // change value of TotDischarge
            }

            Derivative_WriteRates();

            DB = Inflow - Discharg - Evap + Entr;
            if (Math.Abs(DB) < Globals.Consts.Small) DB = 0;
            AQTSeg.VolumeUpdated = AQTSeg.TPresent;

        } //TVolume.Derivative

        // ------------------------------------------------------
        public void SetMeanDischarge_AverageVolumeLoads(ref DateTime TimeIndex, ref double AverageDischargeLoad, ref double AverageInflowLoad)
        {
            DateTime DateIndex;
            double N;
            double Sum_Dl;
            double Sum_IL;
            Sum_Dl = 0;
            Sum_IL = 0;
            N = 0;
            DateIndex = TimeIndex.AddDays(-1);
            do
            {
                DateIndex = DateIndex.AddDays(1);
                N = N + 1;
                CalculateLoad(DateIndex);
                Sum_Dl = Sum_Dl + DischargeLoad;
                Sum_IL = Sum_IL + InflowLoad;
            } while (!((DateIndex.AddDays( -365) >= TimeIndex) || (DateIndex >= AQTSeg.PSetup.LastDay.Val)));

            AverageDischargeLoad = Sum_Dl / N;
            AverageInflowLoad = Sum_IL / N;
            CalculateLoad(TimeIndex);
            // reset TVolume values

        }

        // ------------------------------------------------------
        public double SetMeanDischarge_CalcKnownValueMD(ref DateTime TimeIndex, ref double MV)
        {
            double result;
            // Calculates the discharge for every time step then averages over a year
            // Given that KnownValLoad - State = Inflow - Discharge - Evap
            // and that for each day, State should = KnownValLoad(T-1) then
            // Discharge = Inflow - KnownValLoad + KnownValLoad(T-1) - Evap
            DateTime DateIndex;
            double N;
            double KnownVal_Tminus1;
            double Sum_Disch;
            double SumVol;
            Sum_Disch = 0;
            SumVol = 0;
            KnownVal_Tminus1 = InitialCond;
            N = 0;
            DateIndex = TimeIndex.AddDays(-1);
            do
            {
                DateIndex = DateIndex.AddDays(1);
                N = N + 1;
                CalculateLoad(DateIndex);
                Sum_Disch = Sum_Disch + InflowLoad - KnownValueLoad + KnownVal_Tminus1 - Evaporation();
                // handle dynamic evaporation properly
                KnownVal_Tminus1 = KnownValueLoad;
                SumVol = SumVol + KnownValueLoad;
            } while (!(((DateIndex.AddDays(-365)) >= TimeIndex) || (DateIndex >= AQTSeg.PSetup.LastDay.Val)));
            result = Sum_Disch / N;
            MV = SumVol / N;
            if (result < 0.0)
            {
                result = 0.0;
            }
            CalculateLoad(TimeIndex);
            // reset TVolume values

            return result;
        }

        // ------------------------------------------------------
        //public void SetMeanDischarge_CalcEstMeanVars()
        //{
        //    // Calculate MeanDischarge and MeanEstVel for Estuaries
        //    double N = 0;
        //    double DateIndex;
        //    double SumEstVel;
        //    double SumDisch;
        //    DateTime TTPres = AQTSeg.TPresent;
        //    TSalinity TS;

        //    TS = GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
        //    SumEstVel = 0;
        //    SumDisch = 0;
        //    DateIndex = TimeIndex - 1;
        //    do
        //    {
        //        DateIndex = DateIndex + 1;
        //        N = N + 1;
        //        TPresent = DateIndex;
        //        CalculateLoad(DateIndex);
        //        TS.CalculateLoad(DateIndex);
        //        Location.Discharge[VerticalSegments.Epilimnion] = UpperOutflow();
        //        SumEstVel = SumEstVel + Velocity(0, 0, false);
        //        SumDisch = SumDisch + Location.Discharge[VerticalSegments.Epilimnion];
        //    } while (!(((DateIndex - 365) >= TimeIndex) || (DateIndex >= SetupRec.LastDay.Val)));
        //    MeanDischarge = SumDisch / N;
        //    MeanEstVel = SumEstVel / N;
        //    TPresent = TTPres;
        //    CalculateLoad(TimeIndex);
        //    // reset TVolume values
        //    TS.CalculateLoad(TimeIndex);
        //    Location.Discharge[VerticalSegments.Epilimnion] = UpperOutflow();
        //}

        // ------------------------------------------------------
        public double SetMeanDischarge_CalcDynamicMV(ref DateTime TimeIndex)
        {
            double result;
            // Calculates the volume for every time step then averages over a year
            DateTime DateIndex;
            double N;
            double DynamVol;
            double SumVol;
            SumVol = 0;
            DynamVol = State;
            N = 0;
            DateIndex = TimeIndex.AddDays ( -1);
            do
            {
                DateIndex = DateIndex.AddDays(1);
                N = N + 1;
                CalculateLoad(DateIndex);
                DynamVol = DynamVol + InflowLoad - DischargeLoad - Evaporation();
                // handle dynamic evaporation properly
                SumVol = SumVol + DynamVol;
            } while (!((DateIndex.AddDays(-365) >= TimeIndex) || (DateIndex >= AQTSeg.PSetup.LastDay.Val)));
            result = SumVol / N;
            if (result < 0)
            {
                result = 0;
            }
            CalculateLoad(TimeIndex);
            // reset TVolume values

            return result;
        }

        public void SetMeanDischarge(DateTime TimeIndex)
        {
            double MD;
            double MV;
            double AverageDischargeLoad=0;
            double AverageInflowLoad=0;
            // ------------------------------------------------------
            double Temp;
            MD = 0;
            MV = 0;
            AQTSeg.MeanDischarge = 0;
            AQTSeg.MeanVolume = 0;

            if ((Calc_Method==VolumeMethType.Dynam)|| (Calc_Method == VolumeMethType.KeepConst)|| (Calc_Method == VolumeMethType.Manning))
            {
                SetMeanDischarge_AverageVolumeLoads(ref TimeIndex,ref AverageDischargeLoad,ref AverageInflowLoad);
            }
            switch (Calc_Method)
            {
                case VolumeMethType.Manning:
                    // Meandischarge is set, depending on the volume calculation method
                    MD = AverageDischargeLoad;
                    Temp = Discharg;
                    // calculate manning's volume based on mean discharge
                    Discharg = MD;
                    MV = Manning_Volume();
                    Discharg = Temp;
                    break;
                case VolumeMethType.Dynam:
                    MD = AverageDischargeLoad;
                    MV = SetMeanDischarge_CalcDynamicMV(ref TimeIndex );
                    break;
                case VolumeMethType.KeepConst:
                    MD = AverageInflowLoad - Evaporation();
                    // Currently Assuming Evap is constant over the year
                    // need to handle dynamic evaporation properly
                    if (MD < 0)  { MD = 0; }
                    MV = InitialCond;
                    break;
                case VolumeMethType.KnownVal:
                    MD = SetMeanDischarge_CalcKnownValueMD(ref TimeIndex, ref MV);
                    break;
                    // Also Assuming Evap is constant over the year, this can be changed
                    // MV Calculated in CalcKnownValueMD as well
            }
            // Case

            //if (Stratified && !LinkedMode)
            //{
            //    MorphRecord _wvar1 = Location.Morph;
            //    switch (StratOutflow)
            //    {
            //        case FlowType.FTBoth:
            //            MeanDischarge = MD * (SegVol() / Volume_Last_Step);
            //            break;
            //        case FlowType.FTEpi:
            //            // Discharge is split up between Epi & Hyp segments weighted by volume
            //            if (VSeg == VerticalSegments.Epilimnion)
            //            {
            //                MeanDischarge = MD;
            //            }
            //            else
            //            {
            //                MeanDischarge = 0;
            //            }
            //            break;
            //        case FlowType.FTHyp:
            //            if (VSeg == VerticalSegments.Hypolimnion)
            //            {
            //                MeanDischarge = MD;
            //            }
            //            else
            //            {
            //                MeanDischarge = 0;
            //            }
            //            break;
            //    }
            //    // Case
            //}
            //else
            //{
                AQTSeg.MeanDischarge = MD;

            //}
            //if (Stratified && !LinkedMode)
            //{
            //    MorphRecord _wvar2 = Location.Morph;
            //    // Volume is split up between Epi & Hyp segments
            //    MeanVolume = MV * (SegVol() / Volume_Last_Step);
            //}
            //else
            //{
                AQTSeg.MeanVolume = MV;
            //}

            if (AQTSeg.MeanVolume == 0)
            {
                SetMeanDischarge_CalcDynamicMV(ref TimeIndex);
            }
        }




    } // end TVolume

} // Namespace

