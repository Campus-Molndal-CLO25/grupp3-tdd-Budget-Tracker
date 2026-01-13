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
