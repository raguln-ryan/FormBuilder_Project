using System.Collections.Generic;

namespace FormBuilder.API.DTOs
{
    public class FormSubmissionDTO
    {
        public int FormId { get; set; }
        public string ResponseId { get; set; }
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
    }
}
