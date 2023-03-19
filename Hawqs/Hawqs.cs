using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hawqs
{
    public class Hawqs
    {
        string hawqsAPIUrl = "https://dev-api.hawqs.tamu.edu/";
        string hawqsAPIKey = "A-DEFAULT-API-KEY-IF-YOU_WANT";

        // API Key is not required for input-definitions
        public async Task<string> GetProjectInputDefinitions()
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(hawqsAPIUrl + "projects/input-definitions");
        }

        public async Task<string> SubmitProject(string apiKey, string hawqsInputData)
        {
            if (apiKey == null)
            {
                apiKey = hawqsAPIKey;
            }

            // validate and constrain inputData ranges

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            HttpContent inputData = new StringContent(hawqsInputData, Encoding.UTF8, "application/json");
            inputData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(hawqsAPIUrl + "projects/submit", inputData);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProjectStatus(string apiKey, string projectId)
        {
            if (apiKey == null)
            {
                apiKey = hawqsAPIKey;
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            return await client.GetStringAsync(hawqsAPIUrl + "projects/" + projectId);
        }

        public async Task<string> GetProjectData(string apiKey, string projectId, bool processData)
        {
            if (apiKey == null)
            {
                apiKey = hawqsAPIKey;
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            var projectData = await client.GetStringAsync(hawqsAPIUrl + "projects/" + projectId);
            if (processData)
            {
                HawqsData hawqsData = new HawqsData();
                hawqsData.projectData = projectData;
                ProcessedHawqsData processedHawqsData = HawqsDataProcessor.ProcessData(hawqsData);
                return "data processing not yet implemented";
            }
            else
            {
                return projectData;
            }
        }

        public async Task<string> CancelProjectExecution(string apiKey, string projectId)
        {
            if (apiKey == null)
            {
                apiKey = hawqsAPIKey;
            }
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            HttpResponseMessage response = await client.PatchAsync(hawqsAPIUrl + "projects/cancel/" + projectId, null);
            return await response.Content.ReadAsStringAsync();
        }
    }

}
