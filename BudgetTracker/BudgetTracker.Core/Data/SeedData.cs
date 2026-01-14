using BudgetTracker.Core.Domain; // Import domain models.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Data; // Define data namespace.

public static class SeedData // Define seed helper.
{ // Open the class block.
    public static async Task EnsureSeededAsync(BudgetTrackerDbContext db) // Seed default categories if missing.
    { // Open the method block.
        if (await db.Categories.AnyAsync()) // Check if categories already exist.
            return; // Exit early when data exists.

        var defaults = new List<Category> // Define default categories.
        { // Open the list initializer.
            new Category { Name = "Salary", CategoryType = CategoryType.Income, Color = "#2f855a" }, // Add salary category.
            new Category { Name = "Rent", CategoryType = CategoryType.Expense, Color = "#c53030" }, // Add rent category.
            new Category { Name = "Food", CategoryType = CategoryType.Expense, Color = "#dd6b20" }, // Add food category.
            new Category { Name = "Entertainment", CategoryType = CategoryType.Expense, Color = "#805ad5" } // Add entertainment category.
        }; // Close the list initializer.

        db.Categories.AddRange(defaults); // Add default categories.
        await db.SaveChangesAsync(); // Persist seeded data.
    } // Close the method block.
} // Close the class block.
