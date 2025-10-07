using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    public class Response : BaseEntity
    {
       

        [ForeignKey("Form")]
        public required int FormId { get; set; }  // Now integer

        [ForeignKey("User")]
        public required int UserId { get; set; }  // Changed from string → int

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public List<ResponseDetail> Details { get; set; } = new List<ResponseDetail>();

        // Navigation properties
        public User? User { get; set; }
    }
}
