using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Repositories.Ef; // Define EF repository namespace.

public class AccountRepository : IAccountRepository // Implement account repository.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public AccountRepository(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign the DbContext.
    } // Close the constructor block.

    public Task<bool> NameExistsAsync(string name) // Check for duplicate names.
    { // Open the method block.
        return _db.Accounts.AnyAsync(a => a.Name == name); // Query for name match.
    } // Close the method block.

    public async Task<Account> AddAsync(Account account) // Add a new account.
    { // Open the method block.
        _db.Accounts.Add(account); // Add the entity to the context.
        await _db.SaveChangesAsync(); // Persist changes.
        return account; // Return the saved account.
    } // Close the method block.

    public Task<List<Account>> GetAllAsync() // Fetch all accounts.
    { // Open the method block.
        return _db.Accounts.AsNoTracking().OrderBy(a => a.Name).ToListAsync(); // Query accounts ordered by name.
    } // Close the method block.

    public Task<Account?> GetByIdAsync(int id) // Fetch account by id.
    { // Open the method block.
        return _db.Accounts.FirstOrDefaultAsync(a => a.Id == id); // Query account by id.
    } // Close the method block.

    public Task UpdateAsync(Account account) // Update an account.
    { // Open the method block.
        _db.Accounts.Update(account); // Mark entity as updated.
        return Task.CompletedTask; // Return completed task.
    } // Close the method block.

    public Task DeleteAsync(Account account) // Delete an account.
    { // Open the method block.
        _db.Accounts.Remove(account); // Remove the entity.
        return Task.CompletedTask; // Return completed task.
    } // Close the method block.

    public Task SaveChangesAsync() // Persist changes.
    { // Open the method block.
        return _db.SaveChangesAsync(); // Save changes in the context.
    } // Close the method block.
} // Close the class block.
