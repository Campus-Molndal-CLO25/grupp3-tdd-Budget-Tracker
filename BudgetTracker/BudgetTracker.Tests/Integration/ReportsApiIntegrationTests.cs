using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class ReportsApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public ReportsApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

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
}
