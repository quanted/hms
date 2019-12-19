using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.OrgMatter;
using AQUATOX.Diagenesis;
using AQUATOX.Plants;
using AQUATOX.Organisms;
using AQUATOX.Animals;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.Nutrients
{

    public class TRemineralize : TStateVariable
    {

        public void Check_Nutrient_IC()
        {
            // if using TN or TP for init cond
            TNH4Obj PNH4;
            double CNutrient, Nut2Org;
            AllVariables NSLoop;

            if (!((NState == AllVariables.Nitrate) || (NState == AllVariables.Phosphate))) return;  // procedure for TP and TN only
            if (!((SVType == T_SVType.StV) || (Layer == T_SVLayer.WaterCol))) return;

            if (((NState == AllVariables.Nitrate) && (((TNO3Obj)this).TN_IC)) || ((NState == AllVariables.Phosphate) && (((TPO4Obj)this).TP_IC)))
                {
                    CNutrient = State;   // Total Nutrient in mg/L

                    for (NSLoop = AllVariables.DissRefrDetr; NSLoop <= AllVariables.SuspLabDetr; NSLoop++)
                    {
                        if (AQTSeg.GetStateVal(NSLoop, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                        {
                            if (NState == AllVariables.Nitrate)
                            {
                            Nut2Org = NSLoop switch
                            {
                                AllVariables.SuspRefrDetr => Location.Remin.N2Org_Refr,
                                AllVariables.SuspLabDetr => Location.Remin.N2OrgLab,
                                AllVariables.DissRefrDetr => Location.Remin.N2OrgDissRefr,
                                _ => Location.Remin.N2OrgDissLab,
                            };
                            // Case
                        }
                            else
                            {
                            Nut2Org = NSLoop switch
                            {
                                AllVariables.SuspRefrDetr => Location.Remin.P2Org_Refr,
                                AllVariables.SuspLabDetr => Location.Remin.P2OrgLab,
                                AllVariables.DissRefrDetr => Location.Remin.P2OrgDissRefr,
                                _ => Location.Remin.P2OrgDissLab,
                            };
                        }
                            // Case
                            CNutrient = CNutrient - AQTSeg.GetState(NSLoop, T_SVType.StV, T_SVLayer.WaterCol) * Nut2Org;
                            // mg/L     // mg/L             // mg/L      // N2Org
                        }
                    }

                    for (NSLoop = Consts.FirstPlant; NSLoop <= Consts.LastPlant; NSLoop++)
                    {
                        TStateVariable TSV = AQTSeg.GetStatePointer(NSLoop, T_SVType.StV, T_SVLayer.WaterCol);
                        if (TSV != null)
                        {
                            TPlant PPl = TSV as TPlant;
                            if (PPl.IsPhytoplankton())
                            {
                                if (NState == AllVariables.Nitrate)
                                {
                                    Nut2Org = PPl.PAlgalRec.N2OrgInit;
                                }
                                else
                                {
                                    Nut2Org = PPl.PAlgalRec.P2OrgInit;
                                }
                                CNutrient = CNutrient - PPl.State * Nut2Org;
                                // mg/L     // mg/L     // mg/L    // N2Org
                            }
                        }
                    }
                    if (CNutrient < 0) CNutrient = 0;
                    State = CNutrient;

                    if (NState == AllVariables.Nitrate)
                    {
                        // No ammonia if nitrogen initial condition is input as Total N
                        PNH4 = (TNH4Obj)AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
                        PNH4.State = 0;
                    }
                }
        }



        public TRemineralize(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
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
        public void CalculateLoad_TotNutrient_Alt_Ldg(ref double AddLoad, int LdType, ref DateTime TimeIndex, ref double SegVolume)
        {
            double CNutrient;
            AllVariables nsloop;
            double DetrAltLdg;
            TDetritus TDetr;
            DetritalInputRecordType PInputRec; 
            PInputRec = ((AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TDissRefrDetr).InputRecord;

            CNutrient = AddLoad;

            for (nsloop = AllVariables.DissRefrDetr; nsloop <= AllVariables.SuspLabDetr; nsloop++)
            {
                TDetr = AQTSeg.GetStatePointer(nsloop, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
                if (TDetr != null)
                {
                    DetrAltLdg = PInputRec.Load.ReturnAltLoad(TimeIndex, LdType); 
                    // g/d                                                       
                    DetrAltLdg = DetrAltLdg / SegVolume;
                    // mg/L d      // g/d     // cu m
                    CNutrient = CNutrient - DetrAltLdg * CalculateLoad_DetrNutr2Org(nsloop);
                    // mg/L       // mg/L    // mg/L                   // Nut2Org
                }
            }

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


        public override void SetToInitCond()
        {
            base.SetToInitCond();
            Check_Nutrient_IC();
        }
        // ------------------------------------------------------------------------------------------------
        public void CalculateLoad_TotNutrient_Dynamic_Inflow(ref DateTime TimeIndex, ref double Inflow, ref double SegVolume)
        {
            double CNutrient;
            double PlantInflow;
            double DetrInflow;
            TStateVariable PSV;
            AllVariables nsLoop;
            double InflLoad;
            CNutrient = Loading;
            // Total Nutrient loading in mg/L d

            for (nsLoop = AllVariables.DissRefrDetr; nsLoop <= AllVariables.SuspLabDetr; nsLoop++)  
            {
                PSV = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (PSV != null)
                {
                    InflLoad = ((PSV) as TDetritus).GetInflowLoad(TimeIndex);
                    // Inflow Loadings Only
                    DetrInflow = InflLoad * Inflow / SegVolume; // Inflow Loadings Only
                    // mg/L d   // mg/L d  // m3/d     // m3
                    CNutrient = CNutrient - DetrInflow * CalculateLoad_DetrNutr2Org(nsLoop);
                    // mg/L d  // mg/L d     // mg/L d                // Nut2Org
                }
            }

            for (nsLoop = Consts.FirstPlant; nsLoop <= Consts.LastPlant; nsLoop++) 
            {
                PSV = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (PSV != null)
                {
                    if (((PSV) as TPlant).IsPhytoplankton())
                    {
                        InflLoad = ((PSV) as TStateVariable).GetInflowLoad(TimeIndex);
                        // Inflow Loadings Only
                        PlantInflow = InflLoad * Inflow / SegVolume;
                        // Inflow Loadings Only
                        if (NState == AllVariables.Phosphate)
                             CNutrient = CNutrient - PlantInflow * ((PSV) as TPlant).P_2_Org();
                        else CNutrient = CNutrient - PlantInflow * ((PSV) as TPlant).N_2_Org();
                    }        // mg/L d     // mg/L d   // mg/L d                    // N2Org

                }
            }

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
            Inflow = Location.Morph.InflowH2O; // * (Location.Morph.OOSInflowFrac);  Removed Linked Mode Logic

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
                    CalculateLoad_TotNutrient_Dynamic_Inflow(ref TimeIndex, ref Inflow, ref SegVolume);
                }
            }
            if ((NState == AllVariables.Nitrate))
            {
                if (PNO3.TN_Inflow)
                {
                    CalculateLoad_TotNutrient_Dynamic_Inflow(ref TimeIndex, ref Inflow, ref SegVolume);
                }
            }
            if ((NState == AllVariables.Phosphate))
            {
                if (((this) as TPO4Obj).TP_Inflow)
                {
                    CalculateLoad_TotNutrient_Dynamic_Inflow(ref TimeIndex, ref Inflow, ref SegVolume);
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
                            AddLoad = LoadsRec.ReturnAltLoad(TimeIndex, Loop);  //multiplied by multldg factor here
                            // g/d or g/sq m. d

                            AddLoad = AddLoad / SegVolume;
                            // mg/L d  // g/d     // cu m      // note if direct precip result is mg/(sq m.*L*d)

                            if (Loop == 1) AddLoad = AddLoad * Location.Locale.SurfArea;  // Loop = 1 is DirectPrecip loadings type
                                                                                          // mg/L d // mg/(sq m.*L*d)            // sq m.

                            if (NState == AllVariables.Phosphate)
                            {
                                TPO4Obj _TP = ((this) as TPO4Obj);
                                if (((Loop == 0) && _TP.TP_PS) || ((Loop == 2) && _TP.TP_NPS))
                                {
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop, ref TimeIndex, ref SegVolume);
                                }
                            }
                            if (NState == AllVariables.Nitrate)
                            {
                                if (((Loop == 0) && PNO3.TN_PS) || ((Loop == 2) && PNO3.TN_NPS))
                                {
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop, ref TimeIndex, ref SegVolume);
                                }
                            }
                            if (NState == AllVariables.Ammonia)
                            {
                                if (((Loop == 0) && PNO3.TN_PS) || ((Loop == 2) && PNO3.TN_NPS))
                                {  // 3/27/08
                                    AddLoad = PNO3.LoadsRec.ReturnAltLoad(TimeIndex, Loop);
                                    AddLoad = AddLoad / SegVolume;
                                    // mg/L d   // g/d    // cu m
                                    CalculateLoad_TotNutrient_Alt_Ldg(ref AddLoad, Loop, ref TimeIndex, ref SegVolume);
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
        public double SumDetrDecomp(T_SVType OType, bool SedOnly)
        {
            // If OType=NTrack returns sum of detrital decomp into ammonia
            // If OType=NTrack returns sum detrital decomp into dissolved phosphorus
            // otherwise returns sum of detrital decomposition in terms of organic matter
            // Sum of Labile Detritus Decomposition
            TRemineralize RP;
            AllVariables EndLoop;
            AllVariables Loop;
            double SumDecomp;
            double FracAerobic=0;
            double Decomp;
            SumDecomp = 0;
            if (SedOnly)
            {
                EndLoop = AllVariables.SedmLabDetr;
            }
            else
            {
                EndLoop = Consts.LastDetr;
            }

            for (Loop = Consts.FirstDetr; Loop <= EndLoop; Loop++)
            {
                ReminRecord _1 = Location.Remin;
                RP = (TRemineralize)AQTSeg.GetStatePointer(Loop, T_SVType.StV, T_SVLayer.WaterCol);
                if ((RP == null))
                {
                    Decomp = 0;
                }
                else
                {
                    Decomp = RP.Decomposition(_1.DecayMax_Lab, Consts.KAnaerobic, ref FracAerobic);
                }

                if (OType == T_SVType.NTrack)
                {
                    if (Loop == AllVariables.DissLabDetr)
                    {
                        Decomp = Decomp * _1.N2OrgDissLab;
                    }
                    else
                    {
                        Decomp = Decomp * _1.N2OrgLab;
                    }
                }
                if (OType == T_SVType.PTrack)
                {
                    if (Loop == AllVariables.DissLabDetr)
                    {
                        Decomp = Decomp * _1.P2OrgDissLab;
                    }
                    else
                    {
                        Decomp = Decomp * _1.P2OrgLab;
                    }
                }
                SumDecomp = SumDecomp + Decomp;
            }
            return SumDecomp;
        }



        // -------------------------
        // sum instantaneous
        // contributions from
        // each process to Detr
        // -------------------------
        public double CalcAnimPredn()  
        {
            double NetPredation;
            // -----------------------------------------------------------
            void CalcAnimPredn_NutrPred(TStateVariable P)
            {
                double Cons;
                double PreyNutr2Org;
                double PredNutr2Org;
                double DiffNutrFrac;
                if (P.IsAnimal())
                {
                    TAnimal PA = (TAnimal)P;
                    if (NState == AllVariables.Phosphate) 
                         PredNutr2Org = PA.PAnimalData.P2Org;
                    else PredNutr2Org = PA.PAnimalData.N2Org;
 
                    PreyNutr2Org = 0;
                    Cons = PA.EatEgest(false);
                    if (Cons > 0)
                    {
                        if (NState == AllVariables.Phosphate)
                             PreyNutr2Org = PA.PhosCons / Cons;
                        else PreyNutr2Org = PA.NitrCons / Cons;
                    }
                    DiffNutrFrac = PredNutr2Org - PreyNutr2Org;
                    NetPredation = NetPredation - Cons * DiffNutrFrac;
                }
            }
            // -----------------------------------------------------------
            NetPredation = 0.0;
            foreach (TStateVariable TSV in AQTSeg.SV) CalcAnimPredn_NutrPred(TSV);
            return NetPredation;
        }

        // Remineralization
        public double NutrRelPeriScr()
        {
            // When Periphyton is scoured into phytoplankton nutrient balance must
            // be maintained if they have different stochiometry
            AllVariables PeriLoop;
            AllVariables PhytLoop;
            TPlant PPeri;
            TPlant PPhyt;
            double NRPS;
            double j;
            double Nut2OrgPeri;
            double Nut2OrgPhyt;
            NRPS = 0;
            for (PhytLoop = Consts.FirstAlgae; PhytLoop <= Consts.LastAlgae; PhytLoop++)
            {
                for (PeriLoop = Consts.FirstAlgae; PeriLoop <= Consts.LastAlgae; PeriLoop++)
                {
                    PPhyt = AQTSeg.GetStatePointer(PhytLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    PPeri = AQTSeg.GetStatePointer(PeriLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if ((PPhyt != null) && (PPeri != null))
                    {
                        if ((PPeri.PSameSpecies == PPhyt.NState))
                        {
                            PPeri.CalcSlough();
                            // update sloughevent
                            if (PPeri.SloughEvent)
                            {
                                if ((NState == AllVariables.Ammonia))
                                    Nut2OrgPeri = PPeri.N_2_Org();
                                else
                                    Nut2OrgPeri = PPeri.P_2_Org();
                                
                                if ((NState == AllVariables.Ammonia))
                                    Nut2OrgPhyt = PPhyt.N_2_Org();
                                else
                                    Nut2OrgPhyt = PPhyt.P_2_Org();

                                j = -999;  // signal to not write mass balance tracking
                                PPeri.Derivative(ref j);  // update sloughing
                                NRPS = NRPS + PPeri.Sloughing * (1 / 3) * (Nut2OrgPeri - Nut2OrgPhyt);
                                // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.
                            }
                        }
                    }
                }
            }
            return NRPS;
        }

        // -------------------------------------------------------------------------------------------------------
        public double NutrRelGamLoss()  
        {
            // When Gameteloss takes place in animals, (-->SuspLabDetr)
            // excess nutrients are converted into NH4.
            AllVariables ns;
            TAnimal PAn;
            double DiffNFrac;
            double NGL;
            NGL = 0;
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    ReminRecord RR = Location.Remin;

                    if (NState == AllVariables.Ammonia)
                          DiffNFrac = PAn.PAnimalData.N2Org - RR.N2OrgLab;
                    else  DiffNFrac = PAn.PAnimalData.P2Org - RR.P2OrgLab;

                    NGL = NGL + PAn.GameteLoss() * DiffNFrac;
                }
            }
            return NGL;
        }

        // -------------------------------------------------------------------------------------------------------
        public double NutrRelMortality()  
        {
            // When Anim & Plant die, excess nutrients are converted into NH4;
            // Detritus tends to have a lower fraction of nutrients then the dying organisms.
            // Macrophyte breakage and Tox Dislodge are included in mortality here
            // for accounting of nutrient mass.
            AllVariables ns;
            TDetritus PDRD;
            TDetritus PDLD;
            TDetritus PPRD;
            TDetritus PPLD;
            TOrganism POr;
            double Mort;
            double DetrNFrac;
            double DiffNFrac;
            double NMort;
            double j;
            double FracMult;
            double Nut2Org_Refr;
            double Nut2Org_Lab;
            double Nut2Org_DissRefr;
            double Nut2Org_DissLab;
            PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            PPRD = AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            PPLD = AQTSeg.GetStatePointer(AllVariables.SuspLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            ReminRecord _1 = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_Refr = _1.N2Org_Refr;
                Nut2Org_Lab = _1.N2OrgLab;
                Nut2Org_DissRefr = _1.N2OrgDissRefr;
                Nut2Org_DissLab = _1.N2OrgDissLab;
            }
            else
            {
                Nut2Org_Refr = _1.P2Org_Refr;
                Nut2Org_Lab = _1.P2OrgLab;
                Nut2Org_DissRefr = _1.P2OrgDissRefr;
                Nut2Org_DissLab = _1.P2OrgDissLab;
            }
            NMort = 0;
            for (ns = Consts.FirstBiota; ns <= Consts.LastBiota; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    POr = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TOrganism;
                    DetrNFrac = PDRD.Mort_To_Detr(ns) * Nut2Org_DissRefr + PPRD.Mort_To_Detr(ns) * Nut2Org_Refr + PPLD.Mort_To_Detr(ns) * Nut2Org_Lab + PDLD.Mort_To_Detr(ns) * Nut2Org_DissLab;
                    DiffNFrac = POr.NutrToOrg(NState) - DetrNFrac;
                    Mort = POr.Mortality();
                    if (POr.IsMacrophyte())
                    {
                        Mort = Mort + ((POr) as TMacrophyte).Breakage();
                    }
                    if (POr.IsPlant() && (!POr.IsMacrophyte()))
                    {
                        TPlant TP = ((POr) as TPlant);
                        Mort = Mort + TP.ToxicDislodge();
                        ((POr) as TPlant).CalcSlough();
                        // update sloughevent
                        if (TP.SloughEvent)
                        {
                            j = -999;
                            // signal to not write mass balance tracking
                            ((POr) as TPlant).Derivative(ref j);
                            // update sloughing
                            if (TP.PSameSpecies == AllVariables.NullStateVar)
                            {
                                FracMult = 1.0;
                            }
                            else
                            {
                                FracMult = 2.0 / 3.0;
                            }
                            // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.
                            Mort = Mort + TP.Sloughing * FracMult;
                        }
                    }
                    NMort = NMort + Mort * DiffNFrac;
                }
            }
            return NMort;
        }

     //    -------------------------------------------------------------------------------------------------------
        public double NutrRelPlantSink()   
        {
            double result;
        // When Plants sink, excess nutrients are converted into NH4;
        // Sedimented Detritus tends to have a lower fraction of nutrients then
        // the sinking plants
        AllVariables ns;
        AllVariables ploop;
        TDetritus PSRD;
        TDetritus PSLD;
        TPlant PPl;
        TPlant PPeri;
        double DetrNFrac;
        double DiffNFrac;
        double NSink;
        double Nut2Org_Refr;
        double Nut2Org_Lab;
        double PeriMass;
        double NumPeriLinks;
        double PeriNFrac;
        double PNFrac2;
        result = 0;
            PSRD = AQTSeg.GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            PSLD = AQTSeg.GetStatePointer(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            if (PSRD == null)
            {
                return result;  
            }


            // diagenesis model in place
            ReminRecord _1 = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_Refr = _1.N2Org_Refr;
                Nut2Org_Lab = _1.N2OrgLab;
            }
            else
            {
                Nut2Org_Refr = _1.P2Org_Refr;
                Nut2Org_Lab = _1.P2OrgLab;
            }
            NSink = 0;
            for (ns = Consts.FirstAlgae; ns <= Consts.LastAlgae; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if (PPl.IsLinkedPhyto())
                    {
                        PeriNFrac = 0;
                        PNFrac2 = 0;
                        NumPeriLinks = 0;
                        PeriMass = 0;
                        for (ploop = Consts.FirstAlgae; ploop <= Consts.LastAlgae; ploop++)
                        {
                            PPeri = AQTSeg.GetStatePointer(ploop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                            if (PPeri != null)
                            {
                                if ((PPeri.IsPeriphyton()) && (PPeri.PSameSpecies == PPl.NState))
                                {
                                    NumPeriLinks = NumPeriLinks + 1.0;
                                    PeriMass = PeriMass + PPeri.State;
                                    if (NState == AllVariables.Ammonia)
                                    {
                                        PeriNFrac = PeriNFrac + PPeri.State* PPeri.N_2_Org();
                                    }
                                    else
                                    {
                                        PeriNFrac = PeriNFrac + PPeri.State* PPeri.P_2_Org();
                                    }
                                    if (PeriMass<Consts.VSmall)
                                    {
                                        if (NState == AllVariables.Ammonia)
                                        {
                                            PNFrac2 = PNFrac2 + PPeri.N_2_Org();
                                        }
                                        else
                                        {
                                            PNFrac2 = PNFrac2 + PPeri.P_2_Org();
                                        }
                                    }
                                }
                            }
                            // will count itself and any other peiphyton species linked to this phytoplankton
                        }
                        // 9/20/2004 debug against zero periphyton
                        if (PeriMass< Consts.VSmall)
                        {
                            // used to split evenly among peri comps.
                            PeriNFrac = PNFrac2 / NumPeriLinks;
                        }
                        else
                        {
                            PeriNFrac = PeriNFrac / PeriMass;
                        }
                        // used to weight by mass of periphyton comps.
                        if (NState == AllVariables.Ammonia)
                        {
                            DiffNFrac = PPl.N_2_Org() - PeriNFrac;
                        }
                        else
                        {
                            DiffNFrac = PPl.P_2_Org() - PeriNFrac;
                        }
                        NSink = NSink + PPl.Sedimentation() * DiffNFrac;
                    }
                    else
                    {
                        // not linked phyto
                        DetrNFrac = PSRD.PlantSink_To_Detr(ns) * Nut2Org_Refr + PSLD.PlantSink_To_Detr(ns) * Nut2Org_Lab;
                        if (NState == AllVariables.Ammonia)
                        {
                            DiffNFrac = PPl.N_2_Org() - DetrNFrac;
                        }
                        else
                        {
                            DiffNFrac = PPl.P_2_Org() - DetrNFrac;
                        }
                        NSink = NSink + PPl.Sedimentation() * DiffNFrac;
                    }
                }
            }
            return NSink;
        }
//         -------------------------------------------------------------------------------------------------------
        public double NutrRelColonization()  
        {
        // When organic matter is colonized from Refr--> Labile, nutrients
        // must be accounted for in case the nutrient to organic ratio differs between
        // the refractory and labile compartments
        double Nut2Org_Refr;
        double Nut2Org_Lab;
        double Nut2Org_DissRefr;
        double SumColonize;
        AllVariables ns;
        TDetritus PD;
        ReminRecord RR = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_Refr = RR.N2Org_Refr;
                Nut2Org_Lab = RR.N2OrgLab;
                Nut2Org_DissRefr = RR.N2OrgDissRefr;
            }
            else
            {
                Nut2Org_Refr = RR.P2Org_Refr;
                Nut2Org_Lab = RR.P2OrgLab;
                Nut2Org_DissRefr = RR.P2OrgDissRefr;
            }
            SumColonize = 0;
            for (ns = AllVariables.SedmRefrDetr; ns <= AllVariables.SuspLabDetr; ns++)
            {
                if ((ns== AllVariables.SedmRefrDetr) || (ns == AllVariables.SuspRefrDetr) || (ns == AllVariables.DissRefrDetr))
                {
                    PD = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
                    if (PD != null)
                    {
                        if (ns == AllVariables.DissRefrDetr) SumColonize = SumColonize + PD.Colonization() * (Nut2Org_DissRefr - Nut2Org_Lab);
                        else                                 SumColonize = SumColonize + PD.Colonization() * (Nut2Org_Refr - Nut2Org_Lab);
                    }
                }
            }
            return SumColonize;
        }

        public double NutrRelDefecation()  
        {
            double result;
            // When Defecation takes place in animals, excess nutrients are converted
            // into NH4.  Sedimented Detritus tends to have a lower fraction of nutrients then
            // the animals which are defecating organic matter.
            AllVariables ns;
            TAnimal PAn;
            double DetrNFrac;
            double DiffNFrac;
            double NDef;
            double Nut2Org_Refr;
            double Nut2Org_Lab;
            // NutrRelDefecation := 0;
            // If AQTSeg.Diagenesis_Included then exit;  6/6/08, procedure now relevant to diagenesis model
            ReminRecord _1 = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_Refr = _1.N2Org_Refr;
                Nut2Org_Lab = _1.N2OrgLab;
            }
            else
            {
                Nut2Org_Refr = _1.P2Org_Refr;
                Nut2Org_Lab = _1.P2OrgLab;
            }
            NDef = 0;
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    DetrNFrac = (1 - Consts.Def2SedLabDetr) * Nut2Org_Refr + Consts.Def2SedLabDetr * Nut2Org_Lab;
                    if (AQTSeg.Diagenesis_Included())
                    {
                        DetrNFrac = Nut2Org_Lab;
                    }
                    // 6/6/2008, diagenesis defecation has same nutrients as labile detritus
                    if (NState == AllVariables.Ammonia)
                          DiffNFrac = PAn.PAnimalData.N2Org - DetrNFrac;
                    else  DiffNFrac = PAn.PAnimalData.P2Org - DetrNFrac;

                    NDef = NDef + PAn.Defecation() * DiffNFrac;
                }
            }
            return NDef;
        }

       //  -------------------------------------------------------------------------------------------------------
        public double CalcAnimResp_Excr()  
        {
            // When Excretion (5/13/2013 and respiration) takes place in animals, _excess_ nutrients are converted
            // into NH4.  Dissolved Detritus tends to have a lower fraction of nutrients then
            // the animals which are excreting organic matter.
            AllVariables ns;
            TAnimal PAn;
            double DetrNFrac;
            double DiffNFrac;
            double Excret;
            double Nut2Org_DissRefr;
            double Nut2Org_DissLab;
            TDetritus PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            TDetritus PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            ReminRecord _1 = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_DissRefr = _1.N2OrgDissRefr;
                Nut2Org_DissLab = _1.N2OrgDissLab;
            }
            else
            {
                Nut2Org_DissRefr = _1.P2OrgDissRefr;
                Nut2Org_DissLab = _1.P2OrgDissLab;
            }
            Excret = 0;
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PAn = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    DetrNFrac = PDRD.Excr_To_Diss_Detr(ns) * Nut2Org_DissRefr + PDLD.Excr_To_Diss_Detr(ns) * Nut2Org_DissLab;
                    if (NState == AllVariables.Ammonia)
                    {
                        DiffNFrac = PAn.PAnimalData.N2Org - DetrNFrac;
                    }
                    else
                    {
                        DiffNFrac = PAn.PAnimalData.P2Org - DetrNFrac;
                    }
                    // was AnimExcretion
                    Excret = Excret + PAn.Respiration() * DiffNFrac;
                    // 5/13/2013
                }
            }
            return Excret;
        }

        // -------------------------------------------------------------------------------------------------------
        public double CalcPhotoResp()  
        {
            // When photorespiration takes place in plants, excess nutrients are converted
            // into NH4.  Dissolved Detritus tends to have a lower fraction of nutrients then
            // the plants which are going through photorespiration
            AllVariables ns;
            TDetritus PDRD;
            TDetritus PDLD;
            TPlant PPl;
            double DetrNFrac;
            double DiffNFrac;
            double PhotoRsp;
            double Nut2Org_DissRefr;
            double Nut2Org_DissLab;
            PDRD = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            PDLD = AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            ReminRecord _1 = Location.Remin;
            if (NState == AllVariables.Ammonia)
            {
                Nut2Org_DissRefr = _1.N2OrgDissRefr;
                Nut2Org_DissLab = _1.N2OrgDissLab;
            }
            else
            {
                Nut2Org_DissRefr = _1.P2OrgDissRefr;
                Nut2Org_DissLab = _1.P2OrgDissLab;
            }
            PhotoRsp = 0;
            for (ns = Consts.FirstPlant; ns <= Consts.LastPlant; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    DetrNFrac = PDRD.Excr_To_Diss_Detr(ns) * Nut2Org_DissRefr + PDLD.Excr_To_Diss_Detr(ns) * Nut2Org_DissLab;
                    if (NState == AllVariables.Ammonia)
                    {
                        DiffNFrac = PPl.N_2_Org() - DetrNFrac;
                    }
                    else
                    {
                        DiffNFrac = PPl.P_2_Org() - DetrNFrac;
                    }
                    PhotoRsp = PhotoRsp + PPl.PhotoResp() * DiffNFrac;
                }
            }
            return PhotoRsp;
        }

        // -------------------------------------------------------------------------------------------------------
        public double CalcDarkResp()  // When dark respiration occurs, excess nutrients are converted into NH4.
        {
            AllVariables ns;
            TPlant PPl;
            double Resp;
            double Nutr2Org;
            Resp = 0;
            for (ns = Consts.FirstPlant; ns <= Consts.LastPlant; ns++)
            {
                if (AQTSeg.GetStateVal(ns, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                {
                    PPl = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if (NState == AllVariables.Ammonia)   Nutr2Org = PPl.N_2_Org();
                    else                                  Nutr2Org = PPl.P_2_Org();
                    Resp = Resp + PPl.Respiration() * Nutr2Org;
                }
            }
            return Resp;
        }

        // -------------------------------------
        // assimilation of nutrient by algae
        // incl. ammonia preference factor of
        // Thomann and Fitzpatrick, 1982
        // used in WASP (Ambrose et al., 1991)
        // -------------------------------------
        public double Assimilation()  
        {
            double Assim;
            double SVN, SVA, NH4Pref = 0;
            const double N2NO3 = 0.23;
            const double N2NH4 = 0.78;


            void Assimilation_AddAssim(TStateVariable P)   // Assimilation of nutrients by algae    
            {
                TPlant PP;
                double UptkNut;
                const double UptakeCO2 = 0.53;
                UptkNut = 0;
                if ((P.IsPlant()))
                {
                    PP = ((P) as TPlant);
                    if (((NState == AllVariables.Ammonia) || (NState == AllVariables.Nitrate)) && (PP.IsFixingN())) return; // N-fixation, not assimilation from the water column
                    if ((NState == AllVariables.CO2) && PP.Is_Pcp_CaCO3()) return;    // 10-26-2007 Because plants are deriving C from the bicarbonate reaction, their photosynthesis does not result in a loss of CO2.

                    PlantRecord PAR = PP.PAlgalRec;
                    // JSC 9-25-2002, bryophytes assimilate nutrients
                    // JSC 10-21-2007, Free-floating macro. assimilate nutrients
                    if ((P.IsAlgae()) || (PAR.PlantType == "Bryophytes") || ((PAR.PlantType == "Macrophytes") && (PP.MacroType == TMacroType.Freefloat)))
                    {
                        if (NState != AllVariables.CO2)
                        {
                            //if ((NState == AllVariables.Ammonia) || (NState == AllVariables.Nitrate))   // FIXME Internal Nutrients
                            //     TNIP = AQTSeg.GetStatePointer(PP.NState, T_SVType.NIntrnl, T_SVLayer.WaterCol);
                            //else TNIP = AQTSeg.GetStatePointer(PP.NState, T_SVType.PIntrnl, T_SVLayer.WaterCol);

                            //if (AQTSeg.PSetupRec.Internal_Nutrients && (TNIP != null))
                            //{   // mg/L    // ug/L      // mg/ug
                            //    UptkNut = TNIP.Uptake() * 1e-3;
                            //}
                            //else
                            //{
                            // external nutrients

                            if ((NState == AllVariables.Ammonia) || (NState == AllVariables.Nitrate))
                                UptkNut = PP.N_2_Org() * PP.Photosynthesis();
                            else
                                UptkNut = PP.P_2_Org() * PP.Photosynthesis();
                            // mg/L     // g/g               // mg/L

                            if ((NState == AllVariables.Ammonia) || (NState == AllVariables.Nitrate))
                            {
                                if (((PAR.KN + SVA) * (PAR.KN + SVN)) != 0)
                                    NH4Pref = SVA * SVN / ((PAR.KN + SVA) * (PAR.KN + SVN)) + SVA * PAR.KN / ((SVA + SVN) * (PAR.KN + SVN));
                                else
                                    NH4Pref = 0;  // Protect Against Div by 0
                            }
                        }   // non CO2 code

                        switch (NState)
                        {
                            case AllVariables.Ammonia:
                                Assim = Assim + UptkNut * NH4Pref;
                                // total nutr assimilated = Sum(photosyn * uptake * proportion)
                                // g/cu m-d                     g/cu m-d  ~Redfield ratio unitless
                                break;

                            case AllVariables.Nitrate:
                                Assim = Assim + UptkNut * (1.0 - NH4Pref);
                                break;

                            case AllVariables.Phosphate:
                                Assim = Assim + UptkNut;  // mg/L
                                break;

                            case AllVariables.CO2:
                                Assim = Assim + PP.Photosynthesis() * UptakeCO2;
                                break;   // mg/L      // mg/L        // g/g

                        }   // switch
                    }   // if relevant plant type
                }      // if is plant          
            }         // AddAssim     

        

            Assim = 0.0;
            SVN = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol) * N2NO3;
            SVA = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) * N2NH4;

            if ((SVA > 0) || (SVN > 0))
                foreach (TStateVariable TSV in AQTSeg.SV) Assimilation_AddAssim(TSV);

            return Assim;
        }

    } // end TRemineralize

    public class TPO4Obj : TRemineralize
        {
        // Function AtmosDeposition : double ;
        public double FracAvail = 0;
        public double[] Alt_FracAvail = new double[3];
        public bool TP_IC = false;
        public bool TP_Inflow = false;
        public bool TP_PS = false;
        public bool TP_NPS = false;

        public Loadings.TLoadings Assimilation_Link = null;
        public Loadings.TLoadings Animal_Remin_Link = null;
        public Loadings.TLoadings Plant_Remin_Link = null;
        public Loadings.TLoadings OM_Remin_Link = null;
        public Loadings.TLoadings CalcitePcpt_Link = null;

        // Remineralization
        public double Remineralization()
            {
            double Remin;
            Remin = 0;

            if (OM_Remin_Link != null) Remin = Remin + OM_Remin_Link.ReturnLoad(AQTSeg.TPresent);
            if (Animal_Remin_Link != null) Remin = Remin + Animal_Remin_Link.ReturnLoad(AQTSeg.TPresent);
            if (Plant_Remin_Link != null) Remin = Remin + Plant_Remin_Link.ReturnLoad(AQTSeg.TPresent);

            Remin = Remin + SumDetrDecomp(T_SVType.PTrack, false);  
            // mg P/ L       // mg P/ L
                
            Remin = Remin + CalcPhotoResp();    
            Remin = Remin + CalcDarkResp();      
            Remin = Remin + CalcAnimResp_Excr();    
            Remin = Remin + CalcAnimPredn();        
            Remin = Remin + NutrRelDefecation();    
            Remin = Remin + NutrRelColonization();  
            Remin = Remin + NutrRelPlantSink();  
            Remin = Remin + NutrRelMortality();
            Remin = Remin + NutrRelGamLoss();       
            Remin = Remin + NutrRelPeriScr();    
            return Remin;
            }

            //Constructor  Init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
            public TPO4Obj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
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
                //Setup_Record _1 = AQTSeg.SetupRec;
                //if ((_1.SaveBRates || _1.ShowIntegration))
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
                //LoadInKg = Lo * _1.SegVol() * 1000.0 * 1e-6;
                //MBLR.BoundLoad[_1.DerivStep] = .BoundLoad[_1.DerivStep] + LoadInKg;
                // Load into modeled system

                //  LoadInKg = (Lo + WaI) * AQTSeg.SegVol() * 1000.0 * 1e-6;
                //// kg P       // mg P/L        // m3      // L/m3  // kg/mg 
                //{
                //    LoadInKg = (Lo + WaI + En) * _1.SegVol() * 1000.0 * 1e-6;
                //}
                //MBLR.TotOOSLoad[_1.DerivStep] = .TotOOSLoad[_1.DerivStep] + LoadInKg;
                //MBLR.LoadH2O[_1.DerivStep] = .LoadH2O[_1.DerivStep] + LoadInKg;
                //MBLossRecord _3 = _1.MBLossArray[AllVariables.Phosphate];

                //    MorphRecord Morph = AQTSeg.Location.Morph;
                //// * OOSDischFrac
                //LossInKg = WaO * AQTSeg.SegVol() * 1000.0 * 1e-6;
                //// 3/20/2014 remove OOSDischFrac
                //// kg P
                //// mg P/L
                //// m3
                //// L/m3
                //// kg/mg
                //_3.BoundLoss[_1.DerivStep] = _3.BoundLoss[_1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //if (En < 0)
                //{
                //    // *OOSDischFrac
                //    LossInKg = (-En + WaO) * _1.SegVol() * 1000.0 * 1e-6;
                //}
                //// 3/20/2014 remove OOSDischFrac
                //_3.TotalNLoss[_1.DerivStep] = _3.TotalNLoss[_1.DerivStep] + LossInKg;
                //_3.TotalWashout[_1.DerivStep] = _3.TotalWashout[_1.DerivStep] + LossInKg;
                //_3.WashoutH2O[_1.DerivStep] = _3.WashoutH2O[_1.DerivStep] + LossInKg;
                //LossInKg = CaCO3srb * _1.SegVol() * 1000.0 * 1e-6;
                //// kg P   // mg P/L          // m3    // L/m3  // kg/mg
                //_3.TotalNLoss[_1.DerivStep] = _3.TotalNLoss[_1.DerivStep] + LossInKg;
                //_3.BoundLoss[_1.DerivStep] = _3.BoundLoss[_1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //_3.CaCO3Sorb[_1.DerivStep] = _3.CaCO3Sorb[_1.DerivStep] + LossInKg;
                //MBLayerRecord _5 = _1.MBLayerArray[AllVariables.Phosphate];
                //LayerInKg = (TD + DiffUp + DiffDown) * _1.SegVol() * 1000.0 * 1e-6;
                //// kg N  // mg P/L                           // m3     // L/m3 // kg/mg
                //_5.NTurbDiff[_1.DerivStep] = _5.NTurbDiff[_1.DerivStep] + LayerInKg;
                //_5.NNetLayer[_1.DerivStep] = _5.NNetLayer[_1.DerivStep] + LayerInKg;

            }

            public override void Derivative(ref double DB)
            {
            double Lo =0;
            double Remin =0;
            double Assm =0;
            double WaO =0;
            double WaI=0;
            double TD =0 ;
            double DiffUp=0;
            double DiffDown=0;
            double En =0;
            double DiaFlx=0;
            double CaCO3srb=0;
            TPO4_Sediment PSed;

            // TrackMB
            // --------------------------------------------------
            // double PFluxKg;
            // PO4Obj.Derivative

            Lo = Loading;
            Remin = Remineralization();
            Assm = Assimilation();  
            if (Assimilation_Link != null) Assm = Assimilation_Link.ReturnLoad(AQTSeg.TPresent);

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

            PSed = (TPO4_Sediment) AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1);  // check for diagenesis model  
            if (PSed != null)
            {
                Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
                MorphRecord MR = AQTSeg.Location.Morph;
                DiaFlx = PSed.Flux2Water() * AQTSeg.DiagenesisVol(1) / MR.SegVolum;
              // mg/L d  // g/m3 sed1 d         // m3 sed1               // m3 water

                //_6.Diag_Track[TAddtlOutput.TSP_Diag, _6.DerivStep] = PSed.Flux2Water() * _7.H1.Val * 1e3;
                //// mg/m2 d                  //    // g/m3 sed d                //    // m                //    // mg/g

                // MBLayerRecord _9 = _6.MBLayerArray[AllVariables.Phosphate];
                // 3/16/2010 track total P flux from diagenesis into water column in kg since start of simulation
                // PFluxKg = DiaFlx * _6.SegVol() * 1000.0 * 1e-6;
                // kg     // mg/L        // m3     // L/m3   // kg/mg
                // _9.PFluxD[_6.DerivStep] = _9.PFluxD[_6.DerivStep] + PFluxKg;
            }

            ReminRecord RR = AQTSeg.Location.Remin;   
            CaCO3srb = RR.KD_P_Calcite * State * AQTSeg.CalcitePcpt() * 1e-6;
          // mg P/L d     // L/Kg       // mg/L  // mg Calcite/L d     // kg/mg

            if (CalcitePcpt_Link != null) CaCO3srb = CalcitePcpt_Link.ReturnLoad(AQTSeg.TPresent);  // JSON linkage

            DB = Lo + Remin - Assm - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx - CaCO3srb;
                Derivative_WriteRates();
                Derivative_TrackMB();
            }

        }

        public class TNH4Obj : TRemineralize
        {
        [JsonIgnore] public double PhotoResp = 0;
        [JsonIgnore] public double DarkResp = 0;
        [JsonIgnore] public double AnimExcr = 0;
        [JsonIgnore] public double AnimPredn = 0;
        [JsonIgnore] public double SvNutrRelColonization = 0;
        [JsonIgnore] public double SvNutrRelMortality = 0;
        [JsonIgnore] public double SvNutrRelGamLoss = 0;
        [JsonIgnore] public double SvNutrRelPeriScr = 0;
        [JsonIgnore] public double SvNutrRelPlantSink = 0;
        [JsonIgnore] public double SvNutrRelDefecation = 0;
        [JsonIgnore] public double SvSumDetrDecomp = 0;

        public Loadings.TLoadings Assimilation_Link = null;  // optional linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Animal_Remin_Link = null;
        public Loadings.TLoadings Plant_Remin_Link = null;
        public Loadings.TLoadings OM_Remin_Link = null;

        public TNH4Obj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            }

            // {mg N/ L}
            // ---------------------------------
            // nitrification & denitrification
            // ---------------------------------
            public double Nitrification()
            {
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
                    p = AQTSeg.pHCorr(7.0, 9.0);
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
                return Nitrify;
            }

            // real
            // denitrification
            // -------------------------------------------------------------------------------------------------------
            public double Remineralization()
            {
             double Remin;
             Remin = 0;

                PhotoResp = CalcPhotoResp();  // When photorespiration takes place in plants, excess nutrients are converted into NH4  
                DarkResp = CalcDarkResp();     // When dark respiration occurs, excess nutrients are converted into NH4. 

                AnimExcr = CalcAnimResp_Excr(); // direct respiration losses plus excess nutrient losses from excretion to detritus
                AnimPredn = CalcAnimPredn();     // When predation occurs, differences in nutrients are refelcted in W.C.

                SvNutrRelColonization = NutrRelColonization();  
                SvNutrRelMortality = NutrRelMortality(); 
                SvNutrRelGamLoss = NutrRelGamLoss();  
                SvNutrRelPeriScr = NutrRelPeriScr();   
                SvNutrRelPlantSink = NutrRelPlantSink();
                SvNutrRelDefecation = NutrRelDefecation();  

            SvSumDetrDecomp = SumDetrDecomp(T_SVType.NTrack, false);

            if (OM_Remin_Link != null) Remin = Remin + OM_Remin_Link.ReturnLoad(AQTSeg.TPresent);
            if (Animal_Remin_Link != null) Remin = Remin + Animal_Remin_Link.ReturnLoad(AQTSeg.TPresent);
            if (Plant_Remin_Link != null) Remin = Remin + Plant_Remin_Link.ReturnLoad(AQTSeg.TPresent);

            Remin = Remin + PhotoResp + DarkResp + AnimExcr + AnimPredn + SvNutrRelColonization + SvNutrRelMortality + SvNutrRelGamLoss + SvNutrRelPeriScr + SvNutrRelPlantSink + SvNutrRelDefecation + SvSumDetrDecomp;
            // mg N/ L   // mg N/ L

             return Remin;
            }

            // --------------------------------------------------
            public void Derivative_WriteRates()
            {
                //Setup_Record _1 = AQTSeg.SetupRec;
                //if ((_1.SaveBRates || _1.ShowIntegration))
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

                //MBLoadRecord  = _1.MBLoadArray[AllVariables.Nitrate];
                //LoadInKg = Lo * _1.SegVol() * 1000.0 * 1e-6;
                //.BoundLoad[_1.DerivStep] = .BoundLoad[_1.DerivStep] + LoadInKg;
                //// Load into modeled system
                //LoadInKg = (Lo + WaI) * _1.SegVol() * 1000.0 * 1e-6;
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //if (En > 0)
                //{
                //    LoadInKg = (Lo + WaI + En) * _1.SegVol() * 1000.0 * 1e-6;
                //}
                //.TotOOSLoad[_1.DerivStep] = .TotOOSLoad[_1.DerivStep] + LoadInKg;
                //.LoadH2O[_1.DerivStep] = .LoadH2O[_1.DerivStep] + LoadInKg;
                //MBLossRecord _3 = _1.MBLossArray[AllVariables.Nitrate];
                //MorphRecord _4 = _1.Location.Morph;
                //// * OOSDischFrac
                //LossInKg = WaO * _1.SegVol() * 1000.0 * 1e-6;
                //// 3/20/2014 remove OOSDischFrac
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //_3.BoundLoss[_1.DerivStep] = _3.BoundLoss[_1.DerivStep] + LossInKg;
                //// Loss from the modeled system
                //if (En < 0)
                //{
                //    // * OOSDischFrac
                //    LossInKg = (-En + WaO) * _1.SegVol() * 1000.0 * 1e-6;
                //}
                //// 3/20/2014 remove OOSDischFrac
                //_3.TotalNLoss[_1.DerivStep] = _3.TotalNLoss[_1.DerivStep] + LossInKg;
                //_3.TotalWashout[_1.DerivStep] = _3.TotalWashout[_1.DerivStep] + LossInKg;
                //_3.WashoutH2O[_1.DerivStep] = _3.WashoutH2O[_1.DerivStep] + LossInKg;
                //MBLayerRecord _5 = _1.MBLayerArray[AllVariables.Nitrate];
                //LayerInKg = (TD + DiffUp + DiffDown) * _1.SegVol() * 1000.0 * 1e-6;
                //// kg N
                //// mg N/L
                //// m3
                //// L/m3
                //// kg/mg
                //_5.NTurbDiff[_1.DerivStep] = _5.NTurbDiff[_1.DerivStep] + LayerInKg;
                //_5.NNetLayer[_1.DerivStep] = _5.NNetLayer[_1.DerivStep] + LayerInKg;
                //// with

            }

            public override void Derivative(ref double DB)
            {
                double Lo=0;
                double Re=0;
                double Ni=0;
                double Assm=0;
                double WaO=0;
                double WaI=0;
                double TD=0;
                double DiffUp=0;
                double DiffDown=0;
                double En=0;
                double DiaFlx=0;
                TNH4_Sediment NSed;

            // TrackMB
            // --------------------------------------------------
            // NH4Obj.Derivative

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

                Assm = Assimilation(); 
                if (Assimilation_Link != null) Assm = Assimilation_Link.ReturnLoad(AQTSeg.TPresent);

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

            NSed = (TNH4_Sediment)AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1); // search for diagenesis model 
            if (NSed != null)
            {
                Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
                MorphRecord MR = AQTSeg.Location.Morph;
                DiaFlx = NSed.Flux2Water() * AQTSeg.DiagenesisVol(1) / MR.SegVolum;
                // mg/L d  // g/m3 sed1 d /    // m3 sed1               // m3 water
                // _6.Diag_Track[TAddtlOutput.NH3_Diag, _6.DerivStep] = NSed.Flux2Water() * _7.H1.Val * 1e3;
            }  // mg/m2 d            // g/m3 sed d            // m            // mg/g


            //// if (State < VSmall) and ((Lo + Re + TD + DiffUp + DiffDown + En + DiaFlx) < VSmall)  commented out in AQUATOX v3.2 code
            //// then  begin dB := 0.0; Lo:=0;Re:=0;Ni:=0;Assm:=0;WaO:=0;WaI:=0;TD:=0;DiffUp:=0;DiffDown:=0;En:=0;DiaFlx:=0; End
            //// else

            DB = Lo + Re - Ni - Assm - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx;
                Derivative_WriteRates();
                Derivative_TrackMB();
            }

        } // end TNH4Obj


        public class TNO3Obj : TRemineralize
        {
        public bool TN_IC = false;
        public bool TN_Inflow = false;
        public bool TN_PS = false;
        public bool TN_NPS = false;

        public Loadings.TLoadings Assimilation_Link = null;  // optional linkage from JSON if plants, animals, or OM not modeled

        public TNO3Obj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            }

            // -------------------------------------------------------------------------------------------------------
            public double Denitrification()
            {
                // , Sed_Surf_Corr
                double T, O2;
                double p;
                double DOCorr;
                double Denitrify;
                double EnvironLim;
                if (State > Consts.VSmall)
                {
                    ReminRecord _1 = Location.Remin;
                    T = AQTSeg.TCorr(2.0, 10.0, 30.0, 60.0);
                    p = AQTSeg.pHCorr(5.0, 9.0);
                    if (Location.SiteType == SiteTypes.Marine)
                    {
                        // 5/6/2013
                        DOCorr = 0.0;
                    }
                    else
                    {
                    O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
                    DOCorr =  O2 / (0.5 + O2);
                    }
                    EnvironLim = (1.0 - DOCorr) * T * p;
                    Denitrify = (_1.KDenitri_Wat * EnvironLim) * State;
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
                return Denitrify;
            }

            // --------------------------------------------------
            public void Derivative_WriteRates()
            {
                //Setup_Record _1 = AQTSeg.SetupRec;  // FIXME Output Rates
                //if ((_1.SaveBRates || _1.ShowIntegration))
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
        //    TStates _1 = AQTSeg;
        //    MBLoadRecord  = _1.MBLoadArray[AllVariables.Nitrate];
        //    LoadInKg = Lo * _1.SegVol() * 1000.0 * 1e-6;
        //    .BoundLoad[_1.DerivStep] = .BoundLoad[_1.DerivStep] + LoadInKg;
        //    // Load into modeled system
        //    LoadInKg = (Lo + WaI) * _1.SegVol() * 1000.0 * 1e-6;
        //    // kg N
        //    // mg N/L
        //    // m3
        //    // L/m3
        //    // kg/mg
        //    if ((En > 0))
        //    {
        //        LoadInKg = (Lo + WaI + En) * _1.SegVol() * 1000.0 * 1e-6;
        //    }
        //    .TotOOSLoad[_1.DerivStep] = .TotOOSLoad[_1.DerivStep] + LoadInKg;
        //    .LoadH2O[_1.DerivStep] = .LoadH2O[_1.DerivStep] + LoadInKg;
        //    MBLossRecord _3 = _1.MBLossArray[AllVariables.Nitrate];
        //    MorphRecord _4 = _1.Location.Morph;
        //    // * OOSDischFrac
        //    LossInKg = WaO * _1.SegVol() * 1000.0 * 1e-6;
        //    // 3/20/2014 remove OOSDischFrac
        //    // kg N
        //    // mg N/L
        //    // m3
        //    // L/m3
        //    // kg/mg
        //    _3.BoundLoss[_1.DerivStep] = _3.BoundLoss[_1.DerivStep] + LossInKg;
        //    // Loss from the modeled system
        //    if (En < 0)
        //    {
        //        // * OOSDischFrac
        //        LossInKg = (-En + WaO) * _1.SegVol() * 1000.0 * 1e-6;
        //    }
        //    // 3/20/2014 remove OOSDischFrac
        //    _3.TotalNLoss[_1.DerivStep] = _3.TotalNLoss[_1.DerivStep] + LossInKg;
        //    _3.TotalWashout[_1.DerivStep] = _3.TotalWashout[_1.DerivStep] + LossInKg;
        //    _3.WashoutH2O[_1.DerivStep] = _3.WashoutH2O[_1.DerivStep] + LossInKg;
        //    LossInKg = Denitr * _1.SegVol() * 1000.0 * 1e-6;
        //    // kg N
        //    // mg N/L
        //    // m3
        //    // L/m3
        //    // kg/mg
        //    _3.TotalNLoss[_1.DerivStep] = _3.TotalNLoss[_1.DerivStep] + LossInKg;
        //    _3.BoundLoss[_1.DerivStep] = _3.BoundLoss[_1.DerivStep] + LossInKg;
        //    // Loss from the modeled system
        //    _3.Denitrify[_1.DerivStep] = _3.Denitrify[_1.DerivStep] + LossInKg;
        //    MBLayerRecord _5 = _1.MBLayerArray[AllVariables.Nitrate];
        //    LayerInKg = (TD + DiffUp + DiffDown) * _1.SegVol() * 1000.0 * 1e-6;
        //    // kg N
        //    // mg N/L
        //    // m3
        //    // L/m3
        //    // kg/mg
        //    _5.NTurbDiff[_1.DerivStep] = _5.NTurbDiff[_1.DerivStep] + LayerInKg;
        //    _5.NNetLayer[_1.DerivStep] = _5.NNetLayer[_1.DerivStep] + LayerInKg;
        //    // with

        //}

        // real
        public override void Derivative(ref double DB)
        {
            TNH4Obj P;
                double Lo =0;
                double Nitr =0;
                double Denitr = 0;
                double NO3Assim = 0;
                double WaO = 0;
                double WaI = 0;
                double TD = 0;
                double DiffUp = 0;
                double DiffDown = 0;
                double En = 0;
                double DiaFlx = 0;
                TNO3_Sediment N2Sed;

            // TrackMB
            // --------------------------------------------------
            // NO3Obj.Derivative

            P = ((AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol)) as TNH4Obj);
            Lo = Loading;
            Nitr = P.Nitrification();
            Denitr = Denitrification();
            NO3Assim = Assimilation();   
            if (Assimilation_Link != null) NO3Assim = Assimilation_Link.ReturnLoad(AQTSeg.TPresent);

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

            N2Sed = (TNO3_Sediment)AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            if (N2Sed != null)  
            {
                MorphRecord MR = AQTSeg.Location.Morph;
                DiaFlx = N2Sed.Flux2Water() * AQTSeg.DiagenesisVol(1) / MR.SegVolum;
                // mg/L d    // g/m3 sed1 d           // m3 sed1                 // m3 water

                //_6.Diag_Track[TAddtlOutput.No3_Diag, _6.DerivStep] = N2Sed.Flux2Water() * _7.H1.Val * 1e3;
            }    //mg / m2 d       // g/m3 sed d        // m        // mg/g


//            If(State < VSmall) and((Lo + Nitr + TD + DiffUp + DiffDown + En + DiaFlx) < VSmall)    commented out in AQUATOX 3.2 code
//                 then Begin dB:= 0.0; Lo:= 0; Nitr:= 0; Denitr:= 0; NO3Assim:= 0; WaO:= 0; WaI:= 0; TD:= 0; DiffUp:= 0; DiffDown:= 0; En:= 0; DiaFlx:= 0; End
//                 else   
            
                DB = Lo + Nitr - Denitr - NO3Assim - WaO + WaI + TD + DiffUp + DiffDown + En + DiaFlx;

                // Derivative_WriteRates();
                // Derivative_TrackMB();  // FIXME RATES TRACKING
            }

        } // TNO3Obj



    public class TCO2Obj : TRemineralize
    {
        public bool ImportCo2Equil = false;
        public Loadings.TLoadings CO2Equil = null;

        public Loadings.TLoadings Assimilation_Link = null;
        public Loadings.TLoadings Respiration_Link = null;
        public Loadings.TLoadings OM_Decomp_Link = null;

        // --------------------------------
        // CO2 equilibrim as fn temp
        // Bowie et al., 1985
        // --------------------------------
        public double AtmosExch_CO2Sat()
        {
            double TKelvin;
            double CO2Henry;               // Henry's Law constant for CO2
            const double pCO2 = 0.00035;   // atmos. partial pressure for CO2
            // make variable?  This number is rising
            const double MCO2 = 44000.0;
            if (ImportCo2Equil)
            {
                // Import CO2Equil from CO2SYS or equivalent
                return CO2Equil.ReturnTSLoad(AQTSeg.TPresent);
            }
            else
            {
                // AQUATOX Calculated CO2Equil
                TKelvin = 273.15 + AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
                CO2Henry = MCO2 * Math.Pow(10.0, (2385.73 / TKelvin - 14.0184 + 0.0152642 * TKelvin));
                // g/cu m atm  mg CO2/mole
                return CO2Henry * pCO2;
                // g/cu m   g/cu m-atm atm
            }
        }

        // real
        // reaeration
        // ----------------------------------------------------------
        // atmos. exchange of carbon dioxide,Scwarzenbach et al.'93
        // assuming that reaeration affects only epilimnion
        // ----------------------------------------------------------
        public double AtmosExch()
        {
            // temp adjustment, Churchill et al., 1962
            const double MolWtCO2 = 44.0;
            const double MolWtO2 = 32.0;
            double KLiqCO2;
            TO2Obj P;
            // atmosexch
            //            if (AQTSeg.VSeg == VerticalSegments.Hypolimnion)   return 0;    

            P = ((AQTSeg.GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol)) as TO2Obj);

            KLiqCO2 = P.KReaer() * Math.Pow((MolWtO2 / MolWtCO2), 0.25);              // Schwarzenbach et al., 1993:
            // 1/d        1/d
            return KLiqCO2 * (AtmosExch_CO2Sat() - State);
            // g/cu m-d    1/d        g/cu m  g/cu m
        }     // atmosexch

        // Winberg
        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record _1 = AQTSeg.SetupRec;
        //    if ((_1.SaveBRates || _1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", Lo);
        //        SaveRate("DetDecmp", De);
        //        SaveRate("Respiration", Re);
        //        SaveRate("CO2Assim", CO2Assim);
        //        SaveRate("AtmosEx", AE);
        //        SaveRate("Washout", WaO);
        //        SaveRate("WashIn", WaI);
        //        if (!AQTSeg.LinkedMode)
        //        {
        //            SaveRate("TurbDiff", TD);
        //        }
        //        else
        //        {
        //            // If Not AQTSeg.CascadeRunning
        //            // then
        //            SaveRate("DiffUp", DiffUp);
        //            SaveRate("DiffDown", DiffDown);
        //        }
        //        if (AQTSeg.EstuarySegment)
        //        {
        //            SaveRate("Entrainment", En);
        //        }
        //        SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            TO2Obj O2P;
            double CO2Assim;
            double Re;
            double De;
            double TD;
            double DiffUp;
            double DiffDown;
            double Lo;
            double AE;
            double WaO;
            double WaI;
            double En;
            const double CO2Biomass = 0.526;
            // --------------------------------------------------
            // CO2Obj.Derivative
            CO2Assim = 0;
            Re = 0;
            De = 0;
            TD = 0;
            DiffUp = 0;
            DiffDown = 0;
            Lo = 0;
            AE = 0;
            WaO = 0;
            WaI = 0;
            En = 0;
            ReminRecord RR = Location.Remin;
            O2P = ((AQTSeg.GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol)) as TO2Obj);
            Lo = Loading;

            De = CO2Biomass * SumDetrDecomp(T_SVType.StV, false);
            if (OM_Decomp_Link != null) De = OM_Decomp_Link.ReturnLoad(AQTSeg.TPresent);

            Re = CO2Biomass * O2P.SumRespiration(false);   
            if (Respiration_Link != null) Re = Respiration_Link.ReturnLoad(AQTSeg.TPresent);

            CO2Assim = Assimilation(); 
            if (Assimilation_Link != null) CO2Assim = Assimilation_Link.ReturnLoad(AQTSeg.TPresent);

            AE = AtmosExch();
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
            //if (AQTSeg.EstuarySegment)o
            //{
            //    En = EstuaryEntrainment();
            //}
            DB = Lo + Re + De - CO2Assim + AE - WaO + WaI + TD + DiffUp + DiffDown + En;

            //Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
        public TCO2Obj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            ImportCo2Equil = false;
            CO2Equil = new Loadings.TLoadings();
        }
    }

    public class TO2Obj : TRemineralize
    {
        //public double Threshhold = 0;
        //public bool CalcDuration = false;
        public bool NoLoadOrWash = false;

        public Loadings.TLoadings Photosynthesis_Link = null;
        public Loadings.TLoadings Respiration_Link = null;
        public Loadings.TLoadings Nitrification_Link = null;
        public Loadings.TLoadings CBOD_Link = null;  
        public Loadings.TLoadings SOD_Link = null;  // if nutrients not included in model

        // -------------------------------------------------------------------------------------------------------
        //Constructor  init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
        public TO2Obj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            //CalcDuration = false;
            //Threshhold = 1.0;
            NoLoadOrWash = false;
            // TRemineralize

        }
        //// temp adjustment, Churchill et al., 1962
        //public void KReaer_Estuarine_Reaeration()
        //{
        //    // Banks
        //    double Thick;
        //    double Velocity;
        //    double Wind;

        //    Velocity = AQTSeg.Velocity(AQTSeg.Location.Locale.PctRiffle, AQTSeg.Location.Locale.PctPool, false) / 100;         // For Estuary Velocity, Riffle, Pool parameters irrelevant
        //    // m/s          // cm/s                                             // m/s
        //    Wind = GetState(AllVariables.WindLoading, T_SVType.StV, T_SVLayer.WaterCol);
        //    Thick = Location.MeanThick[VerticalSegments.Epilimnion];
        //    KReaer() = 3.93 * Math.Sqrt(Velocity) / Math.Pow(Thick, 1.5) + (0.728 * Math.Sqrt(Wind) - 0.317 * Wind + 0.0372 * Math.Pow(Wind, 2)) / Thick;
        //}

        // ----------------------------------------------------------
        // reaeration
        // assuming that reaeration affects only epilimnion
        // 
        // updated 7/99  corrected WASP4 error and included
        // refinements for very shallow streams
        // ----------------------------------------------------------
        public double KReaer()
        {
            double result;
            double KReaer1, KReaer2, Vel, TransitionDepth;
            double Wnd;
            double OxygenTransfer;
            double ZDepth;
            double H, U;   // depth and discharge in English units
            const double Theta = 1.024;
            double Temp = (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol));
            if ( Temp < AQTSeg.Ice_Cover_Temp())  // (AQTSeg.VSeg == VerticalSegments.Hypolimnion) || 
                result = 0.0;
            else if (!Location.Locale.UseCovar)  result = Location.Locale.EnteredKReaer;
            else
            {
                // (* or (Location.SiteType in [Lake,Reservr1D])  {JSC Test on Reserv, Lake 8/18/08} *)
                //if ((AQTSeg.EstuarySegment) || (Location.SiteType == SiteTypes.Marine))
                //{
                //    KReaer_Estuarine_Reaeration();
                //    return result;
                //}
                ZDepth = Location.MeanThick;  // [VerticalSegments.Epilimnion]
                Vel = AQTSeg.Velocity(AQTSeg.Location.Locale.PctRiffle, AQTSeg.Location.Locale.PctPool, false) * 0.01;
                // m/s         // cm/s                                                                        // m/cm
                if ((!(Location.SiteType == SiteTypes.Stream)))  { Vel = 0; }           // no velocity reaeration for nonstreams

                TWindLoading TWind = (TWindLoading) AQTSeg.GetStatePointer(AllVariables.WindLoading, T_SVType.StV, T_SVLayer.WaterCol);
                if (TWind == null) Wnd = 0.1;
                else Wnd = TWind.State;  // m/s at 10 m

                // Schwarzenbach et al., 1993, coverted to m/sec:
                KReaer1 = ((4E-4 + 4E-5 * Wnd * Wnd) * 864) / ZDepth;
                // 1/d                        m/sec        m
                if (ZDepth < 0.06)
                {
                    // Krenkel and Orlob 1962 for shallow flume
                    U = Vel * 3.2808;
                    // m/s -> fps
                    H = ZDepth * 3.2808;
                    // m -> ft
                    KReaer2 = (Math.Pow((U * Location.Locale.Channel_Slope), 0.408)) / Math.Pow(H, 0.66);
                }
                else
                {
                    // Covar, 1978; Ambrose et al., 1991:
                    if (Vel < 0.518)
                    {   TransitionDepth = 0.0;     }
                    else
                    {   TransitionDepth = 4.411 * Math.Pow(Vel, 2.9135);  }
                    if (ZDepth < 0.61)
                    {
                        // Owens
                        KReaer2 = 5.349 * Math.Pow(Vel, 0.67) * Math.Pow(ZDepth, -1.85);
                    }
                    else if (ZDepth > TransitionDepth)
                    {
                        // O'Connor
                        KReaer2 = 3.93 * Math.Pow(Vel, 0.50) * Math.Pow(ZDepth, -1.50);
                    }
                    else
                    {
                        KReaer2 = 5.049 * Math.Pow(Vel, 0.97) * Math.Pow(ZDepth, -1.67);
                    }
                    // Churchill
                }
                // Covar
                OxygenTransfer = Math.Max(KReaer1, KReaer2);
                if (OxygenTransfer > 24)
                {
                    OxygenTransfer = 24;
                }
                OxygenTransfer = OxygenTransfer * Math.Pow(Theta, Temp - 20);
                result = OxygenTransfer;
            }

            return result;
        }

        // ----------------------------------
        // oxygen saturation as fn temp
        // & salinity  Bowie et al., 1985
        // ----------------------------------
        public double Reaeration_O2Sat()
        {
            double TKelvin;
            double Salt;
            double lnCsf;
            double lnCss;
            double AltEffect;
            TSalinity TSV = AQTSeg.GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) as TSalinity;
            if (TSV == null) Salt = 0; else Salt = TSV.State;

            TKelvin = 273.15 + AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            lnCsf = -139.34411 + (1.575701E5 / TKelvin) - 6.642308E7 / Math.Pow(TKelvin, 2) + 1.243800E10 / Math.Pow(TKelvin, 3.0) - 8.621949E11 / Math.Pow(TKelvin, 4.0);
            if (Salt > 0)
            {
                lnCss = lnCsf - Salt * (0.017674 - (10.754 / TKelvin) + 2140.7 / Math.Pow(TKelvin, 2));
            }
            else
            {
                lnCss = lnCsf;
            }
            AltEffect = (100 - (0.0035 * 3.28083 * Location.Locale.Altitude)) / 100;
            // Fractional effect due to altitude from Zison et al. 1978
            // m
            return Math.Exp(lnCss) * AltEffect;
            // 8/19/2008, Changed to APHA code as in Thomann & Mueller
            // (*       O2Sat := 1.4277 * exp(-173.492 + 24963.39/TKelvin + 143.3483
            // * ln(TKelvin/100.0) - 0.218492 * TKelvin
            // + Salt * (-0.033096 + 0.00014259 * TKelvin - 1.7e-7 * SQR(TKelvin)));  {JSC 8/18/2008, Change from SQRT to SQR}  *)
        }

        // KReaer
        // -------------------------------------------------------------------------------------------------------
        public double Reaeration()
            // reaeration is additive to the derivative therefore
            // a positive number is oxygen from the air to the WC
            // a negative number is oxygen from the WC to the air
        {
            double result;
            //const double MPH2MPS = 0.447;
            //const double Theta = 1.024;
            // temp adjustment, Churchill et al., 1962
            // MolWt = 44.0;
            double BlG;
            double Other;
            double ZDepth;
            double ZZDepth;
            AllVariables AlgLoop;
            // ---------------------------------
            double O2S;
            // Reaeration

            ZDepth = Location.MeanThick; // [VerticalSegments.Epilimnion];
            BlG = 0;  // count cyanobacteria(blue-greens) biomass
            for (AlgLoop = Consts.FirstBlGreen; AlgLoop <= Consts.LastBlGreen; AlgLoop++) 
            {
                double GSV = AQTSeg.GetStateVal(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (GSV > 0) BlG = BlG + GSV;
            }
            Other = 0; //   count other algae biomass, excluding macrophytes
            for (AlgLoop = Consts.FirstDiatom; AlgLoop <= Consts.LastGreens; AlgLoop++)
            {
                double GSV = AQTSeg.GetStateVal(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (GSV > 0) Other = Other + GSV;
            }
            for (AlgLoop = AllVariables.OtherAlg1; AlgLoop <= AllVariables.OtherAlg2; AlgLoop++)
            {
                double GSV = AQTSeg.GetStateVal(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (GSV > 0) Other = Other + GSV;
            }
            if ((BlG > 1.0) && (BlG > Other))  ZZDepth = 0.25;              // 10-15-2001, Modificaiton to account for Cyanobacteria Blooms
            else  ZZDepth = ZDepth;

            if (ZZDepth > ZDepth)  ZZDepth = ZDepth;  // bullet-proof

            O2S = Reaeration_O2Sat();
            if (ZZDepth < Globals.Consts.Tiny) result = 0;
            else result = KReaer() * (O2S - State) * ZDepth / ZZDepth;
               // mg/L d     1/d       mg/L   mg/L  correct for bl-gr bloom

            if (State > (O2S * 2))                // 10/27/2010  Oxygen is limited to twice saturation in the event of
                if ((result > (O2S * 2) - State)) // ice cover or hypolimnion, extra goes to reaeration in this special case
                {                                 // 5/7/2012 refined logic so that reaeration is not inadvertently _reduced_ by this code (fix of 6-13-2011 bug)
                    result = (O2S * 2) - State;
                }

            return result;
        }

        // ---------------------------------------------------------------------------------
        public double SumPhotosynthesis()
        {
            double Add;

            void SumPhotosynthesis_AddPhoto(TStateVariable P)
            {
                if (P.IsPlant()) Add = Add + ((P) as TPlant).Photosynthesis();
            }

            Add = 0.0;
            foreach (TStateVariable TSV in AQTSeg.SV) SumPhotosynthesis_AddPhoto(TSV);
            return Add;
        }
        // ---------------------------------------------------------------------------------

        public double SumRespiration(bool PlantOnly)
        {
            double Add;

            void AddResp(TStateVariable P)
            {
                if ((P.IsPlant()) || (!PlantOnly && P.IsPlantOrAnimal()))
                    Add = Add + ((P) as TOrganism).Respiration();
            }

            Add = 0.0;
            foreach (TStateVariable TSV in AQTSeg.SV) AddResp(TSV);
            return Add;
        }
        // ---------------------------------------------------------------------------------

        //public void Derivative_WriteRates()
        //{
        //    Setup_Record _1 = AQTSeg.SetupRec;
        //    if ((_1.SaveBRates || _1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", Lo);
        //        if (!AQTSeg.LinkedMode)
        //        {
        //            SaveRate("TurbDiff", TD);
        //        }
        //        else
        //        {
        //            // If Not AQTSeg.CascadeRunning
        //            // then
        //            SaveRate("DiffUp", DiffUp);
        //            SaveRate("DiffDown", DiffDown);
        //        }
        //        SaveRate("Photosyn", Pho);
        //        SaveRate("Reaer", Reae);
        //        SaveRate("CBOD", BOD);
        //        SaveRate("Respiration", Resp);
        //        SaveRate("Nitrific", Nitr);
        //        SaveRate("Washout", WaO);
        //        SaveRate("WashIn", WaI);
        //        SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
        //        if (AQTSeg.EstuarySegment)
        //        {
        //            SaveRate("Entrainment", En);
        //        }
        //        if (AQTSeg.Diagenesis_Included())
        //        {
        //            SaveRate("SOC", SOC2);
        //        }
        //    }
        //}

        // --------------------------------------------------
        public void Derivative_SetAnoxicVar()
        {
            AQTSeg.Anoxic = (State < 0.25);

            //TStates _1 = AQTSeg;
            //if (_1.Stratified)
            //{
            //    if (_1.VSeg == VerticalSegments.Hypolimnion)
            //    {
            //        OtherSegment = _1.EpiSegment;
            //    }
            //    else
            //    {
            //        OtherSegment = _1.HypoSegment;
            //    }
            //}
            //if ((State < 0.25) || (_1.Stratified && (OtherSegment.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol) < 0.25)))
            //{
            //    _1.Anoxic = true;
            //}
            //else
            //{
            //    _1.Anoxic = false;
            //}
            //if (_1.Stratified)
            //{
            //    OtherSegment.Anoxic = _1.Anoxic;
            //}
        }

        public override void Derivative(ref double DB)
        {
            const double O2Photo = 1.6;
            // see Bowie et al., 1985 for numerous references
            double Lo = 0;
            double TD = 0;
            double DiffUp = 0;
            double DiffDown = 0;
            double Pho = 0;
            double Reae = 0;
            double Resp = 0;
            double BOD = 0;
            double SOD2 = 0;
            double Nitr = 0;
            double WaO = 0;
            double WaI = 0;
            double En = 0;
            //double DarkResp = 0;
            //            TStates OtherSegment;
            // --------------------------------------------------
            // TO2Obj.Deriv

            ReminRecord RR = Location.Remin;
            BOD = RR.O2Biomass * SumDetrDecomp(T_SVType.StV, false);
            if (CBOD_Link != null) BOD = CBOD_Link.ReturnLoad(AQTSeg.TPresent);

            // if diagenesis model attached
            if (AQTSeg.Diagenesis_Included())
            {
                MorphRecord MR = AQTSeg.Location.Morph;
                Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
                SOD2 = AQTSeg.SOD * (AQTSeg.DiagenesisVol(2) / DR.H2.Val) / MR.SegVolum;
            } // g/m3 w d   g O2/m2 s d         // m3 s            // m s        // m3

            if (SOD_Link != null) SOD2 = SOD_Link.ReturnLoad(AQTSeg.TPresent);  // linkage if no diagenesis attached

            Resp = RR.O2Biomass * SumRespiration(false);            
            if (Respiration_Link != null) Resp = Respiration_Link.ReturnLoad(AQTSeg.TPresent);

            TNH4Obj PNH4  = (TNH4Obj)(AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol));
            if (PNH4 != null) Nitr = RR.O2N * PNH4.Nitrification();
               else Nitr = 0;

            if (Nitrification_Link != null) Nitr = Nitrification_Link.ReturnLoad(AQTSeg.TPresent);

    //      AQTSeg.TOTResp[AQTSeg.DerivStep] = (Resp + BOD + SOD2 + Nitr) * AQTSeg.SegVol() / AQTSeg.Location.Locale.SurfArea;
            // g O2/m2 d                              // g/m3 d                    // m3                              // m2

            Lo = Loading;

            Pho = O2Photo * SumPhotosynthesis();  
            // mg O2/L =  o2/photo bio. * mg biomass / L
            if (Photosynthesis_Link != null) Pho = Photosynthesis_Link.ReturnLoad(AQTSeg.TPresent);

            // AQTSeg.GPP[AQTSeg.DerivStep] = Pho * AQTSeg.SegVol() / AQTSeg.Location.Locale.SurfArea;    // FIXME calculation of GPP and NPP disabled for now
            // g O2/m2 d            // g/m3 d            // m3            // m2
            // DarkResp = .O2Biomass * SumRespiration(true);  
            // AQTSeg.NPP[.DerivStep] = (Pho - DarkResp) * AQTSeg.SegVol() / AQTSeg.Location.Locale.SurfArea;
            // g O2/m2 d            // g/m3 d  // m3           // m2

            Reae = Reaeration();
            WaO = Washout();

        //if (AQTSeg.LinkedMode)  WaI = Washin();
        //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
        //{   DiffUp = SegmentDiffusion(true);
        //    DiffDown = SegmentDiffusion(false);
        //}
        //else if ((!AQTSeg.LinkedMode)) TD = TurbDiff();
        //if (AQTSeg.EstuarySegment)  En = EstuaryEntrainment();

        if (NoLoadOrWash)
        {
            Lo = 0;
            WaO = 0;
            WaI = 0;
        }
        DB = Lo + Reae + Pho - BOD - SOD2 - Resp - Nitr - WaO + WaI + TD + DiffUp + DiffDown + En;
//          Derivative_WriteRates();
        Derivative_SetAnoxicVar();
        }


    } // end TO2Obj


}