using AgriLedger.Api.DTOs.Analytics;

namespace AgriLedger.Api.Services;

public sealed record CategoryExpenseInput(
    int CategoryId,
    string Code,
    string Name,
    int SortOrder,
    decimal Amount);

public static class MonthlyAnalyticsCalculator
{
    public static MonthlyAnalyticsResponse Calculate(
        int year,
        int month,
        decimal paidRevenue,
        decimal pendingRevenue,
        int totalCasesSold,
        int totalCasesProduced,
        IReadOnlyList<CategoryExpenseInput> categories)
    {
        var totalRevenue = paidRevenue + pendingRevenue;
        var ordered = categories
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToList();
        var totalExpenses = ordered.Sum(category => category.Amount);
        var shares = ordered
            .Select(category => new CategoryExpenseShare
            {
                CategoryId = category.CategoryId,
                Code = category.Code,
                Name = category.Name,
                Amount = category.Amount,
                PercentOfExpenses = totalExpenses == 0
                    ? 0
                    : decimal.Round(category.Amount / totalExpenses * 100m, 2, MidpointRounding.AwayFromZero)
            })
            .ToList();
        var netProfit = totalRevenue - totalExpenses;

        return new MonthlyAnalyticsResponse
        {
            Year = year,
            Month = month,
            TotalRevenue = totalRevenue,
            PaidRevenue = paidRevenue,
            PendingRevenue = pendingRevenue,
            TotalExpenses = totalExpenses,
            ExpensesByCategory = shares,
            NetProfit = netProfit,
            ProfitMarginPercent = totalRevenue == 0
                ? null
                : decimal.Round(netProfit / totalRevenue * 100m, 2, MidpointRounding.AwayFromZero),
            TotalCasesSold = totalCasesSold,
            TotalCasesProduced = totalCasesProduced,
            CostPerCase = totalCasesProduced == 0
                ? null
                : decimal.Round(totalExpenses / totalCasesProduced, 4, MidpointRounding.AwayFromZero)
        };
    }
}
