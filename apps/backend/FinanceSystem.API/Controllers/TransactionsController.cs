using System.Security.Claims;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{

    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _service;

        public TransactionsController(TransactionService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var transactions = await _service.GetAllAsync(userId);

            var dtoList = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Type = t.Type,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                Recurring = t.Recurring,
                CategoryId = t.CategoryId,
                CategoryName = t.Category?.Name
            });

            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var transaction = await _service.CreateAsync(dto, userId);

            return Created("", transaction);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var transaction = await _service.GetByIdAsync(id, userId);

            return transaction is null ? NotFound() : Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TransactionDto dto)
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