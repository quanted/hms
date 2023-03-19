using System.Threading.Tasks;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Radiation Model
    /// </summary>
    public class WSHawqs
    {
        /// <summary>
        /// Gets project input definitions.
        /// </summary>
        /// <returns>JSON string</returns>
        public async Task<string> GetProjectInputDefinitions()
        {
            Hawqs.Hawqs hawqs = new Hawqs.Hawqs();
            string inputDef = await hawqs.GetProjectInputDefinitions();
            return inputDef;
        }

        /// <summary>
        /// Submits a new HAWQS project.
        /// </summary>
        /// <returns>JSON string</returns>
        public async Task<string> SubmitProject(string apiKey, string inputData) 
        {
            Hawqs.Hawqs hawqs = new Hawqs.Hawqs();
            string projectData = await hawqs.SubmitProject(apiKey, inputData);
            return projectData;
        }

        /// <summary>
        /// Gets project status.
        /// </summary>
        /// <returns>JSON string</returns>
        public async Task<string> GetProjectStatus(string apiKey, string projectId)
        {
            Hawqs.Hawqs hawqs = new Hawqs.Hawqs();
            string projectStatus = await hawqs.GetProjectStatus(apiKey, projectId);
            return projectStatus;
        }

        /// <summary>
        /// Gets conpleted project data file urls.
        /// </summary>
        /// <returns>JSON string</returns>
        public async Task<string> GetProjectData(string apiKey, string projectId, bool process)
        {
            Hawqs.Hawqs hawqs = new Hawqs.Hawqs();
            string dataURLs = await hawqs.GetProjectData(apiKey, projectId, process);
            return dataURLs;
        }

        /// <summary>
        /// Cancels submitted project execution.
        /// </summary>
        /// <returns></returns>
        public async Task<string> CancelProjectExecution(string apiKey, string projectId)
        {
            Hawqs.Hawqs hawqs = new Hawqs.Hawqs();
            string cancelStatus = await hawqs.CancelProjectExecution(apiKey, projectId);
            return cancelStatus;
        }
    }
}