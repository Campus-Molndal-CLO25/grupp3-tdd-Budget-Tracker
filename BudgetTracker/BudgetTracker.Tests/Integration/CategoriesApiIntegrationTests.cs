using System.Net;
using System.Net.Http.Json;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using FluentAssertions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public class CategoriesApiIntegrationTests : IClassFixture<SqliteInMemoryFixture>
{
    private readonly HttpClient _client;

    public CategoriesApiIntegrationTests(SqliteInMemoryFixture fixture)
    {
        _client = fixture.Factory.CreateClient();
    }

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
}
