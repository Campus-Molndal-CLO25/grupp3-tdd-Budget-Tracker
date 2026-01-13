using BudgetTracker.Core.Clients;
using BudgetTracker.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;


namespace BudgetTracker.Tests.Unit;

public class InsightsServiceTests
{
    [Fact]
    public async Task GetAdviceAsync_RejectsBlankPrompt()
    {
        var gemini = Substitute.For<IGeminiClient>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new InsightsService(gemini, cache);

        var act = () => service.GetAdviceAsync("   ");

        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        ex.Message.Should().Be("Prompt is required");
    }

    [Fact]
    public async Task GetAdviceAsync_CachesResponses()
    {
        var gemini = Substitute.For<IGeminiClient>();
        gemini.GenerateInsightAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("Advice");
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new InsightsService(gemini, cache);

        var first = await service.GetAdviceAsync("Save more");
        var second = await service.GetAdviceAsync("Save more");

        first.Should().Be("Advice");
        second.Should().Be("Advice");
        await gemini.Received(1).GenerateInsightAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAdviceAsync_SendsShapedPrompt()
    {
        var gemini = Substitute.For<IGeminiClient>();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new InsightsService(gemini, cache);

        await service.GetAdviceAsync("Cut dining out");

        await gemini.Received(1).GenerateInsightAsync(
            Arg.Is<string>(p => p.StartsWith("Give concise, actionable saving advice:")),
            Arg.Any<CancellationToken>());
    }
}
