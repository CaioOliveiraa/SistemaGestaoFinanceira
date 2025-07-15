# Sistema de Gestão Financeira

Este repositório contém um sistema full-stack de gestão financeira pessoal/empresarial, composto por:

-   **Backend** em ASP.NET Core 8 (C#)
-   **Banco de Dados** PostgreSQL + EF Core Migrations
-   **Envio de E-mails** via SMTP (MailKit)
-   **Reset de Senha** com token via e-mail
-   **Autenticação**: JWT em cookie HTTP-Only + OAuth2 Google
-   **Frontend** Angular 19 standalone PWA

---

## 📁 Estrutura Geral

```bash
.
├── .env                       # Variáveis de ambiente (local/development)
├── docker-compose.yml         # Orquestra backend + PostgreSQL
├── apps
│   ├── backend                # Backend .NET 8
│   │   ├── appsettings.json   # Configurações (inclui FrontendUrl, JwtSecret, SMTP_*)
│   │   ├── Controllers        # AuthController, CategoryController, TransactionController
│   │   ├── Data               # DbContext + Migrations
│   │   ├── Dtos               # DTOs de transporte (login, reset, user, category, transaction)
│   │   ├── Models             # Entidades EF Core (User, Category, Transaction, PasswordReset)
│   │   ├── Repositories       # Interfaces e implementações de repositório
│   │   ├── Services           # Lógica de negócio (AuthService, CategoryService, TransactionService, MonthEndEmailService)
│   │   └── Program.cs         # Configuração de DI, DbContext, Swagger, SMTP, Hangfire-like scheduler
│   └── frontend               # Frontend Angular 19 standalone
│       ├── core
│       │   ├── services       # ApiService (HttpClient), AuthService
│       │   └── guards         # authGuard
│       ├── features
│       │   └── auth           # Login, Register, ForgotPassword, ResetPassword
│       ├── shared             # Models/DTOs compartilhados
│       ├── app.routes.ts      # Rotas lazy-loaded com guards
│       └── main.ts            # Bootstrapping Angular PWA
└── README.md                  # Este arquivo
```

---

## ⚙️ Pré-requisitos

-   Docker & Docker Compose
-   .NET 8 SDK
-   Node.js ≥ 18
-   Angular CLI (global)

---

## 🔧 Configuração de Ambiente

### 1. Variáveis de Ambiente (`.env`)

Crie um arquivo `.env` na raiz (será lido pelo Docker e pelo backend):

ini

```ini
# PostgreSQL
DB_HOST=postgres-finance
DB_PORT=5432
DB_NAME=finance_db
DB_USER=postgres
DB_PASS=postgres

# JWT
JwtSecret=UmSegredoLongoParaAssinarJWT

# Frontend
FrontendUrl=http://localhost:4200

# Google OAuth2
GOOGLE_CLIENT_ID=<sua-client-id>
GOOGLE_CLIENT_SECRET=<seu-client-secret>
GOOGLE_REDIRECT_URI=http://localhost:5062/api/auth/oauth/google/callback

# SMTP (MailKit)
SMTP_HOST=smtp.example.com
SMTP_PORT=587
SMTP_USER=user@example.com
SMTP_PASS=senha_smtp
SMTP_FROM=no-reply@seusistema.com
```

### 2. `appsettings.json`

Em `apps/backend/appsettings.json`, você terá novamente essas configurações (sem as credenciais secretas, que podem vir do `.env` ou de variáveis de ambiente em produção):

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "FrontendUrl": "http://localhost:4200",
    "JwtSecret": "",
    "GOOGLE_CLIENT_ID": "",
    "GOOGLE_CLIENT_SECRET": "",
    "GOOGLE_REDIRECT_URI": "",
    "SMTP_HOST": "",
    "SMTP_PORT": "",
    "SMTP_USER": "",
    "SMTP_PASS": "",
    "SMTP_FROM": ""
}
```

---

## 🚀 Executando com Docker

1.  Na raiz do projeto, rode:
    ```bash
    docker-compose up --build
    ```
2.  Isso criará e iniciará:

    -   **postgres-finance**: container PostgreSQL
    -   **backend-finance**: sua API .NET rodando em `http://localhost:5062`

3.  A API executará automaticamente as Migrations EF Core no primeiro start.

---

## 🖥️ Backend (.NET)

### Rodando Local

Dentro da pasta `apps/backend`, você pode executar:

```bash
dotnet run
```

### Endpoints Principais

| Rota                                  | Método | Descrição                                                  |
| ------------------------------------- | ------ | ---------------------------------------------------------- |
| `POST /api/auth/login`                | POST   | Login (email+senha) retorna `UserResponseDto`. Cookie JWT. |
| `POST /api/auth/logout`               | POST   | Remove cookie `jwt`.                                       |
| `POST /api/auth/forgot-password`      | POST   | Gera token e envia e-mail de recuperação (sempre 200).     |
| `POST /api/auth/reset-password`       | POST   | Recebe `{ token, newPassword }`, redefine senha.           |
| `GET /api/auth/me`                    | GET    | Retorna `UserResponseDto` do usuário logado.               |
| `GET /api/auth/oauth/google`          | GET    | Redireciona para OAuth2 Google.                            |
| `GET /api/auth/oauth/google/callback` | GET    | Callback para trocar code por token + login.               |

_NB: há também controllers de **Categories** e **Transactions** com CRUD completo e DTOs padronizados._

---

## 📦 Frontend (Angular 19 Standalone PWA)

### Setup

1.  Dentro de `apps/frontend` instale dependências:

    `npm install`

2.  Rode o servidor de desenvolvimento:

    `ng serve --open`

    O PWA ficará disponível em `http://localhost:4200`.

### Fluxo de Autenticação

-   **Login** → grava cookie `jwt`
-   **Forgot Password** → formulário de e-mail
-   **Reset Password** → lê `?token`, nova senha + confirmação
-   **Guards** → `authGuard` protege as rotas internas (e.g. Dashboard)

---

## 🔄 Migrations EF Core

Sempre que alterar `Models/`:

```bash
cd apps/backend
dotnet ef migrations add NomeDaMigration
dotnet ef database update`
```

-   As migrations ficam em `apps/backend/Data/Migrations`.
-   O `docker-compose up` aplica automaticamente ao criar o DB.

---

## 📝 Commit Estratégicos

Ao longo do desenvolvimento, foram realizados commits do tipo:

-   `chore: scaffolding inicial do projeto`
-   `feat: autenticação JWT + refresh de token`
-   `fix: reset-password POST em vez de PUT`
-   `feat: frontend standalone Angular 19 PWA com auth`
-   `docs: adiciona README completo do projeto`
