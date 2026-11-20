using AgriLedger.Api.DTOs.Analytics;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlyAnalyticsResponse>> GetMonthly(
        [FromQuery] int year,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var report = await _analytics.GetMonthlyAsync(year, month, cancellationToken);
        return Ok(report);
    }
}
