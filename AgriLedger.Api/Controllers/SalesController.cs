using AgriLedger.Api.DTOs.Sales;
using AgriLedger.Api.Enums;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _sales;

    public SalesController(ISaleService sales)
    {
        _sales = sales;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SaleResponse>>> GetAll(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] SaleType? saleType,
        [FromQuery] PaymentStatus? paymentStatus,
        CancellationToken cancellationToken)
    {
        var items = await _sales.GetAllAsync(year, month, saleType, paymentStatus, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SaleResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _sales.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> Create(SaleRequest request, CancellationToken cancellationToken)
    {
        var created = await _sales.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SaleResponse>> Update(
        int id,
        SaleRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sales.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id:int}/payment-status")]
    public async Task<ActionResult<SaleResponse>> UpdatePaymentStatus(
        int id,
        PaymentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _sales.UpdatePaymentStatusAsync(id, request.PaymentStatus!.Value, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sales.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
