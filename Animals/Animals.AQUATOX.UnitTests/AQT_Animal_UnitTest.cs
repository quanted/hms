using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Animals.AQUATOX;

namespace Animals.AQUATOX.UnitTests
{
    [TestClass]
    public class AQT_Animal_UnitTest
    {
        [TestMethod]
        public void AQT_Animals_ValidJSON()
        {
            string filePath = "..\\..\\..\\..\\DOCS\\Animal_Valid.JSON";
            string json = GetTestFile(filePath);
            string errmsg;

            new AQTAnimals(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);
        }

        [TestMethod]
        public void AQT_Animals_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string filePath = "..\\..\\..\\..\\DOCS\\Animal_Valid.JSON";
            string json = GetTestFile(filePath);
            string errmsg = "";

            AQTAnimals AQTM = new AQTAnimals(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Animal_Model_NoVolume.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTAnimals(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            filePath = "..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Animal_Model_Missing_O2.txt";
            json = json = GetTestFile(filePath);
            errmsg = "";
            AQTM = new AQTAnimals(ref json, out errmsg, false);
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
                path2 = Path.Combine("/home/travis/build/quanted/hms/Animals/TEST/", fileName[fileName.Length - 1]);
                if (!File.Exists(path2))
                {
                    path2 = Path.Combine("/home/travis/build/quanted/hms/Animals/TEST/INVALID", fileName[fileName.Length - 1]);
                }
                json = File.ReadAllText(path2);
            }
            return json;
        }
    }
}
