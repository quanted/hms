using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service SoilMoisture Model
    /// </summary>
    public class WSSoilMoisture
    {

        private enum SoilMSources { nldas, gldas }

        /// <summary>
        /// Gets SoilMoisture data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetSoilMoisture(SoilMoistureInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate SoilMoisture sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out SoilMSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SoilMoisture object
            SoilMoisture.SoilMoisture soilM = new SoilMoisture.SoilMoisture();
            soilM.Layers = input.Layers;

            // Assigning dataset values, used to determine base url
            List<string> dataset = new List<string>();
            foreach(string layer in soilM.Layers)
            {
                string l = layer.Replace('-', '_');
                dataset.Add(l + "_SOILM");
            }
            
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            soilM.Input = iFactory.SetTimeSeriesInput(input, dataset, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the SoilMoisture data.
            ITimeSeriesOutput result = soilM.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}