using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.BusinessLayer.Interfaces;
using FormBuilder.API.DTOs;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormBL _formBL;

        public FormController(IFormBL formBL)
        {
            _formBL = formBL;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            var result = await _formBL.GetAllFormsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormById(int id)
        {
            var result = await _formBL.GetFormByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromBody] FormMetadataDTO formDto)
        {
            var result = await _formBL.CreateFormAsync(formDto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] FormMetadataDTO formDto)
        {
            var result = await _formBL.UpdateFormAsync(id, formDto);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            var result = await _formBL.DeleteFormAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishForm(int id)
        {
            var result = await _formBL.PublishFormAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpPost("{id}/sections")]
        public async Task<IActionResult> AddSection(int id, [FromBody] SectionDTO sectionDto)
        {
            var result = await _formBL.AddSectionAsync(id, sectionDto);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
