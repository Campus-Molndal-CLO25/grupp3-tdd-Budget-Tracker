namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CreateBudgetDto // Define budget creation payload.
{ // Open the class block.
    public DateTime Month { get; set; } // Store the budget month.
    public int CategoryId { get; set; } // Store the category id.
    public decimal Amount { get; set; } // Store the budget amount.
} // Close the class block.
