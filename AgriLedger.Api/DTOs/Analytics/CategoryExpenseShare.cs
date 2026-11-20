namespace AgriLedger.Api.DTOs.Analytics;

public class CategoryExpenseShare
{
    public int CategoryId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal PercentOfExpenses { get; set; }
}
