﻿using System;
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

            return AQSim.AQTSeg.AQTPlantModel_CheckDataRequirements();
        }
    }
}
