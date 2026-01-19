# **BudgetTracker – Technical Documentation**

## Overview

**Purpose:** BudgetTracker is an ASP.NET Core solution that tracks accounts, transactions, categories, and budgets, and exposes both a Web UI and a REST API. It also provides AI-based saving advice using the 1minAI API.

**Key Capabilities:**

- Manage accounts, categories, transactions, and budgets.
- Generate dashboard summaries (balances, income/expense, top categories, budget progress).
- Expose REST endpoints for core entities and reports.
- Provide AI advice with caching and a simple prompt flow.

**Tech Stack:**

- .NET 10, ASP.NET Core MVC + Web API
- EF Core with SQLite
- Repository + Service layers
- Typed HttpClient for AI integration
- Memory cache for insights

---

## High-Level Architecture

`Browser -> Web Controllers -> Services -> (Repositories -> DbContext) -> SQLite`
`                               |`
`                               -> InsightsService -> IGeminiClient (1minAI)`

### Layers

| Layer | Key Types | Responsibilities |
| --- | --- | --- |
| **Presentation** | `HomeController`, Razor Views, API Controllers | Handle user input and API requests, return views/JSON |
| **Application** | `AccountService`, `TransactionService`, `BudgetService`, `DashboardService`, `InsightsService` | Orchestrate domain logic, validation, and caching |
| **Integration** | `GeminiClient` (1minAI), `IGeminiClient` | AI HTTP calls (create conversation, send prompt), parse JSON |
| **Persistence** | `BudgetTrackerDbContext`, EF repositories | Read/write entities, enforce unique constraints |
| **Configuration** | `Program.cs` (Web/API) | Register DI, DbContext, HttpClient, and middleware |

---

## Configuration & Secrets

- 1minAI API key is read from environment:

```bash
export ONEMINAI_API_KEY="your-key"
export ONEMINAI_MODEL="gpt-4o-mini"
```

- SQLite uses the default connection string in appsettings.
- Migrations live in `BudgetTracker.Core`.

---

## Data Persistence

- **Entities:** `Account`, `Transaction`, `Category`, `Budget`.
- **DbContext:** `BudgetTrackerDbContext` exposes `DbSet<>` for each entity.
- **Constraints:** unique account names, unique category names, and one budget per category per month.
- **Seed Data:** default categories are inserted on first run in development.

---

## Request Flow (Web UI)

1. User opens the dashboard (`HomeController.Index`).
2. Controller loads accounts and renders the view.
3. User submits "Ask Gemini" form.
4. `InsightsService.GetAdviceAsync` shapes the prompt, caches results, calls `IGeminiClient`.
5. `GeminiClient` creates a 1minAI conversation and posts the prompt.
6. UI renders AI response or error details.

---

## Request Flow (API)

1. Client calls `/api/...` endpoints (Accounts, Transactions, Categories, Budgets, Reports, Dashboard).
2. API controllers validate inputs and delegate to services.
3. Services query EF repositories and return DTOs.

---

## Key Implementation Details

- **DI composition:** services and repositories are registered as scoped in `Program.cs`.
- **EF Core migrations:** executed on startup in development to keep the DB updated.
- **Caching:** insights are cached for 30 minutes to reduce AI calls.
- **AI provider:** implemented in `GeminiClient`, currently backed by 1minAI endpoints.
- **SQLite:** used for local persistence; a seed step inserts default categories.

---

## Running Locally

Web UI:

```bash
dotnet run --project BudgetTracker/BudgetTracker.Web
```

API:

```bash
dotnet run --project BudgetTracker/BudgetTracker.Api
```

---

## Tests

```bash
dotnet test BudgetTracker/BudgetTracker.Tests/BudgetTracker.Tests.csproj
```
