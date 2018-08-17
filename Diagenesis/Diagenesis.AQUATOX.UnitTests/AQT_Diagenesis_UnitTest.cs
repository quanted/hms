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

            string json = File.ReadAllText(path2);

            string errmsg;
            new AQTDiagenesisModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }


        [TestMethod]
        public void AQTDiagenesis_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\TEST\\Diagenesis_MissingSVs.JSON";
            string json = File.ReadAllText(path2);
            string errmsg = "";
            AQTDiagenesisModel AQTM = new AQTDiagenesisModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

        }



    }

}

