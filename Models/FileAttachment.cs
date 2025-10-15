using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    public class FileAttachment : BaseEntity
    {
        [ForeignKey("Response")]
        public int ResponseId { get; set; }

        [Required]
        [StringLength(100)]
        public string QuestionId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; } // Size in bytes

        [Required]
        [Column(TypeName = "LONGTEXT")] // For MySQL to store large base64 strings
        public string Base64Content { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Response? Response { get; set; }
    }
}