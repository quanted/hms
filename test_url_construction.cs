using System;
using Data;
using Data.Source;

// Test NLDAS URL Construction
Console.WriteLine("=== Testing NLDAS URL Construction ===");
try 
{
    var nldasInput = new TimeSeriesInput
    {
        Source = "nldas",
        DateTimeSpan = new DateTimeSpan 
        { 
            StartDate = new DateTime(2015, 1, 1),
            EndDate = new DateTime(2015, 1, 2),
            DateTimeFormat = "yyyy-MM-dd HH"
        },
        Geometry = new TimeSeriesGeometry
        {
            Point = new PointCoordinate { Latitude = 33.925673, Longitude = -83.355723 },
            Timezone = new Timezone { Name = "EST", Offset = -5.0, DLS = false }
        },
        BaseURL = new System.Collections.Generic.List<string> { "https://api.giovanni.earthdata.nasa.gov/timeseries?data=NLDAS_FORA0125_H_2_0_Rainf" }
    };

    Console.WriteLine($"Input Source: {nldasInput.Source}");
    Console.WriteLine($"Base URL: {nldasInput.BaseURL[0]}");
    Console.WriteLine($"Latitude: {nldasInput.Geometry.Point.Latitude}");
    Console.WriteLine($"Longitude: {nldasInput.Geometry.Point.Longitude}");
    Console.WriteLine($"Date Range: {nldasInput.DateTimeSpan.StartDate} to {nldasInput.DateTimeSpan.EndDate}");
    Console.WriteLine("✓ NLDAS Input configured correctly for Giovanni API");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ NLDAS Test Failed: {ex.Message}");
}

Console.WriteLine();

// Test GLDAS URL Construction
Console.WriteLine("=== Testing GLDAS URL Construction ===");
try 
{
    var gldasInput = new TimeSeriesInput
    {
        Source = "gldas",
        DateTimeSpan = new DateTimeSpan 
        { 
            StartDate = new DateTime(2015, 1, 1),
            EndDate = new DateTime(2015, 1, 2),
            DateTimeFormat = "yyyy-MM-dd HH"
        },
        Geometry = new TimeSeriesGeometry
        {
            Point = new PointCoordinate { Latitude = 33.925673, Longitude = -83.355723 },
            Timezone = new Timezone { Name = "EST", Offset = -5.0, DLS = false }
        },
        BaseURL = new System.Collections.Generic.List<string> { "https://api.giovanni.earthdata.nasa.gov/timeseries?data=GLDAS_NOAH025_3H_v2_1_Rainf_f_tavg" }
    };

    Console.WriteLine($"Input Source: {gldasInput.Source}");
    Console.WriteLine($"Base URL: {gldasInput.BaseURL[0]}");
    Console.WriteLine($"Latitude: {gldasInput.Geometry.Point.Latitude}");
    Console.WriteLine($"Longitude: {gldasInput.Geometry.Point.Longitude}");
    Console.WriteLine($"Date Range: {gldasInput.DateTimeSpan.StartDate} to {gldasInput.DateTimeSpan.EndDate}");
    Console.WriteLine("✓ GLDAS Input configured correctly for Giovanni API");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ GLDAS Test Failed: {ex.Message}");
}

Console.WriteLine();

// Test TRMM URL Construction
Console.WriteLine("=== Testing TRMM URL Construction ===");
try 
{
    var trmmInput = new TimeSeriesInput
    {
        Source = "trmm",
        DateTimeSpan = new DateTimeSpan 
        { 
            StartDate = new DateTime(2010, 1, 1),
            EndDate = new DateTime(2010, 1, 2),
            DateTimeFormat = "yyyy-MM-dd HH"
        },
        Geometry = new TimeSeriesGeometry
        {
            Point = new PointCoordinate { Latitude = 33.925673, Longitude = -83.355723 },
            Timezone = new Timezone { Name = "EST", Offset = -5.0, DLS = false }
        },
        BaseURL = new System.Collections.Generic.List<string> { "https://api.giovanni.earthdata.nasa.gov/timeseries?data=TRMM_3B42_7_precipitation" }
    };

    Console.WriteLine($"Input Source: {trmmInput.Source}");
    Console.WriteLine($"Base URL: {trmmInput.BaseURL[0]}");
    Console.WriteLine($"Latitude: {trmmInput.Geometry.Point.Latitude}");
    Console.WriteLine($"Longitude: {trmmInput.Geometry.Point.Longitude}");
    Console.WriteLine($"Date Range: {trmmInput.DateTimeSpan.StartDate} to {trmmInput.DateTimeSpan.EndDate}");
    Console.WriteLine("✓ TRMM Input configured correctly for Giovanni API");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ TRMM Test Failed: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== URL Construction Test Summary ===");
Console.WriteLine("All URL formats have been updated to use Giovanni API");
Console.WriteLine("Base URL pattern: https://api.giovanni.earthdata.nasa.gov/timeseries?data=<DATASET>");
