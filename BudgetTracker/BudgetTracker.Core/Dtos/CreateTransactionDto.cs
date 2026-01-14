using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CreateTransactionDto // Define transaction creation payload.
{ // Open the class block.
    public int AccountId { get; set; } // Store the account id.
    public decimal Amount { get; set; } // Store the transaction amount.
    public TransactionType Type { get; set; } // Store the transaction type.
    public int CategoryId { get; set; } // Store the category id.
    public DateTime Date { get; set; } = DateTime.UtcNow; // Store the transaction date.
    public string? Description { get; set; } // Store the optional description.
} // Close the class block.
