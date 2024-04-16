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
using System.Data;

namespace AQUATOX.AQTSegment


{
    public class AQTSim
    {
        public AQUATOXSegment AQTSeg = null;
        public Dictionary<string, AQUATOXSegment> SavedRuns = null;

        public bool HasResults()
        {
            if (SavedRuns == null) return false;
            else if (SavedRuns.Count == 0) return false;

            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, AQUATOXSegment> entry in SavedRuns)
            {
                AQUATOXSegment segment = entry.Value;
                if (segment.SV[0].SVoutput == null) keysToRemove.Add(entry.Key);  //invalid segment has no output
            }
            foreach (string key in keysToRemove) SavedRuns.Remove(key);  //remove invalid segment in archived results

            return (SavedRuns.Count > 0);
        }

        public string SaveJSON(ref string json)
        {
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(this, AQTJSONSettings());
                // json = json + Newtonsoft.Json.JsonConvert.SerializeObject(SavedRuns, AQTJsonSerializerSettings);
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

        static public JsonSerializerSettings AQTJSONSettings()
        {
            AQTKnownTypesBinder AQTBinder = new AQTKnownTypesBinder();
            return new JsonSerializerSettings()
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = AQTBinder
            };
        }


        public string Instantiate(string json)
        {

            try
            {
                AQTSim tempsim = Newtonsoft.Json.JsonConvert.DeserializeObject<AQTSim>(json, AQTJSONSettings());
                AQTSeg = tempsim.AQTSeg; SavedRuns = tempsim.SavedRuns;
                AQTSeg.SetupLinks();
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


        public string SaveSegJSON(ref string json)
        {
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(AQTSeg, AQTJSONSettings());
                // json = json + Newtonsoft.Json.JsonConvert.SerializeObject(SavedRuns, AQTJsonSerializerSettings);
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

        public string InstantiateSeg(string json)
        {

            try
            {
                AQTSeg = Newtonsoft.Json.JsonConvert.DeserializeObject<AQUATOXSegment>(json, AQTJSONSettings());
                AQTSeg.SetupLinks();
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

        public bool ArchiveSimulation()
        {
            if (SavedRuns == null) SavedRuns = new Dictionary<string, AQUATOXSegment>();
            if (AQTSeg.RunID == "") return false;
            if (SavedRuns.TryGetValue(AQTSeg.RunID, out AQUATOXSegment savedRun)) return false;

            string JSONToArchive = "";
            SaveSegJSON(ref JSONToArchive);
            AQTSim SimToArchive = new AQTSim();
            SimToArchive.InstantiateSeg(JSONToArchive);

            if (SavedRuns.Count > 0)
            {
                string GraphJSON = JsonConvert.SerializeObject(SavedRuns[SavedRuns.Keys.Last()].Graphs, AQTJSONSettings());  
                TGraphs LatestGraphs = JsonConvert.DeserializeObject<TGraphs>(GraphJSON);
                SimToArchive.AQTSeg.Graphs = LatestGraphs;
            }

            SavedRuns.Add(AQTSeg.RunID, SimToArchive.AQTSeg);

            return true;
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
                return AQTSeg.Integrate(PS.FirstDay.Val, PS.LastDay.Val, PS.RelativeError.Val, PS.MinStepSize.Val, PS.StoreStepSize.Val);
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

            AQTSeg.PSetup.FirstDay.Val = StartDate;
            AQTSeg.PSetup.LastDay.Val = EndDate;
            return Integrate();
        }

        private TLoadings TLoadingsFromString(string SVType, int LoadingType)
        {
            bool isdetritus = false;
            TStateVariable TSV = null;
            switch (SVType)
            {
                case "TNH4Obj": TSV = AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol); break;
                case "TN":
                case "TNO3Obj": TSV = AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol); break;
                case "TP":
                case "TPO4Obj": TSV = AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol); break;
                case "TDissRefrDetr": TSV = AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol); isdetritus = true; break;
            }

            if (TSV == null) return null;

            LoadingsRecord LR = TSV.LoadsRec;
            if (isdetritus) { LR = ((TDissRefrDetr)TSV).InputRecord.Load;}

            bool isTN = (SVType == "TN");      //set TN flags based on user input
            if (isTN || (SVType == "TNO3Obj"))
            {
                ((TNO3Obj)TSV).TN_IC = isTN;
                ((TNO3Obj)TSV).TN_Inflow = isTN;
                ((TNO3Obj)TSV).TN_PS = isTN;
                ((TNO3Obj)TSV).TN_NPS = isTN;
                //if (LoadingType < 0) ((TNO3Obj)TSV).TN_Inflow = isTN;
                //else if (LoadingType == 0) ((TNO3Obj)TSV).TN_PS = isTN;
                //else ((TNO3Obj)TSV).TN_NPS = isTN;
            }

            bool isTP = (SVType == "TP");  //set TP flags based on user input
            if (isTP || (SVType == "TPO4Obj"))
            {
                ((TPO4Obj)TSV).TP_IC = isTP;
                ((TPO4Obj)TSV).TP_Inflow = isTP;
                ((TPO4Obj)TSV).TP_PS = isTP;
                ((TPO4Obj)TSV).TP_NPS = isTP;
                //if (LoadingType < 0) ((TPO4Obj)TSV).TP_Inflow = isTP;
                //else if (LoadingType == 0) ((TPO4Obj)TSV).TP_PS = isTP;
                //else ((TPO4Obj)TSV).TP_NPS = isTP;
            }

            if (LoadingType < 0) return LR.Loadings;
            else return LR.Alt_Loadings[LoadingType];
        }

        
        /// <summary>
        /// Accepts an input JSON sting and information about a constant point-source, nonpoint-source, or inflow loading and inserts that into a return JSON string
        /// </summary>
        /// <param name="inJSON"></param>
        /// <param name="SVType">string with the relevant $Type string e.g. "TNH4Obj"</param>
        /// <param name="Loadingtype">int, -1 = inflow load,  0=point-source, 1=direct precipitation (maybe irrelevant), 2=nonpoint-source </param>
        /// <param name="constload">constant daily loading usually in g/d</param>
        /// <param name="multldg">multiply loading by constant, default to 1.0</param>
        /// <returns>
        /// JSON String with loadings inserted, or string starting with "ERROR:" if error encountered
        /// </returns>
        public string InsertLoadings(string inJSON, string SVType, int Loadingtype, double constload, double multldg)
        {
            string instResult = Instantiate(inJSON);
            if (instResult != "") return "ERROR: " + instResult;

            TLoadings TL = TLoadingsFromString(SVType, Loadingtype);
            if (TL == null) return "State Variable Type not Found";

            TL.UseConstant = true;
            TL.MultLdg = multldg;
            TL.ConstLoad = constload;

            string outjson = "";
            string saveErr = SaveJSON(ref outjson);
            if (saveErr != "") return "ERROR: " + saveErr;
            return outjson;
        }

        /// <summary>
        /// Accepts an input JSON sting and information about a constant point-source, nonpoint-source, or inflow loading and inserts that into a return JSON string
        /// </summary>
        /// <param name="inJSON"></param>
        /// <param name="SVType">string with the relevant $Type string e.g. "TNH4Obj"</param>
        /// <param name="Loadingtype">int, -1 = inflow load,  0=point-source, 1=direct precipitation (maybe irrelevant), 2=nonpoint-source </param>
        /// <param name="inlist">SortedList to be inserted with datetime field and double type, usually in g/d</param>
        /// <param name="multldg">multiply loading by constant, default to 1.0</param>
        /// 
        /// <returns>
        /// JSON String with loadings inserted, or string starting with "ERROR:" if error encountered
        /// </returns>
        public string InsertLoadings(string inJSON, string SVType, int Loadingtype, SortedList<DateTime, double> inlist, double multldg)
        {
            string instResult = Instantiate(inJSON);
            if (instResult != "") return "ERROR: " + instResult;

            TLoadings TL = TLoadingsFromString(SVType, Loadingtype);
            if (TL == null) return "State Variable Type not Found";

            TL.UseConstant = false;
            TL.MultLdg = multldg;
            //TL.Hourly = true;  12/14/22
            TL.list = inlist;

            string outjson = "";
            string saveErr = SaveJSON(ref outjson);
            if (saveErr != "") return "ERROR: " + saveErr;
            return outjson;
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

    public class TRate
    {
        public string Name;
        public double[] Rate = new double[7];

        public TRate(string Nm)
        {
            int Stp;
            Name = Nm;
            for (Stp = 0; Stp <= 6; Stp++)
            {
                Rate[Stp] = 0;
            }
        }

        public double GetRate()
        {
            return (Globals.Consts.C1 * Rate[1] + Globals.Consts.C3 * Rate[3] + Globals.Consts.C4 * Rate[4] + Globals.Consts.C6 * Rate[6]);  // RK Solution 

            //try result = (Globals.Consts.C1 * Rate[1] + Globals.Consts.C3 * Rate[3] + Globals.Consts.C4 * Rate[4] + Globals.Consts.C6 * Rate[6]);  // RK Solution 
            //catch result = 0;
            //return result;
        }

    } // end TRate 

    public class SavedResults
    {
        public List<List<double>> Results = new List<List<double>>();   // holds numerical results, internal, not evenly spaced if variable stepsize
        public List<string> Names = new List<string>();
        public List<string> Units = new List<string>();

        public void ClearResults()
        {
            foreach (List<double> res in Results)
                res.Clear();
            Results.Clear();
            Names.Clear();
            Units.Clear();
        }

        public void AddColumn(string nm, string unit)
        {
            Names.Add(nm);
            Units.Add(unit);
            Results.Add(new List<double>());
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
        [JsonIgnore] public SavedResults SVResults = null;   // holds numerical results, internal, not evenly spaced if variable stepsize

        public AQUATOXTSOutput SVoutput = null;  // public and evenly-spaced results following integration / interpolation

        [JsonIgnore] public AQUATOXSegment AQTSeg = null;   // Pointer to Collection of State Variables of which I am a member
        public LoadingsRecord LoadsRec = null;   // Holds all of the Loadings Information for this State Variable  
        public bool UseLoadsRecAsDriver = false; // If user sets this to true, no integration is used but time series driving data from "loadsrec"
        [JsonIgnore] public double Loading = 0;         // Loading of State Variable This time step

        public string StateUnit;
        public string LoadingUnit;       // Consts 

        [JsonIgnore] public TAQTSite Location = null;    // Pointer to Site in which I'm located

        public bool SaveRates = true;      // Does the user want rates written for this SV?
        [JsonIgnore] public List<TRate> RateColl = null; // Collection of saved rates for current timestep
        [JsonIgnore] public int RateIndex = 0;
        [JsonIgnore] public int nontoxloadings;

        public string LoadNotes1;
        public string LoadNotes2;            // Notes associated with loadings
        public bool TrackResults = true;     // Does the user want to save results for this variable?
                                             // public bool IsTemplate = false;       // Is this a member of the template study in a linked system?  True if single study run.
                                             // public double[] WashoutStep = new double[7];     // Saved Washout Variables for use in outputting Cascade Outflow, nosave
                                             //        double WashoutAgg = 0;
                                             //        double LastTimeWrit = 0;


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
            if (P != null) Location = P.Location;

            LoadNotes1 = "";
            LoadNotes2 = "";

            yhold = 0;
            for (j = 1; j <= 6; j++) StepRes[j] = 0;

            LoadsRec = new LoadingsRecord();
        }

        public virtual void UpdateName()
        {
            PName = AQTSeg.StateText(NState);
        }

        public virtual void Derivative(ref double DB)
        {
            DB = 0;
        }

        public virtual string IgnoreLabel()
        {
            return "Ignore All Loadings";  // default label meaning
        }

        public int ToxInt(T_SVType typ)
        {
            return (int)typ - 2;
        }

        public virtual List<string> GUIRadioButtons()
        {
            return null;  //return a list of radio buttons for the interface that is specific to this state variable type.
                          //Null for base TStateVariable or any state variable that does not need radio buttons..
        }

        public virtual int RadioButtonState()
        {
            return -1;  //return the state of the radio button based on current state variable parameters
        }

        public virtual void SetVarFromRadioButton(int iButton)
        {
            //set the current state variable parameters based on which button is true (iButton parameter)  // no action required for base object.
        }

        public virtual void SetToInitCond()
        {
            int j;
            State = InitialCond;

            // Fish, Sedimented Detritus, Periphyton, Macrophytes, Zoobenthos must undergo unit conversion
            if (AQTSeg.Convert_g_m2_to_mg_L(NState, SVType, Layer))
                State = InitialCond * Location.Locale.SurfArea.Val / AQTSeg.Volume_Last_Step;
            // g/m3      // g/m2                     // m2                   // m3

            yhold = 0;

            for (j = 1; j <= 6; j++) StepRes[j] = 0;

            // Initialize Toxics  
            if ((SVType >= Consts.FirstOrgTxTyp) && (SVType <= Consts.LastOrgTxTyp))
            {
                ((this) as TToxics).ppb = 0;

                ((this) as TToxics).IsAGGR = false;
                if ((SVType == Consts.FirstOrgTxTyp) && (AQTSeg.PSetup.T1IsAggregate.Val))
                    ((this) as TToxics).IsAGGR = true;
            }
        }

        public void TakeDerivative(int Step)
        {
            if ((UseLoadsRecAsDriver) || (Location.SiteType == SiteTypes.TribInput))    // If this is true, no integration is used; the variable is driven by time series data from "loadsrec"
            {
                State = LoadsRec.ReturnLoad(AQTSeg.TPresent);
                StepRes[Step] = 0;
            }
            else if ((AQTSeg.WaterVolZero) && (NState != AllVariables.Volume))  // if "water vol zero" integrate water volume variable only, keep this SV constant
            {
                StepRes[Step] = 0;
                return;
            }
            else Derivative(ref StepRes[Step]);
        }

        public TStateVariable()
        { }

        // -------------------------------------------------------------------------------
        public bool Has_Alt_Loadings()
        {
            if (Layer != T_SVLayer.WaterCol) return false;

            bool result = ((NState == AllVariables.Volume) || (NState == AllVariables.H2OTox) || (NState == AllVariables.Phosphate)
            || (NState == AllVariables.Oxygen) || (NState == AllVariables.Ammonia) || (NState == AllVariables.Nitrate)
            || (NState == AllVariables.Salinity) || ((NState >= AllVariables.DissRefrDetr) && (NState <= Consts.LastDetr)));

            if (!result) result = IsFish();
            return result;
        }

        public string LoadUnit(int index)
        {
            if (index < 1) return LoadingUnit;

            int toploadindex = 0;
            if (Has_Alt_Loadings()) toploadindex = 3;
            if ((IsAnimal()) || (NState == AllVariables.Volume)) toploadindex = 2; // specialcases

                if (NState == AllVariables.DissRefrDetr) // special case 
            {
                toploadindex = 4; 
                if (index < 3) return "g / d"; //point and non-point source loadings
                else if (index == 3) return "pct. part.";
                else if (index == 4) return "pct. refr.";
            }

            if (index<=toploadindex)
            { 
                if (index == 1) // pointsource
                {
                    if (IsAnimal()) return "pct./day";  //special case 
                    if (NState == AllVariables.Volume) return "cu.m/d"; //special case 
                    return "g / d"; //point-source loadings
                }
                if (index == 2) // direct precip.
                {
                    if (IsAnimal()) return "g/m2 - d";  //special case 
                    if (NState == AllVariables.Volume) return "cu.m/d"; //special case 
                    return "g/m2 - d"; //direct precip. loadings
                }
                // (index == 3) // non-point source
                {
                    if (IsAnimal()) return "error"; //special case , not relevant to animals
                    if (NState == AllVariables.Volume) return "error"; //special case , not relevant to volume
                    if (NState == AllVariables.CO2) return "mg/L";  // special case equilibrium CO2 Import
                    return "g / d"; //non point-source loadings
                }
            }

            if ((NState >= Consts.FirstDetr) && (NState <= Consts.LastDetr)) return "ug/kg dry";
            else return "ug/kg wet";
        }


        public List<string> LoadList()
        {
            List<string> outList; 
            if ((NState == AllVariables.pH) || (NState == AllVariables.TSS) || (NState == AllVariables.Temperature) || (NState == AllVariables.Salinity) || (NState == AllVariables.COD))
                outList = new List<string>(new string[] { "Segment Values" });  //special case s, no inflow loadings, just in-segment driving values
            else if (NState == AllVariables.Volume) outList = new List<string>(new string[] { "Known Volume(s)", "Inflow Water", "Discharge Water"});  //special case 
            else if (NState == AllVariables.Light) outList = new List<string>(new string[] { "Top of Segment Loading" });  //special case 
            else if (IsAnimal()) outList = new List<string>(new string[] { "In Inflow Water", "Animal Removal", "Animal Stocking" });  //special case 
            else if (NState == AllVariables.DissRefrDetr) outList = new List<string>(new string[] { "In Inflow Water", "Point Source", "Non-Point Source", "Dissolved vs. Particulate", "Labile vs. Refractory" });  //special case for detritus
            else if (Has_Alt_Loadings()) outList = new List<string>(new string[] { "In Inflow Water", "Point Source", "Direct. Precip.", "Non-Point Source" });  
            else outList = new List<string>(new string[] { "In Inflow Water"});

            nontoxloadings = outList.Count;
            AQTSeg.AssignChemRecs();
            if (NState != AllVariables.H2OTox)
            for (T_SVType ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)  // add tox exposures
            {
                TToxics TT = AQTSeg.GetStatePointer(NState, ToxLoop, T_SVLayer.WaterCol) as TToxics;
                if (TT != null) outList.Add(TT.ChemRec.ChemName.Val +" exposure");

            }
            return outList;
        }


        public void UpdateUnits()
        {
            // *********************************
            // Sets the correct units given
            // the statevar type
            // coded by JSC, modified 7/22/98
            // *********************************
            if ((SVType >= Consts.FirstOrgTxTyp && SVType <= Consts.LastOrgTxTyp))
            {
                if ((NState == AllVariables.PoreWater))
                {
                    StateUnit = "ug/L";
                    LoadingUnit = "N A";
                }
                else if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.Cohesives) || (NState == AllVariables.POC_G1) ||
                     (NState == AllVariables.BuriedRefrDetr) || (NState == AllVariables.BuriedLabileDetr))
                {
                    StateUnit = "ug/kg dry";
                    LoadingUnit = "ug/kg dry";
                }
                else
                {
                    StateUnit = "ug/kg wet";
                    LoadingUnit = "ug/kg wet";
                }
            }
            else
            {
                switch (NState)
                {
                    case AllVariables.Light:
                        StateUnit = "Ly/d";
                        LoadingUnit = "Ly/d";
                        break;
                    case AllVariables.pH:
                        StateUnit = "pH";
                        LoadingUnit = "pH";
                        break;
                    case AllVariables.BuriedRefrDetr:
                    case AllVariables.BuriedLabileDetr:
                        StateUnit = "g/m2";
                        LoadingUnit = "N.A.";
                        break;
                    case AllVariables.Temperature:
                        StateUnit = "deg. C";
                        LoadingUnit = "deg. C";
                        break;
                    case AllVariables.Volume:
                        StateUnit = "cu.m";
                        LoadingUnit = "cu.m";
                        break;
                    case AllVariables.WindLoading:
                        StateUnit = "m/s";
                        LoadingUnit = "m/s";
                        break;
                    case AllVariables.PoreWater:
                        StateUnit = "cu.m/m2";
                        LoadingUnit = "N A";
                        break;
                    case AllVariables.Salinity:
                        StateUnit = "ppt";
                        LoadingUnit = "ppt";
                        break;
                    case AllVariables.Ammonia:
                    case AllVariables.Phosphate:
                    case AllVariables.Nitrate:
                    case AllVariables.Avail_Silica:
                    case AllVariables.COD:
                    case AllVariables.TAM:
                    case AllVariables.Silica:
                        if (Layer > T_SVLayer.WaterCol)
                        {
                            StateUnit = "g/m3";
                            LoadingUnit = "N A";
                        }
                        else
                        {
                            StateUnit = "mg/L";
                            LoadingUnit = "mg/L";
                        }
                        break;
                    case AllVariables.POC_G1:
                    case AllVariables.POC_G2:
                    case AllVariables.POC_G3:
                        StateUnit = "g C/m3";
                        LoadingUnit = "N A";
                        break;
                    case AllVariables.PON_G1:
                    case AllVariables.PON_G2:
                    case AllVariables.PON_G3:
                        StateUnit = "g N/m3";
                        LoadingUnit = "N A";
                        break;
                    case AllVariables.POP_G1:
                    case AllVariables.POP_G2:
                    case AllVariables.POP_G3:
                        StateUnit = "g P/m3";
                        LoadingUnit = "N A";
                        break;
                    case AllVariables.Methane:
                    case AllVariables.Sulfide:
                        if (Layer > T_SVLayer.WaterCol)
                        {
                            StateUnit = "g O2eq/ m3";
                            LoadingUnit = "N A";
                        }
                        else
                        {
                            StateUnit = "mg/L";
                            LoadingUnit = "mg/L";
                        }
                        break;
                    case AllVariables.LaDOMPore:
                    case AllVariables.ReDOMPore:
                        StateUnit = "g/cu.m";
                        LoadingUnit = "N A";
                        break;
                    //  AllVariables.Sand .. AllVariables.TSS, AllVariables.Cohesives .. AllVariables.NonCohesives2
                    case AllVariables.TSS:
                        {
                            StateUnit = "mg/L";
                            LoadingUnit = "mg/L";
                        }
                        break;
                    case AllVariables.H2OTox:
                        StateUnit = "ug/L";
                        LoadingUnit = "ug/L";
                        break;
                    case AllVariables pl when ((pl >= Consts.FirstPlant) && (pl <= Consts.LastPlant)):
                        if (SVType == T_SVType.StV)
                            ((TPlant)this).ChangeData();
                        else
                        {
                            // NIntrnl or PIntrnl
                            StateUnit = "ug/L";
                            LoadingUnit = "N A";
                        }
                        break;
                    case AllVariables.SedmRefrDetr:
                    case AllVariables.SedmLabDetr:
                        StateUnit = "g/m2 dry";
                        LoadingUnit = "N A";
                        break;
                    case AllVariables pl when ((pl >= Consts.FirstAnimal) && (pl <= Consts.LastAnimal)):
                        ((TAnimal)this).ChangeData();
                        break;
                    case AllVariables.DissRefrDetr:
                    case AllVariables.SuspLabDetr:
                        StateUnit = "mg/L dry";
                        LoadingUnit = "mg/L dry";
                        break;
                    default:
                        StateUnit = "mg/L";
                        LoadingUnit = "mg/L";
                        break;
                }
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
            // unit/d // m3/d // unit        // cu m.

            // WashoutStep[AllStates.DerivStep] = result * AllStates.SegVol(); // No MB Tracking for now
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
            if (Temp >= 19) Theta = Theta20;
            else Theta = 1.185 - 0.00729 * Temp;

            result = Math.Pow(Theta, (Temp - Location.Remin.TOpt.Val));

            if (Temp > Location.Remin.TMax.Val)
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

            if (((NState == AllVariables.SuspRefrDetr) || (NState == AllVariables.DissRefrDetr) || (NState == AllVariables.SedmRefrDetr)) && (SVType == T_SVType.StV))
                return 0;  // no decomposition of refractory SVs

            if (DecayMax < Consts.Tiny) return 0;

            if (State > Consts.VSmall)
            {
                // orgtox or stv
                if ((Layer == T_SVLayer.SedLayer1) || ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SedmRefrDetr)))
                {   // acct. for anoxia in sediment & near-sed. zone
                    HalfSatO = 8.0;
                }
                else HalfSatO = 0.5;

                // Bowie et al., 1985
                ReminRecord RR = Location.Remin;
                // T := AllStates.TCorr(Q10, TRef, TOpt, TMax);
                T = Decomposition_DecTCorr();
                p = AQTSeg.pHCorr(RR.pHMin.Val, RR.pHMax.Val);

                if (O2Conc == 0) Factor = 0;
                else Factor = O2Conc / (HalfSatO + O2Conc);

                DOCorr = Factor + (1.0 - Factor) * KAnaer / DecayMax;
                if (DOCorr > Consts.Tiny) FracAerobic = (Factor / DOCorr);

                // Return Fraction of Decomp. that is Aerobic for BioTransformation Calc.
                EnvironLim = DOCorr * T * p;
                KMicrob = DecayMax * EnvironLim;
                // 1/d    // 1/d      // frac
                Decomp = KMicrob * State;
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
                   || (NState == AllVariables.SmGameFish1) || (NState == AllVariables.SmGameFish2) || (NState == AllVariables.SmGameFish3) || (NState == AllVariables.SmGameFish4)) && (SVType == T_SVType.StV);
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
                    return RR.Wet2DrySRefr.Val;
                case AllVariables.SedmLabDetr:
                    return RR.Wet2DrySLab.Val;
                case AllVariables.SuspRefrDetr:
                    return RR.Wet2DryPRefr.Val;
                case AllVariables.SuspLabDetr:
                    return RR.Wet2DryPLab.Val;
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
            }  
        }

        public double NutrToOrg(AllVariables S)
        {
            TPlant PP;
            TAnimal PA;
            ReminRecord LR = Location.Remin;
            bool Nitr = ((S == AllVariables.Nitrate) || (S == AllVariables.Ammonia));
            if (Nitr)
            {
                if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SuspRefrDetr)) return LR.N2OrgRefr.Val;
                if ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SuspLabDetr)) return LR.N2OrgLab.Val;
                if (NState == AllVariables.DissRefrDetr) return LR.N2OrgDissRefr.Val;
                if (NState == AllVariables.DissLabDetr) return LR.N2OrgDissLab.Val;

                if ((NState >= Consts.FirstAnimal) && (NState <= Consts.LastAnimal))
                {
                    PA = this as TAnimal;
                    return PA.PAnimalData.N2Org.Val;
                }

                PP = this as TPlant;  // must be a plant
                return PP.N_2_Org();
            }
            else
            {
                if ((NState == AllVariables.SedmRefrDetr) || (NState == AllVariables.SuspRefrDetr)) return LR.P2OrgRefr.Val;
                if ((NState == AllVariables.SedmLabDetr) || (NState == AllVariables.SuspLabDetr)) return LR.P2OrgLab.Val;
                if (NState == AllVariables.DissRefrDetr) return LR.P2OrgDissRefr.Val;
                if (NState == AllVariables.DissLabDetr) return LR.P2OrgDissLab.Val;

                if ((NState >= Consts.FirstAnimal) && (NState <= Consts.LastAnimal))
                {
                    PA = this as TAnimal;
                    return PA.PAnimalData.P2Org.Val;
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
                double EgestRet = 0;
                double GER = 0;
                if (P.IsAnimal())
                {
                    PA = ((P) as TAnimal);
                    Prd = Prd + PA.IngestSpecies(ns, null, ref EgestRet, ref GER);
                }
            }
            //-------------------------------------------------------------------------

            if ((NState == AllVariables.PON_G1) || (NState == AllVariables.POP_G1) || (NState == AllVariables.POC_G1))
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

        public void ClearRate()
        {
            RateIndex = -1;
            if (RateColl == null)
                RateColl = new List<TRate>();
        }

        public void SaveRate(string Nm, double Rt)
        {
            TRate PR;
            RateIndex++;
            if (RateIndex > RateColl.Count - 1)
            {
                PR = new TRate(Nm);
                RateColl.Add(PR);
            }
            PR = RateColl[RateIndex];
            PR.Rate[AQTSeg.DerivStep] = Rt;
        }

        // return output header stint for graphing/export purposes
        public string OutputText(int col)
        {
            if (SVoutput == null) return "";
            return SVoutput.Metadata["State_Variable"] + " " +
                   SVoutput.Metadata["Name_" + col.ToString()] +
                   " (" + SVoutput.Metadata["Unit_" + col.ToString()] + ")";
        }

    }  // end TStateVariable


    public class TStates : List<TStateVariable>
    {
        [JsonIgnore] public List<DateTime> restimes = new List<DateTime>();
    }

    public class SeriesID
    {
        public AllVariables ns;
        public T_SVType typ;
        public T_SVLayer lyr;
        public int indx; //column number in TimeSeriesOutput
        public string nm;
    }

    public class TGraphSetup
    
    
    {
        public string GraphName ;
        public List<SeriesID> YItems = new List<SeriesID>();
        public string Y1Label;  // not currently used

        // public Color[,] Colors;
        // public TSeriesPointerStyle[,] Shapes;
        //public ushort[,] LineThick;
        // public ushort[,] Size;
        // public string XLabel;
        //public string Y2Label;
        //public bool Y1AutoScale;
        //public bool Y2AutoScale;
        //public bool AutoScaleAll;
        //public double Y1Min;
        //public double Y1Max;
        //public double Y2Min;
        //public double Y2Max;
        //public bool Use2Scales;
        //public double XMin;
        //public double XMax;

        public TGraphSetup(string st)
        {
            GraphName = st;
        }

    }



    public class TGraphs
    {
        public int SelectedGraph = 0;
        public List<TGraphSetup> GList;

        public void AddGraph(TGraphSetup G)
        {
            GList.Add(G);
        }

        public void DeleteGraph(int Index)
        {
            GList.Remove(GList[Index]);
        }

        //Constructor  Create()
        public TGraphs()
        {
            GList = new List<TGraphSetup>();
            SelectedGraph = 0;
        }
    } // end TGraphs



    public class AQUATOXSegment
    {
        public TAQTSite Location = new TAQTSite();       // Site data structure

        public TStates SV = new TStates();    // State Variables
        public DateTime TPresent;
        public string StudyName;
        public string RunID = "";
        public string FileName = "";

        public Setup_Record PSetup;

        public bool UseConstEvap = true;
        public TLoadings DynEvap = null;
        public bool UseConstZMean = true;
        public TLoadings DynZMean;
        public LoadingsRecord Shade;
        public TGraphs Graphs = new TGraphs();

        public bool CalcVelocity = true;
        public TLoadings DynVelocity = null;
        public double MeanDischarge = 0;   // output only
        public double residence_time = 1;  // water residence time in days

        public Diagenesis_Rec Diagenesis_Params;
        public bool Diagenesis_Steady_State = false;  // whether to calculate layer 1 as steady state

        public double MF_Spawn_Age = 3.0;  // multi-fish age where fish spawn relevant for age classes only, default is 3.0

        public Loadings.TLoadings BenthicBiomass_Link = null; // optional linkage for diagenesis simulations when benthos not directly simulated, g/m2
        public Loadings.TLoadings AnimalDef_Link = null; // optional linkage to sediment from animal defecation for diagenesis simulations when animals not directly simulated, g/m2

        [JsonIgnore] public BackgroundWorker ProgWorker = null; // report progress and handle cancellation (BackgroundWorker)
        [JsonIgnore] public IProgress<int> ProgHandle = null;   // report progress (Task.Run)
        [JsonIgnore] public CancellationToken _ct = CancellationToken.None; // handle cancellation (Task.Run)

        [JsonIgnore] public double SOD = 0;   // SOD, calculated before derivatives
        [JsonIgnore] public int DerivStep;    // Current Derivative Step 1 to 6, Don't save in json  

        [JsonIgnore] public DateTime SimulationDate = DateTime.MinValue;  // time integration started
        [JsonIgnore] public DateTime VolumeUpdated;  // 
        [JsonIgnore] public double MeanVolume;       // 
        [JsonIgnore] public double Volume_Last_Step;   // Volume in the previous step, used for calculating dilute/conc,  if stratified, volume of whole system(nosave)}  
        [JsonIgnore] public bool Anoxic = false;       // Is System Anoxic , nosave
        [JsonIgnore] public DateTime[] FirstExposure = new DateTime[Consts.NToxs];   // First exposure to organic toxicant, nosave

        [JsonIgnore] public DateTime ModelStartTime;     // Start of model run
        [JsonIgnore] public int YearNum_PrevStep = 0;     // The year number during the previous step of the model run; used to determine when a year has passed

        [JsonIgnore] public double Last_Non_Zero_Vol;     // Used in dilute/conc to ensure no problems when volume is too small or a stream with zero flow
        [JsonIgnore] public bool Water_Was_Zero;
        [JsonIgnore] public bool WaterVolZero;            //  Is Water Volume "Zero", or a stream with zero flow?  No integration will take place.

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

        public bool Has_Nutrient_Model()
        {
            TNH4Obj PNH4 = (TNH4Obj)GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            TNO3Obj PNO3 = (TNO3Obj)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            TPO4Obj PPO4 = (TPO4Obj)GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);

            bool NH4model = (PNH4 != null);
            bool NO3model = (PNO3 != null);
            bool PO4model = (PPO4 != null);

            if (NH4model) NH4model = !(PNH4.UseLoadsRecAsDriver);
            if (NO3model) NO3model = !(PNO3.UseLoadsRecAsDriver);
            if (PO4model) PO4model = !(PPO4.UseLoadsRecAsDriver);

            return (NH4model || NO3model || PO4model);
        }

        public bool Has_OM_Model()
        {
            TDissRefrDetr PDRD = (TDissRefrDetr)GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSuspRefrDetr PSRD = (TSuspRefrDetr)GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSedRefrDetr PSdRD = (TSedRefrDetr)GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);

            bool PDRDmodel = (PDRD != null);
            bool PSRDmodel = (PSRD != null);
            bool PSdRDmodel = (PSdRD != null);

            if (PDRDmodel) PDRDmodel = !(PDRD.UseLoadsRecAsDriver);
            if (PSRDmodel) PSRDmodel = !(PSRD.UseLoadsRecAsDriver);
            if (PSdRDmodel) PSdRDmodel = !(PSdRD.UseLoadsRecAsDriver);

            return (PDRDmodel || PSRDmodel || PSdRDmodel);
        }

        public bool Has_Plant_Model()
        {
            for (AllVariables nS = Consts.FirstPlant; nS <= Consts.LastPlant; nS++)
            {
                TStateVariable TPl = GetStatePointer(nS, T_SVType.StV, T_SVLayer.WaterCol);
                if (TPl != null)
                {
                    if (!(TPl.UseLoadsRecAsDriver)) return true;
                }
            }
            return false;
        }



        public bool Has_Animal_Model()
        {
            for (AllVariables nS = Consts.FirstAnimal; nS <= Consts.LastAnimal; nS++)
            {
                TStateVariable TAn = GetStatePointer(nS, T_SVType.StV, T_SVLayer.WaterCol);
                if (TAn != null)
                {
                    if (!(TAn.UseLoadsRecAsDriver)) return true;
                }
            }
            return false;
        }


        public bool Has_Chemicals()
        {
            for (T_SVType ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
            {
                if (GetStatePointer(AllVariables.H2OTox, ToxLoop, T_SVLayer.WaterCol) != null) return true;
            }
            return false;
        }


        public TStateVariable AddStateVariable(AllVariables NS, T_SVLayer Lyr)
        {
            TStateVariable P = null;

            // Add the NS variable and the associated toxicity S.V. if appropriate
            if (NS == AllVariables.Volume) P = new TVolume(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Temperature) P = new TTemperature(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.WindLoading) P = new TWindLoading(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Light) P = new TLight(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.pH) P = new TpHObj(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Phosphate)
            {
                if (Lyr > T_SVLayer.WaterCol)
                    P = new TPO4_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
                else
                    P = new TPO4Obj(NS, T_SVType.StV, Lyr, "", this, 0);
            }
            else if (NS == AllVariables.Ammonia)
            {
                if (Lyr > T_SVLayer.WaterCol)
                    P = new TNH4_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
                else
                    P = new TNH4Obj(NS, T_SVType.StV, Lyr, "", this, 0);
            }
            else if (NS == AllVariables.Nitrate)
            {
                if (Lyr > T_SVLayer.WaterCol)
                    P = new TNO3_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
                else
                    P = new TNO3Obj(NS, T_SVType.StV, Lyr, "", this, 0);
            }
            else if ((NS == AllVariables.Silica) || (NS == AllVariables.Avail_Silica))
                P = new TSilica_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Sulfide)
                P = new TSulfide_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Methane)
                P = new TMethane(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.COD)
                P = new TCOD(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS >= AllVariables.POC_G1) && (NS <= AllVariables.POC_G3))
                P = new TPOC_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS == AllVariables.POP_G1) && (NS <= AllVariables.POP_G3))
                P = new TPOP_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS == AllVariables.PON_G1) && (NS <= AllVariables.PON_G3))
                P = new TPON_Sediment(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.CO2)
                P = new TCO2Obj(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Oxygen)
                P = new TO2Obj(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.Salinity)
                P = new TSalinity(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.TSS)
                P = new TSandSiltClay(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.SedmLabDetr)
                P = new TSedLabileDetr(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.SedmRefrDetr)
            {
                if (Lyr > T_SVLayer.WaterCol)
                    P = new TSedRefrDetr(NS, T_SVType.StV, Lyr, "", this, 0);
                else if (NS == AllVariables.SuspRefrDetr)
                    P = new TSuspRefrDetr(NS, T_SVType.StV, Lyr, "", this, 0);
            }
            else if (NS == AllVariables.SuspLabDetr)
                P = new TSuspLabDetr(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.DissRefrDetr)
                P = new TDissRefrDetr(NS, T_SVType.StV, Lyr, "", this, 0);
            else if (NS == AllVariables.DissLabDetr)
                P = new TDissLabDetr(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS >= Consts.FirstAlgae) && (NS <= Consts.LastAlgae))
                P = new TPlant(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS >= Consts.FirstMacro) && (NS <= Consts.LastMacro))
                P = new TMacrophyte(NS, T_SVType.StV, Lyr, "", this, 0);
            else if ((NS >= Consts.FirstAnimal) && (NS <= Consts.LastAnimal))
                P = new TAnimal(NS, T_SVType.StV, Lyr, "", this, 0);
            else P = null;

            if (P == null) return null;
            int SVIndex = 0;
            while ((NS > SV[SVIndex].NState) && (SVIndex < SV.Count())) SVIndex++;

            SV.Insert(SVIndex, P);
            SetMemLocRec();

            // Add OrgTox SVs if one or more H2OTox variables are included
            for (T_SVType ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
            {
                if (GetStatePointer(AllVariables.H2OTox, ToxLoop, T_SVLayer.WaterCol) != null)
                    AddOrgToxStateVariable(NS, Lyr, ToxLoop);
            }

            //if ((NS >= Consts.FirstPlant && NS <= Consts.LastPlant) && SV.SetupRec.Internal_Nutrients)
            //{
            //    AddInternalNutSVs(NS);
            //}

            return P;
        }

        public void DeleteVar(TStateVariable PSV)
        {
            for (int i = 0; i < SV.Count; i++)
                if (SV[i] == PSV) { DeleteVar(i); return; }
        }


        public void DeleteVar(int index)
        {
            SV.RemoveAt(index);
            SetMemLocRec();
        }

        public void RemoveSV(int index)
        {
            SV.RemoveAt(index);
            SetMemLocRec();
        }


        public void RemoveOrgToxStateVariable(int index)
        {
            T_SVType RemoveTyp = SV[index].SVType;
            for (int i = SV.Count - 1; i >= 0; i--)
                if (SV[i].SVType == RemoveTyp) DeleteVar(i);
        }


        public void AddOrgToxStateVariable(AllVariables NS, T_SVLayer Lyr, T_SVType ToxType)
        {
            TStateVariable P;

            if ((NS == AllVariables.SuspRefrDetr) || (NS == AllVariables.SuspLabDetr))
                P = new TParticleTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS == AllVariables.DissRefrDetr) || (NS == AllVariables.DissLabDetr))
                P = new TParticleTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS == AllVariables.SedmRefrDetr) || (NS == AllVariables.SedmLabDetr))
                P = new TParticleTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS >= Consts.FirstPlant) && (NS <= Consts.LastPlant))
                P = new TAlgaeTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS >= Consts.FirstInvert) && (NS <= Consts.LastInvert))
                P = new TAnimalTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS >= Consts.FirstFish) && (NS <= Consts.LastFish))
                P = new TAnimalTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);
            else if ((NS >= AllVariables.POC_G1) && (NS <= AllVariables.POC_G3))
                P = new TPOCTox(NS, NS, ToxType, Lyr, "Undisplayed", this, 0);

            //            if (NS == AllVariables.BuriedLabileDetr)
            //                P = new TBuriedDetrTox(NS, NS, ToxType, Lyr, "Undisplayed", SV, 0);  //sequestered organic matter not in HMS version
            //            else if (NS ==  AllVariables.BuriedRefrDetr)
            //                P = new TBuriedDetrTox1(NS, NS, ToxType, Lyr, "Undisplayed", SV, 0);

            else P = null;

            if (P != null) SV.Add(P);
            SetMemLocRec();

            // set up tox loadings for susp&Diss detritus
            if (NS == AllVariables.DissRefrDetr)
            {
                TDissRefrDetr PDRD = GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol) as TDissRefrDetr;
                int TI = PDRD.ToxInt(ToxType);
                PDRD.InputRecord.ToxInitCond[TI] = 0;
                PDRD.InputRecord.ToxLoad[TI] = new LoadingsRecord();
                PDRD.InputRecord.ToxLoad[TI].Loadings.UseConstant = true;
                PDRD.InputRecord.ToxLoad[TI].Loadings.ConstLoad = 0;
                PDRD.InputRecord.ToxLoad[TI].Loadings.MultLdg = 1.0;
                foreach (TLoadings LR in PDRD.InputRecord.ToxLoad[TI].Alt_Loadings)
                {
                    LR.UseConstant = true;
                    LR.ConstLoad = 0;
                    LR.MultLdg = 1.0;
                }
            }
        }


        public TStateVariable Add_OrgTox_SVs(ChemicalRecord CR)
        {
            T_SVType ToxType = T_SVType.OrgTox1;
            while ((ToxType < T_SVType.OrgTox20) && (GetStatePointer(AllVariables.H2OTox, ToxType, T_SVLayer.WaterCol) != null))
                ToxType++;

            if (GetStatePointer(AllVariables.H2OTox, ToxType, T_SVLayer.WaterCol) != null) return null;  // no open slots for organic chemicals

            TToxics ChemVar = new TToxics(AllVariables.H2OTox, AllVariables.H2OTox, ToxType, T_SVLayer.WaterCol, "", this, 0);
            if (ChemVar == null) return null;

            ChemVar.ChemRec = CR;
            int SVIndex = ChemVar.ToxInt(ToxType);
            SV.Insert(SVIndex, ChemVar);
            SetMemLocRec();

            // ChemVar.ChangeData();  

            // For each state variable, add associated toxicant if relevant
            for (AllVariables ToxAddLoop = Consts.FirstState; ToxAddLoop <= Consts.LastState; ToxAddLoop++)
                for (T_SVLayer ToxSedLoop = T_SVLayer.WaterCol; ToxSedLoop <= T_SVLayer.SedLayer2; ToxSedLoop++)
                {
                    TStateVariable p = GetStatePointer(ToxAddLoop, T_SVType.StV, ToxSedLoop);
                    if (p != null) AddOrgToxStateVariable(ToxAddLoop, ToxSedLoop, ToxType);
                }

            SetMemLocRec();
            return ChemVar;
        }

        public string Verify_Runnable()
        {
            SetMemLocRec();
            if (SV.Count < 1) return "No State Variables Are Included in this Simulation.";
            if (Equals(PSetup, default(Setup_Record))) return "PSetup data structure must be initialized.";
            if (PSetup.StoreStepSize.Val < Consts.Tiny) return "PSetup.StoreStepSize must be greater than zero.";
            if (PSetup.FirstDay.Val >= PSetup.LastDay.Val) return "In PSetup Record, Last Day must be after First Day.";

            string dr = AQTVolumeModel_CheckDataRequirements();
            if (dr != "") return dr;

            dr = AQTBioaccumulationModel_CheckDataRequirements();
            if (dr != "") return dr;

            if (Has_Chemicals()) dr = AQTChemicalModel_CheckDataRequirements();
            if (dr != "") return dr;

            if (Diagenesis_Included()) dr = AQTDiagenesisModel_CheckDataRequirements();
            if (dr != "") return dr;

            if (Has_Nutrient_Model()) dr = AQTNutrientModel_CheckDataRequirements();
            if (dr != "") return dr;

            if (Has_OM_Model()) dr = AQTOrganicMatter_CheckDataRequirements();
            if (dr != "") return dr;

            if (Has_Animal_Model()) dr = AQTAnimalModel_CheckDataRequirements();
            if (dr != "") return dr;

            if (Has_Plant_Model()) dr = AQTPlantModel_CheckDataRequirements();
            if (dr != "") return dr;

            return "";
        }

        public string AQTVolumeModel_CheckDataRequirements()
        {

            TVolume TVol = (TVolume)GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            if (TVol == null) return "A Volume State Variable must be included in the simulation. ";
            if (Location == null) return "The 'Location' object must be populated with site data. ";
            if (Location.Locale == null) return "The 'Location.Locale' object must be populated with site data. ";
            if (Location.Locale.SiteLength.Val < Consts.Tiny) return "SiteLength must be greater than zero.";
            if (Location.SiteType == SiteTypes.Stream)
            {
                if (Location.Locale.Channel_Slope.Val < Consts.Tiny) return "Channel_Slope must be greater than zero to use Mannings Equation.";
            }

            return "";
        }

        public string AQTBioaccumulationModel_CheckDataRequirements()
        {
            foreach (TToxics TT in SV.OfType<TToxics>())
            {
                if (TT.NState != AllVariables.H2OTox)  // if this is a bioaccumulation state variable and not a toxicant in water
                {
                    TToxics TTx = GetStatePointer(AllVariables.H2OTox, TT.SVType, T_SVLayer.WaterCol) as TToxics;
                    if (TTx == null) return "The bioaccumulation state variable " + TT.PName + " is present, but the relevant chemical is not present as a state or a driving variable.";

                    TStateVariable Carry = GetStatePointer(TT.NState, T_SVType.StV, T_SVLayer.WaterCol);
                    if (Carry == null) return "The bioaccumulation state variable " + TT.PName + " is present, but its carrying organism is not in the simulation.";
                }

                if (TT.NState == AllVariables.H2OTox) // chemical in water, so ensure it's in all biota and organic matter
                    for (AllVariables ns = Consts.FirstDetr; ns <= Consts.LastBiota; ns++)
                    {
                        TStateVariable carrier = GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
                        if (carrier != null)
                        {
                            TStateVariable TTx = GetStatePointer(carrier.NState, TT.SVType, T_SVLayer.WaterCol);
                            if (TTx == null) return "The chemical " + TT.PName + " is in the simulation but not the bioaccumulation state variable for " + carrier.PName;
                        }
                    }

            }
            return "";
        }

        public string AQTChemicalModel_CheckDataRequirements()
        {
            bool FoundTox = false;
            for (T_SVType Typ = T_SVType.OrgTox1; Typ <= T_SVType.OrgTox20; Typ++)
            {
                TToxics TTx = (TToxics)GetStatePointer(AllVariables.H2OTox, Typ, T_SVLayer.WaterCol);
                if (TTx != null) { FoundTox = true; break; }
            }
            if (!FoundTox) return "A TToxics (toxicant in the water column) state variable must be included in the simulation. ";

            TO2Obj TO2 = (TO2Obj)GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if (TO2 == null) return "An Oxygen state variable or driving variable must be included in a chemical simulation. ";

            TTemperature TTemp = (TTemperature)GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in in a chemical simulation. ";

            TpHObj TpH = (TpHObj)GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if (TpH == null) return "A pH loading variable or state variable must be included in a chemical simulation.";
            if ((!TpH.UseLoadsRecAsDriver) && (TpH.LoadsRec.Loadings.NoUserLoad))  // pH calculation, not a driving variable, check pH model data requirements
            {
                TCO2Obj TCO2 = (TCO2Obj)GetStatePointer(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
                if (TCO2 == null) return "A CO2 state variable or driving variable must be included in the simulation to calculate pH. ";
            }

            TLight TLt = (TLight)GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (TLt == null) return "A Light state variable or driving variable must be included in the simulation. ";

            return "";
        }

        public string AQTEcotoxicologyModel_CheckDataRequirements()
        {
            bool FoundTox = false;
            for (T_SVType Typ = T_SVType.OrgTox1; Typ <= T_SVType.OrgTox20; Typ++)
            {
                TToxics TTx = (TToxics)GetStatePointer(AllVariables.H2OTox, Typ, T_SVLayer.WaterCol);
                if (TTx != null) { FoundTox = true; break; }
            }
            if (!FoundTox) return "A TToxics (toxicant in the water column) state variable must be included in the simulation. ";

            bool FoundBiota = false;
            for (AllVariables ns = Consts.FirstBiota; ns <= Consts.LastBiota; ns++)
            {
                TStateVariable biota = GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
                if (biota != null) { FoundBiota = true; break; }
            }
            if (!FoundBiota) return "To calculate ecotoxicological effects, an animal or plant state variable must be included in the model. ";

            if (!PSetup.UseExternalConcs.Val)
                return AQTBioaccumulationModel_CheckDataRequirements(); // To calculate effects of chemicals based on internal body burdens, a bioaccumulation model must be included.

            return "";
        }

        string CheckForVar(AllVariables ns, T_SVType typ, T_SVLayer wc, string name)
        {
            TStateVariable TSV = GetStatePointer(ns, typ, wc);
            if (TSV == null) return "The Diagenesis simulation is missing a required " + name + " state variable.";
            return "";
        }

        public string AQTOrganicMatter_CheckDataRequirements()
        {
            TDissRefrDetr PDRD = (TDissRefrDetr)GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TDissLabDetr PDLD = (TDissLabDetr)GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSuspRefrDetr PSRD = (TSuspRefrDetr)GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSuspLabDetr PSLD = (TSuspLabDetr)GetStatePointer(AllVariables.SuspLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TStateVariable PSdRD = GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TStateVariable PSdLD = GetStatePointer(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
            if (PSdRD == null) PSdRD = GetStatePointer(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.SedLayer2); // check for diagenesis option
            if (PSdLD == null) PSdLD = GetStatePointer(AllVariables.POC_G2, T_SVType.StV, T_SVLayer.SedLayer2); // check for diagenesis option

            if ((PDRD == null) || (PDLD == null) || (PSRD == null) || (PSLD == null) || (PSdRD == null) || (PSdLD == null))
                return "All six organic matter state variables must be included in an organic-matter simulation.";

            TpHObj PpH = (TpHObj)GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PpH == null)) return "A pH state variable (or driving variable) must be included in an organic-matter simulation.";

            TO2Obj PO2 = (TO2Obj)GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PO2 == null)) return "An oxygen state variable (or driving variable) must be included in an organic-matter simulation.";

            TNO3Obj PN = (TNO3Obj)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PN == null)) return "Nitrogen and Ammonia variables (or driving variable) must be included in an organic-matter simulation.";

            return "";
        }

        public string AQTDiagenesisModel_CheckDataRequirements()
        {
            string rstr = "";

            rstr = CheckForVar(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.SedLayer2, "POC_G1");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.POC_G2, T_SVType.StV, T_SVLayer.SedLayer2, "POC_G2");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.POC_G3, T_SVType.StV, T_SVLayer.SedLayer2, "POC_G3");
            if (rstr != "") return rstr;

            rstr = CheckForVar(AllVariables.PON_G1, T_SVType.StV, T_SVLayer.SedLayer2, "PON_G1");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.PON_G2, T_SVType.StV, T_SVLayer.SedLayer2, "PON_G2");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.PON_G3, T_SVType.StV, T_SVLayer.SedLayer2, "PON_G3");
            if (rstr != "") return rstr;

            rstr = CheckForVar(AllVariables.POP_G1, T_SVType.StV, T_SVLayer.SedLayer2, "POP_G1");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.POP_G2, T_SVType.StV, T_SVLayer.SedLayer2, "POP_G2");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.POP_G3, T_SVType.StV, T_SVLayer.SedLayer2, "POP_G3");
            if (rstr != "") return rstr;

            rstr = CheckForVar(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer1, "Phosphate in Sed Layer 1");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer1, "Ammonia in Sed Layer 1");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer1, "Nitrate in Sed Layer 1");
            if (rstr != "") return rstr;

            rstr = CheckForVar(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.SedLayer2, "Phosphate in Sed Layer 2");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.SedLayer2, "Ammonia in Sed Layer 2");
            if (rstr != "") return rstr;
            rstr = CheckForVar(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.SedLayer2, "Nitrate in Sed Layer 2");
            if (rstr != "") return rstr;

            return "";
        }

        public string AQTNutrientModel_CheckDataRequirements()
        {

            TO2Obj TO2 = (TO2Obj)GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if (TO2 == null) return "An Oxygen state variable or driving variable must be included in the simulation. ";

            TNH4Obj PNH4 = (TNH4Obj)GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            TNO3Obj PNO3 = (TNO3Obj)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            TPO4Obj PPO4 = (TPO4Obj)GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);

            if ((PNH4 == null) && (PPO4 == null) && (TO2 == null)) return "Either phosphorus (TPO4Obj) or nitrogen (TNH4Obj and TNO3Obj) or Oxygen (TO2Obj) must be included in a nutrients simulation.";
            if (((PNH4 != null) && (PNO3 == null)) || ((PNH4 == null) && (PNO3 != null))) return "To model nitrogen both ammonia and nitrate (TNH4Obj and TNO3Obj) must be included.";

            TTemperature TTemp = (TTemperature)GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in the simulation. ";

            TpHObj TpH = (TpHObj)GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if (TpH == null) return "A pH loading variable or state variable must be included in a nutrients simulation.";
            if ((!TpH.UseLoadsRecAsDriver) && (TpH.LoadsRec.Loadings.NoUserLoad))  // pH calculation, not a driving variable, check pH model data requirements
            {
                TCO2Obj TCO2 = (TCO2Obj)GetStatePointer(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
                if (TCO2 == null) return "A CO2 state variable or driving variable must be included in the simulation to calculate pH. ";
            }

            return "";
        }

        public string AQTAnimalModel_CheckDataRequirements()
        {
            bool FoundAnimal = false;
            for (AllVariables nS = Consts.FirstAnimal; nS <= Consts.LastAnimal; nS++)
            {
                TStateVariable TAn = GetStatePointer(nS, T_SVType.StV, T_SVLayer.WaterCol);
                if (TAn != null)
                {
                    FoundAnimal = true;
                }
            }
            if (!FoundAnimal) return "A TAnimal state variable must be included in the simulation. ";

            TO2Obj PCO2 = (TO2Obj)GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PCO2 == null)) return "An O2 state variable (or driving variable) must be included in an animal simulation.";

            TTemperature TTemp = (TTemperature)GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in the simulation. ";

            return "";
        }


        public string AQTPlantModel_CheckDataRequirements()
        {
            bool FoundPlant = false;
            for (AllVariables nS = Consts.FirstPlant; nS <= Consts.LastPlant; nS++)
            {
                TStateVariable TPl = GetStatePointer(nS, T_SVType.StV, T_SVLayer.WaterCol);
                if (TPl != null)
                {
                    FoundPlant = true;
                    if ((PSetup.Internal_Nutrients.Val) && (nS <= Consts.LastAlgae))  // Exclude Macrophytes
                    {
                        TStateVariable TIn = GetStatePointer(nS, T_SVType.NIntrnl, T_SVLayer.WaterCol);
                        if (TIn == null) return "Internal Nutrients in plants have been selected but there is no internal nitrogen state variable for " + TPl.PName;

                        TIn = GetStatePointer(nS, T_SVType.PIntrnl, T_SVLayer.WaterCol);
                        if (TIn == null) return "Internal Nutrients in plants have been selected but there is no internal phosphorus state variable for " + TPl.PName;
                    }
                }
            }
            if (!FoundPlant) return "A TPlant state variable must be included in the simulation. ";

            TpHObj PpH = (TpHObj)GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PpH == null)) return "A pH state variable (or driving variable) must be included in a plant simulation.";

            TCO2Obj PCO2 = (TCO2Obj)GetStatePointer(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PCO2 == null)) return "A CO2 state variable (or driving variable) must be included in a plant simulation.";

            TTemperature TTemp = (TTemperature)GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in the simulation. ";

            TNH4Obj PNH4 = (TNH4Obj)GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            if (PNH4 == null) return "An Ammonia (TNH4Obj) state variable or driving variable must be included in the simulation. ";

            TNO3Obj PNO3 = (TNO3Obj)GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            if (PNO3 == null) return "A Nitrate (TNO3Obj) state variable or driving variable must be included in the simulation. ";

            TPO4Obj PPO4 = (TPO4Obj)GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            if (PPO4 == null) return "A Phosphate (TPO4Obj) state variable or driving variable must be included in the simulation. ";

            TLight TLt = (TLight)GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (TLt == null) return "A Light state variable or driving variable must be included in the simulation. ";

            return "";
        }


        public void ClearResults()
        {

            foreach (TStateVariable TSV in SV)
            {
                if (TSV.SVResults != null) TSV.SVResults.ClearResults();
                TSV.SVoutput = null;
            }

            if (SV.restimes != null) SV.restimes.Clear(); else SV.restimes = new List<DateTime>();

        }


        public void DisplayNames(ref List<string> List, ref List<TStateVariable> TSVList)
        {
            // Puts all statevariables from collection into TStrings item for screen
            // display:   Ordered by enumerated list position. JonC
            TStateVariable TSV;
            AllVariables SVLoop;
            T_SVType SVTLoop;
            string Name;

            if (List == null) List = new List<string>();
            List.Clear();
            if (TSVList == null) TSVList = new List<TStateVariable>();
            TSVList.Clear();

            for (SVTLoop = Consts.FirstOrgTxTyp; SVTLoop <= Consts.LastOrgTxTyp; SVTLoop++)  //list chemicals first
            {
                TSV = GetStatePointer(AllVariables.H2OTox, SVTLoop, T_SVLayer.WaterCol);
                if (TSV != null)
                {
                    Name = TSV.PName;
                    if (Name != "Undisplayed")
                    {
                        List.Add(Name);
                        TSVList.Add(TSV);
                    }
                }
            }

            for (SVLoop = Consts.FirstState; SVLoop <= Consts.LastState; SVLoop++) // list state variables
            {
                TSV = GetStatePointer(SVLoop, T_SVType.StV, T_SVLayer.WaterCol);
                if ((TSV != null) && (SVLoop != AllVariables.DissLabDetr) && (SVLoop != AllVariables.SuspLabDetr) && (SVLoop != AllVariables.SuspRefrDetr))
                {
                    Name = TSV.PName;
                    if (SVLoop == AllVariables.DissRefrDetr) Name = "Suspended & Dissolved Detritus";  //special case, four state variables govered by one GUI screen
                    if (Name != "Undisplayed")
                    {
                        List.Add(Name);
                        TSVList.Add(TSV);
                    }
                }
            }
        }

        public string StateText(AllVariables ns)
        {
            string outtext = ns.ToString();
            if (ns == AllVariables.Volume) outtext = "Water Volume";
            if (ns == AllVariables.Phosphate) outtext = "Phosphate as P";
            if (ns == AllVariables.Ammonia) outtext = "Total Ammonia as N";
            if (ns == AllVariables.Nitrate) outtext = "Nitrate as N";
            if (ns == AllVariables.CO2) outtext = "Carbon dioxide";
            if (ns == AllVariables.TSS) outtext = "Tot. Susp. Solids";
            if (ns == AllVariables.SedmRefrDetr) outtext = "Refrac. sed. detritus";
            if (ns == AllVariables.SedmLabDetr) outtext = "Labile sed. detritus";
            if (ns == AllVariables.DissRefrDetr) outtext = "Susp. and dissolved detritus";
            if (ns == AllVariables.BlGreens1) outtext = "Cyanobacteria1";
            if (ns == AllVariables.BlGreens2) outtext = "Cyanobacteria2";
            if (ns == AllVariables.BlGreens3) outtext = "Cyanobacteria3";
            if (ns == AllVariables.BlGreens4) outtext = "Cyanobacteria4";
            if (ns == AllVariables.BlGreens5) outtext = "Cyanobacteria5";
            if (ns == AllVariables.BlGreens6) outtext = "Cyanobacteria6";
            if (ns == AllVariables.H2OTox) outtext = "Dissolved org. tox";

            return outtext;
        }

        public List<string> GetInsertableStates(ref List<AllVariables> SVs)
        {
            List<string> list = new List<string>();
            SVs = new List<AllVariables>();

            for (AllVariables ns = Consts.FirstState; ns < Consts.LastState; ns++)
            {
                if (GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol) == null)
                {
                    if ((ns == AllVariables.H2OTox) || (ns == AllVariables.TSS) ||  // some variables vestigal from AQUATOX 3.2, not shown here
                        ((ns >= AllVariables.Ammonia) && (ns <= AllVariables.Oxygen)) ||
                        ((ns >= AllVariables.Salinity) && (ns <= AllVariables.LgGameFish4)) ||
                        ((ns >= AllVariables.Volume) && (ns <= AllVariables.pH)))
                    { list.Add(StateText(ns));
                        SVs.Add(ns);
                    }

                }
            }

            return list;
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

            // Parallel.ForEach(SV, TSV => TSV.TakeDerivative(Step));  FIXME Consider Enabling Parallel.ForEach when not debugging... uncertain how much improvement due to transactional costs

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

            if (PSetup.UseFixStepSize.Val)
            {
                h = PSetup.FixStepSize.Val;  // 2/21/2012 new option
                if ((x.AddDays(h) > PSetup.LastDay.Val))
                {
                    // if stepsize can overshoot, decrease
                    h = (PSetup.LastDay.Val - x).TotalDays;
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
                if (!PSetup.UseFixStepSize.Val)
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
                if (!PSetup.UseFixStepSize.Val)
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
            } while (!((MaxError < RelError) || (h < Consts.Minimum_Stepsize) || (PSetup.UseFixStepSize.Val)));
            // If (MaxError>1) and (not StepSizeWarned) then
            // Begin
            // StepSizeWarned = true;
            // MessageStr = 'Warning, the differential equation solver time-step has been forced below the minimum';
            // If ShowDebug then MessageStr = MessageStr + ' due to the "' +ProgData.ErrVar + '" state variable';
            // MessageStr = MessageStr + '.  Continuing to step forward using minimum step-size.';
            // MessageErr = true;
            // TSMessage;
            // End;
            if (PSetup.UseFixStepSize.Val)
            {
                hnext = h;
            }
            else if ((MaxError < RelError))
            {
                // Adaptive Stepsize Adjustment Source ("Delta" code) Dr. Michael Thomas Flanagan www.ee.ucl.ac.uk/~mflanaga, see terms above
                if (MaxError == 0) Delta = 4.0;
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

            if (P.IsPlant()) ((P) as TPlant).NutrLim_Step = ((P) as TPlant).NutrLimit();

            if (Location.Locale.Latitude.Val < 0.0) MidWinterJulianDate = 182;
            else MidWinterJulianDate = 1;

            if (P.IsPlantOrAnimal())
            {
                TOrganism TOR = (TOrganism)P;
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
                        FirstExposure[ToxLoop] = DateTime.MinValue;
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
                else if ((!PAnim.SpawnNow(TPresent.AddDays(-hdid)))) PAnim.Spawned = false;

                // Reset Number of Spawning Times in Midwinter
                if (TPresent.DayOfYear == MidWinterJulianDate) PAnim.SpawnTimes = 0;

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
            if ((PAnml == null)) return;

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
                        for (ToxLoop = Consts.FirstOrgTxTyp; ToxLoop <= Consts.LastOrgTxTyp; ToxLoop++)
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
            else PECalc = 100 * (0.077 * Math.Log(Inorg60) - 0.020);

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


            if (PSetup.ModelTSDays.Val) MaxStep = 1.0;
            else MaxStep = 1.0 / 24.0;  // Hourly
            h = MaxStep;

            SimulationDate = DateTime.Now;

            ModelStartTime = TStart;
            // TPreviousStep = TStart;
            TPresent = TStart;

            ChangeData();
            NormDiff(h);

            Derivs(x, 1);
            WriteResults(true, TStart); // Write Initial Conditions as the first data Point
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
                WriteResults(false, x); // Write output to Results Collection

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
                else if ((Math.Abs(hnext) > residence_time))
                {
                    h = residence_time;  // 12/5/2022 logic to prevent minimum timestep from being below residence time 
                }
                else
                {
                    h = hnext;
                }

                if ((ProgWorker != null)||(ProgHandle != null))
                {
                    int progint = (int)Math.Round(100 * ((x - TStart) / (TEnd - TStart)));
                    if (progint != lastprog)
                    {
                        if (ProgWorker != null) ProgWorker.ReportProgress(progint);
                        else ProgHandle.Report(progint);
                    };

                    lastprog = progint;

                    if (ProgWorker != null) 
                        if (ProgWorker.CancellationPending)
                        {
                            SimulationDate = DateTime.MinValue;
                            return ("User Canceled");
                        }

                    if (_ct != CancellationToken.None)
                        if (_ct.IsCancellationRequested)
                        {
                            SimulationDate = DateTime.MinValue;
                            return ("User Canceled");
                        }
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

            WriteResults(false, x); // Write final step to Results Collection
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

            if (Location.SiteType == SiteTypes.TribInput) return;

            TVolume TV = (TVolume)(GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol));
            if (TV == null) return;

            VolInitCond = TV.InitialCond;
            NewVolume = TV.State;

            // Check for Water Volume Zero and Move On
            if ((NewVolume <= VolInitCond * Location.Locale.Min_Vol_Frac.Val) ||
                ((Location.SiteType == SiteTypes.Stream) && (Location.Morph.InflowH2O < Consts.Tiny)))
            {
                WaterVolZero = true;
                Water_Was_Zero = true;
                Volume_Last_Step = GetState(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
                return;
            }

            if (Water_Was_Zero)  { Volume_Last_Step = Last_Non_Zero_Vol;  }

            // Recover from Water Volume Zero State
            Water_Was_Zero = false;
            WaterVolZero = false;

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
                    if (((TSV.NState >= Globals.Consts.FirstBiota) && (TSV.NState <= Globals.Consts.LastBiota)) ||
                         ((TSV.NState >= AllVariables.Ammonia) && (TSV.NState <= Globals.Consts.LastDetr)) ||
                         ((TSV.NState == AllVariables.H2OTox) && (!PSetup.ChemsDrivingVars.Val)))
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
        public void WriteResults(bool firstwrite, DateTime TimeIndex)
        {
            if ((SV.restimes.Count == 0) || (TimeIndex - SV.restimes[^1]).TotalDays > Consts.VSmall)   // last element is ^1
            {
                SV.restimes.Add(TimeIndex);
                foreach (TStateVariable TSV in SV) if (TSV.TrackResults)
                {
                    int rescnt = 0;
                    double res = TSV.State;
                    if (Convert_g_m2_to_mg_L(TSV.NState, TSV.SVType, TSV.Layer))
                    {
                        res = res * SegVol() / SurfaceArea();
                        //  g/m2  g/m3     m3         m2
                    }

                    if (TSV.SVResults == null) TSV.SVResults = new SavedResults();

                    if (firstwrite)
                    {
                        string ustr = TSV.StateUnit;
                        if ((TSV.SVType >= Consts.FirstOrgTxTyp) && (TSV.SVType <= Consts.LastOrgTxTyp)) ustr = "ug/L";
                        TSV.SVResults.AddColumn("", ustr);
                    }

                    TSV.SVResults.Results[rescnt].Add(res); rescnt++;

                    if ((TSV.SVType >= Consts.FirstOrgTxTyp) && (TSV.SVType <= Consts.LastOrgTxTyp))  // output PPB
                    {
                        if (firstwrite) TSV.SVResults.AddColumn("PPB", "PPB");
                        TSV.SVResults.Results[rescnt].Add(((TToxics)TSV).ppb); rescnt++;
                    }

                    if ((TSV.SVType == T_SVType.NIntrnl) || (TSV.SVType == T_SVType.PIntrnl))   //output internal nutrients in g/g
                    {
                        double TP = GetStateVal(TSV.NState, T_SVType.StV, TSV.Layer);
                        if (firstwrite) TSV.SVResults.AddColumn("Ratio", "g/gOM");
                        TSV.SVResults.Results[rescnt].Add((res / TP) * 1e-3); rescnt++;
                    } //                  (gN/gOM) =    (ug/L)/(mg/L) * (mg/ug) 


                    if ((PSetup.SaveBRates.Val) && (TSV.SaveRates) && (TSV.RateColl != null))  // save rates output
                        foreach (TRate PR in TSV.RateColl)
                        {
                            double ThisRate = PR.GetRate();
                            string ustr = "Fraction";
                            if ((PR.Name.IndexOf("_LIM") <= 0))
                            {
                                if ((PR.Name.IndexOf("GrowthRate2") > 0)) // GrowthRate2 in g/m2
                                {
                                    ThisRate = ThisRate * SegVol() / SurfaceArea();
                                    //   (g/m2) =   (g/m3)    (m3)       (m2)
                                    ustr = "g/m2";
                                }
                                else
                                {
                                    ustr = "Percent";
                                    if (TSV.State < Globals.Consts.Tiny) ThisRate = 0;// avoid divide by zero
                                    else
                                    {
                                        try { ThisRate = (ThisRate / TSV.State) * 100; }    // normal rate output
                                        catch { ThisRate = 0; } // floating point error catch 
                                    }
                                }
                            }

                            if (firstwrite) TSV.SVResults.AddColumn(PR.Name, ustr);
                            TSV.SVResults.Results[rescnt].Add(ThisRate); rescnt++;
                        }

                    if (TSV.NState == AllVariables.Volume)
                    {
                        foreach (TRate PR in TSV.RateColl)  //5/5/2021, always write volume rates
                        {
                            double ThisRate = PR.GetRate();
                            string ustr = "m3/d";
                            if (firstwrite) TSV.SVResults.AddColumn(PR.Name, ustr);
                            TSV.SVResults.Results[rescnt].Add(ThisRate); rescnt++;
                        }

                        // store other general calculations in TVolume state variable for now  

                        // chl a
                        double chla = 0;
                        for (AllVariables NS = Consts.FirstPlant; NS <= Consts.LastPlant; NS++)
                        {
                            TPlant PP = GetStatePointer(NS, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                            if (PP != null) chla += PP.State * ((0.526 / PP.PAlgalRec.Plant_to_Chla.Val) * 1000.0);
                        }
                        if (firstwrite) TSV.SVResults.AddColumn("chlorophyll a", "ug/L");
                        TSV.SVResults.Results[rescnt].Add(chla); rescnt++;

                        // retention time
                        double resTime = 99999;   // if there is no discharge, set ResTime to an arbitrary high number}
                        if (Location.Discharge > Consts.Tiny)
                        { resTime = SegVol() / Location.Discharge;  }
                        //   days     cu m              cu m/ d 
                        if (firstwrite) TSV.SVResults.AddColumn("retention time", "days");
                        TSV.SVResults.Results[rescnt].Add(resTime);   // add rescnt++ if another variable is output after this one.
                    }
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
                { if ((((P) as TPlant).PAlgalRec.PlantType.Val != "Phytoplankton")) Convert = true; }

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

            return Location.Locale.SurfArea.Val;

            //if (!LinkedMode && Stratified && (Location.Locale.UseBathymetry.Val))
            //{
            //    SiteRecord LL = Location.Locale.Val;
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
                        return (-0.0575 * Sal) + (0.001710523 * Math.Pow(Sal, 1.5)) - (0.0002154996 * AQMath.Square(Sal)); // UNESCO (1983), 4/8/2015
                    }
                    else return -1.8;  // default if salinity state variable not found {Ocean water with a typical salinity of 35 parts per thousand freezes only at -1.8 degC (28.9 deg F).}
                case SiteTypes.Stream:
                    return 0.0;  // Temperature at which ice cover occurs in moving water
                default:
                    return 3.0;  // Temperature at which ice cover occurs in fresh water
            }    //  
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
            double stepsize = (PSetup.StoreStepSize.Val);  // step size in days or hours
            if (!PSetup.StepSizeInDays.Val) stepsize = stepsize / 24;  // convert to step size in days
            int NumDays = (PSetup.LastDay.Val - PSetup.FirstDay.Val).Days; // number of days in simulation
            int numsteps = (int)(NumDays / stepsize); // number of time-steps to be written

            if (numsteps <= 0) return "Zero time-steps are written given StoreStepSize and StepSizeInDays flag.";
            if (SV.restimes == null) return "Results Times not initialized for SV List";
            if (SV.restimes.Count == 0) return "No results times saved for SV SV List";

            DateTime lastwritedate = PSetup.FirstDay.Val.AddDays(numsteps * stepsize);

            List<int> StartIndices = new List<int>();  //restime index to start linear interpolation or trapezoidal integration
            int stepindex = 1;
            for (int i = 0; i < SV.restimes.Count; i++)
            {
                DateTime DateToFind = PSetup.FirstDay.Val.AddDays(stepindex * stepsize);
                if (PSetup.AverageOutput.Val) //trapezoidal integration
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
                    if (TSV.SVResults == null) return "Results not initialized for SV " + TSV.PName;
                    if (TSV.SVResults.Results.Count() == 0) return "No results saved for SV " + TSV.PName;

                    TSV.SVoutput = new AQUATOXTSOutput();
                    TSV.SVoutput.Dataset = TSV.PName;
                    TSV.SVoutput.DataSource = "AQUATOX";
                    TSV.SVoutput.Metadata = new Dictionary<string, string>()
                    {
                        {"AQUATOX_DOT_NET_Version", "1.0.0.1"},
                        {"SimulationDate", (SimulationDate.ToString(Consts.DateFormatString))},
                        {"State_Variable", TSV.PName},
                    };

                    TSV.SVoutput.Data = new Dictionary<string, List<string>>();
                    List<string> vallist = new List<string>();
                    TSV.SVoutput.Data.Add(SV.restimes[0].ToString(Consts.DateFormatString), vallist);

                    for (int onum = 1; onum <= TSV.SVResults.Results.Count(); onum++)  // add initial conditions, output names, and units
                    {
                        vallist.Add(TSV.SVResults.Results[onum - 1][0].ToString(Consts.ValFormatString));

                        TSV.SVoutput.Metadata.Add("Name_" + onum.ToString(), TSV.SVResults.Names[onum - 1]);
                        TSV.SVoutput.Metadata.Add("Unit_" + onum.ToString(), TSV.SVResults.Units[onum - 1]);
                    }

                    for (int i = 1; i <= numsteps; i++)
                    {
                        vallist = new List<string>();
                        DateTime steptime = PSetup.FirstDay.Val.AddDays(i * stepsize);

                        for (int onum = 1; onum <= TSV.SVResults.Results.Count(); onum++)
                        {
                            if (PSetup.AverageOutput.Val) val = TrapezoidalIntegration(out errmsg, steptime.AddDays(-stepsize), steptime, TSV.SVResults.Results[onum - 1], StartIndices[i - 1]);
                            else val = InstantaneousConc(out errmsg, steptime, TSV.SVResults.Results[onum - 1], StartIndices[i - 1]);
                            vallist.Add(val.ToString(Consts.ValFormatString));
                        }

                        TSV.SVoutput.Data.Add(steptime.ToString(Consts.DateFormatString), vallist);
                        if (errmsg != "") return errmsg;
                    }
                }
                else TSV.SVoutput = null;  // if !TrackResults
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
                { RefrFrac = (RefrPercent / 100.0); }
                else { RefrFrac = 1.0 - (RefrPercent / 100.0); }

                switch (PD.InputRecord.DataType)
                {
                    case DetrDataType.CBOD:
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
                                        // PR.LoadsRec.Loadings.Hourly = PDIR.Load.Loadings.Hourly;   // All Hourly = true;  12/14/22
                                    }
                                    else PR.InitialCond = PDIR.ToxInitCond[PR.ToxInt(SVLoop)];
                                }
                            }
                        }
                    }
                }
        }

        // ---------------------------------------------------------------
        public void AssignChemRecs()
        {
            foreach (TToxics P in SV.OfType<TToxics>())
                P.ChemRec = ((TToxics)GetStatePointer(AllVariables.H2OTox, P.SVType, T_SVLayer.WaterCol)).ChemRec;
        }

        // ---------------------------------------------------------------

        public void SVsToInitConds()
        {
            CopySuspDetrData();

            TVolume TV = (TVolume)GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);

            if (Location.SiteType == SiteTypes.TribInput)
            {
                TV.Calc_Method = VolumeMethType.Dynam;
                if (TV.InitialCond < Consts.Tiny) TV.InitialCond = 1.0;  // arbitrary but must be non - zero
            }


            AssignChemRecs();

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
                PL.CalculateLoad(PSetup.FirstDay.Val);  // Set DailyLight for First Day
                double MaxDailyLight = PL.DailyLight;
                for (DateTime LightTest = PSetup.FirstDay.Val.AddDays(1); LightTest <= PSetup.FirstDay.Val.AddDays(365); LightTest = LightTest.AddDays(1))
                {   // Get Maximum daily light for one year from simulation start point
                    PL.CalculateLoad(LightTest); // Set DailyLight for "LightTest" Day
                    if (MaxDailyLight < PL.DailyLight) MaxDailyLight = PL.DailyLight;
                }
                PL.CalculateLoad(PSetup.FirstDay.Val);  // Reset DailyLight for First Day

                for (AllVariables NS = Consts.FirstPlant; NS <= Consts.LastPlant; NS++)
                {
                    TPlant PP = GetStatePointer(NS, T_SVType.StV, T_SVLayer.WaterCol) as TPlant;
                    if (PP != null)
                    {
                        if (PP.PAlgalRec.EnteredLightSat.Val >= MaxDailyLight)
                        {
                            PP.ZOpt = 0.1;  //  towards top of water column due to low light conditions
                        }
                        else
                        {
                            PP.ZOpt = Math.Log(PP.PAlgalRec.EnteredLightSat.Val / MaxDailyLight) / -Extinct(PP.IsPeriphyton(), true, true, false, 0);
                        }                                  //   (Ly/d)            (Ly/d)         (1/m)
                    }
                }
            }

            PercentEmbedded = Location.Locale.BasePercentEmbed.Val;
            LastPctEmbedCalc = PSetup.FirstDay.Val;

            SOD = -99;
            YearNum_PrevStep = 0;
        }


        public double DynamicZMean()
        {
            // Variable ZMean of segment or both segments if dynamic stratification
            if (!Location.Locale.UseBathymetry.Val)
                return Volume_Last_Step / Location.Locale.SurfArea.Val;

            if (UseConstZMean)
                return Location.Locale.ICZMean.Val;

            if (DynZMean != null) return DynZMean.ReturnTSLoad(TPresent);  // time series only

            return Location.Locale.ICZMean.Val;  // DynZMean is null
        }

        // variable zmean of seg. or entire system if stratified
        public double StaticZMean()
        {

            TVolume PVol;
            if (Location.Locale.UseBathymetry.Val)
            {
                // zmean does not vary in this case 
                return Location.Locale.ICZMean.Val;
            }
            else
            {
                PVol = GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                return PVol.InitialCond / Location.Locale.SurfArea.Val;    // Initial zmean based on const surf area over vertical profile
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
                throw new ArgumentException("Internal AQUATOX Error: GetState called for non-existant state variable: " + S.ToString(), "original");  
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
            if (MemLocRec == null)
            {
                SetMemLocRec();
            }
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

            if (MustHave60 == 1) result = 0;
            else result = LastInorg;              // Used if Count of historical data =0

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
                    if (((TPresent - LastTime).TotalDays < 60.0) && (MustHave60 == 1))  // not 60 days of data
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
            if (SS60Day < Consts.Tiny) result = 0;
            else result = 0.270 * Math.Log(SS60Day) - 0.072;  // ln
                                                              // kg/m2 day         mg/L }

            if (result < 0) result = 0;
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
            if (Q10 <= 1.0) // JSC added "=" to "<=" otherwise YT=0, division by zero
            {
                Q10 = 2.0;  // rate of change per 10 degrees
            }
            
            if (TMaxAdapt <= TOptAdapt)
            {
                TMaxAdapt = TOptAdapt + Consts.VSmall;
            }
            double WT = Math.Log(Q10) * (TMaxAdapt - TOptAdapt);
            double YT = Math.Log(Q10) * (TMaxAdapt - TOptAdapt + 2.0);
            // NOT IN CEM MODELS
            double XT = (AQMath.Square(WT) * AQMath.Square(1.0 + Math.Sqrt(1.0 + 40.0 / YT))) / 400.0;
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

            return 1.0 + (1E-3 * (28.14 - (0.0735 * Temp) - (0.00469 * AQMath.Square(Temp)) + (0.802 - (0.002 * Temp)) * (Salt - 35.0)));
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
                if (Avg_Disch <= 0) Avg_Disch = Location.Discharge_Using_QBase() / 86400.0;
                //                { m3 / s}             { m3 / d}               {s / d}

                width = Location.Locale.SurfArea.Val / (Location.Locale.SiteLength.Val * 1000.0);
                //{ m}                  { sq.m}                { km}            { m / km}

                double slope = Math.Max(Location.Locale.Channel_Slope.Val, 0.00001);
                channel_depth = Math.Pow(Avg_Disch * Location.ManningCoeff() / (Math.Sqrt(slope) * width), 0.6);

                xsecarea = width * channel_depth;
                // m2       // m         // m
            }
            else xsecarea = vol / (Location.Locale.SiteLength.Val * 1000);
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
                result = Location.Locale.SurfArea.Val * Diagenesis_Params.H1.Val;
            else
                result = Location.Locale.SurfArea.Val * Diagenesis_Params.H2.Val;
            // m3                // m2                     // m


            // double EpiFrac;
            //if (Stratified)
            //{
            //    MorphRecord RR = Location.Morph;
            //    EpiFrac = Location.AreaFrac(Location.MeanThick[VerticalSegments.Epilimnion], Location.Locale.ZMax.Val);
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
                    AllVariables.PON_G1 => Location.Remin.N2OrgLab.Val,    // Was PA.PAnimalData.N2Org, 6/6/2008, defecation has same nutrients as labile detritus
                    AllVariables.POP_G1 => Location.Remin.P2OrgLab.Val,    // Was PA.PAnimalData.P2Org, 6/6/2008, defecation has same nutrients as labile detritus
                    _ => 1.0 / Consts.Detr_OM_2_OC,                      // Winberg et al. 1971, relevant to animals, non-macrophyte plants, bacteria
                };

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
                        // taxonomic type diatoms
                        if (PP.NState >= Consts.FirstDiatom && PP.NState <= Consts.LastDiatom)
                            NFrac = Diagenesis_Params.Si_Diatom.Val;
                        else NFrac = 0;
                        break;
                    case AllVariables.PON_G1:
                    case AllVariables.PON_G2:
                    case AllVariables.PON_G3:
                        NFrac = PP.N_2_Org();
                        break;
                    case AllVariables.POP_G1:
                    case AllVariables.POP_G2:
                    case AllVariables.POP_G3:
                        NFrac = PP.P_2_Org();
                        break;
                    default: // POCG1..POCG3:
                        NFrac = 1.0 / Consts.Detr_OM_2_OC;
                        break;
                        // Winberg et al. 1971, relevant to animals, non-macrophyte plants, bacteria
                }
                // in which case all deposition goes to the periphyton directly
                if (!PP.IsLinkedPhyto())
                {
                    if ((Typ == T_SVType.StV))
                    {
                        // g/m3                       // g/m3
                        Sed = Sed + PP.Sedimentation() * Frac * NFrac;
                    }
                    else
                    {
                        Sed = Sed + PP.Sedimentation() * Frac * GetPPB(PP.NState, Typ, PP.Layer) * 1e-3;
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
                        (AllVariables.PON_G1) => RR.N2OrgLab.Val,
                        (AllVariables.PON_G2) => RR.N2OrgRefr.Val,
                        (AllVariables.POP_G1) => RR.P2OrgLab.Val,
                        (AllVariables.POP_G2) => RR.P2OrgRefr.Val,
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
                if ((Typ == T_SVType.StV))
                    Sed = Sed + ((P) as TSuspendedDetr).Sedimentation() * NFrac;
                else
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
            return (Sed + Def) * MR.SegVolum / DiagenesisVol(2) * Diagenesis_Params.H2.Val;
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
                SOD_test = Jc + (Jn * 4.57);
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
            //        LL = Location.Locale.Val;
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
                    ErrorOK = Math.Abs((SOD - SOD_test) / SOD) <= PSetup.RelativeError.Val;
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
            TSalinity PSalt = (TSalinity)GetStatePointer(AllVariables.Salinity, T_SVType.StV, T_SVLayer.WaterCol);
            if (PSalt == null) Salt = -1.0;
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
                SedN_OM = GetState(AllVariables.PON_G2, T_SVType.StV, T_SVLayer.SedLayer2) / RR.N2OrgRefr.Val;
                SedP_OM = GetState(AllVariables.POP_G2, T_SVType.StV, T_SVLayer.SedLayer2) / RR.P2OrgRefr.Val;
                // g OM/m3 s               // g P or N / m3                                // g P or N / g OM
                SedC_OM = GetState(AllVariables.POC_G2, T_SVType.StV, T_SVLayer.SedLayer2) * Consts.Detr_OM_2_OC;
                // g OM/m3 s                 // g OC/m3                                   // g OM / g OC
            }
            else
            {
                // SedmLabDetr
                SedN_OM = GetState(AllVariables.PON_G1, T_SVType.StV, T_SVLayer.SedLayer2) / RR.N2OrgLab.Val;
                SedP_OM = GetState(AllVariables.POP_G1, T_SVType.StV, T_SVLayer.SedLayer2) / RR.P2OrgLab.Val;
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

                        IncludePlant = (PR.PlantType.Val == "Phytoplankton");  // All Phytoplankton gets included
                        IncludePlant = IncludePlant || (((PR.PlantType.Val == "Periphyton") || (PR.PlantType.Val == "Bryophytes")) && Incl_Periphyton); // Periphyton & Bryophytes get included if requested
                        IncludePlant = IncludePlant || ((PR.PlantType.Val == "Macrophytes") && (Pphyto.MacroType == TMacroType.Benthic) && Incl_BenthicMacro); // Benthic Macrophytes get included if requested
                        IncludePlant = IncludePlant || ((PR.PlantType.Val == "Macrophytes") && (Pphyto.MacroType != TMacroType.Benthic) && Incl_FloatingMacro); // Floating Macrophytes get included if requested

                        if (IncludePlant)
                        {
                            // 3/9/2012 move from "blue-green" to surface floater designation
                            // 3-8-06 account for more intense self shading in upper layer of water column due to concentration of cyanobacteria there
                            if (IsSurfaceFloater && (Pphyto.PAlgalRec.SurfaceFloating.Val))
                            {
                                // 1/m               // 1/m             // 1/(m g/m3)               // g/m3 volume                          // layer, m                                 // thick of algae, m
                                PhytoExtinction = PhytoExtinction + PR.ECoeffPhyto.Val * GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol) * Location.MeanThick / Pphyto.DepthBottom();
                            }
                            else
                            {
                                PhytoExtinction = PhytoExtinction + PR.ECoeffPhyto.Val * GetState(Phyto, T_SVType.StV, T_SVLayer.WaterCol);
                            }
                            // 1/m                 // 1/m              // 1/(m g/m3)                // g/m3                    }
                        }
                    }
                }

                TempExt = PhytoExtinction;
            }

            if ((OrgFlag == 0))
            {
                TempExt = PhytoExtinction + Location.Locale.ECoeffWater.Val;
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
                        if ((DetrLoop == AllVariables.DissRefrDetr) || (DetrLoop == AllVariables.DissLabDetr))
                            TempExt = TempExt + Location.Locale.ECoeffDOM.Val * DetrState;
                        else
                            TempExt = TempExt + Location.Locale.ECoeffPOM.Val * DetrState;
                    }
                }
            }
            // ---------------------------------------------------------------------------
            // Inorganic Suspended Sediment EXTINCTION
            if ((OrgFlag == 0) || (OrgFlag == 2))
            {
                TempExt = TempExt + InorgSedConc() * Location.Locale.ECoeffSed.Val;
            }
            // ---------------------------------------------------------------------------
            if (TempExt < Consts.Tiny) TempExt = Consts.Tiny;
            if (TempExt > 25.0) TempExt = 25.0;

            return TempExt;
        }  // end Extinction


        public double Photoperiod_Radians(double X)
        {
            return Math.PI * X / 180.0;
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
            PL = (TLight)GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (PL.CalculatePhotoperiod)
            {
                if (Location.Locale.Latitude.Val < 0.0) Sign = -1.0;
                else Sign = 1.0;
                X = TPresent.DayOfYear;
                A = 0.1414 * Location.Locale.Latitude.Val - Sign * 2.413;
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
                ZEup = 4.605 / Location.Locale.ECoeffWater.Val;
            }
            else
            {
                ZEup = 4.605 / RExt;
            }
            if (Location.Locale.UseBathymetry.Val)
            {
                if ((ZEup > Location.Locale.ZMax.Val))
                {
                    ZEup = Location.Locale.ZMax.Val;
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

            Location.ChangeData(Location.Locale.ICZMean.Val);

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
                        if (!((PP.nState == AllVariables.SedmRefrDetr) || (PP.nState == AllVariables.SedmLabDetr))
                              && (GetStatePointer(PP.nState, T_SVType.StV, T_SVLayer.WaterCol) == null))
                            PP.Preference = 0;

                        if (!((PP.nState == AllVariables.SedmRefrDetr) || (PP.nState == AllVariables.SedmLabDetr)) // diagenesis model included
                              && (GetStatePointer(AllVariables.POC_G1, T_SVType.StV, T_SVLayer.WaterCol) != null))
                        {
                            { if (Step != -1) PreyState = Diagenesis_Detr(PP.nState); else PreyState = 0; }
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


        public int iTrophInt(AllVariables ns)
        {
            return ((int)ns - (int)AllVariables.Cohesives);
        }

        public DataTable[] TrophInt_to_Table()
        {
            DataTable[] tables =  { new DataTable("Trophic_Interaction_Table"), new DataTable("Egestion_Table"), new DataTable("Trophint_Comments") };
            AllVariables nsloop;
            AllVariables[] Predators = new AllVariables[50];
            SetMemLocRec();

            int PredCount = 0;
            for (int tindx = 0; tindx < 3; tindx++)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = "Prey Items";
                column.DataType = System.Type.GetType("System.String");
                tables[tindx].Columns.Add(column);

                PredCount = 0;
                for (nsloop = Consts.FirstAnimal; nsloop <= Consts.LastAnimal; nsloop++)
                {
                    TAnimal TA = GetStatePointer(nsloop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                    if (TA != null)
                    {
                        PredCount++;
                        Predators[PredCount - 1] = nsloop;
                        column = new DataColumn();
                        column.ColumnName = TA.PName;
                        if (tindx < 2)
                        {
                            column.DataType = System.Type.GetType("System.Double");
                            tables[tindx].Columns.Add(column);
                        }
                        else
                        {
                            column.DataType = System.Type.GetType("System.String");
                            tables[tindx].Columns.Add(column);
                        }
                    }
                }
            }

            for (nsloop = AllVariables.Cohesives; nsloop <= Consts.LastBiota; nsloop++)
            {
                TStateVariable TSV = GetStatePointer(nsloop, T_SVType.StV, T_SVLayer.WaterCol);
                if (TSV != null)
                {
                    for (int tindx = 0; tindx<3; tindx++)
                    {
                        DataRow row = tables[tindx].NewRow();
                        row[0] = TSV.PName;

                        for (int i = 0; i < PredCount; i++)
                        {
                            TAnimal TA = GetStatePointer(Predators[i], T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;

                            if (tindx == 0)
                            { if (TA.PTrophInt[iTrophInt(nsloop)].Pref > 1e-5) row[i + 1] = Math.Round(TA.PTrophInt[iTrophInt(nsloop)].Pref, 3); }
                            if (tindx == 1)
                            { if (TA.PTrophInt[iTrophInt(nsloop)].Pref > 1e-5) row[i + 1] = Math.Round(TA.PTrophInt[iTrophInt(nsloop)].ECoeff, 3); }
                            if (tindx == 2)
                            { row[i + 1] = TA.PTrophInt[iTrophInt(nsloop)].XInteraction; }
                        }

                        tables[tindx].Rows.Add(row);
                    }
                }
            }

            return tables;
        }

        public void Normalize_Trophint_Table(ref DataTable table)
        {
            SetMemLocRec();

            int PredIndex = 0;
            for (AllVariables predloop = Consts.FirstAnimal; predloop <= Consts.LastAnimal; predloop++)
            {
                TAnimal TA = GetStatePointer(predloop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (TA != null)
                {
                    PredIndex++;
                    int PreyIndex = -1;
                    double SumPref = 0;
                    for (AllVariables preyloop = AllVariables.Cohesives; preyloop <= Consts.LastBiota; preyloop++)
                    {
                        TStateVariable TSV = GetStatePointer(preyloop, T_SVType.StV, T_SVLayer.WaterCol);
                        if (TSV != null)
                        {
                            PreyIndex++;
                            object df = table.Rows[PreyIndex][PredIndex];
                            double pref;
                            if (df == DBNull.Value) pref = 0;
                            else pref = (double)df;
                            SumPref += pref;
                        }
                    }

                    PreyIndex = -1;
                    for (AllVariables preyloop = AllVariables.Cohesives; preyloop <= Consts.LastBiota; preyloop++)
                    {
                        TStateVariable TSV = GetStatePointer(preyloop, T_SVType.StV, T_SVLayer.WaterCol);
                        if (TSV != null)
                        {
                            PreyIndex++;
                            object df = table.Rows[PreyIndex][PredIndex];
                            double pref;
                            if (df == DBNull.Value) pref = 0;
                            else pref = (double)df;

                           if (pref > 1e-5) table.Rows[PreyIndex][PredIndex] = Math.Round(pref / SumPref,3);

                        }
                    }
                }
            }
        }

        private double convcell (object obj)
        {
            if (obj == DBNull.Value) return 0.0;
            else return (double)obj;
        }

        private string convstr(object obj)
        {
            if (obj == DBNull.Value) return "";
            else return (string)obj;
        }


        public bool Tables_to_Trophint(DataTable[] tables)
        {
            SetMemLocRec();

            int PredIndex = 0;
            for (AllVariables predloop = Consts.FirstAnimal; predloop <= Consts.LastAnimal; predloop++)
            {
                TAnimal TA = GetStatePointer(predloop, T_SVType.StV, T_SVLayer.WaterCol) as TAnimal;
                if (TA != null)
                {
                    PredIndex++;
                    int PreyIndex = -1;
                    for (AllVariables preyloop = AllVariables.Cohesives; preyloop <= Consts.LastBiota; preyloop++)
                    {
                        TStateVariable TSV = GetStatePointer(preyloop, T_SVType.StV, T_SVLayer.WaterCol);
                        if (TSV != null)
                        {
                            PreyIndex++;
                            object df = tables[0].Rows[PreyIndex][PredIndex];
                            TA.PTrophInt[iTrophInt(preyloop)].Pref = convcell(df);

                            object df2 = tables[1].Rows[PreyIndex][PredIndex];
                            TA.PTrophInt[iTrophInt(preyloop)].ECoeff = convcell(df2);

                            object df3 = tables[2].Rows[PreyIndex][PredIndex];
                            TA.PTrophInt[iTrophInt(preyloop)].XInteraction = convstr(df3);
                        }
                    }
                }
            }
            return true;
        }

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

            if (Diagenesis_Included() && ((S == AllVariables.SedmRefrDetr) || (S == AllVariables.SedmLabDetr)))
            {
                if ((S == AllVariables.SedmRefrDetr))
                    return GetPPB(AllVariables.POC_G2, T, T_SVLayer.SedLayer2) / Consts.Detr_OM_2_OC;
                else
                    return GetPPB(AllVariables.POC_G1, T, T_SVLayer.SedLayer2) / Consts.Detr_OM_2_OC;
                // ug/kg OM                // ug /kg OC                                   // OM/OC
            }

            PT = GetStatePointer(S, T, L) as TToxics;
            if (PT == null) return 0;
            if (PT.IsAGGR) return PT.ppb;

            ToxState = PT.State;
            if (S == AllVariables.H2OTox) return ToxState;      // Toxicant Dissolved in Water already in ug/L

            CarrierState = GetState(S, T_SVType.StV, L);
            if ((CarrierState < Consts.Tiny) || (ToxState < Consts.Tiny)) return 0;

            PPBResult = ToxState / CarrierState * 1e6;             // Toxicant in Carrier in water
            // ug/kg       ug/L         mg/L        mg/kg

            if (S >= AllVariables.POC_G1 && S <= AllVariables.POC_G3)
            {
                if ((CarrierState < Consts.Tiny)) PPBResult = 0;
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
        public string ResultsToCSV()
        {

            TStateVariable TSV1 = null;
            //bool SuppressText = false;

            int sercnt = 0;
            string outtxt = "";

            foreach (TStateVariable TSV in SV) if (TSV.SVoutput != null)
                {
                    TSV1 = TSV; // identify TSV1 with an output that is not null
                    int cnt = 0;
                    if (sercnt == 0) outtxt = "Date, ";

                    List<string> vallist = TSV.SVoutput.Data.Values.ElementAt(0);
                    for (int col = 1; col <= vallist.Count(); col++)
                    {
                        string sertxt = TSV.SVoutput.Metadata["State_Variable"] + " " +
                                TSV.SVoutput.Metadata["Name_" + col.ToString()] +
                                " (" + TSV.SVoutput.Metadata["Unit_" + col.ToString()] + ")";

                        if (col == 1) outtxt = outtxt + sertxt.Replace(",", "") + ", ";  // suppress commas in name for CSV output

                        sercnt++;

                        //SuppressText = (TSV.SVoutput.Data.Keys.Count > 5000);
                        for (int i = 0; i < TSV.SVoutput.Data.Keys.Count; i++)
                        {
                            ITimeSeriesOutput ito = TSV.SVoutput;
                            string datestr = ito.Data.Keys.ElementAt(i).ToString();
                            Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[col - 1]);
                            cnt++;
                        }
                    }
                }

            outtxt = outtxt + Environment.NewLine;

            // if (!SuppressText)
            {
                for (int i = 0; i < TSV1.SVoutput.Data.Keys.Count; i++)
                {
                    bool writedate = true;
                    foreach (TStateVariable TSV in SV) if (TSV.SVoutput != null)
                        {
                            ITimeSeriesOutput ito = TSV.SVoutput;
                            if (writedate)
                            {
                                string datestr = ito.Data.Keys.ElementAt(i).ToString();
                                outtxt = outtxt + datestr + ", ";
                                writedate = false;
                            }
                            Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                            outtxt = outtxt + Val.ToString() + ", ";
                        }
                    outtxt = outtxt + Environment.NewLine;
                }
            }

            return outtxt;
        }

        public void DefaultGraphs()
        {

            TGraphSetup AddVarsToGraph(string name, AllVariables firstvar, AllVariables lastvar)
            {
                TGraphSetup result = new TGraphSetup(name);
                for (AllVariables var = firstvar; var <= lastvar; var++)
                {
                    TStateVariable TSV = GetStatePointer(var, T_SVType.StV, T_SVLayer.WaterCol);
                    if (TSV != null) 
                    {
                        string sertxt = TSV.OutputText(1);
                        result.YItems.Add(new SeriesID() { nm = sertxt, lyr = TSV.Layer, ns = TSV.NState, typ = TSV.SVType, indx = 1});

                        if (var==AllVariables.Volume)
                        {
                            int inflowcolumn = 2;
                            if (PSetup.SaveBRates.Val) inflowcolumn = 5;  // after inflow, outflow, and evap in units of "percent"
                            result.YItems.Add(new SeriesID() { nm = TSV.OutputText(inflowcolumn), lyr = TSV.Layer, ns = TSV.NState, typ = TSV.SVType, indx = inflowcolumn }); //add inflow
                        }

                    }
                }
                return result;
            };

            if (Graphs.GList.Count > 0) return;

            //nutrients
            TGraphSetup ngraph = AddVarsToGraph("Nutrients",AllVariables.Ammonia, AllVariables.Phosphate);
            if (ngraph.YItems.Count > 0) Graphs.GList.Add(ngraph);

            //watervol
            TGraphSetup vgraph = AddVarsToGraph("Water Volume", AllVariables.Volume, AllVariables.Volume);
            if (vgraph.YItems.Count > 0) Graphs.GList.Add(vgraph);

            //OM
            TGraphSetup OMgraph = AddVarsToGraph("Organic Matter", AllVariables.DissRefrDetr, AllVariables.SuspLabDetr);
            if (OMgraph.YItems.Count > 0) Graphs.GList.Add(OMgraph);

            //plants
            TGraphSetup Pgraph = AddVarsToGraph("Plants", Consts.FirstPlant, Consts.LastPlant);
            if (Pgraph.YItems.Count > 0) Graphs.GList.Add(Pgraph);

            //animals
            TGraphSetup Agraph = AddVarsToGraph("Animals", Consts.FirstAnimal, Consts.LastAnimal);
            if (Agraph.YItems.Count > 0) Graphs.GList.Add(Agraph);

        }

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


        public override string IgnoreLabel()
        {
            return "Compute pH from CO2 / alkalinity";  // pH specific label and flag meaning
        }


        public TpHObj(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
            Alkalinity = 1000;   // ( default ueq CaCO3/L)

        }
        public double CalculateLoad_asinh(double x)
        {
            return Math.Log(Math.Sqrt(AQMath.Square(x) + 1) + x);
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

            pH2CO3 = Math.Pow(10.0, -(6.57 - 0.0118 * T + 0.00012 * (AQMath.Square(T))) * 0.92);
            Alpha = pH2CO3 * CCO2 + pkw;
            A = -Math.Log10(Math.Sqrt(Alpha));
            B = 1.0 / Math.Log(10.0);
            C = 2 * (Math.Sqrt(Alpha));
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

        public override string IgnoreLabel()
        {
            return "Estimate Temperature from Annual Mean and Range (in site data)";  // temperature specific label and flag meaning
        }

        public override void SetVarFromRadioButton(int iButton)
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
                if (Location.Locale.Latitude.Val < 0.0) AdjustedJulian = AdjustedJulian + 182.0;
                MeanTemp = Location.Locale.TempMean.Val;  // AQTSeg.VSeg
                TempRange = Location.Locale.TempRange.Val; // AQTSeg.VSeg
                                                           //if (AQTSeg.LinkedMode)
                                                           //{
                                                           //    // MeanTemp and Range are stored in "Epilimnion" for each linked segment regardless of stratification
                                                           //    MeanTemp = Location.Locale.TempMean.Val[VerticalSegments.Epilimnion];
                                                           //    TempRange = Location.Locale.TempRange.Val[VerticalSegments.Epilimnion];
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

        public override string IgnoreLabel()
        {
            return "Estimate Light from Annual Mean and Range (in site data)";  // light specific label and flag meaning
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
                if (Location.Locale.Latitude.Val < 0.0) adjustedjulian = adjustedjulian + 182;

                solar = Location.Locale.LightMean.Val + Location.Locale.LightRange.Val / 2.0 * Math.Sin(0.0174533 * adjustedjulian - 1.76);

                light = solar;
                if (light < 0.0) light = 0.0;

                light = light * LoadsRec.Loadings.MultLdg;  // allow perturbation JSC 1-23-03
            }

            // ACCOUNT FOR ICE COVER
            {
                if (AQTSeg.GetState(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol) < AQTSeg.Ice_Cover_Temp())
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
            if ((!LoadsRec.Loadings.NoUserLoad) && (!AQTSeg.PSetup.ModelTSDays.Val))        // && (LoadsRec.Loadings.Hourly)) 12/14/22
            { HourlyLight = light; }

            // 12-5-2016 correction to properly model hourly light time-series inputs
            if ((!AQTSeg.PSetup.ModelTSDays.Val) && (LoadsRec.Loadings.NoUserLoad || (LoadsRec.Loadings.UseConstant)))  // 12/14/22  removed || (!LoadsRec.Loadings.Hourly)
            {
                // distribute daily loading over daylight hours
                pp = AQTSeg.Photoperiod();
                fracdaypassed = TimeIndex.TimeOfDay.TotalDays;
                lighttime = fracdaypassed - ((1.0 - pp) / 2.0);
                if ((fracdaypassed < (1.0 - pp) / 2.0) || (fracdaypassed > 1.0 - ((1.0 - pp) / 2.0))) State = 0;
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

        public override string IgnoreLabel()
        {
            return "Use Default Time Series based on Mean Windspeed";  // light specific label and flag meaning
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
                    AddVar = Coeffs[N - 1] * Math.Cos(Freq[N - 1] * DateVal); // COS Coefficients in odd array registers
                else AddVar = Coeffs[N - 1] * Math.Sin(Freq[N - 1] * DateVal); // SIN Coefficients in even array registers

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

        public TSandSiltClay(AllVariables Ns, T_SVType SVT, T_SVLayer L, string aName, AQUATOXSegment P, double IC) : base(Ns, SVT, L, aName, P, IC)
        {
        }

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

        public override List<string> GUIRadioButtons()
        {
            return new List<string>(new string[] { "TSS are Solids Including Organics", "TSS are Inorganics Only" });
        }

        public override int RadioButtonState()
        {
            if (TSS_Solids) return 1;
            return 0;
        }

        public override void SetVarFromRadioButton(int iButton)
        {
            TSS_Solids = (iButton == 0);
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
            KnownTypes = new List<Type> { typeof(TStateVariable), typeof(AQUATOXSegment), typeof(TAQTSite), typeof(AQTSim), typeof(Dictionary<string, AQUATOXSegment>),
                                          typeof(SiteRecord), typeof(ReminRecord), typeof(Setup_Record), typeof(AQUATOX.Volume.TVolume), typeof(LoadingsRecord), typeof(TLoadings),
                                          typeof(SortedList<DateTime, double>), typeof(AQUATOXTSOutput), typeof(TRemineralize), typeof(TNH4Obj), typeof(TNO3Obj), typeof(TPO4Obj),
                                          typeof(TSalinity), typeof(TpHObj), typeof(TTemperature), typeof(TCO2Obj), typeof(TO2Obj), typeof(DetritalInputRecordType),
                                          typeof(TDissRefrDetr), typeof(TDissLabDetr), typeof(TSuspRefrDetr), typeof(TSuspLabDetr), typeof(TSedRefrDetr), typeof(TSedLabileDetr),
                                          typeof(TimeSeriesInput), typeof(TimeSeriesOutput), typeof(TNH4_Sediment), typeof(TNO3_Sediment), typeof(TPO4_Sediment),
                                          typeof(TPOC_Sediment), typeof(TPON_Sediment), typeof(TPOP_Sediment), typeof(TMethane), typeof(TSulfide_Sediment),
                                          typeof(TSilica_Sediment), typeof(TCOD), typeof(TParameter), typeof(Diagenesis_Rec), typeof(TToxics), typeof(TLight),
                                          typeof(ChemicalRecord), typeof(TWindLoading), typeof(TPlant), typeof(PlantRecord), typeof(TMacrophyte), typeof(TAnimal),typeof(AnimalRecord),
                                          typeof(TSandSiltClay), typeof(InteractionFields), typeof(TAnimalTox), typeof(TParticleTox), typeof(TBioTransObject),
                                          typeof(TAlgaeTox), typeof(TPlantToxRecord), typeof(TAnimalToxRecord), typeof(T_N_Internal_Plant), typeof(DateTimeSpan), typeof(TimeSeriesGeometry),
                                          typeof(TBoolParam), typeof(TDateParam), typeof (TDropDownParam), typeof (TStringParam),typeof(TGraphSetup) , typeof(TGraphs), typeof(SeriesID),
                                          typeof(PointCoordinate)};   //, typeof(Dictionary<string,TimeSeriesOutput>)
        }
    }

}

