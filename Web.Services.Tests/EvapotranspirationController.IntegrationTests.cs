using Xunit;
using Web.Services.Controllers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Web.Services.Tests
{
    public class EvapotranspirationControllerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        /// <summary>
        /// NLDAS request json string for testing a valid request
        /// </summary>
        const string nldasRequest =
            "{\"source\": \"nldas\", \"algorithm\": \"nldas\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// GLDAS request json string for testing a valid request
        /// </summary>
        const string gldasRequest =
            "{\"source\": \"gldas\", \"algorithm\": \"gldas\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Hamon NLDAS request json string for testing a valid request
        /// </summary>
        const string hamonNldasRequest =
            "{\"source\": \"nldas\", \"algorithm\": \"hamon\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Priestly-Taylor NLDAS request json string for testing a valid request
        /// </summary>
        const string priestlytaylorNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"priestlytaylor\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Granger-Gray NLDAS request json string for testing a valid request
        /// </summary>
        const string grangergrayNldasRequest =
            "{\"source\": \"nldas\", \"algorithm\": \"grangergray\", \"Albedo\": 0.23, \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penpan NLDAS request json string for testing a valid request
        /// </summary>
        const string penpanNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"penpan\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// McJannett NLDAS request json string for testing a valid request
        /// </summary>
        const string mcjannettNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"mcjannett\", \"Albedo\": 0.23, \"Lake Surface Area\": 0.005, " +
            "\"Average Lake Depth\": 0.2, \"AirTemperature\": {\"1\": 1.0, \"2\": 1.0, \"3\": 1.0, \"4\": 1.0, \"5\": 1.0, \"6\": 1.0, \"7\": 1.0, " +
            "\"8\": 1.0, \"9\": 1.0, \"10\": 1.0, \"11\": 1.0, \"12\": 1.0}, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Open Water NLDAS request json string for testing a valid request
        /// </summary>
        const string penmanopenwaterNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"penmanopenwater\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Daily NLDAS request json string for testing a valid request
        /// </summary>
        const string penmandailyNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"penmandaily\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Hourly NLDAS request json string for testing a valid request
        /// </summary>
        const string penmanhourlyNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"penmanhourly\", \"Albedo\": 0.23, \"Central Longitude\": 75.0, \"Sun Angle\": 17.2, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRAE NLDAS request json string for testing a valid request
        /// </summary>
        const string mortoncraeNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"mortoncrae\", \"Albedo\": 0.23, \"Emissivity\": 0.92, \"Model\": \"ETP\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRWE NLDAS request json string for testing a valid request
        /// </summary>
        const string mortoncrweNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"mortoncrwe\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Shuttleworth-Wallace NLDAS request json string for testing a valid request
        /// </summary>
        const string shuttleworthwallaceNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"shuttleworthwallace\", \"Albedo\": 0.23, \"Subsurface Resistance\": 500.0, " +
            "\"Stomatal Resistance\": 400.0, \"Leaf Width\": 0.02, \"Roughness Length\": 0.02, \"Vegetation Height\": 0.12, \"LeafAreaIndices\": {\"1\": 1.0, \"2\": 1.0, \"3\": 1.0, \"4\": 1.0, \"5\": 1.0, \"6\": 1.0, \"7\": 1.0, " +
            "\"8\": 1.0, \"9\": 1.0, \"10\": 1.0, \"11\": 1.0, \"12\": 1.0}, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// HSPF NLDAS request json string for testing a valid request
        /// </summary>
        const string hspfNldasRequest = "{ \"source\": \"nldas\", \"algorithm\": \"hspf\", \"Albedo\": 0.23, \"Central Longitude\": 75.0, " +
            "\"Sun Angle\": 17.2, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";


        ///-----------------GLDAS INTEGRATION TESTS---------------------------///

        /// <summary>
        /// Hamon gldas request json string for testing a valid request
        /// </summary>
        const string hamonGldasRequest =
            "{\"source\": \"gldas\", \"algorithm\": \"hamon\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Priestly-Taylor gldas request json string for testing a valid request
        /// </summary>
        const string priestlytaylorGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"priestlytaylor\",  \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\", \"Albedo\": 0.23}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Granger-Gray gldas request json string for testing a valid request
        /// </summary>
        const string grangergrayGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"grangergray\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\", \"Albedo\": 0.23}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penpan gldas request json string for testing a valid request
        /// </summary>
        const string penpanGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"penpan\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// McJannett gldas request json string for testing a valid request
        /// </summary>
        const string mcjannettGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"mcjannett\", \"Albedo\": 0.23, \"Lake Surface Area\": 0.005, " +
            "\"Average Lake Depth\": 0.2, \"AirTemperature\": {\"1\": 1.0, \"2\": 1.0, \"3\": 1.0, \"4\": 1.0, \"5\": 1.0, \"6\": 1.0, \"7\": 1.0, " +
            "\"8\": 1.0, \"9\": 1.0, \"10\": 1.0, \"11\": 1.0, \"12\": 1.0}, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Open Water gldas request json string for testing a valid request
        /// </summary>
        const string penmanopenwaterGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"penmanopenwater\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Daily gldas request json string for testing a valid request
        /// </summary>
        const string penmandailyGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"penmandaily\", \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Penman Hourly gldas request json string for testing a valid request
        /// </summary>
        const string penmanhourlyGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"penmanhourly\", \"Albedo\": 0.23, \"Central Longitude\": 75.0, \"Sun Angle\": 17.2, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRAE gldas request json string for testing a valid request
        /// </summary>
        const string mortoncraeGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"mortoncrae\", \"Albedo\": 0.23, \"Emissivity\": 0.92, \"Model\": \"ETP\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRWE gldas request json string for testing a valid request
        /// </summary>
        const string mortoncrweGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"mortoncrwe\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Shuttleworth-Wallace gldas request json string for testing a valid request
        /// </summary>
        const string shuttleworthwallaceGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"shuttleworthwallace\", \"Albedo\": 0.23, \"Subsurface Resistance\": 500.0, " +
            "\"Stomatal Resistance\": 400.0, \"Leaf Width\": 0.02, \"Roughness Length\": 0.02, \"Vegetation Height\": 0.12, \"LeafAreaIndices\": {\"1\": 1.0, \"2\": 1.0, \"3\": 1.0, \"4\": 1.0, \"5\": 1.0, \"6\": 1.0, \"7\": 1.0, " +
            "\"8\": 1.0, \"9\": 1.0, \"10\": 1.0, \"11\": 1.0, \"12\": 1.0}, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// HSPF gldas request json string for testing a valid request
        /// </summary>
        const string hspfGldasRequest = "{ \"source\": \"gldas\", \"algorithm\": \"hspf\", \"Albedo\": 0.23, \"Central Longitude\": 75.0, " +
            "\"Sun Angle\": 17.2, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\", " +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        ///-----------------DAYMET INTEGRATION TESTS---------------------------///

        /// <summary>
        /// Hamon daymet request json string for testing a valid request
        /// </summary>
        const string hamonDaymetRequest =
            "{\"source\": \"daymet\", \"algorithm\": \"hamon\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Priestly-Taylor daymet request json string for testing a valid request
        /// </summary>
        const string priestlytaylorDaymetRequest = "{ \"source\": \"daymet\", \"algorithm\": \"priestlytaylor\",  \"Albedo\": 0.23, \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", " +
            "\"endDate\": \"2009-12-31T00:00:00\", \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRAE daymet request json string for testing a valid request
        /// </summary>
        const string mortoncraeDaymetRequest = "{ \"source\": \"daymet\", \"algorithm\": \"mortoncrae\", \"Albedo\": 0.23, \"Emissivity\": 0.92, \"Model\": \"ETP\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Morton CRWE daymet request json string for testing a valid request
        /// </summary>
        const string mortoncrweDaymetRequest = "{ \"source\": \"daymet\", \"algorithm\": \"mortoncrwe\", \"dateTimeSpan\": { \"startDate\": \"2009-01-01T00:00:00\", \"endDate\": \"2009-12-31T00:00:00\"," +
            " \"dateTimeFormat\": \"yyyy-MM-dd HH\" }, \"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// NLDAS request json string for testing a valid request
        /// </summary>
        const string ncdcRequest =
            "{\"source\": \"ncdc\", \"algorithm\": \"hamon\", \"dateTimeSpan\": {\"startDate\": \"2009-01-01T00:00:00\",\"endDate\": \"2009-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"stationID\": \"GHCND:USW00013874\",\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public EvapotranspirationControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
            _client.Timeout = new System.TimeSpan(0, 5, 0); //Many tests were failing due to timeout errors
        }

        /// <summary>
        /// Evapotranspiration controller integration tests for each evapotranspiration data source. All tests should pass.
        /// </summary>
        /// <param name="evapoInputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(nldasRequest)]                      
        [InlineData(gldasRequest)]                      
        [InlineData(hamonNldasRequest)]                 
        [InlineData(priestlytaylorNldasRequest)]        
        [InlineData(grangergrayNldasRequest)]         
        [InlineData(penpanNldasRequest)]              
        [InlineData(mcjannettNldasRequest)]           
        [InlineData(penmanopenwaterNldasRequest)]     
        [InlineData(penmandailyNldasRequest)]         
        [InlineData(penmanhourlyNldasRequest)]        
        [InlineData(mortoncraeNldasRequest)]            
        [InlineData(mortoncrweNldasRequest)]            
        [InlineData(shuttleworthwallaceNldasRequest)] 
        [InlineData(hspfNldasRequest)]                
        [InlineData(hamonGldasRequest)]                
        [InlineData(priestlytaylorGldasRequest)]        
        [InlineData(grangergrayGldasRequest)]         
        [InlineData(penpanGldasRequest)]
        [InlineData(mcjannettGldasRequest)]
        [InlineData(penmanopenwaterGldasRequest)]     
        [InlineData(penmandailyGldasRequest)]         
        [InlineData(penmanhourlyGldasRequest)]        
        [InlineData(mortoncraeGldasRequest)]          
        [InlineData(mortoncrweGldasRequest)]          
        [InlineData(shuttleworthwallaceGldasRequest)] 
        [InlineData(hspfGldasRequest)]                
        [InlineData(hamonDaymetRequest)]                
        [InlineData(priestlytaylorDaymetRequest)]       
        [InlineData(mortoncraeDaymetRequest)]           
        [InlineData(mortoncrweDaymetRequest)]           
        [InlineData(ncdcRequest)]                      
        public async Task ValidRequests(string evapoInputString)
        {
            string endpoint = "api/hydrology/evapotranspiration";
            EvapotranspirationInput input = JsonConvert.DeserializeObject<EvapotranspirationInput>(evapoInputString);
            Debug.WriteLine("Integration Test: Evapotranspiration controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
            string json = JsonConvert.SerializeObject(input);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonConvert.DeserializeObject<TimeSeriesOutput>(result);
            Assert.Equal(365, resultObj.Data.Count);
        }
    }
}