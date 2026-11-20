using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriLedger.Api.Pages;

public abstract class MonthlyPageModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int Year { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Month { get; set; }

    public string? FormError { get; set; }

    protected void NormalizePeriod()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (Month is < 1 or > 12)
        {
            Month = today.Month;
        }

        if (Year is < 2000 or > 2100)
        {
            Year = today.Year;
        }
    }

    protected DateOnly DefaultEntryDate()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (today.Year == Year && today.Month == Month)
        {
            return today;
        }

        return new DateOnly(Year, Month, 1);
    }

    protected async Task<IActionResult> SaveAsync(
        Func<Task> action,
        Func<CancellationToken, Task> reload,
        CancellationToken cancellationToken)
    {
        NormalizePeriod();
        if (!ModelState.IsValid)
        {
            await reload(cancellationToken);
            return Page();
        }

        try
        {
            await action();
        }
        catch (LedgerException exception)
        {
            FormError = exception.Message;
            await reload(cancellationToken);
            return Page();
        }

        return RedirectToPage(new { year = Year, month = Month });
    }
}
