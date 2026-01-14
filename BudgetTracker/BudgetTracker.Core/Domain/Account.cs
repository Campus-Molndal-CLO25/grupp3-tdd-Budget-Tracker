using System.ComponentModel.DataAnnotations; // Import data annotation attributes.

namespace BudgetTracker.Core.Domain; // Define the shared domain namespace.

public class Account // Represent a financial account.
{ // Open the class block.
    public int Id { get; set; } // Store the primary key.

    [Required] // Require a name value.
    [MaxLength(100)] // Limit name length.
    public string Name { get; set; } = ""; // Store the account name.

    public AccountType AccountType { get; set; } // Store the account type.

    [Range(0, double.MaxValue)] // Ensure the balance is non-negative.
    public decimal InitialBalance { get; set; } // Store the starting balance.

    public decimal CurrentBalance { get; set; } // Store the running balance.

    public List<Transaction> Transactions { get; set; } = new(); // Track account transactions.
} // Close the class block.
