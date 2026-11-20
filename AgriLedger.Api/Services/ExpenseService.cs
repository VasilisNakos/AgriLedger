using AgriLedger.Api.Data;
using AgriLedger.Api.DTOs.Expenses;
using AgriLedger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriLedger.Api.Services;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(
        int? year,
        int? month,
        int? categoryId,
        CancellationToken cancellationToken);

    Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ExpenseResponse> CreateAsync(ExpenseRequest request, CancellationToken cancellationToken);

    Task<ExpenseResponse?> UpdateAsync(int id, ExpenseRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class ExpenseService : IExpenseService
{
    private readonly AgriLedgerDbContext _db;

    public ExpenseService(AgriLedgerDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(
        int? year,
        int? month,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var (start, end) = MonthRange.Resolve(year, month);
        var query = _db.Expenses.AsNoTracking();
        if (start is DateOnly from && end is DateOnly to)
        {
            query = query.Where(expense => expense.Date >= from && expense.Date < to);
        }

        if (categoryId is int selectedCategory)
        {
            query = query.Where(expense => expense.CategoryId == selectedCategory);
        }

        return await query
            .OrderByDescending(expense => expense.Date)
            .ThenByDescending(expense => expense.Id)
            .Select(expense => new ExpenseResponse
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryCode = expense.Category.Code,
                CategoryName = expense.Category.Name,
                Supplier = expense.Supplier,
                Description = expense.Description,
                DocumentNumber = expense.DocumentNumber,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public Task<ExpenseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Expenses
            .AsNoTracking()
            .Where(expense => expense.Id == id)
            .Select(expense => new ExpenseResponse
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                CategoryCode = expense.Category.Code,
                CategoryName = expense.Category.Name,
                Supplier = expense.Supplier,
                Description = expense.Description,
                DocumentNumber = expense.DocumentNumber,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ExpenseResponse> CreateAsync(ExpenseRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureCategoryAllowedAsync(request.CategoryId, null, cancellationToken);
        var now = DateTime.UtcNow;
        var expense = new Expense
        {
            CreatedAt = now,
            UpdatedAt = now
        };
        Apply(expense, request);
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(expense.Id, cancellationToken))!;
    }

    public async Task<ExpenseResponse?> UpdateAsync(int id, ExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (expense is null)
        {
            return null;
        }

        Validate(request);
        await EnsureCategoryAllowedAsync(request.CategoryId, expense.CategoryId, cancellationToken);
        Apply(expense, request);
        expense.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var expense = await _db.Expenses.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (expense is null)
        {
            return false;
        }

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureCategoryAllowedAsync(
        int categoryId,
        int? currentCategoryId,
        CancellationToken cancellationToken)
    {
        var category = await _db.ExpenseCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η κατηγορία δεν υπάρχει.");
        }

        if (!category.IsActive && categoryId != currentCategoryId)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Η κατηγορία είναι ανενεργή.");
        }
    }

    private static void Validate(ExpenseRequest request)
    {
        if (request.Amount <= 0)
        {
            throw new LedgerException(StatusCodes.Status400BadRequest, "Το ποσό πρέπει να είναι μεγαλύτερο του μηδενός.");
        }

        EntryRules.EnsureDate(request.Date);
        EntryRules.EnsureOptionalLength(request.Supplier, 200, "Ο προμηθευτής μπορεί να έχει έως 200 χαρακτήρες.");
        EntryRules.EnsureOptionalLength(request.Description, 500, "Η περιγραφή μπορεί να έχει έως 500 χαρακτήρες.");
        EntryRules.EnsureOptionalLength(request.DocumentNumber, 100, "Ο αριθμός παραστατικού μπορεί να έχει έως 100 χαρακτήρες.");
    }

    private static void Apply(Expense expense, ExpenseRequest request)
    {
        expense.Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero);
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;
        expense.Supplier = EntryRules.Clean(request.Supplier);
        expense.Description = EntryRules.Clean(request.Description);
        expense.DocumentNumber = EntryRules.Clean(request.DocumentNumber);
    }
}
