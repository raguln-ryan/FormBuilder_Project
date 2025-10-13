using System;
using System.Collections.Generic;

namespace FormBuilder.API.Models
{
    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; } = false;
        public string? Description { get; set; }
        public int? MaxLength { get; set; }
        public bool Enabled { get; set; } = true;
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
