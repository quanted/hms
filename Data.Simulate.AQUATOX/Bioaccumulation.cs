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

    public class TAlgaeTox : TToxics

    {
        public TPlantToxRecord Plant_Tox = null;

        public TAlgaeTox(AllVariables Ns, AllVariables Carry, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, Carry, SVT, L, aName, P, IC)
        {
        }



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
            ChemOption = Plant_Method;

            double MacroUptake()
            {  double K1;
                double DissocFactor;
                if (NonDissoc() < 0.2)
                    DissocFactor = 0.2;
                else
                    DissocFactor = NonDissoc();

                K1 = 1 / (0.0020 + (500 / (Math.Pow(10, ChemRec.LogKow) * DissocFactor)));

                // K1 function is mirrored in CHEMTOX.PAS, any change here needs to be made there
                double K2 = Plant_Tox.K2;
                if (K2 > 96) K1 = K1 * (96 / K2);  // scaling factor 10-02-03

                return K1 * ToxState * AlgalPtr.State * 1e-6;        // HMS removed Dif
           // ug/L-d (L/kg-d) (ug/L)      (mg/L)      (kg/mg)
            }



            AlgalPtr = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
            ToxState = AQTSeg.GetState(AllVariables.H2OTox, SVType, T_SVLayer.WaterCol);

            if ((AlgalPtr == null) || (ToxState <= Consts.Tiny)) return 0;

            // ---------------------------------------------------------------------
            if (ChemOption != UptakeCalcMethodType.Default_Meth)
            {
                if (ChemOption == UptakeCalcMethodType.CalcK1)
                {
                    // 5/29/2015, add Bio_rate_const
                    Local_K1 = (Plant_Tox.K2 + Plant_Tox.Bio_rate_const) * Plant_Tox.Entered_BCF;
                }
                else
                {
                    Local_K1 = Plant_Tox.K1;
                }

                return Local_K1 * ToxState * AlgalPtr.State * 1e-6;
        // ug/L-d     // L/kg-d    // ug/L        // mg/L    // kg/mg
            }

            // ---------------------------------------------------------------------
            if (NState >= Consts.FirstMacro && NState <= Consts.LastMacro) return MacroUptake();
            else
            {
                // Non Macrophyte Plants
                if (NonDissoc() < 0.2)
                    DissocFactor = 0.2;
                else
                    DissocFactor = NonDissoc();

                Local_K1 = 1 / (1.8e-6 + 1 / (Kow * DissocFactor));
                // fit to Sijm et al.1998 data for PCBs

                UptakeLimit = (AlgalPtr.BCF(0, SVType) * ToxState - GetPPB(NState, SVType, Layer)) / (AlgalPtr.BCF(0, SVType) * ToxState);
                if (UptakeLimit < 0) UptakeLimit = 0;

                K2 = Plant_Tox.K2;
                if (K2 > 96)
                {
                    Local_K1 = Local_K1 * (96 / K2); // scaling factor 10-02-03
                }
                
                return Local_K1 * UptakeLimit * ToxState * AlgalPtr.State * 1e-6;
              //(ug/L-d) (L/kg-d)  (unitless)    (ug/L)        (mg/L)       (kg/mg)
            }
            // algae
        }

        // --------------------------------------------------------------------------
        public double Derivative_ToxToPhytoFromSlough()
        {
            AllVariables PlantLoop;
            TPlant PPl;
            double TPSlough;
            double j = 0;
            TPSlough = 0;
            for (PlantLoop = Consts.FirstAlgae; PlantLoop <= Consts.LastAlgae; PlantLoop++)
            {
                PPl = AQTSeg.GetStatePointer(PlantLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PPl != null)
                {
                    if (PPl.PSameSpecies == NState)
                    {
                        PPl.CalcSlough();
                        // update sloughevent
                        if (PPl.SloughEvent)
                        {
                            j = -999; // signal to not write mass balance tracking
                            PPl.Derivative(ref j); // update sloughing
                            TPSlough = TPSlough + PPl.Sloughing * GetPPB(PPl.NState, SVType, T_SVLayer.WaterCol) * 1e-6 * (1.0 / 3.0); // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/Slough.
                        }
                    }
                }
            }
            return TPSlough;
        }

        // ----------------------------------------------------------------
        public double Derivative_ToxSed2Me(TPlant CP)
        {
            // Calculates toxicant transfer due to sedimentation of phytoplankton
            // to each periphyton compartment JSC Sept 8, 2004
            double PPBPhyto;
            if (!CP.IsPeriphyton()) return 0;
            if (CP.PSameSpecies == AllVariables.NullStateVar) return 0;
            if (AQTSeg.GetStatePointer(CP.PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) == null)  return 0;

            PPBPhyto = GetPPB(CP.PSameSpecies, SVType, T_SVLayer.WaterCol);
            return CP.SedToMe() * PPBPhyto * 1e-6;
            // ug/L   // mg/L    // ug/kg  // kg/mg
        }

        // -------------------------------------------------------------------------
        //   all plants
        // -------------------------------------------------------------------------

        public override void Derivative(ref double DB)
        {
            // Derivitives for Plants, Animals are sent to AnimalDeriv
            TPlant CP;
            double Dep = 0;
            double STH = 0;
            double SFE = 0;
            double BioT_Out = 0;
            double BioT_In = 0;
            double ToxD = 0;
            double Entr = 0;
            double Flt = 0;
            double Pp = 0;
            double WashO = 0;
            double WashI = 0;
            double Lo = 0;
            double Predt = 0;
            double Mort = 0;
            double Sed = 0;
            double Uptake = 0;
            double Exc = 0;
            double Sed2Me = 0;
            double MacBrk = 0;
            double Slgh = 0;
            bool SurfaceFloater;
            // ----------------------------------------------------------------
            CP = AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
            SurfaceFloater = CP.PAlgalRec.SurfaceFloating;
            WashO = 0;
            if (IsAGGR)
            {  DB = 0.0;
                return;
            }

            if ((!(NState >= Consts.FirstPlant && NState <= Consts.LastPlant)))
            {
                throw new Exception("TAlgaeTox must be associated with a plant state variable.");
            }
            else if ((ChemRec.BCFUptake) || (CP.State == 0))
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

                //kg accounting and washin code removed here

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
                // removed washoutstep, estuarysegment, accounting in kg, ToxInCarrierWashin here

                Lo = Loading;

                STH = CP.SinkToHypo * Pp;
                // removed sink from epilimnion code here and surface floater stratification code

                BioT_Out = Biotransformation();
                BioT_In = Biotrans_To_This_Org();
                Predt = CP.Predation() * Pp;
                Mort = CP.Mortality() * Pp;
                Exc = CP.PhotoResp() * Pp;
                ToxD = CP.ToxicDislodge() * Pp;
                Sed = CP.Sedimentation() * Pp;
                // plant sedimentation
                Sed2Me = Derivative_ToxSed2Me(CP);

                if ((CP.IsPeriphyton()))
                    Slgh = CP.Sloughing * Pp;
                else
                    Slgh = -Derivative_ToxToPhytoFromSlough();

                Uptake = PlantUptake();
                Dep = Depuration();

                DB = Lo + Uptake - Dep + WashI - (WashO + Predt + Mort + Sed + Exc + ToxD + Slgh) - BioT_Out + BioT_In - STH + SFE + Flt + Sed2Me + Entr;
                // algae
            }

            // removed phytoplankton current and diffusion (multi-seg variable accounting code)

        }

    } // end TAlgaeTox

    public class TAnimalTox : TToxics { 

        public TAnimalToxRecord Anim_Tox = null;
        public double RecrSave = 0; 

    public TAnimalTox(AllVariables Ns, AllVariables Carry, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, Carry, SVT, L, aName, P, IC)
    {
    }

        // ---------------------------------------------------------------------------
        public override void Derivative(ref double DB)
        {
                // Calculation of Derivs for Inverts and Fish
                TAnimal LgF;
                TAnimal CP;
                double pp = 0;
                double Lo = 0;
                double Gill = 0;
                double Diet = 0;
                double Dep = 0;
                double Predt = 0;
                double Mort = 0;
                double BioT_out = 0;
                double BioT_in = 0;
                double Fi = 0;
                double Entr = 0;
                double Gam = 0;
                double DrifO = 0;
                double DrifI = 0;
                double PLs = 0;
                double PGn = 0;
                double Recr = 0;
                double EmergI = 0;
                double Migr = 0;
                AllVariables BigFishLoop;
                // ----------------------------------------------------------------
                AllVariables LargeCompartment;
                AllVariables SmallCompartment;

                // AnimalDeriv
                CP = ((AQTSeg.GetStatePointer(Carrier, T_SVType.StV, T_SVLayer.WaterCol)) as TAnimal);
                if (ChemRec.BCFUptake)
                {
                    DB = 0;
                    RecrSave = 0;
                }
                else
                {
                    Lo = Loading;
                    pp = GetPPB(NState, SVType, Layer);

                    // Removed Estuary entrainment code, migration code 

                    EmergI = CP.EmergeInsect * pp * 1e-6;

                    // removed accounting in kg code
                
                    DrifO = CP.Drift() * pp * 1e-6; // invertebrates only

                    // removed accounting in kg code and ToxInCarrier Washin

                    BioT_out = Biotransformation();
                    BioT_in = Biotrans_To_This_Org();

                    Gill = CP.GillUptake(SVType, T_SVLayer.WaterCol);

                    // removed porew water gill uptake

                    Diet = SumDietUptake();
                    Dep = Depuration();
                    Predt = CP.Predation() * pp * 1e-6;
                    Mort = CP.Mortality() * pp * 1e-6;
                    Fi = CP.PAnimalData.Fishing_Frac * CP.State * pp * 1e-6;

                    if (CP.PSameSpecies != AllVariables.NullStateVar)
                    {
                        if (!CP.IsSmallFish())
                        {
                            LargeCompartment = CP.NState;
                            SmallCompartment = CP.PSameSpecies;
                        }
                        else
                        {
                            LargeCompartment = CP.PSameSpecies;
                            SmallCompartment = CP.NState;
                        }
                        Recr = CP.Recruit * GetPPB(LargeCompartment, SVType, Layer) * 1e-6;
                        PGn = CP.PromoteGain * GetPPB(SmallCompartment, SVType, Layer) * 1e-6;
                        PLs = CP.PromoteLoss * pp * 1e-6;
                    }
                    if ((CP.OysterCategory > 0))
                    {
                        if (CP.PromoteGain > Consts.Tiny)
                        {
                            SmallCompartment = ((CP.PYounger) as TAnimal).NState;
                            PGn = CP.PromoteGain * GetPPB(SmallCompartment, SVType, Layer) * 1e-6;
                        }
                        if (CP.PromoteLoss > Consts.Tiny)
                        {
                            PLs = CP.PromoteLoss * pp * 1e-6;
                        }
                    }
                    // Calculate Recruitment / Promotion for Multi-Age Fish
                    if ((NState == AllVariables.Fish1))
                    {
                        // Calculate Tox movement through Recruitment to YOY of multi-age fish
                        Recr = 0;
                        for (BigFishLoop = AllVariables.Fish2; BigFishLoop <= AllVariables.Fish15; BigFishLoop++)
                        {
                            if (AQTSeg.GetStatePointer(BigFishLoop, T_SVType.StV, T_SVLayer.WaterCol) != null)
                            {
                                LgF = AQTSeg.GetStatePointer(BigFishLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                                LgF.GameteLoss();
                                // call LgGF^.GameteLoss to get Recruit
                                Recr = Recr - LgF.Recruit * GetPPB(BigFishLoop, SVType, T_SVLayer.WaterCol) * 1e-6;
                                // sum recruitment from larger fish
                            }
                        }
                    }
                    // Fish1
                    if (NState >= AllVariables.Fish2 && NState <= AllVariables.Fish15)
                    {
                        Recr = CP.Recruit * pp * 1e-6;
                    }
                    Gam = CP.GameteLoss() * pp * 1e-6;
                    // Must Be Called After Recr Calculation
                    // + Recr
                    DB = Loading + Gill + Diet - Dep - DrifO + DrifI - BioT_out + BioT_in + Migr - (Predt + Mort + Gam + Fi) + PGn - PLs - EmergI + Entr;
                    if ((AQTSeg.DerivStep == 5)) RecrSave = Recr;
                    // derivstep 5 is time X+h

                    // removed multi-segment diffusion and turbulent diffusion from HMS (pelagic invertebrates)    

                }
                // Not Eutrophication
            }


    } // end TAnimalTox

} 




