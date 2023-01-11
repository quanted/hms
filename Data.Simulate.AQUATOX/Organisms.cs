using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Plants;
using AQUATOX.Bioaccumulation;
using AQUATOX.Animals;
using AQUATOX.Chemicals;
using AQUATOX.OrgMatter;
using AQUATOX.Diagenesis;
using Newtonsoft.Json;
using Globals;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace AQUATOX.Organisms

{
    public class TOrganism : TStateVariable
    {
        public double[] LCInfinite = new double[Consts.NToxs];  // up to 20 toxicants tracked
//        public double[] OrgToxBCF = new double[Consts.NToxs];
        public double[] PrevFracKill = new double[Consts.NToxs];  
        public double[] Resistant = new double[Consts.NToxs];
        public double[,] DeltaCumFracKill = new double[Consts.NToxs, 7];  // Toxicity Tracking Variables
        public double[,] DeltaResistant = new double[Consts.NToxs, 7];  // Toxicity Tracking Variables

        public double[] AmmoniaResistant = new double[2];
        public double[] AmmoniaPrevFracKill = new double[2];

        public double[,] AmmoniaDeltaCumFracKill = new double[2, 7];  // Ammonia EFfects Tracking Variables
        public double[,] AmmoniaDeltaResistant = new double[2, 7];  // Ammonia EFfects Tracking Variables

        public double SedPrevFracKill = 0.0;
        public double SedResistant = 0.0;
        public double[] SedDeltaCumFracKill = new double[7];
        public double[] SedDeltaResistant = new double[7];    // Susp. Sediment EFfects Tracking Variables

        public double[] RedGrowth = new double[Consts.NToxs];
        public double[] RedRepro = new double[Consts.NToxs];
        public double[] FracPhoto = new double[Consts.NToxs];

        public TOrganism(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
         
            int ToxLoop;
            int StepLoop;
            int Ionized;

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                LCInfinite[ToxLoop] = 0;
                for (StepLoop = 1; StepLoop <= 6; StepLoop++)
                {
                    DeltaCumFracKill[ToxLoop, StepLoop] = 0.0;
                    DeltaResistant[ToxLoop, StepLoop] = 0.0;
                }
                Resistant[ToxLoop] = 0.0;
                PrevFracKill[ToxLoop] = 0.0;
//                OrgToxBCF[ToxLoop] = 0.0;
                RedGrowth[ToxLoop] = 0.0;
                RedRepro[ToxLoop] = 0.0;
                FracPhoto[ToxLoop] = 1.0;
            }
            for (Ionized = 0; Ionized <= 1; Ionized++)
            {
                for (StepLoop = 1; StepLoop <= 6; StepLoop++)
                {
                    AmmoniaDeltaCumFracKill[Ionized, StepLoop] = 0.0;
                    AmmoniaDeltaResistant[Ionized, StepLoop] = 0.0;
                }
                AmmoniaResistant[Ionized] = 0.0;
                AmmoniaPrevFracKill[Ionized] = 0.0;
            }
            for (StepLoop = 1; StepLoop <= 6; StepLoop++)
            {
                SedDeltaCumFracKill[StepLoop] = 0.0;
                SedDeltaResistant[StepLoop] = 0.0;
            }
            SedResistant = 0.0;
            SedPrevFracKill = 0.0;

            //LoadsRec.Loadings.ConstLoad = 1e-5;   // for initializing new variable within GUI, not yet implemented in HMS
            //// seed loading
            //if (Ns == AllVariables.Salinity)
            //{
            //    LoadsRec.Loadings.ConstLoad = 0.0;
            //}
        }

        public override void SetToInitCond()
        {
           T_SVType TLP;
           base.SetToInitCond();

           for (TLP = Consts.FirstOrgTxTyp; TLP <= Consts.LastOrgTxTyp; TLP++)
           {
                TToxics TT = AQTSeg.GetStatePointer(NState, TLP, T_SVLayer.WaterCol) as TToxics;
                if (TT!=null)
                    BCF(0, TLP);
           }
        }

        public double PhytoResFactor()
        {
            double result;
            double TotLen;
            double Area_Mi2;
            result = 1.0;
            SiteRecord LL = Location.Locale;
            if (!LL.UsePhytoRetention.Val || ((LL.EnterTotalLength.Val && (LL.TotalLength.Val == 0)) || ((!LL.EnterTotalLength.Val) && (LL.WaterShedArea.Val == 0))))
            {
                return result;
            }
            else
            {
                LL = Location.Locale;
                if (LL.EnterTotalLength.Val)
                {
                    TotLen = LL.TotalLength.Val;
                }
                else
                {
                    Area_Mi2 = LL.WaterShedArea.Val * 0.386;
                    // mi2         // km2
                    TotLen = 1.4 * Math.Pow(Area_Mi2, 0.6);                     // Leopold et al. 1964, p. 145
                    // mi   // mi2
                    TotLen = TotLen * 1.609;
                    // km     // mi
                }
                result = TotLen / LL.SiteLength.Val;
                if (result < 1.0)
                {
                    throw new Exception("Phytoplankton Retention Parameterization Error.  Total site length is less than segment length.");
                }
            }
            return result;
        }


        public override void CalculateLoad(DateTime TimeIndex)  // Inflow Loadings
        {
            // for animals & plants
            double SegVolume = AQTSeg.SegVol(); ;
            double AddLoad;
            double Infl;
            double Wash;
            int Loop; // alt loadings
            Loading = 0.0;  

            base.CalculateLoad(TimeIndex);   // TStateVariable

            Infl = Location.Morph.InflowH2O;  // [VSeg] * (OOSInflowFrac);              // JSC Restore OOSInflowFrac 6/9/2017.  Otherwise inflow organism loadings are non-zero even if there is zero inflow loading to the system.

            //    if (AQTSeg.EstuarySegment)
            //    {
            //        Infl = Location.Morph.InflowH2O[VerticalSegments.Epilimnion] / 2.0;
            //    }    // upstream loadings only, estuary vsn. 10-17-02

            if (Infl > 0.0)
                Loading = Loading * Infl / SegVolume;
             // conc/d // Conc// cu m d  // cu m
            else Loading = 0;

            if (AQTSeg.Convert_g_m2_to_mg_L(NState, SVType, Layer))
            {
                Loading = Loading * Location.Locale.SurfArea.Val / AQTSeg.Volume_Last_Step;  // Convert loading from g/m2 to g/m3 --  Seed loadings only
            }
                        
            if (IsPlant())
            {
                if (((this) as TPlant).IsPhytoplankton())
                {
                    Wash = ((TPlant)this).Washout();
                    Loading = Loading + (Wash - (Wash / PhytoResFactor()));
                    // mg/L  // mg/L   // mg/L  // mg/L     // unitless
                }
            }

            if (IsAnimal())
            {
                if ( (((TAnimal)this).IsPlanktonInvert()) || (NState >= AllVariables.Veliger1 && NState <= AllVariables.Veliger2))
                {
                    Wash = ((TAnimal)this).Drift();
                    Loading = Loading + (Wash - (Wash / PhytoResFactor()));
                    // mg/L   // mg/L  // mg/L  // mg/L      // unitless
                }
            }

            // 10/15/2010 update 10/24/2012 handle fish/animal stocking or time-series fishing or withdrawal
            if ((NState >= Consts.FirstAnimal && NState <= Consts.LastFish) && (!(LoadsRec.Alt_Loadings[0] == null)))
            {
                for (Loop = 0; Loop <= 1; Loop++)  //PointsSource and DirectPrecip
                {
                    // NPS Irrelevant for Fish
                    AddLoad = 0;
                    if (LoadsRec.Alt_Loadings[Loop].UseConstant)
                    {
                        // percent/d or g/sq m. d
                        AddLoad = LoadsRec.Alt_Loadings[Loop].ConstLoad;
                    }
                    else if (LoadsRec.Alt_Loadings[Loop] != null)
                    {
                        AddLoad = LoadsRec.Alt_Loadings[Loop].ReturnTSLoad(TimeIndex);
                    };

                    if (Loop == 1)  // DirectPrecip
                    {
                        AddLoad = AddLoad * LoadsRec.Alt_Loadings[Loop].MultLdg / SegVolume * Location.Locale.SurfArea.Val;
                        // mg/L d   // g/m2 d                // unitless            // cu m                // sq 
                    }
                    else
                    {
                        AddLoad = AddLoad * 0.01 * State * LoadsRec.Alt_Loadings[Loop].MultLdg;          // 4/14/2014 change units, especially relevant for fish/clam/oyster removal
                      // mg/L d  // pct/d frac/pct // mg/L     // unitless
                    };

                    Loading = Loading + AddLoad;
                    // mg/L d          // mg/L d
                }
            }
            // loop
            // inflow loadings

        }

        // Bioconcentration factor, KB
        public virtual double Respiration()
        {
            return 0;
            // see TAnimal.Respiration  TPlant.Respiration
        }


            public void CalcRiskConc_SetOysterCategory(AllVariables ns)
        {
            TAnimal POy;
            POy = AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
            if (POy != null)
            {
                POy.OysterCategory = 0;
                if (POy.NState >= AllVariables.Veliger1 && POy.NState <= AllVariables.Veliger2)
                    POy.OysterCategory = 1;

                else if (POy.NState >= AllVariables.Spat1 && POy.NState <= AllVariables.Spat2)
                    POy.OysterCategory = 2;

                else if (POy.NState >= AllVariables.Clams1 && POy.NState <= AllVariables.Clams4)
                {
                    if (POy.PAnimalData.Guild_Taxa.Val.ToLower().IndexOf("sack") > 0)
                        POy.OysterCategory = 4;

                    else if (POy.PAnimalData.Guild_Taxa.Val.ToLower().IndexOf("seed") > 0)
                        POy.OysterCategory = 3;

                    else if (POy.PSameSpecies == AllVariables.NullStateVar)
                        POy.OysterCategory = 0;

                    else if ((int)(POy.PSameSpecies) > (int)(POy.NState))     // linked to larger clam
                        POy.OysterCategory = 3;

                    else POy.OysterCategory = 4;                              // linked to smaller clam
                }
            }
        }

        public void CalcRiskConc_SetPreyTrophicLevel(AllVariables NS)
        {
            // set trophic levels based on feeding preferences alone, not biomasses or time-dependent feeding
            double PTL, SumPref;
            TAnimal AP, AP2;
            int i;
            TPreference PP;

            AP = AQTSeg.GetStatePointer(NS, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
            AP.ChangeData();
            // reset original preferences
            AP.PreyTrophicLevel = -1;
            SumPref = 0;
            for (i = 0; i < AP.MyPrey.Count; i++)
            {
                PP = AP.MyPrey[i];
                AP2 = AQTSeg.GetStatePointer(PP.nState, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (AP2 != null)
                {
                    if (PP.nState < Consts.FirstInvert)
                    {
                        PTL = 1.0;
                    }
                    else
                    {
                        PTL = AP2.PreyTrophicLevel;
                        if (PTL < 0)
                        {
                            // Circularity found
                            if ((PP.nState == NState))
                            {
                                if (NState < Consts.FirstFish)
                                    AP.PreyTrophicLevel = 2;
                                else
                                    // defaults to resolve circularity
                                    AP.PreyTrophicLevel = 2.5;
                            }
                            else
                            {
                                CalcRiskConc_SetPreyTrophicLevel(PP.nState);
                                PTL = AP2.PreyTrophicLevel;
                            }
                        }
                    }

                    if ((PTL > 0) && (PP.Preference > 0))
                    {
                        if (SumPref == 0)
                              AP.PreyTrophicLevel = PTL + 1;
                        else  AP.PreyTrophicLevel = (((PTL + 1) * PP.Preference) + (SumPref * AP.PreyTrophicLevel)) / (SumPref + PP.Preference);
                        SumPref = SumPref + PP.Preference;
                    }
                }
            }
        }  // SetPreyTrophicLevel


        public void CalcRiskConc_SetOysterData()
        {
            // Set OysterCategory and POlder and PYounger pointers
            AllVariables nsLoop;
            TAnimal POld;
            TAnimal PYng;
            string ErrString;
            ErrString = "";

            TAnimal TA = ((this) as TAnimal);
            TA.POlder = null;
            TA.PYounger = null;

            for (nsLoop = AllVariables.Veliger1; nsLoop <= AllVariables.Clams4; nsLoop++)
                CalcRiskConc_SetOysterCategory(nsLoop);

            if (TA.OysterCategory == 1)
            {
                if (NState == AllVariables.Veliger1)
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Spat1, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;

                if (NState == AllVariables.Veliger2)
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Spat2, T_SVType.StV, T_SVLayer.WaterCol);

                if ((TA.POlder == null) && (NState == AllVariables.Veliger1))
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Spat2, T_SVType.StV, T_SVLayer.WaterCol);

                if ((TA.POlder == null) && (NState == AllVariables.Veliger2))
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Spat1, T_SVType.StV, T_SVLayer.WaterCol);

                for (nsLoop = AllVariables.Clams1; nsLoop <= AllVariables.Clams4; nsLoop++)
                {
                    PYng = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    if (PYng != null)
                    {
                        if ((PYng.OysterCategory == 4))
                        {
                            TA.PYounger = PYng;
                            if (NState == AllVariables.Veliger1)    // assign veliger 1 to sack with lower number
                                break;
                        }
                    }
                }

                if (ErrString != "") throw new Exception(ErrString);

            }

            if (TA.OysterCategory == 2)
            {
                if (NState == AllVariables.Spat1)
                    TA.PYounger = AQTSeg.GetStatePointer(AllVariables.Veliger1, T_SVType.StV, T_SVLayer.WaterCol);

                if (NState == AllVariables.Spat2)
                    TA.PYounger = AQTSeg.GetStatePointer(AllVariables.Veliger2, T_SVType.StV, T_SVLayer.WaterCol);

                if ((TA.PYounger == null) && (NState == AllVariables.Spat1))
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Veliger2, T_SVType.StV, T_SVLayer.WaterCol);

                if ((TA.PYounger == null) && (NState == AllVariables.Spat2))
                    TA.POlder = AQTSeg.GetStatePointer(AllVariables.Veliger1, T_SVType.StV, T_SVLayer.WaterCol);

                for (nsLoop = AllVariables.Clams1; nsLoop <= AllVariables.Clams4; nsLoop++)
                {
                    POld = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    if (POld != null)
                    {
                        if ((POld.OysterCategory == 3))
                        {
                            TA.POlder = POld;
                            if (NState == AllVariables.Spat1)
                                break;
                        }
                    }
                    // assign spat 1 to seed with lower number
                }
                if (TA.POlder == null)
                    ErrString = "Warning. Spat found but no Seed to promote to.";
                if (TA.PYounger == null)
                    ErrString = "Warning. Spat found but no Veliger to promote from.";

                if (ErrString != "") throw new Exception(ErrString);

            }
            if (TA.OysterCategory == 3)
            {
                PYng = AQTSeg.GetStatePointer(AllVariables.Spat1, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (PYng != null)
                {
                    if (PYng.POlder == this)
                        TA.PYounger = PYng;
                }

                PYng = AQTSeg.GetStatePointer(AllVariables.Spat2, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (PYng != null)
                {
                    if (PYng.POlder == this)
                        TA.PYounger = PYng;
                }

                if (TA.PSameSpecies != AllVariables.NullStateVar)
                    TA.POlder = AQTSeg.GetStatePointer(TA.PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol);

                if (TA.POlder == null)
                {
                    for (nsLoop = AllVariables.Clams1; nsLoop <= AllVariables.Clams4; nsLoop++)
                    {
                        POld = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                        if (POld != null)
                        {
                            if ((POld.OysterCategory == 4))
                            {
                                TA.POlder = POld;
                                if (NState == AllVariables.Clams1)
                                    break;

                            }
                        }
                        // assign Clams1 to sack with lower number
                    }
                }
                if (TA.POlder == null)
                    ErrString = "Warning. Seed found but no Sack to promote to.";

                if (TA.PYounger == null)
                    ErrString = "Warning. Seed found but no Spat to promote from.";

                if (ErrString != "") throw new Exception(ErrString);

            }
            if (TA.OysterCategory == 4)
            {
                POld = AQTSeg.GetStatePointer(AllVariables.Veliger1, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (POld != null)
                {
                    if (POld.PYounger == this)
                        TA.POlder = POld;

            }
                POld = AQTSeg.GetStatePointer(AllVariables.Veliger2, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (POld != null)
                {
                    if (POld.PYounger == this)
                        TA.POlder = POld;

                }
                if (TA.PSameSpecies != AllVariables.NullStateVar)
                    TA.PYounger = AQTSeg.GetStatePointer(TA.PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol);

                if (TA.PYounger == null)
                {
                    for (nsLoop = AllVariables.Clams1; nsLoop <= AllVariables.Clams4; nsLoop++)
                    {
                        PYng = AQTSeg.GetStatePointer(nsLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                        if (PYng != null)
                        {
                            if ((POld.OysterCategory == 4))
                            {
                                TA.PYounger = PYng;
                                if (NState == AllVariables.Clams1)
                                    break;

                            }
                        }
                        // assign Clams1 to sack with lower number
                    }
                }
                if (TA.POlder == null)
                    ErrString = "Warning. Seed found but no Sack to promote to.";

                if (TA.PYounger == null)
                    ErrString = "Warning. Seed found but no Spat to promote from.";

                if (ErrString != "") throw new Exception(ErrString);

            }
        }

        // ---------------------------------------------------------------------
        public void CalcRiskConc(bool warn)
        {
            // This procedure should be executed at the beginning of model simulation
            string ErrAnsiString;
            int StepLoop;
            TAnimalToxRecord ATR;
            TPlantToxRecord PTR;
            int ToxLoop;
            double NewTime, LC50_Time, Local_K2, MeanAge;
            TAnimalTox AnimTox;
            TAlgaeTox PlantTox;
            TToxics WaterTox;
            T_SVType ToxTyp;

            if (NState >= AllVariables.Veliger1 && NState <= AllVariables.Clams4)
                CalcRiskConc_SetOysterData();

            for (int Ionized = 0; Ionized <= 1; Ionized++)
            {
                for (StepLoop = 1; StepLoop <= 6; StepLoop++)
                {
                    AmmoniaDeltaCumFracKill[Ionized, StepLoop] = 0;
                    AmmoniaDeltaResistant[Ionized, StepLoop] = 0;
                }
                AmmoniaResistant[Ionized] = 0;
                AmmoniaPrevFracKill[Ionized] = 0;
            }

            for (StepLoop = 1; StepLoop <= 6; StepLoop++)
            {
                SedDeltaCumFracKill[StepLoop] = 0;
                SedDeltaResistant[StepLoop] = 0;
            }

            SedResistant = 0;
            SedPrevFracKill = 0;

            if (IsAnimal()) ((TAnimal)this).Assign_Anim_Tox();
            else ((TPlant)this).Assign_Plant_Tox();

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                ToxTyp = (T_SVType)ToxLoop + 2;
                LCInfinite[ToxLoop] = 0;
                for (StepLoop = 1; StepLoop <= 6; StepLoop++)
                {
                    DeltaCumFracKill[ToxLoop, StepLoop] = 0;
                    DeltaResistant[ToxLoop, StepLoop] = 0;
                }
                PrevFracKill[ToxLoop] = 0;
                Resistant[ToxLoop] = 0;
                RedGrowth[ToxLoop] = 0;
                RedRepro[ToxLoop] = 0;
                FracPhoto[ToxLoop] = 1.0;
                if (IsAnimal())
                {
                    CalcRiskConc_SetPreyTrophicLevel(NState);

                    TAnimal TA = this as TAnimal;
                    TA.HabitatLimit = TA.AHabitat_Limit();
                    
                    AnimTox = AQTSeg.GetStatePointer(NState, ToxTyp, T_SVLayer.WaterCol) as TAnimalTox;
                    WaterTox = AQTSeg.GetStatePointer(AllVariables.H2OTox, ToxTyp, T_SVLayer.WaterCol) as TToxics;

                    if ((AnimTox!=null)&&(WaterTox!=null))
                      { 
                        ATR = TA.Anim_Tox[ToxLoop];

                        Local_K2 = ATR.Entered_K2 + ATR.Bio_rate_const; // 5/11/2015  Add metabolism to k2 for calculations

                        if (Local_K2 > 96)
                            Local_K2 = Local_K2 * (96.0 / Local_K2);  // scaling factor 10-02-03

                        MeanAge = TA.PAnimalData.LifeSpan.Val;
                        if (MeanAge < Consts.Tiny) throw new Exception("lifespan for " + TA.PName + " is set to zero.  This parameter must be set for bioaccumulation calculations.");

                        if (Local_K2 < 0.693 / MeanAge)
                            Local_K2 = 0.693 / MeanAge;  // 9/9/99
                        // Avoid very small LCInfinite for single year classes and YOY

                        // AnimTox.InitialLipid();  // Initialize Lipid before LCINF calculation 10-7-99

                        if (warn && (ATR.LC50 < Consts.Tiny) && (ATR.EC50_repro > 0) && (!AQTSeg.PSetup.UseExternalConcs.Val))
                            ErrAnsiString = "Warning: EC50 Reproduction for " + TA.PName + " within chemical " + WaterTox.ChemRec.ChemName + " is greater than zero, but no reproductive effects will be calculated because LC50 is set to zero. " + " This means that an application factor cannot be calculated (see eqn 420 in the Tech. Doc).";

                        if (warn && (ATR.LC50 < Consts.Tiny) && (ATR.EC50_growth > 0) && (!AQTSeg.PSetup.UseExternalConcs.Val))
                            ErrAnsiString = "Warning: EC50 Growth for " + TA.PName + " within chemical " + WaterTox.ChemRec.ChemName + " is greater than zero, but no growth effects will be calculated because LC50 is set to zero. " + " This means that an application factor cannot be calculated (see eqn 418 in the Tech. Doc).";

                        LC50_Time = ATR.LC50_exp_time;
                        if (warn && (LC50_Time <= 0) && (ATR.LC50 > Consts.Tiny))
                        {
                            if (TA.IsFish())
                                NewTime = 96;
                            else
                                NewTime = 48;
                            ErrAnsiString = "Warning: LC50 Exposure Time for " + TA.PName + " within chemical " + WaterTox.ChemRec.ChemName + " is set to zero.  Replacing zero LC50 Exposure Time with a value of " + (Convert.ToInt64(NewTime)).ToString() + " hours.";
                            LC50_Time = NewTime;
                        }

                        LCInfinite[ToxLoop] = BCF(LC50_Time / 24.0, ToxTyp) * ATR.LC50 * (1.0 - Math.Exp(-Local_K2 * (LC50_Time / 24.0)));
                        // (ppb)             (L/kg)           (h/d)                 (ug/L)                    (1/d)        (h)     (h/d)
                    }
                    // orgtox code
                    // end animal code
                }
                else
                {
                    // Organism is Plant
                    TPlant TPl = this as TPlant;
                    PTR = TPl.Plant_Tox[ToxLoop];

                    TPl.IsEunotia = TPl.PAlgalRec.ScientificName.Val.ToLower().IndexOf("eunotia") > 0;

                    TPl.HabitatLimit = TPl.PHabitat_Limit();

                    PlantTox = AQTSeg.GetStatePointer(NState, ToxTyp, T_SVLayer.WaterCol) as TAlgaeTox;
                    WaterTox = AQTSeg.GetStatePointer(AllVariables.H2OTox, ToxTyp, T_SVLayer.WaterCol) as TToxics;

                    if ((PlantTox != null) && (WaterTox != null))
                    {
                        TPl.Assign_Plant_Tox();


                        Local_K2 = PTR.K2 + PTR.Bio_rate_const; // 5/11/2015  Add metabolism to k2 for calculations

                        if (Local_K2 > 96)
                            Local_K2 = Local_K2 * (96 / Local_K2);   // scaling factor 10-02-03

                        LC50_Time = PTR.LC50_exp_time;
                        if (warn && (LC50_Time <= 0) && (PTR.LC50 > Consts.Tiny))
                        {
                            ErrAnsiString = "Warning: LC50 Exposure Time for " + TPl.PName + " within chemical " + WaterTox.ChemRec.ChemName + " is set to zero.  Replacing zero LC50 Exposure Time with a value of 24 hours.";
                        }
                        if (warn && (PTR.LC50 < Consts.Tiny) && (PTR.EC50_photo > 0) && (!AQTSeg.PSetup.UseExternalConcs.Val))
                        {
                            ErrAnsiString = "Warning: EC50 Photosynthesis for " + TPl.PName + " within chemical " + WaterTox.ChemRec.ChemName + " is greater than zero, but no photosynthesis effects will be calculated because LC50 is set to zero.  This means that an application factor cannot be calculated (see eqn 419 in the Tech. Doc).";
                        }


                        LCInfinite[ToxLoop] = BCF(LC50_Time / 24.0, ToxTyp) * PTR.LC50 * (1.0 - Math.Exp(-Local_K2 * (LC50_Time / 24.0)));
                        // (ppb)             (L/kg)           (h/d)              (ug/L)                    (1/d)        (h)     (h/d)
                    }    // with ChemPtrs
                }  // Plant code
            }    // ToxLoop
        } //     CalcRiskConc

        //// ------------------------------------------------------------------------
        public virtual double BCF(double TElapsed, T_SVType ToxTyp)
        {
            double KB;
            double KPSed;
            double NondissocVal;
            double Lipid;
            double NondissReduc;
            TToxics PT;
            // NLOMFrac,
            double K2;
            UptakeCalcMethodType ChemOption;
            const double DetrOCFrac = 0.526;
            // detritus, Winberg et al., 1971

            PT = AQTSeg.GetStatePointer(AllVariables.H2OTox, ToxTyp, T_SVLayer.WaterCol) as TToxics;
            if (IsAnimal())
                ChemOption = PT.Anim_Method;
            else
                ChemOption = PT.Plant_Method;

            if (ChemOption != UptakeCalcMethodType.Default_Meth)
            {   // -------------------   BCF = K1 / K2 BELOW  ---------------------------------
                if (IsAnimal())
                {
                    TAnimalToxRecord ATR = ((TAnimal)this).Anim_Tox[ToxInt(ToxTyp)];
                    if (ChemOption == UptakeCalcMethodType.CalcBCF)
                    {
                        if ((ATR.Entered_K2 < Consts.Tiny))
                            throw new Exception("K2 values (chemical toxicity screen) must be greater than zero (e.g. " + ATR.Animal_name + ')');
                        else
                            return ATR.Entered_K1 / ATR.Entered_K2;

                    }
                    else  return ATR.Entered_BCF;
                } // end IsAnimal

                if (IsPlant())
                {
                    TPlantToxRecord PTR = ((TPlant)this).Plant_Tox[ToxInt(ToxTyp)];

                    if (ChemOption == UptakeCalcMethodType.CalcBCF)
                    {
                        if ((PTR.K2 < Consts.Tiny))
                            throw new Exception("K2 values (chemical toxicity screen) must be greater than zero (e.g. " + PTR.Plant_name + ')');
                        else
                            return PTR.K1 / PTR.K2;
                    }
                    else return PTR.Entered_BCF;
                }

//                OrgToxBCF[ToxTyp] = result;     // Save for FCM calculation
//                return result;
                // NON Default Methods
            }
            // -------------------   BCF = K1 / K2 ABOVE  ---------------------------------
            
            // Default BCF Calculation Below
            NondissocVal = PT.NonDissoc();
            double Kow = Math.Pow(10, PT.ChemRec.LogKow.Val);
            if (IsAnimal())
            {
                TAnimal PA = ((this) as TAnimal);
                if (IsFish())
                {
                    Lipid = PA.PAnimalData.FishFracLipid.Val;
                    KB = Lipid * WetToDry() * Kow * (NondissocVal + 0.01);
                    // BCF Test 9/8/2010
                    K2 = PA.Anim_Tox[ToxInt(ToxTyp)].Entered_K2;
                    // BCF Test 9/8/2010   Fix this logic, though if LCInfinite is the only thing this is being used for, maybe remove the TElapsed portion
                    if ((K2 < Consts.Tiny))
                         return KB * (1.0 - Math.Exp(-K2 * TElapsed));
                    else return KB;
                }    // fish code

                else if (NState >= Consts.FirstDetrInv && NState <= Consts.LastDetrInv)
                {
                    PT = AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, ToxTyp, T_SVLayer.WaterCol) as TToxics;
                    KPSed = PT.CalculateKOM();  // Use Schwarzenbach Eqn. for KPSED
                    return PA.PAnimalData.FishFracLipid.Val / DetrOCFrac * KPSed; // based on Gobas 1993
                }
                else
                {   // Southworth et al. 1978
                    return WetToDry() * 0.3663 * Math.Pow(Kow, 0.7520) * (NondissocVal + 0.01);
                } // non detritivore detr invert
            }    // animal code

            else
            {
                // Organism is Plant
                NondissReduc = NondissocVal + 0.2;
                if (NondissReduc > 1.0) NondissReduc = 1.0;

                if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
                    return 0.00575 * Math.Pow(Kow, 0.98) * NondissReduc;  // macrophyte, dry weight
                else
                    return NondissocVal * 2.57 * Math.Pow(Kow, 0.93) + (1.0 - NondissocVal) * 0.257 * Math.Pow(Kow, 0.93);
                         // modified from Swackhamer & Skoglund 1993, p. 837 

            }  // plant code
            
            // OrgToxBCF[ToxTyp] = result;
            // Save for FCM calculation
            // return result;
        }

        //// --------------------------------------------------------------------------

        public virtual double Poisoned(T_SVType ToxTyp)
        {
            double result;
            double TElapsed;    // Time elapsed since exposure to toxicant, days
            double LethalConc;  // Time varying LethalConc, ppb
            double Nonresistant, FracKill;
            double LifeSpan;    // Lifespan of animal in days
            double AFGrowth, AFRepro, AFPhoto;
            TToxics WaterTox;
            TAnimalTox AnimTox;
            TAnimalToxRecord ATR = null;
            TAlgaeTox PlantTox;
            TPlantToxRecord PTR = null; 
            double LC50_Local, K2_Local, CumFracNow, NewResist;
            int TInt;

            // --------------------------------------------------------------------------
            void Calculate_Internal_Toxicity()
            {
                double ToxPPB;    // PPB of the toxicant in the organism
                double ToxinOrg;  // Toxicant in the organism
                double Shape;     // unitless param expressing toxic response variability
                double Cum_Frac_Eqn_Part;

                CumFracNow = 0;
                RedGrowth[TInt] = 0.0;
                RedRepro[TInt] = 0.0;
                FracPhoto[TInt] = 1.0;

                if ((LC50_Local < Consts.Tiny)) return;  // 3/23/2012  if LC50 is zero, no effects, application factors are incalculable so no effects on repro, growth, photosynthesis either

                Shape = WaterTox.ChemRec.Weibull_Shape.Val;
                if (Shape <= 0)
                {
                    throw new Exception(WaterTox.ChemRec.ChemName + " Shape Parameter must be greater than zero.");
                }
                ToxinOrg = AQTSeg.GetState(NState, ToxTyp, T_SVLayer.WaterCol);
                if ((ToxinOrg <= 0.0)) return;

                ToxPPB = (ToxinOrg / State) * 1e6;
                // ug/kg         ug/L   /  mg/L     mg/kg

                TElapsed = AQTSeg.CalculateTElapsed(TInt);
                if ((LifeSpan > 0) && (TElapsed > LifeSpan))
                    TElapsed = LifeSpan;    // JSC 8-12-2003

                if ((ToxPPB <= 0.0) || (TElapsed == 0.0)) return;

                if ((K2_Local < Consts.Tiny))
                    LethalConc = LCInfinite[TInt];
                else if (TElapsed > 4.605 / K2_Local)
                    LethalConc = LCInfinite[TInt];
                else if ((1.0 - Math.Exp(-K2_Local * TElapsed)) < 0.0001)                 // bullet proof
                    LethalConc = LCInfinite[TInt] / 0.0001;
                else
                    LethalConc = LCInfinite[TInt] / (1.0 - Math.Exp(-K2_Local * TElapsed));
                // ppb           // ppb                             // 1/d         // d

                if (LethalConc == 0)
                {
                    if ((ToxPPB > 0)) CumFracNow = 1;
                    else              CumFracNow = 0;
                }
                else if ((ToxPPB / LethalConc > 70)) CumFracNow = 1;

                else
                {
                    // Weibull cumulative equation
                    Cum_Frac_Eqn_Part = -Math.Pow(ToxPPB / LethalConc, 1.0 / Shape);
                    // ppb / ppb         unitless
                    CumFracNow = 1.0 - Math.Exp(Cum_Frac_Eqn_Part);

                    if (CumFracNow > 0.95)  CumFracNow = 1.0;

                    if (IsAnimal())
                    {
                        if (AFGrowth < Consts.Tiny) RedGrowth[TInt] = 0;
                        else                        RedGrowth[TInt] = 1.0 - Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFGrowth), 1.0 / Shape));

                        if (AFRepro < Consts.Tiny)  RedRepro[TInt] = 0;
                        else                        RedRepro[TInt] = 1.0 - Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFRepro), 1.0 / Shape));
                    } 
                    else 
                      if (AFPhoto < Consts.Tiny) FracPhoto[TInt] = 1;  // plant
                      else FracPhoto[TInt] = Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFPhoto), 1.0 / Shape));
                }
            }

            // Calculate Internal Toxicity
            // --------------------------------------------------------------------------
            void Calculate_External_Toxicity()
            {
                double k;
                double ETA;
                double SlopeFactor;
                double Slope;
                double ToxinH2O;        // Toxicant in the water
                CumFracNow = 0;
                RedGrowth[TInt] = 0;  // 5/3/2017 defaults for no effects if tox is zero
                RedRepro[TInt] = 0;
                FracPhoto[TInt] = 1;

                SlopeFactor = WaterTox.ChemRec.WeibullSlopeFactor.Val;
                if (IsAnimal())
                    if ((ATR.LC50_Slope > Consts.Tiny))
                        SlopeFactor = ATR.LC50_Slope;  // JSC 12/14/2016 allow for animal-chemical-specific slope factor
                
                if (IsPlant())
                    if ((PTR.LC50_Slope > Consts.Tiny))
                        SlopeFactor = PTR.LC50_Slope; // JSC 12/14/2016 allow for plant-chemical-specific slope factor
                
                ToxinH2O = WaterTox.State;
                if (ToxinH2O < 0)  return;

                if (LC50_Local > Consts.Tiny)
                {
                    if (SlopeFactor <= 0) throw new Exception(WaterTox.PName + " Weibull Slope Factor must be greater than zero.");

                    Slope = SlopeFactor / LC50_Local;
                // unitless = (LC50*Slope)/(LC50)

                    ETA = (-2 * LC50_Local * Slope) / Math.Log(0.5);
                    try
                    {
                        k = -Math.Log(0.5) / Math.Pow(LC50_Local, ETA);
                        CumFracNow = 1.0 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
                    }
                    catch
                    {
                        throw new Exception("Floating Point Error Calculating External Toxicity.  Re-examine input parameters.");
                    }
                }

                if (IsAnimal())
                {
                    TToxics WaterTox = AQTSeg.GetStatePointer(AllVariables.H2OTox, ToxTyp, T_SVLayer.WaterCol) as TToxics;

                    // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Growth

                    if (ATR.EC50_growth < Consts.Tiny)
                        RedGrowth[TInt] = 0;

                    else
                    {
                        if ((ATR.EC50_Growth_Slope > Consts.Tiny))
                            // JSC 4/5/2017 allow for animal-chemical-effect specific slope factor
                            Slope = ATR.EC50_Growth_Slope / ATR.EC50_growth;
                        else
                            Slope = SlopeFactor / ATR.EC50_growth;
                        if (Slope <= 0)
                            throw new Exception(WaterTox.PName + " EC50 Growth Weibull Slope Factor must be greater than zero.");

                        ETA = (-2 * ATR.EC50_growth * Slope) / Math.Log(0.5);
                        try
                        {
                            k = -Math.Log(0.5) / Math.Pow(ATR.EC50_growth, ETA);
                            RedGrowth[TInt] = 1.0 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
                        }
                        catch
                        {
                            throw new Exception("Floating Point Error Calculating Growth Limitations due to External Toxicity.  Re-examine input parameters.");
                        }
                    }
                    // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Repro
                    if (ATR.EC50_repro < Consts.Tiny)
                    {
                        RedRepro[TInt] = 0;
                    }
                    else
                    {
                        if ((ATR.EC50_Repro_Slope > Consts.Tiny))
                        {
                            // JSC 4/5/2017 allow for animal-chemical-effect specific slope factor
                            Slope = ATR.EC50_Repro_Slope / ATR.EC50_repro;
                        }
                        else
                            Slope = SlopeFactor / ATR.EC50_repro;
                        if (Slope <= 0)
                            throw new Exception(WaterTox.PName + " EC50 Repro Weibull Slope Factor must be greater than zero.");

                        ETA = (-2 * ATR.EC50_repro * Slope) / Math.Log(0.5);
                        try
                        {
                            k = -Math.Log(0.5) / Math.Pow(ATR.EC50_repro, ETA);
                            RedRepro[TInt] = 1.0 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
                        }
                        catch
                        {
                            throw new Exception("Floating Point Error Calculating Reproductive Limitations due to External Toxicity.  Re-examine input parameters.");
                        }
                    }
                }  // end animal code
                else
                {
                    // IsPlant
                    if (PTR.EC50_photo < Consts.Tiny)
                    {
                        FracPhoto[TInt] = 1.0;
                    }
                    else
                    {
                        // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Repro  //10/8/2014 JSC FracPhoto should be set to 1.0 for no effect
                        if ((PTR.EC50_Photo_Slope > Consts.Tiny))
                        {
                            // JSC 4/5/2017 allow for plant-chemical-effect specific slope factor
                            Slope = PTR.EC50_Photo_Slope / PTR.EC50_photo;
                        }
                        else Slope = SlopeFactor / PTR.EC50_photo;

                        if (Slope <= 0) throw new Exception(WaterTox.PName + " EC50 Photo Weibull Slope Factor must be greater than zero.");

                        ETA = (-2 * PTR.EC50_photo * Slope) / Math.Log(0.5);
                        try
                        {
                            k = -Math.Log(0.5) / Math.Pow(PTR.EC50_photo, ETA);
                            FracPhoto[TInt] = Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
                        }
                        catch
                        {
                            throw new Exception("Floating Point Error Calculating Photosynthesis Limitations due to External Toxicity.  Re-examine input parameters.");
                        }
                    }
                }
            }
// ------------------------------------------------------------------------------------------------------------------------------------------------
// Start Poisoned Code

            AFGrowth = 0;
            AFRepro = 0.0;
            AFPhoto = 0.0;
            if ((!AQTSeg.PSetup.UseExternalConcs.Val) && (State == 0)) return 0.0;
            if (!(NState >= Consts.FirstBiota && NState <= Consts.LastBiota)) return 0.0;
            TInt = ToxInt(ToxTyp);
            WaterTox = AQTSeg.GetStatePointer(AllVariables.H2OTox, ToxTyp, T_SVLayer.WaterCol) as TToxics;
            if (WaterTox == null) return 0.0;

            if (IsAnimal())
            {
                AnimTox = AQTSeg.GetStatePointer(NState, ToxTyp, T_SVLayer.WaterCol) as TAnimalTox;
                if ((!AQTSeg.PSetup.UseExternalConcs.Val) && (AnimTox == null)) return 0.0;
                ATR = ((TAnimal)this).Anim_Tox[ToxInt(ToxTyp)];
                if (ATR == null) return 0.0;

                PlantTox = null;

                if (ATR.LC50 <= 0.0)
                {
                    AFGrowth = 0.0;
                    AFRepro = 0.0;
                }
                else
                {
                    AFGrowth = ATR.EC50_growth / ATR.LC50;
                    AFRepro = ATR.EC50_repro / ATR.LC50;
                }
                LC50_Local = ATR.LC50;
                K2_Local = ATR.Entered_K2;
                K2_Local = K2_Local + ATR.Bio_rate_const;  // 5/11/2015  Add metabolism to k2 for calculations
                LifeSpan = ((TAnimal)this).PAnimalData.LifeSpan.Val;
            }
            else // is plant
            {
                PlantTox = AQTSeg.GetStatePointer(NState, ToxTyp, T_SVLayer.WaterCol) as TAlgaeTox;
                if ((!AQTSeg.PSetup.UseExternalConcs.Val) && (PlantTox == null)) return 0.0;
                PTR = ((TPlant)this).Plant_Tox[ToxInt(ToxTyp)];
                if (PTR == null) return 0.0;

                AnimTox = null;

                if (PTR.LC50 <= 0.0)
                    AFPhoto = 0.0;
                else
                    AFPhoto = PTR.EC50_photo / PTR.LC50;

                LC50_Local = PTR.LC50;
                K2_Local = PTR.K2;
                K2_Local = K2_Local + PTR.Bio_rate_const;   // 5/11/2015  Add metabolism to k2 for calculations
                LifeSpan = 0.0;
            }

            if (!AQTSeg.PSetup.UseExternalConcs.Val)  Calculate_Internal_Toxicity();
               else Calculate_External_Toxicity();

            if (CumFracNow <= 0.0) return 0.0;

            Nonresistant = State * (1.0 - Resistant[TInt]);            // 9-14-07 conversion of Resistant from biomass units to fraction units
            // mg/L          mg/L          frac

            // if (CumFracNow == 1.0) FracKill = 1.0; else   JSC Test May 2020

            if ((PrevFracKill[TInt] >= CumFracNow) || ((1.0 - PrevFracKill[TInt])<Consts.Tiny))
                FracKill = 0.0;
            else
                FracKill = (CumFracNow - PrevFracKill[TInt]) / (1.0 - PrevFracKill[TInt]);

            result = Resistant[TInt] * State * FracKill + Nonresistant * CumFracNow;
            // mg/L-d        frac             mg/L     g/g-d      mg/L            g/g-d
            NewResist = (State - result) / State;
            // frac = (mg/L) - (mg/L) / (mg/L)

            DeltaResistant[TInt, AQTSeg.DerivStep]   = NewResist - Resistant[TInt];
            DeltaCumFracKill[TInt, AQTSeg.DerivStep] = CumFracNow - PrevFracKill[TInt];
            return result;
        }

        // ------------------------------------------------------------------------

        // Chronic Effects
        public double GutEffOrgTox()
        {

            if ((NState >= Consts.FirstInvert && NState <= Consts.LastInvert))
            {
   
                return 0.35;               // Landrum et al. 1990
            }
            else if ((NState >= AllVariables.LgGameFish1 && NState <= AllVariables.LgGameFish4))
                  return 0.92;
             else return 0.62;
        }

        public double SalMort(double Min, double Max, double Coeff1, double Coeff2)
        {
            double Salt;
            TSalinity PSalt = (TSalinity)AQTSeg.GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);

            if (PSalt == null) Salt = -1.0;
                          else Salt = PSalt.State;

            if (Salt == -1) return 0.0;
            if ((Salt >= Min) && (Salt <= Max)) return 0.0;

            else if (Salt < Min)
            {
                return Coeff1 * Math.Exp(Min - Salt);
            }
            else
            {   // Salt>Max
                return Coeff2 * Math.Exp(Salt - Max);
            }
        }

        public virtual double Mortality()
        {   // overridden by animal and plant methods
            return 0.0;
        }
    } // end TOrganism
       
}  // namespace
