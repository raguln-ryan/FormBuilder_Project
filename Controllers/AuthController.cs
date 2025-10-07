using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Auth;
using FormBuilder.API.Common;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var result = _authManager.Register(request);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _authManager.Login(request);
            if (!result.Success)
                return Unauthorized(result.Message);
            return Ok(result.Data);
        }
    }
}
