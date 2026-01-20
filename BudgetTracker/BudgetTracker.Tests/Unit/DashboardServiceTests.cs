using BudgetTracker.Core.Data;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Unit;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ComputesTotalsAndBudgetProgress()
    {
        var (context, connection) = await CreateDbAsync();
        await using var _ = context;
        await using var __ = connection;

        var food = new Category { Name = "Food", CategoryType = CategoryType.Expense };
        var salary = new Category { Name = "Salary", CategoryType = CategoryType.Income };
        var checking = new Account { Name = "Checking", CurrentBalance = 100 };
        var savings = new Account { Name = "Savings", CurrentBalance = 200 };
        context.Categories.AddRange(food, salary);
        context.Accounts.AddRange(checking, savings);
        await context.SaveChangesAsync();

        context.Budgets.Add(new Budget
        {
            CategoryId = food.Id,
            Month = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount = 100
        });
        context.Transactions.AddRange(
            new Transaction
            {
                CategoryId = food.Id,
                AccountId = checking.Id,
                Amount = 120,
                TransactionType = TransactionType.Expense,
                Date = new DateTime(2025, 2, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                CategoryId = salary.Id,
                AccountId = checking.Id,
                Amount = 500,
                TransactionType = TransactionType.Income,
                Date = new DateTime(2025, 2, 5, 0, 0, 0, DateTimeKind.Utc)
            });
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        var result = await service.GetDashboardAsync(2025, 2);

        result.TotalBalance.Should().Be(300);
        result.MonthIncome.Should().Be(500);
        result.MonthExpense.Should().Be(120);
        result.TopExpenseCategories.Should().ContainSingle(c => c.CategoryName == "Food" && c.TotalExpense == 120);
        result.BudgetProgress.Should().ContainSingle(p =>
            p.CategoryName == "Food" && p.Budgeted == 100 && p.Actual == 120 && p.OverBudget);
        result.RecentTransactions.Should().HaveCount(2);
    }

    private static async Task<(BudgetTrackerDbContext Context, SqliteConnection Connection)> CreateDbAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<BudgetTrackerDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new BudgetTrackerDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return (context, connection);
    }
}
