using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Diagenesis.AQUATOX.UnitTests
{
    [TestClass]
    public class AQT_Diagenesis_UnitTest
    {
        [TestMethod]
        public void AQTDiagenesiss_InvalidJSON()
        {
            string json = "{ invalid JSON }";
            string errmsg = "";
            new AQTDiagenesisModel(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTDiagenesiss_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\Diagenesis_Model_Valid_JSON.txt";

            string json = GetTestFile(path2);

            string errmsg;
            new AQTDiagenesisModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQTDiagenesis_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\TEST\\Diagenesis_MissingSVs.JSON";
            string json = GetTestFile(path2);
            string errmsg = "";
            AQTDiagenesisModel AQTM = new AQTDiagenesisModel(ref json, out errmsg, false);
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
                path2 = "/home/travis/build/quanted/hms/Diagensis/";
                foreach (string p in fileName)
                {
                    if (!p.Equals(".."))
                    {
                        path2 = Path.Combine(path2, p);
                    }
                }
                if (!File.Exists(path2))
                {
                    path2 = Path.Combine("/home/travis/build/quanted/hms/Diagensis/", fileName[fileName.Length - 2], fileName[fileName.Length - 1]);
                }
                json = File.ReadAllText(path2);
            }
            return json;
        }



    }

}

