using System.Net.Http.Json; // Import JSON helpers.

namespace BudgetTracker.Core.Clients; // Define client namespace.

public class GeminiClient : IGeminiClient // Implement Gemini API client.
{ // Open the class block.
    private readonly HttpClient _http; // Hold the HttpClient.

    public GeminiClient(HttpClient http) // Define constructor.
    { // Open the constructor block.
        _http = http; // Assign the HttpClient.
        if (!_http.DefaultRequestHeaders.Contains("X-Api-Key")) // Check if API key is missing.
        { // Open the if block.
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY"); // Read API key from environment.
            if (!string.IsNullOrWhiteSpace(apiKey)) // Validate API key.
                _http.DefaultRequestHeaders.Add("X-Api-Key", apiKey); // Add API key header.
        } // Close the if block.
    } // Close the constructor block.

    public async Task<string> GenerateInsightAsync(string prompt, CancellationToken ct = default) // Request an insight.
    { // Open the method block.
        var request = new { prompt }; // Build the request payload.
        var response = await _http.PostAsJsonAsync("/generate", request, ct); // Send the POST request.
        response.EnsureSuccessStatusCode(); // Throw for non-success status.
        var data = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: ct); // Parse response body.
        return data?.Text ?? ""; // Return the response text.
    } // Close the method block.

    private sealed class GeminiResponse // Define minimal response shape.
    { // Open the class block.
        public string Text { get; set; } = ""; // Store response text.
    } // Close the class block.
} // Close the class block.
