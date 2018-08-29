using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AQUATOXOrganicMatter;
using System.Diagnostics;

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
            new AQTOrganicMatter(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);
        }

        [TestMethod]
        public void AQT_OM_ValidJSON()
        {
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Valid_JSON.txt";
            string json = GetTestFile(filePath);
            string errmsg = "";

            new AQTOrganicMatter(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQT_OM_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Valid_JSON.txt";
            string json = GetTestFile(filePath);
            string errmsg = "";

            AQTOrganicMatter AQTM = new AQTOrganicMatter(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_NoVolume.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_MissingSV.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_pH.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_O2.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);
        }

        private string GetTestFile(string filePath)
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = Path.Combine(path, filePath);
            string json;
            try
            {
                json = File.ReadAllText(path2);
            }
            catch (System.IO.FileNotFoundException)
            {
                var fileName = filePath.Split("\\");
                path2 = Path.Combine("/home/travis/build/quanted/hms/OrganicMatter/TEST/", fileName[fileName.Length - 1]);
                json = File.ReadAllText(path2);
            }
            return json;
        }

    }
}
