using BudgetTracker.Core.Clients; // Import Gemini client.
using BudgetTracker.Core.Data; // Import DbContext and seed helper.
using BudgetTracker.Core.Repositories.Ef; // Import EF repositories.
using BudgetTracker.Core.Repositories.Interfaces; // Import repository contracts.
using BudgetTracker.Core.Services; // Import service implementations.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

var builder = WebApplication.CreateBuilder(args); // Create the application builder.

builder.Services.AddControllersWithViews(); // Register MVC with views.
builder.Services.AddMemoryCache(); // Register in-memory cache.

builder.Services.AddDbContext<BudgetTrackerDbContext>(options => // Register DbContext.
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"))); // Configure SQLite connection.

builder.Services.AddScoped<IAccountRepository, AccountRepository>(); // Register account repository.
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>(); // Register transaction repository.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Register category repository.
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>(); // Register budget repository.

builder.Services.AddScoped<IAccountService, AccountService>(); // Register account service.
builder.Services.AddScoped<ITransactionService, TransactionService>(); // Register transaction service.
builder.Services.AddScoped<ICategoryService, CategoryService>(); // Register category service.
builder.Services.AddScoped<IBudgetService, BudgetService>(); // Register budget service.
builder.Services.AddScoped<IInsightsService, InsightsService>(); // Register insights service.
builder.Services.AddScoped<IReportService, ReportService>(); // Register report service.
builder.Services.AddScoped<IDashboardService, DashboardService>(); // Register dashboard service.

builder.Services.AddHttpClient<IGeminiClient, GeminiClient>(http => // Register Gemini HttpClient.
{ // Open the client configuration block.
    var baseUrl = builder.Configuration["Gemini:BaseUrl"]; // Read base URL from config.
    var envBaseUrl = Environment.GetEnvironmentVariable("GEMINI_BASE_URL"); // Read base URL from env.
    http.BaseAddress = new Uri(string.IsNullOrWhiteSpace(envBaseUrl) ? baseUrl! : envBaseUrl); // Set base address.

    var apiKey = builder.Configuration["Gemini:ApiKey"]; // Read API key from config.
    if (!string.IsNullOrWhiteSpace(apiKey)) // Check if API key exists.
        http.DefaultRequestHeaders.Add("X-Api-Key", apiKey); // Add API key header.
}); // Close the client configuration block.

var app = builder.Build(); // Build the application.

if (!app.Environment.IsDevelopment()) // Check for non-development.
{ // Open the if block.
    app.UseExceptionHandler("/Home/Error"); // Configure error handler.
    app.UseHsts(); // Enable HSTS.
    app.UseHttpsRedirection(); // Enable HTTPS redirection.
} // Close the if block.

if (app.Environment.IsDevelopment()) // Check for development.
{ // Open the if block.
    using var scope = app.Services.CreateScope(); // Create a DI scope.
    var db = scope.ServiceProvider.GetRequiredService<BudgetTrackerDbContext>(); // Resolve DbContext.
    await db.Database.MigrateAsync(); // Apply migrations.
    await SeedData.EnsureSeededAsync(db); // Seed default data.
} // Close the if block.

app.UseStaticFiles(); // Enable static files.
app.UseRouting(); // Enable routing.

app.MapControllerRoute( // Map default MVC route.
    name: "default", // Provide route name.
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Provide route pattern.

app.Run(); // Run the application.

public partial class Program { } // Expose Program for tests.
