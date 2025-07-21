# Sistema de Gestão Financeira

Este repositório reúne uma aplicação completa para controle financeiro pessoal ou empresarial. O projeto é composto por um backend em ASP.NET e um frontend em Angular, além de um container PostgreSQL para armazenamento dos dados.

## Tabela de Conteúdo
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuracao)
  - [Variáveis de ambiente](#variaveis-de-ambiente)
  - [Executar com Docker](#executar-com-docker)
  - [Executar manualmente](#executar-manualmente)
- [Estrutura do repositório](#estrutura-do-repositorio)
- [Migrations](#migrations)
- [Testes](#testes)

## Funcionalidades

### Backend

- Autenticação via JWT (cookie HTTP-only) e login social Google
- Recuperação de senha por token enviado por e-mail
- CRUD de categorias e transações financeiras
- Geração de relatórios em PDF/Excel
- Envio automático mensal de resumo financeiro por e-mail

### Frontend
- Aplicação Angular 19 no formato standalone (PWA)
- Fluxo completo de autenticação e proteção de rotas
- Dashboard com gráficos e histórico de transações

## Tecnologias
- **ASP.NET Core 8** com Entity Framework Core
- **Angular 19**
- **PostgreSQL** via Docker
- **Docker Compose** para orquestração de ambientes

## Pré-requisitos
- Docker e Docker Compose
- .NET SDK 8
- Node.js 18 ou superior

## Configuração
### Variáveis de ambiente
Crie um arquivo `.env` na raiz com as seguintes chaves:

```ini

DB_CONNECTION=Host=<HOST>;Port=<PORT>;Database=<DB_NAME>;Username=<DB_USER>;Password=<DB_PASS>
JwtSecret=<SUA_CHAVE_JWT>
FrontendUrl=http://localhost:4200

GOOGLE_CLIENT_ID=<ID_GOOGLE>
GOOGLE_CLIENT_SECRET=<SEGREDO_GOOGLE>
GOOGLE_REDIRECT_URI=<REDIRECT_URI>
SMTP_HOST=<SERVIDOR_SMTP>
SMTP_PORT=<PORTA_SMTP>
SMTP_USER=<USUARIO_SMTP>
SMTP_PASS=<SENHA_SMTP>
SMTP_FROM=<EMAIL_REMETENTE>
```

---
### Executar com Docker
Na raiz do projeto:

```bash
docker-compose up --build
```
O banco de dados ficará acessível em `localhost:5432` e a API em `localhost:3001`.

### Executar manualmente
1. **Backend**
   ```bash
   cd apps/backend/FinanceSystem.API
   dotnet run
   ```
2. **Frontend**
   ```bash
   cd apps/frontend
   npm install
   npx ng serve --open
   ```
A aplicação web abrirá em `http://localhost:4200`.

## Estrutura do repositório
```
/
├── docker-compose.yml
├── apps
│   ├── backend
│   │   └── FinanceSystem.API
│   └── frontend
└── README.md
```
---
## Migrations
Quando alterar as entidades do backend execute:

```bash
cd apps/backend/FinanceSystem.API
dotnet ef migrations add <NomeDaMigration>
dotnet ef database update
```
## Testes
Os testes de frontend podem ser executados com:
```bash
cd apps/frontend
npm test
```
