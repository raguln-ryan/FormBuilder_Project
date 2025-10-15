using System;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    // Request DTO - Only FormId and Questions for input
    public class FormLayoutRequestDto
    {
        public string FormId { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    // Response DTO - Includes all fields
    public class FormLayoutResponseDto
    {
        public string FormId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormStatusDto? Status { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
