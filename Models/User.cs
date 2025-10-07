namespace FormBuilder.API.Models
{
    public class User : BaseEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; } // Admin or Learner

        // Navigation property
        public List<Response>? Responses { get; set; }
    }
}
