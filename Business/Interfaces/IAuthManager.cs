using FormBuilder.API.DTOs.Auth;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IAuthManager
    {
        (bool Success, string Message, AuthResponse Data) Register(RegisterRequest request);
        (bool Success, string Message, AuthResponse Data) Login(LoginRequest request);
    }
}
