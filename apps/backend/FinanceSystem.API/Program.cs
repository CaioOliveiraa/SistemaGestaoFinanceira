using FinanceSystem.API.Data;
using Microsoft.EntityFrameworkCore;
using FinanceSystem.API.Repositories;
using FinanceSystem.API.Repositories.Interfaces;
using FinanceSystem.API.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Carregar variáveis do arquivo .env
Env.Load(); // ← isso carrega o .env na raiz do projeto

// Configura o DbContext com a string de conexão do .env
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();
