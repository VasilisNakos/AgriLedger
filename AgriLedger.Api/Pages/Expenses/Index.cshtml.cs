using AgriLedger.Api.DTOs.Categories;
using AgriLedger.Api.DTOs.Expenses;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Pages.Expenses;

public class IndexModel : MonthlyPageModel
{
    private readonly IExpenseService _expenses;
    private readonly ICategoryService _categories;

    public IndexModel(IExpenseService expenses, ICategoryService categories)
    {
        _expenses = expenses;
        _categories = categories;
    }

    public IReadOnlyList<ExpenseResponse> Items { get; private set; } = [];

    public IReadOnlyList<CategoryResponse> Categories { get; private set; } = [];

    public decimal MonthTotal => Items.Sum(item => item.Amount);

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    public ExpenseRequest Input { get; set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await LoadAsync(cancellationToken);
        var editing = EditId is int id ? Items.FirstOrDefault(item => item.Id == id) : null;
        if (editing is not null)
        {
            Input = new ExpenseRequest
            {
                Amount = editing.Amount,
                Date = editing.Date,
                CategoryId = editing.CategoryId,
                Supplier = editing.Supplier,
                Description = editing.Description,
                DocumentNumber = editing.DocumentNumber
            };
        }
        else
        {
            EditId = null;
            Input.Date = DefaultEntryDate();
        }
    }

    public Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        return SaveAsync(
            () => _expenses.CreateAsync(Input, cancellationToken),
            LoadAsync,
            cancellationToken);
    }

    public Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        return SaveAsync(async () =>
        {
            if (EditId is not int id)
            {
                throw new LedgerException(StatusCodes.Status400BadRequest, "Δεν επιλέχθηκε έξοδο για διόρθωση.");
            }

            var updated = await _expenses.UpdateAsync(id, Input, cancellationToken);
            if (updated is null)
            {
                throw new LedgerException(StatusCodes.Status404NotFound, "Το έξοδο δεν βρέθηκε.");
            }
        }, LoadAsync, cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await _expenses.DeleteAsync(id, cancellationToken);
        return RedirectToPage(new { year = Year, month = Month });
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Items = await _expenses.GetAllAsync(Year, Month, null, cancellationToken);
        Categories = await _categories.GetAllAsync(includeInactive: true, cancellationToken);
    }
}
