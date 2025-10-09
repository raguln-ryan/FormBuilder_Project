using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Auth;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using FormBuilder.API.Common;

namespace FormBuilder.API.Business.Implementations
{
    public class AuthManager : IAuthManager
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtService _jwtService;

        public AuthManager(IUserRepository userRepository, PasswordHasher passwordHasher, JwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public (bool Success, string Message, AuthResponse? Data) Register(RegisterRequest request)
        {
            if (request.Role == Roles.Admin)
                return (false, "Admin cannot register via API", null);

            var existingUser = _userRepository.GetByEmail(request.Email);
            if (existingUser != null)
                return (false, "Email already exists", null);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = request.Role,
                PasswordHash = _passwordHasher.HashPassword(request.Password) // ✅ required set
            };

            _userRepository.Add(user);

            var token = _jwtService.GenerateToken(user);

            return (true, "User registered successfully", new AuthResponse
            {
                Token = token,
                UserId = user.Id.ToString(),
                Name = user.Name,
                Role = user.Role
            });
        }

        public (bool Success, string Message, AuthResponse? Data) Login(LoginRequest request)
        {
            var user = _userRepository.GetByEmail(request.Email);

            // Hardcoded Admin check
            if (user == null && request.Email == "admin@example.com" && request.Password == "Admin@123")
            {
                var adminUser = new User
                {
                    Id = 0,
                    Name = "Admin",
                    Email = "admin@example.com",
                    Role = Roles.Admin,
                    PasswordHash = _passwordHasher.HashPassword("Admin@123") // ✅ generate hash even for hardcoded admin
                };

                return (true, "Login successful", new AuthResponse
                {
                    Token = _jwtService.GenerateToken(adminUser),
                    UserId = "0",
                    Name = "Admin",
                    Role = Roles.Admin
                });
            }

            if (user == null)
                return (false, "User not found", null);

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return (false, "Invalid credentials", null);

            var token = _jwtService.GenerateToken(user);

            return (true, "Login successful", new AuthResponse
            {
                Token = token,
                UserId = user.Id.ToString(),
                Name = user.Name,
                Role = user.Role
            });
        }

    }
}
