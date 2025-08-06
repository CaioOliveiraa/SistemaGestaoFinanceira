using System.Text;
using DotNetEnv;
using FinanceSystem.API.Data;
using FinanceSystem.API.Mappings;
using FinanceSystem.API.Repositories;
using FinanceSystem.API.Repositories.Interfaces;
using FinanceSystem.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Carregar variáveis do arquivo .env
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Registrar o DbContext com PostgreSQL
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"))
);

// Registro de dependências (injeção de dependência)
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddScoped<DashboardService>();

builder.Services.AddScoped<ExportService>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddHostedService<MonthEndEmailService>();

// Configuração do JWT
var jwtSecret = Environment.GetEnvironmentVariable("JwtSecret");

// Console.WriteLine($"🔐 JwtSecret carregado: {jwtSecret}");
var key = Encoding.UTF8.GetBytes(jwtSecret ?? "");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "finance-api",
            ValidateAudience = true,
            ValidAudience = "finance-app",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        // Extrair o token do cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors
var corsPolicyName = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsPolicyName,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
