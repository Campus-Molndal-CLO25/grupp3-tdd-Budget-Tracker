using System.ComponentModel.DataAnnotations; // Import data annotation attributes.

namespace BudgetTracker.Core.Domain; // Define the shared domain namespace.

public class Transaction // Represent an income or expense.
{ // Open the class block.
    public int Id { get; set; } // Store the primary key.

    public DateTime Date { get; set; } = DateTime.UtcNow; // Store the transaction date.

    [Range(0.01, double.MaxValue)] // Ensure a positive amount.
    public decimal Amount { get; set; } // Store the transaction amount.

    public TransactionType TransactionType { get; set; } // Store the transaction type.

    [MaxLength(500)] // Limit description length.
    public string? Description { get; set; } // Store the optional description.

    public int AccountId { get; set; } // Store the owning account id.
    public Account? Account { get; set; } // Reference the owning account.

    public int CategoryId { get; set; } // Store the category id.
    public Category? Category { get; set; } // Reference the category.
} // Close the class block.
