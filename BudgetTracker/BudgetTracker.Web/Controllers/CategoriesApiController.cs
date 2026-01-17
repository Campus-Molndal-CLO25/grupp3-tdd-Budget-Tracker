using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/categories")] // Set controller route.
public class CategoriesApiController : ControllerBase // Define categories controller.
{ // Open the class block.
    private readonly ICategoryService _service; // Hold category service.

    public CategoriesApiController(ICategoryService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign service.
    } // Close the constructor block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto) // Create category.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var created = await _service.CreateAsync(dto); // Create category.
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // Return 201.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return BadRequest(new { error = ex.Message }); // Return 400.
        } // Close catch block.
        catch (InvalidOperationException ex) // Handle conflicts.
        { // Open catch block.
            return Conflict(new { error = ex.Message }); // Return 409.
        } // Close catch block.
    } // Close the method block.

    [HttpGet] // Handle GET requests.
    public async Task<IActionResult> GetAll() // Get all categories.
    { // Open the method block.
        var categories = await _service.GetAllAsync(); // Fetch categories.
        return Ok(categories); // Return 200.
    } // Close the method block.

    [HttpGet("{id:int}")] // Handle GET by id.
    public async Task<IActionResult> GetById(int id) // Get category by id.
    { // Open the method block.
        var category = await _service.GetByIdAsync(id); // Fetch category.
        return category is null ? NotFound() : Ok(category); // Return 404 or 200.
    } // Close the method block.

    [HttpPut("{id:int}")] // Handle PUT by id.
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto) // Update category.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _service.UpdateAsync(id, dto); // Update category.
            return updated is null ? NotFound() : Ok(updated); // Return 404 or 200.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return BadRequest(new { error = ex.Message }); // Return 400.
        } // Close catch block.
        catch (InvalidOperationException ex) // Handle conflicts.
        { // Open catch block.
            return Conflict(new { error = ex.Message }); // Return 409.
        } // Close catch block.
    } // Close the method block.

    [HttpDelete("{id:int}")] // Handle DELETE by id.
    public async Task<IActionResult> Delete(int id) // Delete category.
    { // Open the method block.
        var deleted = await _service.DeleteAsync(id); // Delete category.
        return deleted ? NoContent() : NotFound(); // Return 204 or 404.
    } // Close the method block.
} // Close the class block.
