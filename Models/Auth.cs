using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoGame.Models
{
    public class Auth
    {
        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB automatically uses _id for the primary key

        public string UserId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
