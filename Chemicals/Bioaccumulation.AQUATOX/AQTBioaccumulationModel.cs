using AQUATOX.AQTSegment;
using AQUATOX.Nutrients;
using AQUATOX.Chemicals;
using AQUATOX.Bioaccumulation;
using Stream.Hydrology.AQUATOX;
using System.Linq;
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
        /// Instantiates an AQUATOX Bioaccumulation model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries.
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
        /// Instantiates an AQUATOX Bioaccumulation model given an existing AQUATOX Simulation insim.  Used for testing multi-purpose models.
        /// </summary>
        /// <param name="insim"></param> AQTSim.  The AQUATOX simulation being typecast or tested as an AQUATOX Bioaccumulation Model
        public AQTBioaccumulationModel(AQTSim insim)
        { AQSim = insim; }


        /// <summary>
        /// Checks for data requirements for an AQTBioaccumulationModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec(); 
            return AQSim.AQTSeg.AQTBioaccumulationModel_CheckDataRequirements();
        }
    }

}

