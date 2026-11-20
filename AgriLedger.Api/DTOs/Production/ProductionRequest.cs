using System.ComponentModel.DataAnnotations;

namespace AgriLedger.Api.DTOs.Production;

public class ProductionRequest
{
    public DateOnly Date { get; set; }

    [Range(1, 1000000, ErrorMessage = "Η ποσότητα θηκών πρέπει να είναι τουλάχιστον 1.")]
    public int CaseQuantity { get; set; }

    [MaxLength(500, ErrorMessage = "Οι σημειώσεις μπορούν να έχουν έως 500 χαρακτήρες.")]
    public string? Notes { get; set; }
}
