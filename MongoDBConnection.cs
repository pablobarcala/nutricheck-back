using MongoDB.Driver;

namespace NutriCheck.Backend
{
    public class MongoDBConnection
    {
        private static readonly string? mongo_uri = "mongodb+srv://pablobarcala1:nutricheck@proyecto-final.crzpib6.mongodb.net/?retryWrites=true&w=majority&appName=proyecto-final";
        private static readonly Lazy<IMongoClient> _lazyClient = new Lazy<IMongoClient>(() => new MongoClient(mongo_uri));
        private readonly IMongoDatabase _database;

        public MongoDBConnection()
        {
            string database = "nutricheck";
            _database = _lazyClient.Value.GetDatabase(database);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
