using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class UpdateAccountDto // Define account update payload.
{ // Open the class block.
    public string Name { get; set; } = ""; // Store the updated name.
    public AccountType AccountType { get; set; } // Store the updated account type.
} // Close the class block.
