using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormBuilder.API.Models
{
    [BsonIgnoreExtraElements]
    public class Question
    {
        private string _questionId;
        
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("question_id")]
        public string QuestionId 
        { 
            get => string.IsNullOrEmpty(_questionId) ? ObjectId.GenerateNewId().ToString() : _questionId;
            set => _questionId = value;
        }

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

        [BsonElement("maxLength")]
        public int? MaxLength { get; set; }

        [BsonElement("enabled")]
        public bool Enabled { get; set; } = true;
    }

    public class Option
    {
        private string _optionId;
        
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("option_id")]
        public string OptionId 
        { 
            get => string.IsNullOrEmpty(_optionId) ? ObjectId.GenerateNewId().ToString() : _optionId;
            set => _optionId = value;
        }

        [BsonElement("value")]
        public string Value { get; set; } = "";
    }
}
