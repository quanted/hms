using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AQUATOXOrganicMatter;

namespace OrganicMatter.AQUATOX.UnitTests
{
    [TestClass]
    public class AQT_OM_UnitTest
    {
                    
        [TestMethod]
        public void AQT_OM_InvalidJSON()
        {
            string json = "{{{{{{";
            string errmsg = "";
            new AQTOrganicMatter(ref json, ref errmsg, false);
            Assert.AreNotEqual("", errmsg);
        }

        [TestMethod]
        public void AQTNutrients_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "/../../../../DOCS/AQUATOX_OM_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            new AQTOrganicMatter(ref json, ref errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQTNutrients_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_OM_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTOrganicMatter AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_NoVolume.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_MissingSV.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_pH.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_O2.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);
        }

    }
}
