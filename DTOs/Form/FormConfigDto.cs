using System;

namespace FormBuilder.API.DTOs.Form
{
    public class FormConfigDto
    {
        public string? FormId { get; set; }  // Add this - will be populated in response
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public FormStatusDto Status { get; set; } = FormStatusDto.Draft;
    }
}
