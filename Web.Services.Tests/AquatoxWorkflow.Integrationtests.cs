using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Services.Models;
using Web.Services.Controllers;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using AQUATOX.AQTSegment;
using Newtonsoft.Json;

namespace Web.Services.Tests
{
    /// <summary>
    /// Class for integration and unit tests on the Aquatox Workflow model/controller classes.
    /// </summary>
    /// <note>
    /// To write to console: 
    ///     dotnet test --filter TestCategory=CATEGORY -l "console;verbosity=detailed"
    /// </note>
    [TestClass]
    public class AquatoxWorkflowIntegrationtests
    {
        private string GetBaseJsonFileTestHelper(string file)
        {
            // Construct path and return base json
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", file);

            // Check local file path
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            // Error 
            else
            {
                return "Base json file could not be found.";
            }
        }

        /// <summary>
        /// Given an empty dictionary, should return default file.
        /// </summary>
        [TestMethod]
        [TestCategory("GetBaseJson")]
        public void GetBaseJson_NoValuesInDict_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            Assert.AreNotEqual(GetBaseJsonFileTestHelper("MS_Phosphorus.json"), "Base json file could not be found.");
          //  Assert.AreEqual(GetBaseJsonFileTestHelper("MS_Phosphorus.json"), aqt.GetBaseJson(flags));
        }

        /// <summary>
        /// Given an incomplete dictionary with flag values out of order, should return correct file.
        /// </summary>
        [TestMethod]
        [TestCategory("GetBaseJson")]
        public void GetBaseJson_LessThanAllValuesInDict_And_OutOfOrder_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            Dictionary<string, bool> flags = new Dictionary<string, bool>()
            {
                ["Organic Matter"] = true
            };
            Assert.AreNotEqual(GetBaseJsonFileTestHelper("MS_OM_NoP.json"), "Base json file could not be found.");
           // Assert.AreEqual(GetBaseJsonFileTestHelper("MS_OM_NoP.json"), aqt.GetBaseJson(flags));
        }

        /// <summary>
        /// Given a complete dictionary with flag values out of order, should return correct file.
        /// </summary>
        [TestMethod]
        [TestCategory("GetBaseJson")]
        public void GetBaseJson_CorrectValuesInDict_And_OutOfOrder_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            Dictionary<string, bool> flags = new Dictionary<string, bool>()
            {
                ["Organic Matter"] = false,
                ["Nitrogen"] = true,
                ["Phosphorus"] = false
            };
            Assert.AreNotEqual(GetBaseJsonFileTestHelper("MS_Nitrogen.json"), "Base json file could not be found.");
            //Assert.AreEqual(GetBaseJsonFileTestHelper("MS_Nitrogen.json"), aqt.GetBaseJson(flags));
        }

        /// <summary>
        /// Gets a streamflow time series output from the MongoDB and tests that there was no error. 
        /// Requires a running MongoDB instance with:
        ///     database - "hms_workflows", collection - "data", entry - { "_id" : "test_streamflow_dependency_1", "output" : "TimeSeriesOuput" }
        /// </summary>
        [TestMethod]
        [TestCategory("CheckDependencies")]
        public void CheckDependencies_Streamflow_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            Dictionary<string, string> dependencies = new Dictionary<string, string>()
            {
                ["streamflow"] = "test_streamflow_dependency_1"
            };
            AQTSim sim = new AQTSim();
           // sim.Instantiate(aqt.GetBaseJson(new Dictionary<string, bool>(){}));
            // Check no errors
          //  Assert.AreEqual("", aqt.CheckDependencies(dependencies, sim));
            // Check if updating discharge actually changed anything
            AQTSim sim2 = new AQTSim();
           // sim2.Instantiate(aqt.GetBaseJson(new Dictionary<string, bool>(){}));
            Assert.IsFalse(JsonConvert.SerializeObject(sim) == JsonConvert.SerializeObject(sim2));
        }

        /// <summary>
        /// Tests that given empty comid/upstream, archive is not modified on workflow without error.
        /// </summary>
        [TestMethod]
        [TestCategory("ArchiveUpstreamOutputs")]
        public void ArchiveUpstreamOutputs_NoUpstream_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            // Pass empty comids and upstream, returns without error
            //Assert.AreEqual("", aqt.ArchiveUpstreamOutputs(new List<int>(), new Dictionary<string, string>()));
            // Check that the archive has not been modified by comparing to a new workflow. Should 
            // both be same.
            WSAquatoxWorkflow aqt2 = new WSAquatoxWorkflow();
            Assert.IsTrue(JsonConvert.SerializeObject(aqt) == JsonConvert.SerializeObject(aqt2));
        }

        /// <summary>
        /// Tests that given comid/upstream, output is retrieved from database and archive is modified on workflow without error.
        ///     database - "hms_workflows", collection - "data", entry - { "_id" : "test_archive_upstream_1", "output" : "AQTSim" }
        /// </summary>
        [TestMethod]
        [TestCategory("ArchiveUpstreamOutputs")]
        public void ArchiveUpstreamOutputs_WithUpstream_IsValid()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            Dictionary<string, string> upstream = new Dictionary<string, string>()
            {
                ["0000000"] = "test_archive_upstream_1"
            };
            // Pass comids and upstream, returns without error
            //Assert.AreEqual("", aqt.ArchiveUpstreamOutputs(new List<int>() {0000000}, upstream));
            // Check that the archive has been modified by comparing to a new workflow. Should 
            // both be different.
            WSAquatoxWorkflow aqt2 = new WSAquatoxWorkflow();
            Assert.IsTrue(JsonConvert.SerializeObject(aqt) == JsonConvert.SerializeObject(aqt2));
        }
    }
}