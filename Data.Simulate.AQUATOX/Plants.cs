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
        public string PlantName;
        public string PlantType;
        public string ToxicityRecord;
        public double EnteredLightSat;
        public string XLightSat;
        public double KPO4;
        public string XKPO4;
        public double KN;        // n half Sat
        public string XKN;
        public double KCarbon;
        public string XKCarbon;
        public double Q10;
        public string XQ10;
        public double TOpt;
        public string XTOpt;
        public double TMax;
        public string XTMax;
        public double TRef;
        public string XTRef;
        public double PMax;
        public string XPMax;
        public double KResp;
        public string XKResp;
        public double KMort;
        public string XKMort;
        public double EMort;
        public string XEMort;
        public double KSed;
        public string XKSed;
        public double ESed;
        public string XESed;
        public double P2OrgInit;
        public string XP2Org;
        public double N2OrgInit;
        public string XN2Org;
        public double ECoeffPhyto;
        public string XECoeffPhyto;
        public double CarryCapac;
        public string XCarryCapac;
        public double Red_Still_Water;
        public string XRed_Still_Water;
        public string Macrophyte_Type;
        public double Macro_Drift;
        public string Taxonomic_Type;
        public double PrefRiffle;
        public string XPrefRiffle;
        public double PrefPool;
        public string XPrefPool;
        public double FCrit;
        public string XFCrit;
        public double Macro_VelMax;
        public string XVelMax;

        public double KSedTemp;
        public string XKSedTemp;
        public double KSedSalinity;
        public string XKSedSalinity;

        // Salinity & Photosynthesis
        // minimum salinity tolerance 0/00
        // max salinity tolerance 0/00
        public double Salmin_Phot;
        public double SalMax_Phot;
        public double Salcoeff1_Phot;
        public double Salcoeff2_Phot;     // unitless
        public string XSalinity_Phot;
        // Salinity & Mortility
        // minimum salinity tolerance 0/00
        // max salinity tolerance 0/00
        public double Salmin_Mort;
        public double SalMax_Mort;
        public double Salcoeff1_Mort;
        public double Salcoeff2_Mort;        // unitless
        public string XSalinity_Mort;
        public double Wet2Dry;
        public string XWet2Dry;
        public double Resp20;
        public string XResp20;
        public double PctSloughed;
        public string XPctSloughed;
        public bool UseAdaptiveLight;
        public double MaxLightSat;
        public string XMaxLightSat;
        public double MinLightSat;
        public string XMinLightSat;
        public string ScientificName;
        public double PlantFracLipid;
        public string XPlantFracLipid;
        public bool SurfaceFloating;
        public double NHalfSatInternal;
        public string XNHalfSatInternal;
        public double PHalfSatInternal;
        public string XPHalfSatInternal;
        public double MaxNUptake;
        public string XMaxNUptake;
        public double MaxPUptake;
        public string XMaxPUptake;
        public double Min_N_Ratio;
        public string XMin_N_Ratio;
        public double Min_P_Ratio;
        public string XMin_P_Ratio;
        public double Plant_to_Chla;
        // g carbon/g chlorophyll
        public string XPlant_to_Chla;
    } // end PlantRecord

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

            PctRun = 100 - AQTSeg.Location.Locale.PctRiffle - AQTSeg.Location.Locale.PctPool;
            PrefRun = 100 - PAlgalRec.PrefRiffle - PAlgalRec.PrefPool;
            HabitatAvail = 0;
            if (PAlgalRec.PrefRiffle > 0)   HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctRiffle / 100;  

            if (PAlgalRec.PrefPool > 0)     HabitatAvail = HabitatAvail + AQTSeg.Location.Locale.PctPool / 100;

            if (PrefRun > 0)                HabitatAvail = HabitatAvail + PctRun / 100;

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
            if (PAlgalRec.PlantType == "Macrophytes")
            {
                if (PAlgalRec.Macrophyte_Type == "benthic")
                {
                    MacroType = TMacroType.Benthic;
                }
                else if (PAlgalRec.Macrophyte_Type == "free-floating")
                {
                    MacroType = TMacroType.Freefloat;
                }
                else if (PAlgalRec.Macrophyte_Type == "rooted floating")
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
            DataName = PAlgalRec.ToxicityRecord.ToLower();

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
                        throw new Exception("Error!  " + PAlgalRec.PlantName + " uses the toxicity record \"" + DataName + "\" which is not found in chemical " + TT.ChemRec.ChemName + "\'s Plant toxicity data.  Study cannot be executed.");

                    PTR = TT.Chem_Plant_Tox[FoundToxIndx];
                    Plant_Tox[ToxLoop] = PTR;
                }
            }
        }

        // ------------------------------------------------------------------------

        public override double WetToDry()
        {
            return PAlgalRec.Wet2Dry;
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
            if ((PAlgalRec.PlantType == "Phytoplankton"))
            {
                // g/cu m-d   1/d    unitless     g/cu m
                Photosyn = PAlgalRec.PMax * PProdLimit() * State;
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
                Photosyn = PAlgalRec.PMax * PPL * Vel_Limit * State * Substrate;
                // g/m3-d      1/d   unitless  unitless   g/m3    unitless
            }
            Salteffect = AQTSeg.SalEffect(PAlgalRec.Salmin_Phot, PAlgalRec.SalMax_Phot, PAlgalRec.Salcoeff1_Phot, PAlgalRec.Salcoeff2_Phot);
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
            LightStress = 1.0 - LtLimit(AQTSeg.PSetup.ModelTSDays);
            if (LightStress < Consts.Small)
            {
                LightStress = 0.0;
            }
            // RAP, 9/7/95, KResp should be mult. by LightStress - see Collins et al., 1985
            // + KStress
            Excrete = Photosynthesis() * (PAlgalRec.KResp * LightStress);
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
            return PAlgalRec.Resp20 * Math.Pow(1.045, (Temp - 20)) * State;
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
            if (WaterTemp > PAlgalRec.TMax)
            {
                // 6-12-2006 Fixed Logic
                ExcessT = 1.0;
            }
            else
            {
                ExcessT = Math.Exp(WaterTemp - PAlgalRec.TMax) / 2.0;
            }
            Stress = (1.0 - Math.Exp(-PAlgalRec.EMort * (1.0 - (NutrLimit() * LtLimit(AQTSeg.PSetup.ModelTSDays)))));
            // emort is approximate maximum fraction killed
            // per day by nutrient and light limitation
            Dead = (PAlgalRec.KMort + ExcessT + Stress) * State;
            // g/cu m-d               (        g/g-d           )   g/cu m
            //MortRates.OtherMort = Dead;
            //MortRates.SaltMort = State * SalMort(PAlgalRec.Salmin_Mort, PAlgalRec.SalMax_Mort, PAlgalRec.Salcoeff1_Mort, PAlgalRec.Salcoeff2_Mort);
            //Dead = Dead + MortRates.SaltMort;
            // g/cu m-d                g/cu m-d
            Setup_Record SR = AQTSeg.PSetup;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                RedGrowth[ToxLoop] = 0;     // 5/3/2017 defaults for no effects if tox is zero
                RedRepro[ToxLoop] = 0;
                FracPhoto[ToxLoop] = 1;

                if (SR.UseExternalConcs) TSV = AQTSeg.GetStatePointer(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol);
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
            return (PAlgalRec.PlantType == "Periphyton");
        }

        public bool IsPhytoplankton()
        {
            return (PAlgalRec.PlantType == "Phytoplankton");
        }

        public bool IsLinkedPhyto()
        {
            AllVariables PLoop;
            TPlant PPeri;
            if (PAlgalRec.PlantType != "Phytoplankton") return false;
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
            if (PAlgalRec.SurfaceFloating)
            {
                return 0;
            }
            SedAccel = Math.Exp(PAlgalRec.ESed * (1.0 - PProdLimit()));
            // Collins & Park, 1989
            DensFactor = AQTSeg.DensityFactor(PAlgalRec.KSedTemp, PAlgalRec.KSedSalinity);

            //if (AQTSeg.EstuarySegment)
            //{
            //    // g/cu m-d   m/d    m     unitless   unitless   g/cu m
            //    Sink = PAlgalRec.KSed / Thick * DensFactor * SedAccel * State;
            //}
            //else
            {
                // unitless       unitless       unitless
                if (AQTSeg.MeanDischarge < Consts.Tiny)
                {
                    // g/cu m-d   m/d    m      unitless   g/cu m    unitless
                    Sink = PAlgalRec.KSed / Thick * SedAccel * State * DensFactor;
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
                    Sink = PAlgalRec.KSed / Thick * SedAccel * State * Decel * DensFactor;
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
            if ((!IsPhytoplankton())&&(Plant_Tox!= null))
            {
                for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
                {
                    if (Plant_Tox[ToxLoop] != null)
                    {
                        if (Plant_Tox[ToxLoop].EC50_dislodge > Consts.Tiny)
                        {
                            ToxLevel = AQTSeg.GetState(AllVariables.H2OTox, T_SVType.OrgTox1+ToxLoop, T_SVLayer.WaterCol);
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
            LtL = LtLimit(AQTSeg.PSetup.ModelTSDays);
            Suboptimal = Nutr * LtL * AQTSeg.TCorr(PAlgalRec.Q10, PAlgalRec.TRef, PAlgalRec.TOpt, PAlgalRec.TMax) * 20;        // 5 to 20 RAP 1-24-05
            if (Suboptimal > 1)  Suboptimal = 1;

            // uses nutrlimit calculated at the beginning of this time-step
            if ((!IsPhytoplankton()))
            {
                // not floating, calculate slough & scour
                Biovol_Fil = State / 8.57E-9 * AQTSeg.DynamicZMean();
                // mm3/mm2
                Biovol_Dia = State / 2.08E-9 * AQTSeg.DynamicZMean();
                // mm3/mm2
                AvgVel = AQTSeg.Velocity(PAlgalRec.PrefRiffle, PAlgalRec.PrefPool, true) * 0.01;   // Avg. Velocity exposure of this organism
          //    {m/s}            {cm/s}                                           {avg}   {m/cm}

                DailyVel = AQTSeg.Velocity(PAlgalRec.PrefRiffle, PAlgalRec.PrefPool, false) * 0.01;  // Avg. Velocity exposure of this organism
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
                        DragForce = Rho * DragCoeff * Math.Pow(DailyVel, 2) * Math.Pow(Biovol_Dia * UnitArea, (2.0/3.0)) * 1E-6;
                        // kg m/s2  kg/m3 unitless                m/s                    mm3/mm2       mm2                 m2/mm2
                        // 3/19/2013 fcrit of zero means no scour

                        if ((Adaptation > 0) && (PAlgalRec.FCrit > Consts.Tiny) && (State > MINBIOMASS) && (DragForce > Suboptimal * PAlgalRec.FCrit * Adaptation))
                        {   // frac living
                            SloughEvent = true;
                            SloughLevel = State * (1.0 - (PAlgalRec.PctSloughed / 100.0));

                            //AQTSeg.ProgData.SloughDia = true;  update progress bar, N/A for HMS
                            //AQTSeg.ProgData.PeriVis = true;
                        }
                    }
                    else
                    {
                        // filamentous (includes greens and blgreens and should not be confused
                        // with filamentous phytoplankton)
                        DragForce = Rho * DragCoeff * Math.Pow(DailyVel, 2.0) * Math.Pow((Biovol_Fil * UnitArea), (2.0 / 3.0)) * 1E-6;
                        // kg m/s2  kg/m3 unitless                m/s                    mm3/mm2       mm2             m2/mm2

                        // 3/19/2013 fcrit of zero means no scour
                        if ((Adaptation > 0) && (PAlgalRec.FCrit > Consts.Tiny) && (State > MINBIOMASS) && (DragForce > Suboptimal * PAlgalRec.FCrit * Adaptation))
                        {
                            SloughEvent = true;
                            SloughLevel = State * (1.0 - (PAlgalRec.PctSloughed / 100.0));

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
            return PAlgalRec.CarryCapac * Location.Locale.SurfArea / AQTSeg.Volume_Last_Step;   //  11/3/2014 replaced static zmean with more consistent conversion
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
                                 
            if (!PAlgalRec.UseAdaptiveLight)  return PAlgalRec.EnteredLightSat;
            LightHistory();
            LightSatCalc = (0.7 * LightHist[0] + 0.2 * LightHist[1] + 0.1 * LightHist[2]);    // 8-20-07

            // ensure calculation is within limits, 8-28-2007
            if (LightSatCalc < PAlgalRec.MinLightSat) LightSatCalc = PAlgalRec.MinLightSat;
            if (LightSatCalc > PAlgalRec.MaxLightSat) LightSatCalc = PAlgalRec.MaxLightSat;

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

            if (AQTSeg.PSetup.ModelTSDays) LightVal = PL.DailyLight;
            else LightVal = PL.HourlyLight;

            if ((EX * DBott) > 20.0) LD = 1.0;
            else
            {
                if (AQTSeg.PSetup.ModelTSDays) LightCorr = 1.0;
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

            if (AQTSeg.Location.Locale.UseBathymetry) DB = AQTSeg.Location.Locale.ZMax;
            else DB = AQTSeg.DynamicZMean();

            if ((IsPhytoplankton()))
            {
                if ((AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) <= AQTSeg.Ice_Cover_Temp()) && (AQTSeg.Location.MeanThick > 2.0))
                {
                    DB = 2.0;  // algae in top 2 m under ice
                }
                
                // Account for buoyancy due to gas vacuoles
                if ((PAlgalRec.SurfaceFloating) && (DB > 1.0))    // Removed 0.2, 1-13-2005 RAP, 2-2007                     // 3/9/2012 surfacefloating variable
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
                        PeriExtinction = PeriExtinction + PPhyt.PAlgalRec.ECoeffPhyto * AQTSeg.GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol);
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
            if ((PAlgalRec.PlantType == "Macrophytes") && (MacroType == TMacroType.Freefloat))  return LL;  

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

                E = AQTSeg.Extinct(false, true, true, PAlgalRec.SurfaceFloating,  0);
                // periphyton are not included because "PeriphytExt" handles periphyton extinction


                if ((!IsPhytoplankton()))                  // 3/9/2012 bl-greens to surfacefloater  // i.e. 'Periphyton', 'Macrophytes', or 'Bryophytes'
                {
                    PeriExt = LtLimit_PeriphytExt();
                }
                else
                {
                    PeriExt = 1.0;
                }
                if (AQTSeg.PSetup.ModelTSDays)
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
                if ((AQTSeg.PSetup.ModelTSDays))  // if daily time step is specified
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
            if (AQTSeg.PSetup.Internal_Nutrients)
            {
                P2O = P_2_Org();
                if (P2O > Consts.Tiny)
                {
                    // Internal nutrientsP Limit
                    PLimit = (1.0 - (PAlgalRec.Min_P_Ratio / P2O));
                }
                else PLimit = 0.0;
            }
            else
            {
                // external nutrients
                if ((AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) + PAlgalRec.KPO4) == 0)
                {
                    PLimit = 1.0;
                }
                else
                {
                    PLimit = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) / (AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol) + PAlgalRec.KPO4);
                }
                // unitless  P/PO4           gPO4/cu m    P/PO4            gPO4/cu m            gP/cu m
            }  // external nutrients


            if (PLimit < Consts.Small)
            {
                PLimit = 0.0;      // RAP, 9/11/95 Tiny -> Small
            }

            return PLimit;
        }

        // phoslimit
        public bool Is_Pcp_CaCO3()
        {
            // Is this plant precipitating CaCO3?
            // JSC, From 8.25 to 7.5 on 7/2/2009
            return (AQTSeg.GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol) >= 7.5) && 
                !((PAlgalRec.PlantType == "Bryophytes") || (NState == AllVariables.OtherAlg1) || (NState == AllVariables.OtherAlg2));
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
                // RAP, 9/9/95 replaced constant KCO2 with PAlgalRec^.KCarbon
                CLimit = C2CO2 * CDioxide / (C2CO2 * CDioxide + PAlgalRec.KCarbon);
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
            if (AQTSeg.PSetup.Internal_Nutrients && (State > Consts.Tiny))
            {
                if (AQTSeg.GetStatePointer(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) != null)
                {
                    return AQTSeg.GetState(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) / State * 1e-3;
                 // (g/g OM)   =   (ug N /L)                                             /(mg OM/L)*(mg/ug)

                }
                else
                {
                    return PAlgalRec.N2OrgInit;
                }
                // rooted macrophyte
            }
            else
            {
                return PAlgalRec.N2OrgInit;
            }
        }

        public double P_2_Org()
        {
            if (AQTSeg.PSetup.Internal_Nutrients && (State > Consts.Tiny))
            {
                if (AQTSeg.GetStatePointer(NState, T_SVType.PIntrnl, T_SVLayer.WaterCol) != null)
                {
                    return AQTSeg.GetState(NState, T_SVType.PIntrnl, T_SVLayer.WaterCol) / State * 1e-3;
                    // ug N /L                                                          // mg/L  // mg/ug

                }
                else
                {
                    return PAlgalRec.P2OrgInit;
                }
                // rooted macrophyte
            }
            else
            {
                return PAlgalRec.P2OrgInit;
            }
        }

        public bool IsFixingN()
        {
            double Nitrogen;
            double InorgP;
            double NtoP;

            if (!(NState >= Consts.FirstBlGreen && NState <= Consts.LastBlGreen)) return false;    // IsFixingN true for cyanobacteria only
            
            if (AQTSeg.GetStatePointer(NState, T_SVType.NIntrnl, T_SVLayer.WaterCol) != null)      // internal nutrients option 3/18/2014
                return (N_2_Org() < 0.5 * PAlgalRec.NHalfSatInternal);
            
            if (!AQTSeg.PSetup.NFix_UseRatio)  // added option 3/19/2010
            {
                Nitrogen = AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
                return (Nitrogen < 0.5 * PAlgalRec.KN);   // Official "Release 3" code
            }
            else                 
            {
                // 12-16-2009 N Fixing Option
                InorgP = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
                // Avoid Divide by Zero
                if (InorgP > Consts.Tiny)
                {
                    NtoP = (AQTSeg.GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol) + AQTSeg.GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol)) / InorgP;
                    if ((NtoP < AQTSeg.PSetup.NtoPRatio))  return true; // If inorganic N over Inorganic P ratio is less than NtoPRatio (Default of 7) then cyanobacteria fix nitrogen
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
            else if (AQTSeg.PSetup.Internal_Nutrients)
            {
                N2O = N_2_Org();
                if (N2O > Consts.Tiny)
                {
                    // Internal nutrients N Limit
                    NL = (1.0 - (PAlgalRec.Min_N_Ratio / N2O));
                }
                else
                {
                    NL = 0.0;
                }
                // with PAlgalRec
            }
            else
            {
                // external nutrients
                if ((Nitrogen + PAlgalRec.KN) == 0)
                {
                    NL = 1;
                }
                else
                {
                    NL = Nitrogen / (Nitrogen + PAlgalRec.KN);
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
            TCorrValue = AQTSeg.TCorr(PAlgalRec.Q10, PAlgalRec.TRef, PAlgalRec.TOpt, PAlgalRec.TMax);
            LL = LtLimit(AQTSeg.PSetup.ModelTSDays);
            NL = NutrLimit();
            AggFP = AggregateFracPhoto();
            Temp_Limit = TCorrValue;  // save for rate output  JSC 9-5-02
            Chem_Limit = AggFP;       // save for rate output  JSC 9-5-02
            PL = LL * NL * TCorrValue * AggFP;
            // all unitless
            if (PL > 1.0)  PL = 1.0; // make sure it is truly a reduction factor

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
                Vel = AQTSeg.Velocity(PAlgalRec.PrefRiffle, PAlgalRec.PrefPool, false);
                // cm/s
                VL = Math.Min(1.0, (PAlgalRec.Red_Still_Water + VelCoeff * Vel / (1.0 + VelCoeff * Vel)));
                // frac                 unitless             // unitless  // m/s    // unitless  // m/s
            }
            else
            {
                VL = PAlgalRec.Red_Still_Water;
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

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    T_SVType ToxLoop;
        //    Setup_Record 1 = AQTSeg.PSetup;
        //    if ((1.SaveBRates || 1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", L);
        //        SaveRate("Photosyn", Pho);
        //        SaveRate("Respir", R);
        //        SaveRate("Excret", Ex);
        //        for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
        //        {
        //            if (AQTSeg.GetStatePointer(NState, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) != null)
        //            {
        //                SaveRate(Consts.PrecText(ToxLoop) + " Poisoned", MortRates.OrgPois[ToxLoop]);
        //            }
        //        }
        //        SaveRate("Other Mort", MortRates.OtherMort);
        //        if (AQTSeg.GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) != -1)
        //        {
        //            SaveRate("Salt Mort", MortRates.SaltMort);
        //        }
        //        SaveRate("Predation", Pr);
        //        SaveRate("Washout", WO);
        //        if (AQTSeg.LinkedMode)
        //        {
        //            SaveRate("Washin", WI);
        //        }
        //        SaveRate("NetBoundary", L + WI - WO + En + DiffUp + DiffDown + TD);
        //        if ((IsPeriphyton()))
        //        {
        //            SaveRate("SedtoPeri", Sed2Me);
        //        }
        //        SaveRate("Sediment", S);
        //        if (IsPhytoplankton())
        //        {
        //            SaveRate("PeriScour", PeriScr);
        //            if (AQTSeg.EstuarySegment)
        //            {
        //                SaveRate("Entrainment", En);
        //            }
        //            if (!AQTSeg.LinkedMode)
        //            {
        //                SaveRate("TurbDiff", TD);
        //            }
        //            else
        //            {
        //                // If not AQTSeg.CascadeRunning then
        //                SaveRate("DiffUp", DiffUp);
        //                SaveRate("DiffDown", DiffDown);
        //            }
        //        }
        //        if (!IsPhytoplankton())
        //        {
        //            SaveRate("Sloughing", Slg);
        //        }
        //        SaveRate("SinkToHypo", STH);
        //        SaveRate("SinkFromEpi", SFE);
        //        SaveRate("Lt_LIM", Lt_Limit);
        //        SaveRate("N_LIM", N_Limit);
        //        SaveRate("PO4_LIM", PO4_Limit);
        //        SaveRate("CO2_LIM", CO2_Limit);
        //        SaveRate("Nutr_LIM", Nutr_Limit);
        //        SaveRate("Temp_LIM", Temp_Limit);
        //        SaveRate("Chem_LIM", Chem_Limit);
        //        if ((IsPeriphyton()))
        //        {
        //            SaveRate("Vel_LIM", Vel_Limit);
        //        }
        //        SaveRate("LowLt_LIM", LowLt_Limit);
        //        SaveRate("HighLt_LIM", HighLt_Limit);
        //        if (SurfaceFloater)
        //        {
        //            SaveRate("Floating", Fl);
        //        }
        //    }
        //}

        // --------------------------------------------------
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
            double R= 0;
            double Slg= 0;
            double ToxD= 0;
            double Ex= 0;
            double M= 0;
            double Pr= 0;
            double WO= 0;
            double WI= 0;
            double S= 0;
            double STH= 0;
            double SFE= 0;
            double TD= 0;
            double DiffUp= 0;
            double DiffDown= 0;
            double En= 0;
            double PeriScr= 0;
            double Sed2Me= 0;
            double Fl= 0;
            //bool Trackit;
            bool SurfaceFloater;
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

            SurfaceFloater = PAlgalRec.SurfaceFloating;
            double L = Loading;
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
            {   PeriScr = PeriphytonSlough();   }

            if (!IsPhytoplankton())
            {   Slg = CalcSlough();             }

            if (SloughEvent)
            {  Slg = 0; }  // set precisely below  11/11/03

            // HMS Removed linked mode and stratification code and estuary-mode code

            DB = L + Pho - R - Ex - M - Pr - WO + WI - S + Sed2Me - STH + SFE + TD + En + DiffUp + DiffDown - ToxD - Slg + PeriScr + Fl;
            if (SloughEvent)
            {
                Slg = State - (SloughLevel / 2.0) + DB;
                // precisely slough to sloughlevel, 11/11/03
                Sloughing = Slg;
                DB = DB - Slg;
            }

            // Derivative_WriteRates();
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
            Vel = AQTSeg.Velocity(PAlgalRec.PrefRiffle, PAlgalRec.PrefPool, false);
            
            if (Vel >= PAlgalRec.Macro_VelMax) // 11/9/2001 constrain breakage so does not exceed "state" (concentration)
                return State;
            else
                return State * (Math.Exp((Vel - PAlgalRec.Macro_VelMax) / Gradual));
              // mg/L d  // mg/L        // cm/s              // cm/s         // cm/s

        }

        public override double Mortality()
        {
            double Dead;
            double Pois;
            int ToxLoop;
            PlantRecord PR = PAlgalRec;
            Dead = (1.0 - Math.Exp(-PR.EMort * (1.0 - AQTSeg.TCorr(PR.Q10, PR.TRef, PR.TOpt, PR.TMax)))) * State + (PR.KMort * State);
            // emort is approximate maximum fraction killed per day by suboptimal temp.
            MortRates.OtherMort = Dead;
            MortRates.SaltMort = State * SalMort(PR.Salmin_Mort, PR.SalMax_Mort, PR.Salcoeff1_Mort, PR.Salcoeff2_Mort);
            Dead = Dead + MortRates.SaltMort;
            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                if ((AQTSeg.PSetup.UseExternalConcs && (AQTSeg.GetStateVal(AllVariables.H2OTox, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) > 0)) 
                     || ((!AQTSeg.PSetup.UseExternalConcs) && (AQTSeg.GetStateVal(NState, T_SVType.OrgTox1 + ToxLoop, T_SVLayer.WaterCol) > 0)))
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
                {
                    KCapEffect = 0;
                }
            }
            if ((PAlgalRec.PlantType == "Bryophytes") || (MacroType == TMacroType.Freefloat))
            {
                NL = NutrLimit();   // JSC 8-12-2002
            }
            else
            {
                NL = 1.0; // floating macrophytes are not subject to light limitation
            }
            
            if ((MacroType == TMacroType.Benthic))
            {
                LL = LtLimit(AQTSeg.PSetup.ModelTSDays);
            }
            else
            {
                LL = 1.0;
            }
            PlantRecord PR = PAlgalRec;
            TCorrValue = AQTSeg.TCorr(PR.Q10, PR.TRef, PR.TOpt, PR.TMax);
            AggFP = AggregateFracPhoto();
            Temp_Limit = TCorrValue;        // save for rate output  JSC 9-5-02
            Chem_Limit = AggFP;             // save for rate output  JSC 9-5-02

            Photosyn = PR.PMax * NL * LL * TCorrValue * AggFP * State;
          // (g/sq m-d)  (1/d)  (these four terms unitless  )   g/sq m
            SaltEffect = AQTSeg.SalEffect(PR.Salmin_Phot, PR.SalMax_Phot, PR.Salcoeff1_Phot, PR.Salcoeff2_Phot);
            // frac littoral limitation applies to benthic and rooted floating macrophytes only
            FracLit = Location.FracLittoral(AQTSeg.ZEuphotic(), AQTSeg.Volume_Last_Step);
            if ((MacroType != TMacroType.Freefloat))
            {
                return SaltEffect * FracLit * Photosyn * HabitatLimit * KCapEffect;
            }
            else
            {
                return SaltEffect * Photosyn * HabitatLimit * KCapEffect;
            }
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


        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    Setup_Record 1 = AQTSeg.PSetup;
        //    if ((1.SaveBRates || 1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", L);
        //        SaveRate("Photosyn", Pho);
        //        SaveRate("Respir", R);
        //        SaveRate("Excret", Ex);
        //        SaveRate("Mort", M);
        //        SaveRate("Predation", Pr);
        //        SaveRate("Breakage", Br);
        //        if (MacroType == TMacroType.Freefloat)
        //        {
        //            SaveRate("Washout", WO);
        //            if (AQTSeg.LinkedMode)
        //            {
        //                SaveRate("Washin", WI);
        //            }
        //            SaveRate("NetBoundary", L + WI - WO);
        //        }
        //        SaveRate("Lt_LIM", Lt_Limit);
        //        if ((PAlgalRec.PlantType == "Bryophytes") || (MacroType == TMacroType.Freefloat))
        //        {
        //            SaveRate("Nutr_LIM", Nutr_Limit);
        //            SaveRate("N_LIM", N_Limit);
        //            SaveRate("PO4_LIM", PO4_Limit);
        //            SaveRate("CO2_LIM", CO2_Limit);
        //        }
        //        SaveRate("Temp_LIM", Temp_Limit);
        //        SaveRate("Chem_LIM", Chem_Limit);
        //        SaveRate("LowLt_LIM", LowLt_Limit);
        //        SaveRate("HighLt_LIM", HighLt_Limit);
        //    }
        //}

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
        //        if (!(PAlgalRec.PlantType == "Bryophytes") && !(MacroType == TMacroType.Freefloat))
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
            double L;
            double Pho = 0;
            double R = 0;
            double Ex = 0;
            double M = 0;
            double Pr = 0;
            double Br = 0;
            double WO = 0;
            double WI = 0;
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

            //Derivative_WriteRates();
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
                    InitialCond = TP.InitialCond * TP.PAlgalRec.N2OrgInit * 1000;
                else
                    InitialCond = TP.InitialCond * TP.PAlgalRec.P2OrgInit * 1000;
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
                HalfSat = PR.KN;
                HalfSatInternal = PR.NHalfSatInternal;
                Ratio = CPlant.N_2_Org();
                MinRatio = PR.Min_N_Ratio;
                MaxUptake = PR.MaxNUptake;
            }
            else
            {
                // SVType = PInternl
                WC_Conc = AQTSeg.GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
                HalfSat = PR.KPO4;
                HalfSatInternal = PR.PHalfSatInternal;
                Ratio = CPlant.P_2_Org();
                MinRatio = PR.Min_P_Ratio;
                MaxUptake = PR.MaxPUptake;
            }
            return   MaxUptake * (WC_Conc / (HalfSat + WC_Conc)) * (HalfSatInternal / (HalfSatInternal + (Ratio - MinRatio))) * CPlant.State * 1e3;
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


        //public void Derivative_WriteRates()
        //{
        //    Setup_Record 1 = AQTSeg.PSetup;
        //    if ((1.SaveBRates || 1.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Loading", Lo);
        //        SaveRate("Uptake", Uptk);
        //        SaveRate("Mortality", Mort);
        //        SaveRate("Respiration", Rsp);
        //        SaveRate("Excretion", Exc);
        //        SaveRate("Predation", Predt);
        //        if ((NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae) && (((AQTSeg.GetStatePointer(NState, T_SVType.StV, T_SVLayer.WaterCol)) as TPlant).IsPhytoplankton()))
        //        {
        //            if (!AQTSeg.LinkedMode)
        //            {
        //                SaveRate("TurbDiff", TD);
        //            }
        //            else
        //            {
        //                // If Not AQTSeg.CascadeRunning then
        //                SaveRate("DiffUp", DiffUp);
        //                SaveRate("DiffDown", DiffDown);
        //            }
        //        }
        //        if ((NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae) && (!CP.IsPhytoplankton()))
        //        {
        //            SaveRate("ToxDislodge", ToxD);
        //        }
        //        if (!(NState >= Consts.FirstMacro && NState <= Consts.LastMacro))
        //        {
        //            SaveRate("Washout", WashO);
        //            SaveRate("Washin", WashI);
        //            SaveRate("SinkToHyp", STH);
        //            SaveRate("SinkFromEp", SFE);
        //            if (SurfaceFloater)
        //            {
        //                SaveRate("Floating", Flt);
        //            }
        //            if (AQTSeg.EstuarySegment)
        //            {
        //                SaveRate("Entrainment", Entr);
        //            }
        //            SaveRate("NetBoundary", Lo + WashI - WashO + DiffUp + Entr + DiffDown + TD);
        //        }
        //        if ((NState >= Consts.FirstMacro && NState <= Consts.LastMacro) && (((CP) as TMacrophyte).MacroType == TMacroType.Freefloat))
        //        {
        //            SaveRate("Washout", WashO);
        //            SaveRate("Washin", WashI);
        //            SaveRate("NetBoundary", Lo + WashI - WashO + DiffUp + DiffDown + TD);
        //            SaveRate("Mac Break", MacBrk);
        //        }
        //        if ((CP.IsPeriphyton()))
        //        {
        //            SaveRate("Sed to Phyt", Sed2Me);
        //        }
        //        if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
        //        {
        //            SaveRate("Peri Slough", Slgh);
        //            SaveRate("Sediment", Sed);
        //        }
        //        if ((SVType == T_SVType.NIntrnl) && (NState >= Consts.FirstBlGreen && NState <= Consts.LastBlGreen))
        //        {
        //            SaveRate("N Fixation", FixN);
        //        }
        //    }
        //}

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
            double Rsp = 0;
            double FixN = 0;
//            double SegVolSave = 0;
            double Sed2Me = 0;
            double MacBrk = 0;
            double Slgh = 0;
//            double LoadInKg = 0;
            bool SurfaceFloater;
            // ----------------------------------------------------------------
            SurfaceFloater = CP.PAlgalRec.SurfaceFloating;
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
                Rsp = CP.Respiration() * N2O;
                MacBrk = ((CP) as TMacrophyte).Breakage() * N2O;
                // macrophytes
                DB = Lo + Uptk - (Predt + Mort + Exc + Rsp + MacBrk) - WashO + WashI;
            }
            else if (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae)
            {
                N2O = CP.Nutr_2_Org(SVType) * 1e3;
         // (ug N / mg OM) (g N / g OM)      (ug/g)
                Lo  =     CP.Loading * N2O;
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
                Rsp = CP.Respiration() * N2O;

                //if ((CP.IsFixingN()) && (SVType == T_SVType.NIntrnl))
                //{
                //    //MBLoadRecord MBLR = 5.MBLoadArray[AllVariables.Nitrate];
                //    FixN = CP.PAlgalRec.MaxNUptake * CP.State * 1e3;
                //    // ug/L d          // g/g d     // mg / L // ug/mg
                //    SegVolSave = AQTSeg.SegVol();

                //    LoadInKg = FixN * SegVolSave * 1000.0 * 1e-9;   // save for nutrient MB & categorization
                //    // kg nutr   // ug/L   // m3    // L/m3 // kg/ug

                //    //MBLR.TotOOSLoad[5.DerivStep] = MBLR.TotOOSLoad[5.DerivStep] + LoadInKg;
                //    //MBLR.LoadFixation[5.DerivStep] = MBLR.LoadFixation[5.DerivStep] + LoadInKg;
                //}

                ToxD = CP.ToxicDislodge() * N2O;
                Sed = CP.Sedimentation() * N2O;   // plant sedimentation
                Sed2Me = Derivative_NSed2Me();
                if ((CP.IsPeriphyton()))  Slgh = CP.Sloughing * N2O;
                     else Slgh = -Derivative_NToPhytoFromSlough();
                Uptk = Uptake();
                // algae
                DB = Lo + Uptk + WashI - (WashO + Predt + Mort + Sed + Exc + Rsp + ToxD + Slgh) - STH + SFE + Flt + Sed2Me + Entr + FixN;
                // algae
            }
            // Phytoplankton are subject to currents , diffusion & TD
            // HMS eliminated turbulent diffusion and linked-segment diffusion as irrelevant to 0D Model

            //if (NState >= Consts.FirstPlant && NState <= Consts.LastPlant)
            //{   Derivative_WriteRates();    }
        }


    } // end T_N_Internal_Plant

} // namespace
