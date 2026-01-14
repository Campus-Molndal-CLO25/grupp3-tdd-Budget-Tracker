using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Repositories.Ef; // Define EF repository namespace.

public class BudgetRepository : IBudgetRepository // Implement budget repository.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public BudgetRepository(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign the DbContext.
    } // Close the constructor block.

    public async Task<Budget> AddAsync(Budget budget) // Add a budget.
    { // Open the method block.
        _db.Budgets.Add(budget); // Add entity to the context.
        await _db.SaveChangesAsync(); // Persist changes.
        return budget; // Return the saved budget.
    } // Close the method block.

    public Task<Budget?> GetByIdAsync(int id) // Fetch budget by id.
    { // Open the method block.
        return _db.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id); // Query budget by id.
    } // Close the method block.

    public Task<Budget?> GetByCategoryAndMonthAsync(int categoryId, DateTime month) // Fetch budget by category and month.
    { // Open the method block.
        return _db.Budgets.FirstOrDefaultAsync(b => b.CategoryId == categoryId && b.Month == month); // Query for budget.
    } // Close the method block.

    public Task<List<Budget>> GetByMonthAsync(DateTime month) // Fetch budgets by month.
    { // Open the method block.
        return _db.Budgets.Include(b => b.Category).AsNoTracking().Where(b => b.Month == month).ToListAsync(); // Query budgets for month.
    } // Close the method block.

    public Task DeleteAsync(Budget budget) // Delete a budget.
    { // Open the method block.
        _db.Budgets.Remove(budget); // Remove the entity.
        return Task.CompletedTask; // Return completed task.
    } // Close the method block.

    public Task SaveChangesAsync() // Persist changes.
    { // Open the method block.
        return _db.SaveChangesAsync(); // Save changes in the context.
    } // Close the method block.
} // Close the class block.
