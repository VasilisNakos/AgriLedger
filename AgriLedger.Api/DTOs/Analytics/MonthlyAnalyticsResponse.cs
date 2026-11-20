namespace AgriLedger.Api.DTOs.Analytics;

public class MonthlyAnalyticsResponse
{
    public int Year { get; set; }

    public int Month { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal PaidRevenue { get; set; }

    public decimal PendingRevenue { get; set; }

    public decimal TotalExpenses { get; set; }

    public IReadOnlyList<CategoryExpenseShare> ExpensesByCategory { get; set; } = [];

    public decimal NetProfit { get; set; }

    public decimal? ProfitMarginPercent { get; set; }

    public int TotalCasesSold { get; set; }

    public int TotalCasesProduced { get; set; }

    public decimal? CostPerCase { get; set; }
}
