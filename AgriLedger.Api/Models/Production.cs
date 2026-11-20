namespace AgriLedger.Api.Models;

public class Production
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int CaseQuantity { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
