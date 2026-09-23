using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class PrecipitationApiTest
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://localhost:60050";
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        Console.WriteLine("=== HMS Precipitation API Test Suite ===");
        Console.WriteLine($"Target: {baseUrl}");
        Console.WriteLine();

        // Test 1: NLDAS Precipitation
        await TestPrecipitation(client, baseUrl, "nldas", "NLDAS");

        Console.WriteLine();

        // Test 2: GLDAS Precipitation  
        await TestPrecipitation(client, baseUrl, "gldas", "GLDAS");

        Console.WriteLine();

        // Test 3: TRMM Precipitation
        await TestPrecipitation(client, baseUrl, "trmm", "TRMM");

        Console.WriteLine();
        Console.WriteLine("=== Test Summary ===");
        Console.WriteLine("Note: Tests validate URL construction and request handling.");
        Console.WriteLine("Actual data retrieval requires valid EarthData credentials.");
    }

    static async Task TestPrecipitation(HttpClient client, string baseUrl, string source, string displayName)
    {
        Console.WriteLine($"--- Testing {displayName} ---");

        var request = new
        {
            Source = source,
            DateTimeSpan = new
            {
                StartDate = "2015-01-01T00:00:00",
                EndDate = "2015-01-02T00:00:00",
                DateTimeFormat = "yyyy-MM-dd HH"
            },
            Geometry = new
            {
                Point = new
                {
                    Latitude = 33.925673,
                    Longitude = -83.355723
                }
            },
            DataValueFormat = "E3",
            TemporalResolution = "default",
            Units = "metric",
            OutputFormat = "json"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            Console.WriteLine($"  Sending POST request to /api/meteorology/precipitation");
            Console.WriteLine($"  Source: {source}");

            var response = await client.PostAsync($"{baseUrl}/api/meteorology/precipitation", content);

            Console.WriteLine($"  Status: {(int)response.StatusCode} {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ {displayName} request processed successfully");
                Console.ResetColor();

                // Parse and display metadata
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("metadata", out var metadata))
                {
                    Console.WriteLine("  Metadata:");
                    if (metadata.TryGetProperty("request_url", out var url))
                        Console.WriteLine($"    request_url: {url.GetString()}");
                    if (metadata.TryGetProperty("retrievalTime", out var time))
                        Console.WriteLine($"    retrievalTime: {time.GetString()}");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  ⚠ {displayName} request returned non-success status");
                Console.ResetColor();

                // Check for common errors
                if (responseBody.Contains("EarthData access token"))
                {
                    Console.WriteLine("  Note: Requires EarthData credentials in .netrc file");
                }
                else if (responseBody.Length < 500)
                {
                    Console.WriteLine($"  Response: {responseBody.Substring(0, Math.Min(200, responseBody.Length))}...");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ Connection failed: {ex.Message}");
            Console.ResetColor();
        }
        catch (TaskCanceledException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ Request timeout");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✗ Error: {ex.Message}");
            Console.ResetColor();
        }
    }
}
