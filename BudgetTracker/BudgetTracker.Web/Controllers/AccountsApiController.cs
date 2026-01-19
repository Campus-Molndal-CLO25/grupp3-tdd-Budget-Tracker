using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/accounts")] // Set controller route.
public class AccountsApiController : ControllerBase // Define accounts controller.
{ // Open the class block.
    private readonly IAccountService _service; // Hold account service.

    public AccountsApiController(IAccountService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign service.
    } // Close the constructor block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto) // Create a new account.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var created = await _service.CreateAsync(dto); // Create account.
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // Return 201 with location.
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
    public async Task<IActionResult> GetAll() // Get all accounts.
    { // Open the method block.
        var accounts = await _service.GetAllAsync(); // Fetch accounts.
        return Ok(accounts); // Return 200.
    } // Close the method block.

    [HttpGet("{id:int}")] // Handle GET by id.
    public async Task<IActionResult> GetById(int id) // Get account by id.
    { // Open the method block.
        var account = await _service.GetByIdAsync(id); // Fetch account.
        return account is null ? NotFound() : Ok(account); // Return 404 or 200.
    } // Close the method block.

    [HttpPut("{id:int}")] // Handle PUT by id.
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountDto dto) // Update account.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _service.UpdateAsync(id, dto); // Update account.
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
    public async Task<IActionResult> Delete(int id) // Delete account.
    { // Open the method block.
        var deleted = await _service.DeleteAsync(id); // Delete account.
        return deleted ? NoContent() : NotFound(); // Return 204 or 404.
    } // Close the method block.
} // Close the class block.
