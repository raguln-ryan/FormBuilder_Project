using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormBuilder.API.Models
{
    public class MongoBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    }
}
