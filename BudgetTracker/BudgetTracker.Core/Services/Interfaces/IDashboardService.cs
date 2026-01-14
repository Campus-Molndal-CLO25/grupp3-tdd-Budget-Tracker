using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface IDashboardService // Define dashboard service contract.
{ // Open the interface block.
    Task<DashboardDto> GetDashboardAsync(int year, int month); // Get dashboard data.
} // Close the interface block.
