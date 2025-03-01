using GeoGame.Models;
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

        public T GetByFilter<T>(string collectionName, string filterName, string filterValue)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(filterName, filterValue);
            return collection.Find(filter).FirstOrDefault();
        }

        public bool Register(string email, string password)
        {
            var users = GetCollection<Auth>("AuthData");

            // Check if email already exists
            var existingUser = users.Find(u => u.Email == email).FirstOrDefault();
            if (existingUser != null)
            {
                return false; // Email already exists
            }

            // Generate random 5-letter UserId
            string userId = GenerateRandomUserId();

            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Create the new user
            Auth newUser = new Auth
            {
                UserId = userId,
                Email = email,
                PasswordHash = hashedPassword
            };

            users.InsertOne(newUser);
            return true;
        }

        // Login Method
        public bool Login(string email, string password)
        {
            var users = GetCollection<Auth>("AuthData");

            // Find the user by email
            var user = users.Find(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return false; // User not found
            }

            // Verify the password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isPasswordValid;
        }

        // Helper method to generate a random 5-letter UserId
        private string GenerateRandomUserId()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Range(0, 5)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
