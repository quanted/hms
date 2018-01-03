using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQSite;
using AQUATOX.Loadings;
using AQUATOX.Volume;
using Newtonsoft.Json.Serialization;
using System.Linq;
using Newtonsoft.Json;

namespace AQUATOX.AQTSegment
    


{

    public class TStateVariable
    {
        public double InitialCond = 0;     // Initial condition
        public double State = 0;           // Current Value, usually concentration
        public AllVariables NState;        // List of Organisms and Toxicants
        public T_SVType SVType;            // StV, OrgTox
        public object Layer;               // Relevant for sed detr, inorg sed., and pore water types
        public string PName = "";          // Text Name
        public double yhold = 0;           // holds State value during derivative cycle
        double yorig = 0;                  // used to restore state to beginning of time step
        double yout = 0;                    // use in Integration
        public double[] StepRes = new double[7];  // Holds Step Results
        public double yerror = 0;          // holds error term from RKCK
        public double yscale = 0;          // use in Integration
        public List<double> Results = new List<double>(); // holds numerical results

        [JsonIgnore] public AQUATOXSegment AQTSeg = null;   // Pointer to Collection of State Variables of which I am a member
        public LoadingsRecord LoadsRec = null;   // Holds all of the Loadings Information for this State Variable  
        [JsonIgnore] public double Loading = 0;         // Loading of State Variable This time step
        bool RequiresData = false;
        bool HasData = false;        // If RequiresUnderlyingData and Not HasData then Model cannot be run
        public string StateUnit;
        public string LoadingUnit;       // Units 

        [JsonIgnore] public TAQTSite Location = null;    // Pointer to Site in which I'm located
 //      public bool PShowRates = true;      // Does the user want rates written for this SV?
 //      public TCollection RateColl = null; // Collection of saved rates for current timestep
 //      public int RateIndex = 0;

        public string LoadNotes1;
        public string LoadNotes2;           // Notes associated with loadings
        public bool TrackResults = true;      // Does the user want to save results for this variable?
                                       //bool IsTemplate = false;       // Is this a member of the template study in a linked system?  True if single study run.
                                       //        public double[] WashoutStep = new double[7];     // Saved Washout Variables for use in outputting Cascade Outflow, nosave
        [JsonIgnore] double WashoutAgg = 0;
        [JsonIgnore] double LastTimeWrit = 0;


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



    public class TExpIncrease : TStateVariable
    {
        public override void Derivative(ref double DB)
        {
            DB = State * 0.04;
        }
    }


    public class T10PctDecrease : TStateVariable

    {
        public override void Derivative(ref double DB)

        {
            DB = -State * 0.1;
        } 

    }

        public class TStates : List<TStateVariable>
        {
            public List<DateTime> restimes = new List<DateTime>();


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



    [Serializable()]
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
        [JsonIgnore] public DateTime VolumeUpdated;
        [JsonIgnore] public double Volume_Last_Step;    //Volume in the previous step, used for calculating dilute/conc,  if stratified, volume of whole system(nosave)}


        public AQUATOXSegment()
        {

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
                TSV.Results.Clear();
            SV.restimes.Clear();

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
            TVolume SVTV = new TVolume(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol, "Water Volume", this, 1000);

            SV.Add(SVI);
            SV.Add(SVD);
            SV.Add(SVF);
            SV.Add(SVTV);

            SVI.State = 10;
            SVD.State = 10;
            SVF.State = 10;
            SVTV.State = 15;

            SVTV.LoadsRec = new LoadingsRecord();
            SVTV.LoadsRec.Loadings = new TLoadings();
            SVTV.LoadsRec.Loadings.list.Add(Convert.ToDateTime("1993-01-04"), 40);
            SVTV.LoadsRec.Loadings.list.Add(Convert.ToDateTime("1993-01-01"), 60);

            TLoadings altLoad1 = new TLoadings();
            altLoad1.list.Add(Convert.ToDateTime("1993-01-04"), 30);
            altLoad1.list.Add(Convert.ToDateTime("1993-01-01"), 45);
            SVTV.LoadsRec.Alt_Loadings[0] = altLoad1;

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
                //@ Unsupported property or method(D): 'UseFixStepSize'
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
            //@ Unsupported property or method(D): 'UseFixStepSize'
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
        public void Integrate(DateTime TStart, DateTime TEnd, double RelError, double h_minimum, double dxsav)
        {
            // Starting Point of Integral
            // Ending Point of Integral
            // Requested Accuracy of Results
            // Smallest Step Size
            // Store Result Interval
            //            bool rk_has_executed;
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

                SV.WriteResults(x); // Write final step to Results Collection

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

        }  // integrate


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
            //@ Unsupported property or method(D): 'SurfArea'
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
                TSV.AQTSeg = this;
                TSV.Location = this.Location;
            }
        }

    }  // end TAQUATOXSegment


    public class KnownTypesBinder : Newtonsoft.Json.Serialization.ISerializationBinder
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

        public KnownTypesBinder()
        {
            KnownTypes = new List<Type> { typeof(TStateVariable), typeof(T10PctDecrease), typeof(TExpIncrease), typeof(AQUATOXSegment), typeof(TAQTSite), typeof(MorphRecord),
                                          typeof(SiteRecord), typeof(ReminRecord), typeof(Setup_Record), typeof(AQUATOX.Volume.TVolume), typeof(LoadingsRecord), typeof(TLoadings),
                                          typeof(SortedList<DateTime, double>)};
        }
    }

}



