using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class DashboardService : IDashboardService // Implement dashboard service.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public DashboardService(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign DbContext.
    } // Close the constructor block.

    public async Task<DashboardDto> GetDashboardAsync(int year, int month) // Build dashboard data.
    { // Open the method block.
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc); // Calculate month start.
        var monthEnd = monthStart.AddMonths(1); // Calculate month end.

        var totalBalance = await _db.Accounts.SumAsync(a => a.CurrentBalance); // Sum account balances.

        var monthTransactions = await _db.Transactions.Include(t => t.Category) // Start query.
            .Where(t => t.Date >= monthStart && t.Date < monthEnd) // Filter month.
            .ToListAsync(); // Execute query.

        var monthIncome = monthTransactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount); // Sum income.
        var monthExpense = monthTransactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount); // Sum expense.

        var topExpenses = monthTransactions.Where(t => t.TransactionType == TransactionType.Expense) // Filter expenses.
            .GroupBy(t => t.CategoryId) // Group by category.
            .Select(g => new CategoryBreakdownDto // Project breakdown DTO.
            { // Open initializer block.
                CategoryId = g.Key, // Map category id.
                CategoryName = g.FirstOrDefault()?.Category?.Name ?? string.Empty, // Map category name.
                TotalIncome = 0, // Set income to zero.
                TotalExpense = g.Sum(x => x.Amount) // Sum expense.
            }) // Close initializer block.
            .OrderByDescending(x => x.TotalExpense) // Sort by expense.
            .Take(5) // Take top 5.
            .ToList(); // Materialize list.

        var budgets = await _db.Budgets.Include(b => b.Category) // Load budgets.
            .Where(b => b.Month == monthStart) // Filter by month.
            .ToListAsync(); // Execute query.

        var budgetProgress = new List<BudgetProgressDto>(); // Prepare budget progress list.
        foreach (var budget in budgets) // Iterate budgets.
        { // Open loop block.
            var actual = monthTransactions.Where(t => t.CategoryId == budget.CategoryId && t.TransactionType == TransactionType.Expense) // Filter by category.
                .Sum(t => t.Amount); // Sum actual.
            var percentage = budget.Amount == 0 ? 0 : Math.Round(actual / budget.Amount * 100m, 2); // Calculate percentage.
            budgetProgress.Add(new BudgetProgressDto // Add progress row.
            { // Open initializer block.
                CategoryId = budget.CategoryId, // Map category id.
                CategoryName = budget.Category?.Name ?? string.Empty, // Map category name.
                Budgeted = budget.Amount, // Map budgeted.
                Actual = actual, // Map actual.
                Percentage = percentage, // Map percentage.
                OverBudget = actual > budget.Amount // Set over budget flag.
            }); // Close initializer block.
        } // Close loop block.

        var recentTransactions = await _db.Transactions.Include(t => t.Account).Include(t => t.Category) // Start query.
            .OrderByDescending(t => t.Date) // Sort by newest.
            .Take(5) // Take latest five.
            .ToListAsync(); // Execute query.

        var recentDtos = recentTransactions.Select(t => new TransactionDto // Map to DTO.
        { // Open initializer block.
            Id = t.Id, // Map id.
            AccountId = t.AccountId, // Map account id.
            AccountName = t.Account?.Name ?? string.Empty, // Map account name.
            CategoryId = t.CategoryId, // Map category id.
            CategoryName = t.Category?.Name ?? string.Empty, // Map category name.
            Amount = t.Amount, // Map amount.
            Type = t.TransactionType, // Map type.
            Date = t.Date, // Map date.
            Description = t.Description // Map description.
        }).ToList(); // Materialize list.

        return new DashboardDto // Build dashboard DTO.
        { // Open initializer block.
            TotalBalance = totalBalance, // Map total balance.
            MonthIncome = monthIncome, // Map month income.
            MonthExpense = monthExpense, // Map month expense.
            TopExpenseCategories = topExpenses, // Map top expenses.
            BudgetProgress = budgetProgress, // Map budget progress.
            RecentTransactions = recentDtos // Map recent transactions.
        }; // Close initializer block.
    } // Close the method block.
} // Close the class block.
