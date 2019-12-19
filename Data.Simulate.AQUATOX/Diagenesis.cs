using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Organisms;
using AQUATOX.OrgMatter;
using AQUATOX.Nutrients;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.Diagenesis
{
    public class TNH4_Sediment : TStateVariable
    {
        // TNH4_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Nitr_Rate(double NH4_1)
        {
            double result;
            // K1 nh4, oxic layer reaction velocity
            double Temp;
            double KappaNH3;
            double O2;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;

            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (AQTSeg.Sulfide_System())
                KappaNH3 = DR.KappaNH3s.Val;
            else
                KappaNH3 = DR.KappaNH3f.Val;

            // EFDC EQ. 5-19
            result = Math.Pow(KappaNH3, 2) * Math.Pow(DR.ThtaNH3.Val, (Temp - 20)) * DR.KM_NH3.Val / (DR.KM_NH3.Val + NH4_1) * O2 / (2 * DR.KM_O2_NH3.Val + O2);
         //(m2/d2)         (m / d)                    (unitless)                        (mg N / L)   (mgN/L)         (mgN/L) (mg/L)              (mg/L)   (mg/L)


            return result;
        }

        // k2 nh4, oxic layer reaction velocity
        public double Flux2Water()
        {
            double result;
            double fda1;
            double s;
            double NH4_0;
            NH4_0 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            fda1 = 1 / (1 + DR.m1.Val * DR.KdNH3.Val);
            result = s * (fda1 * State - NH4_0) / DR.H1.Val;
            // g/m3 d      m/d   g/m3    g/m3                m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //        // loss from the system
        //        LossInKg = (Burial * SedVol * 1e-3);
        //        // kg N/d   g/m3 d    m3      kg/g
        //        TStates DR = AQTSeg;
        //        MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
        //        DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //        DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //        // Loss from the system
        //        DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //    }
        //}

        // rate in g/m3 (sed) d
        public override void Derivative(ref double DB)
        {
            double fpa1, fda1, fpa2, fda2, s, Nitr, Burial, Flux2Anaerobic, Flux2Wat, Dia_Flux, NH4_2, NH4_1;
            AllVariables ns;
            TPON_Sediment ppn;
            // --------------------------------------------------
            // TNH4_Sediment.Derivative
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            NH4_1 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            NH4_2 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer2);
            fda1 = 1 / (1 + DR.m1.Val * DR.KdNH3.Val);
            fpa1 = 1 - fda1;
            fda2 = 1 / (1 + DR.m2.Val * DR.KdNH3.Val);
            fpa2 = 1 - fda2;
            Flux2Anaerobic = -(DR.W12 * (fpa2 * NH4_2 - fpa1 * NH4_1) + DR.KL12 * (fda2 * NH4_2 - fda1 * NH4_1));
            // g/m2 d            m/d     g/m3    g/m3    m/d    g/m3       g/m3

            if (Layer == T_SVLayer.SedLayer1)
                 Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            else Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            // g/m3 d               g/m2 d             m

            if (Layer == T_SVLayer.SedLayer1)
            {
                s = AQTSeg.MassTransfer();
                // m/d
                Burial = DR.w2.Val / DR.H1.Val * State;
                        //    m/d         m       g/m3
                Nitr = Nitr_Rate(State) / s * State / DR.H1.Val;    //EFDC eq. 5 - 20
 //             g/m3 d       m2/d2    m/d   g/m3      m
                Flux2Wat = Flux2Water();
                DB = -Nitr - Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d  // g/m3 d

                if (AQTSeg.Diagenesis_Steady_State)  DB = 0;
                // Layer 1 is STEADY STATE
            }
            else
            {
                // SedLayer2
                Dia_Flux = 0;
                for (ns = AllVariables.PON_G1; ns <= AllVariables.PON_G3; ns++)
                {
                    ppn = (TPON_Sediment) AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                    Dia_Flux = Dia_Flux + ppn.Mineralization();
                    // mg/L d                    // mg/L d
                }
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * NH4_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * NH4_2 / DR.H2.Val;
                DB = Dia_Flux - Burial + Flux2Anaerobic;
                // g/m3 d                // g/m3 d
            }
            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TNH4_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TNH4_Sediment

    public class TNO3_Sediment : TStateVariable
    {
        // TNO3_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Denit_Rate(double NO3Conc)
        {
            double result;
            // eqns 5.23&24
            // m2/d2, L1, m/d L2
            double Temp;
            double KappaNO3;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            if (Layer == T_SVLayer.SedLayer1)
            {
                if (AQTSeg.Sulfide_System())
                {
                    KappaNO3 = DR.KappaNO3_1s.Val;
                }
                else
                {
                    KappaNO3 = DR.KappaNO3_1f.Val;
                }
                result = (Math.Pow(KappaNO3, 2) * Math.Pow(DR.ThtaNO3.Val, (Temp - 20)));
            }
            else
            {
                // SedLayer2, Anaerobic
                result = (DR.KappaNO3_2.Val) * Math.Pow(DR.ThtaNO3.Val, (Temp - 20));
            }
            return result;
        }

        // k2 nh4, oxic layer reaction velocity
        public double Flux2Water()
        {
            double result;
            double s;
            double NO3_0;
            NO3_0 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = s * (State - NO3_0) / DR.H1.Val;
        // g/m3 d   m/d    g/m3    g/m3             m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double DenitrInKg;
        //    double BuryInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //    }
        //    else
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(1);
        //    }
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        // burial loss from the system
        //        BuryInKg = (Burial * SedVol * 1e-3);
        //    }
        //    else
        //    {
        //        BuryInKg = 0;
        //    }
        //    // burial from Layer 1 goes to Layer 2
        //    if ((Layer == T_SVLayer.SedLayer2) || (!AQTSeg.Diagenesis_Steady_State))
        //    {
        //        // don't track denitrification MB in steady-state layer as irrelevant
        //        // denitrification loss from the system
        //        // kg N / d
        //        // g/m3 d
        //        // m3
        //        // kg/g
        //        DenitrInKg = (DeNitr * SedVol * 1e-3);
        //    }
        //    else
        //    {
        //        DenitrInKg = 0;
        //    }
        //    TStates DR = AQTSeg;
        //    MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
        //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + BuryInKg + DenitrInKg;
        //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + BuryInKg;
        //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + BuryInKg + DenitrInKg;
        //    // Loss from the system
        //    DR.Denitrify[DR.DerivStep] = DR.Denitrify[DR.DerivStep] + DenitrInKg;
        //}

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer1))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Nitrification", Nitr);
        //        SaveRate("Denitrification", DeNitr);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Water", Flux2Wat);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer2))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Denitrification", DeNitr);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double s;
            double Nitr;
            double DeNitr;
            double Burial;
            double Flux2Anaerobic;
            double Flux2Wat;
            double NH4_1;
            double NO3_2;
            double NO3_1;
            TNH4_Sediment TNH4_1;
            // --------------------------------------------------
            // TNO3_Sediment.Derivative
            TNH4_1 = (TNH4_Sediment) AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            NO3_1 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            NH4_1 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            NO3_2 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -DR.KL12 * (NO3_2 - NO3_1);
            // g/m2 d
            // m/d
            // g/m3
            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }
            // g/m3 d
            // g/m2 d
            // m
            s = AQTSeg.MassTransfer();
            // m/d
            if (Layer == T_SVLayer.SedLayer1)
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
                // m/d       // m                // mg/L
                Nitr = TNH4_1.Nitr_Rate(NH4_1) / s * NH4_1 / DR.H1.Val;
// EFDC eq. 5-20   // g/m3 d     // m2/d2    // m/d  // g/m3    // m
                DeNitr = Denit_Rate(State) / s * State / DR.H1.Val;
                // g/m3 d        // m2/d2 // m/d // g/m3        // m
                Flux2Wat = Flux2Water();
                DB = Nitr - DeNitr - Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    DB = 0;
                }
                // Layer 1 is STEADY STATE
            }
            else
            {
                // SedLayer2
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * NO3_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * NO3_2 / DR.H2.Val;
                // deep burial
                DeNitr = Denit_Rate(State) * State / DR.H2.Val;
                // g/m3 d          // m/d   // g/m3      // m
                DB = -DeNitr - Burial + Flux2Anaerobic;
                // g/m3 d
            }
            // Derivative_WriteRates();
            // Derivative_TrackMB();
        }

        // rate in g/m3 (sed) d
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TNO3_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TNO3_Sediment

    public class TPO4_Sediment : TStateVariable
    {
        // TPO4_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double fdp1()
        {
            double result;
            double dKDPO41;
            double KDPO41;
            double O2;

            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            if (AQTSeg.Sulfide_System())
            {
                dKDPO41 = DR.dKDPO41s.Val;
            }
            else
            {
                dKDPO41 = DR.dKDPO41f.Val;
            }
            if ((O2 > DR.O2critPO4.Val))
            {
                KDPO41 = DR.KdPO42.Val * dKDPO41;
            }
            else
            {
                KDPO41 = DR.KdPO42.Val * Math.Pow(dKDPO41, (O2 / DR.O2critPO4.Val));
            }
            result = (1 / (1 + DR.m1.Val * KDPO41));
            return result;
        }

        // frac dissolved in layer1
        public double Flux2Water()
        {
            double result;
            double s;
            double PO4_0;
            PO4_0 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = s * (fdp1() * State - PO4_0) / DR.H1.Val;
            // g/m3 d
            // m/d
            // g/m3
            // g/m3
            // m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //        LossInKg = (Burial * SedVol * 1e-3);
        //        // loss from the system
        //        // kg N / d
        //        // g/m3 d
        //        // m3
        //        // kg/g
        //        TStates DR = AQTSeg;
        //        MBLossRecord DR = DR.MBLossArray[AllVariables.Phosphate];
        //        DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //        DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //        // Loss from the system
        //        DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //    }
        //}

        //// --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer1))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Water", Flux2Wat);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer2))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Dia_Flux", Dia_Flux);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double Dia_Flux;
            double Burial;
            double Flux2Anaerobic;
            double Flux2Wat;
            double PO4_2;
            double PO4_1;
            double fdp2;
            double fpp1;
            double fpp2;
            AllVariables ns;
            TPOP_Sediment ppp;
            // --------------------------------------------------
            // TPO4_Sediment.Deriv
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            fpp1 = 1 - fdp1();
            fdp2 = (1 / (1 + DR.m2.Val * DR.KdPO42.Val));
            fpp2 = 1 - fdp2;
            PO4_1 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1);
            PO4_2 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -((DR.W12 * (fpp2 * PO4_2 - fpp1 * PO4_1) + DR.KL12 * (fdp2 * PO4_2 - fdp1() * PO4_1)));
            // g/m2 d
            // m/d
            // g/m3
            // m/d
            // g/m3
            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }
            // g/m3 d
            // g/m2 d
            // m
            if (Layer == T_SVLayer.SedLayer1)
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
                // m/d
                // m
                // g/m3
                Flux2Wat = Flux2Water();
                // g/ m3 d
                DB = -Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    DB = 0;
                }
                // Layer 1 is STEADY STATE
            }
            else
            {
                // SedLayer2
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * PO4_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * PO4_2 / DR.H2.Val;
                Dia_Flux = 0;
                for (ns = AllVariables.POP_G1; ns <= AllVariables.POP_G3; ns++)
                {
                    ppp = (TPOP_Sediment) AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                    Dia_Flux = Dia_Flux + ppp.Mineralization();
                }
                // g/ m3 d
                DB = Dia_Flux - Burial + Flux2Anaerobic;
                // g/m3 d
            }
            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        // rate in g/m3 (sed) d
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPO4_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPO4_Sediment

    public class TPOC_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled

        // TPOC_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            // gC/m3 day
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.POC_G1 => DR.kpoc1.Val * Math.Pow(DR.ThtaPOC1.Val, Temp - 20) * State,
                AllVariables.POC_G2 => DR.kpoc2.Val * Math.Pow(DR.ThtaPOC2.Val, Temp - 20) * State,
                _ => DR.kpoc3.Val * Math.Pow(DR.ThtaPOC3.Val, Temp - 20) * State,
                // g C/m3 d =   // 1/d            // unitless          // g C/m3
            };
            return result;
        }

        public double Burial()
        {
            double result;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = DR.w2.Val / DR.H2.Val * State;
            return result;
        }

        // m/d
        // m
        // mg/L
        public double Predn()
        {
            if (NState == AllVariables.POC_G1)
                return Predation() / Consts.Detr_OM_2_OC;
            if (NState == AllVariables.POC_G2)
                return Predation() / Consts.Detr_OM_2_OC;
            // g OC/m3 w  // g OM /m3    // g OM / g OC

            return 0;
        }

        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Deposition", Deposition);
        //        SaveRate("Mineralization", Minrl);
        //        SaveRate("Burial", Bury);
        //        SaveRate("Predation", Pred);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double Minrl;
            double Deposition;
            double Bury;
            double Pred;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg C/L            // g/m2                                  // m
            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

            //          AQTSeg.Diag_Track[TAddtlOutput.POC_Dep, DR.DerivStep] = DR.Diag_Track[TAddtlOutput.POC_Dep, DR.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d                                                // g/m3 sed             // m sed             // mg/g   
            Minrl = Mineralization();
            Bury = Burial();
            Pred = Predn();
            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
            // ( g/m3 s) = ( g/m3 w) * ( m3 w ) / ( m3 s)

            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            DB = Deposition - Minrl - Burial() - Pred;
         //   Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPOC_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPOC_Sediment

    public class TPON_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled


        // TPON_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.PON_G1 => DR.kpon1.Val * Math.Pow(DR.ThtaPON1.Val, Temp - 20) * State,
                AllVariables.PON_G2 => DR.kpon2.Val * Math.Pow(DR.ThtaPON2.Val, Temp - 20) * State,
                _ => DR.kpon3.Val * Math.Pow(DR.ThtaPON3.Val, Temp - 20) * State,
            };
            // g N/m3 d  =   // 1/d             // unitless           // g N/m3

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    SedVol = AQTSeg.DiagenesisVol(2);
        //    LossInKg = (Burial * SedVol * 1e-3);
        //    // burial loss from the system
        //    // kg N / d            // g/m3 d            // m3            // kg/g
        //    TStates DR = AQTSeg;
        //    MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
        //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //    // Loss from the system
        //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //}

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Deposition", Deposition);
        //        SaveRate("Mineralization", Minerl);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Predation", Pred);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double Minerl;
            double Deposition;
            double Burial;
            double Pred;
            // --------------------------------------------------
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            Minerl = Mineralization();
            Deposition =AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg/L            // g/m2             // m

            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

            // Diag_Track[TAddtlOutput.PON_Dep, DR.DerivStep] = Diag_Track[TAddtlOutput.PON_Dep, DR.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d             // g/m3 sed            // m sed            // mg/g
            Burial = DR.w2.Val / DR.H2.Val * State;
            // m/d         // m   // mg/L

            Pred = 0;
            if (NState == AllVariables.PON_G1)   
                Pred = Predation() * Location.Remin.N2OrgLab;

            if (NState == AllVariables.PON_G2)
                Pred = Predation() * Location.Remin.N2Org_Refr;
             // g N/m3 w     // g OM /m3             // g N / g OM

            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
      // g/m3*sed  g/m3 w    // m3 water         // m3 sed

            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            DB = Deposition - Minerl - Burial - Pred;

         // Derivative_WriteRates();
         // Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPON_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPON_Sediment

    public class TPOP_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled

        // TPOP_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.POP_G1 => DR.kpop1.Val * Math.Pow(DR.ThtaPOP1.Val, Temp - 20) * State,
                AllVariables.POP_G2 => DR.kpop2.Val * Math.Pow(DR.ThtaPOP2.Val, Temp - 20) * State,
                _ => DR.kpop3.Val * Math.Pow(DR.ThtaPOP3.Val, Temp - 20) * State,
                // g P/m3 d   // 1/d           // unitless                 // g C/m3
            };
            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    SedVol = AQTSeg.DiagenesisVol(2);
        //    LossInKg = (Burial * SedVol * 1e-3);
        //    // burial loss from the system
        //    // kg N / d
        //    // g/m3 d
        //    // m3
        //    // kg/g
        //    TStates DR = AQTSeg;
        //    MBLossRecord DR = DR.MBLossArray[AllVariables.Phosphate];
        //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //    // Loss from the system

        //}

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Deposition", Deposition);
        //        SaveRate("Mineralization", Minerl);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Predation", Pred);
        //    }
        //}
        // --------------------------------------------------

        public override void Derivative(ref double DB)
        {
            double Deposition;
            double Burial;
            double Minerl;
            double Pred;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            Minerl = Mineralization();
            Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg/L d            // g/m2 d

            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

                        // m
            //            AQTSeg.Diag_Track[TAddtlOutput.POP_Dep, AQTSeg.DerivStep] = AQTSeg.Diag_Track[TAddtlOutput.POP_Dep, AQTSeg.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d            // g/m3 sed            // m sed            // mg/g
            Burial = DR.w2.Val / DR.H2.Val * State;
            // g/m3    // m/d      // m       // g/m3

            Pred = 0;
            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            if (NState == AllVariables.POP_G1)   
                Pred = Predation() * Location.Remin.P2OrgLab;

            if (NState == AllVariables.POP_G2)
                Pred = Predation() * Location.Remin.P2Org_Refr;
            //g P/m3 w  // g P /m3             // g P / g OM

            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
        // g/m3 s // g/m3 w  // m3 w                // m3 s

            DB = Deposition - Minerl - Burial - Pred;
            // g/m3 d

            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPOP_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPOP_Sediment

    public class TMethane : TStateVariable
    {
        // TMethane
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Dia_Flux", Dia_Flux);
        //        SaveRate("Flux2Water", Flux2Wat);
        //        SaveRate("Oxidation", Oxid);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double SECH_ARG;
            double Dia_Flux;
            double Flux2Wat;
            double Oxid;
            TNO3_Sediment PNO3_1;
            TNO3_Sediment PNO3_2;
            double CSODmax;
            double Temp;
            double CSOD;
            double CH4Sat;
            double S;
            double JO2NO3;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            // ppt

            if (AQTSeg.Sulfide_System())
            {
                // Goes to Sulfide Instead
                Dia_Flux = 0;
                Flux2Wat = 0;
                Oxid = 0;
                DB = 0;
                // Derivative_WriteRates();
                return;
            }
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            // CH4_0  := GetState(Methane,StV,WaterCol);
            Dia_Flux = AQTSeg.Diagenesis(Layer) * DR.H2.Val;
         // g O2/m2 d            // g O2/m3 d         // m

            S = AQTSeg.MassTransfer();
            // m/d
            PNO3_1 = (TNO3_Sediment) AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            PNO3_2 = (TNO3_Sediment) AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
            // g/m2 d
            JO2NO3 = 2.86 * (PNO3_1.Denit_Rate(PNO3_1.State) / S * PNO3_1.State + PNO3_2.Denit_Rate(PNO3_2.State) * PNO3_2.State);
                     // m/d            // g/m3
            Dia_Flux = Dia_Flux - JO2NO3;

            if (Dia_Flux < 0)
                Dia_Flux = 0;

            CH4Sat = 100 * (1 + AQTSeg.DynamicZMean() / 10) * Math.Pow(1.024, (20 - Temp));
            // saturation conc of methane in pore water {g 02/m3
            CSODmax = Math.Min(Math.Sqrt(2 * DR.KL12 * CH4Sat * Dia_Flux), Dia_Flux);
            SECH_ARG = (DR.KappaCH4.Val * Math.Pow(DR.ThtaCH4.Val, (Temp - 20))) / S;
            // CSOD Equation 10.35 from DiTorro
            // The hyperbolic secant is defined as HSec(X) = 2 / (Exp(X) + Exp(-X))
            if ((SECH_ARG < 400))
            {
                CSOD = CSODmax * (1 - (2 / (Math.Exp(SECH_ARG) + Math.Exp(-SECH_ARG))));
            }
            else
            {
                CSOD = CSODmax;
            }
            // HSec(SECH_ARG) < 3.8E-174 ~ 0
            Flux2Wat = Dia_Flux - CSOD;
            // (CSODmax - CSOD);
            // oxidation
            Oxid = CSOD;
            DB = (Dia_Flux - Flux2Wat - Oxid) / DR.H2.Val;
       // g O2eq / m3 d    // g O2eq / m2 d            // m
            // Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TMethane(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TMethane

    public class TSulfide_Sediment : TStateVariable
    {

        public double k2Oxid()
        {
            double result;
            // reaxn vel for sulfide oxidation
            double Temp;
            double O2;
            double fdh2s1;
            double fph2s1;
            result = 0;
            if (Layer == T_SVLayer.SedLayer2)
            {
                return result;
            }
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            fdh2s1 = 1 / (1 + DR.m1.Val * DR.KdH2S1.Val);
            fph2s1 = 1 - fdh2s1;
            result = (Math.Pow(DR.KappaH2Sd1.Val, 2) * fdh2s1 + (Math.Pow(DR.KappaH2Sp1.Val, 2) * fph2s1)) * Math.Pow(DR.ThtaH2S.Val, Temp - 20) * O2 / (2 * DR.KMHSO2.Val);
            // g2/m2

            return result;
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        //public void Derivative_WriteRates()
        //{
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        if (Layer == T_SVLayer.SedLayer1)
        //        {
        //            SaveRate("Oxidation", Oxid);
        //        }
        //        if (Layer == T_SVLayer.SedLayer1)
        //        {
        //            SaveRate("Flux2Water", Flux2Wat);
        //        }
        //        if (Layer == T_SVLayer.SedLayer2)
        //        {
        //            SaveRate("Dia_Flux", Dia_Flux);
        //        }
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double Burial;
            double Oxid;
            double Dia_Flux;
            double Flux2Wat;
            double s;
            double COD_0;
            double Flux2Anaerobic;
            double H2S_2;
            double H2S_1;
            double fph2s1;
            double fdh2s1;
            double fph2s2;
            double fdh2s2;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            // ppt

            if (!AQTSeg.Sulfide_System())
            {
                // Goes to Methane Instead
                Dia_Flux = 0;
                Flux2Wat = 0;
                Oxid = 0;
                Burial = 0;
                Flux2Anaerobic = 0;
                DB = 0;
               //  Derivative_WriteRates();
                return;
            }
            Dia_Flux = AQTSeg.Diagenesis(Layer);
            s = AQTSeg.MassTransfer();
            // m/d
            if (Layer == T_SVLayer.SedLayer1)
            {
                // m2/d2
                // m/d
                // g/m3
                // m
                Oxid = k2Oxid() / s * State / DR.H1.Val;
            }
            else
            {
                Oxid = 0;
            }
            fdh2s1 = 1 / (1 + DR.m1.Val * DR.KdH2S1.Val);
            fph2s1 = 1 - fdh2s1;
            fdh2s2 = 1 / (1 + DR.m2.Val * DR.KdH2S2.Val);
            fph2s2 = 1 - fdh2s2;
            COD_0 = AQTSeg.GetState(AllVariables.COD, T_SVType.StV, T_SVLayer.WaterCol);
            H2S_1 = AQTSeg.GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer1);
            H2S_2 = AQTSeg.GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -((DR.W12 * (fph2s2 * H2S_2 - fph2s1 * H2S_1) + DR.KL12 * (fdh2s2 * H2S_2 - fdh2s1 * H2S_1)));
            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }
            // g/m3 d
            // g/m2 d
            // m
            if (Layer == T_SVLayer.SedLayer2)
            {
                Flux2Anaerobic = Flux2Anaerobic + DR.w2.Val / DR.H2.Val * H2S_1;
            }
            // burial from L1
            if (Layer == T_SVLayer.SedLayer2)
            {
                Flux2Wat = 0;
            }
            else
            {
                Flux2Wat = s * (fdh2s1 * State - COD_0) / DR.H1.Val;
             // (mg/L d) (m/d)          (mg/L)   (mg/L)         (m)

            }
            if (Layer == T_SVLayer.SedLayer2)
            {
                Burial = DR.w2.Val / DR.H2.Val * State;
            }
            else
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
             // (g/m3 d)  (m/d)       (m)        (g/m3)

            }
            if (Layer == T_SVLayer.SedLayer1)
            {
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    // Layer 1 is STEADY STATE
                    DB = 0;
                }
                else
                {
                    DB = -Oxid - Burial - Flux2Wat - Flux2Anaerobic;
                }
                // SedLayer1
            }
            else
            {
                DB = Dia_Flux - Burial + Flux2Anaerobic;
            }
            // Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TSulfide_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSulfide_Sediment

    public class TSilica_Sediment : TStateVariable
    {
        // -------------------------------------------------------------------------------
        // TSilica_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Dissolution(double fdsi2)
        {
            double result;
            double PSi;
            double Si2;
            double Temp;
            result = 0;
            if (Layer == T_SVLayer.SedLayer1)
            {
                return result;
            }
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            PSi = AQTSeg.GetState(AllVariables.Avail_Silica, T_SVType.StV, T_SVLayer.SedLayer2);
            // biogenic silica
            Si2 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer2);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            // g/m3 d
            // 1/d
            result = DR.ksi.Val * Math.Pow(DR.ThtaSi.Val, (Temp - 20)) * (PSi / (PSi + DR.KMPSi.Val)) * (DR.SiSat.Val - fdsi2 * Si2);
            // g/m3     // g/m3

            return result;
        }

        // ----------------------------------------------------------------------
        //public void Derivative_WriteAvailRates()
        //{
        //    // biogenic silica, L2
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Deposition", Deposition);
        //        SaveRate("Dissolution", Diss);
        //        SaveRate("Burial", Burial);
        //    }
        //}

        //// ----------------------------------------------------------------------
        //public void Derivative_WriteSilicaRates()
        //{
        //    // non-biogenic
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer1))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Water", Flux2Wat);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //    Setup_Record DR = AQTSeg.SetupRec;
        //    if ((DR.SaveBRates || DR.ShowIntegration) && (Layer == T_SVLayer.SedLayer2))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Dissolution", Diss);
        //        SaveRate("Burial", Burial);
        //        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double s;
            double O2;
            double Diss;
            double Flux2Anaerobic;
            double Flux2Wat;
            double Si_2, Si_1, Si_0;
            double KdSi1, fdsi1, fdsi2, fpsi1, fpsi2;
            double Deposition;
            double Burial;

            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;

            // writesilicarates
            // ----------------------------------------------------------------------
            // TSilica_Sediment.Derivative
            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((O2 > DR.O2critSi.Val))
            {
                KdSi1 = DR.KDSi2.Val * DR.DKDSi1.Val;
            }
            else
            {
                KdSi1 = DR.KDSi2.Val * Math.Pow(DR.DKDSi1.Val, (O2 / DR.O2critSi.Val));
            }
            fdsi1 = (1 / (1 + DR.m1.Val * KdSi1));
            fpsi1 = 1 - fdsi1;
            fdsi2 = (1 / (1 + DR.m2.Val * DR.KDSi2.Val));
            fpsi2 = 1 - fdsi2;
            if (NState == AllVariables.Avail_Silica)
            {
                // Particulate Biogenic Silica
                Diss = Dissolution(fdsi2);
                // mg/L d
                Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
                // mg/L d                // g/m2 d                // m
                Burial = DR.w2.Val / DR.H2.Val * State;
                // m/d                // m                // g/m3
                DB = Deposition - Diss - Burial;
                // g/m3 d
                // Derivative_WriteAvailRates();
            }
            if (NState == AllVariables.Silica)
            {
                // Silica
                Si_0 = 0; // AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.WaterCol);  assumes silica in water column is zero
                Si_1 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer1);
                Si_2 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer2);
                Flux2Anaerobic = -((DR.W12 * (fpsi2 * Si_2 - fpsi1 * Si_1) + DR.KL12 * (fdsi2 * Si_2 - fdsi1 * Si_1)));
                // g/m2 d                 // m/d                // g/m3                // m/d                // g/m3
                if (Layer == T_SVLayer.SedLayer1)
                {
                    Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
                }
                else
                {
                    Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
                    // g/m3 d            // g/m2 d            // m
                }

                s = AQTSeg.MassTransfer();
                // m/d
                if (Layer == T_SVLayer.SedLayer1)
                {
                    Burial = DR.w2.Val / DR.H1.Val * State;
                    // m/d                    // m           // mg/L
                    Flux2Wat = s * (fdsi1 * State - Si_0) / DR.H1.Val;
                    // m/d   // m/d    // mg/L   // mg/L           // m
                    DB = -Burial - Flux2Wat - Flux2Anaerobic;
                    if (AQTSeg.Diagenesis_Steady_State)
                    {
                        DB = 0;
                    }
                    // Layer 1 is STEADY STATE
                }
                else
                {
                    // SedLayer2
                    Flux2Anaerobic = Flux2Anaerobic + DR.w2.Val * Si_1 / DR.H2.Val;
                    // burial from L1
                    Burial = DR.w2.Val * Si_2 / DR.H2.Val;
                    Diss = Dissolution(fdsi2);
                    DB = Diss - Burial + Flux2Anaerobic;
                }
              //   Derivative_WriteSilicaRates();
            }
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TSilica_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSilica_Sediment

    public class TCOD : TStateVariable
    {
        // -------------------------------------------------------------------------------
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TCOD(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
        public override void Derivative(ref double db)
        {
            db = 0;
            //Setup_Record DR = AQTSeg.SetupRec;
            //if ((DR.SaveBRates || DR.ShowIntegration))
            //{
            //    ClearRate();
            //    SaveRate("State", State);
            //}
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            base.CalculateLoad(TimeIndex);
            // TStateVariable
            State = Loading;
            // valuation not loading, no need to adjust for flow and volume

        }


    } // end TCOD

}