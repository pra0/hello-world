using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Calculator.Services;
using Calculator.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Calculator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ICalculatorService _calculatorService;
    private readonly ICalculationHistoryService _historyService;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(
        ILogger<IndexModel> logger,
        ICalculatorService calculatorService,
        ICalculationHistoryService historyService,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _calculatorService = calculatorService;
        _historyService = historyService;
        _userManager = userManager;
    }

    [BindProperty]
    [Required]
    public string Expression { get; set; } = string.Empty;

    public double? Result { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public List<CalculationHistory> History { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                History = await _historyService.GetUserHistoryAsync(user.Id);
            }
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadHistoryAsync();
            return Page();
        }

        var calculationResult = _calculatorService.Evaluate(Expression);

        if (calculationResult.IsError)
        {
            ErrorMessage = calculationResult.ErrorMessage;
        }
        else
        {
            Result = calculationResult.Result;

            // Save to history if user is logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    await _historyService.SaveCalculationAsync(user.Id, Expression, calculationResult.Result);
                }
            }
        }

        await LoadHistoryAsync();
        return Page();
    }

    private async Task LoadHistoryAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                History = await _historyService.GetUserHistoryAsync(user.Id);
            }
        }
    }
}
