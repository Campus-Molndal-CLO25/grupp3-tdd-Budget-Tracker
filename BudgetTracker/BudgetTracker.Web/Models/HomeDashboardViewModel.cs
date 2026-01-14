using BudgetTracker.Core.Dtos; // Import DTOs.

namespace BudgetTracker.Web.Models; // Define view model namespace.

public class HomeDashboardViewModel // Define dashboard view model.
{ // Open the class block.
    public List<AccountDto> Accounts { get; set; } = new(); // Store accounts.
    public CreateAccountDto NewAccount { get; set; } = new(); // Store form data.
    public string? ErrorMessage { get; set; } // Store account error.
    public string AdvicePrompt { get; set; } = ""; // Store advice prompt.
    public string? AdviceResponse { get; set; } // Store advice response.
    public string? AdviceError { get; set; } // Store advice error.
} // Close the class block.
