namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class MonthlySummaryDto // Define monthly summary report.
{ // Open the class block.
    public int Year { get; set; } // Store the report year.
    public int Month { get; set; } // Store the report month.
    public decimal TotalIncome { get; set; } // Store total income.
    public decimal TotalExpense { get; set; } // Store total expense.
    public decimal NetSavings { get; set; } // Store net savings.
    public decimal SavingsRate { get; set; } // Store savings rate.
    public decimal PreviousNetSavings { get; set; } // Store previous month net savings.
    public decimal NetSavingsChange { get; set; } // Store net savings change.
    public List<MonthlySummaryCategoryDto> Categories { get; set; } = new(); // Store category breakdown.
} // Close the class block.
