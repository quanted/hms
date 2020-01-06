using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Plants;
using AQUATOX.Animals;
using AQUATOX.OrgMatter;
using AQUATOX.Diagenesis;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.Organisms

{
    public class TOrganism : TStateVariable
    {
        public double[] LCInfinite = new double[Consts.NToxs];  // up to 20 toxicants tracked
        public double[] OrgToxBCF = new double[Consts.NToxs];
        public double[] PrevFracKill = new double[Consts.NToxs];
        public double[] Resistant = new double[Consts.NToxs];
        public double[,] DeltaCumFracKill = new double[Consts.NToxs, 7];  // Toxicity Tracking Variables
        public double[,] DeltaResistant = new double[Consts.NToxs, 7];  // Toxicity Tracking Variables

        public double[] AmmoniaResistant = new double[2];
        public double[] AmmoniaPrevFracKill = new double[2];

        public double[,] AmmoniaDeltaCumFracKill = new double[2, 7];  // Ammonia EFfects Tracking Variables
        public double[,] AmmoniaDeltaResistant = new double[2, 7];  // Ammonia EFfects Tracking Variables

        public double SedPrevFracKill = 0;
        public double SedResistant = 0;
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
                    DeltaCumFracKill[ToxLoop, StepLoop] = 0;
                    DeltaResistant[ToxLoop, StepLoop] = 0;
                }
                Resistant[ToxLoop] = 0;
                PrevFracKill[ToxLoop] = 0;
                OrgToxBCF[ToxLoop] = 0;
                RedGrowth[ToxLoop] = 0;
                RedRepro[ToxLoop] = 0;
                FracPhoto[ToxLoop] = 1.0;
            }
            for (Ionized = 0; Ionized <= 1; Ionized++)
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

            //LoadsRec.Loadings.ConstLoad = 1e-5;   // for initializing new variable within GUI, not yet implemented in HMS
            //// seed loading
            //if (Ns == AllVariables.Salinity)
            //{
            //    LoadsRec.Loadings.ConstLoad = 0;
            //}
        }

        public double PhytoResFactor()
        {
            double result;
            double TotLen;
            double Area_Mi2;
            result = 1.0;
            SiteRecord LL = Location.Locale;
            if (!LL.UsePhytoRetention || ((LL.EnterTotalLength && (LL.TotalLength == 0)) || ((!LL.EnterTotalLength) && (LL.WaterShedArea == 0))))
            {
                return result;
            }
            else
            {
                LL = Location.Locale;
                if (LL.EnterTotalLength)
                {
                    TotLen = LL.TotalLength;
                }
                else
                {
                    Area_Mi2 = LL.WaterShedArea * 0.386;
                    // mi2         // km2
                    TotLen = 1.4 * Math.Pow(Area_Mi2, 0.6);                     // Leopold et al. 1964, p. 145
                    // mi   // mi2
                    TotLen = TotLen * 1.609;
                    // km     // mi
                }
                result = TotLen / LL.SiteLength;
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
            Loading = 0;  

            base.CalculateLoad(TimeIndex);   // TStateVariable

            Infl = Location.Morph.InflowH2O;  // [VSeg] * (OOSInflowFrac);              // JSC Restore OOSInflowFrac 6/9/2017.  Otherwise inflow organism loadings are non-zero even if there is zero inflow loading to the system.

            //    if (AQTSeg.EstuarySegment)
            //    {
            //        Infl = Location.Morph.InflowH2O[VerticalSegments.Epilimnion] / 2;
            //    }    // upstream loadings only, estuary vsn. 10-17-02

            if (Infl > 0.0)
                Loading = Loading * Infl / SegVolume;
             // conc/d // Conc// cu m d  // cu m
            else Loading = 0;

            if (AQTSeg.Convert_g_m2_to_mg_L(NState, SVType, Layer))
            {
                Loading = Loading * Location.Locale.SurfArea / AQTSeg.Volume_Last_Step;  // Convert loading from g/m2 to g/m3 --  Seed loadings only
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
                        AddLoad = AddLoad * LoadsRec.Alt_Loadings[Loop].MultLdg / SegVolume * Location.Locale.SurfArea;
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
                    if (POy.PAnimalData.Guild_Taxa.ToLower().IndexOf("sack") > 0)
                        POy.OysterCategory = 4;

                    else if (POy.PAnimalData.Guild_Taxa.ToLower().IndexOf("seed") > 0)
                        POy.OysterCategory = 3;

                    else if (POy.PSameSpecies == AllVariables.NullStateVar)
                        POy.OysterCategory = 0;

                    else if ((int)(POy.PSameSpecies) > (int)(POy.NState))     // linked to larger clam
                        POy.OysterCategory = 3;

                    else POy.OysterCategory = 4;                              // linked to smaller clam
                }
            }
        }

        //public void CalcRiskConc_SetPreyTrophicLevel(AllVariables NS)
        //{
        //    // set trophic levels based on feeding preferences alone, not biomasses or time-dependent feeding
        //    double PTL;
        //    double SumPref;
        //    TAnimal AP;
        //    TAnimal AP2;
        //    int i;
        //    TPreference PP;
        //    AP = GetStatePointer(NS, T_SVType.StV, T_SVLayer.WaterCol);
        //    AP.ChangeData();
        //    // reset original preferences
        //    AP.PreyTrophicLevel = -1;
        //    SumPref = 0;
        //    for (i = 0; i < AP.MyPrey.Count; i++)
        //    {
        //        PP = ((AP.MyPrey.At(i)) as TPreference);
        //        AP2 = GetStatePointer(PP.nState, T_SVType.StV, T_SVLayer.WaterCol);
        //        if (AP2 != null)
        //        {
        //            if (PP.nState < Consts.FirstInvert)
        //            {
        //                PTL = 1.0;
        //            }
        //            else
        //            {
        //                PTL = AP2.PreyTrophicLevel;
        //                if (PTL < 0)
        //                {
        //                    // Circularity found
        //                    if ((PP.nState == NState))
        //                    {
        //                        if (NState < Consts.FirstFish)
        //                        {
        //                            AP.PreyTrophicLevel = 2;
        //                        }
        //                        else
        //                        {
        //                            // defaults to resolve circularity
        //                            AP.PreyTrophicLevel = 2.5;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        CalcRiskConc_SetPreyTrophicLevel(PP.nState);
        //                        PTL = AP2.PreyTrophicLevel;
        //                    }
        //                }
        //            }
        //            if ((PTL > 0) && (PP.Preference > 0))
        //            {
        //                if (SumPref == 0)
        //                {
        //                    AP.PreyTrophicLevel = PTL + 1;
        //                }
        //                else
        //                {
        //                    AP.PreyTrophicLevel = (((PTL + 1) * PP.Preference) + (SumPref * AP.PreyTrophicLevel)) / (SumPref + PP.Preference);
        //                }
        //                SumPref = SumPref + PP.Preference;
        //            }
        //        }
        //    }
        //}  // SetPreyTrophicLevel


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
        //    // This procedure should be executed at the beginning of model
        //    // run and also when stratification occurs
        //    string[] DataName;
        //    string[] ToxRecName;
        //    string ErrAnsiString;
        //    object PTR;
        //    bool Ionized;
        //    int StepLoop;
        //    int i;
        //    int FoundToxIndx;
        //    TAnimalToxRecord ATR;
        //    TPlantToxRecord PlantTox;
        //    T_SVType ToxLoop;
        //    double NewTime;
        //    double LC50_Time;
        //    double Local_K2;
        //    double MeanAge;
        //    TToxics AnimTox;
        //    
        //    PTR = this;
            if (NState >= AllVariables.Veliger1 && NState <= AllVariables.Clams4)
                CalcRiskConc_SetOysterData();

        //    for (Ionized = false; Ionized <= true; Ionized++)
        //    {
        //        for (StepLoop = 1; StepLoop <= 6; StepLoop++)
        //        {
        //            AmmoniaDeltaCumFracKill[Ionized, StepLoop] = 0;
        //            AmmoniaDeltaResistant[Ionized, StepLoop] = 0;
        //        }
        //        AmmoniaResistant[Ionized] = 0;
        //        AmmoniaPrevFracKill[Ionized] = 0;
        //    }
        //    for (StepLoop = 1; StepLoop <= 6; StepLoop++)
        //    {
        //        SedDeltaCumFracKill[StepLoop] = 0;
        //        SedDeltaResistant[StepLoop] = 0;
        //    }
        //    SedResistant = 0;
        //    SedPrevFracKill = 0;
        //    for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop < Consts.LastOrgTxTyp; ToxLoop++)
        //    {
        //        LCInfinite[ToxLoop] = 0;
        //        for (StepLoop = 1; StepLoop <= 6; StepLoop++)
        //        {
        //            DeltaCumFracKill[ToxLoop, StepLoop] = 0;
        //            DeltaResistant[ToxLoop, StepLoop] = 0;
        //        }
        //        PrevFracKill[ToxLoop] = 0;
        //        Resistant[ToxLoop] = 0;
        //        RedGrowth[ToxLoop] = 0;
        //        RedRepro[ToxLoop] = 0;
        //        FracPhoto[ToxLoop] = 1.0;
        //        if (IsAnimal())
        //        {
        //            CalcRiskConc_SetPreyTrophicLevel(NState);
        //            DataName = ((PTR) as TAnimal).PAnimalData.ToxicityRecord.ToLower();
        //            TAnimal 6 = ((PTR) as TAnimal);
        //            6.HabitatLimit = 6.AHabitat_Limit();
        //            if (GetStatePointer(Consts.AssocToxSV(ToxLoop), T_SVType.StV, T_SVLayer.WaterCol) != null)
        //            {
        //                FoundToxIndx = -1;
        //                for (i = 0; i < Chemptrs[ToxLoop].Anim_Tox.Count; i++)
        //                {
        //                    ATR = Chemptrs[ToxLoop].Anim_Tox.At(i);
        //                    ToxRecName = ATR.Animal_name.ToLower();
        //                    if ((ToxRecName == DataName))
        //                    {
        //                        FoundToxIndx = i;
        //                    }
        //                }
        //                if (FoundToxIndx == -1)
        //                {
        //                    throw new Exception("Fatal Parameterization Error:  " + Consts.OutputText(NState, SVType, T_SVLayer.WaterCol, "", false, false, 0) + " uses the toxicity record \"" + DataName + "\" which is not found in the chemical " + Chemptrs[ToxLoop].ChemRec.ChemName + " animal toxicity data.  Study cannot be executed.");
        //                }
        //                ATR = Chemptrs[ToxLoop].Anim_Tox.At(FoundToxIndx);
        //                Local_K2 = ATR.Entered_K2 + ATR.Bio_rate_const;
        //                // 5/11/2015  Add metabolism to k2 for calculations
        //                if (Local_K2 > 96)
        //                {
        //                    Local_K2 = Local_K2 * (96 / Local_K2);
        //                }
        //                // scaling factor 10-02-03
        //                MeanAge = ((PTR) as TAnimal).PAnimalData.LifeSpan;
        //                if (MeanAge < Consts.Tiny)
        //                {
        //                    throw new Exception("lifespan for " + ((PTR) as TAnimal).PName + " is set to zero.  This parameter must be set for bioaccumulation calculations.");
        //                }
        //                if (Local_K2 < 0.693 / MeanAge)
        //                {
        //                    Local_K2 = 0.693 / MeanAge;
        //                }
        //                // 9/9/99
        //                // Avoid very small LCInfinite for single year classes and YOY
        //                AnimTox = GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol);
        //                AnimTox.InitialLipid();
        //                // Initialize Lipid before LCINF calculation 10-7-99
        //                if (warn && (ATR.LC50 < Consts.Tiny) && (ATR.EC50_repro > 0) && (!AQTSeg.SetupRec.UseExternalConcs))
        //                {
        //                    TStates 7 = AQTSeg;
        //                    ErrAnsiString = "Warning: EC50 Reproduction for " + ((PTR) as TAnimal).PName + " within chemical " + 7.Chemptrs[ToxLoop].ChemRec.ChemName + " is greater than zero, but no reproductive effects will be calculated because LC50 is set to zero. " + " This means that an application factor cannot be calculated (see eqn 420 in the Tech. Doc).";
        //                    7.PMessageStr = ErrAnsiString;
        //                    7.PMessageErr = true;
        //                    7.TSMessage;
        //                }
        //                if (warn && (ATR.LC50 < Consts.Tiny) && (ATR.EC50_growth > 0) && (!AQTSeg.SetupRec.UseExternalConcs))
        //                {
        //                    TStates 8 = AQTSeg;
        //                    ErrAnsiString = "Warning: EC50 Growth for " + ((PTR) as TAnimal).PName + " within chemical " + 8.Chemptrs[ToxLoop].ChemRec.ChemName + " is greater than zero, but no growth effects will be calculated because LC50 is set to zero. " + " This means that an application factor cannot be calculated (see eqn 418 in the Tech. Doc).";
        //                    8.PMessageStr = ErrAnsiString;
        //                    8.PMessageErr = true;
        //                    8.TSMessage;
        //                }
        //                LC50_Time = ATR.LC50_exp_time;
        //                if (warn && (LC50_Time <= 0) && (ATR.LC50 > Consts.Tiny))
        //                {
        //                    TStates 9 = AQTSeg;
        //                    if (((PTR) as TAnimal).IsFish())
        //                    {
        //                        NewTime = 96;
        //                    }
        //                    else
        //                    {
        //                        NewTime = 48;
        //                    }
        //                    ErrAnsiString = "Warning: LC50 Exposure Time for " + ((PTR) as TAnimal).PName + " within chemical " + 9.Chemptrs[ToxLoop].ChemRec.ChemName + " is set to zero.  Replacing zero LC50 Exposure Time with a value of " + (Convert.ToInt64(NewTime)).ToString() + " hours.";
        //                    9.PMessageStr = ErrAnsiString;
        //                    9.PMessageErr = true;
        //                    9.TSMessage;
        //                    LC50_Time = NewTime;
        //                }
        //                ((PTR) as TAnimal).Assign_Anim_Tox();
        //                LCInfinite[ToxLoop] = BCF(LC50_Time / 24.0, ToxLoop) * ATR.LC50 * (1 - Math.Exp(-Local_K2 * (LC50_Time / 24.0)));
        //                // ppb
        //                // L/kg
        //                // h/d
        //                // ug/L
        //                // 1/d
        //                // h
        //                // h/d
        //            }
        //            // orgtox code
        //            // end animal code
        //        }
        //        else
        //        {
        //            // Organism is Plant
        //            ((PTR) as TPlant).IsEunotia = ((PTR) as TPlant).PAlgalRec.ScientificName.ToLower().IndexOf("eunotia") > 0;
        //            DataName = ((PTR) as TPlant).PAlgalRec.ToxicityRecord.ToLower();
        //            TPlant 10 = ((PTR) as TPlant);
        //            10.HabitatLimit = 10.PHabitat_Limit();
        //            if (GetStatePointer(Consts.AssocToxSV(ToxLoop), T_SVType.StV, T_SVLayer.WaterCol) != null)
        //            {
        //                TChemical 11 = Chemptrs[ToxLoop];
        //                FoundToxIndx = -1;
        //                for (i = 0; i < 11.Plant_Tox.Count; i++)
        //                {
        //                    PlantTox = 11.Plant_Tox.At(i);
        //                    if (PlantTox.Plant_name.ToLower() == DataName)
        //                    {
        //                        FoundToxIndx = i;
        //                    }
        //                }
        //                if (FoundToxIndx == -1)
        //                {
        //                    throw new Exception("Fatal Parameterization Error:  " + Consts.OutputText(NState, SVType, T_SVLayer.WaterCol, "", false, false, 0) + " uses the toxicity record \"" + DataName + "\" which is not found in the chemical " + Chemptrs[ToxLoop].ChemRec.ChemName + " plant toxicity data.  Study cannot be executed.");
        //                }
        //                PlantTox = Chemptrs[ToxLoop].Plant_Tox.At(FoundToxIndx);
        //                ((PTR) as TPlant).Plant_Tox[ToxLoop] = PlantTox;
        //                Local_K2 = PlantTox.K2;
        //                Local_K2 = Local_K2 + PlantTox.Bio_rate_const;
        //                // 5/11/2015  Add metabolism to k2 for calculations
        //                if (Local_K2 > 96)
        //                {
        //                    Local_K2 = Local_K2 * (96 / Local_K2);
        //                }
        //                // scaling factor 10-02-03
        //                // (*If TPlant(Ptr)^.PAlgalRec^.PlantType='Macrophytes' then MeanAge := 120
        //                // else MeanAge := 30;
        //                // 
        //                // If Local_K2 < 0.693 / MeanAge then Local_K2 := 0.693/MeanAge;  {9/9/99}
        //                // {Avoid very small LCInfinite for single year classes and YOY} *)
        //                LC50_Time = PlantTox.LC50_exp_time;
        //                if (warn && (LC50_Time <= 0) && (PlantTox.LC50 > Consts.Tiny))
        //                {
        //                    TStates 12 = AQTSeg;
        //                    12.PMessageStr = "Warning: LC50 Exposure Time for " + ((PTR) as TPlant).PName + " within chemical " + 12.Chemptrs[ToxLoop].ChemRec.ChemName + " is set to zero.  Replacing zero LC50 Exposure Time with a value of 24 hours.";
        //                    12.PMessageErr = true;
        //                    12.TSMessage;
        //                    LC50_Time = 24;
        //                }
        //                if (warn && (PlantTox.LC50 < Consts.Tiny) && (PlantTox.EC50_photo > 0) && (!AQTSeg.SetupRec.UseExternalConcs))
        //                {
        //                    TStates 13 = AQTSeg;
        //                    ErrAnsiString = "Warning: EC50 Photosynthesis for " + ((PTR) as TPlant).PName + " within chemical " + 13.Chemptrs[ToxLoop].ChemRec.ChemName + " is greater than zero, but no photosynthesis effects will be calculated because LC50 is set to zero. " + " This means that an application factor cannot be calculated (see eqn 419 in the Tech. Doc).";
        //                    13.PMessageStr = ErrAnsiString;
        //                    13.PMessageErr = true;
        //                    13.TSMessage;
        //                }
        //                LCInfinite[ToxLoop] = BCF(LC50_Time / 24.0, ToxLoop) * PlantTox.LC50 * (1 - Math.Exp(-Local_K2 * (LC50_Time / 24.0)));
        //                // ppb
        //                // L/kg
        //                // h/d
        //                // ug/L
        //                // 1/d
        //                // h
        //                // h/d
        //            }
        //            // with ChemPtrs
        //        }
        //        // Plant code
        //    }
        //    // ToxLoop

        } //     CalcRiskConc

        //// ------------------------------------------------------------------------
        //public virtual double BCF(double TElapsed, object ToxTyp)
        //{
        //    double result;
        //    double KB;
        //    double KOW;
        //    double KPSed;
        //    double NondissocVal;
        //    double Lipid;
        //    double NondissReduc;
        //    TAnimal PA;
        //    TPlant PP;
        //    TToxics PT;
        //    // NLOMFrac,
        //    double K2;
        //    UptakeCalcMethodType ChemOption;
        //    const double DetrOCFrac = 0.526;
        //    // detritus, Winberg et al., 1971
        //    ChemicalRecord 1 = Chemptrs[ToxTyp].ChemRec;

        //    if (IsAnimal())
        //    {
        //        ChemOption = Chemptrs[ToxTyp].Anim_Method;
        //    }
        //    else
        //    {
        //        ChemOption = Chemptrs[ToxTyp].Plant_Method;
        //    }
        //    if (ChemOption != UptakeCalcMethodType.Default_Meth)
        //    {
        //        // -------------------   BCF = K1 / K2 BELOW  ---------------------------------
        //        result = 0;
        //        if (IsAnimal())
        //        {
        //            PA = ((this) as TAnimal);
        //            TAnimalToxRecord 3 = PA.Anim_Tox[ToxTyp];
        //            if (ChemOption == UptakeCalcMethodType.CalcBCF)
        //            {
        //                if ((3.Entered_K2 < Consts.Tiny))
        //                {
        //                    throw new Exception("K2 values (chemical toxicity screen) must be greater than zero (e.g. " + 3.Animal_name + ')');
        //                }
        //                else
        //                {
        //                    result = 3.Entered_K1 / 3.Entered_K2;
        //                }
        //            }
        //            else
        //            {
        //                result = 3.Entered_BCF;
        //            }
        //        }
        //        if (IsPlant())
        //        {
        //            PP = ((this) as TPlant);
        //            TPlantToxRecord 4 = PP.Plant_Tox[ToxTyp];
        //            if (ChemOption == UptakeCalcMethodType.CalcBCF)
        //            {
        //                if ((4.K2 < Consts.Tiny))
        //                {
        //                    throw new Exception("K2 values (chemical toxicity screen) must be greater than zero (e.g. " + 4.Plant_name + ')');
        //                }
        //                else
        //                {
        //                    result = 4.K1 / 4.K2;
        //                }
        //            }
        //            else
        //            {
        //                result = 4.Entered_BCF;
        //            }
        //        }
        //        OrgToxBCF[ToxTyp] = result;
        //        // Save for FCM calculation
        //        return result;
        //        // NON Default Methods
        //    }
        //    // -------------------   BCF = K1 / K2 ABOVE  ---------------------------------
        //    // Default BCF Calculation Below
        //    PT = GetStatePointer(Consts.AssocToxSV(ToxTyp), T_SVType.StV, T_SVLayer.WaterCol);
        //    NondissocVal = PT.NonDissoc();
        //    KOW = Chemptrs[ToxTyp].Kow;
        //    if (IsAnimal())
        //    {
        //        // (*       PA := TAnimal(Self);
        //        // Lipid := PA.CalcLipid;
        //        // 
        //        // NLOMFrac := 1/ WettoDry - Lipid;
        //        // 
        //        // BCF := (Lipid *  KOW + NLOMFrac * 0.035 * KOW * (NondissocVal+0.01)) * WettoDry;
        //        // {KBW, L/kg}  {frac}  {L/kg}   {frac}           {frac}    {frac}              {frac}               *)
        //        // Alternate BCF TURNED OFF 10/15/2010
        //        PA = ((this) as TAnimal);
        //        // Code Prior to 9/8/2010
        //        if (IsFish())
        //        {
        //            Lipid = PA.CalcLipid;
        //            KB = Lipid * WetToDry() * KOW * (NondissocVal + 0.01);
        //            // BCF Test 9/8/2010
        //            K2 = PA.Anim_Tox[ToxTyp].Entered_K2;
        //            // BCF Test 9/8/2010   Fix this logic, though if LCInfinite is the only thing this is being used for, maybe remove the TElapsed portion
        //            if ((K2 < Consts.Tiny))
        //            {
        //                result = KB * (1.0 - Math.Exp(-K2 * TElapsed));
        //            }
        //            else
        //            {
        //                result = KB;
        //            }
        //            // fish code
        //        }
        //        else if (NState >= Consts.FirstDetrInv && NState <= Consts.LastDetrInv)
        //        {
        //            PT = GetStatePointer(AllVariables.SuspRefrDetr, ToxTyp, T_SVLayer.WaterCol);
        //            KPSed = PT.CalculateKOM();
        //            // Use Schwarzenbach Eqn. for KPSED
        //            // based on Gobas 1993
        //            result = PA.PAnimalData.FishFracLipid / DetrOCFrac * KPSed;
        //        }
        //        else
        //        {
        //            // Southworth et al. 1978
        //            result = WetToDry() * 0.3663 * Math.Pow(KOW, 0.7520) * (NondissocVal + 0.01);
        //        }
        //        // not detr invert
        //        // animal code
        //    }
        //    else
        //    {
        //        // Organism is Plant
        //        NondissReduc = NondissocVal + 0.2;
        //        if (NondissReduc > 1.0)
        //        {
        //            NondissReduc = 1.0;
        //        }
        //        if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
        //        {
        //            // dry wt.
        //            result = 0.00575 * Math.Pow(KOW, 0.98) * NondissReduc;
        //        }
        //        else
        //        {
        //            result = NondissocVal * 2.57 * Math.Pow(KOW, 0.93) + (1 - NondissocVal) * 0.257 * Math.Pow(KOW, 0.93);
        //        }
        //        // modified from Swackhamer & Skoglund 1993, p. 837
        //        // 7.29 * POWER(KOW,0.681) * NonDissReduc;
        //        // 0.2 * 5 * KOW * NonDissReduc;
        //        // dry wt.
        //        // 0.2 lipid * WetToDry * KOW, which is Dry
        //        ChemicalRecord 5 = Chemptrs[ToxTyp].ChemRec;
        //        if (5.IsPFA)
        //        {
        //            if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
        //            {
        //                result = 5.PFAMacroBCF;
        //            }
        //            else
        //            {
        //                result = 5.PFAAlgBCF;
        //            }
        //        }
        //    }
        //    // plant code
        //    OrgToxBCF[ToxTyp] = result;
        //    // Save for FCM calculation

        //    return result;
        //}

        //// --------------------------------------------------------------------------
        //public void Poisoned_Calculate_Internal_Toxicity()   FIXME ADD WHEN ADDING ECOTOXICOLOGY
        //{
        //    double ToxPPB;
        //    // PPB of the toxicant in the organism
        //    double ToxinOrg;
        //    // Toxicant in the organism
        //    double Shape;
        //    // unitless param expressing toxic response variability
        //    double Cum_Frac_Eqn_Part;
        //    CumFracNow = 0;
        //    RedGrowth[ToxTyp] = 0.0;
        //    RedRepro[ToxTyp] = 0.0;
        //    FracPhoto[ToxTyp] = 1.0;
        //    if ((LC50_Local < Consts.Tiny))
        //    {
        //        return;
        //    }
        //    // 3/23/2012  if LC50 is zero, no effects, application factors are incalculable so no effects on repro, growth, photosynthesis either
        //    Shape = Chemptrs[ToxTyp].ChemRec.Weibull_Shape;
        //    if (Shape <= 0)
        //    {
        //        throw new Exception(Consts.PrecText(ToxTyp) + " Shape Parameter must be greater than zero.");
        //    }
        //    ToxinOrg = AQTSeg.GetState(NState, ToxTyp, T_SVLayer.WaterCol);
        //    if ((ToxinOrg <= 0.0))
        //    {
        //        return;
        //    }
        //    ToxPPB = (ToxinOrg / State) * 1e6;
        //    // ug/kg         ug/L   /  mg/L     mg/kg
        //    TElapsed = AQTSeg.CalculateTElapsed(ToxTyp);
        //    if ((LifeSpan > 0) && (TElapsed > LifeSpan))
        //    {
        //        TElapsed = LifeSpan;
        //    }
        //    // JSC 8-12-2003
        //    if ((ToxPPB <= 0.0) || (TElapsed == 0.0))
        //    {
        //        return;
        //    }
        //    if ((K2_Local < Consts.Tiny))
        //    {
        //        LethalConc = LCInfinite[ToxTyp];
        //    }
        //    else if (TElapsed > 4.605 / K2_Local)
        //    {
        //        LethalConc = LCInfinite[ToxTyp];
        //    }
        //    else if ((1 - Math.Exp(-K2_Local * TElapsed)) < 0.0001)
        //    {
        //        // bullet proof
        //        LethalConc = LCInfinite[ToxTyp] / 0.0001;
        //    }
        //    else
        //    {
        //        LethalConc = LCInfinite[ToxTyp] / (1 - Math.Exp(-K2_Local * TElapsed));
        //    }
        //    // ppb
        //    // ppb
        //    // 1/d
        //    // d
        //    TStates 1 = AQTSeg;
        //    if (LethalConc == 0)
        //    {
        //        if ((ToxPPB > 0))
        //        {
        //            CumFracNow = 1;
        //        }
        //        else
        //        {
        //            CumFracNow = 0;
        //        }
        //    }
        //    else if ((ToxPPB / LethalConc > 70))
        //    {
        //        CumFracNow = 1;
        //    }
        //    else
        //    {
        //        // Weibull cumulative equation
        //        Cum_Frac_Eqn_Part = -Math.Pow(ToxPPB / LethalConc, 1 / Shape);
        //        // ppb / ppb         unitless
        //        CumFracNow = 1 - Math.Exp(Cum_Frac_Eqn_Part);
        //        if (CumFracNow > 0.95)
        //        {
        //            CumFracNow = 1.0;
        //        }
        //        if (IsAnimal())
        //        {
        //            if (AFGrowth < Consts.Tiny)
        //            {
        //                RedGrowth[ToxTyp] = 0;
        //            }
        //            else
        //            {
        //                RedGrowth[ToxTyp] = 1 - Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFGrowth), 1 / Shape));
        //            }
        //            if (AFRepro < Consts.Tiny)
        //            {
        //                RedRepro[ToxTyp] = 0;
        //            }
        //            else
        //            {
        //                RedRepro[ToxTyp] = 1 - Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFRepro), 1 / Shape));
        //            }
        //        }
        //        // plant
        //        else if (AFPhoto < Consts.Tiny)
        //        {
        //            FracPhoto[ToxTyp] = 1;
        //        }
        //        else
        //        {
        //            FracPhoto[ToxTyp] = Math.Exp(-Math.Pow(ToxPPB / (LethalConc * AFPhoto), 1 / Shape));
        //        }
        //    }
        //}

        //// Calculate Internal Toxicity
        //// --------------------------------------------------------------------------
        //public void Poisoned_Calculate_External_Toxicity()
        //{
        //    double k;
        //    double ETA;
        //    double SlopeFactor;
        //    double Slope;
        //    double ToxinH2O;
        //    // Toxicant in the water
        //    CumFracNow = 0;
        //    RedGrowth[ToxTyp] = 0;
        //    // 5/3/2017 defaults for no effects if tox is zero
        //    RedRepro[ToxTyp] = 0;
        //    FracPhoto[ToxTyp] = 1;
        //    SlopeFactor = Chemptrs[ToxTyp].ChemRec.WeibullSlopeFactor;
        //    if (IsAnimal())
        //    {
        //        TAnimalToxRecord 1 = AnimalPtr.Anim_Tox[ToxTyp];
        //        // animal
        //        if ((1.LC50_Slope > Consts.Tiny))
        //        {
        //            SlopeFactor = 1.LC50_Slope;
        //        }
        //    }
        //    // JSC 12/14/2016 allow for animal-chemical-specific slope factor
        //    if (IsPlant())
        //    {
        //        TPlantToxRecord 2 = ((this) as TPlant).Plant_Tox[ToxTyp];
        //        // plant
        //        if ((2.LC50_Slope > Consts.Tiny))
        //        {
        //            SlopeFactor = 2.LC50_Slope;
        //        }
        //    }
        //    // JSC 12/14/2016 allow for plant-chemical-specific slope factor
        //    ToxinH2O = GetState(Consts.AssocToxSV(ToxTyp), T_SVType.StV, T_SVLayer.WaterCol);
        //    if (ToxinH2O < 0)
        //    {
        //        return;
        //    }
        //    if (LC50_Local > Consts.Tiny)
        //    {
        //        if (SlopeFactor <= 0)
        //        {
        //            throw new Exception(Consts.PrecText(ToxTyp) + " Weibull Slope Factor must be greater than zero.");
        //        }
        //        Slope = SlopeFactor / LC50_Local;
        //        // unitless
        //        // LC50*Slope
        //        // LC50
        //        ETA = (-2 * LC50_Local * Slope) / Math.Log(0.5);
        //        try
        //        {
        //            k = -Math.Log(0.5) / Math.Pow(LC50_Local, ETA);
        //            CumFracNow = 1 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
        //        }
        //        catch
        //        {
        //            throw new Exception("Floating Point Error Calculating External Toxicity.  Re-examine input parameters.");
        //            CumFracNow = 0;
        //        }
        //    }
        //    if (IsAnimal())
        //    {
        //        TAnimalToxRecord 3 = AnimalPtr.Anim_Tox[ToxTyp];
        //        // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Growth
        //        if (3.EC50_growth < Consts.Tiny)
        //        {
        //            RedGrowth[ToxTyp] = 0;
        //        }
        //        else
        //        {
        //            if ((3.EC50_Growth_Slope > Consts.Tiny))
        //            {
        //                // JSC 4/5/2017 allow for animal-chemical-effect specific slope factor
        //                Slope = 3.EC50_Growth_Slope / 3.EC50_growth;
        //            }
        //            else
        //            {
        //                Slope = SlopeFactor / 3.EC50_growth;
        //            }
        //            if (Slope <= 0)
        //            {
        //                throw new Exception(Consts.PrecText(ToxTyp) + " EC50 Growth Weibull Slope Factor must be greater than zero.");
        //            }
        //            ETA = (-2 * 3.EC50_growth * Slope) / Math.Log(0.5);
        //            try
        //            {
        //                k = -Math.Log(0.5) / Math.Pow(3.EC50_growth, ETA);
        //                RedGrowth[ToxTyp] = 1 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
        //            }
        //            catch
        //            {
        //                throw new Exception("Floating Point Error Calculating Growth Limitations due to External Toxicity.  Re-examine input parameters.");
        //                RedGrowth[ToxTyp] = 0;
        //            }
        //        }
        //        // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Repro
        //        if (3.EC50_repro < Consts.Tiny)
        //        {
        //            RedRepro[ToxTyp] = 0;
        //        }
        //        else
        //        {
        //            if ((3.EC50_Repro_Slope > Consts.Tiny))
        //            {
        //                // JSC 4/5/2017 allow for animal-chemical-effect specific slope factor
        //                Slope = 3.EC50_Repro_Slope / 3.EC50_repro;
        //            }
        //            else
        //            {
        //                Slope = SlopeFactor / 3.EC50_repro;
        //            }
        //            if (Slope <= 0)
        //            {
        //                throw new Exception(Consts.PrecText(ToxTyp) + " EC50 Repro Weibull Slope Factor must be greater than zero.");
        //            }
        //            ETA = (-2 * 3.EC50_repro * Slope) / Math.Log(0.5);
        //            try
        //            {
        //                k = -Math.Log(0.5) / Math.Pow(3.EC50_repro, ETA);
        //                RedRepro[ToxTyp] = 1 - Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
        //            }
        //            catch
        //            {
        //                throw new Exception("Floating Point Error Calculating Reproductive Limitations due to External Toxicity.  Re-examine input parameters.");
        //                RedRepro[ToxTyp] = 0;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // IsPlant
        //        TPlantToxRecord 4 = ((this) as TPlant).Plant_Tox[ToxTyp];
        //        // plant
        //        if (4.EC50_photo < Consts.Tiny)
        //        {
        //            FracPhoto[ToxTyp] = 1.0;
        //        }
        //        else
        //        {
        //            // 3/9/2012  Remove Requirement for non-zero LC50 to apply EC50_Repro  //10/8/2014 JSC FracPhoto should be set to 1.0 for no effect
        //            if ((4.EC50_Photo_Slope > Consts.Tiny))
        //            {
        //                // JSC 4/5/2017 allow for plant-chemical-effect specific slope factor
        //                Slope = 4.EC50_Photo_Slope / 4.EC50_photo;
        //            }
        //            else
        //            {
        //                Slope = SlopeFactor / 4.EC50_photo;
        //            }
        //            if (Slope <= 0)
        //            {
        //                throw new Exception(Consts.PrecText(ToxTyp) + " EC50 Photo Weibull Slope Factor must be greater than zero.");
        //            }
        //            ETA = (-2 * 4.EC50_photo * Slope) / Math.Log(0.5);
        //            try
        //            {
        //                k = -Math.Log(0.5) / Math.Pow(4.EC50_photo, ETA);
        //                FracPhoto[ToxTyp] = Math.Exp(-k * Math.Pow(ToxinH2O, ETA));
        //            }
        //            catch
        //            {
        //                throw new Exception("Floating Point Error Calculating Photosynthesis Limitations due to External Toxicity.  Re-examine input parameters.");
        //                FracPhoto[ToxTyp] = 1;
        //            }
        //        }
        //    }
        //}

        //// CalcRiskConc was reworked to be run at the beginning of model
        //// run rather than each time step. (JSC Apr 5, 1996)


        //// BCF Calculation
        //// ------------------------------------------------------------------------
        //public virtual double Poisoned(object ToxTyp)
        //{
        //    double result;
        //    double TElapsed;
        //    // Time elapsed since exposure to toxicant, days
        //    double LethalConc;
        //    // Time varying LethalConc, ppb
        //    double Nonresistant;
        //    double FracKill;
        //    double LifeSpan;
        //    // Lifespan of animal in days
        //    double AFGrowth;
        //    double AFRepro;
        //    double AFPhoto;
        //    TAnimal AnimalPtr;
        //    double LC50_Local;
        //    double K2_Local;
        //    double CumFracNow;
        //    double NewResist;
        //    // Calculate External Toxicity
        //    // --------------------------------------------------------------------------
        //    result = 0;
        //    AFGrowth = 0;
        //    AFRepro = 0;
        //    AFPhoto = 0;
        //    if ((!AQTSeg.SetupRec.UseExternalConcs) && (State == 0))
        //    {
        //        return result;
        //    }
        //    if (!(NState >= Consts.FirstBiota && NState <= Consts.LastBiota))
        //    {
        //        return result;
        //    }
        //    if (IsAnimal())
        //    {
        //        AnimalPtr = ((this) as TAnimal);
        //        TAnimalToxRecord 5 = AnimalPtr.Anim_Tox[ToxTyp];
        //        if (AnimalPtr.Anim_Tox[ToxTyp] == null)
        //        {
        //            return result;
        //        }
        //        if (5.LC50 <= 0.0)
        //        {
        //            AFGrowth = 0;
        //            AFRepro = 0;
        //        }
        //        else
        //        {
        //            AFGrowth = 5.EC50_growth / 5.LC50;
        //            AFRepro = 5.EC50_repro / 5.LC50;
        //        }
        //        LC50_Local = 5.LC50;
        //        K2_Local = 5.Entered_K2;
        //        K2_Local = K2_Local + 5.Bio_rate_const;
        //        // 5/11/2015  Add metabolism to k2 for calculations
        //        LifeSpan = AnimalPtr.PAnimalData.LifeSpan;
        //    }
        //    else
        //    {
        //        TPlantToxRecord 6 = ((this) as TPlant).Plant_Tox[ToxTyp];
        //        if (((this) as TPlant).Plant_Tox[ToxTyp] == null)
        //        {
        //            return result;
        //        }
        //        if (6.LC50 <= 0.0)
        //        {
        //            AFPhoto = 0;
        //        }
        //        else
        //        {
        //            AFPhoto = 6.EC50_photo / 6.LC50;
        //        }
        //        LC50_Local = 6.LC50;
        //        K2_Local = 6.K2;
        //        K2_Local = K2_Local + 6.Bio_rate_const;
        //        // 5/11/2015  Add metabolism to k2 for calculations
        //        LifeSpan = 0;
        //        // NA
        //    }
        //    if (!AQTSeg.SetupRec.UseExternalConcs)
        //    {
        //        Poisoned_Calculate_Internal_Toxicity();
        //    }
        //    else
        //    {
        //        Poisoned_Calculate_External_Toxicity();
        //    }
        //    if (CumFracNow <= 0)
        //    {
        //        return result;
        //    }
        //    // 9-14-07 conversion of Resistant from biomass units to fraction units
        //    Nonresistant = State * (1 - Resistant[ToxTyp]);
        //    // mg/L          mg/L          frac
        //    TStates 7 = AQTSeg;
        //    if (PrevFracKill[ToxTyp] >= CumFracNow)
        //    {
        //        FracKill = 0;
        //    }
        //    else
        //    {
        //        FracKill = (CumFracNow - PrevFracKill[ToxTyp]) / (1 - PrevFracKill[ToxTyp]);
        //    }
        //    TStates 8 = AQTSeg;
        //    result = Resistant[ToxTyp] * State * FracKill + Nonresistant * CumFracNow;
        //    // mg/L-d        frac             mg/L     g/g-d      mg/L            g/g-d
        //    NewResist = (State - result) / State;
        //    // frac
        //    // mg/L
        //    // mg/L
        //    // mg/L
        //    DeltaResistant[ToxTyp, 8.DerivStep] = NewResist - Resistant[ToxTyp];
        //    DeltaCumFracKill[ToxTyp, 8.DerivStep] = CumFracNow - PrevFracKill[ToxTyp];
        //    return result;
        //}

        //// ------------------------------------------------------------------------
        //public virtual double Mortality()
        //{
        //    double result;
        //    // overridden
        //    result = 0;
        //    return result;
        //}

        //// Chronic Effects
        //public double GutEffOrgTox(object ToxTyp)
        //{
        //    double result;
        //    double GutEff;
        //    ChemicalRecord 1 = Chemptrs[ToxTyp].ChemRec;
        //    if (1.IsPFA)
        //    {
        //        if (1.PFAType == "carboxylate")
        //        {
        //            // sulfonate
        //            GutEff = Math.Pow(10.0, -0.909539 + 0.085141 * 1.PFAChainLength);
        //        }
        //        else
        //        {
        //            GutEff = -0.68 + 0.21 * 1.PFAChainLength;
        //        }
        //        if ((GutEff > 1.0))
        //        {
        //            GutEff = 1.00;
        //        }
        //        result = GutEff;
        //        // Non PFA code below
        //    }
        //    else if ((NState >= Consts.FirstInvert && NState <= Consts.LastInvert))
        //    {
        //        // Landrum et al. 1990
        //        result = 0.35;
        //    }
        //    else if ((NState >= AllVariables.LgGameFish1 && NState <= AllVariables.LgGameFish4))
        //         result = 0.92;
        //    else result = 0.62;
        //    return result;
        //}

        public double SalMort(double Min, double Max, double Coeff1, double Coeff2)
        {
            double Salt;
            TSalinity PSalt = (TSalinity)AQTSeg.GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);

            if (PSalt == null) Salt = -1.0;
                          else Salt = PSalt.State;

            if (Salt == -1) return 0;
            if ((Salt >= Min) && (Salt <= Max)) return 0;

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
            return 0;
        }
    } // end TOrganism
       
}  // namespace
