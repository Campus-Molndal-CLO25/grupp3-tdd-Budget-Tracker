using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface IAccountService // Define account service contract.
{ // Open the interface block.
    Task<AccountDto> CreateAsync(CreateAccountDto dto); // Create an account.
    Task<List<AccountDto>> GetAllAsync(); // Fetch all accounts.
    Task<AccountDto?> GetByIdAsync(int id); // Fetch account by id.
    Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto); // Update an account.
    Task<bool> DeleteAsync(int id); // Delete an account.
} // Close the interface block.
