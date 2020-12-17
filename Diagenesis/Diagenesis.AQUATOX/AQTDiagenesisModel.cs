using Globals;
using AQUATOX.AQTSegment;
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
        /// <param name="errmsg"></param> string, output parameter: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        public AQTDiagenesisModel(ref string json, out string errmsg, bool RunModel)

        {
            errmsg = "";
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
        /// Checks for data requirements for an AQTDiagenesisModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec();

            AQTNutrientsModel AQTNM = new AQTNutrientsModel(AQSim);
            string checknutrients = AQTNM.CheckDataRequirements();
            if (checknutrients != "") return checknutrients;

            return AQSim.AQTSeg.AQTDiagenesisModel_CheckDataRequirements();
        }
    }

}

