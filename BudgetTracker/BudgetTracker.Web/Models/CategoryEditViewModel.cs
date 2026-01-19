using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class CategoryEditViewModel // Define category edit view model.
{ // Open the class block.
    public int Id { get; set; } // Store category id.
    public UpdateCategoryDto Category { get; set; } = new(); // Store update payload.
    public string? ErrorMessage { get; set; } // Store error message.
} // Close the class block.
