using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using BudgetTracker.Web.Models; // Import view models.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

public class BudgetsController : Controller // Define budgets controller.
{ // Open the class block.
    private readonly IBudgetService _budgets; // Hold budget service.
    private readonly ICategoryService _categories; // Hold category service.

    public BudgetsController(IBudgetService budgets, ICategoryService categories) // Define constructor.
    { // Open the constructor block.
        _budgets = budgets; // Assign budget service.
        _categories = categories; // Assign category service.
    } // Close the constructor block.

    public async Task<IActionResult> Index(string? month) // Render budgets list.
    { // Open the method block.
        var selectedMonth = ResolveMonth(month); // Resolve month.
        var model = await BuildIndexModelAsync(selectedMonth, new CreateBudgetDto { Month = selectedMonth }); // Build view model.
        return View(model); // Return view.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create(CreateBudgetDto dto) // Create budget.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            await _budgets.CreateAsync(dto); // Create budget.
            return RedirectToAction(nameof(Index), new { month = dto.Month.ToString("yyyy-MM") }); // Redirect to list.
        } // Close try block.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) // Handle validation/conflicts.
        { // Open catch block.
            var selectedMonth = ResolveMonth(dto.Month.ToString("yyyy-MM")); // Keep month.
            var model = await BuildIndexModelAsync(selectedMonth, dto); // Rebuild model.
            model.ErrorMessage = ex.Message; // Store error.
            return View("Index", model); // Return view.
        } // Close catch block.
    } // Close the method block.

    public async Task<IActionResult> Edit(int id, string? month) // Render edit form.
    { // Open the method block.
        var budget = await _budgets.GetByIdAsync(id); // Fetch budget.
        if (budget is null) // Check for missing budget.
            return NotFound(); // Return 404.

        var categories = await _categories.GetAllAsync(); // Fetch categories.
        var returnMonth = month ?? budget.Month.ToString("yyyy-MM"); // Resolve return month.
        return View(new BudgetEditViewModel // Build view model.
        { // Open initializer block.
            Id = id, // Map id.
            Categories = categories, // Map categories.
            ReturnMonth = returnMonth, // Map return month.
            Budget = new UpdateBudgetDto // Map update payload.
            { // Open initializer block.
                CategoryId = budget.CategoryId, // Map category id.
                Month = budget.Month, // Map month.
                Amount = budget.Amount // Map amount.
            } // Close initializer block.
        }); // Close initializer block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Edit(int id, UpdateBudgetDto dto, string? returnMonth) // Update budget.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _budgets.UpdateAsync(id, dto); // Update budget.
            if (updated is null) // Check for missing budget.
                return NotFound(); // Return 404.
            return RedirectToAction(nameof(Index), new { month = returnMonth ?? dto.Month.ToString("yyyy-MM") }); // Redirect to list.
        } // Close try block.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) // Handle validation/conflicts.
        { // Open catch block.
            var categories = await _categories.GetAllAsync(); // Fetch categories.
            return View(new BudgetEditViewModel // Return view with error.
            { // Open initializer block.
                Id = id, // Map id.
                Categories = categories, // Map categories.
                ReturnMonth = returnMonth, // Map return month.
                Budget = dto, // Map payload.
                ErrorMessage = ex.Message // Map error.
            }); // Close initializer block.
        } // Close catch block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Delete(int id, string? month) // Delete budget.
    { // Open the method block.
        await _budgets.DeleteAsync(id); // Delete budget.
        return RedirectToAction(nameof(Index), new { month }); // Redirect to list.
    } // Close the method block.

    private async Task<BudgetsIndexViewModel> BuildIndexModelAsync(DateTime selectedMonth, CreateBudgetDto dto) // Build index view model.
    { // Open the method block.
        var budgets = await _budgets.GetByMonthAsync(selectedMonth); // Fetch budgets.
        var categories = await _categories.GetAllAsync(); // Fetch categories.

        return new BudgetsIndexViewModel // Build view model.
        { // Open initializer block.
            Budgets = budgets, // Map budgets.
            Categories = categories, // Map categories.
            SelectedMonth = selectedMonth, // Map selected month.
            NewBudget = dto // Map create payload.
        }; // Close initializer block.
    } // Close the method block.

    private static DateTime ResolveMonth(string? month) // Resolve month string.
    { // Open the method block.
        if (!string.IsNullOrWhiteSpace(month) && DateTime.TryParse($"{month}-01", out var parsed)) // Try parse.
            return new DateTime(parsed.Year, parsed.Month, 1, 0, 0, 0, DateTimeKind.Utc); // Return parsed month.

        var now = DateTime.UtcNow; // Capture current date.
        return new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc); // Return current month.
    } // Close the method block.
} // Close the class block.
