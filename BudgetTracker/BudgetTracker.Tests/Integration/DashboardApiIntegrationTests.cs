using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class DashboardApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public DashboardApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

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
}
