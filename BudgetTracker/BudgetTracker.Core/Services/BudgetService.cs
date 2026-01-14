using BudgetTracker.Core.Domain; // Import domain models.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class BudgetService : IBudgetService // Implement budget service.
{ // Open the class block.
    private readonly IBudgetRepository _budgets; // Hold budget repository.
    private readonly ICategoryRepository _categories; // Hold category repository.

    public BudgetService(IBudgetRepository budgets, ICategoryRepository categories) // Define constructor.
    { // Open the constructor block.
        _budgets = budgets; // Assign budget repository.
        _categories = categories; // Assign category repository.
    } // Close the constructor block.

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto dto) // Create a budget.
    { // Open the method block.
        if (dto.Amount <= 0) // Validate amount.
            throw new ArgumentException("Amount must be greater than zero"); // Throw when invalid.

        var category = await _categories.GetByIdAsync(dto.CategoryId); // Fetch category.
        if (category is null) // Check for missing category.
            throw new ArgumentException("Category not found"); // Throw when missing.

        var normalizedMonth = NormalizeMonth(dto.Month); // Normalize month.
        var existing = await _budgets.GetByCategoryAndMonthAsync(dto.CategoryId, normalizedMonth); // Check for duplicate.
        if (existing is not null) // Check for existing budget.
            throw new InvalidOperationException("Budget already exists for this category and month"); // Throw when duplicate.

        var budget = new Budget // Create budget entity.
        { // Open the initializer block.
            CategoryId = dto.CategoryId, // Assign category id.
            Month = normalizedMonth, // Assign normalized month.
            Amount = dto.Amount // Assign amount.
        }; // Close the initializer block.

        var created = await _budgets.AddAsync(budget); // Persist budget.
        created.Category = category; // Attach category.
        return MapBudget(created); // Map to DTO.
    } // Close the method block.

    public async Task<List<BudgetDto>> GetByMonthAsync(DateTime month) // Fetch budgets by month.
    { // Open the method block.
        var normalizedMonth = NormalizeMonth(month); // Normalize month.
        var budgets = await _budgets.GetByMonthAsync(normalizedMonth); // Query budgets.
        return budgets.Select(MapBudget).ToList(); // Map to DTOs.
    } // Close the method block.

    public async Task<BudgetDto?> UpdateAsync(int id, UpdateBudgetDto dto) // Update a budget.
    { // Open the method block.
        if (dto.Amount <= 0) // Validate amount.
            throw new ArgumentException("Amount must be greater than zero"); // Throw when invalid.

        var budget = await _budgets.GetByIdAsync(id); // Fetch budget.
        if (budget is null) // Check for missing budget.
            return null; // Return null when missing.

        var category = await _categories.GetByIdAsync(dto.CategoryId); // Fetch category.
        if (category is null) // Check for missing category.
            throw new ArgumentException("Category not found"); // Throw when missing.

        var normalizedMonth = NormalizeMonth(dto.Month); // Normalize month.
        var duplicate = await _budgets.GetByCategoryAndMonthAsync(dto.CategoryId, normalizedMonth); // Check for duplicate.
        if (duplicate is not null && duplicate.Id != budget.Id) // Detect duplicate budget.
            throw new InvalidOperationException("Budget already exists for this category and month"); // Throw when duplicate.

        budget.CategoryId = dto.CategoryId; // Update category id.
        budget.Month = normalizedMonth; // Update month.
        budget.Amount = dto.Amount; // Update amount.
        budget.Category = category; // Attach category.

        await _budgets.SaveChangesAsync(); // Persist changes.
        return MapBudget(budget); // Map to DTO.
    } // Close the method block.

    public async Task<bool> DeleteAsync(int id) // Delete a budget.
    { // Open the method block.
        var budget = await _budgets.GetByIdAsync(id); // Fetch budget.
        if (budget is null) // Check for missing budget.
            return false; // Return false when missing.

        await _budgets.DeleteAsync(budget); // Remove budget.
        await _budgets.SaveChangesAsync(); // Persist changes.
        return true; // Return success.
    } // Close the method block.

    private static DateTime NormalizeMonth(DateTime month) // Normalize to first day.
    { // Open the method block.
        return new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc); // Build normalized month.
    } // Close the method block.

    private static BudgetDto MapBudget(Budget budget) // Map budget to DTO.
    { // Open the method block.
        return new BudgetDto // Create DTO.
        { // Open the initializer block.
            Id = budget.Id, // Map id.
            CategoryId = budget.CategoryId, // Map category id.
            CategoryName = budget.Category?.Name ?? string.Empty, // Map category name.
            Month = budget.Month, // Map month.
            Amount = budget.Amount // Map amount.
        }; // Close the initializer block.
    } // Close the method block.
} // Close the class block.
