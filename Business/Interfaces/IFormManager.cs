using FormBuilder.API.DTOs.Form;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IFormManager
    {
        // Form Config
        (bool Success, string Message, FormConfigResponseDto? Data) CreateFormConfig(FormConfigRequestDto dto, string adminUser);
        (bool Success, string Message, FormConfigResponseDto? Data) UpdateFormConfig(string id, FormConfigRequestDto dto);

        // Form Layout
        (bool Success, string Message, FormLayoutResponseDto? Data) CreateFormLayout(FormLayoutRequestDto dto, string adminUser);
        (bool Success, string Message, FormLayoutResponseDto? Data) UpdateFormLayout(string id, FormLayoutRequestDto dto);

        // Common
        (bool Success, string Message) DeleteForm(string id);
        object GetAllForms(ClaimsPrincipal user);
        (bool Success, string Message, FormLayoutResponseDto? Data) GetFormById(string id, ClaimsPrincipal user);
        (bool Success, string Message) PublishForm(string id, string publishedBy);
    }
}
