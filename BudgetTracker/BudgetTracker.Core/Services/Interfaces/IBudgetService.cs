using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface IBudgetService // Define budget service contract.
{ // Open the interface block.
    Task<BudgetDto> CreateAsync(CreateBudgetDto dto); // Create a budget.
    Task<List<BudgetDto>> GetByMonthAsync(DateTime month); // Fetch budgets by month.
    Task<BudgetDto?> UpdateAsync(int id, UpdateBudgetDto dto); // Update a budget.
    Task<bool> DeleteAsync(int id); // Delete a budget.
} // Close the interface block.
