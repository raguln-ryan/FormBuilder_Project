using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FormBuilder.API.Common.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues tokenHeader))
            {
                var token = tokenHeader.FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(token);
                        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims);
                        context.User = new ClaimsPrincipal(claimsIdentity);
                    }
                    catch
                    {
                        // Invalid token
                    }
                }
            }
            await _next(context);
        }
    }
}
