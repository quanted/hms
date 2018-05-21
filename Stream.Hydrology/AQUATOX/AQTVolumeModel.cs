using System;
using AQUATOX.AQTSegment;
using Globals;

namespace AQUATOX.Volume
{

    /// <summary>
    /// An AQUATOX Volume model that can be instantiated and executed given a valid JSON input
    /// </summary>
    public class AQTVolumeModel : AQTSim
    {


        /// <summary>
        /// Instantiates an AQUATOX Volume model given a valid JSON input, checks data requirements, integrates, and saves results back to the JSON as iTimeSeries
        /// Valid JSON inputs must include an AQUATOX segment with a volume state variable attached, valid site record and morphometry data, and a valid PSETUP record
        /// Example valid JSON inputs with comments may be found in the Stream.Hydrology\AQUATOX\DOCS directory.
        /// </summary>
        /// <param name="json"></param> string, passed by reference:  a valid json input that is replaced by the model's json output including model results
        /// <param name="errmsg"></param> string, passed by reference: if blank, no error occured and simulation completed successfully, otherwise error details are provided within the string
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public AQTVolumeModel(ref string json, ref string errmsg)

        {
            errmsg = Instantiate(json);
            if (errmsg == "")
            {
                errmsg = CheckDataRequirements();
                if (errmsg == "")
                {
                    errmsg = Integrate();
                    if (errmsg == "")
                    {
                        errmsg = SaveJSON(ref json);
                    }
                }
            }
        }

        /// <summary>
        /// Checks for data requirements for an AQTVolumeModel including state variable requirements and parameter values.
        /// </summary>
        /// <returns>string: Error message that is non blank if the simulation json structure does not have the required data </returns>
        public string CheckDataRequirements()
        {
            TVolume TVol = (TVolume)AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            if (TVol == null) return "A Volume State Variable must be included in the simulation. ";
            if (AQTSeg.Location == null) return "The 'Location' object must be populated with site data. ";

            if (AQTSeg.Location.Locale.SiteLength < Consts.Tiny) return "SiteLength must be greater than zero.";
            if (AQTSeg.Location.SiteType == AQSite.SiteTypes.Stream)
            {
                if (AQTSeg.Location.Locale.Channel_Slope < Consts.Tiny) return "Channel_Slope must be greater than zero to use Mannings Equation.";
            }

            return "";
        }
    }
    
}
