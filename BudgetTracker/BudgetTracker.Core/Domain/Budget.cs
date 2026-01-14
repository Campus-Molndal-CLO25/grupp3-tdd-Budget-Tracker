using System.ComponentModel.DataAnnotations; // Import data annotation attributes.

namespace BudgetTracker.Core.Domain; // Define the shared domain namespace.

public class Budget // Represent a monthly budget.
{ // Open the class block.
    public int Id { get; set; } // Store the primary key.

    public int CategoryId { get; set; } // Store the category id.
    public Category? Category { get; set; } // Reference the category.

    public DateTime Month { get; set; } // Store the normalized month value.

    [Range(0.01, double.MaxValue)] // Ensure a positive budget amount.
    public decimal Amount { get; set; } // Store the budget amount.
} // Close the class block.
