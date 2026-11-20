namespace AgriLedger.Api.DTOs.Production;

public class ProductionResponse
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int CaseQuantity { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
