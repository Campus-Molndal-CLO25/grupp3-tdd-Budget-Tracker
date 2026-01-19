using BudgetTracker.Core.Data;
using BudgetTracker.Core.Domain;
using BudgetTracker.Core.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Unit;

public class ReportServiceTests
{
    [Fact]
    public async Task GetBudgetVsActualAsync_ComputesStatusAndTotals()
    {
        var (context, connection) = await CreateDbAsync();
        await using var _ = context;
        await using var __ = connection;
        var account = new Account { Name = "Main", CurrentBalance = 0 };
        var category = new Category { Name = "Food", CategoryType = CategoryType.Expense };
        context.Accounts.Add(account);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Budgets.Add(new Budget
        {
            CategoryId = category.Id,
            Month = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            Amount = 100
        });
        context.Transactions.Add(new Transaction
        {
            CategoryId = category.Id,
            AccountId = account.Id,
            Amount = 120,
            TransactionType = TransactionType.Expense,
            Date = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc)
        });
        await context.SaveChangesAsync();

        var service = new ReportService(context);

        var result = await service.GetBudgetVsActualAsync(2025, 2);

        result.TotalBudgeted.Should().Be(100);
        result.TotalActual.Should().Be(120);
        result.TotalDifference.Should().Be(-20);
        result.Categories.Should().ContainSingle();
        result.Categories[0].Status.Should().Be(BudgetStatus.Over);
        result.Categories[0].Percentage.Should().Be(120);
    }

    [Fact]
    public async Task GetMonthlySummaryAsync_ComputesSavingsAndTrend()
    {
        var (context, connection) = await CreateDbAsync();
        await using var _ = context;
        await using var __ = connection;
        var account = new Account { Name = "Main", CurrentBalance = 0 };
        var category = new Category { Name = "Pay", CategoryType = CategoryType.Income };
        context.Accounts.Add(account);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        context.Transactions.AddRange(
            new Transaction
            {
                CategoryId = category.Id,
                AccountId = account.Id,
                Amount = 200,
                TransactionType = TransactionType.Income,
                Date = new DateTime(2025, 2, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                CategoryId = category.Id,
                AccountId = account.Id,
                Amount = 50,
                TransactionType = TransactionType.Expense,
                Date = new DateTime(2025, 2, 7, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                CategoryId = category.Id,
                AccountId = account.Id,
                Amount = 100,
                TransactionType = TransactionType.Income,
                Date = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new Transaction
            {
                CategoryId = category.Id,
                AccountId = account.Id,
                Amount = 20,
                TransactionType = TransactionType.Expense,
                Date = new DateTime(2025, 1, 7, 0, 0, 0, DateTimeKind.Utc)
            });
        await context.SaveChangesAsync();

        var service = new ReportService(context);

        var result = await service.GetMonthlySummaryAsync(2025, 2);

        result.TotalIncome.Should().Be(200);
        result.TotalExpense.Should().Be(50);
        result.NetSavings.Should().Be(150);
        result.SavingsRate.Should().Be(75);
        result.PreviousNetSavings.Should().Be(80);
        result.NetSavingsChange.Should().Be(70);
        result.Categories.Should().ContainSingle(c => c.CategoryName == "Pay");
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
