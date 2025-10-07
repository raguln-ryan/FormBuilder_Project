using System;
using System.Collections.Generic;

namespace FormBuilder.API.Models
{
    public enum FormStatus
    {
        Draft,
        Published
    }

    public class Form : MongoBaseEntity
    {
        public required string Title { get; set; }
        public required string Description { get; set; }

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;


        public FormStatus Status { get; set; } = FormStatus.Draft;

        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
