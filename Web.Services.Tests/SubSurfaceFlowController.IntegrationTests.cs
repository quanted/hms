﻿using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Services.Controllers;
using Xunit;
using System.Text.Json;
using Serilog;

namespace Web.Services.Tests
{
    public class SubSurfaceFlowControllerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        /// <summary>
        /// NLDAS request json string for testing a valid request
        /// </summary>
        const string nldasRequest =
            "{\"source\": \"nldas\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// NLDAS request json string for testing a valid request
        /// </summary>
        const string nldas2Request =
            "{\"source\": \"nldas\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"monthly\",\"timeLocalized\": true," +
            "\"units\": \"imperial\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// GLDAS request json string for testing a valid request
        /// </summary>
        const string gldasRequest =
            "{\"source\": \"gldas\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// GLDAS request json string for testing a valid request
        /// </summary>
        const string gldas2Request =
            "{\"source\": \"gldas\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"monthly\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Curvenumber request json string for testing a valid request
        /// </summary>
        const string curvenumberRequest =
            "{\"source\": \"curvenumber\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"comid\": 9203093," +
            "\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Curvenumber request json string for testing a valid request
        /// </summary>
        const string curvenumber2Request =
            "{\"source\": \"curvenumber\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"monthly\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public SubSurfaceFlowControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// SubSurfaceFlow controller integration tests for each subsurfaceflow data source. All tests should pass.
        /// </summary>
        /// <param name="evapoInputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(nldasRequest, 365)]
        [InlineData(nldas2Request, 12)]
        [InlineData(gldasRequest, 365)]
        [InlineData(gldas2Request, 13)]
        [InlineData(curvenumberRequest, 365)]
        [InlineData(curvenumber2Request, 12)]
        public async Task ValidRequests(string inputString, int expected)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            Thread.Sleep(500);
            string endpoint = "api/hydrology/subsurfaceflow";
            SubSurfaceFlowInput input = JsonSerializer.Deserialize<SubSurfaceFlowInput>(inputString, options);
            Debug.WriteLine("Integration Test: SubsurfaceFlow controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(JsonSerializer.Serialize(input, options), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonSerializer.Deserialize<TimeSeriesOutput>(result, options);
            Assert.Equal(expected, resultObj.Data.Count);
        }

    }
}
