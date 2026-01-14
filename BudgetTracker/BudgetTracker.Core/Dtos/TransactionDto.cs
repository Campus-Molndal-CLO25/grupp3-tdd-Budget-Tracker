using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class TransactionDto // Define transaction response data.
{ // Open the class block.
    public int Id { get; set; } // Store the transaction id.
    public int AccountId { get; set; } // Store the account id.
    public string AccountName { get; set; } = ""; // Store the account name.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public decimal Amount { get; set; } // Store the amount.
    public TransactionType Type { get; set; } // Store the transaction type.
    public DateTime Date { get; set; } // Store the date.
    public string? Description { get; set; } // Store the description.
} // Close the class block.
