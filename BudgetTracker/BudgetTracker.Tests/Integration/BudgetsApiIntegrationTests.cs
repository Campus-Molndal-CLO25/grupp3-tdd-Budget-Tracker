using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class BudgetsApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public BudgetsApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

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
}
