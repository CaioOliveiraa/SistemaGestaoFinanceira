using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceSystem.API.Dtos;
using FinanceSystem.API.Helpers;
using FinanceSystem.API.Models;
using FinanceSystem.API.Repositories.Interfaces;
using FinanceSystem.API.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace FinanceSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        // — 1) Autenticação tradicional (email + senha)
        public async Task<(User user, string token)> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciais inválidas");

            var token = GenerateJwt(user);
            return (user, token);
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
