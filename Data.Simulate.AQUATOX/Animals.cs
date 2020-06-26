using System;
using System.Collections.Generic;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.OrgMatter;
using AQUATOX.Bioaccumulation;
using AQUATOX.Diagenesis;
using AQUATOX.Chemicals;
using Newtonsoft.Json;
using Globals;
using AQUATOX.Organisms;

namespace AQUATOX.Animals

{

    public class AnimalRecord 
    {
        public string AnimalName;
        public string Animal_Type;
        public string ToxicityRecord;
        public double FHalfSat;
        public string XFHalfSat;
        public double CMax;    // max consumption
        public string XCMax;
        public double BMin;
        public string XBMin;
        public double Q10;
        public string XQ10;
        public double TOpt;
        public string XTOpt;
        public double TMax;
        public string XTMax;
        public double TRef;
        public string XTRef;
        public double EndogResp;
        public string XEndogResp;
        public double KResp;
        public string XKResp;
        public double KExcr;
        public string XKExcr;
        public double PctGamete;
        public string XPctGamete;
        public double GMort;
        public string XGMort;
        public double KMort;     // Mortality Coefficient
        public string XKMort;
        public double KCap;
        public string XKCap;
        public double MeanWeight;
        public string XMeanWeight;
        public double FishFracLipid;
        public string XFishFracLipid;
        public double LifeSpan;       // Mean Lifespan in days
        public string XLifeSpan;
        public double AveDrift;
        public string XAveDrift;    // Background Drift
        public bool AutoSpawn;        // Calc Spawn Automatically based on Temps?
        public DateTime SpawnDate1;
        public DateTime SpawnDate2;
        public DateTime SpawnDate3;
        public string XSpawnDate;
        public bool UnlimitedSpawning;
        public int SpawnLimit;
        public bool UseAllom_C;  // use allometric equations to calculate consumption
        public double CA;        // intercept for weight dependence
        public double CB;        // slope for weight dependence
        public bool UseAllom_R;  // use allometric equations to calculate respiration
        public double RA;        // intercept for spec. standard metabolism
        public double RB;        // weight dependence coeff.
        public bool UseSet1;
        // Use "set 1" of resp. eqns.
        public double RQ;          // allometric respiration parameters
        public double RTO;
        public double RTM;
        public double RTL;
        public double RK1;
        public double RK4;
        public double ACT;
        public double BACT;

        public double FracInWaterCol;
        public string XFracInWaterCol;
        public string Guild_Taxa;
        public double PrefRiffle;
        public string XPrefRiffle;
        public double PrefPool;
        public string XPrefPool;
        public double VelMax;
        public string XVelMax;
        public string XAllomConsumpt;
        public string XAllomResp;

        // Salinity & Ingestion
        public double Salmin_Ing; // minimum salinity tolerance 0/00
        public double SalMax_Ing; // max salinity tolerance 0/00
        public double Salcoeff1_Ing;
        public double Salcoeff2_Ing;  // unitless
        public string XSalinity_Ing;
        // Salinity & Gameteloss
        public double Salmin_Gam; // minimum salinity tolerance 0/00
        public double SalMax_Gam; // max salinity tolerance 0/00
        public double Salcoeff1_Gam;
        public double Salcoeff2_Gam;        // unitless
        public string XSalinity_Gam;
        // Salinity & Respiration
        public double Salmin_Rsp; // minimum salinity tolerance 0/00
        public double SalMax_Rsp; // max salinity tolerance 0/00
        public double Salcoeff1_Rsp;
        public double Salcoeff2_Rsp;        // unitless
        public string XSalinity_Rsp;        
        // Salinity & Mortality
        public double Salmin_Mort; // minimum salinity tolerance 0/00
        public double SalMax_Mort; // max salinity tolerance 0/00
        public double Salcoeff1_Mort;
        public double Salcoeff2_Mort;        // unitless
        public string XSalinity_Mort;
        public double Fishing_Frac;        // fraction / day
        public string XFishing_Frac;
        public double P2Org;
        public string XP2Org;
        public double N2Org;
        public string XN2Org;
        public double Wet2Dry;
        public string XWet2Dry;
        public double O2_LethalConc;
        public double O2_LethalPct;
        public string O2_LCRef;
        public double O2_EC50growth;
        public string XO2_EC50growth;
        public double O2_EC50repro;
        public string XO2_EC50repro;
        public double Ammonia_LC50;
        public string XAmmonia_LC50;
        public double Sorting;       // 3.46, SABS
        public string XSorting;
        public bool SuspSedFeeding;
        public string XSuspSedFeeding;
        public double SlopeSSFeed;
        public string XSlopeSSFeed;
        public double InterceptSSFeed;
        public string XInterceptSSFeed;
        public string SenstoSediment = "";
        public string XSensToSediment;
        public double Trigger;
        public string XTrigger;
        public bool SenstoPctEmbed;
        public double PctEmbedThreshold;
        public string XPctEmbedThreshold;
        public string BenthicDesignation;
        public bool CanSeekRefuge;
        public bool Visual_Feeder;
        public bool PlaceholderB2;
        public float PlaceholderS1;
        public double PlaceHolderD1;
        // --------------------------------------------------------------------------------
        public double Burrow_Index;
        public string ScientificName;
        public string XBurrow_Index;

    } // end AnimalRecord

    public class InteractionFields
    {
        public double Pref;
        public double ECoeff;
        public string XInteraction;
    } 

    public class TPreference 
    {
        public TPreference(double pr, double ec, AllVariables av)
        {
            Preference = pr;
            EgestCoeff = ec;
            nState = av;
        }
            
        public double Preference;
        public double EgestCoeff;
        public AllVariables nState; 
    }

    public class TAnimalToxRecord 
    {
        public string Animal_name = String.Empty;
        public double LC50 = 0;
        public double LC50_exp_time = 0;
        public object LC50_comment;
        public double Entered_K2 = 0;
        public double Entered_K1 = 0;
        public double Entered_BCF = 0;
        public double Bio_rate_const = 0;
        public double EC50_growth = 0;
        public double Growth_exp_time = 0;
        public double EC50_repro = 0;
        public double Repro_exp_time = 0;
        public object EC50_comment;
        public double Mean_wet_wt = 0;
        public double Lipid_frac = 0;
        public double Drift_Thresh = 0;
        public object Drift_Comment;
        public double LC50_Slope = 0;         // 12/14/2016 specific to animal/chemical combination
        public double EC50_Growth_Slope = 0;  // 4/5/2017  specific to animal/chemical/effect combination
        public double EC50_Repro_Slope = 0;   // 4/5/2017  specific to animal/chemical/effect combination
    }

    public class MortRatesRecord
    {
        // not saved to disk
        public double[] OrgPois;
        public double O2Mort;
        public double NH4Mort;
        public double NH3Mort;
        public double OtherMort;
        public double SaltMort;
        public double SedMort;
    } // end MortRatesRecord

    public class TAnimal : TOrganism
    {
        public AnimalRecord PAnimalData;
        public InteractionFields[] PTrophInt;        // Eating Preferences and Egestion Coefficients
        [JsonIgnore] public List<TPreference> MyPrey = null;      // Things I Eat
        public bool Spawned = false;        // Has this species spawned already on this date, within the correct temp range
        public int SpawnTimes = 0;          // how many times has this species spawned since midwinter?
//      public AnadromousInputRec AnadRel;  // If size class anadromous, this is relevant
        [JsonIgnore] public double PromoteLoss = 0;      // promotion of biomass from this size class to the next (nosave)
        [JsonIgnore] public double PromoteGain = 0;      // promotion of biomass from lower size class to this (nosave)
        [JsonIgnore] public double Recruit = 0;
        [JsonIgnore] public double Gametes = 0;          // SmGameFish recruited from LgGameFish, or size class (nosave)
        [JsonIgnore] public int OysterCategory = 0;      // 1=Veliger 2=Spat 3=Seed 4=Sack, 0= not an oyster, nosave
        [JsonIgnore] public object POlder = null;
        [JsonIgnore] public object PYounger = null;      // Pointer to older / younger size class, nosave
        [JsonIgnore] public double EmergeInsect = 0;     // insects subject to emergence (nosave)
        [JsonIgnore] public double RecrSave = 0;         // Saved for use in DoThisEveryStep (nosave)
        [JsonIgnore] public bool IsLeavingSeg = false;   // Is 100% of the animal currently migrating out of the segment due to anoxia or salinity(nosave)
//      public MigrationInputRec[] MigrInput;
//      public double KD = 0;               // KD calculated for PFA cheimcals
        [JsonIgnore] public double HabitatLimit = 1.0;     // Habitat Limitation nosave
        [JsonIgnore] public TAnimalToxRecord[] Anim_Tox = new TAnimalToxRecord[Consts.NToxs]; // pointer to relevant animal toxicity data (nosave)  
        public AllVariables PSameSpecies;   // other state variable that represents the same species, relevant to only Sm and Lg Game Fish
        [JsonIgnore] public double SumPrey = 0;          // The total sum of available prey to a predator in a given timestep, calculated at the beginning of each timestep
        [JsonIgnore] public MortRatesRecord MortRates = new MortRatesRecord();    // Holds data about how animal is dying, (nosave)
        [JsonIgnore] public double NitrCons = 0;
        [JsonIgnore] public double PhosCons = 0; // holds data about the consumption of nutrients (nosave)

        [JsonIgnore] public double[] LastO2Calc = new double[3];      //(0=O2Mortality,1=O2Growth_Red,2=O2Repro_Red)
        [JsonIgnore] public DateTime[] LastO2CalcTime = new DateTime[3];  //(0=O2Mortality,1=O2Growth_Red,2=O2Repro_Red)         // optimization

        [JsonIgnore] public double LastSedCalc = 0;
        [JsonIgnore] public DateTime LastSedCalcTime = DateTime.MinValue;        // optimization

//      public AnadromousDataRec AnadromousData = null;        // NoSave
        [JsonIgnore] public double[] DerivedK1 = new double[Consts.NToxs];
        [JsonIgnore] public double[] DerivedK2 = new double[Consts.NToxs];        // 9/27/2010 model calculations for alternative BAF (nosave)

        [JsonIgnore] public double PreyTrophicLevel = 0;
        [JsonIgnore] public double TrophicLevel = 0;

        // ChangeData MUST be called when the underlying data record is changed
        // ------------------------------------------------------------------------
        public override void SetToInitCond()
        {
            Assign_Anim_Tox();

            base.SetToInitCond();
            HabitatLimit = AHabitat_Limit();   //previously set in CalcRiskConc

            Spawned = false;
            SpawnTimes = 0;
            PromoteLoss = 0;
            PromoteGain = 0;
            EmergeInsect = 0;
            Recruit = 0;
            if ((PAnimalData.Animal_Type == "Fish") || (IsPlanktonInvert()))
                PAnimalData.AveDrift = 0;

            CalcRiskConc(true);    // Using ToxicityRecord Initialize Organisms with
                                   // the appropriate RISKCONC, LCINFINITE, and K2
                                   // Set Oyster Category
        }
        // ------------------------------------------------------------------------
        public int iTrophInt(AllVariables ns)
        {
            return ((int)ns - (int)AllVariables.Cohesives);
        }
        // ------------------------------------------------------------------------
        public bool IsBenthos()
        {
            return (PAnimalData.Animal_Type == "Benthic Invert.") || (PAnimalData.Animal_Type == "Benthic Insect");
        }

        // ------------------------------------------------------------------------
        public bool IsPlanktonInvert()
        {
            return PAnimalData.Animal_Type == "Plankton Invert";
        }

        // includes benthic invert and benthic insect
        // ------------------------------------------------------------------------
        public bool IsNektonInvert()
        {
            return PAnimalData.Animal_Type == "Nekton Invert.";
        }

        // ------------------------------------------------------------------------
        public override double WetToDry()
        {
            return PAnimalData.Wet2Dry;
        }

        // ------------------------------------------------------------------------
        public double RefugeFrom(AllVariables Prey)
        {
            double result;
            const double HalfSat = 100.0;
            double MacroState;
            double OysterBio;
            double MacroRefuge;
            double OysterRefuge;
            double MarshRefuge;
            double BurrowRefuge;
            TAnimal PA;
            TAnimal PPrey;
            AllVariables LoopVal;
            if (!(Prey >= Consts.FirstAnimal && Prey <= Consts.LastAnimal))  return 1.0;

            PPrey = AQTSeg.GetStatePointer(Prey, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
            if (PPrey == null)  return 1.0; 

            BurrowRefuge = 1.0;
            AnimalRecord ZR = PPrey.PAnimalData;
            if (ZR.Burrow_Index > Consts.Tiny)
            {
                BurrowRefuge = 1.0 - (ZR.Burrow_Index / (ZR.Burrow_Index + 3.2));
            }
            // 6-27-2014
            result = BurrowRefuge;
            if (!PAnimalData.Visual_Feeder) return result;
            // prey is not subject to visual refuge from this predator

            if (!PPrey.PAnimalData.CanSeekRefuge) return result;

            // This prey type cannot seek visual refuge, burrow refuge only
            MacroState = 0;
            for (LoopVal = Consts.FirstMacro; LoopVal <= Consts.LastMacro; LoopVal++)
            {
                if ((AQTSeg.GetStateVal(LoopVal, T_SVType.StV, T_SVLayer.WaterCol) > -1.0))
                {
                    MacroState = MacroState + AQTSeg.GetState(LoopVal, T_SVType.StV, T_SVLayer.WaterCol);
                }
            }
            MacroRefuge = 1.0 - (MacroState / (MacroState + HalfSat));
            OysterBio = 0.0;
            for (LoopVal = Consts.FirstInvert; LoopVal <= Consts.LastInvert; LoopVal++)
            {
                PA = AQTSeg.GetStatePointer(LoopVal, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (PA != null)
                {
                    if (PA.OysterCategory > 1)
                    {
                        OysterBio = OysterBio + PA.State;
                    }    // mg/L
                }  // seed, sack, spat
            }
            OysterRefuge = 1;

            SiteRecord LL = Location.Locale;
            if (OysterBio > Consts.Tiny)
            {
                OysterBio = OysterBio * AQTSeg.SegVol() / AQTSeg.SurfaceArea();
                // g/m2                // g/m3                // m3|       

                OysterRefuge = 1.0 - (OysterBio / (OysterBio + LL.HalfSatOysterRefuge));                  // g/m2
            }
            
            MarshRefuge = 1;
            if (LL.FractalD > Consts.Tiny)
            {
                MarshRefuge = (1.0 + LL.FD_Refuge_Coeff) / (LL.FractalD + LL.FD_Refuge_Coeff);
            }
            result = MacroRefuge * OysterRefuge * MarshRefuge * BurrowRefuge;
            return result;
        }

        // 2/11/2013  Trophic level for output, nosave
        // ******************************************************************************
        // TANIMAL METHODS
        //Constructor  Init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
        public TAnimal(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            T_SVType ToxLoop;
            InteractionFields Blank;
            AllVariables nsloop;
            //          int MigrLoop;

            if (MyPrey != null) MyPrey.Clear(); else MyPrey = new List<TPreference>();

            Blank = new InteractionFields();
            Blank.Pref = 0;
            Blank.ECoeff = 0;
            Blank.XInteraction = "";

             PTrophInt = new InteractionFields[iTrophInt(Consts.LastBiota)+1];
                         for (nsloop = AllVariables.Cohesives; nsloop <= Consts.LastBiota; nsloop++)
                    PTrophInt[iTrophInt(nsloop)] = Blank;

            PSameSpecies = AllVariables.NullStateVar;

            // TOrganism
            Spawned = false;
            // PRequiresData = true;
            PromoteLoss = 0;
            PromoteGain = 0;
            OysterCategory = 0;
            POlder = null;
            PYounger = null;
            EmergeInsect = 0;
            Recruit = 0;
            RecrSave = 0;
            IsLeavingSeg = false;
            SumPrey = 0;
            MortRates.OtherMort = 0;

            MortRates.OrgPois =  new double[Consts.NToxs]; 
            for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
            {
                MortRates.OrgPois[ToxInt(ToxLoop)] = 0;
            }

            //for (MigrLoop = 1; MigrLoop <= 5; MigrLoop++)
            //{
            //    MigrInput[MigrLoop].FracMigr = 0;
            //    MigrInput[MigrLoop].ToSeg = "";
            //    MigrInput[MigrLoop].DD = 0;
            //    MigrInput[MigrLoop].MM = 0;
            //}
            //AnadRec.IsAnadromous = false;
            //AnadRec.YearsOffSite = 0;
            //AnadRec.DateJuvMigr = 70;
            //AnadRec.DateAdultReturn = 100;
            //AnadRec.FracMigrating = 0.2;
            //AnadRec.MortalityFrac = 0.5;

            LastO2CalcTime[0] = DateTime.MinValue; //TO2Effects.O2Mortality
            LastO2CalcTime[1] = DateTime.MinValue; //TO2Effects.O2Growth_Red
            LastO2CalcTime[2] = DateTime.MinValue; //TO2Effects.O2Repro_Red
        }

        public void Assign_Anim_Tox()
        {
            int FoundToxIndx;
            int i;
            TAnimalToxRecord ATR;
            TToxics TT;
            int ToxLoop;
            string DataName;
            DataName = PAnimalData.ToxicityRecord.ToLower();

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                TT = AQTSeg.GetStatePointer(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) as TToxics;
                if (TT != null)
                {
                    FoundToxIndx = -1;
                    for (i = 0; i < TT.Chem_Anim_Tox.Count; i++)
                    {
                        ATR = TT.Chem_Anim_Tox[i];
                        if (ATR.Animal_name.ToLower() == DataName)
                        {
                            FoundToxIndx = i;
                            break;
                        }
                    }
                    if (FoundToxIndx == -1)
                        throw new Exception("Error!  " + PAnimalData.AnimalName + " uses the toxicity record \"" + DataName + "\" which is not found in chemical " + TT.ChemRec.ChemName + "\'s Anim toxicity data.  Study cannot be executed.");

                    ATR = TT.Chem_Anim_Tox[FoundToxIndx];
                    Anim_Tox[ToxLoop] = ATR;
                }
            }
        }

        public void ChangeData()
        {
            TPreference Pr;
            AllVariables CMP;
            double Pref;
            double Egest;
            // FIX THE UNITS WHICH CHANGE WITH INVERTEBRATE TYPE
            if ((NState >= Consts.FirstInvert && NState <= Consts.LastInvert))
            {
                if ((PAnimalData.Animal_Type == "Benthic Invert.") || (PAnimalData.Animal_Type == "Benthic Insect") || (PAnimalData.Animal_Type == "Fish"))
                {
                    StateUnit = "g/m2 dry";
                    LoadingUnit = "g/m2 dry";
                }
                else
                {
                    StateUnit = "mg/L dry";
                    LoadingUnit = "mg/L dry";
                }
            }
            if ((NState >= AllVariables.Veliger1 && NState <= AllVariables.Veliger2))
            {
                StateUnit = "mg/L dry";
                LoadingUnit = "mg/L dry";
            }

            if (MyPrey != null) MyPrey.Clear(); else MyPrey = new List<TPreference>();
            for (CMP = AllVariables.Cohesives; CMP <= Consts.LastBiota; CMP++)
            {
                if ((CMP == AllVariables.Cohesives)||(CMP == AllVariables.DissRefrDetr) || (CMP == AllVariables.DissLabDetr) || (CMP == AllVariables.BuriedRefrDetr))
                {
                    Pref = 0;
                    Egest = 0;
                }
                else
                {
                    Pref = PTrophInt[iTrophInt(CMP)].Pref;
                    Egest = PTrophInt[iTrophInt(CMP)].ECoeff;
                }
                // or (Egest>0)
                if ((Pref > 0))
                {
                    Pr = new TPreference(Pref, Egest, CMP);
                    MyPrey.Add(Pr);
                }
            }
        }        // Set SumPrey property at start of time-step
        
       


    // ---------------------------process equations--------------------------
    // (*************************************)
    // (* consumption of prey by predator   *)
    // (*   after Park et al., 1978         *)
    // (*************************************)
    public void CalculateSumPrey()
    {
        double SumPrefPrey;
        double PreyState;

            void CalculateSumPrey_SumP(TPreference P)
            {
                double SumTerm;

                if (P.Preference > 0)
                {
                    // diagenesis model included
                    if (((P.nState == AllVariables.SedmRefrDetr) || (P.nState == AllVariables.SedmLabDetr)) && AQTSeg.Diagenesis_Included())
                        PreyState = AQTSeg.Diagenesis_Detr(P.nState);
                    else
                        PreyState = AQTSeg.GetStateVal(P.nState, T_SVType.StV, T_SVLayer.WaterCol);
                    // mg/L wc

                    if (PreyState > 0.0)
                    {
                        SumTerm = P.Preference * (PreyState - BMin_in_mg_L()) * RefugeFrom(P.nState);
                        if (SumTerm < 0) SumTerm = 0;
                        SumPrefPrey = SumPrefPrey + SumTerm;
                    }
                }
            }


        SumPrefPrey = 0;
        foreach (TPreference TP in MyPrey) CalculateSumPrey_SumP(TP);
        SumPrey = SumPrefPrey;
    }


    public double DefecationTox(T_SVType ToxType)
        {
            // Returns rate of transfer of organic toxicant due to defecation by predator
            // This algorithm now utilizes TAnimal.IngestSpecies (11/20/98)
            double DefToxCount=0;
            double EgestRet=0;
            double GutEffRed=0;

            void DefTox(TPreference P)
            {
                double Ingestion;
                double KEgest;
                double PPBPrey;
                Ingestion = IngestSpecies(P.nState, P, ref EgestRet, ref GutEffRed);
                KEgest = (1.0 - GutEffOrgTox() * GutEffRed) * Ingestion;
                PPBPrey = AQTSeg.GetPPB(P.nState, ToxType, T_SVLayer.WaterCol);
                DefToxCount = DefToxCount + KEgest * PPBPrey * 1e-6;
                // mg/L-d   ug/kg   kg/mg

            }
             
            DefToxCount = 0;

            foreach (TPreference TP in MyPrey) DefTox(TP);

            return DefToxCount;
        }

        public double AHabitat_Limit()
    {
        double HabitatAvail;
        double PctRun;
        double PrefRun;

            if (Location.SiteType != SiteTypes.Stream) return 1.0;

            PctRun = 100 - AQTSeg.Location.Locale.PctRiffle - AQTSeg.Location.Locale.PctPool;
            PrefRun = 100 - PAnimalData.PrefRiffle - PAnimalData.PrefPool;
            HabitatAvail = 0;

            if (PAnimalData.PrefRiffle > 0) HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctRiffle / 100.0;

            if (PAnimalData.PrefPool > 0) HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctPool / 100.0;

            if (PrefRun > 0) HabitatAvail = HabitatAvail + PctRun / 100.0;

            return HabitatAvail;
    }

    public double BMin_in_mg_L()
    {
        if (!IsPlanktonInvert())  
        {
            return PAnimalData.BMin / AQTSeg.Volume_Last_Step * Location.Locale.SurfArea;
          // mg/L          // g/m2                // m3                   // m2
        }
            else return PAnimalData.BMin;      // plankton invert already in mg/L
     }

    public double AggregateRedGrowth()
      {
        T_SVType ToxLoop;
        double AggRedGrowth;
        AggRedGrowth = 0;
        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
        {
            if (AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) != null)
            AggRedGrowth = AggRedGrowth + RedGrowth[ToxInt(ToxLoop)];
        }

        if (AggRedGrowth > 1.0)  AggRedGrowth = 1.0;

        return AggRedGrowth;
    }

        public double AggregateRedRepro()
    {
        double AggRedRepro = 0;

        T_SVType ToxLoop;
        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
           {
                if (AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) != null)
                {
                    AggRedRepro = AggRedRepro + RedRepro[ToxInt(ToxLoop)];
                }
            }

            AggRedRepro = AggRedRepro + O2EffectFrac(2);    //2=O2Repro_Red

        if (AggRedRepro > 1.0)  AggRedRepro = 1.0;
        return AggRedRepro;
    }


    public double IngestSpecies(AllVariables Prey, TPreference PPref, ref double EgestReturn, ref double GutEffRed)
    {
        // Returns amount of ingestion of a specific species by the animal
        // If Preference is not passed (i.e. PPref=nil) GetPref finds the appropriate preference record
        // This allows for combined optimization of coding and increased modularity
        double IngestS;
        double Pref;
        double Food;
        double PreyState;
        double SSedEffect;
        double SaltEffect;
        double AggRG = 0;


            // --------------------------------------------------
            void GetPref(TPreference P, ref double EgestReturn)
            {
                if ((P.nState == Prey))
                {
                    Pref = P.Preference;
                    EgestReturn = P.EgestCoeff;
                }
            }
            // --------------------------------------------------
            void Calc_GutEffRed(ref double GutEffRed)
            {
                GutEffRed = 1.0 - AggRG;
            }
            // --------------------------------------------------
            double FoodDilution()
            {
                
                double SurfArea;
                double SandC;
                double Sed;
                double FdSub;
                double PConstant;
                FdSub = Food;
                // mg/L
                Sed = 0;
                if ((PAnimalData.Guild_Taxa == "Susp Feeder") || (PAnimalData.Guild_Taxa == "Clam"))
                {
                    Sed = AQTSeg.InorgSedConc();  
                    SandC = AQTSeg.GetStateVal(AllVariables.Sand, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetStateVal(AllVariables.NonCohesives2, T_SVType.StV, T_SVLayer.WaterCol);
                    if (SandC > 0)  Sed = Sed - SandC; // mg/L
                                                       // Dilution Effects are only based on Silt and Clay

                }

                if ((PAnimalData.Guild_Taxa == "Sed Feeder") || (PAnimalData.Guild_Taxa == "Snail") || (PAnimalData.Guild_Taxa == "Grazer"))
                {
                    if ((PAnimalData.Guild_Taxa == "Sed Feeder"))   PConstant = 0.001;  // RAP 5/6/2009
                       else  PConstant = 0.01;
                    // account for the fact that snails & grazers feed
                    // periphyton above the depositional surface

                    SurfArea = Location.Locale.SurfArea;
                    // m2                    // m2
                    Sed = AQTSeg.InorgSedDep() * 1000 * PConstant;  
                    // g/m2     // kg/m2      // g/kg  // Proportionality Constant
                    FdSub = Food * AQTSeg.Volume_Last_Step / SurfArea;
                    // g/m2  // g/m3          // m3           // m2
                }

                if (Sed > 0)  return FdSub / (FdSub + Sed * (1.0 - PAnimalData.Sorting));
                else          return 1.0;

            }
            // --------------------------------------------------

        double InorgSed;
        double RedGrow;
        // ingestspecies
        IngestS = 0;
        Pref = 0;
        Food = 0;
        EgestReturn = 0;
        // diagenesis model included
        if (((Prey == AllVariables.SedmRefrDetr)&&(Prey == AllVariables.SedmLabDetr)) && AQTSeg.Diagenesis_Included())
              PreyState = AQTSeg.Diagenesis_Detr(Prey);
        else  PreyState = AQTSeg.GetStateVal(Prey, T_SVType.StV, T_SVLayer.WaterCol);         // mg/L wc

        if (PreyState <= 0) return 0;
        
        if (PPref != null)
        {
            Pref = PPref.Preference;
            EgestReturn = PPref.EgestCoeff;
        }
        else foreach (TPreference TP in MyPrey) GetPref(TP, ref EgestReturn);

        if ((Pref > 0))
            Food = (PreyState - BMin_in_mg_L()) * RefugeFrom(Prey);

        if ((Food > Consts.Tiny))
            Food = Food * FoodDilution();

        AggRG = AggregateRedGrowth();  
        RedGrow = (0.2 * AggRG) + O2EffectFrac(1);  // 1=O2Growth_Red
        if (RedGrow > 1.0) RedGrow = 1.0;

        if ((Pref > 0.0) && (Food > 0.0))
        {
            IngestS = MaxConsumption() * AQTSeg.TCorr(PAnimalData.Q10, PAnimalData.TRef, PAnimalData.TOpt, PAnimalData.TMax) * Pref * Food / (SumPrey + PAnimalData.FHalfSat) * (1.0 - RedGrow) * State * HabitatLimit;
            if (IngestS > Food) IngestS = Food;
            if (IngestS < 0)    IngestS = 0;
        }

        SaltEffect = AQTSeg.SalEffect(PAnimalData.Salmin_Ing, PAnimalData.SalMax_Ing, PAnimalData.Salcoeff1_Ing, PAnimalData.Salcoeff2_Ing);
        SSedEffect = 1;
        if (PAnimalData.SuspSedFeeding)
        {
            InorgSed = AQTSeg.InorgSedConc(); 
            if (InorgSed > Consts.Tiny)
                 SSedEffect = PAnimalData.SlopeSSFeed * Math.Log(InorgSed) + PAnimalData.InterceptSSFeed;
            else SSedEffect = 1.0;

            if (SSedEffect > 1) SSedEffect = 1;
            if (SSedEffect < 0) SSedEffect = 0;
        }

        Calc_GutEffRed(ref GutEffRed);
        return IngestS * SaltEffect * SSedEffect;

    }

    public double MaxConsumption()
    {
        if (PAnimalData.UseAllom_C)
              return PAnimalData.CA * Math.Pow(PAnimalData.MeanWeight, PAnimalData.CB);
        else  return PAnimalData.CMax;
    }

    // -------------------------------------------------------------------------------
    public double EatEgest(Boolean CalcEgest)
    {
        
        // This procedure now utilizes TAnimal.IngestSpecies (11/20/98)
        double EECount;
        double Ingestion;
        double IncrEgest;
        double EgestRet=0;
        double GER=0;
        double IngestNoTox;


            void EatEgest_EatE(TPreference P)
            {
                TStateVariable PSV;
                double TL;
                double Nutr2Org;
                // -------------------------------------------------------------------------------
                double GetPreyTrophicLevel(AllVariables NS)
                {
                    if (NS < Consts.FirstInvert) return 1;           // all Anims and detritus to trophic level 1
                    else return ((PSV) as TAnimal).PreyTrophicLevel; // Trophic Level derived from feeding prefs
                }
                // -------------------------------------------------------------------------------
                PSV = AQTSeg.GetStatePointer(P.nState, T_SVType.StV, T_SVLayer.WaterCol);
                if ((P.Preference == 0) || ((PSV == null) && (P.nState!= AllVariables.SedmRefrDetr) && (P.nState != AllVariables.SedmLabDetr)))
                {
                    Ingestion = 0;
                    IngestNoTox = 0;
                }
                else
                {
                    Ingestion = IngestSpecies(P.nState, P, ref EgestRet, ref GER);
                    IngestNoTox = Ingestion / (1.0 - (0.2 * AggregateRedGrowth())); // toxicant effect is removed from ingestion calculation

                    ReminRecord RR = Location.Remin;

                    if (PSV != null) Nutr2Org = PSV.NutrToOrg(AllVariables.Nitrate);
                    else if (P.nState == AllVariables.SedmRefrDetr)
                          Nutr2Org = RR.N2Org_Refr; // diagenesis model special code
                    else  Nutr2Org = RR.N2OrgLab; // SedmLabDetr

                    NitrCons = NitrCons + Ingestion * Nutr2Org;

                    if (PSV != null)  Nutr2Org = PSV.NutrToOrg(AllVariables.Phosphate);
                    else if (P.nState == AllVariables.SedmRefrDetr)
                          Nutr2Org = RR.P2Org_Refr;  // diagenesis model special code
                    else  Nutr2Org = RR.P2OrgLab;    // SedmLabDetr

                    PhosCons = PhosCons + Ingestion * Nutr2Org;
                }
                if (!CalcEgest)
                {
                    if ((Ingestion > Consts.Tiny))
                    {
                        TL = GetPreyTrophicLevel(P.nState);
                        // 2/11/2013, calculate trophic level of prey
                        if (EECount == 0) TrophicLevel = (TL + 1.0);
                        else TrophicLevel = (((TL + 1.0) * Ingestion) + (TrophicLevel * EECount)) / (Ingestion + EECount);

                        // weighted average as a function of diet
                        EECount = EECount + Ingestion;
                    }
                }
                else
                {
                    IncrEgest = (1.0 - P.EgestCoeff) * 0.8 * AggregateRedGrowth();
                    EECount = EECount + (Ingestion * P.EgestCoeff) + (IngestNoTox * IncrEgest);
                }
            }
            // -------------------------------------------------------------------------------
        // EatEgest
        if (!CalcEgest) TrophicLevel = 2;   // 4/14/2014 avoid zero trophic levels even if no food available, minimum of 2.0 at 1/27/2015

        EECount = 0;
        NitrCons = 0;
        PhosCons = 0;
        foreach (TPreference TP in MyPrey) EatEgest_EatE(TP);

        return EECount;
    }

    // -------------------------------------------------------------------------------
    public double Consumption()
    {
        double Consume;
        if (State < Consts.Tiny)
              Consume = 0.0;
        else  Consume = EatEgest(false);  //CalcEgest=False -- calculate Eat not Egest
        return Consume;
    }

    // consume
    public double Defecation()
    {
        double Defecate;
        if ((State < Consts.Tiny) || IsLeavingSeg)
              Defecate = 0.0;
        else  Defecate = EatEgest(true);  //CalcEgest=True 
        return Defecate;
    }

    // Defecation
    // (*************************************)
    // (*     Loss due to respiration       *)
    // (*       last modified 9/23/03       *)
    // (*************************************)
    public override double Respiration()
    {
        double SpecDynAction;
        double SaltEffect;
        double Respire;
        double BasalResp;
        double StdResp;
        double RoutineResp;
        double Activity;
        double TCorr;
        double TFn;
        double Temp;
        double Vel;
        double DensityDep;
        const double IncrResp = 0.5;
        // modified 6/22/2009
        if ((State < Consts.Tiny) || IsLeavingSeg)
        {
            Respire = 0.0;
        }
        else
        {
            TCorr = AQTSeg.TCorr(PAnimalData.Q10, PAnimalData.TRef, PAnimalData.TOpt, PAnimalData.TMax);
            // Stroganov
            RoutineResp = PAnimalData.EndogResp;  // legacy name elsewhere in code
            SpecDynAction = PAnimalData.KResp * (Consumption() - Defecation());    // Hewett & Johnson '92

            if ((IsFish() || IsPlanktonInvert())) // 10-24-12 include inverts
                {
                DensityDep = 1.0 + (IncrResp * State) / KCAP_in_g_m3();  // Kitchell et al., 1974; Park et al., 1974.  Account for Crowding in fish, address KCAP in inverts
                }
            else
            {
                DensityDep = 1.0;
            }
            // allometric resp currently implemented for inverts 10/18/2013
            if (PAnimalData.UseAllom_R)
            {
                if (PAnimalData.UseSet1)
                {
                    Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
                    TFn = Math.Exp(PAnimalData.RQ * Temp);
                    if (Temp > PAnimalData.RTL)
                        Vel = PAnimalData.RK1 * Math.Pow(PAnimalData.MeanWeight, PAnimalData.RK4);
                    else
                        Vel = PAnimalData.ACT * Math.Exp(PAnimalData.BACT * Temp) * Math.Pow(PAnimalData.MeanWeight, PAnimalData.RK4);
                    Activity = Math.Exp(PAnimalData.RTO * Vel);
                }
                else
                {
                    Activity = PAnimalData.ACT;  // Set 2
                    TFn = TCorr;
                }
                BasalResp = PAnimalData.RA * 1.5;   // conversion from O2 to organic matter
                StdResp = State * BasalResp * Math.Pow(PAnimalData.MeanWeight, PAnimalData.RB) * TFn * DensityDep * Activity;
                // <-------------------- STDRESP_PRED -------------------- > < ActiveResp_PRED >
            }
            else
            {
                // not fish or not use allometric fish respiration calculation
                StdResp = RoutineResp * TCorr * State * DensityDep;
            }
            Respire = SpecDynAction + StdResp;
        }
        // with animaldata
        SaltEffect = AQTSeg.SalEffect(PAnimalData.Salmin_Rsp, PAnimalData.SalMax_Rsp, PAnimalData.Salcoeff1_Rsp, PAnimalData.Salcoeff2_Rsp);
        return Respire * SaltEffect;
    }

    // KCAP In proper Consts... 10-30-01, jsc
    public double KCAP_in_g_m3()
    {
        if (IsPlanktonInvert())  return PAnimalData.KCap;   // pelagic KCAP already in g/m3

        AQUATOXSegment TS = AQTSeg;
        // note, deeper sites have lower g/m3 KCAP
        // 11/3/2014 replaced static zmean with more consistent conversion
        return PAnimalData.KCap * TS.Location.Locale.SurfArea / TS.Volume_Last_Step;
       // g/m3           // g/m2                    // m2               // m3

       // removed vertical stratification code in which carrying capacity was split up based on epi benthic area, like the vars are
    }

    // (*************************************)
    // (*     Loss due to excretion         *)
    // (*************************************)
    public virtual double AnimExcretion()
    {
        if ((State < Consts.Tiny) || IsLeavingSeg) return 0;
        else return PAnimalData.KExcr * Respiration();
    }

    public double AmmoniaMortality_External_Mort(double LC50, double Conc)
    {
        double k, ETA, Slope;
        if (Conc < 0) Conc = 0;  // bullet proof

        Slope = 0.7 / LC50;
    // unitless // LC50*Slope  // LC50

        ETA = (-2.0 * LC50 * Slope) / Math.Log(0.5);
        try
        {
            k = -Math.Log(0.5) / Math.Pow(LC50, ETA);
            return 1.0 - Math.Exp(-k * Math.Pow(Conc, ETA));
        }
        catch
        {
            throw new Exception("Floating Point Error Calculating Ammonia Toxicity.  Re-examine input parameters.");
        }
    }

    // (*************************************)
    // (*   Loss due to ammonia toxicity    *)
    // (*************************************)
    public double AmmoniaMortality(bool ionized)
    {
        double result;
        int ionint;
        const double pHt = 7.204;
        const double R = 0.00704;
        double AmmoniaLC50;
        double TKelvin;
        double pkh;
        double Ammonia_Conc;
        double UnIonNH3;
        double pHval;
        double NewResist;
        double AmmoniaNonResistant;
        double CumFracNow;
        double FracKill;
        // AmmoniaMortality

        AmmoniaLC50 = PAnimalData.Ammonia_LC50;
        if (AmmoniaLC50 < Consts.Tiny) return 0;
        pHval = AQTSeg.GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
        if (ionized)
        {   // ionized
            AmmoniaLC50 = (AmmoniaLC50 / ((R / ((1.0 + Math.Pow(10.0, pHt - 8.0))) + (1.0 / (1.0 + Math.Pow(10.0, 8 - pHt)))))) * (1.0 / (1.0 + Math.Pow(10.0, pHval - pHt)));
        }
        else
        {   // unionized
            AmmoniaLC50 = (AmmoniaLC50 / ((R / ((1.0 + Math.Pow(10.0, pHt - 8.0))) + (1.0 / (1.0 + Math.Pow(10.0, 8 - pHt)))))) * (R / (1.0 + Math.Pow(10.0, pHt - pHval)));
        }

        Ammonia_Conc = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
        TKelvin = 273.16 + AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
        pkh = 0.09018 + 2729.92 / TKelvin;
        UnIonNH3 = Ammonia_Conc / (1.0 + Math.Pow(10.0, (pkh - AQTSeg.GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol))));
        // mg/L
        // fraction that is un-ionized
        if (ionized)
            CumFracNow = AmmoniaMortality_External_Mort(AmmoniaLC50, Ammonia_Conc - UnIonNH3);
        else
            CumFracNow = AmmoniaMortality_External_Mort(AmmoniaLC50, UnIonNH3);

        if (CumFracNow < Consts.Tiny) return 0;

        ionint = ionized ? 1 : 0;
        // 9-17-07 conversion of Resistant from biomass units to fraction units
        AmmoniaNonResistant = State * (1.0 - AmmoniaResistant[ionint]);
        // mg/L          mg/L          frac
        if (AmmoniaPrevFracKill[ionized ? 1 : 0] >= CumFracNow)
             FracKill = 0;
        else FracKill = (CumFracNow - AmmoniaPrevFracKill[ionint]) / (1.0 - AmmoniaPrevFracKill[ionint]);
        
        AQUATOXSegment TS = AQTSeg;
        result = AmmoniaResistant[ionized ? 1 : 0] * State * FracKill + AmmoniaNonResistant * CumFracNow;
        // mg/L-d               frac                   mg/L    g/g-d        mg/L                 g/g-d
        NewResist = (State - result) / State;
        // frac    // mg/L   // mg/L  // mg/L

        AmmoniaDeltaResistant[ionint, TS.DerivStep] = NewResist - AmmoniaResistant[ionint];
        AmmoniaDeltaCumFracKill[ionint, TS.DerivStep] = CumFracNow - AmmoniaPrevFracKill[ionint];
        return result;
    }

    // (*************************************)
    // (* Loss due to low oxygen            *)
    // (*************************************)
    public double O2EffectFrac(int O2Eff)
    {
        
        int[] CALCTIMES = { 1, 4, 12, 24, 48, 96 };    // hours
        const int CALCSTART1Day = 3;
        const int CALCSTOP = 5;
        int CalcIteration;
        int Cstart;
        int i;
        double CalcTime;
        double EffectFrac;
        double EffectPct;
        double TimePassed;
        double MaxO2;
        double Intercept;
            double O2EffectPct;
        TSVConc PO2;
        bool OverTime;
        bool Foundone;
        // optimization
        if (AQTSeg.TPresent == LastO2CalcTime[O2Eff])
        {
            return LastO2Calc[O2Eff];
        }
            var O2EffectConc = O2Eff switch
            {
                0 => PAnimalData.O2_LethalConc,
                1 => PAnimalData.O2_EC50growth,
                _ => PAnimalData.O2_EC50repro,
            };
            // case
            O2EffectPct = 50;
        if (O2Eff == 0)   //(0=O2Mortality)
            O2EffectPct = PAnimalData.O2_LethalPct;
 
        EffectFrac = 0;
        if ((O2EffectConc == 0) || (O2EffectPct > 99.9) || (O2EffectPct < 0.1))
             return 0.0;
        if (!AQTSeg.PSetup.ModelTSDays) // (AQTSeg.PModelTimeStep == TSHourly)
             Cstart = 0;
        else Cstart = CALCSTART1Day;
        for (CalcIteration = Cstart; CalcIteration <= CALCSTOP; CalcIteration++)
        {
            CalcTime = CALCTIMES[CalcIteration];
            MaxO2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            i = 0;
            OverTime = false;
            Foundone = false;
            
            if (AQTSeg.PO2Concs.Count > 0)
            {
                do
                {
                    PO2 = AQTSeg.PO2Concs[i];
                    TimePassed = (AQTSeg.TPresent - PO2.Time).TotalDays;
                     // days      
                    if ((TimePassed - 0.001) > (CalcTime / 24.0))
                    {                           // hours / h/d
                       OverTime = true;
                    }
                    else
                    {
                        // only evaluate longer times if enough data exist to support them
                        if ((CalcIteration == Cstart)) Foundone = true;
                        else if ((TimePassed > CALCTIMES[CalcIteration - 1] / 24.0)) Foundone = true;
                        if (PO2.SVConc > MaxO2)  MaxO2 = PO2.SVConc;
                        // find max during time period
                    }
                    i++;
                } while (!((i == AQTSeg.PO2Concs.Count) || OverTime));
            }
            if (Foundone) // must have enough of a time-record of O2 to make a comparison
            {
                Intercept = O2EffectConc + (0.007 * O2EffectPct);
                EffectPct = (((MaxO2 - 0.204 + 0.064 * Math.Log(CalcTime)) / (0.191 * Math.Log(CalcTime) + 0.392)) - Intercept) / -0.007;
                if (EffectPct > 100.0) EffectPct = 100.0;
                if (EffectPct < 0.0)   EffectPct = 0.0;
                if ((EffectFrac * 100.0 < EffectPct)) EffectFrac = EffectPct / 100.0;  // choose highest effect of time-periods evaluated                
            }
        }
        // Optimization
        LastO2CalcTime[O2Eff] = AQTSeg.TPresent;
        LastO2Calc[O2Eff] = EffectFrac;
        return EffectFrac;
    }

    public double Sediment_Mort()
    {
        double result=0;  // Fraction of mortality due to elevated sediment in a given time-step
        double[] CALCTIMES = { 1, 2, 7, 14, 21 };  // days
        const int CALCSTOPTOL = 1;
        const int CALCSTOP = 4;
        double SlopeSS;
        double InterceptSS;
        double SlopeTime;
        double ThisCumFrac;
        double MaxCumFrac;
        double NewResist;
        double SedNonResistant;
        double FracKill;
        int CStop;
        int CalcIteration;
        int i;
        double TimePassed;
        double CalcTime;
        double MinSS;
        bool OverTime;
        bool Foundone;
        TSVConc PSS;
        MaxCumFrac = 0;

        if (!IsFish()) return 0;

            // optimization
            if (AQTSeg.TPresent == LastSedCalcTime)
            {
                result = LastSedCalc;
                return result;
            }
            if (IsFish() && (PAnimalData.SenstoSediment != "Zero Sensitivity") && (PAnimalData.SenstoSediment != ""))
            {
                CStop = CALCSTOPTOL;
                SlopeSS = 1.62;
                InterceptSS = -14.2;
                SlopeTime = 3.5;
                // 'Tolerant'
                if (PAnimalData.SenstoSediment == "Sensitive")
                {
                    // 'Sensitive'
                    SlopeSS = 0.34;
                    InterceptSS = -1.85;
                    SlopeTime = 0.1;
                    CStop = CALCSTOP;
                }
                if (PAnimalData.SenstoSediment == "Highly Sensitive")
                {
                    // 'Highly Sensitive'
                    SlopeSS = 0.328;
                    InterceptSS = -1.375;
                    SlopeTime = 0.1;
                    CStop = CALCSTOP;
                }
                for (CalcIteration = 0; CalcIteration <= CStop; CalcIteration++)
                {
                    CalcTime = CALCTIMES[CalcIteration];
                    MinSS = AQTSeg.InorgSedConc();
                    i = 0;
                    OverTime = false;
                    Foundone = false;
                    if (AQTSeg.PSedConcs.Count > 0)
                    {
                        do
                        {
                            PSS = AQTSeg.PSedConcs[i];
                            TimePassed = (AQTSeg.TPresent - PSS.Time).TotalDays;
                            // days      (min)                           // d
                            if ((TimePassed - 0.001) > CalcTime)
                                OverTime = true;
                            else
                            {
                                // only evaluate longer times if enough data exist to support them
                                if ((CalcIteration == 0))
                                    Foundone = true;
                                else if ((TimePassed > CALCTIMES[CalcIteration - 1]))
                                    Foundone = true;
                                if (PSS.SVConc < MinSS)
                                    MinSS = PSS.SVConc;
                                // find minimum during time period
                            }
                            i++;
                        } while (!((i == AQTSeg.PSedConcs.Count) || OverTime));
                    }
                    if (Foundone && (MinSS > Consts.Tiny))
                    {
                        // must have enough of a time-record of TSS to make a comparison
                        ThisCumFrac = SlopeSS * Math.Log(MinSS) + InterceptSS + SlopeTime * Math.Log(CALCTIMES[CalcIteration]);
                        if (ThisCumFrac > MaxCumFrac)
                        {
                            MaxCumFrac = ThisCumFrac;
                        }
                    }
                    // foundone
                }
                // calciteration
                if (MaxCumFrac < Consts.Tiny)
                {
                    return result;
                }
                SedNonResistant = State * (1.0 - SedResistant);
                // mg/L            mg/L             frac
                if (SedPrevFracKill >= MaxCumFrac)
                {
                    FracKill = 0;
                }
                else
                {
                    FracKill = (MaxCumFrac - SedPrevFracKill) / (1.0 - SedPrevFracKill);
                }

                result = SedResistant * State * FracKill + SedNonResistant * MaxCumFrac;
                // mg/L-d           frac       mg/L     g/g-d        mg/L             g/g-d
                NewResist = (State - result) / State;
                // frac     // mg/L  // mg/L   // mg/L
                SedDeltaResistant[AQTSeg.DerivStep] = NewResist - SedResistant;
                SedDeltaCumFracKill[AQTSeg.DerivStep] = MaxCumFrac - SedPrevFracKill;
            }
            // IsFish
            // Optimization
            LastSedCalcTime = AQTSeg.TPresent;
            LastSedCalc = result;
            return result;
    }

    // (*************************************)
    // (* Loss due to nonpredatory mortality*)
    // (*************************************)
    public override double Mortality()
    {
        
        double Dead;
        double Pois;
        T_SVType ToxLoop;
        if ((State < Consts.Tiny) || IsLeavingSeg)
        {
            Dead = 0.0;
        }
        else
        {
            Dead = PAnimalData.KMort * State;
            // mg/L-d   g/g-d    mg/L
            if (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) > PAnimalData.TMax)
            {
                Dead = (PAnimalData.KMort + Math.Exp(AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) - PAnimalData.TMax) / 2.0) * State;
            }
                if (PAnimalData.SenstoPctEmbed && (AQTSeg.PercentEmbedded > PAnimalData.PctEmbedThreshold))   
            {
                    Dead = State;  // If site's percent embeddedness exceeds the threshold then assume 100% mortality
            }
                
            MortRates.OtherMort = Dead;
            MortRates.O2Mort = O2EffectFrac(0) * State;   //(0=O2Mortality)
            // mg/L-d     g/g-d                     mg/L
            MortRates.NH4Mort = AmmoniaMortality(true);
            // NH4+ is ionized
            MortRates.NH3Mort = AmmoniaMortality(false);
            // NH3 is unionized
            // mg/L-d         mg/L-d
            Dead = Dead + MortRates.O2Mort + MortRates.NH4Mort + MortRates.NH3Mort;
            // mg/L-d             // mg/L-d
            MortRates.SaltMort = State * SalMort(PAnimalData.Salmin_Mort, PAnimalData.SalMax_Mort, PAnimalData.Salcoeff1_Mort, PAnimalData.Salcoeff2_Mort);
            Dead = Dead + MortRates.SaltMort;
            // mg/L-d            // mg/L-d

            Setup_Record SR = AQTSeg.PSetup;
            for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)  
            {
               if ((SR.UseExternalConcs && (AQTSeg.GetStateVal(AllVariables.H2OTox, ToxLoop, T_SVLayer.WaterCol) > 0)) ||
                        ((!SR.UseExternalConcs) && (AQTSeg.GetStateVal(NState, ToxLoop, T_SVLayer.WaterCol) > 0)))
                {
                    Pois = Poisoned(ToxLoop);
                    MortRates.OrgPois[ToxInt(ToxLoop)] = Pois;
                    Dead = Dead + Pois;
                    // mg/L-d // mg/L-d // mg/L-d
                }
                else
                {
                    RedGrowth[ToxInt(ToxLoop)] = 0;  // 5/3/2017 defaults for no effects if tox is zero
                    RedRepro[ToxInt(ToxLoop)] = 0;
                    FracPhoto[ToxInt(ToxLoop)] = 1;
                }
            }

            MortRates.SedMort = Sediment_Mort();           // JSC 1/2/2020, removed * State, units arlready mg/L-d
            Dead = Dead + MortRates.SedMort;
            if (IsBenthos())
            {
                // 10/24/2012 Benthic inverts, hard cap at KCAP to represent available substrate
                if (State > KCAP_in_g_m3())
                {
                    Dead = Dead + (State - KCAP_in_g_m3());
                }
            }
            if (Dead > State)
            {
                Dead = State;
            }
            // mg/L-d  // mg/L
        }
        return Math.Abs(Dead);
    }

    public bool SpawnNow(DateTime InDate)
    {
        bool result;
        double TOptSpawn;
        // Returns true if due to temperature range or date, this species could spawn on this day
        double Temp;

            // --------------------------------------------------------------------
            bool SpawnNow_InTempRange()
            {
                return ((Temp >= 0.6 * TOptSpawn) && (Temp <= (TOptSpawn - 1.0)));
            }

            // --------------------------------------------------------------------
            bool SpawnNow_ThisIsSpawningDate()
            {
                bool UseDate1;
                bool UseDate2;
                bool UseDate3;
                int JulianNow = InDate.DayOfYear;

                UseDate1 = ((PAnimalData.UnlimitedSpawning) || (PAnimalData.SpawnLimit > 0)) && (PAnimalData.SpawnDate1 != DateTime.MinValue);
                UseDate2 = ((PAnimalData.UnlimitedSpawning) || (PAnimalData.SpawnLimit > 1)) && (PAnimalData.SpawnDate2 != DateTime.MinValue);
                UseDate3 = ((PAnimalData.UnlimitedSpawning) || (PAnimalData.SpawnLimit > 2)) && (PAnimalData.SpawnDate3 != DateTime.MinValue);
                return ((JulianNow == PAnimalData.SpawnDate1.DayOfYear) && UseDate1) || ((JulianNow == PAnimalData.SpawnDate2.DayOfYear) && UseDate2) || ((JulianNow == PAnimalData.SpawnDate3.DayOfYear) && UseDate3);
            }
            // --------------------------------------------------------------------
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);

        if (NState != AllVariables.Fish1) TOptSpawn = PAnimalData.TOpt;
        else TOptSpawn = ((AQTSeg.GetStatePointer(AllVariables.Fish2, T_SVType.StV, T_SVLayer.WaterCol)) as TAnimal).PAnimalData.TOpt;
        // for calculating prom/recr, FISH1 must use same TOPTSpawn as all other age classes

        if (PAnimalData.AutoSpawn)
             result = SpawnNow_InTempRange();
        else result = SpawnNow_ThisIsSpawningDate();   // spawning should occur due to temperature or user specification


        result = result && (PAnimalData.UnlimitedSpawning || (SpawnTimes < PAnimalData.SpawnLimit));
        // and they can still spawn this year

        return result;
    }

    public double GameteLoss()
    {
        double result;
        double GamLoss;
        double Temp;
        double SaltEffect;
        double Capacity;
        double FracAdults;
        double IncrMort;
        TAnimal SmFPtr;
        int AgeIndex;
        double SpawnAge;
        bool IsSmallSizeClass;
        double KCapConv;
        Gametes = 0.0;
        result = 0.0;
        Recruit = 0.0;
        if ((State < Consts.Tiny) || IsLeavingSeg)  return 0.0;
        if ((OysterCategory > 0) && (OysterCategory < 4))  return 0.0;

        // gameteloss only relevant for sack
        if (OysterCategory == 4)
        { // POlder points to veliger in this case and these variables track viable spawning/recruitment
             result = (((POlder) as TAnimal).PromoteLoss / (1.0 - PAnimalData.GMort)) * PAnimalData.GMort;
                // living spawn            // conv total spawn            // conv mort spawn
             return result;
        }
        KCapConv = KCAP_in_g_m3();
        if (IsInvertebrate() && (PSameSpecies == AllVariables.NullStateVar))
        {
            // INVERTEBRATE CODE
            // For now the best way to handle gamete loss in invertebrates
            // is to permit it to occur continually 1/17/97
            if (((State > Consts.Tiny) && (KCapConv > 0.0)))
            {
                if (State > KCapConv) Capacity = 0;
                else                  Capacity = KCapConv - State;

                FracAdults = 1.0 - Capacity / KCapConv;
                IncrMort = (1.0 - PAnimalData.GMort) * AggregateRedRepro();
                SaltEffect = 1 + SalMort(PAnimalData.Salmin_Gam, PAnimalData.SalMax_Gam, PAnimalData.Salcoeff1_Gam, PAnimalData.Salcoeff2_Gam);
                // Gameteloss is increased by salinity therefore the mortality equation is used
                Gametes = FracAdults * PAnimalData.PctGamete * State;
                result = (PAnimalData.GMort + IncrMort) * FracAdults * PAnimalData.PctGamete * SaltEffect * State;

                if (result > FracAdults * PAnimalData.PctGamete * State)
                    result = FracAdults * PAnimalData.PctGamete * State;
                // JSC 10/17/2012 added fracadults and percentgamete
            }
            return result;
            // invert calculation complete
        }

        
        Temp = AQTSeg.GetState( AllVariables.Temperature,T_SVType.StV, T_SVLayer.WaterCol);
        if (IsFish() || (PSameSpecies != AllVariables.NullStateVar))
        {
            // 4/14/2014 Add Spawning crabs to this logic
            // SIZE
            GamLoss = 0.0;
            FracAdults = 0.0;
            IsSmallSizeClass = (IsSmallFish() || IsSmallPI()) && (PSameSpecies != AllVariables.NullStateVar);
            // JSC 10/29/2014 allow single-compartment small fish (that are not size class linked) to spawn
            // there are fish available to spawn
            // which don't spawn
            if (SpawnNow(AQTSeg.TPresent) && !Spawned && (State > Consts.Tiny) && (KCapConv > 0.0) && !IsSmallSizeClass)
            {
                // commence spawning
                if (State > KCapConv)
                {
                    Capacity = 0;
                }
                else
                {
                    Capacity = KCapConv - State;
                }
                // If this compartment represents both small and large fish
                if ((PSameSpecies == AllVariables.NullStateVar))
                {
                    // 1.0 - Capacity/KCapConv    1/8/2015 based rougly on roberts life-history tables   then FracAdults := 1.0 - Capacity/KCapConv
                    FracAdults = 0.75;
                }
                else
                {
                    FracAdults = 1;
                }
                if (NState >= AllVariables.Fish1 && NState <= AllVariables.Fish15) // multi age fish
                    {
                    AgeIndex = (int)(NState) - (int)(AllVariables.Fish1) + 1;   // upper bound of fish age, years

                    SpawnAge = AQTSeg.MF_Spawn_Age; 
                    // age at which fish start spawning
                    if ((AgeIndex <= SpawnAge))
                    {
                        // if oldest age in class is less than spawning age, no adults in class
                        FracAdults = 0;
                    }
                    else if ((AgeIndex - 1 >= SpawnAge))
                    { // if youngest age in class exceeds spawning age, all adults in class
                            FracAdults = 1;
                    }
                    else
                    {
                        FracAdults = 1.0 - (SpawnAge - (AgeIndex - 1));  // deal with fractional SpawnAge
                    }
                    
                }
                IncrMort = (1.0 - PAnimalData.GMort) * AggregateRedRepro();
                Gametes = FracAdults * PAnimalData.PctGamete * State;
                GamLoss = (PAnimalData.GMort + IncrMort) * FracAdults * PAnimalData.PctGamete * State;
                if ((FracAdults * PAnimalData.PctGamete) > 0)
                {
                    // fish is spawning
                    if ((!(NState >= AllVariables.Fish1 && NState <= AllVariables.Fish15)) && (PSameSpecies == AllVariables.NullStateVar))
                    {
                        // this compartment represents large and small fish
                        Recruit = 0;
                    }
                    else
                    {
                        Recruit = -(1.0 - (PAnimalData.GMort + IncrMort)) * FracAdults * PAnimalData.PctGamete * State;
                        if (Recruit > 0)
                        {
                            Recruit = 0;
                        }
                        if (!(NState >= AllVariables.Fish1 && NState <= AllVariables.Fish15))
                        {
                            // assign proper recruit value to small fish of same species
                            SmFPtr = AQTSeg.GetStatePointer(PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                            SmFPtr.Recruit = -Recruit;
                        }
                    }
                }
            }
            // if spawnnow and not spawned
            SaltEffect = SalMort(PAnimalData.Salmin_Gam, PAnimalData.SalMax_Gam, PAnimalData.Salcoeff1_Gam, PAnimalData.Salcoeff2_Gam);
            // Gameteloss is increased by salinity therefore the mortality equation is used
            result = Math.Abs(GamLoss) * SaltEffect;
            if ((result > State * FracAdults * PAnimalData.PctGamete))
            {
                result = FracAdults * State * PAnimalData.PctGamete;
            }
            // 6/5/08, limit losses to frac.gametes
        }
        // with

        return result;
    }

    // (*************************************)
    // (* loss due to discharge from system *)
    // (*************************************)
    public virtual double Scour_Entrainment()
    {
        const int Gradual = 25;
        const int MaxRate = 1;   // /d
        double Vel;

        Vel = AQTSeg.Velocity(PAnimalData.PrefRiffle, PAnimalData.PrefPool, false);
        // 11/9/2001 constrain so does not exceed "state"
        if (Vel >= PAnimalData.VelMax)
            return MaxRate * State;
        else
            return State * MaxRate * (Math.Exp((Vel - PAnimalData.VelMax) / Gradual));
        // mg/L d   // mg/L  // /d             // cm/s            // cm/s      // cm/s
    }

    public double Drift()
    {
        double Wash, Dislodge, SegVolume, Disch, EC50Gr;
        double DriftThres, ToxLevel, AccelDrift;
        TAnimalTox PT;
        T_SVType ToxLoop;
        double InorgSdDep;
        Wash = 0.0;
        if (IsFish())
        {
            return 0;
        }
        // fish don't drift
        if ((State < Consts.Tiny) || IsLeavingSeg)
        {
            return 0;
        }
        if (IsInvertebrate())
        {
            SegVolume = AQTSeg.SegVol();
            Disch = AQTSeg.Location.Discharge;
            if (IsBenthos() || IsNektonInvert())
            {
                // benthic
                if (PAnimalData.AveDrift <= 0)
                {
                    return 0;
                }
                // only evaluate if zoobenthos is capable of drifting
                AccelDrift = 1.0;
                if (IsBenthos())  // NOT FOR NEKTON INVERTS
                    {
                    InorgSdDep = AQTSeg.InorgSedDep();
                    if ((InorgSdDep > PAnimalData.Trigger))
                        AccelDrift = Math.Exp(InorgSdDep - PAnimalData.Trigger);
                                                // kg/m2           // kg/m2
                    }
                Dislodge = PAnimalData.AveDrift * AccelDrift;

                for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)  
                {
                    PT = AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) as TAnimalTox;
                    if (PT != null)
                        {
                            PT = AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) as TAnimalTox;
                            EC50Gr = Anim_Tox[ToxInt(ToxLoop)].EC50_growth;
                            if (EC50Gr > Consts.Tiny)
                            {
                                DriftThres = Anim_Tox[ToxInt(ToxLoop)].Drift_Thresh;   // ug/L
                                ToxLevel = AQTSeg.GetState(AllVariables.H2OTox, ToxLoop, T_SVLayer.WaterCol);
                                if ((ToxLevel > DriftThres))  // Toxicant Induced Dislodge for this toxicant
                                    Dislodge = Dislodge + (ToxLevel - DriftThres) / ((ToxLevel - DriftThres) + EC50Gr);
                            }
                        }
                }   // toxloop

                    Wash = Dislodge * State;
              // g/m3 d  // /d     // g/m3
            }
            else
            {
                // pelagic
                Wash = (Disch / SegVolume) * State;
            }
        }
        // invertebrate
        // WashoutStep[AQTSeg.DerivStep] = Wash * AQTSeg.SegVol();
        return Wash;
    }

    // (* drift *)
    public override double Washout()
    {
        return Drift();
    }

        // ---------------------------------------------------------------------------------------------------------------------------------------


        public double GillUptake(T_SVType ToxType)
        {

            const double WEffO2 = 0.62;
            // McKim et al. 1985
            double KUptake;
            double Local_K1;
            double Local_K2;
            double O2Bio;
            double OxygenConc;
            TAnimalTox PT;
            double WEffTox;
            double Frac_This_Layer;
            double ToxState;
            T_SVType ToxLoop;
            // ---------------------------------------------------------------------------------------------------------------------------------------

            // removed multi-seg porewater gill uptake here, multi-layer sediment / chemical model not in HMS
            Frac_This_Layer = 1;
            ToxState = AQTSeg.GetState(AllVariables.H2OTox, ToxType, T_SVLayer.WaterCol);

            for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)   
            {
                Poisoned(ToxLoop); // Ensure RedGrowth data is up-to-date.  This is required for dietary uptake calculation in SpecDynAction to be correct
            }
            
            PT = AQTSeg.GetStatePointer(NState, ToxType, T_SVLayer.WaterCol) as TAnimalTox;
            if (PT == null) return 0;

            if (PT.Anim_Method == UptakeCalcMethodType.Default_Meth)
            {
                O2Bio = Location.Remin.O2Biomass;
                OxygenConc = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
                WEffTox = PT.WEffTox(PT.NonDissoc());
                if (OxygenConc > 0.0)
                {   // 1/d       unitless   mg/L-d       mgO2/mg    mgO2         unitless
                    KUptake = (WEffTox * Respiration() * O2Bio) / (OxygenConc * WEffO2);
                }
                else KUptake = 0.0;

                Local_K2 = Anim_Tox[ToxInt(ToxType)].Entered_K2;
                if (Local_K2 > 96.0)
                    KUptake = KUptake * (96.0 / Local_K2);  // scaling factor 10-02-03

                if (State < Consts.Tiny)
                    DerivedK1[ToxInt(ToxType)] = 0.0;
                else
                {
                    DerivedK1[ToxInt(ToxType)] = KUptake / (State * 1e-6); // 9/9/2010
                }  // L/kg D                     // 1/d  // bmass mg/L // kg/mg

                // removed pore water code

                // ug/L-d       1/d      set to 1.0       ug/L          unitless
                return KUptake * ToxState * Frac_This_Layer;
            }
            else  // non-default method
            {
                // ChemOption <> Default_Meth
                if (PT.Anim_Method == UptakeCalcMethodType.CalcK1)
                {
                    // 5/29/2015 add Bio_rate_const (KM) to K2 when estimating K1
                    Local_K1 = (Anim_Tox[ToxInt(ToxType)].Entered_K2 + Anim_Tox[ToxInt(ToxType)].Bio_rate_const) * Anim_Tox[ToxInt(ToxType)].Entered_BCF;
                }
                else
                {
                    Local_K1 = Anim_Tox[ToxInt(ToxType)].Entered_K1;
                }
                DerivedK1[ToxInt(ToxType)] = Local_K1;
                return Local_K1 * ToxState * State * 1e-6;
                //ug/L-d  //L/kg-d // ug/L   // mg/L  // kg/mg
            }
            
            // With AQTSeg
        }

        // -----------------------------------------------------------------------
        public void Calc_Prom_Recr_Emrg(double Growth)
    {
        // Const  KPro = 0.5; {+1, not YOY}
        TAnimal SmA;
        TAnimal LgA;
        double Temp;
        double Prom;
        AllVariables BigFishLoop;
        double KPro;
            // OysterSizeClassPromotion
            // ------------------------------------------------------------------------
            void Calc_Prom_Recr_Emrg_FishandCrabSizeClassPromotion()
            {
                double JunkDB=0;
                if (IsSmallFish() || IsSmallPI())
                {
                    LgA = AQTSeg.GetStatePointer(PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    LgA.GameteLoss();
                    // call LgGF^.GameteLoss to get "Recruit" Value
                    PromoteLoss = Prom;

                    // anadromous fish logic removed
                }
                else
                {
                    SmA = AQTSeg.GetStatePointer(PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    SmA.Derivative(ref JunkDB);
                    // JSC 11/6/2012 necessary for multi-threading
                    PromoteGain = SmA.PromoteLoss;

                    // anadromous fish logic removed

                }
                // SmallFish are derived first, so PromoteFrom will be current for the given time-step,  JSC Not neccesary with multi threading added line above

            }
            // ------------------------------------------------------------------------
            void Calc_Prom_Recr_Emrg_OysterSizeClassPromotion()
            {
                bool IsOysterHabitat;
                double JunkDB = 0;
                double Sal;
                if (NState >= AllVariables.Veliger1 && NState <= AllVariables.Veliger2)
                {
                    // veliger.  Calculate promotion to spat and recruitment from sack
                    // PYounger points to sack, POlder to Spat
                    if ((POlder == null) || (PYounger == null))
                        IsOysterHabitat = false;
                    else
                        IsOysterHabitat = (((POlder) as TAnimal).State > Consts.Tiny) || (((PYounger) as TAnimal).State > Consts.Tiny);

                    Sal = AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
                    // Salinity in 5-30 ppt {from Cake 2013 4/8/2015} and Seed or sack present (to provide hard substrate and cue)

                    if (IsOysterHabitat && (Sal > 5) && (Sal < 30))
                        PromoteLoss = State * 0.05;
                    // life span of approximately three weeks, the oldest gets promoted or approximately 5% of the biomass each day

                    if (IsOysterHabitat)
                    {
                        AnimalRecord ZR = ((PYounger) as TAnimal).PAnimalData;
                        if ((AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) > 10) && (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) > 20))
                            PromoteGain = ((PYounger) as TAnimal).State * ZR.PctGamete / 275.0 * (1.0 - ZR.GMort);
                    }
                    // for veliger "PromoteTo"= Recruitment from sack, "PYounger" = Sack  {to 275 days 4/8/2015}
                    // assumed 300 days above 20 deg C                    {days}  {living frac}
                }
                if ((NState >= AllVariables.Spat1 && NState <= AllVariables.Spat2))
                {
                    // Spat Calculate promotion to seed and recruitment from Veliger
                    PromoteGain = 0;
                    PromoteLoss = 0;
                    if ((POlder != null)) PromoteLoss = Prom;
                    // 0.5 * growth
                    if ((PYounger != null))
                    {
                        ((PYounger) as TAnimal).Calc_Prom_Recr_Emrg(0);
                        // Veliger promotion is not a function of growth
                        PromoteGain = ((PYounger) as TAnimal).PromoteLoss;
                    }
                }
                if (OysterCategory == 3)
                {
                    // Seed Calculate promotion to sack and recruitment from Spat
                    PromoteGain = 0;
                    PromoteLoss = 0;
                    if ((POlder != null)) PromoteLoss = Prom;
                    // 0.25 * growth
                    if ((PYounger != null))
                    {
                        ((PYounger) as TAnimal).Derivative(ref JunkDB);
                        // Update PromoteLoss
                        PromoteGain = ((PYounger) as TAnimal).PromoteLoss;
                    }
                }
                if (OysterCategory == 4)
                {
                    // Sack Calculate spawning and promotion from Seed
                    PromoteGain = 0;
                    PromoteLoss = 0;
                    if ((POlder != null)) PromoteLoss = ((POlder) as TAnimal).PromoteGain;
                    // POlder points to veliger in this case and these variables track spawning/recruitment
                    if ((PYounger != null))
                    {
                        ((PYounger) as TAnimal).Derivative(ref JunkDB);
                        // Update PromoteLoss
                        PromoteGain = ((PYounger) as TAnimal).PromoteLoss;
                    }
                }
            }

            // ------------------------------------------------------------------------

        PromoteLoss = 0;
        PromoteGain = 0;
        EmergeInsect = 0;
        KPro = 0.5;
        // continuous promotion to next size class or emergence
        Prom = KPro * Growth;
        if (Prom < 0)
        {
            Prom = 0;
        }
        if (OysterCategory > 0)
        {
            Calc_Prom_Recr_Emrg_OysterSizeClassPromotion();
        }
        if ((PSameSpecies != AllVariables.NullStateVar))
        {
            if ((NState >= Consts.FirstFish) || (NState >= AllVariables.SmallPI1 && NState <= AllVariables.PredInvt2))
            {
                Calc_Prom_Recr_Emrg_FishandCrabSizeClassPromotion();
            }
        }
        if ((NState == AllVariables.Fish1))
        {
            // Calculate Recruitment for YOY of multi-age fish
            Recruit = 0;
            for (BigFishLoop = AllVariables.Fish2; BigFishLoop <= AllVariables.Fish15; BigFishLoop++)
            {
                if (AQTSeg.GetStatePointer(BigFishLoop, T_SVType.StV, T_SVLayer.WaterCol) != null)
                {
                    LgA = AQTSeg.GetStatePointer(BigFishLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    LgA.GameteLoss();
                    // call LgGF^.GameteLoss to get Recruit
                    Recruit = Recruit - LgA.Recruit;
                    // sum recruitment from larger fish
                }
            }
        }
        // Fish1
        // Promotion of multi-age fish is found in TStates.DoThisEveryStep
        if ((PAnimalData.Animal_Type == "Benthic Insect"))
        {
                // emergeinsect calculation
                Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
                if ((Temp > 0.8 * PAnimalData.TOpt) && (Temp < (PAnimalData.TOpt - 1.0)))
            {
                EmergeInsect = 2.0 * Prom;
            }
        }
    }

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    T_SVType ToxLoop;
        //    Setup_Record 1 = AQTSeg.SetupRec;
        //    if ((1.SaveBRates || 1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", Lo);
        //        SaveRate("Consumption", Co);
        //        SaveRate("Defecation", De);
        //        SaveRate("Respiration", Re);
        //        SaveRate("Excretion", Ex);
        //        SaveRate("Fishing", Fi);
        //        if (!IsPlanktonInvert())
        //        {
        //            SaveRate("Scour_Entrain", Entr);
        //        }
        //        if (IsInvertebrate() && IsPlanktonInvert())
        //        {
        //            if (AQTSeg.EstuarySegment)
        //            {
        //                SaveRate("Estuary.Entrain", En);
        //            }
        //            if (!AQTSeg.LinkedMode)
        //            {
        //                SaveRate("TurbDiff", TD);
        //            }
        //            else
        //            {
        //                SaveRate("DiffUp", DiffUp);
        //                SaveRate("DiffDown", DiffDown);
        //            }
        //        }
        //        SaveRate("Predation", Pr);
        //        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
        //        {
        //            if (AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) != null)
        //                SaveRate(Consts.PrecText(ToxLoop) + " Poisoned", MortRates.OrgPois[ToxLoop]);
        //                // SaveRate(PrecText(ToxLoop)+' PrevFracKill', PrevFracKill[ToxLoop] );
        //        }
        //        // SaveRate('RedGrowth_LIM',AggregateRedGrowth);
        //        // SaveRate('RedRepro_LIM',AggregateRedRepro);
        //        SaveRate("Low O2 Mort", MortRates.O2Mort);
        //        SaveRate("NH3 Mort", MortRates.NH3Mort);
        //        SaveRate("NH4+ Mort", MortRates.NH4Mort);
        //        if (AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) != -1)
        //            SaveRate("Salt Mort", MortRates.SaltMort);
        //        SaveRate("Other Mort", MortRates.OtherMort);
        //        if (IsFish()) SaveRate("Susp. Sed. Mort", MortRates.SedMort);
        //        SaveRate("Mortality", Mo);
        //        SaveRate("GameteLoss", Ga);
        //        SaveRate("Gametes", Gametes);
        //        SaveRate("Washout-Drift", DrO);
        //        if (AQTSeg.LinkedMode) SaveRate("DriftIn", DrI);

        //        if ((NState >= AllVariables.SmallPI1) && (NState < AllVariables.Fish1))
        //        {
        //            // has potential to be size class animal
        //            if ((State < Consts.Tiny) && (Loading < Consts.Tiny))
        //            {
        //                if (IsSmallFish() || IsSmallPI())
        //                    SaveRate("Promotn. Loss", 0);
        //                else
        //                    SaveRate("Promotn. Gain", 0);
        //                SaveRate("Recruit", 0);
        //            }
        //            else
        //            {
        //                if (IsSmallFish() || IsSmallPI())
        //                    SaveRate("Promotn. Loss", PLs);
        //                else
        //                    SaveRate("Promotn. Gain", Pgn);
        //                SaveRate("Recruit", Recr);
        //            }
        //        }
        //        // Sm,Lg GameFish
        //        if ((OysterCategory > 0))
        //        {
        //            // above veliger
        //            if (OysterCategory > 1)      // gain from lower category
        //                 SaveRate("Promotn. Gain", Pgn);
        //            else  SaveRate("Recruit", Pgn);
        //            // veliger gain from spawning
        //            // below sack
        //            if (OysterCategory < 4)
        //                // promotion to higher category
        //                SaveRate("Promotn. Loss", PLs);
        //                else SaveRate("Spawning", PLs);
        //            // sack "promotion" is spawning
        //        }
        //        if (NState >= AllVariables.Fish1 && NState <= AllVariables.Fish15)   SaveRate("Recruit", Recr);
        //        if ((PAnimalData.Animal_Type == "Benthic Insect"))  SaveRate("EmergeI", Emrg);

        //        if (CanMigrate()) SaveRate("StratMigration", Migr);

        //        SaveRate("GrowthRate", Co - De - Re - Ex);
        //        SaveRate("GrowthRate2", Co - De - Re - Ex);
        //    }
        //}

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double NutrFrac;
        //    double FishInKg;
        //    double LoadInKg;
        //    double LossInKg;
        //    double LayerInKg;
        //    AllVariables Typ;
        //    for (Typ = AllVariables.Nitrate; Typ <= AllVariables.Phosphate; Typ++)
        //    {
        //        if (Typ == AllVariables.Nitrate)
        //            NutrFrac = PAnimalData.N2Org;
        //        else
        //            NutrFrac = PAnimalData.P2Org;
        //        // MBLoadRecord MB = AQTSeg.MBLoadArray[Typ];
        //        // save for tox loss output & categorization
        //        LoadInKg = Lo * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}
        //        2.BoundLoad[AQTSeg.DerivStep] = 2.BoundLoad[1.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        if (En <= 0)
        //            LoadInKg = (Lo + DrI) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        else
        //            LoadInKg = (Lo + DrI + En) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}

        //        2.TotOOSLoad[AQTSeg.DerivStep] = 2.TotOOSLoad[AQTSeg.DerivStep] + LoadInKg;
        //        // out of segment load
        //        2.LoadBiota[AQTSeg.DerivStep] = 2.LoadBiota[AQTSeg.DerivStep] + LoadInKg;
        //        MorphRecord 3 = AQTSeg.Location.Morph;
        //        MBLossRecord 4 = AQTSeg.MBLossArray[Typ];
        //        // save for tox loss output & categorization
        //        // * OOSDischFrac
        //        LossInKg = (DrO + Entr) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // 3/20/2014 remove OOSDischFrac, now out of segment loss
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}

        //        4.BoundLoss[AQTSeg.DerivStep] = 4.BoundLoss[AQTSeg.DerivStep] + LossInKg;
        //        // Loss from the system
        //        if (En < 0)
        //            // * OOSDischFrac
        //            LossInKg = (-En + (DrO + Entr)) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;

        //        // loss from this segment
        //        // 3/20/2014 remove OOSDischFrac
        //        FishInKg = Fi * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // 6/30/2009 broke out fishing from washout in MB Tracking
        //        4.FishingLoss[AQTSeg.DerivStep] = 4.FishingLoss[AQTSeg.DerivStep] + FishInKg;
        //        4.BoundLoss[AQTSeg.DerivStep] = 4.BoundLoss[AQTSeg.DerivStep] + FishInKg;
        //        // Loss from the system
        //        4.TotalNLoss[AQTSeg.DerivStep] = 4.TotalNLoss[AQTSeg.DerivStep] + LossInKg + FishInKg;
        //        4.TotalWashout[AQTSeg.DerivStep] = 4.TotalWashout[AQTSeg.DerivStep] + LossInKg;
        //        4.WashoutAnim[AQTSeg.DerivStep] = 4.WashoutAnim[AQTSeg.DerivStep] + LossInKg;
        //        LossInKg = Emrg * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // emergence
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}
        //        4.TotalNLoss[AQTSeg.DerivStep] = 4.TotalNLoss[AQTSeg.DerivStep] + LossInKg;
        //        4.BoundLoss[AQTSeg.DerivStep] = 4.BoundLoss[AQTSeg.DerivStep] + LossInKg;
        //        // Loss from the modeled system
        //        4.EmergeIns[AQTSeg.DerivStep] = 4.EmergeIns[AQTSeg.DerivStep] + LossInKg;
        //        MBLayerRecord 5 = AQTSeg.MBLayerArray[Typ];
        //        // save for tox loss output & categorization
        //        LayerInKg = (TD + DiffUp + DiffDown) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}
        //        5.NTurbDiff[AQTSeg.DerivStep] = 5.NTurbDiff[AQTSeg.DerivStep] + LayerInKg;
        //        5.NNetLayer[AQTSeg.DerivStep] = 5.NNetLayer[AQTSeg.DerivStep] + LayerInKg;
        //        LayerInKg = Migr * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //       {kg nutr}   {mg org/L}    {m3}  {L/m3} {kg/mg} {nutr / org}
        //        5.NMigrate[AQTSeg.DerivStep] = 5.NMigrate[AQTSeg.DerivStep] + LayerInKg;
        //        5.NNetLayer[AQTSeg.DerivStep] = 5.NNetLayer[AQTSeg.DerivStep] + LayerInKg;
        //    }
        //}

        // Promote_Recruit_Emerge
        // (************************************)
        // (*                                  *)
        // (*     DIFFERENTIAL EQUATIONS       *)
        // (*                                  *)
        // (************************************)
        public override void Derivative(ref double DB)
    {
        double Lo=0;
        double Co=0;
        double De=0;
        double Re=0;
        double Ex=0;
        double Pr=0;
        double Mo=0;
        double Ga=0;
        double Fi=0;
        double DrO=0;
        double TD=0;
        double PLs=0;
        double Pgn=0;
        double Recr=0;
        double Emrg=0;
        double Migr=0;
        double DrI=0;
        double DiffUp=0;
        double DiffDown=0;
        double Entr=0;
        double En=0;
        // --------------------------------------------------
        T_SVType ToxLoop;

        MortRates.OtherMort = 0;
        Fi = 0;
        MortRates.SaltMort = 0;
        En = 0;
        TrophicLevel = 2;
        // 4/14/2014 avoid zero trophic levels even if no food available
        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
        {
            MortRates.OrgPois[ToxInt(ToxLoop)] = 0;
        }
//        Migr = StratMigration();      // StratMigration calculated first to set IsMigrAnoxia to limit other loss terms in that case

        if ((NState >= AllVariables.Veliger1))
        {
            Calc_Prom_Recr_Emrg(0);
        }
        // 10/21/2010 calculate promotion from smaller fish (PromSmFish) for optimization code test below.  3/5/2013, or promotion for oysters
        // Spawning can be an important additive rate (recruit)
        if (((State < Consts.Tiny) && (Migr < Consts.Tiny) && (Loading < Consts.Tiny)) && (PromoteGain < Consts.Tiny) && (!(SpawnNow(AQTSeg.TPresent) && !Spawned)) && (Recruit < Consts.Tiny))    // && (Washin() < Consts.Tiny)
            {
            DB = 0.0;
            RecrSave = 0;
            }
        else
        {
//            Migr = StratMigration();
            Mo = Mortality();
            // Mortality calculated second for Chronic Effect Calculation
            Lo = Loading;
            Co = Consumption();
            De = Defecation();
            Re = Respiration();
            Ex = AnimExcretion();
            Re = Re - Ex;
            // separate out excretion from respiration  5/13/2013
            Pr = Predation();
            Ga = GameteLoss();
            DrO = Drift();
            Calc_Prom_Recr_Emrg(Co - De - Re - Ex - Ga);
            if (NState < AllVariables.Fish1)
            {
                Pgn = PromoteGain;  
                PLs = PromoteLoss;
            }
            Fi = PAnimalData.Fishing_Frac * State;
            //if (AQTSeg.LinkedMode)   DrI = Washin();
            Recr = Recruit;   // Recr value is used in DoThisEveryStep
            Emrg = EmergeInsect;
            if (!IsPlanktonInvert())  Entr = Scour_Entrainment();    // Plankton invertebrates are subject to currents

            // removed linked mode, stratification, and estuary code as not relevant to HMS
            
            DB = Lo + Co - De - Re - Ex - Mo - Pr - Fi - Ga - DrO + DrI + Pgn - PLs - Emrg + En - Entr + TD + Migr + DiffUp + DiffDown;
            // mg/L      // all mg/L

            // Multi-Fish Promotion occurs on the first spawning of the year.
            // Size-class Recruitment occurs each time spawning occurs.
            // These effects are calculated after the timestep is complete;
            // They are unique events, not suitable for the daily timestep inherent in RKQS
            // See TSTATES.DoThisEveryStep

            if ((AQTSeg.DerivStep == 5)) RecrSave = Recr;
            // derivstep 5 is time X+h
        }
        // State>Tiny

        //Derivative_WriteRates();
        //Derivative_TrackMB();
    }


} // end TAnimal


} // namespace
