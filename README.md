# Sistema de Gestão Financeira

Este repositório contém um sistema full-stack de gestão financeira pessoal/empresarial, composto por:

-   **Backend** em ASP.NET Core 8 (C#)
-   **Banco de Dados** PostgreSQL + EF Core Migrations
-   **Envio de E-mails** via SMTP (MailKit)
-   **Reset de Senha** com token via e-mail
-   **Autenticação**: JWT em cookie HTTP-Only + OAuth2 Google
-   **Frontend** Angular 19 standalone 

---

## 📁 Estrutura Geral

```bash
/
├── .env # Variáveis de ambiente para Docker e local
├── docker-compose.yml # Orquestra backend + PostgreSQL
├── apps
│ ├── backend # Backend .NET Core 8
│ │ ├── appsettings.json # Configurações (FrontendUrl, JwtSecret, SMTP, OAuth)
│ │ ├── Controllers # Endpoints API
│ │ │ ├── AuthController.cs
│ │ │ ├── CategoryController.cs
│ │ │ └── TransactionController.cs
│ │ ├── Data # DbContext + Migrations EF Core
│ │ │ ├── FinanceDbContext.cs
│ │ │ └── Migrations/
│ │ ├── Dtos # Objetos de transferência (LoginDto, ResetPasswordDto, etc.)
│ │ ├── Models # Entidades (User, Category, Transaction, PasswordReset)
│ │ ├── Repositories # Interfaces e implementações de acesso a dados
│ │ ├── Services # Lógica de negócio e serviços (AuthService, CategoryService, MonthEndEmailService)
│ │ └── Program.cs # Configuração de DI, Swagger, CORS, SMTP, HostedService
│ └── frontend # Frontend Angular 19 standalone (PWA)
│ ├── src
│ │ ├── app
│ │ │ ├── core # CoreModule: ApiService, AuthService, AuthGuard, interceptors
│ │ │ ├── features
│ │ │ │ └── auth # Auth standalone components
│ │ │ │ ├── login
│ │ │ │ ├── register
│ │ │ │ ├── forgot-password
│ │ │ │ └── reset-password
│ │ │ ├── shared # Interfaces de modelos compartilhados (User, Category, Transaction)
│ │ │ ├── app.routes.ts # Rotas com lazy-load e guardas de autenticação
│ │ │ └── main.ts 
│ │ ├── assets
│ │ ├── environments # Configuração de endpoints por ambiente
│ │ └── index.html
│ └── angular.json # Configurações do Angular CLI
├── README.md # Este documento
└── .gitignore
```

---

## 🚀 Funcionalidades Principais

### Backend

- **Autenticação & Segurança**  
  - Login (email/senha) com BCrypt + JWT em cookie HTTP-Only  
  - Login social Google OAuth2  
  - Recuperação de senha via token (e-mail) e reset seguro  
- **Gestão Financeira**  
  - CRUD de **Categories** (fixas + personalizadas)  
  - CRUD de **Transactions** (receitas, despesas, recorrência, data)  
  - Exportação de relatórios em PDF/Excel  
- **Serviço Agendado**  
  - Envio automático mensal de resumo de ganhos/gastos por e-mail  
- **Infraestrutura**  
  - Containerização via Docker Compose  
  - Versionamento de esquema com EF Core Migrations  

### Frontend (Angular 19 Standalone)

- **PWA Offline-First**: cache inteligente e funcionamento sem conexão  
- **Fluxo de Autenticação**  
  - Componentes standalone para Login, Registro, Forgot & Reset Password  
  - AuthGuard para proteger rotas e interceptors para headers & erros  
- **Dashboard Interativo**  
  - Gráficos de fluxo de caixa e balanço mensal  
  - Tabela de histórico de transações com filtros avançados  
- **Formulários Reativos**  
  - Validações inline e mensagens de erro claras  
- **Lazy Loading** de módulos de feature  
- **Feedback ao Usuário**: snackbars e spinners durante ações  

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

```ini
# ConnectionString única usada pelo DbContext
DB_CONNECTION=Host=<HOST>;Port=<PORT>;Database=<DB_NAME>;Username=<DB_USER>;Password=<DB_PASS>

# JWT
JwtSecret=JwtSecret=<Your_JWT_Secret>

# Frontend (usado para gerar o link de reset_password)
FrontendUrl=http://localhost:4200

# Google OAuth2
GOOGLE_CLIENT_ID=<Your_Google_Client_ID>
GOOGLE_CLIENT_SECRET=<Your_Google_Client_Secret>
GOOGLE_REDIRECT_URI=<Your_Google_Redirect_URI>

# SMTP (para envio de e-mail de reset e resumo mensal)
SMTP_HOST=<Your_SMTP_Host>
SMTP_PORT=<Your_SMTP_Port>
SMTP_USER=<Your_SMTP_User>
SMTP_PASS=<Your_SMTP_Password>
SMTP_FROM=<No-Reply_Email_Address>
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
  "FrontendUrl": "http://localhost:4200"
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

| Rota                                             | Método   | Descrição                                        |
| ------------------------------------------------ | -------- | ------------------------------------------------ |
| **Auth**                                         |          |                                                   |
| `POST /api/auth/login`                           | POST     | Login (email+senha) retorna `UserResponseDto`. Cookie JWT. |
| `POST /api/auth/logout`                          | POST     | Remove cookie `jwt`.                             |
| `POST /api/auth/forgot-password`                 | POST     | Gera token e envia e-mail de recuperação (sempre 200). |
| `POST /api/auth/reset-password`                  | POST     | Recebe `{ token, newPassword }`, redefine senha. |
| `GET  /api/auth/me`                              | GET      | Retorna `UserResponseDto` do usuário logado.     |
| `GET  /api/auth/oauth/google`                    | GET      | Redireciona para OAuth2 Google.                  |
| `GET  /api/auth/oauth/google/callback`           | GET      | Callback para trocar code por token + login.     |
| **Categories**                                   |          |                                                   |
| `GET    /api/categories`                         | GET      | Lista todas as categorias do usuário.            |
| `GET    /api/categories/{id}`                    | GET      | Detalha uma categoria por ID.                    |
| `POST   /api/categories`                         | POST     | Cria nova categoria (`CategoryDto`).             |
| `PUT    /api/categories/{id}`                    | PUT      | Atualiza categoria existente.                    |
| `DELETE /api/categories/{id}`                    | DELETE   | Remove categoria.                                |
| **Transactions**                                 |          |                                                   |
| `GET    /api/transactions`                       | GET      | Lista todas as transações do usuário.            |
| `GET    /api/transactions/{id}`                  | GET      | Detalha uma transação por ID.                    |
| `POST   /api/transactions`                       | POST     | Cria nova transação (`TransactionDto`).          |
| `PUT    /api/transactions/{id}`                  | PUT      | Atualiza transação existente.                    |
| `DELETE /api/transactions/{id}`                  | DELETE   | Remove transação.                                |

---

### Rotas Principais (Frontend)

```text
/auth/login              → LoginComponent
/auth/register           → RegisterComponent
/auth/forgot-password    → ForgotPasswordComponent
/auth/reset-password     → ResetPasswordComponent
/dashboard               → DashboardComponent
/categories              → CategoryListComponent
/categories/new          → CategoryFormComponent (create)
/categories/:id/edit     → CategoryFormComponent (edit)
/transactions            → TransactionListComponent
/transactions/new        → TransactionFormComponent (create)
/transactions/:id/edit   → TransactionFormComponent (edit)
```
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
