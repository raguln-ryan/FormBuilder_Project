using System;
using System.Collections.Generic;

namespace FormBuilder.API.Models.Document
{
    public class Response
    {
        public int FormId { get; set; }
        public string ResponseId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
    }
}
