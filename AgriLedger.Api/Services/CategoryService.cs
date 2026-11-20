using System.Linq.Expressions;
using AgriLedger.Api.Data;
using AgriLedger.Api.DTOs.Categories;
using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);

    Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken cancellationToken);

    Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class CategoryService : ICategoryService
{
    private static readonly Expression<Func<ExpenseCategory, CategoryResponse>> ToResponse = category =>
        new CategoryResponse
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder,
            CreatedAt = category.CreatedAt
        };

    private readonly AgriLedgerDbContext _db;

    public CategoryService(AgriLedgerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = _db.ExpenseCategories.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(category => category.IsActive);
        }

        return await query
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(ToResponse)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.ExpenseCategories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken cancellationToken)
    {
        var (code, name) = Normalize(request);
        await EnsureCodeAvailableAsync(code, null, cancellationToken);

        var category = new ExpenseCategory
        {
            Code = code,
            Name = name,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow
        };

        _db.ExpenseCategories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(category.Id, cancellationToken))!;
    }

    public async Task<CategoryResponse?> UpdateAsync(
        int id,
        CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var (code, name) = Normalize(request);
        await EnsureCodeAvailableAsync(code, id, cancellationToken);
        category.Code = code;
        category.Name = name;
        category.IsActive = request.IsActive;
        category.SortOrder = request.SortOrder;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _db.ExpenseCategories.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return false;
        }

        var used = await _db.Expenses.AnyAsync(expense => expense.CategoryId == id, cancellationToken);
        if (used)
        {
            throw new LedgerException(
                StatusCodes.Status409Conflict,
                "Η κατηγορία χρησιμοποιείται σε έξοδα και δεν διαγράφεται. Απενεργοποίησέ την.");
        }

        _db.ExpenseCategories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureCodeAvailableAsync(string code, int? currentId, CancellationToken cancellationToken)
    {
        var taken = await _db.ExpenseCategories.AnyAsync(
            category => category.Code == code && category.Id != currentId,
            cancellationToken);
        if (taken)
        {
            throw new LedgerException(StatusCodes.Status409Conflict, "Υπάρχει ήδη κατηγορία με αυτόν τον κωδικό.");
        }
    }

    private static (string Code, string Name) Normalize(CategoryRequest request)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var name = request.Name.Trim();
        if (code.Length is < 2 or > 32 || !code.All(character => char.IsAsciiLetterOrDigit(character) || character == '_'))
        {
            throw new LedgerException(
                StatusCodes.Status400BadRequest,
                "Ο κωδικός πρέπει να έχει 2 έως 32 λατινικούς χαρακτήρες, αριθμούς ή κάτω παύλα.");
        }

        if (name.Length is < 1 or > 100)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Το όνομα της κατηγορίας είναι υποχρεωτικό.");
        }

        return (code, name);
    }
}
