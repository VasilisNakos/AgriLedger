using AgriLedger.Api.Data;
using AgriLedger.Api.DTOs.Production;
using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Services;

public interface IProductionService
{
    Task<IReadOnlyList<ProductionResponse>> GetAllAsync(int? year, int? month, CancellationToken cancellationToken);

    Task<ProductionResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ProductionResponse> CreateAsync(ProductionRequest request, CancellationToken cancellationToken);

    Task<ProductionResponse?> UpdateAsync(int id, ProductionRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class ProductionService : IProductionService
{
    private readonly AgriLedgerDbContext _db;

    public ProductionService(AgriLedgerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductionResponse>> GetAllAsync(
        int? year,
        int? month,
        CancellationToken cancellationToken)
    {
        var (start, end) = MonthRange.Resolve(year, month);
        var query = _db.Productions.AsNoTracking();
        if (start is DateOnly from && end is DateOnly to)
        {
            query = query.Where(production => production.Date >= from && production.Date < to);
        }

        return await query
            .OrderByDescending(production => production.Date)
            .ThenByDescending(production => production.Id)
            .Select(production => new ProductionResponse
            {
                Id = production.Id,
                Date = production.Date,
                CaseQuantity = production.CaseQuantity,
                Notes = production.Notes,
                CreatedAt = production.CreatedAt,
                UpdatedAt = production.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public Task<ProductionResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Productions
            .AsNoTracking()
            .Where(production => production.Id == id)
            .Select(production => new ProductionResponse
            {
                Id = production.Id,
                Date = production.Date,
                CaseQuantity = production.CaseQuantity,
                Notes = production.Notes,
                CreatedAt = production.CreatedAt,
                UpdatedAt = production.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductionResponse> CreateAsync(ProductionRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var now = DateTime.UtcNow;
        var production = new Production
        {
            CreatedAt = now,
            UpdatedAt = now
        };
        Apply(production, request);
        _db.Productions.Add(production);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(production.Id, cancellationToken))!;
    }

    public async Task<ProductionResponse?> UpdateAsync(
        int id,
        ProductionRequest request,
        CancellationToken cancellationToken)
    {
        var production = await _db.Productions.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (production is null)
        {
            return null;
        }

        Validate(request);
        Apply(production, request);
        production.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var production = await _db.Productions.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (production is null)
        {
            return false;
        }

        _db.Productions.Remove(production);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(ProductionRequest request)
    {
        EntryRules.EnsureDate(request.Date);
        if (request.CaseQuantity <= 0)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η ποσότητα θηκών πρέπει να είναι τουλάχιστον 1.");
        }

        EntryRules.EnsureOptionalLength(request.Notes, 500, "Οι σημειώσεις μπορούν να έχουν έως 500 χαρακτήρες.");
    }

    private static void Apply(Production production, ProductionRequest request)
    {
        production.Date = request.Date;
        production.CaseQuantity = request.CaseQuantity;
        production.Notes = EntryRules.Clean(request.Notes);
    }
}
