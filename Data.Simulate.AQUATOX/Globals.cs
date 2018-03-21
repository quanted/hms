using System;


namespace Globals
{


    public enum AllVariables
    {
        H2OTox1,
        H2OTox2,
        H2OTox3,
        H2OTox4,
        H2OTox5,
        H2OTox6,
        H2OTox7,
        H2OTox8,
        H2OTox9,
        H2OTox10,
        H2OTox11,
        H2OTox12,
        H2OTox13,
        H2OTox14,
        H2OTox15,
        H2OTox16,
        H2OTox17,
        H2OTox18,
        H2OTox19,
        H2OTox20,
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
        NotUsed,
        NotUsed2,
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
        SedLayer3,
        SedLayer4,
        SedLayer5,
        SedLayer6,
        SedLayer7,
        SedLayer8,
        SedLayer9,
        SedLayer10
    } // end T_SVLayer

    public struct Setup_Record
    {
        public DateTime FirstDay;
        public DateTime LastDay;
        public double StoreStepSize;
        public double MinStepSize;
        public double RelativeError;
//        public bool Placeholder;    // equilibrium fugacity disabled
        public bool SaveBRates;
        public bool AlwaysWriteHypo;
        public bool ShowIntegration;
        public bool UseComplexedInBAF;
        public bool DisableLipidCalc;
        public bool ChemsDrivingVars;
        public bool AverageOutput;
        public bool UseExternalConcs;
        public bool NotUsedBCFUptake;     // Switched to chemical record 2/15/2013
        public bool StepSizeInDays;
        public bool ModelTSDays;
        public bool Spinup_Mode;     // to v 3.56
        public bool NFix_UseRatio;     // to v 3.66    // 3/16/2010, option to use NFix Ratio
        public double NtoPRatio;    // to v 3.67     // 3/18/2010, capability to specify NFix Ratio
        public bool Spin_Nutrients;    // to v 3.77
        public double FixStepSize;
        public bool UseFixStepSize;     // to 3.79
        public bool Internal_Nutrients;    // New to 3.83 and 3.1 plus
        public bool T1IsAggregate;    // to 3.94
        public bool AmmoniaIsDriving;     // to 3.94
        public bool TSedDetrIsDriving;     // to 3.94
    } // end Setup_Record

    public class Consts
    {
        public const double Tiny = 5.0e-19;   // mach. accuracy = 1.0e-19
        public const double Small = 1.0e-6;
        public const double VSmall = 1.0e-10;
        public const double Minimum_Stepsize = 1.0e-5;
        public const string DateFormatString = "yyyy-MM-dd'T'HH:mm:ss";
        public const string ValFormatString = "E";  // e.g. 1.043700E+021

        public const AllVariables FirstDetr = AllVariables.SedmRefrDetr;
        public const AllVariables LastDetr = AllVariables.SuspLabDetr;

    }
}