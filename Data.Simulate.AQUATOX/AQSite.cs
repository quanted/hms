using System;
using System.IO;
using System.Runtime.Serialization;

namespace AQUATOX.AQSite

{

    public enum StreamTypes
    {
        concrete_channel,
        dredged_channel,
        natural_channel
    } // end StreamTypes


    public struct SiteRecord
    {
        public string SiteName;
        public double ECoeffWater;
        public double SiteLength;        // units are KM
        public string XLength;
        public double StaticVolume;
        public string XVolume;
        public double SurfArea;
        public string XSurfArea;
        public double ICZMean;
        public string XZMean;
        public double ZMax;
        public string XZMax;
        public double[] TempMean;
        public string XTempMean;
        public double[] TempRange;
        public string XTempRange;
        public double Latitude;
        public string XLatitude;
        public double LightMean;
        public string XLightMean;
        public double LightRange;
        public string XLightRange;
        public double AlkCaCO3;
        public string XAlkCaCO3;
        public double HardCaCO3;
        public string XHardCaCO3;
        public string SiteComment;
        public double SO4ConC;
        public string XSO4Conc;
        public double TotalDissSolids;
        public string XTotalDissSolids;
        public StreamTypes StreamType;
        public double Channel_Slope;
        public string XChannel_Slope;
        public double Max_Chan_Depth;
        public string XMax_Chan_Depth;
        public double SedDepth;
        public string XSedDepth;
        public double EnclWallArea;
        public string XEnclWallArea;
        public double MeanEvap;         // inches / year
        public string XMeanEvap;
        public bool UseEnteredManning;
        public double EnteredManning;
        public string XECoeffWater;
        public double PctRiffle;
        public string XPctRiffle;
        public double PctPool;
        public string XPctPool;
        public bool UseBathymetry;
        public double ts_clay;         // clay critical shear stress for scour [kg/m2]
        public string Xts_clay;
        public double ts_silt;        // silt critical shear stress for scour [kg/m2]
        public string Xts_silt;
        public double tdep_clay;        // clay critical shear stress for deposition [kg/m2]
        public string Xtdep_clay;
        public double tdep_silt;        // silt critical shear stress for deposition [kg/m2]
        public string Xtdep_silt;
        public double FallVel_clay;        // clay fall velocity, m/s
        public string XFallVel_clay;
        public double FallVel_silt;        // silt fall velocity, m/s
        public string XFallVel_silt;
        // ESTUARY ADDITIONS BELOW
        public double SiteWidth;
        public string XSiteWidth;         // m2
        public double amplitude1;
        public double k1;
        public string ConstRef1;        // s2
        public double amplitude2;
        public double k2;
        public string ConstRef2;        // n2
        public double amplitude3;
        public double k3;
        public string ConstRef3;        // k1
        public double amplitude4;
        public double k4;
        public string ConstRef4;        // o1
        public double amplitude5;
        public double k5;
        public string ConstRef5;        // SSA
        public double amplitude6;
        public double k6;
        public string ConstRef6;        // SA
        public double amplitude7;
        public double k7;
        public string ConstRef7;        // P1
        public double amplitude8;
        public double k8;
        public string ConstRef8;
        public double Min_Vol_Frac;
        public string XMin_Vol_Frac;
        public double WaterShedArea;
        public string XWaterShedArea;
        public bool EnterTotalLength;
        public double TotalLength;
        public string XTotalLength;
        public double ECoeffSED;
        public string XECoeffSED;
        public double ECoeffDOM;
        public string XECoeffDOM;
        public double ECoeffPOM;
        public string XECoeffPOM;
        public bool UseCovar;
        public double EnteredKReaer;
        public string XEnteredKReaer;
        public bool UsePhytoRetention;
        public double BasePercentEmbed;
        public string XBasePercentEmbed;
        public double Altitude;
        public string XAltitude;
        public double FractalD;
        public string XFractalD;
        public double FD_Refuge_Coeff;
        public string XFD_Refuge_Coeff;
        public double HalfSatOysterRefuge;
        public string XHalfSatOysterRefuge;
    } // end SiteRecord


    public struct ReminRecord
{
    public string RemRecName;
    public double DecayMax_Lab;
    public string XDecayMax_Lab;
    public double Q10_NotUsed;
    public string XQ10;
    public double TOpt;
    public string XTOpt;
    public double TMax;
    public string XTMax;
    public double TRef_NotUsed;
    public string XTRef;
    public double pHMin;
    public string XpHMin;
    public double pHMax;
    public string XpHMax;
    public double P2OrgLab;
    public double N2OrgLab;
    public string XP2OrgLab;
    public string XN2OrgLab;
    public double O2Biomass;
    public string XO2Biomass;
    public double O2N;
    public string XO2N;
    public double KSed;
    public string XKsed;
    public double PSedRelease_NotUsed;
    public string XPSedrelease;
    public double NSedRelease_NotUsed;
    public string XNSedRelease;
    public double DecayMax_Refr;    // g/g d
    public string XDecayMax_Refr;
    // ESTUARY ADDITIONS BELOW
    public double KSedTemp;
    public string XKSedTemp;
    public double KSedSalinity;
    public string XKSedSalinity;
    // ESTUARY ADDITIONS Above
    public double P2Org_Refr;
    public string XP2Org_Refr;
    public double N2Org_Refr;
    public string XN2Org_Refr;
    public double Wet2DryPRefr;
    public string XWet2DryPRefr;
    public double Wet2DryPLab;
    public string Xet2DryPLab;
    public double Wet2DrySRefr;
    public string XWet2DrySRefr;
    public double Wet2DrySLab;
    public string XWet2DrySLab;
    public double N2OrgDissLab;
    public string XN2OrgDissLab;
    public double P2OrgDissLab;
    public string XP2OrgDissLab;
    public double N2OrgDissRefr;
    public string XN2OrgDissRefr;
    public double P2OrgDissRefr;
    public string XP2OrgDissRefr;
    public double KD_P_Calcite;    // Sorption of P to CaCO3, L/Kg
    public string XKD_P_Calcite;
//    public double NotUsed;         // Was BOD5_CBODu
//    public string XNotUsed;      // XBOD5_CBODu
    public double KNitri;
    public string XKNitri;
    public double KDenitri_Bot;
    public string XKDenitri_Bot;
    public double KDenitri_Wat;
    public string XKDenitri_Wat;

} // end ReminRecord


    public struct MorphRecord          // Hold Results of Variable Morphometry

    {
        public double SegVolum;     // segment volume last good solution step
        public double InflowH2O;
        public double XSecArea;
        public double OOSDischFrac;   // frac of total discharge that moving out of the system (linked mode)
        public double OOSInflowFrac;
    } // end MorphRecord

    public enum SiteTypes
    {
        Pond,
        Stream,
        Reservr1D,
        Lake,
        Enclosure,
        Estuary,
        TribInput,
        Marine
    } // end SiteTypes


    [Serializable]
    [KnownType(typeof(TAQTSite))]
    [DataContract]
    public class TAQTSite
    {
        [DataMember] public SiteRecord Locale;
        [DataMember] public double Discharge;
        // cu m/day
        [DataMember] public ReminRecord Remin;
        [DataMember] public SiteTypes SiteType;
        [DataMember] public double MeanThick;
        [DataMember] public double P_Shape = 0;
        [DataMember] public double TotDischarge = 0;
        // was originally Q in older code
        [IgnoreDataMember] public MorphRecord Morph;  // variable morphometry results
        [DataMember] public double ICSurfArea = 0;

        // DeltaMorph procedures have been moved to TVOLUME.DELTAVOLUME found in STATE.INC
        // ----------------------------------------------------------------------------------------
        public double AreaFrac(double Z, double ZMax)
        {
            double result;
            // The AreaFrac function returns the fraction of surface area that is
            // at or above depth Z given ZMax and also P which defines the morphometry of the water body
            // For example, if your water body were a cone, when you made horizontal slices thorugh the cone,
            // looking from the top, you could conceivably see both the surface area and the water/sediment boundary
            // where the slice has been made.  This would look like a circle within a circle, or a donut.
            // Given a depth and a maximum depth, Areafrac calculates the fraction of the total surface
            // area that is the donut (epilimnion surf. area)  To get the donut hole, (1-AreaFrac) should be used in the code
            // RAP, 9/5/95 constrained to <= 1
            if (Z > ZMax)
            {
                // Z is greater than maximum depth
                result = 1.0;
            }
            else
            {
                result = (1.0 - P_Shape) * Z / ZMax + P_Shape * Math.Pow(Z / ZMax, 2);
            }
            // elliptic hyperboloid
            // RAP Fix Eqn. Signs 1-26-2001
            // fraction     unitless      m/m       unitless    m/m
            if (result < 0)
            {
                result = 0;
            }
            if (result > 1)
            {
                result = 1;
            }
            return result;
        }

        // areafrac
        // ----------------------------------------------------------------------------------------
        public double FracLittoral(double ZEuphotic, double Volume)
        {
            double result;
            // Fraction of Tot Area Available for PhotoSynth
            double FracLit;
            double LocalZMean;
            if (Locale.UseBathymetry)
            {
                FracLit = AreaFrac(ZEuphotic, Locale.ZMax);
                // 10-14-2010 Note that ZMax parameter pertains to both segments in event of stratification
                if (SiteType == SiteTypes.Enclosure)
                {
                    FracLit = FracLit * (Locale.SurfArea + Locale.EnclWallArea) / Locale.SurfArea;
                }
            }
            else
            {
                // don't use bathymetry, it is less relevant for rivers
                LocalZMean = Volume / Locale.SurfArea;
                // m
                FracLit = ZEuphotic / LocalZMean;
            // frac
            // m
            // m
            }
            if (FracLit > 1)
            {
                FracLit = 1;
            }
            if (FracLit < 0)
            {
                FracLit = 0;
            }
            result = FracLit;
            return result;
        }


        //Constructor  Init()
        public TAQTSite()
        {
            MeanThick = 0;
            P_Shape = 0;
            TotDischarge = 0;
            ICSurfArea = 0;
        }

        public double ManningCoeff()
        {

            // mannings coefficient
            if (Locale.UseEnteredManning)
            {
                return Locale.EnteredManning;
            }
            else if (Locale.StreamType == StreamTypes.concrete_channel)
            {
                return 0.020;
            }
            else if (Locale.StreamType == StreamTypes.dredged_channel)
            {
                return 0.030;
            }
            else
            {
                return 0.040; // natural stream
            }

        }    


        public void ChangeData(double ZM)
        {
            // RAP, 9/5/95 made P calculation universal - no reason to override for site type
            if (Locale.UseBathymetry)
            {
                P_Shape = 6.0 * ZM / Locale.ZMax - 3.0;
            }
            // Junge in Hrbacek '66
            // unitless                  m   m
            // P is constrained:    -1 <= P <= 1
            if ((P_Shape > 1.0))
            {
                P_Shape = 1.0;
            }
            else if ((P_Shape <  -1.0))
            {
                P_Shape =  -1.0;
            }

        }

        // ChangeData MUST be called when the underlying data record is changed
        // ---------------------------------------------------------------
        public double Discharge_Using_QBase()
        {
            double result;
            // THIS FUNCTION NOT UTILIZED BY THE LINKED VERSION
            double QBase;
            double IDepth;
            double Slope;
            double Width;
            // This function is used for streams when Tot_discharge = 0
            IDepth = Locale.ICZMean;
            Slope = Locale.Channel_Slope;
            if (Slope <= 0)
            {
                Slope = 0.0001;
            }
            // site is not a stream
            Width = Locale.SurfArea / (Locale.SiteLength * 1000);
            // m           // m2             // km         // m/km

            // -------------------------------------------------------------
            // BASE FLOW
            // Manning's equation for initial flow depth, rectangular channel
            // -------------------------------------------------------------
            QBase = (Math.Pow(IDepth, (5 / 3)) * Math.Sqrt(Slope) * Width) / ManningCoeff();
            // m3/s          // m                         // m/m    // m       // s/ m^1/3
            result = QBase * 86400;
            // m3/d  m3/s     s/d

            return result;
        }

        public double Conv_CBOD5_to_OM(double RefrPct)
        {
            double result;
            // Conversion Factor CBOD * Conv_BOD5 = OM
            double BOD5_CBODu;
            BOD5_CBODu = 1 / ((100 - RefrPct) / 100);
            result = BOD5_CBODu / Remin.O2Biomass;
            return result;
        }

    } // end TAQTSite


}

