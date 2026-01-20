using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services;
using FluentAssertions;
using NSubstitute;

namespace BudgetTracker.Tests.Unit;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsBlankName()
    {
        var repo = Substitute.For<ICategoryRepository>();
        var service = new CategoryService(repo);

        var act = () => service.CreateAsync(new CreateCategoryDto { Name = "   " });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Name is required");
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateName()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.NameExistsAsync("Food").Returns(true);
        var service = new CategoryService(repo);

        var act = () => service.CreateAsync(new CreateCategoryDto
        {
            Name = "Food",
            Type = CategoryType.Expense
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        ex.Message.Should().Be("Category name must be unique");
    }

    [Fact]
    public async Task CreateAsync_TrimsAndMapsFields()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.NameExistsAsync("Salary").Returns(false);
        repo.AddAsync(Arg.Any<Category>()).Returns(call => call.Arg<Category>());
        var service = new CategoryService(repo);

        var result = await service.CreateAsync(new CreateCategoryDto
        {
            Name = "  Salary ",
            Type = CategoryType.Income,
            Color = "#123456"
        });

        result.Name.Should().Be("Salary");
        result.Type.Should().Be(CategoryType.Income);
        result.Color.Should().Be("#123456");
        await repo.Received(1).AddAsync(Arg.Is<Category>(c =>
            c.Name == "Salary" && c.CategoryType == CategoryType.Income && c.Color == "#123456"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullWhenMissing()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(12).Returns((Category?)null);
        var service = new CategoryService(repo);

        var result = await service.UpdateAsync(12, new UpdateCategoryDto { Name = "Food" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_RejectsDuplicateNameWhenChanged()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(3).Returns(new Category { Id = 3, Name = "Food" });
        repo.NameExistsAsync("Rent").Returns(true);
        var service = new CategoryService(repo);

        var act = () => service.UpdateAsync(3, new UpdateCategoryDto
        {
            Name = "Rent",
            Type = CategoryType.Expense
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        ex.Message.Should().Be("Category name must be unique");
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenMissing()
    {
        var repo = Substitute.For<ICategoryRepository>();
        repo.GetByIdAsync(9).Returns((Category?)null);
        var service = new CategoryService(repo);

        var result = await service.DeleteAsync(9);

        result.Should().BeFalse();
    }
}
