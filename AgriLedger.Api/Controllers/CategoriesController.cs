using AgriLedger.Api.DTOs.Categories;
using AgriLedger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var items = await _categories.GetAllAsync(includeInactive, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _categories.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(
        CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _categories.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(
        int id,
        CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _categories.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _categories.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
