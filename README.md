
# Sistema de Gestão Financeira

Este repositório reúne uma aplicação completa para controle financeiro pessoal ou empresarial. O projeto é composto por um backend em **ASP.NET Core** e um frontend em **Angular**, além de um container PostgreSQL para armazenamento dos dados.  
Em produção, o backend também serve o frontend, permitindo deploy único no **Azure App Service**.

## Tabela de Conteúdo

-   [Funcionalidades](#funcionalidades)
    
-   [Tecnologias](#tecnologias)
    
-   [Pré-requisitos](#pr%C3%A9-requisitos)
    
-   [Configuração](#configuracao)
    
    -   [Variáveis de ambiente](#variaveis-de-ambiente)
        
    -   [Executar com Docker](#executar-com-docker)
        
    -   [Executar manualmente](#executar-manualmente)
        
-   [Estrutura do repositório](#estrutura-do-repositorio)
    
-   [Migrations](#migrations)
    
-   [Testes](#testes)
    

## Funcionalidades

### Backend

-   Autenticação via JWT (cookie HTTP-only) e login social Google
    
-   Recuperação de senha por token enviado por e-mail
    
-   CRUD de categorias e transações financeiras
    
-   Geração de relatórios em PDF/Excel
    
-   Envio automático mensal de resumo financeiro por e-mail
    
-   **Servir aplicação Angular integrada em produção (deploy único no Azure)**
    

### Frontend

-   Aplicação Angular 19 no formato standalone
    
-   Fluxo completo de autenticação e proteção de rotas
    
-   Dashboard com gráficos e histórico de transações
    
-   Em produção, é entregue diretamente pelo backend .NET
    

## Tecnologias

-   **ASP.NET Core 8** com Entity Framework Core
    
-   **Angular 19**
    
-   **PostgreSQL** via Docker
    
-   **Docker Compose** para orquestração de ambientes
    

## Pré-requisitos

-   **Docker** e **Docker Compose**
    
-   **.NET SDK 8**
    
-   **Node.js 18 ou superior** (apenas necessário para desenvolvimento local do frontend)
    

## Configuração

### Variáveis de ambiente

Crie um arquivo `.env` na **raiz do projeto** com as seguintes chaves (em produção, configure-as no Azure App Service → Configurações do aplicativo):
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

----------

### Executar com Docker

Na raiz do projeto:

`docker-compose up --build` 

Este comando sobe **backend, frontend e banco de dados** em um único ambiente integrado.

-   API acessível em `http://localhost:3001`
    
-   Frontend acessível no mesmo endereço, servido pelo backend
    

----------

### Executar manualmente (desenvolvimento)

Para desenvolvimento, é recomendado rodar backend e frontend separadamente para aproveitar o hot reload.

1.  **Backend**
    
    
```bash
   cd apps/backend/FinanceSystem.API
   dotnet run
   ```
   
    
2.  **Frontend**
   ```bash
	   cd apps/frontend
	   npm install
	   npx ng serve --open
   ```
    
💡 Em produção, o backend serve o build do Angular. Para simular isso localmente:
```bash
	cd apps/frontend
	npm install
	ng build --configuration production
```

E depois rodar o backend normalmente (`dotnet run`).

----------

## Estrutura do repositório
```
/
├── docker-compose.yml
├── .env
├── apps
│   ├── backend
│   │   └── FinanceSystem.API
│   └── frontend
│       └── dist  # build final do Angular para produção
└── README.md
```
----------

## Migrations

Quando alterar as entidades do backend execute:
```bash
	cd apps/backend/FinanceSystem.API
	dotnet ef migrations add <NomeDaMigration>
	dotnet ef database update
```

----------

## Testes

Os testes de frontend podem ser executados com:

`cd apps/frontend
npm test`
