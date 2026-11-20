using AgriLedger.Api.Data;
using AgriLedger.Api.DTOs.Sales;
using AgriLedger.Api.Enums;
using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Services;

public interface ISaleService
{
    Task<IReadOnlyList<SaleResponse>> GetAllAsync(
        int? year,
        int? month,
        SaleType? saleType,
        PaymentStatus? paymentStatus,
        CancellationToken cancellationToken);

    Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<SaleResponse> CreateAsync(SaleRequest request, CancellationToken cancellationToken);

    Task<SaleResponse?> UpdateAsync(int id, SaleRequest request, CancellationToken cancellationToken);

    Task<SaleResponse?> UpdatePaymentStatusAsync(int id, PaymentStatus paymentStatus, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class SaleService : ISaleService
{
    private readonly AgriLedgerDbContext _db;

    public SaleService(AgriLedgerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SaleResponse>> GetAllAsync(
        int? year,
        int? month,
        SaleType? saleType,
        PaymentStatus? paymentStatus,
        CancellationToken cancellationToken)
    {
        var (start, end) = MonthRange.Resolve(year, month);
        var query = _db.Sales.AsNoTracking();
        if (start is DateOnly from && end is DateOnly to)
        {
            query = query.Where(sale => sale.Date >= from && sale.Date < to);
        }

        if (saleType is SaleType selectedType)
        {
            EntryRules.EnsureDefined(selectedType, "Ο τύπος πώλησης δεν είναι έγκυρος.");
            query = query.Where(sale => sale.SaleType == selectedType);
        }

        if (paymentStatus is PaymentStatus selectedStatus)
        {
            EntryRules.EnsureDefined(selectedStatus, "Η κατάσταση πληρωμής δεν είναι έγκυρη.");
            query = query.Where(sale => sale.PaymentStatus == selectedStatus);
        }

        return await query
            .OrderByDescending(sale => sale.Date)
            .ThenByDescending(sale => sale.Id)
            .Select(sale => new SaleResponse
            {
                Id = sale.Id,
                Date = sale.Date,
                CustomerName = sale.CustomerName,
                SaleType = sale.SaleType,
                EggSize = sale.EggSize,
                CaseQuantity = sale.CaseQuantity,
                PricePerCase = sale.PricePerCase,
                TotalAmount = sale.TotalAmount,
                PaymentStatus = sale.PaymentStatus,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Sales
            .AsNoTracking()
            .Where(sale => sale.Id == id)
            .Select(sale => new SaleResponse
            {
                Id = sale.Id,
                Date = sale.Date,
                CustomerName = sale.CustomerName,
                SaleType = sale.SaleType,
                EggSize = sale.EggSize,
                CaseQuantity = sale.CaseQuantity,
                PricePerCase = sale.PricePerCase,
                TotalAmount = sale.TotalAmount,
                PaymentStatus = sale.PaymentStatus,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SaleResponse> CreateAsync(SaleRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var now = DateTime.UtcNow;
        var sale = new Sale
        {
            CreatedAt = now,
            UpdatedAt = now
        };
        Apply(sale, request);
        _db.Sales.Add(sale);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(sale.Id, cancellationToken))!;
    }

    public async Task<SaleResponse?> UpdateAsync(int id, SaleRequest request, CancellationToken cancellationToken)
    {
        var sale = await _db.Sales.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (sale is null)
        {
            return null;
        }

        Validate(request);
        Apply(sale, request);
        sale.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<SaleResponse?> UpdatePaymentStatusAsync(
        int id,
        PaymentStatus paymentStatus,
        CancellationToken cancellationToken)
    {
        EntryRules.EnsureDefined(paymentStatus, "Η κατάσταση πληρωμής δεν είναι έγκυρη.");
        var sale = await _db.Sales.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (sale is null)
        {
            return null;
        }

        sale.PaymentStatus = paymentStatus;
        sale.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var sale = await _db.Sales.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (sale is null)
        {
            return false;
        }

        _db.Sales.Remove(sale);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(SaleRequest request)
    {
        EntryRules.EnsureDate(request.Date);
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.CustomerName.Trim().Length > 200)
        {
            throw new LedgerException(
                StatusCodes.Status400BadRequest,
                "Το όνομα πελάτη είναι υποχρεωτικό και μπορεί να έχει έως 200 χαρακτήρες.");
        }

        EntryRules.EnsureDefined(request.SaleType, "Ο τύπος πώλησης δεν είναι έγκυρος.");
        EntryRules.EnsureDefined(request.EggSize, "Το μέγεθος αυγού δεν είναι έγκυρο.");
        if (request.CaseQuantity <= 0)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η ποσότητα θηκών πρέπει να είναι τουλάχιστον 1.");
        }

        if (request.PricePerCase < 0)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η τιμή ανά θήκη δεν μπορεί να είναι αρνητική.");
        }
    }

    private static void Apply(Sale sale, SaleRequest request)
    {
        sale.Date = request.Date;
        sale.CustomerName = request.CustomerName.Trim();
        sale.SaleType = request.SaleType;
        sale.EggSize = request.EggSize;
        sale.CaseQuantity = request.CaseQuantity;
        sale.PricePerCase = decimal.Round(request.PricePerCase, 2, MidpointRounding.AwayFromZero);
        sale.TotalAmount = decimal.Round(sale.CaseQuantity * sale.PricePerCase, 2, MidpointRounding.AwayFromZero);
        sale.PaymentStatus = request.PaymentStatus;
    }
}
