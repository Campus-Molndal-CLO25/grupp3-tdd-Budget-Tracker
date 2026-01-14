using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class ReportService : IReportService // Implement report service.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public ReportService(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign DbContext.
    } // Close the constructor block.

    public async Task<BudgetVsActualReportDto> GetBudgetVsActualAsync(int year, int month) // Build budget vs actual report.
    { // Open the method block.
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc); // Calculate month start.
        var monthEnd = monthStart.AddMonths(1); // Calculate month end.

        var budgets = await _db.Budgets.Include(b => b.Category).Where(b => b.Month == monthStart).ToListAsync(); // Load budgets.
        var actuals = await _db.Transactions.Include(t => t.Category) // Start transaction query.
            .Where(t => t.Date >= monthStart && t.Date < monthEnd && t.TransactionType == TransactionType.Expense) // Filter expenses in month.
            .GroupBy(t => t.CategoryId) // Group by category.
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(x => x.Amount) }) // Project totals.
            .ToListAsync(); // Execute query.

        var actualLookup = actuals.ToDictionary(a => a.CategoryId, a => a.Total); // Build actual lookup.
        var categories = new List<BudgetReportCategoryDto>(); // Prepare output list.

        foreach (var budget in budgets) // Iterate budgets.
        { // Open the loop block.
            var actual = actualLookup.TryGetValue(budget.CategoryId, out var value) ? value : 0m; // Get actual amount.
            var diff = budget.Amount - actual; // Calculate difference.
            var pct = budget.Amount == 0 ? 0 : Math.Round(actual / budget.Amount * 100m, 2); // Calculate percentage.
            var status = actual > budget.Amount ? BudgetStatus.Over : actual == budget.Amount ? BudgetStatus.OnTrack : BudgetStatus.Under; // Determine status.

            categories.Add(new BudgetReportCategoryDto // Add report row.
            { // Open initializer block.
                CategoryId = budget.CategoryId, // Map category id.
                CategoryName = budget.Category?.Name ?? string.Empty, // Map category name.
                Budgeted = budget.Amount, // Map budgeted.
                Actual = actual, // Map actual.
                Difference = diff, // Map difference.
                Percentage = pct, // Map percentage.
                Status = status // Map status.
            }); // Close initializer block.
        } // Close loop block.

        var totalBudgeted = categories.Sum(c => c.Budgeted); // Sum budgeted.
        var totalActual = categories.Sum(c => c.Actual); // Sum actual.
        var totalDifference = categories.Sum(c => c.Difference); // Sum difference.

        return new BudgetVsActualReportDto // Build report DTO.
        { // Open initializer block.
            Year = year, // Map year.
            Month = month, // Map month.
            TotalBudgeted = totalBudgeted, // Map total budgeted.
            TotalActual = totalActual, // Map total actual.
            TotalDifference = totalDifference, // Map total difference.
            Categories = categories // Map category rows.
        }; // Close initializer block.
    } // Close the method block.

    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(int year, int month) // Build monthly summary report.
    { // Open the method block.
        var monthStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc); // Calculate month start.
        var monthEnd = monthStart.AddMonths(1); // Calculate month end.
        var previousStart = monthStart.AddMonths(-1); // Calculate previous month start.
        var previousEnd = monthStart; // Calculate previous month end.

        var monthTransactions = await _db.Transactions.Include(t => t.Category) // Start query.
            .Where(t => t.Date >= monthStart && t.Date < monthEnd) // Filter for month.
            .ToListAsync(); // Execute query.

        var totalIncome = monthTransactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount); // Sum income.
        var totalExpense = monthTransactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount); // Sum expense.
        var netSavings = totalIncome - totalExpense; // Calculate net savings.
        var savingsRate = totalIncome == 0 ? 0 : Math.Round(netSavings / totalIncome * 100m, 2); // Calculate savings rate.

        var previousTransactions = await _db.Transactions // Start previous query.
            .Where(t => t.Date >= previousStart && t.Date < previousEnd) // Filter previous month.
            .ToListAsync(); // Execute query.

        var prevIncome = previousTransactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount); // Sum previous income.
        var prevExpense = previousTransactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount); // Sum previous expense.
        var prevNet = prevIncome - prevExpense; // Calculate previous net.

        var categoryGroups = monthTransactions.GroupBy(t => t.CategoryId).ToList(); // Group by category.
        var categories = new List<MonthlySummaryCategoryDto>(); // Prepare category list.

        foreach (var group in categoryGroups) // Iterate category groups.
        { // Open the loop block.
            var sample = group.FirstOrDefault(); // Take a sample row.
            var name = sample?.Category?.Name ?? string.Empty; // Resolve category name.
            var income = group.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount); // Sum income per category.
            var expense = group.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount); // Sum expense per category.

            categories.Add(new MonthlySummaryCategoryDto // Add category row.
            { // Open initializer block.
                CategoryId = group.Key, // Map category id.
                CategoryName = name, // Map category name.
                Income = income, // Map income.
                Expense = expense // Map expense.
            }); // Close initializer block.
        } // Close loop block.

        return new MonthlySummaryDto // Build summary DTO.
        { // Open initializer block.
            Year = year, // Map year.
            Month = month, // Map month.
            TotalIncome = totalIncome, // Map total income.
            TotalExpense = totalExpense, // Map total expense.
            NetSavings = netSavings, // Map net savings.
            SavingsRate = savingsRate, // Map savings rate.
            PreviousNetSavings = prevNet, // Map previous net.
            NetSavingsChange = netSavings - prevNet, // Map net change.
            Categories = categories // Map categories.
        }; // Close initializer block.
    } // Close the method block.

    public async Task<List<CategoryBreakdownDto>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate) // Build category breakdown.
    { // Open the method block.
        var transactions = await _db.Transactions.Include(t => t.Category) // Start query.
            .Where(t => t.Date >= startDate && t.Date <= endDate) // Filter date range.
            .ToListAsync(); // Execute query.

        var groups = transactions.GroupBy(t => t.CategoryId); // Group by category.
        var results = new List<CategoryBreakdownDto>(); // Prepare result list.

        foreach (var group in groups) // Iterate groups.
        { // Open the loop block.
            var sample = group.FirstOrDefault(); // Grab sample row.
            var name = sample?.Category?.Name ?? string.Empty; // Resolve category name.
            var income = group.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount); // Sum income.
            var expense = group.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount); // Sum expense.

            results.Add(new CategoryBreakdownDto // Add breakdown row.
            { // Open initializer block.
                CategoryId = group.Key, // Map category id.
                CategoryName = name, // Map category name.
                TotalIncome = income, // Map income.
                TotalExpense = expense // Map expense.
            }); // Close initializer block.
        } // Close loop block.

        return results.OrderByDescending(r => r.TotalExpense).ToList(); // Sort by expense.
    } // Close the method block.
} // Close the class block.
