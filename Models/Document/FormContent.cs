using System.Collections.Generic;

namespace FormBuilder.API.Models.Document
{
    public class FormContent
    {
        public int FormId { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
