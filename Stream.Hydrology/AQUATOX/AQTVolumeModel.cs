using AQUATOX.AQTSegment;
using Globals;
using AQUATOX.Volume;
using AQUATOX.AQSite;

namespace Stream.Hydrology.AQUATOX
{

    /// <summary>
    /// An AQUATOX Volume model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTVolumeModel

    {
        /// <summary>
        /// Holds the AQUATOX Simulation
        /// </summary>
        public AQTSim AQSim;

        /// <summary>
        /// Instantiates an AQUATOX Volume model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries
        /// Valid JSON inputs must include an AQUATOX segment with a volume state variable attached, valid site record and morphometry data, and a valid PSETUP record
        /// Example valid JSON inputs with comments may be found in the Stream.Hydrology\AQUATOX\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, output parameter: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <param name="RunModel"></param> bool, if true, the model is run and results saved back to the json string parameter passed by reference. 
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public AQTVolumeModel(ref string json, out string errmsg, bool RunModel)

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
        /// Instantiates an AQUATOX Volume model given an existing AQUATOX Simulation insim.  Used for testing multi-purpose models.
        /// </summary>
        /// <param name="insim"></param> AQTSim.  The AQUATOX simulation being cast or tested as an AQUATOX Volume Model
        public AQTVolumeModel(AQTSim insim)
        { AQSim = insim; }
                

        /// <summary>
        /// Checks for data requirements for an AQTVolumeModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            AQSim.AQTSeg.SetMemLocRec();
            TVolume TVol = (TVolume)AQSim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            if (TVol == null) return "A Volume State Variable must be included in the simulation. ";
            if (AQSim.AQTSeg.Location == null) return "The 'Location' object must be populated with site data. ";
            if (AQSim.AQTSeg.Location.Locale == null) return "The 'Location.Locale' object must be populated with site data. ";
            if (AQSim.AQTSeg.Location.Locale.SiteLength.Val < Consts.Tiny) return "SiteLength must be greater than zero.";
            if (AQSim.AQTSeg.Location.SiteType ==  SiteTypes.Stream)
            {
                if (AQSim.AQTSeg.Location.Locale.Channel_Slope.Val < Consts.Tiny) return "Channel_Slope must be greater than zero to use Mannings Equation.";
            }

            return "";
        }
    }
    
}
