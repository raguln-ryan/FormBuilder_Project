using FormBuilder.API.DTOs.Form;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IFormManager
    {
        // Form Config
        (bool Success, string Message, FormConfigDto? Data) CreateFormConfig(FormConfigDto dto, string adminUser);
        (bool Success, string Message, FormConfigDto? Data) UpdateFormConfig(string id, FormConfigDto dto);

        // Form Layout
        (bool Success, string Message, FormLayoutDto? Data) CreateFormLayout(FormLayoutDto dto, string adminUser);
        (bool Success, string Message, FormLayoutDto? Data) UpdateFormLayout(string id, FormLayoutDto dto);

        // Common
        (bool Success, string Message) DeleteForm(string id);
        object GetAllForms(ClaimsPrincipal user);
        (bool Success, string Message, FormLayoutDto? Data) GetFormById(string id, ClaimsPrincipal user);
        (bool Success, string Message) PublishForm(string id, string publishedBy);
    }
}
