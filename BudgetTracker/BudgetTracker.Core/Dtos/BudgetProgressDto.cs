namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class BudgetProgressDto // Define budget progress data.
{ // Open the class block.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public decimal Budgeted { get; set; } // Store budgeted amount.
    public decimal Actual { get; set; } // Store actual amount.
    public decimal Percentage { get; set; } // Store percent used.
    public bool OverBudget { get; set; } // Store over budget flag.
} // Close the class block.
