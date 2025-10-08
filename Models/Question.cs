using System;
using System.Collections.Generic;

namespace FormBuilder.API.Models
{
    public class Question : MongoBaseEntity
    {
        
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = "text"; // text, radio, checkbox, etc.
        public List<string> Options { get; set; } = new List<string>();

        public string CreatedBy { get; set; } = string.Empty; // Admin user who created the question

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
