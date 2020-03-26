using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
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
    public class WindControllerIntegrationTests
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
        /// GLDAS request json string for testing a valid request
        /// </summary>
        const string gldasRequest =
            "{\"source\": \"gldas\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// NCEI daily request json string for testing a valid request
        /// </summary>
        const string nceiRequest =
             "{\"source\": \"ncei\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\"," +
            "\"geometryMetadata\": {\"stationID\": \"GHCND:USW00013874\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"default\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";


        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public WindControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseSerilog().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Wind controller integration tests for each wind data source. All tests should pass.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(nldasRequest, 365)]
        [InlineData(gldasRequest, 366)]
        [InlineData(nceiRequest, 365)]
        public async Task ValidRequests(string inputString, int expected)
        {
            Thread.Sleep(5000);
            string endpoint = "api/meteorology/wind";
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            WindInput input = JsonSerializer.Deserialize(inputString, typeof(WindInput), options) as WindInput;
/*            WindInput test = JsonSerializer.Deserialize<WindInput>(inputString) as WindInput;*/
            Debug.WriteLine("Integration Test: Wind controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonSerializer.Deserialize<TimeSeriesOutput>(result, options);
            // New nldas/gldas temperature data contains statistics at the end (+3 required)
            //int expected = (input.Source.Equals("nldas") || input.Source.Equals("gldas")) ? 368 : 365;
            Assert.Equal(expected, resultObj.Data.Count);
        }
    }
}
