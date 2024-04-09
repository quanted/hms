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
        // [JsonIgnore] double // LastTimeTA ;        // these don't need saving
        [JsonIgnore] public double Inflow = 0.0;
        [JsonIgnore] public double Discharg = 0.0;    // these don't need saving
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

            if (Q < 0) Q = 0;  //bullet proof 4/4/2024

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

            void Derivative_WriteRates()
            {
                // if ((AQTSeg.PSetup.SaveBRates.Val)&& (SaveRates))  always save rates for volume, for data passage
                {
                    this.ClearRate();
                    this.SaveRate("Inflow", Inflow);
                    this.SaveRate("Discharge", Discharg);
                    this.SaveRate("Evap", Evap);
                }
            }

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
            Derivative_WriteRates();

            DB = Inflow - Discharg - Evap;
            if (Math.Abs(DB) < Globals.Consts.Small)
            {
                DB = 0;
            }
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
        //    double N;
        //    double DateIndex;
        //    double SumEstVel;
        //    double SumDisch;
        //    double TTPres;
        //    TSalinity TS;
        //    N = 0;
        //    TTPres = TPresent;
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

            // AQTSeg.MeanEstVel = 0;
            //if (EstuarySegment)
            //{
            //    // 5-30-2008
            //    MeanVolume = InitialCond;
            //    // for entire system
            //    SetMeanDischarge_CalcEstMeanVars();
            //    return;
            //}

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

