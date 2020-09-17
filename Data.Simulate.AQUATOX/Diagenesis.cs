using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Organisms;
using AQUATOX.OrgMatter;
using AQUATOX.Nutrients;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.Diagenesis
{
    public class Diagenesis_Rec
    {
        public TParameter m1= new TParameter();        // = 0.5;           //(kg/L) solids concentration in layer 1
        public TParameter m2 = new TParameter();        // = 0.5;           //(kg/L) solids concentration in layer 2
        public TParameter H1 = new TParameter();        // = 0.001;         // meters, 1 mm aerobic layer
        public TParameter Dd = new TParameter();        // = 0.001;         //(m^2/d) pore water diffusion coefficient
        public TParameter w2 = new TParameter();        // = 0.0003;        //(m/d) deep burial velocity (Q2K uses 0.000005)
        public TParameter H2 = new TParameter();        // = 0.1;           //(m) thickness of sediment anaerobic layer 2
        public TParameter KappaNH3f = new TParameter();        // = 0.131;  //(m/d) freshwater nitrification velocity
        public TParameter KappaNH3s = new TParameter();        // = 0.131;  //(m/d) saltwater nitrification velocity
        public TParameter KappaNO3_1f = new TParameter();      // = 0.1;    //(m/d) freshwater denitrification velocity
        public TParameter KappaNO3_1s = new TParameter();      // = 0.1;    //(m/d) saltwater denitrification velocity
        public TParameter KappaNO3_2 = new TParameter();       // = 0.25;   //(m/d) denitrification in the anaerobic layer 2
        public TParameter KappaCH4 = new TParameter();        // = 0.7;     //(m/d) methane oxidation in the aerobic sedliment layer 1
        public TParameter KM_NH3 = new TParameter();         // = 0.728;    //(mgN/L) nitrification half-saturation constant for NH4N
        public TParameter KM_O2_NH3 = new TParameter();       // = 0.37;    //(mgO2/L) nitrification half-saturation constant for O2 (DiToro suggests 0.74)
        public TParameter KdNH3 = new TParameter();          // = 1;        //(L/kg) partition coefficient for ammonium in layer 1 and 2
        public TParameter KdPO42 = new TParameter();        // = 20;        //(L/kg) partition coefficient for inorganic P in anaerobic layer 2
        public TParameter dKDPO41f = new TParameter();        // = 20;      //(unitless) freshwater factor that increases the aerobic layer partition coefficient of inorganic P relative to the anaerobic partition coefficient   //gp
        public TParameter dKDPO41s= new TParameter();        // = 20;      //(unitless) saltwater factor that increases the aerobic layer partition coefficient of inorganic P relative to the anaerobic partition coefficient    //gp
        public TParameter O2critPO4= new TParameter();       // = 2;       //(mgO2/L) critical O2 concentration for adjustment of partition coefficient for inorganic P
        //public TParameter Unused_ThtaDp= new TParameter();   // = 1.117;   //for bioturbation particle mixing between layers 1-2
        public TParameter ThtaDd= new TParameter();        // = 1.08;      //for pore water diffusion between layers 1-2
        public TParameter ThtaNH3= new TParameter();        // = 1.123;    //for nitrification
        public TParameter ThtaNO3= new TParameter();        // = 1.08;     //for denitrification
        public TParameter ThtaCH4= new TParameter();        // = 1.079;    //for methane oxidation
        public TParameter SALTSW= new TParameter();        // = 1;         //(ppt) salinity above which sulfide rather than methane is produced from C diagenesis
        public TParameter SALTND= new TParameter();        // = 1;         //(ppt) salinity above which saltwater nitrification/denitrification rates are used for aerobic layer
        public TParameter KappaH2Sd1= new TParameter();        // = 0.2;   //(m/d) aerobic layer reaction velocity for dissolved sulfide oxidation
        public TParameter KappaH2Sp1= new TParameter();        // = 0.4;   //(m/d) aerobic layer reaction velocity for particulate sulfide oxidation
        public TParameter ThtaH2S= new TParameter();        // = 1.08;     //(unitless) temperature coefficient for sulfide oxidation
        public TParameter KMHSO2= new TParameter();        // = 4;         //(mgO2/L) sulfide oxidation normalization constant for O2
        public TParameter KdH2S1= new TParameter();        // = 100;       //(L/kg) partition coefficient for sulfide in aerobic layer 1
        public TParameter KdH2S2= new TParameter();        // = 100;       //(L/kg) partition coefficient for sulfide in anaerobic layer 2
        //public TParameter Unused_frpon1= new TParameter();        // = 0.65;      //fraction of class 1 pon
        //public TParameter Unused_frpon2= new TParameter();        // = 0.25;      //fraction of class 2 pon
        //public TParameter Unused_frpoc1= new TParameter();        // = 0.65;      //fraction of class 1 poc
        //public TParameter Unused_frpoc2= new TParameter();        // = 0.2 ;      //fraction of class 2 poc
        //public TParameter Unused_frpop1= new TParameter();        // = 0.65;      //fraction of class 1 pop
        //public TParameter Unused_frpop2= new TParameter();        // = 0.2 ;      //fraction of class 2 pop
        public TParameter kpon1= new TParameter();        // = 0.035;      //(1/d) G class 1 pon mineralization
        public TParameter kpon2= new TParameter();        // = 0.0018;     //(1/d) G class 2 pon mineralization
        public TParameter kpon3= new TParameter();        // = 0;          //(1/d) G class 2 pon mineralization
        public TParameter kpoc1= new TParameter();        // = 0.035;      //(1/d) G class 1 poc mineralization
        public TParameter kpoc2= new TParameter();        // = 0.0018;     //(1/d) G class 2 poc mineralization
        public TParameter kpoc3= new TParameter();        // = 0;          //(1/d) G class 2 poc mineralization
        public TParameter kpop1= new TParameter();        // = 0.035;      //(1/d) G class 1 pop mineralization
        public TParameter kpop2= new TParameter();        // = 0.0018;     //(1/d) G class 2 pop mineralization
        public TParameter kpop3= new TParameter();        // = 0;          //(1/d) G class 2 pop mineralization
        public TParameter ThtaPON1= new TParameter();        // = 1.1;     //for G class 1 pon
        public TParameter ThtaPON2= new TParameter();        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPON3= new TParameter();        // = 1.17;    //for G class 3 pon
        public TParameter ThtaPOC1= new TParameter();        // = 1.1 ;    //for G class 1 pon
        public TParameter ThtaPOC2= new TParameter();        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPOC3= new TParameter();        // = 1.17;    //for G class 3 pon
        public TParameter ThtaPOP1= new TParameter();        // = 1.1 ;    //for G class 1 pon
        public TParameter ThtaPOP2= new TParameter();        // = 1.15;    //for G class 2 pon
        public TParameter ThtaPOP3= new TParameter();        // = 1.17;    //for G class 3 pon
        //public TParameter Unused_POC1R= new TParameter();        // = 0.1;   //reference G1 at which w12base = Dp / H2 at 20 degC for DiToro eqn 13.1
        public TParameter kBEN_STR= new TParameter();        // = 0.03;    //first-order decay rate constant for benthic stress (1/d) for DiToro eqn 13.3
        //public TParameter Unused_KM_O2_Dp= new TParameter();        // = 4;
        public TParameter ksi= new TParameter();        // First order dissolution rate for particulate biogenic silica (PSi) at 20 degC in layer 2 (1/day)
        public TParameter ThtaSi = new TParameter();        // Constant for temperature adjustment of KSi (unitless)
        public TParameter KMPSi= new TParameter();        // Silica dissolution half-saturation constant for PSi (g Si/m^3)
        public TParameter SiSat = new TParameter();        // Saturation concentration of silica in pore water (g Si/m^3)
        public TParameter KDSi2= new TParameter();        // Partition coefficient for Si in Layer 2, controls sorption of dissolved silica to solids (L/Kg d)
        public TParameter DKDSi1 = new TParameter();        // factor that enhances sorption of silica in layer 1 when D.O. exceeds DOcSi (unitless)
        public TParameter O2critSi= new TParameter();        // Critical dissolved oxygen for silica sorption in layer 1 (mg/L)
        public TParameter LigninDetr = new TParameter();        // Fraction of suspended detritus that is non-reactive (frac.)
        // Unused_Dp : TParameter;   = 0.00012;       //(m^2/d) bioturbation particle mixing diffusion coefficient
        public TParameter Si_Diatom= new TParameter();

        public double W12;  //calculation not parameter
        public double KL12; //calculation not parameter

        public void Setup(bool DefaultVals)
        {
            m1.Name = "Solids concentration in layer 1";
            m2.Name = "Solids concentration in layer 2";
            H1.Name = "Thickness of sediment aerobic layer 1";
            w2.Name = "Deep burial velocity";
            H2.Name = "Thickness of sediment anaerobic layer 2";
            KappaNH3f.Name = "Freshwater nitrification velocity";
            KappaNH3s.Name = "Saltwater nitrification velocity";
            KappaNO3_1f.Name = "Freshwater denitrification velocity";
            KappaNO3_1s.Name = "Saltwater denitrification velocity";
            KappaNO3_2.Name = "Denitrification in the anaerobic layer 2";
            KappaCH4.Name = "Methane oxidation in the aerobic sedliment layer 1";
            KM_NH3.Name = "Nitrification half-saturation constant for NH4N";
            KM_O2_NH3.Name = "Nitrification half-saturation constant for O2";
            KdNH3.Name = "Partition coefficient for ammonium in layer 1 and 2";
            KdPO42.Name = "Partition coefficient for inorganic P in anaerobic layer 2";
            dKDPO41f.Name = "Freshwater factor, incr. aerobic partition coeff. of inorg. P relative to the anaerobic";
            dKDPO41s.Name = "Saltwater factor, incr. aerobic partition coeff. of inorg. P relative to the anaerobic";
            O2critPO4.Name = "Critical O2 conc. for adjustment of partition coefficient for inorganic P";
            ThtaDd.Name = "Theta for pore water diffusion between layers 1-2";
            ThtaNH3.Name = "Theta for nitrification";
            ThtaNO3.Name = "Theta for denitrification";
            ThtaCH4.Name = "Theta for methane oxidation";
            SALTSW.Name = "Salinity above which sulfide rather than methane is produced from C diagenesis";
            SALTND.Name = "Salinity above which saltwater nitr./denitrification rates are used for aerobic layer";
            KappaH2Sd1.Name = "Aerobic layer reaction velocity for dissolved sulfide oxidation";
            KappaH2Sp1.Name = "Aerobic layer reaction velocity for particulate sulfide oxidation";
            ThtaH2S.Name = "Theta for sulfide oxidation";
            KMHSO2.Name = "Sulfide oxidation normalization constant for O2";
            KdH2S1.Name = "Partition coefficient for sulfide in aerobic layer 1";
            KdH2S2.Name = "Partition coefficient for sulfide in anaerobic layer 2";
            kpon1.Name = "G class 1 pon mineralization";
            kpon2.Name = "G class 2 pon mineralization";
            kpon3.Name = "G class 3 pon mineralization";
            kpoc1.Name = "G class 1 poc mineralization";
            kpoc2.Name = "G class 2 poc mineralization";
            kpoc3.Name = "G class 2 poc mineralization";
            kpop1.Name = "G class 1 pop mineralization";
            kpop2.Name = "G class 2 pop mineralization";
            kpop3.Name = "G class 2 pop mineralization";
            ThtaPON1.Name = "Theta for G class 1 PON";
            ThtaPON2.Name = "Theta for G class 2 PON";
            ThtaPON3.Name = "Theta for G class 3 PON";
            ThtaPOC1.Name = "Theta for G class 1 POC";
            ThtaPOC2.Name = "Theta for G class 2 POC";
            ThtaPOC3.Name = "Theta for G class 3 POC";
            ThtaPOP1.Name = "Theta for G class 1 POP";
            ThtaPOP2.Name = "Theta for G class 2 POP";
            ThtaPOP3.Name = "Theta for G class 3 POP";
            kBEN_STR.Name = "First-order decay rate constant for benthic stress for DiToro eqn 13.3";
            ksi.Name = "First order dissolution rate for particulate biogenic silica (PSi) at 20 degC in layer 2";
            ThtaSi.Name = "Constant for temperature adjustment of KSi";
            KMPSi.Name = "Silica dissolution half-saturation constant for PSi";
            SiSat.Name = "Saturation concentration of silica in pore water";
            KDSi2.Name = "Partition coefficient for Si in Layer 2, controls sorption of dissolved silica to solids";
            DKDSi1.Name = "Factor that enhances sorption of silica in layer 1 when D.O. exceeds DOcSi";
            O2critSi.Name = "Critical dissolved oxygen for silica sorption in layer 1";
            LigninDetr.Name = "Fraction of suspended detritus that is non-reactive";
            Si_Diatom.Name = "Fraction of silica in diatoms (dry)";

            if (DefaultVals) m1.Val = 0.5;
            m1.Units = "kg/L";
            m1.Comment = "";
            m1.Symbol = "m1";

            if (DefaultVals) m2.Val = 0.5;
            m2.Units = "kg/L";
            m2.Comment = "";
            m2.Symbol = "m2";

            if (DefaultVals) H1.Val = 0.001;
            H1.Units = "m";
            H1.Comment = "1 mm default, may be increased to speed computation time";
            H1.Symbol = "H1";

            // {    with Dp do
            // Begin
            // Val :=  0.00012;
            // Units := 'm2/d';  name := 'bioturbation particle mixing diffusion coefficient';
            // Comment := '';
            // Symbol := 'Dp';
            // End;  // unused

            if (DefaultVals) Dd.Val = 0.001;  
            Dd.Units = "m2/d";
            Dd.Comment = "";
            Dd.Symbol = "Dd";

            if (DefaultVals) w2.Val = 0.0003;
            w2.Units = "m/d";
            w2.Comment = "(Q2K uses 0.000005)";
            w2.Symbol = "w2";

            if (DefaultVals) H2.Val = 0.1;
            H2.Units = "m";
            H2.Comment = "";
            H2.Symbol = "H2";

            if (DefaultVals) KappaNH3f.Val = 0.131;
            KappaNH3f.Units = "m/d";
            KappaNH3f.Comment = "(Cerco and Cole suggest value of 0.2 m/d for freshwater)";
            KappaNH3f.Symbol = "KappaNH3f";

            if (DefaultVals) KappaNH3s.Val = 0.131;
            KappaNH3s.Units = "m/d";
            KappaNH3s.Comment = "";
            KappaNH3s.Symbol = "KappaNH3s";

            if (DefaultVals) KappaNO3_1f.Val = 0.1;
            KappaNO3_1f.Units = "m/d";
            KappaNO3_1f.Comment = "(Cerco and Cole suggest value of 0.3 m/d for freshwater)";
            KappaNO3_1f.Symbol = "KappaNO3_1f";

            if (DefaultVals) KappaNO3_1s.Val = 0.1;
            KappaNO3_1s.Units = "m/d";
            KappaNO3_1s.Comment = "";
            KappaNO3_1s.Symbol = "KappaNO3_1s";

            if (DefaultVals) KappaNO3_2.Val = 0.25;
            KappaNO3_2.Units = "m/d";
            KappaNO3_2.Comment = "";
            KappaNO3_2.Symbol = "KappaNO3_2";

            if (DefaultVals) KappaCH4.Val = 0.7;
            KappaCH4.Units = "m/d";
            KappaCH4.Comment = "";
            KappaCH4.Symbol = "KappaCH4";

            if (DefaultVals) KM_NH3.Val = 0.728;
            KM_NH3.Units = "mgN/L";
            KM_NH3.Comment = "";
            KM_NH3.Symbol = "KM_NH3";

            if (DefaultVals) KM_O2_NH3.Val = 0.37;
            KM_O2_NH3.Units = "mgO2/L";
            KM_O2_NH3.Comment = "(DiToro suggests 0.74)";
            KM_O2_NH3.Symbol = "KM_O2_NH3";

            if (DefaultVals) KdNH3.Val = 1.0;
            KdNH3.Units = "L/kg";
            KdNH3.Comment = "";
            KdNH3.Symbol = "KdNH3";

            if (DefaultVals) KdPO42.Val = 100.0;
            KdPO42.Units = "L/kg";
            KdPO42.Comment = "(DiToro 2001 suggests value KdPO42=100 L/Kg)";
            KdPO42.Symbol = "KdPO42";

            if (DefaultVals) dKDPO41f.Val = 20.0;
            dKDPO41f.Units = "unitless";
            dKDPO41f.Comment = "(Cerco and Cole 1995 suggest value dKdPO41f=3000)";
            dKDPO41f.Symbol = "dKDPO41f";

            if (DefaultVals) dKDPO41s.Val = 300.0;
            dKDPO41s.Units = "unitless";
            dKDPO41s.Comment = "(DiToro 2001 suggests value dKdPO41s=300)";
            dKDPO41s.Symbol = "dKDPO41s";

            if (DefaultVals) O2critPO4.Val = 2.0;
            O2critPO4.Units = "mgO2/L";
            O2critPO4.Comment = "";
            O2critPO4.Symbol = "O2critPO4";

            ///Unused_ThtaDp).1.117;
            ///Unused_ThtaDp).name := 'for bioturbation particle mixing between layers 1-2';
            //// Comment := '';
            //// Symbol := 'ThtaDp';

            if (DefaultVals) ThtaDd.Val = 1.08;
            ThtaDd.Units = "";
            ThtaDd.Comment = "";
            ThtaDd.Symbol = "ThtaDd";

            if (DefaultVals) ThtaNH3.Val = 1.123;
            ThtaNH3.Units = "";
            ThtaNH3.Comment = "";
            ThtaNH3.Symbol = "ThtaNH3";

            if (DefaultVals) ThtaNO3.Val = 1.08;
            ThtaNO3.Units = "";
            ThtaNO3.Comment = "";
            ThtaNO3.Symbol = "ThtaNO3";

            if (DefaultVals) ThtaCH4.Val = 1.079;
            ThtaCH4.Units = "";
            ThtaCH4.Comment = "";
            ThtaCH4.Symbol = "ThtaCH4";

            if (DefaultVals) SALTSW.Val = 1.0;
            SALTSW.Units = "ppt";
            SALTSW.Comment = "";
            SALTSW.Symbol = "SALTSW";

            if (DefaultVals) SALTND.Val = 1.0;
            SALTND.Units = "ppt";
            SALTND.Comment = "";
            SALTND.Symbol = "SALTND";

            if (DefaultVals) KappaH2Sd1.Val = 0.2;
            KappaH2Sd1.Units = "m/d";
            KappaH2Sd1.Comment = "";
            KappaH2Sd1.Symbol = "KappaH2Sd1";

            if (DefaultVals) KappaH2Sp1.Val = 0.4;
            KappaH2Sp1.Units = "m/d";
            KappaH2Sp1.Comment = "";
            KappaH2Sp1.Symbol = "KappaH2Sp1";

            if (DefaultVals) ThtaH2S.Val = 1.08;
            ThtaH2S.Units = "unitless";
            ThtaH2S.Comment = "";
            ThtaH2S.Symbol = "ThtaH2S";

            if (DefaultVals) KMHSO2.Val = 4.0;
            KMHSO2.Units = "mgO2/L";
            KMHSO2.Comment = "";
            KMHSO2.Symbol = "KMHSO2";

            if (DefaultVals) KdH2S1.Val = 100.0;
            KdH2S1.Units = "L/kg";
            KdH2S1.Comment = "";
            KdH2S1.Symbol = "KdH2S1";

            if (DefaultVals) KdH2S2.Val = 100.0;
            KdH2S2.Units = "L/kg";
            KdH2S2.Comment = "";
            KdH2S2.Symbol = "KdH2S2";

            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpon1);
            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpon2);
            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpoc1);
            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpoc2);
            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpop1);
            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_frpop2);

            if (DefaultVals) kpon1.Val = 0.035;
            kpon1.Units = "1/d";
            kpon1.Comment = "";
            kpon1.Symbol = "kpon1";

            if (DefaultVals) kpon2.Val = 0.0018;
            kpon2.Units = "1/d";
            kpon2.Comment = "";
            kpon2.Symbol = "kpon2";

            if (DefaultVals) kpon3.Val = 0;
            kpon3.Units = "1/d";
            kpon3.Comment = "";
            kpon3.Symbol = "kpon3";

            if (DefaultVals) kpoc1.Val = 0.035;
            kpoc1.Units = "1/d";
            kpoc1.Comment = "";
            kpoc1.Symbol = "kpoc1";

            if (DefaultVals) kpoc2.Val = 0.0018;
            kpoc2.Units = "1/d";
            kpoc2.Comment = "";
            kpoc2.Symbol = "kpoc2";

            if (DefaultVals) kpoc3.Val = 0.0;
            kpoc3.Units = "1/d";
            kpoc3.Comment = "";
            kpoc3.Symbol = "kpoc3";

            if (DefaultVals) kpop1.Val = 0.035;
            kpop1.Units = "1/d";
            kpop1.Comment = "";
            kpop1.Symbol = "kpop1";

            if (DefaultVals) kpop2.Val = 0.0018;
            kpop2.Units = "1/d";
            kpop2.Comment = "";
            kpop2.Symbol = "kpop2";

            if (DefaultVals) kpop3.Val = 0.0;
            kpop3.Units = "1/d";
            kpop3.Comment = "";
            kpop3.Symbol = "kpop3";

            if (DefaultVals) ThtaPON1.Val = 1.1;
            ThtaPON1.Units = "";
            ThtaPON1.Comment = "";
            ThtaPON1.Symbol = "ThtaPON1";

            if (DefaultVals) ThtaPON2.Val = 1.15;
            ThtaPON2.Units = "";
            ThtaPON2.Comment = "";
            ThtaPON2.Symbol = "ThtaPON2";

            if (DefaultVals) ThtaPON3.Val = 1.17;
            ThtaPON3.Units = "";
            ThtaPON3.Comment = "";
            ThtaPON3.Symbol = "ThtaPON3";

            if (DefaultVals) ThtaPOC1.Val = 1.1;
            ThtaPOC1.Units = "";
            ThtaPOC1.Comment = "";
            ThtaPOC1.Symbol = "ThtaPOC1";

            if (DefaultVals) ThtaPOC2.Val = 1.15;
            ThtaPOC2.Units = "";
            ThtaPOC2.Comment = "";
            ThtaPOC2.Symbol = "ThtaPOC2";

            if (DefaultVals) ThtaPOC3.Val = 1.17;
            ThtaPOC3.Units = "";
            ThtaPOC3.Comment = "";
            ThtaPOC3.Symbol = "ThtaPOC3";

            if (DefaultVals) ThtaPOP1.Val = 1.1;
            ThtaPOP1.Units = "";
            ThtaPOP1.Comment = "";
            ThtaPOP1.Symbol = "ThtaPOP1";

            if (DefaultVals) ThtaPOP2.Val = 1.15;
            ThtaPOP2.Units = "";
            ThtaPOP2.Comment = "";
            ThtaPOP2.Symbol = "ThtaPOP2";

            if (DefaultVals) ThtaPOP3.Val = 1.17;
            ThtaPOP3.Units = "";
            ThtaPOP3.Comment = "";
            ThtaPOP3.Symbol = "ThtaPOP3";

            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_POC1R);
            //// with POC1R do
            //// Begin
            //// Val :=  0.1;
            //// Units := 'gC/m3'; name := 'reference G1 at which w12base = Dp/H2 at 20 degC for DiToro eqn 13.1';
            //// Comment := '';
            //// Symbol := 'POC1R';
            //// End;

            if (DefaultVals) kBEN_STR.Val = 0.03;
            kBEN_STR.Units = "1/day";
            kBEN_STR.Comment = "";
            kBEN_STR.Symbol = "kBEN_STR";

            //SetDefaultDiagenesis_SetUnUsed(ref DR.Unused_KM_O2_Dp);
            //// with KM_O2_Dp do
            //// Begin
            //// Val :=  4.0;
            //// Units := 'mgO2/L';  name := 'particle mixing half-saturation constant for O2';
            //// Comment := '';
            //// Symbol := 'KM_O2_Dp';
            //// End;

            if (DefaultVals) ksi.Val = 0.5;
            ksi.Units = "1/day";
            ksi.Comment = "";
            ksi.Symbol = "Ksi";

            if (DefaultVals) ThtaSi.Val = 1.1;
            ThtaSi.Units = "unitless";
            ThtaSi.Comment = "";
            ThtaSi.Symbol = "Thta_si";

            if (DefaultVals) KMPSi.Val = 50000;
            KMPSi.Units = "g Si/m^3";
            KMPSi.Comment = "";
            KMPSi.Symbol = "KMPSi";

            if (DefaultVals) SiSat.Val = 40;
            SiSat.Units = "g Si/m^3";
            SiSat.Comment = "";
            SiSat.Symbol = "SiSat";

            if (DefaultVals) KDSi2.Val = 100;
            KDSi2.Units = "L/Kg";
            KDSi2.Comment = "";
            KDSi2.Symbol = "KDSi2";

            if (DefaultVals) DKDSi1.Val = 10;
            DKDSi1.Units = "unitless";
            DKDSi1.Comment = "";
            DKDSi1.Symbol = "DKDSi1";

            if (DefaultVals) O2critSi.Val = 1;
            O2critSi.Units = "mg/L";
            O2critSi.Comment = "";
            O2critSi.Symbol = "O2critSi";

            if (DefaultVals) LigninDetr.Val = 0.01;
            LigninDetr.Units = "unitless";
            LigninDetr.Comment = "default";
            LigninDetr.Symbol = "LigninDetr";

            if (DefaultVals) Si_Diatom.Val = 0.425;
            Si_Diatom.Units = "g/g dry";
            Si_Diatom.Comment = "Horne (1994) states that silica makes up 25 to 60% of the dry weight of diatoms.";
            Si_Diatom.Symbol = "Si_Diatom";

        }

    } // end Diagenesis_Rec


    public class TNH4_Sediment : TStateVariable
    {
        // TNH4_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Nitr_Rate(double NH4_1)
        {
            double result;
            // K1 nh4, oxic layer reaction velocity
            double Temp;
            double KappaNH3;
            double O2;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;

            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (AQTSeg.Sulfide_System())
                KappaNH3 = DR.KappaNH3s.Val;
            else
                KappaNH3 = DR.KappaNH3f.Val;

            // EFDC EQ. 5-19
            result = AQMath.Square(KappaNH3 ) * Math.Pow(DR.ThtaNH3.Val, (Temp - 20)) * DR.KM_NH3.Val / (DR.KM_NH3.Val + NH4_1) * O2 / (2 * DR.KM_O2_NH3.Val + O2);
         //(m2/d2)         (m / d)                    (unitless)                        (mg N / L)   (mgN/L)         (mgN/L) (mg/L)              (mg/L)   (mg/L)

            return result;
        }

        // k2 nh4, oxic layer reaction velocity
        public double Flux2Water()
        {
            double result;
            double fda1;
            double s;
            double NH4_0;
            NH4_0 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            fda1 = 1.0 / (1.0 + DR.m1.Val * DR.KdNH3.Val);
            result = s * (fda1 * State - NH4_0) / DR.H1.Val;
            // g/m3 d      m/d   g/m3    g/m3             m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //        // loss from the system
        //        LossInKg = (Burial * SedVol * 1e-3);
        //        // kg N/d   g/m3 d    m3      kg/g
        //        TStates DR = AQTSeg;
        //        MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
        //        DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //        DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //        // Loss from the system
        //        DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //    }
        //}



        // rate in g/m3 (sed) d
        public override void Derivative(ref double DB)
        {
            double fpa1, fda1, fpa2, fda2, s, Nitr=0, Burial=0, Flux2Anaerobic=0, Flux2Wat=0, Dia_Flux=0, NH4_2, NH4_1;
            AllVariables ns;
            TPON_Sediment ppn;

            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    if (Layer == T_SVLayer.SedLayer1)
                    {
                        ClearRate();
                        SaveRate("Nitrification", Nitr);
                        SaveRate("Burial", Burial);
                        SaveRate("Flux2Water", Flux2Wat);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                    else
                    {
                        ClearRate();
                        SaveRate("Dia_Flux", Dia_Flux);
                        SaveRate("Burial", Burial);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                }
            }
                // --------------------------------------------------
                // TNH4_Sediment.Derivative

            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            NH4_1 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            NH4_2 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer2);
            fda1 = 1.0 / (1.0 + DR.m1.Val * DR.KdNH3.Val);
            fpa1 = 1.0 - fda1;
            fda2 = 1.0 / (1.0 + DR.m2.Val * DR.KdNH3.Val);
            fpa2 = 1.0 - fda2;
            Flux2Anaerobic = -(DR.W12 * (fpa2 * NH4_2 - fpa1 * NH4_1) + DR.KL12 * (fda2 * NH4_2 - fda1 * NH4_1));
            // g/m2 d            m/d     g/m3    g/m3    m/d    g/m3       g/m3

            if (Layer == T_SVLayer.SedLayer1)
                 Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            else Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            // g/m3 d               g/m2 d             m

            if (Layer == T_SVLayer.SedLayer1)
            {
                s = AQTSeg.MassTransfer();
                // m/d
                Burial = DR.w2.Val / DR.H1.Val * State;
                        //    m/d         m       g/m3
                Nitr = Nitr_Rate(State) / s * State / DR.H1.Val;    //EFDC eq. 5 - 20
 //             g/m3 d       m2/d2    m/d   g/m3      m
                Flux2Wat = Flux2Water();
                DB = -Nitr - Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d  // g/m3 d

                if (AQTSeg.Diagenesis_Steady_State)  DB = 0.0;
                // Layer 1 is STEADY STATE
            }
            else
            {
                // SedLayer2
                Dia_Flux = 0;
                for (ns = AllVariables.PON_G1; ns <= AllVariables.PON_G3; ns++)
                {
                    ppn = (TPON_Sediment) AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                    Dia_Flux = Dia_Flux + ppn.Mineralization();
                    // mg/L d                    // mg/L d
                }
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * NH4_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * NH4_2 / DR.H2.Val;
                DB = Dia_Flux - Burial + Flux2Anaerobic;
                // g/m3 d                // g/m3 d
            }
            Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TNH4_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TNH4_Sediment

    public class TNO3_Sediment : TStateVariable
    {
        // TNO3_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Denit_Rate()
        {
            double result;
            // eqns 5.23&24
            // m2/d2, L1, m/d L2
            double Temp;
            double KappaNO3;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            if (Layer == T_SVLayer.SedLayer1)
            {
                if (AQTSeg.Sulfide_System())
                {
                    KappaNO3 = DR.KappaNO3_1s.Val;
                }
                else
                {
                    KappaNO3 = DR.KappaNO3_1f.Val;
                }
                result = (AQMath.Square(KappaNO3) * Math.Pow(DR.ThtaNO3.Val, (Temp - 20)));
            }
            else
            {
                // SedLayer2, Anaerobic
                result = (DR.KappaNO3_2.Val) * Math.Pow(DR.ThtaNO3.Val, (Temp - 20));
            }
            return result;
        }

        // k2 nh4, oxic layer reaction velocity
        public double Flux2Water()
        {
            double result;
            double s;
            double NO3_0;
            NO3_0 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = s * (State - NO3_0) / DR.H1.Val;
        // g/m3 d   m/d    g/m3    g/m3             m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double DenitrInKg;
        //    double BuryInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //    }
        //    else
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(1);
        //    }
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        // burial loss from the system
        //        BuryInKg = (Burial * SedVol * 1e-3);
        //    }
        //    else
        //    {
        //        BuryInKg = 0;
        //    }
        //    // burial from Layer 1 goes to Layer 2
        //    if ((Layer == T_SVLayer.SedLayer2) || (!AQTSeg.Diagenesis_Steady_State))
        //    {
        //        // don't track denitrification MB in steady-state layer as irrelevant
        //        // denitrification loss from the system
        //        // kg N / d
        //        // g/m3 d
        //        // m3
        //        // kg/g
        //        DenitrInKg = (DeNitr * SedVol * 1e-3);
        //    }
        //    else
        //    {
        //        DenitrInKg = 0;
        //    }
        //    TStates DR = AQTSeg;
        //    MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
        //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + BuryInKg + DenitrInKg;
        //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + BuryInKg;
        //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + BuryInKg + DenitrInKg;
        //    // Loss from the system
        //    DR.Denitrify[DR.DerivStep] = DR.Denitrify[DR.DerivStep] + DenitrInKg;
        //}

        // --------------------------------------------------

        public override void Derivative(ref double DB)
        {
            double s;
            double Nitr=0;
            double DeNitr = 0;
            double Burial = 0;
            double Flux2Anaerobic = 0;
            double Flux2Wat = 0;
            double NH4_1;
            double NO3_2;
            double NO3_1;
            TNH4_Sediment TNH4_1;
            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    if (Layer == T_SVLayer.SedLayer1)
                    {
                        ClearRate();
                        SaveRate("Nitrification", Nitr);
                        SaveRate("Denitrification", DeNitr);
                        SaveRate("Burial", Burial);
                        SaveRate("Flux2Water", Flux2Wat);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                    else  //SedLayer2
                    {
                        ClearRate();
                        SaveRate("Burial", Burial);
                        SaveRate("Denitrification", DeNitr);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                }
            }
            // --------------------------------------------------

            TNH4_1 = (TNH4_Sediment) AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            NO3_1 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            NH4_1 = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);
            NO3_2 = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -DR.KL12 * (NO3_2 - NO3_1);
            // g/m2 d
            // m/d
            // g/m3
            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }
            // g/m3 d
            // g/m2 d
            // m
            s = AQTSeg.MassTransfer();
            // m/d
            if (Layer == T_SVLayer.SedLayer1)
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
                // m/d       // m                // mg/L
                Nitr = TNH4_1.Nitr_Rate(NH4_1) / s * NH4_1 / DR.H1.Val;  // EFDC eq. 5-20   
                // g/m3 d     // m2/d2    // m/d  // g/m3    // m
                DeNitr = Denit_Rate() / s * State / DR.H1.Val;
                // g/m3 d        // m2/d2 // m/d // g/m3        // m
                Flux2Wat = Flux2Water();
                DB = Nitr - DeNitr - Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    DB = 0;
                }
                // Layer 1 is STEADY STATE
            }
            else
            {
                // SedLayer2
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * NO3_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * NO3_2 / DR.H2.Val;
                // deep burial
                DeNitr = Denit_Rate() * State / DR.H2.Val;
                // g/m3 d     // m/d   // g/m3      // m
                DB = -DeNitr - Burial + Flux2Anaerobic;
                // g/m3 d
            }
            Derivative_WriteRates();
            // Derivative_TrackMB();
        }

        // rate in g/m3 (sed) d
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TNO3_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TNO3_Sediment

    public class TPO4_Sediment : TStateVariable
    {
        // TPO4_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double fdp1()
        {
            double result;
            double dKDPO41;
            double KDPO41;
            double O2;

            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            if (AQTSeg.Sulfide_System())
            {
                dKDPO41 = DR.dKDPO41s.Val;
            }
            else
            {
                dKDPO41 = DR.dKDPO41f.Val;
            }
            if ((O2 > DR.O2critPO4.Val))
            {
                KDPO41 = DR.KdPO42.Val * dKDPO41;
            }
            else
            {
                KDPO41 = DR.KdPO42.Val * Math.Pow(dKDPO41, (O2 / DR.O2critPO4.Val));
            }
            result = (1.0 / (1.0 + DR.m1.Val * KDPO41));
            return result;
        }

        // frac dissolved in layer1
        public double Flux2Water()
        {
            double result;
            double s;
            double PO4_0;
            PO4_0 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            s = AQTSeg.MassTransfer();
            // m/d
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = s * (fdp1() * State - PO4_0) / DR.H1.Val;
            // g/m3 d
            // m/d
            // g/m3
            // g/m3
            // m

            return result;
        }

        // --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double SedVol;
        //    double LossInKg;
        //    if (Layer == T_SVLayer.SedLayer2)
        //    {
        //        SedVol = AQTSeg.DiagenesisVol(2);
        //        LossInKg = (Burial * SedVol * 1e-3);
        //        // loss from the system
        //        // kg N / d
        //        // g/m3 d
        //        // m3
        //        // kg/g
        //        TStates DR = AQTSeg;
        //        MBLossRecord DR = DR.MBLossArray[AllVariables.Phosphate];
        //        DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
        //        DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
        //        // Loss from the system
        //        DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
        //    }
        //}


        public override void Derivative(ref double DB)
        {
            double Dia_Flux=0;
            double Burial = 0;
            double Flux2Anaerobic = 0;
            double Flux2Wat = 0;
            double PO4_2 = 0;
            double PO4_1 = 0;
            double fdp2;
            double fpp1;
            double fpp2;
            AllVariables ns;
            TPOP_Sediment ppp;

            //// --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    if (Layer == T_SVLayer.SedLayer1)
                    {
                        ClearRate();
                        SaveRate("Burial", Burial);
                        SaveRate("Flux2Water", Flux2Wat);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                    else
                    {
                        ClearRate();
                        SaveRate("Dia_Flux", Dia_Flux);
                        SaveRate("Burial", Burial);
                        SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                    }
                }
            }
            // --------------------------------------------------
            // TPO4_Sediment.Deriv
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            fpp1 = 1.0 - fdp1();
            fdp2 = (1.0 / (1.0 + DR.m2.Val * DR.KdPO42.Val));
            fpp2 = 1.0 - fdp2;
            PO4_1 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1);
            PO4_2 = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -((DR.W12 * (fpp2 * PO4_2 - fpp1 * PO4_1) + DR.KL12 * (fdp2 * PO4_2 - fdp1() * PO4_1)));
            // {g/m2 d}           {m/d}      {g/m3}      {g/m3}             {m/d}       {g/m3}         {g/m3}

            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }     // g/m3 d        // g/m2 d          // m

            if (Layer == T_SVLayer.SedLayer1)
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
                         // m/d        // m      // g/m3
                Flux2Wat = Flux2Water();
                // g/ m3 d
                DB = -Burial - Flux2Wat - Flux2Anaerobic;
                // g/m3 d
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    DB = 0;  // Layer 1 is STEADY STATE
                }
            }
            else
            {
                // SedLayer2
                Flux2Anaerobic = Flux2Anaerobic + (DR.w2.Val * PO4_1 / DR.H2.Val);
                // include burial from L1
                Burial = DR.w2.Val * PO4_2 / DR.H2.Val;
                Dia_Flux = 0;
                for (ns = AllVariables.POP_G1; ns <= AllVariables.POP_G3; ns++)
                {
                    ppp = (TPOP_Sediment) AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                    Dia_Flux = Dia_Flux + ppp.Mineralization();
                }
                // g/ m3 d
                DB = Dia_Flux - Burial + Flux2Anaerobic;
                // g/m3 d
            }
            Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        // rate in g/m3 (sed) d
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPO4_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPO4_Sediment

    public class TPOC_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled

        // TPOC_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            // gC/m3 day
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.POC_G1 => DR.kpoc1.Val * Math.Pow(DR.ThtaPOC1.Val, Temp - 20) * State,
                AllVariables.POC_G2 => DR.kpoc2.Val * Math.Pow(DR.ThtaPOC2.Val, Temp - 20) * State,
                _ => DR.kpoc3.Val * Math.Pow(DR.ThtaPOC3.Val, Temp - 20) * State,
                // g C/m3 d =   // 1/d            // unitless          // g C/m3
            };
            return result;
        }

        public double Burial()
        {
            double result;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            result = DR.w2.Val / DR.H2.Val * State;
                      // m/d        // m    // mg/L
            return result;
        }

        public double Predn()
        {
            if (NState == AllVariables.POC_G1)
                return Predation() / Consts.Detr_OM_2_OC;
            if (NState == AllVariables.POC_G2)
                return Predation() / Consts.Detr_OM_2_OC;
            // g OC/m3 w  // g OM /m3    // g OM / g OC

            return 0;
        }

        public override void Derivative(ref double DB)
        {
            double Minrl=0;
            double Deposition = 0;
            double Bury = 0;
            double Pred = 0;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;

            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Deposition", Deposition);
                    SaveRate("Mineralization", Minrl);
                    SaveRate("Burial", Bury);
                    SaveRate("Predation", Pred);
                }
            }

            Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg C/L            // g/m2                                  // m
            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

            //          AQTSeg.Diag_Track[TAddtlOutput.POC_Dep, DR.DerivStep] = DR.Diag_Track[TAddtlOutput.POC_Dep, DR.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d                                                // g/m3 sed             // m sed             // mg/g   
            Minrl = Mineralization();
            Bury = Burial();
            Pred = Predn();
            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
            // ( g/m3 s) = ( g/m3 w) * ( m3 w ) / ( m3 s)

            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            DB = Deposition - Minrl - Burial() - Pred;
            Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPOC_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPOC_Sediment

    public class TPON_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled


        // TPON_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.PON_G1 => DR.kpon1.Val * Math.Pow(DR.ThtaPON1.Val, Temp - 20) * State,
                AllVariables.PON_G2 => DR.kpon2.Val * Math.Pow(DR.ThtaPON2.Val, Temp - 20) * State,
                _ => DR.kpon3.Val * Math.Pow(DR.ThtaPON3.Val, Temp - 20) * State,
            };
            // g N/m3 d  =   // 1/d             // unitless           // g N/m3

            return result;
        }

            // --------------------------------------------------
            //public void Derivative_TrackMB()
            //{
            //    double SedVol;
            //    double LossInKg;
            //    SedVol = AQTSeg.DiagenesisVol(2);
            //    LossInKg = (Burial * SedVol * 1e-3);
            //    // burial loss from the system
            //    // kg N / d            // g/m3 d            // m3            // kg/g
            //    TStates DR = AQTSeg;
            //    MBLossRecord DR = DR.MBLossArray[AllVariables.Nitrate];
            //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
            //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
            //    // Loss from the system
            //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
            //}



            public override void Derivative(ref double DB)
        {
            double Minerl=0;
            double Deposition = 0;
            double Burial = 0;
            double Pred = 0;

            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Deposition", Deposition);
                    SaveRate("Mineralization", Minerl);
                    SaveRate("Burial", Burial);
                    SaveRate("Predation", Pred);
                }
            }
            // --------------------------------------------------
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            Minerl = Mineralization();
            Deposition =AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg/L            // g/m2             // m

            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

            // Diag_Track[TAddtlOutput.PON_Dep, DR.DerivStep] = Diag_Track[TAddtlOutput.PON_Dep, DR.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d             // g/m3 sed            // m sed            // mg/g
            Burial = DR.w2.Val / DR.H2.Val * State;
            // m/d         // m   // mg/L

            Pred = 0;
            if (NState == AllVariables.PON_G1)   
                Pred = Predation() * Location.Remin.N2OrgLab;

            if (NState == AllVariables.PON_G2)
                Pred = Predation() * Location.Remin.N2Org_Refr;
             // g N/m3 w     // g OM /m3             // g N / g OM

            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
      // g/m3*sed  g/m3 w    // m3 water         // m3 sed

            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            DB = Deposition - Minerl - Burial - Pred;

          Derivative_WriteRates();
         // Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPON_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPON_Sediment

    public class TPOP_Sediment : TStateVariable
    {
        public Loadings.TLoadings Deposition_Link = null;  // additive linkage from JSON if plants, animals, or OM not modeled
        public Loadings.TLoadings Predation_Link = null;   // linkage from JSON if detritivores not explicitly modeled

        // TPOP_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Mineralization()
        {
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            var result = NState switch
            {
                AllVariables.POP_G1 => DR.kpop1.Val * Math.Pow(DR.ThtaPOP1.Val, Temp - 20) * State,
                AllVariables.POP_G2 => DR.kpop2.Val * Math.Pow(DR.ThtaPOP2.Val, Temp - 20) * State,
                _ => DR.kpop3.Val * Math.Pow(DR.ThtaPOP3.Val, Temp - 20) * State,
                // g P/m3 d   // 1/d           // unitless                 // g C/m3
            };
            return result;
        }

            // --------------------------------------------------
            //public void Derivative_TrackMB()
            //{
            //    double SedVol;
            //    double LossInKg;
            //    SedVol = AQTSeg.DiagenesisVol(2);
            //    LossInKg = (Burial * SedVol * 1e-3);
            //    // burial loss from the system
            //    // kg N / d
            //    // g/m3 d
            //    // m3
            //    // kg/g
            //    TStates DR = AQTSeg;
            //    MBLossRecord DR = DR.MBLossArray[AllVariables.Phosphate];
            //    DR.TotalNLoss[DR.DerivStep] = DR.TotalNLoss[DR.DerivStep] + LossInKg;
            //    DR.Burial[DR.DerivStep] = DR.Burial[DR.DerivStep] + LossInKg;
            //    DR.BoundLoss[DR.DerivStep] = DR.BoundLoss[DR.DerivStep] + LossInKg;
            //    // Loss from the system

            //}


        public override void Derivative(ref double DB)
        {
            double Deposition=0;
            double Burial=0;
            double Minerl = 0;
            double Pred = 0;

            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Deposition", Deposition);
                    SaveRate("Mineralization", Minerl);
                    SaveRate("Burial", Burial);
                    SaveRate("Predation", Pred);
                }
            }
            // --------------------------------------------------

            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            Minerl = Mineralization();
            Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
            // mg/L d            // g/m2 d

            if (Deposition_Link != null) Deposition = Deposition + Deposition_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                    (g/m3 d sediment) = (g/m3 d sediment) + (g/d) / (m3 sediment)

            //     AQTSeg.Diag_Track[TAddtlOutput.POP_Dep, AQTSeg.DerivStep] = AQTSeg.Diag_Track[TAddtlOutput.POP_Dep, AQTSeg.DerivStep] + Deposition * DR.H2.Val * 1e3;
            // mg/m2 d            // g/m3 sed            // m sed            // mg/g
            Burial = DR.w2.Val / DR.H2.Val * State;
            // g/m3    // m/d      // m       // g/m3

            Pred = 0;
            if (Predation_Link != null) Pred = Predation_Link.ReturnLoad(AQTSeg.TPresent) / AQTSeg.DiagenesisVol(2);
            //                          (g/m3 d sediment) = (g/d) / (m3 sediment)

            if (NState == AllVariables.POP_G1)   
                Pred = Predation() * Location.Remin.P2OrgLab;

            if (NState == AllVariables.POP_G2)
                Pred = Predation() * Location.Remin.P2Org_Refr;
            //g P/m3 w  // g P /m3             // g P / g OM

            MorphRecord MR = AQTSeg.Location.Morph;
            Pred = Pred * MR.SegVolum / AQTSeg.DiagenesisVol(2);
        // g/m3 s // g/m3 w  // m3 w                // m3 s

            DB = Deposition - Minerl - Burial - Pred;
            // g/m3 d

            Derivative_WriteRates();
            //Derivative_TrackMB();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TPOP_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TPOP_Sediment

    public class TMethane : TStateVariable
    {
        // TMethane
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

            public override void Derivative(ref double DB)
        {
            double SECH_ARG;
            double Dia_Flux=0;
            double Flux2Wat = 0;
            double Oxid = 0;
            TNO3_Sediment PNO3_1;
            TNO3_Sediment PNO3_2;
            double CSODmax;
            double Temp;
            double CSOD;
            double CH4Sat;
            double S;
            double JO2NO3;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;     // ppt

            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Dia_Flux", Dia_Flux);
                    SaveRate("Flux2Water", Flux2Wat);
                    SaveRate("Oxidation", Oxid);
                }
            }


            if (AQTSeg.Sulfide_System())
            {
                // Goes to Sulfide Instead
                Dia_Flux = 0;
                Flux2Wat = 0;
                Oxid = 0;
                DB = 0;
                Derivative_WriteRates();
                return;
            }
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            // CH4_0  := GetState(Methane,StV,WaterCol);
            Dia_Flux = AQTSeg.Diagenesis(Layer) * DR.H2.Val;
         // g O2/m2 d            // g O2/m3 d         // m

            S = AQTSeg.MassTransfer();
            // m/d
            PNO3_1 = (TNO3_Sediment) AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            PNO3_2 = (TNO3_Sediment) AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
            // g/m2 d
            JO2NO3 = 2.86 * (PNO3_1.Denit_Rate() / S * PNO3_1.State + PNO3_2.Denit_Rate() * PNO3_2.State);
                     // m/d            // g/m3
            Dia_Flux = Dia_Flux - JO2NO3;

            if (Dia_Flux < 0)
                Dia_Flux = 0;

            CH4Sat = 100.0 * (1.0 + AQTSeg.DynamicZMean() / 10) * Math.Pow(1.024, (20 - Temp));
            // saturation conc of methane in pore water {g 02/m3
            CSODmax = Math.Min(Math.Sqrt(2.0 * DR.KL12 * CH4Sat * Dia_Flux), Dia_Flux);
            SECH_ARG = (DR.KappaCH4.Val * Math.Pow(DR.ThtaCH4.Val, (Temp - 20.0))) / S;
            // CSOD Equation 10.35 from DiTorro
            // The hyperbolic secant is defined as HSec(X) = 2.0 / (Exp(X) + Exp(-X))
            if ((SECH_ARG < 400))
            {
                CSOD = CSODmax * (1.0 - (2.0 / (Math.Exp(SECH_ARG) + Math.Exp(-SECH_ARG))));
            }
            else
            {
                CSOD = CSODmax;
            }
            // HSec(SECH_ARG) < 3.8E-174 ~ 0
            Flux2Wat = Dia_Flux - CSOD;
            // (CSODmax - CSOD);
            // oxidation
            Oxid = CSOD;
            DB = (Dia_Flux - Flux2Wat - Oxid) / DR.H2.Val;
       // g O2eq / m3 d    // g O2eq / m2 d            // m
            Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TMethane(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TMethane

    public class TSulfide_Sediment : TStateVariable
    {

        public double k2Oxid()
        {
            double result;
            // reaxn vel for sulfide oxidation
            double Temp;
            double O2;
            double fdh2s1;
            double fph2s1;
            result = 0;
            if (Layer == T_SVLayer.SedLayer2)
            {
                return result;
            }
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            fdh2s1 = 1.0 / (1.0 + DR.m1.Val * DR.KdH2S1.Val);
            fph2s1 = 1.0 - fdh2s1;
            result = (Math.Pow(DR.KappaH2Sd1.Val, 2) * fdh2s1 + (Math.Pow(DR.KappaH2Sp1.Val, 2) * fph2s1)) * Math.Pow(DR.ThtaH2S.Val, Temp - 20) * O2 / (2 * DR.KMHSO2.Val);
            // g2/m2

            return result;
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

            public override void Derivative(ref double DB)
        {
            double Burial=0;
            double Oxid=0;
            double Dia_Flux=0;
            double Flux2Wat = 0;
            double s = 0;
            double COD_0 = 0;
            double Flux2Anaerobic=0;
            double H2S_2;
            double H2S_1;
            double fph2s1;
            double fdh2s1;
            double fph2s2;
            double fdh2s2;
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params; // ppt

            //-----------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    if (Layer == T_SVLayer.SedLayer1)
                    {
                        SaveRate("Oxidation", Oxid);
                        SaveRate("Flux2Water", Flux2Wat);
                    }
                    else
                    {
                        SaveRate("Dia_Flux", Dia_Flux);
                    }

                    SaveRate("Burial", Burial);
                    SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                }
            }
            //-----------------------------------------------

            if (!AQTSeg.Sulfide_System())
            {
                // Goes to Methane Instead
                DB = 0;
                Derivative_WriteRates();
                return;
            }
            Dia_Flux = AQTSeg.Diagenesis(Layer);
            s = AQTSeg.MassTransfer();
            // m/d
            if (Layer == T_SVLayer.SedLayer1)
            {
                // m2/d2
                // m/d
                // g/m3
                // m
                Oxid = k2Oxid() / s * State / DR.H1.Val;
            }
            else
            {
                Oxid = 0;
            }
            fdh2s1 = 1.0 / (1.0 + DR.m1.Val * DR.KdH2S1.Val);
            fph2s1 = 1.0 - fdh2s1;
            fdh2s2 = 1.0 / (1.0 + DR.m2.Val * DR.KdH2S2.Val);
            fph2s2 = 1.0 - fdh2s2;
            COD_0 = AQTSeg.GetState(AllVariables.COD, T_SVType.StV, T_SVLayer.WaterCol);
            H2S_1 = AQTSeg.GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer1);
            H2S_2 = AQTSeg.GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer2);
            Flux2Anaerobic = -((DR.W12 * (fph2s2 * H2S_2 - fph2s1 * H2S_1) + DR.KL12 * (fdh2s2 * H2S_2 - fdh2s1 * H2S_1)));
            if (Layer == T_SVLayer.SedLayer1)
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
            }
            else
            {
                Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
            }      // g/m3 d          // g/m2 d      // m

            if (Layer == T_SVLayer.SedLayer2)
            {
                Flux2Anaerobic = Flux2Anaerobic + DR.w2.Val / DR.H2.Val * H2S_1;
            }
            // burial from L1
            if (Layer == T_SVLayer.SedLayer2)
            {
                Flux2Wat = 0;
            }
            else
            {
                Flux2Wat = s * (fdh2s1 * State - COD_0) / DR.H1.Val;
             // (mg/L d) (m/d)          (mg/L)   (mg/L)         (m)

            }
            if (Layer == T_SVLayer.SedLayer2)
            {
                Burial = DR.w2.Val / DR.H2.Val * State;
            }
            else
            {
                Burial = DR.w2.Val / DR.H1.Val * State;
             // (g/m3 d)  (m/d)       (m)        (g/m3)

            }
            if (Layer == T_SVLayer.SedLayer1)
            {
                if (AQTSeg.Diagenesis_Steady_State)
                {
                    // Layer 1 is STEADY STATE
                    DB = 0;
                }
                else
                {
                    DB = -Oxid - Burial - Flux2Wat - Flux2Anaerobic;
                }
            }  // SedLayer1
            else
            {
                DB = Dia_Flux - Burial + Flux2Anaerobic;  //SedLayer2
            }
            Derivative_WriteRates();
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TSulfide_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSulfide_Sediment

    public class TSilica_Sediment : TStateVariable
    {
        // -------------------------------------------------------------------------------
        // TSilica_Sediment
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
        }

        public double Dissolution(double fdsi2)
        {
            double result;
            double PSi;
            double Si2;
            double Temp;
            result = 0;
            if (Layer == T_SVLayer.SedLayer1)
            {
                return result;
            }
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            PSi = AQTSeg.GetState(AllVariables.Avail_Silica, T_SVType.StV, T_SVLayer.SedLayer2);
            // biogenic silica
            Si2 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer2);
            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;
            // g/m3 d
            // 1/d
            result = DR.ksi.Val * Math.Pow(DR.ThtaSi.Val, (Temp - 20)) * (PSi / (PSi + DR.KMPSi.Val)) * (DR.SiSat.Val - fdsi2 * Si2);
            // g/m3     // g/m3

            return result;
        }

        public override void Derivative(ref double DB)
        {
            double s=0;
            double O2 = 0;
            double Diss = 0;
            double Flux2Anaerobic = 0;
            double Flux2Wat = 0;
            double Si_2 = 0, Si_1 = 0, Si_0 = 0;
            double KdSi1, fdsi1, fdsi2, fpsi1, fpsi2;
            double Deposition = 0;
            double Burial = 0;

            Diagenesis_Rec DR = AQTSeg.Diagenesis_Params;

            // ----------------------------------------------------------------------
            void Derivative_WriteAvailRates()
            {
                // biogenic silica, L2
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Deposition", Deposition);
                    SaveRate("Dissolution", Diss);
                    SaveRate("Burial", Burial);
                }
            }

                // ----------------------------------------------------------------------
                void Derivative_WriteSilicaRates()
                {
                    // non-biogenic
                    if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                    {
                        if (Layer == T_SVLayer.SedLayer1)
                        {
                            ClearRate();
                            SaveRate("Burial", Burial);
                            SaveRate("Flux2Water", Flux2Wat);
                            SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                        }
                        else
                        {
                            ClearRate();
                            SaveRate("Dissolution", Diss);
                            SaveRate("Burial", Burial);
                            SaveRate("Flux2Anaerobic", Flux2Anaerobic);
                        }
                    }
                }
                // ----------------------------------------------------------------------
            // TSilica_Sediment.Derivative
            O2 = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((O2 > DR.O2critSi.Val))
            {
                KdSi1 = DR.KDSi2.Val * DR.DKDSi1.Val;
            }
            else
            {
                KdSi1 = DR.KDSi2.Val * Math.Pow(DR.DKDSi1.Val, (O2 / DR.O2critSi.Val));
            }
            fdsi1 = (1.0 / (1.0 + DR.m1.Val * KdSi1));
            fpsi1 = 1.0 - fdsi1;
            fdsi2 = (1.0 / (1.0 + DR.m2.Val * DR.KDSi2.Val));
            fpsi2 = 1.0 - fdsi2;
            if (NState == AllVariables.Avail_Silica)
            {
                // Particulate Biogenic Silica
                Diss = Dissolution(fdsi2);
                // mg/L d
                Deposition = AQTSeg.CalcDeposition(NState, T_SVType.StV) / DR.H2.Val;
                // mg/L d                // g/m2 d                // m
                Burial = DR.w2.Val / DR.H2.Val * State;
                // m/d                // m                // g/m3
                DB = Deposition - Diss - Burial;
                // g/m3 d
                Derivative_WriteAvailRates();
            }
            if (NState == AllVariables.Silica)
            {
                // Silica
                Si_0 = 0; // AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.WaterCol);  assumes silica in water column is zero
                Si_1 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer1);
                Si_2 = AQTSeg.GetState(AllVariables.Silica, T_SVType.StV, T_SVLayer.SedLayer2);
                Flux2Anaerobic = -((DR.W12 * (fpsi2 * Si_2 - fpsi1 * Si_1) + DR.KL12 * (fdsi2 * Si_2 - fdsi1 * Si_1)));
                // g/m2 d                 // m/d                // g/m3                // m/d                // g/m3
                if (Layer == T_SVLayer.SedLayer1)
                {
                    Flux2Anaerobic = Flux2Anaerobic / DR.H1.Val;
                }
                else
                {
                    Flux2Anaerobic = Flux2Anaerobic / DR.H2.Val;
                    // g/m3 d            // g/m2 d            // m
                }

                s = AQTSeg.MassTransfer();
                // m/d
                if (Layer == T_SVLayer.SedLayer1)
                {
                    Burial = DR.w2.Val / DR.H1.Val * State;
                    // m/d                    // m           // mg/L
                    Flux2Wat = s * (fdsi1 * State - Si_0) / DR.H1.Val;
                    // m/d   // m/d    // mg/L   // mg/L           // m
                    DB = -Burial - Flux2Wat - Flux2Anaerobic;
                    if (AQTSeg.Diagenesis_Steady_State)
                    {
                        DB = 0;
                    }
                    // Layer 1 is STEADY STATE
                }
                else
                {
                    // SedLayer2
                    Flux2Anaerobic = Flux2Anaerobic + DR.w2.Val * Si_1 / DR.H2.Val;
                    // burial from L1
                    Burial = DR.w2.Val * Si_2 / DR.H2.Val;
                    Diss = Dissolution(fdsi2);
                    DB = Diss - Burial + Flux2Anaerobic;
                }
              Derivative_WriteSilicaRates();
            }
        }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TSilica_Sediment(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSilica_Sediment

    public class TCOD : TStateVariable
    {
        // -------------------------------------------------------------------------------
        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC,  IsTempl)
        public TCOD(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
        public override void Derivative(ref double db)
        {
            db = 0;
            //Setup_Record DR = AQTSeg.SetupRec;
            //if ((DR.SaveBRates || DR.ShowIntegration))
            //{
            //    ClearRate();
            //}
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            base.CalculateLoad(TimeIndex);
            // TStateVariable
            State = Loading;
            // valuation not loading, no need to adjust for flow and volume
        }

    } // end TCOD

}