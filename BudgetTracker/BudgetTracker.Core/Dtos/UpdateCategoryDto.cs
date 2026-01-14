using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class UpdateCategoryDto // Define category update payload.
{ // Open the class block.
    public string Name { get; set; } = ""; // Store the updated name.
    public CategoryType Type { get; set; } // Store the updated type.
    public string? Color { get; set; } // Store the updated color.
} // Close the class block.
