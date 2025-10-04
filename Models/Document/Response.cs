using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json; 
namespace FormBuilder.API.Models.Document
{
    public class Response
    {
         [BsonId]
         //primary key in mongodb
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // This maps the MongoDB _id field

        public int FormId { get; set; }
        public string ResponseId { get; set; }
        public DateTime SubmittedAt { get; set; }
       
       public Dictionary<string, JsonElement> Answers { get; set; } = new Dictionary<string, JsonElement>();
    }
}
