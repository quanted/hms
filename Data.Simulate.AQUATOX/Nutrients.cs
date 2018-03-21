using System;
using System.Collections.Generic;
using System.Text;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using Globals;
using System.Runtime.Serialization;

namespace AQUATOX.Nutrients
{

    public class TRemineralize : TStateVariable
    {

        public void Check_Nutrient_IC()
        {
            // if using TN or TP for init cond
            TNO3Obj PNO3;
            TPO4Obj PPO4;
            TNH4Obj PNH4;
            TStateVariable ThisVar;
            // TPlant PPl;
            double Nut2Org;
            double CNutrient;
            AllVariables NutrLoop;
            AllVariables NSLoop;

            PNO3 = (TNO3Obj)AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            PPO4 = (TPO4Obj)AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            for (NutrLoop = AllVariables.Nitrate; NutrLoop <= AllVariables.Phosphate; NutrLoop++)
            {
                if (((NutrLoop == AllVariables.Nitrate) && (PNO3.TN_IC)) || ((NutrLoop == AllVariables.Phosphate) && (PPO4.TP_IC)))
                {
                    if (NutrLoop == AllVariables.Nitrate)
                    {
                        ThisVar = ((PNO3) as TStateVariable);
                    }
                    else
                    {
                        ThisVar = ((PPO4) as TStateVariable);
                    }
                    CNutrient = ThisVar.State;
                    // Total Nutrient in mg/L
                    for (NSLoop = AllVariables.DissRefrDetr; NSLoop <= AllVariables.SuspLabDetr; NSLoop++)
                    {
                        if (AQTSeg.GetState(NSLoop, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                        {
                            if (NutrLoop == AllVariables.Nitrate)
                            {
                                switch (NSLoop)
                                {
                                    case AllVariables.SuspRefrDetr:
                                        Nut2Org = Location.Remin.N2Org_Refr;
                                        break;
                                    case AllVariables.SuspLabDetr:
                                        Nut2Org = Location.Remin.N2OrgLab;
                                        break;
                                    case AllVariables.DissRefrDetr:
                                        Nut2Org = Location.Remin.N2OrgDissRefr;
                                        break;
                                    default:
                                        // DissLabDetr:
                                        Nut2Org = Location.Remin.N2OrgDissLab;
                                        break;
                                }
                                // Case
                            }
                            else
                            {
                                switch (NSLoop)
                                {
                                    case AllVariables.SuspRefrDetr:
                                        Nut2Org = Location.Remin.P2Org_Refr;
                                        break;
                                    case AllVariables.SuspLabDetr:
                                        Nut2Org = Location.Remin.P2OrgLab;
                                        break;
                                    case AllVariables.DissRefrDetr:
                                        Nut2Org = Location.Remin.P2OrgDissRefr;
                                        break;
                                    default:
                                        // DissLabDetr:
                                        Nut2Org = Location.Remin.P2OrgDissLab;
                                        break;
                                }
                            }
                            // Case
                            CNutrient = CNutrient - AQTSeg.GetState(NSLoop, T_SVType.StV, T_SVLayer.WaterCol) * Nut2Org;
                            // mg/L     // mg/L             // mg/L      // N2Org
                        }
                    }
                    //for (NSLoop = Globals.FirstPlant; NSLoop <= Globals.LastPlant; NSLoop++)
                    //{  FIXME LINKAGE TO PLANTS
                    //    PPl = AQTSeg.GetStatePointer(NSLoop, T_SVType.StV, T_SVLayer.WaterCol);
                    //    if (PPl != null)
                    //    {
                    //        if (PPl.IsPhytoplankton())
                    //        {
                    //            if (NutrLoop == AllVariables.Nitrate)
                    //            {
                    //                Nut2Org = PPl.PAlgalRec.N2OrgInit;
                    //            }
                    //            else
                    //            {
                    //                Nut2Org = PPl.PAlgalRec.P2OrgInit;
                    //            }
                    //            CNutrient = CNutrient - PPl.State * Nut2Org;
                    //            // mg/L     // mg/L     // mg/L    // N2Org
                    //        }
                    //    }
                    //}
                    if (CNutrient < 0)
                    {
                        CNutrient = 0;
                    }
                    ThisVar.State = CNutrient;
                    if (NutrLoop == AllVariables.Nitrate)
                    {
                        // No ammonia if nitrogen initial condition is input as Total N
                        PNH4 = (TNH4Obj)AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
                        PNH4.State = 0;
                    }
                }
            }
        }



        public TRemineralize(int Ns, T_SVType SVT, string aName, TStates P, double IC, bool IsTempl)
        {
        }

        // ------------------------------------------------------------------------------------------------
        public double CalculateLoad_DetrNutr2Org(AllVariables ns)
        {
            switch (ns)
            {
                case AllVariables.SedmRefrDetr:
                case AllVariables.SuspRefrDetr:
                    if (NState == AllVariables.Phosphate)
                        return Location.Remin.P2Org_Refr;
                    else
                        return Location.Remin.N2Org_Refr;

                case AllVariables.SedmLabDetr:
                case AllVariables.SuspLabDetr:
                    if (NState == AllVariables.Phosphate)
                        return Location.Remin.P2OrgLab;
                    else
                        return Location.Remin.N2OrgLab;

                case AllVariables.DissRefrDetr:
                    if (NState == AllVariables.Phosphate)
                        return Location.Remin.P2OrgDissRefr;
                    else
                        return Location.Remin.N2OrgDissRefr;

                default:
                    // DissLabDetr
                    if (NState == AllVariables.Phosphate)
                        return Location.Remin.P2OrgDissLab;
                    else
                        return Location.Remin.N2OrgDissLab;

            }
        }

        // Atmospheric and point-source loadings should be to epilimnion. 9/9/98
        const double Infl_NH3_in_DIN = 0.12;
        // Ammonia to Diss. inorg. nitrogen, 12% in inflow water  3/27/08
        const double PS_NH3_in_DIN = 0.15;
        // Ammonia to Diss. inorg. nitrogen, 15% in point-source loadings 3/27/08
        const double NPS_NH3_in_DIN = 0.12;
        // Ammonia to Diss. inorg. nitrogen, 12% in nonpoint-source loadings 3/27/08

        // ------------------------------------------------------------------------------------------------
        public void CalculateLoad_TotNutrient_Alt_Ldg(ref double AddLoad, int LdType)
        {
            double CNutrient;
            //AllVariables nsloop;
            //double DetrAltLdg;
            // TDetritus TDetr;
            int LDType2;
            // DetritalInputRecordType PInputRec;  FIXME ORGANIC MATTER LINKAGE
            // PInputRec = ((AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TDissRefrDetr).InputRecord;

            CNutrient = AddLoad;
            LDType2 = LdType;

            //for (nsloop = AllVariables.DissRefrDetr; nsloop <= AllVariables.SuspLabDetr; nsloop++)
            //{  FIXME ORGANIC MATTER LINKAGE
            //    TDetr = AQTSeg.GetStatePointer(nsloop, T_SVType.StV, T_SVLayer.WaterCol);
            //    if (TDetr != null)
            //    {
            //        DetrAltLdg = Loadings.Consts.Loadings.ReturnAltLoad(TimeIndex, PInputRec.Load, LdType) * TDetr.MultFrac(TimeIndex, true, LDType2);
            //        // g/d                                                                                               // unitless
            //        DetrAltLdg = DetrAltLdg / SegVolume;
            //        // mg/L d      // g/d      // cu m
            //        CNutrient = CNutrient - DetrAltLdg * CalculateLoad_DetrNutr2Org(nsloop);
            //        // mg/L       // mg/L    // mg/L                   // Nut2Org
            //    }
            //}

            if (LdType == 0) // Alt_LoadingsType.PointSource
            {
                if (NState == AllVariables.Ammonia)
                    CNutrient = CNutrient * PS_NH3_in_DIN;
                if (NState == AllVariables.Nitrate)
                    CNutrient = CNutrient * (1 - PS_NH3_in_DIN);
            }
            else // LdType <> 0
            {
                // NPS
                if (NState == AllVariables.Ammonia)
                    CNutrient = CNutrient * NPS_NH3_in_DIN;
                if (NState == AllVariables.Nitrate)
                    CNutrient = CNutrient * (1 - NPS_NH3_in_DIN);
            }

            AddLoad = CNutrient;
            if (CNutrient < 0) AddLoad = 0;
        }

        // ------------------------------------------------------------------------------------------------
        public void CalculateLoad_TotNutrient_Dynamic_Inflow()
        {
            double CNutrient;
            //double PlantInflow;
            //double DetrInflow;
            //TStateVariable PSV;
            //AllVariables nsLoop;
            //double InflLoad;
            CNutrient = Loading;
            // Total Nutrient loading in mg/L d

            //for (nsLoop = AllVariables.DissRefrDetr; nsLoop <= AllVariables.SuspLabDetr; nsLoop++)  // FIXME DETRITUS LINKAGE
            //{
            //    PSV = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol);
            //    if (PSV != null)
            //    {
            //        InflLoad = ((PSV) as TDetritus).GetInflowLoad(TimeIndex);
            //        // Inflow Loadings Only
            //        DetrInflow = InflLoad * Inflow / SegVolume; // Inflow Loadings Only
            //        // mg/L d   // mg/L d  // m3/d     // m3
            //        CNutrient = CNutrient - DetrInflow * CalculateLoad_DetrNutr2Org(nsLoop);
            //        // mg/L d  // mg/L d     // mg/L d                // Nut2Org
            //    }
            //}

            //for (nsLoop = Consts.FirstPlant; nsLoop <= Consts.LastPlant; nsLoop++)  // FIXME PLANT LINKAGE
            //{
            //    PSV = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol);
            //    if (PSV != null)
            //    {
            //        if (((PSV) as TPlant).IsPhytoplankton())
            //        {
            //            InflLoad = ((PSV) as TStateVariable).GetInflowLoad(TimeIndex);
            //            // Inflow Loadings Only
            //            PlantInflow = InflLoad * Inflow / SegVolume;
            //            // Inflow Loadings Only
            //            if (NState == AllVariables.Phosphate)
            //            {
            //                CNutrient = CNutrient - PlantInflow * ((PSV) as TPlant).P_2_Org();
            //            }
            //            else
            //            {
            //                CNutrient = CNutrient - PlantInflow * ((PSV) as TPlant).N_2_Org();
            //            }
            //            // mg/L d
            //            // mg/L d
            //            // mg/L d
            //            // N2Org
            //        }
            //    }
            //}

            if (CNutrient < 0) CNutrient = 0;

            if (NState == AllVariables.Ammonia)
                CNutrient = CNutrient * Infl_NH3_in_DIN;

            if (NState == AllVariables.Nitrate)
                CNutrient = CNutrient * (1 - Infl_NH3_in_DIN);

            Loading = CNutrient;
        }

        // -------------------------------------------------------------------------------------------------------
        public override void CalculateLoad(DateTime TimeIndex)
        {
            // Inflow should be split betweeen both segments weighted by volume: 12-8-99
            int Loop;  // alt loadings
            double SegVolume;
            double Inflow;
            double AddLoad;
            TNO3Obj PNO3;
            // ------------------------------------------------------------------------------------------------
            PNO3 = (TNO3Obj)AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            Loading = 0;
            SegVolume = AQTSeg.SegVol();
            Inflow = Location.Morph.InflowH2O * (Location.Morph.OOSInflowFrac);

            if (LoadsRec.Loadings.NoUserLoad)
            {
                return;
            }

            // Inflow Loadings
            Loading = GetInflowLoad(TimeIndex);
            Loading = Loading * Inflow / SegVolume;
            // unit/d   unit     cu m/d    cu m
            if ((NState == AllVariables.Ammonia))
            {
                if (PNO3.TN_Inflow)
                {
                    // 3/27/08  Loadings must be gathered from NO3 input data structure
                    Loading = PNO3.GetInflowLoad(TimeIndex);
                    Loading = Loading * Inflow / SegVolume;
                    // unit/d   unit     cu m/d    cu m
                    CalculateLoad_TotNutrient_Dynamic_Inflow();
                }
            }
            if ((NState == AllVariables.Nitrate))
            {
                if (PNO3.TN_Inflow)
                {
                    CalculateLoad_TotNutrient_Dynamic_Inflow();
                }
            }
            if ((NState == AllVariables.Phosphate))
            {
                if (((this) as TPO4Obj).TP_Inflow)
                {
                    CalculateLoad_TotNutrient_Dynamic_Inflow();
                }
                Loading = Loading * ((this) as TPO4Obj).FracAvail;
            }
            // Add Point Source Non-Point Source and Direct Precipitation Loadings

            //            if (AQTSeg.VSeg == VerticalSegments.Epilimnion)
            {
                if ((!(LoadsRec.Alt_Loadings[0] == null)))  // 0 = point source
                {
                    for (Loop = 0; Loop <= 2; Loop++)
                    {
                        if ((Loop != 1) || (!(NState >= AllVariables.DissRefrDetr && NState <= AllVariables.SuspLabDetr))) // DirectPrecip Irrelevant for Susp&Dissolved Detritus
                        {

                            AddLoad = 0;
                            AddLoad = LoadsRec.ReturnAltLoad(TimeIndex, Loop);
                            // g/d or g/sq m. d

                            AddLoad = AddLoad * LoadsRec.Alt_Loadings[Loop].MultLdg / SegVolume;
                            // mg/L d    // g/d         // unitless                   // cu m      // note if direct precip result is mg/(sq m.*L*d)

                            if (Loop == 1) AddLoad = AddLoad * Location.Locale.SurfArea;  // Loop = 1 is DirectPrecip loadings type
                                                                                          // mg/L d // mg/(sq m.*L*d)            // sq m.

                            if (NState == AllVariables.Phosphate)
                            {
                                TPO4Obj _TP = ((this) as TPO4Obj);
                                if (((Loop == 0) && _TP.TP_PS) || ((Loop == 2) && _TP.TP_NPS))
                                {
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop);
                                }
                            }
                            if (NState == AllVariables.Nitrate)
                            {
                                if (((Loop == 0) && PNO3.TN_PS) || ((Loop == 2) && PNO3.TN_NPS))
                                {
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop);
                                }
                            }
                            if (NState == AllVariables.Ammonia)
                            {
                                if (((Loop == 0) && PNO3.TN_PS) || ((Loop == 2) && PNO3.TN_NPS))
                                {  // 3/27/08
                                    AddLoad = PNO3.LoadsRec.ReturnAltLoad(TimeIndex, Loop);
                                    AddLoad = AddLoad / SegVolume;
                                    // mg/L d   // g/d    // cu m
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop);
                                }
                            }
                            if (NState == AllVariables.Phosphate)
                            {
                                AddLoad = AddLoad * ((this) as TPO4Obj).Alt_FracAvail[Loop];
                            }

                            Loading = Loading + AddLoad;
                            // mg/L d           // mg/L d
                        }
                    }
                }
            }
            // Loop
            // With LoadsRec

        }

        // ---------------------------process equations--------------------------
        // --------------------------------
        // correction for non-optimal pH
        // --------------------------------
        public double pHCorr(double pHMin, double pHMax)
        {
            const double KpH = 1.0;
            double ppH = AQTSeg.GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            double result = 1.0;
            if (ppH <= pHMin) result = KpH * Math.Exp(ppH - pHMin);
            if (ppH > pHMax) result = KpH * Math.Exp(pHMax - ppH);
            return result;
        }

        // phcorr
        // -------------------------------------------------------------------------------------------------------
        public double SumDetrDecomp(T_SVType OType, bool SedOnly)
        {
            return 0; // fixme detrital linkage
            //double result;
            //// If OType=NTrack returns sum of detrital decomp into ammonia
            //// If OType=NTrack returns sum detrital decomp into dissolved phosphorus
            //// otherwise returns sum of detrital decomposition in terms of organic matter
            //// Sum of Labile Detritus Decomposition
            //TRemineralize RP;
            //AllVariables EndLoop;
            //AllVariables Loop;
            //double SumDecomp;
            //double FracAerobic;
            //double Decomp;
            //SumDecomp = 0;
            //if (SedOnly)
            //{
            //    EndLoop = AllVariables.SedmLabDetr;
            //}
            //else
            //{
            //    EndLoop = Consts.LastDetr;
            //}
            //return 0; //FIXME DETR DECOMPOSITION LINKAGE

            //for (Loop = Consts.FirstDetr; Loop <= EndLoop; Loop++)
            //{
            //    ReminRecord _wvar1 = Location.Remin;
            //    RP = (TRemineralize)AQTSeg.GetStatePointer(Loop, T_SVType.StV, T_SVLayer.WaterCol);
            //    if ((RP == null))
            //    {
            //        Decomp = 0;
            //    }
            //    else
            //    {
            //        Decomp = RP.Decomposition(_wvar1.DecayMax_Lab, Units.KAnaerobic, ref FracAerobic);
            //    }
            //    if (OType == T_SVType.NTrack)
            //    {
            //        if (Loop == AllVariables.DissLabDetr)
            //        {
            //            Decomp = Decomp * _wvar1.N2OrgDissLab;
            //        }
            //        else
            //        {
            //            Decomp = Decomp * _wvar1.N2OrgLab;
            //        }
            //    }
            //    if (OType == T_SVType.PTrack)
            //    {
            //        if (Loop == AllVariables.DissLabDetr)
            //        {
            //            Decomp = Decomp * _wvar1.P2OrgDissLab;
            //        }
            //        else
            //        {
            //            Decomp = Decomp * _wvar1.P2OrgLab;
            //        }
            //    }
            //    SumDecomp = SumDecomp + Decomp;
            //}
            //result = SumDecomp;
            //return result;
        }

        //public void CalcAnimPredn_NutrPred(TAnimal P)
        //{
        // FIXME LINKAGE TO ANIMALS

        //double Cons;
        //double PreyNutr2Org;
        //double PredNutr2Org;
        //double DiffNutrFrac;
        //if (P.IsAnimal())
        //{
        //    if (NState == AllVariables.Phosphate)
        //    {
        //        PredNutr2Org = P.PAnimalData.P2Org;
        //    }
        //    else
        //    {
        //        PredNutr2Org = P.PAnimalData.N2Org;
        //    }
        //    PreyNutr2Org = 0;
        //    Cons = P.EatEgest(Loadings.EatOrEgest.Eat);
        //    if (Cons > 0)
        //    {
        //        if (NState == AllVariables.Phosphate)
        //        {
        //            PreyNutr2Org = P.PhosCons / Cons;
        //        }
        //        else
        //        {
        //            PreyNutr2Org = P.NitrCons / Cons;
        //        }
        //    }
        //    DiffNutrFrac = PredNutr2Org - PreyNutr2Org;
        //    NetPredation = NetPredation - Cons * DiffNutrFrac;
        //}
        //}

        // -------------------------
        // sum instantaneous
        // contributions from
        // each process to Detr
        // -------------------------
        //public double CalcAnimPredn()  //FIXME Animal Linkage
        //{
        //    double result;
        //    double NetPredation;
        //    int i;
        //    // excr
        //    NetPredation = 0.0;
        //    for (i = 0; i < AQTSeg.Count; i++)
        //    {
        //        CalcAnimPredn_NutrPred(AQTSeg.At(i));
        //    }
        //    result = NetPredation;
        //    return result;
        //}

        // Remineralization
        //public double NutrRelPeriScr()  //FIXME Plant Linkage
        //{
        //    double result;
        //    // When Periphyton is scoured into phytoplankton nutrient balance must
        //    // be maintained if they have different stochiometry
        //    AllVariables PeriLoop;
        //    AllVariables PhytLoop;
        //    TPlant PPeri;
        //    TPlant PPhyt;
        //    double NRPS;
        //    double j;
        //    double Nut2OrgPeri;
        //    double Nut2OrgPhyt;
        //    NRPS = 0;
        //    for (PhytLoop = Units.FirstAlgae; PhytLoop <= Units.LastAlgae; PhytLoop++)
        //    {
        //        for (PeriLoop = Units.FirstAlgae; PeriLoop <= Units.LastAlgae; PeriLoop++)
        //        {
        //            PPhyt = AQTSeg.GetStatePointer(PhytLoop, T_SVType.StV, T_SVLayer.WaterCol);
        //            PPeri = AQTSeg.GetStatePointer(PeriLoop, T_SVType.StV, T_SVLayer.WaterCol);
        //            if ((PPhyt != null) && (PPeri != null))
        //            {
        //                if ((PPeri.PSameSpecies == PPhyt.NState))
        //                {
        //                    PPeri.CalcSlough();
        //                    // update sloughevent
        //                    if (PPeri.SloughEvent)
        //                    {
        //                        if ((NState == AllVariables.Ammonia))
        //                        {
        //                            Nut2OrgPeri = PPeri.N_2_Org();
        //                        }
        //                        else
        //                        {
        //                            Nut2OrgPeri = PPeri.P_2_Org();
        //                        }
        //                        if ((NState == AllVariables.Ammonia))
        //                        {
        //                            Nut2OrgPhyt = PPhyt.N_2_Org();
        //                        }
        //                        else
        //                        {
        //                            Nut2OrgPhyt = PPhyt.P_2_Org();
        //                        }
        //                        j = -999;
        //                        // signal to not write mass balance tracking
        //                        PPeri.Derivative(j);
        //                        // update sloughing
        //                        NRPS = NRPS + PPeri.Sloughing * (1 / 3) * (Nut2OrgPeri - Nut2OrgPhyt);
        //                        // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    result = NRPS;
        //    return result;
        //}

        // -------------------------------------------------------------------------------------------------------
        //public double NutrRelGamLoss()  // FIXME Animal Linkage
        //{
        //    double result;
        //    // When Gameteloss takes place in animals, (-->SuspLabDetr)
        //    // excess nutrients are converted into NH4.
        //    AllVariables ns;
        //    TAnimal PAn;
        //    double DiffNFrac;
        //    double NGL;
        //    NGL = 0;
        //    for (ns = Units.FirstAnimal; ns <= Units.LastAnimal; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            ReminRecord _wvar1 = Location.Remin;
        //            if (NState == AllVariables.Ammonia)
        //            {
        //                DiffNFrac = PAn.PAnimalData.N2Org - _wvar1.N2OrgLab;
        //            }
        //            else
        //            {
        //                DiffNFrac = PAn.PAnimalData.P2Org - _wvar1.P2OrgLab;
        //            }
        //            NGL = NGL + PAn.GameteLoss() * DiffNFrac;
        //        }
        //    }
        //    result = NGL;
        //    return result;
        //}

        // -------------------------------------------------------------------------------------------------------
        //public double NutrRelMortality()  // FIXME animal and plant linkage
        //{
        //    double result;
        //    // When Anim & Plant die, excess nutrients are converted into NH4;
        //    // Detritus tends to have a lower fraction of nutrients then the dying organisms.
        //    // Macrophyte breakage and Tox Dislodge are included in mortality here
        //    // for accounting of nutrient mass.
        //    AllVariables ns;
        //    TDetritus PDRD;
        //    TDetritus PDLD;
        //    TDetritus PPRD;
        //    TDetritus PPLD;
        //    TOrganism POr;
        //    double Mort;
        //    double DetrNFrac;
        //    double DiffNFrac;
        //    double NMort;
        //    double j;
        //    double FracMult;
        //    double Nut2Org_Refr;
        //    double Nut2Org_Lab;
        //    double Nut2Org_DissRefr;
        //    double Nut2Org_DissLab;
        //    PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PPRD = AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PPLD = AQTSeg.GetStatePointer(AllVariables.SuspLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_Refr = _wvar1.N2Org_Refr;
        //        Nut2Org_Lab = _wvar1.N2OrgLab;
        //        Nut2Org_DissRefr = _wvar1.N2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.N2OrgDissLab;
        //    }
        //    else
        //    {
        //        Nut2Org_Refr = _wvar1.P2Org_Refr;
        //        Nut2Org_Lab = _wvar1.P2OrgLab;
        //        Nut2Org_DissRefr = _wvar1.P2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.P2OrgDissLab;
        //    }
        //    NMort = 0;
        //    for (ns = Units.FirstBiota; ns <= Units.LastBiota; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            POr = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            DetrNFrac = PDRD.Mort_To_Detr(ns) * Nut2Org_DissRefr + PPRD.Mort_To_Detr(ns) * Nut2Org_Refr + PPLD.Mort_To_Detr(ns) * Nut2Org_Lab + PDLD.Mort_To_Detr(ns) * Nut2Org_DissLab;
        //            DiffNFrac = POr.NutrToOrg(NState) - DetrNFrac;
        //            Mort = POr.Mortality();
        //            if (POr.IsMacrophyte())
        //            {
        //                Mort = Mort + ((POr) as TMacrophyte).Breakage();
        //            }
        //            if (POr.IsPlant() && (!POr.IsMacrophyte()))
        //            {
        //                TPlant _wvar2 = ((POr) as TPlant);
        //                Mort = Mort + _wvar2.ToxicDislodge();
        //                ((POr) as TPlant).CalcSlough();
        //                // update sloughevent
        //                if (_wvar2.SloughEvent)
        //                {
        //                    j = -999;
        //                    // signal to not write mass balance tracking
        //                    ((POr) as TPlant).Derivative(j);
        //                    // update sloughing
        //                    if (_wvar2.PSameSpecies == AllVariables.NullStateVar)
        //                    {
        //                        FracMult = 1.0;
        //                    }
        //                    else
        //                    {
        //                        FracMult = 2 / 3;
        //                    }
        //                    // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.
        //                    Mort = Mort + _wvar2.Sloughing * FracMult;
        //                }
        //            }
        //            NMort = NMort + Mort * DiffNFrac;
        //        }
        //    }
        //    result = NMort;
        //    return result;
        //}

        // -------------------------------------------------------------------------------------------------------
        //public double NutrRelPlantSink()  // FIXME Plant Linkage
        //{
        //    double result;
        //    // When Plants sink, excess nutrients are converted into NH4;
        //    // Sedimented Detritus tends to have a lower fraction of nutrients then
        //    // the sinking plants
        //    AllVariables ns;
        //    AllVariables ploop;
        //    TDetritus PSRD;
        //    TDetritus PSLD;
        //    TPlant PPl;
        //    TPlant PPeri;
        //    double DetrNFrac;
        //    double DiffNFrac;
        //    double NSink;
        //    double Nut2Org_Refr;
        //    double Nut2Org_Lab;
        //    double PeriMass;
        //    double NumPeriLinks;
        //    double PeriNFrac;
        //    double PNFrac2;
        //    result = 0;
        //    PSRD = AQTSeg.GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PSLD = AQTSeg.GetStatePointer(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    if (PSRD == null)
        //    {
        //        return result;
        //    }
        //    // diagenesis model in place
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_Refr = _wvar1.N2Org_Refr;
        //        Nut2Org_Lab = _wvar1.N2OrgLab;
        //    }
        //    else
        //    {
        //        Nut2Org_Refr = _wvar1.P2Org_Refr;
        //        Nut2Org_Lab = _wvar1.P2OrgLab;
        //    }
        //    NSink = 0;
        //    for (ns = Units.FirstAlgae; ns <= Units.LastAlgae; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            if (PPl.IsLinkedPhyto())
        //            {
        //                PeriNFrac = 0;
        //                PNFrac2 = 0;
        //                NumPeriLinks = 0;
        //                PeriMass = 0;
        //                for (ploop = Units.FirstAlgae; ploop <= Units.LastAlgae; ploop++)
        //                {
        //                    PPeri = AQTSeg.GetStatePointer(ploop, T_SVType.StV, T_SVLayer.WaterCol);
        //                    if (PPeri != null)
        //                    {
        //                        if ((PPeri.IsPeriphyton()) && (PPeri.PSameSpecies == PPl.NState))
        //                        {
        //                            NumPeriLinks = NumPeriLinks + 1.0;
        //                            PeriMass = PeriMass + PPeri.State;
        //                            if (NState == AllVariables.Ammonia)
        //                            {
        //                                PeriNFrac = PeriNFrac + PPeri.State * PPeri.N_2_Org();
        //                            }
        //                            else
        //                            {
        //                                PeriNFrac = PeriNFrac + PPeri.State * PPeri.P_2_Org();
        //                            }
        //                            if (PeriMass < Units.VSmall)
        //                            {
        //                                if (NState == AllVariables.Ammonia)
        //                                {
        //                                    PNFrac2 = PNFrac2 + PPeri.N_2_Org();
        //                                }
        //                                else
        //                                {
        //                                    PNFrac2 = PNFrac2 + PPeri.P_2_Org();
        //                                }
        //                            }
        //                        }
        //                    }
        //                    // will count itself and any other peiphyton species linked to this phytoplankton
        //                }
        //                // 9/20/2004 debug against zero periphyton
        //                if (PeriMass < Units.VSmall)
        //                {
        //                    // used to split evenly among peri comps.
        //                    PeriNFrac = PNFrac2 / NumPeriLinks;
        //                }
        //                else
        //                {
        //                    PeriNFrac = PeriNFrac / PeriMass;
        //                }
        //                // used to weight by mass of periphyton comps.
        //                if (NState == AllVariables.Ammonia)
        //                {
        //                    DiffNFrac = PPl.N_2_Org() - PeriNFrac;
        //                }
        //                else
        //                {
        //                    DiffNFrac = PPl.P_2_Org() - PeriNFrac;
        //                }
        //                NSink = NSink + PPl.Sedimentation() * DiffNFrac;
        //            }
        //            else
        //            {
        //                // not linked phyto
        //                DetrNFrac = PSRD.PlantSink_To_Detr(ns) * Nut2Org_Refr + PSLD.PlantSink_To_Detr(ns) * Nut2Org_Lab;
        //                if (NState == AllVariables.Ammonia)
        //                {
        //                    DiffNFrac = PPl.N_2_Org() - DetrNFrac;
        //                }
        //                else
        //                {
        //                    DiffNFrac = PPl.P_2_Org() - DetrNFrac;
        //                }
        //                NSink = NSink + PPl.Sedimentation() * DiffNFrac;
        //            }
        //        }
        //    }
        //    result = NSink;
        //    return result;
        //}
        // -------------------------------------------------------------------------------------------------------
        //public double NutrRelColonization()   FIXME Organic Matter Linkage
        //{
        //    double result;
        //    // When organic matter is colonized from Refr--> Labile, nutrients
        //    // must be accounted for in case the nutrient to organic ratio differs between
        //    // the refractory and labile compartments
        //    double Nut2Org_Refr;
        //    double Nut2Org_Lab;
        //    double Nut2Org_DissRefr;
        //    double SumColonize;
        //    AllVariables ns;
        //    TDetritus PD;
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_Refr = _wvar1.N2Org_Refr;
        //        Nut2Org_Lab = _wvar1.N2OrgLab;
        //        Nut2Org_DissRefr = _wvar1.N2OrgDissRefr;
        //    }
        //    else
        //    {
        //        Nut2Org_Refr = _wvar1.P2Org_Refr;
        //        Nut2Org_Lab = _wvar1.P2OrgLab;
        //        Nut2Org_DissRefr = _wvar1.P2OrgDissRefr;
        //    }
        //    SumColonize = 0;
        //    for (ns = AllVariables.SedmRefrDetr; ns <= AllVariables.SuspLabDetr; ns++)
        //    {
        //        if (new ArrayList(new object[] { AllVariables.SedmRefrDetr, AllVariables.SuspRefrDetr, AllVariables.DissRefrDetr }).Contains(ns))
        //        {
        //            PD = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            if (PD != null)
        //            {
        //                if (ns == AllVariables.DissRefrDetr)
        //                {
        //                    SumColonize = SumColonize + PD.Colonization() * (Nut2Org_DissRefr - Nut2Org_Lab);
        //                }
        //                else
        //                {
        //                    SumColonize = SumColonize + PD.Colonization() * (Nut2Org_Refr - Nut2Org_Lab);
        //                }
        //            }
        //        }
        //    }
        //    result = SumColonize;
        //    return result;
        //}

        //public double NutrRelDefecation()  // FIXME ANIMAL LINKAGE
        //{
        //    double result;
        //    // When Defecation takes place in animals, excess nutrients are converted
        //    // into NH4.  Sedimented Detritus tends to have a lower fraction of nutrients then
        //    // the animals which are defecating organic matter.
        //    AllVariables ns;
        //    TAnimal PAn;
        //    double DetrNFrac;
        //    double DiffNFrac;
        //    double NDef;
        //    double Nut2Org_Refr;
        //    double Nut2Org_Lab;
        //    // NutrRelDefecation := 0;
        //    // If AQTSeg.Diagenesis_Included then exit;  6/6/08, procedure now relevant to diagenesis model
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_Refr = _wvar1.N2Org_Refr;
        //        Nut2Org_Lab = _wvar1.N2OrgLab;
        //    }
        //    else
        //    {
        //        Nut2Org_Refr = _wvar1.P2Org_Refr;
        //        Nut2Org_Lab = _wvar1.P2OrgLab;
        //    }
        //    NDef = 0;
        //    for (ns = Units.FirstAnimal; ns <= Units.LastAnimal; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            DetrNFrac = (1 - Units.Def2SedLabDetr) * Nut2Org_Refr + Units.Def2SedLabDetr * Nut2Org_Lab;
        //            if (AQTSeg.Diagenesis_Included())
        //            {
        //                DetrNFrac = Nut2Org_Lab;
        //            }
        //            // 6/6/2008, diagenesis defecation has same nutrients as labile detritus
        //            if (NState == AllVariables.Ammonia)
        //            {
        //                DiffNFrac = PAn.PAnimalData.N2Org - DetrNFrac;
        //            }
        //            else
        //            {
        //                DiffNFrac = PAn.PAnimalData.P2Org - DetrNFrac;
        //            }
        //            NDef = NDef + PAn.Defecation() * DiffNFrac;
        //        }
        //    }
        //    result = NDef;
        //    return result;
        //}

        // -------------------------------------------------------------------------------------------------------
        //public double CalcAnimResp_Excr()  // FIXME ANIMAL Linkage
        //{
        //    double result;
        //    // When Excretion (5/13/2013 and respiration) takes place in animals, _excess_ nutrients are converted
        //    // into NH4.  Dissolved Detritus tends to have a lower fraction of nutrients then
        //    // the animals which are excreting organic matter.
        //    AllVariables ns;
        //    TDetritus PDRD;
        //    TDetritus PDLD;
        //    TAnimal PAn;
        //    double DetrNFrac;
        //    double DiffNFrac;
        //    double Excret;
        //    double Nut2Org_DissRefr;
        //    double Nut2Org_DissLab;
        //    PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_DissRefr = _wvar1.N2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.N2OrgDissLab;
        //    }
        //    else
        //    {
        //        Nut2Org_DissRefr = _wvar1.P2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.P2OrgDissLab;
        //    }
        //    Excret = 0;
        //    for (ns = Units.FirstAnimal; ns <= Units.LastAnimal; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            DetrNFrac = PDRD.Excr_To_Diss_Detr(ns) * Nut2Org_DissRefr + PDLD.Excr_To_Diss_Detr(ns) * Nut2Org_DissLab;
        //            if (NState == AllVariables.Ammonia)
        //            {
        //                DiffNFrac = PAn.PAnimalData.N2Org - DetrNFrac;
        //            }
        //            else
        //            {
        //                DiffNFrac = PAn.PAnimalData.P2Org - DetrNFrac;
        //            }
        //            // was AnimExcretion
        //            Excret = Excret + PAn.Respiration() * DiffNFrac;
        //            // 5/13/2013
        //        }
        //    }
        //    result = Excret;
        //    return result;
        //}

        // 5/13/2013
        // Function TRemineralize.CalcAnimResp: Double;
        // Var ns: AllVariables;
        // PAn: TAnimal;
        // Resp: Double;
        // Nutr2Org: Double;
        // Begin
        // Resp := 0;
        // For ns := FirstAnimal to LastAnimal do
        // If AQTSeg.GetState(ns,StV,WaterCol)>0 then
        // Begin
        // PAn := AQTSeg.GetStatePointer(ns,StV,WaterCol);
        // If NState=Ammonia
        // then Nutr2Org := PAn.PAnimalData^.N2Org
        // else Nutr2Org := PAn.PAnimalData^.P2Org ;
        // Resp := Resp +  PAn.Respiration * Nutr2Org;
        // End;
        // CalcAnimResp := Resp;
        // End;
        // -------------------------------------------------------------------------------------------------------
        //public double CalcPhotoResp()  // FIXME Plant Linkage
        //{
        //    double result;
        //    // When photorespiration takes place in plants, excess nutrients are converted
        //    // into NH4.  Dissolved Detritus tends to have a lower fraction of nutrients then
        //    // the plants which are going through photorespiration
        //    AllVariables ns;
        //    TDetritus PDRD;
        //    TDetritus PDLD;
        //    TPlant PPl;
        //    double DetrNFrac;
        //    double DiffNFrac;
        //    double PhotoRsp;
        //    double Nut2Org_DissRefr;
        //    double Nut2Org_DissLab;
        //    PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
        //    ReminRecord _wvar1 = Location.Remin;
        //    if (NState == AllVariables.Ammonia)
        //    {
        //        Nut2Org_DissRefr = _wvar1.N2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.N2OrgDissLab;
        //    }
        //    else
        //    {
        //        Nut2Org_DissRefr = _wvar1.P2OrgDissRefr;
        //        Nut2Org_DissLab = _wvar1.P2OrgDissLab;
        //    }
        //    PhotoRsp = 0;
        //    for (ns = Units.FirstPlant; ns <= Units.LastPlant; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            DetrNFrac = PDRD.Excr_To_Diss_Detr(ns) * Nut2Org_DissRefr + PDLD.Excr_To_Diss_Detr(ns) * Nut2Org_DissLab;
        //            if (NState == AllVariables.Ammonia)
        //            {
        //                DiffNFrac = PPl.N_2_Org() - DetrNFrac;
        //            }
        //            else
        //            {
        //                DiffNFrac = PPl.P_2_Org() - DetrNFrac;
        //            }
        //            PhotoRsp = PhotoRsp + PPl.PhotoResp() * DiffNFrac;
        //        }
        //    }
        //    result = PhotoRsp;
        //    return result;
        //}

        // -------------------------------------------------------------------------------------------------------
        //public double CalcDarkResp() // FIXME Plant Linkate
        //{
        //    double result;
        //    // When dark respiration occurs, excess nutrients are converted into NH4.
        //    AllVariables ns;
        //    TPlant PPl;
        //    double Resp;
        //    double Nutr2Org;
        //    Resp = 0;
        //    for (ns = Units.FirstPlant; ns <= Units.LastPlant; ns++)
        //    {
        //        if (AQTSeg.GetState(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
        //        {
        //            PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
        //            if (NState == AllVariables.Ammonia)
        //            {
        //                Nutr2Org = PPl.N_2_Org();
        //            }
        //            else
        //            {
        //                Nutr2Org = PPl.P_2_Org();
        //            }
        //            Resp = Resp + PPl.Respiration() * Nutr2Org;
        //        }
        //    }
        //    result = Resp;
        //    return result;
        //}

        //    public void Assimilation_AddAssim(TStateVariable P)   // Assimilation of nutrients by algae    FIXME Plant Linkage
        //    {
        //        TPlant PP;
        //        double UptkNut;
        //        const double UptakeCO2 = 0.53;
        //        UptkNut = 0;
        //        if ((P.IsPlant()))
        //        {
        //            PP = ((P) as TPlant);
        //            if ((new ArrayList(new object[] { AllVariables.Ammonia, AllVariables.Nitrate }).Contains(NState)) && (PP.IsFixingN()))
        //            {
        //                return;
        //            }
        //            // N-fixation, not assimilation from the water column
        //            if ((NState == AllVariables.CO2) && PP.Is_Pcp_CaCO3())
        //            {
        //                return;
        //            }
        //            // 10-26-2007 Because plants are deriving C from the bicarbonate reaction,
        //            // their photosynthesis does not result in a loss of CO2.
        //            PlantRecord _wvar1 = PP.PAlgalRec;
        //            // JSC 9-25-2002, bryophytes assimilate nutrients
        //            // JSC 10-21-2007, Free-floating macro. assimilate nutrients
        //            if ((P.IsAlgae()) || (_wvar1.PlantType == "Bryophytes") || ((_wvar1.PlantType == "Macrophytes") && (PP.MacroType == TMacroType.Freefloat)))
        //            {
        //                PlantRecord _wvar2 = PP.PAlgalRec;
        //                if (NState != AllVariables.CO2)
        //                {
        //                    if (new ArrayList(new object[] { AllVariables.Ammonia, AllVariables.Nitrate }).Contains(NState))
        //                    {
        //                        TNIP = AQTSeg.GetStatePointer(PP.NState, T_SVType.NIntrnl, T_SVLayer.WaterCol);
        //                    }
        //                    else
        //                    {
        //                        TNIP = AQTSeg.GetStatePointer(PP.NState, T_SVType.PIntrnl, T_SVLayer.WaterCol);
        //                    }
        //                    if (AQTSeg.SetupRec.Internal_Nutrients && (TNIP != null))
        //                    {
        //                        // mg/L
        //                        // ug/L
        //                        // mg/ug
        //                        UptkNut = TNIP.Uptake() * 1e-3;
        //                    }
        //                    else
        //                    {
        //                        // external nutrients
        //                        if (new ArrayList(new object[] { AllVariables.Ammonia, AllVariables.Nitrate }).Contains(NState))
        //                        {
        //                            UptkNut = PP.N_2_Org() * PP.Photosynthesis();
        //                        }
        //                        else
        //                        {
        //                            UptkNut = PP.P_2_Org() * PP.Photosynthesis();
        //                        }
        //                        // mg/L
        //                        // g/g
        //                        // mg/L
        //                    }
        //                    if (new ArrayList(new object[] { AllVariables.Ammonia, AllVariables.Nitrate }).Contains(NState))
        //                    {
        //                        if (((_wvar2.KN + SVA) * (_wvar2.KN + SVN)) != 0)
        //                        {
        //                            NH4Pref = SVA * SVN / ((_wvar2.KN + SVA) * (_wvar2.KN + SVN)) + SVA * _wvar2.KN / ((SVA + SVN) * (_wvar2.KN + SVN));
        //                        }
        //                        else
        //                        {
        //                            NH4Pref = 0;
        //                        }
        //                        // Protect Against Div by 0
        //                    }
        //                }
        //                switch (NState)
        //                {
        //                    case AllVariables.Ammonia:
        //                        // non CO2 code
        //                        Assim = Assim + UptkNut * NH4Pref;
        //                        break;
        //                    case AllVariables.Nitrate:
        //                        // total nutr assimilated = Sum(photosyn * uptake * proportion)
        //                        // g/cu m-d                     g/cu m-d  ~Redfield ratio unitless
        //                        Assim = Assim + UptkNut * (1.0 - NH4Pref);
        //                        break;
        //                    case AllVariables.Phosphate:
        //                        Assim = Assim + UptkNut;
        //                        break;
        //                    case AllVariables.CO2:
        //                        // mg/L
        //                        Assim = Assim + PP.Photosynthesis() * UptakeCO2;
        //                        break;
        //                }
        //                // case
        //                // mg/L
        //                // mg/L
        //                // g/g
        //            }
        //            // with PP PAlgalRec
        //        }
        //        // if is plant

        //    }

        //    // -------------------------------------------------------------------------------------------------------
        //    // -------------------------------------
        //    // assimilation of nutrient by algae
        //    // incl. ammonia preference factor of
        //    // Thomann and Fitzpatrick, 1982
        //    // used in WASP (Ambrose et al., 1991)
        //    // -------------------------------------
        //    public double Assimilation()
        //    {
        //        double result;
        //        double Assim;
        //        double SVN;
        //        double SVA;
        //        double NH4Pref;
        //        T_N_Internal_Plant TNIP;
        //        const double N2NO3 = 0.23;
        //        const double N2NH4 = 0.78;
        //        // Add Assim
        //        int i;
        //        Assim = 0.0;
        //        SVN = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol) * N2NO3;
        //        SVA = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) * N2NH4;
        //        if ((SVA > 0) || (SVN > 0))
        //        {
        //            // prevent Div by 0
        //            TStates _wvar3 = AQTSeg;
        //            for (i = 0; i < _wvar3.Count; i++)
        //            {
        //                Assimilation_AddAssim(_wvar3.At(i));
        //            }
        //        }
        //        result = Assim;
        //        return result;
        //    }

        } // end TRemineralize

        public class TPO4Obj : TRemineralize
        {
            // Function AtmosDeposition : double ;
            public double FracAvail = 0;
            public double[] Alt_FracAvail;
            public bool TP_IC = false;
            public bool TP_Inflow = false;
            public bool TP_PS = false;
            public bool TP_NPS = false;

            // Remineralization
            public double Remineralization()
            {
                double result;
                double Remin;
                Remin = 0;
                Remin = Remin + SumDetrDecomp(T_SVType.PTrack, false);  // FIXME DETRITAL LINKAGE
                // mg P/ L       // mg P/ L

                //Remin = Remin + CalcPhotoResp();   // FIXME PLANT LINKAGE
                //Remin = Remin + CalcDarkResp();     // FIXME PLANT LINKAGE
                //Remin = Remin + CalcAnimResp_Excr();    // FIXME ANIMAL, PLANT LINKAGE
                //Remin = Remin + CalcAnimPredn();        // FIXME ANIMAL, PLANT LINKAGE
                //Remin = Remin + NutrRelDefecation();    // FIXME ANIMAL, PLANT LINKAGE
                //Remin = Remin + NutrRelColonization();  // FIXME DETRITAL LINKAGE
                //Remin = Remin + NutrRelPlantSink();  // FIXME PLANT LINKAGE
                //Remin = Remin + NutrRelMortality();  // FIXME ANIMAL, PLANT LINKAGE
                //Remin = Remin + NutrRelGamLoss();    // FIXME ANIMAL LINKAGE
                //Remin = Remin + NutrRelPeriScr();    // FIXME PLANT LINKAGE
                result = Remin;
                return result;
            }

            //Constructor  Init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
            public TPO4Obj(int Ns, T_SVType SVT, string aName, TStates P, double IC, bool IsTempl) : base(Ns, SVT, aName, P, IC, IsTempl)
            {
                int ALLoop;
                // TRemineralize
                FracAvail = 1;
                for (ALLoop = 0; ALLoop <= 2; ALLoop++)
                    Alt_FracAvail[ALLoop] = 1;

            }
            // --------------------------------------------------
            public void Derivative_WriteRates()
            {
                // PO4Obj
                //Setup_Record _wvar1 = AQTSeg.SetupRec;
                //if ((_wvar1.SaveBRates || _wvar1.ShowIntegration))
                //{
                //    ClearRate();
                //    SaveRate("State", State);
                //    SaveRate("Load", Lo);
                //    SaveRate("Remin", Remin);
                //    SaveRate("Assim", Assm);
                //    SaveRate("Washout", WaO);
                //    SaveRate("WashIn", WaI);
                //    if (!AQTSeg.LinkedMode)
                //    {
                //        SaveRate("TurbDiff", TD);
                //    }
                //    else
                //    {
                //        // If Not AQTSeg.CascadeRunning
                //        // then
                //        SaveRate("DiffUp", DiffUp);
                //        SaveRate("DiffDown", DiffDown);
                //    }
                //    if (AQTSeg.EstuarySegment)
                //    {
                //        SaveRate("Entrainment", En);
                //    }
                //    if (PSed != null)
                //    {
                //        SaveRate("Diag_Flux", DiaFlx);
                //    }
                //    SaveRate("Sorpt.CaCO3", CaCO3srb);
                //    SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
                //}
            }

            // --------------------------------------------------
            public void Derivative_TrackMB()  // FIXME mass balance tracking
            {
                // PO4Obj
                //double LoadInKg;
                //double LossInKg;
                //double LayerInKg;

                //MBLoadRecord MBLR = AQTSeg.MBLoadArray[AllVariables.Phosphate];  // FIXME mass balance tracking
                //LoadInKg = Lo * _wvar1.SegVol() * 1000.0 * 1e-6;
                //MBLR.BoundLoad[_wvar1.DerivStep] = _wvar2.BoundLoad[_wvar1.DerivStep] + LoadInKg;
                // Load into modeled system

                //  LoadInKg = (Lo + WaI) * AQTSeg.SegVol() * 1000.0 * 1e-6;
                //// kg P       // mg P/L        // m3      // L/m3  // kg/mg 
                //{
                //    LoadInKg = (Lo + WaI + En) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //}
                //MBLR.TotOOSLoad[_wvar1.DerivStep] = _wvar2.TotOOSLoad[_wvar1.DerivStep] + LoadInKg;
                //MBLR.LoadH2O[_wvar1.DerivStep] = _wvar2.LoadH2O[_wvar1.DerivStep] + LoadInKg;
                //MBLossRecord _wvar3 = _wvar1.MBLossArray[AllVariables.Phosphate];

                //    MorphRecord Morph = AQTSeg.Location.Morph;
                //// * OOSDischFrac
                //LossInKg = WaO * AQTSeg.SegVol() * 1000.0 * 1e-6;
                //// 3/20/2014 remove OOSDischFrac
                //// kg P
                //// mg P/L
                //// m3
                //// L/m3
                //// kg/mg
                //_wvar3.BoundLoss[_wvar1.DerivStep] = _wvar3.BoundLoss[_wvar1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //if (En < 0)
                //{
                //    // *OOSDischFrac
                //    LossInKg = (-En + WaO) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //}
                //// 3/20/2014 remove OOSDischFrac
                //_wvar3.TotalNLoss[_wvar1.DerivStep] = _wvar3.TotalNLoss[_wvar1.DerivStep] + LossInKg;
                //_wvar3.TotalWashout[_wvar1.DerivStep] = _wvar3.TotalWashout[_wvar1.DerivStep] + LossInKg;
                //_wvar3.WashoutH2O[_wvar1.DerivStep] = _wvar3.WashoutH2O[_wvar1.DerivStep] + LossInKg;
                //LossInKg = CaCO3srb * _wvar1.SegVol() * 1000.0 * 1e-6;
                //// kg P
                //// mg P/L
                //// m3
                //// L/m3
                //// kg/mg
                //_wvar3.TotalNLoss[_wvar1.DerivStep] = _wvar3.TotalNLoss[_wvar1.DerivStep] + LossInKg;
                //_wvar3.BoundLoss[_wvar1.DerivStep] = _wvar3.BoundLoss[_wvar1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //_wvar3.CaCO3Sorb[_wvar1.DerivStep] = _wvar3.CaCO3Sorb[_wvar1.DerivStep] + LossInKg;
                //MBLayerRecord _wvar5 = _wvar1.MBLayerArray[AllVariables.Phosphate];
                //LayerInKg = (TD + DiffUp + DiffDown) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //// kg N
                //// mg P/L
                //// m3
                //// L/m3
                //// kg/mg
                //_wvar5.NTurbDiff[_wvar1.DerivStep] = _wvar5.NTurbDiff[_wvar1.DerivStep] + LayerInKg;
                //_wvar5.NNetLayer[_wvar1.DerivStep] = _wvar5.NNetLayer[_wvar1.DerivStep] + LayerInKg;
                //// with

            }

            public virtual void Derivative(double DB)
            {
                double Lo;
                double Remin;
                double Assm;
                double WaO;
                double WaI;
                double TD;
                double DiffUp;
                double DiffDown;
                double En;
                double DiaFlx;
                double CaCO3srb;

                // TPO4_Sediment PSed;
                // TrackMB
                // --------------------------------------------------
                //double PFluxKg;
                // PO4Obj.Derivative
                Lo = 0;
                Remin = 0;
                Assm = 0;
                WaO = 0;
                WaI = 0;
                TD = 0;
                En = 0;
                DiffUp = 0;
                DiffDown = 0;
                DiaFlx = 0;
                CaCO3srb = 0;
                Lo = Loading;
                Remin = Remineralization();
                // Assm = Assimilation();  fixme plant linkage
                Assm = 0;
                WaO = Washout();

                //if (AQTSeg.LinkedMode)
                //{    WaI = Washin(); }

                //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
                //{
                //    DiffUp = SegmentDiffusion(true);
                //    DiffDown = SegmentDiffusion(false);
                //}
                //else if ((!AQTSeg.LinkedMode))
                //{
                //    TD = TurbDiff();
                //}
                //if (AQTSeg.EstuarySegment)
                //{
                //    En = EstuaryEntrainment();
                //}

                //PSed = AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1);  // check for diagenesis model   //FIXME Diagenesis linkage
                //if (PSed != null)
                //{
                //    TStates _wvar6 = AQTSeg;
                //    Diagenesis_Rec _wvar7 = _wvar6.Diagenesis_Params;
                //    MorphRecord _wvar8 = _wvar6.Location.Morph;
                //    DiaFlx = PSed.Flux2Water() * _wvar6.DiagenesisVol(1) / _wvar8.SegVolum[_wvar6.VSeg];
                //    // mg/L d
                //    // g/m3 sed1 d
                //    // m3 sed1
                //    // m3 water
                //    _wvar6.Diag_Track[TAddtlOutput.TSP_Diag, _wvar6.DerivStep] = PSed.Flux2Water() * _wvar7.H1.Val * 1e3;
                //    // mg/m2 d
                //    // g/m3 sed d
                //    // m
                //    // mg/g
                //    MBLayerRecord _wvar9 = _wvar6.MBLayerArray[AllVariables.Phosphate];
                //    // 3/16/2010 track total P flux from diagenesis into water column in kg since start of simulation
                //    PFluxKg = DiaFlx * _wvar6.SegVol() * 1000.0 * 1e-6;
                //    // kg
                //    // mg/L
                //    // m3
                //    // L/m3
                //    // kg/mg
                //    _wvar9.PFluxD[_wvar6.DerivStep] = _wvar9.PFluxD[_wvar6.DerivStep] + PFluxKg;
                //}

                //ReminRecord RR = AQTSeg.Location.Remin;   // FIXME PLANT LINKAGE CALCITE PCPT
                //CaCO3srb = RR.KD_P_Calcite * State * AQTSeg.CalcitePcpt() * 1e-6;
                // mg P/L d     // L/Kg       // mg/L  // mg Calcite/L d     // kg/mg

                DB = Lo + Remin - Assm - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx - CaCO3srb;
                Derivative_WriteRates();
                Derivative_TrackMB();
            }

        }

        public class TNH4Obj : TRemineralize
        {
            public double PhotoResp = 0;
            public double DarkResp = 0;
            public double AnimExcr = 0;
            public double AnimPredn = 0;
            public double SvNutrRelColonization = 0;
            public double SvNutrRelMortality = 0;
            public double SvNutrRelGamLoss = 0;
            public double SvNutrRelPeriScr = 0;
            public double SvNutrRelPlantSink = 0;
            public double SvNutrRelDefecation = 0;
            public double SvSumDetrDecomp = 0;

            public TNH4Obj(int Ns, T_SVType SVT, string aName, TStates P, double IC, bool IsTempl) : base(Ns, SVT, aName, P, IC, IsTempl)
            {
            }

            // {mg N/ L}
            // ---------------------------------
            // nitrification & denitrification
            // ---------------------------------
            public double Nitrification()
            {
                double result;
                double T;
                double p;
                double Nitrify;
                double DOCorr;
                double EnvironLim;
                // Correction for Oxygen, Temperature, pH
                if (State > Consts.VSmall)
                {
                    ReminRecord RR = Location.Remin;
                    T = AQTSeg.TCorr(2.0, 10.0, 30.0, 60.0);
                    p = pHCorr(7.0, 9.0);
                    if (Location.SiteType == SiteTypes.Marine)
                    {
                        // 5/6/2013
                        DOCorr = 1.0;
                    }
                    else
                    {
                        DOCorr = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol) / (0.5 + AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol));
                    }
                    EnvironLim = DOCorr * T * p;
                    // * Sed_Surf_Corr
                    Nitrify = RR.KNitri * EnvironLim * State;
                    // 3/12/09 Removed Sed_Surf_Corr
                    // Note KNitri may need to be calibrated to a lower value when Sed. Diagenesis enabled
                    // with
                    // > Tiny
                }
                else
                {
                    Nitrify = 0.0;
                }
                result = Nitrify;
                return result;
            }

            // real
            // denitrification
            // -------------------------------------------------------------------------------------------------------
            public double Remineralization()
            {
                double result;
                double Remin;
                Remin = 0;

                //PhotoResp = CalcPhotoResp();  // When photorespiration takes place in plants, excess nutrients are converted into NH4  // FIXME Plant Linkage
                //DarkResp = CalcDarkResp();  // When dark respiration occurs, excess nutrients are converted into NH4. // FIXME Plant Linkage

                //AnimExcr = CalcAnimResp_Excr();  // FIXME Detrital Linkage       // direct respiration losses plus excess nutrient losses from excretion to detritus
                //AnimPredn = CalcAnimPredn();  // FIXME Animal Linkage            // When predation occurs, differences in nutrients are refelcted in W.C.

                //SvNutrRelColonization = NutrRelColonization();  // FIXME Detrital LInkage
                //SvNutrRelMortality = NutrRelMortality(); // FIXME Animal and Plant LInkage
                //SvNutrRelGamLoss = NutrRelGamLoss();  // FIXME Animal Linkage
                //SvNutrRelPeriScr = NutrRelPeriScr();  // FIXME Plant Linkage
                //SvNutrRelPlantSink = NutrRelPlantSink();  // FIXME Plant Linkage
                //SvNutrRelDefecation = NutrRelDefecation(); // FIXME Animal Linkage

                SvSumDetrDecomp = SumDetrDecomp(T_SVType.NTrack, false);
                Remin = Remin + PhotoResp + DarkResp + AnimExcr + AnimPredn + SvNutrRelColonization + SvNutrRelMortality + SvNutrRelGamLoss + SvNutrRelPeriScr + SvNutrRelPlantSink + SvNutrRelDefecation + SvSumDetrDecomp;
                // mg N/ L
                // mg N/ L
                result = Remin;
                return result;
            }

            // --------------------------------------------------
            public void Derivative_WriteRates()
            {
                //Setup_Record _wvar1 = AQTSeg.SetupRec;
                //if ((_wvar1.SaveBRates || _wvar1.ShowIntegration))
                //{
                //    ClearRate();
                //    SaveRate("State", State);
                //    SaveRate("Load", Lo);
                //    SaveRate("Remin", Re);
                //    SaveRate("Nitrif", Ni);
                //    SaveRate("Assimil", Assm);
                //    SaveRate("Washout", WaO);
                //    SaveRate("WashIn", WaI);
                //    if (!AQTSeg.LinkedMode)
                //    {
                //        SaveRate("TurbDiff", TD);
                //    }
                //    else
                //    {
                //        // If Not AQTSeg.CascadeRunning
                //        // then
                //        SaveRate("DiffUp", DiffUp);
                //        SaveRate("DiffDown", DiffDown);
                //    }
                //    if (AQTSeg.EstuarySegment)
                //    {
                //        SaveRate("Entrainment", En);
                //    }
                //    if (NSed != null)
                //    {
                //        SaveRate("Diag_Flux", DiaFlx);
                //    }
                //    SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
                //    // RELEASE 3.1 PLUS EXPLICIT AMMONIA REMINERALIZATION OUTPUT  JSC 8/16/2012
                //    // SaveRate('PhotoResp',PhotoResp);
                //    // SaveRate('DarkResp',DarkResp);
                //    // SaveRate('AnimExcr',AnimExcr);
                //    // SaveRate('AnimPredn',AnimPredn);
                //    // SaveRate('NutrRelColonization',SvNutrRelColonization);
                //    // SaveRate('NutrRelMortality',SvNutrRelMortality);
                //    // SaveRate('NutrRelGamLoss',SvNutrRelGamLoss);
                //    // SaveRate('NutrRelPeriScr',SvNutrRelPeriScr);
                //    // SaveRate('NutrRelPlantSink',SvNutrRelPlantSink);
                //    // SaveRate('NutrRelDefecation',SvNutrRelDefecation);
                //    // SaveRate('DetritalDecomp',SvSumDetrDecomp);
                //    // RELEASE 3.1 PLUS EXPLICIT AMMONIA REMINERALIZATION OUTPUT  JSC 8/16/2012
                // }
            }

            // --------------------------------------------------
            public void Derivative_TrackMB()
            {
                //double LoadInKg;  // FIXME MB Tracking
                //double LossInKg;
                //double LayerInKg;

                //MBLoadRecord _wvar2 = _wvar1.MBLoadArray[AllVariables.Nitrate];
                //LoadInKg = Lo * _wvar1.SegVol() * 1000.0 * 1e-6;
                //_wvar2.BoundLoad[_wvar1.DerivStep] = _wvar2.BoundLoad[_wvar1.DerivStep] + LoadInKg;
                //// Load into modeled system
                //LoadInKg = (Lo + WaI) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //if (En > 0)
                //{
                //    LoadInKg = (Lo + WaI + En) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //}
                //_wvar2.TotOOSLoad[_wvar1.DerivStep] = _wvar2.TotOOSLoad[_wvar1.DerivStep] + LoadInKg;
                //_wvar2.LoadH2O[_wvar1.DerivStep] = _wvar2.LoadH2O[_wvar1.DerivStep] + LoadInKg;
                //MBLossRecord _wvar3 = _wvar1.MBLossArray[AllVariables.Nitrate];
                //MorphRecord _wvar4 = _wvar1.Location.Morph;
                //// * OOSDischFrac
                //LossInKg = WaO * _wvar1.SegVol() * 1000.0 * 1e-6;
                //// 3/20/2014 remove OOSDischFrac
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //_wvar3.BoundLoss[_wvar1.DerivStep] = _wvar3.BoundLoss[_wvar1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //if (En < 0)
                //{
                //    // * OOSDischFrac
                //    LossInKg = (-En + WaO) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //}
                //// 3/20/2014 remove OOSDischFrac
                //_wvar3.TotalNLoss[_wvar1.DerivStep] = _wvar3.TotalNLoss[_wvar1.DerivStep] + LossInKg;
                //_wvar3.TotalWashout[_wvar1.DerivStep] = _wvar3.TotalWashout[_wvar1.DerivStep] + LossInKg;
                //_wvar3.WashoutH2O[_wvar1.DerivStep] = _wvar3.WashoutH2O[_wvar1.DerivStep] + LossInKg;
                //MBLayerRecord _wvar5 = _wvar1.MBLayerArray[AllVariables.Nitrate];
                //LayerInKg = (TD + DiffUp + DiffDown) * _wvar1.SegVol() * 1000.0 * 1e-6;
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //_wvar5.NTurbDiff[_wvar1.DerivStep] = _wvar5.NTurbDiff[_wvar1.DerivStep] + LayerInKg;
                //_wvar5.NNetLayer[_wvar1.DerivStep] = _wvar5.NNetLayer[_wvar1.DerivStep] + LayerInKg;
                //// with

            }

            public virtual void Derivative(double DB)
            {
                double Lo;
                double Re;
                double Ni;
                double Assm;
                double WaO;
                double WaI;
                double TD;
                double DiffUp;
                double DiffDown;
                double En;
                double DiaFlx;
                //            TNH4_Sediment NSed;
                // TrackMB
                // --------------------------------------------------
                // NH4Obj.Derivative
                Lo = 0;
                Re = 0;
                Ni = 0;
                Assm = 0;
                WaO = 0;
                WaI = 0;
                TD = 0;
                DiffUp = 0;
                DiffDown = 0;
                En = 0;
                DiaFlx = 0;
                Lo = Loading;
                DB = 0;
                if (AQTSeg.PSetup.AmmoniaIsDriving)
                {
                    State = Loading;
                    Derivative_WriteRates();
                    // 5/24/2013  Ammonia as a driving variable
                    return;
                    // 5/24/2013  Ammonia as a driving variable
                }
                Re = Remineralization();
                Ni = Nitrification();
                // Assm = Assimilation();  fixme Plant Linkage
                WaO = Washout();

                //if (AQTSeg.LinkedMode)
                //{
                //    WaI = Washin();
                //}
                //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
                //{
                //    DiffUp = SegmentDiffusion(true);
                //    DiffDown = SegmentDiffusion(false);
                //}
                //else if ((!AQTSeg.LinkedMode))
                //{
                //    TD = TurbDiff();
                //}

                //if (AQTSeg.EstuarySegment)
                //{
                //    En = EstuaryEntrainment();
                //}

                //    NSed = AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1); // search for diagenesis model  // FIXME Diagenesis linkage
                //if (NSed != null)
                //{
                //    TStates _wvar6 = AQTSeg;
                //    Diagenesis_Rec _wvar7 = _wvar6.Diagenesis_Params;
                //    MorphRecord _wvar8 = _wvar6.Location.Morph;
                //    DiaFlx = NSed.Flux2Water() * _wvar6.DiagenesisVol(1) / _wvar8.SegVolum[_wvar6.VSeg];
                //    // mg/L d
                //    // g/m3 sed1 d
                //    // m3 sed1
                //    // m3 water
                //    _wvar6.Diag_Track[TAddtlOutput.NH3_Diag, _wvar6.DerivStep] = NSed.Flux2Water() * _wvar7.H1.Val * 1e3;
                //}
                //// mg/m2 d
                //// g/m3 sed d
                //// m
                //// mg/g
                //// if (State < VSmall) and ((Lo + Re + TD + DiffUp + DiffDown + En + DiaFlx) < VSmall)
                //// then  begin dB := 0.0; Lo:=0;Re:=0;Ni:=0;Assm:=0;WaO:=0;WaI:=0;TD:=0;DiffUp:=0;DiffDown:=0;En:=0;DiaFlx:=0; End
                //// else

                DB = Lo + Re - Ni - Assm - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx;
                Derivative_WriteRates();
                Derivative_TrackMB();
            }

        } // end TNH4Obj


        public class TNO3Obj : TRemineralize
        {
            public double NotUsed = 0;
            public double[] Alt_NotUsed;
            public bool TN_IC = false;
            public bool TN_Inflow = false;
            public bool TN_PS = false;
            public bool unused = false;
            public bool TN_NPS = false;


            public TNO3Obj(int Ns, T_SVType SVT, string aName, TStates P, double IC, bool IsTempl) : base(Ns, SVT, aName, P, IC, IsTempl)
            {
            }

            // -------------------------------------------------------------------------------------------------------
            public double Denitrification()
            {
                double result;
                // , Sed_Surf_Corr
                double T;
                double p;
                double DOCorr;
                double Denitrify;
                double EnvironLim;
                if (State > Consts.VSmall)
                {
                    ReminRecord _wvar1 = Location.Remin;
                    T = AQTSeg.TCorr(2.0, 10.0, 30.0, 60.0);
                    p = pHCorr(5.0, 9.0);
                    if (Location.SiteType == SiteTypes.Marine)
                    {
                        // 5/6/2013
                        DOCorr = 0.0;
                    }
                    else
                    {
                        DOCorr = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol) / (0.5 + AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol));
                    }
                    EnvironLim = (1.0 - DOCorr) * T * p;
                    Denitrify = (_wvar1.KDenitri_Wat * EnvironLim) * State;
                    // 3/13/09 Reformulated
                    // (*     With AQTSeg do
                    // {Calculate Correction for Sediment Surface Phenomenon, Unitless}
                    // {Assumed that surface area = sediment area}
                    // Sed_Surf_Corr :=  SurfaceArea/SegVol;         Irrelevant 5-18-2011 *)
                    // (*   If Not AQTSeg.Diagenesis_Included then
                    // Denitrify := Denitrify +  (KDenitri_Bot  * Sed_Surf_Corr * T * P) * State     Removed KDenitri Bottom as per "Denitrification 5-17-11.docx" *)
                    // with
                    // > Tiny
                }
                else
                {
                    Denitrify = 0.0;
                }
                result = Denitrify;
                return result;
            }

            // --------------------------------------------------
            public void Derivative_WriteRates()
            {
                //Setup_Record _wvar1 = AQTSeg.SetupRec;  // FIXME Output Rates
                //if ((_wvar1.SaveBRates || _wvar1.ShowIntegration))
                //{
                //    ClearRate();
                //    SaveRate("State", State);
                //    SaveRate("Load", Lo);
                //    SaveRate("Nitrif", Nitr);
                //    SaveRate("DeNitrif", Denitr);
                //    SaveRate("NO3Assim", NO3Assim);
                //    SaveRate("Washout", WaO);
                //    SaveRate("WashIn", WaI);
                //    if (!AQTSeg.LinkedMode)
                //    {
                //        SaveRate("TurbDiff", TD);
                //    }
                //    else
                //    {
                //        // If Not AQTSeg.CascadeRunning
                //        // then
                //        SaveRate("DiffUp", DiffUp);
                //        SaveRate("DiffDown", DiffDown);
                //    }
                //    if (AQTSeg.EstuarySegment)
                //    {
                //        SaveRate("Entrainment", En);
                //    }
                //    if (N2Sed != null)
                //    {
                //        SaveRate("Diag_Flux", DiaFlx);
                //    }
                //    SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
                //}
            }

            // --------------------------------------------------
            //public void Derivative_TrackMB()  FIXME MB Tracking
            //{
            //    // Track MB For NO3Obj
            //    double LoadInKg;
            //    double LossInKg;
            //    double LayerInKg;
            //    TStates _wvar1 = AQTSeg;
            //    MBLoadRecord _wvar2 = _wvar1.MBLoadArray[AllVariables.Nitrate];
            //    LoadInKg = Lo * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    _wvar2.BoundLoad[_wvar1.DerivStep] = _wvar2.BoundLoad[_wvar1.DerivStep] + LoadInKg;
            //    // Load into modeled system
            //    LoadInKg = (Lo + WaI) * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    // kg N
            //    // mg N/L
            //    // m3
            //    // L/m3
            //    // kg/mg
            //    if ((En > 0))
            //    {
            //        LoadInKg = (Lo + WaI + En) * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    }
            //    _wvar2.TotOOSLoad[_wvar1.DerivStep] = _wvar2.TotOOSLoad[_wvar1.DerivStep] + LoadInKg;
            //    _wvar2.LoadH2O[_wvar1.DerivStep] = _wvar2.LoadH2O[_wvar1.DerivStep] + LoadInKg;
            //    MBLossRecord _wvar3 = _wvar1.MBLossArray[AllVariables.Nitrate];
            //    MorphRecord _wvar4 = _wvar1.Location.Morph;
            //    // * OOSDischFrac
            //    LossInKg = WaO * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    // 3/20/2014 remove OOSDischFrac
            //    // kg N
            //    // mg N/L
            //    // m3
            //    // L/m3
            //    // kg/mg
            //    _wvar3.BoundLoss[_wvar1.DerivStep] = _wvar3.BoundLoss[_wvar1.DerivStep] + LossInKg;
            //    // Loss from the modeled system
            //    if (En < 0)
            //    {
            //        // * OOSDischFrac
            //        LossInKg = (-En + WaO) * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    }
            //    // 3/20/2014 remove OOSDischFrac
            //    _wvar3.TotalNLoss[_wvar1.DerivStep] = _wvar3.TotalNLoss[_wvar1.DerivStep] + LossInKg;
            //    _wvar3.TotalWashout[_wvar1.DerivStep] = _wvar3.TotalWashout[_wvar1.DerivStep] + LossInKg;
            //    _wvar3.WashoutH2O[_wvar1.DerivStep] = _wvar3.WashoutH2O[_wvar1.DerivStep] + LossInKg;
            //    LossInKg = Denitr * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    // kg N
            //    // mg N/L
            //    // m3
            //    // L/m3
            //    // kg/mg
            //    _wvar3.TotalNLoss[_wvar1.DerivStep] = _wvar3.TotalNLoss[_wvar1.DerivStep] + LossInKg;
            //    _wvar3.BoundLoss[_wvar1.DerivStep] = _wvar3.BoundLoss[_wvar1.DerivStep] + LossInKg;
            //    // Loss from the modeled system
            //    _wvar3.Denitrify[_wvar1.DerivStep] = _wvar3.Denitrify[_wvar1.DerivStep] + LossInKg;
            //    MBLayerRecord _wvar5 = _wvar1.MBLayerArray[AllVariables.Nitrate];
            //    LayerInKg = (TD + DiffUp + DiffDown) * _wvar1.SegVol() * 1000.0 * 1e-6;
            //    // kg N
            //    // mg N/L
            //    // m3
            //    // L/m3
            //    // kg/mg
            //    _wvar5.NTurbDiff[_wvar1.DerivStep] = _wvar5.NTurbDiff[_wvar1.DerivStep] + LayerInKg;
            //    _wvar5.NNetLayer[_wvar1.DerivStep] = _wvar5.NNetLayer[_wvar1.DerivStep] + LayerInKg;
            //    // with

            //}

            // real
            public virtual void Derivative(double DB)
            {
                TNH4Obj P;
                double Lo;
                double Nitr;
                double Denitr;
                double NO3Assim;
                double WaO;
                double WaI;
                double TD;
                double DiffUp;
                double DiffDown;
                double En;
                double DiaFlx;
                //    TNO3_Sediment N2Sed;
                // TrackMB
                // --------------------------------------------------
                // NO3Obj.Derivative
                Lo = 0;
                Nitr = 0;
                Denitr = 0;
                NO3Assim = 0;
                WaO = 0;
                WaI = 0;
                TD = 0;
                DiffUp = 0;
                DiffDown = 0;
                En = 0;
                DiaFlx = 0;
                P = ((AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol)) as TNH4Obj);
                Lo = Loading;
                Nitr = P.Nitrification();
                Denitr = Denitrification();
                //  NO3Assim = Assimilation();   FIXME PLANT LINNKAGE
                WaO = Washout();
                //if (AQTSeg.LinkedMode)
                //{
                //    WaI = Washin();
                //}
                //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
                //{
                //    DiffUp = SegmentDiffusion(true);
                //    DiffDown = SegmentDiffusion(false);
                //}
                //else if ((!AQTSeg.LinkedMode))
                //{
                //    TD = TurbDiff();
                //}
                //if (AQTSeg.EstuarySegment)
                //{
                //    En = EstuaryEntrainment();
                //}

                //N2Sed = AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
                //if (N2Sed != null)  // FIXME DIAGENESIS LINKAGE
                //{
                //    TStates _wvar6 = AQTSeg;
                //    Diagenesis_Rec _wvar7 = _wvar6.Diagenesis_Params;
                //    MorphRecord _wvar8 = _wvar6.Location.Morph;
                //    DiaFlx = N2Sed.Flux2Water() * _wvar6.DiagenesisVol(1) / _wvar8.SegVolum[_wvar6.VSeg];
                //    // mg/L d
                //    // g/m3 sed1 d
                //    // m3 sed1
                //    // m3 water
                //    _wvar6.Diag_Track[TAddtlOutput.No3_Diag, _wvar6.DerivStep] = N2Sed.Flux2Water() * _wvar7.H1.Val * 1e3;
                //}
                // mg/m2 d       // g/m3 sed d        // m        // mg/g
                // If (State < VSmall) and ((Lo + Nitr + TD + DiffUp + DiffDown + En+ DiaFlx) < VSmall)
                // then Begin dB := 0.0;  Lo:=0;Nitr:=0;Denitr:=0;NO3Assim:=0;WaO:=0;WaI:=0;TD:=0;DiffUp:=0;DiffDown:=0;En:=0;DiaFlx:=0; End
                // else
                DB = Lo + Nitr - Denitr - NO3Assim - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx;
                Derivative_WriteRates();
                // Derivative_TrackMB();  // FIXME RATES TRACKING
            }

        } // TNO3Obj

            public class TSalinity : TRemineralize  //Salinity is a DRIVING Variable Only
            {
                public double SalinityUpper = 0;
                public double SalinityLower = 0;
                public TSalinity(int Ns, T_SVType SVT, string aName, TStates P, double IC, bool IsTempl) : base(Ns, SVT, aName, P, IC, IsTempl)
                {
                }

                public override void Derivative(ref double DB)
                {
                    base.Derivative(ref DB);
                    DB = 0;
                }

                public override void CalculateLoad(DateTime TimeIndex)
                {
                    Loading = 0;
                    State = LoadsRec.ReturnLoad(TimeIndex);
                    SalinityUpper = State;
                    SalinityLower = LoadsRec.ReturnAltLoad(TimeIndex, 0);

                    //if (AQTSeg != null)
                    //{
                    //    if (AQTSeg.VSeg == VerticalSegments.Hypolimnion)
                    //    {
                    //        State = SalinityLower;
                    //    }
                    //}
                }

            } // end TSalinity



        }