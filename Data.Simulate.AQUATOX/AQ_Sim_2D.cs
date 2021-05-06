using System;
using System.Collections.Generic;
using Globals;
using AQUATOX.AQSite;
using AQUATOX.AQTSegment;
using AQUATOX.Volume;
using AQUATOX.Loadings;

using System.Threading;
using System.Threading.Tasks;

using System.Linq;
using Newtonsoft.Json;
using Data;
using System.ComponentModel;
using System.IO;
using System.Data;
using AQUATOX.Plants;
using AQUATOX.Animals;

namespace AQUATOX.AQSim_2D


{
    public class AQSim_2D
    {
        public class streamNetwork
        {
            public string[][] network;
            public int[][] order;
            public Dictionary<string, int[]> sources;
        }

        public string baseSimJSON;
        public streamNetwork SN = null;
        public List<string> SVList = null;
        public Setup_Record MasterSetup = null;

        public void CreateStreamNetwork(string SNJSON)
        {
            SN = Newtonsoft.Json.JsonConvert.DeserializeObject<streamNetwork>(SNJSON);

        }

        public void UpdateDischarge(TVolume TVol, Data.TimeSeriesOutput ATSO)
        {
            TVol.Calc_Method = VolumeMethType.Manning;

            // 1 is discharge in this case
            TLoadings DischargeLoad = TVol.LoadsRec.Alt_Loadings[1];

            if (DischargeLoad.ITSI == null)
            {
                TimeSeriesInputFactory Factory = new TimeSeriesInputFactory();
                TimeSeriesInput input = (TimeSeriesInput)Factory.Initialize();
                input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                DischargeLoad.ITSI = input;
            }

            DischargeLoad.ITSI.InputTimeSeries.Add("input", ATSO);
            DischargeLoad.Translate_ITimeSeriesInput();
            DischargeLoad.MultLdg = 1e6;
            DischargeLoad.UseConstant = false;
            TVol.LoadNotes1 = "From 2-D Linkage";
            TVol.LoadNotes2 = "";
            DischargeLoad.ITSI = null;

        }

        public Dictionary<int, archived_results> archive = new Dictionary<int, archived_results>();

        public class archived_results
        {
            public DateTime[] dates;
            public double[] washout;  // m3
            public double[][] concs; // g/m3 or mg/m3 depending on state var
        }


        private void archiveOutput(int comid, AQTSim Sim)
        {
            archived_results AR = new archived_results();
            TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;

            ITimeSeriesOutput ito = tvol.SVoutput;

            int washoutindex = -1;
            int ccount = ito.Data.Values.ElementAt(0).Count();
            for (int col = 2; col <= ccount; col++)
            {
                if ((ito.Metadata["Name_" + col.ToString()] == "Discharge")
                    && (ito.Metadata["Unit_" + col.ToString()] == "m3/d")) washoutindex = col;
            }

            int ndates = tvol.SVoutput.Data.Keys.Count;
            AR.dates = new DateTime[ndates];
            AR.washout = new double[ndates];

            for (int i = 0; i < ndates; i++)  
            {
                string datestr = ito.Data.Keys.ElementAt(i).ToString();
                Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[washoutindex-1]);
                AR.dates[i] = Convert.ToDateTime(datestr);
                AR.washout[i] = Val;  // m3/d
            }

            AR.concs = new double[Sim.AQTSeg.SV.Count()][];
            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                AR.concs[iTSV] = new double[ndates];
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];
                for (int i = 0; i < ndates; i++)
                {
                    ito = TSV.SVoutput;
                    Double Val = Convert.ToDouble(ito.Data.Values.ElementAt(i)[0]);
                    AR.concs[iTSV][i] = Val;
                }
            }

            archive.Add(comid, AR);
        }

        private void Pass_Data(AQTSim Sim, int SrcID, int ninputs)
        {
            archived_results AR;
            archive.TryGetValue(SrcID, out AR);

            for (int iTSV = 0; iTSV < Sim.AQTSeg.SV.Count; iTSV++)
            {
                TStateVariable TSV = Sim.AQTSeg.SV[iTSV];

                if (((TSV.NState >= AllVariables.H2OTox) && (TSV.NState <= AllVariables.TSS)) ||   //fixme improve conditions for drift
                    ((TSV.NState >= AllVariables.DissRefrDetr) && (TSV.NState <= AllVariables.SuspLabDetr)) || // fixme macrophytes 
                    ((TSV.IsPlant()) && ((TPlant)TSV).IsPhytoplankton()) ||
                    ((TSV.IsAnimal()) && ((TAnimal)TSV).IsPlanktonInvert()))
                {
                    TVolume tvol = Sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume;
                    int ndates = AR.dates.Count();
                    TLoadings DischargeLoad = tvol.LoadsRec.Alt_Loadings[1];  //fixme refine with inflow load?
                    SortedList<DateTime, double> newlist = new SortedList<DateTime, double>();

                    for (int i = 0; i < ndates; i++)
                    {
                        double InVol = AR.washout[i];  // fixme replace with calculated inflow volume using Mannings N;
                        double OutVol = AR.washout[i];

                        if (ninputs == 1) newlist.Add(AR.dates[i], AR.concs[iTSV][i] * (OutVol / InVol));  // first or only input
                        else newlist.Add(AR.dates[i], TSV.LoadsRec.Loadings.list.Values[i] + AR.concs[iTSV][i] * (OutVol / InVol));  //adding second or third inputs

                    }

                    TSV.LoadsRec.Loadings.list = newlist;
                }

            }
        }

        public bool executeModel(int comid, ref string outstr, ref string jsonstring)  
        {
            AQTSim Sim = new AQTSim();
            outstr = Sim.Instantiate(jsonstring);

            if (outstr != "")  return false; 
            Sim.AQTSeg.SetMemLocRec();
          //   Sim.AQTSeg.PSetup = MasterSetup;  // fixme test copying back of parameters


            if (SVList == null)
            {
                SVList = new List<string>();
                foreach (TStateVariable SV in Sim.AQTSeg.SV)
                {
                    SVList.Add(SV.PName);   //save list of SVs for output
                }
            }
                

            int nSources = 0;
            if (SN.sources.TryGetValue(comid.ToString(), out int[] Sources))
                foreach (int SrcID in Sources)
                {
                    if (SrcID != comid)  // set to itself in boundaries 
                    {
                        nSources++;
                        Pass_Data(Sim, SrcID, nSources);
                        outstr = outstr + "Passed data from Source " + SrcID.ToString() + " into COMID " + comid.ToString() + Environment.NewLine;
                    }
                };

            Sim.AQTSeg.RunID = "2D Run: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            string errmessage = Sim.Integrate();
            if (errmessage == "")
            {
                archiveOutput(comid, Sim);
                string jsondata = "";
                errmessage = Sim.SaveJSON(ref jsondata);

                if (errmessage != "")
                {
                    outstr += errmessage + Environment.NewLine;
                    return false;
                }
            }
            else { 
                   outstr += errmessage + Environment.NewLine;
                   return false;
                 };

            outstr += "--> Executed COMID " + comid.ToString();
            return true;
        }




    }




}


