# Docker

## Arquivos

- `docker-compose.db.yml`: sobe apenas o SQL Server
- `docker-compose.yml`: sobe a API conectada ao banco Docker
- `.env.example`: modelo das variaveis do ambiente

## Preparacao

No diretorio `buzzaraApi`, crie um `.env` a partir do exemplo:

```bash
cp .env.example .env
```

No Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Preencha pelo menos:

- `MSSQL_SA_PASSWORD`
- `JWT_SECRET`

## Subir banco e API juntos

```bash
docker compose --env-file .env -f docker-compose.db.yml -f docker-compose.yml up --build -d
```

Fluxo de inicializacao:

1. O container `buzzara-db` sobe
2. A API espera o banco ficar acessivel
3. A API aplica todas as migrations automaticamente
4. O seed cria o admin inicial se ele ainda nao existir

## Subir so o banco

```bash
docker compose --env-file .env -f docker-compose.db.yml up -d
```

## Admin inicial

Por padrao:

- E-mail: `admin@buzzara.com.br`
- Senha: `123456`

Esses dados podem ser alterados no `.env`:

- `SEED_ADMIN_EMAIL`
- `SEED_ADMIN_PASSWORD`
- `SEED_ADMIN_NAME`
- `SEED_ADMIN_PHONE`
- `SEED_ADMIN_CPF`
- `SEED_ADMIN_GENDER`

## String de conexao local para a API fora do Docker

```text
Server=localhost,1433;Initial Catalog=buzzaradb;User ID=sa;Password=<senha-do-.env>;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
```

## Observacoes

- O seed e idempotente: se o admin ja existir, ele nao sera recriado.
- A API recebe a connection string via variavel de ambiente, sem depender do banco remoto do `appsettings.json`.
- O volume `sqlserver-data` persiste os dados do SQL Server.
- O volume `uploads-data` persiste os uploads da API.
