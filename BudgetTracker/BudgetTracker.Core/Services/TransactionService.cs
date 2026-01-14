using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class TransactionService : ITransactionService // Implement transaction service.
{ // Open the class block.
    private readonly ITransactionRepository _transactions; // Hold transaction repository.
    private readonly IAccountRepository _accounts; // Hold account repository.
    private readonly ICategoryRepository _categories; // Hold category repository.

    public TransactionService(ITransactionRepository transactions, IAccountRepository accounts, ICategoryRepository categories) // Define constructor.
    { // Open the constructor block.
        _transactions = transactions; // Assign transaction repository.
        _accounts = accounts; // Assign account repository.
        _categories = categories; // Assign category repository.
    } // Close the constructor block.

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto) // Create a transaction.
    { // Open the method block.
        if (dto.Amount <= 0) // Validate amount.
            throw new ArgumentException("Amount must be greater than zero"); // Throw when invalid.

        var account = await _accounts.GetByIdAsync(dto.AccountId); // Fetch account.
        if (account is null) // Check for missing account.
            throw new ArgumentException("Account not found"); // Throw when account missing.

        var category = await _categories.GetByIdAsync(dto.CategoryId); // Fetch category.
        if (category is null) // Check for missing category.
            throw new ArgumentException("Category not found"); // Throw when category missing.

        var transaction = new Transaction // Create transaction entity.
        { // Open the initializer block.
            AccountId = dto.AccountId, // Assign account id.
            CategoryId = dto.CategoryId, // Assign category id.
            Amount = dto.Amount, // Assign amount.
            TransactionType = dto.Type, // Assign type.
            Date = dto.Date, // Assign date.
            Description = dto.Description // Assign description.
        }; // Close the initializer block.

        account.CurrentBalance += GetSignedAmount(dto.Amount, dto.Type); // Update account balance.

        await _transactions.AddAsync(transaction); // Persist transaction.
        await _accounts.UpdateAsync(account); // Mark account updated.
        await _accounts.SaveChangesAsync(); // Persist account changes.

        transaction.Account = account; // Attach account for mapping.
        transaction.Category = category; // Attach category for mapping.

        return MapTransaction(transaction); // Map to DTO.
    } // Close the method block.

    public async Task<List<TransactionDto>> GetAllAsync(TransactionFilterDto filter) // Fetch filtered transactions.
    { // Open the method block.
        var transactions = await _transactions.GetAllAsync(filter); // Query transactions.
        return transactions.Select(MapTransaction).ToList(); // Map to DTOs.
    } // Close the method block.

    public async Task<TransactionDto?> GetByIdAsync(int id) // Fetch transaction by id.
    { // Open the method block.
        var transaction = await _transactions.GetByIdAsync(id); // Query transaction.
        return transaction is null ? null : MapTransaction(transaction); // Map or return null.
    } // Close the method block.

    public async Task<TransactionDto?> UpdateAsync(int id, UpdateTransactionDto dto) // Update a transaction.
    { // Open the method block.
        if (dto.Amount <= 0) // Validate amount.
            throw new ArgumentException("Amount must be greater than zero"); // Throw when invalid.

        var transaction = await _transactions.GetByIdAsync(id); // Fetch transaction.
        if (transaction is null) // Check for missing transaction.
            return null; // Return null when missing.

        var newAccount = await _accounts.GetByIdAsync(dto.AccountId); // Fetch target account.
        if (newAccount is null) // Check for missing account.
            throw new ArgumentException("Account not found"); // Throw when missing.

        var newCategory = await _categories.GetByIdAsync(dto.CategoryId); // Fetch target category.
        if (newCategory is null) // Check for missing category.
            throw new ArgumentException("Category not found"); // Throw when missing.

        var oldAccount = await _accounts.GetByIdAsync(transaction.AccountId); // Fetch old account.
        if (oldAccount is null) // Check for missing old account.
            throw new ArgumentException("Account not found"); // Throw when missing.

        var oldDelta = GetSignedAmount(transaction.Amount, transaction.TransactionType); // Calculate old delta.
        oldAccount.CurrentBalance -= oldDelta; // Reverse old balance impact.

        transaction.AccountId = dto.AccountId; // Update account id.
        transaction.CategoryId = dto.CategoryId; // Update category id.
        transaction.Amount = dto.Amount; // Update amount.
        transaction.TransactionType = dto.Type; // Update type.
        transaction.Date = dto.Date; // Update date.
        transaction.Description = dto.Description; // Update description.

        var newDelta = GetSignedAmount(dto.Amount, dto.Type); // Calculate new delta.
        if (oldAccount.Id == newAccount.Id) // Check if account unchanged.
        { // Open the if block.
            oldAccount.CurrentBalance += newDelta; // Apply new delta on same account.
            await _accounts.UpdateAsync(oldAccount); // Mark account updated.
        } // Close the if block.
        else // Handle account change.
        { // Open the else block.
            newAccount.CurrentBalance += newDelta; // Apply new delta to new account.
            await _accounts.UpdateAsync(oldAccount); // Mark old account updated.
            await _accounts.UpdateAsync(newAccount); // Mark new account updated.
        } // Close the else block.

        await _transactions.SaveChangesAsync(); // Persist transaction changes.
        await _accounts.SaveChangesAsync(); // Persist account changes.

        transaction.Account = newAccount; // Attach account for mapping.
        transaction.Category = newCategory; // Attach category for mapping.

        return MapTransaction(transaction); // Map to DTO.
    } // Close the method block.

    public async Task<bool> DeleteAsync(int id) // Delete a transaction.
    { // Open the method block.
        var transaction = await _transactions.GetByIdAsync(id); // Fetch transaction.
        if (transaction is null) // Check for missing transaction.
            return false; // Return false when missing.

        var account = await _accounts.GetByIdAsync(transaction.AccountId); // Fetch account.
        if (account is null) // Check for missing account.
            throw new ArgumentException("Account not found"); // Throw when missing.

        account.CurrentBalance -= GetSignedAmount(transaction.Amount, transaction.TransactionType); // Reverse transaction impact.

        await _transactions.DeleteAsync(transaction); // Remove transaction.
        await _accounts.UpdateAsync(account); // Mark account updated.
        await _transactions.SaveChangesAsync(); // Persist transaction changes.
        await _accounts.SaveChangesAsync(); // Persist account changes.

        return true; // Return success.
    } // Close the method block.

    private static decimal GetSignedAmount(decimal amount, TransactionType type) // Calculate signed delta.
    { // Open the method block.
        return type == TransactionType.Income ? amount : -amount; // Return signed amount.
    } // Close the method block.

    private static TransactionDto MapTransaction(Transaction transaction) // Map transaction to DTO.
    { // Open the method block.
        return new TransactionDto // Create DTO.
        { // Open the initializer block.
            Id = transaction.Id, // Map id.
            AccountId = transaction.AccountId, // Map account id.
            AccountName = transaction.Account?.Name ?? string.Empty, // Map account name.
            CategoryId = transaction.CategoryId, // Map category id.
            CategoryName = transaction.Category?.Name ?? string.Empty, // Map category name.
            Amount = transaction.Amount, // Map amount.
            Type = transaction.TransactionType, // Map type.
            Date = transaction.Date, // Map date.
            Description = transaction.Description // Map description.
        }; // Close the initializer block.
    } // Close the method block.
} // Close the class block.
