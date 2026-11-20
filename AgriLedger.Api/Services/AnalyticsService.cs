using AgriLedger.Api.Data;
using AgriLedger.Api.DTOs.Analytics;
using AgriLedger.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Services;

public interface IAnalyticsService
{
    Task<MonthlyAnalyticsResponse> GetMonthlyAsync(int year, int month, CancellationToken cancellationToken);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly AgriLedgerDbContext _db;

    public AnalyticsService(AgriLedgerDbContext db)
    {
        _db = db;
    }

    public async Task<MonthlyAnalyticsResponse> GetMonthlyAsync(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var (start, end) = MonthRange.Resolve(year, month);
        var from = start!.Value;
        var to = end!.Value;

        var sales = _db.Sales.AsNoTracking().Where(sale => sale.Date >= from && sale.Date < to);
        var paidRevenue = await sales
            .Where(sale => sale.PaymentStatus == PaymentStatus.Paid)
            .SumAsync(sale => (decimal?)sale.TotalAmount, cancellationToken) ?? 0m;
        var pendingRevenue = await sales
            .Where(sale => sale.PaymentStatus == PaymentStatus.Pending)
            .SumAsync(sale => (decimal?)sale.TotalAmount, cancellationToken) ?? 0m;
        var totalCasesSold = await sales.SumAsync(sale => (int?)sale.CaseQuantity, cancellationToken) ?? 0;
        var totalCasesProduced = await _db.Productions
            .AsNoTracking()
            .Where(production => production.Date >= from && production.Date < to)
            .SumAsync(production => (int?)production.CaseQuantity, cancellationToken) ?? 0;

        var expenseAmounts = await _db.Expenses
            .AsNoTracking()
            .Where(expense => expense.Date >= from && expense.Date < to)
            .GroupBy(expense => expense.CategoryId)
            .Select(group => new { CategoryId = group.Key, Amount = group.Sum(expense => expense.Amount) })
            .ToListAsync(cancellationToken);
        var amountByCategory = expenseAmounts.ToDictionary(row => row.CategoryId, row => row.Amount);

        var categories = await _db.ExpenseCategories
            .AsNoTracking()
            .OrderBy(category => category.SortOrder)
            .ToListAsync(cancellationToken);
        var inputs = categories
            .Where(category => category.IsActive || amountByCategory.GetValueOrDefault(category.Id) > 0)
            .Select(category => new CategoryExpenseInput(
                category.Id,
                category.Code,
                category.Name,
                category.SortOrder,
                amountByCategory.GetValueOrDefault(category.Id)))
            .ToList();

        return MonthlyAnalyticsCalculator.Calculate(
            year,
            month,
            paidRevenue,
            pendingRevenue,
            totalCasesSold,
            totalCasesProduced,
            inputs);
    }
}
