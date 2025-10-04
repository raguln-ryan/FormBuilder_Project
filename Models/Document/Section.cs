using System.Collections.Generic;

namespace FormBuilder.API.Models.Document
{
    public class Section
    {
        public string SectionId { get; set; }
        public string Title { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
    }

    public class Field
    {
        public string FieldId { get; set; }
        public string Label { get; set; }
        public string Type { get; set; } // e.g., text, number, date
        public bool Required { get; set; }
    }
}
