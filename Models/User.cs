using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GeoGame.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }  // MongoDB automatically uses _id for the primary key


    }
}
