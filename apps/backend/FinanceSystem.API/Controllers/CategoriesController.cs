using System.Security.Claims;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var results = await _service.GetAllAsync(userId);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var category = await _service.CreateAsync(dto, userId);

            return Created($"api/categories/{category.Id}", category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);

            return updated is null ? NotFound() : Ok(updated); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var success = await _service.DeleteAsync(id, userId);

            return success ? NoContent() : NotFound();
        }
    }
}