using System;
using System.IO;
using System.Collections.Generic;

namespace AQUATOX.Loadings
{
    
    public class TLoadings

        {
        public SortedList<DateTime, double> list;
        public bool Hourly = false;
        public double ConstLoad = 0;         // User Input constant load
        public bool UseConstant = true;      // Flag for using constant load
        public bool NoUserLoad = false;      // Flag for using user input load, or ignoring  the load and using annual ranges and means.  Relevant to Temp, Light, pH, and Nutrients
        public double MultLdg = 1;           // to perturb loading


        public double ReturnLoad(DateTime TimeIndex)
        {
            double RetLoad;
            if (UseConstant)
            {
                RetLoad = ConstLoad;
            }
            else
            {
                RetLoad = 0;
                if (list != null)
                {
                    RetLoad = list.IndexOfKey(TimeIndex);
                }
            }
            // else
            return RetLoad;
        }

    } // end TLoadings


    public class LoadingsRecord
    {
        public TLoadings Loadings = new TLoadings();
        // Alt_Loadings is reserved for point source, non pont source and
        // direct precipitation loadings; these vars are relevant only for
        // nstate in [H2OTox,MeHg,HgII,Hg0,Phosphate,Ammonia,Nitrate,All SuspDetr]
        // Has_Alt_Loadings(nstate) is a boolean function in GLOBALS
        public TLoadings[] Alt_Loadings = new TLoadings[3];   // Time series loading

        public LoadingsRecord()
            {
            }


        // -------------------------------------------------------------------
        public double ReturnLoad(DateTime TimeIndex)
        {
            double RetLoad;
            // Hold Result
            if (Loadings.UseConstant)
            {
                RetLoad = Loadings.ConstLoad;
            }
            else
            {
                RetLoad = 0;
                if (Loadings != null)
                {
                    RetLoad = Loadings.list.IndexOfKey(TimeIndex);
                }
            }
            // else
            RetLoad = RetLoad * Loadings.MultLdg;
            return RetLoad;
        }

        // -------------------------------------------------------------------
        public double ReturnAltLoad(DateTime TimeIndex, int AltLdg)
        {
            double result;
            double RetLoad;
            // Hold Result
            if (Alt_Loadings[AltLdg].UseConstant)
            {
                RetLoad = Alt_Loadings[AltLdg].ConstLoad;
            }
            else
            {
                RetLoad = 0;
                if (Alt_Loadings[AltLdg] != null)
                {
                    RetLoad = Alt_Loadings[AltLdg].list.IndexOfKey(TimeIndex);
                }
            }
            // else
            RetLoad = RetLoad * Alt_Loadings[AltLdg].MultLdg;
            return RetLoad;
        }

    } // end Loadings



}



