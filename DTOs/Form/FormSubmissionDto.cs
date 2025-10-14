using System.Collections.Generic;
using FormBuilder.API.Models;

namespace FormBuilder.API.DTOs.Form
{
    public class FormSubmissionDto
    {
        public string FormId { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }

    public class AnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;  // Must match Question.QuestionId
        public string? Answer { get; set; }
    }
}
