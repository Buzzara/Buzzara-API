# Buzzara API

API backend da plataforma Buzzara, desenvolvida em ASP.NET Core 8 com Entity Framework Core e SQL Server.

Este projeto concentra autenticação, cadastro de usuários, criação de perfis, anúncios, mídia, dados públicos, dashboard, pagamentos e serviços auxiliares da aplicação.

## Visão Geral

O sistema foi estruturado como uma API REST em .NET 8.

Hoje o projeto entrega principalmente:

- autenticação com JWT
- cadastro de usuário com validação
- gerenciamento de usuários
- criação e atualização de perfil de acompanhante
- criação, edição e listagem de anúncios
- upload de fotos e vídeos
- visualização pública de anúncios e perfis
- métricas básicas de dashboard
- fluxo inicial de pagamentos para anúncios
- suporte a localização via GeoNames

## Stack Tecnológica

Tecnologias e bibliotecas principais:

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- JWT Bearer Authentication
- BCrypt para hash de senha
- Docker
- Docker Compose
- Swagger / Swashbuckle
- QRCoder
- FFMpegCore
- DotNetEnv

Pacotes relevantes usados no projeto:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- `BCrypt.Net-Next`
- `Swashbuckle.AspNetCore`
- `QRCoder`
- `FFMpegCore`

## Arquitetura do Projeto

O projeto segue uma organização simples por responsabilidades:

- `Controllers`: endpoints HTTP
- `Services`: regras de negócio
- `Models`: entidades persistidas no banco
- `DTOs`: contratos de entrada e saída
- `Data`: `DbContext` e bootstrap de banco
- `Migrations`: migration inicial do banco
- `Filters`: filtros auxiliares
- `Properties`: configuração de execução local
- `wwwroot/uploads`: arquivos enviados pela aplicação em runtime

Fluxo principal da aplicação:

1. O controller recebe a requisição.
2. O service executa a regra de negócio.
3. O `ApplicationDbContext` persiste ou consulta os dados.
4. A API responde via DTO ou objeto de retorno.

## Estrutura de Pastas

```text
Buzzara-API/
├── README.md
├── buzzaraApi.sln
├── dotnet-tools.json
├── buzzaraApi/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Filters/
│   ├── Migrations/
│   ├── Models/
│   ├── Properties/
│   ├── Services/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── docker-compose.db.yml
│   ├── .env.example
│   ├── appsettings.json
│   └── buzzaraApi.csproj
└── azure-pipelines.yml
```

## Principais Módulos da API

### Autenticação

Arquivos principais:

- `AuthController`
- `AuthService`

Responsabilidades:

- login
- refresh token
- logout
- recuperação e redefinição de senha
- alteração de senha
- endpoint `me`

### Usuários

Arquivos principais:

- `UsuariosController`
- `UsuarioService`
- `NovoUsuarioController`
- `NovoUsuarioService`

Responsabilidades:

- cadastro de usuário
- validação de conta
- perfil do usuário autenticado
- atualização de fotos de perfil e capa

### Perfis de Acompanhante

Arquivos principais:

- `PerfisAcompanhantesController`
- `PerfilAcompanhanteService`

Responsabilidades:

- criar perfil
- buscar perfil
- atualizar perfil
- deletar perfil

### Anúncios

Arquivos principais:

- `AnunciosController`
- `ServicoService`
- `PublicoAnunciosController`
- `AnuncioPublicoService`

Responsabilidades:

- criar anúncio
- editar anúncio
- remover anúncio
- upload de fotos
- upload de vídeo
- listar anúncios do usuário
- listar anúncios públicos

### Pagamentos

Arquivos principais:

- `PagamentosController`
- `PagamentoService`
- `QRCodeService`

Responsabilidades:

- finalizar anúncio
- gerar QR code
- confirmar pagamento

### Dashboard

Arquivos principais:

- `DashboardController`
- `DashboardService`

Responsabilidades:

- total de anúncios
- total de fotos
- total de vídeos
- último upload

### Público e Localização

Arquivos principais:

- `PublicoController`
- `PublicoService`
- `LocalidadesController`
- `GeoNamesService`

Responsabilidades:

- perfil público
- listagem pública de anúncios
- busca de localidades próximas

## Modelos de Dados

Entidades principais do banco:

- `Usuario`
- `PerfilAcompanhante`
- `Servico`
- `FotoAcompanhante`
- `VideoAcompanhante`
- `FotoAnuncio`
- `VideoAnuncio`
- `Localizacao`
- `SobreUsuario`
- `ServicoCache`
- `PagamentoAnuncio`
- `Agendamento`
- `HorarioAtendimento`

Resumo das relações:

- um `Usuario` pode ter um ou mais `PerfisAcompanhantes`
- um `PerfilAcompanhante` pertence a um `Usuario`
- um `PerfilAcompanhante` pode ter vários `Servicos`
- um `Servico` pode ter fotos, vídeos, localização, horários, cache e dados complementares
- pagamentos são associados a `Servicos`
- agendamentos relacionam usuário cliente com perfil acompanhante

## Banco de Dados

O banco usado no projeto é SQL Server.

O projeto foi reorganizado para iniciar com uma migration única e limpa, baseada no modelo atual:

- migration atual: `InitialCreate`

Na inicialização da API:

1. a aplicação tenta conectar no banco
2. aplica `Database.Migrate()`
3. faz o seed do admin inicial, se ele ainda não existir

Arquivo responsável pelo bootstrap:

- [DatabaseInitializationExtensions.cs](/C:/Users/natan/source/repos/Buzzara-API/buzzaraApi/Data/DatabaseInitializationExtensions.cs)

### Seed Inicial

Por padrão, o projeto cria:

- e-mail: `admin@buzzara.com.br`
- senha: `123456`
- role: `admin`

Esse seed é idempotente.

Se o usuário já existir, ele não será recriado.

## Configuração por Ambiente

Arquivos relevantes:

- `buzzaraApi/appsettings.json`
- `buzzaraApi/.env.example`
- `buzzaraApi/.env`

Variáveis importantes:

- `MSSQL_SA_PASSWORD`
- `DB_NAME`
- `DB_PORT`
- `API_PORT`
- `JWT_SECRET`
- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `BACKEND_URL`
- `FRONTEND_URL`
- `PUBLIC_BASE_URL`
- `SEED_ADMIN_EMAIL`
- `SEED_ADMIN_PASSWORD`

## Requisitos Para Rodar o Projeto

Para desenvolvimento local com Docker:

- Git
- Docker Desktop
- Docker Compose

Para desenvolvimento local sem Docker:

- .NET SDK 8
- SQL Server

Opcional para manutenção de migrations:

- `dotnet-ef` via tool manifest já configurado no repositório

## Como Baixar do GitHub

### 1. Clonar o repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd Buzzara-API
```

### 2. Entrar na pasta da API

```bash
cd buzzaraApi
```

## Como Subir Com Docker

### 1. Criar o arquivo `.env`

No Linux/macOS:

```bash
cp .env.example .env
```

No Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

### 2. Ajustar o `.env`

Preencha pelo menos:

```env
MSSQL_SA_PASSWORD=SuaSenhaForteAqui
JWT_SECRET=SeuSegredoJwtAqui
```

Se quiser, também pode alterar:

- porta da API
- porta do banco
- dados do admin seed
- URLs do frontend e backend

### 3. Subir banco e API

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml up --build -d
```

Esse comando:

- sobe o SQL Server em container
- sobe a API em container
- conecta a API ao banco Docker
- aplica a migration
- cria o admin inicial

### 4. Verificar containers

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml ps
```

### 5. Ver logs da API

```bash
docker logs -f buzzara-api
```

### 6. Ver logs do banco

```bash
docker logs -f buzzara-db
```

## Endereços Padrão

Com a configuração atual:

- API: `http://localhost:8080`
- SQL Server: `localhost:1433`

## Como Parar a Stack

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml down
```

## Como Resetar Tudo

Esse comando remove containers, rede e volumes do banco/uploads:

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml down -v
```

Use isso quando quiser recriar o banco do zero.

## Rodando Sem Docker

Se alguém quiser rodar localmente sem container:

### 1. Restaurar ferramentas e dependências

Na raiz do repositório:

```bash
dotnet tool restore
dotnet restore
```

### 2. Ajustar a connection string

Edite `buzzaraApi/appsettings.json` ou use variável de ambiente:

```text
Server=localhost,1433;Initial Catalog=buzzaradb;User ID=sa;Password=<senha>;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
```

### 3. Rodar a API

Na raiz do repositório:

```bash
dotnet run --project buzzaraApi/buzzaraApi.csproj
```

## Migrações

O projeto está com tool manifest local para EF Core.

Restaurar ferramenta:

```bash
dotnet tool restore
```

Criar nova migration:

```bash
dotnet tool run dotnet-ef migrations add NomeDaMigration --project buzzaraApi/buzzaraApi.csproj --startup-project buzzaraApi/buzzaraApi.csproj --output-dir Migrations
```

Aplicar migrations localmente:

```bash
dotnet tool run dotnet-ef database update --project buzzaraApi/buzzaraApi.csproj --startup-project buzzaraApi/buzzaraApi.csproj
```

Remover última migration:

```bash
dotnet tool run dotnet-ef migrations remove --project buzzaraApi/buzzaraApi.csproj --startup-project buzzaraApi/buzzaraApi.csproj
```

## Build do Projeto

Na raiz do repositório:

```bash
dotnet build buzzaraApi.sln
```

## Arquivos Importantes Para Continuidade

Arquivos mais importantes para quem vai dar continuidade:

- `buzzaraApi/Program.cs`
- `buzzaraApi/Data/ApplicationDbContext.cs`
- `buzzaraApi/Data/DatabaseInitializationExtensions.cs`
- `buzzaraApi/Migrations/*`
- `buzzaraApi/Controllers/*`
- `buzzaraApi/Services/*`
- `buzzaraApi/Models/*`
- `buzzaraApi/docker-compose.yml`
- `buzzaraApi/docker-compose.db.yml`
- `buzzaraApi/.env.example`

## Observações Técnicas

- a autenticação usa JWT Bearer e também leitura do token via cookie
- os uploads ficam persistidos em volume Docker
- o SQL Server também usa volume persistente
- Swagger fica disponível somente em ambiente de desenvolvimento
- existe um `HostedService` chamado `UsuarioStatusService`
- o projeto compila com warnings de nulabilidade, mas sem erros

## Status Atual de Infraestrutura

O setup atual foi ajustado para permitir:

- clonar o projeto
- criar `.env`
- subir banco e API por Docker
- aplicar a estrutura do banco automaticamente
- seedar o admin automaticamente
- entregar um banco limpo e funcional para continuidade

## Próximos Pontos Recomendados Para Quem Continuar

- revisar autenticação e autorização por recurso
- aumentar cobertura de testes
- revisar validação de uploads
- reforçar fluxo de pagamento real
- revisar exposição de dados públicos sensíveis
- adicionar README específico de endpoints, se necessário

## Próximos Passos de Implementação

Itens recomendados para a próxima fase do projeto:

### Crítico

- implementar endpoint para receber integração de gateway de pagamento
- vincular a confirmação do gateway ao ciclo real de pagamento do anúncio
- substituir o fluxo atual de pagamento simulado por integração real com provedor
- revisar completamente o método de recuperação de senha
- separar corretamente tokens de recuperação de senha e tokens de sessão

### Importante

- implementar validação de documentos dos usuários
- definir fluxo de upload, análise e aprovação de documentos
- implementar validação de imagens com IA
- validar fotos enviadas para identificar conteúdo inválido, inadequado ou fora da política da plataforma

### Evolução

- registrar auditoria dos processos de validação de documentos e imagens
- criar trilha de status para documentos e mídias analisadas
- definir políticas internas de moderação e reprovação automática

## Comandos Rápidos

Subir tudo:

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml up --build -d
```

Parar tudo:

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml down
```

Resetar tudo:

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml down -v
```

Build:

```bash
dotnet build buzzaraApi.sln
```

Rodar local:

```bash
dotnet run --project buzzaraApi/buzzaraApi.csproj
```
