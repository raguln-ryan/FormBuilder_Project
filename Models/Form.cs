using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FormBuilder.API.DTOs.Form;
namespace FormBuilder.API.Models
{
[BsonIgnoreExtraElements]
public class Form : MongoBaseEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    public FormStatus Status { get; set; } = FormStatus.Draft;
        
    // Form-level metadata fields
    [BsonElement("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
        
    // Optional fields for when form is published
    [BsonElement("published_by")]
    public string? PublishedBy { get; set; }

    [BsonElement("published_at")]
    public DateTime? PublishedAt { get; set; }
        
    // Optional field for tracking who last updated
    [BsonElement("updated_by")]
    public string? UpdatedBy { get; set; }
        
    // Questions collection - no metadata per question
    [BsonElement("questions")]
    public List<Question> Questions { get; set; } = new List<Question>();
        
    // Remove this if not needed
    public FormConfigDto? Config { get; set; }
}
}
