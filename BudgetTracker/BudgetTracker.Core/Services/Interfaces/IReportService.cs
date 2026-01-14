using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface IReportService // Define report service contract.
{ // Open the interface block.
    Task<BudgetVsActualReportDto> GetBudgetVsActualAsync(int year, int month); // Get budget vs actual report.
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(int year, int month); // Get monthly summary report.
    Task<List<CategoryBreakdownDto>> GetCategoryBreakdownAsync(DateTime startDate, DateTime endDate); // Get category breakdown report.
} // Close the interface block.
