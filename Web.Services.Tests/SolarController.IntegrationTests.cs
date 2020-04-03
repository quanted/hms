using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Data;
using System.Diagnostics;
using Xunit;
using Web.Services.Models;
using System.Text.Json;
using Serilog;

namespace Web.Services.Tests
{
    public class SolarControllerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        /// <summary>
        /// request json string for testing a valid request
        /// </summary>
        const string requestString =
            "{\"model\": \"year\",\"localTime\": \"12:00:00\", \"dateTimeSpan\": {\"startDate\": \"2015-01-01T00:00:00\",\"endDate\": \"2015-12-31T00:00:00\"," +
            "\"dateTimeFormat\": \"yyyy-MM-dd HH\"},\"geometry\": {\"description\": \"EPA Athens Office\",\"point\": " +
            "{\"latitude\": 33.925673,\"longitude\": -83.355723},\"geometryMetadata\": {\"City\": \"Athens\",\"State\": \"Georgia\",\"Country\": \"United States\"}," +
            "\"timezone\": {\"name\": \"EST\",\"offset\": -5,\"dls\": false}},\"dataValueFormat\": \"E3\",\"temporalResolution\": \"daily\",\"timeLocalized\": true," +
            "\"units\": \"default\",\"outputFormat\": \"json\"}";


        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public SolarControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseSerilog().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Solar controller integration tests for each meteorology data simulation. All tests should pass.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(requestString)]
        public async Task MeteorologyValidRequests(string inputString)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            string endpoint = "api/meteorology/solar";
            SolarCalculatorInput input = JsonSerializer.Deserialize<SolarCalculatorInput>(inputString, options);
            Debug.WriteLine("Integration Test: Solar controller; Endpoint: " + endpoint + "; Data source: " + input.Model);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(JsonSerializer.Serialize(input, options), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonSerializer.Deserialize<TimeSeriesOutput>(result, options);
            Assert.Equal(365, resultObj.Data.Count);
        }

        // Test for api/water-quality/solar/run (GET)
        // Test for api/water-quality/solar/run (POST)
        // Test for api/water-quality/solar/inputs (GET)
        // Test for api/water-quality/solar/inputs/metadata (GET)

    }
}
