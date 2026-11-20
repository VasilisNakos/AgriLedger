using AgriLedger.Api.DTOs.Categories;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriLedger.Api.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryService _categories;

    public IndexModel(ICategoryService categories)
    {
        _categories = categories;
    }

    public IReadOnlyList<CategoryResponse> Items { get; private set; } = [];

    public string? FormError { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    public CategoryRequest Input { get; set; } = new() { IsActive = true };

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var editing = EditId is int id ? Items.FirstOrDefault(item => item.Id == id) : null;
        if (editing is not null)
        {
            Input = new CategoryRequest
            {
                Code = editing.Code,
                Name = editing.Name,
                IsActive = editing.IsActive,
                SortOrder = editing.SortOrder
            };
        }
        else
        {
            EditId = null;
            Input.IsActive = true;
            if (Input.SortOrder == 0)
            {
                Input.SortOrder = Items.Count == 0 ? 1 : Items.Max(item => item.SortOrder) + 1;
            }
        }
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        try
        {
            await _categories.CreateAsync(Input, cancellationToken);
        }
        catch (LedgerException exception)
        {
            FormError = exception.Message;
            await LoadAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        try
        {
            if (EditId is not int id)
            {
                throw new LedgerException(StatusCodes.Status400BadRequest, "Δεν επιλέχθηκε κατηγορία.");
            }

            var updated = await _categories.UpdateAsync(id, Input, cancellationToken);
            if (updated is null)
            {
                throw new LedgerException(StatusCodes.Status404NotFound, "Η κατηγορία δεν βρέθηκε.");
            }
        }
        catch (LedgerException exception)
        {
            FormError = exception.Message;
            await LoadAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _categories.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                FormError = "Η κατηγορία δεν βρέθηκε.";
                await LoadAsync(cancellationToken);
                return Page();
            }
        }
        catch (LedgerException exception)
        {
            FormError = exception.Message;
            await LoadAsync(cancellationToken);
            return Page();
        }

        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Items = await _categories.GetAllAsync(includeInactive: true, cancellationToken);
    }
}
