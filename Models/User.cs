using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoGame.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB automatically uses _id for the primary key

        public string UserId { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public List<string> Friends { get; set; }

        public List<string> Cities { get; set; }

        public int CurrentCityId { get; set; }
    }
}
