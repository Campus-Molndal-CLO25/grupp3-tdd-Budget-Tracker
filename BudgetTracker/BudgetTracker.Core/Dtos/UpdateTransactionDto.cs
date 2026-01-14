using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class UpdateTransactionDto // Define transaction update payload.
{ // Open the class block.
    public int AccountId { get; set; } // Store the updated account id.
    public decimal Amount { get; set; } // Store the updated amount.
    public TransactionType Type { get; set; } // Store the updated transaction type.
    public int CategoryId { get; set; } // Store the updated category id.
    public DateTime Date { get; set; } // Store the updated date.
    public string? Description { get; set; } // Store the updated description.
} // Close the class block.
