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
    public class PrecipCompareControllerIntegrationTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        /// <summary>
        /// GHCND daily one year request json string for testing a valid request
        /// </summary>
        const string dailyOneYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"daily\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND monthly one year request json string for testing a valid request
        /// </summary>
        const string monthlyOneYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"monthly\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND annual one year request json string for testing a valid request
        /// </summary>
        const string annualOneYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"annual\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND extreme event one year request json string for testing a valid request
        /// </summary>
        const string extremeOneYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"extreme_5\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND daily two year request json string for testing a valid request
        /// </summary>
        const string dailyTwoYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"daily\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND monthly two year request json string for testing a valid request
        /// </summary>
        const string monthlyTwoYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"monthly\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND annual two year request json string for testing a valid request
        /// </summary>
        const string annualTwoYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"annual\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND extreme event two year request json string for testing a valid request
        /// </summary>
        const string extremeTwoYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"extreme_5\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP daily one year request json string for testing a valid request
        /// </summary>
        const string dailyOneYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"daily\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP monthly one year request json string for testing a valid request
        /// </summary>
        const string monthlyOneYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"monthly\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP annual one year request json string for testing a valid request
        /// </summary>
        const string annualOneYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"annual\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP extreme event one year request json string for testing a valid request
        /// </summary>
        const string extremeOneYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"extreme_5\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP daily two year request json string for testing a valid request
        /// </summary>
        const string dailyTwoYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"daily\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP monthly two year request json string for testing a valid request
        /// </summary>
        const string monthlyTwoYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"monthly\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP annual two year request json string for testing a valid request
        /// </summary>
        const string annualTwoYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"annual\", \"timeLocalized\": true}";

        /// <summary>
        /// COOP extreme event two year request json string for testing a valid request
        /// </summary>
        const string extremeTwoYearMissing =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2011-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"COOP:090451\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"extreme_5\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND daily 7 year request json string for testing a valid request
        /// </summary>
        const string dailySevenYear =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": false, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2000-01-01T00:00:00\", \"endDate\": \"2006-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"annual\", \"timeLocalized\": true}";

        /// <summary>
        /// GHCND daily one year spatially weighted request json string for testing a valid request
        /// </summary>
        const string dailyOneYearWeighted =
            "{ \"dataset\": \"Precipitation\", \"sourceList\": [ \"nldas\", \"gldas\", \"daymet\", \"prism\" ], \"source\": \"compare\", \"Weighted\": true, " +
            "\"ExtremeDaily\": 5, \"ExtremeTotal\": 10, \"dateTimeSpan\": { \"startDate\": \"2010-01-01T00:00:00\", \"endDate\": \"2010-12-31T00:00:00\" }, " +
            "\"geometry\": { \"comID\": 6411690, \"StationID\": \"GHCND:USW00013874\", \"geometryMetadata\": {}, \"timezone\": { \"name\": \"EST\", \"offset\": -5, " +
            "\"dls\": true } }, \"temporalResolution\": \"daily\", \"timeLocalized\": true}";

        /// <summary>
        /// Integration test constructor creates test server and test client.
        /// </summary>
        public PrecipCompareControllerIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Precipitation compare controller integration tests for each precip data source and temporal aggregation. All tests should pass.
        /// Test Exception: precip test for prism may fail due to firewall restrictions.
        /// All tests are searching for NCDC, NLDAS, GLDAS, Daymet, and PRISM.
        /// All will use comID: 6411690 and StationID: GHCND:USW00013874 OR COOP:090451 as well as 5Day>=10mm and Daily>=5mm
        /// </summary>
        /// <param name="precipInputString"></param>
        /// <returns></returns>
        [Trait("Priority", "1")]
        [Theory]
        [InlineData(dailyOneYear, 365)]
        [InlineData(monthlyOneYear, 12)]
        [InlineData(annualOneYear, 0)]
        [InlineData(extremeOneYear, 198)]
        [InlineData(dailyTwoYear, 730)]
        [InlineData(monthlyTwoYear, 24)]
        [InlineData(annualTwoYear, 2)]
        [InlineData(extremeTwoYear, 400)]
        /*[InlineData(dailyOneYearMissing, 366)]
        [InlineData(monthlyOneYearMissing, 12)]
        [InlineData(annualOneYearMissing, 0)]
        [InlineData(extremeOneYearMissing, 206)]
        [InlineData(dailyTwoYearMissing, 730)]
        [InlineData(monthlyTwoYearMissing, 24)]
        [InlineData(annualTwoYearMissing, 2)]
        [InlineData(extremeTwoYearMissing, 419)]*/
        [InlineData(dailySevenYear, 7)]
        //[InlineData(dailyOneYearWeighted, 366)]
        public async Task ValidRequests(string precipInputString, int expected)
        {
            string endpoint = "api/workflow/precipitation/precip_compare";
            PrecipitationCompareInput input = JsonConvert.DeserializeObject<PrecipitationCompareInput>(precipInputString);
            Debug.WriteLine("Integration Test: Precipitation Compare controller; Endpoint: " + endpoint + "; Data source: " + input.Source);
            var response = await _client.PostAsync(
                endpoint,
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            TimeSeriesOutput resultObj = JsonConvert.DeserializeObject<TimeSeriesOutput>(result);
            Assert.Equal(expected, resultObj.Data.Count);
        }
    }
}
