using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Utilities.Tests
{
    [TestClass]
    public class JSONTests
    {
        [TestMethod]
        public void DeserializedValidTest()
        {

            string validJsonString = "{ 'name':'John', 'age':31, 'city':'New York' }";
            Dictionary<string, object> validDict = JSON.Deserialize<Dictionary<string, object>>(validJsonString);
            Dictionary<string, object> expectedDict = new Dictionary<string, object>()
                {
                    { "name", "John" },
                    { "age", 31 },
                    { "city", "New York" }
                };
            Assert.AreEqual(expectedDict.ToString(), validDict.ToString() );
        }

        [TestMethod]
        public void DeserializedInValidString()
        {
            string validJsonString = "{ 'name:'John', 'age':31, 'city':'New York' }";
            Dictionary<string, object> validDict = JSON.Deserialize<Dictionary<string, object>>(validJsonString);
            Assert.AreEqual(null, validDict);
        }

        [TestMethod]
        public void SerializeValidDictTest()
        {
            Dictionary<string, object> validDict = new Dictionary<string, object>()
                {
                    { "name", "John" },
                    { "age", 31 },
                    { "city", "New York" }
                };
            string expectedString = @"{""name"":""John"",""age"":31,""city"":""New York""}";
            string jsonString = JSON.Serialize(validDict);
            Assert.AreEqual(expectedString, jsonString);
        }

        [TestMethod]
        public void SerializeValidListTest()
        {
            List<object> validList = new List<object>(new object[] { "a string", 'c', 100, 1.1 });
            string expectedString = @"[""a string"",""c"",100,1.1]";
            string jsonString = JSON.Serialize(validList);
            Assert.AreEqual(expectedString, jsonString);
        }

        [TestMethod]
        public void SerializeValidDictFormatTest0()
        {
            Dictionary<string, object> validDict = new Dictionary<string, object>()
                {
                    { "name", "John" },
                    { "age", 31 },
                    { "city", "New York" }
                };
            string expectedString = @"{""name"":""John"",""age"":31,""city"":""New York""}";
            string jsonString = JSON.Serialize(validDict, 0);
            Assert.AreEqual(expectedString, jsonString);
        }

        [TestMethod]
        public void SerializeValidDictFormatTest1()
        {
            Dictionary<string, object> validDict = new Dictionary<string, object>()
                {
                    { "name", "John" },
                    { "age", 31 },
                    { "city", "New York" }
                };
            string expectedString = @"{""name"":""John"",""age"":31,""city"":""New York""}";
            string jsonString = JSON.Serialize(validDict,1);
            Assert.AreEqual(expectedString, jsonString);
        }

        [TestMethod]
        public void SerializeValidDictFormatTest2()
        {
            Dictionary<string, object> validDict = new Dictionary<string, object>()
                {
                    { "name", "John" },
                    { "age", 31 },
                    { "city", "New York" }
                };
            string expectedString = @"{""name"":""John"",""age"":31,""city"":""New York""}";
            string jsonString = JSON.Serialize(validDict, 2);
            Assert.AreEqual(expectedString, jsonString);
        }

    }
}
