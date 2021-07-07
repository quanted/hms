using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.OrgMatter;
using AQUATOX.Diagenesis;
using Newtonsoft.Json;
using Globals;
using AQUATOX.Organisms;
using AQUATOX.Chemicals;

namespace AQUATOX.Plants

{
    public class PlantRecord
    {
        public TStringParam PlantName = new TStringParam();
        public TStringParam ScientificName = new TStringParam();
        public TDropDownParam PlantType = new TDropDownParam();
        public TDropDownParam ToxicityRecord = new TDropDownParam();
        public TBoolParam SurfaceFloating = new TBoolParam();
        public TDropDownParam Macrophyte_Type = new TDropDownParam();
        public TDropDownParam Taxonomic_Type = new TDropDownParam();
        public TParameter EnteredLightSat = new TParameter();
        public TBoolParam UseAdaptiveLight = new TBoolParam();
        public TParameter MaxLightSat = new TParameter();
        public TParameter MinLightSat = new TParameter();
        public TParameter KPO4 = new TParameter();
        public TParameter KN = new TParameter();
        public TParameter KCarbon = new TParameter();
        public TParameter Q10 = new TParameter();
        public TParameter TOpt = new TParameter();
        public TParameter TMax = new TParameter();
        public TParameter TRef = new TParameter();
        public TParameter PMax = new TParameter();
        public TParameter KResp = new TParameter();
        public TParameter Resp20 = new TParameter();
        public TParameter KMort = new TParameter();
        public TParameter EMort = new TParameter();
        public TParameter P2OrgInit = new TParameter();
        public TParameter N2OrgInit = new TParameter();
        public TParameter ECoeffPhyto = new TParameter();
        public TParameter Wet2Dry = new TParameter();
        public TParameter PlantFracLipid = new TParameter();
        public TParameter NHalfSatInternal = new TParameter();
        public TParameter PHalfSatInternal = new TParameter();
        public TParameter MaxNUptake = new TParameter();
        public TParameter MaxPUptake = new TParameter();
        public TParameter Min_N_Ratio = new TParameter();
        public TParameter Min_P_Ratio = new TParameter();
        public TParameter Plant_to_Chla = new TParameter();
        public TParameter KSed = new TParameter();
        public TParameter KSedTemp = new TParameter();
        public TParameter KSedSalinity = new TParameter();
        public TParameter ESed = new TParameter();
        public TParameter CarryCapac = new TParameter();
        public TParameter Macro_VelMax = new TParameter();
        public TParameter Red_Still_Water = new TParameter();
        public TParameter FCrit = new TParameter();
        public TParameter PctSloughed = new TParameter();
        public TParameter PrefRiffle = new TParameter();
        public TParameter PrefPool = new TParameter();
        public TParameter SalMin_Phot = new TParameter();
        public TParameter SalMax_Phot = new TParameter();
        public TParameter SalCoeff1_Phot = new TParameter();
        public TParameter SalCoeff2_Phot = new TParameter();

        public TParameter SalMin_Mort = new TParameter();
        public TParameter SalMax_Mort = new TParameter();
        public TParameter SalCoeff1_Mort = new TParameter();
        public TParameter SalCoeff2_Mort = new TParameter();

        public void Setup()
        {
            PlantName.Name = "Plant Name";
            ScientificName.Name = "Scientific Name";
            PlantType.Name = "Plant Type"; PlantType.ValList = new string[] { "Phytoplankton", "Periphyton", "Macrophytes", "Bryophytes" };
            SurfaceFloating.Name = "Plant is Surface Floating"; SurfaceFloating.Name = "Is this plant surface floating?";
            Macrophyte_Type.Name = "Macrophyte Type"; Macrophyte_Type.ValList = new string[] { "Benthic", "Rooted floating", "Free-floating" };
            Taxonomic_Type.Name = "Taxonomic Group"; Taxonomic_Type.ValList = new string[] { "Diatoms", "Greens", "BlueGreens", "Other Algae", "Macrophyte" };
            ToxicityRecord.Name = "Toxicity Record"; ToxicityRecord.ValList = new string[] { "Diatoms", "Greens", "BlueGreens", "Other Algae", "Macrophyte" };
            EnteredLightSat.Symbol = "Saturating Light"; EnteredLightSat.Name = "Light saturation level for photosynthesis"; EnteredLightSat.Units = "ly/d";
            UseAdaptiveLight.Symbol = "Use Adaptive Light"; UseAdaptiveLight.Name = "Use adaptive light construct?"; UseAdaptiveLight.Units = "Boolean";
            MaxLightSat.Symbol = "Max. Saturating Light"; MaxLightSat.Name = "Maximum light saturation allowed from adaptive light equation"; MaxLightSat.Units = "ly/d";
            MinLightSat.Symbol = "Min. Saturating Light"; MinLightSat.Name = "Minimum light saturation allowed from adaptive light equation"; MinLightSat.Units = "ly/d";
            KPO4.Symbol = "P Half-saturation"; KPO4.Name = "Half-saturation constant for phosphorus"; KPO4.Units = "gP/m3";
            KN.Symbol = "N Half-saturation"; KN.Name = "Half-saturation constant for nitrogen"; KN.Units = "gN/m3";
            KCarbon.Symbol = "Inorg C Half-saturation"; KCarbon.Name = "Half-saturation constant for carbon"; KCarbon.Units = "gC/m3";
            Q10.Symbol = "Temp Response Slope"; Q10.Name = "Slope or rate of change per 10°C temperature change"; Q10.Units = "Unitless";
            TOpt.Symbol = "Optimum Temperature"; TOpt.Name = "Optimum temperature"; TOpt.Units = "°C";
            TMax.Symbol = "Maximum Temperature"; TMax.Name = "Maximum temperature tolerated"; TMax.Units = "°C";
            TRef.Symbol = "Min. Adaptation Temp"; TRef.Name = "Adaptation temperature below which there is no acclimation"; TRef.Units = "°C";
            PMax.Symbol = "Max. Photosynthesis Rate"; PMax.Name = "Maximum photosynthetic rate"; PMax.Units = "1/d";
            KResp.Symbol = "Photorespiration Coefficient"; KResp.Name = "Coefficient of proportionality between.  Excretion and photosynthesis at optimal light levels"; KResp.Units = "Unitless";
            Resp20.Symbol = "Resp Rate at 20 deg. C"; Resp20.Name = @"Respiration rate at 20°C "; Resp20.Units = "g/g∙d";
            KMort.Symbol = "Mortality Coefficient"; KMort.Name = "Intrinsic mortality rate"; KMort.Units = "g/g∙d";
            EMort.Symbol = "Exponential Mort Coeff"; EMort.Name = "Exponential factor for suboptimal conditions"; EMort.Units = "g/g∙d";
            P2OrgInit.Symbol = "P to Photosynthate"; P2OrgInit.Name = "Initial ratio of phosphate to organic matter for given species"; P2OrgInit.Units = "fraction dry weight";
            N2OrgInit.Symbol = "N to Photosynthate"; N2OrgInit.Name = "Initial ratio of nitrate to organic matter for given species"; N2OrgInit.Units = "fraction dry weight";
            ECoeffPhyto.Symbol = "Light Extinction"; ECoeffPhyto.Name = "Attenuation coefficient for given alga"; ECoeffPhyto.Units = "1/m-g/m3w";
            Wet2Dry.Symbol = "Wet to Dry"; Wet2Dry.Name = "Ratio of wet weight to dry weight for given species"; Wet2Dry.Units = "Ratio";
            PlantFracLipid.Symbol = "Fraction that is lipid"; PlantFracLipid.Name = "Fraction of lipid in organism"; PlantFracLipid.Units = "g lipid/g org. Wet";
            NHalfSatInternal.Symbol = "N Half-saturation Internal"; NHalfSatInternal.Name = "half-saturation constant for intracellular nitrogen"; NHalfSatInternal.Units = "gN / gAFDW";
            PHalfSatInternal.Symbol = "P Half-saturation Internal"; PHalfSatInternal.Name = "half-saturation constant for intracellular phosphorus"; PHalfSatInternal.Units = "gP / gAFDW";
            MaxNUptake.Symbol = "N Max Uptake Rate"; MaxNUptake.Name = "the maximum uptake rate for nitrogen"; MaxNUptake.Units = "gN / gAFDW∙d";
            MaxPUptake.Symbol = "P Max Uptake Rate"; MaxPUptake.Name = "the maximum uptake rate for phosphorus"; MaxPUptake.Units = "gP / gAFDW∙d";
            Min_N_Ratio.Symbol = "Min N Ratio"; Min_N_Ratio.Name = "the ratio of intracellular nitrogen at which growth ceases  "; Min_N_Ratio.Units = "gN / gAFDW";
            Min_P_Ratio.Symbol = "Min P Ratio"; Min_P_Ratio.Name = "the ratio of intracellular phosphorus at which growth ceases  "; Min_P_Ratio.Units = "gP / gAFDW";
            Plant_to_Chla.Symbol = "Phytoplankton: C:Chlorophyll a"; Plant_to_Chla.Name = "ratio of carbon to chlorophyll a"; Plant_to_Chla.Units = "g carbon/g chl. a";
            KSed.Symbol = "Phytoplankton: Sedimentation Rate (KSed)"; KSed.Name = "Intrinsic settling rate"; KSed.Units = "m/d";
            KSedTemp.Symbol = "Phytoplankton: Temperature of Obs. KSed"; KSedTemp.Name = "Reference temperature of water for calculating Nhytoplankton sinking rate"; KSedTemp.Units = "deg. C";
            KSedSalinity.Symbol = "Phytoplankton: Salinity of Obs. KSed"; KSedSalinity.Name = "Reference salinity of water for calculating Nhytoplankton sinking rate"; KSedSalinity.Units = "‰";
            ESed.Symbol = "Phytoplankton: Exp. Sedimentation Coeff"; ESed.Name = "Exponential settling coefficient"; ESed.Units = "Unitless";
            CarryCapac.Symbol = "Macrophytes: Carrying Capacity"; CarryCapac.Name = "Macrophyte carrying capacity, converted to g/m3 and used to calculate washout of free-floating macrophytes "; CarryCapac.Units = "g/m2";
            Macro_VelMax.Symbol = "Macrophytes: VelMax "; Macro_VelMax.Name = "Velocity at which total breakage occurs"; Macro_VelMax.Units = "cm/s ";
            Red_Still_Water.Symbol = "Periphyton: Reduction in Still Water"; Red_Still_Water.Name = "Reduction in photosynthesis in absence of current"; Red_Still_Water.Units = "Unitless";
            FCrit.Symbol = "Periphyton: Critical Force (FCrit)"; FCrit.Name = "Critical force necessary to dislodge given periphyton group"; FCrit.Units = "newtons (kg m/s2)";
            PctSloughed.Symbol = "Percent Lost in Slough Event"; PctSloughed.Name = "Fraction of biomass lost at one time"; PctSloughed.Units = "%";
            PrefRiffle.Symbol = "Percent in Riffle"; PrefRiffle.Name = "Percentage of biomass of plant that is in riffle, as opposed to run or pool"; PrefRiffle.Units = "%";
            PrefPool.Symbol = "Percent in Pool"; PrefPool.Name = "Percentage of biomass of plant that is in pool, as opposed to run or riffle"; PrefPool.Units = "%";
            SalMin_Phot.Symbol = "SalMin Photo."; SalMin_Phot.Name = "Minimum Salinity for Photosynthesis"; SalMin_Phot.Units = "‰";
            SalMax_Phot.Symbol = "SalMax Photo."; SalMax_Phot.Name = "Maximum Salinity for Photosynthesis"; SalMax_Phot.Units = "‰";
            SalCoeff1_Phot.Symbol = "SalCoeff1 Photo."; SalCoeff1_Phot.Name = "Salinity Coefficient 1 for Photosynthesis"; SalCoeff1_Phot.Units = "unitless";
            SalCoeff2_Phot.Symbol = "SalCoeff2 Photo."; SalCoeff2_Phot.Name = "Salinity Coefficient 2 for Photosynthesis"; SalCoeff2_Phot.Units = "unitless";
            SalMin_Mort.Symbol = "SalMin Mort."; SalMin_Mort.Name = "Minimum Salinity for Mortality"; SalMin_Mort.Units = "‰";
            SalMax_Mort.Symbol = "SalMax Mort."; SalMax_Mort.Name = "Maximum Salinity for Mortality"; SalMax_Mort.Units = "‰";
            SalCoeff1_Mort.Symbol = "SalCoeff1 Mort."; SalCoeff1_Mort.Name = "Salinity Coefficient 1 for Mortality"; SalCoeff1_Mort.Units = "unitless";
            SalCoeff2_Mort.Symbol = "SalCoeff2 Mort."; SalCoeff2_Mort.Name = "Salinity Coefficient 2 for Mortality"; SalCoeff2_Mort.Units = "unitless";
        }

        public TParameter[] InputArray()
        {
            return new TParameter[] {new TSubheading("Plant Parameters for "+PlantName.Val,""), PlantName,
                ScientificName, PlantType,SurfaceFloating,
                Taxonomic_Type, ToxicityRecord, EnteredLightSat, PMax, KPO4, KN,TOpt, KMort,
                new TSubheading("Adaptive Light, Stoichiometry, Etc.","Defaults are usually acceptable"),
                UseAdaptiveLight, MaxLightSat, MinLightSat, 
                KCarbon, Q10,  TMax, TRef,  KResp, Resp20,  EMort,
                P2OrgInit, N2OrgInit, ECoeffPhyto, Wet2Dry, PlantFracLipid,
                new TSubheading("Internal Nutrients Parameters","Only relevant if 'internal nutrients' is selected in setup"), NHalfSatInternal, PHalfSatInternal, MaxNUptake,
                MaxPUptake,  Min_N_Ratio, Min_P_Ratio,
                new TSubheading("Phytoplankton Only","Chlorophyll a conversion and sedimentation"), Plant_to_Chla, KSed, KSedTemp, KSedSalinity, ESed,
                new TSubheading("Periphyton and Macrophytes Only","Important calibration parameters, but for periphyton and macrophytes only"),  Macrophyte_Type, CarryCapac, Macro_VelMax, Red_Still_Water, FCrit,
                new TSubheading("If in Stream","Default is generally acceptable; stream-habitat preferences"),   PctSloughed, PrefRiffle, PrefPool,
                new TSubheading("Salinity Effects","Only relevant if calculating salinity impacts"),  SalMin_Phot, SalMax_Phot, SalCoeff1_Phot, SalCoeff2_Phot, SalMin_Mort, SalMax_Mort, SalCoeff1_Mort, SalCoeff2_Mort };
        }
    }



        public class TPlantToxRecord
    {
        public string Plant_name = String.Empty;
        public double EC50_photo = 0;
        public double EC50_exp_time = 0;
        public double EC50_dislodge = 0;
        public object EC50_comment;
        public double K2 = 0;
        public double K1 = 0;
        public double Entered_BCF = 0;
        public double Bio_rate_const = 0;
        public double LC50 = 0;
        public double LC50_exp_time = 0;
        public object LC50_comment;
        public double Lipid_frac = 0;
        public double LC50_Slope = 0;        // 12/14/2016 specific to plant/chemical combination
        public double EC50_Photo_Slope = 0;  // 4/5/2017  specific to plant/chemical/effect combination 
    }

    public class MortRatesRecord   // HMS not yet utilized as partial rates not yet implemented
    {
        // not saved to disk
        public double[] OrgPois = new double[Consts.NToxs];
        public double O2Mort;
        public double NH4Mort;
        public double NH3Mort;
        public double OtherMort;
        public double SaltMort;
        public double SedMort;
    } // end MortRatesRecord

    public class TPlant : TOrganism
    {
        public Loadings.TLoadings Predation_Link = null;
        public PlantRecord PAlgalRec;
        public double SinkToHypo = 0;
        // Set in Sedimentation
        public double[] EC50_Photo;
        [JsonIgnore] public TPlantToxRecord[] Plant_Tox = new TPlantToxRecord[Consts.NToxs];     // pointer to relevant plant toxicity data (nosave)
        [JsonIgnore] public MortRatesRecord MortRates = new MortRatesRecord();    // Holds data about how plant category is dying, (nosave)
        [JsonIgnore] public TMacroType MacroType;        // If plant is macrophyte, what type is it (nosave)
        [JsonIgnore] public bool SloughEvent = false;         // have conditions for a sloughing event been met?  Nosave
        [JsonIgnore] public double SloughLevel = 0;         // how far will biomass drop in a sloughin event?   NoSave
        public double Sloughing = 0;
        [JsonIgnore] public double NutrLim_Step = 1.0;         // nutrlimit calculated at the beginning of each step  NoSave
        [JsonIgnore] public double HabitatLimit = 1.0;       // Habitat Limitation nosave  
        public double ZOpt = 0;         // optimum depth for a given plant (a constant approximated at the beginning of the simulation)
        public double Lt_Limit = 0;
        public double Nutr_Limit = 0;
        public double Temp_Limit = 0;
        public double N_Limit = 0;
        public double PO4_Limit = 0;
        public double CO2_Limit = 0;
        public double Chem_Limit = 0;
        public double Vel_Limit = 0;
        public double LowLt_Limit = 0;
        [JsonIgnore] public double HighLt_Limit = 0;         // track for rates, NOSAVE JSC 9-5-2002
        [JsonIgnore] public double ResidenceTime = 0;        // phytoplankton residence time, set in washout, NOSAVE
        [JsonIgnore] public bool IsEunotia = false;          // Is this Eunotia?  Scientific name includes "eunotia,"  NOSAVE
        public AllVariables PSameSpecies = AllVariables.NullStateVar;

        public override void UpdateName()
        {
            PName = AQTSeg.StateText(NState) + ": [" + PAlgalRec.PlantName.Val + ']';
        }

        public TPlant(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            int ToxLoop;

            MortRates.OtherMort = 0;
            MortRates.SaltMort = 0;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                MortRates.OrgPois[ToxLoop] = 0;
            }
        }

        public override void SetToInitCond()
        {
            Assign_Plant_Tox();


            base.SetToInitCond();
            SinkToHypo = 0;
            SloughEvent = false;
            NutrLim_Step = 1;

            HabitatLimit = PHabitat_Limit();   //previously set in CalcRiskConc
            CalcRiskConc(true);   // Using ToxicityRecord Initialize Organisms                                   
        }


        public double PHabitat_Limit()
        {
            double HabitatAvail;
            double PctRun;
            double PrefRun;
            if (Location.SiteType != SiteTypes.Stream) return 1.0;

            PctRun = 100 - AQTSeg.Location.Locale.PctRiffle.Val - AQTSeg.Location.Locale.PctPool.Val;
            PrefRun = 100 - PAlgalRec.PrefRiffle.Val - PAlgalRec.PrefPool.Val;
            HabitatAvail = 0;
            if (PAlgalRec.PrefRiffle.Val > 0) HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctRiffle.Val / 100;

            if (PAlgalRec.PrefPool.Val > 0) HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctPool.Val / 100;

            if (PrefRun > 0) HabitatAvail = HabitatAvail + PctRun / 100;

            return HabitatAvail;
        }

        public void ChangeData()
        {
            // Modify THE UNITS WHICH CHANGE WITH PLANT TYPE
            if ((IsPhytoplankton()))
            {
                StateUnit = "mg/L dry";
                LoadingUnit = "mg/L dry";
            }
            else
            {
                StateUnit = "g/m2 dry";
                LoadingUnit = "g/m2 dry";
            }
            if (PAlgalRec.PlantType.Val == "Macrophytes")
            {
                if (PAlgalRec.Macrophyte_Type.Val == "benthic")
                {
                    MacroType = TMacroType.Benthic;
                }
                else if (PAlgalRec.Macrophyte_Type.Val == "free-floating")
                {
                    MacroType = TMacroType.Freefloat;
                }
                else if (PAlgalRec.Macrophyte_Type.Val == "rooted floating")
                {
                    MacroType = TMacroType.Rootedfloat;
                }
                else
                {
                    //   MessageBox.Show("Warning:  Macrophyte " + PName + " has an unrecognized type!", Application.ProductName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);  raise error?
                }
            }
        }

        // ChangeData MUST be called when the underlying data record is changed
        // ------------------------------------------------------------------------
        public void Assign_Plant_Tox()
        {
            int FoundToxIndx;
            int i;
            TPlantToxRecord PTR;
            TToxics TT;
            int ToxLoop;
            string DataName;
            DataName = PAlgalRec.ToxicityRecord.Val.ToLower();

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                TT = AQTSeg.GetStatePointer(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) as TToxics;
                if (TT != null)
                {
                    FoundToxIndx = -1;
                    for (i = 0; i < TT.Chem_Plant_Tox.Count; i++)
                    {
                        PTR = TT.Chem_Plant_Tox[i];
                        if (PTR.Plant_name.ToLower() == DataName)
                        {
                            FoundToxIndx = i;
                            break;
                        }
                    }
                    if (FoundToxIndx == -1)
                        throw new Exception("Error!  " + PAlgalRec.PlantName.Val + " uses the toxicity record \"" + DataName + "\" which is not found in chemical " + TT.ChemRec.ChemName + "\'s Plant toxicity data.  Study cannot be executed.");

                    PTR = TT.Chem_Plant_Tox[FoundToxIndx];
                    Plant_Tox[ToxLoop] = PTR;
                }
            }
        }

        // ------------------------------------------------------------------------

        public override double WetToDry()
        {
            return PAlgalRec.Wet2Dry.Val;
        }

        public double AggregateFracPhoto()
        {
            int ToxLoop;
            double AggFracPhoto;
            AggFracPhoto = 0;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                if (AQTSeg.GetStatePointer(NState, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) != null)
                {
                    AggFracPhoto = AggFracPhoto + (1.0 - FracPhoto[ToxLoop]);
                }
            }
            if (AggFracPhoto > 1.0)
            {
                AggFracPhoto = 1.0;
            }
            AggFracPhoto = (1.0 - AggFracPhoto);
            return AggFracPhoto;
        }

        public virtual double Photosynthesis()
        {
            double Photosyn;
            double Substrate;
            double PPL;
            AllVariables Macro;
            double MacroBiomass;
            double MacroArea;
            object PMacro;
            double Salteffect;
            if ((PAlgalRec.PlantType.Val == "Phytoplankton"))
            {
                // g/cu m-d   1/d    unitless     g/cu m
                Photosyn = PAlgalRec.PMax.Val * PProdLimit() * State;
            }
            else
            {
                // benthic
                MacroBiomass = 0;
                MacroArea = 0;
                for (Macro = Consts.FirstMacro; Macro <= Consts.LastMacro; Macro++)
                {
                    PMacro = AQTSeg.GetStatePointer(Macro, T_SVType.StV, T_SVLayer.WaterCol);
                    if (PMacro != null)
                    {
                        MacroBiomass = MacroBiomass + AQTSeg.GetState(Macro, T_SVType.StV, T_SVLayer.WaterCol);
                    }
                }
                if (IsPeriphyton())
                {
                    MacroArea = 0.12 * MacroBiomass;
                }
                Substrate = Location.FracLittoral(AQTSeg.ZEuphotic(), AQTSeg.Volume_Last_Step) + MacroArea;
                PPL = PProdLimit();
                Vel_Limit = VLimit();
                Photosyn = PAlgalRec.PMax.Val * PPL * Vel_Limit * State * Substrate;
                // g/m3-d      1/d   unitless  unitless   g/m3    unitless
            }
            Salteffect = AQTSeg.SalEffect(PAlgalRec.SalMin_Phot.Val, PAlgalRec.SalMax_Phot.Val, PAlgalRec.SalCoeff1_Phot.Val, PAlgalRec.SalCoeff2_Phot.Val);
            return Photosyn * HabitatLimit * Salteffect;
            // with
        }

        // Photosynthesis
        // (*********************************)
        // (* algal excretion due to photo- *)
        // (* respiration, Desormeau, 1978  *)
        // (*********************************)
        public virtual double PhotoResp()
        {
            double Excrete;
            double LightStress;
            // const      actually = KResp
            // KStress = 0.03;
            // not 3.0; RAP 9/5/95
            LightStress = 1.0 - LtLimit(AQTSeg.PSetup.ModelTSDays.Val);
            if (LightStress < Consts.Small)
            {
                LightStress = 0.0;
            }
            // RAP, 9/7/95, KResp should be mult. by LightStress - see Collins et al., 1985
            // + KStress
            Excrete = Photosynthesis() * (PAlgalRec.KResp.Val * LightStress);
            // g/cu m-d  g/cu m-d         unitless unitless     unitless

            return Excrete;
        }

        // algalexcr
        // (***********************************)
        // (* dark respiration, Collins, 1980 *)
        // (***********************************)
        public override double Respiration()
        {
            double Temp;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            return PAlgalRec.Resp20.Val * Math.Pow(1.045, (Temp - 20)) * State;
            // g/g day        // 1/C                  // deg C       // g/cu m
        }

        // algalrespire
        // (*****************************************************************)
        // (* algal mortality due to low nutrients,  subopt light & high T  *)
        // (* ExcessT: Scavia & Park, 1977; Stress: Collins & Park, 1989    *)
        // (*****************************************************************)
        public override double Mortality()
        {
            double ExcessT;
            double Dead;
            double Stress;
            TStateVariable TSV;
            double Pois;
            int ToxLoop;
            double WaterTemp;
            WaterTemp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (WaterTemp > PAlgalRec.TMax.Val)
            {
                // 6-12-2006 Fixed Logic
                ExcessT = 1.0;
            }
            else
            {
                ExcessT = Math.Exp(WaterTemp - PAlgalRec.TMax.Val) / 2.0;
            }
            Stress = (1.0 - Math.Exp(-PAlgalRec.EMort.Val * (1.0 - (NutrLimit() * LtLimit(AQTSeg.PSetup.ModelTSDays.Val)))));
            // emort is approximate maximum fraction killed
            // per day by nutrient and light limitation
            Dead = (PAlgalRec.KMort.Val + ExcessT + Stress) * State;
            // g/cu m-d               (        g/g-d           )   g/cu m
            //MortRates.OtherMort = Dead;
            //MortRates.SaltMort = State * SalMort(PAlgalRec.SalMin_Mort.Val, PAlgalRec.SalMax_Mort.Val, PAlgalRec.SalCoeff1_Mort.Val, PAlgalRec.SalCoeff2_Mort.Val);
            //Dead = Dead + MortRates.SaltMort;
            // g/cu m-d                g/cu m-d
            Setup_Record SR = AQTSeg.PSetup;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                RedGrowth[ToxLoop] = 0;     // 5/3/2017 defaults for no effects if tox is zero
                RedRepro[ToxLoop] = 0;
                FracPhoto[ToxLoop] = 1;

                if (SR.UseExternalConcs.Val) TSV = AQTSeg.GetStatePointer(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol);
                else TSV = AQTSeg.GetStatePointer(NState, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol);
                if (TSV != null)
                    if (TSV.State > 0)
                    {
                        Pois = Poisoned((T_SVType)ToxLoop + 2);
                        MortRates.OrgPois[ToxLoop] = Pois;
                        Dead = Dead + Pois;
                    };
            }
            if (Dead > State)
            {
                Dead = State;
            }
            return Dead;
        }

        public bool IsPeriphyton()
        {
            return (PAlgalRec.PlantType.Val == "Periphyton");
        }

        public bool IsPhytoplankton()
        {
            return (PAlgalRec.PlantType.Val == "Phytoplankton");
        }

        public bool IsLinkedPhyto()
        {
            AllVariables PLoop;
            TPlant PPeri;
            if (PAlgalRec.PlantType.Val != "Phytoplankton") return false;
            for (PLoop = Consts.FirstAlgae; PLoop <= Consts.LastAlgae; PLoop++)
            {
                PPeri = AQTSeg.GetStatePointer(PLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PPeri != null)
                {
                    if ((PPeri.IsPeriphyton()) && (PPeri.PSameSpecies == NState)) return true;
                }
            }
            return false;
        }

        public double SedToMe()
        {
            // Calculates sedimentation of phytoplankton to each periphyton compartment
            // JSC Sept 8, 2004
            AllVariables ploop;
            TPlant PPeri;
            TPlant PPhyto;
            double LinkCount;
            double LinkMass;
            if (!IsPeriphyton()) return 0;
            if (PSameSpecies == AllVariables.NullStateVar) return 0;
            if (AQTSeg.GetStatePointer(PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) == null) return 0;

            LinkCount = 0;
            LinkMass = 0;
            for (ploop = Consts.FirstAlgae; ploop <= Consts.LastAlgae; ploop++)
            {
                PPeri = AQTSeg.GetStatePointer(ploop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PPeri != null)
                {
                    if ((PPeri.IsPeriphyton()) && (PPeri.PSameSpecies == PSameSpecies))
                    {
                        LinkCount = LinkCount + 1.0;
                        LinkMass = LinkMass + PPeri.State;
                        // mg/L
                    }
                }
                // will count itself and any other periphyton species linked to this phytoplankton
            }
            PPhyto = AQTSeg.GetStatePointer(PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
            if (PPhyto == null) return 0;
            // If no periphyton mass, dist. evenly among all periphyton comps.  9/20/04

            if (LinkMass < Consts.VSmall)
                return PPhyto.Sedimentation() / LinkCount;
            else
                return PPhyto.Sedimentation() * (State / LinkMass);
            // mg/L          // mg/L
        }

        // (**************************************)
        // (*       loss due to sinking          *)
        // (**************************************)
        public double Sedimentation()
        {
            double SedAccel;
            double Sink;
            double Thick;
            double Decel;
            double DensFactor;
            // TBottomSediment TopCohesive;
            SinkToHypo = 0;
            Thick = Location.MeanThick;
            if (!(IsPhytoplankton()))
            {
                return 0;
            }
            if (PAlgalRec.SurfaceFloating.Val)
            {
                return 0;
            }
            SedAccel = Math.Exp(PAlgalRec.ESed.Val * (1.0 - PProdLimit()));
            // Collins & Park, 1989
            DensFactor = AQTSeg.DensityFactor(PAlgalRec.KSedTemp.Val, PAlgalRec.KSedSalinity.Val);

            //if (AQTSeg.EstuarySegment)
            //{
            //    // g/cu m-d   m/d    m     unitless   unitless   g/cu m
            //    Sink = PAlgalRec.KSed.Val / Thick * DensFactor * SedAccel * State;
            //}
            //else
            {
                // unitless       unitless       unitless
                if (AQTSeg.MeanDischarge < Consts.Tiny)
                {
                    // g/cu m-d   m/d    m      unitless   g/cu m    unitless
                    Sink = PAlgalRec.KSed.Val / Thick * SedAccel * State * DensFactor;
                }
                else
                {
                    // calculate Decel
                    //if (AQTSeg.SedModelIncluded())
                    //{
                    //    TopCohesive = (AQTSeg.GetStatePointer(AllVariables.Cohesives, T_SVType.StV, T_SVLayer.SedLayer1));
                    //    if ((TopCohesive.Scour() == 0) && (TopCohesive.Deposition() == 0))
                    //        Decel = 0;
                    //    else if ((TopCohesive.Deposition() > TopCohesive.Scour()))
                    //        Decel = 1;
                    //    else
                    //        Decel = TopCohesive.Deposition() / TopCohesive.Scour();
                    //}
                    //else
                    {
                        // No SedModel Included
                        if (Location.TotDischarge > AQTSeg.MeanDischarge)
                            Decel = AQTSeg.MeanDischarge / (Location.TotDischarge);
                        // cu m/d                    cu m/d
                        else
                            Decel = 1;
                    }
                    Sink = PAlgalRec.KSed.Val / Thick * SedAccel * State * Decel * DensFactor;
                }
                // MeanDisch > Tiny

                //if (!AQTSeg.SedModelIncluded())
                //{
                //    // m/sec
                if ((AQTSeg.GetState(AllVariables.WindLoading, T_SVType.StV, T_SVLayer.WaterCol) >= 2.5) && (Thick <= 1.0))
                {
                    Sink = 0.0;                  // should be a power fn. of wind & depth
                }
                // }

            }  // if Not EstuarySegment

            return Sink;

            // stratified code removed
        }

        // public double Floating()   code for vertically stratified segments removed

        // photosynthetically active radiation at optimum depth
        // (******************************************)
        // (* Sloughing & washout, last mod. 3/19/03 *)
        // (******************************************)
        public double ToxicDislodge()
        {
            double ToxLevel;
            double Dislodge;
            int ToxLoop;
            const double MaxToxSlough = 0.1;
            // 10% per day
            Dislodge = 0;
            if ((!IsPhytoplankton()) && (Plant_Tox != null))
            {
                for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
                {
                    if (Plant_Tox[ToxLoop] != null)
                    {
                        if (Plant_Tox[ToxLoop].EC50_dislodge > Consts.Tiny)
                        {
                            ToxLevel = AQTSeg.GetState(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol);
                            // evaluates to -1 if tox not in study
                            if (ToxLevel > 0)
                            {      // Toxicant Induced Dislodge

                                Dislodge = Dislodge + MaxToxSlough * ToxLevel / (ToxLevel + Plant_Tox[ToxLoop].EC50_dislodge) * State;
                            }   // g/m3 d    g/m3 d    10% / d        ug/L         ug/L                          ug/L          g/m3

                        }
                    }
                }
            }
            if (Dislodge > State)
            {
                Dislodge = State;
            }
            return Dislodge;
            // g/m3 d   g/m3 d
        }

        public double CalcSlough()
        {
            const double UnitArea = 1.0;    // mm2
            const double DragCoeff = 2.53E-4;  // unitless, suitable for calibration because little exposure; 0.01 in Asaeda & Son, 2000
            const int Rho = 1000;       // kg/m3

            const int MINBIOMASS = 0;  // 1e-6 mg/L  // DISABLED
            double Wash;
            double Biovol_Fil;
            double Biovol_Dia;
            double DragForce;   // Newtons
            double AvgVel;
            double DailyVel;
            double Nutr;
            double LtL;
            double Suboptimal;
            double Adaptation;
            Wash = 0;   // mg/L d

            Nutr = NutrLim_Step;
            LtL = LtLimit(AQTSeg.PSetup.ModelTSDays.Val);
            Suboptimal = Nutr * LtL * AQTSeg.TCorr(PAlgalRec.Q10.Val, PAlgalRec.TRef.Val, PAlgalRec.TOpt.Val, PAlgalRec.TMax.Val) * 20;        // 5 to 20 RAP 1-24-05
            if (Suboptimal > 1) Suboptimal = 1;

            // uses nutrlimit calculated at the beginning of this time-step
            if ((!IsPhytoplankton()))
            {
                // not floating, calculate slough & scour
                Biovol_Fil = State / 8.57E-9 * AQTSeg.DynamicZMean();
                // mm3/mm2
                Biovol_Dia = State / 2.08E-9 * AQTSeg.DynamicZMean();
                // mm3/mm2
                AvgVel = AQTSeg.Velocity(PAlgalRec.PrefRiffle.Val, PAlgalRec.PrefPool.Val, true) * 0.01;   // Avg. Velocity exposure of this organism
                                                                                                           //    {m/s}            {cm/s}                                           {avg}   {m/cm}

                DailyVel = AQTSeg.Velocity(PAlgalRec.PrefRiffle.Val, PAlgalRec.PrefPool.Val, false) * 0.01;  // Avg. Velocity exposure of this organism
                                                                                                             //    {m/s}            {cm/s}                                           {avg}   {m/cm}

                if (!(Location.SiteType == SiteTypes.Stream))
                {
                    AvgVel = 0.003;  // based on KCap of 200 g/m2
                }

                Adaptation = Math.Pow(AvgVel, 2.0) / 0.006634;
                // RAP 1/20/2005
                if (!SloughEvent) // check to see if sloughevent should be initiated
                {
                    if ((NState >= Consts.FirstDiatom) && (NState <= Consts.LastDiatom))
                    {
                        // Periphyton -- Diatoms
                        DragForce = Rho * DragCoeff * AQMath.Square(DailyVel) * Math.Pow(Biovol_Dia * UnitArea, (2.0 / 3.0)) * 1E-6;
                        // kg m/s2  kg/m3 unitless                m/s                     mm3/mm2       mm2                 m2/mm2
                        // 3/19/2013 fcrit of zero means no scour

                        if ((Adaptation > 0) && (PAlgalRec.FCrit.Val > Consts.Tiny) && (State > MINBIOMASS) && (DragForce > Suboptimal * PAlgalRec.FCrit.Val * Adaptation))
                        {   // frac living
                            SloughEvent = true;
                            SloughLevel = State * (1.0 - (PAlgalRec.PctSloughed.Val / 100.0));

                            //AQTSeg.ProgData.SloughDia = true;  update progress bar, N/A for HMS
                            //AQTSeg.ProgData.PeriVis = true;
                        }
                    }
                    else
                    {
                        // filamentous (includes greens and blgreens and should not be confused
                        // with filamentous phytoplankton)
                        DragForce = Rho * DragCoeff * AQMath.Square(DailyVel) * Math.Pow((Biovol_Fil * UnitArea), (2.0 / 3.0)) * 1E-6;
                        // kg m/s2  kg/m3 unitless                m/s                      mm3/mm2       mm2             m2/mm2

                        // 3/19/2013 fcrit of zero means no scour
                        if ((Adaptation > 0) && (PAlgalRec.FCrit.Val > Consts.Tiny) && (State > MINBIOMASS) && (DragForce > Suboptimal * PAlgalRec.FCrit.Val * Adaptation))
                        {
                            SloughEvent = true;
                            SloughLevel = State * (1.0 - (PAlgalRec.PctSloughed.Val / 100.0));

                            //AQTSeg.ProgData.PeriVis = true;   Progress Dialog Update, N/A HMS
                            //if (NState >= Consts.FirstGreens && NState <= Consts.LastGreens)
                            //    ProgData.SloughGr = true;
                            //else
                            //    ProgData.SloughBlGr = true;
                        }
                    }
                }

            }  // if not phytoplankton

            return Wash;
            // g/m3 d
        }

        // Function Sloughing
        public override double Washout()
        {
            double Wash;
            double SegVolume;
            double Disch;
            SegVolume = AQTSeg.SegVol();
            Wash = 0;
            Disch = Location.Discharge;
            if (Disch == 0)
            {
                return Wash;
            }
            if ((IsPhytoplankton()))
            {
                // Phytoplankton Code
                Wash = Disch / SegVolume * State;
                // (g/cu m-d) (cu m/d) (cu m)  (g/cu m)
                if (Disch < Consts.Tiny) ResidenceTime = 999999;
                else ResidenceTime = SegVolume / Disch * PhytoResFactor();
                // days      // cu m   // cu m/ d    // unitless
            }
            // end Phytoplankton Code

            //             WashoutStep[AQTSeg.DerivStep] = Wash * AQTSeg.SegVol();
            return Wash;
            // g/m3 d

        }

        // Function Washout
        public double KCAP_in_g_m3()
        {
            // note, deeper sites have lower g/m3 KCAP
            return PAlgalRec.CarryCapac.Val * Location.Locale.SurfArea.Val / AQTSeg.Volume_Last_Step;   //  11/3/2014 replaced static zmean with more consistent conversion
                                                                                                    //   {g/m3}              {g/m2}                      {m2}               {m3}


            // Stratified and linked-mode code removed
        }

        // Photosynthesis
        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        public double PAR_OD(double Light)
        {
            // photosynthetically active radiation at optimum depth  8/22/2007
            double EX;
            double PAR;
            PAR = Light * 0.5;
            EX = AQTSeg.Extinct(IsPeriphyton(), true, true, false, 0);
            // don't include cyanobacteria self shading effects because result is used for LtAtTop and LtAtDepth
            return PAR * Math.Exp(-EX * ZOpt);
            // TPlant.ZOpt Set within Initial Condition in TStates.SVsToInitConds()
        }

        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        public double LightSat()             // Aug 16, 2007, Light Saturation including Adaptive Light

        {
            double LightSatCalc;
            double[] LightHist = new double[3 + 1];

            void LightHistory() // light 1 2 and 3 calendar days before TPresent
            {
                int i;
                int index;
                TSVConc LightVal;
                int[] n = new int[3];
                TLight PL;
                for (i = 0; i <= 2; i++)
                {
                    LightHist[i] = 0;
                    n[i] = 0;
                }

                for (i = 0; i < AQTSeg.PLightVals.Count; i++)
                {
                    LightVal = AQTSeg.PLightVals[i];
                    index = (int)(AQTSeg.TPresent.Date - (LightVal.Time.AddDays(-0.001).Date)).TotalDays;
                    if (index >= 0 && index <= 2)
                    {
                        if (n[index] == 0)
                        {
                            LightHist[index] = PAR_OD(LightVal.SVConc);
                        }
                        // photosynthetically active radiation at optimum depth based on hist. light
                        n[index]++;
                        // count incidences to ensure record exists
                    }
                }

                // if inadequate history exists, fill in historical values with more recent values
                if (n[0] == 0)
                {
                    PL = AQTSeg.GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol) as TLight;
                    LightHist[0] = PAR_OD(PL.DailyLight);  // photosynthetically active radiation at optimum depth based on current daily light
                }
                if (n[1] == 0) LightHist[1] = LightHist[0];
                if (n[2] == 0) LightHist[2] = LightHist[1];
            } // end LightHistory function

            if (!PAlgalRec.UseAdaptiveLight.Val) return PAlgalRec.EnteredLightSat.Val;
            LightHistory();
            LightSatCalc = (0.7 * LightHist[0] + 0.2 * LightHist[1] + 0.1 * LightHist[2]);    // 8-20-07

            // ensure calculation is within limits, 8-28-2007
            if (LightSatCalc < PAlgalRec.MinLightSat.Val) LightSatCalc = PAlgalRec.MinLightSat.Val;
            if (LightSatCalc > PAlgalRec.MaxLightSat.Val) LightSatCalc = PAlgalRec.MaxLightSat.Val;

            return LightSatCalc;
        }

        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public double LtAtTop(bool DailyStep)  // light limitation at the top of the water column
                                               // for HMS implementation, vertical stratification not implemented, hypolimnion code was removed
        {
            double LightVal, LT, EX;
            double DTop;
            double LightCorr;
            bool Incl_Periphyton;
            TLight PL;
            // Floating Macrophytes are not subject to light limitation, this function is never executed for them

            Incl_Periphyton = !(IsPhytoplankton());
            DTop = 0;
            EX = 0;

            // only half is in spectrum used for photsyn. - Edmondson '56
            PL = AQTSeg.GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol) as TLight;

            if (DailyStep) LightVal = PL.DailyLight;
            else LightVal = PL.HourlyLight;

            if (DailyStep) LightCorr = 1.0;
            else LightCorr = 1.25;

            LT = (-((LightVal / 2) / (LightCorr * LightSat())) * Math.Exp(-EX * DTop));
            // unitless      ly/d                  ly/d                     1/m  m

            if (LT < -30) LT = 0;  // Prevent a Crash, JSC
            else LT = Math.Exp(LT);

            return LT;
        }         // ltatTop

        // light limitation at depth
        public double LtAtDepth(bool DailyStep)
        {   // HMS Light at Top, Vertical stratification code stripped, not relevant to HMS Work Flows
            // Floating Macrophytes not subject to light limitation, this function is never executed for them

            double LightVal, LD, EX, DBott;
            double LightCorr;
            bool Incl_Periphyton;
            TLight PL = AQTSeg.GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol) as TLight;

            Incl_Periphyton = !(IsPhytoplankton());
            EX = AQTSeg.Extinct(Incl_Periphyton, true, true, false, 0);
            DBott = DepthBottom();

            if (AQTSeg.PSetup.ModelTSDays.Val) LightVal = PL.DailyLight;
            else LightVal = PL.HourlyLight;

            if ((EX * DBott) > 20.0) LD = 1.0;
            else
            {
                if (AQTSeg.PSetup.ModelTSDays.Val) LightCorr = 1.0;
                else LightCorr = 1.25;

                // only half is in spectrum used for photsyn. - Edmondson '56
                LD = (-((LightVal / 2.0) / (LightCorr * LightSat())) * Math.Exp(-EX * (DBott)));
                // unitless     ly/d                   ly/d           1/m    m        m  

                if (LD < -30) LD = 0;    // Prevent a Crash, JSC
                else LD = Math.Exp(LD);
            }

            return LD;
        }  // ltatdepth

        public double DepthBottom()
        {  // HMS Vertical stratification code stripped, not relevant to HMS Work Flows

            double DB;

            if (AQTSeg.Location.Locale.UseBathymetry.Val) DB = AQTSeg.Location.Locale.ZMax.Val;
            else DB = AQTSeg.DynamicZMean();

            if ((IsPhytoplankton()))
            {
                if ((AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) <= AQTSeg.Ice_Cover_Temp()) && (AQTSeg.Location.MeanThick > 2.0))
                {
                    DB = 2.0;  // algae in top 2 m under ice
                }

                // Account for buoyancy due to gas vacuoles
                if ((PAlgalRec.SurfaceFloating.Val) && (DB > 1.0))    // Removed 0.2, 1-13-2005 RAP, 2-2007                     // 3/9/2012 surfacefloating variable
                {
                    if ((NutrLimit() > 0.25))  // healthy
                    {

                        if ((AQTSeg.GetState(AllVariables.WindLoading, T_SVType.StV, T_SVLayer.WaterCol) < 3))
                        // wind < 3.0 m/sec, otherwise Langmuir circ.
                        {
                            DB = 0.1;                             // RAP 1-12-2005
                        }
                        else if (DB > 3.0)
                        {
                            DB = 3.0; // roughly the depth of a Langmuir cell
                        }
                    }
                }
            }  // phytoplankton
            else
            {
                // macrophytes and periphyton
                if (AQTSeg.ZEuphotic() < DB)
                {
                    DB = AQTSeg.ZEuphotic();
                }
            }
            return DB;
        }

        // -------------------------------------------------
        public double LtLimit_PeriphytExt()
        {
            double PeriExtinction;
            AllVariables Phyto;
            TPlant PPhyt;
            PeriExtinction = 0;
            // initialize
            for (Phyto = Consts.FirstAlgae; Phyto <= Consts.LastAlgae; Phyto++)
            {
                PPhyt = AQTSeg.GetStatePointer(Phyto, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PPhyt != null)
                {
                    if ((PPhyt.IsPeriphyton()))
                    {
                        PeriExtinction = PeriExtinction + PPhyt.PAlgalRec.ECoeffPhyto.Val * AQTSeg.GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol);
                    }
                }
            }
            return Math.Exp(-PeriExtinction);
            // 1/m
        }

        // (*****************************************)
        // (* algal light limitation                *)
        // (* Steele, 1962; Kremer & Nixon, 1979    *)
        // (* based on modification by DiToro, 1971 *)
        // (*****************************************)
        public double LtLimit(bool DailyStep)
        {
            double LL;
            // RAP changed DB to DBot & added P & E. 8/23/95
            double DBot;
            double DT;
            double LT;
            double LD;
            double P;
            double E;
            // -------------------------------------------------
            double PeriExt;
            bool Inhibition;
            //            bool WellMixedLinked;
            double Const_a;
            LL = 1;
            if ((PAlgalRec.PlantType.Val == "Macrophytes") && (MacroType == TMacroType.Freefloat)) return LL;

            DBot = DepthBottom();
            DT = 0;  // HMS no vertical stratification
            if ((DBot - DT) < Consts.Tiny)
            {
                LL = 1.0;
                // avoid divide by zero
                LowLt_Limit = 1.0;
                HighLt_Limit = 1.0;
            }
            else
            {
                LT = LtAtTop(DailyStep);
                LD = LtAtDepth(DailyStep);
                P = AQTSeg.Photoperiod();

                E = AQTSeg.Extinct(false, true, true, PAlgalRec.SurfaceFloating.Val, 0);
                // periphyton are not included because "PeriphytExt" handles periphyton extinction


                if ((!IsPhytoplankton()))                  // 3/9/2012 bl-greens to surfacefloater  // i.e. 'Periphyton', 'Macrophytes', or 'Bryophytes'
                {
                    PeriExt = LtLimit_PeriphytExt();
                }
                else
                {
                    PeriExt = 1.0;
                }
                if (AQTSeg.PSetup.ModelTSDays.Val)
                {
                    LL = 0.85 * (2.71828 * P / (E * (DBot - DT))) * (LD - LT) * PeriExt;
                }
                else
                {
                    LL = (2.71828 / (E * (DBot - DT))) * (LD - LT) * PeriExt;
                }
                // removed 0.85 and photoperiod
                if (LL > 1.0)
                {
                    LL = 1.0;
                }
                if (LL < Consts.Small)
                {
                    LL = 0.0;
                }
                // RAP, 9/11/95
                if ((AQTSeg.PSetup.ModelTSDays.Val))  // if daily time step is specified
                {
                    Lt_Limit = LL;  // save for rate output  JSC 9-5-02
                    Const_a = Math.Exp(-E * DT) * Math.Exp(-E * DBot);
                    Inhibition = LT - (Const_a * LD) < 0;
                    if (Inhibition)
                    {
                        LowLt_Limit = 1.0;
                        HighLt_Limit = Lt_Limit;
                        // 1/13/2014 make non-affected limitation 1.0
                    }
                    else
                    {
                        LowLt_Limit = Lt_Limit;
                        HighLt_Limit = 1.0;
                    }
                }
            }
            return LL;
        }

        // (********************************)
        // (* algal phosphate limitation   *)
        // (* Michaelis-Menten kinetics    *)
        // (********************************)
        public double PO4Limit()
        {
            double PLimit;
            double P2O;
            if (AQTSeg.PSetup.Internal_Nutrients.Val)
            {
                P2O = P_2_Org();
                if (P2O > Consts.Tiny)
                {
                    // Internal nutrientsP Limit
                    PLimit = (1.0 - (PAlgalRec.Min_P_Ratio.Val / P2O));
                }
                else PLimit = 0.0;
            }
            else
            {
                // external nutrients
                if ((AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) + PAlgalRec.KPO4.Val) == 0)
                {
                    PLimit = 1.0;
                }
                else
                {
                    PLimit = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) / (AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) + PAlgalRec.KPO4.Val);
                } // unitless =                                                       P/PO4                        gPO4/cu m                                                         P/PO4            gPO4/cu m            gP/cu m

            }  // external nutrients


            if (PLimit < Consts.Small)
                PLimit = 0.0;      // RAP, 9/11/95 Tiny -> Small

            return PLimit;
        }

        // phoslimit
        public bool Is_Pcp_CaCO3()
        {
            // Is this plant precipitating CaCO3?
            // JSC, From 8.25 to 7.5 on 7/2/2009
            return (AQTSeg.GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol) >= 7.5) &&
                !((PAlgalRec.PlantType.Val == "Bryophytes") || (NState == AllVariables.OtherAlg1) || (NState == AllVariables.OtherAlg2));
            // subset of plants, all plants except Bryophytes and "Other Algae" compartments
        }

        // (********************************)
        // (* algal carbon limitation      *)
        // (* Michaelis-Menten kinetics    *)
        // (********************************)
        public double CO2Limit()
        {
            double CLimit;
            double CDioxide;
            const double C2CO2 = 0.27;
            // KCO2 = 0.346;
            // ave from Collins & Wlosinski, 1983
            // IF Is_Pcp_CaCO3
            // Then Begin CO2Limit := 1.0; Exit; End;
            // {10/26/07 -- Because plants are deriving C from the bicarbonate reaction,
            // those plants precipitating calcite are not limited by CO2 in the water column
            // 11/09/07 -- CO2 Limitation Returned to precipitating plants
            CDioxide = AQTSeg.GetState(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
            if (CDioxide < Consts.Small)
            {
                CLimit = 0.0;
            }
            else
            {
                // RAP, 9/9/95 replaced constant KCO2 with PAlgalRec^.KCarbon.Val
                CLimit = C2CO2 * CDioxide / (C2CO2 * CDioxide + PAlgalRec.KCarbon.Val);
            }
            // unitless C/CO2   gCO2/cu m C/CO2        gCO2/cu m      gC/cu m
            return CLimit;
        }

        // phoslimit
        public double Nutr_2_Org(T_SVType NTyp)
        {
            if (NTyp == T_SVType.NIntrnl)
                return N_2_Org();
            else
                return P_2_Org();
        }

        public double N_2_Org()
        {
            // g N / g OM
            if (AQTSeg.PSetup.Internal_Nutrients.Val && (State > Consts.Tiny))
            {
                if (AQTSeg.GetStatePointer(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) != null)
                    return AQTSeg.GetState(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) / State * 1e-3;
                // (g/g OM)   =   (ug N /L)                                             /(mg OM/L)*(mg/ug)

                else return PAlgalRec.N2OrgInit.Val;     // rooted macrophyte
            }
            else
            {
                return PAlgalRec.N2OrgInit.Val;
            }
        }

        public double P_2_Org()
        {
            if (AQTSeg.PSetup.Internal_Nutrients.Val && (State > Consts.Tiny))
            {
                if (AQTSeg.GetStatePointer(NState, T_SVType.PIntrnl, T_SVLayer.WaterCol) != null)
                {
                    return AQTSeg.GetState(NState, T_SVType.PIntrnl, T_SVLayer.WaterCol) / State * 1e-3;
                    // ug N /L                                                          // mg/L  // mg/ug

                }
                else
                    return PAlgalRec.P2OrgInit.Val;  // rooted macrophyte
            }
            else
            {
                return PAlgalRec.P2OrgInit.Val;
            }
        }

        public bool IsFixingN()
        {
            double Nitrogen;
            double InorgP;
            double NtoP;

            if (!(NState >= Consts.FirstBlGreen && NState <= Consts.LastBlGreen)) return false;    // IsFixingN true for cyanobacteria only

            if (AQTSeg.GetStatePointer(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) != null)      // internal nutrients option 3/18/2014
                return (N_2_Org() < 0.5 * PAlgalRec.NHalfSatInternal.Val);

            if (!AQTSeg.PSetup.NFix_UseRatio.Val)  // added option 3/19/2010
            {
                Nitrogen = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
                return (Nitrogen < 0.5 * PAlgalRec.KN.Val);   // Official "Release 3" code
            }
            else
            {
                // 12-16-2009 N Fixing Option
                InorgP = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
                // Avoid Divide by Zero
                if (InorgP > Consts.Tiny)
                {
                    NtoP = (AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol)) / InorgP;
                    if ((NtoP < AQTSeg.PSetup.NtoPRatio.Val)) return true; // If inorganic N over Inorganic P ratio is less than NtoPRatio (Default of 7) then cyanobacteria fix nitrogen
                }

            }
            return false;
        }

        // (********************************)
        // (* algal nitrogen limitation    *)
        // (********************************)
        public double NLimit()
        {
            double Nitrogen;
            double NL;
            double N2O;

            Nitrogen = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            // gN/cu m    N/NH4            gNH4/cu m                                                                 N/NO3           gNO3/cu m
            if (IsFixingN())
            {
                // N-fixation
                NL = 1.0;
            }
            else if (AQTSeg.PSetup.Internal_Nutrients.Val)
            {
                N2O = N_2_Org();
                if (N2O > Consts.Tiny)
                {
                    // Internal nutrients N Limit
                    NL = (1.0 - (PAlgalRec.Min_N_Ratio.Val / N2O));
                }
                else
                {
                    NL = 0.0;
                }
                // with PAlgalRec.Val
            }
            else
            {
                // external nutrients
                if ((Nitrogen + PAlgalRec.KN.Val) == 0)
                {
                    NL = 1;
                }
                else
                {
                    NL = Nitrogen / (Nitrogen + PAlgalRec.KN.Val);
                }
                // unitless gN/cu m gN/cu m     gN/cu m
            }
            if (NL < Consts.Small)
            {
                NL = 0.0;
            }
            return NL;
        }

        // nitrolimit
        public double NutrLimit()
        {
            double NLM;
            PO4_Limit = PO4Limit();
            // save for rate output  JSC 1-30-03
            N_Limit = NLimit();
            // save for rate output  JSC 1-30-03
            NLM = Math.Min(N_Limit, PO4_Limit);
            CO2_Limit = CO2Limit();
            // save for rate output  JSC 1-30-03
            NLM = Math.Min(NLM, CO2_Limit);
            Nutr_Limit = NLM;
            // save for rate output  JSC 9-5-02
            return NLM;
        }

        public double PProdLimit()
        {
            double PL;
            double AggFP;
            double LL;
            double NL;
            double TCorrValue;
            TCorrValue = AQTSeg.TCorr(PAlgalRec.Q10.Val, PAlgalRec.TRef.Val, PAlgalRec.TOpt.Val, PAlgalRec.TMax.Val);
            LL = LtLimit(AQTSeg.PSetup.ModelTSDays.Val);
            NL = NutrLimit();
            AggFP = AggregateFracPhoto();
            Temp_Limit = TCorrValue;  // save for rate output  JSC 9-5-02
            Chem_Limit = AggFP;       // save for rate output  JSC 9-5-02
            PL = LL * NL * TCorrValue * AggFP;
            // all unitless
            if (PL > 1.0) PL = 1.0; // make sure it is truly a reduction factor

            return PL;
        }

        // (***************************************)
        // (*  current limitation for periphyton  *)
        // (*  Colby and McIntire, 1978           *)
        // (***************************************)
        public double VLimit()
        {
            double Vel;
            double VL;
            const double VelCoeff = 0.057;
            if ((Location.SiteType == SiteTypes.Stream))
            {
                Vel = AQTSeg.Velocity(PAlgalRec.PrefRiffle.Val, PAlgalRec.PrefPool.Val, false);
                // cm/s
                VL = Math.Min(1.0, (PAlgalRec.Red_Still_Water.Val + VelCoeff * Vel / (1.0 + VelCoeff * Vel)));
                // frac                 unitless             // unitless  // m/s    // unitless  // m/s
            }
            else
            {
                VL = PAlgalRec.Red_Still_Water.Val;
            }
            return VL;
        }

        public double PeriphytonSlough()
        {
            AllVariables PlantLoop;
            TPlant PP;
            double PSlough;
            double j;
            PSlough = 0;
            for (PlantLoop = Consts.FirstAlgae; PlantLoop <= Consts.LastAlgae; PlantLoop++)
            {
                PP = AQTSeg.GetStatePointer(PlantLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PP != null)
                {
                    if (PP.PSameSpecies == NState)
                    {
                        PP.Washout();
                        // update sloughevent
                        if (PP.SloughEvent)
                        {
                            j = -999;
                            // signal to not write mass balance tracking
                            PP.Derivative(ref j);
                            // update sloughing
                            PSlough = PSlough + PP.Sloughing * (1.0 / 3.0);
                            // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/Slough.
                        }
                    }
                }
            }
            return PSlough;
        }

        //public void Derivative_TrackMB()
        //{
        //    double NutrFrac;
        //    double LoadInKg;
        //    double LossInKg;
        //    double LayerInKg;
        //    AllVariables Typ;
        //    for (Typ = AllVariables.Nitrate; Typ <= AllVariables.Phosphate; Typ++)
        //    {
        //        if (Typ == AllVariables.Nitrate)  NutrFrac = N_2_Org();
        //        else NutrFrac = P_2_Org();

        //        MBLoadRecord 2 = 1.MBLoadArray[Typ];
        //        // save for tox loss output & categorization
        //        LoadInKg = L * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr  // mg org/L // m3 // L/m3 // kg/mg // nutr / org

        //        2.BoundLoad[1.DerivStep] = 2.BoundLoad[1.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        if (En <= 0)
        //        {
        //            LoadInKg = (L + WI) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        else
        //        {
        //            LoadInKg = (L + WI + En) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        // kg nutr  // mg org/L // m3 // L/m3 // kg/mg // nutr / org
        //        2.TotOOSLoad[1.DerivStep] = 2.TotOOSLoad[1.DerivStep] + LoadInKg;
        //        2.LoadBiota[1.DerivStep] = 2.LoadBiota[1.DerivStep] + LoadInKg;
        //        if ((IsPhytoplankton()))
        //        {
        //            MorphRecord 3 = 1.Location.Morph;
        //            MBLossRecord 4 = 1.MBLossArray[Typ];
        //            // save for tox loss output & categorization
        //            // * OOSDischFrac
        //            LossInKg = WO * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;                     // 3/20/2014 remove OOSDischFrac
        //            // kg nutr // mg org/L // frac // m3 // L/m3 // kg/mg // nutr / org
        //            4.BoundLoss[1.DerivStep] = 4.BoundLoss[1.DerivStep] + LossInKg;
        //            // Loss from the modeled system
        //            if (En < 0)
        //            {
        //                // * OOSDischFrac
        //                LossInKg = (-En + WO) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //            }
        //            // loss from this segment
        //            // 3/20/2014 remove OOSDischFrac
        //            4.TotalNLoss[1.DerivStep] = 4.TotalNLoss[1.DerivStep] + LossInKg;
        //            4.TotalWashout[1.DerivStep] = 4.TotalWashout[1.DerivStep] + LossInKg;
        //            4.WashoutPlant[1.DerivStep] = 4.WashoutPlant[1.DerivStep] + LossInKg;
        //        }
        //        MBLayerRecord 5 = 1.MBLayerArray[Typ];
        //        // save for tox loss output & categorization
        //        LayerInKg = (SFE - STH) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr // mg org/L // frac // m3 // L/m3 // kg/mg // nutr / org

        //        5.NSink[1.DerivStep] = 5.NSink[1.DerivStep] + LayerInKg;
        //        5.NNetLayer[1.DerivStep] = 5.NNetLayer[1.DerivStep] + LayerInKg;
        //        LayerInKg = (TD + DiffUp + DiffDown) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr // mg org/L // frac // m3 // L/m3 // kg/mg // nutr / org
        //        5.NTurbDiff[1.DerivStep] = 5.NTurbDiff[1.DerivStep] + LayerInKg;
        //        5.NNetLayer[1.DerivStep] = 5.NNetLayer[1.DerivStep] + LayerInKg;
        //        // gN/cu m
        //        if ((!AQTSeg.PSetup.Internal_Nutrients))
        //        {
        //            // nitrogen fixation
        //            if ((Typ == AllVariables.Nitrate) && (IsFixingN()))
        //            {
        //                MBLoadRecord 6 = 1.MBLoadArray[Typ];
        //                // save for tox loss output & categorization
        //                LoadInKg = Pho * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //                // kg nutr // mg org/L // frac // m3 // L/m3 // kg/mg // nutr / org
        //                6.TotOOSLoad[1.DerivStep] = 6.TotOOSLoad[1.DerivStep] + LoadInKg;
        //                6.LoadFixation[1.DerivStep] = 6.LoadFixation[1.DerivStep] + LoadInKg;
        //            }
        //        }
        //    }
        //}

        // (************************************)
        // (*                                  *)
        // (*     DIFFERENTIAL EQUATIONS       *)
        // (*                                  *)
        // (************************************)
        public override void Derivative(ref double DB)
        {
            double Pho = 0;
            double R = 0;
            double Slg = 0;
            double ToxD = 0;
            double Ex = 0;
            double M = 0;
            double Pr = 0;
            double WO = 0;
            double WI = 0;
            double S = 0;
            double STH = 0;
            double SFE = 0;
            double TD = 0;
            double DiffUp = 0;
            double DiffDown = 0;
            double En = 0;
            double PeriScr = 0;
            double Sed2Me = 0;
            double Fl = 0;
            double L = 0;
            //bool Trackit;
            bool SurfaceFloater;

            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                T_SVType ToxLoop;
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Load", L);
                    SaveRate("Photosyn", Pho);
                    SaveRate("Respir", R);
                    SaveRate("Excret", Ex);
                    for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
                    {
                        if (AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) != null)
                            SaveRate("T" + ((int)ToxLoop - 1) + " Poisoned", MortRates.OrgPois[ToxInt(ToxLoop)]);

                    }
                    SaveRate("Other Mort", MortRates.OtherMort);
                    if (AQTSeg.GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) != null)
                        SaveRate("Salt Mort", MortRates.SaltMort);

                    SaveRate("Predation", Pr);
                    SaveRate("Washout", WO);
                    SaveRate("NetBoundary", L + WI - WO + En + DiffUp + DiffDown + TD);
                    if ((IsPeriphyton()))
                        SaveRate("SedtoPeri", Sed2Me);

                    SaveRate("Sediment", S);
                    if (IsPhytoplankton())
                        SaveRate("PeriScour", PeriScr);

                    if (!IsPhytoplankton())
                        SaveRate("Sloughing", Slg);

                    SaveRate("SinkToHypo", STH);
                    SaveRate("SinkFromEpi", SFE);
                    SaveRate("Lt_LIM", Lt_Limit);
                    SaveRate("N_LIM", N_Limit);
                    SaveRate("PO4_LIM", PO4_Limit);
                    SaveRate("CO2_LIM", CO2_Limit);
                    SaveRate("Nutr_LIM", Nutr_Limit);
                    SaveRate("Temp_LIM", Temp_Limit);
                    SaveRate("Chem_LIM", Chem_Limit);
                    if ((IsPeriphyton()))
                        SaveRate("Vel_LIM", Vel_Limit);

                    SaveRate("LowLt_LIM", LowLt_Limit);
                    SaveRate("HighLt_LIM", HighLt_Limit);
                    if (SurfaceFloater)
                        SaveRate("Floating", Fl);

                }
            }

            // --------------------------------------------------
            int ToxLoop;

            MortRates.OtherMort = 0;
            Sloughing = 0;
            MortRates.SaltMort = 0;

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                MortRates.OrgPois[ToxLoop] = 0;
            }

            //Trackit = (DB != -999);
            // signal to not write mass balance tracking

            SurfaceFloater = PAlgalRec.SurfaceFloating.Val;
            L = Loading;
            // WI = Washin();
            M = Mortality();  // Mortality calculated first for Chronic Effect Calculation
            Pho = Photosynthesis();
            R = Respiration();
            Ex = PhotoResp();
            Pr = Predation();
            if (Predation_Link != null) Pr = Predation_Link.ReturnLoad(AQTSeg.TPresent);

            WO = Washout();
            S = Sedimentation();
            // SinkToHypo is Calculated Here
            if (!SurfaceFloater)
            {
                STH = SinkToHypo;
                // SFE = SinkFromEp();  HMS Removed, no vert stratification
                Sed2Me = SedToMe();
            }
            //if (SurfaceFloater)
            //{
            //    // Fl = Floating();  HMS Removed, no vert stratification
            //}

            if (IsPhytoplankton())
            { PeriScr = PeriphytonSlough(); }

            if (!IsPhytoplankton())
            { Slg = CalcSlough(); }

            if (SloughEvent)
            { Slg = 0; }  // set precisely below  11/11/03

            // HMS Removed linked mode and stratification code and estuary-mode code

            DB = L + Pho - R - Ex - M - Pr - WO + WI - S + Sed2Me - STH + SFE + TD + En + DiffUp + DiffDown - ToxD - Slg + PeriScr + Fl;
            if (SloughEvent)
            {
                Slg = State - (SloughLevel / 2.0) + DB;
                // precisely slough to sloughlevel, 11/11/03
                Sloughing = Slg;
                DB = DB - Slg;
            }

            Derivative_WriteRates();
            // if (Trackit)  Derivative_TrackMB();

        }

    } // end TPlant


    public class TMacrophyte : TPlant
    {
        public TMacrophyte(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

        // =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        // Macrophyte Specific Code
        public double Breakage()
        {
            // (*******************************************)
            // * breakage term for macrophytes 10-5-2001 *
            // (*******************************************)

            const int Gradual = 20;
            // JSC via RAP 11-05-2002
            double Vel;
            Vel = AQTSeg.Velocity(PAlgalRec.PrefRiffle.Val, PAlgalRec.PrefPool.Val, false);

            if (Vel >= PAlgalRec.Macro_VelMax.Val) // 11/9/2001 constrain breakage so does not exceed "state" (concentration)
                return State;
            else
                return State * (Math.Exp((Vel - PAlgalRec.Macro_VelMax.Val) / Gradual));
            // mg/L d  // mg/L        // cm/s              // cm/s         // cm/s

        }

        public override double Mortality()
        {
            double Dead;
            double Pois;
            int ToxLoop;
            PlantRecord PR = PAlgalRec;
            Dead = (1.0 - Math.Exp(-PR.EMort.Val * (1.0 - AQTSeg.TCorr(PR.Q10.Val, PR.TRef.Val, PR.TOpt.Val, PR.TMax.Val)))) * State + (PR.KMort.Val * State);
            // emort is approximate maximum fraction killed per day by suboptimal temp.
            MortRates.OtherMort = Dead;
            MortRates.SaltMort = State * SalMort(PR.SalMin_Mort.Val, PR.SalMax_Mort.Val, PR.SalCoeff1_Mort.Val, PR.SalCoeff2_Mort.Val);
            Dead = Dead + MortRates.SaltMort;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                if ((AQTSeg.PSetup.UseExternalConcs.Val && (AQTSeg.GetStateVal(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) > 0))
                     || ((!AQTSeg.PSetup.UseExternalConcs.Val) && (AQTSeg.GetStateVal(NState, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) > 0)))
                {
                    Pois = Poisoned((T_SVType)ToxLoop + 2);
                    MortRates.OrgPois[ToxLoop] = Pois;
                    Dead = Dead + Pois;
                }
                else
                {
                    RedGrowth[ToxLoop] = 0;
                    // 5/3/2017 defaults for no effects if tox is zero; applied to macrophytes 10/12/2017
                    RedRepro[ToxLoop] = 0;
                    FracPhoto[ToxLoop] = 1;
                }
            }
            if (Dead > State)
            {
                Dead = State;
            }
            return Dead;
        }        // TMacrophyte.Mortality


        public override double Photosynthesis()
        {
            double LL;
            double NL;
            double Photosyn;
            double FracLit;
            double TCorrValue;
            double AggFP;
            double KCap;
            double SaltEffect;
            double KCapEffect;
            KCap = KCAP_in_g_m3();
            // g/m3
            KCapEffect = 1.0;  // 10/12/2014 KCap affects photosynthesis for macrophytes, not washout  added to Rel 3.2 4/26/2019
            if ((KCap > Consts.Tiny) && (State >= 0.9 * KCap))
            {
                KCapEffect = 1.0 - (State - 0.9 * KCap) / (0.1 * KCap);
                if (KCapEffect < 0)
                    KCapEffect = 0;

            }
            if ((PAlgalRec.PlantType.Val == "Bryophytes") || (MacroType == TMacroType.Freefloat))
                NL = NutrLimit();   // JSC 8-12-2002
            else
                NL = 1.0; // floating macrophytes are not subject to light limitation

            if ((MacroType == TMacroType.Benthic))
                LL = LtLimit(AQTSeg.PSetup.ModelTSDays.Val);
            else
                LL = 1.0;

            PlantRecord PR = PAlgalRec;
            TCorrValue = AQTSeg.TCorr(PR.Q10.Val, PR.TRef.Val, PR.TOpt.Val, PR.TMax.Val);
            AggFP = AggregateFracPhoto();
            Temp_Limit = TCorrValue;        // save for rate output  JSC 9-5-02
            Chem_Limit = AggFP;             // save for rate output  JSC 9-5-02

            Photosyn = PR.PMax.Val * NL * LL * TCorrValue * AggFP * State;
            // (g/sq m-d)  (1/d)  (these four terms unitless  )   g/sq m
            SaltEffect = AQTSeg.SalEffect(PR.SalMin_Phot.Val, PR.SalMax_Phot.Val, PR.SalCoeff1_Phot.Val, PR.SalCoeff2_Phot.Val);
            // frac littoral limitation applies to benthic and rooted floating macrophytes only
            FracLit = Location.FracLittoral(AQTSeg.ZEuphotic(), AQTSeg.Volume_Last_Step);
            if ((MacroType != TMacroType.Freefloat))
                return SaltEffect * FracLit * Photosyn * HabitatLimit * KCapEffect;
            else
                return SaltEffect * Photosyn * HabitatLimit * KCapEffect;
        }

        public override double Washout()  // HMS  multi-segment water flow logic disabled
        {
            double SegVolume;
            double Disch;
            double KCap;
            double KCapLimit;

            // change macrophyte washout for Integral, 10/12/2017
            if ((MacroType != TMacroType.Freefloat)) return 0;

            SegVolume = AQTSeg.Location.Morph.SegVolum;

            Disch = Location.Discharge;

            KCap = KCAP_in_g_m3();
            // g/m3
            KCapLimit = 1.0 - ((KCap - State) / KCap);
            return KCapLimit * Disch / SegVolume * State;
            // g/cu m-d  (fraction) (cu m/d)   (cu m)   (g/cu m)

            //         WashoutStep[AQTSeg.DerivStep] = result * AQTSeg.SegVol();
        }

        //// --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double NutrFrac;
        //    double LoadInKg;
        //    AllVariables Typ;
        //    for (Typ = AllVariables.Nitrate; Typ <= AllVariables.Phosphate; Typ++)
        //    {
        //        TStates 1 = AQTSeg;
        //        if (Typ == AllVariables.Nitrate)
        //        {
        //            NutrFrac = N_2_Org();
        //        }
        //        else
        //        {
        //            NutrFrac = P_2_Org();
        //        }
        //        LoadInKg = L * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;                  // save for tox loss output & categorization
        //        BoundLoad[1.DerivStep] = 2.BoundLoad[1.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        LoadInKg = (L + WI) * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr // mg org/L        // m3     // L/m3 // kg/mg     // nutr / org
        //        2.TotOOSLoad[1.DerivStep] = 2.TotOOSLoad[1.DerivStep] + LoadInKg;
        //        2.LoadBiota[1.DerivStep] = 2.LoadBiota[1.DerivStep] + LoadInKg;
        //        // bryophytes and freely floating macrophytes assimilate nutrients from water
        //        if (!(PAlgalRec.PlantType.Val == "Bryophytes") && !(MacroType == TMacroType.Freefloat))
        //        {
        //            MBLoadRecord 3 = 1.MBLoadArray[Typ];
        //            // save for tox loss output & categorization
        //            LoadInKg = Pho * 1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //            // kg nutr // mg org/L        // m3     // L/m3 // kg/mg     // nutr / org

        //            3.TotOOSLoad[1.DerivStep] = 3.TotOOSLoad[1.DerivStep] + LoadInKg;
        //            3.LoadPWMacro[1.DerivStep] = 3.LoadPWMacro[1.DerivStep] + LoadInKg;
        //        }
        //    }
        //}

        public override void Derivative(ref double DB)
        {
            double L = 0;
            double Pho = 0;
            double R = 0;
            double Ex = 0;
            double M = 0;
            double Pr = 0;
            double Br = 0;
            double WO = 0;
            double WI = 0;

            // --------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Load", L);
                    SaveRate("Photosyn", Pho);
                    SaveRate("Respir", R);
                    SaveRate("Excret", Ex);
                    SaveRate("Mort", M);
                    SaveRate("Predation", Pr);
                    SaveRate("Breakage", Br);
                    if (MacroType == TMacroType.Freefloat)
                    {
                        SaveRate("Washout", WO);
                        SaveRate("NetBoundary", L + WI - WO);
                    }
                    SaveRate("Lt_LIM", Lt_Limit);
                    if ((PAlgalRec.PlantType.Val == "Bryophytes") || (MacroType == TMacroType.Freefloat))
                    {
                        SaveRate("Nutr_LIM", Nutr_Limit);
                        SaveRate("N_LIM", N_Limit);
                        SaveRate("PO4_LIM", PO4_Limit);
                        SaveRate("CO2_LIM", CO2_Limit);
                    }
                    SaveRate("Temp_LIM", Temp_Limit);
                    SaveRate("Chem_LIM", Chem_Limit);
                    SaveRate("LowLt_LIM", LowLt_Limit);
                    SaveRate("HighLt_LIM", HighLt_Limit);
                }
            }
            // --------------------------------------------------

            L = Loading;
            if ((State < Consts.Tiny))
            {
                //  WI = WashIn();
                DB = L + WI;
            }
            else
            {
                M = Mortality();
                // Mortality calculated first for Chronic Effect Calculation
                Pho = Photosynthesis();
                R = Respiration();
                Ex = PhotoResp();
                Pr = Predation();
                if (Predation_Link != null) Pr = Predation_Link.ReturnLoad(AQTSeg.TPresent);

                Br = Breakage();
                if (MacroType == TMacroType.Freefloat)
                {
                    WO = Washout();
                    // WI = WashIn();
                    // note: no macrophytes are subject to diffusion between segments
                }
                DB = L + Pho - R - Ex - M - Pr - WO + WI - Br;
            }

            Derivative_WriteRates();
            //Derivative_TrackMB();

            // floating
            //if ((MacroType != TMacroType.Benthic) && (AQTSeg.VSeg == VerticalSegments.Hypolimnion) && (State > Consts.Tiny))
            //    throw new Exception("Floating Macrophytes in a Hypolimnion segment. " + "This is the result of an invalid initial condition in the Linked-Study setup.");

        }

    } // end TMacrophyte


    // internal nutrients in plants
    // ------------------   INTERNAL NUTRIENTS OBJECTS --------------------------


    public class T_N_Internal_Plant : TStateVariable
    {
        public override void SetToInitCond()
        {
            base.SetToInitCond();
            // initialize internal nutrients in ug/L  

            TPlant TP = AQTSeg.GetStatePointer(NState, T_SVType.StV, Layer) as TPlant; // associated plant

            if (SVType == T_SVType.NIntrnl)
                InitialCond = TP.InitialCond * TP.PAlgalRec.N2OrgInit.Val * 1000;
            else
                InitialCond = TP.InitialCond * TP.PAlgalRec.P2OrgInit.Val * 1000;
            // (ug N/L) =  (mg OM/L)   *              (gN/gOM)   (ug/mg)

            State = InitialCond;
        }


        public double Uptake()
        {
            double WC_Conc;
            double HalfSat;
            double HalfSatInternal;
            double Ratio;
            double MinRatio;
            double MaxUptake;
            TPlant CPlant;
            CPlant = AQTSeg.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
            PlantRecord PR = CPlant.PAlgalRec;
            if (SVType == T_SVType.NIntrnl)
            {
                if ((CPlant.IsFixingN())) return 0;   // N-fixation, not assimilation from the water column, uptake=0

                WC_Conc = AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
                HalfSat = PR.KN.Val;
                HalfSatInternal = PR.NHalfSatInternal.Val;
                Ratio = CPlant.N_2_Org();
                MinRatio = PR.Min_N_Ratio.Val;
                MaxUptake = PR.MaxNUptake.Val;
            }
            else
            {
                // SVType = PInternl
                WC_Conc = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
                HalfSat = PR.KPO4.Val;
                HalfSatInternal = PR.PHalfSatInternal.Val;
                Ratio = CPlant.P_2_Org();
                MinRatio = PR.Min_P_Ratio.Val;
                MaxUptake = PR.MaxPUptake.Val;
            }

            return MaxUptake * (WC_Conc / (HalfSat + WC_Conc)) * (HalfSatInternal / (HalfSatInternal + (Ratio - MinRatio))) * CPlant.State * 1e3;
            //  {ug/L d}    {g/g d}    {       unitless            }    {                        unitless                        }   {   mg / L  }  {ug/mg}

        }

        // --------------------------------------------------------------------------------------------------------------------------------------

        // HMS multi-segment interaction handled by HMS workflow.  ToxInCarrierWashin removed, NutrSegDiff removed, NutInCarrierWashin Removed

        // --------------------------------------------------------------------------------------------------------------------------------------

        public double Derivative_NToPhytoFromSlough()
        {
            AllVariables PlantLoop;
            TPlant PPl;
            double NPSlough;
            double j;
            NPSlough = 0;
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
                            j = -999;
                            // signal to not write mass balance tracking
                            PPl.Derivative(ref j);
                            // update sloughing
                            NPSlough = NPSlough + PPl.Sloughing * PPl.Nutr_2_Org(SVType) * 1e3 * (1.0 / 3.0);
                            // ug/L    // mg/L        // g/g                   // ug/mg        // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/Slough.
                        }
                    }
                }
            }
            return NPSlough;
        }

        public override void Derivative(ref double DB)
        {
            TPlant CP;
            CP = AQTSeg.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;

            // ----------------------------------------------------------------
            double Derivative_NSed2Me()
            {
                // Calculates nutrient transfer due to sedimentation of phytoplankton
                // to each periphyton compartment
                if (!CP.IsPeriphyton()) return 0;
                if (CP.PSameSpecies == AllVariables.NullStateVar) return 0;
                if (AQTSeg.GetStatePointer(CP.PSameSpecies, T_SVType.StV, T_SVLayer.WaterCol) == null) return 0;

                return CP.SedToMe() * CP.Nutr_2_Org(SVType) * 1e3;
                // ug/L   // mg/L            // g/g         // ug/mg
            }

            // ----------------------------------------------------------------

            //TPlant HypCp;
            //TPlant EpiCp;
            double STH = 0;
            double SFE = 0;
            double ToxD = 0;
            double Entr = 0;
            double Flt = 0;
            //          double TD = 0;
            //          double DiffUp = 0;
            //          double DiffDown = 0;
            double N2O = 0;
            double WashO = 0;
            double WashI = 0;
            double Lo = 0;
            double Predt = 0;
            double Mort = 0;
            double Sed = 0;
            double Uptk = 0;
            double Exc = 0;
            double Exc2 = 0;
            double Rsp = 0;
            double FixN = 0;
            //          double SegVolSave = 0;
            double Sed2Me = 0;
            double MacBrk = 0;
            double Slgh = 0;
            double Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            //          double LoadInKg = 0;
            bool SurfaceFloater;

            // ----------------------------------------------------------------
            void Derivative_WriteRates()
            {
                if ((AQTSeg.PSetup.SaveBRates.Val) && (SaveRates))
                {
                    ClearRate();
                    SaveRate("Loading", Lo);
                    SaveRate("Uptake", Uptk);
                    SaveRate("Mortality", Mort);
                    SaveRate("Respiration", Rsp);
                    SaveRate("Excretion", Exc);
                    SaveRate("Predation", Predt);

                    if ((NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae) && (!CP.IsPhytoplankton()))
                    {
                        SaveRate("ToxDislodge", ToxD);
                    }
                    if (!(NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
                    {
                        SaveRate("Washout", WashO);
                        //SaveRate("Washin", WashI);
                        SaveRate("SinkToHyp", STH);
                        SaveRate("SinkFromEp", SFE);
                        if (SurfaceFloater)
                        {
                            SaveRate("Floating", Flt);
                        }

                        SaveRate("NetBoundary", Lo + WashI - WashO + Entr);  //  DiffUp + DiffDown + TD;
                    }
                    if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro) && (((CP) as TMacrophyte).MacroType == TMacroType.Freefloat))
                    {
                        SaveRate("Washout", WashO);
                        //SaveRate("Washin", WashI);
                        SaveRate("NetBoundary", Lo + WashI - WashO + Entr);  //  DiffUp + DiffDown + TD;
                        SaveRate("Mac Break", MacBrk);
                    }
                    if ((CP.IsPeriphyton()))
                        SaveRate("Sed to Phyt", Sed2Me);

                    if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
                    {
                        SaveRate("Peri Slough", Slgh);
                        SaveRate("Sediment", Sed);
                    }

                    if ((SVType == T_SVType.NIntrnl) && (NState >= Consts.FirstBlGreen && NState <= Consts.LastBlGreen))
                        SaveRate("N Fixation", FixN);

                }
            }
            // ----------------------------------------------------------------

            SurfaceFloater = CP.PAlgalRec.SurfaceFloating.Val;
            if ((CP.State < Consts.Tiny))
            {
                DB = 0.0;
            }
            // macrophytes
            else if (NState >= Consts.FirstMacro && NState <= Consts.LastMacro)
            {
                N2O = CP.Nutr_2_Org(SVType) * 1e3;
                // (ug N / mg OM) (g N / g OM)      (ug/g)
                Lo = CP.Loading * N2O;
                // (ug N/L d) (mg OM/L d) (ug N/mg OM)
                WashO = CP.Washout() * N2O;
                //              WashoutStep[AQTSeg.DerivStep] = WashO * AQTSeg.SegVol();
                //              WashI = NutInCarrierWashin();
                Uptk = Uptake();
                Mort = CP.Mortality() * N2O;
                Predt = CP.Predation() * N2O;
                if (CP.Predation_Link != null) Predt = CP.Predation_Link.ReturnLoad(AQTSeg.TPresent) * N2O;

                Exc = CP.PhotoResp() * N2O;
                Exc2 = 0.09 * Math.Pow(1.07, (Temp - 20)) * State;  // Adapted from Ambrose 2006 -- JSC  7/12/2020

                Rsp = CP.Respiration() * N2O;
                MacBrk = ((CP) as TMacrophyte).Breakage() * N2O;
                // macrophytes
                DB = Lo + Uptk - (Predt + Mort + Exc + Exc2 + Rsp + MacBrk) - WashO + WashI;
            }
            else if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
            {
                N2O = CP.Nutr_2_Org(SVType) * 1e3;
                // (ug N / mg OM) (g N / g OM)      (ug/g)
                Lo = CP.Loading * N2O;
                // (ug N/L d) (mg OM/L d) (ug N/mg OM)
                WashO = CP.Washout() * N2O;
                //              WashoutStep[AQTSeg.DerivStep] = WashO * AQTSeg.SegVol();
                //              HMS EstuarySegment code omitted

                //              WashI = NutInCarrierWashin();
                STH = CP.SinkToHypo * N2O;

                //if (VSeg == VerticalSegments.Hypolimnion)
                //{
                //    EpiCp = EpiSegment.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol);  // Refinement 10-20-2002 JSC
                //    SFE = EpiCp.SinkToHypo * EpiCp.Nutr_2_Org(SVType) * 1e3;
                //}

                //if (SurfaceFloater)
                //  if (AQTSeg.Stratified)
                //    {
                //        Flt = CP.Floating() * N2O;
                //        TStates 4 = AQTSeg;
                //        if (4.VSeg == VerticalSegments.Epilimnion)
                //        {
                //            HypCp = 4.HypoSegment.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol);
                //            Flt = HypCp.Floating() * HypCp.Nutr_2_Org(SVType) * 1e3;
                //        }
                //    }

                Predt = CP.Predation() * N2O;
                if (CP.Predation_Link != null) Predt = CP.Predation_Link.ReturnLoad(AQTSeg.TPresent) * N2O;

                Mort = CP.Mortality() * N2O;
                Exc = CP.PhotoResp() * N2O;
                Exc2 = 0.09 * Math.Pow(1.07, (Temp - 20)) * State;  // Adapted from Ambrose 2006 -- JSC  7/12/2020

                Rsp = CP.Respiration() * N2O;

                if ((CP.IsFixingN()) && (SVType == T_SVType.NIntrnl))
                {
                    //    //MBLoadRecord MBLR = 5.MBLoadArray[AllVariables.Nitrate];
                    FixN = CP.PAlgalRec.MaxNUptake.Val * CP.State * 1e3;
                    //    // ug/L d          // g/g d     // mg / L // ug/mg
                    //    SegVolSave = AQTSeg.SegVol();

                    //    LoadInKg = FixN * SegVolSave * 1000.0 * 1e-9;   // save for nutrient MB & categorization
                    //    // kg nutr   // ug/L   // m3    // L/m3 // kg/ug

                    //    //MBLR.TotOOSLoad[5.DerivStep] = MBLR.TotOOSLoad[5.DerivStep] + LoadInKg;
                    //    //MBLR.LoadFixation[5.DerivStep] = MBLR.LoadFixation[5.DerivStep] + LoadInKg;
                }

                ToxD = CP.ToxicDislodge() * N2O;
                Sed = CP.Sedimentation() * N2O;   // plant sedimentation
                Sed2Me = Derivative_NSed2Me();
                if ((CP.IsPeriphyton())) Slgh = CP.Sloughing * N2O;
                else Slgh = -Derivative_NToPhytoFromSlough();
                Uptk = Uptake();
                // algae
                DB = Lo + Uptk + WashI - (WashO + Predt + Mort + Sed + Exc + Exc2 + Rsp + ToxD + Slgh) - STH + SFE + Flt + Sed2Me + Entr + FixN;
                // algae
            }
            // Phytoplankton are subject to currents , diffusion & TD
            // HMS eliminated turbulent diffusion and linked-segment diffusion as irrelevant to 0D Model

            Derivative_WriteRates();
        }


    } // end T_N_Internal_Plant

} // namespace