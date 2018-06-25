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
            new AQTNutrientsModel(ref json, ref errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTNutrients_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Nutrient_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            new AQTNutrientsModel(ref json, ref errmsg, false);
            Assert.AreEqual("", errmsg);

        }

    
        [TestMethod]
        public void AQTNutrients_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Nutrient_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTNutrientsModel AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoNutrients.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_pH_NoCO2.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoVolume.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NH4_no_Nitrate.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoOxygen.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\AQUATOX_Nutrient_Model_NoOxygen.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTNutrientsModel(ref json, ref errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);
            

        }


    }



}

