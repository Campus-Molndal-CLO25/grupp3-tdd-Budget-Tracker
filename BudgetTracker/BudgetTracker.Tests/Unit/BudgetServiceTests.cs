using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Dtos;
using BudgetTracker.Core.Repositories.Interfaces;
using BudgetTracker.Core.Services;
using FluentAssertions;
using NSubstitute;

namespace BudgetTracker.Tests.Unit;

public class BudgetServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsNonPositiveAmount()
    {
        var budgets = Substitute.For<IBudgetRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var service = new BudgetService(budgets, categories);

        var act = () => service.CreateAsync(new CreateBudgetDto
        {
            CategoryId = 1,
            Month = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
            Amount = 0
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Amount must be greater than zero");
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingCategory()
    {
        var budgets = Substitute.For<IBudgetRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        categories.GetByIdAsync(7).Returns((Category?)null);
        var service = new BudgetService(budgets, categories);

        var act = () => service.CreateAsync(new CreateBudgetDto
        {
            CategoryId = 7,
            Month = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
            Amount = 300
        });

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Category not found");
    }

    [Fact]
    public async Task CreateAsync_NormalizesMonthAndMapsCategory()
    {
        var budgets = Substitute.For<IBudgetRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var category = new Category { Id = 2, Name = "Rent" };
        categories.GetByIdAsync(2).Returns(category);
        budgets.GetByCategoryAndMonthAsync(2, Arg.Any<DateTime>()).Returns((Budget?)null);
        budgets.AddAsync(Arg.Any<Budget>()).Returns(call => call.Arg<Budget>());
        var service = new BudgetService(budgets, categories);

        var result = await service.CreateAsync(new CreateBudgetDto
        {
            CategoryId = 2,
            Month = new DateTime(2025, 2, 15, 11, 0, 0, DateTimeKind.Utc),
            Amount = 900
        });

        result.Month.Should().Be(new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        result.CategoryName.Should().Be("Rent");
        await budgets.Received(1).AddAsync(Arg.Is<Budget>(b =>
            b.CategoryId == 2 && b.Month == new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task UpdateAsync_RejectsDuplicateBudgetForMonth()
    {
        var budgets = Substitute.For<IBudgetRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        var existing = new Budget { Id = 3, CategoryId = 1, Month = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc) };
        budgets.GetByIdAsync(3).Returns(existing);
        categories.GetByIdAsync(1).Returns(new Category { Id = 1, Name = "Food" });
        budgets.GetByCategoryAndMonthAsync(1, Arg.Any<DateTime>()).Returns(new Budget { Id = 9 });
        var service = new BudgetService(budgets, categories);

        var act = () => service.UpdateAsync(3, new UpdateBudgetDto
        {
            CategoryId = 1,
            Month = new DateTime(2025, 2, 12, 0, 0, 0, DateTimeKind.Utc),
            Amount = 120
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        ex.Message.Should().Be("Budget already exists for this category and month");
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenMissing()
    {
        var budgets = Substitute.For<IBudgetRepository>();
        var categories = Substitute.For<ICategoryRepository>();
        budgets.GetByIdAsync(44).Returns((Budget?)null);
        var service = new BudgetService(budgets, categories);

        var result = await service.DeleteAsync(44);

        result.Should().BeFalse();
    }
}
