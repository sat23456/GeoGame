using System;
using GeoGame.Models;
using MongoDB.Driver;

namespace GeoGame.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly Random _random = new Random();

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

        public void Update<T>(string collectionName, string keyName, string id, T entity)
        {
            var collection = GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(keyName, id);
            collection.ReplaceOne(filter, entity);
        }

        public async Task<bool> MoveToNextCityAsync(string userId)
        {
            var users = _database.GetCollection<User>("User");

            // Find the user by UserId
            var user = await users.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (user == null)
                return false;

            // Check if the user has any cities to move to
            if (user.Cities == null || user.Cities.Count == 0)
                return false;

            // Ensure that CurrentCityId is a valid index in the CityIds list
            if (user.CurrentCityId < 0 || user.CurrentCityId >= user.Cities.Count)
                return false;

            // Check if the user has a next city to move to
            if (user.CurrentCityId + 1 >= user.Cities.Count)
                return false;

            // Update the CurrentCityId to the next city index
            var nextCityIndex = user.CurrentCityId + 1;
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<User>.Update.Set(u => u.CurrentCityId, nextCityIndex);

            await users.UpdateOneAsync(filter, update);

            // Return the next city index
            return true;
        }

        public async Task<bool> ResetCityListAsync(string userId)
        {
            var users = _database.GetCollection<User>("User");

            // Find the user by UserId
            var user = await users.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (user == null)
                return false;

            // Generate a new random list of cities (random numbers between 1-100)
            user.Cities = GenerateRandomCityList();
            user.CurrentCityId = 0; // Reset the CurrentCityId to start from the first city

            // Update the user's city list and reset index
            var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
            var update = Builders<User>.Update.Set(u => u.Cities, user.Cities)
                                              .Set(u => u.CurrentCityId, user.CurrentCityId);

            await users.UpdateOneAsync(filter, update);

            // Return a success message
            return true;
        }

        public string GenerateFriendRequestToken()
        {
            return Guid.NewGuid().ToString(); // Generates a unique token
        }

        // Method to create a friend request (store token in the database)
        public async Task<string> CreateFriendRequestAsync(string userId)
        {
            // Generate a unique token
            var token = GenerateFriendRequestToken();

            // Create the friend request with a token
            var friendRequest = new FriendRequest
            {
                Token = token,
                UserId = userId,
                ExpirationDate = DateTime.UtcNow.AddDays(1) // Set the token expiration time (optional)
            };

            // Store the friend request in the FriendRequests collection
            var requests = _database.GetCollection<FriendRequest>("FriendRequests");
            await requests.InsertOneAsync(friendRequest);

            // Generate and return the URL with the token
            return $"http://localhost:5000/api/user/acceptFriendRequest?token={friendRequest.Token}&userId={userId}";
        }

        // Method to add a friend based on the token
        public async Task<string> AddFriendAsync(string userId, string token)
        {
            var users = _database.GetCollection<User>("User");

            // Check if the token is valid and exists in the FriendRequests collection
            var friendRequest = await _database.GetCollection<FriendRequest>("FriendRequests")
                                                .Find(fr => fr.Token == token)
                                                .FirstOrDefaultAsync();

            if (friendRequest == null)
                return "Invalid or expired token.";

            // Find the user who generated the friend request
            var friendUser = await users.Find(u => u.UserId == friendRequest.UserId).FirstOrDefaultAsync();
            var user = await users.Find(u => u.UserId == userId).FirstOrDefaultAsync();

            if (user == null || friendUser == null)
                return "User not found.";

            // Add each user to each other's friends list if they're not already friends
            if (!user.Friends.Contains(friendUser.UserId))
            {
                user.Friends.Add(friendUser.UserId);
                var userFilter = Builders<User>.Filter.Eq(u => u.UserId, userId);
                var userUpdate = Builders<User>.Update.Set(u => u.Friends, user.Friends);
                await users.UpdateOneAsync(userFilter, userUpdate);
            }

            if (!friendUser.Friends.Contains(user.UserId))
            {
                friendUser.Friends.Add(user.UserId);
                var friendFilter = Builders<User>.Filter.Eq(u => u.UserId, friendUser.UserId);
                var friendUpdate = Builders<User>.Update.Set(u => u.Friends, friendUser.Friends);
                await users.UpdateOneAsync(friendFilter, friendUpdate);
            }

            // Optionally, delete the token from the FriendRequests collection after use
            var tokenFilter = Builders<FriendRequest>.Filter.Eq(fr => fr.Token, token);
            await _database.GetCollection<FriendRequest>("FriendRequests").DeleteOneAsync(tokenFilter);

            return "Friendship established!";
        }

        private List<string> GenerateRandomCityList()
        {
            return Enumerable.Range(1, 100)
                             .OrderBy(x => _random.Next())
                             .Select(x => x.ToString())
                             .ToList();
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

    public class FriendRequest
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public DateTime ExpirationDate { get; set; } // Optional, to expire the token after a certain time
    }
}
