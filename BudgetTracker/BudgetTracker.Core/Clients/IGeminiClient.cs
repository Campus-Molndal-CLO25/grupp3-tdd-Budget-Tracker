namespace BudgetTracker.Core.Clients;

// Interface gör det testbart (mock i NSubstitute)
public interface IGeminiClient
{
    Task<string> GenerateInsightAsync(string prompt, CancellationToken ct = default);
}
