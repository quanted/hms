using AQUATOX.AQTSegment;
using AQUATOX.Chemicals;
using Stream.Hydrology.AQUATOX;
using Globals;
using AQUATOXBioaccumulation;


namespace AQUATOXEcotoxicology

{
    /// <summary>
    /// An AQUATOX Ecotoxicology model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTEcotoxicologyModel
    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;

        /// <summary>
        /// Instantiates an AQUATOX Ecotoxicology model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries.
        /// Valid JSON inputs must include an AQUATOX segment with one or more TToxics state variables attached, and valid site record, morphometry data, and PSETUP records.
        /// Example valid JSON inputs and documentation including a list of data requirements may be found in the Ecotoxicology\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, output parameter: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        public AQTEcotoxicologyModel(ref string json, out string errmsg, bool RunModel)

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
        /// Instantiates an AQUATOX Ecotoxicology model given an existing AQUATOX Simulation insim.  Used for testing multi-purpose models.
        /// </summary>
        /// <param name="insim"></param> AQTSim.  The AQUATOX simulation being typecast or tested as an AQUATOX Ecotoxicology Model
        public AQTEcotoxicologyModel(AQTSim insim)
        { AQSim = insim; }


        /// <summary>
        /// Checks for data requirements for an AQTEcotoxicologyModel including state variable requirements and parameter values.
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

            bool FoundBiota = false;
            for (AllVariables ns = Consts.FirstBiota; ns <= Consts.LastBiota; ns++)
            {
                TStateVariable biota = AQSim.AQTSeg.GetStatePointer(ns, T_SVType.StV, T_SVLayer.WaterCol);
                if (biota != null) { FoundBiota = true; break; }
            }
            if (!FoundBiota) return "To calculate ecotoxicological effects, an animal or plant state variable must be included in the model. ";

            if (!AQSim.AQTSeg.PSetup.UseExternalConcs)  // To calculate effects of chemicals based on internal body burdens, a bioaccumulation model must be included.
            {
                AQTBioaccumulationModel AQTBM = new AQTBioaccumulationModel(AQSim);
                string checkbio = AQTBM.CheckDataRequirements();
                if (checkbio != "") return checkbio;
            }

           
            return "";
        }
    }

}

