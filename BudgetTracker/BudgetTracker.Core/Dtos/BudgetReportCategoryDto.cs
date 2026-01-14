using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class BudgetReportCategoryDto // Define budget report category data.
{ // Open the class block.
    public int CategoryId { get; set; } // Store the category id.
    public string CategoryName { get; set; } = ""; // Store the category name.
    public decimal Budgeted { get; set; } // Store the budgeted amount.
    public decimal Actual { get; set; } // Store the actual spend.
    public decimal Difference { get; set; } // Store the budget difference.
    public decimal Percentage { get; set; } // Store the budget percentage.
    public BudgetStatus Status { get; set; } // Store the budget status.
} // Close the class block.
