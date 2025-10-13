using System;

namespace FormBuilder.API.Models
{
    public class FormConfig
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public FormStatus Status { get; set; } = FormStatus.Draft;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
