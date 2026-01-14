using BudgetTracker.Core.Domain; // Import domain models.

namespace BudgetTracker.Core.Repositories.Interfaces; // Define repository namespace.

public interface ICategoryRepository // Define category repository contract.
{ // Open the interface block.
    Task<bool> NameExistsAsync(string name); // Check for duplicate category name.
    Task<Category> AddAsync(Category category); // Add a category.
    Task<List<Category>> GetAllAsync(); // Fetch all categories.
    Task<Category?> GetByIdAsync(int id); // Fetch category by id.
    Task DeleteAsync(Category category); // Delete a category.
    Task SaveChangesAsync(); // Persist pending changes.
} // Close the interface block.
