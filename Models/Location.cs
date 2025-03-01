using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoGame.Models
{
    public class Location
    {
        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB automatically uses _id for the primary key

        [JsonPropertyName("cityId")]
        public string cityId { get; set; }

        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("country")]
        public string country { get; set; }

        [JsonPropertyName("clues")]
        public List<string> clues { get; set; }

        [JsonPropertyName("funFact")]
        public List<string> funFact { get; set; }

        [JsonPropertyName("trivia")]
        public List<string> trivia { get; set; }
    }
}
