using System.Collections.Generic;
using FormBuilder.API.Models;

namespace FormBuilder.API.DTOs.Form
{
    public class QuestionAnswerDto
    {
        public string? QuestionId { get; set; }
       public string Answer { get; set; } = string.Empty;
    }

    public class FormSubmissionDto
    {
        public string? FormId { get; set; }
        public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
    }
}
