using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Utilities
{
    public static class MongoDB
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

        /// <summary>
        /// Given a databaseName, collectionName, and taskId: finds a single result that matches.
        /// </summary>
        /// <param name="databaseName">Database</param>
        /// <param name="collectionName">Collection</param>
        /// <param name="taskID">Task ID</param>
        /// <returns></returns>
        public static async Task<BsonDocument> FindByTaskIDAsync(string databaseName, string collectionName, string taskID)
        {
            DateTime now = DateTime.Now;
            string host = (Environment.GetEnvironmentVariable("MONGODB") != null) ? Environment.GetEnvironmentVariable("MONGODB") : "localhost";
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, 27017)
            };
            var client = new MongoClient(settings);
            IMongoDatabase db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", taskID);
            return await collection.Find(filter).SingleAsync();
        }

        public static string FindInGridFS(string databaseName, BsonObjectId dataID)
        {
            string host = (Environment.GetEnvironmentVariable("MONGODB") != null) ? Environment.GetEnvironmentVariable("MONGODB") : "localhost";
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, 27017)
            };
            var client = new MongoClient(settings);
            IMongoDatabase db = client.GetDatabase(databaseName);
            var bucket = new GridFSBucket(db);
            return System.Text.Encoding.UTF8.GetString(bucket.DownloadAsBytesAsync(dataID).Result).Replace("\\", "");
        }

        /// <summary>
        /// Given a databaseName, collectionName, and data: inserts a single entry.
        /// </summary>
        /// <param name="databaseName">Database</param>
        /// <param name="collectionName">Collection</param>
        /// <param name="data">Data to insert</param>
        public static void InsertOne(string databaseName, string collectionName, BsonDocument data)
        {
            DateTime now = DateTime.Now;
            string host = (Environment.GetEnvironmentVariable("MONGODB") != null) ? Environment.GetEnvironmentVariable("MONGODB") : "localhost";
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, 27017)
            };
            var client = new MongoClient(settings);
            IMongoDatabase db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(data);
        }
    }
}
