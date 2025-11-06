using FormBuilder.API.DTOs.Form;
using FormBuilder.API.DTOs.Common;
using FormBuilder.API.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
        public interface IResponseManager
        {
            (bool Success, string Message, PaginatedResponse<FormLayoutResponseDto> Data) GetPublishedForms(int pageNumber = 1, int pageSize = 10, string searchTerm = null);
            FormLayoutResponseDto GetFormById(string formId);
            
            // Add search parameter for viewing form responses
            (bool Success, string Message, PaginatedResponse<object> Data) GetResponsesByForm(string formId, int pageNumber = 1, int pageSize = 10, string searchTerm = null);
            
            (bool Success, string Message, Response? Data) GetResponseById(string responseId);
            (bool Success, string Message, Response? Data) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user);
            (bool Success, string Message, FileAttachment? Data) GetFileAttachment(int responseId, string questionId);
            (bool Success, string Message, object? Data) GetResponseWithFiles(int responseId);
        
            // Add search and pagination for user submissions
            (bool Success, string Message, PaginatedResponse<object> Data) GetUserSubmissions(int userId, int pageNumber = 1, int pageSize = 10, string searchTerm = null);
        }
}
