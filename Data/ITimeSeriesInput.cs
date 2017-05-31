using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    /// <summary>
    /// Interface for timeseries inputs.
    /// </summary>
    public interface ITimeSeriesInput
    {
        /// <summary>
        /// Data source for the timeseries.
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// Gets date object for time series.
        /// </summary>
        ITimeSeriesTimeSpan TimeSpan { get; set; }

        /// <summary>
        /// Gets geometry object for time series.
        /// </summary>
        ITimeSeriesGeometry Geometry { get; set; }

        /// <summary>
        /// The temporal resolution of the time series to be returned.
        /// </summary>
        string TemporalResolution { get; set; }

        /// <summary>
        /// Indicates whether the output time values are used for the timezone of the input geometry.
        /// </summary>
        bool TimeLocalized { get; set; }

        /// <summary>
        /// Unit system applied to output.
        /// </summary>
        string Units { get; set; }

        /// <summary>
        /// Indicates the type of output for the given spatial data.
        /// Default: Return all datapoints with no aggregation.
        /// Options: 1. Return a subset of all datapoints (for handling large input geometries)
        ///          2. Type of spatial aggregation (total of area, average of area... )
        /// </summary>
        string SpatialAggregation { get; set; }

        /// <summary>
        /// Specifies output format type.
        /// JSON, XML... other.
        /// </summary>
        string OutputFormat { get; set; }
    }


    /// <summary>
    /// Date interface for time series components.
    /// </summary>
    public interface ITimeSeriesTimeSpan
    {
        /// <summary>
        /// Start date of the time series.
        /// </summary>
        DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the time series.
        /// </summary>
        DateTime EndDate { get; set; }
    }


    /// <summary>
    /// Timezone information for input geometry.
    /// </summary>
    public interface ITimezone
    {
        /// <summary>
        /// Timezone name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Timezone offset from GMT.
        /// </summary>
        double Offset { get; set; }

        /// <summary>
        /// Indicates if day light savings is active or not.
        /// </summary>
        bool DLS { get; set; }
    }

    /// <summary>
    /// Geometry interface for time series components.
    /// </summary>
    public interface ITimeSeriesGeometry
    {
        /// <summary>
        /// Specifies the kind of input geometry is being used to get the time series.
        /// Valid types: coordinate, geojson, huc, generic location (can be used for NCDC).
        /// </summary>
        int Location { get; set; }

        /// <summary>
        /// Lat/lon point for when a coordinates are used as the geometry type.
        /// </summary>
        ICoordinate Point { get; }

        /// <summary>
        /// HUC object.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Timezone information for the input geometry.
        /// </summary>
        ITimezone Timezone { get; set; }        
    }

    /// <summary>
    /// Point geometry object interface.
    /// </summary>
    public interface ICoordinate
    {
        /// <summary>
        /// Latitude value of point geometry.
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        /// Longitude value of point geometry.
        /// </summary>
        double Longitude { get; set; }
    }

    /// <summary>
    /// HUC geometry object interface.
    /// </summary>
    public interface IHUC
    {
        /// <summary>
        /// Indicates the specific HUC level of this HUC object.
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Name of HUC object.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// ID number of HUC object.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// HUC geometry string, as geojson, of HUC object.
        /// </summary>
        string Geometry { get; set; }

        /// <summary>
        /// Centroid of HUC
        /// </summary>
        ICoordinate Centroid { get; set; }
    }

    /// <summary>
    /// Location object interface.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// ID for the input location.
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Coordinates for the input location.
        /// </summary>
        ICoordinate Point { get; set; }
    }
}
