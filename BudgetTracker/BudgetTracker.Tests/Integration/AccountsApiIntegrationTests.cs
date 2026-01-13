using System.Net;
using System.Net.Http.Json;
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
}
