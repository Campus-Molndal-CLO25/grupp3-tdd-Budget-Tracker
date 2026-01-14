using System.ComponentModel.DataAnnotations; // Import data annotation attributes.

namespace BudgetTracker.Core.Domain; // Define the shared domain namespace.

public class Category // Represent a budget category.
{ // Open the class block.
    public int Id { get; set; } // Store the primary key.

    [Required] // Require a name value.
    [MaxLength(100)] // Limit name length.
    public string Name { get; set; } = ""; // Store the category name.

    [MaxLength(24)] // Limit color token length.
    public string? Color { get; set; } // Store the optional color value.

    public CategoryType CategoryType { get; set; } // Store the category type.

    public List<Transaction> Transactions { get; set; } = new(); // Track category transactions.
    public List<Budget> Budgets { get; set; } = new(); // Track category budgets.
} // Close the class block.
