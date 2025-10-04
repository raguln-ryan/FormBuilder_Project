// File: BusinessLayer/DTOs/ResponseDTO.cs
namespace FormBuilder.API.BusinessLayer.DTOs
{
    public class ResponseDTO
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public required string UserId { get; set; }
        public required string AnswersJson { get; set; } // Store answers as JSON or a suitable structure
        public DateTime SubmittedAt { get; set; }
    }
}
