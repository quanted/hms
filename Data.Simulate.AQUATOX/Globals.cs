using System;


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
        public const AllVariables FirstBiota = AllVariables.Diatoms1;
        public const AllVariables LastBiota = AllVariables.Fish15;


        public const T_SVType FirstOrgTxTyp = T_SVType.OrgTox1;
        public const T_SVType LastOrgTxTyp = T_SVType.OrgTox20;
    }

    public class TSVConc
    {
        public double SVConc;
        public DateTime Time;
    }

    public class TParameter
    {
        public double Val;
        public object Symbol;
        public object Name;
        public object Comment;
        public object Units;
    } // end TParameter

    public class Diagenesis_Rec
    {
        public TParameter m1;        // = 0.5;           //(kg/L) solids concentration in layer 1
        public TParameter m2;        // = 0.5;           //(kg/L) solids concentration in layer 2
        public TParameter H1;        // = 0.001;         // meters, 1 mm aerobic layer
        public TParameter Dd;        // = 0.001;         //(m^2/d) pore water diffusion coefficient
        public TParameter w2;        // = 0.0003;        //(m/d) deep burial velocity (Q2K uses 0.000005)
        public TParameter H2;        // = 0.1;           //(m) thickness of sediment anaerobic layer 2
        public TParameter KappaNH3f;        // = 0.131;  //(m/d) freshwater nitrification velocity
        public TParameter KappaNH3s;        // = 0.131;  //(m/d) saltwater nitrification velocity
        public TParameter KappaNO3_1f;      // = 0.1;  //(m/d) freshwater denitrification velocity
        public TParameter KappaNO3_1s;      // = 0.1;  //(m/d) saltwater denitrification velocity
        public TParameter KappaNO3_2;        // = 0.25;  //(m/d) denitrification in the anaerobic layer 2
        public TParameter KappaCH4;        // = 0.7;     //(m/d) methane oxidation in the aerobic sedliment layer 1
        public TParameter KM_NH3;        // = 0.728;     //(mgN/L) nitrification half-saturation constant for NH4N
        public TParameter KM_O2_NH3;        // = 0.37;   //(mgO2/L) nitrification half-saturation constant for O2 (DiToro suggests 0.74)
        public TParameter KdNH3;        // = 1;          //(L/kg) partition coefficient for ammonium in layer 1 and 2
        public TParameter KdPO42;        // = 20;        //(L/kg) partition coefficient for inorganic P in anaerobic layer 2
        public TParameter dKDPO41f;        // = 20;      //(unitless) freshwater factor that increases the aerobic layer partition coefficient of inorganic P relative to the anaerobic partition coefficient   //gp
        public TParameter dKDPO41s;        // = 20;      //(unitless) saltwater factor that increases the aerobic layer partition coefficient of inorganic P relative to the anaerobic partition coefficient    //gp
        public TParameter O2critPO4;        // = 2;      //(mgO2/L) critical O2 concentration for adjustment of partition coefficient for inorganic P
        public TParameter Unused_ThtaDp;        // = 1.117;     //for bioturbation particle mixing between layers 1-2
        public TParameter ThtaDd;        // = 1.08;      //for pore water diffusion between layers 1-2
        public TParameter ThtaNH3;        // = 1.123;    //for nitrification
        public TParameter ThtaNO3;        // = 1.08;     //for denitrification
        public TParameter ThtaCH4;        // = 1.079;    //for methane oxidation
        public TParameter SALTSW;        // = 1;         //(ppt) salinity above which sulfide rather than methane is produced from C diagenesis
        public TParameter SALTND;        // = 1;         //(ppt) salinity above which saltwater nitrification/denitrification rates are used for aerobic layer
        public TParameter KappaH2Sd1;        // = 0.2;   //(m/d) aerobic layer reaction velocity for dissolved sulfide oxidation
        public TParameter KappaH2Sp1;        // = 0.4;   //(m/d) aerobic layer reaction velocity for particulate sulfide oxidation
        public TParameter ThtaH2S;        // = 1.08;     //(unitless) temperature coefficient for sulfide oxidation
        public TParameter KMHSO2;        // = 4;         //(mgO2/L) sulfide oxidation normalization constant for O2
        public TParameter KdH2S1;        // = 100;       //(L/kg) partition coefficient for sulfide in aerobic layer 1
        public TParameter KdH2S2;        // = 100;       //(L/kg) partition coefficient for sulfide in anaerobic layer 2
        public TParameter Unused_frpon1;        // = 0.65;      //fraction of class 1 pon
        public TParameter Unused_frpon2;        // = 0.25;      //fraction of class 2 pon
        public TParameter Unused_frpoc1;        // = 0.65;      //fraction of class 1 poc
        public TParameter Unused_frpoc2;        // = 0.2 ;      //fraction of class 2 poc
        public TParameter Unused_frpop1;        // = 0.65;      //fraction of class 1 pop
        public TParameter Unused_frpop2;        // = 0.2 ;      //fraction of class 2 pop
        public TParameter kpon1;        // = 0.035;      //(1/d) G class 1 pon mineralization
        public TParameter kpon2;        // = 0.0018;     //(1/d) G class 2 pon mineralization
        public TParameter kpon3;        // = 0;          //(1/d) G class 2 pon mineralization
        public TParameter kpoc1;        // = 0.035;      //(1/d) G class 1 poc mineralization
        public TParameter kpoc2;        // = 0.0018;     //(1/d) G class 2 poc mineralization
        public TParameter kpoc3;        // = 0;          //(1/d) G class 2 poc mineralization
        public TParameter kpop1;        // = 0.035;      //(1/d) G class 1 pop mineralization
        public TParameter kpop2;        // = 0.0018;     //(1/d) G class 2 pop mineralization
        public TParameter kpop3;        // = 0;          //(1/d) G class 2 pop mineralization
        public TParameter ThtaPON1;        // = 1.1;     //for G class 1 pon
        public TParameter ThtaPON2;        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPON3;        // = 1.17;    //for G class 3 pon
        public TParameter ThtaPOC1;        // = 1.1 ;    //for G class 1 pon
        public TParameter ThtaPOC2;        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPOC3;        // = 1.17;    //for G class 3 pon
        public TParameter ThtaPOP1;        // = 1.1 ;    //for G class 1 pon
        public TParameter ThtaPOP2;        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPOP3;        // = 1.17;    //for G class 3 pon
        public TParameter Unused_POC1R;        // = 0.1;   //reference G1 at which w12base = Dp / H2 at 20 degC for DiToro eqn 13.1
        public TParameter kBEN_STR;        // = 0.03;    //first-order decay rate constant for benthic stress (1/d) for DiToro eqn 13.3
        public TParameter Unused_KM_O2_Dp;        // = 4;
        public TParameter ksi;        // First order dissolution rate for particulate biogenic silica (PSi) at 20 degC in layer 2 (1/day)
        public TParameter ThtaSi;        // Constant for temperature adjustment of KSi (unitless)
        public TParameter KMPSi;        // Silica dissolution half-saturation constant for PSi (g Si/m^3)
        public TParameter SiSat;        // Saturation concentration of silica in pore water (g Si/m^3)
        public TParameter KDSi2;        // Partition coefficient for Si in Layer 2, controls sorption of dissolved silica to solids (L/Kg d)
        public TParameter DKDSi1;        // factor that enhances sorption of silica in layer 1 when D.O. exceeds DOcSi (unitless)
        public TParameter O2critSi;        // Critical dissolved oxygen for silica sorption in layer 1 (mg/L)
        public TParameter LigninDetr;        // Fraction of suspended detritus that is non-reactive (frac.)
        // Unused_Dp : TParameter;   = 0.00012;       //(m^2/d) bioturbation particle mixing diffusion coefficient
        public TParameter Si_Diatom;
        public double W12;
        public double KL12;
    } // end Diagenesis_Rec

}

