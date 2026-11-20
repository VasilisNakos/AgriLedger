using AgriLedger.Api.DTOs.Analytics;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Pages;

public class IndexModel : MonthlyPageModel
{
    private readonly IAnalyticsService _analytics;

    public IndexModel(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    public MonthlyAnalyticsResponse Report { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        NormalizePeriod();
        Report = await _analytics.GetMonthlyAsync(Year, Month, cancellationToken);
        return Page();
    }
}
