using System;
using Globals;
using AQUATOX.AQTSegment;
using AQUATOX.OrgMatter;
using AQUATOX.Nutrients;
using Stream.Hydrology.AQUATOX;

namespace AQUATOXOrganicMatter
{

    /// <summary>
    /// An AQUATOX Organic Matter (detritus) model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTOrganicMatter
    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;

        /// <summary>
        /// Instantiates an AQUATOX Nutrients model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries
        /// Valid JSON inputs must include an AQUATOX segment with a Nutrients state variable attached, valid site record and morphometry data, and a valid PSETUP record
        /// Example valid JSON inputs with comments may be found in the Stream.Hydrology\AQUATOX\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, passed by reference: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public AQTOrganicMatter(ref string json, ref string errmsg, bool RunModel)
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


        /// <summary>
        /// Checks for data requirements for an AQTOrganicMatter including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQTVolumeModel AQTVM = new AQTVolumeModel(AQSim);
            string checkvol = AQTVM.CheckDataRequirements();
            if (checkvol != "") return checkvol;

            TDissRefrDetr PDRD = (TDissRefrDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TDissLabDetr PDLD = (TDissLabDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.DissLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSuspRefrDetr PSRD = (TSuspRefrDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.SuspRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSuspLabDetr PSLD = (TSuspLabDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.SuspLabDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSedRefrDetr PSdRD = (TSedRefrDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.SedmRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
            TSedLabileDetr PSdLD = (TSedLabileDetr)AQSim.AQTSeg.GetStatePointer(AllVariables.SedmLabDetr, T_SVType.StV, T_SVLayer.WaterCol);

            if ((PDRD == null) || (PDLD == null) || (PSRD == null) || (PSLD == null) || (PSdRD == null) || (PSdLD == null))
                return "All six organic matter state variables must be included in an organic-matter simulation.";

            TpHObj PpH = (TpHObj)AQSim.AQTSeg.GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PpH == null)) return "A pH state variable (or driving variable) must be included in an organic-matter simulation.";

            TO2Obj PO2 = (TO2Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PO2 == null)) return "An oxygen state variable (or driving variable) must be included in an organic-matter simulation.";

            

            return "";
        }
    }
}
