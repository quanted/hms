using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMSWebServices.Controllers;
using System.Collections.Generic;

namespace UnitTest_HMSWebServices
{
    [TestClass]
    public class HMSWebServicesTests2
    {
        [TestMethod()]
        public void BaseFlowGETValidTest()
        {
            string dataset = "baseflow";
            WSBaseFlowController baseflow = new WSBaseFlowController();
            Dictionary<string, string> queryString = new Dictionary<string, string>(){
                { "nldas", "source=nldas&startdate=01-01-2010&enddate=01-10-2010&latitude=33&longitude=-83" },
                { "gldas", "source=gldas&startdate=01-01-2010&enddate=01-10-2010&latitude=33&longitude=-83" } };
            foreach(KeyValuePair<string, string> pair in queryString)
            {
                HMSJSON.HMSJSON.HMSData result = baseflow.Get(pair.Value);
                Assert.IsTrue(result.source.Contains(pair.Key));
                //Assert.IsTrue(result.dataset.ToLower().Contains(dataset));
                //Assert.IsNotNull(result.metadata);
                //Assert.IsNotNull(result.data);
            }
        }

    }
}
