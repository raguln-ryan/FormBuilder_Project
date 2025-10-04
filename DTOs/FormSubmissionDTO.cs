using System.Collections.Generic;
using System.Text.Json; 

namespace FormBuilder.API.DTOs
{
    public class FormSubmissionDTO
    {
        public int FormId { get; set; }
        public string ResponseId { get; set; }
         public Dictionary<string, JsonElement> Answers { get; set; } = new Dictionary<string, JsonElement>();
    }
}
