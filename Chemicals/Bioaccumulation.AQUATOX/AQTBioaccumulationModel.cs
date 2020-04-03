using AQUATOX.AQTSegment;
using AQUATOX.Nutrients;
using AQUATOX.Chemicals;
using AQUATOX.Bioaccumulation;
using Stream.Hydrology.AQUATOX;
using Globals;

namespace AQUATOXBioaccumulation

{
    /// <summary>
    /// An AQUATOX bioaccumulation model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTBioaccumulationModel
    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;

        /// <summary>
        /// Instantiates an AQUATOX Chemicals model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries.
        /// Valid JSON inputs must include an AQUATOX segment with one or more TToxics state variables attached, and valid site record, morphometry data, and PSETUP records.
        /// Example valid JSON inputs and documentation including a list of data requirements may be found in the Chemicals\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, output parameter: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        public AQTBioaccumulationModel(ref string json, out string errmsg, bool RunModel)

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
        /// Instantiates an AQUATOX Chemicals model given an existing AQUATOX Simulation insim.  Used for testing multi-purpose models.
        /// </summary>
        /// <param name="insim"></param> AQTSim.  The AQUATOX simulation being typecast or tested as an AQUATOX Chemicals Model
        public AQTBioaccumulationModel(AQTSim insim)
        { AQSim = insim; }


        /// <summary>
        /// Checks for data requirements for an AQTChemicalsModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec();

            bool FoundTox = false;
            for (T_SVType Typ = T_SVType.OrgTox1; Typ <= T_SVType.OrgTox20; Typ++)
                { TToxics TTx = (TToxics)AQSim.AQTSeg.GetStatePointer(AllVariables.H2OTox, Typ, T_SVLayer.WaterCol);
                  if (TTx != null) { FoundTox = true; break; }
                }
            if (!FoundTox) return "A TToxics (toxicant in the water column) state variable must be included in the simulation. ";


            AQTVolumeModel AQTVM = new AQTVolumeModel(AQSim);
            string checkvol = AQTVM.CheckDataRequirements();
            if (checkvol != "") return checkvol;

            TO2Obj TO2 = (TO2Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.Oxygen, T_SVType.StV, T_SVLayer.WaterCol);
            if (TO2 == null) return "An Oxygen state variable or driving variable must be included in the simulation. ";

            TTemperature TTemp = (TTemperature)AQSim.AQTSeg.GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in the simulation. ";

            TpHObj TpH = (TpHObj)AQSim.AQTSeg.GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if (TpH == null) return "A pH loading variable or state variable must be included in a chemical simulation.";
            if ((!TpH.UseLoadsRecAsDriver) && (TpH.LoadsRec.Loadings.NoUserLoad))  // pH calculation, not a driving variable, check pH model data requirements
            {
                TCO2Obj TCO2 = (TCO2Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
                if (TCO2 == null) return "A CO2 state variable or driving variable must be included in the simulation to calculate pH. ";
            }

            TLight TLt = (TLight)AQSim.AQTSeg.GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (TLt == null) return "A Light state variable or driving variable must be included in the simulation. ";

            
            return "";
        }
    }

}

