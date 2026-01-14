namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class DashboardDto // Define dashboard response data.
{ // Open the class block.
    public decimal TotalBalance { get; set; } // Store total account balance.
    public decimal MonthIncome { get; set; } // Store month income.
    public decimal MonthExpense { get; set; } // Store month expense.
    public List<CategoryBreakdownDto> TopExpenseCategories { get; set; } = new(); // Store top expenses.
    public List<BudgetProgressDto> BudgetProgress { get; set; } = new(); // Store budget progress.
    public List<TransactionDto> RecentTransactions { get; set; } = new(); // Store recent transactions.
} // Close the class block.
