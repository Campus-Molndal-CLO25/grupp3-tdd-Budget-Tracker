using BudgetTracker.Core.Data; // Import DbContext.
using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Repositories.Ef; // Define EF repository namespace.

public class CategoryRepository : ICategoryRepository // Implement category repository.
{ // Open the class block.
    private readonly BudgetTrackerDbContext _db; // Hold the DbContext.

    public CategoryRepository(BudgetTrackerDbContext db) // Define constructor.
    { // Open the constructor block.
        _db = db; // Assign the DbContext.
    } // Close the constructor block.

    public Task<bool> NameExistsAsync(string name) // Check for duplicate names.
    { // Open the method block.
        return _db.Categories.AnyAsync(c => c.Name == name); // Query for name match.
    } // Close the method block.

    public async Task<Category> AddAsync(Category category) // Add a category.
    { // Open the method block.
        _db.Categories.Add(category); // Add entity to the context.
        await _db.SaveChangesAsync(); // Persist changes.
        return category; // Return the saved category.
    } // Close the method block.

    public Task<List<Category>> GetAllAsync() // Fetch all categories.
    { // Open the method block.
        return _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(); // Query categories ordered by name.
    } // Close the method block.

    public Task<Category?> GetByIdAsync(int id) // Fetch category by id.
    { // Open the method block.
        return _db.Categories.FirstOrDefaultAsync(c => c.Id == id); // Query category by id.
    } // Close the method block.

    public Task DeleteAsync(Category category) // Delete a category.
    { // Open the method block.
        _db.Categories.Remove(category); // Remove the entity.
        return Task.CompletedTask; // Return completed task.
    } // Close the method block.

    public Task SaveChangesAsync() // Persist changes.
    { // Open the method block.
        return _db.SaveChangesAsync(); // Save changes in the context.
    } // Close the method block.
} // Close the class block.
