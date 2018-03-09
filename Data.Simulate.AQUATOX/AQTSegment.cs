using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQSite;
using AQUATOX.Loadings;


using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Data;

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
                string errmsg = AQTSeg.Verify_Runnable();
                if (errmsg != "") return errmsg;

                AQTSeg.ClearResults();
                AQTSeg.SVsToInitConds();
                AQTSeg.Integrate(AQTSeg.PSetup.FirstDay, AQTSeg.PSetup.LastDay, 0.1, 1e-5, 1);
                return "";
            }
            //catch (Exception e)
            //{
            //    return e.Message;
            //}

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

        // ITimeSeries AQTSim.ReturnResults(SV-Type)
        // ITimeSeries AQTSim.ReturnResults(SV-Type) (StartDate, EndDate)

    }

    public class AQUATOXITSOutput : ITimeSeriesOutput
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

        public AQUATOXITSOutput ()
        {
            Data = new Dictionary<string, List<string>>();
            Dataset = "";
            DataSource = "";
            Metadata = new Dictionary<string, string>();
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
        [JsonIgnore] double yorig;                  // used to restore state to beginning of time step
        [JsonIgnore] double yout;                    // use in Integration
        [JsonIgnore] public double[] StepRes = new double[7];  // Holds Step Results
        [JsonIgnore] public double yerror = 0;          // holds error term from RKCK
        [JsonIgnore] public double yscale = 0;          // use in Integration
        [JsonIgnore] public List<double> Results = new List<double>(); // holds numerical results, internal, not evenly spaced if variable stepsize
        public AQUATOXITSOutput output;  // public and evenly-spaced results following integration / interpolation

        [JsonIgnore] public AQUATOXSegment AQTSeg = null;   // Pointer to Collection of State Variables of which I am a member
        public LoadingsRecord LoadsRec = null;   // Holds all of the Loadings Information for this State Variable  
        [JsonIgnore] public double Loading = 0;         // Loading of State Variable This time step
//        bool RequiresData = false;
//        bool HasData = false;        // If RequiresUnderlyingData and Not HasData then Model cannot be run
        public string StateUnit;
        public string LoadingUnit;       // Units 

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

        public virtual void SetToInitCond()
        {
            int j;
            State = InitialCond;
            // init risk conc, internal nutrients, toxicants

            yhold= 0;
            yorig= 0;

            for (j = 1; j <= 6; j++)
            {
                StepRes[j] = 0;
            }

        }

        public void TakeDerivative(int Step)
        {
            Derivative(ref StepRes[Step]);
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
        yorig = 0;
        for (j = 1; j <= 6; j++)
        {
            StepRes[j] = 0;
        }
    }

        public virtual void CalculateLoad(DateTime TimeIndex)
        {
            Loading = 0;    
        }


    }  // end TStateVariable

        
    public class TExpIncrease : TStateVariable  //temporary test class for differentiation test
    {
        public override void Derivative(ref double DB)
        {
            DB = State * 0.04;
        }
    }

    public class T10PctDecrease : TStateVariable  //temporary test class for differentiation test

    {
        public override void Derivative(ref double DB)

        {
            DB = -State * 0.1;
        } 

    }

    public class TStates : List<TStateVariable>
        {
           [JsonIgnore] public List<DateTime> restimes = new List<DateTime>();

        public void WriteResults(DateTime TimeIndex)
            {
                if ((restimes.Count == 0) || (TimeIndex - restimes[restimes.Count - 1]).TotalDays > Consts.VSmall)
                {
                    restimes.Add(TimeIndex);
                    foreach (TStateVariable TSV in this)
                        TSV.Results.Add(TSV.State);
                }
            }
        }



    public class AQUATOXSegment 
    {
        public TAQTSite Location = new TAQTSite();       // Site data structure

        public TStates SV = new TStates();    // State Variables
        public DateTime TPresent;
        public Setup_Record PSetup;

        public bool UseConstEvap = true;
        public TLoadings DynEvap = null;
        public TLoadings DynZMean;

        [JsonIgnore] public int DerivStep;    // Current Derivative Step 1 to 6, Don't save in json  

        [JsonIgnore] public DateTime SimulationDate;  // time integration started
        [JsonIgnore] public DateTime VolumeUpdated;  // 
        [JsonIgnore] public double Volume_Last_Step;    //Volume in the previous step, used for calculating dilute/conc,  if stratified, volume of whole system(nosave)}  


        public AQUATOXSegment()
        {

        }

        public string Verify_Runnable()
        {
            if (SV.Count < 1) return "No State Variables Are Included in this Simulation.";
            if (Equals(PSetup, default(Setup_Record))) return "PSetup data structure must be initiailized.";
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

        public void RunTest()
        {

            TExpIncrease SVI = new TExpIncrease();
            SVI.PName = "4% increase";
            SVI.LoadsRec = new LoadingsRecord();
            T10PctDecrease SVD = new T10PctDecrease();
            SVD.PName = "10% decrease";
            TStateVariable SVF = new TStateVariable();
            SVF.PName = "Flat";
//            TVolume SVTV = new TVolume(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol, "Water Volume", this, 1000);

            SV.Add(SVI);
            SV.Add(SVD);
            SV.Add(SVF);
            //          SV.Add(SVTV);

            SVI.InitialCond = 10;
            SVI.State = 10;
            SVD.State = 10;
            SVD.InitialCond = 10;
            SVF.State = 10;
            SVF.InitialCond = 10;
            //            SVTV.State = 15;

            //            SVTV.LoadsRec = new LoadingsRecord();
            //            SVTV.LoadsRec.Loadings = new TLoadings();
            //            SVTV.LoadsRec.Loadings.list.Add(Convert.ToDateTime("1993-01-04"), 40);
            //            SVTV.LoadsRec.Loadings.list.Add(Convert.ToDateTime("1993-01-01"), 60);

            //TLoadings altLoad1 = new TLoadings();
            //altLoad1.list.Add(Convert.ToDateTime("1993-01-04"), 30);
            //altLoad1.list.Add(Convert.ToDateTime("1993-01-01"), 45);
            //SVTV.LoadsRec.Alt_Loadings[0] = altLoad1;

            SetDefaultSetup();

            Integrate(PSetup.FirstDay, PSetup.LastDay, 0.1, 1e-5, 1);
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

            foreach (TStateVariable TSV in SV)
            {
                TSV.TakeDerivative(Step);
            }
        }


        public void TryRKStep_CheckZeroState(TStateVariable p)
        {
            if (p.State < Consts.Tiny)
            {
                p.State = 0.0;
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
            {
                TSV.State = TSV.yhold;
            }
        }

        public void TryRKStep_SaveStates_to_Holder()
        {
            foreach (TStateVariable TSV in SV)
            {
                TSV.yhold = TSV.State;
            }
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
            double[] A = { 0, 0, 0.2, 0.3, 0.6, 1, 0.875 };
            double[] B5 = { 0, 0.09788359788, 0, 0.40257648953, 0.21043771044, 0, 0.28910220215 };
            double[] B4 = { 0, 0.10217737269, 0, 0.38390790344, 0.24459273727, 0.01932198661, 0.25 };  // Butcher Tableau
            double[,] Tableau = { { 0.2, 0, 0, 0, 0 }, { 3 / 40, 9 / 40, 0, 0, 0 }, { 3 / 10, -9 / 10, 6 / 5, 0, 0 }, { -11 / 54, 5 / 2, -70 / 27, 35 / 27, 0 }, { 1631 / 55296, 175 / 512, 575 / 13824, 44275 / 110592, 253 / 4096 } };
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
                        {
                            TSV.State = TSV.State + h * Tableau[Steps - 1, SubStep - 1] * TSV.StepRes[SubStep];
                        }
                    }
                }
            }
            // 6 steps of integration
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
        public void AdaptiveStep(ref DateTime x, double hstart, double RelError, ref double h_taken, ref double hnext)
        {
            const double SAFETY = 0.9;
            double h;
            double Delta;
            TStateVariable ErrVar;
            double MaxError;
            double MaxStep;
            //            string ErrText;

            MaxStep = 1.0;

            //            if (SV.PModelTimeStep == Global.TimeStepType.TSDaily)
            //            {
            //                MaxStep = 1.0;
            //            }
            //            else
            //            {
            //                MaxStep = 1 / 24; // Hourly
            //            }

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
                                    ErrVar = TSV;                                  // save error variable for later use in screen display
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
                        if ((Delta < 0.1))
                        {
                            h = h * 0.1;
                        }
                        else
                        {
                            h = h * Delta;
                        }
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
                if (MaxError == 0)
                {
                    Delta = 4.0;
                }
                else
                {
                    Delta = SAFETY * Math.Pow(MaxError / RelError, -0.2);
                }
                if ((Delta > 4.0))
                {
                    Delta = 4.0;
                }
                if ((Delta < 1.0))
                {
                    Delta = 1.0;
                }
                hnext = h * Delta;
            }
            h_taken = h;
            if (hnext > MaxStep)
            {
                hnext = MaxStep;
            }
            if (h > MaxStep)
            {
                h = MaxStep;
            }
            x = x.AddDays(h);

            foreach (TStateVariable TSV in SV)
            {
                // reasonable error, so copy results of differentiation saved in YHolders
                TSV.State = TSV.yhold;
            }
            //            Perform_Dilute_or_Concentrate(x);
        }


        public void Integrate_CheckZeroState(TStateVariable p)
        {
            if (p.State < 0)
            {
                p.State = 0.0;
            }
        }

        public void Integrate_CheckZeroStateAllSVs()
        {
            foreach (TStateVariable TSV in SV)
            {
                Integrate_CheckZeroState(TSV);
            }
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
            //                DateTime xsav;
            bool simulation_done;
            //                bool FinishPoint;
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
            MaxStep = 1.0;
            h = MaxStep;

            SimulationDate = DateTime.Now;

            //            ModelStartTime = TStart;
            //            TPreviousStep = TStart;
            TPresent = TStart;
            SV.WriteResults(TStart); // Write Initial Conditions as the first data Point

            Derivs(x, 1);

            // (**  Start stepping the RungeKutta.....**)
            while (!simulation_done)
            {
                Integrate_CheckZeroStateAllSVs();
                Derivs(x, 1);

                foreach (TStateVariable TSV in SV)
                {
                    Integrate_SetYScale(TSV);
                }
                //                  FinishPoint = (Convert.ToInt32(x * (1 / dxsav)) > Convert.ToInt32(xsav * (1 / dxsav)));

                SV.WriteResults(x); // Write output to Results Collection

                //                    if (FinishPoint) // if it is time to write rates
                //                    {      xsav = x;      }

                if ((x.AddDays(h) - TEnd).TotalDays > 0.0)  // Question about x+h-TStart
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

                AdaptiveStep(ref x, h, RelError, ref h_taken, ref hnext);
                //                rk_has_executed = true;

                TPresent = x;

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

            SV.WriteResults(x); // Write final step to Results Collection
            return PostProcessResults();

        }  // integrate

        public double TrapezoidalIntegration(ref string ErrMsg, DateTime Start_Interval_Time, DateTime End_Interval_Time, List<double> vals, int rti)
        {
            double End_SI_Val;  // ending sub-interval value
            double Start_SI_Val; // starting sub-interval value
            double SumThusFar = 0; // running sum of trapezoid areas

            ErrMsg = "";
            if (rti <= 0) { ErrMsg = "TrapezoidalIntegration index error, rti<=0"; return -99; };
            if (rti > vals.Count - 1) { ErrMsg = "TrapezoidalIntegration index error rti >count"; return -99; };
            bool firststep = true;

            for (int i=rti-1; (firststep||SV.restimes[i]<End_Interval_Time); i++)
            {
                firststep = false;
                DateTime Start_SI_Time = SV.restimes[i];
                DateTime End_SI_Time = SV.restimes[i + 1];
                Start_SI_Val = vals[i];
                End_SI_Val = vals[i + 1];

                if (End_SI_Time > End_Interval_Time)
                {
                    // Linearly interpolate to get the end sub-interval point
                    End_SI_Val = LinearInterpolate(ref ErrMsg,Start_SI_Val, End_SI_Val, Start_SI_Time, End_SI_Time, End_Interval_Time);
                    if (ErrMsg!="") return -99;
                    End_SI_Time = End_Interval_Time;
                }

            if (Start_SI_Time < Start_Interval_Time)
            {
                // Linearly interpolate to get the beginning sub-interval point
                Start_SI_Val = LinearInterpolate(ref ErrMsg, Start_SI_Val, End_SI_Val, Start_SI_Time, End_SI_Time, Start_Interval_Time);
                if (ErrMsg != "") return -99;
                Start_SI_Time = Start_Interval_Time;
            }

            SumThusFar = SumThusFar + ((Start_SI_Val + End_SI_Val) / 2) * (End_SI_Time - Start_SI_Time).TotalDays;
             // The area of the relevant trapezoid is calculated above

            }

            return SumThusFar/ (End_Interval_Time - Start_Interval_Time).TotalDays; 
        }

        public double LinearInterpolate(ref string ErrMsg, double OldVal, double NewVal, DateTime OldTime, DateTime NewTime, DateTime InterpTime)
        {
            // Interpolates to InterpTime between two points, OldPoint and NewPoint
            if ((InterpTime > NewTime) || (InterpTime < OldTime)) { ErrMsg = "Linear Interpolation Timestamp Error"; return -99; };

            return OldVal + ((NewVal - OldVal) / (NewTime - OldTime).TotalDays) * (InterpTime - OldTime).TotalDays;
                   // y1    // Slope  (dy/dx)                                      // Delta X
        }

        public double InstantaneousConc(ref string ErrMsg, DateTime steptime, List<double> vals, int rti)
        {
            if (rti <= 0) { ErrMsg = "Linear interpolation index error, rti<=0"; return -99; };
            if (rti > vals.Count-1) { ErrMsg = "Linear interpolation index error rti >count"; return -99; };

            double OldVal = vals[rti - 1];
            double NewVal = vals[rti];
            DateTime OldTime = SV.restimes[rti-1];
            DateTime NewTime = SV.restimes[rti];

            return LinearInterpolate(ref ErrMsg, OldVal, NewVal, OldTime, NewTime, steptime);
        }



        public string PostProcessResults()
        {
            double val;
            string errmsg="";
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
            
            foreach (TStateVariable TSV in SV)
            {
                if (TSV.Results == null) return "Results not initialized for SV " + TSV.PName;
                if (TSV.Results.Count == 0) return "No results saved for SV " + TSV.PName;

                TSV.output = new AQUATOXITSOutput();
                TSV.output.Dataset = TSV.PName;
                TSV.output.DataSource = "AQUATOX";
                TSV.output.Metadata = new Dictionary<string, string>()
                {
                    {"AQUATOX_HMS_Version", "1.0.0"},
                    {"SimulationDate", (SimulationDate.ToString(Consts.DateFormatString))},
                };

                TSV.output.Data = new Dictionary<string, List<string>>();
                List<string> vallist = new List<string>();
                vallist.Add(TSV.Results[0].ToString(Consts.ValFormatString));
                TSV.output.Data.Add(SV.restimes[0].ToString(Consts.DateFormatString), vallist);
                for (int i = 1; i <= numsteps; i++)
                {
                    DateTime steptime = PSetup.FirstDay.AddDays(i * stepsize);
                    if (PSetup.AverageOutput) val = TrapezoidalIntegration(ref errmsg,steptime.AddDays(-stepsize) ,steptime, TSV.Results, StartIndices[i - 1]);
                    else val = InstantaneousConc(ref errmsg, steptime, TSV.Results, StartIndices[i - 1]);
                    vallist = new List<string>();
                    vallist.Add(val.ToString(Consts.ValFormatString));
                    TSV.output.Data.Add(steptime.ToString(Consts.DateFormatString), vallist);
                    if (errmsg != "") return errmsg;

                }

            }
            return "";
        }

        public void CalculateAllLoads(DateTime TimeIndex)
        // If EstuarySegment then TSalinity(GetStatePointer(Salinity, StV, WaterCol)).CalculateLoad(TimeIndex); {get salinity vals for salt balance}
        // TVolume(GetStatePointer(Volume, StV, WaterCol)).CalculateLoad(TimeIndex);
        // TVolume(GetStatePointer(Volume, StV, WaterCol)).Derivative(Junk);
        {
            foreach (TStateVariable TSV in SV)
                TSV.CalculateLoad(TimeIndex);
        }


    public void SVsToInitConds()
        {
            foreach (TStateVariable TSV in SV)
                TSV.SetToInitCond();
        }


    public double DynamicZMean()
    {
        double result;
        // Variable ZMean of segment or both segments if dynamic stratification
        if (!Location.Locale.UseBathymetry)
        {
            result = Volume_Last_Step / Location.Locale.SurfArea;
            return result;
        }
        result = DynZMean.ReturnLoad(TPresent);

        if (result == 0)
        {
            result = Location.Locale.ICZMean;
        }
        return result;
    }

        public void SetupLinks()
        {
            foreach (TStateVariable TSV in SV)
            {
                TSV.StepRes = new double[7];
                TSV.AQTSeg = this;
                TSV.Location = this.Location;
            }
        }

    public double GetState(AllVariables S, T_SVType T, T_SVLayer L)
    {
        double result;
        TStateVariable p;
        p = GetStatePointer(S, T, L);
        if (!(p == null))
        {
            result = p.State;
        }
        else
        {
            result = -1;
        }
        return result;
    }

        public TStateVariable GetStatePointer(AllVariables S, T_SVType T, T_SVLayer L)
        {
            foreach (TStateVariable TSV in SV)
                if ((TSV.NState == S) && (TSV.SVType == T) && (TSV.Layer == L))  // needs optimization!
                {
                    return TSV;
                }
            return null;
        }



        //public void calchradius(bool averaged)
        //{
        //    // calculate the hydraulic radius & channel depth
        //    double runoff;
        //    double vol;
        //    // update runoff / discharge data
        //    if (averaged)
        //    {
        //        runoff = meandischarge;
        //    }
        //    else
        //    {
        //        runoff = location.discharge[vseg];
        //    }
        //    sed_data.avg_disch = runoff / 86400;
        //    if (sed_data.avg_disch <= 0)
        //    {
        //        sed_data.avg_disch = location.discharge_using_qbase() / 86400;
        //    }
        //    // m3/s
        //    // m3/d
        //    // s/d
        //    sed_data.channel_depth = math.pow(sed_data.avg_disch * sed_data.manning / (math.sqrt(sed_data.slope) * sed_data.width), 3 / 5);
        //    if (averaged)
        //    {
        //        vol = meanvolume;
        //    }
        //    else
        //    {
        //        vol = volume_last_step;
        //    }
        //    //@ unsupported property or method(d): 'surfarea'
        //    sed_data.avg_depth = vol / location.locale.surfarea;
        //    // simpler depth formulation to match hspf 2-5-2003
        //    sed_data.hradius = sed_data.avg_depth * sed_data.width / (2 * sed_data.avg_depth + sed_data.width);
        //    // with

        //}

        //// ---------------------------------------------------------------


        //public double velocity(ref string errstr, double pctriffle, double pctpool, bool averaged)
        //{
        //    double xsecarea;
        //    double avgflow;
        //    double upflow;
        //    double downflow;
        //    double pctrun;
        //    double runvel;
        //    double rifflevel;
        //    double poolvel;
        //    double vol;
        //    // ----------------------------------------------------------------------------------------------------
        //    if (averaged) vol = meanvolume;
        //    else vol = volume_last_step;

        //    if ((location.sitetype == sitetypes.stream))
        //    {
        //        calchradius(averaged);
        //        xsecarea = sed_data.width * sed_data.channel_depth;
        //        // m2               // m                // m
        //        if (averaged)
        //        {
        //            calchradius(false);
        //        }
        //    }
        //    else
        //    {
        //        xsecarea = vol / (location.locale.sitelength * 1000);
        //        // m3                    // km      // m/km
        //    }
        //    pctrun = 100 - pctriffle - pctpool;
        //    if ((calcvelocity || averaged))
        //    {
        //        morphrecord _wvar3 = location.morph;
        //        upflow = _wvar3.inflowh2o[vseg];
        //        downflow = location.discharge[vseg];
        //        if (averaged)
        //        {
        //            upflow = meandischarge;
        //            // m3/d
        //            downflow = meandischarge;
        //        }
        //        avgflow = (upflow + downflow) / 2;
        //        // m3/d   // m3/d    // m3/d
        //        runvel = avgflow / xsecarea * (1 / 86400) * 100;
        //        // cm/s  // m3/d    // m2         // d/s  // cm/m

        //        if (runvel < 0) runvel = 0;
        //    }
        //    else
        //    {
        //        // user entered velocity
        //        runvel = dynvelocity.getload(tpresent, true);
        //        // cm/s
        //        avgflow = runvel * xsecarea * 86400 * 0.01;
        //        // m3/d  // cm/s   // m2    // s/d  // m/cm
        //    }
        //    if (avgflow < 2.59e5)
        //    {
        //        // q < 2.59e5 m3/d
        //        rifflevel = 1.60 * runvel;
        //        poolvel = 0.36 * runvel;
        //    }
        //    else if (avgflow < 5.18e5)
        //    {
        //        // 2.59e5 m3/d < q < 5.18e5 m3/d
        //        rifflevel = 1.30 * runvel;
        //        poolvel = 0.46 * runvel;
        //    }
        //    else if (avgflow < 7.77e5)
        //    {
        //        // 5.18e5 m3/d q < 7.77e5 m3/d
        //        rifflevel = 1.10 * runvel;
        //        poolvel = 0.56 * runvel;
        //    }
        //    else
        //    {
        //        // q >= 7.77e5 m3/d
        //        rifflevel = 1.00 * runvel;
        //        poolvel = 0.66 * runvel;
        //    }
        //    // cm/s

        //    return (rifflevel * (pctriffle / 100)) + (runvel * (pctrun / 100)) + (poolvel * (pctpool / 100));
        //}

    }  // end TAQUATOXSegment

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
            KnownTypes = new List<Type> { typeof(TStateVariable), typeof(T10PctDecrease), typeof(TExpIncrease), typeof(AQUATOXSegment), typeof(TAQTSite), typeof(MorphRecord),
                                          typeof(SiteRecord), typeof(ReminRecord), typeof(Setup_Record), typeof(AQUATOX.Volume.TVolume), typeof(LoadingsRecord), typeof(TLoadings),
                                          typeof(SortedList<DateTime, double>), typeof(AQUATOXITSOutput) };  // , typeof(Dictionary<string,string>), typeof(Dictionary<string, List<string>>) };
    }
}

}

