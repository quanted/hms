using Microsoft.VisualStudio.TestTools.UnitTesting;
using AQUATOXNutrientModel;
using System.IO;

namespace Nutrients.AQUATOX.UnitTests
{
    [TestClass]
    public class AQTTest1
    {
        [TestMethod]
        public void AQTNutrients_InvalidJSON()
        {
            string json = "this is just not going to work";
            string errmsg = "";
            new AQTNutrientsModel(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTNutrients_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_Valid_JSON.txt";
            string json = GetTestFile(filePath);
            string errmsg = "";

            new AQTNutrientsModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQTNutrients_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_Valid_JSON.txt";
            string json = GetTestFile(filePath);
            string errmsg = "";

            AQTNutrientsModel AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoNutrients.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_pH_NoCO2.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoVolume.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NH4_no_Nitrate.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoOxygen.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoOxygen.txt";
            json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, out errmsg, false);
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
                path2 = Path.Combine("/home/travis/build/quanted/hms/Nutrients/TEST", fileName[fileName.Length - 1]);
                json = File.ReadAllText(path2);
            }
            return json;
        }
    }
}