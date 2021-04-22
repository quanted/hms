using System;
using Microsoft.Extensions.Localization.Internal;
using Newtonsoft.Json;


namespace Globals
{
    public enum AllVariables
    {
        H2OTox,
        H2OTox2_deprecated,  // H2OTox2-20 no longer used in HMS code
        H2OTox3_deprecated,
        H2OTox4_deprecated,
        H2OTox5_deprecated,
        H2OTox6_deprecated,
        H2OTox7_deprecated,
        H2OTox8_deprecated,
        H2OTox9_deprecated,
        H2OTox10_deprecated,
        H2OTox11_deprecated,
        H2OTox12_deprecated,
        H2OTox13_deprecated,
        H2OTox14_deprecated,
        H2OTox15_deprecated,
        H2OTox16_deprecated,
        H2OTox17_deprecated,
        H2OTox18_deprecated,
        H2OTox19_deprecated,
        H2OTox20_deprecated,
        Ammonia,
        Nitrate,
        Phosphate,
        CO2,
        Oxygen,
        PoreWater,
        ReDOMPore,
        LaDOMPore,
        Sand,
        Silt,
        Clay,
        TSS,
        Silica,
        Avail_Silica,
        COD,
        TAM,
        Methane,
        Sulfide,
        POC_G1,
        POC_G2,
        POC_G3,
        PON_G1,
        PON_G2,
        PON_G3,
        POP_G1,
        POP_G2,
        POP_G3,
        Cohesives,
        NonCohesives,
        NonCohesives2,
        Salinity,
        SedmRefrDetr,
        SedmLabDetr,
        DissRefrDetr,
        DissLabDetr,
        SuspRefrDetr,
        SuspLabDetr,
        BuriedRefrDetr,
        BuriedLabileDetr,
        Diatoms1,
        Diatoms2,
        Diatoms3,
        Diatoms4,
        Diatoms5,
        Diatoms6,
        Greens1,
        Greens2,
        Greens3,
        Greens4,
        Greens5,
        Greens6,
        BlGreens1,
        BlGreens2,
        BlGreens3,
        BlGreens4,
        BlGreens5,
        BlGreens6,
        OtherAlg1,
        OtherAlg2,
        Macrophytes1,
        Macrophytes2,
        Macrophytes3,
        Macrophytes4,
        Macrophytes5,
        Macrophytes6,
        SuspFeeder1,
        SuspFeeder2,
        SuspFeeder3,
        SuspFeeder4,
        SuspFeeder5,
        SuspFeeder6,
        SuspFeeder7,
        SuspFeeder8,
        SuspFeeder9,
        DepFeeder1,
        DepFeeder2,
        DepFeeder3,
        Veliger1,
        Veliger2,
        Spat1,
        Spat2,
        Clams1,
        Clams2,
        Clams3,
        Clams4,
        Snail1,
        Snail2,
        SmallPI1,
        SmallPI2,
        PredInvt1,
        PredInvt2,
        PredInvt3,
        PredInvt4,
        SmForageFish1,
        SmForageFish2,
        LgForageFish1,
        LgForageFish2,
        SmBottomFish1,
        SmBottomFish2,
        LgBottomFish1,
        LgBottomFish2,
        SmGameFish1,
        SmGameFish2,
        SmGameFish3,
        SmGameFish4,
        LgGameFish1,
        LgGameFish2,
        LgGameFish3,
        LgGameFish4,
        Fish1,
        Fish2,
        Fish3,
        Fish4,
        Fish5,
        Fish6,
        Fish7,
        Fish8,
        Fish9,
        Fish10,
        Fish11,
        Fish12,
        Fish13,
        Fish14,
        Fish15,
        Volume,
        Temperature,
        WindLoading,
        Light,
        pH,
        NullStateVar
    } // end AllVariables

    
    //    public enum Alt_LoadingsType
    //    
    //0        PointSource,  or Inflow(TVolume)
    //1        DirectPrecip, or Discharge(TVolume)
    //2        NonPointSource
    //    } // end Alt_LoadingsType


    public enum T_SVType
    {
        StV,
        Porewaters,
        OrgTox1,
        OrgTox2,
        OrgTox3,
        OrgTox4,
        OrgTox5,
        OrgTox6,
        OrgTox7,
        OrgTox8,
        OrgTox9,
        OrgTox10,
        OrgTox11,
        OrgTox12,
        OrgTox13,
        OrgTox14,
        OrgTox15,
        OrgTox16,
        OrgTox17,
        OrgTox18,
        OrgTox19,
        OrgTox20,
        OtherOutput,
        NTrack,
        PTrack,
        NIntrnl,
        PIntrnl
    } // end T_SVType


    public enum T_SVLayer
    {
        WaterCol,
        SedLayer1,
        SedLayer2,
        //SedLayer3,
        //SedLayer4,
        //SedLayer5,
        //SedLayer6,
        //SedLayer7,
        //SedLayer8,
        //SedLayer9,
        //SedLayer10
    } // end T_SVLayer

    public class Setup_Record
    {
        public TDateParam FirstDay = new TDateParam();
        public TDateParam LastDay = new TDateParam();
        public TParameter StoreStepSize = new TParameter();
        public TParameter MinStepSize = new TParameter();
        public TParameter RelativeError = new TParameter();
        public TBoolParam SaveBRates = new TBoolParam();
        // public TBoolParam AlwaysWriteHypo;  Irrelevant to AQUATOX 4.0
        // public TBoolParam ShowIntegration;  Irrelevant to AQUATOX 4.0
        // public TBoolParam UseComplexedInBAF; Irrelevant to AQUATOX 4.0
        public TBoolParam ChemsDrivingVars = new TBoolParam();
        public TBoolParam AverageOutput = new TBoolParam();
        public TBoolParam UseExternalConcs = new TBoolParam();
        public TBoolParam StepSizeInDays = new TBoolParam();
        public TBoolParam ModelTSDays = new TBoolParam();
        // public TBoolParam Spinup_Mode;  Not yet part ofAQUATOX 4.0
        public TBoolParam NFix_UseRatio = new TBoolParam();   // 3/16/2010, option to use NFix Ratio
        public TParameter NtoPRatio = new TParameter();     // 3/18/2010, capability to specify NFix Ratio
        // public TBoolParam Spin_Nutrients;  Not yet part ofAQUATOX 4.0
        public TParameter FixStepSize = new TParameter();
        public TBoolParam UseFixStepSize = new TBoolParam();
        public TBoolParam Internal_Nutrients = new TBoolParam();
        public TBoolParam T1IsAggregate = new TBoolParam();
        public TBoolParam AmmoniaIsDriving = new TBoolParam();
        public TBoolParam TSedDetrIsDriving = new TBoolParam();

        public void Setup(bool DefaultVals)
        {
            FirstDay.Name = "End of Model Simulation";
            LastDay.Name = "End of Model Simulation";
            StoreStepSize.Name = "Data-storage Step Size (averaging period)";
            StoreStepSize.Units = "hours or days";
            MinStepSize.Name = "Minimum Step Size";
            MinStepSize.Units = "days";

            RelativeError.Name = "Relative Error";
            RelativeError.Units = "fraction";

            SaveBRates.Name = "Save Derivative Rates for Simulation";

            //ShowIntegration.Name = "
            //UseComplexedInBAF.Name = "

            ChemsDrivingVars.Name = "Chemicals are 'Driving Variables'";
            AverageOutput.Name = "Trapezoidally Integrate Results";
            UseExternalConcs.Name = "Calculate Toxicity using External Concentrations";
            StepSizeInDays.Name = "Storage Stepsize is Days (not Hours)";
            ModelTSDays.Name = "Model Stepsize is Days (not Hours)";
            // Spinup_Mode.Name = "

            NFix_UseRatio.Name = "Calculate Nitrogen Fixation using N to Inorganic P Ratio";

            NtoPRatio.Name = "N to Inorganic P Ratio for N-Fix";
            NtoPRatio.Units = "ratio";

            //Spin_Nutrients.Name = "

            FixStepSize.Name = "Fixed Step Size";
            FixStepSize.Units = "days";

            UseFixStepSize.Name = "Use Fixed Step Size";
            Internal_Nutrients.Name = "Model Nutrients Internally in Plants";
            T1IsAggregate.Name = "T1 is an aggregate of all other toxicants in study";
            AmmoniaIsDriving.Name = "Ammonia is a 'Driving Variable'";
            TSedDetrIsDriving.Name = "Toxicant in Sediment Detritus is a 'Driving Variable'";

            if (DefaultVals)
            {
                FirstDay.Val = new DateTime(1999, 1, 1);
                LastDay.Val = new DateTime(1999, 1, 31);
                StoreStepSize.Val = 1;
                StepSizeInDays.Val = true;
                ModelTSDays.Val = true;
                RelativeError.Val = 0.001;
                MinStepSize.Val = 1e-10;
                SaveBRates.Val = false;

                //AlwaysWriteHypo.Val = false;
                //ShowIntegration.Val = false;
                //UseComplexedInBAF.Val = false;
                //Spinup_Mode.Val = false;
                // Spin_Nutrients.Val = true;

                UseExternalConcs.Val = false;
                NFix_UseRatio.Val = false;
                NtoPRatio.Val = 7.0;
                FixStepSize.Val = 0.1;
                UseFixStepSize.Val = false;
                T1IsAggregate.Val = false;
                AmmoniaIsDriving.Val = false;
                TSedDetrIsDriving.Val = false;
            }
        }
    } // end Setup_Record

    public enum DetrDataType
    {
        CBOD,
        Org_Carb,
        Org_Matt
    } // end DetrDataType

    public enum TMacroType
    {
        Benthic,
        Rootedfloat,
        Freefloat
    } // end TMacroType

    public enum UptakeCalcMethodType   // method for calculating uptake into animals and plants
    {
        Default_Meth,
        CalcBCF,
        CalcK1,
        CalcK2
    }

    public class Consts
    {
        public const int NToxs = 20;
        public const double Tiny = 5.0e-19;   // mach. accuracy = 1.0e-19
        public const double Small = 1.0e-6;
        public const double VSmall = 1.0e-10;
        public const double Minimum_Stepsize = 1.0e-5;
        public const string DateFormatString = "yyyy-MM-dd'T'HH:mm:ss";
        public const string ValFormatString = "E";  // e.g. 1.043700E+021

        public const double C1 = 0.09788359788;
        public const double C3 = 0.40257648953;
        public const double C4 = 0.21043771044;
        public const double C6 = 0.28910220215;

        public const double Def2SedLabDetr = 0.5;  // Defecation that is labile 
        public const double Detr_OM_2_OC = 1.90;
        public const double KAnaerobic = 0.3;  //  (1/d)  decomp reduction - check Sanders, Gunnison

        public const AllVariables FirstPlant = AllVariables.Diatoms1;
        public const AllVariables LastPlant = AllVariables.Macrophytes6;
        public const AllVariables FirstDetr = AllVariables.SedmRefrDetr;
        public const AllVariables LastDetr = AllVariables.SuspLabDetr;
        public const AllVariables FirstAlgae = AllVariables.Diatoms1;
        public const AllVariables LastAlgae = AllVariables.OtherAlg2;
        public const AllVariables FirstDiatom = AllVariables.Diatoms1;
        public const AllVariables LastDiatom = AllVariables.Diatoms6;
        public const AllVariables FirstGreens = AllVariables.Greens1;
        public const AllVariables LastGreens = AllVariables.Greens6;
        public const AllVariables FirstBlGreen = AllVariables.BlGreens1;
        public const AllVariables LastBlGreen = AllVariables.BlGreens6;
        public const AllVariables FirstMacro = AllVariables.Macrophytes1;
        public const AllVariables LastMacro = AllVariables.Macrophytes6;
        public const AllVariables FirstAnimal = AllVariables.SuspFeeder1;
        public const AllVariables LastAnimal = AllVariables.Fish15;
        public const AllVariables FirstInvert = AllVariables.SuspFeeder1;
        public const AllVariables LastInvert = AllVariables.PredInvt4;
        public const AllVariables FirstDetrInv = AllVariables.DepFeeder1;
        public const AllVariables LastDetrInv = AllVariables.DepFeeder3;
        public const AllVariables FirstFish = AllVariables.SmForageFish1;
        public const AllVariables LastFish = AllVariables.Fish15;
        public const AllVariables FirstState = AllVariables.H2OTox;
        public const AllVariables LastState = AllVariables.NullStateVar;

        public const AllVariables FirstBiota = AllVariables.Diatoms1;
        public const AllVariables LastBiota = AllVariables.Fish15;

        public const T_SVType FirstOrgTxTyp = T_SVType.OrgTox1;
        public const T_SVType LastOrgTxTyp = T_SVType.OrgTox20;

    }

    public static class AQMath
    {
        public static double Square(double input)
        {
            return input * input;
        }

        public static double Cube(double input)
        {
            return input * input * input;
        }

    }

    public class TSVConc
    {
        public double SVConc;
        public DateTime Time;
    }

    public class TParameter
    {
        public double Val;
        [JsonIgnore] public string Symbol = "";   // not user editable;  
        [JsonIgnore] public string Name;          // not user editable
        public string Comment = "";
        [JsonIgnore] public string Units = "";    // not user editable
        [JsonIgnore] public bool Primary = true;         // not user editable
        //  [JsonIgnore] public bool Contextsensitive    // not yet  -- document 
    } // end TParameter

    public class TBoolParam : TParameter
    {
        new public bool Val;
    }

    public class TStringParam : TParameter
    {
      new public string Val;
    } 

    public class TDropDownParam : TParameter
    {
        new public string Val;
        public string[] ValList;  // not user editable
    }

    public class TDateParam : TParameter
    {
        new public DateTime Val;
        public string[] ValList;  // not user editable
    }

    public class TSubheading : TParameter  // used to format input screens
    {
        new public string Val;
        public bool expanded = false;

        public TSubheading(string Title, string Context) : base()
        {
            Val = Title;
            Comment = Context;
        }
    }

}

