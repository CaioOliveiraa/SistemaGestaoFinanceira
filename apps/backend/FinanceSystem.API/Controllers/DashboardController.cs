using System.Security.Claims;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/Dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;

        public DashboardController(DashboardService service)
        {
            _service = service;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = GetUserId();
            var summary = await _service.GetSymmaryAsync(userId);
            return Ok(summary);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly()
        {
            var userId = GetUserId();
            var result = await _service.GetMonthlyAsync(userId);
            return Ok(result);
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetByCategory()
        {
            var userId = GetUserId();
            var result = await _service.GetByCategoryAsync(userId);
            return Ok(result);
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily()
        {
            var userId = GetUserId();
            var result = await _service.GetDailyAsync(userId);
            return Ok(result);
        }

    }
}