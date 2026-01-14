using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

public class AccountsController : Controller // Define accounts controller.
{ // Open the class block.
    private readonly IAccountService _service; // Hold account service.

    public AccountsController(IAccountService service) // Define constructor.
    { // Open the constructor block.
        _service = service; // Assign account service.
    } // Close the constructor block.

    public async Task<IActionResult> Index() // Render accounts list.
    { // Open the method block.
        var accounts = await _service.GetAllAsync(); // Fetch accounts.
        return View(accounts); // Return view with model.
    } // Close the method block.
} // Close the class block.
