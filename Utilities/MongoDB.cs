using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Utilities
{
    public class MongoDB
    {

        public static async void DumpData(string taskID, string data)
        {
            string databaseName = "hms";
            string collectionName = "data";

            DateTime now = DateTime.Now;
            string host = (Environment.GetEnvironmentVariable("MONGODB") != null) ? Environment.GetEnvironmentVariable("MONGODB") :"localhost";
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, 27017)
            };
            var client = new MongoClient(settings);
            IMongoDatabase db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            var newDump = new BsonDocument
            {
                { "_id", BsonValue.Create(taskID) },
                { "date", BsonValue.Create(now.ToString()) },
                { "data", BsonValue.Create(data) }
            };
            await collection.InsertOneAsync(newDump);
        }
    }
}
