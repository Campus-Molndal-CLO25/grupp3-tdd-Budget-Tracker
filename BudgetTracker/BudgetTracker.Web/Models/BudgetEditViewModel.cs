using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class BudgetEditViewModel // Define budget edit view model.
{ // Open the class block.
    public int Id { get; set; } // Store budget id.
    public UpdateBudgetDto Budget { get; set; } = new(); // Store update payload.
    public List<CategoryDto> Categories { get; set; } = new(); // Store categories.
    public string? ErrorMessage { get; set; } // Store error message.
    public string? ReturnMonth { get; set; } // Store return month param.
} // Close the class block.
