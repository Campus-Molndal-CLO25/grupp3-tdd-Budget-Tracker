namespace BudgetTracker.Core.Domain; // Define the shared domain namespace.

public enum AccountType // Declare the supported account types.
{ // Open the enum block.
    Checking, // Represent a standard checking account.
    Savings, // Represent a savings account.
    Cash // Represent a cash account.
} // Close the enum block.

public enum TransactionType // Declare the supported transaction types.
{ // Open the enum block.
    Income, // Represent income transactions.
    Expense // Represent expense transactions.
} // Close the enum block.

public enum CategoryType // Declare the supported category types.
{ // Open the enum block.
    Income, // Represent income categories.
    Expense // Represent expense categories.
} // Close the enum block.

public enum BudgetStatus // Declare the budget status values.
{ // Open the enum block.
    Under, // Actual spend is below budget.
    OnTrack, // Actual spend matches budget.
    Over // Actual spend exceeds budget.
} // Close the enum block.
