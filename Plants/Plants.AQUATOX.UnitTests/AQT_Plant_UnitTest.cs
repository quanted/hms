using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Plants.AQUATOX.UnitTests
{
    [TestClass]
    public class AQT_Plant_UnitTest
    {
        [TestMethod]
        public void AQT_Plants_ValidJSON()
        {
            string filePath = "..\\..\\..\\..\\DOCS\\AQUATOX_Plant_Model_Simple_Valid.json";
            string json = GetTestFile(filePath);
            string errmsg;

            new Plants.AQUATOX.AQTPlants(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\DOCS\\AQUATOX_Plant_Model_Complex_Valid.json";
            json = GetTestFile(filePath);

            new Plants.AQUATOX.AQTPlants(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\DOCS\\AQUATOX_Plant_Model_Internal_Nutrients_Valid.JSON";
            json = GetTestFile(filePath);

            new Plants.AQUATOX.AQTPlants(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }

        [TestMethod]
        public void AQT_Plants_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Plant_Model_Valid_JSON.txt";
            string json = GetTestFile(filePath);
            string errmsg = "";

            AQTPlants AQTM = new AQTPlants(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Plant_Model_NoVolume.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTPlants(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Plant_Model_Missing_CO2.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTPlants(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Plant_Model_Missing_TSP.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTPlants(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Plant_Model_Missing_Light.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTPlants(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Plant_Model_Missing_Internal_Nutrient_SV.JSON";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTPlants(ref json, out errmsg, false);
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
                path2 = "/home/travis/build/quanted/hms/Plants/";
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
