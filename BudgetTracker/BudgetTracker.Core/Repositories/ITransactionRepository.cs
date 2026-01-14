using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import filter DTOs.

namespace BudgetTracker.Core.Repositories.Interfaces; // Define repository namespace.

public interface ITransactionRepository // Define transaction repository contract.
{ // Open the interface block.
    Task<Transaction> AddAsync(Transaction transaction); // Add a transaction.
    Task<List<Transaction>> GetAllAsync(TransactionFilterDto filter); // Fetch filtered transactions.
    Task<Transaction?> GetByIdAsync(int id); // Fetch transaction by id.
    Task DeleteAsync(Transaction transaction); // Delete a transaction.
    Task SaveChangesAsync(); // Persist pending changes.
} // Close the interface block.
