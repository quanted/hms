using System;
using Data;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    public class WSAquatoxWorkflow
    {
        /// <summary>
        /// Gets output from upstream segments from momgodb and merges them to
        /// the input of the current segment in input.sim_input. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errormsg"></param>
        /// <returns>Serialized json string of the AQUATOX simulation output.</returns>
        public string Run(WSAquatoxWorkflowInput input, out string errormsg)
        {
            errormsg = "";
            return "";
        }

        /// <summary>
        /// Utility function to check for error after running a simulation.
        /// </summary>
        /// <returns>ITimeSeriesOutput</returns>
        public ITimeSeriesOutput CheckForErrors(string errorMsg)
        {
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            if (!String.IsNullOrEmpty(errorMsg) || errorMsg.ToUpper().Contains("ERROR"))
            {
                return err.ReturnError(errorMsg);
            }

            return null;
        }
    }
}
