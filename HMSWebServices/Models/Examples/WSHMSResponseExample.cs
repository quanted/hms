using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HMSWebServices.Controllers
{
    public class WSHMSResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            HMSJSON.HMSJSON.HMSData example = new HMSJSON.HMSJSON.HMSData();
            example.dataset = "example_dataset";
            example.source = "example_source";
            example.metadata = new Dictionary<string, string>(){
                { "startdate", "01-01-2010" },
                { "enddate", "01-05-2010" },
                { "latitude", "33" },
                { "longitude", "-81" }
            };
            example.data = new Dictionary<string, List<string>>()
            {
                { "01-01-2010", new List<string>(){ "100" } },
                { "01-02-2010", new List<string>(){ "101" } },
                { "01-03-2010", new List<string>(){ "102" } },
                { "01-04-2010", new List<string>(){ "103" } },
                { "01-05-2010", new List<string>(){ "104" } }
            };
            return example;
        }
    }
}