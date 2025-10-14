using System;

namespace FormBuilder.API.Models
{
    public class FormConfig
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public FormStatus Status { get; set; } = FormStatus.Draft;

      
    }
}
