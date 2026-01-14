namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class BudgetDto // Define budget response data.
{ // Open the class block.
    public int Id { get; set; } // Store the budget id.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public DateTime Month { get; set; } // Store the month.
    public decimal Amount { get; set; } // Store the budget amount.
} // Close the class block.
