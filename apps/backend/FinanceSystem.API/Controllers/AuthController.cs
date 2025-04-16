using System.Security.Claims;
using AutoMapper;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // login retorna o user e o token
                var (user, token) = await _authService.LoginAsync(dto);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(3)
                });

                var userDto = _mapper.Map<UserResponseDto>(user);
                return Ok(userDto);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { error = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
