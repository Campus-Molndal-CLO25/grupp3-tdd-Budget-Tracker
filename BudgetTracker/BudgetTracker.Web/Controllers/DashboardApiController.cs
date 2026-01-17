using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/dashboard")] // Set controller route.
public class DashboardApiController : ControllerBase // Define dashboard controller.
{ // Open the class block.
    private readonly IDashboardService _dashboard; // Hold dashboard service.

    public DashboardApiController(IDashboardService dashboard) // Define constructor.
    { // Open the constructor block.
        _dashboard = dashboard; // Assign dashboard service.
    } // Close the constructor block.

    [HttpGet] // Handle GET requests.
    public async Task<IActionResult> Get([FromQuery] int? year, [FromQuery] int? month) // Get dashboard data.
    { // Open the method block.
        var now = DateTime.UtcNow; // Capture current date.
        var targetYear = year ?? now.Year; // Resolve year.
        var targetMonth = month ?? now.Month; // Resolve month.
        if (targetMonth is < 1 or > 12) // Validate month.
            return BadRequest(new { error = "Invalid month" }); // Return 400.

        var data = await _dashboard.GetDashboardAsync(targetYear, targetMonth); // Fetch dashboard data.
        return Ok(data); // Return 200.
    } // Close the method block.
} // Close the class block.
