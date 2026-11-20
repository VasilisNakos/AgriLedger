using AgriLedger.Api.Data;
using Microsoft.AspNetCore.Http;
using AgriLedger.Api.DTOs.Categories;
using AgriLedger.Api.DTOs.Expenses;
using AgriLedger.Api.DTOs.Production;
using AgriLedger.Api.DTOs.Sales;
using AgriLedger.Api.Enums;
using AgriLedger.Api.Models;
using AgriLedger.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Tests;

public class MonthlyAnalyticsCalculatorTests
{
    [Fact]
    public void CostUsesProducedCasesAndLeavesSoldCasesSeparate()
    {
        var report = MonthlyAnalyticsCalculator.Calculate(
            2026,
            11,
            paidRevenue: 45m,
            pendingRevenue: 10m,
            totalCasesSold: 15,
            totalCasesProduced: 20,
            categories:
            [
                new CategoryExpenseInput(1, "FEED", "Ζωοτροφές", 1, 100m),
                new CategoryExpenseInput(2, "ELECTRICITY", "Ρεύμα", 2, 50m)
            ]);

        Assert.Equal(55m, report.TotalRevenue);
        Assert.Equal(150m, report.TotalExpenses);
        Assert.Equal(-95m, report.NetProfit);
        Assert.Equal(-172.73m, report.ProfitMarginPercent);
        Assert.Equal(66.67m, report.ExpensesByCategory[0].PercentOfExpenses);
        Assert.Equal(33.33m, report.ExpensesByCategory[1].PercentOfExpenses);
        Assert.Equal(15, report.TotalCasesSold);
        Assert.Equal(20, report.TotalCasesProduced);
        Assert.Equal(7.5m, report.CostPerCase);
    }

    [Fact]
    public void EmptyProductionAndRevenueStayNullInsteadOfDividingByZero()
    {
        var report = MonthlyAnalyticsCalculator.Calculate(
            2026,
            11,
            paidRevenue: 0,
            pendingRevenue: 0,
            totalCasesSold: 8,
            totalCasesProduced: 0,
            categories: [new CategoryExpenseInput(1, "FEED", "Ζωοτροφές", 1, 30m)]);

        Assert.Null(report.ProfitMarginPercent);
        Assert.Null(report.CostPerCase);
        Assert.Equal(-30m, report.NetProfit);
        Assert.Equal(8, report.TotalCasesSold);
    }

    [Fact]
    public void ZeroExpensesKeepCategoryRowsAtZeroPercent()
    {
        var report = MonthlyAnalyticsCalculator.Calculate(
            2026,
            11,
            paidRevenue: 20m,
            pendingRevenue: 0,
            totalCasesSold: 4,
            totalCasesProduced: 3,
            categories: [new CategoryExpenseInput(1, "FEED", "Ζωοτροφές", 1, 0m)]);

        Assert.Equal(0m, report.ExpensesByCategory[0].PercentOfExpenses);
        Assert.Equal(100m, report.ProfitMarginPercent);
        Assert.Equal(0m, report.CostPerCase);
    }
}

public class LedgerServiceTests
{
    [Fact]
    public async Task MonthlyReportUsesProducedCasesAndIgnoresOtherMonths()
    {
        using var database = new LedgerDatabase();
        var expenses = new ExpenseService(database.Db);
        var sales = new SaleService(database.Db);
        var production = new ProductionService(database.Db);
        var categories = new CategoryService(database.Db);
        var analytics = new AnalyticsService(database.Db);

        var packaging = await categories.CreateAsync(new CategoryRequest
        {
            Code = "packaging",
            Name = "Συσκευασία",
            IsActive = true,
            SortOrder = 20
        }, CancellationToken.None);
        await categories.CreateAsync(new CategoryRequest
        {
            Code = "VET",
            Name = "Κτηνιατρικά",
            IsActive = false,
            SortOrder = 21
        }, CancellationToken.None);

        await expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 100m,
            Date = new DateOnly(2026, 11, 4),
            CategoryId = 1
        }, CancellationToken.None);
        await expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 50m,
            Date = new DateOnly(2026, 11, 8),
            CategoryId = packaging.Id
        }, CancellationToken.None);
        await categories.UpdateAsync(packaging.Id, new CategoryRequest
        {
            Code = packaging.Code,
            Name = packaging.Name,
            IsActive = false,
            SortOrder = packaging.SortOrder
        }, CancellationToken.None);
        await expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 999m,
            Date = new DateOnly(2026, 12, 1),
            CategoryId = 1
        }, CancellationToken.None);

        var sale = await sales.CreateAsync(new SaleRequest
        {
            Date = new DateOnly(2026, 11, 10),
            CustomerName = "Χονδρέμπορος",
            SaleType = SaleType.Wholesale,
            EggSize = EggSize.Large,
            CaseQuantity = 10,
            PricePerCase = 4.50m,
            PaymentStatus = PaymentStatus.Paid
        }, CancellationToken.None);
        await sales.CreateAsync(new SaleRequest
        {
            Date = new DateOnly(2026, 11, 12),
            CustomerName = "Κατάστημα",
            SaleType = SaleType.Retail,
            EggSize = EggSize.Medium,
            CaseQuantity = 5,
            PricePerCase = 2m,
            PaymentStatus = PaymentStatus.Pending
        }, CancellationToken.None);

        await production.CreateAsync(new ProductionRequest
        {
            Date = new DateOnly(2026, 11, 3),
            CaseQuantity = 20
        }, CancellationToken.None);
        await production.CreateAsync(new ProductionRequest
        {
            Date = new DateOnly(2026, 12, 3),
            CaseQuantity = 100
        }, CancellationToken.None);

        Assert.Equal(45m, sale.TotalAmount);

        var november = await analytics.GetMonthlyAsync(2026, 11, CancellationToken.None);
        Assert.Equal(55m, november.TotalRevenue);
        Assert.Equal(45m, november.PaidRevenue);
        Assert.Equal(10m, november.PendingRevenue);
        Assert.Equal(150m, november.TotalExpenses);
        Assert.Equal(15, november.TotalCasesSold);
        Assert.Equal(20, november.TotalCasesProduced);
        Assert.Equal(7.5m, november.CostPerCase);
        Assert.Contains(november.ExpensesByCategory, share => share.Code == "PACKAGING" && share.Amount == 50m);
        Assert.DoesNotContain(november.ExpensesByCategory, share => share.Code == "VET");
        Assert.Contains(november.ExpensesByCategory, share => share.Code == "FEED" && share.Amount == 100m);

        var december = await analytics.GetMonthlyAsync(2026, 12, CancellationToken.None);
        Assert.Equal(0m, december.TotalRevenue);
        Assert.Null(december.ProfitMarginPercent);
        Assert.Equal(100, december.TotalCasesProduced);
        Assert.Equal(0, december.TotalCasesSold);
        Assert.Equal(9.99m, december.CostPerCase);
        Assert.DoesNotContain(december.ExpensesByCategory, share => share.Code == "PACKAGING");
    }

    [Fact]
    public async Task RejectsZeroExpenseUnknownCategoryAndDeletesUnusedCategory()
    {
        using var database = new LedgerDatabase();
        var expenses = new ExpenseService(database.Db);
        var categories = new CategoryService(database.Db);

        var error = await Assert.ThrowsAsync<LedgerException>(() => expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 0,
            Date = new DateOnly(2026, 11, 1),
            CategoryId = 1
        }, CancellationToken.None));
        Assert.Equal(StatusCodes.Status400BadRequest, error.StatusCode);

        var missing = await Assert.ThrowsAsync<LedgerException>(() => expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 10,
            Date = new DateOnly(2026, 11, 1),
            CategoryId = 999
        }, CancellationToken.None));
        Assert.Equal(StatusCodes.Status400BadRequest, missing.StatusCode);

        var created = await categories.CreateAsync(new CategoryRequest
        {
            Code = "water",
            Name = "Νερό",
            IsActive = true,
            SortOrder = 8
        }, CancellationToken.None);
        Assert.Equal("WATER", created.Code);

        var duplicate = await Assert.ThrowsAsync<LedgerException>(() => categories.CreateAsync(new CategoryRequest
        {
            Code = "feed",
            Name = "Άλλη ζωοτροφή",
            IsActive = true,
            SortOrder = 9
        }, CancellationToken.None));
        Assert.Equal(StatusCodes.Status409Conflict, duplicate.StatusCode);

        Assert.True(await categories.DeleteAsync(created.Id, CancellationToken.None));
        await expenses.CreateAsync(new ExpenseRequest
        {
            Amount = 12,
            Date = new DateOnly(2026, 11, 2),
            CategoryId = 1,
            Supplier = "  Μύλος  "
        }, CancellationToken.None);
        var blocked = await Assert.ThrowsAsync<LedgerException>(() => categories.DeleteAsync(1, CancellationToken.None));
        Assert.Equal(StatusCodes.Status409Conflict, blocked.StatusCode);

        var stored = await expenses.GetAllAsync(2026, 11, null, CancellationToken.None);
        Assert.Equal("Μύλος", stored[0].Supplier);
    }
}

internal sealed class LedgerDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public LedgerDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AgriLedgerDbContext>()
            .UseSqlite(_connection)
            .Options;
        Db = new AgriLedgerDbContext(options);
        Db.Database.EnsureCreated();
    }

    public AgriLedgerDbContext Db { get; }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
