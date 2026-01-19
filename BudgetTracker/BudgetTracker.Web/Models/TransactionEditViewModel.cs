using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class TransactionEditViewModel // Define transaction edit view model.
{ // Open the class block.
    public int Id { get; set; } // Store transaction id.
    public UpdateTransactionDto Transaction { get; set; } = new(); // Store update payload.
    public List<AccountDto> Accounts { get; set; } = new(); // Store accounts.
    public List<CategoryDto> Categories { get; set; } = new(); // Store categories.
    public string? ErrorMessage { get; set; } // Store error message.
} // Close the class block.
