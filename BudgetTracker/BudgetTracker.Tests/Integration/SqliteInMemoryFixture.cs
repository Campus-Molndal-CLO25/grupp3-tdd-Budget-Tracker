using System.Data.Common;
using BudgetTracker.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace BudgetTracker.Tests.Integration;

public sealed class SqliteInMemoryFixture : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    public WebApplicationFactory<global::Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        Factory = new ApiWebApplicationFactory(_connection);

        // ✅ Skapa schema exakt en gång per fixture
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BudgetTrackerDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        Factory.Dispose();
        _connection.Dispose();
        return Task.CompletedTask;
    }

    private sealed class ApiWebApplicationFactory : WebApplicationFactory<global::Program>
    {
        private readonly DbConnection _connection;

        public ApiWebApplicationFactory(DbConnection connection) => _connection = connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); // ✅ viktigt

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<BudgetTrackerDbContext>>();
                services.AddDbContext<BudgetTrackerDbContext>(options =>
                    options.UseSqlite(_connection));

                // ❌ Ingen EnsureCreated/Migrate här
            });
        }
    }
}
