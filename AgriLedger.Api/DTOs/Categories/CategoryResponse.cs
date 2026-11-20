namespace AgriLedger.Api.DTOs.Categories;

public class CategoryResponse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
