using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class CategoryService : ICategoryService // Implement category service.
{ // Open the class block.
    private readonly ICategoryRepository _categories; // Hold category repository.

    public CategoryService(ICategoryRepository categories) // Define constructor.
    { // Open the constructor block.
        _categories = categories; // Assign repository.
    } // Close the constructor block.

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto) // Create a category.
    { // Open the method block.
        if (string.IsNullOrWhiteSpace(dto.Name)) // Validate name.
            throw new ArgumentException("Name is required"); // Throw when name missing.

        if (await _categories.NameExistsAsync(dto.Name.Trim())) // Check for duplicate name.
            throw new InvalidOperationException("Category name must be unique"); // Throw when duplicate.

        var category = new Category // Create the category entity.
        { // Open the initializer block.
            Name = dto.Name.Trim(), // Assign name.
            CategoryType = dto.Type, // Assign type.
            Color = dto.Color // Assign optional color.
        }; // Close the initializer block.

        var created = await _categories.AddAsync(category); // Persist category.
        return MapCategory(created); // Map to DTO.
    } // Close the method block.

    public async Task<List<CategoryDto>> GetAllAsync() // Fetch all categories.
    { // Open the method block.
        var categories = await _categories.GetAllAsync(); // Query categories.
        return categories.Select(MapCategory).ToList(); // Map to DTOs.
    } // Close the method block.

    public async Task<CategoryDto?> GetByIdAsync(int id) // Fetch category by id.
    { // Open the method block.
        var category = await _categories.GetByIdAsync(id); // Query category.
        return category is null ? null : MapCategory(category); // Map or return null.
    } // Close the method block.

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto) // Update category.
    { // Open the method block.
        var category = await _categories.GetByIdAsync(id); // Fetch category.
        if (category is null) // Check for missing category.
            return null; // Return null when missing.

        if (string.IsNullOrWhiteSpace(dto.Name)) // Validate name.
            throw new ArgumentException("Name is required"); // Throw when name missing.

        var trimmed = dto.Name.Trim(); // Trim name.
        if (!string.Equals(category.Name, trimmed, StringComparison.OrdinalIgnoreCase)) // Check for name change.
        { // Open the if block.
            if (await _categories.NameExistsAsync(trimmed)) // Check for duplicate name.
                throw new InvalidOperationException("Category name must be unique"); // Throw when duplicate.
        } // Close the if block.

        category.Name = trimmed; // Update name.
        category.CategoryType = dto.Type; // Update type.
        category.Color = dto.Color; // Update color.

        await _categories.SaveChangesAsync(); // Persist changes.
        return MapCategory(category); // Map to DTO.
    } // Close the method block.

    public async Task<bool> DeleteAsync(int id) // Delete category.
    { // Open the method block.
        var category = await _categories.GetByIdAsync(id); // Fetch category.
        if (category is null) // Check for missing category.
            return false; // Return false when missing.

        await _categories.DeleteAsync(category); // Remove category.
        await _categories.SaveChangesAsync(); // Persist changes.
        return true; // Return success.
    } // Close the method block.

    private static CategoryDto MapCategory(Category category) // Map category to DTO.
    { // Open the method block.
        return new CategoryDto // Create DTO.
        { // Open the initializer block.
            Id = category.Id, // Map id.
            Name = category.Name, // Map name.
            Type = category.CategoryType, // Map type.
            Color = category.Color // Map color.
        }; // Close the initializer block.
    } // Close the method block.
} // Close the class block.
