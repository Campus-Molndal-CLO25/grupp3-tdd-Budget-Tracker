using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using BudgetTracker.Web.Models; // Import view models.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

public class CategoriesController : Controller // Define categories controller.
{ // Open the class block.
    private readonly ICategoryService _service; // Hold category service.

    public CategoriesController(ICategoryService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign service.
    } // Close the constructor block.

    public async Task<IActionResult> Index() // Render categories list.
    { // Open the method block.
        var model = await BuildIndexModelAsync(); // Build view model.
        return View(model); // Return view.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create(CreateCategoryDto dto) // Create category.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            await _service.CreateAsync(dto); // Create category.
            return RedirectToAction(nameof(Index)); // Redirect to list.
        } // Close try block.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) // Handle validation/conflicts.
        { // Open catch block.
            var model = await BuildIndexModelAsync(); // Rebuild model.
            model.NewCategory = dto; // Preserve input.
            model.ErrorMessage = ex.Message; // Store error.
            return View("Index", model); // Return view.
        } // Close catch block.
    } // Close the method block.

    public async Task<IActionResult> Edit(int id) // Render edit form.
    { // Open the method block.
        var category = await _service.GetByIdAsync(id); // Fetch category.
        if (category is null) // Check for missing category.
            return NotFound(); // Return 404.

        return View(new CategoryEditViewModel // Build view model.
        { // Open initializer block.
            Id = id, // Map id.
            Category = new UpdateCategoryDto // Map update payload.
            { // Open initializer block.
                Name = category.Name, // Map name.
                Type = category.Type, // Map type.
                Color = category.Color // Map color.
            } // Close initializer block.
        }); // Close initializer block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Edit(int id, UpdateCategoryDto dto) // Update category.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _service.UpdateAsync(id, dto); // Update category.
            if (updated is null) // Check for missing category.
                return NotFound(); // Return 404.
            return RedirectToAction(nameof(Index)); // Redirect to list.
        } // Close try block.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) // Handle validation/conflicts.
        { // Open catch block.
            return View(new CategoryEditViewModel // Return view with error.
            { // Open initializer block.
                Id = id, // Map id.
                Category = dto, // Map payload.
                ErrorMessage = ex.Message // Map error.
            }); // Close initializer block.
        } // Close catch block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Delete(int id) // Delete category.
    { // Open the method block.
        await _service.DeleteAsync(id); // Delete category.
        return RedirectToAction(nameof(Index)); // Redirect to list.
    } // Close the method block.

    private async Task<CategoriesIndexViewModel> BuildIndexModelAsync() // Build index view model.
    { // Open the method block.
        var categories = await _service.GetAllAsync(); // Fetch categories.
        return new CategoriesIndexViewModel // Build view model.
        { // Open initializer block.
            Categories = categories // Map categories.
        }; // Close initializer block.
    } // Close the method block.
} // Close the class block.
