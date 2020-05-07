using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AQUATOXBioaccumulation;

namespace Bioaccumulation.AQUATOX.UnitTests
{
    [TestClass]
    public class AQTTest1
    {
        [TestMethod]
        public void AQTBioaccumulation_InvalidJSON()
        {
            string json = @"{""Comment"": ""JSON Export from AQUATOX 3.2"", {""$type"": ""AQUATOXUnknownType""}}";  //unknown type causes problem
            string errmsg = "";
            new AQTBioaccumulationModel(ref json, out errmsg, false);
            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTBioaccumulation_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Bioaccumulation_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            new AQTBioaccumulationModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);
        }

        [TestMethod]
        public void AQTBioaccumulation_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "\\..\\..\\..\\..\\DOCS\\AQUATOX_Bioaccumulation_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTBioaccumulationModel AQTM = new AQTBioaccumulationModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Bioaccumulation_Missing_Carrier.JSON";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTBioaccumulationModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);

            path2 = path + "\\..\\..\\..\\..\\TEST\\INVALID\\AQUATOX_Bioaccumulation_Missing_Chem.JSON";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTBioaccumulationModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();
            Assert.AreNotEqual("", errmsg);
                       

        }


    }



}


