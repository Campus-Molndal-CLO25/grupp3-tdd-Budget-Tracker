using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class CreateAccountDto // Define account creation payload.
{ // Open the class block.
    public string Name { get; set; } = ""; // Store the account name.
    public AccountType AccountType { get; set; } // Store the account type.
    public decimal InitialBalance { get; set; } // Store the initial balance.
} // Close the class block.
