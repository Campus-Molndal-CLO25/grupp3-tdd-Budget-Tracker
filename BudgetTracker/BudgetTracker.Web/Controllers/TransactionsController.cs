using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using BudgetTracker.Web.Models; // Import view models.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

public class TransactionsController : Controller // Define transactions controller.
{ // Open the class block.
    private readonly ITransactionService _transactions; // Hold transaction service.
    private readonly IAccountService _accounts; // Hold account service.
    private readonly ICategoryService _categories; // Hold category service.

    public TransactionsController(ITransactionService transactions, IAccountService accounts, ICategoryService categories) // Define constructor.
    { // Open the constructor block.
        _transactions = transactions; // Assign transaction service.
        _accounts = accounts; // Assign account service.
        _categories = categories; // Assign category service.
    } // Close the constructor block.

    public async Task<IActionResult> Index() // Render transactions list.
    { // Open the method block.
        var model = await BuildIndexModelAsync(new CreateTransactionDto { Date = DateTime.UtcNow }); // Build view model.
        return View(model); // Return view.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Create(CreateTransactionDto dto) // Create transaction.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            await _transactions.CreateAsync(dto); // Create transaction.
            return RedirectToAction(nameof(Index)); // Redirect to list.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            var model = await BuildIndexModelAsync(dto); // Rebuild model.
            model.ErrorMessage = ex.Message; // Store error.
            return View("Index", model); // Return view.
        } // Close catch block.
    } // Close the method block.

    public async Task<IActionResult> Edit(int id) // Render edit form.
    { // Open the method block.
        var transaction = await _transactions.GetByIdAsync(id); // Fetch transaction.
        if (transaction is null) // Check for missing transaction.
            return NotFound(); // Return 404.

        var accounts = await _accounts.GetAllAsync(); // Fetch accounts.
        var categories = await _categories.GetAllAsync(); // Fetch categories.

        return View(new TransactionEditViewModel // Build view model.
        { // Open initializer block.
            Id = id, // Map id.
            Accounts = accounts, // Map accounts.
            Categories = categories, // Map categories.
            Transaction = new UpdateTransactionDto // Map update payload.
            { // Open initializer block.
                AccountId = transaction.AccountId, // Map account id.
                CategoryId = transaction.CategoryId, // Map category id.
                Amount = transaction.Amount, // Map amount.
                Type = transaction.Type, // Map type.
                Date = transaction.Date, // Map date.
                Description = transaction.Description // Map description.
            } // Close initializer block.
        }); // Close initializer block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Edit(int id, UpdateTransactionDto dto) // Update transaction.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            var updated = await _transactions.UpdateAsync(id, dto); // Update transaction.
            if (updated is null) // Check for missing transaction.
                return NotFound(); // Return 404.
            return RedirectToAction(nameof(Index)); // Redirect to list.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            var accounts = await _accounts.GetAllAsync(); // Fetch accounts.
            var categories = await _categories.GetAllAsync(); // Fetch categories.
            return View(new TransactionEditViewModel // Return view with error.
            { // Open initializer block.
                Id = id, // Map id.
                Accounts = accounts, // Map accounts.
                Categories = categories, // Map categories.
                Transaction = dto, // Map payload.
                ErrorMessage = ex.Message // Map error.
            }); // Close initializer block.
        } // Close catch block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> Delete(int id) // Delete transaction.
    { // Open the method block.
        await _transactions.DeleteAsync(id); // Delete transaction.
        return RedirectToAction(nameof(Index)); // Redirect to list.
    } // Close the method block.

    private async Task<TransactionsIndexViewModel> BuildIndexModelAsync(CreateTransactionDto dto) // Build index view model.
    { // Open the method block.
        var accounts = await _accounts.GetAllAsync(); // Fetch accounts.
        var categories = await _categories.GetAllAsync(); // Fetch categories.
        var transactions = await _transactions.GetAllAsync(new TransactionFilterDto { Skip = 0, Take = 100 }); // Fetch transactions.

        return new TransactionsIndexViewModel // Build view model.
        { // Open initializer block.
            Accounts = accounts, // Map accounts.
            Categories = categories, // Map categories.
            Transactions = transactions, // Map transactions.
            NewTransaction = dto // Map new transaction.
        }; // Close initializer block.
    } // Close the method block.
} // Close the class block.
