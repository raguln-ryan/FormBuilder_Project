using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.BusinessLayer.Interfaces;
using FormBuilder.API.BusinessLayer.DTOs;
using FormBuilder.API.DTOs;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseBL _responseBL;

        public ResponseController(IResponseBL responseBL)
        {
            _responseBL = responseBL;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitResponse([FromBody] FormSubmissionDTO submissionDto)
        {
            var result = await _responseBL.SubmitResponseAsync(submissionDto);
            return Ok(result);
        }

        [HttpGet("{formId}")]
        public async Task<IActionResult> GetResponsesByForm(int formId)
        {
            var result = await _responseBL.GetResponsesByFormAsync(formId);
            return Ok(result);
        }

        [HttpGet("{formId}/{responseId}")]
        public async Task<IActionResult> GetResponseById(int formId, string responseId)
        {
            var result = await _responseBL.GetResponseByIdAsync(formId, responseId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
