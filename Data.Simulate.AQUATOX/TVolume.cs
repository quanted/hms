using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using Globals;
using Newtonsoft.Json;

namespace AQUATOX.Volume
{

    public enum VolumeMethType
    {
        Manning,
        KeepConst,
        Dynam, 
        KnownVal
    } // end VolumeMethType

   
    public class TVolume : TStateVariable
    {
        [JsonIgnore] double LastCalcTA ;  
        [JsonIgnore] double LastTimeTA ;        // don't need saving
        [JsonIgnore] double Inflow = 0;
        [JsonIgnore] double Discharg = 0;          // don't need saving
        [JsonIgnore] double InflowLoad = 0;
        [JsonIgnore] double DischargeLoad = 0;
        [JsonIgnore] double KnownValueLoad = 0;
        // [JsonIgnore] double OOSDischFracLoad = 0;
        // [JsonIgnore] double OOSInflowFracLoad = 0;  // don't need saving

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
            LastTimeTA = 0;
            LastCalcTA = 0;
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
            CLength = Location.Locale.SiteLength * 1000;
            // m                        // km     // m/km
            // AVERAGE FLOW DISCHARGE
            Q = Discharg / 86400;
            // m3/s // m3/d  // s/d
            Width = Location.Locale.SurfArea / (Location.Locale.SiteLength * 1000);
            // m                   // sq.m                      // km       // m/km
            Y = Math.Pow((Q * Location.ManningCoeff()) / (Math.Sqrt(Location.Locale.Channel_Slope) * Width), 0.6);
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
            ZZMax3 = Math.Pow(Z / ZMx, 3.0);
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
                return Location.Locale.MeanEvap * (0.0254 / 365) * Location.Locale.SurfArea;
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
            result = Loading - Evaporation();
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
            // *         Volume, InflowH2O, Discharge, ZMix, MeanThick, TotDischarge                          *
            // *                                                                                              *
            // *  Other Morph variables are not dynamic over a study run, such as                             *
            // *         P_Shape, ECoeffWater.  They are not modified by this Procedure              *
            // *                                                                                              *
            // *  DeltaVolume is called whenever the Volume S.V. or Volume Loadings Data have changed         *
            // *                                                                                              *
            // ************************************************************************************************
            double Avg_Disch;
            double WidthCalc;
            double Channel_Depth;
            // ----------------------------------------------------------
            if (this.State < Consts.Tiny)
            {
                this.State = 0;
            }            // Volume cannot be negative

            // not Stratified
            Location.Morph.SegVolum = AQTSeg.Volume_Last_Step;

            Location.MeanThick = AQTSeg.DynamicZMean();
            Location.Discharge = Discharg;
            Location.Morph.InflowH2O = Inflow;

            if ((Location.SiteType == SiteTypes.Stream))
            {
                WidthCalc = Location.Locale.SurfArea / (Location.Locale.SiteLength * 1000);
                // m                        // sq.m                      // km     // m/km

                Avg_Disch = Location.Discharge / 86400;
                Channel_Depth = Math.Pow(Avg_Disch * Location.ManningCoeff() / (Math.Sqrt(Location.Locale.Channel_Slope)) * WidthCalc, 3 / 5);
                Location.Morph.XSecArea = WidthCalc * Channel_Depth;
                // m2       // m          // m
            }
            else
            {
                Location.Morph.XSecArea = AQTSeg.Volume_Last_Step / (Location.Locale.SiteLength * 1000);
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
                KnownValueLoad = LoadsRec.Loadings.ReturnLoad(TimeIndex);
            }

            DeltaVolume();
        }  // CalculateLoad

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
                    if (Inflow < 0)
                    {
                        Discharg = Discharg - Inflow;
                        Inflow = 0;
                    }
                    break;
            }    // Switch

            DeltaVolume();
            // change value of TotDischarge
            // Write Rates for Output

            DB = Inflow - Discharg - Evap;
            if (Math.Abs(DB) < Globals.Consts.Small)
            {
                DB = 0;
            }
            AQTSeg.VolumeUpdated = AQTSeg.TPresent;
        } //TVolume.Derivative

    } // end TVolume

} // Namespace

