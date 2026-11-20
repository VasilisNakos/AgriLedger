using AgriLedger.Api.Enums;

namespace AgriLedger.Api.Models;

public class Sale
{
    public const int EggsPerCase = 30;

    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public SaleType SaleType { get; set; }

    public EggSize EggSize { get; set; }

    public int CaseQuantity { get; set; }

    public decimal PricePerCase { get; set; }

    public decimal TotalAmount { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
