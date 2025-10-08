using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormStatusDto Status { get; set; } = FormStatusDto.Draft;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
