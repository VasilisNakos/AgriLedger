using AgriLedger.Api.DTOs.Sales;
using AgriLedger.Api.Enums;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Pages.Sales;

public class IndexModel : MonthlyPageModel
{
    private readonly ISaleService _sales;

    public IndexModel(ISaleService sales)
    {
        _sales = sales;
    }

    public IReadOnlyList<SaleResponse> Items { get; private set; } = [];

    public decimal MonthTotal => Items.Sum(item => item.TotalAmount);

    public int MonthCases => Items.Sum(item => item.CaseQuantity);

    [BindProperty(SupportsGet = true)]
    public int? EditId { get; set; }

    [BindProperty]
    public SaleRequest Input { get; set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await LoadAsync(cancellationToken);
        var editing = EditId is int id ? Items.FirstOrDefault(item => item.Id == id) : null;
        if (editing is not null)
        {
            Input = new SaleRequest
            {
                Date = editing.Date,
                CustomerName = editing.CustomerName,
                SaleType = editing.SaleType,
                EggSize = editing.EggSize,
                CaseQuantity = editing.CaseQuantity,
                PricePerCase = editing.PricePerCase,
                PaymentStatus = editing.PaymentStatus
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
            () => _sales.CreateAsync(Input, cancellationToken),
            LoadAsync,
            cancellationToken);
    }

    public Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        return SaveAsync(async () =>
        {
            if (EditId is not int id)
            {
                throw new LedgerException(StatusCodes.Status400BadRequest, "Δεν επιλέχθηκε πώληση για διόρθωση.");
            }

            var updated = await _sales.UpdateAsync(id, Input, cancellationToken);
            if (updated is null)
            {
                throw new LedgerException(StatusCodes.Status404NotFound, "Η πώληση δεν βρέθηκε.");
            }
        }, LoadAsync, cancellationToken);
    }

    public async Task<IActionResult> OnPostPaymentAsync(int id, PaymentStatus status, CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await _sales.UpdatePaymentStatusAsync(id, status, cancellationToken);
        return RedirectToPage(new { year = Year, month = Month });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        NormalizePeriod();
        await _sales.DeleteAsync(id, cancellationToken);
        return RedirectToPage(new { year = Year, month = Month });
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Items = await _sales.GetAllAsync(Year, Month, null, null, cancellationToken);
    }
}
