using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface ITransactionService // Define transaction service contract.
{ // Open the interface block.
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto); // Create a transaction.
    Task<List<TransactionDto>> GetAllAsync(TransactionFilterDto filter); // Fetch filtered transactions.
    Task<TransactionDto?> GetByIdAsync(int id); // Fetch transaction by id.
    Task<TransactionDto?> UpdateAsync(int id, UpdateTransactionDto dto); // Update a transaction.
    Task<bool> DeleteAsync(int id); // Delete a transaction.
} // Close the interface block.
