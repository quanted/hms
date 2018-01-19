using System;
using AQUATOX.AQTSegment;
using Globals;

namespace AQUATOX.Volume
    {
    public class AQTVolumeModel : AQTSim
    {
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

        public string CheckDataRequirements()
        {
            TVolume TVol = (TVolume)AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol);
            if (TVol == null) return "A Volume State Variable must be included in the simulation. ";
            if (AQTSeg.Location.Locale.SiteLength < Consts.Tiny) return "SiteLength must be greater than zero.";

            if (AQTSeg.Location.SiteType == AQSite.SiteTypes.Stream)
            {
                if (AQTSeg.Location.Locale.Channel_Slope < Consts.Tiny) return "Channel_Slope must be greater than zero to use Mannings Equation.";
            }

            return "";
        }
    }



}
