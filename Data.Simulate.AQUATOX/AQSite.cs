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

        public TParameter amplitude1 = new TParameter();
        public TParameter amplitude2 = new TParameter();
        public TParameter amplitude3 = new TParameter();
        public TParameter amplitude4 = new TParameter();
        public TParameter amplitude5 = new TParameter();
        public TParameter amplitude6 = new TParameter();
        public TParameter amplitude7 = new TParameter();
        public TParameter amplitude8 = new TParameter();
        public TParameter k1 = new TParameter();
        public TParameter k2 = new TParameter();
        public TParameter k3 = new TParameter();
        public TParameter k4 = new TParameter();
        public TParameter k5 = new TParameter();
        public TParameter k6 = new TParameter();
        public TParameter k7 = new TParameter();
        public TParameter k8 = new TParameter();


        public void Setup()
        {
            SiteName.Symbol = "Site Name"; SiteName.Name = "Site Name";
            SiteLength.Symbol = "SiteLength"; SiteLength.Name = "Max Length (or reach)"; SiteLength.Units = "km";
            Volume.Symbol = "Vol."; Volume.Name = "Volume (reference only)"; Volume.Units = "m3";
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

        public TParameter[] InputArray()
        {
            return new TParameter[] {new TSubheading("Site Parameters",""),
                            SiteName, SiteLength, SurfArea, Volume, Min_Vol_Frac, UseBathymetry, ICZMean, ZMax,
                            new TSubheading("Temperature, Light, Reaeration","Important if 'mean range' option is chosen for temperature or light, or oxygen calibration is required."),
                            TempMean,TempRange,Latitude,
                            Altitude,LightMean,LightRange,EnclWallArea,MeanEvap,UseCovar,EnteredKReaer,
                            new TSubheading("Water Clarity Parameters","Default may be used unless secchi depth requires calibration"),
                            ECoeffWater,ECoeffSed,ECoeffDOM,ECoeffPOM,BasePercentEmbed,
                            new TSubheading("Phytoplankton/Zooplankton Retention","Optional model for 0-D models of short stream reaches"),
                            UsePhytoRetention,EnterTotalLength,TotalLength,WaterShedArea,
                            new TSubheading("Refuge","Available refuge for prey items"), FractalD,FD_Refuge_Coeff,HalfSatOysterRefuge,
                            new TSubheading("Stream Parameters","Streams only: slope, manning coefficient, and riffle/run/pool"),
                            Channel_Slope,UseEnteredManning,EnteredManning,StreamType,PctRiffle,PctPool};
        }

     } // end SiteRecord


        public class ReminRecord
    {
        public TStringParam RemRecName = new TStringParam();
        public TParameter DecayMax_Lab = new TParameter();
        public TParameter DecayMax_Refr = new TParameter();
        public TParameter TOpt = new TParameter();
        public TParameter TMax = new TParameter();
        public TParameter pHMin = new TParameter();
        public TParameter pHMax = new TParameter();
        public TParameter KNitri = new TParameter();
        // public TParameter KDenitri_Bot = new TParameter();
        public TParameter KDenitri_Wat = new TParameter();
        public TParameter P2OrgLab = new TParameter();
        public TParameter N2OrgLab = new TParameter();
        public TParameter P2OrgRefr = new TParameter();
        public TParameter N2OrgRefr = new TParameter();
        public TParameter P2OrgDissLab = new TParameter();
        public TParameter N2OrgDissLab = new TParameter();
        public TParameter P2OrgDissRefr = new TParameter();
        public TParameter N2OrgDissRefr = new TParameter();
        public TParameter O2Biomass = new TParameter();
        // public TParameter BOD5_CBODu = new TParameter();
        public TParameter O2N = new TParameter();
        public TParameter KSed = new TParameter();
        public TParameter KSedTemp = new TParameter();
        public TParameter KSedSalinity = new TParameter();
        public TParameter PSedRelease = new TParameter();
        public TParameter NSedRelease = new TParameter();
        public TParameter Wet2DrySLab = new TParameter();
        public TParameter Wet2DrySRefr = new TParameter();
        public TParameter Wet2DryPLab = new TParameter();
        public TParameter Wet2DryPRefr = new TParameter();
        public TParameter KD_P_Calcite = new TParameter();

        public void Setup()
        {
            RemRecName.Symbol = "RemRecName"; RemRecName.Name = "Remineralization Record Name";
            DecayMax_Lab.Symbol = "Max. Degrdn Rate, labile"; DecayMax_Lab.Name = "Maximum decomposition rate"; DecayMax_Lab.Units = "g/g∙d";
            DecayMax_Refr.Symbol = "Max Degrdn Rate, Refrac"; DecayMax_Refr.Name = "Maximum colonization rate under ideal conditions"; DecayMax_Refr.Units = "g/g∙d";
            TOpt.Symbol = "Optimum Temperature"; TOpt.Name = "Optimum temperature for degredation to occur"; TOpt.Units = "°C";
            TMax.Symbol = "Maximum Temperature"; TMax.Name = "Maximum temperature at which degradation will occur"; TMax.Units = "°C";
            pHMin.Symbol = "Min pH for Degradation"; pHMin.Name = "Minimum ph below which limitation on biodegradation rate occurs."; pHMin.Units = "pH";
            pHMax.Symbol = "Max pH for Degradation"; pHMax.Name = "Maximum ph above which limitation on biodegradation occurs."; pHMax.Units = "pH";
            KNitri.Symbol = "KNitri, Max Rate of Nitrif."; KNitri.Name = "Maximum rate of nitrification"; KNitri.Units = "1/day";
//            KDenitri_Bot.Symbol = "KDenitri Bottom (max.)"; KDenitri_Bot.Name = "Maximum rate of denitrification at the sed/water interface"; KDenitri_Bot.Units = "1/day";
            KDenitri_Wat.Symbol = "KDenitri"; KDenitri_Wat.Name = "Maximum rate of denitrification"; KDenitri_Wat.Units = "1/day";
            P2OrgLab.Symbol = "P to Organics, Labile"; P2OrgLab.Name = "Ratio of phosphate to labile organic matter "; P2OrgLab.Units = "fraction dry weight";
            N2OrgLab.Symbol = "N to Organics, Labile"; N2OrgLab.Name = "Ratio of nitrate to labile organic matter "; N2OrgLab.Units = "fraction dry weight";
            P2OrgRefr.Symbol = "P to Organics, Refractory"; P2OrgRefr.Name = "Ratio of phosphate to refractory organic matter "; P2OrgRefr.Units = "fraction dry weight";
            N2OrgRefr.Symbol = "N to Organics, Refractory"; N2OrgRefr.Name = "Ratio of nitrate to refractory organic matter "; N2OrgRefr.Units = "fraction dry weight";
            P2OrgDissLab.Symbol = "P to Organics, Diss. Labile"; P2OrgDissLab.Name = "Ratio of phosphate to dissolved labile organic matter "; P2OrgDissLab.Units = "fraction dry weight";
            N2OrgDissLab.Symbol = "N to Organics, Diss. Labile"; N2OrgDissLab.Name = "Ratio of nitrate to dissolved labile organic matter "; N2OrgDissLab.Units = "fraction dry weight";
            P2OrgDissRefr.Symbol = "P to Organics, Diss. Refr."; P2OrgDissRefr.Name = "Ratio of phosphate to dissolved refractory organic matter "; P2OrgDissRefr.Units = "fraction dry weight";
            N2OrgDissRefr.Symbol = "N to Organics, Diss. Refr."; N2OrgDissRefr.Name = "Ratio of nitrate to dissolved refractory organic matter "; N2OrgDissRefr.Units = "fraction dry weight";
            O2Biomass.Symbol = "O2 : Biomass, Respiration"; O2Biomass.Name = "Ratio of oxygen to organic matter"; O2Biomass.Units = "unitless ratio";
//            BOD5_CBODu.Symbol = "CBODu to BOD5 conversion factor"; BOD5_CBODu.Name = "Not utilized as a parameter by the code."; BOD5_CBODu.Units = "unitless ratio";
            O2N.Symbol = "O2: N, Nitrification"; O2N.Name = "Ratio of oxygen to nitrogen"; O2N.Units = "unitless ratio";
            KSed.Symbol = "Detrital Sed Rate (KSed)"; KSed.Name = "Intrinsic sedimentation rate"; KSed.Units = "m/d";
            KSedTemp.Symbol = "Temperature of Obs. KSed"; KSedTemp.Name = "Reference temperature of water for calculating detrital sinking rate"; KSedTemp.Units = "deg. c";
            KSedSalinity.Symbol = "Salinity of Obs. KSed"; KSedSalinity.Name = "Reference salinity of water for calculating detrital sinking rate"; KSedSalinity.Units = "‰";
            PSedRelease.Symbol = "PO4, Anaerobic Sed."; PSedRelease.Name = "Not utilized as a parameter by the code."; PSedRelease.Units = "g/m2∙d";
            NSedRelease.Symbol = "NH4, Aerobic Sed."; NSedRelease.Name = "Not utilized as a parameter by the code."; NSedRelease.Units = "g/m2∙d";
            Wet2DrySLab.Symbol = "Wet to Dry Susp. Labile"; Wet2DrySLab.Name = "Wet weight to dry weight ratio for suspended labile detritus"; Wet2DrySLab.Units = "ratio";
            Wet2DrySRefr.Symbol = "Wet to Dry Susp. Refr"; Wet2DrySRefr.Name = "Wet weight to dry weight ratio for suspended refractory detritus"; Wet2DrySRefr.Units = "ratio";
            Wet2DryPLab.Symbol = "Wet to Dry Sed. Labile"; Wet2DryPLab.Name = "Wet weight to dry weight ratio for particulate labile detritus"; Wet2DryPLab.Units = "ratio";
            Wet2DryPRefr.Symbol = "Wet to Dry Sed. Refr."; Wet2DryPRefr.Name = "Wet weight to dry weight ratio for particulate refractory detritus"; Wet2DryPRefr.Units = "ratio";
            KD_P_Calcite.Symbol = "KD, P to CaCO3"; KD_P_Calcite.Name = "Partition coefficient for phosphorus to calcite"; KD_P_Calcite.Units = "L / kg";
        }

        public TParameter[] InputArray()
        {
            return new TParameter[] {new TSubheading("Organic Matter Parameters",""), RemRecName,
                new TSubheading("Degradation and Nitrification","Defaults may be used barring site-specific data"), DecayMax_Lab, DecayMax_Refr, TOpt, TMax, pHMin, pHMax, KNitri, KDenitri_Wat,
                new TSubheading("Stoichiometry","Defaults may be used; ratios of nutrients to organic matter"), P2OrgLab, N2OrgLab, P2OrgRefr, N2OrgRefr, P2OrgDissLab, N2OrgDissLab, P2OrgDissRefr,
                N2OrgDissRefr, O2Biomass, O2N, Wet2DrySLab, Wet2DrySRefr, Wet2DryPLab, Wet2DryPRefr,
                new TSubheading("Sedimentation Rate","Sinking of organic matter"), KSed, KSedTemp, KSedSalinity, PSedRelease, NSedRelease,
                new TSubheading("Calcite Precipitation",""), KD_P_Calcite};
        }

    } // end ReminRecord


        public class MorphRecord          // Hold Results of Variable Morphometry, not user input
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
            result = BOD5_CBODu / Remin.O2Biomass.Val;
            return result;
        }

    } // end TAQTSite


}
