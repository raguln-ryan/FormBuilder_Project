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
        
    }
}
