using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Evapotranspiration Model
    /// </summary>
    public class WSEvapotranspiration
    {

        private enum EvapoSources { nldas, gldas, daymet, wgen, prism, ncdc }

        /// <summary>
        /// Gets evapotranspiration data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetEvapotranspiration(EvapotranspirationInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate evapotranspiration sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out EvapoSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Evapotranspiration object
            Evapotranspiration.Evapotranspiration evapo = new Evapotranspiration.Evapotranspiration()
            {
                Algorithm = input.Algorithm,
                Albedo = input.Albedo,
                CentralLongitude = input.CentralLongitude,
                SunAngle = input.SunAngle,
                Emissivity = input.Emissivity,
                Model = input.Model,
                Zenith = input.Zenith,
                LakeSurfaceArea = input.LakeSurfaceArea,
                LakeDepth = input.LakeDepth,
                SubsurfaceResistance = input.SubsurfaceResistance,
                StomatalResistance = input.StomatalResistance,
                LeafWidth = input.LeafWidth,
                RoughnessLength = input.RoughnessLength,
                VegetationHeight = input.VegetationHeight,
                LeafAreaIndices = input.LeafAreaIndices,
                AirTemperature = input.AirTemperature
        };

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            evapo.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "EVAPOT" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the Evapotranspiration data.
            ITimeSeriesOutput result = evapo.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}