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
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public EvapotranspirationControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Evapotranspiration controller integration tests for each evapotranspiration data source. All tests should pass.
        /// </summary>
        /// <param name="evapoInputString"></param>
        /// <returns></returns>
        [Theory]
        [InlineData(nldasRequest)]
        [InlineData(gldasRequest)]
        public async Task ValidRequests(string evapoInputString)
        {
            string endpoint = "api/hydrology/evapotranspiration";
            EvapotranspirationInput input = JsonConvert.DeserializeObject<EvapotranspirationInput>(evapoInputString);
            Debug.WriteLine("Integration Test: Evapotranspiration controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonConvert.DeserializeObject<TimeSeriesOutput>(result);
            Assert.Equal(365, resultObj.Data.Count);
        }

    }
}
