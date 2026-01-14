namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class MonthlySummaryCategoryDto // Define monthly summary category data.
{ // Open the class block.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public decimal Income { get; set; } // Store total income.
    public decimal Expense { get; set; } // Store total expense.
} // Close the class block.
