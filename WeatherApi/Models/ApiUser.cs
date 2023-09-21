using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.Design;

namespace WeatherApi.Models
    {
    public class ApiUser
        {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }

        [BsonElement("Last Access")]
        public DateTime LastAccess { get; set; }
        public string APIKey { get; set; }
        }
    }
