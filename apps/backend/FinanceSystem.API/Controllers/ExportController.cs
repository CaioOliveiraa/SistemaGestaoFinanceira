using System.Security.Claims;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/export")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ExportService _service;

        public ExportController(ExportService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("excel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Data inicial deve ser menor que a data final.");

            var userId = GetUserId();
            var fileBytes = await _service.ExportToExcelAsync(userId, startDate, endDate);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transacoes.xlsx");

        }

        [HttpGet("pdf")]
        public async Task<IActionResult> ExportToPdf([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("Data inicial deve ser menor que a data final.");

            var userId = GetUserId();
            var fileBytes = await _service.ExportToPdfAsync(userId, startDate, endDate);

            return File(fileBytes, "application/pdf", "transacoes.pdf");
        }
    }
}