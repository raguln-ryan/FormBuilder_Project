using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormBuilder.API.Models
{
[BsonIgnoreExtraElements]
public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("question_id")]
    public string QuestionId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("type")]
    public string Type { get; set; } = ""; // short_text, long_text, choice, file_upload, date_picker

    [BsonElement("question")]
    public string QuestionText { get; set; } = "";

    [BsonElement("description_enabled")]
    public bool DescriptionEnabled { get; set; } = false;

    [BsonElement("description")]
    public string Description { get; set; } = "";

    [BsonElement("single_choice")]
    public bool SingleChoice { get; set; } = false;

    [BsonElement("multiple_choice")]
    public bool MultipleChoice { get; set; } = false;

    [BsonElement("options")]
    public List<Option>? Options { get; set; }

    [BsonElement("format")]
    public string? Format { get; set; } // For date picker

    [BsonElement("required")]
    public bool Required { get; set; } = false;

    [BsonElement("order")]
    public int Order { get; set; }
}

public class Option
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [BsonElement("option_id")]
    public string OptionId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("value")]
    public string Value { get; set; } = "";
}
}
