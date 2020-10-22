using System;
using System.IO;
using Newtonsoft.Json;
using Globals;

namespace AQUATOX.AQSite

{

    public enum StreamTypes
    {
        concrete_channel,
        dredged_channel,
        natural_channel
    } // end StreamTypes


    public class SiteRecord
    {
        public TStringParam SiteName = new TStringParam();
        public TParameter SiteLength = new TParameter();
        public TParameter Volume = new TParameter();
        public TParameter SurfArea = new TParameter();
        public TParameter ICZMean = new TParameter();
        public TParameter ZMax = new TParameter();
        public TParameter TempMean = new TParameter();
        public TParameter TempRange = new TParameter();
        public TParameter Latitude = new TParameter();
        public TParameter Altitude = new TParameter();
        public TParameter LightMean = new TParameter();
        public TParameter LightRange = new TParameter();
        public TParameter EnclWallArea = new TParameter();
        public TParameter MeanEvap = new TParameter();
        public TParameter ECoeffWater = new TParameter();
        public TParameter ECoeffSed = new TParameter();
        public TParameter ECoeffDOM = new TParameter();
        public TParameter ECoeffPOM = new TParameter();
        public TParameter BasePercentEmbed = new TParameter();
        public TParameter Min_Vol_Frac = new TParameter();
        public TBoolParam UseCovar = new TBoolParam();
        public TParameter EnteredKReaer = new TParameter();

        public TBoolParam EnterTotalLength = new TBoolParam();
        public TParameter TotalLength = new TParameter();
        public TParameter WaterShedArea = new TParameter();
        public TParameter FractalD = new TParameter();
        public TParameter FD_Refuge_Coeff = new TParameter();
        public TParameter HalfSatOysterRefuge = new TParameter();

        public TParameter Channel_Slope = new TParameter();
        public TBoolParam UseBathymetry = new TBoolParam();
        public TDropDownParam StreamType = new TDropDownParam();
        public TBoolParam UseEnteredManning = new TBoolParam();
        public TParameter EnteredManning = new TParameter();
        public TParameter PctRiffle = new TParameter();
        public TParameter PctPool = new TParameter();
        public TBoolParam UsePhytoRetention = new TBoolParam();

        public void Setup()
        {
            SiteName.Symbol = "Site Name"; SiteName.Name = "Site Name";
            SiteLength.Symbol = "SiteLength"; SiteLength.Name = "Max Length (or reach)"; SiteLength.Units = "km";
            Volume.Symbol = "Vol."; Volume.Name = "Volume (reference only)"; SiteLength.Units = "m3";
            SurfArea.Symbol = "Surface Area"; SurfArea.Name = "Initial Condition Surface Area"; SurfArea.Units = "m2"; 
            ICZMean.Symbol = "Mean Depth"; ICZMean.Name = "Mean depth, (initial condition if dynamic mean depth is selected)"; ICZMean.Units = "M";
            ZMax.Symbol = "Maximum Depth"; ZMax.Name = "Maximum depth"; ZMax.Units = "M";
            TempMean.Symbol = "Ave. Temp. (epilimnetic or hypolimnetic) "; TempMean.Name = "Mean annual temperature of epilimnion (or hypolimnion) "; TempMean.Units = "°C";
            TempRange.Symbol = "Epilimnetic Temp. Range (or hypolimnetic)"; TempRange.Name = "Annual temperature range of epilimnion (or hypolimnion)"; TempRange.Units = "°C";
            Latitude.Symbol = "Latitude"; Latitude.Name = "Latitude"; Latitude.Units = "Deg, decimal";
            Altitude.Symbol = "Altitude (affects oxygen sat.)"; Altitude.Name = "Site specific altitude "; Altitude.Units = "m";
            LightMean.Symbol = "Average Light"; LightMean.Name = "Mean annual light intensity"; LightMean.Units = "Langleys/d";
            LightRange.Symbol = "Annual Light Range"; LightRange.Name = "Annual range in light intensity"; LightRange.Units = "Langleys/d";
            EnclWallArea.Symbol = "Enclosure Wall Area"; EnclWallArea.Name = "Area of experimental enclosures walls; only relevant to enclosure"; EnclWallArea.Units = "m2";
            MeanEvap.Symbol = "Mean Evaporation"; MeanEvap.Name = "Mean annual evaporation"; MeanEvap.Units = "inches / year";
            ECoeffWater.Symbol = "Extinct. Coeff Water"; ECoeffWater.Name = "Light extinction of wavelength 312.5 nm in pure water"; ECoeffWater.Units = "1/m";
            ECoeffSed.Symbol = "Extinct. Coeff Sediment"; ECoeffSed.Name = "Light extinction due to inorganic sediment in water"; ECoeffSed.Units = "1/\n(m·g/m3)";
            ECoeffDOM.Symbol = "Extinct. Coeff DOM"; ECoeffDOM.Name = "Light extinction due to dissolved organic matter in water"; ECoeffDOM.Units = "1/\n(m·g/m3)";
            ECoeffPOM.Symbol = "Extinct. Coeff POM"; ECoeffPOM.Name = "Light extinction due to particulate organic matter in water"; ECoeffPOM.Units = "1/\n(m·g/m3)";
            BasePercentEmbed.Symbol = "Baseline Percent Embeddedness"; BasePercentEmbed.Name = "Observed embeddedness that is used as an initial condition"; BasePercentEmbed.Units = "percent (0-100)";
            Min_Vol_Frac.Symbol = "Minimum Volume Frac."; Min_Vol_Frac.Name = "Fraction of initial condition that is the minimum volume of a site "; Min_Vol_Frac.Units = "frac. of Initial Condition";
            UseCovar.Name = "Auto Select Eqn. for reaeration";
            EnteredKReaer.Symbol = "Enter KReaer"; EnteredKReaer.Name = "Depth-averaged reaeration coefficient"; EnteredKReaer.Units = "1/d";
            TotalLength.Symbol = "Total Length"; TotalLength.Name = "Total river length for calculating phytoplankton retention"; TotalLength.Units = "km";
            WaterShedArea.Symbol = "Watershed Area"; WaterShedArea.Name = "Watershed area for estimating total river length (above)"; WaterShedArea.Units = "km2";
            FractalD.Symbol = "Fractal Dimension"; FractalD.Name = "Fractal dimension of marsh-water interface for the site."; FractalD.Units = "unitless";
            FD_Refuge_Coeff.Symbol = "Fractal D. Refuge Coefficient"; FD_Refuge_Coeff.Name = "Fractal dimension Refuge coefficient (-0.5 to 100 with the lowest values providing the strongest Refuge effect)."; FD_Refuge_Coeff.Units = "unitless";
            HalfSatOysterRefuge.Symbol = "Half Sat Oyster Refuge"; HalfSatOysterRefuge.Name = "Half-saturation constant for oysters in terms of providing refuge from feeding"; HalfSatOysterRefuge.Units = "g/m2";
            Channel_Slope.Symbol = "Channel Slope"; Channel_Slope.Name = "Slope of channel"; Channel_Slope.Units = "m/m";
            UseBathymetry.Name = "Use Bathymetry (instead of 'vertical' walls)";
            StreamType.Symbol = "Stream Type"; StreamType.Name = "Stream Type";  StreamType.ValList = new string[] { "Concrete Channel", "Dredged Channel", "Natural Channel", "N/A"}; 
            UseEnteredManning.Name = "Use Entered Manning Coefficient";
            EnteredManning.Symbol = "Mannings Coefficient"; EnteredManning.Name = "Manually entered Manning coefficient."; EnteredManning.Units = "s / m1/3";
            PctRiffle.Symbol = "Percent Riffle"; PctRiffle.Name = "Percent riffle in stream reach "; PctRiffle.Units = "%";
            PctPool.Symbol = "Percent Pool"; PctPool.Name = "Percent pool in stream reach"; PctPool.Units = "%";
            UsePhytoRetention.Symbol = "UsePhytoRetention"; UsePhytoRetention.Name = "Use phytoplankton / zooplankton retention?";
            EnterTotalLength.Symbol = "EnterTotalLength"; EnterTotalLength.Name = "If so, enter total length?";

    }

    } // end SiteRecord


        public class ReminRecord
    {
    public string RemRecName;
    public double DecayMax_Lab;
    public string XDecayMax_Lab;
    public double TOpt;
    public string XTOpt;
    public double TMax;
    public string XTMax;
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
    public double DecayMax_Refr;    // g/g d
    public string XDecayMax_Refr;
    public double KSedTemp;
    public string XKSedTemp;
    public double KSedSalinity;
    public string XKSedSalinity;
    public double P2Org_Refr;
    public string XP2Org_Refr;
    public double N2Org_Refr;
    public string XN2Org_Refr;
    public double Wet2DryPRefr;
    public string XWet2DryPRefr;
    public double Wet2DryPLab;
    public string XWet2DryPLab;
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


    public class MorphRecord          // Hold Results of Variable Morphometry

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


    public class TAQTSite
    {
        public SiteRecord Locale;
        [JsonIgnore] public double Discharge;     // cu m/day
        public ReminRecord Remin;
        public SiteTypes SiteType;
        [JsonIgnore] public double MeanThick;
        [JsonIgnore] public double P_Shape = 0;
        [JsonIgnore] public double TotDischarge = 0;    // was originally Q in older code
        [JsonIgnore] public MorphRecord Morph = new MorphRecord();  // variable morphometry results, NoSave
        [JsonIgnore] public double ICSurfArea = 0;

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
                result = (1.0 - P_Shape) * Z / ZMax + P_Shape * AQMath.Square(Z / ZMax);
            }
            // elliptic hyperboloid
            // RAP Fix Eqn. Signs 1-26-2001
            // fraction     unitless      m/m       unitless    m/m
            if (result < 0.0)
            {
                result = 0.0;
            }
            if (result > 1.0)
            {
                result = 1.0;
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
            if (Locale.UseBathymetry.Val)
            {
                FracLit = AreaFrac(ZEuphotic, Locale.ZMax.Val);
                // 10-14-2010 Note that ZMax parameter pertains to both segments in event of stratification
                if (SiteType == SiteTypes.Enclosure)
                {
                    FracLit = FracLit * (Locale.SurfArea.Val + Locale.EnclWallArea.Val) / Locale.SurfArea.Val;
                }
            }
            else
            {
                // don't use bathymetry, it is less relevant for rivers
                LocalZMean = Volume / Locale.SurfArea.Val;
                // m
                FracLit = ZEuphotic / LocalZMean;
            // frac
            // m
            // m
            }
            if (FracLit > 1.0)
            {
                FracLit = 1.0;
            }
            if (FracLit < 0.0)
            {
                FracLit = 0.0;
            }
            result = FracLit;
            return result;
        }


        //Constructor  Init()
        public TAQTSite()
        {
            MeanThick = 0.0;
            P_Shape = 0.0;
            TotDischarge = 0.0;
            ICSurfArea = 0.0;
        }

        public double ManningCoeff()
        {

            // mannings coefficient
            if (Locale.UseEnteredManning.Val)
            {
                return Locale.EnteredManning.Val;
            }
            else if (Locale.StreamType.Val == "Concrete Channel")
            {
                return 0.020;
            }
            else if (Locale.StreamType.Val == "Dredged Channel")
            {
                return 0.030;
            }
            else
            {
                return 0.040; // "Natural Stream"
            }

        }


        public void ChangeData(double ZM)
        {
            // RAP, 9/5/95 made P calculation universal - no reason to override for site type
            if (Locale.UseBathymetry.Val)
            {
                P_Shape = 6.0 * ZM / Locale.ZMax.Val - 3.0;
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
            IDepth = Locale.ICZMean.Val;
            Slope = Locale.Channel_Slope.Val;
            if (Slope <= 0)
            {
                Slope = 0.0001;
            }
            // site is not a stream
            Width = Locale.SurfArea.Val / (Locale.SiteLength.Val * 1000.0);
            // m           // m2             // km         // m/km

            // -------------------------------------------------------------
            // BASE FLOW
            // Manning's equation for initial flow depth, rectangular channel
            // -------------------------------------------------------------
            QBase = (Math.Pow(IDepth, (5.0 / 3.0)) * Math.Sqrt(Slope) * Width) / ManningCoeff();
            // m3/s          // m                         // m/m    // m       // s/ m^1/3
            result = QBase * 86400.0;
            // m3/d  m3/s     s/d

            return result;
        }

        public double Conv_CBOD5_to_OM(double RefrPct)
        {
            double result;
            // Conversion Factor CBOD * Conv_BOD5 = OM
            double BOD5_CBODu;
            BOD5_CBODu = 1.0 / ((100.0 - RefrPct) / 100.0);
            result = BOD5_CBODu / Remin.O2Biomass;
            return result;
        }

    } // end TAQTSite


}
