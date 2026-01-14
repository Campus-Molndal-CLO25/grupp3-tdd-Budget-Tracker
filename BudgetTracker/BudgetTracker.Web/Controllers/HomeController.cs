using System.Diagnostics; // Import diagnostics.
using BudgetTracker.Core.Dtos; // Import DTOs.
using BudgetTracker.Core.Services.Interfaces; // Import service contracts.
using BudgetTracker.Web.Models; // Import view models.
using Microsoft.AspNetCore.Mvc; // Import MVC APIs.

namespace BudgetTracker.Web.Controllers; // Define MVC namespace.

public class HomeController : Controller // Define home controller.
{ // Open the class block.
    private readonly IAccountService _accounts; // Hold account service.
    private readonly IInsightsService _insights; // Hold insights service.

    public HomeController(IAccountService accounts, IInsightsService insights) // Define constructor.
    { // Open the constructor block.
        _accounts = accounts; // Assign account service.
        _insights = insights; // Assign insights service.
    } // Close the constructor block.

    public async Task<IActionResult> Index() // Render dashboard.
    { // Open the method block.
        var accounts = await _accounts.GetAllAsync(); // Fetch accounts.
        return View(new HomeDashboardViewModel { Accounts = accounts }); // Return view model.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> CreateAccount(CreateAccountDto dto) // Create account.
    { // Open the method block.
        try // Start exception handling.
        { // Open try block.
            await _accounts.CreateAsync(dto); // Create account.
            return RedirectToAction(nameof(Index)); // Redirect to dashboard.
        } // Close try block.
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) // Handle validation conflicts.
        { // Open catch block.
            var accounts = await _accounts.GetAllAsync(); // Reload accounts.
            return View("Index", new HomeDashboardViewModel // Return view with errors.
            { // Open initializer block.
                Accounts = accounts, // Map accounts.
                NewAccount = dto, // Map form values.
                ErrorMessage = ex.Message // Map error message.
            }); // Close initializer block.
        } // Close catch block.
    } // Close the method block.

    [HttpPost] // Handle POST requests.
    public async Task<IActionResult> AskGemini(string prompt) // Ask for advice.
    { // Open the method block.
        var accounts = await _accounts.GetAllAsync(); // Fetch accounts.
        try // Start exception handling.
        { // Open try block.
            var advice = await _insights.GetAdviceAsync(prompt); // Get advice.
            return View("Index", new HomeDashboardViewModel // Return view with advice.
            { // Open initializer block.
                Accounts = accounts, // Map accounts.
                AdvicePrompt = prompt, // Map prompt.
                AdviceResponse = advice // Map advice.
            }); // Close initializer block.
        } // Close try block.
        catch (ArgumentException ex) // Handle validation errors.
        { // Open catch block.
            return View("Index", new HomeDashboardViewModel // Return view with error.
            { // Open initializer block.
                Accounts = accounts, // Map accounts.
                AdvicePrompt = prompt, // Map prompt.
                AdviceError = ex.Message // Map error.
            }); // Close initializer block.
        } // Close catch block.
        catch (HttpRequestException) // Handle network errors.
        { // Open catch block.
            return View("Index", new HomeDashboardViewModel // Return view with error.
            { // Open initializer block.
                Accounts = accounts, // Map accounts.
                AdvicePrompt = prompt, // Map prompt.
                AdviceError = "Gemini is unreachable. Set GEMINI_BASE_URL or update Gemini:BaseUrl." // Map error.
            }); // Close initializer block.
        } // Close catch block.
    } // Close the method block.

    public IActionResult Privacy() // Render privacy page.
    { // Open the method block.
        return View(); // Return view.
    } // Close the method block.

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Disable caching.
    public IActionResult Error() // Render error page.
    { // Open the method block.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Return error view.
    } // Close the method block.
} // Close the class block.
