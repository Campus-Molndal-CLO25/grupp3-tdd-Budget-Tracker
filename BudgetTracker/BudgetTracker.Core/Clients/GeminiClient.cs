using System.Text; // Import text encoding.
using System.Text.Json; // Import JSON APIs.

namespace BudgetTracker.Core.Clients; // Define client namespace.

public class GeminiClient : IGeminiClient // Implement Gemini API client.
{ // Open the class block.
    private readonly HttpClient _http; // Hold the HttpClient.
    private readonly string? _apiKey; // Hold API key.
    private readonly string _model; // Hold model name.
    private const string DefaultModel = "gpt-4o-mini"; // Define default model.

    public GeminiClient(HttpClient http) // Define constructor.
    { // Open the constructor block.
        _http = http; // Assign the HttpClient.
        _apiKey = Environment.GetEnvironmentVariable("ONEMINAI_API_KEY"); // Read API key from environment.
        _model = Environment.GetEnvironmentVariable("ONEMINAI_MODEL") ?? DefaultModel; // Read model override.
        if (!string.IsNullOrWhiteSpace(_apiKey) && !_http.DefaultRequestHeaders.Contains("API-KEY")) // Check if API key is missing.
            _http.DefaultRequestHeaders.Add("API-KEY", _apiKey); // Add API key header.
    } // Close the constructor block.

    public async Task<string> GenerateInsightAsync(string prompt, CancellationToken ct = default) // Request an insight.
    { // Open the method block.
        if (string.IsNullOrWhiteSpace(_apiKey)) // Ensure API key exists.
            throw new HttpRequestException("ONEMINAI_API_KEY not set in environment variables."); // Throw missing key error.

        var conversationId = await CreateConversationAsync(ct); // Create a new conversation.
        return await SendQuestionAsync(conversationId, prompt, ct); // Send prompt.
    } // Close the method block.

    private async Task<string> CreateConversationAsync(CancellationToken ct) // Create a new conversation.
    { // Open the method block.
        var createJson = JsonSerializer.Serialize(new // Build payload.
        { // Open initializer block.
            title = "Quick Chat", // Set title.
            type = "CHAT_WITH_AI", // Set type.
            model = _model // Set model.
        }); // Close initializer block.
        var response = await _http.PostAsync( // Send request.
            "/api/conversations", // Provide endpoint.
            new StringContent(createJson, Encoding.UTF8, "application/json"), // Provide content.
            ct); // Pass cancellation.
        await EnsureSuccessAsync(response, ct, "1minAI conversation"); // Ensure success.
        var content = await response.Content.ReadAsStringAsync(ct); // Read content.
        using var doc = JsonDocument.Parse(content); // Parse JSON.
        if (!doc.RootElement.TryGetProperty("conversation", out var conversation) || // Validate shape.
            !conversation.TryGetProperty("uuid", out var uuidElement)) // Validate UUID.
            throw new HttpRequestException($"1minAI response missing conversation uuid. {content}"); // Throw error.
        var uuid = uuidElement.GetString(); // Read UUID.
        if (string.IsNullOrWhiteSpace(uuid)) // Validate UUID.
            throw new HttpRequestException($"1minAI response missing conversation uuid. {content}"); // Throw error.
        return uuid; // Return UUID.
    } // Close the method block.

    private async Task<string> SendQuestionAsync(string conversationId, string question, CancellationToken ct) // Send question.
    { // Open the method block.
        var requestJson = JsonSerializer.Serialize(new // Build payload.
        { // Open initializer block.
            type = "CHAT_WITH_AI", // Set type.
            model = _model, // Set model.
            conversationId, // Set conversation ID.
            promptObject = new // Set prompt object.
            { // Open initializer block.
                prompt = question, // Set prompt.
                temperature = 0.7, // Set temperature.
                max_tokens = 500, // Set max tokens.
                top_p = 0.9 // Set top_p.
            } // Close initializer block.
        }); // Close initializer block.
        var response = await _http.PostAsync( // Send request.
            "/api/features", // Provide endpoint.
            new StringContent(requestJson, Encoding.UTF8, "application/json"), // Provide content.
            ct); // Pass cancellation.
        await EnsureSuccessAsync(response, ct, "1minAI completion"); // Ensure success.
        var content = await response.Content.ReadAsStringAsync(ct); // Read content.
        using var doc = JsonDocument.Parse(content); // Parse JSON.
        if (!doc.RootElement.TryGetProperty("aiRecord", out var aiRecord) || // Validate shape.
            !aiRecord.TryGetProperty("aiRecordDetail", out var detail) || // Validate detail.
            !detail.TryGetProperty("resultObject", out var resultObject) || // Validate result.
            resultObject.ValueKind != JsonValueKind.Array || // Ensure array.
            resultObject.GetArrayLength() == 0) // Ensure non-empty.
            throw new HttpRequestException($"1minAI response missing result. {content}"); // Throw error.
        var result = resultObject[0].GetString(); // Read result.
        return string.IsNullOrWhiteSpace(result) ? "No response" : result; // Return result.
    } // Close the method block.

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct, string context) // Ensure success.
    { // Open the method block.
        if (response.IsSuccessStatusCode) // Check success.
            return; // Exit early.
        var body = await response.Content.ReadAsStringAsync(ct); // Read error body.
        throw new HttpRequestException($"{context} failed ({(int)response.StatusCode}). {body}"); // Throw error.
    } // Close the method block.
} // Close the class block.
