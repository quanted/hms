using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using HMSWebServices.Controllers;

namespace UnitTest_HMSWebServices
{
    [TestFixture]
    class HMSWebServicesTest
    {

        [Test]
        public void BaseFlowGETValidTest()
        {
            string dataset = "baseflow";
            WSBaseFlowController baseflow = new WSBaseFlowController();
            Dictionary<string, string> queryString = new Dictionary<string, string>(){
                { "nldas", "source=nldas&startdate=01-01-2010&enddate=01-10-2010&latitude=33&longitude=-83" },
                { "gldas", "source=gldas&startdate=01-01-2010&enddate=01-10-2010&latitude=33&longitude=-83" } };
            foreach (KeyValuePair<string, string> pair in queryString)
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
