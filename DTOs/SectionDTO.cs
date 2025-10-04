using System.Collections.Generic;

namespace FormBuilder.API.DTOs
{
    public class SectionDTO
    {
        public required string SectionId { get; set; }
        public required string Title { get; set; }
        public List<FieldDTO> Fields { get; set; } = new List<FieldDTO>();
    }

    public class FieldDTO
    {
        public string FieldId { get; set; }
        public string Label { get; set; }
        // "text", "textarea", "number", "email", "date", "dropdown", "radio", "checkbox", "file"
        public string Type { get; set; }
        public bool Required { get; set; }

        // Add this for multiple-choice options
         public List<string>? Options { get; set; } = new List<string>();
    }
}
