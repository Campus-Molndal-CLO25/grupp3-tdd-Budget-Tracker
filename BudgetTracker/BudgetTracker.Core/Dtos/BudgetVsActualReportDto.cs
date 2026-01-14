namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class BudgetVsActualReportDto // Define budget vs actual report.
{ // Open the class block.
    public int Year { get; set; } // Store the report year.
    public int Month { get; set; } // Store the report month.
    public decimal TotalBudgeted { get; set; } // Store total budgeted amount.
    public decimal TotalActual { get; set; } // Store total actual amount.
    public decimal TotalDifference { get; set; } // Store total difference.
    public List<BudgetReportCategoryDto> Categories { get; set; } = new(); // Store category rows.
} // Close the class block.
