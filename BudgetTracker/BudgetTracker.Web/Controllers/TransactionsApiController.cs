using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/transactions")] // Set controller route.
public class TransactionsApiController : ControllerBase // Define transactions controller.
{ // Open the class block.
    private readonly ITransactionService _service; // Hold transaction service.

    public TransactionsApiController(ITransactionService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign service.
    } // Close the constructor block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto) // Create transaction.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var created = await _service.CreateAsync(dto); // Create transaction.
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // Return 201.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return BadRequest(new { error = ex.Message }); // Return 400.
        } // Close catch block.
    } // Close the method block.

    [HttpGet] // Handle GET requests.
    public async Task<IActionResult> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? categoryId, [FromQuery] string? type, [FromQuery] int skip = 0, [FromQuery] int take = 50) // Get filtered transactions.
    { // Open the method block.
        var filter = new TransactionFilterDto // Build filter DTO.
        { // Open initializer block.
            StartDate = startDate, // Map start date.
            EndDate = endDate, // Map end date.
            CategoryId = categoryId, // Map category id.
            Type = Enum.TryParse(type, true, out BudgetTracker.Core.Domain.TransactionType parsed) ? parsed : null, // Parse type.
            Skip = skip, // Map skip.
            Take = take // Map take.
        }; // Close initializer block.

        var transactions = await _service.GetAllAsync(filter); // Fetch transactions.
        return Ok(transactions); // Return 200.
    } // Close the method block.

    [HttpGet("{id:int}")] // Handle GET by id.
    public async Task<IActionResult> GetById(int id) // Get transaction by id.
    { // Open the method block.
        var transaction = await _service.GetByIdAsync(id); // Fetch transaction.
        return transaction is null ? NotFound() : Ok(transaction); // Return 404 or 200.
    } // Close the method block.

    [HttpPut("{id:int}")] // Handle PUT by id.
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto dto) // Update transaction.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _service.UpdateAsync(id, dto); // Update transaction.
            return updated is null ? NotFound() : Ok(updated); // Return 404 or 200.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return BadRequest(new { error = ex.Message }); // Return 400.
        } // Close catch block.
    } // Close the method block.

    [HttpDelete("{id:int}")] // Handle DELETE by id.
    public async Task<IActionResult> Delete(int id) // Delete transaction.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var deleted = await _service.DeleteAsync(id); // Delete transaction.
            return deleted ? NoContent() : NotFound(); // Return 204 or 404.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return BadRequest(new { error = ex.Message }); // Return 400.
        } // Close catch block.
    } // Close the method block.
} // Close the class block.
