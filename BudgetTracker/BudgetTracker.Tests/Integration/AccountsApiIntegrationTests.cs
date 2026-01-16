using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class AccountsApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public AccountsApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

    [Fact]
    public async Task CreateAccount_ThenGetAll_ReturnsAccount()
    {
        var create = new CreateAccountDto
        {
            Name = "Main",
            AccountType = AccountType.Checking,
            InitialBalance = 125
        };

        var postResponse = await _client.PostAsJsonAsync("/api/accounts", create);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var accounts = await _client.GetFromJsonAsync<List<Account>>("/api/accounts");
        accounts.Should().NotBeNull();
        accounts!.Should().ContainSingle(a => a.Name == "Main");
    }

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
}
