namespace BudgetTracker.Web.Models; // Define view model namespace.

public class ErrorViewModel // Define error view model.
{ // Open the class block.
    public string? RequestId { get; set; } // Store request id.

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Indicate when to show id.
} // Close the class block.
