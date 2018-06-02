using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stream.Hydrology.AQUATOX;
using System.IO;


namespace StreamHydrologyAQUATOXUnitTest
{
    [TestClass]
    public class AQTTest1
    {
        [TestMethod]
        public void AQTVolume_InvalidJSON()
        {
            string json = "invalid";
            string errmsg="";
            new AQTVolumeModel(ref json,ref errmsg);
            Assert.AreNotEqual("", errmsg);
            
        }

        [TestMethod]
        public void AQTVolume_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path +"../../../../../AQUATOX/DOCS/AQUATOX_Volume_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            new AQTVolumeModel(ref json, ref errmsg);
            Assert.AreEqual("", errmsg);

        }

        [TestMethod]
        public void AQTVolume_Verify_Runnable()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "../../../../../AQUATOX/TEST/AQUATOX_Model_NoSV_JSON.txt";
            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTVolumeModel AQTM = new AQTVolumeModel(ref json, ref errmsg);
            errmsg = AQTM.AQSim.AQTSeg.Verify_Runnable();

            Assert.AreNotEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/AQUATOX_Model_NoPSETUP_JSON.txt";
            json = File.ReadAllText(path2);
            errmsg = "";

            AQTM = new AQTVolumeModel(ref json, ref errmsg);
            errmsg = AQTM.AQSim.AQTSeg.Verify_Runnable();

            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTVolume_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "../../../../../AQUATOX/DOCS/AQUATOX_Volume_Model_Valid_JSON.txt";

            string json = File.ReadAllText(path2);
            string errmsg = "";

            AQTVolumeModel AQTM = new AQTVolumeModel(ref json, ref errmsg);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/AQUATOX_NOSV_Volume_JSON.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTVolumeModel(ref json, ref errmsg);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreNotEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/AQUATOX_NoLocale_JSON.txt";
            json = File.ReadAllText(path2);
            errmsg = "";
            AQTM = new AQTVolumeModel(ref json, ref errmsg);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreNotEqual("", errmsg);

        }


    }



}
