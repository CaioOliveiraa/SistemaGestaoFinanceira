using System.Security.Claims;
using System.Text.Json;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using FinanceSystem.API.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace FinanceSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetRepository _passwordResetRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepo, IPasswordResetRepository passwordResetRepo, IEmailService emailService, IConfiguration config, ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _passwordResetRepo = passwordResetRepo;
            _emailService = emailService;
            _config = config;
            _logger = logger;
        }

        // — 1) Autenticação tradicional (email + senha)

        public async Task<(User user, string token)> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Credenciais inválidas");

            var senha = dto.Password.Trim();
            if (!BCrypt.Net.BCrypt.Verify(senha, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciais inválidas");

            return (user, GenerateJwt(user));
        }


        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _userRepo.GetByIdAsync(Guid.Parse(id));
            return user ?? throw new Exception("Usuário não encontrado");
        }

        // — 2) Geração de token avulso (caso precise)
        public Task<string> GenerateTokenAsync(User user)
        {
            return Task.FromResult(GenerateJwt(user));
        }

        // — 3) Busca ou cria usuário via OAuth externo
        public async Task<User> FindOrCreateExternalUserAsync(
            string provider, string providerId, string email, string name)
        {
            var user = await _userRepo.GetByExternalIdAsync(provider, providerId);
            if (user != null)
                return user;

            user = new User
            {
                Name = name,
                Email = email,
                Type = UserType.Common,
                OAuthProvider = provider,
                OauthId = providerId
            };

            await _userRepo.AddAsync(user);
            return user;
        }

        // Fluxo completo de login via Google (troca code → valida → emite JWT)
        public async Task<(User user, string token)> LoginWithGoogleAsync(string code)
        {
            // troca o código pelo IdToken do Google
            var googleTokens = await ExchangeCodeForTokenAsync(code);

            // valida o IdToken e extrai payload
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokens.IdToken);

            // encontra ou cria o usuário no banco
            var user = await FindOrCreateExternalUserAsync(
                provider: "Google",
                providerId: payload.Subject,
                email: payload.Email,
                name: payload.Name
            );

            // emite o JWT da aplicação
            var token = GenerateJwt(user);
            return (user, token);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email) ?? throw new InvalidOperationException("E-mail não cadastrado");

            var token = Guid.NewGuid().ToString("N");
            var expiry = DateTime.UtcNow.AddHours(1);

            var existing = await _passwordResetRepo.GetByUserIdAsync(user.Id);
            if (existing != null)
            {
                existing.Token = token;
                existing.ExpiresAt = expiry;
                existing.CreatedAt = DateTime.UtcNow;
                await _passwordResetRepo.UpdateAsync(existing);
            }
            else
            {
                var pr = new PasswordReset
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiresAt = expiry
                };
                await _passwordResetRepo.AddAsync(pr);
            }

            return token;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            var body = $@"
                <p>Para redefinir sua senha, clique <a href=""{resetLink}"">aqui</a>.</p>
                <p>Se não solicitou, ignore este e-mail.</p>";

            await _emailService.SendEmailAsync(email, "Recuperação de Senha", body);
        }

        public async Task ResetPasswordAsync(PasswordResetDto dto)
        {
            var pr = await _passwordResetRepo.GetByTokenAsync(dto.Token)
                ?? throw new InvalidOperationException("Token inválido ou expirado.");

            if (pr.ExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Token expirado.");

            var user = await _userRepo.GetByIdAsync(pr.UserId)
                ?? throw new InvalidOperationException("Usuário não encontrado.");

            var cleanPwd = dto.NewPassword.Trim();
            var newHash = BCrypt.Net.BCrypt.HashPassword(cleanPwd);

            user.PasswordHash = newHash;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);

            await _passwordResetRepo.DeleteAsync(pr);
        }


        // — Métodos auxiliares privados — //

        private async Task<GoogleTokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var values = new Dictionary<string, string>
            {
                ["client_id"] = _config["GOOGLE_CLIENT_ID"]!,
                ["client_secret"] = _config["GOOGLE_CLIENT_SECRET"]!,
                ["code"] = code,
                ["redirect_uri"] = _config["GOOGLE_REDIRECT_URI"]!,
                ["grant_type"] = "authorization_code"
            };

            using var http = new HttpClient();
            var response = await http.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(values)
            );
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GoogleTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (string.IsNullOrWhiteSpace(result?.IdToken))
                throw new Exception("ID Token não retornado pelo Google. Resposta completa: " + json);

            return result!;
        }

        private string GenerateJwt(User user)
        {
            var secret = _config["JwtSecret"]!;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,         user.Email),
                new Claim(ClaimTypes.Role,          user.Type.ToString())
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: "finance-api",
                audience: "finance-app",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        private class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;
            [JsonPropertyName("id_token")]
            public string IdToken { get; set; } = string.Empty;
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = string.Empty;
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("scope")]
            public string Scope { get; set; } = string.Empty;
        }
    }
}
