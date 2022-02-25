using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class MyDatabase<T> : IMyDatabase<T>
    {
        public IMongoDatabase Database {get;}

        public IMongoCollection<T> Collection {get;}
        public MyDatabase(string collectionName, string connectionString, string database)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(database);
            Collection = Database.GetCollection<T>(collectionName);
        }

        
    }

    public interface IMyDatabase<T>
    {
        IMongoDatabase Database { get; }
        IMongoCollection<T> Collection { get; }
    }
}