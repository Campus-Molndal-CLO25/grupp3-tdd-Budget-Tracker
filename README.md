
# grupp3-tdd-Budget-Tracker

[![](https://img.shields.io/badge/grupp3-tdd_Budget_Tracker-blue?style=for-the-badge)]()
[![.NET Build & Test](https://github.com/Campus-Molndal-CLO25/grupp3-tdd-Budget-Tracker/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Campus-Molndal-CLO25/grupp3-tdd-Budget-Tracker/actions/workflows/dotnet-desktop.yml)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

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

Set 1minAI configuration via environment variables (optional):

```bash
Linux
export ONEMINAI_API_KEY="your-key"
export ONEMINAI_MODEL="gpt-4o-mini"

Window 
$env:ONEMINAI_API_KEY="din-nyckel-här"
$env:ONEMINAI_MODEL="gpt-4o-mini"


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
