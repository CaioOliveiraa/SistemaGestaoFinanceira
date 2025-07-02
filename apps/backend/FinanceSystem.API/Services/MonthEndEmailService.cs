using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceSystem.API.Services;
using FinanceSystem.API.Dtos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class MonthEndEmailService : IHostedService, IDisposable
{
    private readonly ILogger<MonthEndEmailService> _logger;
    private readonly IServiceProvider _provider;
    private Timer? _timer;

    public MonthEndEmailService(ILogger<MonthEndEmailService> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Calcula o próximo 1º dia do mês + 5 minutos
        var now = DateTime.UtcNow;
        var next = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddMinutes(5);
        var initialDelay = next - now;

        // Timer configurado para disparar a cada 30 dias após o primeiro disparo
        _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromDays(30));

        _logger.LogInformation("MonthEndEmailService iniciado. Próximo envio em {next}", next);
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _provider.CreateScope();
        var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var reportSvc = scope.ServiceProvider.GetRequiredService<IReportService>();

        try
        {
            // Gera relatório do mês anterior
            var report = await reportSvc.GenerateMonthlyReportAsync(DateTime.UtcNow.AddMonths(-1));

            foreach (var userSummary in report)
            {
                var body = BuildEmailBody(userSummary);
                await emailSvc.SendEmailAsync(userSummary.Email, "Seu resumo financeiro do mês", body);
            }

            _logger.LogInformation("E-mails enviados com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mails.");
        }
    }

    private string BuildEmailBody(UserMonthlySummary summary)
    {
        var culture = new System.Globalization.CultureInfo("pt-BR");
        var dataReferencia = new DateTime(summary.Year, summary.Month, 1);
        var mesFormatado = culture.TextInfo.ToTitleCase(dataReferencia.ToString("MMMM/yyyy", culture));

        return $@"
        <div style='font-family: Arial, sans-serif; max-width:600px; margin:auto; border:1px solid #ddd; padding:20px;'>
            <h2 style='color:#2c3e50; border-bottom:1px solid #eee; padding-bottom:10px;'>
                Resumo Financeiro - {mesFormatado}
            </h2>
            <table style='width:100%; margin-top:20px;'>
                <tr>
                    <td style='padding:8px; font-weight:bold; color:#27ae60;'>Receita:</td>
                    <td style='padding:8px; color:#27ae60;'>R${summary.TotalIncome:F2}</td>
                </tr>
                <tr>
                    <td style='padding:8px; font-weight:bold; color:#c0392b;'>Despesa:</td>
                    <td style='padding:8px; color:#c0392b;'>R${summary.TotalExpense:F2}</td>
                </tr>
                <tr>
                    <td style='padding:8px; font-weight:bold; color:#2980b9;'>Saldo:</td>
                    <td style='padding:8px; color:#2980b9;'>R${summary.Balance:F2}</td>
                </tr>
            </table>
            <p style='font-size:12px; color:#888; margin-top:20px;'>
                Este é um resumo automático gerado pelo seu sistema de gestão financeira.
            </p>
        </div>";
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}
