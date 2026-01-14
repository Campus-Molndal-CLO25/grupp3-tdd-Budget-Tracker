using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CategoryDto // Define category response data.
{ // Open the class block.
    public int Id { get; set; } // Store the category id.
    public string Name { get; set; } = ""; // Store the category name.
    public CategoryType Type { get; set; } // Store the category type.
    public string? Color { get; set; } // Store the category color.
} // Close the class block.
