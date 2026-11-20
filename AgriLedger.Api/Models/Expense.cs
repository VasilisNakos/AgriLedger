namespace AgriLedger.Api.Models;

public class Expense
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public int CategoryId { get; set; }

    public ExpenseCategory Category { get; set; } = null!;

    public string? Supplier { get; set; }

    public string? Description { get; set; }

    public string? DocumentNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
