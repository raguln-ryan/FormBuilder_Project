using System;
using System.Collections.Generic;
using FormBuilder.API.DTOs.Form;
namespace FormBuilder.API.Models
{
    public class Form : MongoBaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormStatus Status { get; set; } = FormStatus.Draft;
        public string CreatedBy { get; set; } = string.Empty;
        public string? PublishedBy { get; set; }

        // Nullable dates
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Optional PublishedAt
        public DateTime? PublishedAt { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public FormConfigDto? Config { get; set; }
    }
}
