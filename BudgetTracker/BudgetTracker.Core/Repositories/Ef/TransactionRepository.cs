using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import filter DTOs.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Repositories.Ef; // Define EF repository namespace.

public class TransactionRepository : ITransactionRepository // Implement transaction repository.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public TransactionRepository(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign the DbContext.
    } // Close the constructor block.

    public async Task<Transaction> AddAsync(Transaction transaction) // Add a transaction.
    { // Open the method block.
        _db.Transactions.Add(transaction); // Add the entity to the context.
        await _db.SaveChangesAsync(); // Persist changes.
        return transaction; // Return the saved transaction.
    } // Close the method block.

    public async Task<List<Transaction>> GetAllAsync(TransactionFilterDto filter) // Fetch filtered transactions.
    { // Open the method block.
        var query = _db.Transactions.AsNoTracking().Include(t => t.Account).Include(t => t.Category).AsQueryable(); // Start query with joins.

        if (filter.StartDate.HasValue) // Check for a start date filter.
            query = query.Where(t => t.Date >= filter.StartDate.Value); // Apply start date filter.

        if (filter.EndDate.HasValue) // Check for an end date filter.
            query = query.Where(t => t.Date <= filter.EndDate.Value); // Apply end date filter.

        if (filter.CategoryId.HasValue) // Check for a category filter.
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value); // Apply category filter.

        if (filter.Type.HasValue) // Check for a type filter.
            query = query.Where(t => t.TransactionType == filter.Type.Value); // Apply type filter.

        query = query.OrderByDescending(t => t.Date); // Sort by newest first.

        if (filter.Skip > 0) // Check for skip.
            query = query.Skip(filter.Skip); // Apply skip.

        if (filter.Take > 0) // Check for take.
            query = query.Take(filter.Take); // Apply take.

        return await query.ToListAsync(); // Execute query.
    } // Close the method block.

    public Task<Transaction?> GetByIdAsync(int id) // Fetch transaction by id.
    { // Open the method block.
        return _db.Transactions.Include(t => t.Account).Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id); // Query transaction by id.
    } // Close the method block.

    public Task DeleteAsync(Transaction transaction) // Delete a transaction.
    { // Open the method block.
        _db.Transactions.Remove(transaction); // Remove the entity.
        return Task.CompletedTask; // Return completed task.
    } // Close the method block.

    public Task SaveChangesAsync() // Persist changes.
    { // Open the method block.
        return _db.SaveChangesAsync(); // Save changes in the context.
    } // Close the method block.
} // Close the class block.
