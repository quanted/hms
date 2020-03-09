using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Evapotranspiration
{
    /// <summary>
    /// Evapotranspiration dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class Evapotranspiration : ITimeSeriesComponent
    {

        // -------------- Evapotranspiration Variables -------------- //

        // Evapotranspiration specific variables are listed here.
        /// <summary>
        /// REQUIRED: Algorithm used for Evapotranspiration.
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// REQUIRED: Albedo coefficient.
        /// </summary>
        public double Albedo { get; set; }

        /// <summary>
        /// REQUIRED: Central Longitude of Time Zone in degrees.
        /// </summary>
        public double CentralLongitude { get; set; }

        /// <summary>
        /// REQUIRED: Angle of the sun in degrees.
        /// </summary>
        public double SunAngle { get; set; }

        /// <summary>
        /// REQUIRED: The ability of a surface to emit radiant energy.
        /// </summary>
        public double Emissivity { get; set; }

        /// <summary>
        /// REQUIRED: Specifies if potential, actual, or wet environment evaporation are used.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// REQUIRED: Zenith Albedo coefficient.
        /// </summary>
        public double Zenith { get; set; }

        /// <summary>
        /// REQUIRED: Surface area of lake in square kilometers.
        /// </summary>
        public double LakeSurfaceArea { get; set; }

        /// <summary>
        /// REQUIRED: Average depth of lake in meters.
        /// </summary>
        public double LakeDepth { get; set; }

        /// <summary>
        /// REQUIRED: Subsurface Resistance.
        /// </summary>
        public double SubsurfaceResistance { get; set; }

        /// <summary>
        /// REQUIRED: Stomatal Resistance.
        /// </summary>
        public double StomatalResistance { get; set; }

        /// <summary>
        /// REQUIRED: Leaf Width in meters.
        /// </summary>
        public double LeafWidth { get; set; }

        /// <summary>
        /// REQUIRED: Roughness Length in meters.
        /// </summary>
        public double RoughnessLength { get; set; }

        /// <summary>
        /// REQUIRED: Vegetation Height in meters.
        /// </summary>
        public double VegetationHeight { get; set; }

        /// <summary>
        /// REQUIRED: Monthly leaf area indices.
        /// </summary>
        public Hashtable LeafAreaIndices { get; set; }

        /// <summary>
        /// REQUIRED: Monthly air temperature coefficients.
        /// </summary>
        public Hashtable AirTemperature { get; set; }

        /// <summary>
        /// OPTIONAL: Data file provided by the user.
        /// </summary>
        public string UserData { get; set; }

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }


        // -------------- Evapotranspiration Constructors -------------- //

        /// <summary>
        /// Default Evapotranspiration constructor
        /// </summary>
        public Evapotranspiration() { }


        // -------------- Evapotranspiration Functions -------------- //

        /// <summary>
        /// Get Evapotranspiration data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg)
        {
            errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0 && !this.Input.Source.Contains("ncdc")) //if (this.Input.Geometry.Timezone.Offset == 0) 
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            //TODO: Check Source and run specific subcomponent class for source
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            Elevation elev = new Elevation(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude);
            Utilities.Time offsets = new Utilities.Time();

            //this.Algorithm = (this.Input.Source != null) ? this.Input.Source : this.Algorithm;

            //Error checking and data validation
            if (this.Input.Source != "gldas" && this.Algorithm == "gldas")
            {
                errorMsg = "ERROR: GLDAS algorithm requires GLDAS data source.";
                return null;
            }
            if (this.Input.Source != "nldas" && this.Algorithm == "nldas")
            {
                errorMsg = "ERROR: NLDAS algorithm requires NLDAS data source.";
                return null;
            }
            if (this.Input.Source == "daymet" && this.Algorithm != "hamon" && this.Algorithm != "hargreaves" && this.Algorithm != "mortoncrae" && this.Algorithm != "mortoncrwe" && this.Algorithm != "priestlytaylor")
            {
                errorMsg = "ERROR: Algorithm is incompatible with Daymet data source.";
                return null;
            }
            if (this.Input.Source == "ncdc" && this.Algorithm != "hamon")
            {
                errorMsg = "ERROR: Algorithm is incompatible with NCDC data source.";
                return null;
            }
            if(this.Algorithm == "hargreaves" && this.Input.TemporalResolution == "hourly")
            {
                errorMsg = "ERROR: Algorithm does not support hourly aggregation.";
                return null;
            }

            switch (this.Algorithm)
            {
                case "nldas":
                    // NLDAS Evapotranspiration Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS Evapotranspiration Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "hamon":
                    // Hamon Evapotranspiration Data call
                    Hamon hamon = new Hamon();
                    hamon.Latitude = this.Input.Geometry.Point.Latitude;
                    hamon.Longitude = this.Input.Geometry.Point.Longitude;
                    this.Output = hamon.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "priestlytaylor":
                    // Priestly Taylor Evapotranspiration Data call
                    PriestleyTaylor priestleyTaylor = new PriestleyTaylor();
                    priestleyTaylor.Latitude = this.Input.Geometry.Point.Latitude;
                    priestleyTaylor.Longitude = this.Input.Geometry.Point.Longitude;
                    priestleyTaylor.Albedo = this.Albedo;
                    priestleyTaylor.Elevation = elev.getElevation(out errorMsg);
                    this.Output = priestleyTaylor.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "grangergray":
                    // Granger Gray Evapotranspiration Data call
                    var test = this.Input;
                    GrangerGray grangerGray = new GrangerGray();
                    grangerGray.Latitude = this.Input.Geometry.Point.Latitude;
                    grangerGray.Longitude = this.Input.Geometry.Point.Longitude;
                    grangerGray.Albedo = this.Albedo;
                    grangerGray.Elevation = elev.getElevation(out errorMsg);
                    this.Output = grangerGray.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penpan":
                    // Penpan Evapotranspiration Data call
                    Penpan penpan = new Penpan();
                    penpan.Latitude = this.Input.Geometry.Point.Latitude;
                    penpan.Longitude = this.Input.Geometry.Point.Longitude;
                    penpan.Albedo = this.Albedo;
                    penpan.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penpan.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mcjannett":
                    // McJannett Evapotranspiration Data call
                    McJannett mcjannett = new McJannett();
                    mcjannett.Latitude = this.Input.Geometry.Point.Latitude;
                    mcjannett.Longitude = this.Input.Geometry.Point.Longitude;
                    mcjannett.Albedo = this.Albedo;
                    mcjannett.Elevation = elev.getElevation(out errorMsg);
                    mcjannett.SurfaceArea = this.LakeSurfaceArea;
                    mcjannett.LakeDepth = this.LakeDepth;
                    mcjannett.airToWaterTempFactor[1] = Convert.ToDouble(this.AirTemperature["1"]);
                    mcjannett.airToWaterTempFactor[2] = Convert.ToDouble(this.AirTemperature["2"]);
                    mcjannett.airToWaterTempFactor[3] = Convert.ToDouble(this.AirTemperature["3"]);
                    mcjannett.airToWaterTempFactor[4] = Convert.ToDouble(this.AirTemperature["4"]);
                    mcjannett.airToWaterTempFactor[5] = Convert.ToDouble(this.AirTemperature["5"]);
                    mcjannett.airToWaterTempFactor[6] = Convert.ToDouble(this.AirTemperature["6"]);
                    mcjannett.airToWaterTempFactor[7] = Convert.ToDouble(this.AirTemperature["7"]);
                    mcjannett.airToWaterTempFactor[8] = Convert.ToDouble(this.AirTemperature["8"]);
                    mcjannett.airToWaterTempFactor[9] = Convert.ToDouble(this.AirTemperature["9"]);
                    mcjannett.airToWaterTempFactor[10] = Convert.ToDouble(this.AirTemperature["10"]);
                    mcjannett.airToWaterTempFactor[11] = Convert.ToDouble(this.AirTemperature["11"]);
                    mcjannett.airToWaterTempFactor[12] = Convert.ToDouble(this.AirTemperature["12"]);
                    this.Output = mcjannett.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmanopenwater":
                    // Penman Open Water Evapotranspiration Data call
                    PenmanOpenWater penmanOpenWater = new PenmanOpenWater();
                    penmanOpenWater.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanOpenWater.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanOpenWater.Albedo = this.Albedo;
                    penmanOpenWater.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penmanOpenWater.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmandaily":
                    // Penman Daily Evapotranspiration Data call
                    PenmanDaily penmanDaily = new PenmanDaily();
                    penmanDaily.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanDaily.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanDaily.Albedo = this.Albedo;
                    penmanDaily.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penmanDaily.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmanhourly":
                    // Penman Hourly Evapotranspiration Data call
                    PenmanHourly penmanHourly = new PenmanHourly();
                    penmanHourly.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanHourly.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanHourly.Albedo = this.Albedo;
                    penmanHourly.Elevation = elev.getElevation(out errorMsg);
                    penmanHourly.TimeZoneCentralLongitude = this.CentralLongitude;
                    penmanHourly.SunAngle = this.SunAngle;
                    this.Output = penmanHourly.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mortoncrae":
                    // Morton CRAE Evapotranspiration Data call
                    MortonCRAE mortonCRAE = new MortonCRAE();
                    mortonCRAE.Latitude = this.Input.Geometry.Point.Latitude;
                    mortonCRAE.Longitude = this.Input.Geometry.Point.Longitude;
                    mortonCRAE.Albedo = this.Albedo;
                    mortonCRAE.Elevation = elev.getElevation(out errorMsg);
                    mortonCRAE.Emissivity = this.Emissivity;
                    int model = Utilities.Utility.CalculateMortonMethod(this.Model);
                    double aprecip = 0.0;
                    mortonCRAE.AnnualPrecipitation = aprecip;
                    this.Output = mortonCRAE.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, model, out aprecip, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mortoncrwe":
                    // Morton CRWE Evapotranspiration Data call
                    MortonCRWE mortonCRWE = new MortonCRWE();
                    mortonCRWE.Latitude = this.Input.Geometry.Point.Latitude;
                    mortonCRWE.Longitude = this.Input.Geometry.Point.Longitude;
                    mortonCRWE.Albedo = this.Albedo;
                    mortonCRWE.Elevation = elev.getElevation(out errorMsg);
                    mortonCRWE.Emissivity = this.Emissivity;
                    mortonCRWE.Azenith = this.Zenith;
                    int model2 = Utilities.Utility.CalculateMortonMethod(this.Model);
                    double aprecip2 = 0.0;
                    mortonCRWE.AnnualPrecipitation = aprecip2;
                    this.Output = mortonCRWE.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, model2, out aprecip2, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "shuttleworthwallace":
                    // Shuttleworth Wallace Evapotranspiration Data call
                    ShuttleworthWallace shuttleworthWallace = new ShuttleworthWallace();
                    shuttleworthWallace.Latitude = this.Input.Geometry.Point.Latitude;
                    shuttleworthWallace.Longitude = this.Input.Geometry.Point.Longitude;
                    shuttleworthWallace.Albedo = this.Albedo;
                    shuttleworthWallace.Elevation = elev.getElevation(out errorMsg);
                    shuttleworthWallace.ResistanceSurfaceSoil = this.SubsurfaceResistance;
                    shuttleworthWallace.ResistanceStomatal = this.StomatalResistance;
                    shuttleworthWallace.WidthLeaf = this.LeafWidth;
                    shuttleworthWallace.GroundRoughnessLength = this.RoughnessLength;
                    shuttleworthWallace.VegetationHeight = this.VegetationHeight;
                    shuttleworthWallace.leafAreaIndex[1] = Convert.ToDouble(this.LeafAreaIndices["1"]);
                    shuttleworthWallace.leafAreaIndex[2] = Convert.ToDouble(this.LeafAreaIndices["2"]);
                    shuttleworthWallace.leafAreaIndex[3] = Convert.ToDouble(this.LeafAreaIndices["3"]);
                    shuttleworthWallace.leafAreaIndex[4] = Convert.ToDouble(this.LeafAreaIndices["4"]);
                    shuttleworthWallace.leafAreaIndex[5] = Convert.ToDouble(this.LeafAreaIndices["5"]);
                    shuttleworthWallace.leafAreaIndex[6] = Convert.ToDouble(this.LeafAreaIndices["6"]);
                    shuttleworthWallace.leafAreaIndex[7] = Convert.ToDouble(this.LeafAreaIndices["7"]);
                    shuttleworthWallace.leafAreaIndex[8] = Convert.ToDouble(this.LeafAreaIndices["8"]);
                    shuttleworthWallace.leafAreaIndex[9] = Convert.ToDouble(this.LeafAreaIndices["9"]);
                    shuttleworthWallace.leafAreaIndex[10] = Convert.ToDouble(this.LeafAreaIndices["10"]);
                    shuttleworthWallace.leafAreaIndex[11] = Convert.ToDouble(this.LeafAreaIndices["11"]);
                    shuttleworthWallace.leafAreaIndex[12] = Convert.ToDouble(this.LeafAreaIndices["12"]);
                    this.Output = shuttleworthWallace.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "hspf":
                    // HSPF Evapotranspiration Data call
                    HSPF hspf = new HSPF();
                    hspf.Latitude = this.Input.Geometry.Point.Latitude;
                    hspf.Longitude = this.Input.Geometry.Point.Longitude;
                    hspf.Albedo = this.Albedo;
                    hspf.Elevation = elev.getElevation(out errorMsg);
                    hspf.TimeZoneCentralLongitude = this.CentralLongitude;
                    hspf.SunAngle = this.SunAngle;
                    this.Output = hspf.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "hargreaves":
                    Hargreaves hargreaves = new Hargreaves();
                    hargreaves.Latitude = this.Input.Geometry.Point.Latitude;
                    hargreaves.Longitude = this.Input.Geometry.Point.Longitude;
                    this.Output = hargreaves.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Algorithm' for evapotranspiration was not found among available sources or is invalid.";
                    break;
            };

            // Adds Geometry metadata to the output metadata. NOT WORKING
            this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Algorithm + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Algorithm + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }

        /* <summary>
        /// Get Evapotranspiration data function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetHamonData(out string errorMsg)
        {
            errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0 && !this.Input.Source.Contains("ncdc")) //if (this.Input.Geometry.Timezone.Offset == 0) 
            {
                Utilities.Time tz = new Utilities.Time();
                this.Input.Geometry.Timezone = tz.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                if (errorMsg.Contains("ERROR")) { return null; }
            }

            //TODO: Check Source and run specific subcomponent class for source
            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();
            Elevation elev = new Elevation(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude);
            Utilities.Time offsets = new Utilities.Time();

            // Hamon Evapotranspiration Data call
            Hamon hamon = new Hamon();
            hamon.Latitude = this.Input.Geometry.Point.Latitude;
            hamon.Longitude = this.Input.Geometry.Point.Longitude;
            switch (this.Input.Source)
            {
                case "nldas":
                    // NLDAS Precipitation Data call
                    NLDAS nldas = new NLDAS();
                    this.Output = nldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "gldas":
                    // GLDAS Precipitation Data call
                    GLDAS gldas = new GLDAS();
                    this.Output = gldas.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "ncei":
                // NCDC Precipitation Data call
                //NCDC ncdc = new NCDC();
                //this.Output = ncdc.GetData(out errorMsg, this.Output, this.Input);
                //if (errorMsg.Contains("ERROR")) { return null; }
                //this.Input.Source = "ncei";
                //break;
                case "daymet":
                    // daymet Precipitation Data call
                    Daymet daymet = new Daymet();
                    this.Output = daymet.GetData(out errorMsg, this.Output, this.Input);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for precipitation was not found among available sources or is invalid.";
                    break;
            };
            this.Output = hamon.Compute(this.Input, this.Output, this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Adds Geometry metadata to the output metadata. NOT WORKING
            this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Algorithm + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Algorithm + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }*/

        /// <summary>
        /// Check evapotranspiration data endpoints.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> CheckEndpointStatus()
        {
            switch (this.Input.Source)
            {
                case "nldas":
                    return NLDAS.CheckStatus(this.Input);
                case "gldas":
                    return GLDAS.CheckStatus(this.Input);
                default:
                    return new Dictionary<string, string>() { { "status", "invalid source" } };
            }
        }

    }
}