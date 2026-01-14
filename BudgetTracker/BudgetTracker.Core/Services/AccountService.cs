using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class AccountService : IAccountService // Implement account service.
{ // Open the class block.
    private readonly IAccountRepository _accounts; // Hold account repository.

    public AccountService(IAccountRepository accounts) // Define constructor.
    { // Open the constructor block.
        _accounts = accounts; // Assign repository.
    } // Close the constructor block.

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto) // Create an account.
    { // Open the method block.
        if (string.IsNullOrWhiteSpace(dto.Name)) // Validate name.
            throw new ArgumentException("Name is required"); // Throw when name missing.

        if (dto.InitialBalance < 0) // Validate initial balance.
            throw new ArgumentException("Initial balance cannot be negative"); // Throw when balance negative.

        if (await _accounts.NameExistsAsync(dto.Name.Trim())) // Check for duplicate name.
            throw new InvalidOperationException("Account name must be unique"); // Throw when duplicate.

        var account = new Account // Create the account entity.
        { // Open the initializer block.
            Name = dto.Name.Trim(), // Assign trimmed name.
            AccountType = dto.AccountType, // Assign account type.
            InitialBalance = dto.InitialBalance, // Assign initial balance.
            CurrentBalance = dto.InitialBalance // Initialize current balance.
        }; // Close the initializer block.

        var created = await _accounts.AddAsync(account); // Persist the account.
        return MapAccount(created); // Map to DTO.
    } // Close the method block.

    public async Task<List<AccountDto>> GetAllAsync() // Fetch all accounts.
    { // Open the method block.
        var accounts = await _accounts.GetAllAsync(); // Query all accounts.
        return accounts.Select(MapAccount).ToList(); // Map to DTOs.
    } // Close the method block.

    public async Task<AccountDto?> GetByIdAsync(int id) // Fetch account by id.
    { // Open the method block.
        var account = await _accounts.GetByIdAsync(id); // Query account.
        return account is null ? null : MapAccount(account); // Map or return null.
    } // Close the method block.

    public async Task<AccountDto?> UpdateAsync(int id, UpdateAccountDto dto) // Update account details.
    { // Open the method block.
        var account = await _accounts.GetByIdAsync(id); // Fetch account.
        if (account is null) // Check for missing account.
            return null; // Return null when missing.

        if (string.IsNullOrWhiteSpace(dto.Name)) // Validate name.
            throw new ArgumentException("Name is required"); // Throw when name missing.

        var trimmed = dto.Name.Trim(); // Trim the name.
        if (!string.Equals(account.Name, trimmed, StringComparison.OrdinalIgnoreCase)) // Check for name change.
        { // Open the if block.
            if (await _accounts.NameExistsAsync(trimmed)) // Check for duplicate name.
                throw new InvalidOperationException("Account name must be unique"); // Throw when duplicate.
        } // Close the if block.

        account.Name = trimmed; // Update name.
        account.AccountType = dto.AccountType; // Update type.

        await _accounts.UpdateAsync(account); // Mark entity updated.
        await _accounts.SaveChangesAsync(); // Persist changes.

        return MapAccount(account); // Map to DTO.
    } // Close the method block.

    public async Task<bool> DeleteAsync(int id) // Delete account.
    { // Open the method block.
        var account = await _accounts.GetByIdAsync(id); // Fetch account.
        if (account is null) // Check for missing account.
            return false; // Return false when missing.

        await _accounts.DeleteAsync(account); // Remove account.
        await _accounts.SaveChangesAsync(); // Persist changes.
        return true; // Return success.
    } // Close the method block.

    private static AccountDto MapAccount(Account account) // Map account to DTO.
    { // Open the method block.
        return new AccountDto // Create the DTO.
        { // Open the initializer block.
            Id = account.Id, // Map id.
            Name = account.Name, // Map name.
            AccountType = account.AccountType, // Map type.
            CurrentBalance = account.CurrentBalance, // Map current balance.
            InitialBalance = account.InitialBalance // Map initial balance.
        }; // Close the initializer block.
    } // Close the method block.
} // Close the class block.
