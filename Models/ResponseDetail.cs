using System.ComponentModel.DataAnnotations.Schema;

namespace FormBuilder.API.Models
{
    public class ResponseDetail : BaseEntity
    {
       

        [ForeignKey("Response")]
        public int ResponseId { get; set; } // FK to Response table

        // MongoDB question ID remains string
        public string QuestionId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        public Response? Response { get; set; }
    }
}
