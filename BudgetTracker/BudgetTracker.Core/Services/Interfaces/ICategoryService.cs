using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface ICategoryService // Define category service contract.
{ // Open the interface block.
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto); // Create a category.
    Task<List<CategoryDto>> GetAllAsync(); // Fetch all categories.
    Task<CategoryDto?> GetByIdAsync(int id); // Fetch category by id.
    Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto); // Update a category.
    Task<bool> DeleteAsync(int id); // Delete a category.
} // Close the interface block.
