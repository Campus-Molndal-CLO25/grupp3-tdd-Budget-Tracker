using BudgetTracker.Core.Domain; // Import domain models.
using Microsoft.EntityFrameworkCore; // Import EF Core APIs.

namespace BudgetTracker.Core.Data; // Define data namespace.

public class BudgetTrackerDbContext : DbContext // Define the EF Core DbContext.
{ // Open the class block.
    public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options) { } // Forward options to base.

    public DbSet<Account> Accounts => Set<Account>(); // Expose accounts table.
    public DbSet<Transaction> Transactions => Set<Transaction>(); // Expose transactions table.
    public DbSet<Category> Categories => Set<Category>(); // Expose categories table.
    public DbSet<Budget> Budgets => Set<Budget>(); // Expose budgets table.

    protected override void OnModelCreating(ModelBuilder modelBuilder) // Configure model constraints.
    { // Open the method block.
        base.OnModelCreating(modelBuilder); // Call the base configuration.

        modelBuilder.Entity<Account>().HasIndex(a => a.Name).IsUnique(); // Enforce unique account names.
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique(); // Enforce unique category names.
        modelBuilder.Entity<Budget>().HasIndex(b => new { b.CategoryId, b.Month }).IsUnique(); // Enforce unique budget per month.
    } // Close the method block.
} // Close the class block.
