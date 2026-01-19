using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class TransactionsIndexViewModel // Define transactions index view model.
{ // Open the class block.
    public List<TransactionDto> Transactions { get; set; } = new(); // Store transactions.
    public List<AccountDto> Accounts { get; set; } = new(); // Store accounts.
    public List<CategoryDto> Categories { get; set; } = new(); // Store categories.
    public CreateTransactionDto NewTransaction { get; set; } = new(); // Store create payload.
    public string? ErrorMessage { get; set; } // Store error message.
} // Close the class block.
