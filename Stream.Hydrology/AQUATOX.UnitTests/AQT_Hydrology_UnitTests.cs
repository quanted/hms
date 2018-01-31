using Microsoft.VisualStudio.TestTools.UnitTesting;
using AQUATOX.Volume;
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


    }



}
