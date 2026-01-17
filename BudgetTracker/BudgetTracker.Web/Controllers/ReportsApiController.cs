using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

[ApiController] // Enable API conventions.
[Route("api/reports")] // Set controller route.
public class ReportsApiController : ControllerBase // Define reports controller.
{ // Open the class block.
    private readonly IReportService _reports; // Hold report service.

    public ReportsApiController(IReportService reports) // Define constructor.
    { // Open the constructor block.
        _reports = reports; // Assign report service.
    } // Close the constructor block.

    [HttpGet("budget-vs-actual")] // Handle budget vs actual report.
    public async Task<IActionResult> GetBudgetVsActual([FromQuery] int year, [FromQuery] int month) // Get report.
    { // Open the method block.
        if (year <= 0 || month is < 1 or > 12) // Validate year and month.
            return BadRequest(new { error = "Invalid year or month" }); // Return 400.

        var report = await _reports.GetBudgetVsActualAsync(year, month); // Fetch report.
        return Ok(report); // Return 200.
    } // Close the method block.

    [HttpGet("monthly-summary")] // Handle monthly summary report.
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month) // Get summary.
    { // Open the method block.
        if (year <= 0 || month is < 1 or > 12) // Validate year and month.
            return BadRequest(new { error = "Invalid year or month" }); // Return 400.

        var report = await _reports.GetMonthlySummaryAsync(year, month); // Fetch report.
        return Ok(report); // Return 200.
    } // Close the method block.

    [HttpGet("category-breakdown")] // Handle category breakdown report.
    public async Task<IActionResult> GetCategoryBreakdown([FromQuery] DateTime startDate, [FromQuery] DateTime endDate) // Get breakdown.
    { // Open the method block.
        if (startDate == default || endDate == default || endDate < startDate) // Validate date range.
            return BadRequest(new { error = "Invalid date range" }); // Return 400.

        var report = await _reports.GetCategoryBreakdownAsync(startDate, endDate); // Fetch report.
        return Ok(report); // Return 200.
    } // Close the method block.
} // Close the class block.
