using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class BudgetsIndexViewModel // Define budgets index view model.
{ // Open the class block.
    public List<BudgetDto> Budgets { get; set; } = new(); // Store budgets.
    public List<CategoryDto> Categories { get; set; } = new(); // Store categories.
    public CreateBudgetDto NewBudget { get; set; } = new(); // Store create payload.
    public DateTime SelectedMonth { get; set; } // Store selected month.
    public string? ErrorMessage { get; set; } // Store error message.
} // Close the class block.
