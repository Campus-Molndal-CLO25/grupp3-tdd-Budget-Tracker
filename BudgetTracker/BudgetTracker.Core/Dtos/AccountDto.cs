using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class AccountDto // Define account response data.
{ // Open the class block.
    public int Id { get; set; } // Store the account id.
    public string Name { get; set; } = ""; // Store the account name.
    public AccountType AccountType { get; set; } // Store the account type.
    public decimal CurrentBalance { get; set; } // Store the current balance.
    public decimal InitialBalance { get; set; } // Store the initial balance.
} // Close the class block.
