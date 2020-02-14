using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.OrgMatter;
using AQUATOX.Organisms;
using AQUATOX.Plants;
using AQUATOX.Animals;
using AQUATOX.Nutrients;
using AQUATOX.Diagenesis;
using AQUATOX.Chemicals;
using Newtonsoft.Json;
using Globals;
using System.Linq;

namespace AQUATOX.Bioaccumulation
{
    public class TSuspSedimentTox : TToxics
    {

        public TSuspSedimentTox(AllVariables Ns, AllVariables Carry, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, Carry, SVT, L, aName, P, IC)
        { }

        // --------------------------------------------------------------------------------------------------------------
        // *************************************
        // *      TSuspSedimentTOX OBJECT      *
        // *************************************
        public override void Derivative(ref double DB)
        {
            double ScourTox=0;
            double DeposTox=0;
            double Sorpt =0;
            double Desorpt =0;
            // TBottomSediment TopLayer;
            TDetritus CP;
            double WashO=0;
            double WashI=0;
            double FracAerobic=0;
            double Mic=0;
            double Mic_in_Aer, Mic_in_Anaer;
            // --------------------------------------------------

            if (IsAGGR)
            {
                DB = 0.0;
                return;
            }
            double Lo = Loading;

            CP = AQTSeg.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            // Susp sediment in which this tox is located

            //removed multi-layer tox code here

            WashO = CP.Washout() * GetPPB(NState, SVType, Layer) * 1e-6;

            Mic = MicrobialMetabolism(ref FracAerobic);    // returns FracAerobic which is not used
            Mic_in_Aer = Microbial_BioTrans_To_This_SV(true);
            Mic_in_Anaer = Microbial_BioTrans_To_This_SV(false);
            Sorpt = Sorption();
            Desorpt = Desorption();

            DB = Lo + ScourTox - DeposTox - WashO + WashI - Mic + Mic_in_Aer + Mic_in_Anaer + Sorpt - Desorpt;
            // all units ug(chemical)/L (wc) d
        }

    } // end TSuspSedimentTox


    public class TParticleTox : TToxics
    {
        public TParticleTox(AllVariables Ns, AllVariables Carry, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, Carry, SVT, L, aName, P, IC)
        { }

        // Various Helper functions for calculating derivs
        // ----------------------------------------------------------
        public double Derivative_Decomp(AllVariables ns)
        {
            // Labile Compartments only
            double FracAerobic=0;
            return  ((AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol)) as TRemineralize).Decomposition(AQTSeg.Location.Remin.DecayMax_Lab, Consts.KAnaerobic, ref FracAerobic);
        }

        // ----------------------------------------------------------
        public double Derivative_ColonizeDissRefrDetr()
        {
            return ((AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TDissRefrDetr).Colonization();
        }

        // ----------------------------------------------------------
        public double Derivative_ColonizeSuspRefrDetr()
        {
            return ((AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TSuspRefrDetr).Colonization();
        }

        // ----------------------------------------------------------
        public double Derivative_ColonizeSedRefrDetr()
        {
            return ((AQTSeg.GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TSedRefrDetr).Colonization();
        }

        // ----------------------------------------------------------
        public double Derivative_SumDefecationTox()
        {
            // sum all of the Predator defecation of toxicant
            double SumDef;
            AllVariables ns;
            SumDef = 0;
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                if (!(AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) == null))
                {
                    SumDef = SumDef + ((AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol)) as TAnimal).DefecationTox(SVType);
                }
            }
            return SumDef;
        }

        // ----------------------------------------------------------
        public double Derivative_SumExcToxToDiss(AllVariables DissPart)
        {
            // Sum of Toxicant Excretion from Plants to Dissolved Particulate
            AllVariables Loop;
            TDetritus DP;
            TPlant PPl;
            double Exc2Diss;
            Exc2Diss = 0;
            DP = AQTSeg.GetStatePointer(DissPart, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
            for (Loop = Consts.FirstPlant; Loop <= Consts.LastPlant; Loop++)
            {
                if (AQTSeg.GetStatePointer(Loop, T_SVType.StV, T_SVLayer.WaterCol) != null)
                {
                    PPl = AQTSeg.GetStatePointer(Loop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    Exc2Diss = Exc2Diss + PPl.PhotoResp() * DP.Excr_To_Diss_Detr(Loop) * GetPPB(Loop, SVType, Layer) * 1e-6;
                }
            }
            return Exc2Diss;
        }


        // ----------------------------------------------------------
        public double Derivative_IngestOfCarrier()
        {
            double SumIngest;
            double ER=0;
            double GER=0;
            object Ptr;
            AllVariables ns;
            SumIngest = 0;
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                Ptr = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
                if (!(Ptr == null))
                {
                    SumIngest = SumIngest + ((Ptr) as TAnimal).IngestSpecies(Carrier, null, ref ER, ref GER);
                }
            }
            return SumIngest;
        }

        // ----------------------------------------------------------
        public double Derivative_Sediment(AllVariables ns)
        {
            object P;
            P = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
            if (ns == AllVariables.SuspLabDetr)
                return ((P) as TSuspLabDetr).Sedimentation();
            else
                return ((P) as TSuspRefrDetr).Sedimentation();
        }

        // ----------------------------------------------------------
        //public double Derivative_Resuspension(AllVariables ns)
        //{
        //    TSuspendedDetr PSD = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TSuspendedDetr;
        //    return PSD.Resuspension();
        //}

        // ----------------------------------------------------------
        public void Derivative_MortalityToDetrTox_SumMortTox(ref double MortTox, AllVariables ns, TOrganism POr)
        {
            TDetritus DP;
            double Mort2Detr;
            double j;
            double Mort;
            double FracMult;
            if (POr.IsPlantOrAnimal())
            {
                DP = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TDetritus;
                // Get detrital toxicant carrier state variable
                Mort2Detr = DP.Mort_To_Detr(POr.NState);
                Mort = POr.Mortality();
                if (POr.IsMacrophyte())
                    Mort = Mort + ((POr) as TMacrophyte).Breakage();

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
                            FracMult = 1.0;
                        else
                            FracMult = 0.666666667;
                           // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.

                        Mort = Mort + TP.Sloughing * FracMult;
                    }
                }
                MortTox = MortTox + Mort2Detr * Mort * AQTSeg.GetPPB(POr.NState, SVType, T_SVLayer.WaterCol) * 1e-6;
            }

        }

        // ----------------------------------------------------------
        public double Derivative_MortalityToDetrTox(AllVariables ns)
        {
            // Calculates the toxicants in mortality / macro breakage / toxdischarge / sloughing to detritus interactions
            double MortTox;
            // SumMortTox
            MortTox = 0;
            
            foreach (TOrganism TO in AQTSeg.SV.OfType<TOrganism>())
                    Derivative_MortalityToDetrTox_SumMortTox(ref MortTox, ns, TO);

            return MortTox;
        }

        // ----------------------------------------------------------
        public double Derivative_GamLossToDetrTox()
        {
            
            // Calculates the toxicants in gamete loss to detritus interactions
            double GamTox;
            // SumGamTox
            GamTox = 0;
            foreach (TAnimal TA in AQTSeg.SV.OfType<TAnimal>())
                GamTox = GamTox + TA.GameteLoss() * AQTSeg.GetPPB(TA.NState, SVType, Layer) * 1e-6;

            return GamTox;
        }

        // ----------------------------------------------------------
        public double Derivative_SumPlantSedTox(TDetritus DP)
        {
            double SedTox = 0;

            foreach (TPlant TP in AQTSeg.SV.OfType<TPlant>())
              if (TP.IsAlgae())  // exclude macrophytes
            {
                double Sed2Detr = DP.PlantSink_To_Detr(TP.NState);
                SedTox = SedTox + (TP.Sedimentation() * Sed2Detr * GetPPB(TP.NState, SVType, T_SVLayer.WaterCol) * 1e-6);
            }
            return SedTox;
        }

        // ----------------------------------------------------------
        // removed pore water code here
        // ----------------------------------------------------------
        // removed buried detritus code here
        // ----------------------------------------------------------

        public override void Derivative(ref double DB)
        {
//            double M2_L() { return Location.Locale.SurfArea / (AQTSeg.SegVol() * 1000); }

            // DERIVATIVE FOR TOXICANT IN DETRITUS
            TRemineralize CP;
            TSuspendedDetr EpiCP;
            double Lo=0;
            double So = 0;
            double Des = 0;
            double Co = 0;
            double pp = 0;
            double Ing = 0;
            double SumSed = 0;
            double Photo = 0;
            double WashO = 0;
            double WashI = 0;
            double MTD = 0;
            double STH = 0;
            double Decmp = 0;
            double Sedm = 0;
            double SumDef = 0;
            double Hydr = 0;
            double MM = 0;
            double SETTD = 0;
            double ToPW = 0;
            double PWExp = 0;
            double Entr = 0;
            double FracAerobic =0;
            double Mic_in_Aer = 0;
            double Mic_in_Anaer = 0;
            double Resusp = 0;
            double DiffSed = 0;
            double GTD = 0;
            double TD = 0;
            double DiffUp = 0;
            double DiffDown = 0;
            double SFE = 0;
            double DepT = 0;
            double ScrT = 0;
            double BrT = 0;
            double ExpT = 0;
            AllVariables SedDetrVar;
            AllVariables SuspDetrVar;

            // ----------------------------------------------------------------

            if (IsAGGR)  // chemical variable used to track the aggregation of other chemicals e.g. Total PCB
            {
                DB = 0.0;
//              Derivative_WriteRates();
                return;
            }
            CP = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol) as TRemineralize;
            pp = GetPPB(NState, SVType, Layer);

            if (((NState==AllVariables.SedmLabDetr)||(NState==AllVariables.SedmRefrDetr)) && (AQTSeg.PSetup.TSedDetrIsDriving))
            {
//              Derivative_WriteRates();
                return;  // 6/7/2013, toxicant set to loading in CalculateLoad .

            }
            if ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SuspLabDetr) || (NState == AllVariables.DissLabDetr)) // labile
            {
                SedDetrVar = AllVariables.SedmLabDetr;
                SuspDetrVar = AllVariables.SuspLabDetr;
            }
            else
            {
                // nstate in [SedmRefrDetr,SuspRefrDetr,DissRefrDetr])  //refractory
                SedDetrVar = AllVariables.SedmRefrDetr;
                SuspDetrVar = AllVariables.SuspRefrDetr;
            }
  
            if (ChemRec.BCFUptake)
            {
                DB = 0.0;
            }
            else
            {
                Lo = Loading;

                Hydr = Hydrolysis();
                MM = MicrobialMetabolism(ref FracAerobic);
                Mic_in_Aer = Microbial_BioTrans_To_This_SV(true);
                Mic_in_Anaer = Microbial_BioTrans_To_This_SV(false);

                if ((NState == AllVariables.SedmLabDetr))
                {
                    // buried detritus code removed here
                    SumDef = Derivative_SumDefecationTox() * Consts.Def2SedLabDetr;
                    Ing = Derivative_IngestOfCarrier() * pp * 1e-6;
                    So = Sorption();
                    Des = Desorption();
                    Co = (Derivative_ColonizeSedRefrDetr() * AQTSeg.GetPPB(AllVariables.SedmRefrDetr, SVType, Layer) * 1e-6);
                    SumSed = Derivative_SumPlantSedTox(CP as TDetritus);
                    Decmp = Derivative_Decomp(AllVariables.SedmLabDetr) * pp * 1e-6;
                    Sedm = Derivative_Sediment(AllVariables.SuspLabDetr);
                    if (Sedm > 0)
                    {   // both sedimentation and resuspension covered by Sedm term depending on sign of variable
                        Sedm = Sedm * AQTSeg.GetPPB(AllVariables.SuspLabDetr, SVType, Layer) * 1e-6;  //resuspension
                    }
                    else Sedm = Sedm * pp * 1e-6;  // sedimentation
                    
                    //Resusp = Derivative_Resuspension(AllVariables.SuspLabDetr) * pp * 1e-6; // resuspension in multi-sed-layer model disabled

                    DB = Lo + So - Des + Co + SumDef - (Decmp + Ing) + Sedm - Resusp + Mic_in_Aer + Mic_in_Anaer - Hydr - MM + SumSed + DepT - ScrT - BrT + ExpT;
                    // SedLabileDetrTox
                }
                else if ((NState == AllVariables.SedmRefrDetr)) 
                {
                    // buried detritus code removed here
                    So = Sorption();
                    Des = Desorption();
                    Co = Derivative_ColonizeSedRefrDetr() * pp * 1e-6;
                    SumSed = Derivative_SumPlantSedTox(CP as TDetritus);
                    SumDef = Derivative_SumDefecationTox();
                    SumDef = SumDef * (1 - Consts.Def2SedLabDetr);

                    Ing = Derivative_IngestOfCarrier() * pp * 1e-6;
                    Sedm = Derivative_Sediment(SuspDetrVar);
                    if (Sedm > 0)
                    {   // both sedimentation and resuspension covered by Sedm term depending on sign of variable
                        Sedm = Sedm * AQTSeg.GetPPB(SuspDetrVar, SVType, Layer) * 1e-6;  //resuspension
                    }
                    else Sedm = Sedm * pp * 1e-6;  //sedimentation

                    // resuspension
                    // Resusp = Derivative_Resuspension(SuspDetrVar) * pp * 1e-6;   // resuspension in multi-sed-layer model disabled

                    // kinetic derivative for Chemical in SedmRefrDetr
                    DB = Lo + So - Des + SumDef + SumSed - (Co + Ing) + Sedm - Resusp - Hydr - MM + Mic_in_Aer + Mic_in_Anaer + DepT - ScrT - BrT + ExpT;
                    // SedmRefrDetr / SedmDetr
                }
                else if (NState == AllVariables.SuspLabDetr)
                {
                    // buried detritus code removed here

                    Photo = Photolysis();
                    WashO = CP.Washout() * AQTSeg.GetPPB(AllVariables.SuspLabDetr, SVType, Layer) * 1e-6;

                    // HMS multi-segment interaction handled by HMS workflow.  ToxInCarrierWashin removed,  ToxDiff removed, DiffUp and DiffDown removed. Sink to Hypolimnion Removed

                    So = Sorption();
                    Des = Desorption();

                    Sedm = Derivative_Sediment(AllVariables.SuspLabDetr);
                    Decmp = Derivative_Decomp(AllVariables.SuspLabDetr) * AQTSeg.GetPPB(AllVariables.SuspLabDetr, SVType, Layer) * 1e-6;

                    if (AQTSeg.Diagenesis_Included() && (Sedm < 0)) Sedm = 0;  // no resuspension in diagenesis model
                    if (Sedm >= 0)  // sedimentation
                        Sedm = Sedm * AQTSeg.GetPPB(AllVariables.SuspLabDetr, SVType, Layer) * 1e-6;
                    else // resuspension
                        Sedm = Sedm * AQTSeg.GetPPB(AllVariables.SedmLabDetr, SVType, Layer) * 1e-6;

                    // resuspension in multi-sed-layer model disabled;  code removed
                    MTD = Derivative_MortalityToDetrTox(AllVariables.SuspLabDetr);
                    GTD = Derivative_GamLossToDetrTox();

                    Ing = Derivative_IngestOfCarrier() * AQTSeg.GetPPB(AllVariables.SuspLabDetr, SVType, Layer) * 1e-6;
                    Co = Derivative_ColonizeSuspRefrDetr() * AQTSeg.GetPPB(AllVariables.SuspRefrDetr, SVType, Layer) * 1e-6 + Derivative_ColonizeDissRefrDetr() * AQTSeg.GetPPB(AllVariables.DissRefrDetr, SVType, Layer) * 1e-6;

                    DB = Lo + So - Des + MTD + GTD + WashI + Resusp - (Sedm + WashO + Decmp + Ing) + Entr + Co - Hydr - Photo - MM + TD + DepT + ScrT + DiffUp + DiffDown - STH + SFE + Mic_in_Aer + Mic_in_Anaer;
                    // SuspLabDetr Toxicant Deriv
                }
                else if (NState == AllVariables.SuspRefrDetr) 
                {
                    // Buried detritus code removed
                    Photo = Photolysis();
                    WashO = CP.Washout() * pp * 1e-6;

                    // HMS multi-segment interaction handled by HMS workflow.  ToxInCarrierWashin removed,  ToxDiff removed, DiffUp and DiffDown removed. Sink to Hypolimnion Removed

                    So = Sorption();
                    Des = Desorption();
                    Sedm = Derivative_Sediment(NState);
                    
                    if (AQTSeg.Diagenesis_Included() && (Sedm < 0)) Sedm = 0; // no resuspension in diagenesis model
                    if (Sedm >= 0) // sedimentation
                        Sedm = Sedm * AQTSeg.GetPPB(NState, SVType, Layer) * 1e-6;
                    else // resuspension
                        Sedm = Sedm * AQTSeg.GetPPB(SedDetrVar, SVType, Layer) * 1e-6;

                    // resuspension in multi-sed-layer model disabled;  code removed

                    Co = Derivative_ColonizeSuspRefrDetr() * pp * 1e-6;
                    MTD = Derivative_MortalityToDetrTox(NState);
                    Ing = Derivative_IngestOfCarrier() * pp * 1e-6;

                    DB = Lo + So - Des + MTD + WashI + Resusp - (Sedm + WashO + Co + Ing) + Entr - Hydr + GTD - Photo - MM + TD + DiffUp + DiffDown + DepT + ScrT - STH + SFE + Mic_in_Aer + Mic_in_Anaer;
                    // SuspRefrDetr Toxicant Deriv
                }
                else if ((NState == AllVariables.DissLabDetr))
                {
                    Photo = Photolysis();
                    WashO = CP.Washout() * pp * 1e-6;

                    // HMS multi-segment interaction handled by HMS workflow.  ToxInCarrierWashin removed,  ToxDiff removed, DiffUp and DiffDown removed. 
                    // pore water diffusion removed, no multi-layer sediment model in HMS at this time

                    So = Sorption();
                    Des = Desorption();
                    MTD = Derivative_MortalityToDetrTox(AllVariables.DissLabDetr);
                    SETTD = Derivative_SumExcToxToDiss(AllVariables.DissLabDetr);
                    Decmp = Derivative_Decomp(AllVariables.DissLabDetr) * AQTSeg.GetPPB(AllVariables.DissLabDetr, SVType, Layer) * 1e-6;

                    // Co    := ColonizeDissRefrDetr * GetPPB(DissRefrDetrTox) * 1e-6; // Colonization of DissRefrDetr-->suspLabDetr June 24, 1998

                    DB = Lo + So - Des + MTD + SETTD + WashI - (WashO + Decmp) + Entr - Hydr - Photo - MM + TD + DiffUp + DiffDown + DiffSed + Mic_in_Aer + Mic_in_Anaer + PWExp - ToPW;
                    // DissLabDetr Tox Deriv
                }
                else if ((NState == AllVariables.DissRefrDetr))
                {
                    Photo = Photolysis();
                    WashO = CP.Washout() * pp * 1e-6;

                    // HMS multi-segment interaction handled by HMS workflow.  ToxInCarrierWashin removed,  ToxDiff removed, DiffUp and DiffDown removed. 
                    // pore water diffusion removed, no multi-layer sediment model in HMS at this time

                    So = Sorption();
                    Des = Desorption();
                    MTD = Derivative_MortalityToDetrTox(NState);
                    SETTD = Derivative_SumExcToxToDiss(NState);
                    Co = Derivative_ColonizeDissRefrDetr() * AQTSeg.GetPPB(NState, SVType, Layer) * 1e-6;

                    DB = Lo + So - Des + MTD + SETTD + WashI - (WashO + Co) + Entr - Hydr - Photo - MM + TD + DiffUp + DiffDown + DiffSed + Mic_in_Aer + Mic_in_Anaer + PWExp - ToPW;
                }
                // DissRefrDetrTox
            }
        }

    } // end TParticleTox

    public class TPOCTox : TParticleTox
    {
        public TPOCTox(AllVariables Ns, AllVariables Carry, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, Carry, SVT, L, aName, P, IC)
        { }

        // -------------------------------------------------------------------------
        public override void Derivative(ref double db)
        {
            double So;
            double Des;
            double DepTox;
            double Minrl;
            double Bur;
            double Pred;
            double Mic_In_Anaer;
            double Hydr;
            double MM;
            TPOC_Sediment CP;
            double PP;
            // ----------------------------------------------------------------
            double FA=0;
            double H2;
            double BuryInKg;
            double SedDesorpInKg;

            CP = AQTSeg.GetStatePointer(NState, T_SVType.StV, Layer) as TPOC_Sediment;
            So = 0;
            Des = 0;
            DepTox = 0;
            Minrl = 0;
            Bur = 0;
            Pred = 0;
            Mic_In_Anaer = 0;
            Hydr = 0;
            MM = 0;
            if (IsAGGR)
            {
                db = 0.0;
//              Derivative_WriteRates();
                return;
            }
            if (ChemRec.BCFUptake || (CP.State == 0)) db = 0.0;
            else
            {
                So = Sorption();      // ug/m2 d
                Des = Desorption();   // ug/m2 d
                  DepTox = AQTSeg.CalcDeposition(NState, SVType);
                // ug/m2 d

                PP = GetPPB(NState, SVType, Layer);
                H2 = AQTSeg.Diagenesis_Params.H2.Val;
                Minrl = CP.Mineralization() * PP * H2 * 1e-3;
                Bur = CP.Burial() * PP * H2 * 1e-3;
             // ug/m2 d  (g /m3 d)(ug/kg) (m) (kg/g)

                MorphRecord MR = Location.Morph;
                Pred = CP.Predn() * PP *   MR.SegVolum / Location.Locale.SurfArea * 1e-3;
          // (ug/m2 d) (g OC/m3 w)(ug tox/kg OC) (m3)                     (m2)      (kg/g)

                MM = MicrobialMetabolism(ref FA);
                // anaerobic only
                // ug/m2 d
                Mic_In_Anaer = Microbial_BioTrans_To_This_SV(false);
                // anaerobic only
                Hydr = Hydrolysis();
                db = So - Des + DepTox - Minrl - Bur - Pred - MM + Mic_In_Anaer - Hydr;
                // ug/m2 d
                // input is ug/kg
            }
        }
    } // end TPOCTox

    public class TAlgae_ZooTox : TToxics
    {
        public TToxics(int Ns, int Carry, T_SVType SVT, T_SVLayer L, string aName, TStates P, double IC, bool IsTempl) : base(Ns, Carry, SVT, L, aName, P, IC, IsTempl)
        {
        }

        public double MacroUptake()
        {
            
            double K1;
            double DissocFactor;
            if (NonDissoc() < 0.2)
                DissocFactor = 0.2;
            else
                DissocFactor = NonDissoc();

            K1 = 1 / (0.0020 + (500 / (Kow * DissocFactor)));

            // K1 function is mirrored in CHEMTOX.PAS, any change here needs to be made there
            K2 = AlgalPtr.Plant_Tox[SVType].K2;
            if (K2 > 96)
            {
                K1 = K1 * (96 / K2);
            }
            // scaling factor 10-02-03

            TStates _wvar4 = AQTSeg;
            return  K1 * _wvar4.Diff[SVType] * ToxState * AlgalPtr.State * 1e-6;
            // ug/L-d
            // L/kg-d
            // unitless
            // ug/L
            // mg/L
            // kg/mg

        }

        // animals only
        // -------------------------------------------------------------------------
        public double PlantUptake()
        {
            
            // Integrated MacroUptake into AlgalUptake for ease of coding / code reading  9/16/98 jsc
            double PFAK2;
            double K2;
            double Local_K1;
            double UptakeLimit;
            double ToxState;
            double DissocFactor;
            TPlant AlgalPtr;
            UptakeCalcMethodType ChemOption;
            ChemOption = Chemptrs[SVType].Plant_Method;
            result = 0;
            AlgalPtr = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol);
            ToxState = GetState(Consts.AssocToxSV(SVType), T_SVType.StV, T_SVLayer.WaterCol);
            if ((AlgalPtr == null) || (ToxState <= Consts.Tiny))
            {
                return result;
            }
            // ---------------------------------------------------------------------
            if (ChemOption != Consts.UptakeCalcMethodType.Default_Meth)
            {
                TPlantToxRecord _wvar5 = AlgalPtr.Plant_Tox[SVType];
                if (ChemOption == Consts.UptakeCalcMethodType.CalcK1)
                {
                    // 5/29/2015, add Bio_rate_const
                    Local_K1 = (_wvar5.K2 + _wvar5.Bio_rate_const) * _wvar5.Entered_BCF;
                }
                else
                {
                    Local_K1 = _wvar5.K1;
                }
                TStates _wvar6 = AQTSeg;
                result = Local_K1 * _wvar6.Diff[SVType] * ToxState * AlgalPtr.State * 1e-6;
                // ug/L-d
                // L/kg-d
                // ug/L
                // mg/L
                // kg/mg
                return result;
            }
            // ---------------------------------------------------------------------
            if (NState >= Consts.FirstMacro && NState <= Consts.LastMacro)
            {
                result = PlantUptake_MacroUptake();
            }
            else
            {
                // Non Macrophyte Plants
                if (NonDissoc() < 0.2)
                {
                    DissocFactor = 0.2;
                }
                else
                {
                    DissocFactor = NonDissoc();
                }
                TChemical _wvar7 = Chemptrs[SVType];
                ChemicalRecord _wvar8 = _wvar7.ChemRec;
                if (_wvar8.IsPFA)
                {
                    PFAK2 = AlgalPtr.Plant_Tox[SVType].K2;
                    // L/kg-d
                    // L/kg
                    // 1/d
                    Local_K1 = _wvar8.PFAAlgBCF * PFAK2;
                }
                else
                {
                    Local_K1 = 1 / (1.8e-6 + 1 / (Chemptrs[SVType].Kow * DissocFactor));
                }
                // fit to Sijm et al.1998 data for PCBs
                TStates _wvar9 = AQTSeg;
                UptakeLimit = (AlgalPtr.BCF(0, SVType) * ToxState - _wvar9.GetPPB(NState, SVType, Layer)) / (AlgalPtr.BCF(0, SVType) * ToxState);
                if (UptakeLimit < 0)
                {
                    UptakeLimit = 0;
                }
                K2 = AlgalPtr.Plant_Tox[SVType].K2;
                if (K2 > 96)
                {
                    Local_K1 = Local_K1 * (96 / K2);
                }
                // scaling factor 10-02-03
                TStates _wvar10 = AQTSeg;
                result = Local_K1 * UptakeLimit * _wvar10.Diff[SVType] * ToxState * AlgalPtr.State * 1e-6;
                // ug/L-d
                // L/kg-d
                // unitless
                // ug/L
                // mg/L
                // kg/mg
            }
            // algae

            return result;
        }

        // --------------------------------------------------------------------------
        public double SumDietUptake()
        {
            
            // *************************************
            // Calculate dietary uptake of Org Tox
            // from all prey.  Tox Uptake that is
            // defecated is excluded from sum.
            // modified JSC, 7/21/98
            // *************************************
            int Loop;
            double SumDiet;
            double KD;
            double ToxPrey;
            double EgestCoeff;
            double GutEffRed;
            double GutEffTox;
            TAnimal CP;
            CP = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol);
            SumDiet = 0;
            for (Loop = Consts.FirstDetr; Loop <= Consts.LastAnimal; Loop++)
            {
                if (new ArrayList(new object[] { Consts.FirstDetr, Consts.FirstBiota }).Contains(Loop))
                {
                    KD = CP.IngestSpecies(Loop, null, ref EgestCoeff, ref GutEffRed);
                    ToxPrey = 0;
                    if (KD > 0)
                    {
                        TStates _wvar1 = AQTSeg;
                        GutEffTox = CP.GutEffOrgTox(SVType) * GutEffRed;
                        if (_wvar1.GetState(Loop, SVType, T_SVLayer.WaterCol) > -1)
                        {
                            // ug/L
                            // ug/kg
                            // mg/L
                            // kg/mg
                            ToxPrey = _wvar1.GetPPB(Loop, SVType, T_SVLayer.WaterCol) * KD * 1e-6 * GutEffTox;
                        }
                        else if (_wvar1.Diagenesis_Included())
                        {
                            if ((Loop == AllVariables.SedmRefrDetr))
                            {
                                ToxPrey = _wvar1.GetPPB(AllVariables.POC_G2, SVType, T_SVLayer.SedLayer2) * KD * 1e-6 * GutEffTox / Consts.Detr_OM_2_OC;
                            }
                            // ug/L
                            // ug /kg OC
                            // mg OM/L
                            // kg/mg
                            // unitless
                            // OM/OC
                            if ((Loop == AllVariables.SedmLabDetr))
                            {
                                ToxPrey = _wvar1.GetPPB(AllVariables.POC_G1, SVType, T_SVLayer.SedLayer2) * KD * 1e-6 * GutEffTox / Consts.Detr_OM_2_OC;
                            }
                            // ug/L
                            // ug /kg OC
                            // mg OM/L
                            // kg/mg
                            // unitless
                            // OM/OC
                        }
                        else
                        {
                            ToxPrey = 0;
                        }
                        if (ToxPrey < 0)
                        {
                            ToxPrey = 0;
                        }
                    }
                    SumDiet = SumDiet + ToxPrey;
                }
            }
            result = SumDiet;
            return result;
        }

        // ----------------------------------------------------------------
        public double Derivative_ToxToPhytoFromSlough()
        {
            
            AllVariables PlantLoop;
            TPlant PPl;
            double TPSlough;
            double j;
            TPSlough = 0;
            for (PlantLoop = Consts.FirstAlgae; PlantLoop <= Consts.LastAlgae; PlantLoop++)
            {
                PPl = AQTSeg.GetStatePointer(PlantLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if (PPl != null)
                {
                    if (PPl.PSameSpecies == NState)
                    {
                        PPl.CalcSlough();
                        // update sloughevent
                        if (PPl.SloughEvent)
                        {
                            j = -999;
                            // signal to not write mass balance tracking
                            PPl.Derivative(j);
                            // update sloughing
                            TPSlough = TPSlough + PPl.Sloughing * GetPPB(PPl.NState, SVType, T_SVLayer.WaterCol) * 1e-6 * (1 / 3);
                            // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/Slough.
                        }
                    }
                }
            }
            result = TPSlough;
            return result;
        }

        // ----------------------------------------------------------------
        public double Derivative_ToxSed2Me()
        {
            
            // Calculates toxicant transfer due to sedimentation of phytoplankton
            // to each periphyton compartment JSC Sept 8, 2004
            double PPBPhyto;
            result = 0;
            if (!CP.IsPeriphyton())
            {
                return result;
            }
            if (CP.PSameSpecies == AllVariables.NullStateVar)
            {
                return result;
            }
            if (CP.GetStatePointer(CP.PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) == null)
            {
                return result;
            }
            PPBPhyto = GetPPB(CP.PSameSpecies, SVType, T_SVLayer.WaterCol);
            result = CP.SedToMe() * PPBPhyto * 1e-6;
            // ug/L
            // mg/L
            // ug/kg
            // kg/mg

            return result;
        }

        // ----------------------------------------------------------------
        public void Derivative_WriteRates()
        {
            Setup_Record _wvar1 = AQTSeg.SetupRec;
            if ((_wvar1.SaveBRates || _wvar1.ShowIntegration))
            {
                ClearRate();
                SaveRate("State", State);
                SaveRate("Loading", Lo);
                if ((Carrier >= Consts.FirstAlgae && Carrier <= Consts.LastAlgae) && (((AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol)) as TPlant).IsPhytoplankton()))
                {
                    if (!AQTSeg.LinkedMode)
                    {
                        SaveRate("TurbDiff", TD);
                    }
                    else
                    {
                        // If Not AQTSeg.CascadeRunning then
                        SaveRate("DiffUp", DiffUp);
                        SaveRate("DiffDown", DiffDown);
                    }
                }
                if ((Carrier >= Consts.FirstAlgae && Carrier <= Consts.LastAlgae) && (!((AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol)) as TPlant).IsPhytoplankton()))
                {
                    SaveRate("ToxDislodge", ToxD);
                }
                SaveRate("Biotr IN", BioT_In);
                SaveRate("Biotr OUT", BioT_Out);
                if (!(NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
                {
                    SaveRate("Washout", WashO);
                    SaveRate("Washin", WashI);
                    SaveRate("SinkToHyp", STH);
                    SaveRate("SinkFromEp", SFE);
                    if (SurfaceFloater)
                    {
                        SaveRate("Floating", Flt);
                    }
                    if (AQTSeg.EstuarySegment)
                    {
                        SaveRate("Entrainment", Entr);
                    }
                    SaveRate("NetBoundary", Lo + WashI - WashO + DiffUp + Entr + DiffDown + TD);
                }
                if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro) && (((CP) as TMacrophyte).MacroType == Consts.TMacroType.Freefloat))
                {
                    SaveRate("Washout", WashO);
                    SaveRate("Washin", WashI);
                    SaveRate("NetBoundary", Lo + WashI - WashO + DiffUp + DiffDown + TD);
                    SaveRate("Mac Break", MacBrk);
                }
                SaveRate("Uptake", Uptake);
                SaveRate("Mortality", Mort);
                SaveRate("Predation", Predt);
                SaveRate("Depuration", Dep);
                SaveRate("Excretion", Exc);
                if ((CP.IsPeriphyton()))
                {
                    SaveRate("Sed to Phyt", Sed2Me);
                }
                if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
                {
                    SaveRate("Peri Slough", Slgh);
                    SaveRate("Sediment", Sed);
                }
            }
        }

        // all plants
        // -------------------------------------------------------------------------
        public override void Derivative(double DB)
        {
            // Derivitives for Plants, Animals are sent to AnimalDeriv
            TPlant HypCp;
            TPlant EpiCp;
            TPlant CP;
            double Dep;
            double STH;
            double SFE;
            double BioT_Out;
            double BioT_In;
            double ToxD;
            double OOSDriftInKg;
            double Entr;
            double Flt;
            double TD;
            double DiffUp;
            double DiffDown;
            double Pp;
            double WashO;
            double WashI;
            double Lo;
            double Predt;
            double Mort;
            double Sed;
            double Uptake;
            double Exc;
            double Sed2Me;
            double MacBrk;
            double Slgh;
            double LoadInKg;
            bool SurfaceFloater;
            // ----------------------------------------------------------------
            CP = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol);
            SurfaceFloater = CP.PAlgalRec.SurfaceFloating;
            WashO = 0;
            Lo = 0;
            Predt = 0;
            Mort = 0;
            Sed = 0;
            WashI = 0;
            DiffUp = 0;
            DiffDown = 0;
            Uptake = 0;
            Exc = 0;
            SFE = 0;
            BioT_Out = 0;
            BioT_In = 0;
            TD = 0;
            ToxD = 0;
            Entr = 0;
            Sed = 0;
            MacBrk = 0;
            Slgh = 0;
            Sed2Me = 0;
            Flt = 0;
            if (IsAGGR)
            {
                DB = 0.0;
                Derivative_WriteRates();
                return;
            }
            if ((!(NState >= Consts.FirstPlant && NState <= Consts.LastPlant)))
            {
                DB = AnimalDeriv();
            }
            else if ((Consts.Eutrophication || Chemptrs[SVType].ChemRec.BCFUptake || (CP.State == 0)))
            {
                DB = 0.0;
            }
            // macrophytes
            else if (NState >= Consts.FirstMacro && NState <= Consts.LastMacro)
            {
                BioT_Out = Biotransformation();
                BioT_In = Biotrans_To_This_Org();
                Lo = Loading;
                Pp = GetPPB(NState, SVType, Layer) * 1e-6;
                WashO = CP.Washout() * Pp;
                WashoutStep[AQTSeg.DerivStep] = WashO * AQTSeg.SegVol();
                TStates _wvar2 = AQTSeg;
                ToxLossRecord _wvar3 = _wvar2.ToxLossArray[SVType];
                MorphRecord _wvar4 = _wvar2.Location.Morph;
                // save for tox loss output & categorization
                // * OOSDischFrac
                OOSDriftInKg = WashO * _wvar2.SegVol() * 1000.0 * 1e-9;
                // kg
                // ug/L
                // frac
                // m3
                // L/m3
                // kg/ug
                _wvar3.TotalToxLoss[_wvar2.DerivStep] = _wvar3.TotalToxLoss[_wvar2.DerivStep] + OOSDriftInKg;
                _wvar3.TotalWashout[_wvar2.DerivStep] = _wvar3.TotalWashout[_wvar2.DerivStep] + OOSDriftInKg;
                _wvar3.WashoutPlant[_wvar2.DerivStep] = _wvar3.WashoutPlant[_wvar2.DerivStep] + OOSDriftInKg;
                WashI = ToxInCarrierWashin();
                TStates _wvar5 = AQTSeg;
                ToxLoadRecord _wvar6 = _wvar5.ToxLoadArray[SVType];
                // save for tox loss output & categorization
                LoadInKg = (Lo + WashI) * _wvar5.SegVol() * 1000.0 * 1e-9;
                // kg
                // ug/L
                // m3
                // L/m3
                // kg/ug
                _wvar6.TotOOSLoad[_wvar5.DerivStep] = _wvar6.TotOOSLoad[_wvar5.DerivStep] + LoadInKg;
                _wvar6.ToxLoadBiota[_wvar5.DerivStep] = _wvar6.ToxLoadBiota[_wvar5.DerivStep] + LoadInKg;
                Dep = Depuration();
                Uptake = PlantUptake();
                Mort = CP.Mortality() * Pp;
                Predt = CP.Predation() * GetPPB(NState, SVType, Layer) * 1e-6;
                Exc = CP.PhotoResp() * Pp;
                MacBrk = ((CP) as TMacrophyte).Breakage() * Pp;
                DB = Lo + Uptake - Dep - (Predt + Mort + Exc + MacBrk) - BioT_Out + BioT_In - WashO + WashI;
            }
            else if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
            {
                Pp = GetPPB(NState, SVType, Layer) * 1e-6;
                WashO = CP.Washout() * Pp;
                WashoutStep[AQTSeg.DerivStep] = WashO * AQTSeg.SegVol();
                TStates _wvar7 = AQTSeg;
                if ((((CP) as TPlant).IsPhytoplankton()) && _wvar7.EstuarySegment)
                {
                    Entr = EstuaryEntrainment();
                    if (Entr < 0)
                    {
                        Entr = Entr * Pp;
                    }
                    if (Entr > 0)
                    {
                        Entr = Entr * _wvar7.HypoSegment.GetPPB(NState, SVType, Layer) * 1e-6;
                    }
                }
                TStates _wvar8 = AQTSeg;
                ToxLossRecord _wvar9 = _wvar8.ToxLossArray[SVType];
                MorphRecord _wvar10 = _wvar8.Location.Morph;
                // save for tox loss output & categorization
                if (Entr < 0)
                {
                    // * OOSDischFrac
                    OOSDriftInKg = (-Entr + WashO) * _wvar8.SegVol() * 1000.0 * 1e-9;
                }
                else
                {
                    // * OOSDischFrac
                    OOSDriftInKg = WashO * _wvar8.SegVol() * 1000.0 * 1e-9;
                }
                // kg
                // ug/L
                // frac
                // m3
                // L/m3
                // kg/ug
                _wvar9.TotalToxLoss[_wvar8.DerivStep] = _wvar9.TotalToxLoss[_wvar8.DerivStep] + OOSDriftInKg;
                _wvar9.TotalWashout[_wvar8.DerivStep] = _wvar9.TotalWashout[_wvar8.DerivStep] + OOSDriftInKg;
                _wvar9.WashoutPlant[_wvar8.DerivStep] = _wvar9.WashoutPlant[_wvar8.DerivStep] + OOSDriftInKg;
                WashI = ToxInCarrierWashin();
                Lo = Loading;
                TStates _wvar11 = AQTSeg;
                ToxLoadRecord _wvar12 = _wvar11.ToxLoadArray[SVType];
                // save for tox loss output & categorization
                if (Entr > 0)
                {
                    LoadInKg = (Lo + Entr + WashI) * _wvar11.SegVol() * 1000.0 * 1e-9;
                }
                else
                {
                    LoadInKg = (Lo + WashI) * _wvar11.SegVol() * 1000.0 * 1e-9;
                }
                // kg
                // ug/L
                // m3
                // L/m3
                // kg/ug
                _wvar12.TotOOSLoad[_wvar11.DerivStep] = _wvar12.TotOOSLoad[_wvar11.DerivStep] + LoadInKg;
                _wvar12.ToxLoadBiota[_wvar11.DerivStep] = _wvar12.ToxLoadBiota[_wvar11.DerivStep] + LoadInKg;
                STH = CP.SinkToHypo * Pp;
                TStates _wvar13 = AQTSeg;
                if (_wvar13.VSeg == Consts.VerticalSegments.Hypolimnion)
                {
                    EpiCp = _wvar13.EpiSegment.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol);
                    // Refinement 10-20-2002 JSC
                    SFE = EpiCp.SinkToHypo * _wvar13.EpiSegment.GetPPB(NState, SVType, T_SVLayer.WaterCol) * 1e-6;
                }
                if (SurfaceFloater)
                {
                    if (AQTSeg.Stratified)
                    {
                        Flt = CP.Floating() * Pp;
                        TStates _wvar14 = AQTSeg;
                        if (_wvar14.VSeg == Consts.VerticalSegments.Epilimnion)
                        {
                            HypCp = _wvar14.HypoSegment.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol);
                            Flt = HypCp.Floating() * _wvar14.HypoSegment.GetPPB(NState, SVType, T_SVLayer.WaterCol) * 1e-6;
                        }
                    }
                }
                BioT_Out = Biotransformation();
                BioT_In = Biotrans_To_This_Org();
                Predt = CP.Predation() * Pp;
                Mort = CP.Mortality() * Pp;
                Exc = CP.PhotoResp() * Pp;
                ToxD = CP.ToxicDislodge() * Pp;
                Sed = CP.Sedimentation() * Pp;
                // plant sedimentation
                Sed2Me = Derivative_ToxSed2Me();
                if ((CP.IsPeriphyton()))
                {
                    Slgh = CP.Sloughing * Pp;
                }
                else
                {
                    Slgh = -Derivative_ToxToPhytoFromSlough();
                }
                Uptake = PlantUptake();
                Dep = Depuration();
                DB = Lo + Uptake - Dep + WashI - (WashO + Predt + Mort + Sed + Exc + ToxD + Slgh) - BioT_Out + BioT_In - STH + SFE + Flt + Sed2Me + Entr;
                // algae
            }
            // Phytoplankton are subject to currents
            PlantRecord _wvar15 = ((AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol)) as TPlant).PAlgalRec;
            if ((Carrier >= Consts.FirstAlgae && Carrier <= Consts.LastAlgae) && (_wvar15.PlantType == "Phytoplankton") && (!_wvar15.SurfaceFloating))
            {
                if (!AQTSeg.LinkedMode)
                {
                    TD = ToxDiff();
                }
                else if (!AQTSeg.CascadeRunning)
                {
                    DiffUp = ToxSegDiff(true);
                    DiffDown = ToxSegDiff(false);
                }
                DB = DB + TD + DiffUp + DiffDown;
            }
            if (NState >= Consts.FirstPlant && NState <= Consts.LastPlant)
            {
                Derivative_WriteRates();
            }
        }

    } // end TAlgae_ZooTox

    public class TFishTox : TToxics
    {
        // ---------------------------------------------------------------------------
        public override void Derivative(double DB)
        {
            DB = AnimalDeriv();
            // no turb diff for fish

        }

        public override void Store(bool IsTemp, ref Stream st)
        {
            base.Store();
            // TToxics.Store(IsTemp, St);

        }

        //Constructor  load( IsTemp, ref  st,  ReadVersionNum)
        public TFishTox(bool IsTemp, ref Stream st, double ReadVersionNum) : this()
        {
            // TToxics.Load(IsTemp, St,ReadVersionNum);

        }
    } // end TFishTox



}
