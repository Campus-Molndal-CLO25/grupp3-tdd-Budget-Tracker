using BudgetTracker.Core.Domain; // Import domain models.

namespace BudgetTracker.Core.Repositories.Interfaces; // Define repository namespace.

public interface IAccountRepository // Define account repository contract.
{ // Open the interface block.
    Task<bool> NameExistsAsync(string name); // Check for duplicate account name.
    Task<Account> AddAsync(Account account); // Add a new account.
    Task<List<Account>> GetAllAsync(); // Fetch all accounts.
    Task<Account?> GetByIdAsync(int id); // Fetch account by id.
    Task UpdateAsync(Account account); // Update an existing account.
    Task DeleteAsync(Account account); // Delete an account.
    Task SaveChangesAsync(); // Persist pending changes.
} // Close the interface block.
