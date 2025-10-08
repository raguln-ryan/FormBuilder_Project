using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormLayoutDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public FormStatusDto Status { get; set; } = FormStatusDto.Draft;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
