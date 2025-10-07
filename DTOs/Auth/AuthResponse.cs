namespace FormBuilder.API.DTOs.Auth
{
    public class AuthResponse
    {
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; }
    }
}
