using System;

namespace FormBuilder.API.DTOs.Form
{
    public class QuestionDto
    {
        public string? Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string[]? Options { get; set; }
        public bool Required { get; set; }
        public string? Description { get; set; }
        public int? MaxLength { get; set; }
        public bool Enabled { get; set; } = true;
        
        // Add these missing properties
        public bool DescriptionEnabled { get; set; }
        public bool SingleChoice { get; set; }
        public bool MultipleChoice { get; set; }
        public string? Format { get; set; }
        public int Order { get; set; }
    }
}
