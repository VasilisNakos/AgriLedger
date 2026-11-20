using AgriLedger.Api.DTOs.Production;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Pages.Production;

public class IndexModel : MonthlyPageModel
{
    private readonly IProductionService _production;

    public IndexModel(IProductionService production)
    {
        _production = production;
    }

    public IReadOnlyList<ProductionResponse> Items { get; private set; } = [];

    public int MonthCases => Items.Sum(item => item.CaseQuantity);

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    public ProductionRequest Input { get; set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await LoadAsync(cancellationToken);
        var editing = EditId is int id ? Items.FirstOrDefault(item => item.Id == id) : null;
        if (editing is not null)
        {
            Input = new ProductionRequest
            {
                Date = editing.Date,
                CaseQuantity = editing.CaseQuantity,
                Notes = editing.Notes
            };
        }
        else
        {
            EditId = null;
            Input.Date = DefaultEntryDate();

            if (Input.CaseQuantity == 0)
            {
                Input.CaseQuantity = 1;
            }
        }
    }

    public Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        return SaveAsync(
            () => _production.CreateAsync(Input, cancellationToken),
            LoadAsync,
            cancellationToken);
    }

    public Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        return SaveAsync(async () =>
        {
            if (EditId is not int id)
            {
                throw new LedgerException(StatusCodes.Status400BadRequest, "Δεν επιλέχθηκε παραγωγή για διόρθωση.");
            }

            var updated = await _production.UpdateAsync(id, Input, cancellationToken);
            if (updated is null)
            {
                throw new LedgerException(StatusCodes.Status404NotFound, "Η καταχώρηση παραγωγής δεν βρέθηκε.");
            }
        }, LoadAsync, cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await _production.DeleteAsync(id, cancellationToken);
        return RedirectToPage(new { year = Year, month = Month });
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Items = await _production.GetAllAsync(Year, Month, cancellationToken);
    }
}
