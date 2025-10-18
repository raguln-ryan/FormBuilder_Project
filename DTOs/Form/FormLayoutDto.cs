using System;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    // Request DTO - Only Questions (no FormId since it comes from path)
    public class FormLayoutRequestDto
    {
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }

    // Response DTO - Includes all fields from DB
    public class FormLayoutResponseDto
    {
        public string FormId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormStatusDto? Status { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
