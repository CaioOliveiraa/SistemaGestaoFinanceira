using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FinanceSystem.API.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // 1) Monta a mensagem MIME
            var message = new MimeMessage();

            // Define o "From" a partir da variável SMTP_FROM (.env)
            // Ex.: SMTP_FROM="Finance System <noreply@seu-dominio.com>"
            var fromAddress = _config["SMTP_USER"];
            if (string.IsNullOrWhiteSpace(fromAddress))
                throw new InvalidOperationException("SMTP_USER não foi configurado.");

            message.From.Add(MailboxAddress.Parse(fromAddress));

            // Destinatário
            message.To.Add(MailboxAddress.Parse(to));

            // Assunto
            message.Subject = subject;

            // Corpo em HTML
            message.Body = new TextPart("html")
            {
                Text = body
            };

            // 2) Envia via SMTP
            using var client = new SmtpClient();
            try
            {
                // Conecta ao servidor SMTP com STARTTLS (porta 587)
                var host = _config["SMTP_HOST"];
                var port = int.Parse(_config["SMTP_PORT"] ?? "587");
                await client.ConnectAsync(host!, port, MailKit.Security.SecureSocketOptions.StartTls);

                // Autentica com usuário e senha
                var user = _config["SMTP_USER"];
                var pass = _config["SMTP_PASS"];
                await client.AuthenticateAsync(user!, pass!);

                // Envia o e-mail
                await client.SendAsync(message);

                // Desconecta
                await client.DisconnectAsync(true);

                _logger.LogInformation("E-mail enviado com sucesso para {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail para {To}", to);
                throw;
            }
        }
    }
}
