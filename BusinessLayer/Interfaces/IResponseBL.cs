using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.DTOs;
using FormBuilder.API.BusinessLayer.DTOs;

namespace FormBuilder.API.BusinessLayer.Interfaces
{
    public interface IResponseBL
    {
        Task<bool> SubmitResponseAsync(FormSubmissionDTO submissionDto);
        Task<List<ResponseDTO>> GetResponsesByFormAsync(int formId);
        Task<ResponseDTO> GetResponseByIdAsync(int formId, string responseId);
    }
}
