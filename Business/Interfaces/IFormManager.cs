using FormBuilder.API.DTOs.Form;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IFormManager
    {
        (bool Success, string Message, FormLayoutDto Data) CreateForm(FormLayoutDto dto);
        (bool Success, string Message, FormLayoutDto Data) UpdateForm(string id, FormLayoutDto dto);
        (bool Success, string Message) DeleteForm(string id);
        object GetAllForms(ClaimsPrincipal user);
        (bool Success, string Message, FormLayoutDto Data) GetFormById(string id, ClaimsPrincipal user);
        (bool Success, string Message) PublishForm(string id);
        (bool Success, string Message) DraftForm(string id);
        (bool Success, string Message, FormLayoutDto Data) PreviewForm(string id);
    }
}
