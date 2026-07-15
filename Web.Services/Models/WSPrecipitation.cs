using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service S Model
    /// </summary>
    public class WSPrecipitation
    {
        private enum PrecipSources{ nldas, gldas, trmm, ncei, daymet, wgen, prism, nwm };

        /// <summary>
        /// Gets precipitation data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input">ITimeSeriesInput</param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetPrecipitation(PrecipitationInput input)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate precipitation sources.
            if(input.Source == "ncdc") { input.Source = "ncei"; }
            errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid.": "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            string nldasAccessToken = null;

            // Precipitation object
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            precip.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }           

            if (precip.Input.Source.Contains("ncei"))
            {
                precip.Input.Geometry.GeometryMetadata["token"] = (precip.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? precip.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }

            if (precip.Input.Source.Contains("nldas") && string.IsNullOrWhiteSpace(input.PreFetchedNldasData))
            {
                try
                {
                    using var gesDisc = new Utilities.clsNLDAS_GES_DISC(AppContext.BaseDirectory);
                    nldasAccessToken = gesDisc.GetAccessToken();
                }
                catch (Exception ex)
                {
                    return err.ReturnError("ERROR: Unable to get EarthData access token for NLDAS. Ensure .netrc credentials are configured. " + ex.Message);
                }
            }

            // Gets the Precipitation data.
            ITimeSeriesOutput result = precip.GetData(
                out errorMsg,
                accessToken: nldasAccessToken,
                preFetchedData: input.PreFetchedNldasData);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            if (result == null)
            {
                return err.ReturnError("ERROR: No precipitation data was returned from the selected source.");
            }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, precip.Input, result);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            bool isNldas = precip.Input.Source.Contains("nldas");

            if (isNldas)
            {
                string baseUrl = !string.IsNullOrWhiteSpace(input.PriorNldasBaseUrl)
                    ? input.PriorNldasBaseUrl
                    : (precip.Input.BaseURL != null && precip.Input.BaseURL.Count > 0 ? precip.Input.BaseURL[0] : null);

                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    result.Metadata = Utilities.Metadata.AddToMetadata("base_url", baseUrl, result.Metadata);
                }

                if (input.PriorNldasMetadata != null && input.PriorNldasMetadata.Count > 0)
                {
                    result.Metadata = Utilities.Metadata.MergeMetadata(result.Metadata, input.PriorNldasMetadata, "prior_nldas");
                }

                if (!string.IsNullOrWhiteSpace(input.PreFetchedNldasData))
                {
                    result.Metadata = Utilities.Metadata.AddToMetadata("nldas_prefetched", "true", result.Metadata);
                }
            }

            return result;
         }
    }
}