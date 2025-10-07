using System;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class QuestionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
