namespace BudgetTracker.Core.Services.Interfaces; // Define service namespace.

public interface IInsightsService // Define insights service contract.
{ // Open the interface block.
    Task<string> GetMonthlyInsightAsync(int year, int month, CancellationToken ct = default); // Get monthly advice.
    Task<string> GetAdviceAsync(string prompt, CancellationToken ct = default); // Get free-form advice.
} // Close the interface block.
