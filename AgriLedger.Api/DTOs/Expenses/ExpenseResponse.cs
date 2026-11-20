namespace AgriLedger.Api.DTOs.Expenses;

public class ExpenseResponse
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public int CategoryId { get; set; }

    public string CategoryCode { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? Supplier { get; set; }

    public string? Description { get; set; }

    public string? DocumentNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
