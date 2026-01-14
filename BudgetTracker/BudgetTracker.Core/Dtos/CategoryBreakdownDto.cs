namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CategoryBreakdownDto // Define category breakdown data.
{ // Open the class block.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public decimal TotalIncome { get; set; } // Store total income.
    public decimal TotalExpense { get; set; } // Store total expense.
} // Close the class block.
