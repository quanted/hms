using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Evapotranspiration
{
    /// <summary>
    /// Evapotranspiration dataset class: Implements ITimeSeriesComponent
    /// </summary>
    public class Evapotranspiration : ITimeSeriesComponent
    {

        // -------------- Evapotranspiration Variables -------------- //

        // Evapotranspiration specific variables are listed here.


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
            if (this.Input.Geometry.Timezone.Offset == 0)
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
            switch (this.Input.Source)
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
                    PointCoordinate latlong = new PointCoordinate();
                    this.Output = hamon.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "priestlytaylor":
                    // Priestly Taylor Evapotranspiration Data call
                    PriestleyTaylor priestleyTaylor = new PriestleyTaylor();
                    priestleyTaylor.Latitude = this.Input.Geometry.Point.Latitude;
                    priestleyTaylor.Longitude = this.Input.Geometry.Point.Longitude;
                    priestleyTaylor.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    priestleyTaylor.Elevation = elev.getElevation(out errorMsg);
                    this.Output = priestleyTaylor.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "grangergray":
                    // Granger Gray Evapotranspiration Data call
                    var test = this.Input;
                    GrangerGray grangerGray = new GrangerGray();
                    grangerGray.Latitude = this.Input.Geometry.Point.Latitude;
                    grangerGray.Longitude = this.Input.Geometry.Point.Longitude;
                    grangerGray.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    grangerGray.Elevation = elev.getElevation(out errorMsg);
                    this.Output = grangerGray.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penpan":
                    // Penpan Evapotranspiration Data call
                    Penpan penpan = new Penpan();
                    penpan.Latitude = this.Input.Geometry.Point.Latitude;
                    penpan.Longitude = this.Input.Geometry.Point.Longitude;
                    penpan.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    penpan.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penpan.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mcjannett":
                    // McJannett Evapotranspiration Data call
                    McJannett mcjannett = new McJannett();
                    mcjannett.Latitude = this.Input.Geometry.Point.Latitude;
                    mcjannett.Longitude = this.Input.Geometry.Point.Longitude;
                    mcjannett.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    mcjannett.Elevation = elev.getElevation(out errorMsg);
                    mcjannett.SurfaceArea = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Lake Surface Area"]);
                    mcjannett.LakeDepth = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Average Lake Depth"]);
                    mcjannett.airToWaterTempFactor[1] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JanuaryTemp"]);
                    mcjannett.airToWaterTempFactor[2] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["FebruaryTemp"]);
                    mcjannett.airToWaterTempFactor[3] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["MarchTemp"]);
                    mcjannett.airToWaterTempFactor[4] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["AprilTemp"]);
                    mcjannett.airToWaterTempFactor[5] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["MayTemp"]);
                    mcjannett.airToWaterTempFactor[6] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JuneTemp"]);
                    mcjannett.airToWaterTempFactor[7] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JulyTemp"]);
                    mcjannett.airToWaterTempFactor[8] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["AugustTemp"]);
                    mcjannett.airToWaterTempFactor[9] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["SeptemberTemp"]);
                    mcjannett.airToWaterTempFactor[10] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["OctoberTemp"]);
                    mcjannett.airToWaterTempFactor[11] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["NovemberTemp"]);
                    mcjannett.airToWaterTempFactor[12] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["DecemberTemp"]);
                    this.Output = mcjannett.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmanopenwater":
                    // Penman Open Water Evapotranspiration Data call
                    PenmanOpenWater penmanOpenWater = new PenmanOpenWater();
                    penmanOpenWater.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanOpenWater.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanOpenWater.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    penmanOpenWater.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penmanOpenWater.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmandaily":
                    // Penman Daily Evapotranspiration Data call
                    PenmanDaily penmanDaily = new PenmanDaily();
                    penmanDaily.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanDaily.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanDaily.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    penmanDaily.Elevation = elev.getElevation(out errorMsg);
                    this.Output = penmanDaily.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "penmanhourly":
                    // Penman Hourly Evapotranspiration Data call
                    PenmanHourly penmanHourly = new PenmanHourly();
                    penmanHourly.Latitude = this.Input.Geometry.Point.Latitude;
                    penmanHourly.Longitude = this.Input.Geometry.Point.Longitude;
                    penmanHourly.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    penmanHourly.Elevation = elev.getElevation(out errorMsg);
                    penmanHourly.TimeZoneCentralLongitude = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Central Longitude"]);
                    penmanHourly.SunAngle = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Sun Angle"]);
                    this.Output = penmanHourly.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mortoncrae":
                    // Morton CRAE Evapotranspiration Data call
                    MortonCRAE mortonCRAE = new MortonCRAE();
                    mortonCRAE.Latitude = this.Input.Geometry.Point.Latitude;
                    mortonCRAE.Longitude = this.Input.Geometry.Point.Longitude;
                    mortonCRAE.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    mortonCRAE.Elevation = elev.getElevation(out errorMsg);
                    mortonCRAE.Emissivity = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Emissivity"]);
                    int model = Utilities.Utility.CalculateMortonMethod(this.Input.Geometry.GeometryMetadata["Model"]);
                    double aprecip = 0.0;
                    mortonCRAE.AnnualPrecipitation = aprecip;
                    this.Output = mortonCRAE.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, model, out aprecip, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "mortoncrwe":
                    // Morton CRWE Evapotranspiration Data call
                    MortonCRWE mortonCRWE = new MortonCRWE();
                    mortonCRWE.Latitude = this.Input.Geometry.Point.Latitude;
                    mortonCRWE.Longitude = this.Input.Geometry.Point.Longitude;
                    mortonCRWE.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    mortonCRWE.Elevation = elev.getElevation(out errorMsg);
                    mortonCRWE.Emissivity = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Emissivity"]);
                    mortonCRWE.Azenith = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Zenith"]);
                    int model2 = Utilities.Utility.CalculateMortonMethod(this.Input.Geometry.GeometryMetadata["Model"]);
                    double aprecip2 = 0.0;
                    mortonCRWE.AnnualPrecipitation = aprecip2;
                    this.Output = mortonCRWE.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, model2, out aprecip2, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "shuttleworthwallace":
                    // Shuttleworth Wallace Evapotranspiration Data call
                    ShuttleworthWallace shuttleworthWallace = new ShuttleworthWallace();
                    shuttleworthWallace.Latitude = this.Input.Geometry.Point.Latitude;
                    shuttleworthWallace.Longitude = this.Input.Geometry.Point.Longitude;
                    shuttleworthWallace.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    shuttleworthWallace.Elevation = elev.getElevation(out errorMsg);
                    shuttleworthWallace.ResistanceSurfaceSoil = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Subsurface Resistance"]);
                    shuttleworthWallace.ResistanceStomatal = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Stomatal Resistance"]);
                    shuttleworthWallace.WidthLeaf = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Leaf Width"]);
                    shuttleworthWallace.GroundRoughnessLength = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Roughness Length"]);
                    shuttleworthWallace.VegetationHeight = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Vegetation Height"]);
                    shuttleworthWallace.leafAreaIndex[1] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JanuaryIndex"]);
                    shuttleworthWallace.leafAreaIndex[2] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["FebruaryIndex"]);
                    shuttleworthWallace.leafAreaIndex[3] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["MarchIndex"]);
                    shuttleworthWallace.leafAreaIndex[4] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["AprilIndex"]);
                    shuttleworthWallace.leafAreaIndex[5] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["MayIndex"]);
                    shuttleworthWallace.leafAreaIndex[6] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JuneIndex"]);
                    shuttleworthWallace.leafAreaIndex[7] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["JulyIndex"]);
                    shuttleworthWallace.leafAreaIndex[8] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["AugustIndex"]);
                    shuttleworthWallace.leafAreaIndex[9] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["SeptemberIndex"]);
                    shuttleworthWallace.leafAreaIndex[10] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["OctoberIndex"]);
                    shuttleworthWallace.leafAreaIndex[11] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["NovemberIndex"]);
                    shuttleworthWallace.leafAreaIndex[12] = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["DecemberIndex"]);
                    this.Output = shuttleworthWallace.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "hspf":
                    // HSPF Evapotranspiration Data call
                    HSPF hspf = new HSPF();
                    hspf.Latitude = this.Input.Geometry.Point.Latitude;
                    hspf.Longitude = this.Input.Geometry.Point.Longitude;
                    hspf.Albedo = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Albedo"]);
                    hspf.Elevation = elev.getElevation(out errorMsg);
                    hspf.TimeZoneCentralLongitude = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Central Longitude"]);
                    hspf.SunAngle = Convert.ToDouble(this.Input.Geometry.GeometryMetadata["Sun Angle"]);
                    this.Output = hspf.Compute(this.Input.Geometry.Point.Latitude, this.Input.Geometry.Point.Longitude, this.Input.DateTimeSpan.StartDate.ToString(), this.Input.DateTimeSpan.EndDate.ToString(), (int)this.Input.Geometry.Timezone.Offset, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                    errorMsg = "ERROR: 'Source' for evapotranspiration was not found among available sources or is invalid.";
                    break;
            };

            // Adds Geometry metadata to the output metadata. NOT WORKING
            this.Output.Metadata.Concat(this.Input.Geometry.GeometryMetadata);

            // Adds Timezone info to metadata
            this.Output.Metadata.Add(this.Input.Source + "_timeZone", this.Input.Geometry.Timezone.Name);
            this.Output.Metadata.Add(this.Input.Source + "_tz_offset", this.Input.Geometry.Timezone.Offset.ToString());

            //TODO: Add output format control

            return this.Output;
        }

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