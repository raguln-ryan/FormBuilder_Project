using System;

namespace FormBuilder.API.DTOs.Form
{
    // Request DTO - Only Title and Description for input
    public class FormConfigRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Response DTO - Includes FormId, Title, and Description
    public class FormConfigResponseDto
    {
        public string FormId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
