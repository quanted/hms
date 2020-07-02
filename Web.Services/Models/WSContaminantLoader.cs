using Data;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service ContaminantLoader Model
    /// </summary>
    public class WSContaminantLoader
    {

        /// <summary>
        /// Gets pressure data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> LoadContaminant(ContaminantLoaderInput input)
        {

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // ContaminantLoader object
            ContaminantLoader.ContaminantLoader contam = new ContaminantLoader.ContaminantLoader(input.ContaminantType, input.ContaminantInputType, input.ContaminantInput);

            return contam.Result;

        }
    }
}