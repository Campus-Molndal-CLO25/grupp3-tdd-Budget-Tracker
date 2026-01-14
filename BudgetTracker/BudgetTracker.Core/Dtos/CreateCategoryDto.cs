using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CreateCategoryDto // Define category creation payload.
{ // Open the class block.
    public string Name { get; set; } = ""; // Store the category name.
    public CategoryType Type { get; set; } // Store the category type.
    public string? Color { get; set; } // Store the optional color.
} // Close the class block.
