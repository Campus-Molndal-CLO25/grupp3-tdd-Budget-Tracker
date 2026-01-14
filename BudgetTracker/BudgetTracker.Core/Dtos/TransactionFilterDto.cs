using BudgetTracker.Core.Domain; // Import domain enums.

namespace BudgetTracker.Core.Dtos; // Define DTO namespace.

public class TransactionFilterDto // Define transaction filter options.
{ // Open the class block.
    public DateTime? StartDate { get; set; } // Store the optional start date.
    public DateTime? EndDate { get; set; } // Store the optional end date.
    public int? CategoryId { get; set; } // Store the optional category id.
    public TransactionType? Type { get; set; } // Store the optional transaction type.
    public int Skip { get; set; } // Store pagination skip.
    public int Take { get; set; } = 50; // Store pagination take.
} // Close the class block.
