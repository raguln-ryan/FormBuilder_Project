using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormLayoutDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
