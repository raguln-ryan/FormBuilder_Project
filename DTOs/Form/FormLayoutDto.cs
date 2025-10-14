using System;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormLayoutDto
    {
        public string? FormId { get; set; } 
        public string Title { get; set; } = string.Empty;  // Optional - only if you want to update
        public string Description { get; set; } = string.Empty; // Optional - only if you want to update  
        public FormStatusDto? Status { get; set; }  // Optional
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();

      
    }
}
