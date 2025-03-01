using MongoDB.Driver;

namespace GeoGame.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("MongoDB:ConnectionString"));
            _database = client.GetDatabase(configuration.GetValue<string>("MongoDB:DatabaseName"));
        }

        // Generic method to get collection
        private IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        // Generic method to retrieve all documents
        public List<T> GetAll<T>(string collectionName)
        {
            var collection = GetCollection<T>(collectionName);
            return collection.Find(Builders<T>.Filter.Empty).ToList();
        }

        // Generic method to retrieve one document by id
        public T GetById<T>(string collectionName, string id)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return collection.Find(filter).FirstOrDefault();
        }

        public T GetByFilter<T>(string collectionName, string filterName, string filterValue)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(filterName, filterValue);
            return collection.Find(filter).FirstOrDefault();
        }

        // Generic method to create a document
        public void Create<T>(string collectionName, T entity)
        {
            var collection = GetCollection<T>(collectionName);
            collection.InsertOne(entity);
        }

        // Generic method to update a document
        public void Update<T>(string collectionName, string id, T entity)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);
            collection.ReplaceOne(filter, entity);
        }

        // Generic method to delete a document
        public void Delete<T>(string collectionName, string id)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("_id", id);
            collection.DeleteOne(filter);
        }
    }
}
