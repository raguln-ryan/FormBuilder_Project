using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Form
{
    public class FormSubmissionDto
    {
        public string FormId { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
        public List<FileUploadDto> FileUploads { get; set; } = new List<FileUploadDto>();
    }

    public class AnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;  // Must match Question.QuestionId
        public string? Answer { get; set; }
    }

    public class FileUploadDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Base64Content { get; set; } = string.Empty;
    }
}
