using System;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormDto
    {
        public string Id { get; set; }                  // Form ID (or string if MongoDB Id)
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }              // FormStatus enum as int
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
