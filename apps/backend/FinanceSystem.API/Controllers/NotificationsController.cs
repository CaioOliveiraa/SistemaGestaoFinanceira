using FinanceSystem.API.Dtos;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public NotificationsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendEmailAsync(dto.To, dto.Subject, dto.Body);
            return Ok(new { message = "E-mail (mock) enviado com sucesso." });
        }
    }
}
