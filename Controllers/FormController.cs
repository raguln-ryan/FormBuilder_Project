using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormManager _formManager;

        public FormController(IFormManager formManager)
        {
            _formManager = formManager;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreateForm([FromBody] FormLayoutDto dto)
        {
            var result = _formManager.CreateForm(dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateForm(string id, [FromBody] FormLayoutDto dto)
        {
            var result = _formManager.UpdateForm(id, dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DeleteForm(string id)
        {
            var result = _formManager.DeleteForm(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllForms()
        {
            var result = _formManager.GetAllForms(User);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetFormById(string id)
        {
            var result = _formManager.GetFormById(id, User);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult PublishForm(string id)
        {
            var result = _formManager.PublishForm(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPost("{id}/draft")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DraftForm(string id)
        {
            var result = _formManager.DraftForm(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("{id}/preview")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult PreviewForm(string id)
        {
            var result = _formManager.PreviewForm(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }
    }
}
