using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Controllers
{
    public class WSBaseFlowGetRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            string queryString = "startdate=01-01-2010&enddate=01-10-2010&latitude=33&longitude=-81&source=nldas";
            return queryString;
        }
    }
}