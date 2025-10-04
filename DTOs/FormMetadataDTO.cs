using System;

namespace FormBuilder.API.DTOs
{
    public class FormMetadataDTO
    {
        public int Id { get; set; }
        public required string FormName { get; set; }
        public required string CreatedBy { get; set; }
        public string? PublishedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public required string WorkflowUsage { get; set; }
    }
}
