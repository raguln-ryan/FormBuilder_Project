using FormBuilder.API.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormBuilder.API.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly int _expiryMinutes;

        public JwtService(string secret, int expiryMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(secret) || Encoding.ASCII.GetBytes(secret).Length < 32)
                throw new ArgumentException("JWT secret must be at least 32 bytes long", nameof(secret));

            _secret = secret;
            _expiryMinutes = expiryMinutes;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // int -> string
                    new Claim(ClaimTypes.Name, user.Name ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
