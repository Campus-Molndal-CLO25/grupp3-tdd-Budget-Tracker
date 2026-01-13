# BudgetTracker

A personal budgeting application with a clean Core library, API endpoints, and a web dashboard.

## Projects

- `BudgetTracker.Core` - Domain, DTOs, repositories, and services.
- `BudgetTracker.Api` - REST API for accounts, transactions, categories, budgets, reports, and dashboard.
- `BudgetTracker.Web` - MVC UI with account and Gemini advice experiences.

## Requirements

- .NET 10 SDK
- SQLite

## Configuration

Set Gemini configuration via environment variables (optional):

```bash
export GEMINI_BASE_URL="https://your-gemini-proxy-host"
export GEMINI_API_KEY="your-key"
```

`appsettings.json` contains the default SQLite connection string.

## Run the API

```bash
dotnet run --project BudgetTracker.Api
```

The API exposes endpoints under `/api`.

## Run the Web UI

```bash
dotnet run --project BudgetTracker.Web
```

## Migrations

Migrations live in `BudgetTracker.Core`. To add and apply migrations:

```bash
dotnet ef migrations add <Name> -p BudgetTracker.Core -s BudgetTracker.Api
dotnet ef database update -p BudgetTracker.Core -s BudgetTracker.Api
```

## Tests

```bash
dotnet test BudgetTracker.Tests/BudgetTracker.Tests.csproj
```

## Coverage

Run coverage with the built-in coverlet collector:

```bash
dotnet test BudgetTracker.Tests/BudgetTracker.Tests.csproj --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Coverage reports are emitted under `BudgetTracker.Tests/TestResults/` as `coverage.cobertura.xml`.

## Docs

- `UsersStory.md` contains the user stories and acceptance criteria.
