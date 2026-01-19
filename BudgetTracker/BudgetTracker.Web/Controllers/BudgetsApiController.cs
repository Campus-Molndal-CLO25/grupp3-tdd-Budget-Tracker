using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/budgets")] // Set controller route.
public class BudgetsApiController : ControllerBase // Define budgets controller.
{ // Open the class block.
    private readonly IBudgetService _service; // Hold budget service.

    public BudgetsApiController(IBudgetService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign service.
    } // Close the constructor block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create([FromBody] CreateBudgetDto dto) // Create budget.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var created = await _service.CreateAsync(dto); // Create budget.
            return CreatedAtAction(nameof(GetByMonth), new { month = dto.Month.ToString("yyyy-MM") }, created); // Return 201.
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
    public async Task<IActionResult> GetByMonth([FromQuery] string month) // Get budgets for month.
    { // Open the method block.
        if (!DateTime.TryParse($"{month}-01", out var parsed)) // Validate month format.
            return BadRequest(new { error = "Invalid month format. Use YYYY-MM." }); // Return 400.

        var budgets = await _service.GetByMonthAsync(parsed); // Fetch budgets.
        return Ok(budgets); // Return 200.
    } // Close the method block.

    [HttpPut("{id:int}")] // Handle PUT by id.
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetDto dto) // Update budget.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _service.UpdateAsync(id, dto); // Update budget.
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
    public async Task<IActionResult> Delete(int id) // Delete budget.
    { // Open the method block.
        var deleted = await _service.DeleteAsync(id); // Delete budget.
        return deleted ? NoContent() : NotFound(); // Return 204 or 404.
    } // Close the method block.
} // Close the class block.
