using AgriLedger.Api.DTOs.Expenses;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenses;

    public ExpensesController(IExpenseService expenses)
    {
        _expenses = expenses;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExpenseResponse>>> GetAll(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] int? categoryId,
        CancellationToken cancellationToken)
    {
        var items = await _expenses.GetAllAsync(year, month, categoryId, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _expenses.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create(ExpenseRequest request, CancellationToken cancellationToken)
    {
        var created = await _expenses.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExpenseResponse>> Update(
        int id,
        ExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _expenses.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _expenses.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
