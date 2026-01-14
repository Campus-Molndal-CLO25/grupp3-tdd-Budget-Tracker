using BudgetTracker.Core.Clients; // Import Gemini client.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.Extensions.Caching.Memory; // Import caching.

namespace BudgetTracker.Core.Services; // Define service namespace.

public class InsightsService : IInsightsService // Implement insights service.
{ // Open the class block.
    private readonly IGeminiClient _gemini; // Hold Gemini client.
    private readonly IMemoryCache _cache; // Hold cache.

    public InsightsService(IGeminiClient gemini, IMemoryCache cache) // Define constructor.
    { // Open the constructor block.
        _gemini = gemini; // Assign client.
        _cache = cache; // Assign cache.
    } // Close the constructor block.

    public async Task<string> GetMonthlyInsightAsync(int year, int month, CancellationToken ct = default) // Get monthly advice.
    { // Open the method block.
        var cacheKey = $"insight:{year:D4}-{month:D2}"; // Build cache key.
        if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null) // Check cache.
            return cached; // Return cached value.

        var prompt = $"Give short budgeting advice for {year}-{month:D2}."; // Build prompt.
        var insight = await _gemini.GenerateInsightAsync(prompt, ct); // Call Gemini.
        _cache.Set(cacheKey, insight, TimeSpan.FromMinutes(30)); // Cache result.
        return insight; // Return insight.
    } // Close the method block.

    public async Task<string> GetAdviceAsync(string prompt, CancellationToken ct = default) // Get free-form advice.
    { // Open the method block.
        if (string.IsNullOrWhiteSpace(prompt)) // Validate prompt.
            throw new ArgumentException("Prompt is required"); // Throw when prompt missing.

        var normalized = prompt.Trim(); // Normalize prompt.
        var cacheKey = $"advice:{normalized.ToLowerInvariant()}"; // Build cache key.
        if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null) // Check cache.
            return cached; // Return cached value.

        var shapedPrompt = $"Give concise, actionable saving advice: {normalized}"; // Shape prompt.
        var insight = await _gemini.GenerateInsightAsync(shapedPrompt, ct); // Call Gemini.
        _cache.Set(cacheKey, insight, TimeSpan.FromMinutes(30)); // Cache result.
        return insight; // Return insight.
    } // Close the method block.
} // Close the class block.
