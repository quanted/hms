using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQSite;
using AQUATOX.Loadings;
using AQUATOX.Nutrients;
using AQUATOX.Volume;
using AQUATOX.OrgMatter;
using AQUATOX.Diagenesis;
using AQUATOX.Chemicals;
using AQUATOX.Plants;
using AQUATOX.Animals;
using AQUATOX.Organisms;
using AQUATOX.Bioaccumulation;

using System.Threading;
using System.Threading.Tasks;

using System.Linq;
using Newtonsoft.Json;
using Data;
using System.ComponentModel;

namespace AQUATOX.AQTSegment

{
    public class AQTSim
    {
        public AQUATOXSegment AQTSeg = null;

        public string SaveJSON(ref string json)
        {
            try
            {
                AQTKnownTypesBinder AQTBinder = new AQTKnownTypesBinder();
                JsonSerializerSettings AQTJsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    TypeNameHandling = TypeNameHandling.Objects,
                    SerializationBinder = AQTBinder
                };
                json = Newtonsoft.Json.JsonConvert.SerializeObject(AQTSeg, AQTJsonSerializerSettings);
                return "";
            }
            catch (Newtonsoft.Json.JsonWriterException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

            finally
            {
            }

        }

        public string ExportJSON(ref string json)
        {

            try
            {
                AQTKnownTypesBinder AQTBinder = new AQTKnownTypesBinder();
                JsonSerializerSettings AQTJsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    TypeNameHandling = TypeNameHandling.Objects,
                    SerializationBinder = AQTBinder
                };
                json = Newtonsoft.Json.JsonConvert.SerializeObject(AQTSeg, AQTJsonSerializerSettings);
                return json;
            }
            catch (Newtonsoft.Json.JsonWriterException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

            finally
            {
            }

        }


        public string Instantiate(string json)
        {

            try
            {
                AQTKnownTypesBinder AQTBinder = new AQTKnownTypesBinder();
                JsonSerializerSettings AQTJsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    TypeNameHandling = TypeNameHandling.Objects,
                    SerializationBinder = AQTBinder
                };
                AQTSeg = Newtonsoft.Json.JsonConvert.DeserializeObject<AQUATOXSegment>(json, AQTJsonSerializerSettings);
                AQTSeg.SetupLinks();
                return "";
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }

            finally
            {
            }
        }

        public string Integrate()
        {
            if (AQTSeg == null) return "AQTSeg not Instantiated";

            try
            {
                AQTSeg.SetMemLocRec();
                string errmsg = AQTSeg.Verify_Runnable();
                if (errmsg != "") return errmsg;

                AQTSeg.ClearResults();
                AQTSeg.SVsToInitConds();
                Setup_Record PS = AQTSeg.PSetup;
                AQTSeg.Integrate(PS.FirstDay, PS.LastDay, PS.RelativeError, PS.MinStepSize, PS.StoreStepSize );
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
            }
        }

        public string Integrate(DateTime StartDate, DateTime EndDate)
        {
            if (AQTSeg == null) return "AQTSeg not Instantiated";

            AQTSeg.PSetup.FirstDay = StartDate;
            AQTSeg.PSetup.LastDay = EndDate;
            return Integrate();
        }

    }

    
    public class AQUATOXTSOutput : ITimeSeriesOutput
    {

        /// <summary>
        /// Dataset for the time series.
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Source of the dataset.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Metadata dictionary providing details for the time series.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Time series data.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public Dictionary<string, List<string>> Data { get; set; }

        public AQUATOXTSOutput()
        {
            Data = new Dictionary<string, List<string>>();
            Dataset = "";
            DataSource = "";
            Metadata = new Dictionary<string, string>();
        }

        public ITimeSeriesOutput Clone()
        {
            return this;
        }
    }

    public class TStateVariable
    {
        public double InitialCond = 0;     // Initial condition
        public double State = 0;           // Current Value, usually concentration
        public AllVariables NState;        // List of Organisms and Toxicants
        public T_SVType SVType;            // StV, OrgTox
        public T_SVLayer Layer;               // Relevant for sed detr, inorg sed., and pore water types
        public string PName = "";          // Text Name
        [JsonIgnore] public double yhold = 0;           // holds State value during derivative cycle
        [JsonIgnore] public double[] StepRes = new double[7];  // Holds Step Results
        [JsonIgnore] public double yerror = 0;          // holds error term from RKCK
        [JsonIgnore] public double yscale = 0;          // use in Integration
        [JsonIgnore] public List<double> Results = new List<double>(); // holds numerical results, internal, not evenly spaced if variable stepsize
        public AQUATOXTSOutput output = null;  // public and evenly-spaced results following integration / interpolation

        [JsonIgnore] public AQUATOXSegment AQTSeg = null;   // Pointer to Collection of State Variables of which I am a member
        public LoadingsRecord LoadsRec = null;   // Holds all of the Loadings Information for this State Variable  
        public bool UseLoadsRecAsDriver = false; // If user sets this to true, no integration is used but time series driving data from "loadsrec"
        [JsonIgnore] public double Loading = 0;         // Loading of State Variable This time step
//      bool RequiresData = false;
//      bool HasData = false;        // If RequiresUnderlyingData and Not HasData then Model cannot be run
        public string StateUnit;
        public string LoadingUnit;       // Consts 

        [JsonIgnore] public TAQTSite Location = null;    // Pointer to Site in which I'm located
//      public bool PShowRates = true;      // Does the user want rates written for this SV?
//      public TCollection RateColl = null; // Collection of saved rates for current timestep
//      public int RateIndex = 0;

        public string LoadNotes1;
        public string LoadNotes2;           // Notes associated with loadings
        public bool TrackResults = true;      // Does the user want to save results for this variable?
                                                           //public bool IsTemplate = false;       // Is this a member of the template study in a linked system?  True if single study run.
                                                           //public double[] WashoutStep = new double[7];     // Saved Washout Variables for use in outputting Cascade Outflow, nosave
//        double WashoutAgg = 0;
//        double LastTimeWrit = 0;


        public virtual void Derivative(ref double DB)
        {
            DB = 0;
        }

        public int ToxInt(T_SVType typ)
        {
            return (int)typ - 2;
        }

        public virtual void SetToInitCond()
        {
            int j;
            State = InitialCond;

            // Fish, Sedimented Detritus, Periphyton, Macrophytes, Zoobenthos must undergo unit conversion
            if (AQTSeg.Convert_g_m2_to_mg_L(NState, SVType, Layer))
                State = InitialCond * Location.Locale.SurfArea / AQTSeg.Volume_Last_Step;
              // g/m3      // g/m2                     // m2                   // m3

            yhold = 0;

            for (j = 1; j <= 6; j++) StepRes[j] = 0;

            // Initialize Toxics  
            if ((SVType >= Consts.FirstOrgTxTyp) && (SVType <= Consts.LastOrgTxTyp))
            {
                ((this) as TToxics).ppb = 0;

                ((this) as TToxics).IsAGGR = false;
                if ((SVType == Consts.FirstOrgTxTyp) && (AQTSeg.PSetup.T1IsAggregate))
                {
                    ((this) as TToxics).IsAGGR = true;
                }
            }

        }

        public void TakeDerivative(int Step)
        {   // commment
            if (UseLoadsRecAsDriver)  // If this is true, no integration is used; the variable is driven by time series data from "loadsrec"
            {
                State = LoadsRec.ReturnLoad(AQTSeg.TPresent);
                StepRes[Step] = 0;
            }
            else Derivative(ref StepRes[Step]);
        }

        public TStateVariable()
        { }

        //Constructor  Init( Ns,  SVT,  L,  aName,  P,  IC)
        public TStateVariable(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC)
        {
            int j;
            PName = aName;
            NState = Ns;
            SVType = SVT;
            Layer = L;
            InitialCond = IC;
            State = 0;

            AQTSeg = P;
            if (P != null)
            {
                Location = P.Location;
            }
            LoadNotes1 = "";
            LoadNotes2 = "";

            yhold = 0;
            for (j = 1; j <= 6; j++)
            {
                StepRes[j] = 0;
            }
        }

        public virtual void CalculateLoad(DateTime TimeIndex)
        {
            Loading = GetInflowLoad(TimeIndex); 
        }

        public virtual double GetInflowLoad(DateTime TimeIndex)
        {
            // This Procedure returns inflow loadings only
            // In the case of stratification, the user may specify which segment the inflow goes to 2-6-2007

            return LoadsRec.ReturnLoad(TimeIndex);
        }


        public virtual double Washout()
        {
            double result;
            // Downstream Washout of all dissolved or floating immobile SVs.
            // Virtual method that is overridden if necessary
            double Disch = Location.Discharge;
            if (Disch < Globals.Consts.Small) return 0;
            result = Disch * State / AQTSeg.SegVol();
            // unit / d // m3/d // unit           // cu m.

            // WashoutStep[AllStates.DerivStep] = result * AllStates.SegVol(); // FIXME MB Tracking 
            // 1000*mass                   // mass/L*d            // m3

            return result;
        }


        public double Decomposition_DecTCorr()
        {

            double result;
            double Temp;
            double Theta;
            const double Theta20 = 1.047;

            Temp = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (Temp >= 19)  Theta = Theta20;
            else             Theta = 1.185 - 0.00729 * Temp;

            result = Math.Pow(Theta, (Temp - Location.Remin.TOpt));

            if (Temp > Location.Remin.TMax)
                result = 0;

            return result;
        }

        // -----------------------------------------
        // microbial decomposition
        // Relevant to Organic Matter and Toxicant
        // -----------------------------------------
        public double Decomposition(double DecayMax, double KAnaer, ref double FracAerobic)
        {
            double T;
            double p;
            double Decomp;
            double KMicrob;
            double DOCorr;
            double Factor;
            double HalfSatO;
            double O2Conc;
            double EnvironLim;
            // DecTCorr
            // -------------------------------------------------------------

            // O2Conc := 0;
            // If Layer = WaterCol  then O2Conc := GetState(Oxygen,StV,WaterCol);
            // (*  If (Layer = SedLayer1) or (NState in [SedmLabDetr,SedmRefrDetr])
            // then  O2Conc := GetState(Oxygen,StV,WaterCol) * 0.50; {Assume 50% as much O2 in active layer}  *)

            O2Conc = AQTSeg.GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if (Layer > T_SVLayer.SedLayer1) O2Conc = 0;  // Anaerobic below active layer
            FracAerobic = 0;

            if (((NState == AllVariables.SuspRefrDetr) || (NState == AllVariables.DissRefrDetr) || (NState == AllVariables.SedmRefrDetr))&& (SVType == T_SVType.StV))
                 return 0;  // no decomposition of refractory SVs

            if (DecayMax < Consts.Tiny) return 0;
            
            if (State > Consts.VSmall)
            {
                // orgtox or stv
                if ((Layer == T_SVLayer.SedLayer1) || ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SedmRefrDetr)) )
                {   // acct. for anoxia in sediment & near-sed. zone
                    HalfSatO = 8.0;
                }
                else HalfSatO = 0.5;

                // Bowie et al., 1985
                ReminRecord RR = Location.Remin;
                // T := AllStates.TCorr(Q10, TRef, TOpt, TMax);
                T = Decomposition_DecTCorr();
                p = AQTSeg.pHCorr(RR.pHMin, RR.pHMax);

                if (O2Conc == 0) Factor = 0;
                else Factor = O2Conc / (HalfSatO + O2Conc);

                DOCorr = Factor + (1.0 - Factor) * KAnaer / DecayMax;
                if (DOCorr > Consts.Tiny) FracAerobic = (Factor / DOCorr);

                // Return Fraction of Decomp. that is Aerobic for BioTransformation Calc.
                EnvironLim = DOCorr * T * p;
                KMicrob = DecayMax * EnvironLim;
                // 1/d    // 1/d      // frac
                Decomp =   KMicrob * State;
                // g/m3 d  // 1/d  // g/m3 or ug/L for toxicants
            }
            else Decomp = 0.0;
            return Decomp;
        }  // Decomposition


        // TSTATEVARIABLE IDENTIFICATION METHODS 9/13/98 jsc
        public bool IsPlant()
        {
           return (NState >= Consts.FirstPlant && NState <= Consts.LastPlant) && (SVType == T_SVType.StV);
        }

        public bool IsMacrophyte()
        {
            return (NState >= Consts.FirstMacro && NState <= Consts.LastMacro) && (SVType == T_SVType.StV);
        }

        public bool IsAlgae()
        {
            bool result;
            result = (NState >= Consts.FirstAlgae && NState <= Consts.LastAlgae) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsAnimal()
        {
            bool result;
            result = (NState >= Consts.FirstAnimal && NState <= Consts.LastAnimal) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsFish()
        {
            bool result;
            result = (NState >= Consts.FirstFish && NState <= Consts.LastFish) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsSmallFish()
        {
            bool result;
            result = ((NState == AllVariables.SmForageFish1) || (NState == AllVariables.SmForageFish2) || (NState == AllVariables.SmBottomFish1) || (NState == AllVariables.SmBottomFish2) 
                   || (NState == AllVariables.SmGameFish1)) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsSmallPI()
        {
            bool result;
            result = (NState >= AllVariables.SmallPI1 && NState <= AllVariables.SmallPI2) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsInvertebrate()
        {
            bool result;
            result = (NState >= Consts.FirstInvert && NState <= Consts.LastInvert) && (SVType == T_SVType.StV);
            return result;
        }

        public bool IsPlantOrAnimal()
        {
            bool result;
            result = (NState >= Consts.FirstBiota && NState <= Consts.LastBiota) && (SVType == T_SVType.StV);
            return result;
        }

        public virtual double WetToDry()
        {
            
            ReminRecord RR = Location.Remin;
            switch (NState)
            {
                case AllVariables.SedmRefrDetr:
                    return RR.Wet2DrySRefr;
                case AllVariables.SedmLabDetr:
                    return RR.Wet2DrySLab;
                case AllVariables.SuspRefrDetr:
                    return RR.Wet2DryPRefr;
                case AllVariables.SuspLabDetr:
                    return RR.Wet2DryPLab;
                case AllVariables.DissRefrDetr:
                case AllVariables.DissLabDetr:
                    return 1.0;
                case AllVariables.ReDOMPore:
                case AllVariables.LaDOMPore:
                case AllVariables.PoreWater:
                    return 1.0;
                case AllVariables.BuriedRefrDetr:
                case AllVariables.BuriedLabileDetr:
                    return 1.0;
                default:
                    throw new Exception("TStateVariable Wet To Dry called for irrelevant variable.");
            }   // case
        }

        public double NutrToOrg(AllVariables S)
        {
            TPlant PP;
            TAnimal PA;
            ReminRecord LR = Location.Remin;
            bool Nitr = ((S == AllVariables.Nitrate) || (S == AllVariables.Ammonia));
            if (Nitr)
            {
                if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SuspRefrDetr)) return LR.N2Org_Refr;
                if ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SuspLabDetr)) return LR.N2OrgLab;
                if (NState == AllVariables.DissRefrDetr) return LR.N2OrgDissRefr;
                if (NState == AllVariables.DissLabDetr) return LR.N2OrgDissLab;

                if ((NState >= Consts.FirstAnimal) && (NState <= Consts.LastAnimal))  
                {
                    PA = this as TAnimal;
                    return PA.PAnimalData.N2Org;
                }

                PP = this as TPlant;  // must be a plant
                return PP.N_2_Org();
            }
            else
            {
                if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SuspRefrDetr)) return LR.P2Org_Refr;
                if ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SuspLabDetr)) return LR.P2OrgLab;
                if (NState == AllVariables.DissRefrDetr) return LR.P2OrgDissRefr;
                if (NState == AllVariables.DissLabDetr) return LR.P2OrgDissLab;

                if ((NState >= Consts.FirstAnimal) && (NState <= Consts.LastAnimal))  
                {
                    PA = this as TAnimal;
                    return PA.PAnimalData.P2Org;
                }

                PP = this as TPlant;  // must be a plant
                return PP.P_2_Org();

            };
        }

        public double Predation()
        {   // Calculates Predation of the given organism or organic matter using TAnimal.IngestSpecies 
            double Prd = 0;
            AllVariables ns = NState;
            //-------------------------------------------------------------------------
            void CalcPredation(TStateVariable P)
            {
                TAnimal PA;
                double EgestRet=0;
                double GER=0;
                if (P.IsAnimal())
                {
                    PA = ((P) as TAnimal);
                    Prd = Prd + PA.IngestSpecies(ns, null, ref EgestRet, ref GER);
                }
            }
            //-------------------------------------------------------------------------

            if ((NState == AllVariables.PON_G1)|| (NState == AllVariables.POP_G1) || (NState == AllVariables.POC_G1))
                    ns = AllVariables.SedmLabDetr;

            if ((NState == AllVariables.PON_G2) || (NState == AllVariables.POP_G2) || (NState == AllVariables.POC_G2))
                ns = AllVariables.SedmRefrDetr;

            foreach (TStateVariable TSV in AQTSeg.SV) CalcPredation(TSV);

            return Prd;
        }

       public double GetPPB(AllVariables S, T_SVType T, T_SVLayer L)
        {
            return AQTSeg.GetPPB(S, T, L);
        }


    }  // end TStateVariable


    public class TStates : List<TStateVariable>
    {
           [JsonIgnore] public List<DateTime> restimes = new List<DateTime>();

    }


    public class AQUATOXSegment
    {
        public TAQTSite Location = new TAQTSite();       // Site data structure

        public TStates SV = new TStates();    // State Variables
        public DateTime TPresent;
        public Setup_Record PSetup;

        public bool UseConstEvap = true;
        public TLoadings DynEvap = null;
        public bool UseConstZMean = true;
        public TLoadings DynZMean;
        public LoadingsRecord Shade;

        public bool CalcVelocity = true;
        public TLoadings DynVelocity = null;
        public double MeanDischarge = 0;    // output only

        public Diagenesis_Rec Diagenesis_Params;
        public bool Diagenesis_Steady_State = false;  // whether to calculate layer 1 as steady state

        public double MF_Spawn_Age = 3.0;  // multi-fish age where fish spawn relevant for age classes only, default is 3.0

        public Loadings.TLoadings BenthicBiomass_Link = null; // optional linkage for diagenesis simulations when benthos not directly simulated, g/m2
        public Loadings.TLoadings AnimalDef_Link = null; // optional linkage to sediment from animal defecation for diagenesis simulations when animals not directly simulated, g/m2

        [JsonIgnore] public BackgroundWorker ProgWorker = null;  // report progress

        [JsonIgnore] public double SOD = 0;   // SOD, calculated before derivatives
        [JsonIgnore] public int DerivStep;    // Current Derivative Step 1 to 6, Don't save in json  

        [JsonIgnore] public DateTime SimulationDate;  // time integration started
        [JsonIgnore] public DateTime VolumeUpdated;  // 
        [JsonIgnore] public double MeanVolume;       // 
        [JsonIgnore] public double Volume_Last_Step;   // Volume in the previous step, used for calculating dilute/conc,  if stratified, volume of whole system(nosave)}  
        [JsonIgnore] public bool Anoxic = false;       // Is System Anoxic , nosave
        [JsonIgnore] public DateTime[] FirstExposure = new DateTime[Consts.NToxs];   // First exposure to organic toxicant, nosave

        [JsonIgnore] public DateTime ModelStartTime;     // Start of model run
        [JsonIgnore] public int YearNum_PrevStep = 0;     // The year number during the previous step of the model run; used to determine when a year has passed

        [JsonIgnore] public DateTime LastPctEmbedCalc = DateTime.MinValue;
        [JsonIgnore] public double PercentEmbedded = 0;

        [JsonIgnore] public TStateVariable[,,] MemLocRec = null;   // Array of pointers to SV loc in memory
        [JsonIgnore] public List<TSVConc> PLightVals = new List<TSVConc>();
        [JsonIgnore] public List<TSVConc> PSedConcs = new List<TSVConc>(); // time history of Susp Inorg. Sed concs
        [JsonIgnore] public List<TSVConc> PO2Concs = new List<TSVConc>();

        [JsonIgnore] public DateTime[] TimeLastInorgSedAvg = new DateTime[2];
        [JsonIgnore] public double[] LastInorgSedAvg = new double[2];

        public AQUATOXSegment()
        {

        }

        public string Verify_Runnable()
        {
            if (SV.Count < 1) return "No State Variables Are Included in this Simulation.";
            if (Equals(PSetup, default(Setup_Record))) return "PSetup data structure must be initialized.";
            if (PSetup.StoreStepSize < Consts.Tiny) return "PSetup.StoreStepSize must be greater than zero.";
            if (PSetup.FirstDay >= PSetup.LastDay) return "In PSetup Record, Last Day must be after First Day.";

            return "";
        }


        // -------------------------------------------------
        public void SetDefaultSetup()
        {
            PSetup.FirstDay = new DateTime(1999, 1, 1);
            PSetup.LastDay = new DateTime(1999, 1, 31);
            PSetup.StoreStepSize = 1;
            PSetup.StepSizeInDays = true;
            PSetup.ModelTSDays = true;
            PSetup.RelativeError = 0.001;
            PSetup.MinStepSize = 1e-10;
            PSetup.SaveBRates = false;
            PSetup.AlwaysWriteHypo = false;
            PSetup.ShowIntegration = false;
            PSetup.UseComplexedInBAF = false;
            PSetup.DisableLipidCalc = true;  //disabled 4/28/09
            PSetup.UseExternalConcs = false;
            //   PSetup^.BCFUptake         = false;
            PSetup.Spinup_Mode = false;
            PSetup.NFix_UseRatio = false;
            PSetup.NtoPRatio = 7.0;
            PSetup.Spin_Nutrients = true;

            PSetup.FixStepSize = 0.1;
            PSetup.UseFixStepSize = false;
            PSetup.T1IsAggregate = false;
            PSetup.AmmoniaIsDriving = false;
            PSetup.TSedDetrIsDriving = false;

        }

        public void ClearResults()
        {
            foreach (TStateVariable TSV in SV)
                if (TSV.Results != null) TSV.Results.Clear(); else TSV.Results = new List<double>();
            if (SV.restimes != null) SV.restimes.Clear(); else SV.restimes = new List<DateTime>();

        }


        public void Derivs_ZeroDerivative(TStateVariable P)
        {
            int j2;
            for (j2 = 1; j2 <= 6; j2++)
            {
                P.StepRes[j2] = 0;
            }
        }

        // -------------------------------------------------------------------------------
        public void Derivs(DateTime X, int Step)
        {
            TPresent = X;

            DerivStep = Step;

            // Zero_Utility_Variables();  animal trophic level only

            CalculateAllLoads(TPresent);      // Calculate loads and ensure Morphometry is up-to-date
            if (Step == 1) CalculateSOD(); //  If Sed Diagenesis Model is attached, calculate SOD First
            CalculateSumPrey();
            NormDiff(Step);

            foreach (TStateVariable TSV in SV) TSV.TakeDerivative(Step);

            // Parallel.ForEach(SV, TSV => TSV.TakeDerivative(Step));  FIXME Enable Parallel.ForEach when not debugging
                
        }


        public void TryRKStep_CheckZeroState(TStateVariable p)
        {
            if (p.State < Consts.Tiny)
            {
                p.State = 0.0;
                if ((p.SVType >= Consts.FirstOrgTxTyp) && (p.SVType <= Consts.LastOrgTxTyp))
                  ((TToxics)p).ppb = 0;

            }
        }

        public void TryRKStep_CheckZeroStateAllSVs()
        {
            foreach (TStateVariable TSV in SV)
            {
                TryRKStep_CheckZeroState(TSV);
            }
        }

        public void TryRKStep_RestoreStates_From_Holder()
        {
            foreach (TStateVariable TSV in SV)
                TSV.State = TSV.yhold;
        }

        public void TryRKStep_SaveStates_to_Holder()
        {
            foreach (TStateVariable TSV in SV)
                TSV.yhold = TSV.State;
        }

        // Modify db to Account for a changing volume
        // Below functions in NUMERICAL.INC
        // -------------------------------------------------------------------------
        // Cash-Karp RungeKutta with adaptive stepsize.
        // 
        // Source, Cash, Karp, "A Variable Order Runge-Kutta Method for Initial
        // value problems with rapidly varying right-hand sides" ACM Transactions
        // on Mathematical Software 16: 201-222, 1990. doi:10.1145/79505.79507
        // 
        // Uses Nested Loops to determine fourth and fifth-order accurate solutions
        // 
        // See also: http://en.wikipedia.org/wiki/Cash-Karp
        // http://en.wikipedia.org/wiki/Runge%E2%80%93Kutta_method
        // 
        // -------------------------------------------------------------------------
        public void TryRKStep(DateTime x, double h)
        {
            double[] A = { 0, 0, 0.2, 0.3, 0.6, 1.0, 0.875 };
            double[] B5 = { 0, 0.09788359788, 0, 0.40257648953, 0.21043771044, 0, 0.28910220215 };
            double[] B4 = { 0, 0.10217737269, 0, 0.38390790344, 0.24459273727, 0.01932198661, 0.25 };  // Butcher Tableau
            double[,] Tableau = { { 0.2, 0, 0, 0, 0 }, 
                                  { 0.075, 0.225, 0, 0, 0 }, 
                                  { 0.3, -0.9, 1.2, 0, 0 }, 
                                  { -0.2037037037037, 2.5, -2.5925925925926, 1.2962962963, 0 }, 
                                  { 0.029495804398148, 0.341796875000000, 0.041594328703704, 0.400345413773148, 0.061767578125000 } };
            int Steps;
            int SubStep;
            double YFourth;
            TryRKStep_SaveStates_to_Holder();
            for (Steps = 1; Steps <= 6; Steps++)
            {
                if (Steps > 1)
                {
                    // First step derivative already completed
                    TryRKStep_CheckZeroStateAllSVs();
                    Derivs(x.AddDays(A[Steps] * h), Steps);
                    TryRKStep_RestoreStates_From_Holder();
                }
                if (Steps < 6)
                {
                    foreach (TStateVariable TSV in SV)
                    {
                        for (SubStep = 1; SubStep <= Steps; SubStep++)
                            TSV.State = TSV.State + h * Tableau[Steps - 1, SubStep - 1] * TSV.StepRes[SubStep];
                    }
                }
            }  // 6 steps of integration

            foreach (TStateVariable TSV in SV)
            {
                TSV.yhold = TSV.State;
                YFourth = TSV.State;
                for (Steps = 1; Steps <= 6; Steps++)
                {  // zero weights for both solutions
                    if (Steps != 2)
                    {
                        TSV.yhold = TSV.yhold + h * (B5[Steps] * TSV.StepRes[Steps]);    // Fifth Order Accurate Soln
                        YFourth = YFourth + h * (B4[Steps] * TSV.StepRes[Steps]);        // Fourth Order Accurate Soln
                    }
                }
                TSV.yerror = YFourth - TSV.yhold;      // Error is taken to be the difference between the fourth and fith-order accurate solutions
            }
        }

        // -------------------------------------------------------------------------
        // Cash-Karp RungeKutta with adaptive stepsize.
        // 
        // Adaptive Stepsize Adjustment Source Dr. Michael Thomas Flanagan
        // www.ee.ucl.ac.uk/~mflanaga
        // 
        // -------------------------------------------------------------------------
        public void AdaptiveStep(ref DateTime x, double hstart, double RelError, ref double h_taken, ref double hnext, double MaxStep)
        {
            const double SAFETY = 0.9;
            double h;
            double Delta;
            TStateVariable ErrVar;
            double MaxError;

            if (PSetup.UseFixStepSize)
            {
                h = PSetup.FixStepSize;  // 2/21/2012 new option
                if ((x.AddDays(h) > PSetup.LastDay))
                {
                    // if stepsize can overshoot, decrease
                    h = (PSetup.LastDay - x).TotalDays;
                }
            }
            else
            {
                h = hstart;
            }
            do
            {
                TryRKStep(x, h);
                // calculate RKQS 4th and 5th order solutions and estimate error based on their difference
                MaxError = 0;
                ErrVar = null;
                if (!PSetup.UseFixStepSize)
                {
                    foreach (TStateVariable TSV in SV)
                    {
                        if ((Math.Abs(TSV.yscale) > Consts.VSmall))
                        {
                            if ((Math.Abs(TSV.yerror / TSV.yscale) > MaxError))
                            {
                                if (!((TSV.yhold < 0) && (TSV.State < Consts.VSmall)))  // no need to track error, state variable constrained to zero
                                {

                                    MaxError = Math.Abs(TSV.yerror / TSV.yscale);    // maximum error of all differential equations evaluated
                                    ErrVar = TSV;                                   // save error variable for later use in screen display
                                }
                            }
                        }
                    }
                }
                if (!PSetup.UseFixStepSize)
                {
                    if ((MaxError >= RelError))
                    {
                        // Step has failed, reduce step-size and try again
                        // Adaptive Stepsize Adjustment Source ("Delta" Code) Dr. Michael Thomas Flanagan
                        // www.ee.ucl.ac.uk/~mflanaga
                        // 
                        // Permission to use, copy and modify this software and its documentation for
                        // NON-COMMERCIAL purposes is granted, without fee, provided that an acknowledgement
                        // to the author, Dr Michael Thomas Flanagan at www.ee.ucl.ac.uk/~mflanaga, appears in all copies.
                        // Dr Michael Thomas Flanagan makes no representations about the suitability or fitness
                        // of the software for any or for a particular purpose. Dr Michael Thomas Flanagan shall
                        // not be liable for any damages suffered as a result of using, modifying or distributing
                        // this software or its derivatives.

                        Delta = SAFETY * Math.Pow(MaxError / RelError, -0.25);
                        if ((Delta < 0.1)) h = h * 0.1;
                        else h = h * Delta;
                    }
                }
                // no warning at this time
            } while (!((MaxError < RelError) || (h < Consts.Minimum_Stepsize) || (PSetup.UseFixStepSize)));
            // If (MaxError>1) and (not StepSizeWarned) then
            // Begin
            // StepSizeWarned = true;
            // MessageStr = 'Warning, the differential equation solver time-step has been forced below the minimum';
            // If ShowDebug then MessageStr = MessageStr + ' due to the "' +ProgData.ErrVar + '" state variable';
            // MessageStr = MessageStr + '.  Continuing to step forward using minimum step-size.';
            // MessageErr = true;
            // TSMessage;
            // End;
            if (PSetup.UseFixStepSize)
            {
                hnext = h;
            }
            else if ((MaxError < RelError))
            {
                // Adaptive Stepsize Adjustment Source ("Delta" code) Dr. Michael Thomas Flanagan www.ee.ucl.ac.uk/~mflanaga, see terms above
                if (MaxError == 0)  Delta = 4.0;
                else Delta = SAFETY * Math.Pow(MaxError / RelError, -0.2);

                if ((Delta > 4.0)) Delta = 4.0;
                if ((Delta < 1.0)) Delta = 1.0;

                hnext = h * Delta;
            }
            h_taken = h;

            if (hnext > MaxStep) hnext = MaxStep;
            if (h > MaxStep) h = MaxStep;

            x = x.AddDays(h);

            foreach (TStateVariable TSV in SV)               // reasonable error, so copy results of differentiation saved in YHolders
                TSV.State = TSV.yhold;
            
            Perform_Dilute_or_Concentrate();
        }


        public void Integrate_CheckZeroState(TStateVariable p)
        {
            if (p.State < 0)
            {
                p.State = 0.0;
                if ((p.SVType >= Consts.FirstOrgTxTyp) && (p.SVType <= Consts.LastOrgTxTyp))
                    ((TToxics)p).ppb = 0;

            }
        }

        public void Integrate_CheckZeroStateAllSVs()
        {
            foreach (TStateVariable TSV in SV)
            {
                Integrate_CheckZeroState(TSV);
            }
        }

        // -----------------------------------------------------------------
        public void DoThisEveryStep_SetFracKilled_and_Spawned(TStateVariable P, double hdid) 
        {
            TAnimal PAnim;
            int MidWinterJulianDate;
            int sl;
            int ToxLoop;
            int ionized;
            double WeightedCumFracKill;
            double WeightedTempResistant;

            if (P.IsPlant())  ((P) as TPlant).NutrLim_Step = ((P) as TPlant).NutrLimit();

            if (Location.Locale.Latitude < 0.0) MidWinterJulianDate = 182;
                                           else MidWinterJulianDate = 1;

            if (P.IsPlantOrAnimal())
            {   TOrganism TOR = (TOrganism)P;
                if ((P.State < Consts.VSmall) && (P.InitialCond > Consts.VSmall) && (P.Loading < Consts.VSmall))
                {
                    P.State = P.State + 1e-7;                     // 1/15/2015
                    // prevent extinction outside of derivs to prevent stiff eqn 4/15/2015
                }

                WeightedCumFracKill = (Consts.C1 * TOR.SedDeltaCumFracKill[1] + Consts.C3 * TOR.SedDeltaCumFracKill[3] + Consts.C4 * TOR.SedDeltaCumFracKill[4] + Consts.C6 * TOR.SedDeltaCumFracKill[6]) * hdid;
                WeightedTempResistant = (Consts.C1 * TOR.SedDeltaResistant[1] + Consts.C3 * TOR.SedDeltaResistant[3] + Consts.C4 * TOR.SedDeltaResistant[4] + Consts.C6 * TOR.SedDeltaResistant[6]) * hdid;
                if (WeightedCumFracKill > 0)
                    TOR.SedPrevFracKill = TOR.SedPrevFracKill + WeightedCumFracKill;

                if (WeightedTempResistant > 0)
                    TOR.SedResistant = TOR.SedResistant + WeightedTempResistant;

                if (TOR.SedResistant > 1)
                    TOR.SedResistant = 1;

                for (sl = 1; sl <= 6; sl++)
                {
                    TOR.SedDeltaCumFracKill[sl] = 0;  // initialize for next step's calculations
                    TOR.SedDeltaResistant[sl] = 0;
                }
                for (ionized = 0; ionized <= 1; ionized++)
                {
                    WeightedCumFracKill = (Consts.C1 * TOR.AmmoniaDeltaCumFracKill[ionized, 1] + Consts.C3 * TOR.AmmoniaDeltaCumFracKill[ionized, 3] + Consts.C4 * TOR.AmmoniaDeltaCumFracKill[ionized, 4] + Consts.C6 * TOR.AmmoniaDeltaCumFracKill[ionized, 6]) * hdid;
                    WeightedTempResistant = (Consts.C1 * TOR.AmmoniaDeltaResistant[ionized, 1] + Consts.C3 * TOR.AmmoniaDeltaResistant[ionized, 3] + Consts.C4 * TOR.AmmoniaDeltaResistant[ionized, 4] + Consts.C6 * TOR.AmmoniaDeltaResistant[ionized, 6]) * hdid;
                    if (WeightedCumFracKill > 0)
                        TOR.AmmoniaPrevFracKill[ionized] = TOR.AmmoniaPrevFracKill[ionized] + WeightedCumFracKill;
                    if (WeightedTempResistant > 0)
                        TOR.AmmoniaResistant[ionized] = TOR.AmmoniaResistant[ionized] + WeightedTempResistant;
                    if (TOR.AmmoniaResistant[ionized] > 1)
                        TOR.AmmoniaResistant[ionized] = 1;
                    for (sl = 1; sl <= 6; sl++)
                    {
                        TOR.AmmoniaDeltaCumFracKill[ionized, sl] = 0;
                        TOR.AmmoniaDeltaResistant[ionized, sl] = 0;
                    }
                }

                for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
                {
                    WeightedCumFracKill = (Consts.C1 * TOR.DeltaCumFracKill[ToxLoop, 1] + Consts.C3 * TOR.DeltaCumFracKill[ToxLoop, 3] + Consts.C4 * TOR.DeltaCumFracKill[ToxLoop, 4] + Consts.C6 * TOR.DeltaCumFracKill[ToxLoop, 6]) * hdid;
                    WeightedTempResistant = (Consts.C1 * TOR.DeltaResistant[ToxLoop, 1] + Consts.C3 * TOR.DeltaResistant[ToxLoop, 3] + Consts.C4 * TOR.DeltaResistant[ToxLoop, 4] + Consts.C6 * TOR.DeltaResistant[ToxLoop, 6]) * hdid;
                    if (WeightedCumFracKill > 0)
                        TOR.PrevFracKill[ToxLoop] = TOR.PrevFracKill[ToxLoop] + WeightedCumFracKill;
                    if (WeightedTempResistant > 0)
                        TOR.Resistant[ToxLoop] = TOR.Resistant[ToxLoop] + WeightedTempResistant;
                    if (TOR.Resistant[ToxLoop] > 1)
                        TOR.Resistant[ToxLoop] = 1;
                    for (sl = 1; sl <= 6; sl++)
                    {
                        TOR.DeltaCumFracKill[ToxLoop, sl] = 0;
                        TOR.DeltaResistant[ToxLoop, sl] = 0;
                    }
                }

                if (TPresent.DayOfYear == MidWinterJulianDate)
                {
                    // 9-17-07 JSC, see specs in section 8.1
                    // it is assumed that resistance persists in the population until the end of the growing season
                    TOR.SedPrevFracKill = 0;
                    TOR.SedResistant = 0;
                    for (ionized = 0; ionized <= 1; ionized++)
                    {
                        TOR.AmmoniaPrevFracKill[ionized] = 0;
                        TOR.AmmoniaResistant[ionized] = 0;
                    }

                    for (ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
                    {
                        TOR.PrevFracKill[ToxLoop] = 0;
                        TOR.Resistant[ToxLoop] = 0;
                        FirstExposure[ToxLoop] =   DateTime.MinValue;
                    }
                }
            }
            if (P.IsAnimal())
            {
                PAnim = ((P) as TAnimal);
                if (((PAnim.SpawnNow(TPresent.AddDays(-hdid))) && (!PAnim.Spawned)))
                {
                    PAnim.SpawnTimes++;
                    // Spawned must be set to true so that fish do not
                    // spawn multiple times in the same day / temp. range
                    PAnim.Spawned = true;
                }
                else if ((!PAnim.SpawnNow(TPresent.AddDays(-hdid))))  PAnim.Spawned = false;

                // Reset Number of Spawning Times in Midwinter
                if (TPresent.DayOfYear == MidWinterJulianDate)  PAnim.SpawnTimes = 0;

            }
            // if IsAnimal

        }


        public void DoThisEveryStep_CheckSloughEvent(TStateVariable P)  //turn off sloughing following a 1/day slough event
        {
            if (P.IsPlant())
            {
                if (((TPlant)P).SloughEvent)
                    if ((P.State <= ((TPlant)P).SloughLevel))
                        ((TPlant)P).SloughEvent = false;
                // HMS removed code to update progress dialog 

                ((TPlant)P).NutrLim_Step = ((TPlant)P).NutrLimit();
            }
        }

        public void DoThisEveryStep_UpdateO2Concs()
        {
            TSVConc pconc;
            TSVConc newconc = new TSVConc();
            int i;
            bool deleted;

            newconc.SVConc = GetStateVal(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if (newconc.SVConc < 0) newconc.SVConc = 0;

            newconc.Time = TPresent;
            PO2Concs.Insert(0, newconc);
            i = PO2Concs.Count - 1;
            do
            {
                // clean up any data points greater than 96 hours old
                deleted = false;
                pconc = PO2Concs[i];
                
                if ((TPresent - pconc.Time).TotalDays > 4) // 4 days or 96 hours
                {
                    PO2Concs.RemoveAt(i);
                    deleted = true;
                }
                i -= 1;
            } while (!(!deleted || (i < 0)));
        }


        // -----------------------------------------------------------------
        public void DoThisEveryStep_UpdateSedConcs()
        {   // write suspended sed concs for mort effects
            TSVConc pconc;
            TSVConc newconc = new TSVConc();
            int i;
            bool deleted;
            newconc.SVConc = InorgSedConc();
            newconc.Time = TPresent;

            PSedConcs.Insert(0, newconc);

            i = PSedConcs.Count - 1;
            do
            {
                // clean up any data points greater than 61 days old
                deleted = false;
                pconc = PSedConcs[i];
                // days
                if ((TPresent - pconc.Time).TotalDays > 61) // 61 days
                {
                    PSedConcs.RemoveAt(i);
                    deleted = true;
                }
                i -= 1;
            } while (!(!deleted || (i < 0)));
        }

        // -----------------------------------------------------------------

        public void DoThisEveryStep_UpdateLightVals()
        {
            // Time history of daily average light values
            TSVConc pconc;
            TSVConc newconc;
            int i;
            TLight PL;
            bool deleted;
            PL = GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol) as TLight;
            if (PL != null)
            {
                newconc = new TSVConc();
                newconc.SVConc = PL.DailyLight;
                newconc.Time = TPresent;
                PLightVals.Insert(0, newconc);
                i = PLightVals.Count - 1;
                do
                {
                    // clean up any data points greater than 96 hours old
                    deleted = false;
                    pconc = PLightVals[i];
                    if ((TPresent - pconc.Time).TotalDays > 4)                  // 4 days or 96 hours
                    {
                        PLightVals.RemoveAt(i); 
                        deleted = true;
                    }
                    i -= 1;
                } while (!(!deleted || (i < 0)));
            }
        }

        // -----------------------------------------------------------------
        public void DoThisEveryStep_MultiFishPromote(double hdid)
        {
            AllVariables Young;
            AllVariables MFLoop;
            TStateVariable PYF;
            TStateVariable POF;
            // PYoungFish, POldFish
            TAnimal PAnml;
            T_SVType ToxLoop;
            bool OldestFish;
            // Is program evaluating oldest age-class in simulation
            PAnml = GetStatePointer(AllVariables.Fish3, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
            if ((PAnml == null))  return;

            OldestFish = true;
            if (PAnml.SpawnNow(TPresent.AddDays(-hdid)) && (!PAnml.Spawned) && (PAnml.SpawnTimes == 0))
            {
                // age class change on first spawning of the year
                for (MFLoop = AllVariables.Fish15; MFLoop >= AllVariables.Fish1; MFLoop--)
                {
                    if (GetStatePointer(MFLoop, T_SVType.StV, T_SVLayer.WaterCol) != null)
                    {
                        Young = MFLoop - 1;
                        // promote fish itself
                        POF = GetStatePointer(MFLoop, T_SVType.StV, T_SVLayer.WaterCol);
                        PYF = GetStatePointer(Young, T_SVType.StV, T_SVLayer.WaterCol);
                        if (OldestFish) POF.State = POF.State + PYF.State;
                        else if (MFLoop == AllVariables.Fish1)  // YOY
                             POF.State = 0;
                        else POF.State = PYF.State;

                        // promote org toxicants and mercury
                        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.FirstOrgTxTyp; ToxLoop++)
                        {
                            POF = GetStatePointer(MFLoop, ToxLoop, T_SVLayer.WaterCol);
                            if (POF != null)
                            {
                                PYF = GetStatePointer(Young, ToxLoop, T_SVLayer.WaterCol);
                                if (OldestFish) POF.State = POF.State + PYF.State;
                                else
                                {
                                    if (MFLoop == AllVariables.Fish1) POF.State = 0;
                                    else POF.State = PYF.State;
                                }
                            }
                        }
                        if (OldestFish) OldestFish = false;
                   }
                }
            }
        }

        public void DoThisEveryStep_FishRecruit()
        {
            AllVariables FLoop;
            TAnimal PFish;
            TAnimalTox PToxFish;
            T_SVType ToxLoop;
            for (FLoop = Consts.FirstInvert; FLoop <= Consts.LastFish; FLoop++)
            {
                // 4/16/2014 add incorporation of invert recrsave
                PFish = GetStatePointer(FLoop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (PFish != null)
                {
                    PFish.State = PFish.State + PFish.RecrSave;
                    for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
                    {
                        PToxFish = GetStatePointer(FLoop, ToxLoop, T_SVLayer.WaterCol) as TAnimalTox;
                        if (PToxFish != null)
                        {
                            PToxFish.State = PToxFish.State + PToxFish.RecrSaveTox; 
                        }
                    }
                }
            }
        }

        // -----------------------------------------------------------------
        public void DoThisEveryStep_SumAggr()  // 10/6/2014
        {
            AllVariables SVLoop;
            T_SVType OrgLoop, TypLoop;
            TToxics PTox, PTox2;
            double SumState, SumPPB;

            PTox = GetStatePointer(AllVariables.H2OTox, T_SVType.OrgTox1, T_SVLayer.WaterCol) as TToxics;   // orgtox 1 is the aggregate compartment
            if (PTox == null) return;
            if (!PTox.IsAGGR) return;

            SumState = 0;
            SumPPB = 0;
            for (TypLoop = T_SVType.OrgTox2; TypLoop <= T_SVType.OrgTox20; TypLoop++)
            {
                PTox2 = GetStatePointer(AllVariables.H2OTox, TypLoop, T_SVLayer.WaterCol) as TToxics;
                if (PTox2 != null)
                {
                    SumState = SumState + PTox2.State;
                    SumPPB = SumPPB + PTox2.ppb;
                }
            }
            PTox.State = SumState;  // sum up individual Bins

            PTox.ppb = SumPPB;
            // sum up Kow Bins (ppb)
            for (SVLoop = Consts.FirstDetr; SVLoop <= Consts.LastAnimal; SVLoop++)
            {
                SumState = 0;
                SumPPB = 0;
                PTox = GetStatePointer(SVLoop, T_SVType.OrgTox1, T_SVLayer.WaterCol) as TToxics;
                if (PTox != null)
                {
                    if (PTox.IsAGGR)
                    {
                        for (OrgLoop = T_SVType.OrgTox2; OrgLoop <= T_SVType.OrgTox20; OrgLoop++)
                        {
                            PTox2 = GetStatePointer(SVLoop, OrgLoop, T_SVLayer.WaterCol) as TToxics;
                            if (PTox2 != null)
                            {
                                SumState = SumState + PTox2.State;
                                SumPPB = SumPPB + PTox2.ppb;
                            }
                        }
                        PTox.State = SumState;    // sum up Kow Bins
                        PTox.ppb = SumPPB;        // sum up Kow Bins (ppb)
                    }
                }
            }
        }

        public void DoThisEveryStep_CalculatePercentEmbedded()
        {   // 3-12-08
            double PECalc;
            double Inorg60 = InorgSed60Day(1);
            if (Inorg60 < Consts.Tiny) PECalc = 0;
            else  PECalc = 100 * (0.077 * Math.Log(Inorg60) - 0.020);

            if (PECalc < 0) PECalc = 0;
            if (PECalc > 100.0) PECalc = 100.0;
            if (PECalc > PercentEmbedded) PercentEmbedded = PECalc;

            LastPctEmbedCalc = TPresent;
        }

        public void DoThisEveryStep(double hdid)
        {
            // Procedure runs after the derivatives have completed each time step
            int CurrentYearNum;
            // -----------------------------------------------------------------

            foreach (TStateVariable TSV in SV)
                DoThisEveryStep_CheckSloughEvent(TSV);

            DoThisEveryStep_UpdateLightVals();   // update light history values for calculating effects
            DoThisEveryStep_UpdateO2Concs();   // update oxygen concentration history for calculating effects
            DoThisEveryStep_UpdateSedConcs(); // update sediment conc. history for calculating effects

            DoThisEveryStep_MultiFishPromote(hdid);
            DoThisEveryStep_FishRecruit();     // add effects of recruitment to all fish vars.  Must be called after multifish promote.

            //DoThisEveryStep_Anadromous_Migr();
            //if (GetStatePointer(AllVariables.Sand, T_SVType.StV, T_SVLayer.WaterCol) != null)
            //    Update_Sed_Bed(TPresent - TPreviousStep);
            //// JSC 2-21-2003, Update sediment bed after each derivative step if sediment model is running

            // After every step, PrevFracKill must be set to Current FracKill for
            // correct computation of POISONED
            // Also, for each animal species spawning data must be updated
            foreach (TStateVariable TSV in SV)
                DoThisEveryStep_SetFracKilled_and_Spawned(TSV, hdid);

            DoThisEveryStep_SumAggr();

            int dayspassed = (TPresent - ModelStartTime).Days;
            CurrentYearNum = (int)((dayspassed + 2.0) / 365.0) + 1;
            if (CurrentYearNum > YearNum_PrevStep)   // (!EstuarySegment && (CurrentYearNum > YearNum_PrevStep))
            {
                TVolume PV = GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                PV.SetMeanDischarge(TPresent); // update meandischarge calculation each year
            }     

            // days
            if ((TPresent - LastPctEmbedCalc).TotalDays > 60)
                DoThisEveryStep_CalculatePercentEmbedded();  

            YearNum_PrevStep = CurrentYearNum;

            //{   ProgData.AnoxicVis = Anoxic;} 
        }

        // ------------------------------------------------------------------------
        // Cash-Karp RungeKutta with adaptive stepsize.
        // 
        // The Integrate function steps from the beginning to the end of the
        // time period and handles bookkeeping at the start and between steps
        // ------------------------------------------------------------------------
        public string Integrate(DateTime TStart, DateTime TEnd, double RelError, double h_minimum, double dxsav)
        {
            // parameters above -- Starting Point of Integral; Ending Point of Integral; Requested Accuracy of Results; Smallest Step Size; Store-Result Interval

            DateTime x;
            double hnext = 0;
            //  DateTime xsav;
            int lastprog = -1;
            bool simulation_done;
            //  bool FinishPoint;
            double MaxStep;
            double h_taken = 0;
            double h;
            // numsteps         : integer;
            // sumsteps, avgstep: double;
            // (*  numsteps = 0;  {debug code}
            // sumsteps = 0;  {debug code} *)
            //            rk_has_executed = false;
            simulation_done = false;
            if (dxsav < 0.01)
            {
                dxsav = 1;
            }
            // ensure dxsave <> 0, which causes crash
            // (**  Initialize variables........*)
            x = TStart;


            if (PSetup.ModelTSDays) MaxStep = 1.0;
                               else MaxStep = 1.0 / 24.0;  // Hourly
            h = MaxStep;

            SimulationDate = DateTime.Now;

            ModelStartTime = TStart;
            // TPreviousStep = TStart;
            TPresent = TStart;

            ChangeData();
            NormDiff(h);

            Derivs(x, 1);   
            WriteResults(TStart); // Write Initial Conditions as the first data Point
            CalcPPB();

            // (**  Start stepping the RungeKutta.....**)
            while (!simulation_done)
            {
                Integrate_CheckZeroStateAllSVs();
                Derivs(x, 1);

                foreach (TStateVariable TSV in SV)
                {
                    Integrate_SetYScale(TSV);
                }
                //                  FinishPoint = (Convert.ToInt32(x * (1.0 / dxsav)) > Convert.ToInt32(xsav * (1.0 / dxsav)));

                CalcPPB();
                WriteResults(x); // Write output to Results Collection

                //                    if (FinishPoint) // if it is time to write rates
                //                    {      xsav = x;      }

                if ((x.AddDays(h) - TEnd).TotalDays > 0.0)
                {
                    // if stepsize can overshoot, decrease
                    h = (TEnd - x).TotalDays;
                }

                //                else if (SV.LinkedMode && ((x + h) > Convert.ToInt32(x + 1)))
                //                {
                //                    h = (Convert.ToInt32(x + h) - x);
                //                }
                // force steps to stop on even one-day increments.
                // This is required for cascade segments to preserve mass balance (given output avg.)

                AdaptiveStep(ref x, h, RelError, ref h_taken, ref hnext, MaxStep);
                //                rk_has_executed = true;

                TPresent = x;
                DoThisEveryStep(h_taken);
                CalcPPB();

                if ((x - TEnd).TotalDays >= 0.0)
                {
                    // are we done?
                    simulation_done = true;
                }
                else if ((Math.Abs(hnext) < h_minimum))
                {
                    h = h_minimum;  // attempt to control min. timestep
                }
                else
                {
                    h = hnext;
                }

                if (ProgWorker != null)
                {
                    int progint = (int)Math.Round(100 * ((x - TStart) / (TEnd - TStart)));
                    if (progint==lastprog) ProgWorker.ReportProgress(progint);
                    lastprog = progint;
                }

                Integrate_CheckZeroStateAllSVs();

                void Integrate_SetYScale(TStateVariable p)
                {
                    if (p.State == 0)
                    {
                        p.yscale = 0;
                    }
                    else
                    {
                        p.yscale = Math.Abs(p.State) + Math.Abs(p.StepRes[1] * h) + Consts.Tiny;
                    }
                    // Scale of state variable used to assess relative error, 12/24/96

                }

            }

            DoThisEveryStep(h_taken);
            CalcPPB();

            WriteResults(x); // Write final step to Results Collection
            return PostProcessResults();

        }  // integrate


        // *************************************
        // Modifies Concentration to
        // account for a change in volume
        // Executed after successful rkqs step
        // coded by JSC, 10/5/99
        // Added pore waters, 11/29/00
        // Added delta thermocline 7/16/07
        // *************************************

        public void Perform_Dilute_or_Concentrate()
        {
            double Vol_Prev_Step;
            double NewVolume;
            double FracChange;
            double VolInitCond;

            TVolume TV = (TVolume)(GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol));
            if (TV == null) return;

            VolInitCond = TV.InitialCond;
            NewVolume = TV.State;

            //// Check for Water Volume Zero and Move On
            //if (NewVolume <= VolInitCond * Location.Locale.Min_Vol_Frac)
            //    {
            //        WaterVolZero = true;
            //        Water_Was_Zero = true;
            //        Volume_Last_Step = GetState(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            //        return;
            //    }

            //if (Water_Was_Zero)  { Volume_Last_Step = Last_Non_Zero_Vol;  }

            // // Recover from Water Volume Zero State
            // Water_Was_Zero = false;
            // WaterVolZero = false;

            Vol_Prev_Step = Volume_Last_Step;

            //Stratification code removed here 

            NewVolume = GetState(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            Volume_Last_Step = NewVolume;
            //LossVol = 0;
            //GainVol = 0;
            //PrevSegVol = 0;
            //NewSegVol = 0;
            //          NewVolFrac = 1;

            // Stratification code removed here 

            //          Last_Non_Zero_Vol = NewVolume;
            FracChange = (NewVolume / Vol_Prev_Step);
            //          VolFrac_Last_Step = NewVolFrac;

            TV.DeltaVolume();  // Update SegVolum Calculations

            foreach (TStateVariable TSV in SV)
            {
                // PERFORM DILUTE-CONCENTRATE
                if (TSV.Layer == T_SVLayer.WaterCol)
                    if ( ((TSV.NState>=Globals.Consts.FirstBiota)&&(TSV.NState <= Globals.Consts.LastBiota)) ||
                         ((TSV.NState >= AllVariables.Ammonia) && (TSV.NState <= Globals.Consts.LastDetr)) ||
                         ((TSV.NState == AllVariables.H2OTox) && (!PSetup.ChemsDrivingVars)) )
                    {
                        TSV.State = TSV.State / FracChange;
                    }
                // dilute/concentrate
            }

            //if ((LossVol > 0) || (GainVol > 0))  Dynamic Stratification N/A to HMS
            //        {
            //            for (i = 0; i < Count; i++)
            //            {
            //                PSV = At(i);
            //                if (PSV.Layer == T_SVLayer.WaterCol)
            //                {
            //                    if ((new ArrayList(new object[] { Consts.FirstBiota, Consts.FirstTox, AllVariables.Ammonia }).Contains(PSV.NState)) && !((PSV.NState >= Consts.FirstTox && PSV.NState <= Consts.LastTox) && SetupRec.ChemsDrivingVars))
            //                    {
            //                        // move water based on delta z thermocline 7-27-07
            //                        MassT0 = PSV.State * PrevSegVol;
            //                        // g
            //                        // g/m3
            //                        // m3
            //                        if (VSeg == VerticalSegments.Epilimnion)
            //                        {
            //                            OtherSegState = HypoSegment.GetState(PSV.NState, PSV.SVType, PSV.Layer);
            //                        }
            //                        else
            //                        {
            //                            OtherSegState = EpiSegment.GetState(PSV.NState, PSV.SVType, PSV.Layer);
            //                        }
            //                        if (LossVol > 0)
            //                        {
            //                            mass = (PSV.State * PrevSegVol) - (LossVol * PSV.State);
            //                        }
            //                        else
            //                        {
            //                            mass = (PSV.State * PrevSegVol) + (GainVol * OtherSegState);
            //                        }
            //                        PSV.State = mass / NewSegVol;
            //                        Perform_Dilute_or_Concentrate_Track_Nutrient_Exchange(PSV.NState, mass - MassT0, WorkingTStates);
            //                        // net mass transfer
            //                    }
            //                }
            //            }
            //        }

            //        if ((LossVol > 0) || (GainVol > 0)) TV.DeltaVolume();

            // pore waters also dilute/concentrate  N/A HMS
            //for (LayerLoop = T_SVLayer.SedLayer1; LayerLoop <= Consts.LowestLayer; LayerLoop++)
            //{
            //    if (GetStatePointer(AllVariables.PoreWater, T_SVType.StV, LayerLoop) != null)
            //    {
            //        NewVolume = GetState(AllVariables.PoreWater, T_SVType.StV, LayerLoop);
            //        if (NewVolume * SV.SedLayerArea() > Consts.Tiny)
            //        {
            //            // m3/m2
            //            // m2
            //            if (PWVol_Last_Step[LayerLoop] < Consts.Tiny)
            //            {
            //                FracChange = 1;
            //            }
            //            else
            //            {
            //                FracChange = (NewVolume / PWVol_Last_Step[LayerLoop]);
            //            }
            //            if (FracChange != 1.0)
            //            {
            //                for (VarLoop = AllVariables.PoreWater; VarLoop <= AllVariables.LaDOMPore; VarLoop++)
            //                {
            //                    for (ToxLoop = T_SVType.StV; ToxLoop <= Consts.LastToxTyp; ToxLoop++)
            //                    {
            //                        PSV = GetStatePointer(VarLoop, ToxLoop, LayerLoop);
            //                        if ((PSV != null) && !((VarLoop == AllVariables.PoreWater) && (ToxLoop == T_SVType.StV)))
            //                        {
            //                            PSV.State = PSV.State / FracChange;
            //                        }
            //                    }
            //                }
            //            }
            //            PWVol_Last_Step[LayerLoop] = NewVolume;
            //        }
            //        else
            //        {
            //            if (PWVol_Last_Step[LayerLoop] > 0)
            //            {
            //                // need to handle residual toxicant dissolved in pore water
            //                for (PoreLoop = AllVariables.PoreWater; PoreLoop <= AllVariables.LaDOMPore; PoreLoop++)
            //                {
            //                    for (ToxLoop = T_SVType.StV; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
            //                    {
            //                        PTox = SV.GetStatePointer(PoreLoop, ToxLoop, LayerLoop);
            //                        if (PTox != null)
            //                        {
            //                            if (ToxLoop > T_SVType.StV)
            //                            {
            //                                PWater = SV.GetStatePointer(Consts.AssocToxSV(ToxLoop), T_SVType.StV, T_SVLayer.WaterCol);
            //                                PWater.State = PWater.State + PTox.State * SV.PWVol_Last_Step[LayerLoop] * SV.SedLayerArea() / SV.SegVol();
            //                            }
            //                            PTox.State = 0;
            //                        }
            //                    }
            //                }
            //            }
            //            PWVol_Last_Step[LayerLoop] = 0;
            //            // newvolume < tiny
            //        }
            //    }

        }

        // -------------------------------------------------------------------------------------------------------------------------------
        public void WriteResults(DateTime TimeIndex)
        {
            double res = 0;
            if ((SV.restimes.Count == 0) || (TimeIndex - SV.restimes[^1]).TotalDays > Consts.VSmall)   // last element is ^1
            {
                SV.restimes.Add(TimeIndex);
                foreach (TStateVariable TSV in SV) if (TSV.TrackResults)
                {
                    res = TSV.State;
                    if (Convert_g_m2_to_mg_L(TSV.NState, TSV.SVType, TSV.Layer))
                    {
                        res = res * SegVol() / SurfaceArea();
                        //  g/m2  g/m3     m3         m2
                    }

                    if ((TSV.SVType >= Consts.FirstOrgTxTyp) && (TSV.SVType <= Consts.LastOrgTxTyp))  //fixme output chem PPB
                        res = ((TToxics)TSV).ppb; 

                    TSV.Results.Add(res);
                }
            }
        }

        public bool Convert_g_m2_to_mg_L(AllVariables S, T_SVType T, T_SVLayer L)
        {
            bool Convert;
            Convert = false;
            TStateVariable P = GetStatePointer(S, T, L);
            if ((L == T_SVLayer.WaterCol)) //  && (P != null))
            {
                // Fish must be converted from mg/L to g/sq.m
                if (((S >= Consts.FirstFish && S <= Consts.LastFish) && (T == T_SVType.StV))) Convert = true;  

                // Sedimented Detritus must be converted from mg/L to g/sq.m
                if (((S == AllVariables.SedmRefrDetr) || (S == AllVariables.SedmLabDetr)) && (T == T_SVType.StV)) Convert = true;

                // Periphyton & Macrophytes must be converted from mg/L to g/sq.m  
                if ((T == T_SVType.StV) && (P.IsPlant())) 
                {  if ((((P) as TPlant).PAlgalRec.PlantType != "Phytoplankton")) Convert = true; }

                // ZooBenthos and nekton must be converted from mg/L to g/sq.m  
                if ((T == T_SVType.StV) && (P.IsAnimal()))
                   { if (!((P) as TAnimal).IsPlanktonInvert()) Convert = true; }

                if ((S >= AllVariables.Veliger1 && S <= AllVariables.Veliger2)) Convert = false; 
            }

            //if ((T == T_SVType.OtherOutput) && (((TAddtlOutput)(S)) == TAddtlOutput.MultiFishConc)) convert = true;
            // Sum of multifish concs. needs to be converted for output

            return Convert;
        }

        // ----------------------------------------------------------------------
        public double SurfaceArea()
        {
              // Surface area of segment or individual layer if stratified

            return Location.Locale.SurfArea;

            //if (!LinkedMode && Stratified && (Location.Locale.UseBathymetry))
            //{
            //    SiteRecord LL = Location.Locale;
            //    double EpiFrac = LL.AreaFrac(Location.MeanThick[VerticalSegments.Epilimnion], LL.ZMax);
            //    if (VSeg == VerticalSegments.Epilimnion)
            //    {
            //        result = result * EpiFrac;
            //    }
            //    else
            //    {
            //        result = result * (1.0 - EpiFrac);
            //    }
            //}
        }

        // ----------------------------------------------------------------------
        public double Ice_Cover_Temp()
        {
            
            double Sal;
            switch (Location.SiteType)
            {
                case SiteTypes.Estuary:
                case SiteTypes.Marine:
                    Sal = GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
                    if (Sal > 0.0)
                    {
                        return (-0.0575 * Sal) + (0.001710523 * Math.Pow(Sal, 1.5)) - (0.0002154996 * Math.Pow(Sal, 2)); // UNESCO (1983), 4/8/2015
                    }
                    else return -1.8;  // default if salinity state variable not found {Ocean water with a typical salinity of 35 parts per thousand freezes only at -1.8 degC (28.9 deg F).}
                case SiteTypes.Stream:
                    return 0.0;  // Temperature at which ice cover occurs in moving water
                default:
                    return 3.0;  // Temperature at which ice cover occurs in fresh water
            }    // case
        }


        public double TrapezoidalIntegration(out string ErrMsg, DateTime Start_Interval_Time, DateTime End_Interval_Time, List<double> vals, int rti)
        {
            double End_SI_Val;  // ending sub-interval value
            double Start_SI_Val; // starting sub-interval value
            double SumThusFar = 0; // running sum of trapezoid areas

            ErrMsg = "";
            if (rti <= 0) { ErrMsg = "TrapezoidalIntegration index error, rti<=0"; return -99; };
            if (rti > vals.Count - 1) { ErrMsg = "TrapezoidalIntegration index error rti >count"; return -99; };
            bool firststep = true;

            for (int i = rti - 1; (firststep || SV.restimes[i] < End_Interval_Time); i++)
            {
                firststep = false;
                DateTime Start_SI_Time = SV.restimes[i];
                DateTime End_SI_Time = SV.restimes[i + 1];
                Start_SI_Val = vals[i];
                End_SI_Val = vals[i + 1];

                if (End_SI_Time > End_Interval_Time)
                {
                    // Linearly interpolate to get the end sub-interval point
                    End_SI_Val = LinearInterpolate(out ErrMsg, Start_SI_Val, End_SI_Val, Start_SI_Time, End_SI_Time, End_Interval_Time);
                    if (ErrMsg != "") return -99;
                    End_SI_Time = End_Interval_Time;
                }

                if (Start_SI_Time < Start_Interval_Time)
                {
                    // Linearly interpolate to get the beginning sub-interval point
                    Start_SI_Val = LinearInterpolate(out ErrMsg, Start_SI_Val, End_SI_Val, Start_SI_Time, End_SI_Time, Start_Interval_Time);
                    if (ErrMsg != "") return -99;
                    Start_SI_Time = Start_Interval_Time;
                }

                SumThusFar = SumThusFar + ((Start_SI_Val + End_SI_Val) / 2.0) * (End_SI_Time - Start_SI_Time).TotalDays;
                // The area of the relevant trapezoid is calculated above

            }

            return SumThusFar / (End_Interval_Time - Start_Interval_Time).TotalDays;
        }

        public double LinearInterpolate(out string ErrMsg, double OldVal, double NewVal, DateTime OldTime, DateTime NewTime, DateTime InterpTime)
        {
            ErrMsg = "";
            // Interpolates to InterpTime between two points, OldPoint and NewPoint
            if ((InterpTime > NewTime) || (InterpTime < OldTime)) { ErrMsg = "Linear Interpolation Timestamp Error"; return -99; };

            return OldVal + ((NewVal - OldVal) / (NewTime - OldTime).TotalDays) * (InterpTime - OldTime).TotalDays;
            // y1    // Slope  (dy/dx)                                      // Delta X
        }

        public double InstantaneousConc(out string ErrMsg, DateTime steptime, List<double> vals, int rti)
        {
            ErrMsg = "";
            if (rti <= 0) { ErrMsg = "Linear interpolation index error, rti<=0"; return -99; };
            if (rti > vals.Count - 1) { ErrMsg = "Linear interpolation index error rti >count"; return -99; };

            double OldVal = vals[rti - 1];
            double NewVal = vals[rti];
            DateTime OldTime = SV.restimes[rti - 1];
            DateTime NewTime = SV.restimes[rti]; 

            return LinearInterpolate(out ErrMsg, OldVal, NewVal, OldTime, NewTime, steptime);
        }



        public string PostProcessResults()
        {
            double val;
            string errmsg = "";
            double stepsize = (PSetup.StoreStepSize);  // step size in days or hours
            if (!PSetup.StepSizeInDays) stepsize = stepsize / 24;  // convert to step size in days
            int NumDays = (PSetup.LastDay - PSetup.FirstDay).Days; // number of days in simulation
            int numsteps = (int)(NumDays / stepsize); // number of time-steps to be written

            if (numsteps <= 0) return "Zero time-steps are written given StoreStepSize and StepSizeInDays flag.";
            if (SV.restimes == null) return "Results Times not initialized for SV List";
            if (SV.restimes.Count == 0) return "No results times saved for SV SV List";

            DateTime lastwritedate = PSetup.FirstDay.AddDays(numsteps * stepsize);

            List<int> StartIndices = new List<int>();  //restime index to start linear interpolation or trapezoidal integration
            int stepindex = 1;
            for (int i = 0; i < SV.restimes.Count; i++)
            {
                DateTime DateToFind = PSetup.FirstDay.AddDays(stepindex * stepsize);
                if (PSetup.AverageOutput) //trapezoidal integration
                {
                    if (DateToFind < SV.restimes[i].AddDays(stepsize))  // one time step before the reporting time step for integration
                    {
                        StartIndices.Add(i);
                        stepindex++; i--;  //try this same i for the next stepindex before continuing
                    }
                }
                else
                {
                    if (DateToFind <= SV.restimes[i])
                    {
                        StartIndices.Add(i);
                        stepindex++; i--;
                    }

                }

            }

            foreach (TStateVariable TSV in SV) if (TSV.TrackResults)
                {
                    if (TSV.Results == null) return "Results not initialized for SV " + TSV.PName;
                    if (TSV.Results.Count == 0) return "No results saved for SV " + TSV.PName;

                    TSV.output = new AQUATOXTSOutput();
                    TSV.output.Dataset = TSV.PName;
                    TSV.output.DataSource = "AQUATOX";
                    TSV.output.Metadata = new Dictionary<string, string>()
                {
                    {"AQUATOX_HMS_Version", "1.0.0"},
                    {"SimulationDate", (SimulationDate.ToString(Consts.DateFormatString))},
                    {"Result Unit", TSV.StateUnit},
                };

                    TSV.output.Data = new Dictionary<string, List<string>>();
                    List<string> vallist = new List<string>();
                    vallist.Add(TSV.Results[0].ToString(Consts.ValFormatString));
                    TSV.output.Data.Add(SV.restimes[0].ToString(Consts.DateFormatString), vallist);
                    for (int i = 1; i <= numsteps; i++)
                    {
                        DateTime steptime = PSetup.FirstDay.AddDays(i * stepsize);
                        if (PSetup.AverageOutput) val = TrapezoidalIntegration(out errmsg, steptime.AddDays(-stepsize), steptime, TSV.Results, StartIndices[i - 1]);
                        else val = InstantaneousConc(out errmsg, steptime, TSV.Results, StartIndices[i - 1]);
                        vallist = new List<string>();
                        vallist.Add(val.ToString(Consts.ValFormatString));
                        TSV.output.Data.Add(steptime.ToString(Consts.DateFormatString), vallist);
                        if (errmsg != "") return errmsg;
                    }
                }
                else TSV.output = null;  // if !TrackResults
            return "";
        }

        public void CalculateAllLoads(DateTime TimeIndex)
        // If EstuarySegment then TSalinity(GetStatePointer(Salinity, StV, WaterCol)).CalculateLoad(TimeIndex); {get salinity vals for salt balance}

        {
            double junk = 0;
            TVolume TV = (TVolume)GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            TV.CalculateLoad(TimeIndex);
            TV.Derivative(ref junk);
            foreach (TStateVariable TSV in SV)
                TSV.CalculateLoad(TimeIndex);
        }


                // ----------------------------------------------------------
        public void CopySuspDetrData()
        {
            // Copies Data stored within DissRefrDetr into the Initial Conditions of the two or four detrital
            // compartments: supports SuspDetr input Interface
            TRemineralize PR;
            TDissRefrDetr PD;
            AllVariables Loop;
            T_SVType SVLoop;
            double MultFrac;
            // ----------------------------------------------------------        

            void CalcMultFrac(AllVariables NS, T_SVType S_Type)
            {
                // Gets the correct fraction to multiply the general loadings
                // data by to fit the appropriate compartment / data type
                double ConvertFrac, RefrFrac, PartFrac, RefrPercent, PartPercent;

                MultFrac = 1.0;
                ConvertFrac = 1.0;
                if (S_Type > T_SVType.StV) return;     // Tox data is ppb, so it stays const. for four compartments regardless of breakdown

                RefrPercent = PD.InputRecord.Percent_RefrIC;
                PartPercent = PD.InputRecord.Percent_PartIC;
                if (NS >= AllVariables.DissRefrDetr && NS <= AllVariables.DissLabDetr)
                     PartFrac = 1.0 - (PartPercent / 100.0);
                else PartFrac = (PartPercent / 100.0);

                if ((NS == AllVariables.DissRefrDetr) || (NS == AllVariables.SuspRefrDetr))
                {      RefrFrac = (RefrPercent / 100.0);      }
                else { RefrFrac = 1.0 - (RefrPercent / 100.0);  }

                switch (PD.InputRecord.DataType)
                {   case DetrDataType.CBOD:
                        ConvertFrac = Location.Conv_CBOD5_to_OM(RefrPercent);
                        break;
                    case DetrDataType.Org_Carb:
                        ConvertFrac = Consts.Detr_OM_2_OC;
                        break;
                }

                MultFrac = ConvertFrac * RefrFrac * PartFrac;
            }

        // ---------------------------------------------------------------

            PD = GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDissRefrDetr;
            if (PD != null)
              for (SVLoop = T_SVType.StV; SVLoop <= Consts.LastOrgTxTyp; SVLoop++)
                {
                    // Loop through state variable type and then each associated toxicant type
                    if (SVLoop != T_SVType.Porewaters)
                    {
                        if ((SVLoop == T_SVType.StV) || GetStatePointer(AllVariables.H2OTox, SVLoop, T_SVLayer.WaterCol) != null)
                        {
                            for (Loop = AllVariables.DissRefrDetr; Loop <= AllVariables.SuspLabDetr; Loop++)
                            {  // Loop through each detritus record in water col.

                                CalcMultFrac(Loop, SVLoop);   // Determine MultFrac for Initial condition

                                PR = GetStatePointer(Loop, SVLoop, T_SVLayer.WaterCol) as TRemineralize;
                                if (PR != null)
                                {
                                    DetritalInputRecordType PDIR = PD.InputRecord;
                                    if (SVLoop == T_SVType.StV)
                                        {
                                            PR.InitialCond = PDIR.InitCond * MultFrac;
                                            PR.LoadsRec.Loadings.Hourly = PDIR.Load.Loadings.Hourly;
                                        }
                                    else PR.InitialCond = PDIR.ToxInitCond[PR.ToxInt(SVLoop)];
                                }
                            }
                        }
                    }
                }
        }

        // ---------------------------------------------------------------

        public void SVsToInitConds()
        {
            CopySuspDetrData();

            TVolume TV = (TVolume)GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);

            foreach (TToxics P in SV.OfType<TToxics>()) 
                P.ChemRec = ((TToxics)GetStatePointer(AllVariables.H2OTox, P.SVType, T_SVLayer.WaterCol)).ChemRec;

            TV.SetToInitCond();
            foreach (TStateVariable TSV in SV)
                TSV.SetToInitCond();

            for (int ToxLoop = 0; ToxLoop < Consts.NToxs; ToxLoop++)
            {
                FirstExposure[ToxLoop] = DateTime.MinValue;
            }

            PO2Concs.Clear();
            PSedConcs.Clear();
            PLightVals.Clear();

            TLight PL = GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol) as TLight;
            if (PL != null)
            {
                PL.CalculateLoad(PSetup.FirstDay);  // Set DailyLight for First Day
                double MaxDailyLight = PL.DailyLight;
                for (DateTime LightTest = PSetup.FirstDay.Date.AddDays(1); LightTest <= PSetup.FirstDay.Date.AddDays(365); LightTest = LightTest.AddDays(1))
                {   // Get Maximum daily light for one year from simulation start point
                    PL.CalculateLoad(LightTest); // Set DailyLight for "LightTest" Day
                    if (MaxDailyLight < PL.DailyLight) MaxDailyLight = PL.DailyLight;
                }
                PL.CalculateLoad(PSetup.FirstDay);  // Reset DailyLight for First Day

                for (AllVariables NS = Consts.FirstPlant; NS <= Consts.LastPlant; NS++)
                {
                    TPlant PP = GetStatePointer(NS, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if (PP != null)
                    {
                        if (PP.PAlgalRec.EnteredLightSat >= MaxDailyLight) 
                        {
                            PP.ZOpt = 0.1;  //  towards top of water column due to low light conditions
                        }
                        else
                        {
                            PP.ZOpt = Math.Log(PP.PAlgalRec.EnteredLightSat / MaxDailyLight) / -Extinct(PP.IsPeriphyton(), true, true, false, 0);
                        }                                  //   (Ly/d)            (Ly/d)         (1/m)
                    }
                }
            }

            PercentEmbedded = Location.Locale.BasePercentEmbed;
            LastPctEmbedCalc = PSetup.FirstDay;

            SOD = -99;
            YearNum_PrevStep = 0;
        }


        public double DynamicZMean()
        {
            // Variable ZMean of segment or both segments if dynamic stratification
            if (!Location.Locale.UseBathymetry)
                return Volume_Last_Step / Location.Locale.SurfArea;

            if (UseConstZMean)
                return Location.Locale.ICZMean;

            if (DynZMean != null) return DynZMean.ReturnTSLoad(TPresent);  // time series only

            return Location.Locale.ICZMean;  // DynZMean is null
        }

        // variable zmean of seg. or entire system if stratified
        public double StaticZMean()
        {
         
            TVolume PVol;
            if (Location.Locale.UseBathymetry)
            {
                // zmean does not vary in this case
                return Location.Locale.ICZMean;
            }
            else
            {
                PVol = GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                return PVol.InitialCond / Location.Locale.SurfArea;    // Initial zmean based on const surf area over vertical profile
                // m         // m3                         // m2
            }
        }



        public void SetupLinks()
        {
            foreach (TStateVariable TSV in SV)
            {
                TSV.StepRes = new double[7];
                TSV.AQTSeg = this;
                TSV.Location = Location;
            }
        }

        public double GetState(AllVariables S, T_SVType T, T_SVLayer L)
        {
            TStateVariable p;
            p = GetStatePointer(S, T, L);
            if (!(p == null)) { return p.State; }
            else
            {
                throw new ArgumentException("GetState called for non-existant state variable: " + S.ToString(), "original");
                // result = -1;
            }
        }

        public double GetStateVal(AllVariables S, T_SVType T, T_SVLayer L)  // returns -1.0 if variable is non existant
        {
            TStateVariable p;
            p = GetStatePointer(S, T, L);
            if (!(p == null)) { return p.State; }
            else return -1.0;
        }

        public TStateVariable GetStatePointer(AllVariables S, T_SVType T, T_SVLayer L)
        {
            return MemLocRec[(int)S, (int)T, (int)L];
        }

        public void SetMemLocRec()
        {
            int NumS = Enum.GetNames(typeof(AllVariables)).Length;
            int NumT = Enum.GetNames(typeof(T_SVType)).Length;
            int NumL = Enum.GetNames(typeof(T_SVLayer)).Length;

            MemLocRec = new TStateVariable[NumS, NumT, NumL];

            foreach (TStateVariable TSV in SV)
                MemLocRec[(int)TSV.NState, (int)TSV.SVType, (int)TSV.Layer] = TSV;
        }


        public double SegVol()        // volume of segment or individual layer if stratified
        {
            return Volume_Last_Step;
            //if  (LinkedMode) or (!Stratified) return Volume_Last_Step; // or linked mode
            // else return Location.Morph.SegVolum[VSeg];
        }


        //// -------------------------------------------------------------------------------------------------------
        public double InorgSedConc()  // Removed code for weighted average in the event of linked-mode well-mixed stratification, 2-27-08
        {
            double InorgSed; // Conc Inorganic sediments in mg/L
            AllVariables DetrLoop;
            AllVariables AlgLoop;
            TSandSiltClay PTSS;
            TPlant PPhyto;

            InorgSed = 0;
            PTSS = GetStatePointer(AllVariables.TSS, T_SVType.StV, T_SVLayer.WaterCol) as TSandSiltClay;
            if (PTSS != null)
            {
                // If there is TSS in a simulation there are no cohesives nor sand..clay
                InorgSed = GetState(AllVariables.TSS, T_SVType.StV, T_SVLayer.WaterCol);

                if (PTSS.TSS_Solids)
                {
                    // TSS includes algae so, to avoid double-counting, this algorithm subtracts the phytoplankton biomass
                    // from the TSS ("inorganic sediment") before computing the extinction coeff.
                    for (AlgLoop = Consts.FirstAlgae; AlgLoop <= Consts.LastAlgae; AlgLoop++)
                    {
                        PPhyto = GetStatePointer(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                        if (PPhyto != null)
                        {
                            if (PPhyto.IsPhytoplankton())
                                InorgSed = InorgSed - GetState(AlgLoop, T_SVType.StV, T_SVLayer.WaterCol);       // 1/7/2005
                        }
                    }

                    for (DetrLoop = AllVariables.SuspRefrDetr; DetrLoop <= AllVariables.SuspLabDetr; DetrLoop++)
                    {
                        if (GetState(DetrLoop, T_SVType.StV, T_SVLayer.WaterCol) > 0)
                        {
                            InorgSed = InorgSed - GetState(DetrLoop, T_SVType.StV, T_SVLayer.WaterCol);
                        }
                    }
                }
                if (InorgSed < 0) InorgSed = 0;  // 10-18-07 bullet proofing
            }
            return InorgSed;
        }

        // inorganic seds in mg/L
        // -------------------------------------------------------------------------------------------------------
        public double InorgSed60Day(int MustHave60)
        {
            double result;
            // 60 day running average of inorganic seds, mg/L
            // 3/5/2008, if MustHave60=TRUE then returns zero if the data-record (simulation time) is less than 60 days
            bool OverTime;
            TSVConc PSS;
            int i;
            double RunningSum;
            double LastInorg;
            DateTime LastTime;

            if (TimeLastInorgSedAvg[MustHave60].Date == TPresent.Date) // optimization, only calculate once per day
            {                
                result = LastInorgSedAvg[MustHave60];
                return result;
            }
            LastInorg = InorgSedConc();

            if (MustHave60==1) result = 0;
                         else  result = LastInorg;              // Used if Count of historical data =0

            LastTime = TPresent;
            OverTime = false;
            i = 0;
            RunningSum = 0;
            if (PSedConcs.Count > 0)
            {
                do
                {
                    PSS = PSedConcs[i];
                    if ((TPresent - PSS.Time).TotalDays > 60.001)
                    {
                        OverTime = true;
                    }

                    RunningSum = RunningSum + ((PSS.SVConc + LastInorg) / 2.0) * (LastTime - PSS.Time).TotalDays;
                    // mg/L d      // mg/L        // mg/L      // mg/L                                  // d
                    // trapezoidal integration

                    LastTime = PSS.Time;
                    LastInorg = PSS.SVConc;
                    i++;
                } while (!((i == PSedConcs.Count) || OverTime));

                if ((TPresent - LastTime).TotalDays > Consts.Tiny) // must have time - record to process
                {
                    if (((TPresent - LastTime).TotalDays < 60.0) && (MustHave60==1))  // not 60 days of data
                        result = 0.0;
                    else
                        result = RunningSum / (TPresent - LastTime).TotalDays;
                }                  // mg/L d            // d
            } // Count>0

            TimeLastInorgSedAvg[MustHave60] = TPresent;
            LastInorgSedAvg[MustHave60] = result;
            return result;
        }

        // -------------------------------------------------------------------------------------------------------
        public double InorgSedDep()
        {
            double result;
            double SS60Day = InorgSed60Day(0);  // function is used for both filter-feeding dilution and benthic drift trigger 
            if (SS60Day<Consts.Tiny)  result = 0;
              else result = 0.270 * Math.Log(SS60Day) - 0.072;  // ln
                         // kg/m2 day         mg/L }

            if (result <0) result = 0;
            return result;
        }



        // ***  Process Equations  ***
        // *******************************************************
        // Stroganov Function
        // Limits the rate for non-optimal temp - biolog10ical
        // based on O'Neill et al., 1972; Kitchell et al., 1972
        // and Bloomfield et al., 1973
        // *******************************************************
        public double TCorr(double Q10, double TRef, double TOpt, double TMax)
        {
            
            const double XM = 2.0;
            const double KT = 0.5;
            const double Minus = -1.0;
            // tcorr
            double Sign = 1.0;
            // initialize
            double Temp = GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if ((Temp - TRef) < 0.0) Sign = Minus;

            double Acclimation = Sign * XM * (1.0 - Math.Exp(-KT * Math.Abs(Temp - TRef)));
            // Kitchell et al., 1972
            double TMaxAdapt = TMax + Acclimation;
            // C          C     C
            double TOptAdapt = TOpt + Acclimation;
            if (Q10 <= 1.0)
            {
                // JSC added "=" to "<=" otherwise YT=0, division by zero
                Q10 = 2.0;
            }
            // rate of change per 10 degrees
            if (TMaxAdapt <= TOptAdapt)
            {
                TMaxAdapt = TOptAdapt + Consts.VSmall;
            }
            double WT = Math.Log(Q10) * (TMaxAdapt - TOptAdapt);
            double YT = Math.Log(Q10) * (TMaxAdapt - TOptAdapt + 2.0);
            // NOT IN CEM MODELS
            double XT = (Math.Pow(WT, 2.0) * Math.Pow(1.0 + Math.Sqrt(1.0 + 40.0 / YT), 2)) / 400.0;
            double VT = (TMaxAdapt - Temp) / (TMaxAdapt - TOptAdapt);
            if (VT < 0.0) return 0.0;
            else return Math.Pow(VT, XT) * Math.Exp(XT * (1.0 - VT));     // unitless
        }


        public double WaterDensity(bool Reference, double KSTemp, double KSSalt)
        {
            
            double Salt;
            double Temp;
            if (Reference)
            {
                Temp = KSTemp;
                Salt = KSSalt;
            }
            else
            {   // ambient
                Temp = GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
                Salt = GetState(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            }

            return 1.0 + (1E-3 * (28.14 - (0.0735 * Temp) - (0.00469 * Math.Pow(Temp, 2.0)) + (0.802 - (0.002 * Temp)) * (Salt - 35.0)));
        }

        // mortality
        public double DensityFactor(double KSTemp, double KSSalt)
        {
            if (GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol) == null) return 1;
            return WaterDensity(true, KSTemp, KSSalt) / WaterDensity(false, KSTemp, KSSalt);
        }

        //// ---------------------------------------------------------------


        public double Velocity(double pctriffle, double pctpool, bool averaged)
        {
            double xsecarea, avgflow;
            double upflow, downflow;
            double pctrun, runvel, rifflevel, poolvel;
            double vol;
            double width, channel_depth;
            // ----------------------------------------------------------------------------------------------------

            if (averaged) vol = MeanVolume;
            else vol = Volume_Last_Step;

            if (Location.SiteType == SiteTypes.Stream)
            {
                if (averaged) downflow = MeanDischarge;
                else downflow = Location.Discharge;

                double Avg_Disch = downflow / 86400;
                if (Avg_Disch<=0) Avg_Disch = Location.Discharge_Using_QBase() / 86400.0;
                //                { m3 / s}             { m3 / d}               {s / d}

                width = Location.Locale.SurfArea / (Location.Locale.SiteLength * 1000.0);
                //{ m}                  { sq.m}                { km}            { m / km}

                double slope = Math.Max(Location.Locale.Channel_Slope, 0.00001);
                channel_depth = Math.Pow(Avg_Disch * Location.ManningCoeff() / (Math.Sqrt(slope) * width), 0.6);

                xsecarea =  width * channel_depth;
                // m2       // m         // m
            }
            else xsecarea = vol / (Location.Locale.SiteLength * 1000);
                          // m3                    // km      // m/km
            
            pctrun = 100.0 - pctriffle - pctpool;
            if ((CalcVelocity || averaged))
            {
                upflow = Location.Morph.InflowH2O;  //vseg
                downflow = Location.Discharge;      //vseg
                if (averaged)
                {
                    upflow = MeanDischarge;        // m3/d
                    downflow = MeanDischarge;
                }
                avgflow = (upflow + downflow) / 2.0;
                // m3/d   // m3/d    // m3/d
                runvel = (avgflow / xsecarea) * (1.0 / 86400.0) * 100.0;
                // cm/s  // m3/d    // m2         // d/s       // cm/m

                if (runvel < 0) runvel = 0;
            }
            else
            {
                // user entered velocity
                runvel = DynVelocity.ReturnTSLoad(TPresent);  // cm/s
                avgflow = runvel * xsecarea * 86400 * 0.01;
                // m3/d  // cm/s   // m2    // s/d  // m/cm
            }
            if (avgflow < 2.59e5)
            {
                // q < 2.59e5 m3/d
                rifflevel = 1.60 * runvel;
                poolvel = 0.36 * runvel;
            }
            else if (avgflow < 5.18e5)
            {
                // 2.59e5 m3/d < q < 5.18e5 m3/d
                rifflevel = 1.30 * runvel;
                poolvel = 0.46 * runvel;
            }
            else if (avgflow < 7.77e5)
            {
                // 5.18e5 m3/d q < 7.77e5 m3/d
                rifflevel = 1.10 * runvel;
                poolvel = 0.56 * runvel;
            }
            else
            {
                // q >= 7.77e5 m3/d
                rifflevel = 1.00 * runvel;
                poolvel = 0.66 * runvel;
            }
            // cm/s

            return (rifflevel * (pctriffle / 100.0)) + (runvel * (pctrun / 100.0)) + (poolvel * (pctpool / 100.0));
        }


        // Diagenesis Model
        // -----------------------------------------------------------------------------------------

        // Is diagenesis model included?
        public bool Diagenesis_Included()
        {
            bool result;
            result = GetStatePointer(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.SedLayer2) != null;
            return result;
        }

        // -----------------------------------------------------------------------------------------
        public double Diagenesis(T_SVLayer Layer)
        {
            double s;
            double Jc;
            double Jn;
            AllVariables ns;
            TPOC_Sediment ppc;
            TNO3_Sediment PNO31;
            TNO3_Sediment PNO32;
            if ((Layer == T_SVLayer.SedLayer1)) return 0.0;

            // determine diagenesis fluxes
            Jc = 0;
            Diagenesis_Rec DR = Diagenesis_Params;
            for (ns = AllVariables.POC_G1; ns <= AllVariables.POC_G3; ns++)
            {
                ppc = (TPOC_Sediment)GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                Jc = Jc + ppc.Mineralization() * 32.0 / 12.0;
            }
            // gO2/ m3 d             // g C / m3             // g O2/ g C
            PNO31 = (TNO3_Sediment)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);
            PNO32 = (TNO3_Sediment)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
            s = MassTransfer();
            // m/d            // g/m3 d            // m2/d2            // m/d            // g/m3
            Jn = 2.86 * (PNO31.Denit_Rate() / s * PNO31.State + PNO32.Denit_Rate() * PNO32.State) / DR.H2.Val;
            // m/d            // g/m3            // m
            return Jc - Jn;
            // g O2/m3 d  // g O2/m3 d
        }

        // -----------------------------------------------------------------------------------------

        public double DiagenesisVol(int Layer)
        {
            double result;
            // Sed Volume Diagensis
            if (Layer == 1)
                result = Location.Locale.SurfArea * Diagenesis_Params.H1.Val;
            else
                result = Location.Locale.SurfArea * Diagenesis_Params.H2.Val;
            // m3                // m2                     // m


            // double EpiFrac;
            //if (Stratified)
            //{
            //    MorphRecord RR = Location.Morph;
            //    EpiFrac = Location.AreaFrac(Location.MeanThick[VerticalSegments.Epilimnion], Location.Locale.ZMax);
            //    // 10-14-2010 Note that ZMax parameter pertains to both segments in event of stratification
            //    if (VSeg == VerticalSegments.Epilimnion)
            //    {
            //        result = result * EpiFrac;
            //    }
            //    else
            //    {
            //        result = result * (1.0 - EpiFrac);
            //    }
            //}

            return result;
        }

        // vol of sed bed in m3
        // -----------------------------------------------------------------------------------------
        public double MassTransfer()
        {
            double result;  
            double O2;
            const double MAX_S = 1.0;
            // m/d
            O2 = GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((O2 < Consts.Tiny))
            {
                // max mass transfer if o2 goes to zero
                result = MAX_S;
            }
            else result = SOD / O2;
            // m/d =  go2/m2 d /  gO2/m3

            // mass transfer coeff
            if (result > MAX_S) result = MAX_S;
            if (result < Consts.Tiny) result = Consts.Tiny;
            // avoid divide by zero error

            return result;
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void CalcDeposition_SumDef(TStateVariable P, AllVariables NS, T_SVType Typ, ref double Def)
        {
            double Def2Detr;
            const double Def_to_G3 = 0.00;          // 0% of defecation to G3

            // all defecation goes to sediment
            if (NS == AllVariables.Avail_Silica)
            {
                return;
            }
            if (P.IsAnimal())
            {
                switch (NS)
                {
                    case AllVariables.PON_G1:
                    case AllVariables.POP_G1:
                    case AllVariables.POC_G1:
                        // G1 equivalent to labile
                        Def2Detr = Consts.Def2SedLabDetr * (1.0 - Def_to_G3);
                        break;
                    case AllVariables.PON_G2:
                    case AllVariables.POP_G2:
                    case AllVariables.POC_G2:
                        // G2 equivalent to refractory
                        Def2Detr = (1.0 - Consts.Def2SedLabDetr) * (1.0 - Def_to_G3);
                        break;
                    default:
                        Def2Detr = Def_to_G3;
                        break;
                        // G3 class is inert
                }
                var NFrac = NS switch
                {
                    AllVariables.PON_G1 => Location.Remin.N2OrgLab,    // Was PA.PAnimalData.N2Org, 6/6/2008, defecation has same nutrients as labile detritus
                    AllVariables.POP_G1 => Location.Remin.P2OrgLab,    // Was PA.PAnimalData.P2Org, 6/6/2008, defecation has same nutrients as labile detritus
                    _ => 1.0 / Consts.Detr_OM_2_OC,                      // Winberg et al. 1971, relevant to animals, non-macrophyte plants, bacteria
                };
                // Case
                TAnimal PA = P as TAnimal;
                if ((Typ == T_SVType.StV))
                    Def = Def + Def2Detr * PA.Defecation() * NFrac;
                // g/m3                 // g/m3

                else Def = Def + Def2Detr * PA.DefecationTox(Typ) * 1e3;  
                        // ug/m3 // unitless    // ug/L           // L/m3
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void CalcDeposition_SumSed(TStateVariable P, AllVariables NS, T_SVType Typ, ref double Sed)
        {
            const double PlantSinkLabile = 0.92;
            TPlant PP;    
            double NFrac;
            double Frac;
            // Ignore the N and P content in G3 because it is so small.
            // Currently No Macrophyte Linkage through mortality & breakage
            if ((P.SVType != T_SVType.StV))
            {
                return;
            }
            if (P.IsAlgae())
            {
                PP = ((P) as TPlant);
                switch (NS)
                {
                    case AllVariables.Avail_Silica:
                        Frac = 1.0;
                        break;
                    case AllVariables.PON_G1:
                    case AllVariables.POP_G1:
                    case AllVariables.POC_G1:
                        // G1 equivalent to labile
                        Frac = PlantSinkLabile;
                        break;
                    case AllVariables.PON_G2:
                    case AllVariables.POP_G2:
                    case AllVariables.POC_G2:
                        // G2 equivalent to refractory
                        Frac = (1.0 - PlantSinkLabile);
                        break;
                    default:
                        Frac = 0;
                        break;
                        // G3 class is inert, no plant sink to G3 for now
                }
                switch (NS)
                {
                    case AllVariables.Avail_Silica:
                        // Case
                        // taxonomic type diatoms
                        if (PP.NState >= Consts.FirstDiatom && PP.NState <= Consts.LastDiatom)
                            NFrac = Diagenesis_Params.Si_Diatom.Val;
                        else NFrac = 0;
                        break;
                    // Modify the A .. B: AllVariables.PON_G1 .. AllVariables.PON_G3
                    case AllVariables.PON_G1:
                        NFrac = PP.N_2_Org();
                        break;
                    // Modify the A .. B: AllVariables.POP_G1 .. AllVariables.POP_G3
                    case AllVariables.POP_G1:
                        NFrac = PP.P_2_Org();
                        break;
                    default:
                        // POCG1..POCG3:
                        NFrac = 1.0 / Consts.Detr_OM_2_OC;
                        break;
                        // Winberg et al. 1971, relevant to animals, non-macrophyte plants, bacteria
                }
                // case
                // in which case all deposition goes to the periphyton directly
                if (!PP.IsLinkedPhyto())
                {
                    if ((Typ == T_SVType.StV))
                    {
                        // g/m3                       // g/m3
                        Sed = Sed + PP.Sedimentation() * Frac * NFrac;
                    }
                    else  
                    { Sed = Sed + PP.Sedimentation() * Frac * GetPPB(PP.NState, Typ, PP.Layer) * 1e-3;
                    } // ug/m3           // g/m3             // ug/kg                            // kg/g
                }
                // not linkedphyto
            }  // algae

            // G1 equivalent to labile
            if (((P.NState == AllVariables.SuspLabDetr) && ((NS == AllVariables.PON_G1) || (NS == AllVariables.POP_G1) || (NS == AllVariables.POC_G1))) ||  //  G1 equivalent to labile
                ((P.NState == AllVariables.SuspRefrDetr) && ((NS == AllVariables.PON_G2) || (NS == AllVariables.POP_G2) || (NS == AllVariables.POC_G2) || (NS == AllVariables.POC_G3))))
            {
                if ((Typ == T_SVType.StV))
                {
                    ReminRecord RR = Location.Remin;
                    NFrac = NS switch
                    {
                        (AllVariables.PON_G1) => RR.N2OrgLab,
                        (AllVariables.PON_G2) => RR.N2Org_Refr,
                        (AllVariables.POP_G1) => RR.P2OrgLab,
                        (AllVariables.POP_G2) => RR.P2Org_Refr,
                        (AllVariables.POC_G3) => 1.0 / Consts.Detr_OM_2_OC * Diagenesis_Params.LigninDetr.Val,
                        (AllVariables.POC_G2) => 1.0 / Consts.Detr_OM_2_OC * (1.0 - Diagenesis_Params.LigninDetr.Val),
                        _ => NFrac = 1.0 / Consts.Detr_OM_2_OC,  // POCG1: 
                    };
                }
                else
                {
                    // If (Typ <> StV) then
                    NFrac = NS switch
                    {
                        AllVariables.POC_G3 => Diagenesis_Params.LigninDetr.Val,
                        AllVariables.POC_G2 => (1.0 - Diagenesis_Params.LigninDetr.Val),
                        _ => 1,   // POC_G1
                    }; 
                }
                // Case
                if ((Typ == T_SVType.StV))
                    Sed = Sed + ((P) as TSuspendedDetr).Sedimentation() * NFrac;
                //else
                    // ug/m3                      g/m3                      ug/kg                     kg/g
                    Sed = Sed + ((P) as TSuspendedDetr).Sedimentation() * NFrac * GetPPB(P.NState, Typ, P.Layer) * 1e-3;
            }
            // Detritus

        }

        // "s" in m/d
        // -----------------------------------------------------------------------------------------
        public double CalcDeposition(AllVariables NS, T_SVType Typ)
        {
            
            // Calc deposition input into diagenesis model
            // Calculate Deposition for each sed carbon & nutrient class in  (gC/m2 d) (gN/m2 d) (gP/m2 d) (gSi/m2 d
            double Def;
            double Sed;
            // - - - - - - - - - - - - - - - - - - - - - - - - - - -
            Def = 0;
            Sed = 0;
            foreach (TStateVariable TSV in SV)
            {
                CalcDeposition_SumSed(TSV, NS, Typ, ref Sed);
                CalcDeposition_SumDef(TSV, NS, Typ, ref Sed);  
            }
            MorphRecord MR = Location.Morph;
            return   (Sed + Def) * MR.SegVolum / DiagenesisVol(2) * Diagenesis_Params.H2.Val;
            //        (g / m2 d) (g / m3 w )        (m3 w)        (m3 sed)         (m sed)
            //        (ug / m2 d)(ug / m3 w)        (m3 w)        (m3 sed)         (m sed)  (toxicant deposition Consts)
        }



        // AQUATOX has been modified to include a representation of the sediment bed as
        // presented in Di Toro’s Sediment Flux Modeling (2001).
        public void CalculateSOD_SetL1State(AllVariables NS, double Val)
        {
            TStateVariable PSV;
            PSV = GetStatePointer(NS, T_SVType.StV, T_SVLayer.SedLayer1);
            PSV.State = Val;
        }

        public static void Linear_System(double a11, double a12, double a21, double a22, double b1, double b2, ref double x1, ref double x2)
        {
            x1 = (a22 * b1 - a12 * b2) / (a11 * a22 - a12 * a21);
            x2 = (a11 * b2 - a21 * b1) / (a11 * a22 - a12 * a21);
        }

        // -----------------------------------------------------------------------------------------
        public void CalculateSOD()
        {
            double SOD_test;
            double s;
            double Temp;
            double O2;
            double CSOD;
            double NSOD;
            // AllVariables IV;
            AllVariables ns;
            //          TAnimal TInv;
            double BenthicBiomass;
            bool ErrorOK;
            double Jc;
            double Jn;
            double Jp;
            double Jc_O2Equiv;
            // diagenesis tracking
            TPOC_Sediment ppc;
            TPON_Sediment ppn;
            TPOP_Sediment ppp;
            double JO2NO3, K2NH4, K2Denit_1, KDenit_2, NH3_0, NO3_0, NO3_1, NO3_2, NH4_1, NH4_2, PO4_0, PO4_1, PO4_2, COD_0;
            double HST1 = 0, HST2, a11, a12, b1, a21, a22, b2, Sech_Arg, CH4Sat, CSODmax;   // CH4toCO2, 
            double fda1, fpa1, fda2, fpa2;   // ammonia
            double fd1, fp1, fd2, fp2, F12, F21, k2Oxid;
            // double k1h1d, k1h1p,  k2h2d, k2h2p,  xk1, xk2;

            int IterCount;
            // CalculateSOD
            if (!Diagenesis_Included())
            {
                return; // No Diagenesis Model attached
            }
            
            O2 = GetState(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            Temp = GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            // determine diagenesis fluxes
            Jc = 0;
            Jn = 0;
            Jp = 0;
            for (ns = AllVariables.POC_G1; ns <= AllVariables.POC_G3; ns++)
            {
                ppc = (TPOC_Sediment)GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                Jc = Jc + ppc.Mineralization() * Diagenesis_Params.H2.Val * 32.0 / 12.0;
                // gO2/m2 d           // g C / m3 d                     // m    // g O2/ g C
                // CSOD
            }
            for (ns = AllVariables.PON_G1; ns <= AllVariables.PON_G3; ns++)
            {
                ppn = (TPON_Sediment)GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                Jn = Jn + ppn.Mineralization() * Diagenesis_Params.H2.Val;
                // NSOD in N Consts
            }
            // gN/ m2 d
            // gN / m3 d
            // m
            for (ns = AllVariables.POP_G1; ns <= AllVariables.POP_G3; ns++)
            {
                ppp = (TPOP_Sediment)GetStatePointer(ns, T_SVType.StV, T_SVLayer.SedLayer2);
                Jp = Jp + ppp.Mineralization() * Diagenesis_Params.H2.Val;
                // p mineralization
            }
            // gP/ m2 d
            // gP / m3 d
            // m
            if (SOD < 0)
            {
                // start with an initial esitmate of SOD
                SOD_test = Jc +   (Jn * 4.57);
         // (gO2/m2 d)=(gO2/d)+ (gN2/d)*(gO2/gN)

            }
            else
            {
                SOD_test = SOD;
            }
            // use SOD prev. time step
            // POCG1_2 := GetState(POC_G1,StV,SedLayer2) * 32.0 / 12.0 ;
            // {mg O2/L            // mg C /            // mg O2/ mg C

            BenthicBiomass = 0;
            if (BenthicBiomass_Link != null) BenthicBiomass = BenthicBiomass_Link.ReturnLoad(TPresent);  // JSON linkage

            //for (IV = Consts.FirstInvert; IV <= Consts.LastInvert; IV++)  
            //{
            //    TInv = GetStatePointer(IV, T_SVType.StV, T_SVLayer.WaterCol);
            //    if (TInv != null)
            //    {
            //        LL = Location.Locale;
            //        if (TInv.IsBenthos())
            //        {
            //            BenthicBiomass = BenthicBiomass + TInv.State * Volume_Last_Step / LL.SurfArea;
            //            // g/m2               g/m2             g/m3          m3                m2
            //        }
            //    }
            //}


            if (BenthicBiomass < 0.001)
            {
                BenthicBiomass = 0.001;
            }
            // corresponds to min particle mixing of 1.67E-06 cm/d

            Diagenesis_Rec DR = Diagenesis_Params;

            DR.W12 = Math.Pow(10.0, Math.Log10(BenthicBiomass) - 2.778151) * 0.0001 / DR.H2.Val;
            //  (m/d)    (cm2 / d)                 (g / m2)                    (m2/cm2)        (m)

            //w12 := (Dp.val * POWER(ThtaDp.val, (Temp - 20)) / (H2.Val * POC1R.Val + 1e-18)) * (POCG1_2(/ POC1R.Val))
            //(m / d)(m2 / d)(m)
            //                * (O2 / (Km_O2_Dp.val + O2 + 1e-18)) + (DpMin.Val)0.00006 / H2.Val ;
            //(min bioturb)  (m2 / d)(m) 

            DR.KL12 = DR.Dd.Val * Math.Pow(Convert.ToDouble(DR.ThtaDd.Val), (Temp - 20.0)) / (DR.H2.Val);
            // m/d      // m2/d                                                                 // m
            // ** NO Dissolved Phase Mixing Coefficient Due to Organism Activities  **

            IterCount = 0;
            do
            {
                NH3_0 = GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);                // dissolved ammonia water column
                NH4_1 = GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1);                // ammonia Layer 1
                NH4_2 = GetState(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer2);                // ammonia Layer 2
                NO3_0 = GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);                // nitrate water column
                NO3_1 = GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1);                // nitrate Layer 1
                NO3_2 = GetState(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2);
                PO4_0 = GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);                // phosphate water column
                PO4_1 = GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1);
                PO4_2 = GetState(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer2);
                // compute s
                if ((O2 < Consts.Tiny)) s = 1e6;
                else s = SOD_test / O2;
                // mass transfer coeff        // m/d  go2/m2 d  gO2/m3
                if (s < Consts.Tiny)
                {
                    s = Consts.Tiny;
                }
                // avoid crash solving linear eqns.
                // Solve for Ammonia
                fda1 = 1.0 / (1.0 + Diagenesis_Params.m1.Val * Diagenesis_Params.KdNH3.Val);
                fpa1 = 1.0 - fda1;
                fda2 = 1.0 / (1.0 + Diagenesis_Params.m2.Val * Diagenesis_Params.KdNH3.Val);
                fpa2 = 1.0 - fda2;
                if (NH4_1 < Consts.Tiny)
                {
                    K2NH4 = 0;
                }
                else
                {
                    K2NH4 = ((TNH4_Sediment)(GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1))).Nitr_Rate(NH4_1);
                }
                // Write linear system of equations around Ammonia
                a11 = -fda1 * Diagenesis_Params.KL12 - fpa1 * Diagenesis_Params.W12 - K2NH4 / s - fda1 * s - Diagenesis_Params.w2.Val;
                a12 = fda2 * Diagenesis_Params.KL12 + fpa2 * Diagenesis_Params.W12;
                b1 = -s * NH3_0;
                a21 = fda1 * Diagenesis_Params.KL12 + fpa1 * Diagenesis_Params.W12 + Diagenesis_Params.w2.Val;
                a22 = -fda2 * Diagenesis_Params.KL12 - fpa2 * Diagenesis_Params.W12 - Diagenesis_Params.w2.Val - Diagenesis_Params.H2.Val / (0.002);
                // If Layer2 Steady State then a22 := -fda2 * KL12 - fpa2 * w12 - w2.Val;
                b2 = -Jn - Diagenesis_Params.H2.Val / (0.002) * NH4_2;
                // If Layer2 Steady State then b2 := -Jn;
                Linear_System(a11, a12, a21, a22, b1, b2, ref NH4_1, ref NH4_2);
                NSOD = 4.57 * K2NH4 / s * NH4_1;
                // gO2/gN
                // Solve for Nitrate
                K2Denit_1 = ((TNO3_Sediment)(GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1))).Denit_Rate();
                // m2/d2
                KDenit_2 = ((TNO3_Sediment)(GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2))).Denit_Rate();
                // m/d
                a11 = -Diagenesis_Params.KL12 - K2Denit_1 / s - Diagenesis_Params.w2.Val - s;
                //                         (m/d)   (m2/d2)   (m/d)                  (m/d)

                a12 = Diagenesis_Params.KL12;
                b1 = -s * NO3_0 - K2NH4 / s * NH4_1;
                //  (m/d) (g/m3)   (  m/d  )   (g/m3)
                a21 = Diagenesis_Params.KL12 + Diagenesis_Params.w2.Val;
                a22 = -Diagenesis_Params.KL12 - KDenit_2 - Diagenesis_Params.w2.Val - Diagenesis_Params.H2.Val / (0.002);
                //                      (m/d)     (m/d)                     (m/d)                        (m/d)
                // If Layer2 Steady_State then a22 := -w12 - kDenit_2 - w2.Val;
                b2 = -Diagenesis_Params.H2.Val / (0.002) * NO3_2;
                // If Layer2 Steady State then b2 := 0;
                Linear_System(a11, a12, a21, a22, b1, b2, ref NO3_1, ref NO3_2);
                // Solve for Methane / Sulfide
                JO2NO3 = 2.86 * (K2Denit_1 / s * NO3_1 + KDenit_2 * NO3_2);
                //(g/m2d)          (m2/d2)   (m/d) (g/m3)   (m/d)     (g/m3)
                Jc_O2Equiv = Jc - JO2NO3;
                // ppt
                if (!Sulfide_System())
                {
                    // Solve for Methane
                    if (Jc_O2Equiv < 0)
                    {
                        Jc_O2Equiv = 0;
                    }
                    CH4Sat = 100 * (1.0 + DynamicZMean() / 10) * Math.Pow(1.024, (20 - Temp));
                    // m
                    CSODmax = Math.Min(Math.Sqrt(2 * Diagenesis_Params.KL12 * CH4Sat * Jc_O2Equiv), Jc_O2Equiv);
                    Sech_Arg = (Diagenesis_Params.KappaCH4.Val * Math.Pow(Diagenesis_Params.ThtaCH4.Val, (Temp - 20))) / s;
                    // CSOD Equation 10.35 from DiTorro
                    // The hyperbolic secant is defined as HSec(X) = 2.0 / (Exp(X) + Exp(-X))
                    if ((Sech_Arg < 400))
                    {
                        CSOD = CSODmax * (1.0 - (2.0 / (Math.Exp(Sech_Arg) + Math.Exp(-Sech_Arg))));
                    }
                    else
                    {
                        CSOD = CSODmax;
                    }
                    // HSec(SECH_ARG) < 3.8E-174 ~ 0
                }
                else
                {
                    // solve for sulfide
                    HST1 = GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer1);
                    HST2 = GetState(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer2);
                    COD_0 = GetState(AllVariables.COD, T_SVType.StV, T_SVLayer.WaterCol);
                    fd1 = 1.0 / (1.0 + Diagenesis_Params.m1.Val * Diagenesis_Params.KdH2S1.Val);
                    fp1 = 1.0 - fd1;
                    fd2 = 1.0 / (1.0 + Diagenesis_Params.m2.Val * Diagenesis_Params.KdH2S2.Val);
                    fp2 = 1.0 - fd2;
                    k2Oxid = ((TSulfide_Sediment)(GetStatePointer(AllVariables.Sulfide, T_SVType.StV, T_SVLayer.SedLayer1))).k2Oxid();
                    F12 = Diagenesis_Params.W12 * fp1 + Diagenesis_Params.KL12 * fd1;
                    F21 = Diagenesis_Params.W12 * fp2 + Diagenesis_Params.KL12 * fd2;
                    a11 = -F12 - k2Oxid / s - Diagenesis_Params.w2.Val - fd1 * s;
                    a12 = F21;
                    b1 = -s * COD_0;
                    a21 = F12 + Diagenesis_Params.w2.Val;
                    a22 = -F21 - Diagenesis_Params.w2.Val - Diagenesis_Params.H2.Val / 0.002;
                    b2 = -Jc_O2Equiv - Diagenesis_Params.H2.Val / 0.002 * HST2;
                    Linear_System(a11, a12, a21, a22, b1, b2, ref HST1, ref HST2);
                    // CSOD
                    CSOD = (k2Oxid / s) * HST1;
                }
                // Test Derived SOD_Test
                SOD = (SOD_test + CSOD + NSOD) / 2.0;
                // g O2/m2 d
                if (SOD == 0)
                {
                    ErrorOK = true;
                }
                else
                {
                    ErrorOK = Math.Abs((SOD - SOD_test) / SOD) <= PSetup.RelativeError;
                }
                IterCount++;
                if (IterCount > 5000)
                {
                    ErrorOK = true;
                }
                SOD_test = SOD;
            } while (!(ErrorOK));
            if (Diagenesis_Steady_State)
            {
                // STEADY STATE IN LAYER 1
                CalculateSOD_SetL1State(AllVariables.Ammonia, NH4_1);
                CalculateSOD_SetL1State(AllVariables.Nitrate, NO3_1);
                CalculateSOD_SetL1State(AllVariables.Sulfide, HST1);
                // Solve PO4
                fd1 = ((TPO4_Sediment)(GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1))).fdp1();
                fp1 = 1.0 - fd1;
                fd2 = (1.0 / (1.0 + Diagenesis_Params.m2.Val * Diagenesis_Params.KdPO42.Val));
                fp2 = 1.0 - fd2;
                a11 = -fd1 * Diagenesis_Params.KL12 - fp1 * Diagenesis_Params.W12 - fd1 * s - Diagenesis_Params.w2.Val;
                a12 = fd2 * Diagenesis_Params.KL12 + fp2 * Diagenesis_Params.W12;
                b1 = -s * PO4_0;
                a21 = fd1 * Diagenesis_Params.KL12 + fp1 * Diagenesis_Params.W12 + Diagenesis_Params.w2.Val;
                a22 = -fd2 * Diagenesis_Params.KL12 - fp2 * Diagenesis_Params.W12 - Diagenesis_Params.w2.Val - Diagenesis_Params.H2.Val / 0.002;
                b2 = -Jp - Diagenesis_Params.H2.Val / 0.002 * PO4_2;
                // If Layer2 steadystate Then
                // Begin
                // a22 := -fd2 * KL12 - fp2 * w12 - w2.val;
                // b2  := -Jp;
                // End;
                Linear_System(a11, a12, a21, a22, b1, b2, ref PO4_1, ref PO4_2);
                CalculateSOD_SetL1State(AllVariables.Phosphate, PO4_1);
                // SetL1State(Silica,Si_1);
                // steady state for silica not complete
            }
        }

        // Iterative Solution Scheme to calculate SOD before derivatives are solved
        // CalculateSOD
        // -----------------------------------------------------------------------------------------
        public bool Sulfide_System()
        {
            double Salt;
            TSalinity PSalt = (TSalinity) GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            if (PSalt== null) Salt = -1.0; 
                         else Salt = PSalt.State;
            return (Salt > Diagenesis_Params.SALTND.Val);
        }

    public double Diagenesis_Detr(AllVariables NS)
        {
            
            // quantity of sedimented detritus in diagenesis model in mg/L(wc)
            double OM;
            double SedN_OM;
            double SedP_OM;
            double SedC_OM;
            ReminRecord RR = Location.Remin;
            if (NS == AllVariables.SedmRefrDetr)
            {
                SedN_OM = GetState(AllVariables.PON_G2, T_SVType.StV, T_SVLayer.SedLayer2) / RR.N2Org_Refr;
                SedP_OM = GetState(AllVariables.POP_G2, T_SVType.StV, T_SVLayer.SedLayer2) / RR.P2Org_Refr;
                // g OM/m3 s               // g P or N / m3                                // g P or N / g OM
                SedC_OM = GetState(AllVariables.POC_G2, T_SVType.StV, T_SVLayer.SedLayer2) * Consts.Detr_OM_2_OC;
                // g OM/m3 s                 // g OC/m3                                   // g OM / g OC
            }
            else
            {
                // SedmLabDetr
                SedN_OM = GetState(AllVariables.PON_G1, T_SVType.StV, T_SVLayer.SedLayer2) / RR.N2OrgLab;
                SedP_OM = GetState(AllVariables.POP_G1, T_SVType.StV, T_SVLayer.SedLayer2) / RR.P2OrgLab;
                // g OM/m3                // g P or N / m3                          // g P or N / g OM
                SedC_OM = GetState(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.SedLayer2) * Consts.Detr_OM_2_OC;
                // g OM/m3                // g OC/m3                                // g OM / g OC
            }
            OM = Math.Min(SedN_OM, Math.Min(SedP_OM, SedC_OM));
            // g OM /m3
            MorphRecord MR = Location.Morph;
            return OM * DiagenesisVol(2) / MR.SegVolum;
        // g OM/m3 water  // g/m3   // m3  // m3 water
        }

        // ---------------------------------------------------------------

        // (************************************)
        // (* Straskraba, '76; Hutchinson, '57 *)
        // (* BUT simplify to linear relation  *)
        // (************************************)

        public double Extinct(bool Incl_Periphyton, bool Incl_BenthicMacro, bool Incl_FloatingMacro, bool IsSurfaceFloater, int OrgFlag)
        {
            // orgflag = 0 -- Normal execution,  orgflag = 1, organics only, orgflag = 2, inorganics only, orgflag = 3 phytoplankton only
            double PhytoExtinction;
            double TempExt = 0;
            TPlant Pphyto;
            AllVariables DetrLoop, Phyto;
            double DetrState;
            bool IncludePlant;
            //TStates OtherSeg;
            //double ThisSegThick;
            //double OtherSegThick;
  
            PhytoExtinction = 0.0;
            if ((OrgFlag == 0) || (OrgFlag == 3))
            {
                for (Phyto = Consts.FirstPlant; Phyto <= Consts.LastPlant; Phyto++)
                {
                    Pphyto = GetStatePointer(Phyto, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if (Pphyto != null)
                    {
                        PlantRecord PR = Pphyto.PAlgalRec;

                        IncludePlant = (PR.PlantType == "Phytoplankton");  // All Phytoplankton gets included
                        IncludePlant = IncludePlant || (((PR.PlantType == "Periphyton") || (PR.PlantType == "Bryophytes")) && Incl_Periphyton); // Periphyton & Bryophytes get included if requested
                        IncludePlant = IncludePlant || ((PR.PlantType == "Macrophytes") && (Pphyto.MacroType == TMacroType.Benthic) && Incl_BenthicMacro); // Benthic Macrophytes get included if requested
                        IncludePlant = IncludePlant || ((PR.PlantType == "Macrophytes") && (Pphyto.MacroType != TMacroType.Benthic) && Incl_FloatingMacro); // Floating Macrophytes get included if requested

                        if (IncludePlant)
                        {
                            // 3/9/2012 move from "blue-green" to surface floater designation
                            // 3-8-06 account for more intense self shading in upper layer of water column due to concentration of cyanobacteria there
                            if (IsSurfaceFloater && (Pphyto.PAlgalRec.SurfaceFloating))
                            {
                                // 1/m               // 1/m             // 1/(m g/m3)               // g/m3 volume                          // layer, m                                 // thick of algae, m
                                PhytoExtinction = PhytoExtinction + PR.ECoeffPhyto * GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol) * Location.MeanThick / Pphyto.DepthBottom();
                            }
                            else
                            {
                                PhytoExtinction = PhytoExtinction + PR.ECoeffPhyto * GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol);
                            }
                            // 1/m                 // 1/m              // 1/(m g/m3)                // g/m3                    }
                        }
                    }
                }

                TempExt = PhytoExtinction;
            }

            if ((OrgFlag == 0))
            {
                TempExt = PhytoExtinction + Location.Locale.ECoeffWater;
            }
            // ---------------------------------------------------------------------------
            // Organic Suspended Sediment Extinction
            if ((OrgFlag == 0) || (OrgFlag == 1))
            {
                for (DetrLoop = AllVariables.DissRefrDetr; DetrLoop <= Consts.LastDetr; DetrLoop++)
                {
                    DetrState = GetState(DetrLoop, T_SVType.StV, T_SVLayer.WaterCol);
                    if (DetrState > 0)
                    {
                        if ((DetrLoop == AllVariables.DissRefrDetr)||(DetrLoop == AllVariables.DissLabDetr))
                            TempExt = TempExt + Location.Locale.ECoeffDOM * DetrState;
                        else
                            TempExt = TempExt + Location.Locale.ECoeffPOM * DetrState;
                    }
                }
            }
            // ---------------------------------------------------------------------------
            // Inorganic Suspended Sediment EXTINCTION
            if ((OrgFlag == 0) || (OrgFlag == 2))
            {
                TempExt = TempExt + InorgSedConc() * Location.Locale.ECoeffSed;  
            }
            // ---------------------------------------------------------------------------
            if (TempExt < Consts.Tiny)  TempExt = Consts.Tiny;
            if (TempExt > 25.0)         TempExt = 25.0;

            return TempExt;
        }  // end Extinction


        public double Photoperiod_Radians(double X)
        {
            return  Math.PI * X / 180.0;
        }

        // (*****************************************)
        // (*     fraction of day with daylight     *)
        // (*  A = amplitude, from Groden, 1977     *)
        // (*  P formulated & coded by RAP, 6/26/98 *)
        // (*****************************************)
        public double Photoperiod()
        {
            TLight PL;
            double A, P, Sign, X;
            PL = (TLight) GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (PL.CalculatePhotoperiod)
            {
                if (Location.Locale.Latitude < 0.0) Sign = -1.0;
                                               else Sign = 1.0;
                X = TPresent.DayOfYear; 
                A = 0.1414 * Location.Locale.Latitude - Sign * 2.413;
                P = (12.0 + A * Math.Cos(380.0 * Photoperiod_Radians(X) / 365.0 + 248)) / 24.0;
                return P;
            }
            else return PL.UserPhotoPeriod / 24.0;
        }

        // --------------------------------
        // correction for non-optimal pH, used for microbial decomposition 
        // --------------------------------
        public double pHCorr(double pHMin, double pHMax)
        {
            const double KpH = 1.0;
            double ppH = GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            double result = 1.0;
            if (ppH <= pHMin) result = KpH * Math.Exp(ppH - pHMin);
            if (ppH > pHMax) result = KpH * Math.Exp(pHMax - ppH);
            return result;
        }   // phcorr

        // -------------------------------------------------------------------------------------------------------
        // (************************************)
        // (* Straskraba, '76; Hutchinson, '57 *)
        // (* BUT simplify to linear relation  *)
        // (************************************)

        public double ZEuphotic()
        {
            
            double RExt;
            double ZEup;
            RExt = Extinct(false, false, true, false, 0);
            if (RExt <= 0.0)
            {   // 4.605 is ln 1% of surface light
                // m                            // 1/m
                ZEup = 4.605 / Location.Locale.ECoeffWater;
            }
            else
            {
                ZEup = 4.605 / RExt;
            }
            if (Location.Locale.UseBathymetry)
            {
                if ((ZEup > Location.Locale.ZMax))
                {
                    ZEup = Location.Locale.ZMax;
                }
            }
            // else if (ZEup > DynamicZMean) then ZEup := DynamicZMean;
            return ZEup;
        }
        
        public double SalEffect(double Min, double Max, double Coeff1, double Coeff2)
        {
            double Salt;
            TStateVariable SSV;

            SSV = GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            if (SSV == null) return 1;

            Salt = SSV.State;
            if ((Salt >= Min) && (Salt <= Max)) return 1;
            else if (Salt < Min) return Coeff1 * Math.Exp(Salt - Min);
            else return Coeff2 * Math.Exp(Max - Salt);
        }

        public double CalcitePcpt()
        {
            const double OM_2_OC = 1.90;
            const double C2Calcite = 8.33;
            TPlant TP;
            AllVariables PVars;
            double Photo;
            Photo = 0;
            if (GetState(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol) < 7.5) return 0;   // JSC, From 8.25 to 7.5 on 7/2/2009

            for (PVars = Consts.FirstPlant; PVars <= Consts.LastPlant; PVars++)
            {
                TP = GetStatePointer(PVars, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                if (TP != null)
                {
                    if (TP.Is_Pcp_CaCO3())                     // subset of plants, all plants except Bryophytes and "Other Algae" compartments
                        Photo = Photo + TP.Photosynthesis();
                }
            }
            return C2Calcite * (Photo / OM_2_OC);
        // (mg calcite/L d)=(g Calcite / gC)*((mg OM/L d)/(g OM/ gOC))
        }

        public void ChangeData()     // Make sure all data is updated prior to study run
        {
            foreach (TStateVariable TSV in SV)
            {
                if (TSV.IsAnimal()) ((TSV) as TAnimal).ChangeData();
                if (TSV.IsPlant()) ((TSV) as TPlant).ChangeData();
            }

            Location.ChangeData(Location.Locale.ICZMean);

        }

        // ---------------------------------------------------------------
        public double CalculateTElapsed(int Tox)
        {
            // Calculates the number of elapsed days since first exposure to the given toxicant
            // the first exposure to the tox is assumed to be at the first toxicant presence within the simulation.
            if (FirstExposure[Tox] == DateTime.MinValue) // first exposure not yet initialized
            {
                FirstExposure[Tox] = TPresent;  // JSC 9-4-2001  Removed steady-state assumption
            }
            return (TPresent - FirstExposure[Tox]).TotalDays + 1;
        }
        // ---------------------------------------------------------------
        public void CalculateSumPrey()
        {
            AllVariables nsloop;
            TAnimal PA;
            // Calculates the total prey available to an animal in the given timestep.  This is used
            // for normalization of the ingestion function within TANIMAL.INGESTS
            for (nsloop = Consts.FirstAnimal; nsloop <= Consts.LastAnimal; nsloop++)
             {
               PA = GetStatePointer(nsloop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
               if (PA != null) PA.CalculateSumPrey();
             }
        }

        // ----------------------------------------
        // NORMDIFF
        // 
        // compute normalized rate differences
        // for uptake rates given conc. gradient
        // for Org Tox
        // 
        // also, normalize preference for the
        // amount of food available in a given
        // time step
        // ----------------------------------------
        public void NormDiff(double Step)
        {
            AllVariables ns;
            TAnimal CP;
            double PreyState;
            double SumPref;

            //double Egest;
            //double Pref;
            //T_SVType ToxLoop;
            //T_SVLayer LayLoop;
            //bool SedModelRunning;

            //if (Step > -1)
            //{
            //    // don't calculate Diff when exporting trophic interactions
            //    for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
            //    {
            //        // Begin
            //        Diff[ToxLoop] = 1;
            //    }
            //}


            // normalize preference for the amount of food avail. in a given time step
            for (ns = Consts.FirstAnimal; ns <= Consts.LastAnimal; ns++)
            {
                CP = ((GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol)) as TAnimal);
                if (CP != null)
                {
                    CP.ChangeData();
                    // reload original preferences from entry screen / JSON
                    // Sum up preference values for all food sources that are present above
                    // the minimum biomass level for feeding during a particular time step.

                    SumPref = 0;
                    foreach (TPreference PP in CP.MyPrey)
                    {
                        if ( !((PP.nState == AllVariables.SedmRefrDetr)||(PP.nState == AllVariables.SedmLabDetr))
                              && (GetStatePointer(PP.nState, T_SVType.StV, T_SVLayer.WaterCol) == null) )  
                            PP.Preference = 0;

                        if (!((PP.nState == AllVariables.SedmRefrDetr) || (PP.nState == AllVariables.SedmLabDetr)) // diagenesis model included
                              && (GetStatePointer(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.WaterCol) != null))
                        {
                            {  if (Step != -1)  PreyState = Diagenesis_Detr(PP.nState); else PreyState = 0; }
                        }
                        else PreyState = GetStateVal(PP.nState, T_SVType.StV, T_SVLayer.WaterCol);
                                       // mg/L wc


                        if ((PreyState <= CP.BMin_in_mg_L()) && (Step != -1))
                            PP.Preference = 0;

                        SumPref = SumPref + PP.Preference;
                    }

                    // normalize preferences
                    foreach (TPreference PP in CP.MyPrey)
                       if ((PP.Preference > 0) && (SumPref > 0))
                            PP.Preference = PP.Preference / SumPref;

                }  // if TAnimal not nil
            } // loop through animals

            //// **GULL MODEL** Normalize preferences for bird model
            //BirdPrey.FreeAll();
            //for (ns = AllVariables.Cohesives; ns <= Consts.LastBiota; ns++)
            //{
            //    // RELOAD ORIGINAL PREFERENCES
            //    if (GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) != null)
            //    {
            //        if (new ArrayList(new object[] { AllVariables.DissRefrDetr, AllVariables.DissLabDetr, AllVariables.BuriedRefrDetr }).Contains(ns))
            //        {
            //            Pref = 0;
            //        }
            //        else
            //        {
            //            Pref = GullPref[ns].Pref;
            //        }
            //        if ((Pref > 0))
            //        {
            //            Egest = 0;
            //            PP = new TPreference(Pref, Egest, ns);
            //            BirdPrey.Insert(PP);
            //        }
            //    }
            //}
            //// **GULL MODEL** Sum up preference values for all food sources that are present above
            //// the minimum biomass level for feeding during a particular time step.
            //SumPref = 0;
            //for (i = 0; i < BirdPrey.Count; i++)
            //{
            //    PP = ((BirdPrey.At(i)) as TPreference);
            //    SumPref = SumPref + PP.Preference;
            //}
            //// **GULL MODEL** normalize preferences
            //for (i = 0; i < BirdPrey.Count; i++)
            //{
            //    PP = ((BirdPrey.At(i)) as TPreference);
            //    if ((PP.Preference > 0) && (SumPref > 0))
            //    {
            //        PP.Preference = PP.Preference / SumPref;
            //    }
            //}

        } // end NORMDIFF

        public virtual double GetPPB(AllVariables S, T_SVType T, T_SVLayer L)
        {
            // calculation of toxicant ppb levels during kinetic derivatives
            // Returns dry weight ppb
            TToxics PT;
            double CarrierState;
            double ToxState;
            double PPBResult;

            // ns must be a toxicant state variable or the program will halt
            if (!(T >= Consts.FirstOrgTxTyp && T <= Consts.LastOrgTxTyp)) throw new Exception("Programming Error, GetPPB has been passed a non-toxicant.");

            if ( Diagenesis_Included() && ((S== AllVariables.SedmRefrDetr)||(S==AllVariables.SedmLabDetr)) )
            {
                if ((S == AllVariables.SedmRefrDetr))
                    return GetPPB(AllVariables.POC_G2, T, T_SVLayer.SedLayer2) / Consts.Detr_OM_2_OC;
                else
                    return GetPPB(AllVariables.POC_G1, T, T_SVLayer.SedLayer2) / Consts.Detr_OM_2_OC;
                // ug/kg OM                // ug /kg OC                                   // OM/OC
            }

            PT = GetStatePointer(S, T, L) as TToxics;
            if (PT == null) return 0;
            if (PT.IsAGGR)  return PT.ppb;

            ToxState = PT.State;
            if (S == AllVariables.H2OTox) return ToxState;      // Toxicant Dissolved in Water already in ug/L

            CarrierState = GetState(S, T_SVType.StV, L);
            if ((CarrierState < Consts.Tiny) || (ToxState < Consts.Tiny)) return 0;

            PPBResult = ToxState / CarrierState * 1e6;             // Toxicant in Carrier in water
            // ug/kg       ug/L         mg/L        mg/kg

            if (S >= AllVariables.POC_G1 && S <= AllVariables.POC_G3)
            {
                if ((CarrierState < Consts.Tiny))  PPBResult = 0;
                else
                {
                    PPBResult = (ToxState / (CarrierState * Diagenesis_Params.H2.Val)) * 1e3;
                    // ug/kg    // ug/m2      // g/m3                         // m     // g/kg
                }
            }
                        
            return PPBResult;
        }

        // ------------------------------------------------------------------------
        public void CalcPPB()
        {

            foreach (TToxics P in SV.OfType<TToxics>())
            {
                if (P.IsAGGR) return;
                if ((P.NState >= Consts.FirstDetr) && (P.NState <= Consts.LastDetr))
                    ((P) as TToxics).ppb = GetPPB(P.NState, P.SVType, P.Layer);   // dry-weight concentration for detritus
                else ((P) as TToxics).ppb = GetPPB(P.NState, P.SVType, P.Layer) / ((P) as TToxics).WetToDry();    // wet-weight concentration
            }
        }
        // ------------------------------------------------------------------------


        }  // end TAQUATOXSegment


        public class TSalinity : TRemineralize  //Salinity is a DRIVING Variable Only
    {
        //public double SalinityUpper = 0;
        //public double SalinityLower = 0;
        public TSalinity(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

        public override void Derivative(ref double DB)
        {
            base.Derivative(ref DB);
            DB = 0;
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
            State = LoadsRec.ReturnLoad(TimeIndex);
            //SalinityUpper = State;
            //SalinityLower = LoadsRec.ReturnAltLoad(TimeIndex, 0);

            //if (AQTSeg != null)
            //{
            //    if (AQTSeg.VSeg == VerticalSegments.Hypolimnion)
            //    {
            //        State = SalinityLower;
            //    }
            //}
        }

    } // end TSalinity


    public class TpHObj : TStateVariable
    {
        public double Alkalinity = 1000;
        // -------------------------------------------------------------------------------
        //Constructor  Init( Ns,  SVT,  Lyr,  aName,  P,  IC,  IsTempl)
        public TpHObj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            Alkalinity = 1000;   // ( default ueq CaCO3/L)

        }
        public double CalculateLoad_asinh(double x)
        {
            return Math.Log(Math.Sqrt(Math.Pow(x, 2) + 1) + x);
        }

        // pH calculation based on Marmorek et al., 1996 (modified Small and Sutton, 1986)
        public double CalculateLoad_pHCalc()
        {
            
            const double pkw = 1E-14;
            // ionization constant water
            double T, CCO2, DOM, pH2CO3, Alpha, A, B, C;

            T = AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);          // deg C
            CCO2 = AQTSeg.GetState(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol) / 44.0 * 1000.0;   // ueq/mg  
            TDissRefrDetr PDOM = (TDissRefrDetr)AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            if (PDOM == null) DOM = 0;
            else DOM = PDOM.State;   // mg/L

            pH2CO3 = Math.Pow(10.0, -(6.57 - 0.0118 * T + 0.00012 * (Math.Pow(T, 2))) * 0.92);
            Alpha = pH2CO3 * CCO2 + pkw;
            A = -Math.Log10(Math.Pow(Alpha, 0.5));
            B = 1.0 / Math.Log(10.0);
            C = 2 * (Math.Pow(Alpha, 0.5));
            try
            {
                return A + B * CalculateLoad_asinh((Alkalinity - 5.1 * DOM * 0.5) / C);
            }
            catch
            {
                return 7; // default if ASINH function crashes
            }
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            if (LoadsRec.Loadings.NoUserLoad)
            {
                State = CalculateLoad_pHCalc();              // pHCalc
                if ((State > 8.25)) { State = 8.25; }
                if ((State < 3.75)) { State = 3.75; }
            }
            else State = LoadsRec.ReturnLoad(TimeIndex);            // allows for user input of load
        }

    } // end TpHObj


    public class TTemperature : TStateVariable
    {
        public TTemperature(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {

        }

        // (***********************************)
        // (* water temperature of segment    *)
        // (* Ward 1963, ASCE 1989, 6:1-16    *)
        // (***********************************)
        public override void CalculateLoad(DateTime TimeIndex)
        {
            // Changes State, not Loadings... Rewritten JSC, 22-May-95
            const int PhaseShift = 90;
            double Temperature;
            double MeanTemp;
            double TempRange;
            double AdjustedJulian;
            // Julian date adjusted for hemisphere
            if (!LoadsRec.Loadings.NoUserLoad)
            {

                //if ((AQTSeg.VSeg == VerticalSegments.Epilimnion) || (AQTSeg.HypoTempLoads.NoUserLoad))
                //{
                Loading = LoadsRec.ReturnLoad(TimeIndex);
                //}
                //else
                //{
                //    // hypolimnion: User entered data
                //    LoadingsRecord _wvar2 = AQTSeg.HypoTempLoads;
                //    if (_wvar2.UseConstant)   {  Loading = _wvar2.ConstLoad; }
                //    else
                //    {
                //        Loading = 0;
                //        if (_wvar2.Loadings != null)    { Loading = _wvar2.Loadings.GetLoad(TimeIndex, true);  }
                //    }
                //    // else
                //    Loading = Loading * _wvar2.MultLdg;
                //}
                // With HypoTempLoads

                State = Loading;
            }  // if userload
            else
            {
                // NoUserLoad for both Epi and Hypo Temp Loadings
                AdjustedJulian = TimeIndex.DayOfYear;
                if (Location.Locale.Latitude < 0.0) AdjustedJulian = AdjustedJulian + 182.0;
                MeanTemp = Location.Locale.TempMean;  // AQTSeg.VSeg
                TempRange = Location.Locale.TempRange; // AQTSeg.VSeg
                                                       //if (AQTSeg.LinkedMode)
                                                       //{
                                                       //    // MeanTemp and Range are stored in "Epilimnion" for each linked segment regardless of stratification
                                                       //    MeanTemp = Location.Locale.TempMean[VerticalSegments.Epilimnion];
                                                       //    TempRange = Location.Locale.TempRange[VerticalSegments.Epilimnion];
                                                       //}

                Temperature = MeanTemp + (-1.0 * TempRange / 2.0 * (Math.Sin(0.0174533 * (0.987 * (AdjustedJulian + PhaseShift) - 30.0))));
                if (Temperature < 0.0) { Temperature = 0.0; }

                // Temperature = Temperature * MultLdg;                // allow perturbation JSC 1-23-03

                State = Temperature;
            }
            Loading = 0;
        }

    } // end TTemperature

    

    public class TLight : TStateVariable
    {
        public bool CalculatePhotoperiod = true;
        public double UserPhotoPeriod = 12;
        public double DailyLight = 0;
        public double HourlyLight = 0;

        //Constructor  Init( Ns,  SVT,  Lyr,  aName,  P,  IC,  IsTempl)
        public TLight(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            // TStateVariable.Init(Ns,SVT,Lyr,aName,P,IC,IsTempl);
            CalculatePhotoperiod = true;
            UserPhotoPeriod = 12;
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            double adjustedjulian, lighttime, light, solar, ShadeVal, pp, fracdaypassed;
            // Calculates light load at the top layer of the water system, modified
            // later with LtTop and LtDepth which use DepthTop and DepthBottom

            // allows for user input of light
            if (!LoadsRec.Loadings.NoUserLoad)
            {
                base.CalculateLoad(TimeIndex); // TStateVariable
                light = Loading;
            }
            else
            {   // NoUserLoad, calculate based on date
                adjustedjulian = TimeIndex.DayOfYear;
                if (Location.Locale.Latitude < 0.0) adjustedjulian = adjustedjulian + 182;

                solar = Location.Locale.LightMean + Location.Locale.LightRange / 2.0 * Math.Sin(0.0174533 * adjustedjulian - 1.76);

                light = solar;
                if (light < 0.0)  light = 0.0;

                light = light * LoadsRec.Loadings.MultLdg;  // allow perturbation JSC 1-23-03
            }

            // ACCOUNT FOR ICE COVER
            {   if (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) < AQTSeg.Ice_Cover_Temp())
                {   // Aug 2007, changed from 33% to 15%
                    light = light * 0.15;
                }
            }

            // Wetzel (2001).
            // Light:=Light*0.33;   {ave. of values, Wetzel '75, p. 61, Used in Rel2.2 and before

            ShadeVal = 1.0 - (0.98 * AQTSeg.Shade.ReturnLoad(TimeIndex));   // 11/18/2009  2% of incident radiation is transmitted through canopy
            light = light * ShadeVal;
            State = light;
            DailyLight = light;

            HourlyLight = 0;
            if ((!LoadsRec.Loadings.NoUserLoad) && (!AQTSeg.PSetup.ModelTSDays) && (LoadsRec.Loadings.Hourly))
                { HourlyLight = light; }

            // 12-5-2016 correction to properly model hourly light time-series inputs
            if ((!AQTSeg.PSetup.ModelTSDays) && (LoadsRec.Loadings.NoUserLoad || (LoadsRec.Loadings.UseConstant) || (!LoadsRec.Loadings.Hourly)))
            {
                // distribute daily loading over daylight hours
                pp = AQTSeg.Photoperiod();
                fracdaypassed = TimeIndex.TimeOfDay.TotalDays;
                lighttime = fracdaypassed - ((1.0 - pp) / 2.0);
                if ((fracdaypassed < (1.0 - pp) / 2.0) || (fracdaypassed > 1.0 - ((1.0 - pp) / 2.0)))  State = 0;
                else State = (Math.PI / 2.0) * (light / pp) * Math.Sin(Math.PI * lighttime / pp) * LoadsRec.Loadings.MultLdg * ShadeVal;
                HourlyLight = State;
            }

            Loading = 0;     // this procedure sets "state" not "Loading" which is irrelevant for light
        }

    } // end TLight


    public class TWindLoading : TStateVariable
    {
        public double MeanValue = 0;
        // ----------------------------------------
        // wind, based on 140-day Missouri record
        // computed using first 10 harmonics
        // therefore, will have a 140-day repeat
        // ----------------------------------------

        public TWindLoading(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            MeanValue = 0;
        }

        public static double CalculateWindLoading(DateTime TimeIndex, double MeanVal)
        {             // JSC Modified 4/29/2008 to incorporate 365 day series.
            double AddVar, Wind, DateVal;
            int N;
            //            const double Pi = 3.141592654;
            //            const double Intercept = 5.043;

            double[] Coeffs = { 0.83408, 0.87256, 0.4245, -0.2871, -0.2158, -0.6634, -0.0264, -0.2766, 0.0236, -0.3492, -0.442, 0.89, -1.4385, 0.634, 0.0935, -1.06, -0.564, -0.291, -0.6484, 0.6162, 0.1083, 0.4047, 0.0268, -0.1209 };
            double[] Freq = { 1, 1, 2, 2, 4, 4, 8, 8, 16, 16, 32, 32, 64, 64, 128, 128, 200, 200, 300, 300, 6, 6, 3, 3 };
            if (MeanVal <= 0)
            {   // default
                MeanVal = 3.0;
            }
            Wind = MeanVal;

            DateVal = 2 * Math.PI * TimeIndex.DayOfYear / 365.0;
            for (N = 1; N <= 24; N++)
            {
                if (N % 2 == 1)
                     AddVar = Coeffs[N-1] * Math.Cos(Freq[N-1] * DateVal); // COS Coefficients in odd array registers
                else AddVar = Coeffs[N-1] * Math.Sin(Freq[N-1] * DateVal); // SIN Coefficients in even array registers

                Wind = Wind + AddVar;
            }
            if (Wind < 0.0) Wind = 0.0;
            
            return Wind; 
        }

        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;
            if (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) < AQTSeg.Ice_Cover_Temp())
            {
                State = 0;  // no wind loading as iced over
                return;
            }

            //if (AllStates.VSeg == VerticalSegments.Hypolimnion)
            //{
            //    State = 0;
            //    return;
            //}

            // allows for user input of load, jsc
            if (!LoadsRec.Loadings.NoUserLoad)
            {
                base.CalculateLoad(TimeIndex);  // TStateVariable
                State = Loading;
                return;
            }

            State = CalculateWindLoading(TimeIndex, MeanValue) * LoadsRec.Loadings.MultLdg;
        }


    } // end TWindLoading

    public class TSandSiltClay : TStateVariable   // TSS
    {
        public bool TSS_Solids = true;

        // ----------------------------------------------------------------------
        public override void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0.0;
            base.CalculateLoad(TimeIndex);
            if (NState == AllVariables.TSS)
            {
                State = this.Loading;  // valuation not loading, no need to adjust for flow and volume
            }
            else
            {
                throw new ArgumentException("TSandSiltClay Implemented for TSS only.  Sand, Silt, Clay model not implemented in AQUATOX HMS");
            }

        }

        public override void Derivative(ref double DB)
        {
            DB = 0.0;  //TSS is a driving variable only
        }
    }



        public class AQTKnownTypesBinder : Newtonsoft.Json.Serialization.ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public Type BindToType(string assemblyName, string typeName)
    {
        return KnownTypes.SingleOrDefault(t => t.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }

    public AQTKnownTypesBinder()
    {
            KnownTypes = new List<Type> { typeof(TStateVariable), typeof(AQUATOXSegment), typeof(TAQTSite), 
                                          typeof(SiteRecord), typeof(ReminRecord), typeof(Setup_Record), typeof(AQUATOX.Volume.TVolume), typeof(LoadingsRecord), typeof(TLoadings),
                                          typeof(SortedList<DateTime, double>), typeof(AQUATOXTSOutput), typeof(TRemineralize), typeof(TNH4Obj), typeof(TNO3Obj), typeof(TPO4Obj),
                                          typeof(TSalinity), typeof(TpHObj), typeof(TTemperature), typeof(TCO2Obj), typeof(TO2Obj), typeof(DetritalInputRecordType),
                                          typeof(TDissRefrDetr), typeof(TDissLabDetr), typeof(TSuspRefrDetr), typeof(TSuspLabDetr), typeof(TSedRefrDetr), typeof(TSedLabileDetr),
                                          typeof(TimeSeriesInput), typeof(TimeSeriesOutput), typeof(TNH4_Sediment), typeof(TNO3_Sediment), typeof(TPO4_Sediment),
                                          typeof(TPOC_Sediment), typeof(TPON_Sediment), typeof(TPOP_Sediment), typeof(TMethane), typeof(TSulfide_Sediment),
                                          typeof(TSilica_Sediment), typeof(TCOD), typeof(TParameter), typeof(Diagenesis_Rec), typeof(TToxics), typeof(TLight),
                                          typeof(ChemicalRecord), typeof(TWindLoading), typeof(TPlant), typeof(PlantRecord), typeof(TMacrophyte), typeof(TAnimal),typeof(AnimalRecord),
                                          typeof(TSandSiltClay), typeof(InteractionFields), typeof(TAnimalTox), typeof(TParticleTox), typeof(TBioTransObject),
                                          typeof(TAlgaeTox), typeof(TPlantToxRecord), typeof(TAnimalToxRecord), typeof(T_N_Internal_Plant)}; 
    }
}

}

