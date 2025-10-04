using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.BusinessLayer.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.DataAccessLayer.Interfaces;
using AutoMapper;
using FormBuilder.API.BusinessLayer.DTOs;

namespace FormBuilder.API.BusinessLayer.Implementations
{
    public class ResponseBL : IResponseBL
    {
        private readonly IResponseDAL _responseDAL;
        private readonly IMapper _mapper;

        public ResponseBL(IResponseDAL responseDAL, IMapper mapper)
        {
            _responseDAL = responseDAL;
            _mapper = mapper;
        }

        public async Task<bool> SubmitResponseAsync(FormSubmissionDTO submissionDto)
        {
            var response = _mapper.Map<Models.Document.Response>(submissionDto);
            return await _responseDAL.SubmitResponseAsync(response);
        }

        public async Task<List<ResponseDTO>> GetResponsesByFormAsync(int formId)
        {
            var responses = await _responseDAL.GetResponsesByFormAsync(formId);
            return _mapper.Map<List<ResponseDTO>>(responses);
        }

        public async Task<ResponseDTO> GetResponseByIdAsync(int formId, string responseId)
        {
            var response = await _responseDAL.GetResponseByIdAsync(formId, responseId);
            return _mapper.Map<ResponseDTO>(response);
        }
    }
}
