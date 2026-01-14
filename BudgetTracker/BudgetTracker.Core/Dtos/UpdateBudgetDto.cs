namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class UpdateBudgetDto // Define budget update payload.
{ // Open the class block.
    public DateTime Month { get; set; } // Store the updated month.
    public int CategoryId { get; set; } // Store the updated category id.
    public decimal Amount { get; set; } // Store the updated amount.
} // Close the class block.
