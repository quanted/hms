using System;
using Globals;
using AQUATOX.AQTSegment;
using AQUATOX.Nutrients;
using Stream.Hydrology.AQUATOX;

namespace Plants.AQUATOX
{
    /// <summary>
    /// An AQUATOX Plants model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTPlants
    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;

        /// <summary>
        /// Instantiates an AQUATOX Plants model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries.
        /// Valid JSON inputs must include an AQUATOX segment with a set of Plant state variables attached, and a valid site record and morphometry data, and PSETUP record.
        /// Example valid JSON inputs and documentation including a discussion of data requirements may be found in the Plants\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, output parameter: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        /// 
        public AQTPlants(ref string json, out string errmsg, bool RunModel)
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
        /// Checks for data requirements for an AQTPlants model including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec();

            AQTVolumeModel AQTVM = new AQTVolumeModel(AQSim);
            string checkvol = AQTVM.CheckDataRequirements();
            if (checkvol != "") return checkvol;

            TpHObj PpH = (TpHObj)AQSim.AQTSeg.GetStatePointer(AllVariables.pH, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PpH == null)) return "A pH state variable (or driving variable) must be included in a plant simulation.";

            TCO2Obj PCO2 = (TCO2Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.CO2, T_SVType.StV, T_SVLayer.WaterCol);
            if ((PCO2 == null)) return "A CO2 state variable (or driving variable) must be included in a plant simulation.";

            TTemperature TTemp = (TTemperature)AQSim.AQTSeg.GetStatePointer(AllVariables.Temperature, T_SVType.StV, T_SVLayer.WaterCol);
            if (TTemp == null) return "A Temperature state variable or driving variable must be included in the simulation. ";

            TNH4Obj PNH4 = (TNH4Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.Ammonia, T_SVType.StV, T_SVLayer.WaterCol);
            if (PNH4 == null) return "An Ammonia (TNH4Obj) state variable or driving variable must be included in the simulation. ";

            TNO3Obj PNO3 = (TNO3Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.Nitrate, T_SVType.StV, T_SVLayer.WaterCol);
            if (PNO3 == null) return "A Nitrate (TNO3Obj) state variable or driving variable must be included in the simulation. ";

            TPO4Obj PPO4 = (TPO4Obj)AQSim.AQTSeg.GetStatePointer(AllVariables.Phosphate, T_SVType.StV, T_SVLayer.WaterCol);
            if (PPO4 == null) return "A Phosphate (TPO4Obj) state variable or driving variable must be included in the simulation. ";

            TLight TLt = (TLight)AQSim.AQTSeg.GetStatePointer(AllVariables.Light, T_SVType.StV, T_SVLayer.WaterCol);
            if (TLt == null) return "A Light state variable or driving variable must be included in the simulation. ";

            return "";
        }

    }
}
