using System.Collections.Generic;
using FormBuilder.API.Models;

namespace FormBuilder.API.DTOs.Form
{
    public class QuestionAnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;
       public string Answer { get; set; } = string.Empty;
    }

    public class FormSubmissionDto
    {
        public int FormId { get; set; }
        public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
    }
}
