using System;
using AQUATOX.AQTSegment;
using AQUATOX.AQSite;
using AQUATOX.Loadings;
using AQUATOX.Nutrients;
using AQUATOX.Organisms;
using AQUATOX.Plants;
using AQUATOX.Animals;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.OrgMatter
{

// ************
// * DETRITUS *
// ************
public class TDetritus : TRemineralize
    {
        public Loadings.TLoadings DF_Mort_Link = null;  // optional linkage from JSON if plants, animals not modeled
        public Loadings.TLoadings DF_Excr_Link = null;
        public Loadings.TLoadings DF_Sed_Link = null;
        public Loadings.TLoadings DF_Gameteloss_Link = null;

        public TDetritus(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
        public double MultFrac(DateTime TimeIndex, bool IsAlt, int PAltLdg)
        {
            double result;
            // Gets the correct fraction to multiply the general loadings data by to fit the appropriate compartment / data type
            // This is based on Refr/Labile split and Part/Diss split.  Inflow loadings get appropriate conversion factor
            // as well depending on whether data are BOD, TOC, or Org. Matter
            double ConvertFrac, RefrFrac, PartFrac, RefrPercent, PartPercent;
            // User Input Percentage of Refractory or Particulate

            TDissRefrDetr TDRD = (TDissRefrDetr) AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            DetritalInputRecordType PInputRec = TDRD.InputRecord;

            if ((NState==AllVariables.SedmRefrDetr)|| (NState == AllVariables.SedmLabDetr))
               throw new Exception("Programming Error:  Mult Frac is not relevant to Sed. Detritus");
            
            ConvertFrac = 1.0;
            if (IsAlt) RefrPercent = PInputRec.Percent_Refr.ReturnAltLoad(TimeIndex, PAltLdg);  
            else  RefrPercent = PInputRec.Percent_Refr.ReturnLoad(TimeIndex); 

            if (IsAlt) PartPercent = PInputRec.Percent_Part.ReturnAltLoad(TimeIndex, PAltLdg);  
            else  PartPercent = PInputRec.Percent_Part.ReturnLoad(TimeIndex);  

            if ((NState == AllVariables.DissRefrDetr) || (NState == AllVariables.DissLabDetr))
            {         PartFrac = 1.0 - (PartPercent / 100.0);  }
            else  {   PartFrac = (PartPercent / 100.0);      }

            if ((NState == AllVariables.DissRefrDetr) || (NState == AllVariables.SuspRefrDetr))
            {  RefrFrac = (RefrPercent / 100.0); }
            else
            {  RefrFrac = 1.0 - (RefrPercent / 100.0); }

            // Don't convert Point Source, Non Point Source Loads
            if (!IsAlt)
            {  switch (PInputRec.DataType)
                {
                    case DetrDataType.CBOD:
                        ConvertFrac = Location.Conv_CBOD5_to_OM(RefrPercent);
                        break;
                    case DetrDataType.Org_Carb:
                        ConvertFrac = Consts.Detr_OM_2_OC;
                        break;
                }
            }
            // Case
            result = ConvertFrac * RefrFrac * PartFrac;
            return result;
        }

        // -------------------------------------------------------------------------------------
        public override double GetInflowLoad(DateTime TimeIndex)
        {
            double result;
            DetritalInputRecordType PInputRec;
            PInputRec = ((AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TDissRefrDetr).InputRecord;
            result = 0;
            if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SedmLabDetr))
            {
                // TStateVariable
                // This function is for Susp&Diss Detritus Only
                base.GetInflowLoad(TimeIndex);
            }
            else
            {
                // Inflow Loadings
                result = PInputRec.Load.ReturnLoad(TimeIndex) * MultFrac(TimeIndex, false, -1);
            }
            return result;
        }

        // Relevant for SedDetr only
        // -------------------------------------------------------------------------------------
        public override void CalculateLoad(DateTime TimeIndex)
        {
            // This Procedure calculates inflow Susp & Diss Detrital loadings as input by the user
            DetritalInputRecordType PInputRec;
            int Loop;
            double Inflow;
            double SegVolume;
            double LoadRes;  // Hold the Result
            double AddLoad;

            PInputRec = ((AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol)) as TDissRefrDetr).InputRecord;
            SegVolume = AQTSeg.SegVol();
            if (SegVolume == 0)  throw new Exception("Water Volume is Zero, Cannot continue Simulation");
            
            MorphRecord MR = Location.Morph;
            Inflow = MR.InflowH2O; //  [vseg] * (OOSInflowFrac);    // inflow from inflow screen only, associated with loadings

            if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SedmLabDetr))
            {
                // This function is for Susp&Diss Detritus Only
                base.CalculateLoad(TimeIndex);
            }
            else
            {
                Loading = 0;
                LoadRes = GetInflowLoad(TimeIndex);
                LoadRes = LoadRes * Inflow / SegVolume;
                // unit/d     unit     cu m/d     cu m
                // Atmospheric and point-source loadings should be to epilimnion in single-segment mode;  9/9/98

                //if (AQTSeg.LinkedMode || (AQTSeg.VSeg == VerticalSegments.Epilimnion))
                //{
                    for (Loop = 0; Loop <= 2; Loop++)  //point source, direct precip, non-point source
                    {
                        AddLoad = PInputRec.Load.ReturnAltLoad(TimeIndex, Loop) * MultFrac(TimeIndex, true, Loop);
                                                   // g/d                        // unitless
                        AddLoad = AddLoad / SegVolume;
                        // mg/L d  // g/d   // cu m
                        LoadRes = LoadRes + AddLoad;
                        // mg/L d         // mg/L d
                    }
                // }
                Loading = LoadRes;
            }
            // Else Susp&Diss Detritus

        }

        // Const N2NH4 = 0.78;
        // N2NO3 = 0.23;
        // HalfSatN = 0.15;
        // MinN = 0.1;
        // ColonizeMax = 0.05; Saunders 1980, 0.04
        // 0.007;   McIntire & Colby, '78; Saunders '80
        public double Colonization_DecTCorr()
        {
            double result;
            double Temp;
            double Theta;
            const double Theta20 = 1.047;
            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (Temp >= 19)
            {
                Theta = Theta20;
            }
            else
            {
                Theta = 1.185 - 0.00729 * Temp;
            }
            result = Math.Pow(Theta, (Temp - Location.Remin.TOpt));
            if (Temp > Location.Remin.TMax)
            {
                result = 0;
            }
            return result;
        }

        // -------------------------------------------------------------------------------------------------------
        public double Colonization()
        {
            double result;
            // N,
            double T;
            double p;
            double DOCorr;
            double NLimit;
            double ColonizeMax;
            // DecTCorr
            if (!((NState == AllVariables.SuspRefrDetr) || (NState == AllVariables.DissRefrDetr) || (NState == AllVariables.SedmRefrDetr)))
            {
                throw new Exception("Programming Error, Colonization must be passed a Refr Detr");
            }
            // N := N2NH4*GetState(Ammonia) +  N2NO3*GetState(Nitrate);
            // 
            // If N > MinN then  NLimit := (N - MinN)/(N - MinN + HalfSatN)
            // else
            NLimit = 1.0;
            ColonizeMax = Location.Remin.DecayMax_Refr;
            ReminRecord w1 = Location.Remin;
            // T := AQTSeg.TCorr(Q10, TRef, TOpt, TMax);
            T = Colonization_DecTCorr();
            p = AQTSeg.pHCorr(w1.pHMin, w1.pHMax);
            // anoxic
            if ((NState == AllVariables.SedmRefrDetr) && (State > 50))
                  DOCorr = 0.001;
            else  DOCorr = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol) / (0.75 + AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol));

            result = (ColonizeMax * NLimit * T * p * DOCorr) * State;
            // g/m3 d   // g/g d   // unitless                // g/m3

            return result;
        }

        // assimilation
        // -------------------------------------------------------------------------------------------------------
        // ---------------------------
        // sum sources of detritus
        // ---------------------------
        public double PlantSink_To_Detr(AllVariables Ns)  
        {
            if ((Ns >= Consts.FirstAlgae) && (Ns <= Consts.LastAlgae))
            {
                if (NState == AllVariables.SedmLabDetr) return 0.92;
                if (NState == AllVariables.SedmRefrDetr) return 0.08;
            }

            return 0;
        }

        public double Mort_To_Detr(AllVariables Ns)
        {
            TPlant PPl;
            if ((Ns >= Consts.FirstAlgae) && (Ns <= Consts.LastAlgae))
            {
                if (NState == AllVariables.DissLabDetr) return 0.27;
                if (NState == AllVariables.DissRefrDetr) return 0.03;
                if (NState == AllVariables.SuspLabDetr) return 0.65;
                if (NState == AllVariables.SuspRefrDetr) return 0.05;
            };

            if ((Ns >= Consts.FirstMacro) && (Ns <= Consts.LastMacro))
            {
                PPl = AQTSeg.GetStatePointer(Ns, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (PPl.PAlgalRec.PlantType == "Bryophytes")
                {
                    if (NState == AllVariables.DissLabDetr) return 0.00;
                    if (NState == AllVariables.DissRefrDetr) return 0.25;
                    if (NState == AllVariables.SuspLabDetr) return 0.00;
                    if (NState == AllVariables.SuspRefrDetr) return 0.75;
                }
                else //not bryophytes
                {
                    if (NState == AllVariables.DissLabDetr) return 0.24;
                    if (NState == AllVariables.DissRefrDetr) return 0.01;
                    if (NState == AllVariables.SuspLabDetr) return 0.38;
                    if (NState == AllVariables.SuspRefrDetr) return 0.37;
                }
            }

            if ((Ns >= Consts.FirstAnimal) && (Ns <= Consts.LastAnimal))
            {
                if (NState == AllVariables.DissLabDetr) return 0.27;
                if (NState == AllVariables.DissRefrDetr) return 0.03;
                if (NState == AllVariables.SuspLabDetr) return 0.56;
                if (NState == AllVariables.SuspRefrDetr) return 0.14;
            };
            
            return 0;
        }

        //// Mort To Detr
        // -------------------------------------------------------------------------------------------------------
        public double Excr_To_Diss_Detr(AllVariables Ns)   
        {
            if ((Ns >= Consts.FirstAlgae) && (Ns <= Consts.LastAlgae))
            {
                if (NState == AllVariables.DissLabDetr) return 0.9;
                if (NState == AllVariables.DissRefrDetr) return 0.1;
            }

            if ((Ns >= Consts.FirstMacro) && (Ns <= Consts.LastMacro))
            {
                if (NState == AllVariables.DissLabDetr) return 0.8;
                if (NState == AllVariables.DissRefrDetr) return 0.2;
            }

            // otherwise it's an animal
            {
                if (NState == AllVariables.DissLabDetr) return 1.0;
                return 0.0;  // it must be that (NState == AllVariables.DissRefrDetr) 
            }
        }

        // -------------------------------------------------------------------------------------------------------
        public double SumGameteLoss()  
        {
            double GamLoss;
            AllVariables Loop;
            TAnimal PA;
            GamLoss = 0;
            for (Loop = Consts.FirstAnimal; Loop <= Consts.LastAnimal; Loop++)
            {
                PA = AQTSeg.GetStatePointer(Loop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (PA != null)
                {
                    GamLoss = GamLoss + PA.GameteLoss();
                }
            }
            // loop
            return GamLoss;
        }

        public void DetritalFormation_SumDF(TStateVariable P, ref double Mort, ref double Excr)  
{
    if (P.IsPlantOrAnimal())
    {
        Mort = Mort + ((P) as TOrganism).Mortality() * Mort_To_Detr(P.NState);

        if (P.IsAnimal())  
          {
            // AnimExcretion 5/13/2013
            Excr = Excr + ((P) as TAnimal).Respiration() * Excr_To_Diss_Detr(P.NState);
          }
        else
            Excr = Excr + ((P) as TPlant).PhotoResp() * Excr_To_Diss_Detr(P.NState);

        if ((P.IsMacrophyte()))
          {
            Mort = Mort + ((P) as TMacrophyte).Breakage() * Mort_To_Detr(P.NState);
          }

        if (P.IsPlant() && (!P.IsMacrophyte()))
        {
            TPlant w1 = ((P) as TPlant);
            Mort = Mort + ((P) as TPlant).ToxicDislodge() * Mort_To_Detr(P.NState);
            ((P) as TPlant).CalcSlough();
            // update sloughevent
            if (w1.SloughEvent)
            {
                double FracMult;
                double j = -999; // signal to not write mass balance tracking
                ((P) as TPlant).Derivative(ref j);  // update sloughing
   
                if (w1.PSameSpecies == AllVariables.NullStateVar) FracMult = 1.0;
                   else FracMult = 2.0 / 3.0; // 1/3 of periphyton will go to phytoplankton and 2/3 to detritus with sloughing/scour.

                Mort = Mort + w1.Sloughing * FracMult * Mort_To_Detr(P.NState);
            }
        }
    }
}

// -------------------------------------------------------------------------------------------------------
public double DetritalFormation(ref double Mort, ref double Excr, ref double Sed, ref double Gam)
        {
            double result;
            Mort = 0;
            Excr = 0;
            Sed = 0;
            Gam = 0;

            if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SedmLabDetr))  Sed = SedDetritalFormation(); 
            else foreach (TStateVariable TSV in AQTSeg.SV) DetritalFormation_SumDF(TSV, ref Mort, ref Excr); 
            

            if ((NState == AllVariables.SuspLabDetr))  Gam = SumGameteLoss(); 

            result = Mort + Excr + Sed + Gam;
            return result;
        }




        // -------------------------------------------------------------------------------------------------------
        public double SedDetritalFormation()
        {
            double Def;
            double Sed;

                // ------------------------------------------------------
                void SedDetritalFormation_SumSed(TStateVariable P)  
                {
                  if (P.IsAlgae())
                    {
                        TPlant PP = ((P) as TPlant);
                        if (!PP.IsLinkedPhyto()) Sed = Sed + PP.Sedimentation() * PlantSink_To_Detr(P.NState);
                    }
                }
                // ------------------------------------------------------
                void SedDetritalFormation_SumDef(TStateVariable P)
                {
                    double Def2Detr; // all defecation goes to sediment
                    if (P.IsAnimal())
                    {
                        if (NState == AllVariables.SedmLabDetr) Def2Detr = Consts.Def2SedLabDetr;
                        else Def2Detr = 1.0 - Consts.Def2SedLabDetr;

                        Def = Def + Def2Detr * ((TAnimal)P).Defecation();
                    }
                }
                // ------------------------------------------------------

            Def = 0;
            Sed = 0;

            foreach (TStateVariable TSV in AQTSeg.SV)  SedDetritalFormation_SumSed(TSV); 

            foreach (TStateVariable TSV in AQTSeg.SV) SedDetritalFormation_SumDef(TSV);   

            return Def + Sed;
        }

        public double DailyBurial()
        {
            double result;
            // double BenthArea;
            // Enabled during dynamic stratification,  6-30-2009

            if (!((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SedmLabDetr)))
            {   throw new Exception("Programming error: Daily Burial Called for Non-Sed Detritus");  }

            if (NState == AllVariables.SedmLabDetr) return 0;
            if (State <= Consts.Tiny) return 0;

            
                //// dynamic stratification daily burial enabled 6/30/2009
                //if (AQTSeg.Stratified && (!AQTSeg.LinkedMode))
                //{
                //    TAQTSite w2 = w1.Location;
                //    w3 = w2.Locale;
                //    BenthArea = w3.AreaFrac(w2.MeanThick[VerticalSegments.Epilimnion], w3.ZMax);
                //    if (w3.VSeg == VerticalSegments.Hypolimnion)
                //    {
                //        BenthArea = 1.0 - BenthArea;
                //    }
                //    BenthArea = BenthArea * Location.Locale.SurfArea;
                //    // m2
                //    result = (State - (InitialCond * BenthArea / w3.Morph.SegVolum[w3.VSeg]));
                //    // g/m3 d    //    // g/m3  //    // g/m2  //    // m2 seg.      //    // m3 seg.
                //}
                //else
                //{
            result = (State - (InitialCond * Location.Locale.SurfArea / AQTSeg.Volume_Last_Step));
        // g/m3 d  // g/m3      // g/m2            // m2 entire system        // m3 entire sys.
            
            if ((result < 0)) result = 0;
            return result;
        }

    } // end TDetritus

    public class DetritalInputRecordType
    {
        public DetrDataType DataType;   // CBOD,Org_Carb,Org_Matt
        public double InitCond;         // Initial Condition of TOC/CBOD/organic matter
        public double Percent_PartIC;
        public double Percent_RefrIC;   // Break down of the above Initial Condition
        public LoadingsRecord Load;     // Loadings of organics
        public LoadingsRecord Percent_Part;  // Constant or dynamic breakdowns of inflow, PS, NPS loadings
        public LoadingsRecord Percent_Refr;  // Constant or dynamic breakdowns of inflow, PS, NPS loadings
        public double[] ToxInitCond = new double[Consts.NToxs];         // Tox. exposure of Init. Cond
        public LoadingsRecord[] ToxLoad = new LoadingsRecord[Consts.NToxs]; // External loadings of toxicant in detritus
    } // end DetritalInputRecordType

    // Record
    public class TDissDetr : TDetritus
    {
        
        public TDissDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
        // ---------------------------------------------------
        //   pore water section removed (Derivative_CalcPW)
        // ---------------------------------------------------

        // ---------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double LoadInKg;
        //    double LossInKg;
        //    double LayerInKg;
        //    double NutrFrac;
        //    AllVariables Typ;
        //    for (Typ = AllVariables.Nitrate; Typ <= AllVariables.Phosphate; Typ++)
        //    {
        //        if (NState == AllVariables.DissLabDetr)
        //        {
        //            if (Typ == AllVariables.Nitrate)
        //            {
        //                NutrFrac = AQTSeg.Location.Remin.N2OrgDissLab;
        //            }
        //            else
        //            {
        //                NutrFrac = AQTSeg.Location.Remin.P2OrgDissLab;
        //            }
        //        }
        //        else if (Typ == AllVariables.Nitrate)
        //        {
        //            NutrFrac = AQTSeg.Location.Remin.N2OrgDissRefr;
        //        }
        //        else
        //        {
        //            NutrFrac = AQTSeg.Location.Remin.P2OrgDissRefr;
        //        }
        //        MBLoadRecord w2 = AQTSeg.MBLoadArray[Typ];
        //        LoadInKg = Lo * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        w2.BoundLoad[AQTSeg.DerivStep] = w2.BoundLoad[AQTSeg.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        LoadInKg = (Lo + WaI) * AQTSeg.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr
        //        // mg org/L
        //        // m3
        //        // L/m3
        //        // kg/mg
        //        // nutr / org
        //        if (En > 0)
        //        {
        //            LoadInKg = (Lo + WaI + En) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        w2.TotOOSLoad[w1.DerivStep] = w2.TotOOSLoad[w1.DerivStep] + LoadInKg;
        //        w2.LoadDetr[w1.DerivStep] = w2.LoadDetr[w1.DerivStep] + LoadInKg;
        //        MBLossRecord w3 = w1.MBLossArray[Typ];
        //        MorphRecord w4 = w1.Location.Morph;
        //        // *OOSDischFrac
        //        LossInKg = WaO * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // 3/20/2014 remove OOSDischFrac
        //        // kg nutr
        //        // mg org/L
        //        // m3
        //        // L/m3
        //        // kg/mg
        //        // nutr / org
        //        w3.BoundLoss[w1.DerivStep] = w3.BoundLoss[w1.DerivStep] + LossInKg;
        //        // Loss from the modeled system
        //        if (En < 0)
        //        {
        //            // *OOSDischFrac
        //            LossInKg = (-En + WaO) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        // 3/20/2014 remove OOSDischFrac
        //        w3.TotalNLoss[w1.DerivStep] = w3.TotalNLoss[w1.DerivStep] + LossInKg;
        //        w3.TotalWashout[w1.DerivStep] = w3.TotalWashout[w1.DerivStep] + LossInKg;
        //        w3.WashoutDetr[w1.DerivStep] = w3.WashoutDetr[w1.DerivStep] + LossInKg;
        //        MBLayerRecord w5 = w1.MBLayerArray[Typ];
        //        LayerInKg = (TD + DiffUp + DiffDown) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr
        //        // mg org/L
        //        // m3
        //        // L/m3
        //        // kg/mg
        //        // nutr / org
        //        w5.NTurbDiff[w1.DerivStep] = w5.NTurbDiff[w1.DerivStep] + LayerInKg;
        //        w5.NNetLayer[w1.DerivStep] = w5.NNetLayer[w1.DerivStep] + LayerInKg;
        //    }
        //    // Typ Loop

        //}

        public override void Derivative(ref double DB)
        {
            double Lo=0;
            double TD=0;
            double DiffUp=0;
            double DiffDown=0;
            double DiffSed=0;
            double WaO=0;
            double WaI=0;
            double DF=0;
            double DE=0;
            double Co=0;
            double PWExp=0;
            double ToPW=0;
            double En=0;
            double DFM=0;
            double DFE=0;
            double DFS=0;
            double DFG=0;
            // TDOMPoreWater DetrActiveLayer;
            //AllVariables DOMState;
            // TrackMB
            // --------------------------------------------------
            double FracAerobic=0;
            // DissDetr.Derivative

            //if (NState == AllVariables.DissRefrDetr) DOMState = AllVariables.ReDOMPore;
            //else                                     DOMState = AllVariables.LaDOMPore; // NState = DissLabDetr

            Lo = Loading;
            if (NState == AllVariables.DissLabDetr)  DE = Decomposition(Location.Remin.DecayMax_Lab, Consts.KAnaerobic, ref FracAerobic);

            DF = DetritalFormation(ref DFM, ref DFE, ref DFS, ref DFG);

            if (DF_Mort_Link != null) DFM = DF_Mort_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Excr_Link != null) DFE = DF_Excr_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Sed_Link != null) DFS = DF_Sed_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Gameteloss_Link != null) DFG =  DF_Gameteloss_Link.ReturnLoad(AQTSeg.TPresent);
            DF = DFM + DFE + DFS + DFG;

            
            if ((NState == AllVariables.DissRefrDetr))  Co = Colonization();

            WaO = Washout();
            //if (AQTSeg.LinkedMode)  WaI = Washin();

            //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
            //{
            //    DiffUp = SegmentDiffusion(true);
            //    DiffDown = SegmentDiffusion(false);
            //}
            //else if ((!AQTSeg.LinkedMode))
            //{
            //    TD = TurbDiff();
            //}

            //if (AQTSeg.MultiLayerSedModelIncluded())
            //{
            //    DetrActiveLayer = AQTSeg.GetStatePointer(DOMState, T_SVType.StV, T_SVLayer.SedLayer1);
            //    DiffSed = -DetrActiveLayer.UpperDiffusion(true);
            //}

            //if (AQTSeg.EstuarySegment)
            //{
            //    En = EstuaryEntrainment();
            //}
            //Derivative_CalcPW();

            DB = Lo + DF - DE - Co - WaO + WaI + TD + DiffUp + DiffDown + DiffSed + PWExp - ToPW + En;
            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

    } // end TDissDetr

    public class TDissRefrDetr : TDissDetr
    {

        public DetritalInputRecordType InputRecord = new DetritalInputRecordType();

        // -------------------------------------------------------------------------------------------------------
        //Constructor  Init( Ns,  SVT,  aName,  P,  IC,  IsTempl)
        public TDissRefrDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            int ToxLoop;
            int Alt_Loop;
            InputRecord.DataType = DetrDataType.Org_Matt;
            InputRecord.InitCond = 0;
            InputRecord.Percent_PartIC = 90;
            InputRecord.Percent_RefrIC = 10;

            // Set up Empty Loadings Data Record to copy into other LoadingsRecords
            InputRecord.Load = new LoadingsRecord();
            InputRecord.Percent_Part = new LoadingsRecord();
            InputRecord.Percent_Part.Loadings.ConstLoad = 90;
            InputRecord.Percent_Refr = new LoadingsRecord();
            InputRecord.Percent_Refr.Loadings.ConstLoad = 10;

            for (Alt_Loop = 0; Alt_Loop <= 2; Alt_Loop++)
            {
                InputRecord.Percent_Part.Alt_Loadings[Alt_Loop].UseConstant = true;
                InputRecord.Percent_Part.Alt_Loadings[Alt_Loop].ConstLoad = 90;

                InputRecord.Percent_Refr.Alt_Loadings[Alt_Loop].UseConstant = true;
                InputRecord.Percent_Refr.Alt_Loadings[Alt_Loop].ConstLoad = 90;
            }

            for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                InputRecord.ToxInitCond[ToxLoop] = 0;
                InputRecord.ToxLoad[ToxLoop] = null;
            }
            // with InputRecord
            // TRemineralize

        }

    } // end TDissRefrDetr

    public class TDissLabDetr : TDissDetr
    {
        public TDissLabDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
    } // end TDissLabDetr


    public class TSuspendedDetr : TDetritus
    {
        [JsonIgnore] public double DetrSinkToHypo = 0;
        public Loadings.TLoadings Predation_Link = null;

        public TSuspendedDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

//        public double Resuspension()  // currently only relevant to multi-layer sediment model
//        {
//            double result;
//            double SedState;
////          TBottomCohesives PTopC;
//            double ErodVel;
//            double Thick;
//            // Relevant to multi-layer sed model only
//            result = 0;
//            if (AQTSeg.SedModelIncluded())
//            {
//                PTopC = (AQTSeg.GetStatePointer(AllVariables.Cohesives, T_SVType.StV, T_SVLayer.SedLayer1));
//                ErodVel = PTopC.EVel;
//                // m/d
//                Thick = Location.MeanThick[AQTSeg.VSeg];
//                switch (NState)
//                {
//                    case AllVariables.SuspLabDetr:
//                        SedState = AQTSeg.GetState(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
//                        break;
//                    default:
//                        // SuspRefrDetr
//                        SedState = AQTSeg.GetState(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
//                        break;
//                        // g/m3
//                }
//                // case
//                result = SedState * ErodVel / Thick;
//                // mg/L d // mg/L  // m/d     // m
//            }
//            return result;
//        }

        public double Sedimentation()
        {
            double Sedimented;
            double Thick;
            double Decel;
            double DensFactor;
            //double FracDep;
            //double DepVel;
            //TBottomCohesives PTopC;
            //TSuspSediment PSuspC;

            Thick = Location.MeanThick;   // [AQTSeg.VSeg];
            ReminRecord RR = Location.Remin;
            DensFactor = AQTSeg.DensityFactor(RR.KSedTemp, RR.KSedSalinity);

            //if (AQTSeg.EstuarySegment)
            //{
            //    ReminRecord w2 = Location.Remin;
            //    Sedimented = w2.KSed / Thick * DensFactor * State;
            //    // g/cu m-d       m/d    m       unitless   g/cu m
            //    // ESTUARYSEGMENT Sedimented Calculation
            //}
            //else if (AQTSeg.SedModelIncluded())
            //{
            //    PTopC = (AQTSeg.GetStatePointer(AllVariables.Cohesives, T_SVType.StV, T_SVLayer.SedLayer1));
            //    PSuspC = (AQTSeg.GetStatePointer(AllVariables.Cohesives, T_SVType.StV, T_SVLayer.WaterCol));
            //    if (AQTSeg.UseSSC)
            //    {
            //        if (PSuspC.State < Consts.Tiny)  FracDep = 0; 
            //        else
            //        {
            //            FracDep = PTopC.Deposition() / (PSuspC.State * AQTSeg.SegVol());
            //            // 1/d          // g/d              // g/m3            // m3
            //        }
            //        Sedimented = State * FracDep;
            //        // mg/L d  // mg/L   // 1/d
            //    }
            //    else
                //{
                //    DepVel = PTopC.DVel;
                //    // m/d
                //    Sedimented = State * DepVel / Thick;
                //    // mg/L d  // mg/L  // m/d   // m
                //}
            //}
            //else
            {
                // NOT ESTUARY, Sed Model Not Included
                DetrSinkToHypo = 0;
                //if (GetState(AllVariables.Sand, T_SVType.StV, T_SVLayer.WaterCol) > -1) return 0;
                // if the site is a stream and inorganic sediments are being modeled (SSC Model), sedimentation
                // and resuspension are handled using silt as an indicator

                if ((AQTSeg.MeanDischarge > 0) && (Location.TotDischarge > AQTSeg.MeanDischarge))  
                        Decel = AQTSeg.MeanDischarge / (Location.TotDischarge);
                  else  Decel = 1;

                if (AQTSeg.MeanDischarge < Consts.Small)  Sedimented = RR.KSed / Thick * State * DensFactor;
                else                                      Sedimented = RR.KSed / Thick * State * Decel * DensFactor;

                if ((AQTSeg.GetStateVal(AllVariables.WindLoading, T_SVType.StV, T_SVLayer.WaterCol) >= 5.5) && (Thick <= 1.0))  
                    Sedimented = -Sedimented;        // should be a power fn. of depth

                if ((AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) < AQTSeg.Ice_Cover_Temp()))
                {   Sedimented = 2 * Sedimented;   }

                if (Sedimented < 0)
                {
                    // resuspension, but don't resuspend more Sed Detritus than exists
                    var SedState = NState switch
                    {
                        AllVariables.SuspLabDetr => AQTSeg.GetState(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol),
                                               _ => AQTSeg.GetState(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol),
                    };
                    if (-Sedimented > SedState) Sedimented = -SedState;  
                }
            }

            // Not Estuary Sed model not inluded code
            //DetrSinkToHypo = 0;
            //if ((!AQTSeg.Stratified) || (AQTSeg.VSeg == VerticalSegments.Hypolimnion) || (Sedimented < 0))
            //{
            //    // mg/L d
            //    // mg/L d
            //    result = Sedimented;
            //}
            //else
            //{
            //    // stratified
            //    TStates w4 = AQTSeg;
            //    TAQTSite w5 = w4.Location;
            //    w6 = w5.Locale;
            //    if (!w6.UseBathymetry)
            //    {
            //        DetrSinkToHypo = (w6.ThermoclArea / w6.SurfArea) * Sedimented;
            //    }
            //    else
            //    {
            //        DetrSinkToHypo = (1.0 - w6.AreaFrac(w6.MaxEpiThick, w6.ZMax)) * Sedimented;
            //    }
            //    // 10-14-2010 Note that ZMax parameter pertains to both segments in event of stratification
            //    result = Sedimented - DetrSinkToHypo;
            //    // mg/L d  // mg/L d     // frac
            //}

            return Sedimented;
        }

        // sedimentation
        //public double DetrSinkFromEp()
        //{
        //    double result;
        //    double SFE;
        //    double EpiVol;
        //    double HypVol;
        //    TStates w1 = AQTSeg;
        //    if (!w1.Stratified || (w1.VSeg == VerticalSegments.Epilimnion))
        //    {
        //        SFE = 0;
        //    }
        //    else
        //    {
        //        SFE = ((w1.EpiSegment.GetStatePointer(NState, SVType, Layer)) as TSuspendedDetr).DetrSinkToHypo;
        //        EpiVol = w1.EpiSegment.SegVol();
        //        HypVol = w1.SegVol();
        //        SFE = SFE * EpiVol / HypVol;
        //    }
        //    result = SFE;
        //    return result;
        //}

        // ----------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    TStates w1 = AQTSeg;
        //    Setup_Record w2 = w1.SetupRec;
        //    if ((w2.SaveBRates || w2.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", Lo);
        //        if ((NState != AllVariables.SuspRefrDetr))
        //        {
        //            SaveRate("Decomp", De);
        //            SaveRate("PlntSlough", PlSlg);
        //            SaveRate("Macrobreak", McB);
        //            SaveRate("PlntTDislodge", PlToxD);
        //        }
        //        SaveRate("DetrFm", DF);
        //        SaveRate("DF_Mortality", DFM);
        //        SaveRate("DF_Excretion", DFE);
        //        if ((NState == AllVariables.SuspLabDetr))
        //        {
        //            SaveRate("DF_Gameteloss", DFG);
        //        }
        //        SaveRate("Colonz", Math.Abs(Co));
        //        SaveRate("Washout", WaO);
        //        SaveRate("WashIn", WaI);
        //        SaveRate("Predation", Pr);
        //        SaveRate("Sedimentation", Se);
        //        if (w1.SedModelIncluded())
        //        {
        //            SaveRate("Resuspension", Re);
        //        }
        //        SaveRate("SinkToHypo", STH);
        //        SaveRate("SinkFromEpi", SFE);
        //        if (!AQTSeg.LinkedMode)
        //        {
        //            SaveRate("TurbDiff", TD);
        //        }
        //        else
        //        {
        //            // If Not AQTSeg.CascadeRunning
        //            // then
        //            SaveRate("DiffUp", DiffUp);
        //            SaveRate("DiffDown", DiffDown);
        //        }
        //        if (AQTSeg.EstuarySegment)
        //        {
        //            SaveRate("Entrainment", En);
        //        }
        //        SaveRate("NetBoundary", Lo + WaI - WaO + En + DiffUp + DiffDown + TD);
        //        SaveRate("Scour", Scour);
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
        //        TStates w1 = AQTSeg;
        //        if (NState == AllVariables.SuspLabDetr)
        //        {
        //            if (Typ == AllVariables.Nitrate)  NutrFrac = w1.Location.Remin.N2OrgLab;
        //            else                              NutrFrac = w1.Location.Remin.P2OrgLab;
        //        }
        //        else if (Typ == AllVariables.Nitrate)  NutrFrac = w1.Location.Remin.N2Org_Refr;
        //                                               NutrFrac = w1.Location.Remin.P2Org_Refr;
        //        MBLoadRecord w2 = w1.MBLoadArray[Typ];
        //        LoadInKg = Lo * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        w2.BoundLoad[w1.DerivStep] = w2.BoundLoad[w1.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        if (En > 0)
        //        {
        //            LoadInKg = (Lo + WaI + En) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        else
        //        {
        //            LoadInKg = (Lo + WaI) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }  // kg nutr   // mg org/L   // m3  // L/m3   // kg/mg   // nutr / org
        //        
        //        w2.TotOOSLoad[w1.DerivStep] = w2.TotOOSLoad[w1.DerivStep] + LoadInKg;
        //        w2.LoadDetr[w1.DerivStep] = w2.LoadDetr[w1.DerivStep] + LoadInKg;
        //        MorphRecord w3 = w1.Location.Morph;
        //        MBLossRecord w4 = w1.MBLossArray[Typ];
        //        WashoutStep[w1.DerivStep] = WaO * AQTSeg.SegVol();
        //        // g
        //        // mg/L
        //        // m3
        //        // * OOSDischFrac
        //        LossInKg = WaO * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // 3/20/2014 remove OOSDischFrac
        //        // kg nutr   // mg org/L   // m3  // L/m3   // kg/mg   // nutr / org
        //        w4.BoundLoss[w1.DerivStep] = w4.BoundLoss[w1.DerivStep] + LossInKg;
        //        // Loss from the modeled system
        //        if (En < 0)
        //        {
        //            // * OOSDischFrac
        //            LossInKg = (-En + WaO) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        }
        //        // loss from the system
        //        // 3/20/2014 remove OOSDischFrac
        //        w4.TotalNLoss[w1.DerivStep] = w4.TotalNLoss[w1.DerivStep] + LossInKg;
        //        w4.TotalWashout[w1.DerivStep] = w4.TotalWashout[w1.DerivStep] + LossInKg;
        //        w4.WashoutDetr[w1.DerivStep] = w4.WashoutDetr[w1.DerivStep] + LossInKg;
        //        MBLayerRecord w5 = w1.MBLayerArray[Typ];
        //        LayerInKg = (SFE - STH) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr   // mg org/L   // m3  // L/m3   // kg/mg   // nutr / org
        //        w5.NSink[w1.DerivStep] = w5.NSink[w1.DerivStep] + LayerInKg;
        //        w5.NNetLayer[w1.DerivStep] = w5.NNetLayer[w1.DerivStep] + LayerInKg;
        //        MBLayerRecord w6 = w1.MBLayerArray[Typ];
        //        LayerInKg = (TD + DiffUp + DiffDown) * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr   // mg org/L   // m3  // L/m3   // kg/mg   // nutr / org
        //        w6.NTurbDiff[w1.DerivStep] = w6.NTurbDiff[w1.DerivStep] + LayerInKg;
        //        w6.NNetLayer[w1.DerivStep] = w6.NNetLayer[w1.DerivStep] + LayerInKg;
        //    }
        //}

        // (************************************)
        // (*                                  *)
        // (*     DIFFERENTIAL EQUATIONS       *)
        // (*                                  *)
        // (************************************)
        public override void Derivative(ref double DB)
        {
            TDissRefrDetr DP;
            TSuspRefrDetr PRP;
            double Lo=0;
            double De = 0;
            double WaO=0;
            double WaI=0;
            double Pr=0;
            double Se=0;
            double Re=0;
            double TD=0;
            double DiffUp=0;
            double DiffDown=0;
            double FracAerobic=0;
            double DF=0;
            double Co=0;
            double STH=0;
            double SFE=0;
            double PlSlg=0;
            double PlToxD=0;
            double McB=0;
            double En=0;
            double Scour=0;
            double DFM=0;
            double DFE=0;
            double DFS=0;
            double DFG=0;
//            TBuriedDetr1 PBD;
            // --------------------------------------------------
//            double M2_M3=0;

            // TSuspendedDetr.Derivative

            //if ((NState == AllVariables.SuspLabDetr))
            //{
            //    PBD = AQTSeg.GetStatePointer(AllVariables.BuriedLabileDetr, T_SVType.StV, T_SVLayer.WaterCol);
            //}
            //else
            //{
            //    PBD = AQTSeg.GetStatePointer(AllVariables.BuriedRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            //}

            if ((NState == AllVariables.SuspLabDetr))
            {
                PRP = AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TSuspRefrDetr;
                DP = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDissRefrDetr;
                Co = PRP.Colonization() + DP.Colonization();
            }

            if ((NState == AllVariables.SuspRefrDetr))
            {
                Co = -Colonization();
            }
            Lo = Loading;

            // Sloughing and Macrophyte Breakage handled within DetrialFormation Function
            DF = DetritalFormation(ref DFM, ref DFE, ref DFS, ref DFG);

            if (DF_Mort_Link != null) DFM = DF_Mort_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Excr_Link != null) DFE = DF_Excr_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Sed_Link != null) DFS = DF_Sed_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Gameteloss_Link != null) DFG = DF_Gameteloss_Link.ReturnLoad(AQTSeg.TPresent);
            DF = DFM + DFE + DFS + DFG;

            WaO = Washout();

            //if (AQTSeg.LinkedMode)
            //{  WaI = Washin();  }

            Pr = Predation();  
            if (Predation_Link != null) Pr = Predation_Link.ReturnLoad(AQTSeg.TPresent);

            Se = Sedimentation();

            if (AQTSeg.Diagenesis_Included() && (Se < 0))
              {  Se = 0; }  // no resuspension in diagenesis model

            // Re = Resuspension(); currently only relevant to multi-layer sediment model

            if (NState == AllVariables.SuspLabDetr)
                De = Decomposition(Location.Remin.DecayMax_Lab, Consts.KAnaerobic, ref FracAerobic);

            // STH = DetrSinkToHypo;
            // SinkToHypo=0 if vseg=hypo
            // SFE = DetrSinkFromEp();
            //if (AQTSeg.GetStatePointer(AllVariables.Sand, T_SVType.StV, T_SVLayer.WaterCol) != null)
            //{
            //    if (PBD == null) throw new Exception("Buried Detritus must be utilized with sand silt clay");

            //    M2_M3 = Location.Locale.SurfArea / AQTSeg.SegVol();
            //    // m2/M3                // m2               // m3
            //    PBD.CalcTotalScour();
            //    Scour = State * PBD.Frac_Sed_Scour;
            //    // mg/L
            //    PBD.CalcTotalDep();
            //    Se = PBD.TotalDep * PBD.Frac_Dep_ToSed * M2_M3;
            //    // g/m2 d)                       {m2/m3
            //}
            //if (AQTSeg.LinkedMode && (!AQTSeg.CascadeRunning))
            //{
            //    DiffUp = SegmentDiffusion(true);
            //    DiffDown = SegmentDiffusion(false);
            //}
            //else if ((!AQTSeg.LinkedMode))
            //{
            //    TD = TurbDiff();
            //}
            //if (AQTSeg.EstuarySegment)
            //    En = EstuaryEntrainment();

            DB = Lo + DF + Co - De - WaO + WaI - Se + Re - Pr + PlSlg + PlToxD + McB - STH + SFE + TD + En + DiffUp + DiffDown + Scour;

            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

    } // end TSuspendedDetr

    public class TSuspLabDetr : TSuspendedDetr
    {
        public TSuspLabDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }
    } // end TSuspLabDetr

    public class TSuspRefrDetr : TSuspendedDetr
    {
        public TSuspRefrDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSuspRefrDetr

    public class TSedimentedDetr : TDetritus
    {
        public Loadings.TLoadings Predation_Link = null;

        public TSedimentedDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

        // --------------------------------------------------
        //public void Derivative_WriteRates()
        //{
        //    TStates w1 = AQTSeg;
        //    Setup_Record w2 = w1.SetupRec;
        //    if ((w2.SaveBRates || w2.ShowIntegration))
        //    {
        //        ClearRate();
        //        SaveRate("State", State);
        //        SaveRate("Load", Lo);
        //        SaveRate("DetrFm", DF);
        //        SaveRate("DF_Sedimentn.", DFS);
        //        SaveRate("Colonz", Math.Abs(Co));
        //        SaveRate("Predation", Pr);
        //        if ((NState != AllVariables.SedmRefrDetr))
        //        {
        //            SaveRate("Decomp", De);
        //        }
        //        SaveRate("Sedimentation", Se);
        //        SaveRate("Resuspension", Re);
        //        SaveRate("Burial", Bur);
        //        SaveRate("Scour", Scour);
        //        SaveRate("Exposure", Exps);
        //    }
        //}

        //// --------------------------------------------------
        //public void Derivative_TrackMB()
        //{
        //    double LoadInKg;
        //    double LossInKg;
        //    double NutrFrac;
        //    AllVariables Typ;
        //    for (Typ = AllVariables.Nitrate; Typ <= AllVariables.Phosphate; Typ++)
        //    {
        //        TStates w1 = AQTSeg;
        //        if (NState == AllVariables.SedmLabDetr)
        //        {
        //            if (Typ == AllVariables.Nitrate)
        //            {
        //                NutrFrac = w1.Location.Remin.N2OrgLab;
        //            }
        //            else
        //            {
        //                NutrFrac = w1.Location.Remin.P2OrgLab;
        //            }
        //        }
        //        else if (Typ == AllVariables.Nitrate)
        //        {
        //            NutrFrac = w1.Location.Remin.N2Org_Refr;
        //        }
        //        else
        //        {
        //            NutrFrac = w1.Location.Remin.P2Org_Refr;
        //        }
        //        MBLoadRecord w2 = w1.MBLoadArray[Typ];
        //        LoadInKg = Lo * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr // mg org/L // m3     //  L/m3// kg/mg // nutr / org
        //        w2.BoundLoad[w1.DerivStep] = w2.BoundLoad[w1.DerivStep] + LoadInKg;
        //        // Load into modeled system
        //        w2.TotOOSLoad[w1.DerivStep] = w2.TotOOSLoad[w1.DerivStep] + LoadInKg;
        //        w2.LoadDetr[w1.DerivStep] = w2.LoadDetr[w1.DerivStep] + LoadInKg;
        //        LoadInKg = Exps * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr // mg org/L // m3     //  L/m3// kg/mg // nutr / org

        //        w2.TotOOSLoad[w1.DerivStep] = w2.TotOOSLoad[w1.DerivStep] + LoadInKg;
        //        w2.Exposure[w1.DerivStep] = w2.Exposure[w1.DerivStep] + LoadInKg;
        //        MBLossRecord w3 = w1.MBLossArray[Typ];
        //        LossInKg = Bur * w1.SegVol() * 1000.0 * 1e-6 * NutrFrac;
        //        // kg nutr
        //        // mg org/L
        //        // m3
        //        // L/m3
        //        // kg/mg
        //        // nutr / org
        //        w3.TotalNLoss[w1.DerivStep] = w3.TotalNLoss[w1.DerivStep] + LossInKg;
        //        w3.BoundLoss[w1.DerivStep] = w3.BoundLoss[w1.DerivStep] + LossInKg;
        //        w3.Burial[w1.DerivStep] = w3.Burial[w1.DerivStep] + LossInKg;
        //    }
        //    // Typ Loop

        //}

        public override void Derivative(ref double DB)
        {
            TSuspendedDetr SuspP;
            TSedRefrDetr RefrP;
            double FracAerobic=0;
//            TBuriedDetr1 PBD=0;
            double Lo=0;
            double De=0;
            double Pr=0;
            double Se=0;
            double Re=0;
            double DF=0;
            double Co=0;
            double Bur=0;
            double Scour=0;
            double Exps=0;
            double Dep=0;
            double DFM=0;
            double DFE=0;
            double DFS=0;
            double DFG=0;
            // TrackMB
            // --------------------------------------------------
            //double M2_M3=0;
            
            // TSedimentedDetr.Derivative
            if ((NState == AllVariables.SedmLabDetr))
            {
                SuspP = AQTSeg.GetStatePointer(AllVariables.SuspLabDetr, T_SVType.StV, T_SVLayer.WaterCol) as TSuspendedDetr;
             //   PBD = AQTSeg.GetStatePointer(AllVariables.BuriedLabileDetr, T_SVType.StV, T_SVLayer.WaterCol);
            }
            else
            {
                // NState=SedmRefrDetr
                SuspP = AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TSuspendedDetr;
             //   PBD = AQTSeg.GetStatePointer(AllVariables.BuriedRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            }
            Lo = Loading;
            // If we want to zero out loading we have to change interface
            DF = DetritalFormation(ref DFM, ref DFE, ref DFS, ref DFG);

            if (DF_Mort_Link != null) DFM = DF_Mort_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Excr_Link != null) DFE = DF_Excr_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Sed_Link != null) DFS = DF_Sed_Link.ReturnLoad(AQTSeg.TPresent);
            if (DF_Gameteloss_Link != null) DFG = DF_Gameteloss_Link.ReturnLoad(AQTSeg.TPresent);
            DF = DFM + DFE + DFS + DFG;

            RefrP = AQTSeg.GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TSedRefrDetr;
            Co = RefrP.Colonization();
            if (NState == AllVariables.SedmRefrDetr)
            {
                Co = -Co;
            }
             Pr = Predation();  
            if (Predation_Link != null) Pr = Predation_Link.ReturnLoad(AQTSeg.TPresent);

            //if (PBD != null)
            //{
            //    M2_M3 = Location.Locale.SurfArea / AQTSeg.SegVol();
            //    // m2/M3                // m2              // m3
            //    PBD.CalcTotalScour();
            //    Scour = State * PBD.Frac_Sed_Scour;
            //    // mg/L
            //    Exps = PBD.BuriedDetr_To_Sed * M2_M3;
            //    // mg/L       // g/m2 d      // m2 / m3
            //    PBD.CalcTotalDep();
            //    Dep = PBD.TotalDep * PBD.Frac_Dep_ToSed * M2_M3;
            //    // g/m2 d)                           {m2/m3
            //    Bur = PBD.SedDetr_To_Buried + DailyBurial();
            //    // mg/L                        // mg/L
            //}

            if (NState != AllVariables.SedmRefrDetr)
            {  De = Decomposition(Location.Remin.DecayMax_Lab, Consts.KAnaerobic, ref FracAerobic);  }

            Se = SuspP.Sedimentation();

            if (AQTSeg.Diagenesis_Included() && (Se < 0)) Se = 0;  // no resuspension in diagenesis model

            //Re = SuspP.Resuspension();
            Se = Se + Dep;  // Combines deposition into the sedimentation rate

            DB = Lo + Se - Re + DF + Co - Pr - De + Exps - Bur - Scour;

            //Derivative_WriteRates();
            //Derivative_TrackMB();
        }

    } // end TSedimentedDetr

    public class TSedLabileDetr : TSedimentedDetr
    {
        public TSedLabileDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

    } // end TSedLabileDetr

    public class TSedRefrDetr : TSedimentedDetr
    {
        public TSedRefrDetr(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            
        }

    } // end TSedRefrDetr

}