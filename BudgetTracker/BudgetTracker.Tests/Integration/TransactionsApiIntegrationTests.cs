using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class TransactionsApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public TransactionsApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

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
}
