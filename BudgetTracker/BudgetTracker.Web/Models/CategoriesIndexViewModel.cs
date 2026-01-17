using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class CategoriesIndexViewModel // Define categories index view model.
{ // Open the class block.
    public List<CategoryDto> Categories { get; set; } = new(); // Store categories.
    public CreateCategoryDto NewCategory { get; set; } = new(); // Store create payload.
    public string? ErrorMessage { get; set; } // Store error message.
} // Close the class block.
