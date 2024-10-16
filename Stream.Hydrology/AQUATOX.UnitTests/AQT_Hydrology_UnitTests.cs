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
            string errmsg;
            new AQTVolumeModel(ref json,out errmsg, false);
            Assert.AreNotEqual("", errmsg);
            
        }

        [TestMethod]
        public void AQTVolume_ValidJSON()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path +"../../../../../AQUATOX/DOCS/AQUATOX_Volume_Model_Valid.JSON";

            string json = GetTestFile(path2);
            string errmsg = "";

            new AQTVolumeModel(ref json, out errmsg, false);
            Assert.AreEqual("", errmsg);

        }

        [TestMethod]
        public void AQTVolume_Verify_Runnable()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "../../../../../AQUATOX/TEST/INVALID/AQUATOX_Model_NoSV.JSON";
            string json = GetTestFile(path2);
            string errmsg = "";

            AQTVolumeModel AQTM = new AQTVolumeModel(ref json, out errmsg, false);
            errmsg = AQTM.AQSim.AQTSeg.Verify_Runnable();

            Assert.AreNotEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/INVALID/AQUATOX_Model_NoPSETUP.JSON";
            json = GetTestFile(path2);
            errmsg = "";

            AQTM = new AQTVolumeModel(ref json, out errmsg, false);
            errmsg = AQTM.AQSim.AQTSeg.Verify_Runnable();

            Assert.AreNotEqual("", errmsg);

        }

        [TestMethod]
        public void AQTVolume_Check_Data_Requirements()
        {
            string path = System.Environment.CurrentDirectory;
            string path2 = path + "../../../../../AQUATOX/DOCS/AQUATOX_Volume_Model_Valid.JSON";

            string json = GetTestFile(path2);
            string errmsg = "";

            AQTVolumeModel AQTM = new AQTVolumeModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/INVALID/AQUATOX_NOSV_Volume.JSON";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTVolumeModel(ref json, out errmsg, false);
            errmsg = AQTM.CheckDataRequirements();

            Assert.AreNotEqual("", errmsg);

            path2 = path + "../../../../../AQUATOX/TEST/INVALID/AQUATOX_NoLocale.JSON";
            json = GetTestFile(path2);
            errmsg = "";
            AQTM = new AQTVolumeModel(ref json, out errmsg, false);
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
                path2 = "/home/travis/build/quanted/hms/Stream.Hydrology/";
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
