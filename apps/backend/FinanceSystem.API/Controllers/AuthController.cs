using System.Security.Claims;
using AutoMapper;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FinanceSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthController(AuthService authService, IMapper mapper, IConfiguration config)
        {
            _authService = authService;
            _mapper = mapper;
            _config = config;
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
                    SameSite = SameSiteMode.None,
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _authService.GeneratePasswordResetTokenAsync(dto.Email);

                // Monta o link
                var frontendUrl = _config["FrontendUrl"]?.TrimEnd('/') ?? throw new InvalidOperationException("FrontendUrl não configurado");
                var resetLink = QueryHelpers.AddQueryString(
                    $"{frontendUrl}/auth/reset-password",
                    "token", token
                );

                await _authService.SendPasswordResetEmailAsync(dto.Email, resetLink);

                return Ok(new { message = "E-mail de redefinição de senha enviado" });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.ResetPasswordAsync(dto);
                return Ok(new { message = "Senha redefinida com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário a partir do token

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { error = "Usuário não autenticado" });

                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(new { error = "Usuário não encontrado" });

                var userDto = _mapper.Map<UserResponseDto>(user);
                return Ok(userDto);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        /// <summary>
        /// Inicia o fluxo OAuth2 com o Google: redireciona para o consent screen.
        /// </summary>
        [HttpGet("oauth/google")]
        public IActionResult GoogleLogin()
        {
            var clientId = _config["GOOGLE_CLIENT_ID"]!;
            var redirectUri = _config["GOOGLE_REDIRECT_URI"]!;
            var state = Guid.NewGuid().ToString("N");

            // Construímos a URL de autorização do Google
            var url = QueryHelpers.AddQueryString(
                "https://accounts.google.com/o/oauth2/v2/auth",
                new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["redirect_uri"] = redirectUri,
                    ["response_type"] = "code",
                    ["scope"] = "openid email profile",
                    ["state"] = state
                });

            // Redireciona o browser do usuário para o Google
            return Redirect(url);
        }


        /// <summary>
        /// Callback que o Google chama com o código de autorização.
        /// Aqui trocamos o code, validamos o ID token e geramos nosso JWT.
        /// </summary>
        [HttpGet("oauth/google/callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            try
            {
                var (user, token) = await _authService.LoginWithGoogleAsync(code);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(3)
                });

                var frontend = _config["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:4200";
                var callbackUrl = $"{frontend}/auth/oauth-callback";
                return Redirect(callbackUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao processar callback do Google: " + ex);
                var frontend = _config["FrontendUrl"]?.TrimEnd('/') ?? "http://localhost:4200";
                var callbackUrl = $"{frontend}/auth/oauth-callback?error=1";
                return Redirect(callbackUrl);
            }
        }


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new { message = "Logout efetuado com sucesso" });
        }
    }
}
