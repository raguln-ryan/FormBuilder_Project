using FormBuilder.API.DTOs.Form;
using FormBuilder.API.DTOs.Common;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IFormManager
    {
        // Form Config - Creates and updates form metadata (title, description)
        (bool Success, string Message, FormConfigResponseDto? Data) CreateFormConfig(FormConfigRequestDto dto, string adminUser);
        (bool Success, string Message, FormConfigResponseDto? Data) UpdateFormConfig(string id, FormConfigRequestDto dto);

        // Form Layout - Only updates questions for existing forms
        (bool Success, string Message, FormLayoutResponseDto? Data) UpdateFormLayout(string formId, FormLayoutRequestDto dto, string adminUser);

        // Common operations
        (bool Success, string Message) DeleteForm(string id);
        (bool Success, string Message, PaginatedResponse<FormLayoutResponseDto> Data) GetAllForms(ClaimsPrincipal user, int pageNumber = 1, int pageSize = 10, string searchTerm = null);
        (bool Success, string Message, FormLayoutResponseDto? Data) GetFormById(string id, ClaimsPrincipal user);
        (bool Success, string Message) PublishForm(string id, string publishedBy);
    }
}
