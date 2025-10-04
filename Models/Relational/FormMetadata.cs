using System;

namespace FormBuilder.API.Models.Relational
{
    public class FormMetadata
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public string CreatedBy { get; set; }
        public string PublishedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public string WorkflowUsage { get; set; }
    }
}
