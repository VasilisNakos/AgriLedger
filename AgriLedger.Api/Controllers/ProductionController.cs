using AgriLedger.Api.DTOs.Production;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Controllers;

[ApiController]
[Route("api/production")]
public class ProductionController : ControllerBase
{
    private readonly IProductionService _production;

    public ProductionController(IProductionService production)
    {
        _production = production;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductionResponse>>> GetAll(
        [FromQuery] int? year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var items = await _production.GetAllAsync(year, month, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductionResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _production.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ProductionResponse>> Create(
        ProductionRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _production.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductionResponse>> Update(
        int id,
        ProductionRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _production.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _production.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
