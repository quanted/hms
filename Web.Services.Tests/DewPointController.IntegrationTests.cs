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
    public class DewPointControllerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        /// <summary>
        /// PRISM request json string for testing a valid request
        /// </summary>
        const string prismRequest =
            "{\"source\": \"prism\",\"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";

        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public DewPointControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseSerilog().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Dew Point controller integration tests for each temperature data source. All tests should pass.
        /// </summary>
        /// <param name="evapoInputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(prismRequest, 365)]
        public async Task ValidRequests(string inputString, int expected)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            Thread.Sleep(500);
            string endpoint = "api/meteorology/dewpoint";
            DewPointInput input = JsonSerializer.Deserialize<DewPointInput>(inputString, options);
            Debug.WriteLine("Integration Test: Dew Point controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
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
