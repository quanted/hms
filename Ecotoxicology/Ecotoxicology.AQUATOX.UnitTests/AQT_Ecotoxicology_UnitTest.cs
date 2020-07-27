using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AQUATOXEcotoxicology;

namespace Ecotoxicology.AQUATOX.UnitTests
{
    [TestClass]
    public class AQTTest1
    {
        [TestMethod]
        public void AQTEcotoxicology_InvalidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\TEST\\Invalid\\AQUATOX_Ecotoxicology_Invalid.JSON";
            string json = GetTestFile(path2);
            string errmsg = "";
            new AQTEcotoxicologyModel(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTEcotoxicology_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Ecotoxicology_Valid.JSON";

            string json = GetTestFile(path2);
            string errmsg = "";

            new AQTEcotoxicologyModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);
        }

        [TestMethod]
        public void AQTEcotoxicology_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\TEST\\Ecotoxicology\\Farm Pond MO Esfenval External No Bioaccumulation.JSON";

            string json = GetTestFile(path2);
            string errmsg = "";

            AQTEcotoxicologyModel AQTM = new AQTEcotoxicologyModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Ecotoxicology_No_Chemical.JSON";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTEcotoxicologyModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Ecotoxicology_No_Biota.JSON";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTEcotoxicologyModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Ecotoxicology_Missing_BSV.JSON";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTEcotoxicologyModel(ref json, out errmsg, false);
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
                path2 = "/home/travis/build/quanted/hms/Ecotoxicology/";
                foreach (string p in fileName)
                {
                    if (!p.Equals(".."))
                    {
                        path2 = Path.Combine(path2, p);
                    }
                }
                if (!File.Exists(path2))
                {
                    path2 = Path.Combine("/home/travis/build/quanted/hms/Ecotoxicology/", fileName[fileName.Length - 2], fileName[fileName.Length - 1]);
                }
                json = File.ReadAllText(path2);
            }
            return json;
        }

    }

}


