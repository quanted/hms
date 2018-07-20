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
            new AQTOrganicMatter(ref json, ref errmsg, false);
            Assert.AreNotEqual("", errmsg);
        }

        [TestMethod]
        public void AQTNutrients_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Valid_JSON.txt";
            //string filePath2 = "TEST/AQUATOX_OM_Model_Valid_JSON.txt";
            string path2 = Path.Combine(path, filePath);
            string json;
            try
            {
                json = File.ReadAllText(path2);
            }
            catch (System.IO.FileNotFoundException)
            {
                var fileName = filePath.Split("\\");
                path2 = Path.Combine("/home/travis/build/quanted/hms/OrganicMatter/", fileName[fileName.Length - 1]);
                json = File.ReadAllText(path2);
            }
            string errmsg = "";

            new AQTOrganicMatter(ref json, ref errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQTNutrients_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Valid_JSON.txt";
            string path2 = Path.Combine(path, filePath);
            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTOrganicMatter AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_NoVolume.txt";
            path2 = Path.Combine(path, filePath);
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_MissingSV.txt";
            path2 = Path.Combine(path, filePath);
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_pH.txt";
            path2 = Path.Combine(path, filePath);
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_OM_Model_Missing_O2.txt";
            path2 = Path.Combine(path, filePath);
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTOrganicMatter(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);
        }

    }
}
