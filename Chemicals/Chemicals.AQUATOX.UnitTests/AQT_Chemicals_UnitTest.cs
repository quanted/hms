using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AQUATOXChemicals;

namespace Chemicals.AQUATOX.UnitTests
{
    [TestClass]
    public class AQTTest1
    {
        [TestMethod]
        public void AQTChemicals_InvalidJSON()
        {
            string json = @"{""Comment"": ""JSON Export from AQUATOX 3.2"", ""$type"": ""AQUATOXUnknownType""}";  //unknown type causes problem
            string errmsg = "";
            new AQTChemicalModel(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTChemicals_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Chemical_Model_Valid_JSON.txt";

            string json = GetTestFile(path2);
            string errmsg = "";

            new AQTChemicalModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);
        }

        [TestMethod]
        public void AQTChemicals_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Chemical_Model_Valid_JSON.txt";

            string json = GetTestFile(path2);
            string errmsg = "";

            AQTChemicalModel AQTM = new AQTChemicalModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Chemical_Model_NoChemicals.txt";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTChemicalModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Chemical_Model_NopH.txt";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTChemicalModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Chemical_Model_NoVolume.txt";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTChemicalModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Chemical_Model_NoLight.txt";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTChemicalModel(ref json, out errmsg, false);
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
                path2 = "/home/travis/build/quanted/hms/Chemicals/";
                foreach (string p in fileName)
                {
                    if (!p.Equals(".."))
                    {
                        path2 = Path.Combine(path2, p);
                    }
                }
                json = File.ReadAllText(path2);
            }
            return json;
        }


    }



}


