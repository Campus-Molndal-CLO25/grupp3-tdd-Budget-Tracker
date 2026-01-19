# 💰 Budget Tracker - User Stories

**Tema:** Personlig budget och ekonomihantering  
**Domän:** Finansiell planering

---

## 📊 Core Entities

- **Account** (Konto: Bankkonto, Sparkonto, Kontant)
- **Transaction** (Transaktion: Inkomst/Utgift)
- **Category** (Kategori: Lön, Mat, Hyra, Nöje)
- **Budget** (Budget: Månatlig planering)

---

## 📝 User Stories

### Epic 1: Kontohantering

#### US-1: Skapa Konto

**Som** användare  
**Vill jag** kunna skapa ett konto  
**För att** hålla koll på olika konton (bank, sparkonto, kontant)

**Acceptance Criteria:**

- [ ] POST /api/accounts endpoint finns
- [ ] Kräver: name, accountType (checking/savings/cash), initialBalance
- [ ] Validering: name måste vara unikt per användare
- [ ] Validering: initialBalance >= 0
- [ ] Response returnerar skapat konto med ID
- [ ] Status 201 vid success

**Gherkin:**

```gherkin
Feature: Skapa Konto

Scenario: Skapa bankkonto med initial saldo
  Given att jag är inloggad
  When jag skapar konto:
    | Name        | Type     | InitialBalance |
    | Swedbank    | checking | 10000          |
  Then ska kontot sparas
  And mitt totala saldo ska vara 10000
  And response ska vara 201 Created

Scenario: Försök skapa konto med negativt saldo
  When jag försöker skapa konto med initialBalance -500
  Then ska response vara 400 Bad Request
  And felmeddelande "Initial balance cannot be negative"
```

**Test Example:**

```csharp
[Fact]
public async Task CreateAccount_ReturnsCreatedAccount_AndUpdatesDashboardBalance()
{
    var suffix = Guid.NewGuid().ToString("N");
    var beforeDashboard = await _client.GetFromJsonAsync<DashboardDto>("/api/dashboard");
    beforeDashboard.Should().NotBeNull();
    var create = new CreateAccountDto
    {
        Name = $"Swedbank-{suffix}",
        AccountType = AccountType.Checking,
        InitialBalance = 10000
    };

    var postResponse = await _client.PostAsJsonAsync("/api/accounts", create);
    postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var created = await postResponse.Content.ReadFromJsonAsync<AccountDto>();
    created.Should().NotBeNull();
    created!.Id.Should().BeGreaterThan(0);
    created.Name.Should().Be($"Swedbank-{suffix}");
    created.InitialBalance.Should().Be(10000);
    created.CurrentBalance.Should().Be(10000);

    var dashboard = await _client.GetFromJsonAsync<DashboardDto>("/api/dashboard");
    dashboard.Should().NotBeNull();
    dashboard!.TotalBalance.Should().Be(beforeDashboard!.TotalBalance + 10000);
}

[Fact]
public async Task CreateAccount_WithNegativeBalance_ReturnsBadRequest()
{
    var create = new CreateAccountDto
    {
        Name = "Invalid",
        AccountType = AccountType.Cash,
        InitialBalance = -500
    };

    var postResponse = await _client.PostAsJsonAsync("/api/accounts", create);
    postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var payload = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
    payload.GetProperty("error").GetString().Should().Be("Initial balance cannot be negative");
}
```

**Story Points:** 3  
**Priority:** Must Have

---

#### US-2: Visa Alla Konton

**Som** användare  
**Vill jag** se alla mina konton  
**För att** få översikt över mina tillgångar

**Acceptance Criteria:**

- [ ] GET /api/accounts endpoint finns
- [ ] Returnerar alla användares konton
- [ ] Visar: name, accountType, currentBalance
- [ ] Sorterat på name
- [ ] Beräknar currentBalance baserat på transaktioner

**Gherkin:**

```gherkin
Feature: Visa Konton

Scenario: Visa konton med beräknade saldon
  Given att jag har följande konton:
    | Name      | Type     | InitialBalance |
    | Swedbank  | checking | 10000          |
    | Sparkonto | savings  | 50000          |
  And jag har gjort utgift 500 från Swedbank
  When jag hämtar alla konton
  Then ska response visa:
    | Name      | CurrentBalance |
    | Sparkonto | 50000          |
    | Swedbank  | 9500           |
```

**Test Example:**

```csharp
[Fact]
public async Task GetAllAccounts_ReturnsSortedWithComputedBalances()
{
    var suffix = Guid.NewGuid().ToString("N");
    var swedbankName = $"Swedbank-{suffix}";
    var sparkontoName = $"Sparkonto-{suffix}";

    var swedbank = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountDto
    {
        Name = swedbankName,
        AccountType = AccountType.Checking,
        InitialBalance = 10000
    });
    swedbank.StatusCode.Should().Be(HttpStatusCode.Created);
    var swedbankAccount = await swedbank.Content.ReadFromJsonAsync<AccountDto>();
    swedbankAccount.Should().NotBeNull();

    var sparkonto = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountDto
    {
        Name = sparkontoName,
        AccountType = AccountType.Savings,
        InitialBalance = 50000
    });
    sparkonto.StatusCode.Should().Be(HttpStatusCode.Created);
    var sparkontoAccount = await sparkonto.Content.ReadFromJsonAsync<AccountDto>();
    sparkontoAccount.Should().NotBeNull();

    var categories = await _client.GetFromJsonAsync<List<CategoryDto>>("/api/categories");
    categories.Should().NotBeNull();
    var expenseCategory = categories!.FirstOrDefault(c => c.Type == CategoryType.Expense);
    if (expenseCategory is null)
    {
        var createCategory = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
        {
            Name = $"Expense-{suffix}",
            Type = CategoryType.Expense,
            Color = "#c53030"
        });
        createCategory.StatusCode.Should().Be(HttpStatusCode.Created);
        expenseCategory = await createCategory.Content.ReadFromJsonAsync<CategoryDto>();
    }

    var transaction = await _client.PostAsJsonAsync("/api/transactions", new CreateTransactionDto
    {
        AccountId = swedbankAccount!.Id,
        Amount = 500,
        Type = TransactionType.Expense,
        CategoryId = expenseCategory!.Id,
        Date = DateTime.UtcNow,
        Description = "Test expense"
    });
    transaction.StatusCode.Should().Be(HttpStatusCode.Created);

    var accounts = await _client.GetFromJsonAsync<List<AccountDto>>("/api/accounts");
    accounts.Should().NotBeNull();
    accounts!.Select(a => a.Name).Should().BeInAscendingOrder();

    accounts.Single(a => a.Name == sparkontoName).CurrentBalance.Should().Be(50000);
    accounts.Single(a => a.Name == swedbankName).CurrentBalance.Should().Be(9500);
}
```

**Story Points:** 3  
**Priority:** Must Have

---

### Epic 2: Transaktioner

#### US-3: Registrera Transaktion

**Som** användare  
**Vill jag** kunna registrera en transaktion  
**För att** spåra mina inkomster och utgifter

**Acceptance Criteria:**

- [ ] POST /api/transactions endpoint finns
- [ ] Kräver: accountId, amount, type (income/expense), categoryId, date, description
- [ ] Validering: amount > 0
- [ ] Validering: konto och kategori måste finnas
- [ ] Uppdaterar kontosaldo automatiskt
- [ ] Status 201 vid success

**Gherkin:**

```gherkin
Feature: Registrera Transaktion

Scenario: Registrera inkomst
  Given att konto "Swedbank" har saldo 10000
  And kategori "Lön" finns
  When jag registrerar inkomst:
    | Amount | Category | Description  |
    | 30000  | Lön      | Januarilön   |
  Then ska transaktionen sparas
  And Swedbank saldo ska vara 40000
  And response 201

Scenario: Registrera utgift
  Given att konto "Swedbank" har saldo 10000
  And kategori "Mat" finns
  When jag registrerar utgift 500 för "Mat"
  Then ska Swedbank saldo vara 9500
```

**Test Example:**

```csharp
[Theory]
[InlineData(1000, TransactionType.Income, 11000)]
[InlineData(500, TransactionType.Expense, 9500)]
public async Task CreateTransaction_UpdatesAccountBalance(
    decimal amount, TransactionType type, decimal expectedBalance)
{
    // Arrange
    var account = new Account { Id = 1, CurrentBalance = 10000 };
    var dto = new CreateTransactionDto
    {
        AccountId = 1,
        Amount = amount,
        Type = type,
        CategoryId = 1,
        Date = DateTime.UtcNow
    };

    // Act
    await _service.CreateTransactionAsync(dto);

    // Assert
    var updated = await _context.Accounts.FindAsync(1);
    Assert.Equal(expectedBalance, updated.CurrentBalance);
}
```

**Story Points:** 5  
**Priority:** Must Have

---

#### US-4: Visa Transaktioner med Filter

**Som** användare  
**Vill jag** filtrera transaktioner på datum och kategori  
**För att** analysera mina utgifter

**Acceptance Criteria:**

- [ ] GET /api/transactions endpoint finns
- [ ] Query params: startDate, endDate, categoryId, type
- [ ] Returnerar matchande transaktioner
- [ ] Sorterat på datum (nyast först)
- [ ] Pagination (skip/take)

**Gherkin:**

```gherkin
Feature: Filtrera Transaktioner

Scenario: Filtrera på månad och kategori
  Given att följande transaktioner finns:
    | Date       | Category | Amount |
    | 2025-01-05 | Mat      | 500    |
    | 2025-01-10 | Mat      | 300    |
    | 2025-01-15 | Nöje     | 200    |
    | 2025-02-01 | Mat      | 400    |
  When jag filtrerar på januari och kategori "Mat"
  Then ska jag få 2 transaktioner
  And total summa ska vara 800
```

**Test Example:**

```csharp
[Fact]
public async Task GetTransactions_WithFilters_ReturnsMatchingSortedPage()
{
    var suffix = Guid.NewGuid().ToString("N");
    var accountResponse = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountDto
    {
        Name = $"Main-{suffix}",
        AccountType = AccountType.Checking,
        InitialBalance = 1000
    });
    accountResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var account = await accountResponse.Content.ReadFromJsonAsync<AccountDto>();
    account.Should().NotBeNull();

    var foodResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Food-{suffix}",
        Type = CategoryType.Expense,
        Color = "#dd6b20"
    });
    foodResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var food = await foodResponse.Content.ReadFromJsonAsync<CategoryDto>();
    food.Should().NotBeNull();

    var funResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Fun-{suffix}",
        Type = CategoryType.Expense,
        Color = "#805ad5"
    });
    funResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var fun = await funResponse.Content.ReadFromJsonAsync<CategoryDto>();
    fun.Should().NotBeNull();

    var jan05 = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc);
    var jan10 = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc);
    var jan15 = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc);
    var feb01 = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc);

    var transactions = new[]
    {
        new CreateTransactionDto
        {
            AccountId = account!.Id,
            Amount = 500,
            Type = TransactionType.Expense,
            CategoryId = food!.Id,
            Date = jan05,
            Description = "Food"
        },
        new CreateTransactionDto
        {
            AccountId = account.Id,
            Amount = 300,
            Type = TransactionType.Expense,
            CategoryId = food.Id,
            Date = jan10,
            Description = "More food"
        },
        new CreateTransactionDto
        {
            AccountId = account.Id,
            Amount = 200,
            Type = TransactionType.Expense,
            CategoryId = fun!.Id,
            Date = jan15,
            Description = "Fun"
        },
        new CreateTransactionDto
        {
            AccountId = account.Id,
            Amount = 400,
            Type = TransactionType.Expense,
            CategoryId = food.Id,
            Date = feb01,
            Description = "Later food"
        }
    };

    foreach (var dto in transactions)
    {
        var response = await _client.PostAsJsonAsync("/api/transactions", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    var url = $"/api/transactions?startDate=2025-01-01&endDate=2025-01-31&categoryId={food.Id}&type=Expense&skip=0&take=10";
    var filtered = await _client.GetFromJsonAsync<List<TransactionDto>>(url);
    filtered.Should().NotBeNull();
    filtered!.Should().HaveCount(2);
    filtered.Select(t => t.Date).Should().BeInDescendingOrder();
    filtered.Should().OnlyContain(t => t.CategoryId == food.Id);

    var firstPageUrl = $"/api/transactions?startDate=2025-01-01&endDate=2025-01-31&categoryId={food.Id}&type=Expense&skip=0&take=1";
    var firstPage = await _client.GetFromJsonAsync<List<TransactionDto>>(firstPageUrl);
    firstPage.Should().NotBeNull();
    firstPage!.Should().ContainSingle();
    firstPage[0].Date.Should().Be(jan10);

    var secondPageUrl = $"/api/transactions?startDate=2025-01-01&endDate=2025-01-31&categoryId={food.Id}&type=Expense&skip=1&take=1";
    var secondPage = await _client.GetFromJsonAsync<List<TransactionDto>>(secondPageUrl);
    secondPage.Should().NotBeNull();
    secondPage!.Should().ContainSingle();
    secondPage[0].Date.Should().Be(jan05);
}
```

**Story Points:** 5  
**Priority:** Should Have

---

### Epic 3: Budget & Rapporter

#### US-5: Skapa Månadsbudget

**Som** användare  
**Vill jag** sätta budget per kategori och månad  
**För att** planera mina utgifter

**Acceptance Criteria:**

- [ ] POST /api/budgets endpoint finns
- [ ] Kräver: month (YYYY-MM), categoryId, amount
- [ ] Validering: amount > 0
- [ ] En budget per kategori per månad
- [ ] Status 201 vid success

**Gherkin:**

```gherkin
Feature: Månadsbudget

Scenario: Skapa budget för mat
  Given att kategori "Mat" finns
  When jag skapar budget för januari:
    | Category | Amount |
    | Mat      | 5000   |
  Then ska budget sparas
  And response 201

Scenario: Försök skapa duplicat budget
  Given att budget för "Mat" i januari redan finns
  When jag försöker skapa ny budget för "Mat" i januari
  Then ska response vara 409 Conflict
```

**Test Example:**

```csharp
[Fact]
public async Task CreateBudget_ReturnsCreatedBudget()
{
    var suffix = Guid.NewGuid().ToString("N");
    var categoryResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Groceries-{suffix}",
        Type = CategoryType.Expense,
        Color = "#dd6b20"
    });
    categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
    category.Should().NotBeNull();

    var month = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var budgetResponse = await _client.PostAsJsonAsync("/api/budgets", new CreateBudgetDto
    {
        CategoryId = category!.Id,
        Month = month,
        Amount = 5000
    });
    budgetResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var created = await budgetResponse.Content.ReadFromJsonAsync<BudgetDto>();
    created.Should().NotBeNull();
    created!.Id.Should().BeGreaterThan(0);
    created.CategoryId.Should().Be(category.Id);
    created.Amount.Should().Be(5000);
    created.Month.Year.Should().Be(2025);
    created.Month.Month.Should().Be(1);
    created.Month.Day.Should().Be(1);
}
```

**Story Points:** 3  
**Priority:** Should Have

---

#### US-6: Budget vs Faktiskt (Rapport)

**Som** användare  
**Vill jag** se hur mycket jag spenderat vs budget  
**För att** hålla mig inom min budget

**Acceptance Criteria:**

- [ ] GET /api/reports/budget-vs-actual endpoint finns
- [ ] Query param: month (YYYY-MM)
- [ ] Visar per kategori: budgeted, actual, difference, percentage
- [ ] Markerar över-budget kategorier
- [ ] Summerad totalt i botten

**Gherkin:**

```gherkin
Feature: Budget vs Faktiskt

Scenario: Visa budget-rapport för månad
  Given att jag har budget för januari:
    | Category | Amount |
    | Mat      | 5000   |
    | Nöje     | 2000   |
  And jag har spenderat:
    | Category | Amount |
    | Mat      | 5500   |
    | Nöje     | 1500   |
  When jag hämtar rapport för januari
  Then ska rapporten visa:
    | Category | Budget | Actual | Diff | Status     |
    | Mat      | 5000   | 5500   | -500 | Over       |
    | Nöje     | 2000   | 1500   | +500 | Under      |
    | TOTALT   | 7000   | 7000   | 0    | On-track   |
```

**Test Example:**

```csharp
[Fact]
public async Task GetBudgetReport_ShowsActualVsBudget()
{
    // Arrange
    var budget = new Budget
    {
        CategoryId = 1,
        Month = new DateTime(2025, 1, 1),
        Amount = 5000
    };

    var transactions = new List<Transaction>
    {
        new Transaction { CategoryId = 1, Amount = 3000,
                          Type = TransactionType.Expense },
        new Transaction { CategoryId = 1, Amount = 2500,
                          Type = TransactionType.Expense }
    };

    // Act
    var report = await _service.GetBudgetReportAsync(2025, 1);

    // Assert
    var category = report.Categories.First();
    Assert.Equal(5000, category.Budgeted);
    Assert.Equal(5500, category.Actual);
    Assert.Equal(-500, category.Difference);
    Assert.Equal(BudgetStatus.Over, category.Status);
}
```

**Story Points:** 8  
**Priority:** Should Have

---

#### US-7: Månadssammanfattning

**Som** användare  
**Vill jag** se total inkomst, utgift och sparande per månad  
**För att** förstå min ekonomiska situation

**Acceptance Criteria:**

- [ ] GET /api/reports/monthly-summary endpoint finns
- [ ] Query param: year, month
- [ ] Visar: totalIncome, totalExpense, netSavings, savingsRate
- [ ] Breakdown per kategori
- [ ] Jämför med föregående månad

**Gherkin:**

```gherkin
Feature: Månadssammanfattning

Scenario: Visa januari sammanfattning
  Given att jag har transaktioner i januari:
    | Type    | Amount |
    | Income  | 30000  |
    | Expense | 20000  |
  When jag hämtar sammanfattning för januari
  Then ska rapporten visa:
    | Field         | Value |
    | TotalIncome   | 30000 |
    | TotalExpense  | 20000 |
    | NetSavings    | 10000 |
    | SavingsRate   | 33.3% |
```

**Test Example:**

```csharp
[Fact]
public async Task GetMonthlySummary_ReturnsTotalsAndCategoryBreakdown()
{
    var suffix = Guid.NewGuid().ToString("N");
    var accountResponse = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountDto
    {
        Name = $"Main-{suffix}",
        AccountType = AccountType.Checking,
        InitialBalance = 0
    });
    accountResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var account = await accountResponse.Content.ReadFromJsonAsync<AccountDto>();
    account.Should().NotBeNull();

    var incomeResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Salary-{suffix}",
        Type = CategoryType.Income,
        Color = "#2f855a"
    });
    incomeResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var incomeCategory = await incomeResponse.Content.ReadFromJsonAsync<CategoryDto>();
    incomeCategory.Should().NotBeNull();

    var expenseResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Rent-{suffix}",
        Type = CategoryType.Expense,
        Color = "#c53030"
    });
    expenseResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var expenseCategory = await expenseResponse.Content.ReadFromJsonAsync<CategoryDto>();
    expenseCategory.Should().NotBeNull();

    var janIncome = new CreateTransactionDto
    {
        AccountId = account!.Id,
        Amount = 3000,
        Type = TransactionType.Income,
        CategoryId = incomeCategory!.Id,
        Date = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc),
        Description = "Salary"
    };
    var janExpense = new CreateTransactionDto
    {
        AccountId = account.Id,
        Amount = 1000,
        Type = TransactionType.Expense,
        CategoryId = expenseCategory!.Id,
        Date = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
        Description = "Rent"
    };
    var decIncome = new CreateTransactionDto
    {
        AccountId = account.Id,
        Amount = 2000,
        Type = TransactionType.Income,
        CategoryId = incomeCategory.Id,
        Date = new DateTime(2024, 12, 5, 0, 0, 0, DateTimeKind.Utc),
        Description = "Prev salary"
    };
    var decExpense = new CreateTransactionDto
    {
        AccountId = account.Id,
        Amount = 500,
        Type = TransactionType.Expense,
        CategoryId = expenseCategory.Id,
        Date = new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc),
        Description = "Prev rent"
    };

    foreach (var dto in new[] { janIncome, janExpense, decIncome, decExpense })
    {
        var response = await _client.PostAsJsonAsync("/api/transactions", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    var summary = await _client.GetFromJsonAsync<MonthlySummaryDto>("/api/reports/monthly-summary?year=2025&month=1");
    summary.Should().NotBeNull();
    summary!.TotalIncome.Should().Be(3000);
    summary.TotalExpense.Should().Be(1000);
    summary.NetSavings.Should().Be(2000);
    summary.SavingsRate.Should().Be(66.67m);
    summary.PreviousNetSavings.Should().Be(1500);
    summary.NetSavingsChange.Should().Be(500);
    summary.Categories.Should().Contain(c => c.CategoryId == incomeCategory.Id && c.Income == 3000);
    summary.Categories.Should().Contain(c => c.CategoryId == expenseCategory.Id && c.Expense == 1000);
}
```

**Story Points:** 5  
**Priority:** Could Have

---

### Epic 4: Kategorier

#### US-8: Skapa Kategori

**Som** användare  
**Vill jag** skapa egna kategorier  
**För att** organisera mina transaktioner

**Acceptance Criteria:**

- [ ] POST /api/categories endpoint finns
- [ ] Kräver: name, type (income/expense), color (optional)
- [ ] Validering: name unikt per användare
- [ ] Default kategorier ska skapas vid användarregistrering

**Test Example:**

```csharp
[Fact]
public async Task CreateCategory_ReturnsCreatedCategory()
{
    var suffix = Guid.NewGuid().ToString("N");
    var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Custom-{suffix}",
        Type = CategoryType.Expense,
        Color = "#ff8800"
    });
    response.StatusCode.Should().Be(HttpStatusCode.Created);

    var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
    created.Should().NotBeNull();
    created!.Id.Should().BeGreaterThan(0);
    created.Name.Should().Be($"Custom-{suffix}");
    created.Type.Should().Be(CategoryType.Expense);
    created.Color.Should().Be("#ff8800");
}
```

**Story Points:** 2  
**Priority:** Must Have

---

### Epic 5: Dashboard

#### US-9: Dashboard Overview

**Som** användare  
**Vill jag** se en dashboard med nyckeltal  
**För att** snabbt få överblick

**Acceptance Criteria:**

- [ ] GET /api/dashboard endpoint finns
- [ ] Visar: totalt saldo alla konton, månadens inkomst/utgift
- [ ] Top 5 utgiftskategorier denna månad
- [ ] Budget progress bars
- [ ] Senaste 5 transaktionerna

**Test Example:**

```csharp
[Fact]
public async Task GetDashboard_ReturnsMonthlySummaryData()
{
    var suffix = Guid.NewGuid().ToString("N");
    var accountResponse = await _client.PostAsJsonAsync("/api/accounts", new CreateAccountDto
    {
        Name = $"Main-{suffix}",
        AccountType = AccountType.Checking,
        InitialBalance = 10000
    });
    accountResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var account = await accountResponse.Content.ReadFromJsonAsync<AccountDto>();
    account.Should().NotBeNull();

    var categoryResponse = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryDto
    {
        Name = $"Utilities-{suffix}",
        Type = CategoryType.Expense,
        Color = "#c53030"
    });
    categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
    category.Should().NotBeNull();

    var month = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var budgetResponse = await _client.PostAsJsonAsync("/api/budgets", new CreateBudgetDto
    {
        CategoryId = category!.Id,
        Month = month,
        Amount = 2000
    });
    budgetResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var transactionResponse = await _client.PostAsJsonAsync("/api/transactions", new CreateTransactionDto
    {
        AccountId = account!.Id,
        Amount = 500,
        Type = TransactionType.Expense,
        CategoryId = category.Id,
        Date = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
        Description = "Utilities bill"
    });
    transactionResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var dashboard = await _client.GetFromJsonAsync<DashboardDto>("/api/dashboard?year=2025&month=1");
    dashboard.Should().NotBeNull();
    dashboard!.TotalBalance.Should().Be(9500);
    dashboard.MonthIncome.Should().Be(0);
    dashboard.MonthExpense.Should().Be(500);
    dashboard.TopExpenseCategories.Should().ContainSingle(c => c.CategoryId == category.Id && c.TotalExpense == 500);
    dashboard.BudgetProgress.Should().ContainSingle(p => p.CategoryId == category.Id && p.Budgeted == 2000 && p.Actual == 500);
    dashboard.RecentTransactions.Should().ContainSingle(t => t.CategoryId == category.Id && t.Amount == 500);
}
```

**Story Points:** 8  
**Priority:** Could Have

---

## 🧪 Test Scenarios

### Edge Cases att Testa

**Konton:**

- [ ] Skapa konto med 0 initial balance
- [ ] Uppdatera konto till negativt saldo (tillåt?)
- [ ] Ta bort konto med transaktioner (soft delete?)

**Transaktioner:**

- [ ] Transaktion med framtida datum
- [ ] Mycket stora belopp (decimal precision)
- [ ] Transaktion utan beskrivning (optional?)
- [ ] Redigera historisk transaktion (uppdatera saldo?)

**Budget:**

- [ ] Budget med 0 belopp
- [ ] Ändra budget mitt i månad
- [ ] Budget för kategori som inte används
- [ ] Flera budgets för samma månad (totalbudget?)

**Rapporter:**

- [ ] Tom månad (inga transaktioner)
- [ ] Månad i framtiden
- [ ] Mycket stora datumspann

---

## 📊 API Endpoints Summary

```
Accounts:
POST   /api/accounts
GET    /api/accounts
GET    /api/accounts/{id}
PUT    /api/accounts/{id}
DELETE /api/accounts/{id}

Transactions:
POST   /api/transactions
GET    /api/transactions?startDate={}&endDate={}&categoryId={}&type={}
GET    /api/transactions/{id}
PUT    /api/transactions/{id}
DELETE /api/transactions/{id}

Categories:
POST   /api/categories
GET    /api/categories
GET    /api/categories/{id}
PUT    /api/categories/{id}
DELETE /api/categories/{id}

Budgets:
POST   /api/budgets
GET    /api/budgets?month={}
PUT    /api/budgets/{id}
DELETE /api/budgets/{id}

Reports:
GET    /api/reports/budget-vs-actual?year={}&month={}
GET    /api/reports/monthly-summary?year={}&month={}
GET    /api/reports/category-breakdown?startDate={}&endDate={}

Dashboard:
GET    /api/dashboard
```

---

## 🎯 Minimum Viable Product (MVP)

**Sprint 1 (Must Have):**

- US-1: Skapa Konto
- US-2: Visa Alla Konton
- US-3: Registrera Transaktion
- US-8: Skapa Kategori

**Sprint 2 (Should Have):**

- US-4: Filtrera Transaktioner
- US-5: Skapa Månadsbudget
- US-6: Budget vs Faktiskt

**Future (Could Have):**

- US-7: Månadssammanfattning
- US-9: Dashboard

---
