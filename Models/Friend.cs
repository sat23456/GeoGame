using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoGame.Models
{
    public class Friend
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }
    }
}
