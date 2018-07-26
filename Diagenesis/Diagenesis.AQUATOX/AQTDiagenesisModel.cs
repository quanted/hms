using System;
using Globals;
using AQUATOX.AQTSegment;
using Stream.Hydrology.AQUATOX;
using AQUATOXNutrientModel;

namespace Diagenesis.AQUATOX
{

    /// <summary>
    /// An AQUATOX Diagenesis model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTDiagenesisModel
    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;


        /// <summary>
        /// Instantiates an AQUATOX Diagenesis model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries.
        /// Valid JSON inputs must include an AQUATOX segment with one or more Diagenesis state variables attached, and valid site record, morphometry data, and PSETUP records.
        /// Example valid JSON inputs and documentation including a list of data requirements may be found in the Diagenesis\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, passed by reference: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        public AQTDiagenesisModel(ref string json, ref string errmsg, bool RunModel)

        {
            AQSim = new AQTSim();
            errmsg = AQSim.Instantiate(json);
            if (errmsg == "")
            {
                errmsg = CheckDataRequirements();
                if ((errmsg == "") && RunModel)
                {
                    errmsg = AQSim.Integrate();
                    if (errmsg == "")
                    {
                        errmsg = AQSim.SaveJSON(ref json);
                    }
                }
            }
        }

        string CheckForVar(AllVariables ns, T_SVType typ, T_SVLayer wc, string name)
        {
            TStateVariable TSV = AQSim.AQTSeg.GetStatePointer(ns, typ, wc);
            if (TSV == null) return "Simulation is missing a required " + name + " state variable.";
            return "";
        }

        /// <summary>
        /// Checks for data requirements for an AQTDiagenesisModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec();

            AQTNutrientsModel AQTNM = new AQTNutrientsModel(AQSim);
            string checknutrients = AQTNM.CheckDataRequirements();
            if (checknutrients != "") return checknutrients;

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
    }

}

