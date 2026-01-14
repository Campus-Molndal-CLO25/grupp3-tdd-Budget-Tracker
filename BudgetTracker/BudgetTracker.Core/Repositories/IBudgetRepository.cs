using BudgetTracker.Core.Domain; // Import domain models.

namespace BudgetTracker.Core.Repositories.Interfaces; // Define repository namespace.

public interface IBudgetRepository // Define budget repository contract.
{ // Open the interface block.
    Task<Budget> AddAsync(Budget budget); // Add a budget.
    Task<Budget?> GetByIdAsync(int id); // Fetch budget by id.
    Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, DateTime month); // Fetch budget by category and month.
    Task<List<Budget>> GetByMonthAsync(DateTime month); // Fetch budgets for a month.
    Task DeleteAsync(Budget budget); // Delete a budget.
    Task SaveChangesAsync(); // Persist pending changes.
} // Close the interface block.
