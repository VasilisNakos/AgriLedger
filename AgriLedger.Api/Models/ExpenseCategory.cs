namespace AgriLedger.Api.Models;

public class ExpenseCategory
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
