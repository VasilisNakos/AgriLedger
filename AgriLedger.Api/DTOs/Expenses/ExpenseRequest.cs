using System.ComponentModel.DataAnnotations;

namespace AgriLedger.Api.DTOs.Expenses;

public class ExpenseRequest
{
    [Range(0.01, 999999999, ErrorMessage = "Το ποσό πρέπει να είναι μεγαλύτερο του μηδενός.")]
    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Επίλεξε κατηγορία.")]
    public int CategoryId { get; set; }

    [MaxLength(200, ErrorMessage = "Ο προμηθευτής μπορεί να έχει έως 200 χαρακτήρες.")]
    public string? Supplier { get; set; }

    [MaxLength(500, ErrorMessage = "Η περιγραφή μπορεί να έχει έως 500 χαρακτήρες.")]
    public string? Description { get; set; }

    [MaxLength(100, ErrorMessage = "Ο αριθμός παραστατικού μπορεί να έχει έως 100 χαρακτήρες.")]
    public string? DocumentNumber { get; set; }
}
